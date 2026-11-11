# Test Plan

## Automated (no hardware) – `tests/HTISORipper.UnitTests`

`MockSectorSource` simulates a disc: transient and permanent medium errors, removal at an LBA, per-read callbacks for cancellation, and sectors that read back differently. `IsoBuilder` creates images with real ISO 9660 / Joliet / El Torito / UDF descriptors.

| Spec scenario | Test(s) |
|---|---|
| 1/2 Data CD/DVD to ISO (bit-exact) | `CleanDisc_ProducesBitExactImage_AndMatchingSha256` |
| 3 Bootable disc keeps boot record | `EndToEnd_RipThenVerify_IsSourceVerified`, `DetectsJolietElToritoAndUdf` |
| 4/5/6 ISO 9660, Joliet, UDF detection | `FilesystemProbeTests` |
| 7 Multisession handled honestly | `Multisession_IsRefused` |
| 9 Damaged disc: retries, abort, incomplete | `TransientErrors_*`, `PermanentError_*`, `TrailingRunOutSectors_*`, `UnreadableSector_InsideFilesystem_*` |
| 10 Disc removed during imaging | `DiscRemoved_FailsWithDiscRemoved_AndDeletesPartialFile` |
| 11 Destination full | `InsufficientSpace_IsDetectedBeforeReading` (runtime disk-full also mapped to the same error) |
| 12 Cancellation | `Cancellation_StopsSafely_AndDeletesPartialFile` |
| 14 SHA-256 verification | `ImageVerifierTests` |
| Audio / mixed-mode / protected refused | `DiscAssessmentTests` |
| No unsafe overwrite | `ExistingOutput_*` |
| "Verified" wording only when truly verified | `CompletionWordingTests` |
| Path validation, settings | `PathUtilTests`, `SettingsTests` |

13 (compression round trip) belongs to Stage 3.

## Hardware acceptance – `tests/HTISORipper.IntegrationTests`

```bash
set HTISO_TEST_DRIVE=D
```

```bash
dotnet test tests/HTISORipper.IntegrationTests
```

Each run images the inserted disc, fully verifies it against the disc, and checks that the El Torito boot record is present in the image when the disc is bootable.

## Manual hardware matrix (record the result, drive model and log file)

| # | Disc / condition | Expected |
|---|---|---|
| H1 | Pressed data CD-ROM | Verified; SHA-256 equals a reference tool (e.g. `dd` on Linux) |
| H2 | CD-R written TAO | Verified; result notes "run-out sectors excluded" if present |
| H3 | DVD-ROM / DVD±R / DVD±RW | Verified |
| H4 | DVD±R DL (> 4.7 GB) | Verified; progress and ETA plausible; memory flat |
| H5 | Windows installer DVD | Verified; ISO boots in a VM (Hyper-V/VirtualBox) |
| H6 | Joliet-only long file names | Label from Joliet shown |
| H7 | UDF-only DVD | Detected as UDF; verified |
| H8 | Multisession CD | Refused with explanation |
| H9 | Audio CD, Enhanced CD | Refused; BIN/CUE recommended |
| H10 | Commercial DVD-Video (CSS) | Refused; "does not bypass" message |
| H11 | Blank CD-R / DVD-R | Refused as blank/unreadable |
| H12 | Scratched disc | Retries logged; Stop policy fails cleanly; Continue policy produces `.INCOMPLETE.iso` + list |
| H13 | Eject/unplug USB drive mid-rip | "Disc removed" error; no output file left |
| H14 | Destination USB stick nearly full | Space error before reading |
| H15 | Cancel during a stalled read | Stops within seconds; temp deleted |
| H16 | Empty drive; insert disc while app open | Status changes automatically to Disc Ready |
| H17 | USB external drive | Same as H1/H3 |
| H18 | Standard (non-admin) user account | Everything works without elevation |
| H19 | Windows 7 SP1 x86 and x64, 8.1, 10, 11 | H1, H3, H5, H16 pass on each |
| H20 | 150% / 200% display scaling | Layout readable, no clipping |
