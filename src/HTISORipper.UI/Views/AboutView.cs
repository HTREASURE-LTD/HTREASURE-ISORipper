using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using HTISORipper.Core;
using HTISORipper.UI.Forms;

namespace HTISORipper.UI.Views
{
    public partial class AboutView : UserControl
    {
        private AppServices _services;

        public AboutView()
        {
            InitializeComponent();
            Theme.StyleSecondary(viewLogButton);
            Theme.StyleSecondary(openLogFolderButton);
            logoPictureBox.Image = Theme.LoadImage("ht_logo.png");
            productLabel.Text = AppInfo.ProductName;
            taglineLabel.Text = AppInfo.Tagline;
            publisherLabel.Text = "Developed by " + AppInfo.Publisher;
            versionLabel.Text = "Version " + AppInfo.Version + "  (" + AppInfo.RuntimeBuild + ", "
                                + (Environment.Is64BitProcess ? "64-bit" : "32-bit") + ")";
            copyrightLabel.Text = AppInfo.Copyright;
            noticeLabel.Text = "HT ISO Ripper reads CDs and DVDs strictly read-only. It does not bypass DRM, encryption or copy protection, "
                + "collects no telemetry and needs no internet connection.";
        }

        internal void Initialize(AppServices services)
        {
            _services = services;
        }

        private void WebsiteLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MainForm.OpenWebsite(FindForm());
        }

        private void ViewLogButton_Click(object sender, EventArgs e)
        {
            if (_services == null) return;
            using (var viewer = new LogViewerForm(_services.Log.CurrentLogFile)) viewer.ShowDialog(FindForm());
        }

        private void OpenLogFolderButton_Click(object sender, EventArgs e)
        {
            if (_services == null) return;
            string dir = _services.Log.LogDirectory;
            if (Directory.Exists(dir)) Process.Start(new ProcessStartInfo("explorer.exe", "\"" + dir + "\"") { UseShellExecute = false });
        }
    }
}
