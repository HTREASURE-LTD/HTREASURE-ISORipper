using System;
using System.Threading;
using System.Windows.Forms;
using HTISORipper.Core;
using HTISORipper.DiscEngine;
using HTISORipper.Logging;
using HTISORipper.UI.Forms;

namespace HTISORipper.UI
{
    /// <summary>Shared services handed to every view.</summary>
    internal sealed class AppServices
    {
        public FileLogger Log { get; set; }
        public SettingsStore SettingsStore { get; set; }
        public AppSettings Settings { get; set; }
        public DriveMonitor Monitor { get; set; }
    }

    internal static class Program
    {
        private const uint SEM_FAILCRITICALERRORS = 0x0001;
        private static FileLogger _log;

        [STAThread]
        private static void Main()
        {
            // Never show the Windows "There is no disk in the drive" dialog; we report media state ourselves.
            SetErrorMode(SEM_FAILCRITICALERRORS);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += OnThreadException;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            bool createdNew;
            using (var mutex = new Mutex(true, @"Local\HTREASURE.HTISORipper.SingleInstance", out createdNew))
            {
                if (!createdNew)
                {
                    MessageBox.Show("HT ISO Ripper is already running.", AppInfo.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var services = new AppServices();
                _log = new FileLogger(FileLogger.DefaultDirectory);
                services.Log = _log;
                services.SettingsStore = new SettingsStore(SettingsStore.DefaultPath);
                services.Settings = services.SettingsStore.Load();
                _log.ApplyRetention(services.Settings.LogRetentionDays);
                _log.Info("HT ISO Ripper " + AppInfo.Version + " starting on " + Environment.OSVersion.VersionString
                          + (Environment.Is64BitProcess ? " (64-bit process)" : " (32-bit process)")
                          + ", CLR " + Environment.Version);
                services.Monitor = new DriveMonitor(_log, TimeSpan.FromSeconds(2));

                using (var splash = new SplashForm())
                {
                    splash.Show();
                    Application.DoEvents();
                    var main = new MainForm(services);
                    main.Shown += delegate { splash.Close(); };
                    Application.Run(main);
                }

                services.Monitor.Dispose();
                _log.Info("HT ISO Ripper exiting.");
            }
        }

        private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ReportFatal(e.Exception);
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ReportFatal(e.ExceptionObject as Exception);
        }

        private static void ReportFatal(Exception ex)
        {
            if (_log != null) _log.Error("Unhandled exception", ex);
            MessageBox.Show("An unexpected problem occurred. Details have been written to the log file:\n\n"
                            + (_log != null ? _log.CurrentLogFile : "(log unavailable)"),
                AppInfo.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern uint SetErrorMode(uint mode);
    }
}
