@echo off
setlocal
cd /d "%~dp0"
%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0tools\Configura-GitHub.ps1"
set "RC=%ERRORLEVEL%"
echo.
pause
exit /b %RC%
