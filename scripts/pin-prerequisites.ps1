<#
.SYNOPSIS
    Pins or verifies the Microsoft prerequisite packages listed in installer\prerequisites.ini.

.DESCRIPTION
    -Mode Pin     Resolve each FwLink to its final URL (HTTPS on an allow-listed Microsoft host only), download it,
                  require a valid Authenticode signature from O=Microsoft Corporation, record URL, size,
                  SHA-256 and signer thumbprint in the manifest. Run once per manifest update and review the diff.
    -Mode Verify  (CI / release builds) Download each pinned Url if not already present in installer\redist,
                  and fail unless SHA-256 and signature match the manifest exactly. Never edits the manifest.

    Downloads are stored in installer\redist (git-ignored). They are needed to build the offline installer.
#>
param(
    [ValidateSet('Pin', 'Verify')][string]$Mode = 'Verify',
    [string]$Root = (Split-Path -Parent $PSScriptRoot)
)
$ErrorActionPreference = 'Stop'
[Net.ServicePointManager]::SecurityProtocol = [Net.ServicePointManager]::SecurityProtocol -bor [Net.SecurityProtocolType]::Tls12

$manifestPath = Join-Path $Root 'installer\prerequisites.ini'
$redist = Join-Path $Root 'installer\redist'
New-Item -ItemType Directory -Force $redist | Out-Null
$allowedHosts = @('download.microsoft.com', 'download.visualstudio.microsoft.com', 'go.microsoft.com')

function Read-Ini([string]$path) {
    $data = [ordered]@{}; $section = $null
    foreach ($line in Get-Content $path) {
        if ($line -match '^\s*\[(.+)\]\s*$') { $section = $Matches[1]; $data[$section] = [ordered]@{} }
        elseif ($section -and $line -match '^\s*([^;=][^=]*)=(.*)$') { $data[$section][$Matches[1].Trim()] = $Matches[2] }
    }
    return $data
}

function Set-IniValue([string]$path, [string]$section, [string]$key, [string]$value) {
    $lines = [System.Collections.Generic.List[string]](Get-Content $path)
    $inSection = $false
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -match '^\s*\[(.+)\]\s*$') { $inSection = ($Matches[1] -eq $section); continue }
        if ($inSection -and $lines[$i] -match "^\s*$([regex]::Escape($key))=") { $lines[$i] = "$key=$value"; break }
    }
    [IO.File]::WriteAllLines($path, $lines)
}

function Assert-AllowedUrl([string]$url) {
    $u = [Uri]$url
    if ($u.Scheme -ne 'https') { throw "Refusing non-HTTPS URL: $url" }
    if ($allowedHosts -notcontains $u.Host.ToLowerInvariant()) { throw "Refusing URL on non-allow-listed host: $url" }
}

function Resolve-FinalUrl([string]$url) {
    for ($hop = 0; $hop -lt 6; $hop++) {
        Assert-AllowedUrl $url
        $req = [Net.HttpWebRequest]::Create($url)
        $req.Method = 'HEAD'; $req.AllowAutoRedirect = $false; $req.UserAgent = 'HTISORipper-ReleaseTools'
        $resp = $req.GetResponse()
        try {
            $code = [int]$resp.StatusCode
            if ($code -ge 300 -and $code -lt 400) { $url = [Uri]::new([Uri]$url, $resp.Headers['Location']).AbsoluteUri; continue }
            return $url
        } finally { $resp.Close() }
    }
    throw "Too many redirects for $url"
}

function Test-MicrosoftSignature([string]$file) {
    $sig = Get-AuthenticodeSignature -LiteralPath $file
    if ($sig.Status -ne 'Valid') { throw "Authenticode status '$($sig.Status)' for $file" }
    if ($sig.SignerCertificate.Subject -notmatch 'O=Microsoft Corporation') { throw "Unexpected signer '$($sig.SignerCertificate.Subject)' for $file" }
    return $sig.SignerCertificate.Thumbprint
}

$manifest = Read-Ini $manifestPath
foreach ($id in $manifest.Keys | Where-Object { $_ -ne 'Manifest' }) {
    $p = $manifest[$id]
    Write-Host "== $id : $($p.Name)"
    if ($Mode -eq 'Pin') {
        if ($p.DirectUrl) {
            # Documented override for retired fwlinks; still subject to the host allow-list and signature check.
            $final = $p.DirectUrl
        } else {
            $final = Resolve-FinalUrl $p.FwLink
        }
        Assert-AllowedUrl $final
        if (([Uri]$final).Host -eq 'go.microsoft.com') { throw "FwLink did not resolve to a download host: $final" }
        $fileName = [IO.Path]::GetFileName(([Uri]$final).LocalPath)
    } else {
        if (-not $p.SHA256) { throw "$id is not pinned. Run: scripts\pin-prerequisites.ps1 -Mode Pin" }
        $final = $p.Url; $fileName = $p.FileName
        Assert-AllowedUrl $final
    }
    $target = Join-Path $redist $fileName
    if (-not (Test-Path $target)) {
        Write-Host "   downloading $final"
        $tmp = "$target.download"
        Invoke-WebRequest -Uri $final -OutFile $tmp -UseBasicParsing -UserAgent 'HTISORipper-ReleaseTools'
        Move-Item -Force $tmp $target
    }
    $thumb = Test-MicrosoftSignature $target
    $hash = (Get-FileHash -Algorithm SHA256 -LiteralPath $target).Hash.ToLowerInvariant()
    $size = (Get-Item $target).Length
    Write-Host "   $fileName  $size bytes  SHA-256 $hash  signer $thumb"

    if ($Mode -eq 'Pin') {
        Set-IniValue $manifestPath $id 'Url' $final
        Set-IniValue $manifestPath $id 'FileName' $fileName
        Set-IniValue $manifestPath $id 'SizeBytes' $size
        Set-IniValue $manifestPath $id 'SHA256' $hash
        Set-IniValue $manifestPath $id 'SignerThumbprint' $thumb
        Set-IniValue $manifestPath $id 'PinnedOn' (Get-Date -Format 'yyyy-MM-dd')
    } elseif ($hash -ne $p.SHA256.ToLowerInvariant()) {
        Remove-Item -Force $target
        throw "$id SHA-256 mismatch: expected $($p.SHA256), got $hash. The file was deleted."
    }
}
if ($Mode -eq 'Pin') { Write-Host 'Prerequisites pinned. Review the manifest diff before committing.' }
else { Write-Host 'All prerequisites verified against the manifest.' }
