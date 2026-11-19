using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace HTISORipper.UI
{
    /// <summary>HTREASURE LTD brand palette and shared styling helpers.</summary>
    internal static class Theme
    {
        public static readonly Color Navy = Color.FromArgb(22, 58, 110);   // HTREASURE design template accent (shared with HT ISO Creator)
        public static readonly Color Blue = ColorTranslator.FromHtml("#1976D2");
        public static readonly Color White = Color.White;
        public static readonly Color LightGrey = ColorTranslator.FromHtml("#F3F6FA");
        public static readonly Color Success = ColorTranslator.FromHtml("#28A745");
        public static readonly Color Error = ColorTranslator.FromHtml("#DC3545");
        public static readonly Color Warning = ColorTranslator.FromHtml("#FF9800");

        public static readonly Color Text = ColorTranslator.FromHtml("#1F2937");
        public static readonly Color Muted = ColorTranslator.FromHtml("#5B6472");
        public static readonly Color Border = ColorTranslator.FromHtml("#D6DEEA");
        public static readonly Color NavyHover = ColorTranslator.FromHtml("#0D4AB0");
        public static readonly Color SuccessTint = ColorTranslator.FromHtml("#E6F4EA");
        public static readonly Color ErrorTint = ColorTranslator.FromHtml("#FBE9EB");
        public static readonly Color WarningTint = ColorTranslator.FromHtml("#FFF3E0");
        public static readonly Color InfoTint = ColorTranslator.FromHtml("#E3EEFB");

        public const string FontName = "Segoe UI";

        /// <summary>Marks the button as the window's primary action; ThemeManager colours it.</summary>
        public static void StylePrimary(Button button)
        {
            button.Tag = ThemeManager.PrimaryTag;
        }

        /// <summary>Secondary buttons use the standard Windows style in the design template.</summary>
        public static void StyleSecondary(Button button)
        {
        }

        public static Image LoadImage(string name)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("HTISORipper.UI." + name);
            if (stream == null) return null;
            using (stream)
            using (var temp = Image.FromStream(stream))
            {
                return new Bitmap(temp);   // detach from the stream
            }
        }

        public static Icon LoadAppIcon()
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("HTISORipper.UI.htisoripper.ico");
            if (stream == null) return null;
            using (stream) return new Icon(stream);
        }
    }
}
