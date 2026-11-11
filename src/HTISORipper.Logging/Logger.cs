using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace HTISORipper.Logging
{
    public enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }

    /// <summary>Minimal logging abstraction used by every HT ISO Ripper component.</summary>
    public interface ILogger
    {
        void Write(LogLevel level, string message, Exception exception = null);
    }

    public static class LoggerExtensions
    {
        public static void Debug(this ILogger log, string message) { log.Write(LogLevel.Debug, message); }
        public static void Info(this ILogger log, string message) { log.Write(LogLevel.Info, message); }
        public static void Warn(this ILogger log, string message) { log.Write(LogLevel.Warning, message); }
        public static void Error(this ILogger log, string message, Exception ex = null) { log.Write(LogLevel.Error, message, ex); }
    }

    public sealed class NullLogger : ILogger
    {
        public static readonly NullLogger Instance = new NullLogger();
        private NullLogger() { }
        public void Write(LogLevel level, string message, Exception exception = null) { }
    }

    /// <summary>
    /// Thread-safe, append-only text logger. One file per day:
    /// %LOCALAPPDATA%\HTREASURE LTD\HT ISO Ripper\Logs\HTISORipper_yyyyMMdd.log
    /// Full exception details go to the log file only; the UI shows friendly messages.
    /// </summary>
    public sealed class FileLogger : ILogger
    {
        private readonly object _sync = new object();
        private readonly string _directory;

        public event EventHandler<LogEntryEventArgs> EntryWritten;

        public FileLogger(string directory)
        {
            if (string.IsNullOrWhiteSpace(directory)) throw new ArgumentNullException("directory");
            _directory = directory;
            Directory.CreateDirectory(_directory);
        }

        public static string DefaultDirectory
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "HTREASURE LTD", "HT ISO Ripper", "Logs");
            }
        }

        public string LogDirectory { get { return _directory; } }

        public string CurrentLogFile
        {
            get
            {
                return Path.Combine(_directory,
                    "HTISORipper_" + DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture) + ".log");
            }
        }

        public void Write(LogLevel level, string message, Exception exception = null)
        {
            var sb = new StringBuilder();
            sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture));
            sb.Append(" [").Append(LevelText(level)).Append("] ");
            sb.Append(message);
            if (exception != null)
            {
                sb.AppendLine();
                sb.Append("    ").Append(exception.ToString().Replace(Environment.NewLine, Environment.NewLine + "    "));
            }
            string line = sb.ToString();

            lock (_sync)
            {
                try
                {
                    File.AppendAllText(CurrentLogFile, line + Environment.NewLine, Encoding.UTF8);
                }
                catch (IOException)
                {
                    // Logging must never crash the application.
                }
                catch (UnauthorizedAccessException)
                {
                }
            }

            var handler = EntryWritten;
            if (handler != null) handler(this, new LogEntryEventArgs(level, line));
        }

        /// <summary>Deletes log files older than the retention period. Returns number deleted.</summary>
        public int ApplyRetention(int retentionDays)
        {
            if (retentionDays <= 0) return 0;
            int deleted = 0;
            DateTime cutoff = DateTime.Now.AddDays(-retentionDays);
            try
            {
                foreach (string file in Directory.GetFiles(_directory, "HTISORipper_*.log"))
                {
                    if (File.GetLastWriteTime(file) < cutoff)
                    {
                        try { File.Delete(file); deleted++; }
                        catch (IOException) { }
                        catch (UnauthorizedAccessException) { }
                    }
                }
            }
            catch (IOException) { }
            catch (UnauthorizedAccessException) { }
            return deleted;
        }

        private static string LevelText(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug: return "DEBUG";
                case LogLevel.Info: return "INFO ";
                case LogLevel.Warning: return "WARN ";
                default: return "ERROR";
            }
        }
    }

    public sealed class LogEntryEventArgs : EventArgs
    {
        public LogEntryEventArgs(LogLevel level, string line) { Level = level; Line = line; }
        public LogLevel Level { get; private set; }
        public string Line { get; private set; }
    }
}
