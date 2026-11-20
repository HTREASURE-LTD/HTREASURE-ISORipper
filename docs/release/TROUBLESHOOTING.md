# Troubleshooting

## Setup

| Problem | Cause and solution |
|---|---|
| "HT ISO Ripper requires Windows 7 SP1 or later" | Windows 7 without Service Pack 1, or Windows Vista or earlier. Install SP1 through Windows Update. |
| "Setup could not download … securely" | The computer could not open a secure (HTTPS/TLS 1.2) connection to download.microsoft.com. This is common on Windows 7 systems that are missing updates. Setup never weakens security to work around it. Use **HT-ISO-Ripper-Offline-Setup-1.0.1.exe** instead, or see OFFLINE-INSTALLATION below. |
| "SHA-256 fingerprint does not match" | The download was damaged or altered in transit. The file was deleted and not run. Try again on a different network, or use the offline installer. |
| "does not carry a valid Microsoft digital signature" | The package failed Windows' signature check. It was deleted and not run. Use the offline installer from an official HTREASURE LTD source. |
| .NET Framework installer fails on Windows 7 offline | Microsoft's offline installer needs the *Microsoft Root Certificate Authority 2011* certificate. Install the latest Windows 7 SP1 updates first. Microsoft's logs are in `%TEMP%` (files starting with `dd_`). |
| "A newer version is already installed" | Setup does not downgrade. Uninstall the newer version first if you really need this one. |
| "The system drive needs at least … MB free" | .NET Framework needs 850 MB (32-bit) or 2 GB (64-bit) of free space on the Windows drive. |
| Setup continues after restart but then stops | Run the same Setup file again. It re-checks everything and only does what is still needed. |
| Setup log | Every run writes a log to `%TEMP%\Setup Log yyyy-mm-dd #nnn.txt`. |

## Offline installation without the offline installer

1. Download Microsoft's official .NET Framework installer on another computer:
   - Windows 7 SP1, 8.1, 10, 11: .NET Framework 4.8 offline installer (Microsoft KB4503548).
   - Windows 8: .NET Framework 4.6.1 offline installer (Microsoft Download Center, id 49982).
2. Copy it to the target computer and run it.
3. Run `HT-ISO-Ripper-Setup-1.0.1.exe`. It detects .NET and downloads nothing.

## Application

See USER-GUIDE.md → Troubleshooting for disc-reading problems.
