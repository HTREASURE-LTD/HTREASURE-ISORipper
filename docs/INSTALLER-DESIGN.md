# Installer Design (Stage 4)

## 1. Technology choice

| | Inno Setup 6.7 (chosen) | WiX Toolset (Burn bundle + MSI) |
|---|---|---|
| Runs with no .NET installed | Yes, native setup engine | Yes, native Burn engine. The *managed* bootstrapper UI, though, itself needs .NET. |
| Windows 7 SP1 / 8 / 8.1 / 10 / 11 | Yes (6.3+: Windows 7 SP1 minimum, which matches our floor) | Yes |
| Dynamic compatibility and prerequisite tables with a consent checkbox | Pascal Script custom pages, straightforward | The standard bootstrapper UI is static; dynamic tables need a custom C++ bootstrapper |
| SHA-256-verified download with progress, retry and abort | Built in (`CreateDownloadPage`) | Built in (payload hashes) |
| Restart and resume | Restart prompt built in; resume via RunOnce (implemented here) | Built in |
| Online and offline from one source | One script, `/DOffline=1` | Two bundle definitions |
| MSI for enterprise deployment (GPO/Intune/SCCM) | No (EXE with silent switches) | Yes |
| Licence for commercial use | Free, any purpose including commercial | WiX v6+ asks commercial users to pay an open-source maintenance fee |
| Already in our toolchain | Yes | No |

**Decision: Inno Setup.** It meets every requirement with the least moving parts, and the whole installer is one readable script that HTREASURE LTD can maintain. If enterprise customers later need an MSI, WiX can wrap the same build folders. The decision logic in `installer/decision.iss` is documented in PREREQUISITES.md and could be ported.

## 2. Files

| File | Purpose |
|---|---|
| `installer/HTISORipper.iss` | Main script: wizard, packaging, download, prerequisite install, resume, uninstall |
| `installer/decision.iss` | Pure detection and decision functions, shared with the test harness |
| `installer/prerequisites.ini` | Versioned prerequisite manifest: official URLs, sizes, SHA-256, signer thumbprints |
| `installer/branding.iss` | Publisher details (empty until supplied; never invented) |
| `installer/PRIVACY.txt` | Privacy page shown during setup and installed with the app |
| `installer/images/*.bmp` | Wizard images (`scripts/generate-installer-images.ps1`) |
| `tests/installer/DecisionHarness.iss` | Compiles `decision.iss` for automated tests |
| `scripts/pin-prerequisites.ps1` | Pin or verify the Microsoft packages |
| `scripts/build-release.ps1` | Full release pipeline |

## 3. Flow

```
Setup.exe (native, 32-bit engine; installs the 64-bit app in 64-bit mode on x64/ARM64)
 │ InitializeSetup: DetectSystem → SelectBuild → NetSatisfied → disk space → existing version → resume state
 ▼
Welcome → Licence → Privacy → System Compatibility Check ──(Next disabled if not compatible)
 → Prerequisite Check ──(Next disabled until "I agree" when something must be installed)
 → Install location → Shortcuts → Ready (summary incl. prerequisites)
 ▼ Next on Ready
 Acquire prerequisite: online = download page (HTTPS, SHA-256 checked by Inno, up to 3 attempts)
                       offline = extract bundled copy
                       both    = SHA-256 recheck + Authenticode (Microsoft Corporation)
 ▼ PrepareToInstall
 Run Microsoft installer "/passive /norestart", check exit code:
   0 → re-read registry; must now satisfy the requirement
   3010/1641 → save resume state, RunOnce, "Restart now / later" (Inno's restart page)
   1602 cancelled · 5100 requirements not met · other → error with log location
 ▼
 Copy exactly one build folder (net48|net461 × x86|x64), shortcuts, uninstall registration → Finish (Launch)
```

## 4. Build selection

| Windows | Build | Runtime requirement (`Release` DWORD) | Prerequisite if missing |
|---|---|---|---|
| 7 without SP1, Vista, XP | refused | – | – |
| 7 SP1 | net48 | ≥ 528040 | .NET 4.8 |
| 8 (client) | **net461** | ≥ 394254 | .NET 4.6.1. **4.8 is never offered.** |
| Server 2012 (6.2 server) | net48 | ≥ 528040 | .NET 4.8 |
| 8.1, 10, 11 and later | net48 | ≥ 528040 | .NET 4.8 |

Architecture: x64 build on 64-bit Windows (x64, and ARM64 via x64 emulation); x86 build on 32-bit Windows. Comparing with "≥" means any newer 4.x runtime satisfies an older requirement, and nothing is ever downgraded or reinstalled.

**Windows version detection.** Setup uses `GetVersionEx`, through Inno's `GetWindowsVersionEx`. Setup.exe's manifest declares Windows 7–11 compatibility, so the true version is reported rather than a compatibility-shimmed one. `ProductType` separates clients from servers.

## 5. Security

- Only HTTPS URLs on `download.microsoft.com`, enforced at compile time (`#error`) and by the pin script's host allow-list. Certificate validation is the operating system's; it is never disabled or bypassed.
- Every package is pinned by SHA-256 at release time from a file whose Microsoft Authenticode signature was verified. At run time:
  1. Inno Setup verifies the SHA-256 during download and deletes a mismatch.
  2. Setup re-hashes the file (offline bundles too).
  3. `Get-AuthenticodeSignature` must report a valid signature from O=Microsoft Corporation. An unsigned, tampered or wrong-publisher package is deleted and never run. If the chain can't be built (an old root store), or PowerShell is blocked, the pinned SHA-256 has already proven the exact Microsoft file; that state is logged and Setup continues.
- No web scraping: URLs come only from the manifest.
- Downloads go to Setup's private temporary folder, are deleted after installation, and failed downloads are removed.
- Silent installs never download anything, because no one has given consent.
- Resume state lives in HKLM, which only administrators can write. It is trusted only if the Setup file the RunOnce entry points to has the SHA-256 recorded at save time and the same version.

## 6. Uninstall

The uninstaller removes the program files, shortcuts, uninstall registration and resume data. It then asks, defaulting to **No**, whether to remove the current user's settings and logs. It never removes disc images or the .NET Framework.

## 7. Known limitations

- Resume uses the RunOnce key of the account that ran Setup. If Setup was elevated with a *different* administrator account ("over-the-shoulder" UAC), the resume entry belongs to that account; the user simply runs Setup again, and detection makes this safe.
- "Check for updates when launched" is not offered: HTREASURE LTD has no update service yet (`UpdatesUrl` in branding.iss is empty).
- Setup is an EXE, not an MSI (see section 1).
