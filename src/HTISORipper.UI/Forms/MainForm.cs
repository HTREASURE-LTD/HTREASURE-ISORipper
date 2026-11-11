using System;
using System.Drawing;
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

        private readonly AppServices _services;
        private readonly DashboardView _dashboard;
        private readonly DiscInfoView _discInfo;
        private readonly SettingsView _settings;
        private readonly AboutView _about;
        private Button[] _navButtons;
        private bool _closeWhenIdle;

        internal MainForm(AppServices services)
        {
            _services = services;
            InitializeComponent();
            Icon = Theme.LoadAppIcon();
            logoPictureBox.Image = Theme.LoadImage("ht_logo_64.png");
            Text = AppInfo.ProductName + " - " + AppInfo.Publisher;

            _dashboard = new DashboardView { Dock = DockStyle.Fill };
            _discInfo = new DiscInfoView { Dock = DockStyle.Fill };
            _settings = new SettingsView { Dock = DockStyle.Fill };
            _about = new AboutView { Dock = DockStyle.Fill };
            contentPanel.Controls.Add(_dashboard);
            contentPanel.Controls.Add(_discInfo);
            contentPanel.Controls.Add(_settings);
            contentPanel.Controls.Add(_about);

            _navButtons = new[] { navDashboardButton, navDiscInfoButton, navSettingsButton, navAboutButton };
            foreach (Button b in _navButtons) b.Click += NavButton_Click;

            _dashboard.Initialize(services);
            _settings.Initialize(services);
            _about.Initialize(services);
            _dashboard.DiscChanged += (s, e) => _discInfo.ShowDisc(_dashboard.CurrentDrive, _dashboard.CurrentDisc, _dashboard.CurrentStateText);
            _dashboard.StatusChanged += Dashboard_StatusChanged;
            _dashboard.BusyChanged += Dashboard_BusyChanged;
            _settings.SettingsSaved += (s, e) => _dashboard.ApplySettings();

            ShowView(navDashboardButton);
            SetStatus("Starting", StatusKind.Neutral);
        }

        private void NavButton_Click(object sender, EventArgs e)
        {
            ShowView((Button)sender);
        }

        private void ShowView(Button selected)
        {
            foreach (Button b in _navButtons)
            {
                bool active = b == selected;
                b.ForeColor = active ? Theme.Navy : Theme.Muted;
                b.Font = new Font(Theme.FontName, 9.75F, active ? FontStyle.Bold : FontStyle.Regular);
                b.FlatAppearance.BorderSize = 0;
                b.BackColor = active ? Theme.InfoTint : Theme.White;
            }
            _dashboard.Visible = selected == navDashboardButton;
            _discInfo.Visible = selected == navDiscInfoButton;
            _settings.Visible = selected == navSettingsButton;
            _about.Visible = selected == navAboutButton;
            if (selected == navSettingsButton) _settings.LoadFromSettings();
        }

        private void Dashboard_StatusChanged(object sender, StatusEventArgs e)
        {
            SetStatus(e.Text, e.Kind);
        }

        private void Dashboard_BusyChanged(object sender, EventArgs e)
        {
            navSettingsButton.Enabled = !_dashboard.IsBusy;
            if (!_dashboard.IsBusy && _closeWhenIdle) BeginInvoke(new Action(Close));
        }

        private void SetStatus(string text, StatusKind kind)
        {
            Color fg, bg;
            switch (kind)
            {
                case StatusKind.Ready: fg = Theme.Success; bg = Theme.SuccessTint; break;
                case StatusKind.Busy: fg = Theme.Blue; bg = Theme.InfoTint; break;
                case StatusKind.Warning: fg = Color.FromArgb(0xB2, 0x5E, 0x00); bg = Theme.WarningTint; break;
                case StatusKind.Error: fg = Theme.Error; bg = Theme.ErrorTint; break;
                default: fg = Theme.Muted; bg = Theme.LightGrey; break;
            }
            statusPill.SetState(text, fg, bg);
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
                if (_closeWhenIdle) { e.Cancel = true; return; }
                DialogResult answer = MessageBox.Show(this,
                    "A disc is being imaged. Cancel imaging and exit?\n\nThe incomplete image will be deleted.",
                    AppInfo.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                e.Cancel = true;
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
                if (evt == DBT_DEVICEARRIVAL || evt == DBT_DEVICEREMOVECOMPLETE)
                    _services.Monitor.RequestRefresh();
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
