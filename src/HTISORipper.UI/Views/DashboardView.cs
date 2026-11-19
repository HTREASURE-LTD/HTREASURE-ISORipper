using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HTISORipper.Core;
using HTISORipper.DiscEngine;
using HTISORipper.Imaging;
using HTISORipper.Logging;
using HTISORipper.UI.Forms;
using HTISORipper.Verification;

namespace HTISORipper.UI.Views
{
    public partial class DashboardView : UserControl
    {
        private enum MessageTone { None, Dim, Warning, Error, Success }

        private AppServices _services;
        private List<OpticalDriveInfo> _drives = new List<OpticalDriveInfo>();
        private OpticalDriveInfo _currentDrive;
        private DiscInfo _currentDisc;
        private string _currentStateText = "Detecting drives";
        private int _inspectSequence;
        private bool _busy;
        private bool _suppressPathEvents;
        private bool _pathEditedByUser;
        private CancellationTokenSource _cts;
        private Stopwatch _ripClock;
        private string _lastOutputPath;
        private MessageTone _sourceTone = MessageTone.None;
        private MessageTone _progressTone = MessageTone.None;

        public event EventHandler DiscChanged;
        public event EventHandler<StatusEventArgs> StatusChanged;
        public event EventHandler BusyChanged;

        public DashboardView()
        {
            InitializeComponent();

            formatComboBox.Items.Add(new ComboItem<ImageFormat>("Standard ISO (.iso)", ImageFormat.Iso));
            formatComboBox.SelectedIndex = 0;
            compressionComboBox.Items.Add(new ComboItem<int>("None (standard, uncompressed ISO)", 0));
            compressionComboBox.SelectedIndex = 0;
            toolTip.SetToolTip(compressionComboBox, "Lossless .iso.gz / .iso.xz output is planned for a later version. Only fully implemented formats are offered.");
            toolTip.SetToolTip(formatComboBox, "BIN/CUE for audio and mixed-mode CDs is planned for a later version.");

            verifyComboBox.Items.Add(new ComboItem<VerificationMode>("Full: re-read disc and compare", VerificationMode.FullSourceCompare));
            verifyComboBox.Items.Add(new ComboItem<VerificationMode>("Structural: re-hash file + check ISO/UDF", VerificationMode.Structural));
            verifyComboBox.Items.Add(new ComboItem<VerificationMode>("Off: size check only", VerificationMode.None));
            speedComboBox.Items.Add(new ComboItem<ReadSpeedPreference>("Automatic", ReadSpeedPreference.Automatic));
            speedComboBox.Items.Add(new ComboItem<ReadSpeedPreference>("Quiet", ReadSpeedPreference.Quiet));
            speedComboBox.Items.Add(new ComboItem<ReadSpeedPreference>("Balanced", ReadSpeedPreference.Balanced));
            speedComboBox.Items.Add(new ComboItem<ReadSpeedPreference>("Maximum", ReadSpeedPreference.Maximum));
            policyComboBox.Items.Add(new ComboItem<ErrorPolicy>("Stop imaging (recommended)", ErrorPolicy.AbortOnUnreadableSector));
            policyComboBox.Items.Add(new ComboItem<ErrorPolicy>("Continue, save as INCOMPLETE image", ErrorPolicy.ContinueAndMarkIncomplete));
            toolTip.SetToolTip(policyComboBox, "\"Continue\" fills unreadable sectors with zeros, names the file *.INCOMPLETE.iso and writes a bad-sector list. Such an image is never reported as verified.");
            toolTip.SetToolTip(verifyComboBox, "Only \"Full\" verification can confirm that the image matches the disc.");
            toolTip.SetToolTip(speedComboBox, "Not every drive supports speed control. Quiet/Balanced reduce noise and can help with marginal discs.");
            toolTip.SetToolTip(footerLabel, "HT ISO Ripper opens the drive read-only. The source disc is never modified.");
        }

        public bool IsBusy { get { return _busy; } }
        public DiscInfo CurrentDisc { get { return _currentDisc; } }
        public OpticalDriveInfo CurrentDrive { get { return _currentDrive; } }
        public string CurrentStateText { get { return _currentStateText; } }

        internal void Initialize(AppServices services)
        {
            _services = services;
            ApplySettings();
            _services.Monitor.DriveStateChanged += Monitor_DriveStateChanged;
            _services.Monitor.DriveListChanged += Monitor_DriveListChanged;
            UpdateControls();
        }

        /// <summary>Copies saved defaults into the per-rip options.</summary>
        public void ApplySettings()
        {
            AppSettings s = _services.Settings;
            SelectItem(verifyComboBox, s.VerificationMode);
            SelectItem(speedComboBox, s.ReadSpeed);
            SelectItem(policyComboBox, s.ErrorPolicy);
            retriesUpDown.Value = Math.Max(retriesUpDown.Minimum, Math.Min(retriesUpDown.Maximum, s.MaxReadRetries));
            ejectCheckBox.Checked = s.EjectAfterSuccess;
            if (!_pathEditedByUser) SuggestOutputPath();
        }

        /// <summary>Re-applies state colours after ThemeManager has re-coloured the form.</summary>
        public void ApplyThemeColours()
        {
            ApplyTone(sourceMessageLabel, _sourceTone);
            ApplyTone(progressStatusLabel, _progressTone);
            ThemeManager.UpdatePrimary(startButton);
        }

        public void Start() { LoadDrivesAsync(); }
        public void RefreshDrives() { if (!_busy) LoadDrivesAsync(); }
        public void StartRipping() { StartButton_Click(this, EventArgs.Empty); }
        public void ChooseOutputFile() { if (!_busy) BrowseButton_Click(this, EventArgs.Empty); }
        public void EjectDisc() { EjectButton_Click(this, EventArgs.Empty); }

        public void OpenOutputFolder()
        {
            string target = _lastOutputPath;
            string dir = null;
            try { dir = Path.GetDirectoryName(target ?? outputPathTextBox.Text.Trim()); } catch (ArgumentException) { }
            if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir)) dir = _services.Settings.DefaultOutputDirectory;
            if (!Directory.Exists(dir))
            {
                MessageBox.Show(FindForm(), "The output folder does not exist yet. It is created when the first image is saved.",
                    AppInfo.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string args = target != null && File.Exists(target) ? "/select,\"" + target + "\"" : "\"" + dir + "\"";
            Process.Start(new ProcessStartInfo("explorer.exe", args) { UseShellExecute = false });
        }

        public void RequestCancel()
        {
            var cts = _cts;
            if (cts != null && !cts.IsCancellationRequested)
            {
                cts.Cancel();
                progressStatusLabel.Text = "Cancelling...";
                cancelButton.Enabled = false;
            }
        }

        // ------------------------------------------------------------------ drives

        private async void LoadDrivesAsync()
        {
            ILogger log = _services.Log;
            List<OpticalDriveInfo> drives;
            try
            {
                drives = await Task.Run(() => DriveEnumerator.GetOpticalDrives(log));
            }
            catch (Exception ex)
            {
                log.Error("Drive enumeration failed", ex);
                drives = new List<OpticalDriveInfo>();
            }

            string previous = _currentDrive != null ? _currentDrive.DriveLetter : null;
            _drives = drives;
            driveComboBox.BeginUpdate();
            driveComboBox.Items.Clear();
            foreach (var d in drives) driveComboBox.Items.Add(d);
            driveComboBox.EndUpdate();
            log.Info("Optical drives: " + (drives.Count == 0 ? "none" : string.Join("; ", drives.Select(d => d.ToString()).ToArray())));

            if (drives.Count == 0)
            {
                _currentDrive = null;
                driveComboBox.Items.Add("No optical drive detected");
                driveComboBox.SelectedIndex = 0;
                SetDisc(null, "No optical drive detected");
                discDetailLabel.Text = "No CD/DVD drive was found. Connect a drive (USB drives are supported) and it will appear automatically.";
                RaiseStatus("No optical drive detected", StatusKind.Warning);
            }
            else
            {
                int index = previous != null ? drives.FindIndex(d => d.DriveLetter == previous) : -1;
                driveComboBox.SelectedIndex = index >= 0 ? index : 0;
            }
            _services.Monitor.Start(drives.Select(d => d.DriveLetter));
            UpdateControls();
        }

        private void Monitor_DriveListChanged(object sender, EventArgs e)
        {
            if (IsHandleCreated) BeginInvoke(new Action(LoadDrivesAsync));
        }

        private void Monitor_DriveStateChanged(object sender, DriveStateChangedEventArgs e)
        {
            if (!IsHandleCreated) return;
            BeginInvoke(new Action(() =>
            {
                if (_busy || _currentDrive == null || !string.Equals(e.DriveLetter, _currentDrive.DriveLetter, StringComparison.OrdinalIgnoreCase)) return;
                ApplyMediaState(e.NewState);
            }));
        }

        private void DriveComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentDrive = driveComboBox.SelectedItem as OpticalDriveInfo;
            if (_currentDrive == null) return;
            SetDisc(null, "Checking drive...");
            discDetailLabel.Text = "Checking drive...";
            DriveMediaState known = _services.Monitor.GetState(_currentDrive.DriveLetter);
            if (known == DriveMediaState.Unknown) _services.Monitor.RequestRefresh();   // result arrives via DriveStateChanged
            else ApplyMediaState(known);
        }

        private void ApplyMediaState(DriveMediaState state)
        {
            switch (state)
            {
                case DriveMediaState.Ready:
                    InspectCurrentDriveAsync();
                    break;
                case DriveMediaState.NoDisc:
                    SetDisc(null, "Waiting for disc");
                    discDetailLabel.Text = "Disc status: waiting for disc. Insert a CD or DVD.";
                    RaiseStatus("Waiting for disc", StatusKind.Neutral);
                    break;
                case DriveMediaState.NotReady:
                    SetDisc(null, "Disc not ready");
                    discDetailLabel.Text = "Disc status: the drive is reading the disc. Please wait...";
                    RaiseStatus("Disc not ready", StatusKind.Busy);
                    break;
                case DriveMediaState.AccessDenied:
                    SetDisc(null, "Device access denied");
                    discDetailLabel.Text = "Windows denied access to this drive. A group policy may restrict CD/DVD access for your account.";
                    RaiseStatus("Device access denied", StatusKind.Error);
                    break;
                default:
                    SetDisc(null, "Drive not responding");
                    discDetailLabel.Text = "The drive did not respond. Check the connection and try again.";
                    RaiseStatus("Drive not responding", StatusKind.Error);
                    break;
            }
        }

        private async void InspectCurrentDriveAsync()
        {
            OpticalDriveInfo drive = _currentDrive;
            if (drive == null) return;
            int sequence = ++_inspectSequence;
            discDetailLabel.Text = "Reading disc information...";
            RaiseStatus("Reading disc information", StatusKind.Busy);

            DiscInfo disc = null;
            DriveMediaState state = DriveMediaState.Unknown;
            string error = null;
            ILogger log = _services.Log;
            try
            {
                disc = await Task.Run(() => { DriveMediaState s; var d = DiscInspector.Inspect(drive, log, out s); state = s; return d; });
            }
            catch (DeviceAccessException ex)
            {
                log.Error("Disc inspection failed on " + drive.RootPath, ex);
                error = ex.IsAccessDenied
                    ? "Windows denied access to this drive. A group policy may restrict CD/DVD access for your account."
                    : "The disc could not be read. It may be unreadable or the drive may have stopped responding.";
            }
            catch (Exception ex)
            {
                log.Error("Disc inspection failed on " + drive.RootPath, ex);
                error = "The disc information could not be read. Details were written to the log.";
            }

            if (sequence != _inspectSequence || drive != _currentDrive || _busy) return;   // superseded

            if (error != null)
            {
                SetDisc(null, "Unreadable disc");
                discDetailLabel.Text = error;
                RaiseStatus("Unreadable disc", StatusKind.Error);
                return;
            }
            if (disc == null)
            {
                ApplyMediaState(state == DriveMediaState.Ready ? DriveMediaState.Error : state);
                return;
            }

            SetDisc(disc, disc.CanCreateIso ? "Disc ready" : "Unsupported disc");
            discDetailLabel.Text = disc.CanCreateIso
                ? "Disc ready. Check the options and output file, then click Start Ripping."
                : "This disc cannot be saved as a standard ISO image.";
            RaiseStatus(disc.CanCreateIso ? "Disc ready" : "Unsupported disc", disc.CanCreateIso ? StatusKind.Ready : StatusKind.Warning);
            if (!_pathEditedByUser) SuggestOutputPath();
        }

        private void SetDisc(DiscInfo disc, string stateText)
        {
            _currentDisc = disc;
            _currentStateText = stateText;

            if (disc == null)
            {
                labelValue.Text = typeValue.Text = fsValue.Text = sizeValue.Text = tracksValue.Text = formatSupportValue.Text = "--";
            }
            else
            {
                labelValue.Text = disc.Label;
                typeValue.Text = disc.MediaTypeName;
                fsValue.Text = disc.Filesystem.Describe();
                sizeValue.Text = Format.BytesWithExact(disc.ImageBytes);
                tracksValue.Text = disc.SessionCount + " / " + disc.Tracks.Count
                    + (disc.AudioTrackCount > 0 ? " (" + disc.AudioTrackCount + " audio)" : string.Empty);
                formatSupportValue.Text = disc.CanCreateIso ? "ISO" : disc.RecommendedFormat == ImageFormat.BinCue ? "BIN/CUE needed (not yet supported)" : "Not supported";
            }

            if (disc != null && !disc.CanCreateIso) ShowSourceMessage(disc.UnsupportedReason, MessageTone.Error);
            else if (disc != null && disc.Warnings.Count > 0) ShowSourceMessage(string.Join(" ", disc.Warnings.ToArray()), MessageTone.Warning);
            else ShowSourceMessage(null, MessageTone.None);

            UpdateControls();
            var handler = DiscChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private void ShowSourceMessage(string text, MessageTone tone)
        {
            _sourceTone = tone;
            sourceMessageLabel.Text = text ?? string.Empty;
            sourceMessageLabel.Visible = !string.IsNullOrEmpty(text);
            toolTip.SetToolTip(sourceMessageLabel, text ?? string.Empty);
            ApplyTone(sourceMessageLabel, tone);
        }

        private static void ApplyTone(Label label, MessageTone tone)
        {
            ThemePalette p = ThemeManager.Current;
            switch (tone)
            {
                case MessageTone.Error: label.ForeColor = p.ErrorText; break;
                case MessageTone.Warning: label.ForeColor = p.WarningText; break;
                case MessageTone.Success: label.ForeColor = p.SuccessText; break;
                case MessageTone.Dim: label.ForeColor = p.DimText; break;
                default: label.ForeColor = p.Text; break;
            }
        }

        // ------------------------------------------------------------------ output path

        private void SuggestOutputPath()
        {
            string directory = null;
            string current = outputPathTextBox.Text.Trim();
            if (current.Length > 0)
            {
                try { directory = Path.GetDirectoryName(current); } catch (ArgumentException) { directory = null; }
            }
            if (string.IsNullOrEmpty(directory)) directory = _services.Settings.DefaultOutputDirectory;
            string name = PathUtil.SanitizeBaseName(_currentDisc != null ? _currentDisc.Label : "DISC") + ".iso";
            _suppressPathEvents = true;
            outputPathTextBox.Text = Path.Combine(directory, name);
            _suppressPathEvents = false;
        }

        private void OutputPathTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!_suppressPathEvents) _pathEditedByUser = true;
        }

        private void BrowseButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new SaveFileDialog())
            {
                dialog.Title = "Save disc image as";
                dialog.Filter = "ISO disc image (*.iso)|*.iso";
                dialog.DefaultExt = "iso";
                dialog.AddExtension = true;
                dialog.OverwritePrompt = true;
                dialog.CheckPathExists = true;
                try
                {
                    string current = outputPathTextBox.Text.Trim();
                    string dir = Path.GetDirectoryName(current);
                    if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir)) dialog.InitialDirectory = dir;
                    else if (Directory.Exists(_services.Settings.DefaultOutputDirectory)) dialog.InitialDirectory = _services.Settings.DefaultOutputDirectory;
                    dialog.FileName = Path.GetFileName(current);
                }
                catch (ArgumentException) { }

                if (dialog.ShowDialog(FindForm()) == DialogResult.OK)
                {
                    _suppressPathEvents = true;
                    outputPathTextBox.Text = dialog.FileName;
                    _suppressPathEvents = false;
                    _pathEditedByUser = true;
                }
            }
        }

        // ------------------------------------------------------------------ ripping

        private async void StartButton_Click(object sender, EventArgs e)
        {
            if (_busy || _currentDisc == null || _currentDrive == null || !_currentDisc.CanCreateIso) return;

            string path = outputPathTextBox.Text.Trim();
            string problem = ValidateOutputPath(ref path);
            if (problem != null)
            {
                MessageBox.Show(FindForm(), problem, AppInfo.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                outputPathTextBox.Focus();
                return;
            }

            bool overwrite = false;
            if (File.Exists(path))
            {
                DialogResult answer = MessageBox.Show(FindForm(), "\"" + path + "\" already exists.\n\nReplace it? The existing file is only replaced after the new image has been read completely.",
                    AppInfo.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (answer != DialogResult.Yes) return;
                overwrite = true;
            }

            await RipAsync(_currentDrive, _currentDisc, path, overwrite);
        }

        private static string ValidateOutputPath(ref string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return "Choose where to save the ISO image.";
            string directory, name;
            try
            {
                directory = Path.GetDirectoryName(path);
                name = Path.GetFileName(path);
            }
            catch (ArgumentException) { return "The output path is not valid."; }
            string problem = PathUtil.ValidateDirectory(directory) ?? PathUtil.ValidateFileName(name);
            if (problem != null) return problem;
            if (!name.EndsWith(".iso", StringComparison.OrdinalIgnoreCase)) path += ".iso";
            if (path.EndsWith(".INCOMPLETE.iso", StringComparison.OrdinalIgnoreCase))
                return "Choose a name that does not end in .INCOMPLETE.iso (that suffix marks damaged images).";
            return null;
        }

        private async Task RipAsync(OpticalDriveInfo drive, DiscInfo disc, string path, bool overwrite)
        {
            var mode = SelectedValue<VerificationMode>(verifyComboBox);
            var speed = SelectedValue<ReadSpeedPreference>(speedComboBox);
            var policy = SelectedValue<ErrorPolicy>(policyComboBox);
            bool eject = ejectCheckBox.Checked;
            int blockSectors = _services.Settings.ReadBlockSectors;
            FileLogger log = _services.Log;

            var options = new ImagingOptions
            {
                OutputPath = path,
                AllowOverwrite = overwrite,
                TotalSectors = disc.ImageSectors,
                MinimumRequiredSectors = disc.MinimumFilesystemSectors,
                RunOutToleranceSectors = disc.Family == MediaFamily.CD ? 2 : 0,
                BlockSectors = blockSectors,
                RetryCount = (int)retriesUpDown.Value,
                ErrorPolicy = policy,
                SourceDescription = drive + " | " + disc.Label + " | " + disc.MediaTypeName + " | " + disc.Filesystem.Describe()
            };

            _cts = new CancellationTokenSource();
            CancellationToken token = _cts.Token;
            SetBusy(true, drive);
            RaiseStatus("Ripping " + disc.Label, StatusKind.Busy);
            ResetProgress("Preparing...");
            _ripClock = Stopwatch.StartNew();

            var progress = new Progress<ImagingProgress>(p => ShowProgress(p, disc.Family));
            ImagingResult result = null;
            VerificationReport report = null;
            bool ejected = false;
            string fatal = null;

            try
            {
                await Task.Run(() =>
                {
                    using (OpticalSectorSource source = OpticalSectorSource.Open(drive.DriveLetter, blockSectors))
                    {
                        if (!source.Device.SetMediaLock(true)) log.Warn("Tray lock not supported; the disc can be ejected during imaging.");
                        ushort? kbps = SpeedControl.KilobytesPerSecond(speed, disc.Family);
                        if (kbps.HasValue)
                        {
                            int speedError;
                            if (source.Device.TrySetReadSpeed(kbps.Value, out speedError)) log.Info("Read speed set to " + kbps.Value + " KB/s (" + speed + ")");
                            else log.Info("Drive does not support speed control (Win32 " + speedError + "); continuing at drive default.");
                        }

                        result = new ImagingEngine(log).Run(source, options, progress, token);

                        if (result.Outcome == ImagingOutcome.Success)
                        {
                            report = new ImageVerifier(log) { BlockSectors = blockSectors }.Verify(
                                result.OutputPath, result.ImageBytes, result.Sha256, mode,
                                mode == VerificationMode.FullSourceCompare ? source : null, progress, token);
                        }

                        if (kbps.HasValue) { int ignored; source.Device.TrySetReadSpeed(0xFFFF, out ignored); }
                        source.Device.SetMediaLock(false);

                        bool good = result.Outcome == ImagingOutcome.Success && report != null && report.Level != VerificationLevel.Failed && !report.Cancelled;
                        if (eject && good)
                        {
                            int ejectError;
                            ejected = source.Device.Eject(out ejectError);
                            if (!ejected) log.Warn("Eject failed (Win32 " + ejectError + ")");
                        }
                    }
                });
            }
            catch (DeviceAccessException ex)
            {
                log.Error("Could not open drive for imaging", ex);
                fatal = ex.IsAccessDenied
                    ? "Windows denied access to the optical drive. A group policy may restrict CD/DVD access for your account."
                    : "The optical drive could not be opened. Check that the disc is still inserted.";
            }
            catch (Exception ex)
            {
                log.Error("Imaging task failed", ex);
                fatal = "An unexpected error occurred. Details were written to the log.";
            }
            finally
            {
                _ripClock.Stop();
                _cts.Dispose();
                _cts = null;
                SetBusy(false, drive);
                _services.Monitor.RequestRefresh();
            }

            if (fatal != null)
            {
                SetProgressStatus("Imaging failed", MessageTone.Error);
                RaiseStatus("Imaging failed", StatusKind.Error);
                MessageBox.Show(FindForm(), fatal, AppInfo.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (result.OutputPath != null) _lastOutputPath = result.OutputPath;
            ShowOutcome(result, report);
            using (var completion = new CompletionForm(disc, drive, result, report, ejected, log.CurrentLogFile))
            {
                completion.ShowDialog(FindForm());
                if (completion.RipAnother)
                {
                    ResetProgress("Ready. Insert the next disc.");
                    _pathEditedByUser = false;
                }
            }
            _services.Monitor.RequestRefresh();
            if (!ejected) InspectCurrentDriveAsync();
        }

        private void ResetProgress(string status)
        {
            progressBar.Value = 0;
            SetProgressStatus(status, MessageTone.None);
            readValue.Text = elapsedValue.Text = remainingValue.Text = speedValue.Text = errorsValue.Text = "--";
        }

        private void SetProgressStatus(string text, MessageTone tone)
        {
            _progressTone = tone;
            progressStatusLabel.Text = text;
            ApplyTone(progressStatusLabel, tone);
        }

        private void ShowOutcome(ImagingResult result, VerificationReport report)
        {
            CompletionStatus status = CompletionStatus.Evaluate(result, report);
            MessageTone tone = status.Color == Theme.Success ? MessageTone.Success : status.Color == Theme.Error ? MessageTone.Error : MessageTone.Warning;
            SetProgressStatus(status.Headline, tone);
            if (result.Outcome == ImagingOutcome.Success) progressBar.Value = progressBar.Maximum;
            elapsedValue.Text = Format.Duration(result.Elapsed);
            remainingValue.Text = "--";
            StatusKind kind = tone == MessageTone.Success ? StatusKind.Ready : tone == MessageTone.Error ? StatusKind.Error : StatusKind.Warning;
            RaiseStatus(status.Headline, kind);
        }

        private void ShowProgress(ImagingProgress p, MediaFamily family)
        {
            string text;
            switch (p.Phase)
            {
                case ImagingPhase.Reading: text = "Reading disc..."; break;
                case ImagingPhase.Finalising: text = "Finalising image..."; break;
                case ImagingPhase.VerifyingFile: text = "Verifying the image file..."; break;
                case ImagingPhase.VerifyingSource: text = "Verifying: comparing the image with the disc..."; break;
                default: text = p.Message ?? "Working..."; break;
            }
            if (_cts != null && _cts.IsCancellationRequested) text = "Cancelling...";
            SetProgressStatus(text, MessageTone.None);
            progressBar.Value = (int)Math.Round(p.Fraction * progressBar.Maximum);

            readValue.Text = Format.Bytes(p.SectorsDone * 2048) + " of " + Format.Bytes(p.TotalSectors * 2048)
                             + " (" + (p.Fraction * 100).ToString("0.0", CultureInfo.CurrentCulture) + "%)";
            speedValue.Text = Format.Speed(p.BytesPerSecond, family);
            remainingValue.Text = p.EstimatedRemaining.HasValue ? Format.Duration(p.EstimatedRemaining.Value) : "calculating...";
            if (_ripClock != null) elapsedValue.Text = Format.Duration(_ripClock.Elapsed);
            if (p.Phase == ImagingPhase.Reading)
                errorsValue.Text = p.UnreadableSectors + (p.RecoveredSectors > 0 ? "  (" + p.RecoveredSectors + " recovered by retry)" : string.Empty);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            RequestCancel();
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            RefreshDrives();
        }

        private void OpenFolderButton_Click(object sender, EventArgs e)
        {
            OpenOutputFolder();
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Form form = FindForm();
            if (form != null) form.Close();
        }

        private async void EjectButton_Click(object sender, EventArgs e)
        {
            if (_busy || _currentDrive == null) return;
            string letter = _currentDrive.DriveLetter;
            ejectButton.Enabled = false;
            ILogger log = _services.Log;
            bool ok = await Task.Run(() =>
            {
                try
                {
                    using (var device = OpticalDevice.Open(letter, true))
                    {
                        int error;
                        bool done = device.Eject(out error);
                        if (!done) log.Warn("Eject failed on " + letter + ": Win32 " + error);
                        return done;
                    }
                }
                catch (DeviceAccessException ex)
                {
                    log.Warn("Eject failed: " + ex.Message);
                    return false;
                }
            });
            UpdateControls();
            if (!ok) MessageBox.Show(FindForm(), "The drive did not eject the disc. It may be in use by another program.", AppInfo.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            _services.Monitor.RequestRefresh();
        }

        // ------------------------------------------------------------------ state helpers

        private void SetBusy(bool busy, OpticalDriveInfo drive)
        {
            _busy = busy;
            if (drive != null) _services.Monitor.SetBusy(drive.DriveLetter, busy);
            cancelButton.Enabled = busy;
            UpdateControls();
            var handler = BusyChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private void UpdateControls()
        {
            startButton.Enabled = !_busy && _currentDisc != null && _currentDisc.CanCreateIso;
            ThemeManager.UpdatePrimary(startButton);
            driveComboBox.Enabled = !_busy;
            refreshButton.Enabled = !_busy;
            ejectButton.Enabled = !_busy && _currentDrive != null;
            outputPathTextBox.ReadOnly = _busy;
            browseButton.Enabled = !_busy;
            closeButton.Enabled = !_busy;
            verifyComboBox.Enabled = speedComboBox.Enabled = policyComboBox.Enabled = retriesUpDown.Enabled = ejectCheckBox.Enabled = !_busy;
        }

        private void RaiseStatus(string text, StatusKind kind)
        {
            var handler = StatusChanged;
            if (handler != null) handler(this, new StatusEventArgs(text, kind));
        }

        private static void SelectItem<T>(ComboBox combo, T value)
        {
            for (int i = 0; i < combo.Items.Count; i++)
            {
                var item = combo.Items[i] as ComboItem<T>;
                if (item != null && EqualityComparer<T>.Default.Equals(item.Value, value)) { combo.SelectedIndex = i; return; }
            }
            if (combo.Items.Count > 0) combo.SelectedIndex = 0;
        }

        private static T SelectedValue<T>(ComboBox combo)
        {
            var item = combo.SelectedItem as ComboItem<T>;
            return item != null ? item.Value : default(T);
        }
    }

    internal sealed class ComboItem<T>
    {
        public ComboItem(string text, T value) { Text = text; Value = value; }
        public string Text { get; private set; }
        public T Value { get; private set; }
        public override string ToString() { return Text; }
    }
}
