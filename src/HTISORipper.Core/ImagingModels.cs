using System;
using System.Collections.Generic;

namespace HTISORipper.Core
{
    public sealed class ImagingOptions
    {
        public ImagingOptions()
        {
            SectorSize = 2048;
            BlockSectors = 32;
            RetryCount = 3;
            RetryDelayMilliseconds = 150;
            ErrorPolicy = ErrorPolicy.AbortOnUnreadableSector;
        }

        /// <summary>Final output path, e.g. C:\Users\Public\Documents\HT ISO Ripper\DISC.iso</summary>
        public string OutputPath { get; set; }
        public bool AllowOverwrite { get; set; }
        public int SectorSize { get; set; }
        /// <summary>Total sectors to image starting at LBA 0.</summary>
        public long TotalSectors { get; set; }
        /// <summary>
        /// Sectors the file system needs. Unreadable sectors at or beyond this value AND within the last
        /// <see cref="RunOutToleranceSectors"/> sectors of the track are treated as CD-R run-out blocks:
        /// the image is ended before them. 0 disables the tolerance.
        /// </summary>
        public long MinimumRequiredSectors { get; set; }
        public int RunOutToleranceSectors { get; set; }
        /// <summary>Sectors per read request (streamed; memory use = BlockSectors * SectorSize).</summary>
        public int BlockSectors { get; set; }
        public int RetryCount { get; set; }
        public int RetryDelayMilliseconds { get; set; }
        public ErrorPolicy ErrorPolicy { get; set; }
        /// <summary>Free text written to the log to identify the source (drive, label).</summary>
        public string SourceDescription { get; set; }
    }

    public enum ImagingPhase
    {
        Preparing,
        Reading,
        Finalising,
        VerifyingFile,
        VerifyingSource,
        Completed
    }

    public sealed class ImagingProgress
    {
        public ImagingPhase Phase { get; set; }
        public long SectorsDone { get; set; }
        public long TotalSectors { get; set; }
        public double BytesPerSecond { get; set; }
        public TimeSpan? EstimatedRemaining { get; set; }
        public int UnreadableSectors { get; set; }
        public int RecoveredSectors { get; set; }
        public string Message { get; set; }

        public double Fraction
        {
            get { return TotalSectors <= 0 ? 0 : Math.Min(1.0, (double)SectorsDone / TotalSectors); }
        }
    }

    public enum ImagingOutcome
    {
        /// <summary>Every sector read successfully and the image was finalised.</summary>
        Success,
        /// <summary>Image finalised but contains zero-filled unreadable sectors (INCOMPLETE).</summary>
        CompletedWithErrors,
        Cancelled,
        Failed
    }

    public enum ImagingErrorKind
    {
        None,
        NoDisc,
        DiscNotReady,
        DiscRemoved,
        UnsupportedDisc,
        UnreadableSectors,
        InsufficientSpace,
        DestinationPermissionDenied,
        OutputExists,
        DeviceAccessDenied,
        InvalidDestination,
        WriteFailed,
        VerificationMismatch,
        Cancelled,
        Unexpected
    }

    public sealed class ImagingResult
    {
        public ImagingResult()
        {
            UnreadableLbas = new List<long>();
        }

        public ImagingOutcome Outcome { get; set; }
        public ImagingErrorKind Error { get; set; }
        public string Message { get; set; }
        /// <summary>The finalised image path, or null if nothing was kept.</summary>
        public string OutputPath { get; set; }
        /// <summary>Path of the bad-sector report written alongside an INCOMPLETE image.</summary>
        public string BadSectorReportPath { get; set; }
        public long SectorsWritten { get; set; }
        public long ImageBytes { get; set; }
        public TimeSpan Elapsed { get; set; }
        public double AverageBytesPerSecond { get; set; }
        /// <summary>SHA-256 of the image bytes computed while streaming them to disk (lower-case hex).</summary>
        public string Sha256 { get; set; }
        public int RecoveredSectors { get; set; }
        public List<long> UnreadableLbas { get; private set; }
        /// <summary>Set when trailing unreadable run-out sectors were excluded from the image.</summary>
        public int RunOutSectorsExcluded { get; set; }
    }

    public enum VerificationLevel
    {
        NotVerified,
        /// <summary>Size, re-hash and file-system structures checked; source not re-read.</summary>
        StructuralOnly,
        /// <summary>Every sector re-read from the disc and matched the image.</summary>
        SourceVerified,
        Failed
    }

    public sealed class VerificationReport
    {
        public VerificationReport()
        {
            Notes = new List<string>();
            MismatchedLbas = new List<long>();
            UnreadableLbas = new List<long>();
        }

        public VerificationLevel Level { get; set; }
        public bool SizeOk { get; set; }
        public bool? FileHashMatches { get; set; }
        public bool? StructureOk { get; set; }
        public string StructureSummary { get; set; }
        public bool SourceCompared { get; set; }
        public bool? SourceMatches { get; set; }
        public List<long> MismatchedLbas { get; private set; }
        public List<long> UnreadableLbas { get; private set; }
        public List<string> Notes { get; private set; }
        public bool Cancelled { get; set; }

        public string Summary
        {
            get
            {
                switch (Level)
                {
                    case VerificationLevel.SourceVerified: return "Verified against source disc (every sector matched)";
                    case VerificationLevel.StructuralOnly: return "Structural check passed (source disc not re-read)";
                    case VerificationLevel.Failed: return "VERIFICATION FAILED";
                    default: return Cancelled ? "Verification cancelled" : "Not verified";
                }
            }
        }
    }
}
