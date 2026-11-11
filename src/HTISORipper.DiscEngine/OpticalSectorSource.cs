using System;
using System.Runtime.InteropServices;
using HTISORipper.Core;

namespace HTISORipper.DiscEngine
{
    /// <summary>
    /// Reads 2048-byte user-data sectors from an optical drive through the volume device (\\.\D:),
    /// which the Windows CD-ROM class driver maps directly to absolute LBAs. The handle is opened
    /// read-only with FILE_FLAG_NO_BUFFERING, so every read reaches the drive (no stale cache) and the
    /// source can never be modified.
    /// </summary>
    public sealed class OpticalSectorSource : ISectorSource, ICancellableIo, IDisposable
    {
        public const int CdDvdSectorSize = 2048;

        private readonly OpticalDevice _device;
        private readonly bool _ownsDevice;
        private readonly int _maxSectorsPerRead;
        private readonly object _ioSync = new object();
        private IntPtr _alignedBuffer;
        private readonly int _alignedBufferSize;
        private IntPtr _readingThread;      // valid only while a ReadFile is in progress
        private bool _disposed;

        public OpticalSectorSource(OpticalDevice device, int maxSectorsPerRead, bool ownsDevice)
        {
            if (device == null) throw new ArgumentNullException("device");
            if (maxSectorsPerRead < 1 || maxSectorsPerRead > 64) throw new ArgumentOutOfRangeException("maxSectorsPerRead");
            _device = device;
            _ownsDevice = ownsDevice;
            _maxSectorsPerRead = maxSectorsPerRead;
            _alignedBufferSize = maxSectorsPerRead * CdDvdSectorSize;
            // VirtualAlloc returns page-aligned memory, satisfying the alignment rules of unbuffered I/O.
            _alignedBuffer = NativeMethods.VirtualAlloc(IntPtr.Zero, (UIntPtr)(uint)_alignedBufferSize,
                NativeMethods.MEM_COMMIT | NativeMethods.MEM_RESERVE, NativeMethods.PAGE_READWRITE);
            if (_alignedBuffer == IntPtr.Zero)
                throw new OutOfMemoryException("Could not allocate the disc read buffer.");

            // The mounted file system (CDFS/UDFS) would otherwise refuse reads past its own volume size,
            // e.g. the final sectors of a track. Failure is not fatal: older drivers simply do not need it.
            _device.AllowExtendedDasdIo();
        }

        public static OpticalSectorSource Open(string driveLetter, int maxSectorsPerRead)
        {
            var device = OpticalDevice.Open(driveLetter, true);
            try
            {
                return new OpticalSectorSource(device, maxSectorsPerRead, true);
            }
            catch
            {
                device.Dispose();
                throw;
            }
        }

        public int SectorSize { get { return CdDvdSectorSize; } }
        public OpticalDevice Device { get { return _device; } }

        public SectorReadResult Read(long lba, int count, byte[] buffer, int offset)
        {
            if (_disposed) throw new ObjectDisposedException("OpticalSectorSource");
            if (lba < 0) throw new ArgumentOutOfRangeException("lba");
            if (count < 1 || count > _maxSectorsPerRead) throw new ArgumentOutOfRangeException("count");
            if (buffer == null) throw new ArgumentNullException("buffer");
            int bytes = count * CdDvdSectorSize;
            if (offset < 0 || offset + bytes > buffer.Length) throw new ArgumentOutOfRangeException("offset");

            long position;
            if (!NativeMethods.SetFilePointerEx(_device.Handle, lba * CdDvdSectorSize, out position, 0))
                return Classify(Marshal.GetLastWin32Error(), "seek");

            int read;
            bool ok;
            int error;
            IntPtr thread = NativeMethods.OpenThread(NativeMethods.THREAD_TERMINATE, false, NativeMethods.GetCurrentThreadId());
            lock (_ioSync) { _readingThread = thread; }
            try
            {
                ok = NativeMethods.ReadFile(_device.Handle, _alignedBuffer, bytes, out read, IntPtr.Zero);
                error = ok ? 0 : Marshal.GetLastWin32Error();
            }
            finally
            {
                lock (_ioSync) { _readingThread = IntPtr.Zero; }
                if (thread != IntPtr.Zero) NativeMethods.CloseHandle(thread);
            }

            if (!ok) return Classify(error, "read");
            if (read != bytes)
            {
                // Short read: the drive returned fewer sectors than requested (end of readable area or a defect).
                return new SectorReadResult(SectorReadStatus.MediumError, 0,
                    "Drive returned " + read + " of " + bytes + " bytes");
            }
            Marshal.Copy(_alignedBuffer, buffer, offset, bytes);
            return SectorReadResult.Success;
        }

        /// <summary>Aborts a blocking ReadFile (a damaged sector can stall the drive for many seconds).</summary>
        public void CancelPendingIo()
        {
            lock (_ioSync)
            {
                if (_readingThread != IntPtr.Zero) NativeMethods.CancelSynchronousIo(_readingThread);
            }
        }

        internal static SectorReadResult Classify(int error, string operation)
        {
            string message = operation + " failed: " + new System.ComponentModel.Win32Exception(error).Message;
            switch (error)
            {
                case NativeMethods.ERROR_NOT_READY:
                case NativeMethods.ERROR_NO_MEDIA_IN_DRIVE:
                case NativeMethods.ERROR_MEDIA_CHANGED:
                case NativeMethods.ERROR_DEVICE_NOT_CONNECTED:
                case NativeMethods.ERROR_NO_SUCH_DEVICE:
                case NativeMethods.ERROR_INVALID_HANDLE:
                case NativeMethods.ERROR_UNRECOGNIZED_MEDIA:
                    return new SectorReadResult(SectorReadStatus.NotReady, error, message);
                case NativeMethods.ERROR_ACCESS_DENIED:
                    return new SectorReadResult(SectorReadStatus.AccessDenied, error, message);
                case NativeMethods.ERROR_OPERATION_ABORTED:
                    return new SectorReadResult(SectorReadStatus.Cancelled, error, message);
                case NativeMethods.ERROR_CRC:
                case NativeMethods.ERROR_SEEK:
                case NativeMethods.ERROR_SECTOR_NOT_FOUND:
                case NativeMethods.ERROR_READ_FAULT:
                case NativeMethods.ERROR_GEN_FAILURE:
                case NativeMethods.ERROR_IO_DEVICE:
                case NativeMethods.ERROR_DEVICE_HARDWARE_ERROR:
                case NativeMethods.ERROR_INVALID_PARAMETER:   // some drivers report out-of-range/illegal-mode sectors this way
                    return new SectorReadResult(SectorReadStatus.MediumError, error, message);
                default:
                    return new SectorReadResult(SectorReadStatus.Fatal, error, message);
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            if (_alignedBuffer != IntPtr.Zero)
            {
                NativeMethods.VirtualFree(_alignedBuffer, UIntPtr.Zero, NativeMethods.MEM_RELEASE);
                _alignedBuffer = IntPtr.Zero;
            }
            if (_ownsDevice) _device.Dispose();
        }
    }
}
