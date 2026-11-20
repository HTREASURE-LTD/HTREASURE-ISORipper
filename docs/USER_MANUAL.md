# HT ISO Ripper 1.0.1 – User Guide

HT ISO Ripper creates exact ISO disc images from data CDs and DVDs. It reads discs **read-only**, needs no administrator rights and no internet connection.

## The main window

- **Header:** the program name and the HTREASURE LTD website link.
- **Menus:**
  - **File:** Start Ripping (Ctrl+R), Choose Output File (Ctrl+S), Open Output Folder, Exit.
  - **Tools:** Refresh Drives (F5), Disc Information (Ctrl+I), Eject Disc (Ctrl+E), View Log, Open Log Folder, Settings.
  - **Help:** User Guide (F1), Troubleshooting, Release Notes, Licence, Third-Party Notices, About.
- **Source Drive**, **Image Options**, **Output Destination** and **Progress:** the four sections described below.
- **Status bar:** shows what HT ISO Ripper is doing, plus the drive and disc type.

## Creating an ISO

1. Start **HT ISO Ripper**. Your CD/DVD drives are detected automatically. If you have more than one, choose one under **Source Drive**.
2. Insert a disc. After a few seconds **Source Drive** shows the disc label, type, file system and image size, and the status bar shows **Disc ready**.
3. Check **Output Destination**. The file name is suggested from the disc label. Click **Save As…** to choose another location. You are always asked before an existing file is replaced.
4. Choose **Image Options**:
   * **Verification:** *Full* (recommended) re-reads the whole disc and compares it with the image. *Structural* re-checks the file only. *Off* checks the size only.
   * **Read speed:** Quiet or Balanced reduce noise and can help with marginal discs. Not every drive supports this.
   * **Retries:** how many times an unreadable sector is retried.
   * **If unreadable:** *Stop imaging* (recommended), or *Continue*, which saves a clearly marked `.INCOMPLETE.iso` with a list of bad sectors.
   * **Eject the disc after a successful rip**.
5. Click **Start Ripping**. **Progress** shows how much has been read, the elapsed and remaining time, the speed and any read errors. **Cancel** stops safely and deletes the partial file.
6. The result window shows the image name, location, size, time, speed, verification result, SHA-256 and read errors. From there you can open the folder, copy the SHA-256, view the log, eject the disc or rip another.

**Appearance:** *Tools → Settings → Theme* switches between Light, Dark and *Same as Windows*.

**IMAGE CREATED AND VERIFIED SUCCESSFULLY** appears only when every sector was read and the image matched the disc on a second full read.

## Discs that cannot be saved as ISO

| Disc | Message |
|---|---|
| Audio CD, Enhanced/Mixed-mode CD | Needs BIN/CUE (coming in a later version) |
| Multisession disc | Not supported in this version |
| Copy-protected video DVD | Not supported. HT ISO Ripper does not bypass copy protection. |
| Blank disc | No data to read |

## Files and folders

* Default output folder: `C:\Users\Public\Documents\HT ISO Ripper`
* While imaging: `NAME.iso.htpart` (temporary, renamed when complete)
* Logs: `%LOCALAPPDATA%\HTREASURE LTD\HT ISO Ripper\Logs` (Tools → View Log)
* Settings: `%APPDATA%\HTREASURE LTD\HT ISO Ripper\settings.xml`

## Troubleshooting

| Message | What to do |
|---|---|
| No optical drive detected | Connect the drive. USB drives appear automatically. |
| Disc not ready | Wait a few seconds for the drive to spin up. |
| Access denied | Your organisation may block CD/DVD access. Contact your administrator. |
| Sector could not be read | Clean the disc, try Quiet speed and more retries, or try another drive. |
| Not enough free space | Choose a destination with more space than the disc size. |
