using System;
using System.Collections.Generic;

namespace HTISORipper.Core
{
    public static class AppInfo
    {
        public const string ProductName = "HT ISO Ripper";
        public const string Publisher = "HTREASURE LTD";
        public const string Tagline = "Professional Disc Imaging Software";
        public const string Description = "Professional CD/DVD to ISO Imaging and Backup Utility";
        public const string Copyright = "© 2026 HTREASURE LTD";
        public const string Version = "1.0.0";
        public const string Stage = "Stage 1 build";
    }

    public enum DriveMediaState
    {
        Unknown,
        NoDisc,
        NotReady,
        Ready,
        AccessDenied,
        Error
    }

    public sealed class OpticalDriveInfo
    {
        public string DriveLetter { get; set; }     // "D"
        public string RootPath { get { return DriveLetter + ":\\"; } }
        public string Vendor { get; set; }
        public string Product { get; set; }
        public string Revision { get; set; }
        public string BusType { get; set; }

        public string DeviceName
        {
            get
            {
                string name = ((Vendor ?? string.Empty).Trim() + " " + (Product ?? string.Empty).Trim()).Trim();
                return name.Length == 0 ? "Optical Drive" : name;
            }
        }

        public override string ToString()
        {
            return RootPath + "  " + DeviceName + (string.IsNullOrEmpty(BusType) ? string.Empty : " (" + BusType + ")");
        }
    }

    public enum MediaFamily
    {
        Unknown,
        CD,
        DVD,
        BluRay,
        Other
    }

    public sealed class TrackInfo
    {
        public int Number { get; set; }
        public int Session { get; set; }
        public long StartLba { get; set; }
        public long LengthSectors { get; set; }
        public byte Control { get; set; }
        /// <summary>Control bit 2 (0x04) set means a data track; clear means audio.</summary>
        public bool IsData { get { return (Control & 0x04) != 0; } }
    }

    public sealed class FilesystemInfo
    {
        public bool HasIso9660 { get; set; }
        public bool HasJoliet { get; set; }
        public bool HasUdf { get; set; }
        public bool HasUdfAnchor { get; set; }
        public bool HasElTorito { get; set; }
        public string Iso9660VolumeId { get; set; }
        /// <summary>ISO 9660 volume space size in logical blocks (from the PVD), or 0.</summary>
        public long Iso9660VolumeBlocks { get; set; }
        public int Iso9660BlockSize { get; set; }
        public string UdfRevisionHint { get; set; }

        public long Iso9660VolumeBytes { get { return Iso9660VolumeBlocks * (Iso9660BlockSize <= 0 ? 2048 : Iso9660BlockSize); } }

        public bool AnyRecognised { get { return HasIso9660 || HasUdf; } }

        public string Describe()
        {
            var parts = new List<string>();
            if (HasIso9660) parts.Add(HasJoliet ? "ISO 9660 + Joliet" : "ISO 9660");
            if (HasUdf) parts.Add(string.IsNullOrEmpty(UdfRevisionHint) ? "UDF" : "UDF (" + UdfRevisionHint + ")");
            string text = parts.Count == 0 ? "Not recognised" : string.Join(" / ", parts.ToArray());
            if (HasElTorito) text += ", bootable (El Torito)";
            return text;
        }
    }

    public enum ImageFormat
    {
        None,
        Iso,
        BinCue
    }

    public sealed class DiscInfo
    {
        public DiscInfo()
        {
            Tracks = new List<TrackInfo>();
            Filesystem = new FilesystemInfo();
            Warnings = new List<string>();
            SectorSize = 2048;
        }

        public OpticalDriveInfo Drive { get; set; }
        public string Label { get; set; }
        public int ProfileCode { get; set; }
        public string MediaTypeName { get; set; }
        public MediaFamily Family { get; set; }
        public int SessionCount { get; set; }
        public List<TrackInfo> Tracks { get; private set; }
        public int SectorSize { get; set; }
        /// <summary>Number of 2048-byte sectors the ISO image will contain (LBA 0 .. lead-out).</summary>
        public long ImageSectors { get; set; }
        /// <summary>Size of the data reported by the mounted file system (may be smaller than the image).</summary>
        public long FilesystemBytes { get; set; }
        public FilesystemInfo Filesystem { get; set; }
        public bool IsCopyProtected { get; set; }
        public List<string> Warnings { get; private set; }

        /// <summary>Whether a standard ISO image can faithfully represent this disc.</summary>
        public bool CanCreateIso { get; set; }
        /// <summary>Human-readable explanation when <see cref="CanCreateIso"/> is false.</summary>
        public string UnsupportedReason { get; set; }
        /// <summary>The correct format for this disc (BIN/CUE for audio, etc.).</summary>
        public ImageFormat RecommendedFormat { get; set; }

        public long ImageBytes { get { return ImageSectors * SectorSize; } }
        public int AudioTrackCount
        {
            get { int n = 0; foreach (var t in Tracks) if (!t.IsData) n++; return n; }
        }
        public int DataTrackCount
        {
            get { int n = 0; foreach (var t in Tracks) if (t.IsData) n++; return n; }
        }

        /// <summary>
        /// The smallest number of sectors that must be read for the file system to be complete.
        /// Used to recognise unreadable TAO run-out blocks at the very end of a CD-R track.
        /// </summary>
        public long MinimumFilesystemSectors
        {
            get
            {
                long iso = Filesystem.HasIso9660 ? (Filesystem.Iso9660VolumeBytes + SectorSize - 1) / SectorSize : 0;
                return iso;
            }
        }
    }

    public enum ErrorPolicy
    {
        /// <summary>Stop imaging at the first sector that cannot be read after all retries. Default.</summary>
        AbortOnUnreadableSector = 0,
        /// <summary>
        /// Zero-fill unreadable sectors, continue, and save the result as an INCOMPLETE image with a bad-sector list.
        /// Such an image is never reported as verified.
        /// </summary>
        ContinueAndMarkIncomplete = 1
    }

    public enum VerificationMode
    {
        /// <summary>Size check and streaming SHA-256 only.</summary>
        None = 0,
        /// <summary>Size check, re-hash the written file, validate ISO 9660/UDF structures.</summary>
        Structural = 1,
        /// <summary>Structural checks plus re-reading every sector from the disc and comparing.</summary>
        FullSourceCompare = 2
    }

    public enum ReadSpeedPreference
    {
        Automatic = 0,
        Quiet = 1,
        Balanced = 2,
        Maximum = 3
    }
}
