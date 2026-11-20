<#
.SYNOPSIS
    Builds the complete HT ISO Ripper release into release\v<version>\.

.DESCRIPTION
    1. Runs all unit and installer tests (skip with -SkipTests).
    2. Builds Release for x86 and x64; each produces the net48 and net461 builds.
    3. Verifies every package folder: exact expected files, 100% managed (IL) code, correct runtime config.
    4. Verifies the pinned Microsoft prerequisites (scripts\pin-prerequisites.ps1 -Mode Verify).
    5. Optionally Authenticode-signs application binaries and installers (see Code signing below).
    6. Compiles the online and offline installers with Inno Setup.
    7. Creates portable ZIPs, copies release documentation, writes SHA256SUMS.txt and release-manifest.json.

    Code signing (optional; without it the release is marked UNSIGNED, which is a release blocker).
    Route A - certificate in the Windows certificate store (OV/EV certificate from a commercial CA):
      $env:HT_SIGN_THUMBPRINT  SHA-1 thumbprint of HTREASURE LTD's code-signing certificate in the user/machine store
      $env:HT_TIMESTAMP_URL    RFC 3161 timestamp server URL of the certificate authority
    Route B - Azure Artifact Signing (formerly Trusted Signing; see docs\CODE-SIGNING.md):
      $env:HT_ARTIFACT_SIGNING_ENDPOINT  e.g. https://weu.codesigning.azure.net/
      $env:HT_ARTIFACT_SIGNING_ACCOUNT   Artifact Signing account name
      $env:HT_ARTIFACT_SIGNING_PROFILE   certificate profile name
      $env:HT_ARTIFACT_SIGNING_DLIB      path to Azure.CodeSigning.Dlib.dll (x64) from Microsoft's signing client
      Sign-in: Azure CLI (az login) locally, or AZURE_CLIENT_ID/AZURE_TENANT_ID/AZURE_CLIENT_SECRET in CI.
    Both routes:
      $env:SIGNTOOL_PATH       optional path to signtool.exe (Windows SDK, x64)
#>
param(
    [string]$Version = '1.0.1',
    [switch]$SkipTests,
    [string]$Root = (Split-Path -Parent $PSScriptRoot)
)
$ErrorActionPreference = 'Stop'
Set-Location $Root
$out = Join-Path $Root "release\v$Version"

function Step($text) { Write-Host ''; Write-Host "==> $text" -ForegroundColor Cyan }
function Find-Iscc {
    $c = @($env:ISCC_PATH, "$env:LOCALAPPDATA\Programs\Inno Setup 6\ISCC.exe", "${env:ProgramFiles(x86)}\Inno Setup 6\ISCC.exe", "$env:ProgramFiles\Inno Setup 6\ISCC.exe")
    $f = $c | Where-Object { $_ -and (Test-Path $_) } | Select-Object -First 1
    if (-not $f) { throw 'Inno Setup 6.3+ (ISCC.exe) not found. Install it or set ISCC_PATH.' }
    return $f
}
function Invoke-Checked([string]$exe, [string[]]$arguments) {
    & $exe @arguments
    if ($LASTEXITCODE -ne 0) { throw "$exe failed with exit code $LASTEXITCODE" }
}

$iscc = Find-Iscc
$useArtifactSigning = [bool]$env:HT_ARTIFACT_SIGNING_ACCOUNT
$signing = [bool]$env:HT_SIGN_THUMBPRINT -or $useArtifactSigning
if ($signing) {
    $signtool = $env:SIGNTOOL_PATH
    if (-not $signtool) { $signtool = Get-ChildItem "${env:ProgramFiles(x86)}\Windows Kits\10\bin\*\x64\signtool.exe" -ErrorAction SilentlyContinue | Sort-Object FullName -Descending | Select-Object -First 1 -ExpandProperty FullName }
    if (-not $signtool) { throw 'signtool.exe not found (install the Windows SDK or set SIGNTOOL_PATH).' }
    if ($useArtifactSigning) {
        foreach ($v in 'HT_ARTIFACT_SIGNING_ENDPOINT', 'HT_ARTIFACT_SIGNING_PROFILE', 'HT_ARTIFACT_SIGNING_DLIB') {
            if (-not [Environment]::GetEnvironmentVariable($v)) { throw "$v must be set for Azure Artifact Signing." }
        }
        if (-not (Test-Path $env:HT_ARTIFACT_SIGNING_DLIB)) { throw "Azure.CodeSigning.Dlib.dll not found at $env:HT_ARTIFACT_SIGNING_DLIB" }
        $metadata = Join-Path $env:TEMP 'htisoripper-artifact-signing.json'
        [ordered]@{ Endpoint = $env:HT_ARTIFACT_SIGNING_ENDPOINT; CodeSigningAccountName = $env:HT_ARTIFACT_SIGNING_ACCOUNT
                    CertificateProfileName = $env:HT_ARTIFACT_SIGNING_PROFILE } | ConvertTo-Json | Set-Content -Encoding UTF8 $metadata
        # Microsoft's documented signtool invocation for Artifact Signing (its own timestamp service).
        $signArgs = @('sign', '/fd', 'SHA256', '/tr', 'http://timestamp.acs.microsoft.com', '/td', 'SHA256', '/d', 'HT ISO Ripper',
                      '/dlib', $env:HT_ARTIFACT_SIGNING_DLIB, '/dmdf', $metadata)
    } else {
        if (-not $env:HT_TIMESTAMP_URL) { throw 'HT_TIMESTAMP_URL must be set when signing with a certificate thumbprint.' }
        $signArgs = @('sign', '/sha1', $env:HT_SIGN_THUMBPRINT, '/fd', 'SHA256', '/tr', $env:HT_TIMESTAMP_URL, '/td', 'SHA256', '/d', 'HT ISO Ripper')
    }
}

if (-not $SkipTests) {
    Step 'Running tests'
    Invoke-Checked 'dotnet' @('test', 'HTISORipper.sln', '-c', 'Release', '-nologo', '-v', 'q')
}

Step 'Building Release x86 and x64 (net48 + net461)'
foreach ($platform in 'x86', 'x64') {
    Invoke-Checked 'dotnet' @('build', 'HTISORipper.sln', '-c', 'Release', "-p:Platform=$platform", '-nologo', '-v', 'q')
}

Step 'Verifying application packages'
$expected = @('HTISORipper.exe', 'HTISORipper.exe.config', 'HTISORipper.Core.dll', 'HTISORipper.DiscEngine.dll',
              'HTISORipper.Imaging.dll', 'HTISORipper.Logging.dll', 'HTISORipper.Verification.dll')
$builds = @()
foreach ($platform in 'x86', 'x64') {
    foreach ($tfm in 'net48', 'net461') {
        $dir = Join-Path $Root "src\HTISORipper.UI\bin\$platform\Release\$tfm"
        $files = Get-ChildItem $dir -File | Where-Object { $_.Extension -ne '.pdb' } | Select-Object -ExpandProperty Name
        $missing = $expected | Where-Object { $files -notcontains $_ }
        $extra = $files | Where-Object { $expected -notcontains $_ }
        if ($missing) { throw "$platform/$tfm is missing: $($missing -join ', ')" }
        if ($extra) { throw "$platform/$tfm contains unexpected files: $($extra -join ', ')" }
        foreach ($bin in Get-ChildItem $dir -Include *.exe, *.dll -File -Recurse) {
            # Throws BadImageFormatException for native (non-.NET) binaries: the package must be 100% managed,
            # which is why no Visual C++ runtime is ever required.
            $name = [Reflection.AssemblyName]::GetAssemblyName($bin.FullName)
            $arch = $name.ProcessorArchitecture.ToString()
            $ok = ($arch -eq 'MSIL') -or ($platform -eq 'x86' -and $arch -eq 'X86') -or ($platform -eq 'x64' -and $arch -eq 'Amd64')
            if (-not $ok) { throw "$($bin.Name) in $platform/$tfm has architecture $arch" }
        }
        $config = Get-Content (Join-Path $dir 'HTISORipper.exe.config') -Raw
        $sku = if ($tfm -eq 'net48') { 'v4.8"' } else { 'v4.6.1"' }
        if ($config -notmatch [regex]::Escape($sku)) { throw "$platform/$tfm has the wrong runtime in HTISORipper.exe.config" }
        if ($tfm -eq 'net461' -and $config -match 'ApplicationConfigurationSection') { throw 'net461 config must not contain the WinForms DPI section' }
        Write-Host "   $platform/$tfm OK ($($files.Count) files, all managed)"
        $builds += [pscustomobject]@{ Platform = $platform; Framework = $tfm; Dir = $dir }
    }
}

Step 'Verifying pinned Microsoft prerequisites'
& (Join-Path $PSScriptRoot 'pin-prerequisites.ps1') -Mode Verify -Root $Root

if ($signing) {
    Step 'Signing application binaries'
    foreach ($b in $builds) {
        $targets = Get-ChildItem $b.Dir -Include *.exe, *.dll -File -Recurse | Select-Object -ExpandProperty FullName
        Invoke-Checked $signtool ($signArgs + $targets)
    }
} else {
    Write-Warning 'No signing configured (HT_SIGN_THUMBPRINT or HT_ARTIFACT_SIGNING_*): building UNSIGNED installers.'
}

Step 'Compiling installers'
if (Test-Path $out) { Remove-Item -Recurse -Force $out }
$isccBase = @('/Q', "/DOutputRoot=$out", "/DAppVersion=$Version")
if ($signing) {
    # Inno Setup sign-tool command: $q is a quote character and $f the (already quoted) file to sign.
    $innoArgs = foreach ($a in $signArgs) { if ($a -match '\s') { '$q' + $a + '$q' } else { $a } }
    $isccBase += '/Shtsign=$q' + $signtool + '$q ' + ($innoArgs -join ' ') + ' $f'
    $isccBase += '/DSignTool=htsign'
}
Invoke-Checked $iscc ($isccBase + @('installer\HTISORipper.iss'))
Invoke-Checked $iscc ($isccBase + @('/DOffline=1', 'installer\HTISORipper.iss'))

Step 'Creating portable packages'
$portable = Join-Path $out 'portable'
New-Item -ItemType Directory -Force $portable | Out-Null
foreach ($b in $builds) {
    $zip = Join-Path $portable "HT-ISO-Ripper-Portable-$Version-$($b.Framework)-$($b.Platform).zip"
    $stage = Join-Path $env:TEMP ("htportable_" + [guid]::NewGuid().ToString('N'))
    New-Item -ItemType Directory $stage | Out-Null
    Get-ChildItem $b.Dir -File | Where-Object { $_.Extension -ne '.pdb' } | Copy-Item -Destination $stage
    Copy-Item LICENSE (Join-Path $stage 'LICENSE.txt'); Copy-Item installer\PRIVACY.txt $stage
    # Documents opened from the Help menu (same layout as the installed program).
    $docs = New-Item -ItemType Directory (Join-Path $stage 'Documentation')
    Copy-Item docs\USER_MANUAL.md (Join-Path $docs 'USER-GUIDE.md')
    Copy-Item docs\release\TROUBLESHOOTING.md, docs\release\RELEASE-NOTES.md, docs\release\INSTALLATION.md, docs\release\THIRD-PARTY-LICENSES.md $docs
    Compress-Archive -Path "$stage\*" -DestinationPath $zip
    Remove-Item -Recurse -Force $stage
}
Copy-Item docs\release\PORTABLE-README.txt (Join-Path $portable 'README.txt')

Step 'Copying release documentation'
$docOut = Join-Path $out 'documentation'
New-Item -ItemType Directory -Force $docOut, (Join-Path $out 'testing'), (Join-Path $out 'checksums') | Out-Null
Copy-Item docs\release\INSTALLATION.md, docs\release\TROUBLESHOOTING.md, docs\release\RELEASE-NOTES.md, docs\release\THIRD-PARTY-LICENSES.md $docOut
Copy-Item docs\COMPATIBILITY.md $docOut
Copy-Item docs\USER_MANUAL.md (Join-Path $docOut 'USER-GUIDE.md')
Copy-Item docs\release\RELEASE-TEST-REPORT.md (Join-Path $out 'testing')

Step 'Verifying signatures'
$setups = Get-ChildItem $out -Recurse -Filter '*.exe'
foreach ($s in $setups) {
    $sig = Get-AuthenticodeSignature $s.FullName
    if ($signing -and $sig.Status -ne 'Valid') { throw "$($s.Name) signature status: $($sig.Status)" }
    Write-Host "   $($s.Name): $($sig.Status)"
}

Step 'Writing checksums and release manifest'
$artifacts = Get-ChildItem $out -Recurse -File | Where-Object { $_.FullName -notlike "*\checksums\*" } | Sort-Object FullName
$lines = foreach ($a in $artifacts) {
    $rel = $a.FullName.Substring($out.Length + 1).Replace('\', '/')
    "$((Get-FileHash -Algorithm SHA256 $a.FullName).Hash.ToLowerInvariant()) *$rel"
}
[IO.File]::WriteAllLines((Join-Path $out 'checksums\SHA256SUMS.txt'), [string[]]$lines)

$commit = (& git rev-parse HEAD 2>$null)
$dirty = [bool](& git status --porcelain 2>$null)
$manifest = [ordered]@{
    product = 'HT ISO Ripper'; publisher = 'HTREASURE LTD'; version = $Version
    builtUtc = (Get-Date).ToUniversalTime().ToString('yyyy-MM-ddTHH:mm:ssZ')
    gitCommit = $commit; workingTreeClean = -not $dirty
    signed = $signing
    innoSetup = (Get-Item $iscc).Directory.Name
    prerequisites = @(
        foreach ($id in 'NDP48', 'NDP461') {
            $sec = Get-Content installer\prerequisites.ini | Select-String -Pattern "^\[$id\]" -Context 0, 20
            [ordered]@{ id = $id
                url = (($sec.Context.PostContext | Where-Object { $_ -like 'Url=*' }) -replace '^Url=', '')
                sha256 = (($sec.Context.PostContext | Where-Object { $_ -like 'SHA256=*' }) -replace '^SHA256=', '') }
        })
    artifacts = @(foreach ($a in $artifacts) {
        [ordered]@{ path = $a.FullName.Substring($out.Length + 1).Replace('\', '/'); bytes = $a.Length
                    sha256 = (Get-FileHash -Algorithm SHA256 $a.FullName).Hash.ToLowerInvariant() } })
}
$manifest | ConvertTo-Json -Depth 5 | Set-Content -Encoding UTF8 (Join-Path $out 'checksums\release-manifest.json')

Step 'Done'
Get-ChildItem $out -Recurse -File | Select-Object @{n = 'File'; e = { $_.FullName.Substring($out.Length + 1) } }, @{n = 'MB'; e = { [math]::Round($_.Length / 1MB, 2) } } | Format-Table -AutoSize
if (-not $signing) { Write-Warning 'Release is UNSIGNED. Do not distribute publicly until signed (see docs\COMMERCIAL-RELEASE.md).' }
