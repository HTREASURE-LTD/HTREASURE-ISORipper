# Changelog

## 1.0.0-stage1 – 2026-11-11

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
