param([Parameter(Mandatory=$true)][string]$InstallDir)
$ErrorActionPreference='Stop'
$shell=New-Object -ComObject WScript.Shell
$programas=Join-Path ([Environment]::GetFolderPath('Programs')) 'El Cantar de Mio Cid'
[IO.Directory]::CreateDirectory($programas) | Out-Null
foreach($ruta in @((Join-Path ([Environment]::GetFolderPath('Desktop')) 'El Cantar de Mio Cid.lnk'),(Join-Path $programas 'El Cantar de Mio Cid.lnk'))) {
    $acceso=$shell.CreateShortcut($ruta)
    $acceso.TargetPath=Join-Path $InstallDir 'ElCid.exe'
    $acceso.WorkingDirectory=$InstallDir
    $acceso.Description='Juego de 100 decisiones basado en el Cantar de Mio Cid'
    $acceso.Save()
}
$desinstalar=$shell.CreateShortcut((Join-Path $programas 'Desinstalar.lnk'))
$desinstalar.TargetPath=Join-Path $InstallDir 'desinstalar.cmd'
$desinstalar.WorkingDirectory=$InstallDir
$desinstalar.Save()
