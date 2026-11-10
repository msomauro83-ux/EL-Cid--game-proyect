$ErrorActionPreference='Stop'
$base=[IO.Path]::GetFullPath($env:LOCALAPPDATA)
$destino=[IO.Path]::GetFullPath((Join-Path $base 'El Cantar de Mio Cid'))
if (-not $destino.StartsWith($base.TrimEnd('\')+'\',[StringComparison]::OrdinalIgnoreCase) -or [IO.Path]::GetFileName($destino) -ne 'El Cantar de Mio Cid') {
    throw 'La ruta de instalación no es válida.'
}
$inicio=[IO.Path]::GetFullPath((Join-Path ([Environment]::GetFolderPath('Programs')) 'El Cantar de Mio Cid'))
$programas=[IO.Path]::GetFullPath([Environment]::GetFolderPath('Programs'))
if (-not $inicio.StartsWith($programas.TrimEnd('\')+'\',[StringComparison]::OrdinalIgnoreCase)) { throw 'La ruta del menú Inicio no es válida.' }
Remove-Item -LiteralPath (Join-Path ([Environment]::GetFolderPath('Desktop')) 'El Cantar de Mio Cid.lnk') -ErrorAction SilentlyContinue
Remove-Item -LiteralPath $inicio -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -LiteralPath $destino -Recurse -Force -ErrorAction SilentlyContinue
