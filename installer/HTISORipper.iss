// HT ISO Ripper Setup (Inno Setup 6.3+; built and tested with 6.7).
//
// One script produces both customer installers:
//   ISCC HTISORipper.iss                 -> HT-ISO-Ripper-Setup-1.0.0.exe          (downloads .NET only if missing)
//   ISCC /DOffline=1 HTISORipper.iss     -> HT-ISO-Ripper-Offline-Setup-1.0.0.exe  (.NET installers bundled)
// Normally run through scripts\build-release.ps1, which builds the application first.
//
// Setup is a native program: it runs on a computer with no .NET Framework at all.

#ifndef AppVersion
  #define AppVersion "1.0.0"
#endif
#ifndef Offline
  #define Offline 0
#endif
#ifndef OutputRoot
  #define OutputRoot "..\release\v" + AppVersion
#endif

#include "branding.iss"

// ---------------------------------------------------------------- prerequisite manifest (compile time)
#define Manifest SourcePath + "prerequisites.ini"
#define NDP48_NAME   ReadIni(Manifest, "NDP48", "Name", "")
#define NDP48_URL    ReadIni(Manifest, "NDP48", "Url", "")
#define NDP48_FILE   ReadIni(Manifest, "NDP48", "FileName", "")
#define NDP48_SHA    ReadIni(Manifest, "NDP48", "SHA256", "")
#define NDP48_SIZE   ReadIni(Manifest, "NDP48", "SizeBytes", "0")
#define NDP48_ARGS   ReadIni(Manifest, "NDP48", "InstallArgs", "")
#define NDP48_INFO   ReadIni(Manifest, "NDP48", "InfoPage", "")
#define NDP461_NAME  ReadIni(Manifest, "NDP461", "Name", "")
#define NDP461_URL   ReadIni(Manifest, "NDP461", "Url", "")
#define NDP461_FILE  ReadIni(Manifest, "NDP461", "FileName", "")
#define NDP461_SHA   ReadIni(Manifest, "NDP461", "SHA256", "")
#define NDP461_SIZE  ReadIni(Manifest, "NDP461", "SizeBytes", "0")
#define NDP461_ARGS  ReadIni(Manifest, "NDP461", "InstallArgs", "")
#define NDP461_INFO  ReadIni(Manifest, "NDP461", "DownloadCenterPage", "")

#if NDP48_SHA == "" || NDP461_SHA == ""
  #error Prerequisites are not pinned. Run scripts\pin-prerequisites.ps1 -Mode Pin first.
#endif
#if Pos("https://download.microsoft.com/", NDP48_URL) != 1 || Pos("https://download.microsoft.com/", NDP461_URL) != 1
  #error Prerequisite URLs must be https://download.microsoft.com/ URLs.
#endif

#if Offline
  #define Redist SourcePath + "redist\"
  #if !FileExists(Redist + NDP48_FILE) || !FileExists(Redist + NDP461_FILE)
    #error Offline build needs the pinned packages in installer\redist. Run scripts\pin-prerequisites.ps1 -Mode Verify.
  #endif
  #if LowerCase(GetSHA256OfFile(Redist + NDP48_FILE)) != LowerCase(NDP48_SHA) || LowerCase(GetSHA256OfFile(Redist + NDP461_FILE)) != LowerCase(NDP461_SHA)
    #error A bundled prerequisite does not match its pinned SHA-256.
  #endif
#endif

// ---------------------------------------------------------------- application builds (compile time)
#define AppBin SourcePath + "..\src\HTISORipper.UI\bin\"
#if !FileExists(AppBin + "x64\Release\net48\HTISORipper.exe") || !FileExists(AppBin + "x86\Release\net48\HTISORipper.exe") || !FileExists(AppBin + "x64\Release\net461\HTISORipper.exe") || !FileExists(AppBin + "x86\Release\net461\HTISORipper.exe")
  #error Release builds missing. Build Release for x86 and x64 first (scripts\build-release.ps1 does this).
#endif

[Setup]
AppId={{7C1F3B2E-5A4D-4E8B-9C21-3F6A8D0B4E17}
AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
AppPublisher={#AppPublisher}
AppCopyright={#AppCopyright}
#if AppPublisherUrl != ""
AppPublisherURL={#AppPublisherUrl}
#endif
#if SupportUrl != ""
AppSupportURL={#SupportUrl}
#endif
#if SupportPhone != ""
AppSupportPhone={#SupportPhone}
#endif
#if UpdatesUrl != ""
AppUpdatesURL={#UpdatesUrl}
#endif
AppComments={#AppDescription}
VersionInfoVersion={#AppVersion}.0
VersionInfoCompany={#AppPublisher}
VersionInfoCopyright={#AppCopyright}
VersionInfoProductName={#AppName}
VersionInfoProductVersion={#AppVersion}
#if Offline
VersionInfoDescription={#AppName} Offline Setup
OutputDir={#OutputRoot}\offline
OutputBaseFilename=HT-ISO-Ripper-Offline-Setup-{#AppVersion}
#else
VersionInfoDescription={#AppName} Setup
OutputDir={#OutputRoot}\online
OutputBaseFilename=HT-ISO-Ripper-Setup-{#AppVersion}
#endif

; Windows 7 SP1 is the oldest supported system (also Inno Setup 6.3+'s own minimum).
MinVersion=6.1sp1
ArchitecturesAllowed=x86compatible x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
#ifdef Preview
; Developer preview of the wizard pages only (ISCC /DPreview=1): no admin rights, never shipped.
PrivilegesRequired=lowest
#else
; Program Files and the .NET Framework both need administrator rights.
PrivilegesRequired=admin
#endif
DefaultDirName={autopf}\HTREASURE LTD\HT ISO Ripper
DefaultGroupName=HT ISO Ripper
DisableProgramGroupPage=yes
DisableWelcomePage=no
DisableDirPage=no
AllowNoIcons=yes
LicenseFile=..\LICENSE
InfoBeforeFile=PRIVACY.txt
SetupIconFile=..\assets\icons\htisoripper.ico
UninstallDisplayIcon={app}\HTISORipper.exe
UninstallDisplayName={#AppName}
WizardStyle=modern
WizardImageFile=images\wizard-large-100.bmp,images\wizard-large-200.bmp
WizardSmallImageFile=images\wizard-small-100.bmp,images\wizard-small-200.bmp
Compression=lzma2/max
SolidCompression=yes
SetupLogging=yes
CloseApplications=yes
RestartApplications=no
AppMutex=Local\HTREASURE.HTISORipper.SingleInstance
; Upgrades: same AppId -> installs over the previous version in place.
UsePreviousAppDir=yes
UsePreviousTasks=yes
#ifdef SignTool
SignTool={#SignTool}
SignedUninstaller=yes
#endif

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Messages]
WelcomeLabel1=Welcome to HT ISO Ripper Setup
WelcomeLabel2=Professional CD/DVD Disc Imaging and Backup Software%n%nDeveloped by HTREASURE LTD%n%nSetup checks this computer, installs anything Windows is missing (only with your permission) and then installs HT ISO Ripper {#AppVersion}.%n%nClose other applications before continuing.
FinishedHeadingLabel=Installation complete
FinishedLabelNoIcons=HT ISO Ripper was installed successfully.
FinishedLabel=HT ISO Ripper was installed successfully.

[Tasks]
Name: "startmenuicon"; Description: "Create a &Start Menu shortcut"; GroupDescription: "Shortcuts:"
Name: "desktopicon"; Description: "Create a &desktop shortcut"; GroupDescription: "Shortcuts:"; Flags: unchecked

[Files]
; Exactly one application build is installed, chosen at run time (see SelectBuild in decision.iss).
; Files from different builds are never mixed: each entry copies one complete build folder.
Source: "{#AppBin}x64\Release\net48\*";  Excludes: "*.pdb"; DestDir: "{app}"; Flags: ignoreversion; Check: IsSelectedBuild('net48', 'x64')
Source: "{#AppBin}x86\Release\net48\*";  Excludes: "*.pdb"; DestDir: "{app}"; Flags: ignoreversion; Check: IsSelectedBuild('net48', 'x86')
Source: "{#AppBin}x64\Release\net461\*"; Excludes: "*.pdb"; DestDir: "{app}"; Flags: ignoreversion; Check: IsSelectedBuild('net461', 'x64')
Source: "{#AppBin}x86\Release\net461\*"; Excludes: "*.pdb"; DestDir: "{app}"; Flags: ignoreversion; Check: IsSelectedBuild('net461', 'x86')
Source: "..\LICENSE"; DestDir: "{app}"; DestName: "LICENSE.txt"; Flags: ignoreversion
Source: "PRIVACY.txt"; DestDir: "{app}"; Flags: ignoreversion
; Documents opened from the Help menu.
Source: "..\docs\USER_MANUAL.md"; DestDir: "{app}\Documentation"; DestName: "USER-GUIDE.md"; Flags: ignoreversion
Source: "..\docs\release\TROUBLESHOOTING.md"; DestDir: "{app}\Documentation"; Flags: ignoreversion
Source: "..\docs\release\RELEASE-NOTES.md"; DestDir: "{app}\Documentation"; Flags: ignoreversion
Source: "..\docs\release\INSTALLATION.md"; DestDir: "{app}\Documentation"; Flags: ignoreversion
Source: "..\docs\release\THIRD-PARTY-LICENSES.md"; DestDir: "{app}\Documentation"; Flags: ignoreversion
#if Offline
; Bundled Microsoft redistributables: extracted to {tmp} only when needed, verified again before use.
Source: "redist\{#NDP48_FILE}";  Flags: dontcopy nocompression
Source: "redist\{#NDP461_FILE}"; Flags: dontcopy nocompression
#endif

[Icons]
Name: "{autoprograms}\HT ISO Ripper"; Filename: "{app}\HTISORipper.exe"; Comment: "{#AppDescription}"; Tasks: startmenuicon
Name: "{autodesktop}\HT ISO Ripper"; Filename: "{app}\HTISORipper.exe"; Comment: "{#AppDescription}"; Tasks: desktopicon

[Run]
Filename: "{app}\HTISORipper.exe"; Description: "Launch HT ISO Ripper"; Flags: postinstall nowait skipifsilent

[UninstallDelete]
; Resume data from an interrupted installation (never user data).
Type: filesandordirs; Name: "{commonappdata}\HTREASURE LTD\HT ISO Ripper Setup"

#include "decision.iss"

[Code]
const
  SETUP_STATE_KEY = 'SOFTWARE\HTREASURE LTD\HT ISO Ripper\Setup';
  RUNONCE_KEY = 'Software\Microsoft\Windows\CurrentVersion\RunOnce';
  RUNONCE_VALUE = 'HTISORipperSetupResume';
  UNINSTALL_KEY = 'Software\Microsoft\Windows\CurrentVersion\Uninstall\{7C1F3B2E-5A4D-4E8B-9C21-3F6A8D0B4E17}_is1';
  OFFLINE_BUILD = {#Offline};
  MAX_DOWNLOAD_ATTEMPTS = 3;

var
  Sys: TSysInfo;
  SelBuild, SelReason, BlockReason: String;
  Supported, NeedPrereq, Resuming: Boolean;
  PrereqName, PrereqUrl, PrereqFile, PrereqSha, PrereqArgs, PrereqInfo: String;
  PrereqSize: Int64;
  PrereqLocalPath, InstalledVersion: String;
  CompatPage, PrereqPage: TWizardPage;
  DownloadPage: TDownloadWizardPage;
  ConsentCheck: TNewCheckBox;
  NotesMemo: TNewMemo;

// ---------------------------------------------------------------- helpers

function IsSelectedBuild(const Build, Arch: String): Boolean;
begin
  Result := Supported and (SelBuild = Build) and (AppArchitecture(Sys) = Arch);
end;

function FormatMB(Bytes: Int64): String;
begin
  Result := IntToStr(Bytes div (1024 * 1024)) + ' MB';
end;

procedure LoadPrerequisite(const Id: String);
begin
  if Id = 'NDP461' then begin
    PrereqName := '{#NDP461_NAME}'; PrereqUrl := '{#NDP461_URL}'; PrereqFile := '{#NDP461_FILE}';
    PrereqSha := '{#NDP461_SHA}'; PrereqArgs := '{#NDP461_ARGS}'; PrereqSize := {#NDP461_SIZE}; PrereqInfo := '{#NDP461_INFO}';
  end else begin
    PrereqName := '{#NDP48_NAME}'; PrereqUrl := '{#NDP48_URL}'; PrereqFile := '{#NDP48_FILE}';
    PrereqSha := '{#NDP48_SHA}'; PrereqArgs := '{#NDP48_ARGS}'; PrereqSize := {#NDP48_SIZE}; PrereqInfo := '{#NDP48_INFO}';
  end;
end;

function FormatSpace(MB: Int64): String;
begin
  if MB < 0 then Result := 'unknown'
  else if MB < 1024 then Result := IntToStr(MB) + ' MB'
  else Result := IntToStr(MB div 1024) + '.' + IntToStr(((MB mod 1024) * 10) div 1024) + ' GB';
end;

function ReadInstalledVersion(): String;
begin
  Result := '';
  if IsWin64 and RegQueryStringValue(HKLM64, UNINSTALL_KEY, 'DisplayVersion', Result) then exit;
  if RegQueryStringValue(HKLM32, UNINSTALL_KEY, 'DisplayVersion', Result) then exit;
  Result := '';
end;

function FreeSpaceMB(const Path: String): Int64;
var
  Free, Total: Int64;
begin
  if GetSpaceOnDisk64(Path, Free, Total) then Result := Free div (1024 * 1024) else Result := -1;
end;

function MakeLabel(Page: TWizardPage; const Text: String; X, Y, W: Integer; Bold: Boolean): TNewStaticText;
begin
  Result := TNewStaticText.Create(Page);
  Result.Parent := Page.Surface;
  Result.Left := X;
  Result.Top := Y;
  Result.AutoSize := False;
  Result.Width := W;
  Result.Height := ScaleY(16);
  Result.Caption := Text;
  if Bold then Result.Font.Style := [fsBold];
end;

function AddRow(Page: TWizardPage; Y: Integer; const Caption, Value: String): Integer;
begin
  MakeLabel(Page, Caption, 0, Y, ScaleX(150), True);
  MakeLabel(Page, Value, ScaleX(155), Y, Page.SurfaceWidth - ScaleX(155), False);
  Result := Y + ScaleY(20);
end;

// ---------------------------------------------------------------- resume after restart

procedure ClearResumeState();
begin
  RegDeleteKeyIncludingSubkeys(HKLM, SETUP_STATE_KEY);
  RegDeleteValue(HKCU, RUNONCE_KEY, RUNONCE_VALUE);
end;

// Saves the user's choices and registers Setup to continue at the next sign-in. The registry key is
// administrator-only (HKLM); the copy of Setup is checked against a stored SHA-256 before state is trusted.
procedure SaveResumeState();
var
  Dir, CopyPath: String;
begin
  Dir := ExpandConstant('{commonappdata}\HTREASURE LTD\HT ISO Ripper Setup');
  ForceDirectories(Dir);
  CopyPath := Dir + '\' + ExtractFileName(ExpandConstant('{srcexe}'));
  if CompareText(CopyPath, ExpandConstant('{srcexe}')) <> 0 then
    FileCopy(ExpandConstant('{srcexe}'), CopyPath, False);
  RegWriteStringValue(HKLM, SETUP_STATE_KEY, 'Stage', 'PrerequisiteInstalled');
  RegWriteStringValue(HKLM, SETUP_STATE_KEY, 'AppDir', WizardDirValue);
  RegWriteStringValue(HKLM, SETUP_STATE_KEY, 'Tasks', WizardSelectedTasks(False));
  RegWriteStringValue(HKLM, SETUP_STATE_KEY, 'SetupSha256', GetSHA256OfFile(CopyPath));
  RegWriteStringValue(HKLM, SETUP_STATE_KEY, 'SetupPath', CopyPath);
  RegWriteStringValue(HKLM, SETUP_STATE_KEY, 'Version', '{#AppVersion}');
  // RunOnce of the installing user: Windows starts Setup once after sign-in; Setup then asks for
  // administrator permission again (UAC), which is the user's consent to continue.
  RegWriteStringValue(HKCU, RUNONCE_KEY, RUNONCE_VALUE, '"' + CopyPath + '" /HTRESUME');
  Log('Resume state saved; Setup will continue after restart from ' + CopyPath);
end;

// True when this run was started by the RunOnce entry and the saved state is intact.
function ValidResumeState(): Boolean;
var
  Stage, Sha, Path, Ver: String;
begin
  Result := False;
  if not RegQueryStringValue(HKLM, SETUP_STATE_KEY, 'Stage', Stage) then exit;
  RegQueryStringValue(HKLM, SETUP_STATE_KEY, 'SetupSha256', Sha);
  RegQueryStringValue(HKLM, SETUP_STATE_KEY, 'SetupPath', Path);
  RegQueryStringValue(HKLM, SETUP_STATE_KEY, 'Version', Ver);
  Result := (Stage = 'PrerequisiteInstalled') and (Ver = '{#AppVersion}')
    and (CompareText(Path, ExpandConstant('{srcexe}')) = 0)
    and (CompareText(Sha, GetSHA256OfFile(ExpandConstant('{srcexe}'))) = 0);
  if not Result then Log('Saved resume state rejected (does not match this Setup file).');
end;

// ---------------------------------------------------------------- startup

function InitializeSetup(): Boolean;
var
  ReqMB, FreeMB: Int64;
begin
  Result := True;
  DetectSystem(Sys);
#ifdef Preview
  // Preview builds only: /simmajor= /simminor= /simbuild= /simx64= /simrelease= to view the pages for other systems.
  if ExpandConstant('{param:simmajor|}') <> '' then begin
    Sys.Major := StrToIntDef(ExpandConstant('{param:simmajor|}'), Sys.Major);
    Sys.Minor := StrToIntDef(ExpandConstant('{param:simminor|0}'), 0);
    Sys.Build := StrToIntDef(ExpandConstant('{param:simbuild|0}'), 0);
    Sys.Is64Bit := ExpandConstant('{param:simx64|1}') = '1';
    Sys.NetRelease := StrToIntDef(ExpandConstant('{param:simrelease|0}'), 0);
  end;
#endif
  SelBuild := SelectBuild(Sys, SelReason);
  Supported := SelBuild <> '';
  BlockReason := '';
  if not Supported then BlockReason := SelReason;

  NeedPrereq := Supported and not NetSatisfied(Sys, SelBuild);
  if Supported then LoadPrerequisite(PrerequisiteId(SelBuild));

  InstalledVersion := ReadInstalledVersion();
  if Supported and (InstalledVersion <> '') and (CompareVersionStrings(InstalledVersion, '{#AppVersion}') > 0) then begin
    Supported := False;
    BlockReason := 'A newer version of HT ISO Ripper (' + InstalledVersion + ') is already installed. Setup will not downgrade it.';
  end;

  if Supported and NeedPrereq then begin
    ReqMB := PrerequisiteDiskMB(Sys);
    FreeMB := FreeSpaceMB(ExpandConstant('{sd}\'));
    if (FreeMB >= 0) and (FreeMB < ReqMB) then begin
      Supported := False;
      BlockReason := 'The system drive needs at least ' + IntToStr(ReqMB) + ' MB free to install ' + PrereqName
        + ' (' + IntToStr(FreeMB) + ' MB available). Free up space and run Setup again.';
    end;
  end;

  Resuming := (Pos('/HTRESUME', Uppercase(GetCmdTail)) > 0) and ValidResumeState();
  if (Pos('/HTRESUME', Uppercase(GetCmdTail)) > 0) and not Resuming then ClearResumeState();

  Log(Format('System: %s; %s; .NET %s; build %s (%s); prerequisite needed: %d; restart pending: %d; installed: %s', [
    OsDisplayName(Sys), ArchDisplayName(Sys), NetReleaseDisplayName(Sys.NetRelease), SelBuild, SelReason,
    Ord(NeedPrereq), Ord(Sys.RestartPending), InstalledVersion]));

  if WizardSilent and not Supported then begin
    Log('Silent install stopped: ' + BlockReason);
    Result := False;
  end;
end;

procedure ConsentClick(Sender: TObject);
begin
  WizardForm.NextButton.Enabled := ConsentCheck.Checked;
end;

procedure InitializeWizard();
var
  Y: Integer;
  Status, Notes, Source, ExistingText: String;
  T: TNewStaticText;
begin
  // ---- System Compatibility Check
  CompatPage := CreateCustomPage(wpInfoBefore, 'System Compatibility Check',
    'Setup has checked this computer and selected the correct version of HT ISO Ripper.');
  Y := 0;
  Y := AddRow(CompatPage, Y, 'Operating system', OsDisplayName(Sys));
  Y := AddRow(CompatPage, Y, 'Architecture', ArchDisplayName(Sys));
  if Supported or (SelBuild <> '') then begin
    if SelBuild = BUILD_NET461 then
      Y := AddRow(CompatPage, Y, 'Application build', 'HT ISO Ripper for .NET Framework 4.6.1 (' + AppArchitecture(Sys) + ', Windows 8)')
    else
      Y := AddRow(CompatPage, Y, 'Application build', 'HT ISO Ripper for .NET Framework 4.8 (' + AppArchitecture(Sys) + ')');
    Y := AddRow(CompatPage, Y, 'Required .NET', RequiredNetName(SelBuild));
  end else
    Y := AddRow(CompatPage, Y, 'Application build', 'None available for this system');
  Y := AddRow(CompatPage, Y, 'Installed .NET', NetReleaseDisplayName(Sys.NetRelease));
  Y := AddRow(CompatPage, Y, 'Available storage', 'System drive ' + FormatSpace(FreeSpaceMB(ExpandConstant('{sd}\'))) + ' free');
  if InstalledVersion <> '' then ExistingText := 'Version ' + InstalledVersion + ' (will be upgraded)' else ExistingText := 'None';
  Y := AddRow(CompatPage, Y, 'Existing installation', ExistingText);
  if not Supported then Status := 'Not compatible'
  else if Sys.RestartPending then Status := 'Compatible - a Windows restart is pending'
  else Status := 'Compatible';
  MakeLabel(CompatPage, 'Status', 0, Y, ScaleX(150), True);
  T := MakeLabel(CompatPage, Status, ScaleX(155), Y, CompatPage.SurfaceWidth - ScaleX(155), True);
  if Supported then T.Font.Color := $45A728 else T.Font.Color := $4535DC;   // BGR: #28A745 green / #DC3545 red
  Y := Y + ScaleY(24);

  Notes := '';
  if BlockReason <> '' then Notes := Notes + BlockReason + #13#10#13#10;
  if Sys.RestartPending then Notes := Notes + 'Windows is waiting for a restart (for example after updates). If installing a prerequisite fails, restart Windows and run Setup again.' + #13#10#13#10;
  if SupportNotice(Sys) <> '' then Notes := Notes + 'Security notice: ' + SupportNotice(Sys) + #13#10#13#10;
  if RuntimeNotice(SelBuild) <> '' then Notes := Notes + RuntimeNotice(SelBuild) + #13#10#13#10;
  if Notes = '' then Notes := SelReason;
  NotesMemo := TNewMemo.Create(CompatPage);
  NotesMemo.Parent := CompatPage.Surface;
  NotesMemo.Top := Y;
  NotesMemo.Width := CompatPage.SurfaceWidth;
  NotesMemo.Height := CompatPage.SurfaceHeight - Y;
  NotesMemo.ReadOnly := True;
  NotesMemo.ScrollBars := ssVertical;
  NotesMemo.Text := Trim(Notes);

  // ---- Prerequisite Check
  PrereqPage := CreateCustomPage(CompatPage.ID, 'Prerequisite Check',
    'Only components this computer is actually missing will be installed.');
  Y := 0;
  MakeLabel(PrereqPage, 'Component', 0, Y, ScaleX(190), True);
  MakeLabel(PrereqPage, 'Status', ScaleX(195), Y, ScaleX(150), True);
  MakeLabel(PrereqPage, 'Action', ScaleX(350), Y, PrereqPage.SurfaceWidth - ScaleX(350), True);
  Y := Y + ScaleY(20);
  if Supported then begin
    MakeLabel(PrereqPage, PrereqName, 0, Y, ScaleX(190), False);
    if NeedPrereq then begin
      MakeLabel(PrereqPage, 'Missing', ScaleX(195), Y, ScaleX(150), False);
      MakeLabel(PrereqPage, 'Install', ScaleX(350), Y, PrereqPage.SurfaceWidth - ScaleX(350), False);
    end else begin
      MakeLabel(PrereqPage, 'Installed (' + NetReleaseDisplayName(Sys.NetRelease) + ')', ScaleX(195), Y, ScaleX(150), False);
      MakeLabel(PrereqPage, 'Skip', ScaleX(350), Y, PrereqPage.SurfaceWidth - ScaleX(350), False);
    end;
    Y := Y + ScaleY(20);
  end;
  // HT ISO Ripper is 100% managed code: verified at build time by scripts\build-release.ps1.
  MakeLabel(PrereqPage, 'Visual C++ Runtime', 0, Y, ScaleX(190), False);
  MakeLabel(PrereqPage, 'Not required', ScaleX(195), Y, ScaleX(150), False);
  MakeLabel(PrereqPage, 'Skip', ScaleX(350), Y, PrereqPage.SurfaceWidth - ScaleX(350), False);
  Y := Y + ScaleY(20);
  MakeLabel(PrereqPage, 'Application libraries', 0, Y, ScaleX(190), False);
  MakeLabel(PrereqPage, 'Included', ScaleX(195), Y, ScaleX(150), False);
  MakeLabel(PrereqPage, 'Continue', ScaleX(350), Y, PrereqPage.SurfaceWidth - ScaleX(350), False);
  Y := Y + ScaleY(28);

  if Supported and NeedPrereq then begin
    if OFFLINE_BUILD = 1 then
      Source := 'It is included in this installer and will be checked against its SHA-256 fingerprint and Microsoft digital signature before it runs.'
    else
      Source := 'Setup will download it (' + FormatMB(PrereqSize) + ') directly from Microsoft (download.microsoft.com) over a secure HTTPS connection, '
        + 'check its SHA-256 fingerprint and Microsoft digital signature, and then run Microsoft''s installer.';
    T := MakeLabel(PrereqPage, PrereqName + ' (publisher: Microsoft Corporation) is required to run HT ISO Ripper. ' + Source
      + ' Windows may need to restart afterwards; Setup will then continue automatically after you sign in.',
      0, Y, PrereqPage.SurfaceWidth, False);
    T.WordWrap := True;
    T.Height := ScaleY(84);
    Y := Y + ScaleY(90);
    ConsentCheck := TNewCheckBox.Create(PrereqPage);
    ConsentCheck.Parent := PrereqPage.Surface;
    ConsentCheck.Top := Y;
    ConsentCheck.Width := PrereqPage.SurfaceWidth;
    ConsentCheck.Height := ScaleY(18);
    if OFFLINE_BUILD = 1 then
      ConsentCheck.Caption := 'I agree to install ' + PrereqName + ' (Microsoft licence terms apply)'
    else
      ConsentCheck.Caption := 'I agree to download and install ' + PrereqName + ' (Microsoft licence terms apply)';
    ConsentCheck.OnClick := @ConsentClick;
  end else if Supported then begin
    T := MakeLabel(PrereqPage, 'Everything HT ISO Ripper needs is already installed. Nothing will be downloaded.', 0, Y, PrereqPage.SurfaceWidth, False);
    T.WordWrap := True;
    T.Height := ScaleY(36);
  end;

  DownloadPage := CreateDownloadPage('Downloading Microsoft .NET Framework',
    'Downloading from download.microsoft.com and verifying the file. This may take several minutes.', nil);

  if Resuming then Log('Resuming installation after restart.');
end;

procedure RestoreResumeChoices();
var
  Dir, Tasks: String;
begin
  if RegQueryStringValue(HKLM, SETUP_STATE_KEY, 'AppDir', Dir) and (Dir <> '') then WizardForm.DirEdit.Text := Dir;
  if RegQueryStringValue(HKLM, SETUP_STATE_KEY, 'Tasks', Tasks) then WizardSelectTasks(Tasks);
end;

procedure CurPageChanged(CurPageID: Integer);
begin
  if CurPageID = wpWelcome then begin
    if Resuming then begin
      WizardForm.WelcomeLabel2.Caption := 'Windows has restarted after installing ' + PrereqName
        + '. Setup will now check the computer again and finish installing HT ISO Ripper with the options you chose.';
      RestoreResumeChoices();
    end;
  end else if CurPageID = CompatPage.ID then
    WizardForm.NextButton.Enabled := Supported
  else if CurPageID = PrereqPage.ID then begin
    if Supported and NeedPrereq then WizardForm.NextButton.Enabled := ConsentCheck.Checked
    else WizardForm.NextButton.Enabled := Supported;
  end;
end;

function UpdateReadyMemo(Space, NewLine, MemoUserInfoInfo, MemoDirInfo, MemoTypeInfo, MemoComponentsInfo,
  MemoGroupInfo, MemoTasksInfo: String): String;
begin
  Result := 'System:' + NewLine + Space + OsDisplayName(Sys) + ', ' + ArchDisplayName(Sys) + NewLine + NewLine;
  Result := Result + 'Application build:' + NewLine + Space + 'HT ISO Ripper {#AppVersion} for ' + RequiredNetName(SelBuild)
    + ' (' + AppArchitecture(Sys) + ')' + NewLine + NewLine;
  if NeedPrereq then begin
    Result := Result + 'Prerequisites to install first:' + NewLine + Space + PrereqName;
    if OFFLINE_BUILD = 1 then Result := Result + ' (included in this installer)'
    else Result := Result + ' (download ' + FormatMB(PrereqSize) + ' from download.microsoft.com)';
    Result := Result + NewLine + NewLine;
  end;
  Result := Result + MemoDirInfo + NewLine + NewLine;
  if MemoTasksInfo <> '' then Result := Result + MemoTasksInfo;
end;

// ---------------------------------------------------------------- acquire and verify the prerequisite

function VerifyPackage(const Path: String; var Problem: String): Boolean;
var
  Sig: Integer;
  Detail: String;
begin
  Result := False;
  if not HashMatches(Path, PrereqSha) then begin
    Problem := 'The file''s SHA-256 fingerprint does not match the official ' + PrereqName + ' package. It may be damaged or tampered with, so it was not run.';
    DeleteFile(Path);
    Log('SHA-256 mismatch for ' + Path);
    exit;
  end;
  Sig := CheckAuthenticode(Path, 'Microsoft Corporation', Detail);
  Log('Signature check: ' + IntToStr(Sig) + ' - ' + Detail);
  if Sig = 1 then begin
    Problem := 'The package does not carry a valid Microsoft digital signature. It was not run. (' + Detail + ')';
    DeleteFile(Path);
    exit;
  end;
  // Sig 2 (old root certificates) or 3 (check unavailable): the pinned SHA-256 already proves this is the
  // exact Microsoft file, so installation continues; the condition is logged.
  Result := True;
end;

function AcquirePrerequisite(): Boolean;
var
  Attempt: Integer;
  Problem, Path: String;
  Done: Boolean;
begin
  Result := False;
  Path := ExpandConstant('{tmp}\') + PrereqFile;
  if OFFLINE_BUILD = 1 then begin
    try
      ExtractTemporaryFile(PrereqFile);
    except
      MsgBox('The bundled ' + PrereqName + ' package could not be extracted: ' + GetExceptionMessage, mbCriticalError, MB_OK);
      exit;
    end;
  end else begin
    Done := False;
    Attempt := 0;
    while not Done do begin
      Attempt := Attempt + 1;
      DownloadPage.Clear;
      // Inno Setup verifies the SHA-256 itself and deletes the file if it does not match.
      DownloadPage.Add(PrereqUrl, PrereqFile, PrereqSha);
      DownloadPage.Show;
      try
        try
          DownloadPage.Download;
          Done := True;
        except
          Problem := GetExceptionMessage;
          Log('Download attempt ' + IntToStr(Attempt) + ' failed: ' + Problem);
          if DownloadPage.AbortedByUser then exit;
          if (Attempt >= MAX_DOWNLOAD_ATTEMPTS) or (SuppressibleMsgBox('Downloading ' + PrereqName + ' failed:' + #13#10 + Problem
              + #13#10#13#10 + 'Check your internet connection and try again.', mbError, MB_RETRYCANCEL, IDCANCEL) <> IDRETRY) then begin
            SuppressibleMsgBox('Setup could not download ' + PrereqName + ' securely.' + #13#10#13#10
              + 'Older versions of Windows sometimes cannot connect to Microsoft securely. Setup never weakens security to work around this.' + #13#10#13#10
              + 'Use HT-ISO-Ripper-Offline-Setup-{#AppVersion}.exe instead (no internet needed), or install ' + PrereqName
              + ' from Microsoft (' + PrereqInfo + ') and run this Setup again.', mbInformation, MB_OK, IDOK);
            exit;
          end;
        end;
      finally
        DownloadPage.Hide;
      end;
    end;
  end;

  if not VerifyPackage(Path, Problem) then begin
    SuppressibleMsgBox(Problem, mbCriticalError, MB_OK, IDOK);
    exit;
  end;
  PrereqLocalPath := Path;
  Result := True;
end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;
  if (CurPageID = wpReady) and Supported and NeedPrereq and (PrereqLocalPath = '') then
    Result := AcquirePrerequisite();
end;

// ---------------------------------------------------------------- install the prerequisite

function PrepareToInstall(var NeedsRestart: Boolean): String;
var
  Code: Integer;
  Progress: TOutputProgressWizardPage;
begin
  Result := '';
  if WizardSilent and NeedPrereq and (PrereqLocalPath = '') then begin
    // Silent mode: the user gave no interactive consent, so prerequisites are never downloaded silently.
    Result := PrereqName + ' is required. Install it first, or run Setup interactively.';
    exit;
  end;
  if PrereqLocalPath = '' then exit;

  Progress := CreateOutputProgressPage('Installing ' + PrereqName, 'Microsoft''s installer is running. This can take several minutes.');
  Progress.SetText('Installing ' + PrereqName + '...', 'Please wait and do not turn off the computer.');
  Progress.ProgressBar.Style := npbstMarquee;
  Progress.Show;
  try
    if not Exec(PrereqLocalPath, PrereqArgs, '', SW_SHOW, ewWaitUntilTerminated, Code) then begin
      Result := 'Could not start the ' + PrereqName + ' installer: ' + SysErrorMessage(Code);
      exit;
    end;
  finally
    Progress.Hide;
  end;
  Log(PrereqName + ' installer exit code: ' + IntToStr(Code));

  case Code of
    0: begin
         Sys.NetRelease := ReadNetRelease();
         if not NetSatisfied(Sys, SelBuild) then
           Result := PrereqName + ' reported success, but Setup cannot detect it. Restart Windows and run Setup again.';
       end;
    1641, 3010: begin
         SaveResumeState();
         NeedsRestart := True;
         Result := PrereqName + ' was installed. Windows must restart before HT ISO Ripper can be installed.' + #13#10#13#10
           + 'After you sign in again, Setup will ask for permission and then continue automatically.';
       end;
    1602: Result := 'The ' + PrereqName + ' installation was cancelled. HT ISO Ripper needs it and was not installed.';
    5100: Result := 'Microsoft''s installer reports that this computer does not meet the requirements for ' + PrereqName + '.';
  else
    Result := PrereqName + ' could not be installed (Microsoft installer exit code ' + IntToStr(Code) + '). '
      + 'Microsoft''s log files are in ' + ExpandConstant('{%TEMP}') + ' (names starting with dd_ or Microsoft .NET).';
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then begin
    ClearResumeState();
    if PrereqLocalPath <> '' then DeleteFile(PrereqLocalPath);
  end;
end;

procedure RegisterPreviousData(PreviousDataKey: Integer);
begin
  SetPreviousData(PreviousDataKey, 'AppDir', WizardDirValue);
end;

// ---------------------------------------------------------------- uninstall

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
begin
  if (CurUninstallStep = usPostUninstall) and not UninstallSilent then begin
    // Shared Microsoft runtimes are never removed. Disc images are never touched.
    if MsgBox('Also remove your HT ISO Ripper settings and log files?' + #13#10#13#10
        + 'Your disc images are never removed.', mbConfirmation, MB_YESNO or MB_DEFBUTTON2) = IDYES then begin
      DelTree(ExpandConstant('{userappdata}\HTREASURE LTD\HT ISO Ripper'), True, True, True);
      DelTree(ExpandConstant('{localappdata}\HTREASURE LTD\HT ISO Ripper'), True, True, True);
    end;
  end;
end;
