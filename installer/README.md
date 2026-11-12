# HT ISO Ripper Installer

Inno Setup 6.3+ script (built and tested with 6.7) that produces both customer installers:

| Output | Command |
|---|---|
| `release\v1.0.0\online\HT-ISO-Ripper-Setup-1.0.0.exe` | `ISCC installer\HTISORipper.iss` |
| `release\v1.0.0\offline\HT-ISO-Ripper-Offline-Setup-1.0.0.exe` | `ISCC /DOffline=1 installer\HTISORipper.iss` |

Normally run `scripts\build-release.ps1`, which builds and verifies the application first.

Both installers detect Windows version, architecture and .NET Framework, install exactly one of the four application builds (net48/net461 × x86/x64), and install Microsoft .NET Framework only if it is missing and the user agrees.

| File | Purpose |
|---|---|
| `HTISORipper.iss` | Main script |
| `decision.iss` | Detection and decision logic (shared with `tests\installer\DecisionHarness.iss`) |
| `prerequisites.ini` | Pinned Microsoft packages: URL, size, SHA-256, signer |
| `branding.iss` | Publisher details (fill in when supplied) |
| `PRIVACY.txt` | Privacy page |
| `images\` | Wizard bitmaps |
| `redist\` | Downloaded Microsoft packages for the offline build (git-ignored) |

Developer preview of the wizard pages, which needs no admin rights and never ships, with optional simulation of another OS:

```bash
ISCC /DPreview=1 installer\HTISORipper.iss
```

```bash
HT-ISO-Ripper-Setup-1.0.0.exe /simmajor=6 /simminor=2 /simbuild=9200 /simx64=0 /simrelease=0
```

See docs\INSTALLER-DESIGN.md, docs\PREREQUISITES.md and docs\OFFLINE-INSTALLATION.md.
