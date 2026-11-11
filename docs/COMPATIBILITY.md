# Windows Compatibility Notes

## Runtime

| OS | .NET Framework 4.8 | Status |
|---|---|---|
| Windows 7 SP1 x86/x64 | Installable (offline installer `ndp48-x86-x64-allos-enu.exe`) | Supported. Needs SP1 and SHA-2 code-signing update KB4474419 for signed installers. |
| Windows 8 (8.0) x86/x64 | **Not supported by Microsoft** (last is .NET 4.6.1) | **Not covered by a 4.8 build.** Free upgrade to 8.1 required, or a separate 4.6.x build (decision pending). |
| Windows 8.1 x86/x64 | Installable | Supported |
| Windows 10 x86/x64 | Built in from 1903; installable on older builds | Supported |
| Windows 11 x64 | Built in | Supported |

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
| Release build x86 / x64 / AnyCPU | Builds, 0 warnings |
| x86 exe runs as a 32-bit process on Windows 11 x64 | Yes |
| UI starts, no-drive state on Windows 11 | Yes |
| Unit tests (mock device) | 51/51 pass |
| Real optical hardware | **Not yet tested.** The build machine has no optical drive. |
| Windows 7 / 8.1 / 10 | **Not yet tested.** Needs VMs or physical machines (see TESTING.md). |
