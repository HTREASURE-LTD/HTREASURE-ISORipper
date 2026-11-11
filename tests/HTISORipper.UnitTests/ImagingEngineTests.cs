using System;
using System.IO;
using System.Linq;
using System.Threading;
using HTISORipper.Core;
using HTISORipper.Imaging;
using HTISORipper.Logging;
using Xunit;

namespace HTISORipper.UnitTests
{
    public class ImagingEngineTests
    {
        private static ImagingOptions Options(string path, long sectors)
        {
            return new ImagingOptions
            {
                OutputPath = path,
                TotalSectors = sectors,
                BlockSectors = 8,
                RetryCount = 3,
                RetryDelayMilliseconds = 0,
                SourceDescription = "mock"
            };
        }

        private static ImagingEngine Engine()
        {
            return new ImagingEngine(NullLogger.Instance) { FreeSpaceProvider = _ => long.MaxValue };
        }

        [Fact]
        public void CleanDisc_ProducesBitExactImage_AndMatchingSha256()
        {
            using (var dir = new TempDir())
            {
                byte[] disc = IsoBuilder.Build(301);   // deliberately not a multiple of the block size
                var source = new MockSectorSource(disc);
                string path = dir.File("DISC.iso");

                ImagingResult r = Engine().Run(source, Options(path, 301), null, CancellationToken.None);

                Assert.Equal(ImagingOutcome.Success, r.Outcome);
                Assert.Equal(path, r.OutputPath);
                Assert.Equal(disc, File.ReadAllBytes(path));
                Assert.Equal(Sha.Of(disc), r.Sha256);
                Assert.Equal(301L * 2048, r.ImageBytes);
                Assert.False(File.Exists(PathUtil.GetTempPathFor(path)));
            }
        }

        [Fact]
        public void TransientErrors_AreRetried_AndImageIsStillExact()
        {
            using (var dir = new TempDir())
            {
                byte[] disc = IsoBuilder.Build(64);
                var source = new MockSectorSource(disc);
                // The first failure is absorbed by the multi-sector block read; "recovered" counts sectors that
                // also failed an individual re-read and then succeeded on retry.
                source.TransientFailures[10] = 2;
                source.TransientFailures[40] = 3;
                source.TransientFailures[50] = 1;   // fails only the block read: re-read succeeds first time
                string path = dir.File("DISC.iso");

                ImagingResult r = Engine().Run(source, Options(path, 64), null, CancellationToken.None);

                Assert.Equal(ImagingOutcome.Success, r.Outcome);
                Assert.Equal(2, r.RecoveredSectors);
                Assert.Empty(r.UnreadableLbas);
                Assert.Equal(disc, File.ReadAllBytes(path));
            }
        }

        [Fact]
        public void PermanentError_WithAbortPolicy_Fails_AndLeavesNoFiles()
        {
            using (var dir = new TempDir())
            {
                var source = new MockSectorSource(IsoBuilder.Build(64));
                source.PermanentlyBad.Add(33);
                string path = dir.File("DISC.iso");

                ImagingResult r = Engine().Run(source, Options(path, 64), null, CancellationToken.None);

                Assert.Equal(ImagingOutcome.Failed, r.Outcome);
                Assert.Equal(ImagingErrorKind.UnreadableSectors, r.Error);
                Assert.Equal(new long[] { 33 }, r.UnreadableLbas.ToArray());
                Assert.Null(r.OutputPath);
                Assert.Empty(Directory.GetFiles(dir.Path));
            }
        }

        [Fact]
        public void PermanentError_WithContinuePolicy_SavesIncompleteImage_NeverSuccess()
        {
            using (var dir = new TempDir())
            {
                byte[] disc = IsoBuilder.Build(64);
                var source = new MockSectorSource(disc);
                source.PermanentlyBad.Add(20);
                source.PermanentlyBad.Add(21);
                string path = dir.File("DISC.iso");
                ImagingOptions o = Options(path, 64);
                o.ErrorPolicy = ErrorPolicy.ContinueAndMarkIncomplete;

                ImagingResult r = Engine().Run(source, o, null, CancellationToken.None);

                Assert.Equal(ImagingOutcome.CompletedWithErrors, r.Outcome);
                Assert.Equal(dir.File("DISC.INCOMPLETE.iso"), r.OutputPath);
                Assert.False(File.Exists(path), "a damaged image must never get the normal name");
                Assert.True(File.Exists(r.BadSectorReportPath));
                Assert.Contains("20", File.ReadAllText(r.BadSectorReportPath));

                byte[] image = File.ReadAllBytes(r.OutputPath);
                Assert.Equal(disc.Length, image.Length);
                Assert.True(image.Skip(20 * 2048).Take(2 * 2048).All(b => b == 0));
                Assert.Equal(disc.Take(20 * 2048), image.Take(20 * 2048));
                Assert.Equal(disc.Skip(22 * 2048), image.Skip(22 * 2048));
            }
        }

        [Fact]
        public void DiscRemoved_FailsWithDiscRemoved_AndDeletesPartialFile()
        {
            using (var dir = new TempDir())
            {
                var source = new MockSectorSource(IsoBuilder.Build(128)) { RemovedAtLba = 70 };
                string path = dir.File("DISC.iso");

                ImagingResult r = Engine().Run(source, Options(path, 128), null, CancellationToken.None);

                Assert.Equal(ImagingOutcome.Failed, r.Outcome);
                Assert.Equal(ImagingErrorKind.DiscRemoved, r.Error);
                Assert.Empty(Directory.GetFiles(dir.Path));
            }
        }

        [Fact]
        public void Cancellation_StopsSafely_AndDeletesPartialFile()
        {
            using (var dir = new TempDir())
            using (var cts = new CancellationTokenSource())
            {
                var source = new MockSectorSource(IsoBuilder.Build(256));
                source.OnRead = lba => { if (lba >= 100) cts.Cancel(); };
                string path = dir.File("DISC.iso");

                ImagingResult r = Engine().Run(source, Options(path, 256), null, cts.Token);

                Assert.Equal(ImagingOutcome.Cancelled, r.Outcome);
                Assert.Empty(Directory.GetFiles(dir.Path));
            }
        }

        [Fact]
        public void ExistingOutput_IsNotOverwritten_WithoutPermission()
        {
            using (var dir = new TempDir())
            {
                string path = dir.File("DISC.iso");
                File.WriteAllText(path, "precious");
                var source = new MockSectorSource(IsoBuilder.Build(32));

                ImagingResult r = Engine().Run(source, Options(path, 32), null, CancellationToken.None);

                Assert.Equal(ImagingErrorKind.OutputExists, r.Error);
                Assert.Equal("precious", File.ReadAllText(path));
                Assert.Equal(0, source.ReadCalls);
            }
        }

        [Fact]
        public void ExistingOutput_IsReplaced_OnlyAfterSuccessfulRead_WhenAllowed()
        {
            using (var dir = new TempDir())
            {
                string path = dir.File("DISC.iso");
                File.WriteAllText(path, "old");
                byte[] disc = IsoBuilder.Build(32);
                ImagingOptions o = Options(path, 32);
                o.AllowOverwrite = true;

                // A failed read must leave the old file untouched.
                var failing = new MockSectorSource(disc) { RemovedAtLba = 5 };
                Engine().Run(failing, o, null, CancellationToken.None);
                Assert.Equal("old", File.ReadAllText(path));

                ImagingResult r = Engine().Run(new MockSectorSource(disc), o, null, CancellationToken.None);
                Assert.Equal(ImagingOutcome.Success, r.Outcome);
                Assert.Equal(disc, File.ReadAllBytes(path));
            }
        }

        [Fact]
        public void InsufficientSpace_IsDetectedBeforeReading()
        {
            using (var dir = new TempDir())
            {
                var source = new MockSectorSource(IsoBuilder.Build(64));
                var engine = new ImagingEngine(NullLogger.Instance) { FreeSpaceProvider = _ => 1024 * 1024 };

                ImagingResult r = engine.Run(source, Options(dir.File("DISC.iso"), 64), null, CancellationToken.None);

                Assert.Equal(ImagingErrorKind.InsufficientSpace, r.Error);
                Assert.Equal(0, source.ReadCalls);
                Assert.Empty(Directory.GetFiles(dir.Path));
            }
        }

        [Fact]
        public void TrailingRunOutSectors_BeyondFilesystem_AreExcluded_NotZeroFilled()
        {
            using (var dir = new TempDir())
            {
                // 100-sector track whose ISO 9660 file system ends at 98; the last 2 sectors (TAO run-out) are unreadable.
                byte[] disc = IsoBuilder.Build(100, declaredVolumeBlocks: 98);
                var source = new MockSectorSource(disc);
                source.PermanentlyBad.Add(98);
                source.PermanentlyBad.Add(99);
                string path = dir.File("CDR.iso");
                ImagingOptions o = Options(path, 100);
                o.MinimumRequiredSectors = 98;
                o.RunOutToleranceSectors = 2;

                ImagingResult r = Engine().Run(source, o, null, CancellationToken.None);

                Assert.Equal(ImagingOutcome.Success, r.Outcome);
                Assert.Equal(2, r.RunOutSectorsExcluded);
                Assert.Equal(98L * 2048, new FileInfo(path).Length);
                Assert.Equal(disc.Take(98 * 2048), File.ReadAllBytes(path));
                Assert.Equal(Sha.Of(disc.Take(98 * 2048).ToArray()), r.Sha256);
            }
        }

        [Fact]
        public void UnreadableSector_InsideFilesystem_IsNeverTreatedAsRunOut()
        {
            using (var dir = new TempDir())
            {
                byte[] disc = IsoBuilder.Build(100, declaredVolumeBlocks: 100);
                var source = new MockSectorSource(disc);
                source.PermanentlyBad.Add(99);
                ImagingOptions o = Options(dir.File("CDR.iso"), 100);
                o.MinimumRequiredSectors = 100;
                o.RunOutToleranceSectors = 2;

                ImagingResult r = Engine().Run(source, o, null, CancellationToken.None);

                Assert.Equal(ImagingOutcome.Failed, r.Outcome);
                Assert.Equal(ImagingErrorKind.UnreadableSectors, r.Error);
            }
        }

        [Fact]
        public void InvalidFileName_IsRejected()
        {
            using (var dir = new TempDir())
            {
                var source = new MockSectorSource(IsoBuilder.Build(32));
                ImagingResult r = Engine().Run(source, Options(dir.File("CON.iso"), 32), null, CancellationToken.None);
                Assert.Equal(ImagingOutcome.Failed, r.Outcome);
                Assert.Equal(ImagingErrorKind.InvalidDestination, r.Error);
            }
        }

        [Fact]
        public void ProgressReports_ReachTotal()
        {
            using (var dir = new TempDir())
            {
                var source = new MockSectorSource(IsoBuilder.Build(64));
                var reports = new System.Collections.Generic.List<ImagingProgress>();
                var engine = Engine();
                engine.ProgressInterval = TimeSpan.Zero;

                engine.Run(source, Options(dir.File("DISC.iso"), 64), new SyncProgress(reports.Add), CancellationToken.None);

                Assert.Contains(reports, p => p.Phase == ImagingPhase.Reading && p.SectorsDone == 64);
            }
        }

        private sealed class SyncProgress : IProgress<ImagingProgress>
        {
            private readonly Action<ImagingProgress> _action;
            public SyncProgress(Action<ImagingProgress> action) { _action = action; }
            public void Report(ImagingProgress value) { _action(value); }
        }
    }
}
