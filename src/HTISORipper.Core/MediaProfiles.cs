namespace HTISORipper.Core
{
    /// <summary>MMC "current profile" codes reported by GET CONFIGURATION.</summary>
    public static class MediaProfiles
    {
        public static string Name(int profile)
        {
            switch (profile)
            {
                case 0x0000: return "Unknown";
                case 0x0008: return "CD-ROM";
                case 0x0009: return "CD-R";
                case 0x000A: return "CD-RW";
                case 0x0010: return "DVD-ROM";
                case 0x0011: return "DVD-R";
                case 0x0012: return "DVD-RAM";
                case 0x0013: return "DVD-RW (restricted overwrite)";
                case 0x0014: return "DVD-RW";
                case 0x0015: return "DVD-R DL";
                case 0x0016: return "DVD-R DL (layer jump)";
                case 0x0017: return "DVD-RW DL";
                case 0x001A: return "DVD+RW";
                case 0x001B: return "DVD+R";
                case 0x002A: return "DVD+RW DL";
                case 0x002B: return "DVD+R DL";
                case 0x0040: return "BD-ROM";
                case 0x0041: return "BD-R";
                case 0x0042: return "BD-R (random)";
                case 0x0043: return "BD-RE";
                default: return "Profile 0x" + profile.ToString("X4");
            }
        }

        public static MediaFamily Family(int profile)
        {
            if (profile >= 0x0008 && profile <= 0x000A) return MediaFamily.CD;
            if (profile >= 0x0010 && profile <= 0x002B) return MediaFamily.DVD;
            if (profile >= 0x0040 && profile <= 0x0043) return MediaFamily.BluRay;
            if (profile == 0) return MediaFamily.Unknown;
            return MediaFamily.Other;
        }
    }
}
