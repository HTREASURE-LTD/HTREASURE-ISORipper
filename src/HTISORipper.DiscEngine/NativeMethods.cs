using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace HTISORipper.DiscEngine
{
    /// <summary>
    /// Win32 declarations. Every API here is available on Windows 7 SP1 (most since Windows XP/Vista);
    /// see docs/COMPATIBILITY.md for the per-API minimum versions.
    /// </summary>
    internal static class NativeMethods
    {
        public const uint GENERIC_READ = 0x80000000;
        public const uint FILE_SHARE_READ = 0x00000001;
        public const uint FILE_SHARE_WRITE = 0x00000002;
        public const uint OPEN_EXISTING = 3;
        public const uint FILE_FLAG_NO_BUFFERING = 0x20000000;
        public const uint FILE_READ_ATTRIBUTES = 0x0080;

        // ---- IOCTL codes (CTL_CODE(DeviceType, Function, Method, Access)) ----
        // IOCTL_STORAGE_BASE = 0x2D, IOCTL_CDROM_BASE = 0x02, IOCTL_DVD_BASE = 0x33
        public const uint IOCTL_STORAGE_CHECK_VERIFY2 = 0x002D0800;   // any access
        public const uint IOCTL_STORAGE_MEDIA_REMOVAL = 0x002D4804;   // read access
        public const uint IOCTL_STORAGE_EJECT_MEDIA = 0x002D4808;     // read access
        public const uint IOCTL_STORAGE_LOAD_MEDIA = 0x002D480C;      // read access
        public const uint IOCTL_STORAGE_QUERY_PROPERTY = 0x002D1400;  // any access
        public const uint IOCTL_CDROM_GET_DRIVE_GEOMETRY_EX = 0x00024050;
        public const uint IOCTL_CDROM_READ_TOC_EX = 0x00024054;
        public const uint IOCTL_CDROM_GET_CONFIGURATION = 0x00024058;
        public const uint IOCTL_CDROM_SET_SPEED = 0x00024060;
        public const uint IOCTL_DVD_READ_STRUCTURE = 0x00335140;
        public const uint FSCTL_ALLOW_EXTENDED_DASD_IO = 0x00090083;

        public const uint MEM_COMMIT = 0x1000;
        public const uint MEM_RESERVE = 0x2000;
        public const uint MEM_RELEASE = 0x8000;
        public const uint PAGE_READWRITE = 0x04;

        public const uint THREAD_TERMINATE = 0x0001;

        // ---- Win32 error codes relevant to optical reads ----
        public const int ERROR_SUCCESS = 0;
        public const int ERROR_ACCESS_DENIED = 5;
        public const int ERROR_INVALID_HANDLE = 6;
        public const int ERROR_NOT_READY = 21;
        public const int ERROR_CRC = 23;
        public const int ERROR_SEEK = 25;
        public const int ERROR_SECTOR_NOT_FOUND = 27;
        public const int ERROR_WRITE_FAULT = 29;
        public const int ERROR_READ_FAULT = 30;
        public const int ERROR_GEN_FAILURE = 31;
        public const int ERROR_INVALID_PARAMETER = 87;
        public const int ERROR_INSUFFICIENT_BUFFER = 122;
        public const int ERROR_NO_SUCH_DEVICE = 433;
        public const int ERROR_DEVICE_HARDWARE_ERROR = 483;
        public const int ERROR_OPERATION_ABORTED = 995;
        public const int ERROR_MEDIA_CHANGED = 1110;
        public const int ERROR_NO_MEDIA_IN_DRIVE = 1112;
        public const int ERROR_IO_DEVICE = 1117;
        public const int ERROR_NOT_SUPPORTED = 50;
        public const int ERROR_INVALID_FUNCTION = 1;
        public const int ERROR_DEVICE_NOT_CONNECTED = 1167;
        public const int ERROR_UNRECOGNIZED_MEDIA = 1785;
        public const int ERROR_FILE_NOT_FOUND = 2;
        public const int ERROR_PATH_NOT_FOUND = 3;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern SafeFileHandle CreateFile(string fileName, uint desiredAccess, uint shareMode,
            IntPtr securityAttributes, uint creationDisposition, uint flagsAndAttributes, IntPtr templateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeviceIoControl(SafeFileHandle device, uint ioControlCode,
            byte[] inBuffer, int inBufferSize, byte[] outBuffer, int outBufferSize,
            out int bytesReturned, IntPtr overlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeviceIoControl(SafeFileHandle device, uint ioControlCode,
            IntPtr inBuffer, int inBufferSize, IntPtr outBuffer, int outBufferSize,
            out int bytesReturned, IntPtr overlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReadFile(SafeFileHandle file, IntPtr buffer, int numberOfBytesToRead,
            out int numberOfBytesRead, IntPtr overlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetFilePointerEx(SafeFileHandle file, long distanceToMove, out long newFilePointer, uint moveMethod);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAlloc(IntPtr address, UIntPtr size, uint allocationType, uint protect);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool VirtualFree(IntPtr address, UIntPtr size, uint freeType);

        [DllImport("kernel32.dll")]
        public static extern uint GetCurrentThreadId();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenThread(uint desiredAccess, [MarshalAs(UnmanagedType.Bool)] bool inheritHandle, uint threadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr handle);

        /// <summary>Windows Vista and later.</summary>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CancelSynchronousIo(IntPtr thread);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern uint SetErrorMode(uint mode);

        public const uint SEM_FAILCRITICALERRORS = 0x0001;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetVolumeInformation(string rootPathName, char[] volumeNameBuffer, int volumeNameSize,
            out uint volumeSerialNumber, out uint maximumComponentLength, out uint fileSystemFlags,
            char[] fileSystemNameBuffer, int fileSystemNameSize);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetDiskFreeSpaceEx(string directoryName, out ulong freeBytesAvailable,
            out ulong totalNumberOfBytes, out ulong totalNumberOfFreeBytes);
    }
}
