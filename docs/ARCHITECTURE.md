# HT ISO Ripper – Architecture and Technical Decisions

HTREASURE LTD · Version 1.0.0 · Stage 1

## 1. Disc-access approach (evaluation)

| Option | Admin rights | Win7–11 | What it can read | Verdict |
|---|---|---|---|---|
| Copy visible files from the mounted drive | No | Yes | Files only; loses boot records, file-system metadata, sector layout | **Rejected.** That is not a disc image (spec §5). |
| `ReadFile` on the volume device `\\.\D:` + documented CD-ROM/DVD/storage IOCTLs | **No** | Yes (XP+) | 2048-byte user-data sectors at absolute LBAs, TOC, sessions, media profile, DVD copyright descriptor, speed, eject, tray lock | **Selected for Stage 1.** |
| SCSI Pass-Through (`IOCTL_SCSI_PASS_THROUGH_DIRECT`) with MMC commands | Usually requires a read/write handle; often refused for standard users | Yes | Raw 2352-byte sectors, subchannel, C2 errors | Planned for Stage 3 (BIN/CUE), behind an explicit, explained elevation prompt only if needed. |
| `IOCTL_CDROM_RAW_READ` | No | Yes | Raw 2352-byte audio sectors | Planned for Stage 3 audio CDs. |
| IMAPI2 | No | Yes | Designed for burning, not imaging | Not suitable. |
| Third-party drivers (SPTD, etc.) | Install/kernel driver | Varies | Everything | **Rejected:** unnecessary driver install, security risk. |

**How the selected method works**

* The volume device is opened with `GENERIC_READ` only and `FILE_FLAG_NO_BUFFERING`, so every read goes to the drive (no stale OS cache) and the source can never be written.
* `FSCTL_ALLOW_EXTENDED_DASD_IO` lets reads continue past the size reported by the mounted CDFS/UDFS file system, so the whole data track is imaged, not just what the file system declares.
* The Windows CD-ROM class driver maps volume offset `n × 2048` to absolute LBA `n`, so the image is LBA 0 … lead-out − 1 in order. That is exactly the layout of a standard ISO, including system area, volume descriptors, El Torito boot catalogue/images and UDF structures.
* Reads use a page-aligned `VirtualAlloc` buffer, default 32 sectors (64 KB, safe for USB bridges), configurable 16/32/64.
* A blocking read on a damaged sector can be aborted with `CancelSynchronousIo` (Vista+), so **Cancel** works even while the drive is retrying internally.

## 2. Limitations of standard ISO imaging (and how they are handled)

An ISO file is a flat sequence of 2048-byte user-data sectors. Anything that is not 2048-byte user data in a single contiguous data area cannot be represented. HT ISO Ripper detects these cases and **refuses with an explanation** rather than producing a misleading ISO:

| Disc | Why ISO is wrong | Behaviour in Stage 1 | Correct format |
|---|---|---|---|
| Audio CD | Audio tracks are 2352-byte raw frames, no user-data sectors | Refused, explained | BIN/CUE (Stage 3) |
| Mixed-mode / Enhanced CD | ISO would keep the data track and silently drop audio | Refused, explained | BIN/CUE (Stage 3) |
| Multisession data CD/DVD | Unreadable lead-out/lead-in gaps between sessions; later sessions refer to absolute LBAs of earlier ones | Refused, explained | Session-aware ISO (Stage 3) |
| CD-ROM XA Mode 2 Form 2 (VCD, some PlayStation discs) | 2324-byte sectors | Reads fail and are reported as unreadable – never zero-filled into a "good" image | BIN/CUE (Stage 3) |
| CSS-protected video DVD | Encrypted sectors; we do not bypass protection | Refused when the DVD copyright descriptor reports protection | Not supported (by design) |
| Blank / unfinalised disc | No readable data | Refused | – |
| Blu-ray | Outside product scope | Refused | – |

**CD-R run-out blocks.** Track-at-once CD-Rs report up to 2 extra sectors at the end of the track that cannot be read. If, and only if, unreadable sectors fall in the final 2 sectors of a CD track **and** lie beyond the ISO 9660 volume size, the image ends before them. This is logged and shown on the result screen. An unreadable sector anywhere the file system needs is always an error.

## 3. Libraries and frameworks

| Component | Choice | Reason |
|---|---|---|
| Runtime | .NET Framework 4.8 | Ships with Windows 10 1903+ and 11; installs on Windows 7 SP1, 8.1. Windows 8 (non-.1) supports .NET 4.6.x at most, so it is **not** covered; see COMPATIBILITY.md. |
| UI | Windows Forms | Mature, works identically on Win7–11, per-monitor DPI V2 support from .NET 4.7. |
| Language | C# 7.3 | Default for .NET Framework, so it builds in VS 2019 and VS 2022. |
| Device access | Win32 P/Invoke (kernel32) | No third-party code anywhere near the hardware. |
| Hashing | `SHA256Cng` (Windows CNG) | FIPS-validated, fast, Vista+; managed fallback. |
| Tests | xUnit 2.9 (Apache-2.0), Xunit.SkippableFact (MS-PL) | Test-only; not shipped. |
| Build | Microsoft.NETFramework.ReferenceAssemblies (MIT) | Build-time only; lets machines without the 4.8 targeting pack build. |

**Shipped third-party dependencies: none.** Stage 3 compression will add a Deflate implementation from the framework (`System.IO.Compression.GZipStream`, no new dependency) and evaluate an XZ/LZMA2 library; Zstandard needs a native DLL per architecture and will only be offered if it can be packaged and tested for both x86 and x64.

## 4. Solution structure

```
HTISORipper.sln
src/
  HTISORipper.Logging        ILogger, FileLogger (daily files, retention)
  HTISORipper.Core           Models, ISectorSource, FilesystemProbe (ISO 9660/Joliet/El Torito/UDF), PathUtil, settings, hashing
  HTISORipper.DiscEngine     Win32 interop, OpticalDevice (IOCTLs), OpticalSectorSource, DiscInspector, DriveMonitor
  HTISORipper.Imaging        ImagingEngine: streaming, retries, error policy, temp file + atomic rename
  HTISORipper.Verification   ImageVerifier: size, re-hash, structure, source compare
  HTISORipper.UI             WinForms app -> HTISORipper.exe
tests/
  HTISORipper.UnitTests        51 tests using an in-memory mock disc
  HTISORipper.IntegrationTests Real-drive tests, skipped unless HTISO_TEST_DRIVE is set
assets/branding, assets/icons  Placeholder logo + icon (replaceable)
scripts/                       generate-placeholder-branding.ps1
docs/                          This file, BUILD, COMPATIBILITY, TESTING, USER_MANUAL
installer/                     Stage 4
```

Dependency direction: `UI → Imaging, Verification, DiscEngine → Core → Logging`. The imaging and verification engines only know the `ISectorSource` interface. The real implementation is `OpticalSectorSource`; tests use `MockSectorSource`. Hardware-independent logic is fully tested, and the shipped code path performs real disc reads.

## 5. Imaging pipeline

```
Inspect ─► Validate ─► Pre-checks ─► Read loop ─────────────► Finalise ─► Verify ─► Report
 TOC        drive,      output exists?  block read (32 sect)    flush(true)  size      CompletionStatus
 profile    disc,       free space      ├ medium error →        SHA-256      re-hash   decides wording
 sessions   path        temp .htpart      per-sector retries    MoveFileEx   structure
 FS probe               SetLength()     ├ not ready → removed   (atomic,     source
 assess                 (reserve)       ├ cancel → delete temp  write-thru)  compare
                                        └ write + hash in order
```

* Temporary output `NAME.iso.htpart` is renamed to `NAME.iso` only after a complete, flushed write (`MoveFileEx … MOVEFILE_WRITE_THROUGH`). A pre-existing `NAME.iso` is replaced only after the new image is complete, and only with the user's confirmation.
* Error policy **Stop** (default): the first unrecoverable sector fails the job and the partial file is deleted. Policy **Continue**: unreadable sectors are zero-filled, the file is named `NAME.INCOMPLETE.iso`, `NAME.INCOMPLETE.iso.badsectors.txt` lists every LBA, and the result can never show as verified.
* The tray is locked during imaging (`IOCTL_STORAGE_MEDIA_REMOVAL`) where the drive supports it. A forced removal is still detected (`ERROR_NOT_READY`/`ERROR_MEDIA_CHANGED`) and stops the job safely.

## 6. Verification levels

| Level | Checks | Wording shown |
|---|---|---|
| Off | File size equals sectors × 2048 | "IMAGE CREATED – NOT VERIFIED" |
| Structural | Size; file re-read and re-hashed (must equal the hash computed while writing); ISO 9660/UDF descriptors valid and volume fits in file | "IMAGE CREATED – STRUCTURAL CHECK PASSED" (states the disc was not re-read) |
| Full (default) | Structural + every sector re-read from the disc and compared byte-for-byte | "**IMAGE CREATED AND VERIFIED SUCCESSFULLY**" only if all match |

The wording comes from one class, `CompletionStatus`, and a unit test enforces that the success headline appears only for full source verification.

Note: the drive's own small read cache may still hold the last few blocks of the first pass. Because the verify pass starts again from LBA 0, practically all sectors are re-read from the media.

## 7. Functions that require real optical-drive testing

These cannot be proven with the mock device. They are covered by `tests/HTISORipper.IntegrationTests` and the manual matrix in TESTING.md:

1. `OpticalDevice.GetCurrentProfile`: profile codes across CD/DVD media types and drive vendors.
2. `OpticalDevice.ReadToc` / `ReadLastSessionNumber`: TOC parsing on single-session, multisession, audio, mixed-mode and DVD (emulated TOC) discs.
3. `OpticalDevice.GetDiskSize`: agreement with the TOC lead-out on DVD±R/RW, DL and DVD+RW (formatted capacity).
4. `OpticalDevice.GetDvdCopyProtectionType`: CSS detection on a commercial DVD-Video.
5. `OpticalSectorSource.Read`: block reads, short reads at the end of the track, Win32 error codes from scratched discs, `FSCTL_ALLOW_EXTENDED_DASD_IO` behaviour, USB bridge transfer limits.
6. `CancelSynchronousIo` during a stalled read on a damaged disc.
7. CD-R TAO run-out handling (real TAO disc).
8. `TrySetReadSpeed`, `SetMediaLock`, `Eject` on drives with and without support.
9. `DriveMonitor` insertion/removal detection, and disc removal during imaging (USB unplug, forced eject).
10. Boot preservation: image a bootable installer disc, then boot the ISO in a VM.
11. Standard-user (non-admin) access on Windows 7, 8.1, 10 and 11, including the "Restrict CD-ROM access" group policy.
