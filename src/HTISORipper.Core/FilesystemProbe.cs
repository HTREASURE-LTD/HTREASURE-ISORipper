using System;
using System.Text;

namespace HTISORipper.Core
{
    /// <summary>
    /// Detects ISO 9660, Joliet, El Torito and UDF structures from raw 2048-byte sectors.
    /// Works identically on a physical disc and on an image file, so it is used both for disc
    /// inspection and for structural validation of finished images.
    /// </summary>
    public static class FilesystemProbe
    {
        public const int SectorSize = 2048;
        private const int FirstDescriptorSector = 16;
        private const int MaxDescriptorSectors = 64;
        private const int UdfAnchorSector = 256;

        /// <param name="readSector">Returns the 2048-byte sector at the given LBA, or null if unreadable/out of range.</param>
        public static FilesystemInfo Probe(Func<long, byte[]> readSector)
        {
            if (readSector == null) throw new ArgumentNullException("readSector");
            var info = new FilesystemInfo();
            bool udfBea = false;

            for (long lba = FirstDescriptorSector; lba < FirstDescriptorSector + MaxDescriptorSectors; lba++)
            {
                byte[] s = readSector(lba);
                if (s == null || s.Length < SectorSize) break;

                string id = Ascii(s, 1, 5);
                if (id == "CD001")
                {
                    byte type = s[0];
                    if (type == 0 && Ascii(s, 7, 23) == "EL TORITO SPECIFICATION")
                    {
                        info.HasElTorito = true;
                    }
                    else if (type == 1 && !info.HasIso9660)
                    {
                        info.HasIso9660 = true;
                        info.Iso9660VolumeId = Ascii(s, 40, 32).Trim();
                        info.Iso9660VolumeBlocks = BitConverter.ToUInt32(s, 80);   // little-endian half of both-endian field
                        info.Iso9660BlockSize = BitConverter.ToUInt16(s, 128);
                    }
                    else if (type == 2)
                    {
                        // Supplementary volume descriptor: Joliet escape sequences %/@, %/C or %/E at byte 88.
                        if (s[88] == 0x25 && s[89] == 0x2F && (s[90] == 0x40 || s[90] == 0x43 || s[90] == 0x45))
                            info.HasJoliet = true;
                    }
                    else if (type == 255)
                    {
                        // Volume descriptor set terminator: UDF recognition sequence may follow.
                    }
                    continue;
                }

                if (id == "BEA01") { udfBea = true; continue; }
                if (id == "NSR02" || id == "NSR03")
                {
                    if (udfBea)
                    {
                        info.HasUdf = true;
                        info.UdfRevisionHint = id == "NSR02" ? "1.02-1.50" : "2.00+";
                    }
                    continue;
                }
                if (id == "TEA01") break;
                if (id == "BOOT2" || id == "CDW02") continue;

                // Anything else ends the volume recognition area.
                break;
            }

            byte[] anchor = readSector(UdfAnchorSector);
            if (anchor != null && anchor.Length >= SectorSize)
            {
                ushort tagId = BitConverter.ToUInt16(anchor, 0);
                // Tag identifier 2 = Anchor Volume Descriptor Pointer; verify the tag checksum (byte 4).
                if (tagId == 2 && TagChecksumOk(anchor))
                {
                    info.HasUdfAnchor = true;
                    if (udfBea) info.HasUdf = true;
                }
            }

            if (info.HasIso9660 && (info.Iso9660BlockSize != 2048 && info.Iso9660BlockSize != 0))
            {
                // Non-2048 logical blocks are legal but unusual; size calculations still use the declared value.
            }
            return info;
        }

        private static bool TagChecksumOk(byte[] tag)
        {
            int sum = 0;
            for (int i = 0; i < 16; i++) if (i != 4) sum += tag[i];
            return (byte)sum == tag[4];
        }

        private static string Ascii(byte[] data, int offset, int length)
        {
            if (offset + length > data.Length) return string.Empty;
            return Encoding.ASCII.GetString(data, offset, length);
        }
    }
}
