<#
.SYNOPSIS
    Generates the TEMPORARY HT ISO Ripper placeholder logo and application icon.

.DESCRIPTION
    Writes:
      assets\branding\ht_logo.png        (256x256, used in the header, splash and About window)
      assets\branding\ht_logo_64.png     (64x64)
      assets\icons\htisoripper.ico       (16/24/32/48/64 BMP frames + 256 PNG frame; exe, shortcut, installer)

    To use the official HTREASURE LTD logo, replace these three files with artwork of the same names and sizes
    and rebuild. No code changes are needed.
#>
param([string]$Root = (Split-Path -Parent $PSScriptRoot))

Add-Type -AssemblyName System.Drawing
$ErrorActionPreference = 'Stop'

$navy  = [System.Drawing.ColorTranslator]::FromHtml('#0B3D91')
$blue  = [System.Drawing.ColorTranslator]::FromHtml('#1976D2')
$white = [System.Drawing.Color]::White

function New-LogoBitmap([int]$size) {
    $bmp = New-Object System.Drawing.Bitmap($size, $size, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
    $g.Clear([System.Drawing.Color]::Transparent)

    # Rounded navy tile with a blue gradient.
    $r = [Math]::Max(2, [int]($size * 0.22))
    $path = New-Object System.Drawing.Drawing2D.GraphicsPath
    $path.AddArc(0, 0, $r, $r, 180, 90)
    $path.AddArc($size - $r - 1, 0, $r, $r, 270, 90)
    $path.AddArc($size - $r - 1, $size - $r - 1, $r, $r, 0, 90)
    $path.AddArc(0, $size - $r - 1, $r, $r, 90, 90)
    $path.CloseFigure()
    $rect = New-Object System.Drawing.Rectangle(0, 0, $size, $size)
    $brush = New-Object System.Drawing.Drawing2D.LinearGradientBrush($rect, $blue, $navy, 45.0)
    $g.FillPath($brush, $path)

    # Disc: white ring, blue track groove, hub.
    $c = $size / 2.0
    $outer = $size * 0.30
    $penW = [Math]::Max(1.5, $size * 0.075)
    $pen = New-Object System.Drawing.Pen($white, $penW)
    $g.DrawEllipse($pen, [float]($c - $outer), [float]($c - $outer), [float]($outer * 2), [float]($outer * 2))
    if ($size -ge 32) {
        $groove = $size * 0.18
        $pen2 = New-Object System.Drawing.Pen($white, [Math]::Max(1.0, $size * 0.035))
        $g.DrawArc($pen2, [float]($c - $groove), [float]($c - $groove), [float]($groove * 2), [float]($groove * 2), 200, 120)
    }
    $hub = [Math]::Max(1.5, $size * 0.075)
    $g.FillEllipse((New-Object System.Drawing.SolidBrush($white)), [float]($c - $hub), [float]($c - $hub), [float]($hub * 2), [float]($hub * 2))
    $g.Dispose()
    return $bmp
}

function Get-DibBytes([System.Drawing.Bitmap]$bmp) {
    $w = $bmp.Width; $h = $bmp.Height
    $ms = New-Object System.IO.MemoryStream
    $bw = New-Object System.IO.BinaryWriter($ms)
    # BITMAPINFOHEADER (height doubled for XOR + AND masks)
    $bw.Write([int]40); $bw.Write([int]$w); $bw.Write([int]($h * 2)); $bw.Write([int16]1); $bw.Write([int16]32)
    $bw.Write([int]0); $bw.Write([int]0); $bw.Write([int]0); $bw.Write([int]0); $bw.Write([int]0); $bw.Write([int]0)
    for ($y = $h - 1; $y -ge 0; $y--) {
        for ($x = 0; $x -lt $w; $x++) {
            $p = $bmp.GetPixel($x, $y)
            $bw.Write([byte]$p.B); $bw.Write([byte]$p.G); $bw.Write([byte]$p.R); $bw.Write([byte]$p.A)
        }
    }
    $maskStride = [int]([Math]::Ceiling($w / 32.0) * 4)
    $bw.Write((New-Object byte[] ($maskStride * $h)))   # AND mask all zero: alpha channel is authoritative
    $bw.Flush()
    return ,$ms.ToArray()
}

function Get-PngBytes([System.Drawing.Bitmap]$bmp) {
    $ms = New-Object System.IO.MemoryStream
    $bmp.Save($ms, [System.Drawing.Imaging.ImageFormat]::Png)
    return ,$ms.ToArray()
}

$branding = Join-Path $Root 'assets\branding'
$icons = Join-Path $Root 'assets\icons'
New-Item -ItemType Directory -Force $branding, $icons | Out-Null

$logo = New-LogoBitmap 256
$logo.Save((Join-Path $branding 'ht_logo.png'), [System.Drawing.Imaging.ImageFormat]::Png)
$logo64 = New-LogoBitmap 64
$logo64.Save((Join-Path $branding 'ht_logo_64.png'), [System.Drawing.Imaging.ImageFormat]::Png)

$sizes = 16, 24, 32, 48, 64, 256
$frames = @()
foreach ($s in $sizes) {
    $b = New-LogoBitmap $s
    if ($s -eq 256) { $frames += ,(Get-PngBytes $b) } else { $frames += ,(Get-DibBytes $b) }
    $b.Dispose()
}

$ico = New-Object System.IO.MemoryStream
$w = New-Object System.IO.BinaryWriter($ico)
$w.Write([int16]0); $w.Write([int16]1); $w.Write([int16]$sizes.Count)
$offset = 6 + 16 * $sizes.Count
for ($i = 0; $i -lt $sizes.Count; $i++) {
    $s = $sizes[$i]
    $dim = if ($s -ge 256) { 0 } else { $s }
    $w.Write([byte]$dim); $w.Write([byte]$dim); $w.Write([byte]0); $w.Write([byte]0)
    $w.Write([int16]1); $w.Write([int16]32)
    $w.Write([int]$frames[$i].Length); $w.Write([int]$offset)
    $offset += $frames[$i].Length
}
foreach ($f in $frames) { $w.Write([byte[]]$f) }
$w.Flush()
[System.IO.File]::WriteAllBytes((Join-Path $icons 'htisoripper.ico'), $ico.ToArray())

Write-Host "Placeholder branding written to $branding and $icons"
