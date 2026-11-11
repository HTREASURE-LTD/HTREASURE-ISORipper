namespace HTISORipper.Core
{
    public enum SectorReadStatus
    {
        /// <summary>All requested sectors were read.</summary>
        Ok,
        /// <summary>A medium/CRC/seek error. The sector may succeed on retry.</summary>
        MediumError,
        /// <summary>The disc was removed, the drive is not ready or the device disappeared. Fatal.</summary>
        NotReady,
        /// <summary>Access to the device was refused. Fatal.</summary>
        AccessDenied,
        /// <summary>The read was cancelled by the user.</summary>
        Cancelled,
        /// <summary>Any other unexpected failure. Fatal.</summary>
        Fatal
    }

    public struct SectorReadResult
    {
        public SectorReadResult(SectorReadStatus status, int win32Error, string message)
        {
            Status = status;
            Win32Error = win32Error;
            Message = message;
        }

        public SectorReadStatus Status { get; private set; }
        public int Win32Error { get; private set; }
        public string Message { get; private set; }

        public bool IsOk { get { return Status == SectorReadStatus.Ok; } }

        public static readonly SectorReadResult Success = new SectorReadResult(SectorReadStatus.Ok, 0, null);

        public override string ToString()
        {
            return Status + (Win32Error != 0 ? " (Win32 error " + Win32Error + ")" : string.Empty)
                   + (string.IsNullOrEmpty(Message) ? string.Empty : ": " + Message);
        }
    }

    /// <summary>
    /// A read-only source of fixed-size logical sectors. The real implementation reads an optical drive;
    /// tests use an in-memory mock. Implementations must never write to the source.
    /// </summary>
    public interface ISectorSource
    {
        /// <summary>Logical sector size in bytes (2048 for data CDs/DVDs).</summary>
        int SectorSize { get; }

        /// <summary>
        /// Reads <paramref name="count"/> consecutive sectors starting at <paramref name="lba"/>
        /// into <paramref name="buffer"/> at <paramref name="offset"/>.
        /// On anything other than <see cref="SectorReadStatus.Ok"/> the buffer content is undefined.
        /// </summary>
        SectorReadResult Read(long lba, int count, byte[] buffer, int offset);
    }

    /// <summary>Optional: lets the imaging engine abort a blocking hardware read on cancellation.</summary>
    public interface ICancellableIo
    {
        void CancelPendingIo();
    }
}
