using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using HTISORipper.Core;
using Microsoft.Win32.SafeHandles;

namespace HTISORipper.DiscEngine
{
    public sealed class DeviceAccessException : Exception
    {
        public DeviceAccessException(string message, int win32Error)
            : base(message + " (" + new Win32Exception(win32Error).Message + ", Win32 error " + win32Error + ")")
        {
            Win32Error = win32Error;
        }

        public int Win32Error { get; private set; }
        public bool IsAccessDenied { get { return Win32Error == NativeMethods.ERROR_ACCESS_DENIED; } }
    }

    /// <summary>
    /// A handle to an optical drive's volume (\\.\D:) opened READ-ONLY.
    /// Only documented storage/CD-ROM/DVD IOCTLs are used; none of them require administrator rights
    /// and none of them can modify the disc. HT ISO Ripper never opens the device with write access.
    /// </summary>
    public sealed class OpticalDevice : IDisposable
    {
        private readonly SafeFileHandle _handle;
        private readonly string _driveLetter;
        private bool _locked;

        private OpticalDevice(SafeFileHandle handle, string driveLetter)
        {
            _handle = handle;
            _driveLetter = driveLetter;
        }

        public string DriveLetter { get { return _driveLetter; } }
        internal SafeFileHandle Handle { get { return _handle; } }

        public static string DevicePath(string driveLetter)
        {
            if (string.IsNullOrEmpty(driveLetter) || driveLetter.Length != 1 || !char.IsLetter(driveLetter[0]))
                throw new ArgumentException("Drive letter must be a single letter.", "driveLetter");
            return @"\\.\" + char.ToUpperInvariant(driveLetter[0]) + ":";
        }

        /// <summary>
        /// Opens the drive. <paramref name="forReading"/> = false opens a query-only handle
        /// (does not spin up or require media); true opens GENERIC_READ for sector reads and disc IOCTLs.
        /// </summary>
        public static OpticalDevice Open(string driveLetter, bool forReading)
        {
            uint access = forReading ? NativeMethods.GENERIC_READ : NativeMethods.FILE_READ_ATTRIBUTES;
            uint flags = forReading ? NativeMethods.FILE_FLAG_NO_BUFFERING : 0;
            SafeFileHandle handle = NativeMethods.CreateFile(DevicePath(driveLetter), access,
                NativeMethods.FILE_SHARE_READ | NativeMethods.FILE_SHARE_WRITE, IntPtr.Zero,
                NativeMethods.OPEN_EXISTING, flags, IntPtr.Zero);
            if (handle.IsInvalid)
            {
                int error = Marshal.GetLastWin32Error();
                handle.Dispose();
                throw new DeviceAccessException("Could not open drive " + driveLetter + ":", error);
            }
            return new OpticalDevice(handle, driveLetter.ToUpperInvariant());
        }

        // ------------------------------------------------------------------ status

        /// <summary>Asks the drive whether media is present and ready. Cheap; safe to poll.</summary>
        public DriveMediaState CheckMedia(out int win32Error)
        {
            int returned;
            if (NativeMethods.DeviceIoControl(_handle, NativeMethods.IOCTL_STORAGE_CHECK_VERIFY2,
                    IntPtr.Zero, 0, IntPtr.Zero, 0, out returned, IntPtr.Zero))
            {
                win32Error = 0;
                return DriveMediaState.Ready;
            }
            win32Error = Marshal.GetLastWin32Error();
            switch (win32Error)
            {
                case NativeMethods.ERROR_NO_MEDIA_IN_DRIVE: return DriveMediaState.NoDisc;
                case NativeMethods.ERROR_NOT_READY:
                case NativeMethods.ERROR_MEDIA_CHANGED: return DriveMediaState.NotReady;
                case NativeMethods.ERROR_ACCESS_DENIED: return DriveMediaState.AccessDenied;
                default: return DriveMediaState.Error;
            }
        }

        public void QueryDeviceDescriptor(OpticalDriveInfo target)
        {
            // STORAGE_PROPERTY_QUERY { PropertyId = StorageDeviceProperty(0), QueryType = PropertyStandardQuery(0), AdditionalParameters[1] }
            byte[] query = new byte[12];
            byte[] output = new byte[1024];
            int returned;
            if (!NativeMethods.DeviceIoControl(_handle, NativeMethods.IOCTL_STORAGE_QUERY_PROPERTY,
                    query, query.Length, output, output.Length, out returned, IntPtr.Zero) || returned < 36)
                return;

            // STORAGE_DEVICE_DESCRIPTOR layout:
            //  0 Version, 4 Size, 8 DeviceType, 9 Modifier, 10 RemovableMedia, 11 CommandQueueing,
            // 12 VendorIdOffset, 16 ProductIdOffset, 20 ProductRevisionOffset, 24 SerialNumberOffset, 28 BusType
            target.Vendor = ReadAnsi(output, BitConverter.ToInt32(output, 12), returned);
            target.Product = ReadAnsi(output, BitConverter.ToInt32(output, 16), returned);
            target.Revision = ReadAnsi(output, BitConverter.ToInt32(output, 20), returned);
            target.BusType = BusTypeName(BitConverter.ToInt32(output, 28));
        }

        // ------------------------------------------------------------------ media identification

        /// <summary>MMC current profile (e.g. 0x0010 = DVD-ROM) via GET CONFIGURATION. Returns 0 if unavailable.</summary>
        public int GetCurrentProfile()
        {
            // GET_CONFIGURATION_IOCTL_INPUT { ULONG Feature; ULONG RequestType; PVOID Reserved[2]; }
            int inputSize = 8 + 2 * IntPtr.Size;
            byte[] input = new byte[inputSize];
            BitConverter.GetBytes(0).CopyTo(input, 0);   // Feature 0 = profile list
            BitConverter.GetBytes(2).CopyTo(input, 4);   // SCSI_GET_CONFIGURATION_REQUEST_TYPE_ONE
            byte[] output = new byte[256];
            int returned;
            if (!NativeMethods.DeviceIoControl(_handle, NativeMethods.IOCTL_CDROM_GET_CONFIGURATION,
                    input, input.Length, output, output.Length, out returned, IntPtr.Zero) || returned < 8)
                return 0;
            return (output[6] << 8) | output[7];   // GET_CONFIGURATION_HEADER.CurrentProfile (big-endian)
        }

        /// <summary>Reads the table of contents in LBA form. Returns null if the drive cannot provide one (blank/unreadable).</summary>
        public List<TrackInfo> ReadToc(out long leadOutLba, out int win32Error)
        {
            leadOutLba = -1;
            // CDROM_READ_TOC_EX { Format:4 Reserved:3 Msf:1; SessionTrack; Reserved2; Reserved3 }
            byte[] input = { 0x00 /* CDROM_READ_TOC_EX_FORMAT_TOC, Msf = 0 (LBA) */, 1, 0, 0 };
            byte[] output = new byte[4 + 100 * 8];
            int returned;
            if (!NativeMethods.DeviceIoControl(_handle, NativeMethods.IOCTL_CDROM_READ_TOC_EX,
                    input, input.Length, output, output.Length, out returned, IntPtr.Zero) || returned < 4)
            {
                win32Error = Marshal.GetLastWin32Error();
                return null;
            }
            win32Error = 0;

            int dataLength = (output[0] << 8) | output[1];
            int entries = Math.Min((Math.Min(dataLength + 2, returned) - 4) / 8, 100);
            var tracks = new List<TrackInfo>();
            for (int i = 0; i < entries; i++)
            {
                int o = 4 + i * 8;
                byte control = (byte)(output[o + 1] & 0x0F);           // TRACK_DATA: Control:4 (low), Adr:4 (high)
                int number = output[o + 2];
                long lba = ((long)output[o + 4] << 24) | ((long)output[o + 5] << 16) | ((long)output[o + 6] << 8) | output[o + 7];
                if (number == 0xAA) { leadOutLba = lba; continue; }
                tracks.Add(new TrackInfo { Number = number, StartLba = lba, Control = control, Session = 1 });
            }
            for (int i = 0; i < tracks.Count; i++)
            {
                long end = i + 1 < tracks.Count ? tracks[i + 1].StartLba : leadOutLba;
                tracks[i].LengthSectors = end > tracks[i].StartLba ? end - tracks[i].StartLba : 0;
            }
            return tracks;
        }

        /// <summary>Returns the number of the last complete session (1 for single-session discs), or 0 if unknown.</summary>
        public int ReadLastSessionNumber(out int firstTrackOfLastSession)
        {
            firstTrackOfLastSession = 0;
            byte[] input = { 0x01 /* CDROM_READ_TOC_EX_FORMAT_SESSION */, 0, 0, 0 };
            byte[] output = new byte[12];
            int returned;
            if (!NativeMethods.DeviceIoControl(_handle, NativeMethods.IOCTL_CDROM_READ_TOC_EX,
                    input, input.Length, output, output.Length, out returned, IntPtr.Zero) || returned < 12)
                return 0;
            firstTrackOfLastSession = output[6];
            return output[3];   // LastCompleteSession
        }

        /// <summary>Disc size in bytes as reported by the drive (READ CAPACITY), or -1.</summary>
        public long GetDiskSize(out int bytesPerSector)
        {
            bytesPerSector = 0;
            byte[] output = new byte[256];
            int returned;
            if (!NativeMethods.DeviceIoControl(_handle, NativeMethods.IOCTL_CDROM_GET_DRIVE_GEOMETRY_EX,
                    null, 0, output, output.Length, out returned, IntPtr.Zero) || returned < 32)
                return -1;
            // DISK_GEOMETRY (24 bytes: Cylinders 8, MediaType 4, TracksPerCylinder 4, SectorsPerTrack 4, BytesPerSector 4) + DiskSize 8
            bytesPerSector = BitConverter.ToInt32(output, 20);
            return BitConverter.ToInt64(output, 24);
        }

        /// <summary>
        /// DVD copyright descriptor. Returns the copy-protection system type (0 = none, 1 = CSS/CPPM, 2 = CPRM),
        /// or -1 if not a DVD / not available.
        /// </summary>
        public int GetDvdCopyProtectionType()
        {
            // DVD_READ_STRUCTURE { LARGE_INTEGER BlockByteOffset; DVD_STRUCTURE_FORMAT Format; DVD_SESSION_ID SessionId; UCHAR LayerNumber; } -> 24 bytes with padding
            byte[] input = new byte[24];
            BitConverter.GetBytes(1).CopyTo(input, 8);   // DvdCopyrightDescriptor
            byte[] output = new byte[8];                 // DVD_DESCRIPTOR_HEADER (4) + DVD_COPYRIGHT_DESCRIPTOR (4)
            int returned;
            if (!NativeMethods.DeviceIoControl(_handle, NativeMethods.IOCTL_DVD_READ_STRUCTURE,
                    input, input.Length, output, output.Length, out returned, IntPtr.Zero) || returned < 5)
                return -1;
            return output[4];
        }

        // ------------------------------------------------------------------ control

        /// <summary>
        /// Requests a read speed in KB/s (0xFFFF = maximum). Returns false if the drive does not support speed control;
        /// that is normal and not an error.
        /// </summary>
        public bool TrySetReadSpeed(ushort kilobytesPerSecond, out int win32Error)
        {
            // CDROM_SET_SPEED { CDROM_SPEED_REQUEST RequestType; USHORT ReadSpeed; USHORT WriteSpeed; WRITE_ROTATION RotationControl; }
            byte[] input = new byte[12];
            BitConverter.GetBytes(0).CopyTo(input, 0);                          // CdromSetSpeed
            BitConverter.GetBytes(kilobytesPerSecond).CopyTo(input, 4);
            BitConverter.GetBytes((ushort)0xFFFF).CopyTo(input, 6);             // leave write speed at maximum/default
            BitConverter.GetBytes(0).CopyTo(input, 8);                          // CdromDefaultRotation
            int returned;
            if (NativeMethods.DeviceIoControl(_handle, NativeMethods.IOCTL_CDROM_SET_SPEED,
                    input, input.Length, null, 0, out returned, IntPtr.Zero))
            {
                win32Error = 0;
                return true;
            }
            win32Error = Marshal.GetLastWin32Error();
            return false;
        }

        /// <summary>Allows reads beyond the size the mounted file system reports (needed for full-track imaging).</summary>
        public bool AllowExtendedDasdIo()
        {
            int returned;
            return NativeMethods.DeviceIoControl(_handle, NativeMethods.FSCTL_ALLOW_EXTENDED_DASD_IO,
                IntPtr.Zero, 0, IntPtr.Zero, 0, out returned, IntPtr.Zero);
        }

        /// <summary>Locks or unlocks the tray so the disc cannot be ejected with the button while imaging.</summary>
        public bool SetMediaLock(bool locked)
        {
            byte[] input = { (byte)(locked ? 1 : 0) };   // PREVENT_MEDIA_REMOVAL { BOOLEAN PreventMediaRemoval; }
            int returned;
            bool ok = NativeMethods.DeviceIoControl(_handle, NativeMethods.IOCTL_STORAGE_MEDIA_REMOVAL,
                input, input.Length, null, 0, out returned, IntPtr.Zero);
            if (ok) _locked = locked;
            return ok;
        }

        public bool Eject(out int win32Error)
        {
            if (_locked) SetMediaLock(false);
            int returned;
            if (NativeMethods.DeviceIoControl(_handle, NativeMethods.IOCTL_STORAGE_EJECT_MEDIA,
                    IntPtr.Zero, 0, IntPtr.Zero, 0, out returned, IntPtr.Zero))
            {
                win32Error = 0;
                return true;
            }
            win32Error = Marshal.GetLastWin32Error();
            return false;
        }

        public void Dispose()
        {
            if (_locked && !_handle.IsClosed)
            {
                try { SetMediaLock(false); } catch (ObjectDisposedException) { }
            }
            _handle.Dispose();
        }

        // ------------------------------------------------------------------ helpers

        private static string ReadAnsi(byte[] buffer, int offset, int limit)
        {
            if (offset <= 0 || offset >= limit) return string.Empty;
            int end = offset;
            while (end < limit && buffer[end] != 0) end++;
            return Encoding.ASCII.GetString(buffer, offset, end - offset).Trim();
        }

        private static string BusTypeName(int busType)
        {
            switch (busType)
            {
                case 1: return "SCSI";
                case 2: return "ATAPI";
                case 3: return "ATA";
                case 4: return "IEEE 1394";
                case 7: return "USB";
                case 11: return "SATA";
                case 10: return "SAS";
                default: return string.Empty;
            }
        }
    }
}
