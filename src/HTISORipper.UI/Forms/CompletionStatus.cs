using System.Drawing;
using HTISORipper.Core;

namespace HTISORipper.UI.Forms
{
    /// <summary>
    /// The single place that decides what result wording the user sees. "VERIFIED SUCCESSFULLY" is only
    /// produced when every sector was read AND the image was compared against the disc with no differences.
    /// </summary>
    internal sealed class CompletionStatus
    {
        public string Headline { get; private set; }
        public string ShortText { get; private set; }
        public string Detail { get; private set; }
        public Color Color { get; private set; }

        public static CompletionStatus Evaluate(ImagingResult result, VerificationReport report)
        {
            var s = new CompletionStatus();
            Color amber = Color.FromArgb(0xB2, 0x5E, 0x00);
            switch (result.Outcome)
            {
                case ImagingOutcome.Success:
                    if (report == null)
                    {
                        s.Set("IMAGE CREATED - NOT VERIFIED", "Not Verified", amber, "The image was written but no verification was performed.");
                    }
                    else if (report.Cancelled)
                    {
                        s.Set("IMAGE CREATED - VERIFICATION CANCELLED", "Not Verified", amber,
                            "All sectors were read, but verification was cancelled. The image has not been verified.");
                    }
                    else
                    {
                        switch (report.Level)
                        {
                            case VerificationLevel.SourceVerified:
                                s.Set("IMAGE CREATED AND VERIFIED SUCCESSFULLY", "Verified", Theme.Success,
                                    "Every sector was read without errors and the image matched the disc when re-read and compared.");
                                break;
                            case VerificationLevel.StructuralOnly:
                                s.Set("IMAGE CREATED - STRUCTURAL CHECK PASSED", "Image Created", Theme.Success,
                                    "All sectors were read without errors. The file was re-read and its SHA-256 and file-system structures checked. "
                                    + "The disc was NOT re-read, so this is not a full source-to-image verification.");
                                break;
                            case VerificationLevel.Failed:
                                s.Set("IMAGE CREATED BUT VERIFICATION FAILED", "Verify Failed", Theme.Error,
                                    "Do not rely on this image. " + string.Join(" ", report.Notes.ToArray()));
                                break;
                            default:
                                s.Set("IMAGE CREATED - NOT VERIFIED", "Not Verified", amber,
                                    "All sectors were read without errors. Verification was switched off; only the file size was checked.");
                                break;
                        }
                    }
                    break;
                case ImagingOutcome.CompletedWithErrors:
                    s.Set("INCOMPLETE IMAGE - " + result.UnreadableLbas.Count + " UNREADABLE SECTOR(S)", "Incomplete", Theme.Error,
                        result.Message + " See the bad-sector report next to the image.");
                    break;
                case ImagingOutcome.Cancelled:
                    s.Set("IMAGING CANCELLED", "Cancelled", amber, result.Message);
                    break;
                default:
                    s.Set("IMAGING FAILED", "Failed", Theme.Error, result.Message);
                    break;
            }
            return s;
        }

        private void Set(string headline, string shortText, Color color, string detail)
        {
            Headline = headline;
            ShortText = shortText;
            Color = color;
            Detail = detail;
        }
    }
}
