# HT ISO Ripper 1.0.0 – Release Notes

**Status: release candidate.** Not for public distribution until the blockers below are cleared.

## Included

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
1. Code-signing certificate for HTREASURE LTD (installers and binaries are currently unsigned).
2. Approved licence text (LICENSE is a placeholder) and official support/website details.
3. Tests on real optical drives and on Windows 7, 8 (x86/x64), 8.1 and 10, which are still pending.
