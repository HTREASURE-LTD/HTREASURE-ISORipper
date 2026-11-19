using System;
using System.Drawing;
using System.Windows.Forms;

namespace HTISORipper.UI.Forms
{
    /// <summary>Themed dialog window that hosts one view (Disc Information, Settings, About).</summary>
    public partial class HostForm : Form
    {
        public HostForm()
        {
            InitializeComponent();
        }

        public HostForm(string title, Control content, Size clientSize) : this()
        {
            Icon = Theme.LoadAppIcon();
            Text = title;
            ClientSize = clientSize;
            content.Dock = DockStyle.Fill;
            contentPanel.Controls.Add(content);
            ThemeManager.Apply(this);
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
