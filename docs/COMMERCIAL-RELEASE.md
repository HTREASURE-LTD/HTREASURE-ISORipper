# Commercial Release – Outstanding Business Items

These items need decisions or documents from HTREASURE LTD. Engineering can't complete them, and nothing has been invented to fill them.

| # | Item | Why it blocks release | What is needed |
|---|---|---|---|
| 1 | **Authenticode code-signing certificate** issued to HTREASURE LTD (OV or EV) | Unsigned installers trigger SmartScreen "Unknown publisher" warnings and look untrustworthy | Certificate (EV recommended for immediate SmartScreen reputation) plus the CA's timestamp URL. Then set `HT_SIGN_THUMBPRINT` and `HT_TIMESTAMP_URL` locally, or the `HT_SIGN_PFX_BASE64` / `HT_SIGN_PFX_PASSWORD` secrets and the `HT_TIMESTAMP_URL` variable in GitHub. EV certificates on hardware tokens can't be used from GitHub-hosted runners; sign locally instead. |
| 2 | **Licence agreement text** | The installer shows `LICENSE`, which is a placeholder | Approved EULA |
| 3 | **Support and website details** | Shown in Programs and Features and the installer | Values for `installer/branding.iss` |
| 4 | **Redistribution of Microsoft .NET installers** | The offline installer bundles them | Legal confirmation, or drop the offline installer |
| 5 | **Real-hardware and legacy-Windows validation** | Required before claiming compatibility | VMs and test drives (WINDOWS-TEST-MATRIX.md) |
| 6 | **Update service** (optional) | "Check for updates" is not offered without one | Decide whether to provide an update URL |

## Signing pipeline (ready)

`scripts/build-release.ps1` signs, with SHA-256 digests and RFC 3161 timestamps:
1. every application .exe/.dll in all four build folders, before packaging (so the installers and portable ZIPs contain signed binaries);
2. both Setup executables and their uninstallers, through Inno Setup's `SignTool` with `SignedUninstaller`.

It then verifies every signature and stops on any that isn't valid. Without a certificate it builds an UNSIGNED release, says so, and records `"signed": false` in `release-manifest.json`.
