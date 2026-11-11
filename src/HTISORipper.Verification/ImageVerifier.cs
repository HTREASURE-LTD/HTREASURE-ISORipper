using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using HTISORipper.Core;
using HTISORipper.Logging;

namespace HTISORipper.Verification
{
    /// <summary>
    /// Verifies a finished image. Three clearly separated levels:
    ///  1. Size check - the file has exactly the expected number of bytes.
    ///  2. Structural - the file is re-read from disk and re-hashed (must equal the SHA-256 computed while writing),
    ///     and ISO 9660 / UDF descriptors are validated.
    ///  3. Source compare - every sector is re-read from the disc and compared byte-for-byte with the file.
    /// Only level 3 may be presented as "verified against the disc".
    /// </summary>
    public sealed class ImageVerifier
    {
        private const int MaxReportedDifferences = 1000;
        private readonly ILogger _log;

        public ImageVerifier(ILogger log)
        {
            _log = log ?? NullLogger.Instance;
            BlockSectors = 32;
            SourceRetries = 2;
        }

        public int BlockSectors { get; set; }
        public int SourceRetries { get; set; }

        public VerificationReport Verify(string imagePath, long expectedBytes, string expectedSha256, VerificationMode mode,
            ISectorSource source, IProgress<ImagingProgress> progress, CancellationToken token)
        {
            var report = new VerificationReport();
            if (mode == VerificationMode.FullSourceCompare && source == null)
                throw new ArgumentException("A sector source is required for source comparison.", "source");

            var info = new FileInfo(imagePath);
            report.SizeOk = info.Exists && info.Length == expectedBytes;
            if (!report.SizeOk)
            {
                report.Notes.Add(string.Format(CultureInfo.InvariantCulture, "Size mismatch: expected {0} bytes, file has {1}.",
                    expectedBytes, info.Exists ? info.Length : -1));
                report.Level = VerificationLevel.Failed;
                _log.Error("Verification: " + report.Notes[report.Notes.Count - 1]);
                return report;
            }
            if (mode == VerificationMode.None)
            {
                report.Level = VerificationLevel.NotVerified;
                report.Notes.Add("Verification was switched off; only the file size was checked.");
                return report;
            }

            // ---- structure
            FilesystemInfo fs = ProbeFile(imagePath);
            if (!fs.AnyRecognised)
            {
                report.StructureOk = null;
                report.StructureSummary = "No ISO 9660 or UDF structures found (nothing to validate)";
            }
            else
            {
                bool ok = true;
                if (fs.HasIso9660 && fs.Iso9660VolumeBytes > info.Length)
                {
                    ok = false;
                    report.Notes.Add(string.Format(CultureInfo.InvariantCulture,
                        "ISO 9660 volume declares {0} bytes but the image holds only {1}.", fs.Iso9660VolumeBytes, info.Length));
                }
                report.StructureOk = ok;
                report.StructureSummary = fs.Describe();
            }

            // ---- re-hash (and optional source compare in the same pass)
            int sectorSize = source != null ? source.SectorSize : 2048;
            bool compare = mode == VerificationMode.FullSourceCompare;
            report.SourceCompared = compare;
            string actualSha;
            bool completed;
            try
            {
                completed = HashAndCompare(imagePath, sectorSize, compare ? source : null, report, progress, token, out actualSha);
            }
            catch (VerificationAbortedException ex)
            {
                report.SourceMatches = false;
                report.Level = VerificationLevel.Failed;
                _log.Error("Verification aborted: " + ex.Message);
                return report;
            }
            if (!completed)
            {
                report.Cancelled = true;
                report.Level = VerificationLevel.NotVerified;
                report.Notes.Add("Verification was cancelled before completion.");
                _log.Warn("Verification cancelled.");
                return report;
            }

            report.FileHashMatches = string.Equals(actualSha, expectedSha256, StringComparison.OrdinalIgnoreCase);
            if (report.FileHashMatches == false)
                report.Notes.Add("The file read back from disk does not match the data that was written (SHA-256 differs). The destination drive may be faulty.");

            if (compare)
            {
                report.SourceMatches = report.MismatchedLbas.Count == 0 && report.UnreadableLbas.Count == 0;
                if (report.MismatchedLbas.Count > 0) report.Notes.Add(report.MismatchedLbas.Count + " sector(s) differ between the disc and the image.");
                if (report.UnreadableLbas.Count > 0) report.Notes.Add(report.UnreadableLbas.Count + " sector(s) could not be re-read from the disc for comparison.");
            }

            bool failed = report.FileHashMatches == false || report.StructureOk == false || report.SourceMatches == false;
            if (failed) report.Level = VerificationLevel.Failed;
            else if (compare) report.Level = VerificationLevel.SourceVerified;
            else report.Level = VerificationLevel.StructuralOnly;

            _log.Info("Verification result: " + report.Level + "; structure: " + (report.StructureSummary ?? "-")
                      + "; notes: " + string.Join(" | ", report.Notes.ToArray()));
            return report;
        }

        public static FilesystemInfo ProbeFile(string imagePath)
        {
            using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return FilesystemProbe.Probe(lba =>
                {
                    long pos = lba * FilesystemProbe.SectorSize;
                    if (pos + FilesystemProbe.SectorSize > stream.Length) return null;
                    stream.Position = pos;
                    byte[] sector = new byte[FilesystemProbe.SectorSize];
                    int read = 0;
                    while (read < sector.Length)
                    {
                        int n = stream.Read(sector, read, sector.Length - read);
                        if (n == 0) return null;
                        read += n;
                    }
                    return sector;
                });
            }
        }

        /// <summary>Computes the SHA-256 of a file (lower-case hex). Returns null if cancelled.</summary>
        public static string ComputeSha256(string path, IProgress<double> progress, CancellationToken token)
        {
            using (SHA256 sha = Hashing.CreateSha256())
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 20, FileOptions.SequentialScan))
            {
                byte[] buffer = new byte[1 << 20];
                long done = 0;
                int n;
                while ((n = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (token.IsCancellationRequested) return null;
                    sha.TransformBlock(buffer, 0, n, null, 0);
                    done += n;
                    if (progress != null && stream.Length > 0) progress.Report((double)done / stream.Length);
                }
                sha.TransformFinalBlock(new byte[0], 0, 0);
                return Format.Hex(sha.Hash);
            }
        }

        private bool HashAndCompare(string imagePath, int sectorSize, ISectorSource source, VerificationReport report,
            IProgress<ImagingProgress> progress, CancellationToken token, out string sha256)
        {
            sha256 = null;
            var cancellable = source as ICancellableIo;
            CancellationTokenRegistration reg = cancellable != null ? token.Register(cancellable.CancelPendingIo) : default(CancellationTokenRegistration);
            try
            {
                using (SHA256 sha = Hashing.CreateSha256())
                using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 20, FileOptions.SequentialScan))
                {
                    long totalSectors = stream.Length / sectorSize;
                    int block = Math.Max(1, BlockSectors);
                    byte[] fileBuf = new byte[block * sectorSize];
                    byte[] discBuf = source != null ? new byte[block * sectorSize] : null;
                    long lba = 0;
                    DateTime lastReport = DateTime.MinValue;
                    DateTime start = DateTime.UtcNow;

                    while (lba < totalSectors)
                    {
                        if (token.IsCancellationRequested) return false;
                        int count = (int)Math.Min(block, totalSectors - lba);
                        int bytes = count * sectorSize;
                        ReadExactly(stream, fileBuf, bytes);
                        sha.TransformBlock(fileBuf, 0, bytes, null, 0);

                        if (source != null)
                        {
                            if (!CompareBlock(source, lba, count, fileBuf, discBuf, report, token)) return false;
                        }

                        lba += count;
                        if (progress != null && (DateTime.UtcNow - lastReport).TotalMilliseconds > 250)
                        {
                            lastReport = DateTime.UtcNow;
                            double secs = (lastReport - start).TotalSeconds;
                            double bps = secs > 0 ? lba * (double)sectorSize / secs : 0;
                            progress.Report(new ImagingProgress
                            {
                                Phase = source != null ? ImagingPhase.VerifyingSource : ImagingPhase.VerifyingFile,
                                SectorsDone = lba,
                                TotalSectors = totalSectors,
                                BytesPerSecond = bps,
                                EstimatedRemaining = bps > 0 ? TimeSpan.FromSeconds((totalSectors - lba) * sectorSize / bps) : (TimeSpan?)null,
                                Message = source != null ? "Comparing image with disc" : "Re-reading image file"
                            });
                        }
                    }

                    // Any trailing partial sector (should not happen for a valid image) is still hashed.
                    int tail = (int)(stream.Length - stream.Position);
                    if (tail > 0)
                    {
                        byte[] t = new byte[tail];
                        ReadExactly(stream, t, tail);
                        sha.TransformBlock(t, 0, tail, null, 0);
                        report.Notes.Add("Image size is not a multiple of the sector size.");
                    }
                    sha.TransformFinalBlock(new byte[0], 0, 0);
                    sha256 = Format.Hex(sha.Hash);
                    return true;
                }
            }
            finally
            {
                reg.Dispose();
            }
        }

        private bool CompareBlock(ISectorSource source, long lba, int count, byte[] fileBuf, byte[] discBuf,
            VerificationReport report, CancellationToken token)
        {
            int ss = source.SectorSize;
            SectorReadResult r = source.Read(lba, count, discBuf, 0);
            if (r.IsOk)
            {
                for (int i = 0; i < count; i++)
                    if (!SectorEquals(fileBuf, discBuf, i * ss, ss)) AddLimited(report.MismatchedLbas, lba + i);
                return true;
            }
            if (r.Status == SectorReadStatus.Cancelled) return false;
            if (r.Status != SectorReadStatus.MediumError)
            {
                _log.Error("Verification source read failed at LBA " + lba + ": " + r);
                for (int i = 0; i < count; i++) AddLimited(report.UnreadableLbas, lba + i);
                report.Notes.Add("The disc stopped responding during verification (" + r.Status + ").");
                throw new VerificationAbortedException(r.ToString());
            }

            for (int i = 0; i < count; i++)
            {
                SectorReadResult sr = default(SectorReadResult);
                for (int attempt = 0; attempt <= SourceRetries; attempt++)
                {
                    if (token.IsCancellationRequested) return false;
                    sr = source.Read(lba + i, 1, discBuf, i * ss);
                    if (sr.Status != SectorReadStatus.MediumError) break;
                }
                if (sr.Status == SectorReadStatus.Cancelled) return false;
                if (!sr.IsOk) { AddLimited(report.UnreadableLbas, lba + i); continue; }
                if (!SectorEquals(fileBuf, discBuf, i * ss, ss)) AddLimited(report.MismatchedLbas, lba + i);
            }
            return true;
        }

        private static void AddLimited(System.Collections.Generic.List<long> list, long lba)
        {
            if (list.Count < MaxReportedDifferences) list.Add(lba);
        }

        private static bool SectorEquals(byte[] a, byte[] b, int offset, int length)
        {
            for (int i = offset; i < offset + length; i++) if (a[i] != b[i]) return false;
            return true;
        }

        private static void ReadExactly(Stream stream, byte[] buffer, int count)
        {
            int read = 0;
            while (read < count)
            {
                int n = stream.Read(buffer, read, count - read);
                if (n == 0) throw new EndOfStreamException("Image file ended unexpectedly.");
                read += n;
            }
        }
    }

    /// <summary>Thrown when the source disc becomes unavailable during source comparison.</summary>
    public sealed class VerificationAbortedException : Exception
    {
        public VerificationAbortedException(string message) : base(message) { }
    }
}
