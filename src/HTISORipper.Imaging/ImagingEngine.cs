using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using HTISORipper.Core;
using HTISORipper.Logging;

namespace HTISORipper.Imaging
{
    /// <summary>
    /// Streams sectors from an <see cref="ISectorSource"/> into an image file.
    ///
    /// Guarantees:
    ///  * Sectors are written strictly in LBA order; the image is a sector-for-sector copy (LBA 0 .. TotalSectors-1).
    ///  * Data goes to "&lt;name&gt;.htpart" first and is renamed to the final name only after a complete, flushed write.
    ///  * Unreadable sectors are never hidden: with the default policy imaging stops; with the "continue" policy the
    ///    output is named "*.INCOMPLETE.iso", a bad-sector report is written, and the outcome is CompletedWithErrors.
    ///  * Memory use is bounded by one read block (BlockSectors * SectorSize).
    /// </summary>
    public sealed class ImagingEngine
    {
        private const long SpaceSafetyMarginBytes = 16L * 1024 * 1024;
        private readonly ILogger _log;

        public ImagingEngine(ILogger log)
        {
            _log = log ?? NullLogger.Instance;
            FreeSpaceProvider = PathUtil.GetAvailableFreeSpace;
            ProgressInterval = TimeSpan.FromMilliseconds(250);
        }

        /// <summary>Returns free bytes for a directory (-1 if unknown). Replaceable for tests.</summary>
        public Func<string, long> FreeSpaceProvider { get; set; }
        public TimeSpan ProgressInterval { get; set; }

        public ImagingResult Run(ISectorSource source, ImagingOptions options, IProgress<ImagingProgress> progress, CancellationToken token)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (options == null) throw new ArgumentNullException("options");

            var result = new ImagingResult();
            var clock = Stopwatch.StartNew();
            string tempPath = null;
            FileStream output = null;
            SHA256 sha = null;
            CancellationTokenRegistration registration = default(CancellationTokenRegistration);

            try
            {
                string problem = ValidateOptions(source, options);
                if (problem != null) return Fail(result, ImagingErrorKind.InvalidDestination, problem, clock);

                string finalPath = Path.GetFullPath(options.OutputPath);
                string directory = Path.GetDirectoryName(finalPath);
                long totalBytes = options.TotalSectors * options.SectorSize;

                _log.Info("=== Imaging started ===");
                _log.Info("Source: " + (options.SourceDescription ?? "(unspecified)"));
                _log.Info(string.Format(CultureInfo.InvariantCulture,
                    "Output: {0}; sectors {1} x {2} = {3} bytes; block {4}; retries {5}; policy {6}",
                    finalPath, options.TotalSectors, options.SectorSize, totalBytes, options.BlockSectors,
                    options.RetryCount, options.ErrorPolicy));

                try
                {
                    Directory.CreateDirectory(directory);
                }
                catch (UnauthorizedAccessException)
                {
                    return Fail(result, ImagingErrorKind.DestinationPermissionDenied, "You do not have permission to create or write to the folder \"" + directory + "\".", clock);
                }
                catch (IOException ex)
                {
                    return Fail(result, ImagingErrorKind.InvalidDestination, "The destination folder could not be created: " + ex.Message, clock);
                }

                if (File.Exists(finalPath) && !options.AllowOverwrite)
                    return Fail(result, ImagingErrorKind.OutputExists, "The file \"" + finalPath + "\" already exists.", clock);

                tempPath = PathUtil.GetTempPathFor(finalPath);
                if (File.Exists(tempPath))
                {
                    _log.Warn("Removing stale temporary file from an earlier interrupted run: " + tempPath);
                    File.Delete(tempPath);
                }

                long free = FreeSpaceProvider(directory);
                if (free >= 0 && free < totalBytes + SpaceSafetyMarginBytes)
                {
                    return Fail(result, ImagingErrorKind.InsufficientSpace, string.Format(CultureInfo.CurrentCulture,
                        "Not enough free space in \"{0}\". Required: {1}; available: {2}.",
                        directory, Format.Bytes(totalBytes + SpaceSafetyMarginBytes), Format.Bytes(free)), clock);
                }

                output = new FileStream(tempPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 1 << 20, FileOptions.None);
                // Reserve the space up front: a full destination is detected before reading starts, and fragmentation is reduced.
                output.SetLength(totalBytes);
                output.Position = 0;

                sha = Hashing.CreateSha256();
                var cancellable = source as ICancellableIo;
                if (cancellable != null) registration = token.Register(cancellable.CancelPendingIo);

                ImagingErrorKind failure;
                string failureMessage;
                long endSector;
                bool ok = CopySectors(source, options, output, sha, progress, token, result, clock, out endSector, out failure, out failureMessage);
                if (!ok)
                {
                    CloseAndDelete(ref output, tempPath);
                    if (failure == ImagingErrorKind.Cancelled)
                    {
                        result.Outcome = ImagingOutcome.Cancelled;
                        result.Error = ImagingErrorKind.Cancelled;
                        result.Message = "Imaging was cancelled. The partial file has been deleted.";
                        result.Elapsed = clock.Elapsed;
                        _log.Warn("Imaging cancelled by the user after " + result.SectorsWritten + " sectors.");
                        return result;
                    }
                    return Fail(result, failure, failureMessage + " The partial image has been deleted.", clock);
                }

                // ---- finalise
                Report(progress, new ImagingProgress { Phase = ImagingPhase.Finalising, SectorsDone = endSector, TotalSectors = options.TotalSectors, Message = "Finalising image" });
                if (endSector < options.TotalSectors) output.SetLength(endSector * options.SectorSize);
                sha.TransformFinalBlock(new byte[0], 0, 0);
                result.Sha256 = Format.Hex(sha.Hash);
                output.Flush(true);
                output.Dispose();
                output = null;

                result.SectorsWritten = endSector;
                result.ImageBytes = endSector * options.SectorSize;

                if (result.UnreadableLbas.Count > 0)
                {
                    string incompletePath = UniquePath(PathUtil.GetIncompletePathFor(finalPath));
                    MoveIntoPlace(tempPath, incompletePath, false);
                    tempPath = null;
                    result.OutputPath = incompletePath;
                    result.BadSectorReportPath = incompletePath + ".badsectors.txt";
                    WriteBadSectorReport(result.BadSectorReportPath, options, result);
                    result.Outcome = ImagingOutcome.CompletedWithErrors;
                    result.Error = ImagingErrorKind.UnreadableSectors;
                    result.Message = result.UnreadableLbas.Count + " sector(s) could not be read and were filled with zeros. "
                        + "The image is INCOMPLETE and must not be used as a backup.";
                    _log.Warn("Image saved as INCOMPLETE: " + incompletePath + " (" + result.UnreadableLbas.Count + " unreadable sectors)");
                }
                else
                {
                    if (File.Exists(finalPath) && !options.AllowOverwrite)
                        return FailKeepTemp(result, ImagingErrorKind.OutputExists, finalPath, tempPath, clock);
                    MoveIntoPlace(tempPath, finalPath, options.AllowOverwrite);
                    tempPath = null;
                    result.OutputPath = finalPath;
                    result.Outcome = ImagingOutcome.Success;
                    result.Error = ImagingErrorKind.None;
                    result.Message = "All sectors were read successfully.";
                }

                result.Elapsed = clock.Elapsed;
                result.AverageBytesPerSecond = result.Elapsed.TotalSeconds > 0 ? result.ImageBytes / result.Elapsed.TotalSeconds : 0;
                _log.Info(string.Format(CultureInfo.InvariantCulture,
                    "Imaging finished: {0}; {1} bytes in {2:0.0}s; SHA-256 {3}; recovered {4}; unreadable {5}; run-out excluded {6}",
                    result.Outcome, result.ImageBytes, result.Elapsed.TotalSeconds, result.Sha256,
                    result.RecoveredSectors, result.UnreadableLbas.Count, result.RunOutSectorsExcluded));
                return result;
            }
            catch (UnauthorizedAccessException ex)
            {
                _log.Error("Destination access denied", ex);
                CloseAndDelete(ref output, tempPath);
                return Fail(result, ImagingErrorKind.DestinationPermissionDenied, "Windows denied write access to the destination folder.", clock);
            }
            catch (IOException ex)
            {
                _log.Error("Destination write failure", ex);
                CloseAndDelete(ref output, tempPath);
                if (IsDiskFull(ex))
                    return Fail(result, ImagingErrorKind.InsufficientSpace, "The destination drive ran out of space. The partial image has been deleted.", clock);
                return Fail(result, ImagingErrorKind.WriteFailed, "Writing the image file failed: " + ex.Message, clock);
            }
            catch (Exception ex)
            {
                _log.Error("Unexpected imaging failure", ex);
                CloseAndDelete(ref output, tempPath);
                return Fail(result, ImagingErrorKind.Unexpected, "An unexpected error occurred. Details were written to the log.", clock);
            }
            finally
            {
                registration.Dispose();
                if (sha != null) sha.Dispose();
                if (output != null) output.Dispose();
            }
        }

        private bool CopySectors(ISectorSource source, ImagingOptions o, FileStream output, SHA256 sha,
            IProgress<ImagingProgress> progress, CancellationToken token, ImagingResult result, Stopwatch clock,
            out long endSector, out ImagingErrorKind failure, out string failureMessage)
        {
            int ss = o.SectorSize;
            byte[] buffer = new byte[o.BlockSectors * ss];
            var speed = new SpeedTracker();
            speed.Add(clock.Elapsed, 0);
            TimeSpan lastReport = clock.Elapsed;
            long lba = 0;
            failure = ImagingErrorKind.None;
            failureMessage = null;
            endSector = o.TotalSectors;

            while (lba < o.TotalSectors)
            {
                if (token.IsCancellationRequested) { failure = ImagingErrorKind.Cancelled; return false; }

                int count = (int)Math.Min(o.BlockSectors, o.TotalSectors - lba);
                SectorReadResult r = source.Read(lba, count, buffer, 0);

                if (!r.IsOk && r.Status == SectorReadStatus.MediumError)
                {
                    // Fall back to single-sector reads with bounded retries so one bad sector does not cost a whole block.
                    for (int i = 0; i < count; i++)
                    {
                        long sector = lba + i;
                        int attempts;
                        SectorReadResult sr = ReadWithRetries(source, sector, buffer, i * ss, o, token, out attempts);
                        if (sr.IsOk)
                        {
                            if (attempts > 1)
                            {
                                result.RecoveredSectors++;
                                _log.Warn("Sector " + sector + " recovered after " + attempts + " attempts.");
                            }
                            continue;
                        }
                        if (sr.Status != SectorReadStatus.MediumError)
                        {
                            r = sr;   // fatal status: handled below
                            break;
                        }

                        if (IsRunOut(o, sector))
                        {
                            // CD-R track-at-once discs end with up to 2 unreadable run-out blocks that are not
                            // part of the file system. End the image before them.
                            result.RunOutSectorsExcluded = (int)(o.TotalSectors - sector);
                            _log.Warn(string.Format(CultureInfo.InvariantCulture,
                                "Sector {0} unreadable but lies in the final {1} sector(s) beyond the file system end ({2}); treated as run-out. Image ends at sector {0}.",
                                sector, o.RunOutToleranceSectors, o.MinimumRequiredSectors));
                            Commit(output, sha, buffer, i * ss);
                            endSector = sector;
                            return true;
                        }

                        _log.Error("Sector " + sector + " unreadable after " + attempts + " attempts: " + sr);
                        if (o.ErrorPolicy == ErrorPolicy.AbortOnUnreadableSector)
                        {
                            result.UnreadableLbas.Add(sector);
                            failure = ImagingErrorKind.UnreadableSectors;
                            failureMessage = "Sector " + sector.ToString("N0", CultureInfo.CurrentCulture) + " could not be read after "
                                + attempts + " attempt(s). The disc may be scratched or damaged.";
                            return false;
                        }
                        result.UnreadableLbas.Add(sector);
                        Array.Clear(buffer, i * ss, ss);
                    }
                    if (r.Status == SectorReadStatus.MediumError) r = SectorReadResult.Success;   // block handled sector-by-sector
                }

                if (!r.IsOk)
                {
                    switch (r.Status)
                    {
                        case SectorReadStatus.Cancelled:
                            failure = ImagingErrorKind.Cancelled;
                            return false;
                        case SectorReadStatus.NotReady:
                            failure = ImagingErrorKind.DiscRemoved;
                            failureMessage = "The disc was removed or the drive stopped responding at sector "
                                + lba.ToString("N0", CultureInfo.CurrentCulture) + ".";
                            _log.Error("Drive not ready during imaging at LBA " + lba + ": " + r);
                            return false;
                        case SectorReadStatus.AccessDenied:
                            failure = ImagingErrorKind.DeviceAccessDenied;
                            failureMessage = "Windows denied access to the optical drive.";
                            _log.Error("Device access denied at LBA " + lba + ": " + r);
                            return false;
                        default:
                            failure = ImagingErrorKind.Unexpected;
                            failureMessage = "The drive reported an unexpected error at sector " + lba.ToString("N0", CultureInfo.CurrentCulture) + ".";
                            _log.Error("Unexpected read status at LBA " + lba + ": " + r);
                            return false;
                    }
                }

                output.Write(buffer, 0, count * ss);
                sha.TransformBlock(buffer, 0, count * ss, null, 0);
                lba += count;
                result.SectorsWritten = lba;

                if (clock.Elapsed - lastReport >= ProgressInterval || lba == o.TotalSectors)
                {
                    lastReport = clock.Elapsed;
                    speed.Add(clock.Elapsed, lba * ss);
                    double bps = speed.BytesPerSecond;
                    Report(progress, new ImagingProgress
                    {
                        Phase = ImagingPhase.Reading,
                        SectorsDone = lba,
                        TotalSectors = o.TotalSectors,
                        BytesPerSecond = bps,
                        EstimatedRemaining = bps > 0 ? TimeSpan.FromSeconds((o.TotalSectors - lba) * ss / bps) : (TimeSpan?)null,
                        UnreadableSectors = result.UnreadableLbas.Count,
                        RecoveredSectors = result.RecoveredSectors,
                        Message = "Reading sector " + lba.ToString("N0", CultureInfo.CurrentCulture) + " of " + o.TotalSectors.ToString("N0", CultureInfo.CurrentCulture)
                    });
                }
            }
            return true;
        }

        private static void Commit(FileStream output, SHA256 sha, byte[] buffer, int bytes)
        {
            if (bytes <= 0) return;
            output.Write(buffer, 0, bytes);
            sha.TransformBlock(buffer, 0, bytes, null, 0);
        }

        private static bool IsRunOut(ImagingOptions o, long sector)
        {
            return o.RunOutToleranceSectors > 0
                && o.MinimumRequiredSectors > 0
                && sector >= o.MinimumRequiredSectors
                && sector >= o.TotalSectors - o.RunOutToleranceSectors;
        }

        private static SectorReadResult ReadWithRetries(ISectorSource source, long sector, byte[] buffer, int offset,
            ImagingOptions o, CancellationToken token, out int attempts)
        {
            attempts = 0;
            SectorReadResult r = default(SectorReadResult);
            for (int attempt = 0; attempt <= o.RetryCount; attempt++)
            {
                if (token.IsCancellationRequested) return new SectorReadResult(SectorReadStatus.Cancelled, 0, "Cancelled");
                if (attempt > 0 && o.RetryDelayMilliseconds > 0)
                {
                    if (token.WaitHandle.WaitOne(o.RetryDelayMilliseconds)) return new SectorReadResult(SectorReadStatus.Cancelled, 0, "Cancelled");
                }
                attempts++;
                r = source.Read(sector, 1, buffer, offset);
                if (r.Status != SectorReadStatus.MediumError) return r;
            }
            return r;
        }

        private static string ValidateOptions(ISectorSource source, ImagingOptions o)
        {
            if (string.IsNullOrWhiteSpace(o.OutputPath)) return "No output file was specified.";
            string dirProblem = PathUtil.ValidateDirectory(Path.GetDirectoryName(o.OutputPath) ?? string.Empty);
            if (dirProblem != null) return dirProblem;
            string nameProblem = PathUtil.ValidateFileName(Path.GetFileName(o.OutputPath));
            if (nameProblem != null) return nameProblem;
            if (o.TotalSectors <= 0) return "The disc reports no sectors to read.";
            if (o.SectorSize != source.SectorSize) return "Sector size mismatch between disc and imaging options.";
            if (o.BlockSectors < 1 || o.BlockSectors > 64) return "Read block size must be between 1 and 64 sectors.";
            if (o.RetryCount < 0 || o.RetryCount > 50) return "Retry count must be between 0 and 50.";
            return null;
        }

        private ImagingResult Fail(ImagingResult result, ImagingErrorKind kind, string message, Stopwatch clock)
        {
            result.Outcome = ImagingOutcome.Failed;
            result.Error = kind;
            result.Message = message;
            result.OutputPath = null;
            result.Elapsed = clock.Elapsed;
            _log.Error("Imaging failed [" + kind + "]: " + message);
            return result;
        }

        private ImagingResult FailKeepTemp(ImagingResult result, ImagingErrorKind kind, string finalPath, string tempPath, Stopwatch clock)
        {
            // A file with the target name appeared while we were reading. Keep the completed data under its temp name
            // rather than overwrite the newcomer or throw away a good read.
            Fail(result, kind, "\"" + finalPath + "\" was created by another program during imaging. The completed image was kept as \""
                + tempPath + "\" so that nothing is overwritten.", clock);
            return result;
        }

        private void CloseAndDelete(ref FileStream output, string tempPath)
        {
            if (output != null)
            {
                try { output.Dispose(); } catch (IOException) { }
                output = null;
            }
            if (tempPath != null && File.Exists(tempPath))
            {
                try { File.Delete(tempPath); }
                catch (Exception ex)
                {
                    _log.Warn("Could not delete temporary file " + tempPath + ": " + ex.Message + ". It is an incomplete image and can be deleted manually.");
                }
            }
        }

        private static void WriteBadSectorReport(string path, ImagingOptions o, ImagingResult result)
        {
            var sb = new StringBuilder();
            sb.AppendLine("HT ISO Ripper - unreadable sector report");
            sb.AppendLine("THIS IMAGE IS INCOMPLETE. The sectors listed below could not be read and were replaced with zeros.");
            sb.AppendLine("Image: " + result.OutputPath);
            sb.AppendLine("Source: " + o.SourceDescription);
            sb.AppendLine("Created: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            sb.AppendLine("Sector size: " + o.SectorSize);
            sb.AppendLine("Unreadable sectors (LBA): " + result.UnreadableLbas.Count);
            foreach (long lba in result.UnreadableLbas) sb.AppendLine(lba.ToString(CultureInfo.InvariantCulture));
            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }

        private static string UniquePath(string path)
        {
            if (!File.Exists(path)) return path;
            string dir = Path.GetDirectoryName(path) ?? string.Empty;
            string name = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);
            for (int i = 2; i < 1000; i++)
            {
                string candidate = Path.Combine(dir, name + " (" + i + ")" + ext);
                if (!File.Exists(candidate)) return candidate;
            }
            throw new IOException("Could not find a free file name for " + path);
        }

        private static void MoveIntoPlace(string source, string destination, bool replace)
        {
            uint flags = MOVEFILE_WRITE_THROUGH | (replace ? MOVEFILE_REPLACE_EXISTING : 0);
            if (!MoveFileEx(source, destination, flags))
            {
                int error = Marshal.GetLastWin32Error();
                throw new IOException("Could not rename the completed image to \"" + destination + "\" (Win32 error " + error + ").",
                    unchecked((int)0x80070000) | error);
            }
        }

        private static bool IsDiskFull(IOException ex)
        {
            int code = ex.HResult & 0xFFFF;
            return code == 112 /* ERROR_DISK_FULL */ || code == 39 /* ERROR_HANDLE_DISK_FULL */;
        }

        private static void Report(IProgress<ImagingProgress> progress, ImagingProgress value)
        {
            if (progress != null) progress.Report(value);
        }

        private const uint MOVEFILE_REPLACE_EXISTING = 0x1;
        private const uint MOVEFILE_WRITE_THROUGH = 0x8;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool MoveFileEx(string existingFileName, string newFileName, uint flags);
    }

    /// <summary>Speed over a sliding ~4 second window, so the display reacts to drive speed changes.</summary>
    internal sealed class SpeedTracker
    {
        private readonly Queue<KeyValuePair<TimeSpan, long>> _samples = new Queue<KeyValuePair<TimeSpan, long>>();
        private static readonly TimeSpan Window = TimeSpan.FromSeconds(4);

        public void Add(TimeSpan time, long totalBytes)
        {
            _samples.Enqueue(new KeyValuePair<TimeSpan, long>(time, totalBytes));
            while (_samples.Count > 2 && time - _samples.Peek().Key > Window) _samples.Dequeue();
        }

        public double BytesPerSecond
        {
            get
            {
                if (_samples.Count < 2) return 0;
                KeyValuePair<TimeSpan, long> first = _samples.Peek();
                KeyValuePair<TimeSpan, long> last = default(KeyValuePair<TimeSpan, long>);
                foreach (var s in _samples) last = s;
                double seconds = (last.Key - first.Key).TotalSeconds;
                return seconds > 0 ? (last.Value - first.Value) / seconds : 0;
            }
        }
    }
}
