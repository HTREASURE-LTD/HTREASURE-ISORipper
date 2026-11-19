namespace HTISORipper.UI.Forms
{
    partial class CompletionForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.headlinePanel = new System.Windows.Forms.Panel();
            this.headlineLabel = new System.Windows.Forms.Label();
            this.detailLabel = new System.Windows.Forms.Label();
            this.detailsListView = new System.Windows.Forms.ListView();
            this.nameColumn = new System.Windows.Forms.ColumnHeader();
            this.valueColumn = new System.Windows.Forms.ColumnHeader();
            this.shaPanel = new System.Windows.Forms.TableLayoutPanel();
            this.shaLabel = new System.Windows.Forms.Label();
            this.shaTextBox = new System.Windows.Forms.TextBox();
            this.buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.closeButton = new System.Windows.Forms.Button();
            this.ripAnotherButton = new System.Windows.Forms.Button();
            this.ejectButton = new System.Windows.Forms.Button();
            this.viewLogButton = new System.Windows.Forms.Button();
            this.copyShaButton = new System.Windows.Forms.Button();
            this.openFolderButton = new System.Windows.Forms.Button();
            this.headlinePanel.SuspendLayout();
            this.shaPanel.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            //
            // headlinePanel
            //
            this.headlinePanel.Controls.Add(this.headlineLabel);
            this.headlinePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headlinePanel.Name = "headlinePanel";
            this.headlinePanel.Padding = new System.Windows.Forms.Padding(16, 0, 16, 0);
            this.headlinePanel.Size = new System.Drawing.Size(720, 52);
            this.headlinePanel.TabIndex = 0;
            //
            // headlineLabel
            //
            this.headlineLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.headlineLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 12.75F, System.Drawing.FontStyle.Bold);
            this.headlineLabel.ForeColor = System.Drawing.Color.White;
            this.headlineLabel.Name = "headlineLabel";
            this.headlineLabel.Text = "RESULT";
            this.headlineLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // detailLabel
            //
            this.detailLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.detailLabel.Name = "detailLabel";
            this.detailLabel.Padding = new System.Windows.Forms.Padding(16, 10, 16, 6);
            this.detailLabel.Size = new System.Drawing.Size(720, 58);
            this.detailLabel.Text = "Detail";
            //
            // detailsListView
            //
            this.detailsListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.detailsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { this.nameColumn, this.valueColumn });
            this.detailsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.detailsListView.FullRowSelect = true;
            this.detailsListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.detailsListView.HideSelection = false;
            this.detailsListView.Name = "detailsListView";
            this.detailsListView.TabIndex = 1;
            this.detailsListView.UseCompatibleStateImageBehavior = false;
            this.detailsListView.View = System.Windows.Forms.View.Details;
            //
            // nameColumn
            //
            this.nameColumn.Text = "Property";
            this.nameColumn.Width = 150;
            //
            // valueColumn
            //
            this.valueColumn.Text = "Value";
            this.valueColumn.Width = 520;
            //
            // shaPanel
            //
            this.shaPanel.ColumnCount = 2;
            this.shaPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.shaPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.shaPanel.Controls.Add(this.shaLabel, 0, 0);
            this.shaPanel.Controls.Add(this.shaTextBox, 1, 0);
            this.shaPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.shaPanel.Name = "shaPanel";
            this.shaPanel.Padding = new System.Windows.Forms.Padding(12, 6, 12, 0);
            this.shaPanel.RowCount = 1;
            this.shaPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.shaPanel.Size = new System.Drawing.Size(720, 36);
            this.shaPanel.TabIndex = 2;
            //
            // shaLabel
            //
            this.shaLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.shaLabel.AutoSize = true;
            this.shaLabel.Name = "shaLabel";
            this.shaLabel.Text = "SHA-256";
            //
            // shaTextBox
            //
            this.shaTextBox.AccessibleName = "SHA-256 checksum";
            this.shaTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.shaTextBox.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.shaTextBox.Name = "shaTextBox";
            this.shaTextBox.ReadOnly = true;
            this.shaTextBox.TabIndex = 0;
            //
            // buttonPanel
            //
            this.buttonPanel.AutoSize = true;
            this.buttonPanel.Controls.Add(this.closeButton);
            this.buttonPanel.Controls.Add(this.ripAnotherButton);
            this.buttonPanel.Controls.Add(this.ejectButton);
            this.buttonPanel.Controls.Add(this.viewLogButton);
            this.buttonPanel.Controls.Add(this.copyShaButton);
            this.buttonPanel.Controls.Add(this.openFolderButton);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Padding = new System.Windows.Forms.Padding(10, 8, 10, 10);
            this.buttonPanel.TabIndex = 3;
            //
            // closeButton
            //
            this.closeButton.AutoSize = true;
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Name = "closeButton";
            this.closeButton.Padding = new System.Windows.Forms.Padding(6, 2, 6, 2);
            this.closeButton.TabIndex = 5;
            this.closeButton.Text = "Close";
            this.closeButton.Click += new System.EventHandler(this.CloseButton_Click);
            //
            // ripAnotherButton
            //
            this.ripAnotherButton.AutoSize = true;
            this.ripAnotherButton.Name = "ripAnotherButton";
            this.ripAnotherButton.Padding = new System.Windows.Forms.Padding(6, 2, 6, 2);
            this.ripAnotherButton.TabIndex = 4;
            this.ripAnotherButton.Text = "Rip Another Disc";
            this.ripAnotherButton.Click += new System.EventHandler(this.RipAnotherButton_Click);
            //
            // ejectButton
            //
            this.ejectButton.AutoSize = true;
            this.ejectButton.Name = "ejectButton";
            this.ejectButton.Padding = new System.Windows.Forms.Padding(6, 2, 6, 2);
            this.ejectButton.TabIndex = 3;
            this.ejectButton.Text = "Eject Disc";
            this.ejectButton.Click += new System.EventHandler(this.EjectButton_Click);
            //
            // viewLogButton
            //
            this.viewLogButton.AutoSize = true;
            this.viewLogButton.Name = "viewLogButton";
            this.viewLogButton.Padding = new System.Windows.Forms.Padding(6, 2, 6, 2);
            this.viewLogButton.TabIndex = 2;
            this.viewLogButton.Text = "View Log";
            this.viewLogButton.Click += new System.EventHandler(this.ViewLogButton_Click);
            //
            // copyShaButton
            //
            this.copyShaButton.AutoSize = true;
            this.copyShaButton.Name = "copyShaButton";
            this.copyShaButton.Padding = new System.Windows.Forms.Padding(6, 2, 6, 2);
            this.copyShaButton.TabIndex = 1;
            this.copyShaButton.Text = "Copy SHA-256";
            this.copyShaButton.Click += new System.EventHandler(this.CopyShaButton_Click);
            //
            // openFolderButton
            //
            this.openFolderButton.AutoSize = true;
            this.openFolderButton.Name = "openFolderButton";
            this.openFolderButton.Padding = new System.Windows.Forms.Padding(6, 2, 6, 2);
            this.openFolderButton.TabIndex = 0;
            this.openFolderButton.Text = "Open Output Folder";
            this.openFolderButton.Click += new System.EventHandler(this.OpenFolderButton_Click);
            //
            // CompletionForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(720, 470);
            this.Controls.Add(this.detailsListView);
            this.Controls.Add(this.shaPanel);
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this.detailLabel);
            this.Controls.Add(this.headlinePanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(640, 420);
            this.Name = "CompletionForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "HT ISO Ripper - Result";
            this.headlinePanel.ResumeLayout(false);
            this.shaPanel.ResumeLayout(false);
            this.shaPanel.PerformLayout();
            this.buttonPanel.ResumeLayout(false);
            this.buttonPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Panel headlinePanel;
        private System.Windows.Forms.Label headlineLabel;
        private System.Windows.Forms.Label detailLabel;
        private System.Windows.Forms.ListView detailsListView;
        private System.Windows.Forms.ColumnHeader nameColumn;
        private System.Windows.Forms.ColumnHeader valueColumn;
        private System.Windows.Forms.TableLayoutPanel shaPanel;
        private System.Windows.Forms.Label shaLabel;
        private System.Windows.Forms.TextBox shaTextBox;
        private System.Windows.Forms.FlowLayoutPanel buttonPanel;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button ripAnotherButton;
        private System.Windows.Forms.Button ejectButton;
        private System.Windows.Forms.Button viewLogButton;
        private System.Windows.Forms.Button copyShaButton;
        private System.Windows.Forms.Button openFolderButton;
    }
}
