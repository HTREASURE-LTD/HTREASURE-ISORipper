using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace HTISORipper.UI.Controls
{
    /// <summary>White panel with a 1px rounded border, matching the HT ISO Ripper card style.</summary>
    public class CardPanel : Panel
    {
        private int _radius = 8;
        private Color _borderColor = Theme.Border;

        public CardPanel()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            BackColor = Color.White;
            Padding = new Padding(12);
        }

        [DefaultValue(8)]
        public int CornerRadius
        {
            get { return _radius; }
            set { _radius = value; Invalidate(); }
        }

        public Color BorderColor
        {
            get { return _borderColor; }
            set { _borderColor = value; Invalidate(); }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Color outside = Parent != null ? Parent.BackColor : SystemColors.Control;
            e.Graphics.Clear(outside);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle r = new Rectangle(0, 0, Width - 1, Height - 1);
            int radius = LogicalToDeviceUnitsSafe(_radius);
            using (GraphicsPath path = RoundRect(r, radius))
            using (var fill = new SolidBrush(BackColor))
            using (var pen = new Pen(_borderColor))
            {
                e.Graphics.FillPath(fill, path);
                e.Graphics.DrawPath(pen, path);
            }
        }

        private int LogicalToDeviceUnitsSafe(int value)
        {
#if NET461
            // Control.DeviceDpi needs .NET 4.7; the 4.6.1 build is system-DPI aware, so the screen DPI is correct.
            using (Graphics g = CreateGraphics()) return (int)(value * g.DpiX / 96f);
#else
            return (int)(value * DeviceDpi / 96f);
#endif
        }

        internal static GraphicsPath RoundRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            int d = radius * 2;
            if (d <= 0) { path.AddRectangle(r); return path; }
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    /// <summary>Small rounded status badge ("Disc Ready", "No Disc", ...).</summary>
    public class StatusPill : Label
    {
        public StatusPill()
        {
            AutoSize = false;
            TextAlign = ContentAlignment.MiddleCenter;
            Padding = new Padding(8, 2, 8, 2);
            BackColor = Color.Transparent;
            PillColor = Theme.SuccessTint;
            ForeColor = Theme.Success;
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer, true);
        }

        public Color PillColor { get; set; }

        public void SetState(string text, Color foreground, Color background)
        {
            Text = text;
            ForeColor = foreground;
            PillColor = background;
            Size size = TextRenderer.MeasureText(text, Font);
            Width = size.Width + Padding.Horizontal + 6;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle r = new Rectangle(0, 0, Width - 1, Height - 1);
            using (GraphicsPath path = CardPanel.RoundRect(r, Height / 2 - 1))
            using (var brush = new SolidBrush(PillColor))
            {
                e.Graphics.FillPath(brush, path);
            }
            TextRenderer.DrawText(e.Graphics, Text, Font, r, ForeColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine);
        }
    }
}
