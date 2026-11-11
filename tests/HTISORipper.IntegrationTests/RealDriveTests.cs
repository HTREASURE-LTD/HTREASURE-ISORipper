using System;
using System.IO;
using System.Linq;
using System.Threading;
using HTISORipper.Core;
using HTISORipper.DiscEngine;
using HTISORipper.Imaging;
using HTISORipper.Logging;
using HTISORipper.Verification;
using Xunit;
using Xunit.Abstractions;

namespace HTISORipper.IntegrationTests
{
    /// <summary>
    /// Hardware acceptance tests. They talk to a REAL optical drive and are skipped unless configured:
    ///
    ///   set HTISO_TEST_DRIVE=D              (drive letter of the optical drive under test)
    ///   set HTISO_TEST_OUTPUT=C:\ISOTests   (optional; default %TEMP%\HTISORipperIntegration)
    ///   dotnet test tests\HTISORipper.IntegrationTests
    ///
    /// Insert the disc for the scenario being tested (see docs/TESTING.md) before running.
    /// </summary>
    public class RealDriveTests
    {
        private readonly ITestOutputHelper _output;

        public RealDriveTests(ITestOutputHelper output) { _output = output; }

        private static string DriveLetter
        {
            get
            {
                string d = Environment.GetEnvironmentVariable("HTISO_TEST_DRIVE");
                return string.IsNullOrWhiteSpace(d) ? null : d.Trim().TrimEnd(':', '\\').ToUpperInvariant();
            }
        }

        private static string OutputDir
        {
            get
            {
                string d = Environment.GetEnvironmentVariable("HTISO_TEST_OUTPUT");
                return string.IsNullOrWhiteSpace(d) ? Path.Combine(Path.GetTempPath(), "HTISORipperIntegration") : d;
            }
        }

        [SkippableFact]
        public void Enumerate_FindsConfiguredDrive()
        {
            Skip.If(DriveLetter == null, "HTISO_TEST_DRIVE not set - no optical hardware configured.");
            var drives = DriveEnumerator.GetOpticalDrives(NullLogger.Instance);
            foreach (var d in drives) _output.WriteLine(d.ToString());
            Assert.Contains(drives, d => d.DriveLetter == DriveLetter);
        }

        [SkippableFact]
        public void Inspect_ReportsDiscMetadata()
        {
            Skip.If(DriveLetter == null, "HTISO_TEST_DRIVE not set - no optical hardware configured.");
            var drive = DriveEnumerator.GetOpticalDrives(NullLogger.Instance).Single(d => d.DriveLetter == DriveLetter);
            DriveMediaState state;
            DiscInfo disc = DiscInspector.Inspect(drive, NullLogger.Instance, out state);
            Skip.If(disc == null, "No disc ready in drive (state " + state + ").");

            _output.WriteLine("Label: " + disc.Label);
            _output.WriteLine("Type: " + disc.MediaTypeName + " (0x" + disc.ProfileCode.ToString("X4") + ")");
            _output.WriteLine("File systems: " + disc.Filesystem.Describe());
            _output.WriteLine("Sessions/tracks: " + disc.SessionCount + "/" + disc.Tracks.Count);
            _output.WriteLine("Image sectors: " + disc.ImageSectors);
            _output.WriteLine("ISO supported: " + disc.CanCreateIso + " " + disc.UnsupportedReason);
            Assert.True(disc.ImageSectors > 0 || !disc.CanCreateIso);
        }

        [SkippableFact]
        public void RipAndFullVerify_DataDisc()
        {
            Skip.If(DriveLetter == null, "HTISO_TEST_DRIVE not set - no optical hardware configured.");
            var log = new FileLogger(OutputDir);
            var drive = DriveEnumerator.GetOpticalDrives(log).Single(d => d.DriveLetter == DriveLetter);
            DriveMediaState state;
            DiscInfo disc = DiscInspector.Inspect(drive, log, out state);
            Skip.If(disc == null, "No disc ready in drive (state " + state + ").");
            Skip.IfNot(disc.CanCreateIso, "Inserted disc is not ISO-compatible: " + disc.UnsupportedReason);

            Directory.CreateDirectory(OutputDir);
            string path = Path.Combine(OutputDir, PathUtil.SanitizeBaseName(disc.Label) + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".iso");
            using (var source = OpticalSectorSource.Open(drive.DriveLetter, 32))
            {
                var options = new ImagingOptions
                {
                    OutputPath = path,
                    TotalSectors = disc.ImageSectors,
                    MinimumRequiredSectors = disc.MinimumFilesystemSectors,
                    RunOutToleranceSectors = disc.Family == MediaFamily.CD ? 2 : 0,
                    SourceDescription = drive + " " + disc.Label
                };
                ImagingResult result = new ImagingEngine(log).Run(source, options, null, CancellationToken.None);
                _output.WriteLine(result.Outcome + " " + result.Message + " " + result.Sha256);
                Assert.Equal(ImagingOutcome.Success, result.Outcome);

                VerificationReport report = new ImageVerifier(log).Verify(result.OutputPath, result.ImageBytes, result.Sha256,
                    VerificationMode.FullSourceCompare, source, null, CancellationToken.None);
                _output.WriteLine(report.Summary + " | " + string.Join(" ", report.Notes.ToArray()));
                Assert.Equal(VerificationLevel.SourceVerified, report.Level);

                if (disc.Filesystem.HasElTorito)
                    Assert.True(VerificationProbe(result.OutputPath).HasElTorito, "Boot record must be preserved in the image.");
            }
            _output.WriteLine("Image: " + path + "  Log: " + log.CurrentLogFile);
        }

        private static FilesystemInfo VerificationProbe(string path)
        {
            return ImageVerifier.ProbeFile(path);
        }
    }
}
