# Third-party components

## Distributed with HT ISO Ripper

| Component | Where | Licence | Notes |
|---|---|---|---|
| Inno Setup 6.7 (setup and uninstaller engine) | Inside both Setup .exe files | Inno Setup License (free for any purpose, including commercial use; keep the copyright notice) | Copyright (C) 1997-2026 Jordan Russell, (C) 2000-2026 Martijn Laan. jrsoftware.org |
| Microsoft .NET Framework 4.8 offline installer (NDP48-x86-x64-AllOS-ENU.exe) | Offline Setup only | Microsoft .NET Framework redistributable licence terms (shown by Microsoft's installer) | Unmodified, verified by SHA-256 and Microsoft signature. **HTREASURE LTD to confirm the redistribution terms before release.** |
| Microsoft .NET Framework 4.6.1 offline installer (NDP461-KB3102436-x86-x64-AllOS-ENU.exe) | Offline Setup only | Same as above | Same as above |

The online Setup does not include the Microsoft packages. It downloads them from download.microsoft.com, and only with the user's consent.

## Used only to build and test (not distributed)

| Component | Licence |
|---|---|
| xUnit.net 2.9 | Apache License 2.0 |
| Xunit.SkippableFact | Microsoft Public License |
| Microsoft.NET.Test.Sdk | MIT |
| Microsoft.NETFramework.ReferenceAssemblies | MIT |

HT ISO Ripper itself contains no third-party code and no native code, so no Visual C++ redistributable is needed.
