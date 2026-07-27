@echo off
setlocal
cd /d "%~dp0"

set "BUILD_SCRIPT=%~dp0tools\Build-Release.ps1"
set "PREFLIGHT_LOG=%~dp0build\logs\preflight.log"
if not exist "%~dp0build\logs" mkdir "%~dp0build\logs" >nul 2>&1

%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe -NoProfile -ExecutionPolicy Bypass -Command "$tokens=$null; $errors=$null; [System.Management.Automation.Language.Parser]::ParseFile($env:BUILD_SCRIPT,[ref]$tokens,[ref]$errors) | Out-Null; if ($errors.Count -gt 0) { $lines = $errors | ForEach-Object { '{0}:{1}:{2}: {3}' -f $_.Extent.File,$_.Extent.StartLineNumber,$_.Extent.StartColumnNumber,$_.Message }; $lines | Set-Content -LiteralPath $env:PREFLIGHT_LOG -Encoding UTF8; $lines | ForEach-Object { Write-Host $_ -ForegroundColor Red }; exit 2 } else { if (Test-Path -LiteralPath $env:PREFLIGHT_LOG) { Remove-Item -LiteralPath $env:PREFLIGHT_LOG -Force -ErrorAction SilentlyContinue }; exit 0 }"
set "PREFLIGHT_RC=%ERRORLEVEL%"
if not "%PREFLIGHT_RC%"=="0" (
  echo.
  echo SCRIPT DI BUILD NON VALIDO. Log: "%PREFLIGHT_LOG%"
  echo INVALID BUILD SCRIPT. Log: "%PREFLIGHT_LOG%"
  pause
  exit /b %PREFLIGHT_RC%
)

%SystemRoot%\System32\WindowsPowerShell\v1.0\powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%BUILD_SCRIPT%"
set "RC=%ERRORLEVEL%"
echo.
if not "%RC%"=="0" (
  echo BUILD NON RIUSCITA. Consulta il log indicato sopra.
  echo BUILD FAILED. Review the log path shown above.
) else (
  echo Pacchetti MSI disponibili nella cartella release.
  echo MSI packages are available in the release folder.
)
pause
exit /b %RC%
