#requires -Version 5.1
[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$clsid = '{9F3D1E49-A465-4D4D-B570-25EC4D7F4D23}'
$oldClsid = '{7A1C7A42-3E9D-4B4F-9E64-2459BB1B972A}'
$appDir = Join-Path $env:LOCALAPPDATA 'RobocopyDrop\App'
$keys = @(
 'HKCU:\Software\Classes\Directory\shellex\DragDropHandlers\RobocopyDrop',
 'HKCU:\Software\Classes\Drive\shellex\DragDropHandlers\RobocopyDrop',
 'HKCU:\Software\Classes\Folder\shellex\DragDropHandlers\RobocopyDrop',
 "HKCU:\Software\Classes\CLSID\$clsid",
 "HKCU:\Software\Classes\CLSID\$oldClsid"
)
Stop-Process -Name RobocopyDropRunner,RobocopyDropCopy,RobocopyDropAgent -Force -ErrorAction SilentlyContinue
Stop-Process -Name explorer -Force -ErrorAction SilentlyContinue
Start-Sleep -Milliseconds 800
foreach ($key in $keys) { Remove-Item -LiteralPath $key -Recurse -Force -ErrorAction SilentlyContinue }
$approved='HKCU:\Software\Microsoft\Windows\CurrentVersion\Shell Extensions\Approved'
if (Test-Path $approved) {
 Remove-ItemProperty -LiteralPath $approved -Name $clsid -ErrorAction SilentlyContinue
 Remove-ItemProperty -LiteralPath $approved -Name $oldClsid -ErrorAction SilentlyContinue
}
Remove-ItemProperty -LiteralPath 'HKCU:\Software\Microsoft\Windows\CurrentVersion\Run' -Name 'RobocopyDropAgent' -ErrorAction SilentlyContinue
Remove-Item -LiteralPath $appDir -Recurse -Force -ErrorAction SilentlyContinue
Start-Process explorer.exe
Write-Host 'Versione pilota rimossa. Impostazioni e report sono stati conservati.' -ForegroundColor Green
