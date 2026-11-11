using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace HTISORipper.Core
{
    /// <summary>File-name and destination validation. All user-supplied paths pass through here.</summary>
    public static class PathUtil
    {
        private static readonly string[] ReservedNames =
        {
            "CON", "PRN", "AUX", "NUL",
            "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
            "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
        };

        public const int MaxBaseNameLength = 120;

        public static string DefaultOutputDirectory
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), "HT ISO Ripper");
            }
        }

        /// <summary>Turns a disc label into a safe base file name (no extension). Never returns empty.</summary>
        public static string SanitizeBaseName(string label)
        {
            string text = (label ?? string.Empty).Trim();
            var invalid = Path.GetInvalidFileNameChars();
            var sb = new StringBuilder(text.Length);
            foreach (char c in text)
            {
                if (c < 32 || invalid.Contains(c)) sb.Append('_');
                else sb.Append(c);
            }
            string result = sb.ToString().Trim().TrimEnd('.', ' ');
            if (result.Length > MaxBaseNameLength) result = result.Substring(0, MaxBaseNameLength).TrimEnd('.', ' ');
            if (result.Length == 0 || result.All(ch => ch == '_')) result = "DISC";

            string stem = result.Split('.')[0];
            if (ReservedNames.Any(r => string.Equals(r, stem, StringComparison.OrdinalIgnoreCase)))
                result = "_" + result;
            return result;
        }

        /// <summary>
        /// Validates a file name typed by the user (with extension). Returns null if valid, otherwise a message.
        /// </summary>
        public static string ValidateFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return "Enter an output file name.";
            if (fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
                return "The file name contains characters that Windows does not allow (\\ / : * ? \" < > |).";
            if (fileName.EndsWith(".", StringComparison.Ordinal) || fileName.EndsWith(" ", StringComparison.Ordinal))
                return "The file name cannot end with a dot or a space.";
            string stem = fileName.Split('.')[0];
            if (ReservedNames.Any(r => string.Equals(r, stem, StringComparison.OrdinalIgnoreCase)))
                return "\"" + stem + "\" is a reserved Windows device name.";
            if (fileName.Length > 200) return "The file name is too long.";
            return null;
        }

        /// <summary>Validates a destination directory. Returns null if valid, otherwise a message.</summary>
        public static string ValidateDirectory(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory)) return "Choose a destination folder.";
            try
            {
                if (directory.IndexOfAny(Path.GetInvalidPathChars()) >= 0) return "The destination folder path is not valid.";
                if (!Path.IsPathRooted(directory)) return "The destination folder must be a full path (for example C:\\ISO).";
                string full = Path.GetFullPath(directory);
                if (full.StartsWith(@"\\.\", StringComparison.Ordinal) || full.StartsWith(@"\\?\", StringComparison.Ordinal))
                    return "Device paths are not allowed as a destination.";
            }
            catch (ArgumentException) { return "The destination folder path is not valid."; }
            catch (NotSupportedException) { return "The destination folder path is not valid."; }
            catch (PathTooLongException) { return "The destination folder path is too long."; }
            return null;
        }

        /// <summary>The temporary file used while imaging; renamed atomically on success.</summary>
        public static string GetTempPathFor(string finalPath)
        {
            return finalPath + ".htpart";
        }

        /// <summary>"NAME.iso" -> "NAME.INCOMPLETE.iso"</summary>
        public static string GetIncompletePathFor(string finalPath)
        {
            string dir = Path.GetDirectoryName(finalPath) ?? string.Empty;
            string name = Path.GetFileNameWithoutExtension(finalPath);
            string ext = Path.GetExtension(finalPath);
            return Path.Combine(dir, name + ".INCOMPLETE" + ext);
        }

        /// <summary>Free bytes available to the current user on the volume that holds <paramref name="directory"/>.</summary>
        public static long GetAvailableFreeSpace(string directory)
        {
            ulong freeToCaller, total, totalFree;
            string probe = directory;
            while (!string.IsNullOrEmpty(probe) && !Directory.Exists(probe)) probe = Path.GetDirectoryName(probe);
            if (string.IsNullOrEmpty(probe)) return -1;
            if (!probe.EndsWith("\\", StringComparison.Ordinal)) probe += "\\";
            if (!GetDiskFreeSpaceEx(probe, out freeToCaller, out total, out totalFree)) return -1;
            return freeToCaller > long.MaxValue ? long.MaxValue : (long)freeToCaller;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetDiskFreeSpaceEx(string lpDirectoryName, out ulong lpFreeBytesAvailable,
            out ulong lpTotalNumberOfBytes, out ulong lpTotalNumberOfFreeBytes);
    }
}
