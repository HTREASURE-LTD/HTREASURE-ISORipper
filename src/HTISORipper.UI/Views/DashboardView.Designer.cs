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
            this.layout = new System.Windows.Forms.TableLayoutPanel();
            this.sourceCaption = new System.Windows.Forms.Label();
            this.sourceCard = new HTISORipper.UI.Controls.CardPanel();
            this.sourceTable = new System.Windows.Forms.TableLayoutPanel();
            this.discIconBox = new System.Windows.Forms.PictureBox();
            this.driveComboBox = new System.Windows.Forms.ComboBox();
            this.discDetailLabel = new System.Windows.Forms.Label();
            this.ejectButton = new System.Windows.Forms.Button();
            this.sourceMessageLabel = new System.Windows.Forms.Label();
            this.outputCaption = new System.Windows.Forms.Label();
            this.outputCard = new HTISORipper.UI.Controls.CardPanel();
            this.outputTable = new System.Windows.Forms.TableLayoutPanel();
            this.outputPathTextBox = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.formatTable = new System.Windows.Forms.TableLayoutPanel();
            this.formatLabel = new System.Windows.Forms.Label();
            this.compressionLabel = new System.Windows.Forms.Label();
            this.formatComboBox = new System.Windows.Forms.ComboBox();
            this.compressionComboBox = new System.Windows.Forms.ComboBox();
            this.optionsTable = new System.Windows.Forms.TableLayoutPanel();
            this.verifyLabel = new System.Windows.Forms.Label();
            this.speedLabel = new System.Windows.Forms.Label();
            this.retriesLabel = new System.Windows.Forms.Label();
            this.policyLabel = new System.Windows.Forms.Label();
            this.verifyComboBox = new System.Windows.Forms.ComboBox();
            this.speedComboBox = new System.Windows.Forms.ComboBox();
            this.retriesUpDown = new System.Windows.Forms.NumericUpDown();
            this.policyComboBox = new System.Windows.Forms.ComboBox();
            this.ejectCheckBox = new System.Windows.Forms.CheckBox();
            this.startButton = new System.Windows.Forms.Button();
            this.progressCard = new HTISORipper.UI.Controls.CardPanel();
            this.progressTable = new System.Windows.Forms.TableLayoutPanel();
            this.progressStatusLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.progressStatsLabel = new System.Windows.Forms.Label();
            this.footerLabel = new System.Windows.Forms.Label();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.layout.SuspendLayout();
            this.sourceCard.SuspendLayout();
            this.sourceTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.discIconBox)).BeginInit();
            this.outputCard.SuspendLayout();
            this.outputTable.SuspendLayout();
            this.formatTable.SuspendLayout();
            this.optionsTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.retriesUpDown)).BeginInit();
            this.progressCard.SuspendLayout();
            this.progressTable.SuspendLayout();
            this.SuspendLayout();
            //
            // layout
            //
            this.layout.AutoSize = true;
            this.layout.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.layout.ColumnCount = 1;
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.Controls.Add(this.sourceCaption, 0, 0);
            this.layout.Controls.Add(this.sourceCard, 0, 1);
            this.layout.Controls.Add(this.sourceMessageLabel, 0, 2);
            this.layout.Controls.Add(this.outputCaption, 0, 3);
            this.layout.Controls.Add(this.outputCard, 0, 4);
            this.layout.Controls.Add(this.formatTable, 0, 5);
            this.layout.Controls.Add(this.optionsTable, 0, 6);
            this.layout.Controls.Add(this.ejectCheckBox, 0, 7);
            this.layout.Controls.Add(this.startButton, 0, 8);
            this.layout.Controls.Add(this.progressCard, 0, 9);
            this.layout.Controls.Add(this.footerLabel, 0, 10);
            this.layout.Dock = System.Windows.Forms.DockStyle.Top;
            this.layout.Name = "layout";
            this.layout.Padding = new System.Windows.Forms.Padding(16, 8, 16, 16);
            this.layout.RowCount = 11;
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 82F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 54F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.Size = new System.Drawing.Size(780, 640);
            this.layout.TabIndex = 0;
            //
            // sourceCaption
            //
            this.sourceCaption.AutoSize = true;
            this.sourceCaption.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.sourceCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(100)))), ((int)(((byte)(114)))));
            this.sourceCaption.Margin = new System.Windows.Forms.Padding(0, 8, 0, 4);
            this.sourceCaption.Name = "sourceCaption";
            this.sourceCaption.Text = "SOURCE DRIVE";
            //
            // sourceCard
            //
            this.sourceCard.Controls.Add(this.sourceTable);
            this.sourceCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceCard.Margin = new System.Windows.Forms.Padding(0);
            this.sourceCard.Name = "sourceCard";
            this.sourceCard.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.sourceCard.TabIndex = 0;
            //
            // sourceTable
            //
            this.sourceTable.BackColor = System.Drawing.Color.White;
            this.sourceTable.ColumnCount = 3;
            this.sourceTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.sourceTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.sourceTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.sourceTable.Controls.Add(this.discIconBox, 0, 0);
            this.sourceTable.Controls.Add(this.driveComboBox, 1, 0);
            this.sourceTable.Controls.Add(this.discDetailLabel, 1, 1);
            this.sourceTable.Controls.Add(this.ejectButton, 2, 0);
            this.sourceTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceTable.Name = "sourceTable";
            this.sourceTable.RowCount = 2;
            this.sourceTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55F));
            this.sourceTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.sourceTable.TabIndex = 0;
            this.sourceTable.SetRowSpan(this.discIconBox, 2);
            this.sourceTable.SetRowSpan(this.ejectButton, 2);
            //
            // discIconBox
            //
            this.discIconBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.discIconBox.Name = "discIconBox";
            this.discIconBox.Size = new System.Drawing.Size(28, 28);
            this.discIconBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.discIconBox.TabStop = false;
            //
            // driveComboBox
            //
            this.driveComboBox.AccessibleName = "Source optical drive";
            this.driveComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.driveComboBox.Margin = new System.Windows.Forms.Padding(0, 4, 8, 0);
            this.driveComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.driveComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.driveComboBox.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold);
            this.driveComboBox.Name = "driveComboBox";
            this.driveComboBox.TabIndex = 0;
            this.driveComboBox.SelectedIndexChanged += new System.EventHandler(this.DriveComboBox_SelectedIndexChanged);
            //
            // discDetailLabel
            //
            this.discDetailLabel.AutoEllipsis = true;
            this.discDetailLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.discDetailLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(100)))), ((int)(((byte)(114)))));
            this.discDetailLabel.Name = "discDetailLabel";
            this.discDetailLabel.Padding = new System.Windows.Forms.Padding(2, 2, 0, 0);
            this.discDetailLabel.Text = "Detecting optical drives…";
            //
            // ejectButton
            //
            this.ejectButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.ejectButton.AutoSize = true;
            this.ejectButton.Name = "ejectButton";
            this.ejectButton.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.ejectButton.TabIndex = 1;
            this.ejectButton.Text = "Eject";
            this.ejectButton.Click += new System.EventHandler(this.EjectButton_Click);
            //
            // sourceMessageLabel
            //
            this.sourceMessageLabel.AutoSize = true;
            this.sourceMessageLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sourceMessageLabel.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.sourceMessageLabel.MaximumSize = new System.Drawing.Size(2000, 0);
            this.sourceMessageLabel.Name = "sourceMessageLabel";
            this.sourceMessageLabel.Padding = new System.Windows.Forms.Padding(10, 8, 10, 8);
            this.sourceMessageLabel.Visible = false;
            //
            // outputCaption
            //
            this.outputCaption.AutoSize = true;
            this.outputCaption.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.outputCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(100)))), ((int)(((byte)(114)))));
            this.outputCaption.Margin = new System.Windows.Forms.Padding(0, 14, 0, 4);
            this.outputCaption.Name = "outputCaption";
            this.outputCaption.Text = "OUTPUT IMAGE";
            //
            // outputCard
            //
            this.outputCard.Controls.Add(this.outputTable);
            this.outputCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputCard.Margin = new System.Windows.Forms.Padding(0);
            this.outputCard.Name = "outputCard";
            this.outputCard.Padding = new System.Windows.Forms.Padding(12, 6, 8, 6);
            this.outputCard.TabIndex = 1;
            //
            // outputTable
            //
            this.outputTable.BackColor = System.Drawing.Color.White;
            this.outputTable.ColumnCount = 2;
            this.outputTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.outputTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.outputTable.Controls.Add(this.outputPathTextBox, 0, 0);
            this.outputTable.Controls.Add(this.browseButton, 1, 0);
            this.outputTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputTable.Name = "outputTable";
            this.outputTable.RowCount = 1;
            this.outputTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.outputTable.TabIndex = 0;
            //
            // outputPathTextBox
            //
            this.outputPathTextBox.AccessibleName = "Output image file";
            this.outputPathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.outputPathTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.outputPathTextBox.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.outputPathTextBox.Name = "outputPathTextBox";
            this.outputPathTextBox.TabIndex = 0;
            this.outputPathTextBox.TextChanged += new System.EventHandler(this.OutputPathTextBox_TextChanged);
            //
            // browseButton
            //
            this.browseButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.browseButton.AutoSize = true;
            this.browseButton.Name = "browseButton";
            this.browseButton.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.browseButton.TabIndex = 1;
            this.browseButton.Text = "Browse…";
            this.browseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            //
            // formatTable
            //
            this.formatTable.AutoSize = true;
            this.formatTable.ColumnCount = 2;
            this.formatTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.formatTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.formatTable.Controls.Add(this.formatLabel, 0, 0);
            this.formatTable.Controls.Add(this.compressionLabel, 1, 0);
            this.formatTable.Controls.Add(this.formatComboBox, 0, 1);
            this.formatTable.Controls.Add(this.compressionComboBox, 1, 1);
            this.formatTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formatTable.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.formatTable.Name = "formatTable";
            this.formatTable.RowCount = 2;
            this.formatTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.formatTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.formatTable.TabIndex = 2;
            //
            // formatLabel
            //
            this.formatLabel.AutoSize = true;
            this.formatLabel.Name = "formatLabel";
            this.formatLabel.Text = "Image format";
            //
            // compressionLabel
            //
            this.compressionLabel.AutoSize = true;
            this.compressionLabel.Margin = new System.Windows.Forms.Padding(8, 0, 3, 0);
            this.compressionLabel.Name = "compressionLabel";
            this.compressionLabel.Text = "Compression";
            //
            // formatComboBox
            //
            this.formatComboBox.AccessibleName = "Image format";
            this.formatComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formatComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.formatComboBox.Margin = new System.Windows.Forms.Padding(0, 3, 8, 3);
            this.formatComboBox.Name = "formatComboBox";
            this.formatComboBox.TabIndex = 0;
            //
            // compressionComboBox
            //
            this.compressionComboBox.AccessibleName = "Compression";
            this.compressionComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.compressionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.compressionComboBox.Margin = new System.Windows.Forms.Padding(8, 3, 0, 3);
            this.compressionComboBox.Name = "compressionComboBox";
            this.compressionComboBox.TabIndex = 1;
            //
            // optionsTable
            //
            this.optionsTable.AutoSize = true;
            this.optionsTable.ColumnCount = 4;
            this.optionsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.optionsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.optionsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15F));
            this.optionsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.optionsTable.Controls.Add(this.verifyLabel, 0, 0);
            this.optionsTable.Controls.Add(this.speedLabel, 1, 0);
            this.optionsTable.Controls.Add(this.retriesLabel, 2, 0);
            this.optionsTable.Controls.Add(this.policyLabel, 3, 0);
            this.optionsTable.Controls.Add(this.verifyComboBox, 0, 1);
            this.optionsTable.Controls.Add(this.speedComboBox, 1, 1);
            this.optionsTable.Controls.Add(this.retriesUpDown, 2, 1);
            this.optionsTable.Controls.Add(this.policyComboBox, 3, 1);
            this.optionsTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.optionsTable.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.optionsTable.Name = "optionsTable";
            this.optionsTable.RowCount = 2;
            this.optionsTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.optionsTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.optionsTable.TabIndex = 3;
            //
            // verifyLabel
            //
            this.verifyLabel.AutoSize = true;
            this.verifyLabel.Name = "verifyLabel";
            this.verifyLabel.Text = "Verification";
            //
            // speedLabel
            //
            this.speedLabel.AutoSize = true;
            this.speedLabel.Name = "speedLabel";
            this.speedLabel.Text = "Read speed";
            //
            // retriesLabel
            //
            this.retriesLabel.AutoSize = true;
            this.retriesLabel.Name = "retriesLabel";
            this.retriesLabel.Text = "Retries";
            //
            // policyLabel
            //
            this.policyLabel.AutoSize = true;
            this.policyLabel.Name = "policyLabel";
            this.policyLabel.Text = "If a sector cannot be read";
            //
            // verifyComboBox
            //
            this.verifyComboBox.AccessibleName = "Verification";
            this.verifyComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.verifyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.verifyComboBox.Margin = new System.Windows.Forms.Padding(0, 3, 8, 3);
            this.verifyComboBox.Name = "verifyComboBox";
            this.verifyComboBox.TabIndex = 0;
            //
            // speedComboBox
            //
            this.speedComboBox.AccessibleName = "Read speed";
            this.speedComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.speedComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.speedComboBox.Margin = new System.Windows.Forms.Padding(0, 3, 8, 3);
            this.speedComboBox.Name = "speedComboBox";
            this.speedComboBox.TabIndex = 1;
            //
            // retriesUpDown
            //
            this.retriesUpDown.AccessibleName = "Read retries";
            this.retriesUpDown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.retriesUpDown.Margin = new System.Windows.Forms.Padding(0, 3, 8, 3);
            this.retriesUpDown.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            this.retriesUpDown.Name = "retriesUpDown";
            this.retriesUpDown.TabIndex = 2;
            //
            // policyComboBox
            //
            this.policyComboBox.AccessibleName = "Error handling policy";
            this.policyComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.policyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.policyComboBox.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.policyComboBox.Name = "policyComboBox";
            this.policyComboBox.TabIndex = 3;
            //
            // ejectCheckBox
            //
            this.ejectCheckBox.AutoSize = true;
            this.ejectCheckBox.Margin = new System.Windows.Forms.Padding(0, 10, 0, 6);
            this.ejectCheckBox.Name = "ejectCheckBox";
            this.ejectCheckBox.TabIndex = 4;
            this.ejectCheckBox.Text = "Eject disc after successful completion";
            this.ejectCheckBox.UseVisualStyleBackColor = true;
            //
            // startButton
            //
            this.startButton.AccessibleName = "Start ripping";
            this.startButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.startButton.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.startButton.Margin = new System.Windows.Forms.Padding(0, 6, 0, 6);
            this.startButton.Name = "startButton";
            this.startButton.TabIndex = 5;
            this.startButton.Text = "START RIPPING";
            this.startButton.Click += new System.EventHandler(this.StartButton_Click);
            //
            // progressCard
            //
            this.progressCard.AutoSize = true;
            this.progressCard.Controls.Add(this.progressTable);
            this.progressCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressCard.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.progressCard.Name = "progressCard";
            this.progressCard.Padding = new System.Windows.Forms.Padding(12, 10, 12, 10);
            this.progressCard.TabIndex = 6;
            this.progressCard.Visible = false;
            //
            // progressTable
            //
            this.progressTable.AutoSize = true;
            this.progressTable.BackColor = System.Drawing.Color.White;
            this.progressTable.ColumnCount = 2;
            this.progressTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.progressTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.progressTable.Controls.Add(this.progressStatusLabel, 0, 0);
            this.progressTable.Controls.Add(this.cancelButton, 1, 0);
            this.progressTable.Controls.Add(this.progressBar, 0, 1);
            this.progressTable.Controls.Add(this.progressStatsLabel, 0, 2);
            this.progressTable.Dock = System.Windows.Forms.DockStyle.Top;
            this.progressTable.Name = "progressTable";
            this.progressTable.RowCount = 3;
            this.progressTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.progressTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.progressTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.progressTable.TabIndex = 0;
            this.progressTable.SetColumnSpan(this.progressBar, 2);
            this.progressTable.SetColumnSpan(this.progressStatsLabel, 2);
            //
            // progressStatusLabel
            //
            this.progressStatusLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.progressStatusLabel.AutoSize = true;
            this.progressStatusLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold);
            this.progressStatusLabel.Name = "progressStatusLabel";
            this.progressStatusLabel.Text = "Ready";
            //
            // cancelButton
            //
            this.cancelButton.AutoSize = true;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Padding = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            //
            // progressBar
            //
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.progressBar.Maximum = 1000;
            this.progressBar.Name = "progressBar";
            this.progressBar.TabIndex = 1;
            //
            // progressStatsLabel
            //
            this.progressStatsLabel.AutoSize = true;
            this.progressStatsLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(100)))), ((int)(((byte)(114)))));
            this.progressStatsLabel.Name = "progressStatsLabel";
            this.progressStatsLabel.Text = "-";
            //
            // footerLabel
            //
            this.footerLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.footerLabel.AutoSize = true;
            this.footerLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.footerLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(100)))), ((int)(((byte)(114)))));
            this.footerLabel.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.footerLabel.Name = "footerLabel";
            this.footerLabel.Text = "The source disc is opened read-only and is never modified.";
            //
            // DashboardView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.layout);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.Name = "DashboardView";
            this.Size = new System.Drawing.Size(780, 640);
            this.layout.ResumeLayout(false);
            this.layout.PerformLayout();
            this.sourceCard.ResumeLayout(false);
            this.sourceTable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.discIconBox)).EndInit();
            this.outputCard.ResumeLayout(false);
            this.outputTable.ResumeLayout(false);
            this.outputTable.PerformLayout();
            this.formatTable.ResumeLayout(false);
            this.formatTable.PerformLayout();
            this.optionsTable.ResumeLayout(false);
            this.optionsTable.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.retriesUpDown)).EndInit();
            this.progressCard.ResumeLayout(false);
            this.progressCard.PerformLayout();
            this.progressTable.ResumeLayout(false);
            this.progressTable.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TableLayoutPanel layout;
        private System.Windows.Forms.Label sourceCaption;
        private HTISORipper.UI.Controls.CardPanel sourceCard;
        private System.Windows.Forms.TableLayoutPanel sourceTable;
        private System.Windows.Forms.PictureBox discIconBox;
        private System.Windows.Forms.ComboBox driveComboBox;
        private System.Windows.Forms.Label discDetailLabel;
        private System.Windows.Forms.Button ejectButton;
        private System.Windows.Forms.Label sourceMessageLabel;
        private System.Windows.Forms.Label outputCaption;
        private HTISORipper.UI.Controls.CardPanel outputCard;
        private System.Windows.Forms.TableLayoutPanel outputTable;
        private System.Windows.Forms.TextBox outputPathTextBox;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.TableLayoutPanel formatTable;
        private System.Windows.Forms.Label formatLabel;
        private System.Windows.Forms.Label compressionLabel;
        private System.Windows.Forms.ComboBox formatComboBox;
        private System.Windows.Forms.ComboBox compressionComboBox;
        private System.Windows.Forms.TableLayoutPanel optionsTable;
        private System.Windows.Forms.Label verifyLabel;
        private System.Windows.Forms.Label speedLabel;
        private System.Windows.Forms.Label retriesLabel;
        private System.Windows.Forms.Label policyLabel;
        private System.Windows.Forms.ComboBox verifyComboBox;
        private System.Windows.Forms.ComboBox speedComboBox;
        private System.Windows.Forms.NumericUpDown retriesUpDown;
        private System.Windows.Forms.ComboBox policyComboBox;
        private System.Windows.Forms.CheckBox ejectCheckBox;
        private System.Windows.Forms.Button startButton;
        private HTISORipper.UI.Controls.CardPanel progressCard;
        private System.Windows.Forms.TableLayoutPanel progressTable;
        private System.Windows.Forms.Label progressStatusLabel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label progressStatsLabel;
        private System.Windows.Forms.Label footerLabel;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
