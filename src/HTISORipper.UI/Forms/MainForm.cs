using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using HTISORipper.Core;
using HTISORipper.UI.Views;

namespace HTISORipper.UI.Forms
{
    public enum StatusKind { Neutral, Ready, Busy, Warning, Error }

    public partial class MainForm : Form
    {
        private const int WM_DEVICECHANGE = 0x0219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        internal const string WebsiteUrl = "https://www.htreasure.com";

        private readonly AppServices _services;
        private readonly DashboardView _dashboard;
        private StatusKind _statusKind = StatusKind.Neutral;
        private bool _closeWhenIdle;

        internal MainForm(AppServices services)
        {
            _services = services;
            InitializeComponent();
            Icon = Theme.LoadAppIcon();
            headerIcon.Image = Theme.LoadImage("ht_logo_64.png");
            Text = AppInfo.ProductName;

            _dashboard = new DashboardView { Dock = DockStyle.Fill };
            Controls.Add(_dashboard);
            _dashboard.BringToFront();   // fill the space between the menu and the status strip
            _dashboard.Initialize(services);
            _dashboard.StatusChanged += (s, e) => SetStatus(e.Text, e.Kind);
            _dashboard.BusyChanged += Dashboard_BusyChanged;
            _dashboard.DiscChanged += (s, e) => UpdateDriveStatus();

            ApplyTheme();
            SetStatus("Starting", StatusKind.Neutral);
        }

        internal void ApplyTheme()
        {
            ThemeManager.Apply(this);
            _dashboard.ApplyThemeColours();
            SetStatus(statusStripLabel.Text, _statusKind);
        }

        private void SetStatus(string text, StatusKind kind)
        {
            _statusKind = kind;
            statusStripLabel.Text = text;
            ThemePalette p = ThemeManager.Current;
            statusStripLabel.ForeColor = kind == StatusKind.Error ? p.ErrorText : kind == StatusKind.Warning ? p.WarningText : p.Text;
        }

        private void UpdateDriveStatus()
        {
            OpticalDriveInfo drive = _dashboard.CurrentDrive;
            DiscInfo disc = _dashboard.CurrentDisc;
            if (drive == null) statusRightLabel.Text = string.Empty;
            else if (disc == null) statusRightLabel.Text = drive.RootPath + "  " + _dashboard.CurrentStateText;
            else statusRightLabel.Text = drive.RootPath + "  " + disc.MediaTypeName + "  •  " + disc.Filesystem.Describe();
        }

        private void Dashboard_BusyChanged(object sender, EventArgs e)
        {
            bool busy = _dashboard.IsBusy;
            settingsMenuItem.Enabled = !busy;
            chooseOutputMenuItem.Enabled = !busy;
            refreshDrivesMenuItem.Enabled = !busy;
            ejectMenuItem.Enabled = !busy;
            startRippingMenuItem.Enabled = !busy;
            if (!busy && _closeWhenIdle) BeginInvoke(new Action(Close));
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _dashboard.Start();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_dashboard.IsBusy)
            {
                e.Cancel = true;
                if (_closeWhenIdle) return;
                DialogResult answer = MessageBox.Show(this,
                    "A disc is being imaged. Cancel imaging and exit?\n\nThe incomplete image will be deleted.",
                    AppInfo.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (answer == DialogResult.Yes)
                {
                    _closeWhenIdle = true;
                    _dashboard.RequestCancel();
                }
                return;
            }
            base.OnFormClosing(e);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_DEVICECHANGE)
            {
                int evt = m.WParam.ToInt32();
                if (evt == DBT_DEVICEARRIVAL || evt == DBT_DEVICEREMOVECOMPLETE) _services.Monitor.RequestRefresh();
            }
        }

        // ------------------------------------------------------------------ menu: File

        private void StartRippingMenuItem_Click(object sender, EventArgs e) { _dashboard.StartRipping(); }
        private void ChooseOutputMenuItem_Click(object sender, EventArgs e) { _dashboard.ChooseOutputFile(); }
        private void OpenOutputFolderMenuItem_Click(object sender, EventArgs e) { _dashboard.OpenOutputFolder(); }
        private void ExitMenuItem_Click(object sender, EventArgs e) { Close(); }

        // ------------------------------------------------------------------ menu: Tools

        private void RefreshDrivesMenuItem_Click(object sender, EventArgs e) { _dashboard.RefreshDrives(); }
        private void EjectMenuItem_Click(object sender, EventArgs e) { _dashboard.EjectDisc(); }

        private void DiscInfoMenuItem_Click(object sender, EventArgs e)
        {
            var view = new DiscInfoView();
            view.ShowDisc(_dashboard.CurrentDrive, _dashboard.CurrentDisc, _dashboard.CurrentStateText);
            using (var host = new HostForm("Disc Information", view, new System.Drawing.Size(640, 560))) host.ShowDialog(this);
        }

        private void ViewLogMenuItem_Click(object sender, EventArgs e)
        {
            using (var viewer = new LogViewerForm(_services.Log.CurrentLogFile)) viewer.ShowDialog(this);
        }

        private void OpenLogFolderMenuItem_Click(object sender, EventArgs e)
        {
            string dir = _services.Log.LogDirectory;
            if (Directory.Exists(dir)) Process.Start(new ProcessStartInfo("explorer.exe", "\"" + dir + "\"") { UseShellExecute = false });
        }

        private void SettingsMenuItem_Click(object sender, EventArgs e)
        {
            var view = new SettingsView();
            view.Initialize(_services);
            view.SettingsSaved += (s, args) =>
            {
                ThemeManager.SetTheme(_services.Settings.Theme);
                ApplyTheme();
                Form host = view.FindForm();
                if (host != null) ThemeManager.Apply(host);
                _dashboard.ApplySettings();
            };
            using (var host = new HostForm("Settings", view, new System.Drawing.Size(640, 520))) host.ShowDialog(this);
        }

        // ------------------------------------------------------------------ menu: Help

        private void DocumentMenuItem_Click(object sender, EventArgs e)
        {
            string name = (string)((ToolStripItem)sender).Tag;
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(baseDir, "Documentation", name);
            if (!File.Exists(path)) path = Path.Combine(baseDir, name);
            if (!File.Exists(path))
            {
                MessageBox.Show(this, "\"" + name + "\" is not installed with this copy of HT ISO Ripper.", AppInfo.ProductName,
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            try
            {
                // Opens the document in the user's default viewer for .md / .txt files.
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            }
            catch (System.ComponentModel.Win32Exception)
            {
                Process.Start(new ProcessStartInfo("notepad.exe", "\"" + path + "\"") { UseShellExecute = false });
            }
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            var view = new AboutView();
            view.Initialize(_services);
            using (var host = new HostForm("About HT ISO Ripper", view, new System.Drawing.Size(540, 470))) host.ShowDialog(this);
        }

        private void WebsiteLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenWebsite(this);
        }

        internal static void OpenWebsite(IWin32Window owner)
        {
            try
            {
                Process.Start(new ProcessStartInfo(WebsiteUrl) { UseShellExecute = true });
            }
            catch (System.ComponentModel.Win32Exception)
            {
                MessageBox.Show(owner, "Could not open a web browser. The HTREASURE LTD website is " + WebsiteUrl,
                    AppInfo.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

    public sealed class StatusEventArgs : EventArgs
    {
        public StatusEventArgs(string text, StatusKind kind) { Text = text; Kind = kind; }
        public string Text { get; private set; }
        public StatusKind Kind { get; private set; }
    }
}
