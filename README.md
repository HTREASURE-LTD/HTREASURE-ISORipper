# HT ISO Ripper

**Professional CD/DVD to ISO Imaging and Backup Utility** · HTREASURE LTD · Version 1.0.0 (Stages 1 and 4; Stage 3 pending)

HT ISO Ripper reads data CDs and DVDs sector by sector, through documented Windows device interfaces, into standard ISO images. It verifies each image against the disc and never presents a damaged or partial image as a good backup.

## Status

| Feature | Status |
|---|---|
| Visual Studio solution, HTREASURE LTD branding (placeholder logo) | Done |
| Dashboard, Disc Information, Settings, About | Done |
| Optical drive enumeration, insertion/removal detection | Done (polling + `WM_DEVICECHANGE`) |
| Disc metadata: profile, TOC, sessions, ISO 9660/Joliet/El Torito/UDF | Done |
| Sector-level ISO imaging, streaming, temp file + atomic rename | Done |
| Progress, ETA, speed, cancellation (including stalled reads) | Done |
| Bounded retries, Stop/Continue error policy, bad-sector report | Done |
| SHA-256, structural verification, full source-to-image verification | Done |
| Read speed control, tray lock, eject | Done (where the drive supports it) |
| Logging with retention | Done |
| .NET 4.8 main build + .NET 4.6.1 compatibility build (Windows 8.0) | Done |
| Unit tests (mock disc) | 51 passing on both builds |
| Real-drive tests | Written; **not yet run** (no drive on the build machine) |
| Smart installer (online + offline): OS, architecture and .NET detection, automatic build selection, consented and verified .NET download, restart resume | Done (Stage 4) |
| Installer decision and verification tests | 29 passing |
| Release pipeline: package verification, checksums, manifest, signing hooks, GitHub workflows | Done |
| Code signing | **Blocked:** needs an HTREASURE LTD certificate |
| Installer and app on Windows 7 / 8 / 8.1 / 10 | **Not yet tested** (no VMs) |
| Compression, ISO Manager, History, BIN/CUE, multisession | **Stage 3, not started** |

## Quick start

```bash
dotnet build HTISORipper.sln -c Release
```

```bash
dotnet test HTISORipper.sln
```

Run `src\HTISORipper.UI\bin\Release\net48\HTISORipper.exe` on Windows 7 SP1/8.1/10/11, or the `net461` build on Windows 8.0.

Release (installers, portable ZIPs, checksums):

```bash
powershell -File scripts/build-release.ps1
```

## Documentation

* [Architecture and technical decisions](docs/ARCHITECTURE.md): disc-access evaluation, ISO limitations, libraries, design
* [Build instructions](docs/BUILD.md) and release packaging
* [Windows compatibility notes](docs/COMPATIBILITY.md)
* [Test plan](docs/TESTING.md), including the hardware acceptance matrix
* [User manual](docs/USER_MANUAL.md)
* [Installer design](docs/INSTALLER-DESIGN.md), [Prerequisites](docs/PREREQUISITES.md), [Offline installation](docs/OFFLINE-INSTALLATION.md)
* [Windows test matrix](docs/WINDOWS-TEST-MATRIX.md), [Release checklist](docs/RELEASE-CHECKLIST.md), [Commercial release items](docs/COMMERCIAL-RELEASE.md)

## Principles

* The source disc is opened read-only and is never modified.
* No administrator rights, telemetry, advertising, background processes or internet access.
* No DRM, encryption or copy-protection bypass.
* Discs that a standard ISO cannot represent (audio, mixed-mode, multisession) are refused with an explanation, never imaged incompletely.

© 2026 HTREASURE LTD
