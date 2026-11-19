namespace HTISORipper.UI.Views
{
    partial class DashboardView
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.sourceGroupBox = new System.Windows.Forms.GroupBox();
            this.driveLabel = new System.Windows.Forms.Label();
            this.driveComboBox = new System.Windows.Forms.ComboBox();
            this.refreshButton = new System.Windows.Forms.Button();
            this.ejectButton = new System.Windows.Forms.Button();
            this.labelCaption = new System.Windows.Forms.Label();
            this.labelValue = new System.Windows.Forms.Label();
            this.typeCaption = new System.Windows.Forms.Label();
            this.typeValue = new System.Windows.Forms.Label();
            this.fsCaption = new System.Windows.Forms.Label();
            this.fsValue = new System.Windows.Forms.Label();
            this.sizeCaption = new System.Windows.Forms.Label();
            this.sizeValue = new System.Windows.Forms.Label();
            this.tracksCaption = new System.Windows.Forms.Label();
            this.tracksValue = new System.Windows.Forms.Label();
            this.formatSupportCaption = new System.Windows.Forms.Label();
            this.formatSupportValue = new System.Windows.Forms.Label();
            this.discDetailLabel = new System.Windows.Forms.Label();
            this.sourceMessageLabel = new System.Windows.Forms.Label();
            this.optionsGroupBox = new System.Windows.Forms.GroupBox();
            this.formatLabel = new System.Windows.Forms.Label();
            this.formatComboBox = new System.Windows.Forms.ComboBox();
            this.compressionLabel = new System.Windows.Forms.Label();
            this.compressionComboBox = new System.Windows.Forms.ComboBox();
            this.verifyLabel = new System.Windows.Forms.Label();
            this.verifyComboBox = new System.Windows.Forms.ComboBox();
            this.speedLabel = new System.Windows.Forms.Label();
            this.speedComboBox = new System.Windows.Forms.ComboBox();
            this.retriesLabel = new System.Windows.Forms.Label();
            this.retriesUpDown = new System.Windows.Forms.NumericUpDown();
            this.policyLabel = new System.Windows.Forms.Label();
            this.policyComboBox = new System.Windows.Forms.ComboBox();
            this.ejectCheckBox = new System.Windows.Forms.CheckBox();
            this.outputGroupBox = new System.Windows.Forms.GroupBox();
            this.outputLabel = new System.Windows.Forms.Label();
            this.outputPathTextBox = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.progressGroupBox = new System.Windows.Forms.GroupBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.progressStatusLabel = new System.Windows.Forms.Label();
            this.readCaption = new System.Windows.Forms.Label();
            this.readValue = new System.Windows.Forms.Label();
            this.elapsedCaption = new System.Windows.Forms.Label();
            this.elapsedValue = new System.Windows.Forms.Label();
            this.remainingCaption = new System.Windows.Forms.Label();
            this.remainingValue = new System.Windows.Forms.Label();
            this.speedCaption = new System.Windows.Forms.Label();
            this.speedValue = new System.Windows.Forms.Label();
            this.errorsCaption = new System.Windows.Forms.Label();
            this.errorsValue = new System.Windows.Forms.Label();
            this.startButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.openFolderButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.footerLabel = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.sourceGroupBox.SuspendLayout();
            this.optionsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.retriesUpDown)).BeginInit();
            this.outputGroupBox.SuspendLayout();
            this.progressGroupBox.SuspendLayout();
            this.SuspendLayout();
            //
            // sourceGroupBox
            //
            this.sourceGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.sourceGroupBox.Controls.Add(this.driveLabel);
            this.sourceGroupBox.Controls.Add(this.driveComboBox);
            this.sourceGroupBox.Controls.Add(this.refreshButton);
            this.sourceGroupBox.Controls.Add(this.ejectButton);
            this.sourceGroupBox.Controls.Add(this.labelCaption);
            this.sourceGroupBox.Controls.Add(this.labelValue);
            this.sourceGroupBox.Controls.Add(this.typeCaption);
            this.sourceGroupBox.Controls.Add(this.typeValue);
            this.sourceGroupBox.Controls.Add(this.fsCaption);
            this.sourceGroupBox.Controls.Add(this.fsValue);
            this.sourceGroupBox.Controls.Add(this.sizeCaption);
            this.sourceGroupBox.Controls.Add(this.sizeValue);
            this.sourceGroupBox.Controls.Add(this.tracksCaption);
            this.sourceGroupBox.Controls.Add(this.tracksValue);
            this.sourceGroupBox.Controls.Add(this.formatSupportCaption);
            this.sourceGroupBox.Controls.Add(this.formatSupportValue);
            this.sourceGroupBox.Controls.Add(this.discDetailLabel);
            this.sourceGroupBox.Controls.Add(this.sourceMessageLabel);
            this.sourceGroupBox.Location = new System.Drawing.Point(12, 6);
            this.sourceGroupBox.Name = "sourceGroupBox";
            this.sourceGroupBox.Size = new System.Drawing.Size(760, 168);
            this.sourceGroupBox.TabIndex = 0;
            this.sourceGroupBox.TabStop = false;
            this.sourceGroupBox.Text = "Source Drive";
            //
            // driveLabel
            //
            this.driveLabel.AutoSize = true;
            this.driveLabel.Location = new System.Drawing.Point(16, 27);
            this.driveLabel.Name = "driveLabel";
            this.driveLabel.Size = new System.Drawing.Size(81, 15);
            this.driveLabel.Text = "Optical Drive:";
            //
            // driveComboBox
            //
            this.driveComboBox.AccessibleName = "Source optical drive";
            this.driveComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.driveComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.driveComboBox.Location = new System.Drawing.Point(110, 23);
            this.driveComboBox.Name = "driveComboBox";
            this.driveComboBox.Size = new System.Drawing.Size(456, 23);
            this.driveComboBox.TabIndex = 0;
            this.driveComboBox.SelectedIndexChanged += new System.EventHandler(this.DriveComboBox_SelectedIndexChanged);
            //
            // refreshButton
            //
            this.refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshButton.Location = new System.Drawing.Point(574, 21);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(84, 27);
            this.refreshButton.TabIndex = 1;
            this.refreshButton.Text = "Refresh";
            this.refreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
            //
            // ejectButton
            //
            this.ejectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ejectButton.Location = new System.Drawing.Point(664, 21);
            this.ejectButton.Name = "ejectButton";
            this.ejectButton.Size = new System.Drawing.Size(84, 27);
            this.ejectButton.TabIndex = 2;
            this.ejectButton.Text = "Eject";
            this.ejectButton.Click += new System.EventHandler(this.EjectButton_Click);
            //
            // disc detail grid (captions are "dim")
            //
            this.labelCaption.AutoSize = true;
            this.labelCaption.Location = new System.Drawing.Point(16, 58);
            this.labelCaption.Name = "labelCaption";
            this.labelCaption.Tag = "dim";
            this.labelCaption.Text = "Disc label:";
            this.labelValue.AutoEllipsis = true;
            this.labelValue.Location = new System.Drawing.Point(110, 58);
            this.labelValue.Name = "labelValue";
            this.labelValue.Size = new System.Drawing.Size(270, 17);
            this.labelValue.Text = "--";
            this.typeCaption.AutoSize = true;
            this.typeCaption.Location = new System.Drawing.Point(16, 77);
            this.typeCaption.Name = "typeCaption";
            this.typeCaption.Tag = "dim";
            this.typeCaption.Text = "Disc type:";
            this.typeValue.AutoEllipsis = true;
            this.typeValue.Location = new System.Drawing.Point(110, 77);
            this.typeValue.Name = "typeValue";
            this.typeValue.Size = new System.Drawing.Size(270, 17);
            this.typeValue.Text = "--";
            this.fsCaption.AutoSize = true;
            this.fsCaption.Location = new System.Drawing.Point(16, 96);
            this.fsCaption.Name = "fsCaption";
            this.fsCaption.Tag = "dim";
            this.fsCaption.Text = "File system:";
            this.fsValue.AutoEllipsis = true;
            this.fsValue.Location = new System.Drawing.Point(110, 96);
            this.fsValue.Name = "fsValue";
            this.fsValue.Size = new System.Drawing.Size(270, 17);
            this.fsValue.Text = "--";
            this.sizeCaption.AutoSize = true;
            this.sizeCaption.Location = new System.Drawing.Point(400, 58);
            this.sizeCaption.Name = "sizeCaption";
            this.sizeCaption.Tag = "dim";
            this.sizeCaption.Text = "Image size:";
            this.sizeValue.AutoEllipsis = true;
            this.sizeValue.Location = new System.Drawing.Point(512, 58);
            this.sizeValue.Name = "sizeValue";
            this.sizeValue.Size = new System.Drawing.Size(236, 17);
            this.sizeValue.Text = "--";
            this.tracksCaption.AutoSize = true;
            this.tracksCaption.Location = new System.Drawing.Point(400, 77);
            this.tracksCaption.Name = "tracksCaption";
            this.tracksCaption.Tag = "dim";
            this.tracksCaption.Text = "Sessions / tracks:";
            this.tracksValue.AutoEllipsis = true;
            this.tracksValue.Location = new System.Drawing.Point(512, 77);
            this.tracksValue.Name = "tracksValue";
            this.tracksValue.Size = new System.Drawing.Size(236, 17);
            this.tracksValue.Text = "--";
            this.formatSupportCaption.AutoSize = true;
            this.formatSupportCaption.Location = new System.Drawing.Point(400, 96);
            this.formatSupportCaption.Name = "formatSupportCaption";
            this.formatSupportCaption.Tag = "dim";
            this.formatSupportCaption.Text = "Output format:";
            this.formatSupportValue.AutoEllipsis = true;
            this.formatSupportValue.Location = new System.Drawing.Point(512, 96);
            this.formatSupportValue.Name = "formatSupportValue";
            this.formatSupportValue.Size = new System.Drawing.Size(236, 17);
            this.formatSupportValue.Text = "--";
            //
            // discDetailLabel
            //
            this.discDetailLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.discDetailLabel.AutoEllipsis = true;
            this.discDetailLabel.Location = new System.Drawing.Point(16, 118);
            this.discDetailLabel.Name = "discDetailLabel";
            this.discDetailLabel.Size = new System.Drawing.Size(732, 17);
            this.discDetailLabel.Tag = "dim";
            this.discDetailLabel.Text = "Detecting optical drives...";
            //
            // sourceMessageLabel
            //
            this.sourceMessageLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.sourceMessageLabel.Location = new System.Drawing.Point(16, 136);
            this.sourceMessageLabel.Name = "sourceMessageLabel";
            this.sourceMessageLabel.Size = new System.Drawing.Size(732, 30);
            this.sourceMessageLabel.Visible = false;
            //
            // optionsGroupBox
            //
            this.optionsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.optionsGroupBox.Controls.Add(this.formatLabel);
            this.optionsGroupBox.Controls.Add(this.formatComboBox);
            this.optionsGroupBox.Controls.Add(this.compressionLabel);
            this.optionsGroupBox.Controls.Add(this.compressionComboBox);
            this.optionsGroupBox.Controls.Add(this.verifyLabel);
            this.optionsGroupBox.Controls.Add(this.verifyComboBox);
            this.optionsGroupBox.Controls.Add(this.speedLabel);
            this.optionsGroupBox.Controls.Add(this.speedComboBox);
            this.optionsGroupBox.Controls.Add(this.retriesLabel);
            this.optionsGroupBox.Controls.Add(this.retriesUpDown);
            this.optionsGroupBox.Controls.Add(this.policyLabel);
            this.optionsGroupBox.Controls.Add(this.policyComboBox);
            this.optionsGroupBox.Controls.Add(this.ejectCheckBox);
            this.optionsGroupBox.Location = new System.Drawing.Point(12, 180);
            this.optionsGroupBox.Name = "optionsGroupBox";
            this.optionsGroupBox.Size = new System.Drawing.Size(760, 112);
            this.optionsGroupBox.TabIndex = 1;
            this.optionsGroupBox.TabStop = false;
            this.optionsGroupBox.Text = "Image Options";
            //
            // options row 1
            //
            this.formatLabel.AutoSize = true;
            this.formatLabel.Location = new System.Drawing.Point(16, 27);
            this.formatLabel.Name = "formatLabel";
            this.formatLabel.Text = "Image format:";
            this.formatComboBox.AccessibleName = "Image format";
            this.formatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.formatComboBox.Location = new System.Drawing.Point(110, 23);
            this.formatComboBox.Name = "formatComboBox";
            this.formatComboBox.Size = new System.Drawing.Size(270, 23);
            this.formatComboBox.TabIndex = 0;
            this.compressionLabel.AutoSize = true;
            this.compressionLabel.Location = new System.Drawing.Point(400, 27);
            this.compressionLabel.Name = "compressionLabel";
            this.compressionLabel.Text = "Compression:";
            this.compressionComboBox.AccessibleName = "Compression";
            this.compressionComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.compressionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.compressionComboBox.Location = new System.Drawing.Point(512, 23);
            this.compressionComboBox.Name = "compressionComboBox";
            this.compressionComboBox.Size = new System.Drawing.Size(236, 23);
            this.compressionComboBox.TabIndex = 1;
            //
            // options row 2
            //
            this.verifyLabel.AutoSize = true;
            this.verifyLabel.Location = new System.Drawing.Point(16, 56);
            this.verifyLabel.Name = "verifyLabel";
            this.verifyLabel.Text = "Verification:";
            this.verifyComboBox.AccessibleName = "Verification";
            this.verifyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.verifyComboBox.Location = new System.Drawing.Point(110, 52);
            this.verifyComboBox.Name = "verifyComboBox";
            this.verifyComboBox.Size = new System.Drawing.Size(270, 23);
            this.verifyComboBox.TabIndex = 2;
            this.speedLabel.AutoSize = true;
            this.speedLabel.Location = new System.Drawing.Point(400, 56);
            this.speedLabel.Name = "speedLabel";
            this.speedLabel.Text = "Read speed:";
            this.speedComboBox.AccessibleName = "Read speed";
            this.speedComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.speedComboBox.Location = new System.Drawing.Point(512, 52);
            this.speedComboBox.Name = "speedComboBox";
            this.speedComboBox.Size = new System.Drawing.Size(112, 23);
            this.speedComboBox.TabIndex = 3;
            this.retriesLabel.AutoSize = true;
            this.retriesLabel.Location = new System.Drawing.Point(636, 56);
            this.retriesLabel.Name = "retriesLabel";
            this.retriesLabel.Text = "Retries:";
            this.retriesUpDown.AccessibleName = "Read retries";
            this.retriesUpDown.Location = new System.Drawing.Point(692, 53);
            this.retriesUpDown.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            this.retriesUpDown.Name = "retriesUpDown";
            this.retriesUpDown.Size = new System.Drawing.Size(56, 23);
            this.retriesUpDown.TabIndex = 4;
            //
            // options row 3
            //
            this.policyLabel.AutoSize = true;
            this.policyLabel.Location = new System.Drawing.Point(16, 85);
            this.policyLabel.Name = "policyLabel";
            this.policyLabel.Text = "If unreadable:";
            this.policyComboBox.AccessibleName = "If a sector cannot be read";
            this.policyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.policyComboBox.Location = new System.Drawing.Point(110, 81);
            this.policyComboBox.Name = "policyComboBox";
            this.policyComboBox.Size = new System.Drawing.Size(270, 23);
            this.policyComboBox.TabIndex = 5;
            this.ejectCheckBox.AutoSize = true;
            this.ejectCheckBox.Location = new System.Drawing.Point(403, 84);
            this.ejectCheckBox.Name = "ejectCheckBox";
            this.ejectCheckBox.TabIndex = 6;
            this.ejectCheckBox.Text = "Eject the disc after a successful rip";
            this.ejectCheckBox.UseVisualStyleBackColor = true;
            //
            // outputGroupBox
            //
            this.outputGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.outputGroupBox.Controls.Add(this.outputLabel);
            this.outputGroupBox.Controls.Add(this.outputPathTextBox);
            this.outputGroupBox.Controls.Add(this.browseButton);
            this.outputGroupBox.Location = new System.Drawing.Point(12, 298);
            this.outputGroupBox.Name = "outputGroupBox";
            this.outputGroupBox.Size = new System.Drawing.Size(760, 60);
            this.outputGroupBox.TabIndex = 2;
            this.outputGroupBox.TabStop = false;
            this.outputGroupBox.Text = "Output Destination";
            this.outputLabel.AutoSize = true;
            this.outputLabel.Location = new System.Drawing.Point(16, 27);
            this.outputLabel.Name = "outputLabel";
            this.outputLabel.Text = "ISO File:";
            this.outputPathTextBox.AccessibleName = "Output image file";
            this.outputPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.outputPathTextBox.Location = new System.Drawing.Point(110, 23);
            this.outputPathTextBox.Name = "outputPathTextBox";
            this.outputPathTextBox.Size = new System.Drawing.Size(548, 23);
            this.outputPathTextBox.TabIndex = 0;
            this.outputPathTextBox.TextChanged += new System.EventHandler(this.OutputPathTextBox_TextChanged);
            this.browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseButton.Location = new System.Drawing.Point(664, 21);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(84, 27);
            this.browseButton.TabIndex = 1;
            this.browseButton.Text = "Save As...";
            this.browseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            //
            // progressGroupBox
            //
            this.progressGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.progressGroupBox.Controls.Add(this.progressBar);
            this.progressGroupBox.Controls.Add(this.progressStatusLabel);
            this.progressGroupBox.Controls.Add(this.readCaption);
            this.progressGroupBox.Controls.Add(this.readValue);
            this.progressGroupBox.Controls.Add(this.elapsedCaption);
            this.progressGroupBox.Controls.Add(this.elapsedValue);
            this.progressGroupBox.Controls.Add(this.remainingCaption);
            this.progressGroupBox.Controls.Add(this.remainingValue);
            this.progressGroupBox.Controls.Add(this.speedCaption);
            this.progressGroupBox.Controls.Add(this.speedValue);
            this.progressGroupBox.Controls.Add(this.errorsCaption);
            this.progressGroupBox.Controls.Add(this.errorsValue);
            this.progressGroupBox.Location = new System.Drawing.Point(12, 364);
            this.progressGroupBox.Name = "progressGroupBox";
            this.progressGroupBox.Size = new System.Drawing.Size(760, 140);
            this.progressGroupBox.TabIndex = 3;
            this.progressGroupBox.TabStop = false;
            this.progressGroupBox.Text = "Progress";
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(16, 24);
            this.progressBar.Maximum = 1000;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(732, 20);
            this.progressBar.TabIndex = 0;
            this.progressStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.progressStatusLabel.AutoEllipsis = true;
            this.progressStatusLabel.Location = new System.Drawing.Point(16, 50);
            this.progressStatusLabel.Name = "progressStatusLabel";
            this.progressStatusLabel.Size = new System.Drawing.Size(732, 17);
            this.progressStatusLabel.Text = "Ready. Insert a CD or DVD to begin.";
            this.readCaption.AutoSize = true;
            this.readCaption.Location = new System.Drawing.Point(16, 74);
            this.readCaption.Name = "readCaption";
            this.readCaption.Tag = "dim";
            this.readCaption.Text = "Read:";
            this.readValue.AutoEllipsis = true;
            this.readValue.Location = new System.Drawing.Point(110, 74);
            this.readValue.Name = "readValue";
            this.readValue.Size = new System.Drawing.Size(270, 17);
            this.readValue.Text = "--";
            this.elapsedCaption.AutoSize = true;
            this.elapsedCaption.Location = new System.Drawing.Point(400, 74);
            this.elapsedCaption.Name = "elapsedCaption";
            this.elapsedCaption.Tag = "dim";
            this.elapsedCaption.Text = "Elapsed:";
            this.elapsedValue.Location = new System.Drawing.Point(512, 74);
            this.elapsedValue.Name = "elapsedValue";
            this.elapsedValue.Size = new System.Drawing.Size(236, 17);
            this.elapsedValue.Text = "--";
            this.remainingCaption.AutoSize = true;
            this.remainingCaption.Location = new System.Drawing.Point(16, 93);
            this.remainingCaption.Name = "remainingCaption";
            this.remainingCaption.Tag = "dim";
            this.remainingCaption.Text = "Remaining:";
            this.remainingValue.Location = new System.Drawing.Point(110, 93);
            this.remainingValue.Name = "remainingValue";
            this.remainingValue.Size = new System.Drawing.Size(270, 17);
            this.remainingValue.Text = "--";
            this.speedCaption.AutoSize = true;
            this.speedCaption.Location = new System.Drawing.Point(400, 93);
            this.speedCaption.Name = "speedCaption";
            this.speedCaption.Tag = "dim";
            this.speedCaption.Text = "Speed:";
            this.speedValue.Location = new System.Drawing.Point(512, 93);
            this.speedValue.Name = "speedValue";
            this.speedValue.Size = new System.Drawing.Size(236, 17);
            this.speedValue.Text = "--";
            this.errorsCaption.AutoSize = true;
            this.errorsCaption.Location = new System.Drawing.Point(16, 112);
            this.errorsCaption.Name = "errorsCaption";
            this.errorsCaption.Tag = "dim";
            this.errorsCaption.Text = "Read errors:";
            this.errorsValue.Location = new System.Drawing.Point(110, 112);
            this.errorsValue.Name = "errorsValue";
            this.errorsValue.Size = new System.Drawing.Size(500, 17);
            this.errorsValue.Text = "--";
            //
            // footerLabel
            //
            this.footerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.footerLabel.AutoSize = true;
            this.footerLabel.Location = new System.Drawing.Point(12, 530);
            this.footerLabel.Name = "footerLabel";
            this.footerLabel.Tag = "dim";
            this.footerLabel.Text = "Discs are always read read-only.";
            //
            // startButton
            //
            this.startButton.AccessibleName = "Start ripping";
            this.startButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.startButton.Location = new System.Drawing.Point(318, 520);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(115, 34);
            this.startButton.TabIndex = 4;
            this.startButton.Tag = "primary";
            this.startButton.Text = "Start Ripping";
            this.startButton.Click += new System.EventHandler(this.StartButton_Click);
            //
            // cancelButton
            //
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Enabled = false;
            this.cancelButton.Location = new System.Drawing.Point(439, 520);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(95, 34);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            //
            // openFolderButton
            //
            this.openFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.openFolderButton.Location = new System.Drawing.Point(540, 520);
            this.openFolderButton.Name = "openFolderButton";
            this.openFolderButton.Size = new System.Drawing.Size(137, 34);
            this.openFolderButton.TabIndex = 6;
            this.openFolderButton.Text = "Open Output Folder";
            this.openFolderButton.Click += new System.EventHandler(this.OpenFolderButton_Click);
            //
            // closeButton
            //
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.Location = new System.Drawing.Point(683, 520);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(89, 34);
            this.closeButton.TabIndex = 7;
            this.closeButton.Text = "Close";
            this.closeButton.Click += new System.EventHandler(this.CloseButton_Click);
            //
            // DashboardView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(700, 562);
            this.Controls.Add(this.sourceGroupBox);
            this.Controls.Add(this.optionsGroupBox);
            this.Controls.Add(this.outputGroupBox);
            this.Controls.Add(this.progressGroupBox);
            this.Controls.Add(this.footerLabel);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.openFolderButton);
            this.Controls.Add(this.closeButton);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "DashboardView";
            this.Size = new System.Drawing.Size(784, 566);
            this.sourceGroupBox.ResumeLayout(false);
            this.sourceGroupBox.PerformLayout();
            this.optionsGroupBox.ResumeLayout(false);
            this.optionsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.retriesUpDown)).EndInit();
            this.outputGroupBox.ResumeLayout(false);
            this.outputGroupBox.PerformLayout();
            this.progressGroupBox.ResumeLayout(false);
            this.progressGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.GroupBox sourceGroupBox;
        private System.Windows.Forms.Label driveLabel;
        private System.Windows.Forms.ComboBox driveComboBox;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.Button ejectButton;
        private System.Windows.Forms.Label labelCaption;
        private System.Windows.Forms.Label labelValue;
        private System.Windows.Forms.Label typeCaption;
        private System.Windows.Forms.Label typeValue;
        private System.Windows.Forms.Label fsCaption;
        private System.Windows.Forms.Label fsValue;
        private System.Windows.Forms.Label sizeCaption;
        private System.Windows.Forms.Label sizeValue;
        private System.Windows.Forms.Label tracksCaption;
        private System.Windows.Forms.Label tracksValue;
        private System.Windows.Forms.Label formatSupportCaption;
        private System.Windows.Forms.Label formatSupportValue;
        private System.Windows.Forms.Label discDetailLabel;
        private System.Windows.Forms.Label sourceMessageLabel;
        private System.Windows.Forms.GroupBox optionsGroupBox;
        private System.Windows.Forms.Label formatLabel;
        private System.Windows.Forms.ComboBox formatComboBox;
        private System.Windows.Forms.Label compressionLabel;
        private System.Windows.Forms.ComboBox compressionComboBox;
        private System.Windows.Forms.Label verifyLabel;
        private System.Windows.Forms.ComboBox verifyComboBox;
        private System.Windows.Forms.Label speedLabel;
        private System.Windows.Forms.ComboBox speedComboBox;
        private System.Windows.Forms.Label retriesLabel;
        private System.Windows.Forms.NumericUpDown retriesUpDown;
        private System.Windows.Forms.Label policyLabel;
        private System.Windows.Forms.ComboBox policyComboBox;
        private System.Windows.Forms.CheckBox ejectCheckBox;
        private System.Windows.Forms.GroupBox outputGroupBox;
        private System.Windows.Forms.Label outputLabel;
        private System.Windows.Forms.TextBox outputPathTextBox;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.GroupBox progressGroupBox;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label progressStatusLabel;
        private System.Windows.Forms.Label readCaption;
        private System.Windows.Forms.Label readValue;
        private System.Windows.Forms.Label elapsedCaption;
        private System.Windows.Forms.Label elapsedValue;
        private System.Windows.Forms.Label remainingCaption;
        private System.Windows.Forms.Label remainingValue;
        private System.Windows.Forms.Label speedCaption;
        private System.Windows.Forms.Label speedValue;
        private System.Windows.Forms.Label errorsCaption;
        private System.Windows.Forms.Label errorsValue;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button openFolderButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Label footerLabel;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
