using System.Globalization;
using System.Windows.Forms;
using HTISORipper.Core;

namespace HTISORipper.UI.Views
{
    public partial class DiscInfoView : UserControl
    {
        public DiscInfoView()
        {
            InitializeComponent();
            ShowDisc(null, null, "No disc information yet");
        }

        public void ShowDisc(OpticalDriveInfo drive, DiscInfo disc, string stateText)
        {
            propertiesListView.BeginUpdate();
            tracksListView.BeginUpdate();
            propertiesListView.Items.Clear();
            tracksListView.Items.Clear();

            if (drive != null)
            {
                Add("Drive", drive.RootPath);
                Add("Device", drive.DeviceName + (string.IsNullOrEmpty(drive.Revision) ? string.Empty : "  (firmware " + drive.Revision + ")"));
                if (!string.IsNullOrEmpty(drive.BusType)) Add("Connection", drive.BusType);
            }
            Add("Disc status", stateText);

            if (disc != null)
            {
                Add("Disc label", disc.Label);
                Add("Disc type", disc.MediaTypeName + (disc.ProfileCode != 0 ? "  (MMC profile 0x" + disc.ProfileCode.ToString("X4") + ")" : string.Empty));
                Add("File system", disc.Filesystem.Describe());
                if (disc.Filesystem.HasIso9660) Add("ISO 9660 volume", disc.Filesystem.Iso9660VolumeId + "  •  " + Format.Bytes(disc.Filesystem.Iso9660VolumeBytes));
                Add("Number of sessions", disc.SessionCount.ToString(CultureInfo.CurrentCulture));
                Add("Number of tracks", disc.Tracks.Count + "  (" + disc.DataTrackCount + " data, " + disc.AudioTrackCount + " audio)");
                Add("Sector size", disc.SectorSize + " bytes (user data)");
                Add("Readable sector count", disc.ImageSectors.ToString("N0", CultureInfo.CurrentCulture));
                Add("Recorded size (image size)", Format.BytesWithExact(disc.ImageBytes));
                Add("File system data size", disc.FilesystemBytes > 0 ? Format.Bytes(disc.FilesystemBytes) : "Unknown");
                Add("Copy protection", disc.IsCopyProtected ? "Reported (not supported)" : "None reported");
                Add("Supported output formats", disc.CanCreateIso ? "ISO (.iso)" :
                    disc.RecommendedFormat == ImageFormat.BinCue ? "None in this version - requires BIN/CUE (planned)" : "None");
                if (!disc.CanCreateIso) Add("Reason", disc.UnsupportedReason);
                foreach (string w in disc.Warnings) Add("Note", w);

                foreach (TrackInfo t in disc.Tracks)
                {
                    var item = new ListViewItem(t.Number.ToString(CultureInfo.CurrentCulture));
                    item.SubItems.Add(t.IsData ? "Data" : "Audio");
                    item.SubItems.Add(t.Session.ToString(CultureInfo.CurrentCulture));
                    item.SubItems.Add(t.StartLba.ToString("N0", CultureInfo.CurrentCulture));
                    item.SubItems.Add(t.LengthSectors.ToString("N0", CultureInfo.CurrentCulture));
                    item.SubItems.Add(Format.Bytes(t.LengthSectors * 2048));
                    tracksListView.Items.Add(item);
                }
            }

            propertiesListView.EndUpdate();
            tracksListView.EndUpdate();
            propertiesListView.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);
            propertiesListView.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void Add(string name, string value)
        {
            var item = new ListViewItem(name);
            item.SubItems.Add(value ?? "-");
            propertiesListView.Items.Add(item);
        }
    }
}
