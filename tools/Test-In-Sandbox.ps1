#requires -Version 5.1
[CmdletBinding()]
param(
    [string]$ReleasePath,
    [ValidateSet('it','en')][string]$Language = 'it'
)
$ErrorActionPreference = 'Stop'
if (-not $ReleasePath) { $ReleasePath = Join-Path (Split-Path -Parent $PSScriptRoot) 'release' }
$msi = Join-Path $ReleasePath ("RobocopyDrop-1.6.0-$Language-x64.msi")
if (-not (Test-Path -LiteralPath $msi -PathType Leaf)) { throw "MSI non trovato: $msi" }
Write-Host "Avvio MSI $Language dentro Windows Sandbox..." -ForegroundColor Cyan
$arguments = @('/i', ('"' + $msi + '"'), 'ALLUSERS=2', 'MSIINSTALLPERUSER=1', '/passive', '/norestart')
$process = Start-Process -FilePath 'msiexec.exe' -ArgumentList $arguments -Wait -PassThru
if ($process.ExitCode -notin 0, 3010) { throw "Installazione MSI non riuscita. Codice: $($process.ExitCode)" }
Write-Host ''
Write-Host 'Controlli suggeriti:' -ForegroundColor Yellow
Write-Host '1. Verificare Robocopy Drop in App installate.'
Write-Host '2. Aprire Start / Tutte le app / Robocopy Drop.'
Write-Host '3. Aprire Impostazioni, modificare un valore e verificare che Annulla, Esc e X chiudano senza salvare.'
Write-Host '4. Provare Guida, Apri cartella report e Controlla aggiornamenti Robocopy Drop.'
Write-Host '5. Creare due cartelle e provare il trascinamento col tasto destro.'
Write-Host '6. Verificare che non esistano RobocopyDropAgent.exe o hotkey globali.'
Write-Host '7. Verificare che il controllo update non interrompa una copia attiva e che mostri un esito nelle Impostazioni.'
Write-Host '8. Selezionare Disinstalla Robocopy Drop e verificare che parta direttamente Windows Installer.'
Write-Host '9. Confermare la rimozione e verificare la scomparsa delle voci Start.'
pause
