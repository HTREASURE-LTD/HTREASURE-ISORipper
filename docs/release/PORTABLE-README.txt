HT ISO Ripper 1.0.1 - Portable packages
=======================================

Most users should use HT-ISO-Ripper-Setup-1.0.1.exe instead. It picks the right build automatically and
installs Microsoft .NET Framework if it is missing.

The portable ZIPs need no installation and write nothing to Program Files or the registry. Settings and logs
are stored in your user profile, just as with the installed version.

Pick the ZIP that matches your Windows:

  Windows 7 SP1, 8.1, 10, 11, 64-bit   HT-ISO-Ripper-Portable-1.0.1-net48-x64.zip
  Windows 7 SP1, 8.1, 10, 32-bit       HT-ISO-Ripper-Portable-1.0.1-net48-x86.zip
  Windows 8, 64-bit                    HT-ISO-Ripper-Portable-1.0.1-net461-x64.zip
  Windows 8, 32-bit                    HT-ISO-Ripper-Portable-1.0.1-net461-x86.zip

Requirements: Microsoft .NET Framework 4.8 (net48 packages) or 4.6.1 or later (net461 packages).
Windows 10 version 1903 and later, and Windows 11, already include .NET Framework 4.8.

Extract all files to one folder and run HTISORipper.exe. Keep the files together, and don't mix files from
different packages.

Check the ZIP against checksums\SHA256SUMS.txt before use:
  certutil -hashfile HT-ISO-Ripper-Portable-1.0.1-net48-x64.zip SHA256

(c) 2026 HTREASURE LTD
