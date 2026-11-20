# Offline Installation

## HT-ISO-Ripper-Offline-Setup-1.0.1.exe

- Contains both application builds (x86/x64) and Microsoft's .NET Framework 4.8 and 4.6.1 offline installers.
- Needs no internet connection at any point.
- Runs exactly the same compatibility checks as the online installer and picks the same build.
- Uses a bundled .NET installer only if this computer is missing it, and only after the user agrees.
- Before running a bundled package, checks it against the pinned SHA-256 (also at compile time) and its Microsoft signature.
- Handles restarts and resume in the same way as the online installer.

Size: about 182 MB. Setup extracts only the one .NET package it needs.

## Redistribution

The Microsoft .NET Framework offline installers are designed for redistribution with applications, under Microsoft's .NET Framework redistributable licence terms, which the installer displays. **Before public release, HTREASURE LTD must confirm those terms cover distributing them inside this installer** (see COMMERCIAL-RELEASE.md). If not, ship only the online installer and give the procedure below.

## Manual procedure (no offline installer)

1. On a computer with internet access, download Microsoft's offline installer:
   - .NET Framework 4.8 (Windows 7 SP1, 8.1, 10, 11): support.microsoft.com, KB4503548
   - .NET Framework 4.6.1 (Windows 8): Microsoft Download Center, id 49982
2. Check it: right-click → Properties → Digital Signatures should show *Microsoft Corporation*. Or compare its SHA-256 with `installer/prerequisites.ini`:
   ```bash
   certutil -hashfile NDP48-x86-x64-AllOS-ENU.exe SHA256
   ```
3. Copy it to the offline computer, run it, and restart if asked.
4. Run `HT-ISO-Ripper-Setup-1.0.1.exe`. It detects .NET and downloads nothing.

## Windows 7 note

If Microsoft's installer reports a certificate-chain error, the computer is missing the *Microsoft Root Certificate Authority 2011*. Install the Windows 7 SP1 root-certificate update from Microsoft first.
