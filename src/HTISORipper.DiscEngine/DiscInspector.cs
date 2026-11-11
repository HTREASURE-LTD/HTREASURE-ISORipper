using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HTISORipper.Core;
using HTISORipper.Logging;

namespace HTISORipper.DiscEngine
{
    public static class DriveEnumerator
    {
        /// <summary>Lists optical drives with vendor/product information. Never throws for an individual drive.</summary>
        public static List<OpticalDriveInfo> GetOpticalDrives(ILogger log)
        {
            var result = new List<OpticalDriveInfo>();
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType != DriveType.CDRom) continue;
                var info = new OpticalDriveInfo { DriveLetter = drive.Name.Substring(0, 1).ToUpperInvariant() };
                try
                {
                    using (var device = OpticalDevice.Open(info.DriveLetter, false))
                        device.QueryDeviceDescriptor(info);
                }
                catch (DeviceAccessException ex)
                {
                    log.Warn("Could not query device details for " + info.RootPath + ": " + ex.Message);
                }
                result.Add(info);
            }
            return result;
        }

        public static DriveMediaState GetMediaState(string driveLetter)
        {
            try
            {
                using (var device = OpticalDevice.Open(driveLetter, false))
                {
                    int error;
                    return device.CheckMedia(out error);
                }
            }
            catch (DeviceAccessException ex)
            {
                return ex.IsAccessDenied ? DriveMediaState.AccessDenied : DriveMediaState.Error;
            }
        }
    }

    /// <summary>
    /// Reads disc metadata (profile, TOC, sessions, file systems) and decides whether a standard ISO can
    /// faithfully represent the disc. It never guesses: anything it cannot represent is refused with a reason.
    /// </summary>
    public static class DiscInspector
    {
        public static DiscInfo Inspect(OpticalDriveInfo drive, ILogger log, out DriveMediaState state)
        {
            if (drive == null) throw new ArgumentNullException("drive");
            using (var device = OpticalDevice.Open(drive.DriveLetter, true))
            {
                int error;
                state = device.CheckMedia(out error);
                if (state != DriveMediaState.Ready)
                {
                    log.Debug("Drive " + drive.RootPath + " media state " + state + " (Win32 " + error + ")");
                    return null;
                }

                var disc = new DiscInfo { Drive = drive };
                disc.ProfileCode = device.GetCurrentProfile();
                disc.MediaTypeName = MediaProfiles.Name(disc.ProfileCode);
                disc.Family = MediaProfiles.Family(disc.ProfileCode);

                long leadOut;
                int tocError;
                List<TrackInfo> tracks = device.ReadToc(out leadOut, out tocError);
                int firstTrackLastSession;
                int sessions = device.ReadLastSessionNumber(out firstTrackLastSession);
                int bytesPerSector;
                long diskSize = device.GetDiskSize(out bytesPerSector);

                if (tracks != null)
                {
                    disc.Tracks.AddRange(tracks);
                    AssignSessions(disc.Tracks, sessions, firstTrackLastSession);
                }
                disc.SessionCount = sessions > 0 ? sessions : (tracks != null && tracks.Count > 0 ? 1 : 0);

                log.Info(string.Format("Inspect {0}: profile 0x{1:X4} ({2}), tracks {3}, sessions {4}, lead-out {5}, geometry size {6}, bytes/sector {7}",
                    drive.RootPath, disc.ProfileCode, disc.MediaTypeName, tracks == null ? "n/a (Win32 " + tocError + ")" : tracks.Count.ToString(),
                    disc.SessionCount, leadOut, diskSize, bytesPerSector));

                if (disc.Family == MediaFamily.DVD || disc.Family == MediaFamily.BluRay)
                {
                    int protection = device.GetDvdCopyProtectionType();
                    if (protection > 0) disc.IsCopyProtected = true;
                    if (protection >= 0) log.Info("DVD copyright descriptor protection type: " + protection);
                }

                // Image length: LBA 0 up to the lead-out of the (single) session.
                if (leadOut > 0) disc.ImageSectors = leadOut;
                else if (diskSize > 0) disc.ImageSectors = diskSize / OpticalSectorSource.CdDvdSectorSize;
                if (disc.ImageSectors > 0 && diskSize > 0 && leadOut > 0 && Math.Abs(diskSize / 2048 - leadOut) > 2)
                    log.Info("Note: TOC lead-out (" + leadOut + ") and READ CAPACITY (" + diskSize / 2048 + ") differ; TOC is used.");

                // File systems: only meaningful when the first track is a data track.
                bool firstIsData = disc.Tracks.Count == 0 || disc.Tracks[0].IsData;
                if (firstIsData && disc.ImageSectors > 16)
                {
                    using (var source = new OpticalSectorSource(device, 1, false))
                    {
                        byte[] sector = new byte[OpticalSectorSource.CdDvdSectorSize];
                        long limit = disc.ImageSectors;
                        disc.Filesystem = FilesystemProbe.Probe(lba =>
                        {
                            if (lba >= limit) return null;
                            SectorReadResult r = source.Read(lba, 1, sector, 0);
                            return r.IsOk ? (byte[])sector.Clone() : null;
                        });
                    }
                }

                disc.Label = ReadVolumeLabel(drive.RootPath);
                if (string.IsNullOrWhiteSpace(disc.Label)) disc.Label = disc.Filesystem.Iso9660VolumeId;
                if (string.IsNullOrWhiteSpace(disc.Label)) disc.Label = disc.AudioTrackCount > 0 ? "AUDIO_CD" : "DISC";
                disc.FilesystemBytes = ReadFilesystemSize(drive.RootPath);

                Assess(disc);
                log.Info("Assessment for " + drive.RootPath + ": " + (disc.CanCreateIso ? "ISO supported" : "NOT supported - " + disc.UnsupportedReason)
                         + "; file systems: " + disc.Filesystem.Describe() + "; image sectors " + disc.ImageSectors);
                return disc;
            }
        }

        /// <summary>Decides whether a standard ISO image is a faithful representation of the disc.</summary>
        public static void Assess(DiscInfo disc)
        {
            disc.CanCreateIso = false;
            disc.RecommendedFormat = ImageFormat.None;

            if (disc.IsCopyProtected)
            {
                disc.UnsupportedReason = "This disc reports copy protection (for example CSS on a commercial video DVD). "
                    + "HT ISO Ripper does not bypass DRM, encryption or copy protection, so it cannot image this disc.";
                return;
            }
            if (disc.Family == MediaFamily.BluRay)
            {
                disc.UnsupportedReason = "Blu-ray media is outside the supported scope of HT ISO Ripper 1.0 (CD and DVD only).";
                return;
            }
            if (disc.ImageSectors <= 0)
            {
                disc.UnsupportedReason = "The drive did not report any readable data. The disc may be blank, unfinalised or unreadable.";
                return;
            }

            int audio = disc.AudioTrackCount;
            int data = disc.DataTrackCount;
            if (audio > 0 && data == 0)
            {
                disc.RecommendedFormat = ImageFormat.BinCue;
                disc.UnsupportedReason = "This is an audio CD. Audio tracks use 2352-byte raw sectors and cannot be stored in an ISO file. "
                    + "A BIN/CUE image is required to preserve them; BIN/CUE support is planned for a later stage of HT ISO Ripper.";
                return;
            }
            if (audio > 0)
            {
                disc.RecommendedFormat = ImageFormat.BinCue;
                disc.UnsupportedReason = "This is a mixed-mode or enhanced CD (" + data + " data and " + audio + " audio tracks). "
                    + "An ISO would contain only the data track and silently drop the audio, so it is not offered. "
                    + "A BIN/CUE image is required; BIN/CUE support is planned for a later stage.";
                return;
            }
            if (disc.SessionCount > 1 || data > 1)
            {
                disc.UnsupportedReason = "This disc has " + Math.Max(disc.SessionCount, 1) + " sessions / " + data + " data tracks. "
                    + "Multisession discs contain unreadable gaps between sessions, so a plain ISO cannot be produced safely in this "
                    + "version. Multisession support is planned for Stage 3.";
                return;
            }

            disc.CanCreateIso = true;
            disc.RecommendedFormat = ImageFormat.Iso;
            disc.UnsupportedReason = null;
            if (!disc.Filesystem.AnyRecognised)
                disc.Warnings.Add("No ISO 9660 or UDF file system was recognised. The image will still be an exact sector copy of the data track.");
            if (disc.Family == MediaFamily.Unknown)
                disc.Warnings.Add("The drive did not report a media profile; media type is unknown.");
        }

        private static void AssignSessions(List<TrackInfo> tracks, int lastSession, int firstTrackOfLastSession)
        {
            if (lastSession <= 1 || firstTrackOfLastSession <= 0) return;
            // The TOC call does not give per-track sessions; mark tracks of the last session so the count is meaningful.
            foreach (var t in tracks.Where(t => t.Number >= firstTrackOfLastSession)) t.Session = lastSession;
        }

        private static string ReadVolumeLabel(string root)
        {
            char[] name = new char[261];
            char[] fs = new char[261];
            uint serial, maxLen, flags;
            if (!NativeMethods.GetVolumeInformation(root, name, name.Length, out serial, out maxLen, out flags, fs, fs.Length))
                return null;
            return new string(name).TrimEnd('\0').Trim();
        }

        private static long ReadFilesystemSize(string root)
        {
            ulong free, total, totalFree;
            if (!NativeMethods.GetDiskFreeSpaceEx(root, out free, out total, out totalFree)) return -1;
            return (long)total;
        }
    }
}
