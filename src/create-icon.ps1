Add-Type -AssemblyName System.Drawing
$sourcePath = Join-Path $PSScriptRoot 'assets\Cid.png'
$targetPath = Join-Path $PSScriptRoot 'assets\Cid.ico'
$source = [Drawing.Image]::FromFile($sourcePath)
$sizes = @(256, 64, 48, 32, 16)
$payloads = @()
try {
    foreach ($size in $sizes) {
        $bitmap = [Drawing.Bitmap]::new($size, $size)
        $graphics = [Drawing.Graphics]::FromImage($bitmap)
        try {
            $graphics.InterpolationMode = [Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
            $graphics.SmoothingMode = [Drawing.Drawing2D.SmoothingMode]::HighQuality
            $graphics.DrawImage($source, 0, 0, $size, $size)
            $memory = [IO.MemoryStream]::new()
            try { $bitmap.Save($memory, [Drawing.Imaging.ImageFormat]::Png); $payloads += ,$memory.ToArray() }
            finally { $memory.Dispose() }
        } finally { $graphics.Dispose(); $bitmap.Dispose() }
    }
} finally { $source.Dispose() }
$file = [IO.File]::Create($targetPath)
$writer = [IO.BinaryWriter]::new($file)
try {
    $writer.Write([uint16]0)
    $writer.Write([uint16]1)
    $writer.Write([uint16]$sizes.Count)
    $offset = 6 + (16 * $sizes.Count)
    for ($i = 0; $i -lt $sizes.Count; $i++) {
        $writer.Write([byte]($sizes[$i] % 256))
        $writer.Write([byte]($sizes[$i] % 256))
        $writer.Write([byte]0)
        $writer.Write([byte]0)
        $writer.Write([uint16]1)
        $writer.Write([uint16]32)
        $writer.Write([uint32]$payloads[$i].Length)
        $writer.Write([uint32]$offset)
        $offset += $payloads[$i].Length
    }
    foreach ($payload in $payloads) { $writer.Write([byte[]]$payload) }
} finally { $writer.Dispose() }
Get-Item -LiteralPath $targetPath | Select-Object Name,Length
