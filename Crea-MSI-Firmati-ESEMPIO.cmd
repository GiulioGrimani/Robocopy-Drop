@echo off
setlocal
cd /d "%~dp0"
rem ESEMPIO: impostare il thumbprint di un certificato Authenticode valido.
set "THUMBPRINT=INSERIRE_THUMBPRINT_CERTIFICATO"
%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%~dp0tools\Build-Release.ps1" -CertificateThumbprint "%THUMBPRINT%"
set "RC=%ERRORLEVEL%"
pause
exit /b %RC%
