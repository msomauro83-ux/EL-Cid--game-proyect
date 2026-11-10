param([switch]$SkipAssetPreparation)
$ErrorActionPreference = 'Stop'
$project = $PSScriptRoot
$tools = Join-Path $project '.tools'

if (-not $SkipAssetPreparation) {
    & (Join-Path $project 'prepare-assets.ps1')
}

$jdk = Get-ChildItem -LiteralPath (Join-Path $tools 'jdk') -Directory -ErrorAction SilentlyContinue |
    Where-Object { Test-Path -LiteralPath (Join-Path $_.FullName 'bin\java.exe') } |
    Select-Object -First 1 -ExpandProperty FullName
$gradle = Join-Path $tools 'gradle\gradle-8.11.1\bin\gradle.bat'
$sdk = Join-Path $tools 'sdk'
if (-not $jdk) { throw 'Falta Java 17 en android\.tools\jdk.' }
if (-not (Test-Path -LiteralPath $gradle)) { throw 'Falta Gradle 8.11.1 en android\.tools\gradle.' }
if (-not (Test-Path -LiteralPath (Join-Path $sdk 'platforms\android-35'))) { throw 'Falta Android SDK Platform 35.' }

$env:JAVA_HOME = $jdk
$env:ANDROID_HOME = $sdk
$env:ANDROID_SDK_ROOT = $sdk
$env:ANDROID_USER_HOME = Join-Path $tools 'android-user'
$env:GRADLE_USER_HOME = Join-Path $tools 'gradle-user'
[IO.Directory]::CreateDirectory($env:ANDROID_USER_HOME) | Out-Null
[IO.Directory]::CreateDirectory($env:GRADLE_USER_HOME) | Out-Null

Push-Location $project
try { & $gradle --no-daemon assembleDebug; if ($LASTEXITCODE -ne 0) { throw "Gradle falló: $LASTEXITCODE" } }
finally { Pop-Location }

$apk = Join-Path $project 'app\build\outputs\apk\debug\app-debug.apk'
if (-not (Test-Path -LiteralPath $apk)) { throw 'No se generó el APK.' }
$destination = Join-Path (Resolve-Path (Join-Path $project '..\publicar')).Path 'El Cantar de Mio Cid - Android.apk'
Copy-Item -LiteralPath $apk -Destination $destination -Force
Get-Item -LiteralPath $destination | Select-Object FullName,Length
