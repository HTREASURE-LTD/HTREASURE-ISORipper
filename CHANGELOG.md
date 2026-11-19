# Changelog

## 1.0.0 – 2026-11-19

### Changed
- Applied the HTREASURE LTD application design template shared with HT ISO Creator: dark-blue header banner with the www.htreasure.com link, File / Tools / Help menus, grouped Source Drive, Image Options, Output Destination and Progress sections, a status bar, standard Windows buttons with one accent primary button, and Disc Information, Settings and About as separate windows.
- New HT ISO Ripper logo and icon (disc with a "rip to image" arrow), distinct from HT ISO Creator's.

### Added
- Theme setting: Light, Dark or same as Windows (dark title bar on Windows 10 2004+ and 11).
- Help menu opens the installed User Guide, Troubleshooting, Release Notes, Licence and Third-Party Notices.
- Installer and Programs and Features list https://www.htreasure.com as the publisher website.

### Fixed
- Setup's Microsoft signature check could not run when Setup was started from a PowerShell 7 window (inherited module path). Setup now gives Windows PowerShell its own module path; a regression test covers it. This was the cause of the first CI failure.

## 1.0.0 development stages – 2026-11-11

### Added
- Stage 4: smart Inno Setup installer (online and offline) with Windows version, architecture and .NET detection, automatic net48/net461 and x86/x64 selection, consented download of Microsoft .NET from download.microsoft.com with SHA-256 and Authenticode verification, restart and resume, upgrade and uninstall handling. Pinned prerequisite manifest, 29 installer tests, release pipeline (package verification, portable ZIPs, SHA256SUMS, release manifest, signing hooks) and CI/release GitHub workflows.
- .NET Framework 4.6.1 compatibility build alongside the 4.8 build, so Windows 8.0 is supported; unit tests run against both.
- Visual Studio solution (.NET Framework 4.8, WinForms) with Core, Logging, DiscEngine, Imaging, Verification and UI projects.
- Read-only optical-drive engine using documented IOCTLs: drive enumeration, media state, MMC profile, TOC/sessions, capacity, DVD copy-protection detection, read speed, tray lock, eject.
- Disc inspection with ISO 9660, Joliet, El Torito and UDF detection, and an honest ISO-suitability assessment (audio, mixed-mode, multisession and protected discs are refused with an explanation).
- Streaming imaging engine: ordered sector copy, block reads with per-sector retry fallback, Stop/Continue error policies, CD-R run-out handling, temp file with atomic write-through rename, free-space pre-check, safe cancellation.
- Verification: size, file re-hash against the streaming SHA-256, structure validation, full source-to-image comparison.
- Branded dashboard, Disc Information, Settings and About views, result window, log viewer, splash screen.
- Placeholder HTREASURE LTD logo and icon generator.
- 51 unit tests with a mock disc device; hardware integration tests (opt-in).
