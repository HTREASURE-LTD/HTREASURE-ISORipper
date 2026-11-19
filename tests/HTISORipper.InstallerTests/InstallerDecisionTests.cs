using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using Xunit;

namespace HTISORipper.InstallerTests
{
    /// <summary>
    /// Compiles tests\installer\DecisionHarness.iss once. The harness includes the SAME installer\decision.iss
    /// that HT-ISO-Ripper-Setup uses, so these tests exercise the installer's real Pascal code.
    /// Requires Inno Setup 6.3+ (ISCC.exe); tests are skipped when it is not installed.
    /// </summary>
    public sealed class HarnessFixture
    {
        public HarnessFixture()
        {
            RepoRoot = FindRepoRoot();
            Iscc = FindIscc();
            if (Iscc == null) return;
            string script = Path.Combine(RepoRoot, @"tests\installer\DecisionHarness.iss");
            var psi = new ProcessStartInfo(Iscc, "/Q \"" + script + "\"") { UseShellExecute = false, CreateNoWindow = true, RedirectStandardError = true };
            using (var p = Process.Start(psi))
            {
                string err = p.StandardError.ReadToEnd();
                p.WaitForExit();
                if (p.ExitCode != 0) throw new InvalidOperationException("Harness compile failed: " + err);
            }
            HarnessExe = Path.Combine(RepoRoot, @"tests\installer\bin\DecisionHarness.exe");
        }

        public string RepoRoot { get; private set; }
        public string Iscc { get; private set; }
        public string HarnessExe { get; private set; }

        public Dictionary<string, string> Run(string args, string psModulePath = null)
        {
            string output = Path.Combine(Path.GetTempPath(), "htharness_" + Guid.NewGuid().ToString("N") + ".ini");
            var psi = new ProcessStartInfo(HarnessExe, "/VERYSILENT /SUPPRESSMSGBOXES /out=\"" + output + "\" " + args) { UseShellExecute = false };
            if (psModulePath != null) psi.EnvironmentVariables["PSModulePath"] = psModulePath;
            using (var p = Process.Start(psi))
            {
                if (!p.WaitForExit(60000)) { p.Kill(); throw new TimeoutException("Harness did not exit"); }
            }
            var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!File.Exists(output)) throw new InvalidOperationException("Harness wrote no results for: " + args);
            string section = "";
            foreach (string line in File.ReadAllLines(output))
            {
                if (line.StartsWith("[")) { section = line.Trim('[', ']'); continue; }
                int eq = line.IndexOf('=');
                if (eq > 0) values[section + "." + line.Substring(0, eq)] = line.Substring(eq + 1);
            }
            File.Delete(output);
            return values;
        }

        internal static string FindRepoRoot()
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory;
            while (dir != null && !File.Exists(Path.Combine(dir, "HTISORipper.sln"))) dir = Path.GetDirectoryName(dir);
            if (dir == null) throw new InvalidOperationException("Repository root not found");
            return dir;
        }

        private static string FindIscc()
        {
            string env = Environment.GetEnvironmentVariable("ISCC_PATH");
            var candidates = new List<string>();
            if (!string.IsNullOrEmpty(env)) candidates.Add(env);
            candidates.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Programs\Inno Setup 6\ISCC.exe"));
            candidates.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Inno Setup 6\ISCC.exe"));
            candidates.Add(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Inno Setup 6\ISCC.exe"));
            return candidates.FirstOrDefault(File.Exists);
        }
    }

    /// <summary>One compiled harness shared by all installer tests; the collection also stops them running in parallel.</summary>
    [CollectionDefinition("Harness")]
    public sealed class HarnessCollection : ICollectionFixture<HarnessFixture> { }

    [Collection("Harness")]
    public class InstallerDecisionTests
    {
        private readonly HarnessFixture _h;

        public InstallerDecisionTests(HarnessFixture h) { _h = h; }

        private Dictionary<string, string> Sim(int major, int minor, int build, int sp = 0, bool server = false,
            bool x64 = true, bool arm64 = false, int release = 0)
        {
            Skip.If(_h.HarnessExe == null, "Inno Setup (ISCC.exe) is not installed.");
            return _h.Run(string.Format("/major={0} /minor={1} /build={2} /sp={3} /server={4} /x64={5} /arm64={6} /release={7}",
                major, minor, build, sp, server ? 1 : 0, x64 ? 1 : 0, arm64 ? 1 : 0, release));
        }

        [SkippableTheory]
        // OS                          major minor build  sp  x64    release   expected build / prerequisite needed
        [InlineData("Win7 SP1 x86 bare",      6, 1, 7601, 1, false, 0,      "net48", "NDP48", "1")]
        [InlineData("Win7 SP1 x64 has 4.8",   6, 1, 7601, 1, true,  528049, "net48", "NDP48", "0")]
        [InlineData("Win7 SP1 has 4.7.2",     6, 1, 7601, 1, true,  461814, "net48", "NDP48", "1")]
        [InlineData("Win8 x86 bare",          6, 2, 9200, 0, false, 0,      "net461", "NDP461", "1")]
        [InlineData("Win8 x64 has 4.5",       6, 2, 9200, 0, true,  378389, "net461", "NDP461", "1")]
        [InlineData("Win8 has 4.6.1",         6, 2, 9200, 0, true,  394271, "net461", "NDP461", "0")]
        [InlineData("Win8.1 has 4.5.1",       6, 3, 9600, 0, true,  378675, "net48", "NDP48", "1")]
        [InlineData("Win10 1507 has 4.6",     10, 0, 10240, 0, true, 393295, "net48", "NDP48", "1")]
        [InlineData("Win10 22H2 has 4.8",     10, 0, 19045, 0, true, 528372, "net48", "NDP48", "0")]
        [InlineData("Win11 has 4.8.1",        10, 0, 22631, 0, true, 533320, "net48", "NDP48", "0")]
        public void SelectsBuildAndPrerequisite(string name, int major, int minor, int build, int sp, bool x64, int release,
            string expectedBuild, string expectedPrereq, string needsPrereq)
        {
            var r = Sim(major, minor, build, sp, false, x64, false, release);
            Assert.True(r["Decision.Supported"] == "1", name);
            Assert.Equal(expectedBuild, r["Decision.Build"]);
            Assert.Equal(expectedPrereq, r["Decision.Prerequisite"]);
            Assert.Equal(needsPrereq, r["Decision.NeedsPrerequisite"]);
            Assert.Equal(x64 ? "x64" : "x86", r["Decision.AppArchitecture"]);
        }

        [SkippableFact]
        public void Windows8_NeverGetsDotNet48_WhateverIsInstalled()
        {
            foreach (int release in new[] { 0, 378389, 379893, 393297, 394271 })
            foreach (bool x64 in new[] { false, true })
            {
                var r = Sim(6, 2, 9200, 0, false, x64, false, release);
                Assert.Equal("net461", r["Decision.Build"]);
                Assert.Equal("NDP461", r["Decision.Prerequisite"]);
                Assert.Equal("394254", r["Decision.RequiredRelease"]);
            }
        }

        [SkippableFact]
        public void WindowsServer2012_UsesDotNet48()
        {
            var r = Sim(6, 2, 9200, 0, server: true);
            Assert.Equal("net48", r["Decision.Build"]);
            Assert.Equal("Windows Server 2012 (build 9200)", r["System.OsName"]);
        }

        [SkippableTheory]
        [InlineData(6, 1, 7600, 0)]   // Windows 7 without SP1
        [InlineData(6, 0, 6002, 2)]   // Windows Vista
        [InlineData(5, 1, 2600, 3)]   // Windows XP
        public void UnsupportedSystems_AreRefusedWithReason(int major, int minor, int build, int sp)
        {
            var r = Sim(major, minor, build, sp);
            Assert.Equal("0", r["Decision.Supported"]);
            Assert.Equal("", r["Decision.Build"]);
            Assert.False(string.IsNullOrEmpty(r["Decision.Reason"]));
        }

        [SkippableFact]
        public void NewerRuntime_SatisfiesOlderRequirement_NoDowngrade()
        {
            // A machine with 4.8.1 needs nothing, for either build.
            Assert.Equal("0", Sim(6, 2, 9200, release: 533325)["Decision.NeedsPrerequisite"]);
            Assert.Equal("0", Sim(10, 0, 19045, release: 533325)["Decision.NeedsPrerequisite"]);
        }

        [SkippableFact]
        public void Arm64_InstallsX64Build()
        {
            var r = Sim(10, 0, 26100, x64: true, arm64: true, release: 533509);
            Assert.Equal("x64", r["Decision.AppArchitecture"]);
            Assert.StartsWith("ARM64", r["System.Architecture"]);
        }

        [SkippableFact]
        public void SupportNotices_AreShownOnlyForUnsupportedWindows()
        {
            Assert.Equal("", Sim(10, 0, 22631, release: 533320)["Decision.SupportNotice"]);
            Assert.Contains("14 October 2025", Sim(10, 0, 19045, release: 528372)["Decision.SupportNotice"]);
            Assert.Contains("no longer receives security updates", Sim(6, 1, 7601, 1)["Decision.SupportNotice"]);
            Assert.Contains("no longer receives security updates", Sim(6, 2, 9200)["Decision.SupportNotice"]);
            Assert.Contains("4.6.1 is no longer supported", Sim(6, 2, 9200)["Decision.RuntimeNotice"]);
            Assert.Equal("", Sim(6, 3, 9600)["Decision.RuntimeNotice"]);
        }

        [SkippableFact]
        public void PrerequisiteDiskSpace_MatchesMicrosoftRequirements()
        {
            Assert.Equal("850", Sim(6, 2, 9200, x64: false)["Decision.PrerequisiteDiskMB"]);
            Assert.Equal("2048", Sim(6, 2, 9200, x64: true)["Decision.PrerequisiteDiskMB"]);
        }

        [SkippableFact]
        public void VersionComparison_DetectsNewerInstalledVersion()
        {
            var r = Sim(10, 0, 19045);
            Assert.True(int.Parse(r["Version.CompareInstalledNewer"]) > 0);
            Assert.Equal("0", r["Version.CompareEqual"]);
        }

        [SkippableFact]
        public void RealDetection_MatchesThisComputer()
        {
            Skip.If(_h.HarnessExe == null, "Inno Setup (ISCC.exe) is not installed.");
            var r = _h.Run("/real=1");
            object release;
            using (var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64)
                .OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full"))
            {
                release = key == null ? 0 : key.GetValue("Release", 0);
            }
            Assert.Equal(Convert.ToString(release), r["System.NetRelease"]);
            Assert.Equal(Environment.Is64BitOperatingSystem ? "x64" : "x86", r["Decision.AppArchitecture"]);
            Assert.Equal(Environment.OSVersion.Version.Major >= 6 ? "1" : "0", r["Decision.Supported"]);
        }
    }

    [Collection("Harness")]
    public class PackageVerificationTests
    {
        private readonly HarnessFixture _h;

        public PackageVerificationTests(HarnessFixture h) { _h = h; }

        private Dictionary<string, string> Verify(string file, string sha, string org = "Microsoft Corporation")
        {
            Skip.If(_h.HarnessExe == null, "Inno Setup (ISCC.exe) is not installed.");
            return _h.Run("/verify=\"" + file + "\" /sha=" + sha + " /org=\"" + org + "\"");
        }

        private static string Sha256(string file)
        {
            using (var sha = SHA256.Create())
            using (var s = File.OpenRead(file))
                return string.Concat(sha.ComputeHash(s).Select(b => b.ToString("x2")));
        }

        [SkippableFact]
        public void MicrosoftSignedFile_PassesHashAndSignature()
        {
            string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe");
            var r = Verify(file, Sha256(file));
            Assert.Equal("1", r["Verify.HashMatches"]);
            Assert.Equal("0", r["Verify.Signature"]);
        }

        [SkippableFact]
        public void SignatureCheck_StillWorks_WhenSetupStartedFromPowerShell7()
        {
            // Regression: from a PowerShell 7 window, PSModulePath points at PowerShell 7 modules that Windows
            // PowerShell 5.1 cannot load, which disabled the signature check (CI run 43129384954).
            Skip.If(_h.HarnessExe == null, "Inno Setup (ISCC.exe) is not installed.");
            string fake = Path.Combine(Path.GetTempPath(), "htfakeps7_" + Guid.NewGuid().ToString("N"));
            string module = Path.Combine(fake, @"Microsoft.PowerShell.Security\7.0.0.0");
            Directory.CreateDirectory(module);
            File.WriteAllText(Path.Combine(module, "Microsoft.PowerShell.Security.psd1"),
                "@{ ModuleVersion='7.0.0.0'; GUID='a94c8c7e-9810-47c0-b8af-65089c13a35a'; PowerShellVersion='7.0'; CompatiblePSEditions=@('Core'); RootModule='Microsoft.PowerShell.Security.dll' }");
            try
            {
                string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe");
                string path = fake + ";" + Environment.GetEnvironmentVariable("PSModulePath");
                var r = _h.Run("/verify=\"" + file + "\" /sha=" + Sha256(file) + " /org=\"Microsoft Corporation\"", path);
                Assert.Equal("0", r["Verify.Signature"]);
            }
            finally { Directory.Delete(fake, true); }
        }

        [SkippableFact]
        public void WrongPublisher_IsRejected()
        {
            string file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe");
            Assert.Equal("1", Verify(file, Sha256(file), "Some Other Company")["Verify.Signature"]);
        }

        [SkippableFact]
        public void CorruptedDownload_FailsHash_AndUnsignedFileFailsSignature()
        {
            string file = Path.Combine(Path.GetTempPath(), "htcorrupt_" + Guid.NewGuid().ToString("N") + ".exe");
            File.WriteAllBytes(file, Encoding.ASCII.GetBytes("not the real installer"));
            try
            {
                var r = Verify(file, new string('a', 64));
                Assert.Equal("0", r["Verify.HashMatches"]);
                Assert.Equal("1", r["Verify.Signature"]);
            }
            finally { File.Delete(file); }
        }

        [SkippableFact]
        public void PinnedRedistributables_VerifyWhenPresent()
        {
            var manifest = ManifestTests.ReadManifest();
            foreach (string id in new[] { "NDP48", "NDP461" })
            {
                string path = Path.Combine(HarnessFixture.FindRepoRoot(), @"installer\redist", manifest[id]["FileName"]);
                Skip.IfNot(File.Exists(path), "Redistributables not downloaded (scripts\\pin-prerequisites.ps1 -Mode Verify).");
                var r = Verify(path, manifest[id]["SHA256"]);
                Assert.Equal("1", r["Verify.HashMatches"]);
                Assert.Equal("0", r["Verify.Signature"]);
            }
        }
    }

    public class ManifestTests
    {
        internal static Dictionary<string, Dictionary<string, string>> ReadManifest()
        {
            var result = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> current = null;
            foreach (string raw in File.ReadAllLines(Path.Combine(HarnessFixture.FindRepoRoot(), @"installer\prerequisites.ini")))
            {
                string line = raw.Trim();
                if (line.Length == 0 || line.StartsWith(";")) continue;
                if (line.StartsWith("[")) { current = new Dictionary<string, string>(); result[line.Trim('[', ']')] = current; continue; }
                int eq = line.IndexOf('=');
                if (eq > 0 && current != null) current[line.Substring(0, eq)] = line.Substring(eq + 1);
            }
            return result;
        }

        [Theory]
        [InlineData("NDP48", "528040")]
        [InlineData("NDP461", "394254")]
        public void Prerequisite_IsPinned_Official_AndSecure(string id, string minRelease)
        {
            var p = ReadManifest()[id];
            Assert.StartsWith("https://download.microsoft.com/", p["Url"]);
            Assert.Matches(new Regex("^[0-9a-f]{64}$"), p["SHA256"]);
            Assert.Matches(new Regex("^[0-9A-F]{40}$"), p["SignerThumbprint"]);
            Assert.True(long.Parse(p["SizeBytes"]) > 10000000);
            Assert.Equal("Microsoft Corporation", p["Publisher"]);
            Assert.Equal(minRelease, p["MinRelease"]);
            Assert.Contains("/norestart", p["InstallArgs"]);
            Assert.EndsWith(p["FileName"], p["Url"]);
        }

        [Fact]
        public void DecisionModule_UsesSameReleaseThresholdsAsManifest()
        {
            string decision = File.ReadAllText(Path.Combine(HarnessFixture.FindRepoRoot(), @"installer\decision.iss"));
            var m = ReadManifest();
            Assert.Contains("RELEASE_NET48  = " + m["NDP48"]["MinRelease"], decision);
            Assert.Contains("RELEASE_NET461 = " + m["NDP461"]["MinRelease"], decision);
        }

        [Fact]
        public void Manifest_ContainsNoVisualCppRedistributable()
        {
            // HT ISO Ripper is 100% managed code; build-release.ps1 enforces this on every package.
            Assert.DoesNotContain(ReadManifest().Keys, k => k.IndexOf("VC", StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }
}
