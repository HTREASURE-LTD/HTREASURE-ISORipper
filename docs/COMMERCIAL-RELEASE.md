# Commercial Release – Outstanding Business Items

These items need decisions or documents from HTREASURE LTD. Engineering can't complete them, and nothing has been invented to fill them.

| # | Item | Why it blocks release | What is needed |
|---|---|---|---|
| 1 | **Code-signing certificate** issued to HTREASURE LTD | Unsigned installers trigger SmartScreen "Unknown publisher" warnings | **Pipeline ready** for Azure Artifact Signing (recommended; UK organisations eligible) or an OV/EV certificate. HTREASURE LTD must apply. Step-by-step: CODE-SIGNING.md |
| 2 | **Licence agreement text** | Shown by the installer and installed with the program | **Done (2026-11-20):** the HTREASURE LTD EULA from HT ISO Creator, adapted to HT ISO Ripper (personal, non-commercial use free; organisations need a separate licence; England and Wales law; adds a no-circumvention clause). Confirm with your legal adviser. |
| 3 | **Support and website details** | Shown in Programs and Features and the installer | Values for `installer/branding.iss` |
| 4 | **Redistribution of Microsoft .NET installers** | The offline installer bundles them | Legal confirmation, or drop the offline installer |
| 5 | **Real-hardware and legacy-Windows validation** | Required before claiming compatibility | **Partly automated:** scripts/test-installer-e2e.ps1 installs, starts, reinstalls and uninstalls for real; CI runs it on clean Windows Server 2022 for every push. Still needed: run it on Windows 7 SP1, 8, 8.1 and 10 PCs or VMs, plus imaging tests with a real CD/DVD drive (WINDOWS-TEST-MATRIX.md). |
| 6 | **Update service** (optional) | "Check for updates" is not offered without one | Decide whether to provide an update URL |

## Signing pipeline (ready)

`scripts/build-release.ps1` signs, with SHA-256 digests and RFC 3161 timestamps:
1. every application .exe/.dll in all four build folders, before packaging (so the installers and portable ZIPs contain signed binaries);
2. both Setup executables and their uninstallers, through Inno Setup's `SignTool` with `SignedUninstaller`.

It then verifies every signature and stops on any that isn't valid. Without a certificate it builds an UNSIGNED release, says so, and records `"signed": false` in `release-manifest.json`.
