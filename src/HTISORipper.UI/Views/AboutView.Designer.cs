namespace HTISORipper.UI.Views
{
    partial class AboutView
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.card = new HTISORipper.UI.Controls.CardPanel();
            this.layout = new System.Windows.Forms.TableLayoutPanel();
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.productLabel = new System.Windows.Forms.Label();
            this.taglineLabel = new System.Windows.Forms.Label();
            this.publisherLabel = new System.Windows.Forms.Label();
            this.versionLabel = new System.Windows.Forms.Label();
            this.copyrightLabel = new System.Windows.Forms.Label();
            this.websiteLinkLabel = new System.Windows.Forms.LinkLabel();
            this.noticeLabel = new System.Windows.Forms.Label();
            this.buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.viewLogButton = new System.Windows.Forms.Button();
            this.openLogFolderButton = new System.Windows.Forms.Button();
            this.card.SuspendLayout();
            this.layout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // card
            //
            this.card.Controls.Add(this.layout);
            this.card.Dock = System.Windows.Forms.DockStyle.Fill;
            this.card.Name = "card";
            this.card.Padding = new System.Windows.Forms.Padding(20, 16, 20, 8);
            this.card.BorderColor = System.Drawing.Color.Transparent;
            this.card.TabIndex = 0;
            //
            // layout
            //
            this.layout.BackColor = System.Drawing.Color.White;
            this.layout.ColumnCount = 1;
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.Controls.Add(this.logoPictureBox, 0, 0);
            this.layout.Controls.Add(this.productLabel, 0, 1);
            this.layout.Controls.Add(this.taglineLabel, 0, 2);
            this.layout.Controls.Add(this.publisherLabel, 0, 3);
            this.layout.Controls.Add(this.versionLabel, 0, 4);
            this.layout.Controls.Add(this.copyrightLabel, 0, 5);
            this.layout.Controls.Add(this.websiteLinkLabel, 0, 6);
            this.layout.Controls.Add(this.noticeLabel, 0, 7);
            this.layout.Controls.Add(this.buttonPanel, 0, 8);
            this.layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layout.Name = "layout";
            this.layout.RowCount = 9;
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.TabIndex = 0;
            //
            // logoPictureBox
            //
            this.logoPictureBox.Name = "logoPictureBox";
            this.logoPictureBox.Size = new System.Drawing.Size(72, 72);
            this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.logoPictureBox.TabStop = false;
            //
            // productLabel
            //
            this.productLabel.AutoSize = true;
            this.productLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 18F, System.Drawing.FontStyle.Bold);
            this.productLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(61)))), ((int)(((byte)(145)))));
            this.productLabel.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.productLabel.Name = "productLabel";
            this.productLabel.Tag = "accent";
            this.productLabel.Text = "HT ISO Ripper";
            //
            // taglineLabel
            //
            this.taglineLabel.AutoSize = true;
            this.taglineLabel.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.taglineLabel.Name = "taglineLabel";
            this.taglineLabel.Text = "Professional Disc Imaging Software";
            //
            // publisherLabel
            //
            this.publisherLabel.AutoSize = true;
            this.publisherLabel.Margin = new System.Windows.Forms.Padding(0, 12, 0, 0);
            this.publisherLabel.Name = "publisherLabel";
            this.publisherLabel.Text = "Developed by HTREASURE LTD";
            //
            // versionLabel
            //
            this.versionLabel.AutoSize = true;
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Text = "Version 1.0.0";
            //
            // copyrightLabel
            //
            this.copyrightLabel.AutoSize = true;
            this.copyrightLabel.Name = "copyrightLabel";
            this.copyrightLabel.Text = "© 2026 HTREASURE LTD";
            //
            // websiteLinkLabel
            //
            this.websiteLinkLabel.AutoSize = true;
            this.websiteLinkLabel.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.websiteLinkLabel.Name = "websiteLinkLabel";
            this.websiteLinkLabel.TabStop = true;
            this.websiteLinkLabel.Text = "www.htreasure.com";
            this.websiteLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.WebsiteLinkLabel_LinkClicked);
            //
            // noticeLabel
            //
            this.noticeLabel.AutoSize = true;
            this.noticeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(100)))), ((int)(((byte)(114)))));
            this.noticeLabel.Margin = new System.Windows.Forms.Padding(0, 14, 0, 0);
            this.noticeLabel.MaximumSize = new System.Drawing.Size(500, 0);
            this.noticeLabel.Name = "noticeLabel";
            this.noticeLabel.Tag = "dim";
            //
            // buttonPanel
            //
            this.buttonPanel.AutoSize = true;
            this.buttonPanel.Controls.Add(this.viewLogButton);
            this.buttonPanel.Controls.Add(this.openLogFolderButton);
            this.buttonPanel.Margin = new System.Windows.Forms.Padding(0, 16, 0, 0);
            this.buttonPanel.Name = "buttonPanel";
            //
            // viewLogButton
            //
            this.viewLogButton.AutoSize = true;
            this.viewLogButton.Name = "viewLogButton";
            this.viewLogButton.Padding = new System.Windows.Forms.Padding(6, 2, 6, 2);
            this.viewLogButton.TabIndex = 0;
            this.viewLogButton.Text = "View Log";
            this.viewLogButton.Click += new System.EventHandler(this.ViewLogButton_Click);
            //
            // openLogFolderButton
            //
            this.openLogFolderButton.AutoSize = true;
            this.openLogFolderButton.Name = "openLogFolderButton";
            this.openLogFolderButton.Padding = new System.Windows.Forms.Padding(6, 2, 6, 2);
            this.openLogFolderButton.TabIndex = 1;
            this.openLogFolderButton.Text = "Open Log Folder";
            this.openLogFolderButton.Click += new System.EventHandler(this.OpenLogFolderButton_Click);
            //
            // AboutView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.card);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "AboutView";
            this.Size = new System.Drawing.Size(780, 600);
            this.card.ResumeLayout(false);
            this.layout.ResumeLayout(false);
            this.layout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
            this.buttonPanel.ResumeLayout(false);
            this.buttonPanel.PerformLayout();
            this.ResumeLayout(false);
        }

        private HTISORipper.UI.Controls.CardPanel card;
        private System.Windows.Forms.TableLayoutPanel layout;
        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.Label productLabel;
        private System.Windows.Forms.Label taglineLabel;
        private System.Windows.Forms.Label publisherLabel;
        private System.Windows.Forms.Label versionLabel;
        private System.Windows.Forms.Label copyrightLabel;
        private System.Windows.Forms.LinkLabel websiteLinkLabel;
        private System.Windows.Forms.Label noticeLabel;
        private System.Windows.Forms.FlowLayoutPanel buttonPanel;
        private System.Windows.Forms.Button viewLogButton;
        private System.Windows.Forms.Button openLogFolderButton;
    }
}
