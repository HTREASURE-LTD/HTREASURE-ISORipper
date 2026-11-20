<#
.SYNOPSIS
    End-to-end installer test: installs HT ISO Ripper for real, checks it, starts it, upgrades it and
    uninstalls it, then writes a report. Runs in CI on a clean Windows runner and on tester PCs/VMs
    (Windows 7 SP1, 8, 8.1, 10, 11). Compatible with PowerShell 2.0 (stock Windows 7).

.DESCRIPTION
    Run from an elevated (Run as administrator) PowerShell:
        powershell -ExecutionPolicy Bypass -File test-installer-e2e.ps1 -Setup .\HT-ISO-Ripper-Setup-1.0.1.exe

    The silent install never downloads Microsoft .NET (no one can consent), so on a PC without the required
    .NET Framework the install step is expected to fail, and the report says so. Use -Interactive to run the
    normal wizard instead (the tester clicks through it, including the .NET download), then let the script
    check, start and uninstall the program.

    Nothing is changed except installing and removing HT ISO Ripper itself. Settings and logs in your user
    profile are kept, as the real uninstaller does.
#>
param(
    [Parameter(Mandatory = $true)][string]$Setup,
    [string]$Report = (Join-Path (Get-Location) 'HTISORipper-e2e-report.txt'),
    [switch]$Interactive,
    [switch]$SkipLaunch
)

$ErrorActionPreference = 'Stop'
if (-not [IO.Path]::IsPathRooted($Report)) { $Report = Join-Path (Get-Location).Path $Report }
$AppId = '{7C1F3B2E-5A4D-4E8B-9C21-3F6A8D0B4E17}_is1'
$results = New-Object System.Collections.ArrayList
$failures = 0

function Add-Result([string]$name, [bool]$ok, [string]$detail) {
    $status = 'PASS'
    if (-not $ok) { $status = 'FAIL'; $script:failures++ }
    [void]$results.Add(('{0}  {1}  {2}' -f $status, $name, $detail))
    Write-Host ('{0}  {1}  {2}' -f $status, $name, $detail)
}

function Get-UninstallEntry {
    $paths = @("HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\$AppId",
               "HKLM:\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall\$AppId")
    foreach ($p in $paths) { if (Test-Path $p) { return Get-ItemProperty $p } }
    return $null
}

function Count-UninstallEntries {
    $n = 0
    foreach ($root in 'HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall', 'HKLM:\SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall') {
        if (Test-Path $root) {
            foreach ($k in Get-ChildItem $root) {
                $name = (Get-ItemProperty $k.PSPath).DisplayName
                if ($name -eq 'HT ISO Ripper') { $n++ }
            }
        }
    }
    return $n
}

function Read-Shared([string]$path) {
    # The program may be appending to the log at the same moment: open with read/write sharing.
    if (-not (Test-Path $path)) { return '' }
    try {
        $fs = New-Object IO.FileStream($path, [IO.FileMode]::Open, [IO.FileAccess]::Read, [IO.FileShare]::ReadWrite)
        try { return (New-Object IO.StreamReader($fs)).ReadToEnd() } finally { $fs.Dispose() }
    } catch { return '' }
}

function Wait-Until([scriptblock]$condition, [int]$seconds) {
    for ($i = 0; $i -lt $seconds; $i++) { if (& $condition) { return $true }; Start-Sleep -Seconds 1 }
    return (& $condition)
}

function Install-App([string]$label) {
    $log = Join-Path (Split-Path -Parent $Report) ("htisoripper-setup-{0}.log" -f $label)
    if ($Interactive) {
        Write-Host "Complete the HT ISO Ripper Setup wizard ($label). The script continues when Setup closes."
        $p = Start-Process -FilePath $Setup -ArgumentList "/LOG=`"$log`"" -PassThru -Wait
    } else {
        $p = Start-Process -FilePath $Setup -ArgumentList '/VERYSILENT', '/SUPPRESSMSGBOXES', '/NORESTART', "/LOG=`"$log`"" -PassThru -Wait
    }
    return @($p.ExitCode, $log)
}

# ------------------------------------------------------------------ environment
$principal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
if (-not $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Error 'Run this script from an elevated PowerShell (Run as administrator).'
}
if (-not (Test-Path $Setup)) { Write-Error "Setup not found: $Setup" }
$Setup = (Resolve-Path $Setup).Path

$os = Get-WmiObject Win32_OperatingSystem
$release = 0
$ndp = 'HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full'
if (Test-Path $ndp) { $r = (Get-ItemProperty $ndp).Release; if ($r) { $release = [int]$r } }
$is64 = [Environment]::GetEnvironmentVariable('PROCESSOR_ARCHITEW6432') -or ([IntPtr]::Size -eq 8)
$ver = [Version]$os.Version
$isWin8 = ($ver.Major -eq 6 -and $ver.Minor -eq 2 -and $os.ProductType -eq 1)
if ($isWin8) { $expectedSku = 'v4.6.1'; $needed = 394254 } else { $expectedSku = 'v4.8'; $needed = 528040 }
# Setup's own product version (for example 1.0.1) is what Programs and Features must show.
# (Inno Setup pads the version resource string, so trim spaces and NULs.)
$expectedVersion = ([string](Get-Item $Setup).VersionInfo.ProductVersion).Trim([char[]]@([char]0, ' '))

$header = @(
    'HT ISO Ripper - end-to-end installer test',
    ('Date:        {0}' -f (Get-Date -Format 'yyyy-MM-dd HH:mm:ss')),
    ('Computer OS: {0} {1} (build {2}), service pack {3}, {4}' -f $os.Caption, $os.Version, $os.BuildNumber, $os.ServicePackMajorVersion, $os.OSArchitecture),
    ('.NET:        release {0} (this build needs {1} or later)' -f $release, $needed),
    ('Setup:       {0}' -f $Setup),
    ('Mode:        {0}' -f $(if ($Interactive) { 'interactive wizard' } else { 'silent' })),
    '')
$header | ForEach-Object { Write-Host $_ }

if (Get-UninstallEntry) { Write-Error 'HT ISO Ripper is already installed. Uninstall it first so the test starts clean.' }

# ------------------------------------------------------------------ 1. install
$res = Install-App 'install'
$code = $res[0]; $setupLog = $res[1]
if (-not $Interactive -and $release -lt $needed) {
    Add-Result 'Silent install without required .NET is refused' ($code -ne 0) ("exit code $code (silent mode never downloads .NET); see $setupLog")
    $results | Out-File -Encoding UTF8 $Report
    Write-Host "Required .NET is missing: install it (or run with -Interactive) to test the rest. Report: $Report"
    exit $failures
}
Add-Result 'Install' ($code -eq 0) "exit code $code; log $setupLog"

$entry = Get-UninstallEntry
Add-Result 'Registered in Programs and Features' ($entry -ne $null) $AppId
if ($entry -eq $null) { $results | Out-File -Encoding UTF8 $Report; exit 1 }
Add-Result "Display version $expectedVersion" ($entry.DisplayVersion -eq $expectedVersion) $entry.DisplayVersion
Add-Result 'Publisher HTREASURE LTD' ($entry.Publisher -eq 'HTREASURE LTD') $entry.Publisher
Add-Result 'Publisher website' ($entry.URLInfoAbout -eq 'https://www.htreasure.com') $entry.URLInfoAbout

$dir = $entry.InstallLocation.TrimEnd('\')
$exe = Join-Path $dir 'HTISORipper.exe'
Add-Result 'Installed to Program Files' ($dir -like "*\Program Files*\HTREASURE LTD\HT ISO Ripper") $dir
foreach ($f in 'HTISORipper.exe', 'HTISORipper.exe.config', 'HTISORipper.Core.dll', 'HTISORipper.DiscEngine.dll', 'HTISORipper.Imaging.dll',
               'HTISORipper.Logging.dll', 'HTISORipper.Verification.dll', 'LICENSE.txt', 'PRIVACY.txt', 'Documentation\USER-GUIDE.md',
               'Documentation\TROUBLESHOOTING.md', 'Documentation\RELEASE-NOTES.md', 'Documentation\THIRD-PARTY-LICENSES.md') {
    Add-Result "File present: $f" (Test-Path (Join-Path $dir $f)) ''
}
$config = [IO.File]::ReadAllText("$exe.config")
Add-Result "Correct build selected ($expectedSku)" ($config.Contains("Version=$expectedSku`"")) ''
$licence = [IO.File]::ReadAllText((Join-Path $dir 'LICENSE.txt'))
Add-Result 'Licence is the HT ISO Ripper EULA' ($licence.StartsWith('HT ISO Ripper - End-User Licence Agreement')) ''

# Architecture: inspect the PE header (works on PowerShell 2.0 / CLR 2, which cannot load .NET 4 assemblies).
$bytes = [IO.File]::ReadAllBytes($exe)
$pe = [BitConverter]::ToInt32($bytes, 0x3C)
$machine = [BitConverter]::ToUInt16($bytes, $pe + 4)
$expectedMachine = 0x14C
if ($is64) { $expectedMachine = 0x8664 }
Add-Result ('Correct architecture ({0})' -f $(if ($is64) { 'x64' } else { 'x86' })) ($machine -eq $expectedMachine) ('PE machine 0x{0:X4}' -f $machine)

$shortcut = Join-Path ([Environment]::GetFolderPath('CommonPrograms')) 'HT ISO Ripper.lnk'
Add-Result 'Start Menu shortcut' (Test-Path $shortcut) $shortcut

# ------------------------------------------------------------------ 2. start the program
if (-not $SkipLaunch) {
    $logDir = Join-Path $env:LOCALAPPDATA 'HTREASURE LTD\HT ISO Ripper\Logs'
    $logFile = Join-Path $logDir ('HTISORipper_{0}.log' -f (Get-Date -Format 'yyyyMMdd'))
    $before = 0
    if (Test-Path $logFile) { $before = (Get-Item $logFile).Length }
    $app = Start-Process -FilePath $exe -PassThru
    $started = Wait-Until { (Read-Shared $logFile).Length -gt $before -and (Read-Shared $logFile).Substring($before) -match 'Optical drives:' } 45
    $all = Read-Shared $logFile
    $text = ''
    if ($all.Length -gt $before) { $text = $all.Substring($before) }
    Add-Result 'Program starts and scans drives' $started 'log contains "Optical drives:"'
    Add-Result 'No unhandled exception at start-up' (-not ($text -match 'Unhandled exception')) ''
    $m = [regex]::Match($text, 'Optical drives: ([^\r\n]*)')
    if ($m.Success) { Add-Result 'Drive detection result' $true $m.Groups[1].Value }
    if (-not $app.HasExited) { Stop-Process -Id $app.Id -Force }
    Start-Sleep -Seconds 2
}

# ------------------------------------------------------------------ 3. upgrade / reinstall in place
if (-not $Interactive) {
    $res = Install-App 'reinstall'
    Add-Result 'Reinstall over existing version' ($res[0] -eq 0) ("exit code {0}" -f $res[0])
    Add-Result 'Still exactly one Programs and Features entry' ((Count-UninstallEntries) -eq 1) ''
    Add-Result 'Same install folder after reinstall' ((Get-UninstallEntry).InstallLocation.TrimEnd('\') -eq $dir) ''
}

# ------------------------------------------------------------------ 4. uninstall
$uninstaller = Join-Path $dir 'unins000.exe'
Add-Result 'Uninstaller present' (Test-Path $uninstaller) $uninstaller
Start-Process -FilePath $uninstaller -ArgumentList '/VERYSILENT', '/SUPPRESSMSGBOXES', '/NORESTART' -Wait
# Inno's uninstaller re-launches itself from a temporary copy, so wait for the result rather than the process.
$gone = Wait-Until { -not (Get-UninstallEntry) -and -not (Test-Path $exe) } 120
Add-Result 'Uninstall removes registration and program files' $gone ''
Add-Result 'Start Menu shortcut removed' (-not (Test-Path $shortcut)) ''
$settingsDir = Join-Path $env:APPDATA 'HTREASURE LTD\HT ISO Ripper'
if (Test-Path $settingsDir) { Add-Result 'User settings kept by silent uninstall' $true $settingsDir }
$ndpAfter = 0
if (Test-Path $ndp) { $ndpAfter = [int](Get-ItemProperty $ndp).Release }
Add-Result 'Microsoft .NET Framework left installed' ($ndpAfter -eq $release -or $ndpAfter -gt $release) "release $ndpAfter"

# ------------------------------------------------------------------ report
$summary = '{0} check(s), {1} failure(s)' -f $results.Count, $failures
($header + $results + @('', $summary)) | Out-File -Encoding UTF8 $Report
Write-Host ''
Write-Host $summary
Write-Host "Report: $Report"
exit $failures
