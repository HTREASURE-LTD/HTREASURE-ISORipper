using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace HTISORipper.UI.Forms
{
    /// <summary>Read-only viewer for the current log file (opened with sharing so logging continues).</summary>
    public partial class LogViewerForm : Form
    {
        private readonly string _path;

        public LogViewerForm(string path)
        {
            InitializeComponent();
            Icon = Theme.LoadAppIcon();
            Theme.StyleSecondary(openFolderButton);
            Theme.StyleSecondary(closeButton);
            _path = path;
            pathLabel.Text = path;
            LoadLog();
            ThemeManager.Apply(this);
        }

        private void LoadLog()
        {
            try
            {
                using (var stream = new FileStream(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete))
                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    logTextBox.Text = reader.ReadToEnd();
                }
                logTextBox.SelectionStart = logTextBox.TextLength;
                logTextBox.ScrollToCaret();
            }
            catch (IOException ex) { logTextBox.Text = "The log file could not be opened: " + ex.Message; }
            catch (UnauthorizedAccessException ex) { logTextBox.Text = "The log file could not be opened: " + ex.Message; }
        }

        private void OpenFolderButton_Click(object sender, EventArgs e)
        {
            string dir = Path.GetDirectoryName(_path);
            if (Directory.Exists(dir)) Process.Start(new ProcessStartInfo("explorer.exe", "\"" + dir + "\"") { UseShellExecute = false });
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
