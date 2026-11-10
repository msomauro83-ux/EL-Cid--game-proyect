@echo off
setlocal
set "DEST=%LOCALAPPDATA%\El Cantar de Mio Cid"
echo El juego instalado en "%DEST%" sera eliminado.
choice /C SN /M "Deseas continuar"
if errorlevel 2 exit /b 0
cd /d "%TEMP%"
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%DEST%\desinstalar.ps1"
if errorlevel 1 exit /b 1
echo Juego desinstalado.
pause
