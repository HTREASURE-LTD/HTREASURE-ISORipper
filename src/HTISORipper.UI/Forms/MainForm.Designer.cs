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
            this.components = new System.ComponentModel.Container();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.headerIcon = new System.Windows.Forms.PictureBox();
            this.headerLabel = new System.Windows.Forms.Label();
            this.subHeaderLabel = new System.Windows.Forms.Label();
            this.websiteLinkLabel = new System.Windows.Forms.LinkLabel();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.startRippingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chooseOutputMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openOutputFolderMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fileSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshDrivesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.discInfoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ejectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.viewLogMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openLogFolderMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.settingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.userGuideMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.troubleshootingMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.releaseNotesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.licenceMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noticesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusStripLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusRightLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.headerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.headerIcon)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            //
            // headerPanel
            //
            this.headerPanel.Controls.Add(this.headerIcon);
            this.headerPanel.Controls.Add(this.headerLabel);
            this.headerPanel.Controls.Add(this.subHeaderLabel);
            this.headerPanel.Controls.Add(this.websiteLinkLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Size = new System.Drawing.Size(784, 44);
            this.headerPanel.TabIndex = 1;
            this.headerPanel.Tag = "header";
            //
            // headerIcon
            //
            this.headerIcon.Location = new System.Drawing.Point(16, 4);
            this.headerIcon.Name = "headerIcon";
            this.headerIcon.Size = new System.Drawing.Size(36, 36);
            this.headerIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.headerIcon.TabIndex = 0;
            this.headerIcon.TabStop = false;
            //
            // headerLabel
            //
            this.headerLabel.AutoSize = true;
            this.headerLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headerLabel.Location = new System.Drawing.Point(60, 1);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(132, 25);
            this.headerLabel.TabIndex = 1;
            this.headerLabel.Text = "HT ISO Ripper";
            //
            // subHeaderLabel
            //
            this.subHeaderLabel.AutoSize = true;
            this.subHeaderLabel.Location = new System.Drawing.Point(62, 27);
            this.subHeaderLabel.Name = "subHeaderLabel";
            this.subHeaderLabel.Size = new System.Drawing.Size(380, 15);
            this.subHeaderLabel.TabIndex = 2;
            this.subHeaderLabel.Tag = "dim";
            this.subHeaderLabel.Text = "Create an exact ISO image from a CD or DVD. Insert a disc to begin.";
            //
            // websiteLinkLabel
            //
            this.websiteLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.websiteLinkLabel.Location = new System.Drawing.Point(608, 13);
            this.websiteLinkLabel.Name = "websiteLinkLabel";
            this.websiteLinkLabel.Size = new System.Drawing.Size(160, 18);
            this.websiteLinkLabel.TabIndex = 3;
            this.websiteLinkLabel.TabStop = true;
            this.websiteLinkLabel.Text = "www.htreasure.com";
            this.websiteLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip.SetToolTip(this.websiteLinkLabel, "Open the HTREASURE LTD website (https://www.htreasure.com)");
            this.websiteLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.WebsiteLinkLabel_LinkClicked);
            //
            // menuStrip
            //
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.fileMenu, this.toolsMenu, this.helpMenu });
            this.menuStrip.Location = new System.Drawing.Point(0, 44);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(784, 24);
            this.menuStrip.TabIndex = 2;
            //
            // fileMenu
            //
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.startRippingMenuItem, this.chooseOutputMenuItem, this.openOutputFolderMenuItem, this.fileSeparator, this.exitMenuItem });
            this.fileMenu.Name = "fileMenu";
            this.fileMenu.Text = "&File";
            this.startRippingMenuItem.Name = "startRippingMenuItem";
            this.startRippingMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.startRippingMenuItem.Text = "&Start Ripping";
            this.startRippingMenuItem.Click += new System.EventHandler(this.StartRippingMenuItem_Click);
            this.chooseOutputMenuItem.Name = "chooseOutputMenuItem";
            this.chooseOutputMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.chooseOutputMenuItem.Text = "Choose &Output File...";
            this.chooseOutputMenuItem.Click += new System.EventHandler(this.ChooseOutputMenuItem_Click);
            this.openOutputFolderMenuItem.Name = "openOutputFolderMenuItem";
            this.openOutputFolderMenuItem.Text = "Open Output &Folder";
            this.openOutputFolderMenuItem.Click += new System.EventHandler(this.OpenOutputFolderMenuItem_Click);
            this.fileSeparator.Name = "fileSeparator";
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Text = "E&xit";
            this.exitMenuItem.Click += new System.EventHandler(this.ExitMenuItem_Click);
            //
            // toolsMenu
            //
            this.toolsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.refreshDrivesMenuItem, this.discInfoMenuItem, this.ejectMenuItem, this.toolsSeparator1, this.viewLogMenuItem, this.openLogFolderMenuItem, this.toolsSeparator2, this.settingsMenuItem });
            this.toolsMenu.Name = "toolsMenu";
            this.toolsMenu.Text = "&Tools";
            this.refreshDrivesMenuItem.Name = "refreshDrivesMenuItem";
            this.refreshDrivesMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.refreshDrivesMenuItem.Text = "&Refresh Drives";
            this.refreshDrivesMenuItem.Click += new System.EventHandler(this.RefreshDrivesMenuItem_Click);
            this.discInfoMenuItem.Name = "discInfoMenuItem";
            this.discInfoMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.discInfoMenuItem.Text = "Disc &Information...";
            this.discInfoMenuItem.Click += new System.EventHandler(this.DiscInfoMenuItem_Click);
            this.ejectMenuItem.Name = "ejectMenuItem";
            this.ejectMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.ejectMenuItem.Text = "&Eject Disc";
            this.ejectMenuItem.Click += new System.EventHandler(this.EjectMenuItem_Click);
            this.toolsSeparator1.Name = "toolsSeparator1";
            this.viewLogMenuItem.Name = "viewLogMenuItem";
            this.viewLogMenuItem.Text = "View &Log...";
            this.viewLogMenuItem.Click += new System.EventHandler(this.ViewLogMenuItem_Click);
            this.openLogFolderMenuItem.Name = "openLogFolderMenuItem";
            this.openLogFolderMenuItem.Text = "Open Log &Folder";
            this.openLogFolderMenuItem.Click += new System.EventHandler(this.OpenLogFolderMenuItem_Click);
            this.toolsSeparator2.Name = "toolsSeparator2";
            this.settingsMenuItem.Name = "settingsMenuItem";
            this.settingsMenuItem.Text = "&Settings...";
            this.settingsMenuItem.Click += new System.EventHandler(this.SettingsMenuItem_Click);
            //
            // helpMenu
            //
            this.helpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { this.userGuideMenuItem, this.troubleshootingMenuItem, this.releaseNotesMenuItem, this.licenceMenuItem, this.noticesMenuItem, this.helpSeparator, this.aboutMenuItem });
            this.helpMenu.Name = "helpMenu";
            this.helpMenu.Text = "&Help";
            this.userGuideMenuItem.Name = "userGuideMenuItem";
            this.userGuideMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.userGuideMenuItem.Tag = "USER-GUIDE.md";
            this.userGuideMenuItem.Text = "&User Guide";
            this.userGuideMenuItem.Click += new System.EventHandler(this.DocumentMenuItem_Click);
            this.troubleshootingMenuItem.Name = "troubleshootingMenuItem";
            this.troubleshootingMenuItem.Tag = "TROUBLESHOOTING.md";
            this.troubleshootingMenuItem.Text = "&Troubleshooting";
            this.troubleshootingMenuItem.Click += new System.EventHandler(this.DocumentMenuItem_Click);
            this.releaseNotesMenuItem.Name = "releaseNotesMenuItem";
            this.releaseNotesMenuItem.Tag = "RELEASE-NOTES.md";
            this.releaseNotesMenuItem.Text = "&Release Notes";
            this.releaseNotesMenuItem.Click += new System.EventHandler(this.DocumentMenuItem_Click);
            this.licenceMenuItem.Name = "licenceMenuItem";
            this.licenceMenuItem.Tag = "LICENSE.txt";
            this.licenceMenuItem.Text = "&Licence";
            this.licenceMenuItem.Click += new System.EventHandler(this.DocumentMenuItem_Click);
            this.noticesMenuItem.Name = "noticesMenuItem";
            this.noticesMenuItem.Tag = "THIRD-PARTY-LICENSES.md";
            this.noticesMenuItem.Text = "Third-Party &Notices";
            this.noticesMenuItem.Click += new System.EventHandler(this.DocumentMenuItem_Click);
            this.helpSeparator.Name = "helpSeparator";
            this.aboutMenuItem.Name = "aboutMenuItem";
            this.aboutMenuItem.Text = "&About HT ISO Ripper";
            this.aboutMenuItem.Click += new System.EventHandler(this.AboutMenuItem_Click);
            //
            // statusStrip
            //
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.statusStripLabel, this.statusRightLabel });
            this.statusStrip.Location = new System.Drawing.Point(0, 634);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(784, 22);
            this.statusStrip.TabIndex = 3;
            this.statusStripLabel.Name = "statusStripLabel";
            this.statusStripLabel.Spring = true;
            this.statusStripLabel.Text = "Ready";
            this.statusStripLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.statusRightLabel.Name = "statusRightLabel";
            this.statusRightLabel.Text = "";
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 656);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.headerPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(740, 695);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HT ISO Ripper";
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.headerIcon)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.PictureBox headerIcon;
        private System.Windows.Forms.Label headerLabel;
        private System.Windows.Forms.Label subHeaderLabel;
        private System.Windows.Forms.LinkLabel websiteLinkLabel;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem startRippingMenuItem;
        private System.Windows.Forms.ToolStripMenuItem chooseOutputMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openOutputFolderMenuItem;
        private System.Windows.Forms.ToolStripSeparator fileSeparator;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsMenu;
        private System.Windows.Forms.ToolStripMenuItem refreshDrivesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem discInfoMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ejectMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolsSeparator1;
        private System.Windows.Forms.ToolStripMenuItem viewLogMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openLogFolderMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolsSeparator2;
        private System.Windows.Forms.ToolStripMenuItem settingsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpMenu;
        private System.Windows.Forms.ToolStripMenuItem userGuideMenuItem;
        private System.Windows.Forms.ToolStripMenuItem troubleshootingMenuItem;
        private System.Windows.Forms.ToolStripMenuItem releaseNotesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem licenceMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noticesMenuItem;
        private System.Windows.Forms.ToolStripSeparator helpSeparator;
        private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusStripLabel;
        private System.Windows.Forms.ToolStripStatusLabel statusRightLabel;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
