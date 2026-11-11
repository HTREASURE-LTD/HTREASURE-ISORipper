using System.Windows.Forms;
using HTISORipper.Core;

namespace HTISORipper.UI.Forms
{
    public partial class SplashForm : Form
    {
        public SplashForm()
        {
            InitializeComponent();
            logoPictureBox.Image = Theme.LoadImage("ht_logo.png");
            versionLabel.Text = "Version " + AppInfo.Version + "   " + AppInfo.Copyright;
        }
    }
}
