# Installing HT ISO Ripper 1.0.0

## Which file do I need?

| File | Use it when |
|---|---|
| `HT-ISO-Ripper-Setup-1.0.0.exe` (about 2 MB) | The computer has internet access. Microsoft .NET Framework is downloaded only if it is missing. |
| `HT-ISO-Ripper-Offline-Setup-1.0.0.exe` (about 180 MB) | The computer has no internet access, or a secure connection to Microsoft is not possible (common on old Windows 7 systems). |

You don't need to know your Windows version, 32/64-bit architecture or .NET Framework version. Setup detects them and picks the right build for you.

## Supported Windows

Windows 7 SP1, Windows 8, Windows 8.1, Windows 10 and Windows 11, in both 32-bit and 64-bit editions. Windows 11 on ARM uses the 64-bit build.

## What Setup does

1. **Welcome**, **Licence agreement**, then **Privacy information**.
2. **System Compatibility Check:** shows your Windows version, architecture, the build that will be installed, the .NET Framework it needs and what is installed, free space, and any existing HT ISO Ripper installation.
3. **Prerequisite Check:** lists only what is missing. If Microsoft .NET Framework is needed, Setup explains what it is, its size and where it comes from, and continues only after you tick **I agree**.
4. **Install location** (default `C:\Program Files\HTREASURE LTD\HT ISO Ripper`) and **shortcuts**.
5. **Ready to Install:** a summary of everything that will happen.
6. If needed, Setup downloads .NET Framework from `download.microsoft.com`, verifies its SHA-256 fingerprint and Microsoft signature, and runs Microsoft's installer.
7. HT ISO Ripper is installed, and you can launch it from the final page.

### If Windows needs to restart

Installing .NET Framework sometimes requires a restart. Setup then offers **Restart now** or **Restart later**. After you sign in again, Setup starts automatically, asks for administrator permission (Windows' UAC prompt), checks the computer again and finishes with the options you chose. Nothing is installed twice.

If you prefer, just run Setup again yourself after restarting. It detects what is already installed.

## Administrator rights

Setup needs administrator rights to install into Program Files and to install .NET Framework. HT ISO Ripper itself always runs without administrator rights.

## Silent installation (IT administrators)

```bash
HT-ISO-Ripper-Setup-1.0.0.exe /VERYSILENT /SUPPRESSMSGBOXES /NORESTART /LOG="%TEMP%\htisoripper-setup.log"
```

Silent installs never download prerequisites, because no one has given consent. Install .NET Framework first (4.8, or 4.6.1 on Windows 8) with your normal software-deployment tools, then run Setup. If the prerequisite is missing, Setup stops with an error and the log explains why.

## Upgrading and uninstalling

- **Upgrade:** run the newer Setup. It installs over the existing version and keeps your folder and shortcuts. Setup refuses to downgrade.
- **Uninstall:** use *Settings → Apps* or *Control Panel → Programs and Features*. You'll be asked whether to remove your HT ISO Ripper settings and logs (the default is to keep them). Your disc images and the Microsoft .NET Framework are never removed.
