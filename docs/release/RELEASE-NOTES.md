# HT ISO Ripper 1.0.0 – Release Notes

**Status: pre-release** (published 2026-11-20 for HTREASURE LTD internal testing). It becomes a full release once the blockers below are cleared.

**Website:** [www.htreasure.com](https://www.htreasure.com)

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

## Not included in 1.0.0

Stage 3 features are not implemented yet: lossless compression (.iso.gz/.iso.xz), decompression, ISO Manager, history, BIN/CUE and multisession imaging. Neither the installer nor the application offers them. In-app update checks are also absent, because there is no HTREASURE LTD update service to check against.

## Release blockers

See testing/RELEASE-TEST-REPORT.md for the full list. In short:
1. Code-signing certificate for HTREASURE LTD (installers and binaries are currently unsigned; the build pipeline is ready, see CODE-SIGNING.md).
2. Done: the HTREASURE LTD End-User Licence Agreement (adapted from HT ISO Creator) is now included.
3. Tests on real optical drives and on Windows 7, 8 (x86/x64), 8.1 and 10, which are still pending.
