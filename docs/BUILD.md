# Build Instructions

## Requirements

* Windows 10/11 developer machine
* **Visual Studio 2022** (17.x) with the *.NET desktop development* workload, or Visual Studio 2019 16.11+
  *or* the .NET SDK 6+ for command-line builds (the SDK builds .NET Framework 4.8 projects)
* Internet access for the first NuGet restore (test packages, reference assemblies)

The .NET Framework 4.8 targeting pack is optional: the `Microsoft.NETFramework.ReferenceAssemblies` package supplies it.

## Visual Studio

1. Open `HTISORipper.sln`.
2. Set **HTISORipper.UI** as the startup project.
3. Choose configuration `Debug` or `Release` and platform `Any CPU`, `x86` or `x64`.
4. Press F5 to debug. Designer files (`*.Designer.cs`) open in the Windows Forms designer.
5. *Test → Run All Tests* runs the unit tests. Hardware tests show as skipped unless `HTISO_TEST_DRIVE` is set.

## Command line

```bash
dotnet build HTISORipper.sln -c Release
```

```bash
dotnet build HTISORipper.sln -c Release -p:Platform=x86
```

```bash
dotnet build HTISORipper.sln -c Release -p:Platform=x64
```

```bash
dotnet test HTISORipper.sln -c Release
```

Each command builds both target frameworks:

* `src\HTISORipper.UI\bin\<Platform>\Release\net48\`: main build for Windows 7 SP1, 8.1, 10 and 11
* `src\HTISORipper.UI\bin\<Platform>\Release\net461\`: compatibility build for Windows 8.0

Each folder holds `HTISORipper.exe`, `HTISORipper.exe.config` and the five `HTISORipper.*.dll` libraries, which must be distributed together. Don't mix files from the two folders.

To build only one framework, add `-f net48` or `-f net461`. `dotnet test` runs the unit tests twice: on net48, and on net462 against the net461 libraries.

## Replacing the placeholder branding

Replace `assets\branding\ht_logo.png` (256×256), `assets\branding\ht_logo_64.png` (64×64) and `assets\icons\htisoripper.ico` with official artwork of the same names, then rebuild. To regenerate the placeholders, run `scripts\generate-placeholder-branding.ps1`.

## Installers and release packaging

Also needs **Inno Setup 6.3+** (built and tested with 6.7), from jrsoftware.org or `winget install JRSoftware.InnoSetup`.

```bash
powershell -File scripts/build-release.ps1
```

The script runs all tests, builds the four configurations, verifies each package (exact files, all managed code, correct runtime config), verifies the pinned Microsoft prerequisites, signs (if configured), compiles the online and offline installers, and writes portable ZIPs, documentation, `SHA256SUMS.txt` and `release-manifest.json` to `release\v1.0.0\`.

The first time on a new machine, the offline installer needs the Microsoft packages in `installer\redist`. Verify mode downloads them from the pinned URLs and checks them:

```bash
powershell -File scripts/pin-prerequisites.ps1 -Mode Verify
```

Signing: see docs/COMMERCIAL-RELEASE.md. Checklist: docs/RELEASE-CHECKLIST.md.

GitHub: `.github/workflows/ci.yml` builds and tests every push and pull request. `release.yml` (manual) produces the release as a workflow artifact and never publishes a GitHub Release.
