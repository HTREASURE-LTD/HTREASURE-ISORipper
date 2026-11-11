# Installer (Stage 4 – not started)

Planned: WiX Toolset v4 MSI per architecture and runtime:
- HTISORipper-1.0.0-x86.msi / -x64.msi from the net48 build (Windows 7 SP1, 8.1, 10, 11), detecting .NET 4.8 (Release DWORD >= 528040)
- HTISORipper-1.0.0-win8-x86.msi / -win8-x64.msi from the net461 build (Windows 8.0), detecting .NET 4.6.1 (Release DWORD >= 394254)
A bootstrapper may pick the right package by OS version.

Requirements carried from the specification:
- HTREASURE LTD branding (banner/dialog bitmaps from assets/branding), licence agreement page (text from LICENSE once approved)
- Install directory selection (default %ProgramFiles%\HTREASURE LTD\HT ISO Ripper)
- Desktop shortcut (optional) and Start Menu shortcut using assets/icons/htisoripper.ico
- Proper uninstaller (Programs and Features), version information, Authenticode signing of MSI and binaries
- Per-machine install; the application itself runs without elevation

No installer configuration is included in Stage 1, so none is presented as working.
