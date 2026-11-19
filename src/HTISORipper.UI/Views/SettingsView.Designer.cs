namespace HTISORipper.UI.Views
{
    partial class SettingsView
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.layout = new System.Windows.Forms.TableLayoutPanel();
            this.captionLabel = new System.Windows.Forms.Label();
            this.outputDirLabel = new System.Windows.Forms.Label();
            this.outputDirPanel = new System.Windows.Forms.TableLayoutPanel();
            this.outputDirTextBox = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.formatLabel = new System.Windows.Forms.Label();
            this.formatValueLabel = new System.Windows.Forms.Label();
            this.verifyLabel = new System.Windows.Forms.Label();
            this.verifyComboBox = new System.Windows.Forms.ComboBox();
            this.speedLabel = new System.Windows.Forms.Label();
            this.speedComboBox = new System.Windows.Forms.ComboBox();
            this.retriesLabel = new System.Windows.Forms.Label();
            this.retriesUpDown = new System.Windows.Forms.NumericUpDown();
            this.policyLabel = new System.Windows.Forms.Label();
            this.policyComboBox = new System.Windows.Forms.ComboBox();
            this.blockLabel = new System.Windows.Forms.Label();
            this.blockComboBox = new System.Windows.Forms.ComboBox();
            this.themeLabel = new System.Windows.Forms.Label();
            this.themeComboBox = new System.Windows.Forms.ComboBox();
            this.retentionLabel = new System.Windows.Forms.Label();
            this.retentionUpDown = new System.Windows.Forms.NumericUpDown();
            this.ejectCheckBox = new System.Windows.Forms.CheckBox();
            this.buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.saveButton = new System.Windows.Forms.Button();
            this.defaultsButton = new System.Windows.Forms.Button();
            this.statusLabel = new System.Windows.Forms.Label();
            this.layout.SuspendLayout();
            this.outputDirPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.retriesUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.retentionUpDown)).BeginInit();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // layout
            //
            this.layout.AutoSize = true;
            this.layout.ColumnCount = 2;
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 210F));
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.Controls.Add(this.captionLabel, 0, 0);
            this.layout.Controls.Add(this.outputDirLabel, 0, 1);
            this.layout.Controls.Add(this.outputDirPanel, 1, 1);
            this.layout.Controls.Add(this.formatLabel, 0, 2);
            this.layout.Controls.Add(this.formatValueLabel, 1, 2);
            this.layout.Controls.Add(this.verifyLabel, 0, 3);
            this.layout.Controls.Add(this.verifyComboBox, 1, 3);
            this.layout.Controls.Add(this.speedLabel, 0, 4);
            this.layout.Controls.Add(this.speedComboBox, 1, 4);
            this.layout.Controls.Add(this.retriesLabel, 0, 5);
            this.layout.Controls.Add(this.retriesUpDown, 1, 5);
            this.layout.Controls.Add(this.policyLabel, 0, 6);
            this.layout.Controls.Add(this.policyComboBox, 1, 6);
            this.layout.Controls.Add(this.blockLabel, 0, 7);
            this.layout.Controls.Add(this.blockComboBox, 1, 7);
            this.layout.Controls.Add(this.retentionLabel, 0, 8);
            this.layout.Controls.Add(this.retentionUpDown, 1, 8);
            this.layout.Controls.Add(this.themeLabel, 0, 9);
            this.layout.Controls.Add(this.themeComboBox, 1, 9);
            this.layout.Controls.Add(this.ejectCheckBox, 1, 10);
            this.layout.Controls.Add(this.buttonPanel, 1, 11);
            this.layout.Controls.Add(this.statusLabel, 0, 12);
            this.layout.Dock = System.Windows.Forms.DockStyle.Top;
            this.layout.Name = "layout";
            this.layout.Padding = new System.Windows.Forms.Padding(16, 8, 16, 16);
            this.layout.RowCount = 13;
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.SetColumnSpan(this.captionLabel, 2);
            this.layout.SetColumnSpan(this.statusLabel, 2);
            this.layout.TabIndex = 0;
            //
            // captionLabel
            //
            this.captionLabel.AutoSize = true;
            this.captionLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.captionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(100)))), ((int)(((byte)(114)))));
            this.captionLabel.Margin = new System.Windows.Forms.Padding(0, 8, 0, 8);
            this.captionLabel.Name = "captionLabel";
            this.captionLabel.Text = "DEFAULT SETTINGS FOR NEW RIPS";
            //
            // labels
            //
            this.outputDirLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.outputDirLabel.AutoSize = true;
            this.outputDirLabel.Text = "Default output folder";
            this.formatLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.formatLabel.AutoSize = true;
            this.formatLabel.Text = "Default image format";
            this.formatValueLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.formatValueLabel.AutoSize = true;
            this.formatValueLabel.Text = "Standard ISO (.iso), uncompressed";
            this.verifyLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.verifyLabel.AutoSize = true;
            this.verifyLabel.Text = "Automatic verification";
            this.speedLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.speedLabel.AutoSize = true;
            this.speedLabel.Text = "Reading speed";
            this.retriesLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.retriesLabel.AutoSize = true;
            this.retriesLabel.Text = "Maximum read retries";
            this.policyLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.policyLabel.AutoSize = true;
            this.policyLabel.Text = "Unreadable sectors";
            this.blockLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.blockLabel.AutoSize = true;
            this.blockLabel.Text = "Read block size";
            this.retentionLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.retentionLabel.AutoSize = true;
            this.retentionLabel.Text = "Keep log files (days)";
            this.themeLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.themeLabel.AutoSize = true;
            this.themeLabel.Text = "Theme";
            //
            // outputDirPanel
            //
            this.outputDirPanel.AutoSize = true;
            this.outputDirPanel.ColumnCount = 2;
            this.outputDirPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.outputDirPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.outputDirPanel.Controls.Add(this.outputDirTextBox, 0, 0);
            this.outputDirPanel.Controls.Add(this.browseButton, 1, 0);
            this.outputDirPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputDirPanel.Margin = new System.Windows.Forms.Padding(0);
            this.outputDirPanel.Name = "outputDirPanel";
            this.outputDirPanel.RowCount = 1;
            this.outputDirPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            //
            // outputDirTextBox
            //
            this.outputDirTextBox.AccessibleName = "Default output folder";
            this.outputDirTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.outputDirTextBox.Name = "outputDirTextBox";
            this.outputDirTextBox.TabIndex = 0;
            //
            // browseButton
            //
            this.browseButton.AutoSize = true;
            this.browseButton.Name = "browseButton";
            this.browseButton.TabIndex = 1;
            this.browseButton.Text = "Browse…";
            this.browseButton.Click += new System.EventHandler(this.BrowseButton_Click);
            //
            // combos / numbers
            //
            this.verifyComboBox.AccessibleName = "Automatic verification";
            this.verifyComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.verifyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.verifyComboBox.Name = "verifyComboBox";
            this.verifyComboBox.TabIndex = 2;
            this.speedComboBox.AccessibleName = "Reading speed";
            this.speedComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.speedComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.speedComboBox.Name = "speedComboBox";
            this.speedComboBox.TabIndex = 3;
            this.retriesUpDown.AccessibleName = "Maximum read retries";
            this.retriesUpDown.Maximum = new decimal(new int[] { 20, 0, 0, 0 });
            this.retriesUpDown.Name = "retriesUpDown";
            this.retriesUpDown.Size = new System.Drawing.Size(80, 25);
            this.retriesUpDown.TabIndex = 4;
            this.policyComboBox.AccessibleName = "Unreadable sectors policy";
            this.policyComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.policyComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.policyComboBox.Name = "policyComboBox";
            this.policyComboBox.TabIndex = 5;
            this.blockComboBox.AccessibleName = "Read block size";
            this.blockComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.blockComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.blockComboBox.Name = "blockComboBox";
            this.blockComboBox.TabIndex = 6;
            this.themeComboBox.AccessibleName = "Theme";
            this.themeComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.themeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.themeComboBox.Name = "themeComboBox";
            this.themeComboBox.TabIndex = 8;
            this.retentionUpDown.AccessibleName = "Keep log files (days)";
            this.retentionUpDown.Maximum = new decimal(new int[] { 3650, 0, 0, 0 });
            this.retentionUpDown.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.retentionUpDown.Name = "retentionUpDown";
            this.retentionUpDown.Size = new System.Drawing.Size(80, 25);
            this.retentionUpDown.TabIndex = 7;
            this.retentionUpDown.Value = new decimal(new int[] { 30, 0, 0, 0 });
            //
            // ejectCheckBox
            //
            this.ejectCheckBox.AutoSize = true;
            this.ejectCheckBox.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this.ejectCheckBox.Name = "ejectCheckBox";
            this.ejectCheckBox.TabIndex = 8;
            this.ejectCheckBox.Text = "Eject the disc automatically after a successful rip";
            this.ejectCheckBox.UseVisualStyleBackColor = true;
            //
            // buttonPanel
            //
            this.buttonPanel.AutoSize = true;
            this.buttonPanel.Controls.Add(this.saveButton);
            this.buttonPanel.Controls.Add(this.defaultsButton);
            this.buttonPanel.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.buttonPanel.Name = "buttonPanel";
            //
            // saveButton
            //
            this.saveButton.AutoSize = true;
            this.saveButton.Name = "saveButton";
            this.saveButton.Padding = new System.Windows.Forms.Padding(12, 2, 12, 2);
            this.saveButton.TabIndex = 9;
            this.saveButton.Text = "Save";
            this.saveButton.Click += new System.EventHandler(this.SaveButton_Click);
            //
            // defaultsButton
            //
            this.defaultsButton.AutoSize = true;
            this.defaultsButton.Name = "defaultsButton";
            this.defaultsButton.Padding = new System.Windows.Forms.Padding(6, 2, 6, 2);
            this.defaultsButton.TabIndex = 10;
            this.defaultsButton.Text = "Restore Defaults";
            this.defaultsButton.Click += new System.EventHandler(this.DefaultsButton_Click);
            //
            // statusLabel
            //
            this.statusLabel.AutoSize = true;
            this.statusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(100)))), ((int)(((byte)(114)))));
            this.statusLabel.Margin = new System.Windows.Forms.Padding(0, 16, 0, 0);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Text = "";
            //
            // SettingsView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.layout);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "SettingsView";
            this.Size = new System.Drawing.Size(780, 600);
            this.layout.ResumeLayout(false);
            this.layout.PerformLayout();
            this.outputDirPanel.ResumeLayout(false);
            this.outputDirPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.retriesUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.retentionUpDown)).EndInit();
            this.buttonPanel.ResumeLayout(false);
            this.buttonPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TableLayoutPanel layout;
        private System.Windows.Forms.Label captionLabel;
        private System.Windows.Forms.Label outputDirLabel;
        private System.Windows.Forms.TableLayoutPanel outputDirPanel;
        private System.Windows.Forms.TextBox outputDirTextBox;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Label formatLabel;
        private System.Windows.Forms.Label formatValueLabel;
        private System.Windows.Forms.Label verifyLabel;
        private System.Windows.Forms.ComboBox verifyComboBox;
        private System.Windows.Forms.Label speedLabel;
        private System.Windows.Forms.ComboBox speedComboBox;
        private System.Windows.Forms.Label retriesLabel;
        private System.Windows.Forms.NumericUpDown retriesUpDown;
        private System.Windows.Forms.Label policyLabel;
        private System.Windows.Forms.ComboBox policyComboBox;
        private System.Windows.Forms.Label blockLabel;
        private System.Windows.Forms.ComboBox blockComboBox;
        private System.Windows.Forms.Label themeLabel;
        private System.Windows.Forms.ComboBox themeComboBox;
        private System.Windows.Forms.Label retentionLabel;
        private System.Windows.Forms.NumericUpDown retentionUpDown;
        private System.Windows.Forms.CheckBox ejectCheckBox;
        private System.Windows.Forms.FlowLayoutPanel buttonPanel;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button defaultsButton;
        private System.Windows.Forms.Label statusLabel;
    }
}
