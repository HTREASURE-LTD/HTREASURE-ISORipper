using System.IO;
using System.Threading;
using HTISORipper.Core;
using HTISORipper.DiscEngine;
using HTISORipper.Imaging;
using HTISORipper.Logging;
using HTISORipper.UI.Forms;
using HTISORipper.Verification;
using Xunit;

namespace HTISORipper.UnitTests
{
    public class FilesystemProbeTests
    {
        private static FilesystemInfo ProbeBytes(byte[] image)
        {
            return FilesystemProbe.Probe(lba =>
            {
                if ((lba + 1) * 2048 > image.Length) return null;
                var s = new byte[2048];
                System.Buffer.BlockCopy(image, (int)lba * 2048, s, 0, 2048);
                return s;
            });
        }

        [Fact]
        public void DetectsIso9660_WithLabelAndSize()
        {
            FilesystemInfo fs = ProbeBytes(IsoBuilder.Build(300, "WINDOWS_INSTALLER"));
            Assert.True(fs.HasIso9660);
            Assert.False(fs.HasJoliet);
            Assert.False(fs.HasUdf);
            Assert.Equal("WINDOWS_INSTALLER", fs.Iso9660VolumeId);
            Assert.Equal(300, fs.Iso9660VolumeBlocks);
            Assert.Equal(300L * 2048, fs.Iso9660VolumeBytes);
        }

        [Fact]
        public void DetectsJolietElToritoAndUdf()
        {
            FilesystemInfo fs = ProbeBytes(IsoBuilder.Build(300, joliet: true, elTorito: true, udf: true));
            Assert.True(fs.HasIso9660);
            Assert.True(fs.HasJoliet);
            Assert.True(fs.HasElTorito);
            Assert.True(fs.HasUdf);
            Assert.True(fs.HasUdfAnchor);
            Assert.Equal("ISO 9660 + Joliet / UDF (1.02-1.50), bootable (El Torito)", fs.Describe());
        }

        [Fact]
        public void RandomData_IsNotRecognised()
        {
            var data = new byte[300 * 2048];
            new System.Random(7).NextBytes(data);
            Assert.False(ProbeBytes(data).AnyRecognised);
        }
    }

    public class ImageVerifierTests
    {
        private static string WriteImage(TempDir dir, byte[] data)
        {
            string path = dir.File("IMG.iso");
            File.WriteAllBytes(path, data);
            return path;
        }

        [Fact]
        public void FullCompare_MatchingDisc_IsSourceVerified()
        {
            using (var dir = new TempDir())
            {
                byte[] disc = IsoBuilder.Build(120, joliet: true);
                string path = WriteImage(dir, disc);
                var report = new ImageVerifier(NullLogger.Instance) { BlockSectors = 16 }.Verify(path, disc.Length, Sha.Of(disc),
                    VerificationMode.FullSourceCompare, new MockSectorSource(disc), null, CancellationToken.None);

                Assert.Equal(VerificationLevel.SourceVerified, report.Level);
                Assert.True(report.SizeOk);
                Assert.True(report.FileHashMatches);
                Assert.True(report.StructureOk);
                Assert.True(report.SourceMatches);
            }
        }

        [Fact]
        public void FullCompare_DetectsSectorThatDiffersFromDisc()
        {
            using (var dir = new TempDir())
            {
                byte[] disc = IsoBuilder.Build(120);
                string path = WriteImage(dir, disc);
                var source = new MockSectorSource(disc);
                source.CorruptOnRead.Add(77);
                var report = new ImageVerifier(NullLogger.Instance).Verify(path, disc.Length, Sha.Of(disc),
                    VerificationMode.FullSourceCompare, source, null, CancellationToken.None);

                Assert.Equal(VerificationLevel.Failed, report.Level);
                Assert.Equal(new long[] { 77 }, report.MismatchedLbas.ToArray());
            }
        }

        [Fact]
        public void Structural_DetectsFileCorruptedAfterWriting()
        {
            using (var dir = new TempDir())
            {
                byte[] disc = IsoBuilder.Build(120);
                string expectedSha = Sha.Of(disc);
                byte[] corrupted = (byte[])disc.Clone();
                corrupted[100 * 2048 + 5] ^= 0x01;
                string path = WriteImage(dir, corrupted);

                var report = new ImageVerifier(NullLogger.Instance).Verify(path, disc.Length, expectedSha,
                    VerificationMode.Structural, null, null, CancellationToken.None);

                Assert.Equal(VerificationLevel.Failed, report.Level);
                Assert.False(report.FileHashMatches);
            }
        }

        [Fact]
        public void Structural_PassesWithoutClaimingSourceVerification()
        {
            using (var dir = new TempDir())
            {
                byte[] disc = IsoBuilder.Build(120);
                string path = WriteImage(dir, disc);
                var report = new ImageVerifier(NullLogger.Instance).Verify(path, disc.Length, Sha.Of(disc),
                    VerificationMode.Structural, null, null, CancellationToken.None);

                Assert.Equal(VerificationLevel.StructuralOnly, report.Level);
                Assert.False(report.SourceCompared);
            }
        }

        [Fact]
        public void SizeMismatch_Fails()
        {
            using (var dir = new TempDir())
            {
                byte[] disc = IsoBuilder.Build(64);
                string path = WriteImage(dir, disc);
                var report = new ImageVerifier(NullLogger.Instance).Verify(path, disc.Length + 2048, Sha.Of(disc),
                    VerificationMode.Structural, null, null, CancellationToken.None);
                Assert.Equal(VerificationLevel.Failed, report.Level);
                Assert.False(report.SizeOk);
            }
        }

        [Fact]
        public void TruncatedIso_FailsStructuralCheck()
        {
            using (var dir = new TempDir())
            {
                byte[] disc = IsoBuilder.Build(64, declaredVolumeBlocks: 200);   // PVD claims more than the file holds
                string path = WriteImage(dir, disc);
                var report = new ImageVerifier(NullLogger.Instance).Verify(path, disc.Length, Sha.Of(disc),
                    VerificationMode.Structural, null, null, CancellationToken.None);
                Assert.False(report.StructureOk);
                Assert.Equal(VerificationLevel.Failed, report.Level);
            }
        }

        [Fact]
        public void EndToEnd_RipThenVerify_IsSourceVerified()
        {
            using (var dir = new TempDir())
            {
                byte[] disc = IsoBuilder.Build(500, joliet: true, elTorito: true, udf: true);
                var source = new MockSectorSource(disc);
                source.TransientFailures[300] = 1;
                var engine = new ImagingEngine(NullLogger.Instance) { FreeSpaceProvider = _ => long.MaxValue };
                ImagingResult result = engine.Run(source, new ImagingOptions { OutputPath = dir.File("BOOT.iso"), TotalSectors = 500, RetryDelayMilliseconds = 0 },
                    null, CancellationToken.None);
                VerificationReport report = new ImageVerifier(NullLogger.Instance).Verify(result.OutputPath, result.ImageBytes, result.Sha256,
                    VerificationMode.FullSourceCompare, source, null, CancellationToken.None);

                Assert.Equal(ImagingOutcome.Success, result.Outcome);
                Assert.Equal(VerificationLevel.SourceVerified, report.Level);
                Assert.Equal("IMAGE CREATED AND VERIFIED SUCCESSFULLY", CompletionStatus.Evaluate(result, report).Headline);
            }
        }
    }

    public class CompletionWordingTests
    {
        private static ImagingResult Success() { return new ImagingResult { Outcome = ImagingOutcome.Success }; }

        [Fact]
        public void VerifiedWording_OnlyForSourceVerification()
        {
            const string verified = "IMAGE CREATED AND VERIFIED SUCCESSFULLY";
            Assert.Equal(verified, CompletionStatus.Evaluate(Success(), new VerificationReport { Level = VerificationLevel.SourceVerified }).Headline);
            Assert.NotEqual(verified, CompletionStatus.Evaluate(Success(), new VerificationReport { Level = VerificationLevel.StructuralOnly }).Headline);
            Assert.NotEqual(verified, CompletionStatus.Evaluate(Success(), new VerificationReport { Level = VerificationLevel.NotVerified }).Headline);
            Assert.NotEqual(verified, CompletionStatus.Evaluate(Success(), new VerificationReport { Level = VerificationLevel.SourceVerified, Cancelled = true }).Headline);
            Assert.NotEqual(verified, CompletionStatus.Evaluate(Success(), null).Headline);

            var incomplete = new ImagingResult { Outcome = ImagingOutcome.CompletedWithErrors };
            incomplete.UnreadableLbas.Add(5);
            Assert.StartsWith("INCOMPLETE IMAGE", CompletionStatus.Evaluate(incomplete, new VerificationReport { Level = VerificationLevel.SourceVerified }).Headline);
        }
    }

    public class DiscAssessmentTests
    {
        private static DiscInfo Disc(params byte[] controls)
        {
            var d = new DiscInfo { ImageSectors = 1000, Family = MediaFamily.CD, SessionCount = 1 };
            int n = 1;
            foreach (byte c in controls) d.Tracks.Add(new TrackInfo { Number = n++, Control = c, Session = 1 });
            return d;
        }

        [Fact]
        public void SingleDataTrack_IsIso()
        {
            DiscInfo d = Disc(0x04);
            d.Filesystem.HasIso9660 = true;
            DiscInspector.Assess(d);
            Assert.True(d.CanCreateIso);
            Assert.Equal(ImageFormat.Iso, d.RecommendedFormat);
        }

        [Fact]
        public void AudioCd_IsRefused_WithBinCueRecommendation()
        {
            DiscInfo d = Disc(0x00, 0x00, 0x00);
            DiscInspector.Assess(d);
            Assert.False(d.CanCreateIso);
            Assert.Equal(ImageFormat.BinCue, d.RecommendedFormat);
            Assert.Contains("audio CD", d.UnsupportedReason);
        }

        [Fact]
        public void MixedMode_IsRefused()
        {
            DiscInfo d = Disc(0x04, 0x00);
            DiscInspector.Assess(d);
            Assert.False(d.CanCreateIso);
            Assert.Contains("mixed-mode", d.UnsupportedReason);
        }

        [Fact]
        public void Multisession_IsRefused()
        {
            DiscInfo d = Disc(0x04, 0x04);
            d.SessionCount = 2;
            DiscInspector.Assess(d);
            Assert.False(d.CanCreateIso);
            Assert.Contains("sessions", d.UnsupportedReason);
        }

        [Fact]
        public void CopyProtectedDvd_IsRefused()
        {
            DiscInfo d = Disc(0x04);
            d.Family = MediaFamily.DVD;
            d.IsCopyProtected = true;
            DiscInspector.Assess(d);
            Assert.False(d.CanCreateIso);
            Assert.Contains("does not bypass", d.UnsupportedReason);
        }

        [Fact]
        public void BlankDisc_IsRefused()
        {
            var d = new DiscInfo { ImageSectors = 0, Family = MediaFamily.DVD };
            DiscInspector.Assess(d);
            Assert.False(d.CanCreateIso);
        }

        [Fact]
        public void UnrecognisedFilesystem_AllowedWithWarning()
        {
            DiscInfo d = Disc(0x04);
            DiscInspector.Assess(d);
            Assert.True(d.CanCreateIso);
            Assert.NotEmpty(d.Warnings);
        }
    }

    public class PathUtilTests
    {
        [Theory]
        [InlineData("WINDOWS_INSTALLER", "WINDOWS_INSTALLER")]
        [InlineData("My: Disc/2026?", "My_ Disc_2026_")]
        [InlineData("   ", "DISC")]
        [InlineData(null, "DISC")]
        [InlineData("CON", "_CON")]
        [InlineData("lpt1", "_lpt1")]
        [InlineData("trailing. ", "trailing")]
        public void SanitizeBaseName(string input, string expected)
        {
            Assert.Equal(expected, PathUtil.SanitizeBaseName(input));
        }

        [Fact]
        public void SanitizeBaseName_LimitsLength()
        {
            Assert.True(PathUtil.SanitizeBaseName(new string('A', 500)).Length <= PathUtil.MaxBaseNameLength);
        }

        [Theory]
        [InlineData("DISC.iso", true)]
        [InlineData("bad|name.iso", false)]
        [InlineData("NUL.iso", false)]
        [InlineData("name.", false)]
        [InlineData("", false)]
        public void ValidateFileName(string name, bool valid)
        {
            Assert.Equal(valid, PathUtil.ValidateFileName(name) == null);
        }

        [Theory]
        [InlineData(@"C:\ISO", true)]
        [InlineData(@"relative\folder", false)]
        [InlineData(@"\\.\PhysicalDrive0", false)]
        [InlineData("", false)]
        public void ValidateDirectory(string dir, bool valid)
        {
            Assert.Equal(valid, PathUtil.ValidateDirectory(dir) == null);
        }

        [Fact]
        public void IncompleteName()
        {
            Assert.Equal(@"C:\x\DISC.INCOMPLETE.iso", PathUtil.GetIncompletePathFor(@"C:\x\DISC.iso"));
        }
    }

    public class SettingsTests
    {
        [Fact]
        public void SaveAndLoad_RoundTrips_AndCorruptFileFallsBackToDefaults()
        {
            using (var dir = new TempDir())
            {
                var store = new SettingsStore(dir.File("settings.xml"));
                var s = new AppSettings { MaxReadRetries = 7, EjectAfterSuccess = true, VerificationMode = VerificationMode.Structural, DefaultOutputDirectory = @"D:\Images" };
                store.Save(s);
                AppSettings loaded = store.Load();
                Assert.Equal(7, loaded.MaxReadRetries);
                Assert.True(loaded.EjectAfterSuccess);
                Assert.Equal(VerificationMode.Structural, loaded.VerificationMode);
                Assert.Equal(@"D:\Images", loaded.DefaultOutputDirectory);

                File.WriteAllText(store.FilePath, "<not xml");
                Assert.Equal(3, store.Load().MaxReadRetries);
            }
        }

        [Fact]
        public void Normalise_ClampsOutOfRangeValues()
        {
            var s = new AppSettings { MaxReadRetries = 999, LogRetentionDays = 0, ReadBlockSectors = 7, DefaultOutputDirectory = "relative" };
            s.Normalise();
            Assert.Equal(20, s.MaxReadRetries);
            Assert.Equal(1, s.LogRetentionDays);
            Assert.Equal(32, s.ReadBlockSectors);
            Assert.Equal(PathUtil.DefaultOutputDirectory, s.DefaultOutputDirectory);
        }
    }
}
