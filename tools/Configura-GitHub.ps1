#requires -Version 5.1
[CmdletBinding()]
param(
    [string]$Owner,
    [string]$Repository,
    [switch]$RequireSignedUpdates,
    [string[]]$AllowedSignerThumbprints
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$root = Split-Path -Parent $PSScriptRoot
$path = Join-Path $root 'github-release.json'

$current = [ordered]@{
    owner = ''
    repository = 'Robocopy-Drop'
    apiVersion = '2026-03-10'
    requireSignedUpdates = $false
    allowedSignerThumbprints = @()
}
if (Test-Path -LiteralPath $path) {
    $loaded = Get-Content -LiteralPath $path -Raw | ConvertFrom-Json
    if ($null -ne $loaded.owner) { $current.owner = [string]$loaded.owner }
    if ($null -ne $loaded.repository) { $current.repository = [string]$loaded.repository }
    if ($null -ne $loaded.apiVersion) { $current.apiVersion = [string]$loaded.apiVersion }
    if ($null -ne $loaded.requireSignedUpdates) { $current.requireSignedUpdates = [bool]$loaded.requireSignedUpdates }
    if ($null -ne $loaded.allowedSignerThumbprints) { $current.allowedSignerThumbprints = @($loaded.allowedSignerThumbprints) }
}

if ([string]::IsNullOrWhiteSpace($Owner)) {
    $Owner = Read-Host "Proprietario GitHub (utente o organizzazione) [$($current.owner)]"
    if ([string]::IsNullOrWhiteSpace($Owner)) { $Owner = [string]$current.owner }
}
if ([string]::IsNullOrWhiteSpace($Repository)) {
    $Repository = Read-Host "Nome repository [$($current.repository)]"
    if ([string]::IsNullOrWhiteSpace($Repository)) { $Repository = [string]$current.repository }
}
if ([string]::IsNullOrWhiteSpace($Owner)) {
    throw 'Il proprietario GitHub e obbligatorio.'
}
foreach ($entry in @($Owner, $Repository)) {
    if ($entry -notmatch '^[A-Za-z0-9_.-]+$') {
        throw "Valore GitHub non valido: '$entry'."
    }
}

$requireSigned = $current.requireSignedUpdates
if ($PSBoundParameters.ContainsKey('RequireSignedUpdates')) {
    $requireSigned = $RequireSignedUpdates.IsPresent
} elseif (-not $PSBoundParameters.ContainsKey('Owner') -and -not $PSBoundParameters.ContainsKey('Repository')) {
    $answer = Read-Host "Richiedere firma Authenticode attendibile per gli aggiornamenti? [S/N; attuale: $requireSigned]"
    if ($answer -match '^(?i:s|si|y|yes)$') { $requireSigned = $true }
    elseif ($answer -match '^(?i:n|no)$') { $requireSigned = $false }
}

$pins = if ($PSBoundParameters.ContainsKey('AllowedSignerThumbprints')) {
    @($AllowedSignerThumbprints)
} else {
    @($current.allowedSignerThumbprints)
}
$pins = @($pins | ForEach-Object {
    ([string]$_ -replace '[^0-9A-Fa-f]', '').ToUpperInvariant()
} | Where-Object { -not [string]::IsNullOrWhiteSpace($_) } | Select-Object -Unique)

$config = [ordered]@{
    owner = $Owner.Trim()
    repository = $Repository.Trim()
    apiVersion = '2026-03-10'
    requireSignedUpdates = [bool]$requireSigned
    allowedSignerThumbprints = $pins
}
$config | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $path -Encoding UTF8

Write-Host ''
Write-Host 'Configurazione salvata:' -ForegroundColor Green
Write-Host "  Repository: https://github.com/$($config.owner)/$($config.repository)"
Write-Host "  Firma obbligatoria: $($config.requireSignedUpdates)"
Write-Host "  Thumbprint autorizzati: $(@($config.allowedSignerThumbprints).Count)"
Write-Host ''
Write-Host 'Ricrea gli MSI: la configurazione viene incorporata in RobocopyDropRunner.exe.config.'
