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

Output: `src\HTISORipper.UI\bin\<Platform>\Release\net48\HTISORipper.exe`, plus the five `HTISORipper.*.dll` libraries and `HTISORipper.exe.config`. All of these files must be distributed together.

## Replacing the placeholder branding

Replace `assets\branding\ht_logo.png` (256×256), `assets\branding\ht_logo_64.png` (64×64) and `assets\icons\htisoripper.ico` with official artwork of the same names, then rebuild. To regenerate the placeholders, run `scripts\generate-placeholder-branding.ps1`.

## Release packaging (Stage 4 outline)

1. Bump `Version`, `AssemblyVersion` and `FileVersion` in `Directory.Build.props` and `AppInfo.Version`.
2. `dotnet test` must be green and the hardware matrix in TESTING.md signed off.
3. Build `Release|x86` and `Release|x64`.
4. Authenticode-sign `HTISORipper.exe` and the DLLs (`signtool sign /fd SHA256 /tr <timestamp-url> /td SHA256`).
5. Build the installers (WiX, see `installer/README.md`) and sign them.
6. Publish SHA-256 checksums of the installers.
