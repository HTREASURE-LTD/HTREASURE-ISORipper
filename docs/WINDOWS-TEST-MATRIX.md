# Windows Test Matrix

Record each run in `docs/release/RELEASE-TEST-REPORT.md`: date, tester, OS edition, build, architecture, result, Setup log.

## Automated install test – run on every environment first

`scripts/test-installer-e2e.ps1` really installs HT ISO Ripper, then checks:

- registration in Programs and Features, the version, publisher and website;
- every installed file, the documents and the licence;
- that the right build (net48 or net461) and architecture were installed;
- the Start Menu shortcut;
- that the program starts and scans for drives.

It then reinstalls in place and uninstalls, checking that nothing is left behind and that .NET is untouched. It works on PowerShell 2.0, so a stock Windows 7 SP1 can run it. CI runs it on a clean Windows Server 2022 machine for every push.

From an elevated PowerShell on the test PC or VM, with the Setup file copied next to the script:

```bash
powershell -ExecutionPolicy Bypass -File test-installer-e2e.ps1 -Setup HT-ISO-Ripper-Setup-1.0.0.exe
```

Add `-Interactive` to click through the real wizard instead, which is needed when .NET is missing: silent installs never download it. Attach the generated `HTISORipper-e2e-report.txt` to RELEASE-TEST-REPORT.md.

## Environments to prepare (VM snapshots)

| Id | OS | Arch | Starting state |
|---|---|---|---|
| E1 | Windows 7 SP1 | x86 | Clean SP1 + all updates, .NET 4.5.2 or none |
| E2 | Windows 7 SP1 | x64 | Same as E1 |
| E3 | Windows 8 (8.0, build 9200) | x86 | Clean; .NET 4.5 only (as shipped) |
| E4 | Windows 8 (8.0, build 9200) | x64 | Clean; .NET 4.5 only |
| E5 | Windows 8.1 | x64 | Clean; .NET 4.5.1 |
| E6 | Windows 10 1607 | x86 | .NET 4.6.2 (needs 4.8) |
| E7 | Windows 10 22H2 | x64 | .NET 4.8 present |
| E8 | Windows 11 | x64 | .NET 4.8.1 present |
| E9 | Windows 11 on ARM | ARM64 | .NET 4.8.1 present |

Windows 8.0 ISOs come from MSDN / Visual Studio subscriptions or volume licensing. Windows 8.0 is out of support, so keep these VMs offline except while testing the online download.

## W8 – Windows 8.0 validation (E3 and E4); required before claiming Windows 8 support

| Step | Expected |
|---|---|
| Run online Setup | Starts; Compatibility page: "Windows 8 (build 9200)", correct architecture, build ".NET Framework 4.6.1", security and runtime notices |
| Prerequisite page | .NET Framework 4.6.1: Missing / Install; Next disabled until consent |
| Install | Downloads 64 MB from download.microsoft.com, verifies, Microsoft's installer runs |
| Restart prompt (if shown) | Restart now → after sign-in Setup reappears (UAC prompt) and finishes with the same folder and shortcuts |
| Launch app | Main window appears; About shows ".NET Framework 4.6.1 compatibility build" and the correct bitness |
| Disc detection, ISO imaging, verification | As for hardware tests H1 and H3 in TESTING.md (needs a pass-through optical drive) |
| Settings persist | Change a setting, restart the app, setting kept |
| High DPI | 125% and 150% scaling: readable, no clipping (system-DPI scaling) |
| Uninstall | Removed from Programs and Features; settings prompt (default No); ISOs untouched; .NET 4.6.1 still installed |
| Offline Setup on a fresh snapshot without network | Same result with no download |

## Installer scenarios (run on E1/E4/E7 unless noted)

| Id | Scenario | How | Expected |
|---|---|---|---|
| S1 | Clean install, prerequisites present | E7 | No download; app installed and launches |
| S2 | Missing prerequisite | E1, E4, E6 | Consent, download, verify, install, app installed |
| S3 | Failed download | Block download.microsoft.com in the hosts file | Retry dialog (3 attempts), then offline guidance; nothing installed |
| S4 | Interrupted download | Disconnect the network mid-download | Error, retry works after reconnecting |
| S5 | Corrupted download | Redirect to a modified file through a local proxy | "SHA-256 does not match", file deleted, not run |
| S6 | No internet | Network disabled | Offline guidance; offline Setup works |
| S7 | Insufficient disk space | Fill the system drive to < 2 GB (x64) | Compatibility page "Not compatible", Next disabled |
| S8 | No admin permission | Standard user, decline UAC | Setup does not start; nothing changed |
| S9 | Pending restart | Install a Windows update without restarting | Compatibility page shows restart pending |
| S10 | Restart recovery | S2 on E1 where .NET asks for a restart | Restart later → sign in → Setup resumes, no duplicate .NET install |
| S11 | Cancel | Cancel on each page and during download | Clean exit; nothing left in Program Files |
| S12 | Upgrade | Install 1.0.0, then a 1.0.1 test build | In-place upgrade, same folder, shortcuts kept |
| S13 | Downgrade | Install 1.0.1, run 1.0.0 | Refused with message |
| S14 | Uninstall / reinstall | Programs and Features | Clean removal; reinstall works |
| S15 | Silent install | `/VERYSILENT` on E1 without .NET | Fails safely with log entry; nothing downloaded |
| S16 | Silent install with .NET present | `/VERYSILENT` on E7 | Installs |
