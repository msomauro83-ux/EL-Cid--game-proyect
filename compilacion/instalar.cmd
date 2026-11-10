@echo off
setlocal
set "DEST=%LOCALAPPDATA%\El Cantar de Mio Cid"
if not exist "%DEST%\imagenes" mkdir "%DEST%\imagenes"
if errorlevel 1 exit /b 1
copy /Y "%~dp0ElCid.exe" "%DEST%\ElCid.exe" >nul
if errorlevel 1 exit /b 1
copy /Y "%~dp0LEEME-INSTALADO.txt" "%DEST%\LEEME.txt" >nul
if errorlevel 1 exit /b 1
copy /Y "%~dp0desinstalar.cmd" "%DEST%\desinstalar.cmd" >nul
if errorlevel 1 exit /b 1
copy /Y "%~dp0desinstalar.ps1" "%DEST%\desinstalar.ps1" >nul
if errorlevel 1 exit /b 1
copy /Y "%~dp0*.png" "%DEST%\imagenes\" >nul
if errorlevel 1 exit /b 1
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0crear-accesos.ps1" -InstallDir "%DEST%"
if errorlevel 1 exit /b 1
start "" "%DEST%\ElCid.exe"
exit /b 0
