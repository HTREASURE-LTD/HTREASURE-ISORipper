using System;
using System.Globalization;
using System.Text;

namespace HTISORipper.Core
{
    public static class Format
    {
        public static string Bytes(long bytes)
        {
            if (bytes < 0) return "Unknown";
            string[] units = { "bytes", "KB", "MB", "GB", "TB" };
            double value = bytes;
            int unit = 0;
            while (value >= 1024 && unit < units.Length - 1) { value /= 1024; unit++; }
            return unit == 0
                ? bytes.ToString("N0", CultureInfo.CurrentCulture) + " bytes"
                : value.ToString(value >= 100 ? "0" : "0.00", CultureInfo.CurrentCulture) + " " + units[unit];
        }

        public static string BytesWithExact(long bytes)
        {
            return Bytes(bytes) + " (" + bytes.ToString("N0", CultureInfo.CurrentCulture) + " bytes)";
        }

        public static string Speed(double bytesPerSecond, MediaFamily family)
        {
            if (bytesPerSecond <= 0 || double.IsNaN(bytesPerSecond)) return "-";
            string mb = (bytesPerSecond / (1024 * 1024)).ToString("0.0", CultureInfo.CurrentCulture) + " MB/s";
            // 1x CD = 150 KiB/s of user data (2048-byte sectors at 75/s); 1x DVD = 1,385,000 bytes/s.
            double x = family == MediaFamily.DVD ? bytesPerSecond / 1385000.0
                     : family == MediaFamily.CD ? bytesPerSecond / (75 * 2048.0)
                     : 0;
            return x > 0 ? mb + " (" + x.ToString("0.0", CultureInfo.CurrentCulture) + "x)" : mb;
        }

        public static string Duration(TimeSpan span)
        {
            if (span.TotalHours >= 1) return string.Format(CultureInfo.CurrentCulture, "{0}:{1:00}:{2:00}", (int)span.TotalHours, span.Minutes, span.Seconds);
            return string.Format(CultureInfo.CurrentCulture, "{0:00}:{1:00}", span.Minutes, span.Seconds);
        }

        public static string Hex(byte[] data)
        {
            var sb = new StringBuilder(data.Length * 2);
            foreach (byte b in data) sb.Append(b.ToString("x2", CultureInfo.InvariantCulture));
            return sb.ToString();
        }
    }
}
