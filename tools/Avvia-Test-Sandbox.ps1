#requires -Version 5.1
[CmdletBinding()]
param()
$ErrorActionPreference = 'Stop'
$root = Split-Path -Parent $PSScriptRoot
$release = Join-Path $root 'release'
$itMsi = Join-Path $release 'RobocopyDrop-1.6.0-it-x64.msi'
$enMsi = Join-Path $release 'RobocopyDrop-1.6.0-en-x64.msi'
if (-not (Test-Path -LiteralPath $itMsi -PathType Leaf) -or -not (Test-Path -LiteralPath $enMsi -PathType Leaf)) {
    throw "Esegui prima Crea-MSI.cmd. MSI non trovati nella cartella: $release"
}
$escaped = $release.Replace('&','&amp;').Replace('<','&lt;').Replace('>','&gt;')
$wsb = Join-Path $root 'build\RobocopyDrop-Test.wsb'
New-Item -ItemType Directory -Force -Path (Split-Path $wsb -Parent) | Out-Null
@"
<Configuration>
  <vGPU>Disable</vGPU>
  <MappedFolders>
    <MappedFolder>
      <HostFolder>$escaped</HostFolder>
      <SandboxFolder>C:\RobocopyDropRelease</SandboxFolder>
      <ReadOnly>true</ReadOnly>
    </MappedFolder>
  </MappedFolders>
  <LogonCommand>
    <Command>explorer.exe C:\RobocopyDropRelease</Command>
  </LogonCommand>
</Configuration>
"@ | Set-Content -LiteralPath $wsb -Encoding UTF8
Start-Process -FilePath $wsb
