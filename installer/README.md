# Installer (Stage 4 – not started)

Planned: WiX Toolset v4 MSI per architecture (HTISORipper-1.0.0-x86.msi / -x64.msi), plus a bootstrapper that checks for .NET Framework 4.8.

Requirements carried from the specification:
- HTREASURE LTD branding (banner/dialog bitmaps from assets/branding), licence agreement page (text from LICENSE once approved)
- Install directory selection (default %ProgramFiles%\HTREASURE LTD\HT ISO Ripper)
- Desktop shortcut (optional) and Start Menu shortcut using assets/icons/htisoripper.ico
- .NET Framework 4.8 detection (Release DWORD >= 528040)
- Proper uninstaller (Programs and Features), version information, Authenticode signing of MSI and binaries
- Per-machine install; the application itself runs without elevation

No installer configuration is included in Stage 1, so none is presented as working.
