using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using HTISORipper.Core;
using HTISORipper.Logging;

namespace HTISORipper.UI.Views
{
    public partial class SettingsView : UserControl
    {
        private AppServices _services;

        public event EventHandler SettingsSaved;

        public SettingsView()
        {
            InitializeComponent();
            Theme.StylePrimary(saveButton);
            Theme.StyleSecondary(defaultsButton);
            Theme.StyleSecondary(browseButton);

            verifyComboBox.Items.Add(new ComboItem<VerificationMode>("Full: re-read disc and compare (recommended)", VerificationMode.FullSourceCompare));
            verifyComboBox.Items.Add(new ComboItem<VerificationMode>("Structural: re-hash file + check ISO/UDF", VerificationMode.Structural));
            verifyComboBox.Items.Add(new ComboItem<VerificationMode>("Off: size check only", VerificationMode.None));
            speedComboBox.Items.Add(new ComboItem<ReadSpeedPreference>("Automatic (drive default)", ReadSpeedPreference.Automatic));
            speedComboBox.Items.Add(new ComboItem<ReadSpeedPreference>("Quiet (CD 8x / DVD 4x)", ReadSpeedPreference.Quiet));
            speedComboBox.Items.Add(new ComboItem<ReadSpeedPreference>("Balanced (CD 24x / DVD 8x)", ReadSpeedPreference.Balanced));
            speedComboBox.Items.Add(new ComboItem<ReadSpeedPreference>("Maximum", ReadSpeedPreference.Maximum));
            policyComboBox.Items.Add(new ComboItem<ErrorPolicy>("Stop imaging (recommended)", ErrorPolicy.AbortOnUnreadableSector));
            policyComboBox.Items.Add(new ComboItem<ErrorPolicy>("Continue, save as INCOMPLETE image", ErrorPolicy.ContinueAndMarkIncomplete));
            blockComboBox.Items.Add(new ComboItem<int>("16 sectors (32 KB) - most compatible", 16));
            blockComboBox.Items.Add(new ComboItem<int>("32 sectors (64 KB) - default", 32));
            blockComboBox.Items.Add(new ComboItem<int>("64 sectors (128 KB) - fastest on modern drives", 64));
        }

        internal void Initialize(AppServices services)
        {
            _services = services;
            LoadFromSettings();
        }

        public void LoadFromSettings()
        {
            if (_services == null) return;
            Show(_services.Settings);
            statusLabel.Text = "Settings file: " + _services.SettingsStore.FilePath;
        }

        private void Show(AppSettings s)
        {
            outputDirTextBox.Text = s.DefaultOutputDirectory;
            Select(verifyComboBox, s.VerificationMode);
            Select(speedComboBox, s.ReadSpeed);
            Select(policyComboBox, s.ErrorPolicy);
            Select(blockComboBox, s.ReadBlockSectors);
            retriesUpDown.Value = Math.Max(retriesUpDown.Minimum, Math.Min(retriesUpDown.Maximum, s.MaxReadRetries));
            retentionUpDown.Value = Math.Max(retentionUpDown.Minimum, Math.Min(retentionUpDown.Maximum, s.LogRetentionDays));
            ejectCheckBox.Checked = s.EjectAfterSuccess;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            string dir = outputDirTextBox.Text.Trim();
            string problem = PathUtil.ValidateDirectory(dir);
            if (problem != null)
            {
                MessageBox.Show(this, problem, AppInfo.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AppSettings s = _services.Settings;
            s.DefaultOutputDirectory = dir;
            s.VerificationMode = Value<VerificationMode>(verifyComboBox);
            s.ReadSpeed = Value<ReadSpeedPreference>(speedComboBox);
            s.ErrorPolicy = Value<ErrorPolicy>(policyComboBox);
            s.ReadBlockSectors = Value<int>(blockComboBox);
            s.MaxReadRetries = (int)retriesUpDown.Value;
            s.LogRetentionDays = (int)retentionUpDown.Value;
            s.EjectAfterSuccess = ejectCheckBox.Checked;
            try
            {
                _services.SettingsStore.Save(s);
                _services.Log.Info("Settings saved.");
                statusLabel.Text = "Settings saved.";
                statusLabel.ForeColor = Theme.Success;
                var handler = SettingsSaved;
                if (handler != null) handler(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                if (!(ex is IOException) && !(ex is UnauthorizedAccessException) && !(ex is InvalidOperationException)) throw;
                _services.Log.Error("Saving settings failed", ex);
                statusLabel.Text = "Settings could not be saved: " + ex.Message;
                statusLabel.ForeColor = Theme.Error;
            }
        }

        private void DefaultsButton_Click(object sender, EventArgs e)
        {
            Show(new AppSettings());
            statusLabel.Text = "Defaults restored. Click Save to keep them.";
            statusLabel.ForeColor = Theme.Muted;
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Default folder for new disc images";
                if (Directory.Exists(outputDirTextBox.Text)) dialog.SelectedPath = outputDirTextBox.Text;
                if (dialog.ShowDialog(this) == DialogResult.OK) outputDirTextBox.Text = dialog.SelectedPath;
            }
        }

        private static void Select<T>(ComboBox combo, T value)
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                var item = combo.Items[i] as ComboItem<T>;
                if (item != null && EqualityComparer<T>.Default.Equals(item.Value, value)) { combo.SelectedIndex = i; return; }
            }
            if (combo.Items.Count > 0) combo.SelectedIndex = 0;
        }

        private static T Value<T>(ComboBox combo)
        {
            var item = combo.SelectedItem as ComboItem<T>;
            return item != null ? item.Value : default(T);
        }
    }
}
