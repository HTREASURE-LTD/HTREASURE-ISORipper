# HT ISO Ripper 1.0.1 – Release Test Report

Test environment: Windows 11 build 26300 x64, .NET Framework 4.8.1, Inno Setup 6.7. No optical drive, no Windows 7/8/8.1/10 machines or VMs, and not elevated (no UAC approval possible). Installer end-to-end tests also run in CI on clean Windows Server 2022 runners.

**Status key:** Passed (how verified) · Failed · Blocked (cannot run here, and why) · Not tested · N/A

## Summary

Counts are rows in the tables below. A row that passed automated checks but still needs real hardware or a VM counts as Blocked.

| Status | Count |
|---|---|
| Passed | 22 |
| Failed | 0 |
| Blocked | 20 |
| Not tested | 0 |
| N/A (Stage 3 not implemented) | 3 |

**Release decision: NOT READY for public distribution.** Blockers: code signing, licence text, real-drive and legacy-Windows validation.

## Automated suites

| Suite | Result |
|---|---|
| Unit tests on the net48 build (mock disc engine) | Passed: 51/51 |
| Unit tests on the net461 libraries (run under net462) | Passed: 51/51 |
| Installer decision and verification tests (real `decision.iss`, run through a compiled harness) | Passed: 30/30 |
| Hardware integration tests | Blocked: no optical drive (3 skipped) |

## Build and packaging

| # | Scenario | Status |
|---|---|---|
| B1 | net48 x86, net48 x64, net461 x86, net461 x64 Release builds | Passed (`build-release.ps1`) |
| B2 | Each package folder has exactly the expected files, all managed code, correct runtime config | Passed (`build-release.ps1` package verification) |
| B3 | Online and offline installers compile; offline bundles match the pinned SHA-256 | Passed (compile-time checks in HTISORipper.iss) |
| B4 | Pinned Microsoft packages: HTTPS download.microsoft.com, valid Microsoft signature, SHA-256 matches | Passed (`pin-prerequisites.ps1 -Mode Verify`; the 4.6.1 SHA-1 also matches an independent Download Center archive) |
| B5 | SHA256SUMS.txt and release-manifest.json generated | Passed |
| B6 | Authenticode signing of binaries and installers | **Blocked:** no HTREASURE LTD code-signing certificate |

## Installer behaviour

| # | Scenario | Status |
|---|---|---|
| I1 | Correct build and prerequisite for Windows 7 SP1, 8, 8.1, 10, 11 and Server 2012 (x86/x64/ARM64) | Passed (InstallerDecisionTests, simulated systems) |
| I2 | Windows 8 never offered .NET 4.8, at any installed .NET release | Passed |
| I3 | Newer .NET satisfies an older requirement; no downgrade or reinstall | Passed |
| I4 | Unsupported systems (Windows 7 without SP1, Vista, XP) refused with a reason | Passed |
| I5 | Real detection matches this PC (OS, architecture, .NET release) | Passed |
| I6 | Corrupted download rejected (SHA-256); unsigned package or wrong publisher rejected | Passed (PackageVerificationTests) |
| I7 | Wizard pages: Welcome, Licence, Privacy, Compatibility, Prerequisites (consent gate), Location, Tasks, Ready | Passed (preview build, simulated Windows 8 x86; Next stays disabled until consent is given) |
| I8 | Clean installation (silent, administrator) | Passed: CI end-to-end test on clean Windows Server 2022, run 44644917111, 33/33 checks |
| I9 | Installation with all prerequisites present | Passed: same run; no download, correct net48 x64 build, program starts |
| I10 | Installation with a missing prerequisite (download, verify, install .NET) | **Blocked:** needs a VM without .NET 4.8 |
| I11 | Failed / interrupted download, then retry | **Blocked:** needs a VM; manual procedure in WINDOWS-TEST-MATRIX.md |
| I12 | No internet connection: offline guidance shown | **Blocked:** needs a VM |
| I13 | Insufficient disk space for the prerequisite | Passed for the decision logic (requirement values tested); **Blocked** end to end |
| I14 | No administrator permission (standard user declines UAC) | **Blocked:** manual |
| I15 | Pending restart detected and shown | **Blocked:** needs a machine with a pending restart |
| I16 | Restart required after .NET, then resume after sign-in | **Blocked:** needs a VM |
| I17 | Installation cancellation | **Blocked:** manual |
| I18 | Upgrade over a previous version; downgrade refused | **Blocked:** no earlier released version exists yet; downgrade logic Passed (version comparison test) |
| I19 | Uninstall: files, shortcuts and registration removed; .NET untouched | Passed: same run (silent uninstall; the settings prompt is skipped in silent mode by design) |
| I20 | Reinstallation in place | Passed: same run (exit 0, one Programs and Features entry, same folder) |
| I21 | Silent install refuses to download prerequisites | **Blocked:** manual |
| I22 | Signature check still works when Setup is started from a PowerShell 7 window | Passed (regression test; reproduced and fixed locally) |

## Windows compatibility

| # | System | Status |
|---|---|---|
| W1 | Windows 11 x64: application starts (net48 and net461 builds) | Passed (Stage 1/2 smoke tests) |
| W6 | Windows Server 2022 (Windows 10 21H2 core) x64: real install, start, reinstall, uninstall | Passed: CI end-to-end test, 33/33 checks |
| W2 | Windows 10 x86/x64 | **Blocked:** no VM |
| W3 | Windows 8.1 x86/x64 | **Blocked:** no VM |
| W4 | Windows 8.0 x86 and x64 with .NET 4.6.1 | **Blocked:** no VM. Not validated; do not claim Windows 8 compatibility until WINDOWS-TEST-MATRIX.md section W8 passes. |
| W5 | Windows 7 SP1 x86/x64 | **Blocked:** no VM |

## Application functions

| # | Scenario | Status |
|---|---|---|
| A1 | Optical-drive detection | **Blocked:** no optical drive (logic covered by unit tests) |
| A2 | ISO creation from a real disc | **Blocked:** no optical drive |
| A3 | SHA-256 verification | Passed (unit tests); **Blocked** on real discs |
| A4 | Disc-read failures, safe cancellation | Passed (mock-disc unit tests); **Blocked** on real discs |
| A5 | Lossless compression | N/A: Stage 3 not implemented |
| A6 | Decompression | N/A: Stage 3 not implemented |
| A7 | Compression round trip | N/A: Stage 3 not implemented |
