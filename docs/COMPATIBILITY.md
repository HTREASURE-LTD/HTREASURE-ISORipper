# Windows Compatibility Notes

## Runtime and builds

HT ISO Ripper is built twice from the same source:

| Build | Folder | For | Differences |
|---|---|---|---|
| **.NET Framework 4.8** (main) | `bin\<Platform>\Release\net48` | Windows 7 SP1, 8.1, 10, 11 | Per-monitor DPI V2 on Windows 10 1703+ |
| **.NET Framework 4.6.1** (compatibility) | `bin\<Platform>\Release\net461` | **Windows 8.0**, or any PC that cannot install 4.8 | System-DPI aware only: sharp at the main monitor's scaling, bitmap-scaled on monitors with other scaling. All imaging, verification and UI features are identical. |

| OS | .NET 4.8 | .NET 4.6.1 | Build to ship |
|---|---|---|---|
| Windows 7 SP1 x86/x64 | Installable (`ndp48-x86-x64-allos-enu.exe`) | Installable | 4.8 |
| Windows 8 (8.0) x86/x64 | **Cannot be installed** | Installable (`NDP461-KB3102436-x86-x64-AllOS-ENU.exe`) | **4.6.1** |
| Windows 8.1 x86/x64 | Installable | Installable | 4.8 |
| Windows 10 x86/x64 | Built in from 1903 | Runs (4.8 is backwards compatible) | 4.8 |
| Windows 11 x64 | Built in | Runs | 4.8 |

For signed installers on Windows 7, the SHA-2 code-signing update KB4474419 is required.

How the 4.6.1 build differs in source: `App.net461.config` omits the WinForms DPI section (4.6.1 rejects it as unknown), and `CardPanel` reads DPI from `Graphics.DpiX` instead of `Control.DeviceDpi` (`#if NET461`). The compiler builds it against the 4.6.1 reference assemblies, so any newer API is a build error.

## Win32 APIs used and their minimum versions

| API / IOCTL | Minimum Windows | Used for |
|---|---|---|
| `CreateFile` on `\\.\X:` (read-only) | XP | Device access |
| `ReadFile`, `SetFilePointerEx` | XP | Sector reads |
| `IOCTL_STORAGE_CHECK_VERIFY2` | XP | Media present / ready |
| `IOCTL_STORAGE_QUERY_PROPERTY` | XP | Vendor / product / bus |
| `IOCTL_CDROM_READ_TOC_EX` | XP | TOC, sessions |
| `IOCTL_CDROM_GET_CONFIGURATION` | XP SP2 | Media profile |
| `IOCTL_CDROM_GET_DRIVE_GEOMETRY_EX` | XP | Capacity |
| `IOCTL_CDROM_SET_SPEED` | XP | Read speed (optional) |
| `IOCTL_DVD_READ_STRUCTURE` | XP | Copy-protection detection |
| `IOCTL_STORAGE_MEDIA_REMOVAL`, `IOCTL_STORAGE_EJECT_MEDIA` | XP | Tray lock, eject |
| `FSCTL_ALLOW_EXTENDED_DASD_IO` | XP | Read full track |
| `CancelSynchronousIo` | **Vista** | Abort a stalled read |
| `MoveFileEx` (`MOVEFILE_WRITE_THROUGH`) | XP | Atomic finalisation |
| `GetDiskFreeSpaceEx`, `GetVolumeInformation` | XP | Space check, label |
| `SHA256Cng` (CNG) | Vista | Hashing |
| WinForms per-monitor V2 DPI | Windows 10 1703 (falls back to system DPI on 7/8.1) | High-DPI |

No Windows 10/11-only API is used. Where a newer feature exists (per-monitor DPI), Windows 7/8.1 fall back automatically.

## Privileges

* Runs `asInvoker`. No administrator rights are needed for any Stage 1 operation.
* Standard users can read optical drives unless the policy *"Devices: Restrict CD-ROM access to locally logged-on user only"* or removable-storage deny policies are set. The app reports "Device access denied" in that case.

## Builds

* `AnyCPU` (Prefer32Bit off): one binary, native on both x86 and x64.
* `x86` and `x64`: platform-specific builds for the separate installers (`-p:Platform=x86|x64`).
* All P/Invoke structures are pointer-size aware (`GET_CONFIGURATION_IOCTL_INPUT` is sized from `IntPtr.Size`). The other structures have no pointers.

## Verified so far (this build)

| Check | Result |
|---|---|
| Release build x86 / x64 / AnyCPU, net48 and net461 | Builds, 0 warnings |
| x86 exe runs as a 32-bit process on Windows 11 x64 (net48 and net461) | Yes |
| UI starts, no-drive state on Windows 11 (net48 and net461) | Yes |
| Unit tests (mock device) | 51/51 pass on net48, and 51/51 on net462 running against the net461 libraries |
| Windows 8.0 | **Not yet tested.** Needs a Windows 8.0 VM with .NET 4.6.1. |
| Installer decision logic (all supported Windows versions, simulated) | 29/29 installer tests pass |
| Installer wizard on simulated Windows 8 x86 (preview build) | Pages and consent gate verified |
| Real installation and uninstallation | **Not yet tested** (needs UAC approval or VMs; see WINDOWS-TEST-MATRIX.md) |
| Real optical hardware | **Not yet tested.** The build machine has no optical drive. |
| Windows 7 / 8.1 / 10 | **Not yet tested.** Needs VMs or physical machines (see TESTING.md). |
