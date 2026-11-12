// Test harness for installer\decision.iss. It never installs anything.
//
//   DecisionHarness.exe /out=<results.ini> /major=6 /minor=2 /build=9200 /sp=0 /server=0 /x64=1 /arm64=0 /release=0
//   DecisionHarness.exe /out=<results.ini> /real=1                 (use this computer's real values)
//   DecisionHarness.exe /out=<results.ini> /verify=<file> /sha=<hex> /org=<publisher>
//
// Results are written as INI and Setup exits immediately (InitializeSetup returns False).
// Used by tests\HTISORipper.InstallerTests.

[Setup]
AppName=HT ISO Ripper Decision Harness
AppVersion=1.0.0
PrivilegesRequired=lowest
CreateAppDir=no
Uninstallable=no
OutputDir=bin
OutputBaseFilename=DecisionHarness
DisableWelcomePage=yes
ArchitecturesAllowed=x86compatible x64compatible
SetupLogging=no

#include "..\..\installer\decision.iss"

function B(Value: Boolean): String;
begin
  if Value then Result := '1' else Result := '0';
end;

function InitializeSetup(): Boolean;
var
  S: TSysInfo;
  OutFile, Build, Reason, Detail: String;
  Sig: Integer;
begin
  Result := False;   // never continue into a wizard
  OutFile := ExpandConstant('{param:out|}');
  if OutFile = '' then exit;

  if ExpandConstant('{param:verify|}') <> '' then begin
    SetIniString('Verify', 'HashMatches',
      B(HashMatches(ExpandConstant('{param:verify|}'), ExpandConstant('{param:sha|}'))), OutFile);
    Sig := CheckAuthenticode(ExpandConstant('{param:verify|}'), ExpandConstant('{param:org|Microsoft Corporation}'), Detail);
    SetIniString('Verify', 'Signature', IntToStr(Sig), OutFile);
    SetIniString('Verify', 'Detail', Detail, OutFile);
    exit;
  end;

  if ExpandConstant('{param:real|0}') = '1' then
    DetectSystem(S)
  else begin
    S.Major := StrToIntDef(ExpandConstant('{param:major|10}'), 10);
    S.Minor := StrToIntDef(ExpandConstant('{param:minor|0}'), 0);
    S.Build := StrToIntDef(ExpandConstant('{param:build|19045}'), 0);
    S.ServicePack := StrToIntDef(ExpandConstant('{param:sp|0}'), 0);
    S.IsServer := ExpandConstant('{param:server|0}') = '1';
    S.Is64Bit := ExpandConstant('{param:x64|1}') = '1';
    S.IsArm64 := ExpandConstant('{param:arm64|0}') = '1';
    S.NetRelease := StrToIntDef(ExpandConstant('{param:release|0}'), 0);
    S.RestartPending := ExpandConstant('{param:restart|0}') = '1';
  end;

  Build := SelectBuild(S, Reason);
  SetIniString('System', 'OsName', OsDisplayName(S), OutFile);
  SetIniString('System', 'Major', IntToStr(S.Major), OutFile);
  SetIniString('System', 'Build', IntToStr(S.Build), OutFile);
  SetIniString('System', 'Architecture', ArchDisplayName(S), OutFile);
  SetIniString('System', 'NetRelease', IntToStr(S.NetRelease), OutFile);
  SetIniString('System', 'NetName', NetReleaseDisplayName(S.NetRelease), OutFile);
  SetIniString('System', 'RestartPending', B(S.RestartPending), OutFile);
  SetIniString('Decision', 'Build', Build, OutFile);
  SetIniString('Decision', 'Supported', B(Build <> ''), OutFile);
  SetIniString('Decision', 'Reason', Reason, OutFile);
  SetIniString('Decision', 'AppArchitecture', AppArchitecture(S), OutFile);
  if Build <> '' then begin
    SetIniString('Decision', 'RequiredRelease', IntToStr(RequiredRelease(Build)), OutFile);
    SetIniString('Decision', 'Prerequisite', PrerequisiteId(Build), OutFile);
    SetIniString('Decision', 'NeedsPrerequisite', B(not NetSatisfied(S, Build)), OutFile);
    SetIniString('Decision', 'PrerequisiteDiskMB', IntToStr(PrerequisiteDiskMB(S)), OutFile);
  end;
  SetIniString('Decision', 'SupportNotice', SupportNotice(S), OutFile);
  SetIniString('Decision', 'RuntimeNotice', RuntimeNotice(Build), OutFile);
  SetIniString('Version', 'CompareInstalledNewer', IntToStr(CompareVersionStrings('1.2.0', '1.0.0')), OutFile);
  SetIniString('Version', 'CompareEqual', IntToStr(CompareVersionStrings('1.0', '1.0.0')), OutFile);
end;
