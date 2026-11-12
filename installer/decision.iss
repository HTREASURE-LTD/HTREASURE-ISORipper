// HT ISO Ripper Setup - system detection and installation decisions.
//
// Pure decision functions take a TSysInfo record, so the same code runs in the real installer
// (with detected values) and in tests\installer\DecisionHarness.iss (with simulated values).
// Keep this file free of wizard/UI code.

[Code]
const
  NDP_KEY = 'SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full';
  // Minimum "Release" DWORD values from Microsoft's ".NET Framework & Windows OS versions" page.
  // A newer 4.x runtime always has a higher value, so ">=" accepts it (no downgrade, no reinstall).
  RELEASE_NET48  = 528040;  // .NET Framework 4.8 (lowest value of all 4.8 variants)
  RELEASE_NET461 = 394254;  // .NET Framework 4.6.1 (lowest value of all 4.6.1 variants)

  BUILD_NET48  = 'net48';
  BUILD_NET461 = 'net461';

type
  TSysInfo = record
    Major, Minor, Build, ServicePack: Integer;
    IsServer: Boolean;
    Is64Bit: Boolean;        // 64-bit Windows (x64 or ARM64)
    IsArm64: Boolean;
    NetRelease: Cardinal;    // 0 = no .NET Framework 4.5+ found
    RestartPending: Boolean;
  end;

// ---------------------------------------------------------------- naming

function OsDisplayName(const S: TSysInfo): String;
begin
  if (S.Major = 6) and (S.Minor = 1) then begin
    if S.IsServer then Result := 'Windows Server 2008 R2' else Result := 'Windows 7';
  end else if (S.Major = 6) and (S.Minor = 2) then begin
    if S.IsServer then Result := 'Windows Server 2012' else Result := 'Windows 8';
  end else if (S.Major = 6) and (S.Minor = 3) then begin
    if S.IsServer then Result := 'Windows Server 2012 R2' else Result := 'Windows 8.1';
  end else if S.Major = 10 then begin
    if S.IsServer then Result := 'Windows Server'
    else if S.Build >= 22000 then Result := 'Windows 11'
    else Result := 'Windows 10';
  end else if (S.Major = 6) and (S.Minor = 0) then
    Result := 'Windows Vista'
  else
    Result := 'Windows ' + IntToStr(S.Major) + '.' + IntToStr(S.Minor);

  if S.ServicePack > 0 then Result := Result + ' SP' + IntToStr(S.ServicePack);
  Result := Result + ' (build ' + IntToStr(S.Build) + ')';
end;

function ArchDisplayName(const S: TSysInfo): String;
begin
  if S.IsArm64 then Result := 'ARM64 (64-bit, x64 emulation)'
  else if S.Is64Bit then Result := 'x64 (64-bit)'
  else Result := 'x86 (32-bit)';
end;

function NetReleaseDisplayName(Release: Cardinal): String;
begin
  if Release = 0 then Result := 'Not installed (no .NET Framework 4.5 or later)'
  else if Release >= 533320 then Result := '4.8.1'
  else if Release >= 528040 then Result := '4.8'
  else if Release >= 461808 then Result := '4.7.2'
  else if Release >= 461308 then Result := '4.7.1'
  else if Release >= 460798 then Result := '4.7'
  else if Release >= 394802 then Result := '4.6.2'
  else if Release >= 394254 then Result := '4.6.1'
  else if Release >= 393295 then Result := '4.6'
  else if Release >= 379893 then Result := '4.5.2'
  else if Release >= 378675 then Result := '4.5.1'
  else Result := '4.5';
  if Release <> 0 then Result := Result + ' (release ' + IntToStr(Release) + ')';
end;

// ---------------------------------------------------------------- decisions

// Returns 'net48', 'net461' or '' (unsupported, Reason explains why).
function SelectBuild(const S: TSysInfo; var Reason: String): String;
begin
  Result := '';
  Reason := '';
  if (S.Major < 6) or ((S.Major = 6) and (S.Minor < 1)) then begin
    Reason := 'HT ISO Ripper requires Windows 7 SP1 or later.';
    exit;
  end;
  if (S.Major = 6) and (S.Minor = 1) then begin
    if S.ServicePack < 1 then begin
      Reason := 'Windows 7 must have Service Pack 1 installed. Install SP1 from Windows Update, then run Setup again.';
      exit;
    end;
    Result := BUILD_NET48;
    Reason := 'Windows 7 SP1 supports .NET Framework 4.8.';
    exit;
  end;
  if (S.Major = 6) and (S.Minor = 2) then begin
    if S.IsServer then begin
      Result := BUILD_NET48;
      Reason := 'Windows Server 2012 supports .NET Framework 4.8.';
    end else begin
      // Windows 8.0: .NET Framework 4.8 (and 4.6.2+) cannot be installed. Never offer it.
      Result := BUILD_NET461;
      Reason := 'Windows 8 cannot run .NET Framework 4.8; the .NET Framework 4.6.1 compatibility build is used.';
    end;
    exit;
  end;
  // Windows 8.1, 10, 11 and later.
  Result := BUILD_NET48;
  Reason := OsDisplayName(S) + ' supports .NET Framework 4.8.';
end;

function RequiredRelease(const Build: String): Cardinal;
begin
  if Build = BUILD_NET461 then Result := RELEASE_NET461 else Result := RELEASE_NET48;
end;

function RequiredNetName(const Build: String): String;
begin
  if Build = BUILD_NET461 then Result := '.NET Framework 4.6.1 or later' else Result := '.NET Framework 4.8 or later';
end;

// Manifest section of the prerequisite that provides the runtime for a build.
function PrerequisiteId(const Build: String): String;
begin
  if Build = BUILD_NET461 then Result := 'NDP461' else Result := 'NDP48';
end;

function NetSatisfied(const S: TSysInfo; const Build: String): Boolean;
begin
  Result := (Build <> '') and (S.NetRelease >= RequiredRelease(Build));
end;

// Disk space Microsoft lists for installing the .NET Framework redistributable (system drive).
function PrerequisiteDiskMB(const S: TSysInfo): Integer;
begin
  if S.Is64Bit then Result := 2048 else Result := 850;
end;

// Architecture of the application files to install.
function AppArchitecture(const S: TSysInfo): String;
begin
  if S.Is64Bit then Result := 'x64' else Result := 'x86';
end;

// Out-of-support notice for Windows versions Microsoft no longer services (as of 2026).
function SupportNotice(const S: TSysInfo): String;
begin
  Result := '';
  if S.IsServer then exit;
  if (S.Major = 6) and (S.Minor <= 3) then
    Result := OsDisplayName(S) + ' no longer receives security updates from Microsoft. HT ISO Ripper will work, '
      + 'but this computer is at risk. Upgrade to a supported version of Windows when possible.'
  else if (S.Major = 10) and (S.Build < 22000) then
    Result := 'Windows 10 reached end of support on 14 October 2025 (unless enrolled in Extended Security Updates). '
      + 'HT ISO Ripper will work, but consider upgrading to Windows 11.';
end;

// .NET 4.6.1 itself is out of Microsoft support (since 26 April 2022) but is the newest runtime Windows 8 can run.
function RuntimeNotice(const Build: String): String;
begin
  Result := '';
  if Build = BUILD_NET461 then
    Result := '.NET Framework 4.6.1 is no longer supported by Microsoft, but it is the newest version Windows 8 can run. '
      + 'Upgrading to Windows 8.1 (free) or later allows the fully supported .NET Framework 4.8.';
end;

// Compares dotted versions "a.b.c". Returns <0, 0, >0.
function CompareVersionStrings(const A, B: String): Integer;
var
  X, Y: String;
  PA, PB, NA, NB: Integer;
begin
  X := A; Y := B; Result := 0;
  while ((X <> '') or (Y <> '')) and (Result = 0) do begin
    PA := Pos('.', X); PB := Pos('.', Y);
    if PA = 0 then begin NA := StrToIntDef(X, 0); X := ''; end
    else begin NA := StrToIntDef(Copy(X, 1, PA - 1), 0); X := Copy(X, PA + 1, Length(X)); end;
    if PB = 0 then begin NB := StrToIntDef(Y, 0); Y := ''; end
    else begin NB := StrToIntDef(Copy(Y, 1, PB - 1), 0); Y := Copy(Y, PB + 1, Length(Y)); end;
    Result := NA - NB;
  end;
end;

// ---------------------------------------------------------------- detection (real system)

function ReadNetRelease(): Cardinal;
var
  Value: Cardinal;
begin
  Result := 0;
  // The .NET Framework key is present in the 64-bit registry view on 64-bit Windows; Setup itself is 32-bit.
  if IsWin64 then begin
    if RegQueryDWordValue(HKLM64, NDP_KEY, 'Release', Value) then Result := Value;
  end;
  if (Result = 0) and RegQueryDWordValue(HKLM32, NDP_KEY, 'Release', Value) then Result := Value;
end;

function DetectRestartPending(): Boolean;
begin
  Result :=
    RegKeyExists(HKLM, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Component Based Servicing\RebootPending') or
    RegKeyExists(HKLM, 'SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update\RebootRequired');
  if IsWin64 and not Result then
    Result :=
      RegKeyExists(HKLM64, 'SOFTWARE\Microsoft\Windows\CurrentVersion\Component Based Servicing\RebootPending') or
      RegKeyExists(HKLM64, 'SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update\RebootRequired');
end;

procedure DetectSystem(var S: TSysInfo);
var
  V: TWindowsVersion;
begin
  // Setup.exe declares compatibility with Windows 7-11 in its manifest, so GetVersionEx reports the real version.
  GetWindowsVersionEx(V);
  S.Major := V.Major;
  S.Minor := V.Minor;
  S.Build := V.Build;
  S.ServicePack := V.ServicePackMajor;
  S.IsServer := V.ProductType <> VER_NT_WORKSTATION;
  S.Is64Bit := IsX64Compatible;
  S.IsArm64 := IsARM64;
  S.NetRelease := ReadNetRelease();
  S.RestartPending := DetectRestartPending();
end;

// ---------------------------------------------------------------- package verification

// Authenticode check of a downloaded/bundled Microsoft package using Windows' own Get-AuthenticodeSignature
// (PowerShell 2.0+ ships with Windows 7 SP1 and later).
// Returns: 0 = valid signature from ExpectedOrg
//          1 = not signed, tampered or wrong publisher  -> must not run
//          2 = signed but the certificate chain is not trusted on this PC (e.g. old root store) -> warning
//          3 = the check could not be performed (PowerShell unavailable or blocked) -> warning
// The pinned SHA-256 is always verified first and is the mandatory authenticity gate.
function CheckAuthenticode(const FileName, ExpectedOrg: String; var Detail: String): Integer;
var
  OutFile, PsFile, PsOut, Cmd, Status, Subject: String;
  Content: AnsiString;
  Code, P: Integer;
begin
  Result := 3;
  Detail := 'Signature check could not be run.';
  OutFile := ExpandConstant('{tmp}\htsig.txt');
  DeleteFile(OutFile);
  // Escape single quotes for PowerShell single-quoted string literals.
  PsFile := FileName;
  StringChangeEx(PsFile, '''', '''''', True);
  PsOut := OutFile;
  StringChangeEx(PsOut, '''', '''''', True);
  Cmd := '-NoProfile -NonInteractive -ExecutionPolicy Bypass -Command "& { $s = Get-AuthenticodeSignature -LiteralPath '''
    + PsFile + '''; $sub = ''''; if ($s.SignerCertificate) { $sub = $s.SignerCertificate.Subject }; '
    + '[IO.File]::WriteAllText(''' + PsOut + ''', $s.Status.ToString() + ''|'' + $sub) }"';
  if not Exec(ExpandConstant('{sys}\WindowsPowerShell\v1.0\powershell.exe'), Cmd, '', SW_HIDE, ewWaitUntilTerminated, Code) then exit;
  if not LoadStringFromFile(OutFile, Content) then exit;
  DeleteFile(OutFile);

  P := Pos('|', String(Content));
  if P = 0 then exit;
  Status := Copy(String(Content), 1, P - 1);
  Subject := Copy(String(Content), P + 1, Length(String(Content)));
  Detail := 'Authenticode status: ' + Status + '; signer: ' + Subject;

  if (CompareText(Status, 'Valid') = 0) then begin
    if Pos('O=' + ExpectedOrg, Subject) > 0 then Result := 0 else Result := 1;
  end else if (CompareText(Status, 'NotSigned') = 0) or (CompareText(Status, 'HashMismatch') = 0)
       or (CompareText(Status, 'NotSupportedFileFormat') = 0) or (CompareText(Status, 'Incompatible') = 0) then
    Result := 1
  else if Pos('O=' + ExpectedOrg, Subject) > 0 then
    Result := 2   // UnknownError / NotTrusted with the expected publisher: chain cannot be built on this PC
  else
    Result := 1;
end;

// True if the file's SHA-256 equals the pinned value (case-insensitive).
function HashMatches(const FileName, ExpectedSha256: String): Boolean;
begin
  Result := False;
  if (ExpectedSha256 = '') or not FileExists(FileName) then exit;
  try
    Result := CompareText(GetSHA256OfFile(FileName), ExpectedSha256) = 0;
  except
    Result := False;
  end;
end;
