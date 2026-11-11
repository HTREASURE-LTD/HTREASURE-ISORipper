# HT ISO Ripper

**Professional CD/DVD to ISO Imaging and Backup Utility** · HTREASURE LTD · Version 1.0.0 (Stage 1)

HT ISO Ripper reads data CDs and DVDs sector by sector, through documented Windows device interfaces, into standard ISO images. It verifies each image against the disc and never presents a damaged or partial image as a good backup.

## Stage 1 status

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
| Unit tests (mock disc) | 51 passing |
| Real-drive tests | Written; **not yet run** (no drive on the build machine) |
| Compression, ISO Manager, History, BIN/CUE, multisession | Stage 3 |
| Installer, accessibility pass, compatibility sign-off | Stage 4 |

## Quick start

```bash
dotnet build HTISORipper.sln -c Release
```

```bash
dotnet test HTISORipper.sln
```

Run `src\HTISORipper.UI\bin\Release\net48\HTISORipper.exe`.

## Documentation

* [Architecture and technical decisions](docs/ARCHITECTURE.md): disc-access evaluation, ISO limitations, libraries, design
* [Build instructions](docs/BUILD.md) and release packaging
* [Windows compatibility notes](docs/COMPATIBILITY.md)
* [Test plan](docs/TESTING.md), including the hardware acceptance matrix
* [User manual](docs/USER_MANUAL.md)

## Principles

* The source disc is opened read-only and is never modified.
* No administrator rights, telemetry, advertising, background processes or internet access.
* No DRM, encryption or copy-protection bypass.
* Discs that a standard ISO cannot represent (audio, mixed-mode, multisession) are refused with an explanation, never imaged incompletely.

© 2026 HTREASURE LTD
