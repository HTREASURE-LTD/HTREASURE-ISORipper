namespace HTISORipper.UI.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.headerTable = new System.Windows.Forms.TableLayoutPanel();
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.titlePanel = new System.Windows.Forms.Panel();
            this.subtitleLabel = new System.Windows.Forms.Label();
            this.titleLabel = new System.Windows.Forms.Label();
            this.statusPill = new HTISORipper.UI.Controls.StatusPill();
            this.navPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.navDashboardButton = new System.Windows.Forms.Button();
            this.navDiscInfoButton = new System.Windows.Forms.Button();
            this.navSettingsButton = new System.Windows.Forms.Button();
            this.navAboutButton = new System.Windows.Forms.Button();
            this.separator = new System.Windows.Forms.Panel();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.headerTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.titlePanel.SuspendLayout();
            this.navPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // headerTable
            //
            this.headerTable.BackColor = System.Drawing.Color.White;
            this.headerTable.ColumnCount = 3;
            this.headerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.headerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.headerTable.Controls.Add(this.logoPictureBox, 0, 0);
            this.headerTable.Controls.Add(this.titlePanel, 1, 0);
            this.headerTable.Controls.Add(this.statusPill, 2, 0);
            this.headerTable.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerTable.Location = new System.Drawing.Point(0, 0);
            this.headerTable.Name = "headerTable";
            this.headerTable.Padding = new System.Windows.Forms.Padding(16, 12, 16, 8);
            this.headerTable.RowCount = 1;
            this.headerTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.headerTable.Size = new System.Drawing.Size(800, 68);
            this.headerTable.TabIndex = 0;
            //
            // logoPictureBox
            //
            this.logoPictureBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.logoPictureBox.Location = new System.Drawing.Point(19, 14);
            this.logoPictureBox.Name = "logoPictureBox";
            this.logoPictureBox.Size = new System.Drawing.Size(40, 40);
            this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.logoPictureBox.TabStop = false;
            //
            // titlePanel
            //
            this.titlePanel.Controls.Add(this.subtitleLabel);
            this.titlePanel.Controls.Add(this.titleLabel);
            this.titlePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titlePanel.Name = "titlePanel";
            this.titlePanel.TabIndex = 1;
            //
            // titleLabel
            //
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 13F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(41)))), ((int)(((byte)(55)))));
            this.titleLabel.Location = new System.Drawing.Point(0, 2);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Text = "HT ISO Ripper";
            //
            // subtitleLabel
            //
            this.subtitleLabel.AutoSize = true;
            this.subtitleLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(100)))), ((int)(((byte)(114)))));
            this.subtitleLabel.Location = new System.Drawing.Point(1, 27);
            this.subtitleLabel.Name = "subtitleLabel";
            this.subtitleLabel.Text = "HTREASURE LTD  •  Professional Disc Imaging";
            //
            // statusPill
            //
            this.statusPill.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.statusPill.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.statusPill.Name = "statusPill";
            this.statusPill.Size = new System.Drawing.Size(90, 24);
            this.statusPill.TabIndex = 2;
            this.statusPill.Text = "Starting";
            //
            // navPanel
            //
            this.navPanel.BackColor = System.Drawing.Color.White;
            this.navPanel.Controls.Add(this.navDashboardButton);
            this.navPanel.Controls.Add(this.navDiscInfoButton);
            this.navPanel.Controls.Add(this.navSettingsButton);
            this.navPanel.Controls.Add(this.navAboutButton);
            this.navPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.navPanel.Name = "navPanel";
            this.navPanel.Padding = new System.Windows.Forms.Padding(12, 0, 12, 6);
            this.navPanel.Size = new System.Drawing.Size(800, 40);
            this.navPanel.TabIndex = 1;
            this.navPanel.WrapContents = false;
            //
            // navDashboardButton
            //
            this.navDashboardButton.AutoSize = true;
            this.navDashboardButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.navDashboardButton.Name = "navDashboardButton";
            this.navDashboardButton.Padding = new System.Windows.Forms.Padding(8, 2, 8, 2);
            this.navDashboardButton.TabIndex = 0;
            this.navDashboardButton.Text = "Dashboard";
            //
            // navDiscInfoButton
            //
            this.navDiscInfoButton.AutoSize = true;
            this.navDiscInfoButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.navDiscInfoButton.Name = "navDiscInfoButton";
            this.navDiscInfoButton.Padding = new System.Windows.Forms.Padding(8, 2, 8, 2);
            this.navDiscInfoButton.TabIndex = 1;
            this.navDiscInfoButton.Text = "Disc Information";
            //
            // navSettingsButton
            //
            this.navSettingsButton.AutoSize = true;
            this.navSettingsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.navSettingsButton.Name = "navSettingsButton";
            this.navSettingsButton.Padding = new System.Windows.Forms.Padding(8, 2, 8, 2);
            this.navSettingsButton.TabIndex = 2;
            this.navSettingsButton.Text = "Settings";
            //
            // navAboutButton
            //
            this.navAboutButton.AutoSize = true;
            this.navAboutButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.navAboutButton.Name = "navAboutButton";
            this.navAboutButton.Padding = new System.Windows.Forms.Padding(8, 2, 8, 2);
            this.navAboutButton.TabIndex = 3;
            this.navAboutButton.Text = "About";
            //
            // separator
            //
            this.separator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(222)))), ((int)(((byte)(234)))));
            this.separator.Dock = System.Windows.Forms.DockStyle.Top;
            this.separator.Name = "separator";
            this.separator.Size = new System.Drawing.Size(800, 1);
            this.separator.TabIndex = 2;
            //
            // contentPanel
            //
            this.contentPanel.AutoScroll = true;
            this.contentPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.TabIndex = 3;
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(800, 760);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.separator);
            this.Controls.Add(this.navPanel);
            this.Controls.Add(this.headerTable);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.MinimumSize = new System.Drawing.Size(720, 640);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HT ISO Ripper";
            this.headerTable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
            this.titlePanel.ResumeLayout(false);
            this.titlePanel.PerformLayout();
            this.navPanel.ResumeLayout(false);
            this.navPanel.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.TableLayoutPanel headerTable;
        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.Panel titlePanel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label subtitleLabel;
        private HTISORipper.UI.Controls.StatusPill statusPill;
        private System.Windows.Forms.FlowLayoutPanel navPanel;
        private System.Windows.Forms.Button navDashboardButton;
        private System.Windows.Forms.Button navDiscInfoButton;
        private System.Windows.Forms.Button navSettingsButton;
        private System.Windows.Forms.Button navAboutButton;
        private System.Windows.Forms.Panel separator;
        private System.Windows.Forms.Panel contentPanel;
    }
}
