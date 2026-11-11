namespace HTISORipper.UI.Forms
{
    partial class LogViewerForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pathLabel = new System.Windows.Forms.Label();
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.closeButton = new System.Windows.Forms.Button();
            this.openFolderButton = new System.Windows.Forms.Button();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // pathLabel
            //
            this.pathLabel.AutoEllipsis = true;
            this.pathLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Padding = new System.Windows.Forms.Padding(8, 6, 8, 0);
            this.pathLabel.Size = new System.Drawing.Size(860, 28);
            //
            // logTextBox
            //
            this.logTextBox.BackColor = System.Drawing.Color.White;
            this.logTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logTextBox.Font = new System.Drawing.Font("Consolas", 9F);
            this.logTextBox.Multiline = true;
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.logTextBox.TabIndex = 0;
            this.logTextBox.WordWrap = false;
            //
            // buttonPanel
            //
            this.buttonPanel.AutoSize = true;
            this.buttonPanel.Controls.Add(this.closeButton);
            this.buttonPanel.Controls.Add(this.openFolderButton);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Padding = new System.Windows.Forms.Padding(6);
            this.buttonPanel.TabIndex = 1;
            //
            // closeButton
            //
            this.closeButton.AutoSize = true;
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Name = "closeButton";
            this.closeButton.Padding = new System.Windows.Forms.Padding(6, 2, 6, 2);
            this.closeButton.TabIndex = 1;
            this.closeButton.Text = "Close";
            this.closeButton.Click += new System.EventHandler(this.CloseButton_Click);
            //
            // openFolderButton
            //
            this.openFolderButton.AutoSize = true;
            this.openFolderButton.Name = "openFolderButton";
            this.openFolderButton.Padding = new System.Windows.Forms.Padding(6, 2, 6, 2);
            this.openFolderButton.TabIndex = 0;
            this.openFolderButton.Text = "Open Log Folder";
            this.openFolderButton.Click += new System.EventHandler(this.OpenFolderButton_Click);
            //
            // LogViewerForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(860, 560);
            this.Controls.Add(this.logTextBox);
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this.pathLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.MinimizeBox = false;
            this.Name = "LogViewerForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "HT ISO Ripper - Log";
            this.buttonPanel.ResumeLayout(false);
            this.buttonPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.TextBox logTextBox;
        private System.Windows.Forms.FlowLayoutPanel buttonPanel;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button openFolderButton;
    }
}
