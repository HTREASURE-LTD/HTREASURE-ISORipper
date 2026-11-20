# HT ISO Ripper 1.0.1 – Release Notes

**Status: pre-release** (published 2026-11-20 for HTREASURE LTD internal testing). It becomes a full release once the remaining items below are cleared.

**Website:** [www.htreasure.com](https://www.htreasure.com)

## What's new in 1.0.1

- **Licence:** the HTREASURE LTD End-User Licence Agreement (the same terms as HT ISO Creator) replaces the placeholder. Personal, non-commercial use is free; organisations need a separate licence from HTREASURE LTD.
- **Installer tested for real:** a clean Windows machine installs, starts, reinstalls and uninstalls HT ISO Ripper automatically on every code change (33 checks). Testers can run the same test on their own PCs and VMs (`test-installer-e2e.ps1`).
- **Ready for code signing:** the release build supports Azure Artifact Signing and standard OV/EV certificates. Once HTREASURE LTD has its certificate, signed installers need no code changes.

## Included

- HTREASURE LTD application design, shared with HT ISO Creator: blue header banner, File / Tools / Help menus, grouped sections, status bar, and a Light / Dark / Windows theme. HT ISO Ripper has its own logo.
- Sector-exact ISO imaging of data CDs and DVDs, with read retries, safe cancellation and a choice of how to handle unreadable sectors.
- SHA-256 hashing, structural verification and full verification against the source disc.
- Honest handling of discs an ISO can't represent: audio, mixed-mode, multisession and copy-protected discs are refused with an explanation.
- Two builds: .NET Framework 4.8 (Windows 7 SP1, 8.1, 10, 11) and .NET Framework 4.6.1 (Windows 8).
- Smart installer (online and offline):
  - automatic detection of Windows version, architecture and .NET Framework, with automatic build selection;
  - downloads Microsoft .NET Framework only with your consent, then verifies its SHA-256 and Microsoft signature;
  - restart handling with resume after you sign in again;
  - upgrade and uninstall support; your settings are kept by default, and disc images are never touched.
- Portable ZIPs for each build.

## Not included yet

Stage 3 features are not implemented: lossless compression (.iso.gz/.iso.xz), decompression, ISO Manager, history, BIN/CUE and multisession imaging. Neither the installer nor the application offers them. In-app update checks are also absent, because there is no HTREASURE LTD update service to check against.

## Remaining before a full release

See testing/RELEASE-TEST-REPORT.md for the full list.
1. **Code signing:** HTREASURE LTD needs a code-signing certificate (Azure Artifact Signing recommended; see CODE-SIGNING.md). Until then Windows shows "Unknown publisher".
2. **Real hardware and older Windows:** imaging tests with a real CD/DVD drive, and the install test on Windows 7 SP1, 8 (x86/x64), 8.1 and 10.
