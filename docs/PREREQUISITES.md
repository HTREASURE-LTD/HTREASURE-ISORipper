# Prerequisites

## Dependency investigation

| Dependency area | Finding | Installer action |
|---|---|---|
| .NET Framework | Required: 4.8 for the main build, 4.6.1+ for the Windows 8 build | Detect; download (online) or use the bundled copy (offline) only if missing, with consent |
| Visual C++ redistributable | **Not required.** HT ISO Ripper contains no native code. `build-release.ps1` loads every shipped .exe/.dll as a .NET assembly and fails the build if any is native. | Shown as "Not required / Skip" |
| Optical-disc access | Windows' own kernel32 IOCTLs (`cdrom.sys`); no driver or library | None |
| ISO imaging, hashing | Own managed code; SHA-256 via Windows CNG | None |
| Compression libraries | None yet (Stage 3). The planned gzip support uses the framework's GZipStream. An XZ/Zstandard library would be bundled as an app-local DLL and, if native, would add a VC++ check. | None in 1.0.0 |
| Windows components | None beyond a standard Windows installation | None |

## Detection logic (exact)

`installer/decision.iss` is the code Setup runs and the tests exercise.

1. **Windows:** `GetWindowsVersionEx` → Major, Minor, Build, ServicePackMajor, ProductType.
2. **Architecture:** `IsX64Compatible` (true on x64 and ARM64) and `IsARM64`.
3. **.NET Framework:** DWORD `Release` under `HKLM\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full`, read from the 64-bit registry view on 64-bit Windows, else the 32-bit view. Missing value = 0.
4. **Satisfied** when `Release ≥ 528040` (4.8) or `≥ 394254` (4.6.1). These are the lowest values Microsoft lists for each version, so every OS variant and newer version passes.
5. **Pending restart:** registry keys `...\Component Based Servicing\RebootPending` or `...\WindowsUpdate\Auto Update\RebootRequired`.
6. **Disk space:** free space on the system drive must be at least 850 MB (32-bit) or 2,048 MB (64-bit) when .NET must be installed. These are Microsoft's stated requirements.

## Packages (pinned 2026-11-12, see installer/prerequisites.ini)

| Id | Package | File | Size | Source | Applies to |
|---|---|---|---|---|---|
| NDP48 | Microsoft .NET Framework 4.8 offline installer | NDP48-x86-x64-AllOS-ENU.exe | 121,346,568 bytes | KB4503548 → fwlink 2088631 → download.microsoft.com | Windows 7 SP1, 8.1, 10, 11 |
| NDP461 | Microsoft .NET Framework 4.6.1 offline installer | NDP461-KB3102436-x86-x64-AllOS-ENU.exe | 67,681,000 bytes | Download Center id 49982 → download.microsoft.com | Windows 8 |

Both packages carry a valid Authenticode signature from O=Microsoft Corporation. The SHA-256 values are in the manifest. The 4.6.1 file's SHA-1 (`83D048D1…5866`) also matches an independent Download Center archive.

**Why the full offline package even for the online installer?** Microsoft's web installers fetch further files themselves, which we couldn't pin. Downloading the complete, signed redistributable lets Setup verify every byte it runs.

**Note on 4.6.1:** Microsoft ended support for .NET 4.6.1 on 26 April 2022. It is still the newest .NET version Windows 8 can install (4.6.2 and later list Windows 8.1 but not Windows 8), so it is used only for Windows 8. Setup tells the user it is out of support and that a free upgrade to Windows 8.1 enables 4.8. Its old fwlink (linkid=671744) now redirects to the Download Center page instead of the file. The manifest therefore records the file URL from that page as `DirectUrl`, which is still subject to the host allow-list and signature check.

**Windows 7:** Microsoft's .NET offline installers need the *Microsoft Root Certificate Authority 2011* in the certificate store. Up-to-date Windows 7 SP1 systems have it. The application is WinForms-only, so the WPF-related D3DCompiler update (KB4019990) doesn't affect HT ISO Ripper.

## Updating the manifest

```bash
powershell -File scripts/pin-prerequisites.ps1 -Mode Pin
```

Review the diff (URL, size, SHA-256, signer), run the installer tests, and commit. Release builds run `-Mode Verify`, which fails on any mismatch.
