$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing

$projectRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$assets = Join-Path $PSScriptRoot 'app\src\main\assets'
$imagesOut = Join-Path $assets 'images'
$drawableOut = Join-Path $PSScriptRoot 'app\src\main\res\drawable-nodpi'
[IO.Directory]::CreateDirectory($imagesOut) | Out-Null
[IO.Directory]::CreateDirectory($drawableOut) | Out-Null

$questions = Get-Content -Encoding UTF8 -LiteralPath (Join-Path $projectRoot 'publicar\Juego\decisiones.txt')
if ($questions.Count -ne 100) { throw 'Se necesitan 100 decisiones.' }
$rows = [Collections.Generic.List[object]]::new()
foreach ($line in $questions) {
    $parts = $line.Split('|')
    if ($parts.Count -ne 4) { throw "Decisión inválida: $line" }
    $rows.Add([string[]]$parts)
}
$json = ConvertTo-Json -InputObject $rows.ToArray() -Depth 4 -Compress
[IO.File]::WriteAllText((Join-Path $assets 'decisiones.js'), 'window.CID_DECISIONS=' + $json + ';', [Text.UTF8Encoding]::new($false))

$codec = [Drawing.Imaging.ImageCodecInfo]::GetImageEncoders() | Where-Object MimeType -eq 'image/jpeg' | Select-Object -First 1
$parameters = [Drawing.Imaging.EncoderParameters]::new(1)
$parameters.Param[0] = [Drawing.Imaging.EncoderParameter]::new([Drawing.Imaging.Encoder]::Quality, [long]82)
function Save-Scaled($inputFile, $outputFile, $maxWidth, $format) {
    $source = [Drawing.Image]::FromFile($inputFile)
    try {
        $width = [Math]::Min($source.Width, $maxWidth)
        $height = [Math]::Round($source.Height * $width / $source.Width)
        $bitmap = [Drawing.Bitmap]::new([int]$width, [int]$height)
        $graphics = [Drawing.Graphics]::FromImage($bitmap)
        try {
            $graphics.Clear([Drawing.Color]::FromArgb(9, 16, 26))
            $graphics.InterpolationMode = [Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
            $graphics.SmoothingMode = [Drawing.Drawing2D.SmoothingMode]::HighQuality
            $graphics.PixelOffsetMode = [Drawing.Drawing2D.PixelOffsetMode]::HighQuality
            $graphics.DrawImage($source, 0, 0, $width, $height)
            if ($format -eq 'jpeg') { $bitmap.Save($outputFile, $codec, $parameters) }
            else { $bitmap.Save($outputFile, [Drawing.Imaging.ImageFormat]::Png) }
        } finally { $graphics.Dispose(); $bitmap.Dispose() }
    } finally { $source.Dispose() }
}

$inputs = Get-ChildItem -LiteralPath (Join-Path $projectRoot 'imagenes') -Filter '*.png'
if ($inputs.Count -ne 102) { throw 'Se necesitan 102 ilustraciones.' }
foreach ($file in $inputs) {
    Save-Scaled $file.FullName (Join-Path $imagesOut ($file.BaseName + '.jpg')) 1280 'jpeg'
}
$icon = Join-Path $projectRoot 'src\assets\Cid.png'
Save-Scaled $icon (Join-Path $assets 'icon.png') 256 'png'
Save-Scaled $icon (Join-Path $drawableOut 'ic_launcher.png') 256 'png'

$resRoot = Join-Path $PSScriptRoot 'app\src\main\res'
foreach ($item in @(@('mdpi',48),@('hdpi',72),@('xhdpi',96),@('xxhdpi',144),@('xxxhdpi',192))) {
    $dir = Join-Path $resRoot ('mipmap-' + $item[0])
    [IO.Directory]::CreateDirectory($dir) | Out-Null
    Save-Scaled $icon (Join-Path $dir 'ic_launcher.png') ([int]$item[1]) 'png'
}
$foregroundDir = Join-Path $resRoot 'drawable-nodpi'
$foregroundPath = Join-Path $foregroundDir 'ic_launcher_foreground.png'
$sourceIcon = [Drawing.Image]::FromFile($icon)
$canvas = [Drawing.Bitmap]::new(432,432)
$graphics = [Drawing.Graphics]::FromImage($canvas)
try {
    $graphics.Clear([Drawing.Color]::Transparent)
    $graphics.InterpolationMode = [Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $graphics.DrawImage($sourceIcon,72,72,288,288)
    $canvas.Save($foregroundPath,[Drawing.Imaging.ImageFormat]::Png)
} finally { $graphics.Dispose();$canvas.Dispose();$sourceIcon.Dispose() }

$imageFiles = Get-ChildItem -LiteralPath $imagesOut -Filter '*.jpg'
$size = ($imageFiles | Measure-Object Length -Sum).Sum
"Imágenes listas: $($imageFiles.Count); tamaño total: $([Math]::Round($size / 1MB, 1)) MB"
