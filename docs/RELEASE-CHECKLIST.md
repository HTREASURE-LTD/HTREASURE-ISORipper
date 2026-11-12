# Release Checklist – HT ISO Ripper 1.0.0

Each item needs a name and date. **Do not publish until every box is ticked.**

## Content
- [ ] Version set in `Directory.Build.props`, `AppInfo.Version` and the `release.yml` input
- [ ] CHANGELOG.md and docs/release/RELEASE-NOTES.md final
- [ ] LICENSE replaced with the approved HTREASURE LTD licence text (currently a placeholder)
- [ ] Official logo and icon in `assets/`; then `scripts/generate-installer-images.ps1`
- [ ] Publisher website and support details in `installer/branding.iss` (empty values are hidden)
- [ ] Redistribution of the Microsoft .NET offline installers confirmed (or the offline installer withdrawn)

## Build
- [ ] Clean checkout, `scripts/pin-prerequisites.ps1 -Mode Verify` passes
- [ ] `scripts/build-release.ps1` with signing variables set completes with no warnings
- [ ] `release-manifest.json` shows `"signed": true` and `"workingTreeClean": true`
- [ ] Every .exe and .dll in the installers and portable ZIPs is signed and timestamped (`signtool verify /pa /v`)

## Testing
- [ ] All automated suites green (unit net48, unit net462/net461, installer)
- [ ] WINDOWS-TEST-MATRIX.md: S1–S16 and W8 recorded in RELEASE-TEST-REPORT.md
- [ ] Hardware matrix H1–H20 (TESTING.md) on at least one internal SATA and one USB drive
- [ ] Bootable-disc ISO boots in a VM
- [ ] Antivirus false-positive check on the signed installers (Microsoft Defender + one other)

## Publish (explicit approval required)
- [ ] Approval recorded from: ____________________
- [ ] GitHub release created manually from the `release.yml` artifact (the workflow never publishes)
- [ ] SHA256SUMS.txt attached and checked after upload
