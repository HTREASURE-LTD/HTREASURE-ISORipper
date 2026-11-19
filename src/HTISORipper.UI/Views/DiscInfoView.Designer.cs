namespace HTISORipper.UI.Views
{
    partial class DiscInfoView
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
            this.propertiesCaption = new System.Windows.Forms.Label();
            this.propertiesListView = new System.Windows.Forms.ListView();
            this.propertyColumn = new System.Windows.Forms.ColumnHeader();
            this.valueColumn = new System.Windows.Forms.ColumnHeader();
            this.tracksCaption = new System.Windows.Forms.Label();
            this.tracksListView = new System.Windows.Forms.ListView();
            this.trackColumn = new System.Windows.Forms.ColumnHeader();
            this.typeColumn = new System.Windows.Forms.ColumnHeader();
            this.sessionColumn = new System.Windows.Forms.ColumnHeader();
            this.startColumn = new System.Windows.Forms.ColumnHeader();
            this.lengthColumn = new System.Windows.Forms.ColumnHeader();
            this.sizeColumn = new System.Windows.Forms.ColumnHeader();
            this.layout.SuspendLayout();
            this.SuspendLayout();
            //
            // layout
            //
            this.layout.ColumnCount = 1;
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.Controls.Add(this.propertiesCaption, 0, 0);
            this.layout.Controls.Add(this.propertiesListView, 0, 1);
            this.layout.Controls.Add(this.tracksCaption, 0, 2);
            this.layout.Controls.Add(this.tracksListView, 0, 3);
            this.layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layout.Name = "layout";
            this.layout.Padding = new System.Windows.Forms.Padding(16, 8, 16, 16);
            this.layout.RowCount = 4;
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 65F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.layout.TabIndex = 0;
            //
            // propertiesCaption
            //
            this.propertiesCaption.AutoSize = true;
            this.propertiesCaption.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.propertiesCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(100)))), ((int)(((byte)(114)))));
            this.propertiesCaption.Margin = new System.Windows.Forms.Padding(0, 8, 0, 4);
            this.propertiesCaption.Name = "propertiesCaption";
            this.propertiesCaption.Text = "DISC INFORMATION";
            //
            // propertiesListView
            //
            this.propertiesListView.AccessibleName = "Disc information";
            this.propertiesListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.propertiesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { this.propertyColumn, this.valueColumn });
            this.propertiesListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesListView.FullRowSelect = true;
            this.propertiesListView.HideSelection = false;
            this.propertiesListView.Margin = new System.Windows.Forms.Padding(0);
            this.propertiesListView.Name = "propertiesListView";
            this.propertiesListView.TabIndex = 0;
            this.propertiesListView.UseCompatibleStateImageBehavior = false;
            this.propertiesListView.View = System.Windows.Forms.View.Details;
            //
            // propertyColumn
            //
            this.propertyColumn.Text = "Property";
            this.propertyColumn.Width = 200;
            //
            // valueColumn
            //
            this.valueColumn.Text = "Value";
            this.valueColumn.Width = 500;
            //
            // tracksCaption
            //
            this.tracksCaption.AutoSize = true;
            this.tracksCaption.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.tracksCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(91)))), ((int)(((byte)(100)))), ((int)(((byte)(114)))));
            this.tracksCaption.Margin = new System.Windows.Forms.Padding(0, 14, 0, 4);
            this.tracksCaption.Name = "tracksCaption";
            this.tracksCaption.Text = "TRACKS (TABLE OF CONTENTS)";
            //
            // tracksListView
            //
            this.tracksListView.AccessibleName = "Tracks";
            this.tracksListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tracksListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] { this.trackColumn, this.typeColumn, this.sessionColumn, this.startColumn, this.lengthColumn, this.sizeColumn });
            this.tracksListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tracksListView.FullRowSelect = true;
            this.tracksListView.Margin = new System.Windows.Forms.Padding(0);
            this.tracksListView.Name = "tracksListView";
            this.tracksListView.TabIndex = 1;
            this.tracksListView.UseCompatibleStateImageBehavior = false;
            this.tracksListView.View = System.Windows.Forms.View.Details;
            //
            // track columns
            //
            this.trackColumn.Text = "Track";
            this.trackColumn.Width = 60;
            this.typeColumn.Text = "Type";
            this.typeColumn.Width = 80;
            this.sessionColumn.Text = "Session";
            this.sessionColumn.Width = 70;
            this.startColumn.Text = "Start LBA";
            this.startColumn.Width = 110;
            this.lengthColumn.Text = "Sectors";
            this.lengthColumn.Width = 120;
            this.sizeColumn.Text = "Size";
            this.sizeColumn.Width = 110;
            //
            // DiscInfoView
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(246)))), ((int)(((byte)(250)))));
            this.Controls.Add(this.layout);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "DiscInfoView";
            this.Size = new System.Drawing.Size(780, 600);
            this.layout.ResumeLayout(false);
            this.layout.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.TableLayoutPanel layout;
        private System.Windows.Forms.Label propertiesCaption;
        private System.Windows.Forms.ListView propertiesListView;
        private System.Windows.Forms.ColumnHeader propertyColumn;
        private System.Windows.Forms.ColumnHeader valueColumn;
        private System.Windows.Forms.Label tracksCaption;
        private System.Windows.Forms.ListView tracksListView;
        private System.Windows.Forms.ColumnHeader trackColumn;
        private System.Windows.Forms.ColumnHeader typeColumn;
        private System.Windows.Forms.ColumnHeader sessionColumn;
        private System.Windows.Forms.ColumnHeader startColumn;
        private System.Windows.Forms.ColumnHeader lengthColumn;
        private System.Windows.Forms.ColumnHeader sizeColumn;
    }
}
