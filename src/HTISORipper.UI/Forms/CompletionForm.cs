using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using HTISORipper.Core;
using HTISORipper.DiscEngine;

namespace HTISORipper.UI.Forms
{
    public partial class CompletionForm : Form
    {
        private readonly ImagingResult _result;
        private readonly OpticalDriveInfo _drive;
        private readonly string _logPath;

        public CompletionForm(DiscInfo disc, OpticalDriveInfo drive, ImagingResult result, VerificationReport report, bool ejected, string logPath)
        {
            InitializeComponent();
            Icon = Theme.LoadAppIcon();
            _result = result;
            _drive = drive;
            _logPath = logPath;

            foreach (Button b in new[] { openFolderButton, copyShaButton, viewLogButton, ejectButton, closeButton }) Theme.StyleSecondary(b);
            Theme.StylePrimary(ripAnotherButton);

            CompletionStatus status = CompletionStatus.Evaluate(result, report);
            headlineLabel.Text = status.Headline;
            headlinePanel.BackColor = status.Color;
            detailLabel.Text = status.Detail;

            string output = result.OutputPath;
            AddRow("Disc name", disc.Label);
            AddRow("Disc type", disc.MediaTypeName + "  (" + disc.Filesystem.Describe() + ")");
            AddRow("Output filename", output != null ? Path.GetFileName(output) : "(no image kept)");
            AddRow("Output location", output != null ? Path.GetDirectoryName(output) : "-");
            AddRow("Image size", result.ImageBytes > 0 ? Format.BytesWithExact(result.ImageBytes) : "-");
            AddRow("Elapsed time", Format.Duration(result.Elapsed));
            AddRow("Average read speed", Format.Speed(result.AverageBytesPerSecond, disc.Family));
            AddRow("Verification", report != null ? report.Summary : "Not performed");
            if (report != null && report.StructureSummary != null) AddRow("Structure", report.StructureSummary);
            AddRow("Read errors", result.UnreadableLbas.Count.ToString(CultureInfo.CurrentCulture)
                + (result.RecoveredSectors > 0 ? "  (" + result.RecoveredSectors + " sector(s) recovered by retry)" : string.Empty));
            if (result.RunOutSectorsExcluded > 0)
                AddRow("Run-out sectors", result.RunOutSectorsExcluded + " unreadable CD-R run-out sector(s) after the file system were excluded");
            if (result.BadSectorReportPath != null) AddRow("Bad-sector report", result.BadSectorReportPath);
            if (ejected) AddRow("Disc", "Ejected");

            shaTextBox.Text = string.IsNullOrEmpty(result.Sha256) || output == null ? "-" : result.Sha256;
            copyShaButton.Enabled = output != null && !string.IsNullOrEmpty(result.Sha256);
            openFolderButton.Enabled = output != null && File.Exists(output);
            ejectButton.Enabled = !ejected;
            detailsListView.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        public bool RipAnother { get; private set; }

        private void AddRow(string name, string value)
        {
            var item = new ListViewItem(name);
            item.SubItems.Add(value ?? "-");
            detailsListView.Items.Add(item);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (detailsListView != null && detailsListView.Columns.Count == 2)
                detailsListView.Columns[1].Width = Math.Max(100, detailsListView.ClientSize.Width - detailsListView.Columns[0].Width - 4);
        }

        private void OpenFolderButton_Click(object sender, EventArgs e)
        {
            if (_result.OutputPath == null || !File.Exists(_result.OutputPath)) return;
            // explorer.exe with /select is the documented way to open a folder with the file highlighted.
            Process.Start(new ProcessStartInfo("explorer.exe", "/select,\"" + _result.OutputPath + "\"") { UseShellExecute = false });
        }

        private void CopyShaButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_result.Sha256)) return;
            Clipboard.SetText(_result.Sha256);
            copyShaButton.Text = "Copied";
        }

        private void ViewLogButton_Click(object sender, EventArgs e)
        {
            using (var viewer = new LogViewerForm(_logPath)) viewer.ShowDialog(this);
        }

        private async void EjectButton_Click(object sender, EventArgs e)
        {
            ejectButton.Enabled = false;
            string letter = _drive.DriveLetter;
            bool ok = await Task.Run(() =>
            {
                try
                {
                    using (var device = OpticalDevice.Open(letter, true))
                    {
                        int error;
                        return device.Eject(out error);
                    }
                }
                catch (DeviceAccessException) { return false; }
            });
            if (!ok)
            {
                ejectButton.Enabled = true;
                MessageBox.Show(this, "The drive did not eject the disc.", AppInfo.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void RipAnotherButton_Click(object sender, EventArgs e)
        {
            RipAnother = true;
            Close();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
