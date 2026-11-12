<#
.SYNOPSIS
    Generates the HT ISO Ripper Setup wizard bitmaps from assets\branding\ht_logo.png.

    installer\images\wizard-large-100.bmp (164x314) and -200.bmp (328x628): left panel of Welcome/Finish pages
    installer\images\wizard-small-100.bmp (55x55)  and -200.bmp (110x110):  top-right image of inner pages

    Re-run after replacing the placeholder logo with the official HTREASURE LTD artwork.
#>
param([string]$Root = (Split-Path -Parent $PSScriptRoot))
Add-Type -AssemblyName System.Drawing
$ErrorActionPreference = 'Stop'

$navy = [System.Drawing.ColorTranslator]::FromHtml('#0B3D91')
$blue = [System.Drawing.ColorTranslator]::FromHtml('#1976D2')
$logo = [System.Drawing.Image]::FromFile((Join-Path $Root 'assets\branding\ht_logo.png'))
$out = Join-Path $Root 'installer\images'
New-Item -ItemType Directory -Force $out | Out-Null

function Save-Bmp($bmp, $name) {
    # Inno Setup requires 24-bit BMP without alpha.
    $flat = New-Object System.Drawing.Bitmap($bmp.Width, $bmp.Height, [System.Drawing.Imaging.PixelFormat]::Format24bppRgb)
    $g = [System.Drawing.Graphics]::FromImage($flat); $g.DrawImage($bmp, 0, 0, $bmp.Width, $bmp.Height); $g.Dispose()
    $flat.Save((Join-Path $out $name), [System.Drawing.Imaging.ImageFormat]::Bmp); $flat.Dispose()
}

foreach ($scale in 1, 2) {
    $w = 164 * $scale; $h = 314 * $scale
    $bmp = New-Object System.Drawing.Bitmap($w, $h)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.SmoothingMode = 'AntiAlias'; $g.TextRenderingHint = 'AntiAliasGridFit'
    $brush = New-Object System.Drawing.Drawing2D.LinearGradientBrush((New-Object System.Drawing.Rectangle(0, 0, $w, $h)), $blue, $navy, 90.0)
    $g.FillRectangle($brush, 0, 0, $w, $h)
    $size = 88 * $scale; $top = 48 * $scale; $left = [int](($w - $size) / 2)
    $g.DrawImage($logo, $left, $top, $size, $size)
    $white = [System.Drawing.Brushes]::White
    $fmt = New-Object System.Drawing.StringFormat; $fmt.Alignment = 'Center'
    $titleFont = [System.Drawing.Font]::new('Segoe UI Semibold', [single](13 * $scale), [System.Drawing.FontStyle]::Bold, [System.Drawing.GraphicsUnit]::Pixel)
    $subFont = [System.Drawing.Font]::new('Segoe UI', [single](10 * $scale), [System.Drawing.FontStyle]::Regular, [System.Drawing.GraphicsUnit]::Pixel)
    $titleRect = [System.Drawing.RectangleF]::new(0, [single](150 * $scale), $w, [single](24 * $scale))
    $subRect = [System.Drawing.RectangleF]::new(0, [single](176 * $scale), $w, [single](18 * $scale))
    $g.DrawString('HT ISO Ripper', $titleFont, $white, $titleRect, $fmt)
    $g.DrawString('HTREASURE LTD', $subFont, $white, $subRect, $fmt)
    $g.Dispose()
    Save-Bmp $bmp ("wizard-large-{0}.bmp" -f ($scale * 100)); $bmp.Dispose()

    $s = 55 * $scale
    $small = New-Object System.Drawing.Bitmap($s, $s)
    $g = [System.Drawing.Graphics]::FromImage($small); $g.Clear([System.Drawing.Color]::White); $g.SmoothingMode = 'AntiAlias'
    $pad = 3 * $scale; $inner = $s - 2 * $pad
    $g.DrawImage($logo, $pad, $pad, $inner, $inner); $g.Dispose()
    Save-Bmp $small ("wizard-small-{0}.bmp" -f ($scale * 100)); $small.Dispose()
}
$logo.Dispose()
Write-Host "Installer images written to $out"
