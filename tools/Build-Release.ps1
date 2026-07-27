#requires -Version 5.1
[CmdletBinding()]
param(
    [ValidatePattern('^\d+\.\d+\.\d+$')]
    [string]$Version = '1.6.0',

    [ValidatePattern('^[^<>&";]+$')]
    [string]$Publisher = 'Robocopy Drop',

    [string]$CertificateThumbprint,
    [string]$PfxPath,
    [string]$TimestampUrl = 'http://timestamp.digicert.com',

    [switch]$AcceptWixTerms,
    [switch]$NonInteractive
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

$root = Split-Path -Parent $PSScriptRoot
$packageProject = Join-Path $root 'installer\Package\RobocopyDrop.Package.wixproj'
$nugetConfig = Join-Path $root 'NuGet.Config'
$payloadDir = Join-Path $root 'payload'
$assetsDir = Join-Path $root 'assets'
$docsDir = Join-Path $root 'docs'
$srcDir = Join-Path $root 'src'
$buildRoot = Join-Path $root 'build'
$releaseDir = Join-Path $root 'release'
$stagingDir = Join-Path $buildRoot 'staging'
$stagingCommon = Join-Path $stagingDir 'common'
$stagingItDll = Join-Path $stagingDir 'it\RobocopyDropExtension.dll'
$stagingEnDll = Join-Path $stagingDir 'en\RobocopyDropExtension.dll'
$logDir = Join-Path $buildRoot 'logs'
$logPath = Join-Path $logDir ('build-' + (Get-Date -Format 'yyyyMMdd-HHmmss') + '.log')
$toolsDir = Join-Path $root '.tools'
$localDotnet = Join-Path $toolsDir 'dotnet'
$packagesDir = Join-Path $toolsDir 'packages'
$githubConfigPath = Join-Path $root 'github-release.json'
$runnerSourceBase = '1.3.5/1.4.5'
$expectedItDll = '53F1556B1A9BAFE1850A4E320D475A794D30A0A34777333825E567574DE44878'
$expectedEnDll = 'B747FC72581E1F821C86BD4BC7A3ABF0985649472BF800899D003CB76BFC6F04'

# Keep build-tool output deterministic and avoid first-run side effects.
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
$env:DOTNET_NOLOGO = '1'
$env:DOTNET_GENERATE_ASPNET_CERTIFICATE = 'false'

function Write-Step([string]$Message) {
    Write-Host "`n==> $Message" -ForegroundColor Cyan
    Add-Content -LiteralPath $logPath -Value ("`n==> " + $Message) -Encoding UTF8
}

function Write-DiagnosticTail {
    if (-not (Test-Path -LiteralPath $logPath)) { return }
    Write-Host ''
    Write-Host 'Ultime righe del log / Last log lines:' -ForegroundColor Yellow
    Get-Content -LiteralPath $logPath -Tail 60 | ForEach-Object { Write-Host $_ }
}

function Invoke-Checked {
    param(
        [Parameter(Mandatory=$true)][string]$FilePath,
        [Parameter(Mandatory=$true)][string[]]$Arguments,
        [Parameter(Mandatory=$true)][string]$Description
    )
    Write-Host ('    ' + $FilePath + ' ' + ($Arguments -join ' ')) -ForegroundColor DarkGray
    Add-Content -LiteralPath $logPath -Value ('COMMAND: ' + $FilePath + ' ' + ($Arguments -join ' ')) -Encoding UTF8
    & $FilePath @Arguments 2>&1 | ForEach-Object {
        $line = ([string]$_).Replace(([char]0).ToString(), [string]::Empty)
        Write-Host $line
        Add-Content -LiteralPath $logPath -Value $line -Encoding UTF8
    }
    $code = $LASTEXITCODE
    if ($code -ne 0) {
        Write-DiagnosticTail
        throw "$Description non riuscito / failed. Codice / exit code: $code. Log: $logPath"
    }
}

function Get-GitHubUpdateConfiguration {
    $result = [ordered]@{
        owner = ''
        repository = 'Robocopy-Drop'
        apiVersion = '2026-03-10'
        requireSignedUpdates = $false
        allowedSignerThumbprints = @()
    }

    if (Test-Path -LiteralPath $githubConfigPath -PathType Leaf) {
        try {
            $configured = Get-Content -LiteralPath $githubConfigPath -Raw | ConvertFrom-Json
            if ($null -ne $configured.owner) { $result.owner = ([string]$configured.owner).Trim() }
            if ($null -ne $configured.repository) { $result.repository = ([string]$configured.repository).Trim() }
            if ($null -ne $configured.apiVersion -and -not [string]::IsNullOrWhiteSpace([string]$configured.apiVersion)) {
                $result.apiVersion = ([string]$configured.apiVersion).Trim()
            }
            if ($null -ne $configured.requireSignedUpdates) { $result.requireSignedUpdates = [bool]$configured.requireSignedUpdates }
            if ($null -ne $configured.allowedSignerThumbprints) {
                $result.allowedSignerThumbprints = @($configured.allowedSignerThumbprints | ForEach-Object {
                    ([string]$_ -replace '[^0-9A-Fa-f]', '').ToUpperInvariant()
                } | Where-Object { -not [string]::IsNullOrWhiteSpace($_) } | Select-Object -Unique)
            }
        }
        catch {
            throw "Configurazione GitHub non valida in '$githubConfigPath': $($_.Exception.Message)"
        }
    }

    foreach ($field in @('owner','repository')) {
        $value = [string]$result[$field]
        if (-not [string]::IsNullOrWhiteSpace($value) -and $value -notmatch '^[A-Za-z0-9_.-]+$') {
            throw "Valore GitHub '$field' non valido: '$value'."
        }
    }
    if ([string]::IsNullOrWhiteSpace([string]$result.repository)) {
        throw "Il campo repository in '$githubConfigPath' non puo essere vuoto."
    }
    if ([string]$result.apiVersion -notmatch '^\d{4}-\d{2}-\d{2}$') {
        throw "GitHub apiVersion non valida: '$($result.apiVersion)'."
    }
    return $result
}

function ConvertTo-XmlAttributeValue([string]$Value) {
    if ($null -eq $Value) { return '' }
    return [System.Security.SecurityElement]::Escape($Value)
}

function Test-Dotnet8([string]$Path) {
    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) { return $false }
    try {
        $sdks = & $Path --list-sdks 2>$null
        return @($sdks | Where-Object { $_ -match '^8\.' }).Count -gt 0
    } catch { return $false }
}

function Resolve-Dotnet8 {
    $system = Get-Command dotnet.exe -ErrorAction SilentlyContinue
    if ($system -and (Test-Dotnet8 $system.Source)) { return $system.Source }

    $localExe = Join-Path $localDotnet 'dotnet.exe'
    if (Test-Dotnet8 $localExe) { return $localExe }

    Write-Step 'Installazione locale .NET SDK 8 per la build / Installing local .NET SDK 8 for the build'
    New-Item -ItemType Directory -Force -Path $toolsDir, $localDotnet | Out-Null
    $installer = Join-Path $toolsDir 'dotnet-install.ps1'
    [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
    try {
        Invoke-WebRequest -UseBasicParsing -Uri 'https://dot.net/v1/dotnet-install.ps1' -OutFile $installer
    } catch {
        throw "Download di dotnet-install.ps1 non riuscito. Verifica proxy, VPN, firewall e TLS. Dettaglio: $($_.Exception.Message)"
    }

    & powershell.exe -NoProfile -ExecutionPolicy Bypass -File $installer -Channel 8.0 -Quality GA -InstallDir $localDotnet -NoPath 2>&1 |
        ForEach-Object {
            $line = ([string]$_).Replace(([char]0).ToString(), [string]::Empty)
            Write-Host $line
            Add-Content -LiteralPath $logPath -Value $line -Encoding UTF8
        }
    $installExitCode = $LASTEXITCODE
    if ($installExitCode -ne 0 -or -not (Test-Dotnet8 $localExe)) {
        Write-DiagnosticTail
        throw "Installazione locale del .NET SDK 8 non riuscita. Installa manualmente .NET SDK 8 oppure verifica proxy/firewall. Log: $logPath"
    }
    return $localExe
}

function Resolve-CSharpCompiler {
    $candidates = @(
        (Join-Path $env:WINDIR 'Microsoft.NET\Framework64\v4.0.30319\csc.exe'),
        (Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe')
    )
    foreach ($candidate in $candidates) {
        if (Test-Path -LiteralPath $candidate -PathType Leaf) { return $candidate }
    }
    throw 'Compilatore C# di .NET Framework non trovato sul PC di build.'
}

function Find-SignTool {
    $command = Get-Command signtool.exe -ErrorAction SilentlyContinue
    if ($command) { return $command.Source }
    $kits = Join-Path ${env:ProgramFiles(x86)} 'Windows Kits\10\bin'
    if (Test-Path -LiteralPath $kits) {
        $candidate = Get-ChildItem -LiteralPath $kits -Filter signtool.exe -Recurse -ErrorAction SilentlyContinue |
            Where-Object { $_.FullName -match '\\x64\\signtool\.exe$' } |
            Sort-Object FullName -Descending | Select-Object -First 1
        if ($candidate) { return $candidate.FullName }
    }
    return $null
}

function Sign-File([string]$Path) {
    if (-not $script:SigningEnabled) { return }
    $arguments = @('sign','/fd','SHA256','/tr',$TimestampUrl,'/td','SHA256')
    if ($PfxPath) {
        $arguments += @('/f', (Resolve-Path -LiteralPath $PfxPath).Path)
        if ($env:ROBOCOPYDROP_PFX_PASSWORD) { $arguments += @('/p', $env:ROBOCOPYDROP_PFX_PASSWORD) }
    } else {
        $arguments += @('/sha1', $CertificateThumbprint)
    }
    $arguments += $Path
    Invoke-Checked -FilePath $script:SignTool -Arguments $arguments -Description "Firma di $Path"
    Invoke-Checked -FilePath $script:SignTool -Arguments @('verify','/pa','/all',$Path) -Description "Verifica firma di $Path"
}


function Get-NativeErrorCodeFromException {
    param([System.Exception]$Exception)
    $current = $Exception
    while ($null -ne $current) {
        if ($current -is [System.ComponentModel.Win32Exception]) {
            return $current.NativeErrorCode
        }
        $current = $current.InnerException
    }
    return $null
}

function Test-IsApplicationControlBlock {
    param([System.Exception]$Exception)
    $nativeCode = Get-NativeErrorCodeFromException -Exception $Exception
    if ($nativeCode -eq 1260) { return $true }

    $messages = New-Object System.Collections.Generic.List[string]
    $current = $Exception
    while ($null -ne $current) {
        if (-not [string]::IsNullOrWhiteSpace($current.Message)) { $messages.Add($current.Message) }
        $current = $current.InnerException
    }
    $text = $messages -join ' | '
    return $text -match '(?i)application control|criterio di controllo dell.applicazione|blocked by.*policy|bloccato.*criterio|access disabled by policy'
}

function Get-StreamSha256 {
    param([Parameter(Mandatory=$true)][System.IO.Stream]$Stream)
    $algorithm = [System.Security.Cryptography.SHA256]::Create()
    try {
        $hash = $algorithm.ComputeHash($Stream)
        return ([BitConverter]::ToString($hash)).Replace('-', [string]::Empty)
    } finally {
        $algorithm.Dispose()
    }
}

function Test-ByteSequence {
    param(
        [Parameter(Mandatory=$true)][byte[]]$Buffer,
        [Parameter(Mandatory=$true)][byte[]]$Sequence
    )
    if ($Sequence.Length -eq 0 -or $Buffer.Length -lt $Sequence.Length) { return $false }
    $limit = $Buffer.Length - $Sequence.Length
    for ($i = 0; $i -le $limit; $i++) {
        if ($Buffer[$i] -ne $Sequence[0]) { continue }
        $matched = $true
        for ($j = 1; $j -lt $Sequence.Length; $j++) {
            if ($Buffer[$i + $j] -ne $Sequence[$j]) {
                $matched = $false
                break
            }
        }
        if ($matched) { return $true }
    }
    return $false
}

function Test-SetupStatic {
    param(
        [Parameter(Mandatory=$true)][string]$SetupPath,
        [Parameter(Mandatory=$true)][string]$ItalianMsiPath,
        [Parameter(Mandatory=$true)][string]$EnglishMsiPath,
        [Parameter(Mandatory=$true)][string]$ExpectedVersion
    )

    Assert-X64Pe $SetupPath
    $setupItem = Get-Item -LiteralPath $SetupPath
    $italianItem = Get-Item -LiteralPath $ItalianMsiPath
    $englishItem = Get-Item -LiteralPath $EnglishMsiPath
    $minimumPayloadSize = $italianItem.Length + $englishItem.Length
    if ($setupItem.Length -le $minimumPayloadSize) {
        throw "Setup.exe troppo piccolo per contenere entrambi gli MSI. Setup=$($setupItem.Length); MSI=$minimumPayloadSize"
    }

    $fileVersion = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($SetupPath).FileVersion
    if ([string]::IsNullOrWhiteSpace($fileVersion) -or -not $fileVersion.StartsWith($ExpectedVersion, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Versione Setup.exe inattesa durante il controllo statico: '$fileVersion'; attesa '$ExpectedVersion.x'."
    }

    $expectedItHash = (Get-FileHash -LiteralPath $ItalianMsiPath -Algorithm SHA256).Hash
    $expectedEnHash = (Get-FileHash -LiteralPath $EnglishMsiPath -Algorithm SHA256).Hash
    $reflectionError = $null
    $embeddedItHash = $null
    $embeddedEnHash = $null
    try {
        $assembly = [System.Reflection.Assembly]::ReflectionOnlyLoadFrom($SetupPath)
        $resourceNames = @($assembly.GetManifestResourceNames())
        $itResource = $resourceNames | Where-Object { $_.EndsWith('RobocopyDrop-it.msi', [System.StringComparison]::OrdinalIgnoreCase) } | Select-Object -First 1
        $enResource = $resourceNames | Where-Object { $_.EndsWith('RobocopyDrop-en.msi', [System.StringComparison]::OrdinalIgnoreCase) } | Select-Object -First 1
        if ([string]::IsNullOrWhiteSpace($itResource) -or [string]::IsNullOrWhiteSpace($enResource)) {
            throw 'Nomi delle risorse MSI non trovati nel Setup.exe.'
        }

        $itStream = $assembly.GetManifestResourceStream($itResource)
        $enStream = $assembly.GetManifestResourceStream($enResource)
        if ($null -eq $itStream -or $null -eq $enStream) { throw 'Impossibile aprire una risorsa MSI incorporata.' }
        try {
            $embeddedItHash = Get-StreamSha256 -Stream $itStream
            $embeddedEnHash = Get-StreamSha256 -Stream $enStream
        } finally {
            if ($null -ne $itStream) { $itStream.Dispose() }
            if ($null -ne $enStream) { $enStream.Dispose() }
        }
    } catch {
        $reflectionError = $_.Exception.Message
    }

    if ($null -eq $reflectionError) {
        if (-not [string]::Equals($embeddedItHash, $expectedItHash, [System.StringComparison]::OrdinalIgnoreCase)) {
            throw 'Hash MSI italiano incorporato non corrispondente.'
        }
        if (-not [string]::Equals($embeddedEnHash, $expectedEnHash, [System.StringComparison]::OrdinalIgnoreCase)) {
            throw 'Hash MSI inglese incorporato non corrispondente.'
        }
        return "PE x64; FileVersion=$fileVersion; risorse MSI e SHA-256 verificati tramite reflection"
    }

    # Se la policy impedisce anche il caricamento reflection-only, manteniamo
    # un controllo indipendente dall'esecuzione: PE, versione, dimensione e
    # presenza dei nomi risorsa nel metadata. Non e equivalente al self-test
    # eseguibile e viene segnalato chiaramente nella release.
    $buffer = [System.IO.File]::ReadAllBytes($SetupPath)
    $itName = [System.Text.Encoding]::UTF8.GetBytes('RobocopyDrop-it.msi')
    $enName = [System.Text.Encoding]::UTF8.GetBytes('RobocopyDrop-en.msi')
    if (-not (Test-ByteSequence -Buffer $buffer -Sequence $itName) -or
        -not (Test-ByteSequence -Buffer $buffer -Sequence $enName)) {
        throw "Controllo statico Setup.exe non superato. Reflection: $reflectionError"
    }
    return "PE x64; FileVersion=$fileVersion; dimensione e nomi risorsa verificati; reflection non disponibile: $reflectionError"
}

function Assert-X64Pe([string]$Path) {
    $bytes = [IO.File]::ReadAllBytes($Path)
    if ($bytes.Length -lt 256 -or $bytes[0] -ne 0x4D -or $bytes[1] -ne 0x5A) { throw "File PE non valido: $Path" }
    $offset = [BitConverter]::ToInt32($bytes, 0x3C)
    $machine = [BitConverter]::ToUInt16($bytes, $offset + 4)
    if ($machine -ne 0x8664) { throw ('File non x64: {0}; machine 0x{1:X4}' -f $Path, $machine) }
}

function Get-LanguageProperties([string]$Language) {
    if ($Language -eq 'en') {
        return @{
            ProductLanguage='1033'; Culture='en-US'; UiLanguage='en';
            ProductDescription='Copy files and folders with Robocopy from the right-drag menu.';
            DowngradeMessage='A newer version of Robocopy Drop is already installed.';
            NetFxMessage='Robocopy Drop requires Microsoft .NET Framework 4.8 or later. Install the prerequisite before running this MSI.';
            OsMessage='Robocopy Drop supports Windows 10 and Windows 11 x64 only.';
            ArchMessage='This build supports native Windows x64 PCs only. Windows ARM64 is not supported.';
            HandlerDescription='Robocopy Drop Drag-and-Drop Handler';
            SettingsShortcutName='Robocopy Drop Settings';
            SettingsShortcutDescription='Configure language, automatic media profile, and Robocopy threads';
            GuideShortcutName='Robocopy Drop Guide';
            GuideTarget='[INSTALLFOLDER]GUIDE-EN.pdf';
            ReportsShortcutName='Open Robocopy Drop Reports';
            ReportsShortcutDescription='Open the folder containing saved Robocopy Drop reports';
            UpdateShortcutName='Check for Robocopy Drop Updates';
            UpdateShortcutDescription='Check GitHub for a newer Robocopy Drop release';
            UninstallShortcutName='Uninstall Robocopy Drop';
            UninstallShortcutDescription='Start Windows Installer and remove Robocopy Drop';
        }
    }
    return @{
        ProductLanguage='1040'; Culture='it-IT'; UiLanguage='it';
        ProductDescription='Copia file e cartelle con Robocopy dal menu di trascinamento col tasto destro.';
        DowngradeMessage='E gia installata una versione piu recente di Robocopy Drop.';
        NetFxMessage='Robocopy Drop richiede Microsoft .NET Framework 4.8 o successivo. Installa il prerequisito prima di eseguire questo MSI.';
        OsMessage='Robocopy Drop supporta soltanto Windows 10 e Windows 11 x64.';
        ArchMessage='Questa build supporta soltanto PC Windows x64 nativi. Windows ARM64 non e supportato.';
        HandlerDescription='Gestore trascinamento Robocopy Drop';
        SettingsShortcutName='Impostazioni Robocopy Drop';
        SettingsShortcutDescription='Configura lingua, profilo automatico e thread Robocopy';
        GuideShortcutName='Guida Robocopy Drop';
        GuideTarget='[INSTALLFOLDER]GUIDA-IT.pdf';
        ReportsShortcutName='Apri cartella report';
        ReportsShortcutDescription='Apre la cartella contenente i report salvati da Robocopy Drop';
        UpdateShortcutName='Controlla aggiornamenti Robocopy Drop';
        UpdateShortcutDescription='Controlla su GitHub se e disponibile una nuova versione';
        UninstallShortcutName='Disinstalla Robocopy Drop';
        UninstallShortcutDescription='Avvia Windows Installer e rimuove Robocopy Drop';
    }
}


function Write-MsBuildPropertiesFile {
    param(
        [Parameter(Mandatory=$true)][string]$Path,
        [Parameter(Mandatory=$true)][System.Collections.IDictionary]$Properties
    )

    $settings = New-Object System.Xml.XmlWriterSettings
    $settings.Indent = $true
    $settings.OmitXmlDeclaration = $false
    $settings.Encoding = New-Object System.Text.UTF8Encoding -ArgumentList $false

    $writer = [System.Xml.XmlWriter]::Create($Path, $settings)
    try {
        $writer.WriteStartDocument()
        $writer.WriteStartElement('Project')
        $writer.WriteStartElement('PropertyGroup')
        foreach ($entry in $Properties.GetEnumerator()) {
            $writer.WriteStartElement([string]$entry.Key)
            $writer.WriteString([string]$entry.Value)
            $writer.WriteEndElement()
        }
        $writer.WriteEndElement()
        $writer.WriteEndElement()
        $writer.WriteEndDocument()
    }
    finally {
        $writer.Dispose()
    }

    # Round-trip the generated XML before invoking MSBuild. This catches
    # malformed values without relying on command-line quoting rules.
    [xml]$roundTrip = Get-Content -LiteralPath $Path -Raw
    if (-not $roundTrip.Project.PropertyGroup) {
        throw "File proprieta MSBuild non valido: $Path"
    }
    foreach ($entry in $Properties.GetEnumerator()) {
        $actual = [string]$roundTrip.Project.PropertyGroup.($entry.Key)
        if ($actual -ne [string]$entry.Value) {
            throw "Round-trip proprieta MSBuild non riuscito per '$($entry.Key)'."
        }
    }
}

function Build-Msi {
    param([string]$Language, [string]$Dotnet)

    $properties = Get-LanguageProperties $Language
    $outputDir = Join-Path $buildRoot ('msi-' + $Language)
    if (Test-Path -LiteralPath $outputDir) {
        Remove-Item -LiteralPath $outputDir -Recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $outputDir | Out-Null

    $extensionDll = if ($Language -eq 'en') { $stagingEnDll } else { $stagingItDll }
    $propsPath = Join-Path $buildRoot ('RobocopyDrop.Build.' + $Language + '.props')

    # Values with commas (for example the shortcut description) must not be
    # passed through -p:Name=Value: MSBuild treats commas as property
    # separators. A generated .props file avoids quoting and escaping bugs for
    # localized prose, paths with spaces, and future publisher names.
    $buildProperties = [ordered]@{
        ProductVersion = $Version
        Publisher = $Publisher
        ProductLanguage = $properties.ProductLanguage
        Culture = $properties.Culture
        UiLanguage = $properties.UiLanguage
        PayloadDir = $stagingCommon
        ExtensionDll = $extensionDll
        AssetsDir = $assetsDir
        DocsDir = $docsDir
        ProductDescription = $properties.ProductDescription
        DowngradeMessage = $properties.DowngradeMessage
        NetFxMessage = $properties.NetFxMessage
        OsMessage = $properties.OsMessage
        ArchMessage = $properties.ArchMessage
        HandlerDescription = $properties.HandlerDescription
        SettingsShortcutName = $properties.SettingsShortcutName
        SettingsShortcutDescription = $properties.SettingsShortcutDescription
        GuideShortcutName = $properties.GuideShortcutName
        GuideTarget = $properties.GuideTarget
        ReportsShortcutName = $properties.ReportsShortcutName
        ReportsShortcutDescription = $properties.ReportsShortcutDescription
        UpdateShortcutName = $properties.UpdateShortcutName
        UpdateShortcutDescription = $properties.UpdateShortcutDescription
        UninstallShortcutName = $properties.UninstallShortcutName
        UninstallShortcutDescription = $properties.UninstallShortcutDescription
        OutputPath = $outputDir
    }
    Write-MsBuildPropertiesFile -Path $propsPath -Properties $buildProperties
    Add-Content -LiteralPath $logPath -Value ("MSBUILD PROPS: " + $propsPath) -Encoding UTF8

    $previousProps = [Environment]::GetEnvironmentVariable('RobocopyDropBuildProps', 'Process')
    try {
        [Environment]::SetEnvironmentVariable('RobocopyDropBuildProps', $propsPath, 'Process')
        Invoke-Checked `
            -FilePath $Dotnet `
            -Arguments @('build', $packageProject, '-c', 'Release', '--no-restore') `
            -Description "Build MSI $Language"
    }
    finally {
        [Environment]::SetEnvironmentVariable('RobocopyDropBuildProps', $previousProps, 'Process')
    }

    $msi = Get-ChildItem -LiteralPath $outputDir -Filter '*.msi' -Recurse |
        Select-Object -First 1
    if (-not $msi) {
        throw "MSI $Language non trovato dopo la build. Cartella: $outputDir"
    }

    $finalName = Join-Path $releaseDir ("RobocopyDrop-$Version-$Language-x64.msi")
    Copy-Item -LiteralPath $msi.FullName -Destination $finalName -Force
    Sign-File $finalName
    return $finalName
}

try {
    if (-not [Environment]::Is64BitOperatingSystem) { throw 'La build richiede Windows x64.' }
    $architecture = "$env:PROCESSOR_ARCHITECTURE $env:PROCESSOR_ARCHITEW6432"
    if ($architecture -match 'ARM64') { throw 'La build richiede Windows x64 nativo; ARM64 non e supportato.' }
    if ($root.Contains(';') -or $root.Contains(',') -or $root.Contains("'") -or $root.Contains('%')) {
        throw "Il percorso del kit contiene uno dei caratteri non supportati dalla toolchain (; , ' %). Sposta il kit in un percorso semplice, ad esempio C:\Build\RobocopyDrop."
    }

    foreach ($required in @($packageProject,$nugetConfig,
        (Join-Path $payloadDir 'it\RobocopyDropExtension.dll'),
        (Join-Path $payloadDir 'en\RobocopyDropExtension.dll'),
        (Join-Path $payloadDir 'RobocopyDropRunner.exe.config.template'),
        (Join-Path $assetsDir 'RobocopyDrop.ico'),
        (Join-Path $docsDir 'GUIDA-IT.pdf'),
        (Join-Path $docsDir 'GUIDE-EN.pdf'),
        (Join-Path $docsDir 'LICENSE.txt'),
        (Join-Path $docsDir 'NOTICE-WIX.txt'),
        (Join-Path $root 'LICENSE'),
        (Join-Path $root 'PRIVACY.md'),
        $githubConfigPath,
        (Join-Path $srcDir 'RobocopyDropRunner.cs'))) {
        if (-not (Test-Path -LiteralPath $required -PathType Leaf)) { throw "File richiesto mancante: $required" }
    }

    # Validate the Start-menu contract before invoking WiX. This release must
    # contain settings, guide, reports, updates, and direct Windows Installer removal.
    [xml]$packageXml = Get-Content -LiteralPath (Join-Path (Split-Path $packageProject -Parent) 'Package.wxs') -Raw
    $shortcutNodes = @($packageXml.SelectNodes("//*[local-name()='Shortcut']"))
    $shortcutById = @{}
    foreach ($node in $shortcutNodes) { $shortcutById[[string]$node.Id] = $node }
    foreach ($requiredShortcut in @('SettingsShortcut','GuideShortcut','ReportsShortcut','UpdateShortcut','UninstallShortcut')) {
        if (-not $shortcutById.ContainsKey($requiredShortcut)) {
            throw "Scorciatoia Start mancante nel progetto WiX: $requiredShortcut"
        }
    }
    if ([string]$shortcutById['UninstallShortcut'].Target -notmatch 'msiexec\.exe$' -or
        [string]$shortcutById['UninstallShortcut'].Arguments -notmatch '/x\s+\[ProductCode\]') {
        throw 'La scorciatoia Disinstalla non punta a msiexec.exe /x [ProductCode].'
    }
    if ([string]$shortcutById['ReportsShortcut'].Target -notmatch 'cmd\.exe$' -or
        [string]$shortcutById['ReportsShortcut'].Arguments -notmatch 'RobocopyDrop\\Logs') {
        throw 'La scorciatoia Report non apre la cartella LocalAppData\RobocopyDrop\Logs.'
    }
    if ([string]$shortcutById['UpdateShortcut'].Target -notmatch '#RunnerExe' -or
        [string]$shortcutById['UpdateShortcut'].Arguments -notmatch '--check-updates') {
        throw 'La scorciatoia Aggiornamenti non avvia il runner con --check-updates.'
    }

    New-Item -ItemType Directory -Force -Path $buildRoot,$releaseDir,$stagingCommon,$logDir,$toolsDir,$packagesDir | Out-Null
    Get-ChildItem -LiteralPath $releaseDir -Force -ErrorAction SilentlyContinue | Remove-Item -Recurse -Force
    if (Test-Path -LiteralPath $stagingDir) { Remove-Item -LiteralPath $stagingDir -Recurse -Force }
    New-Item -ItemType Directory -Force -Path $stagingCommon,(Split-Path $stagingItDll -Parent),(Split-Path $stagingEnDll -Parent),$logDir | Out-Null
    "Build Robocopy Drop $Version - $(Get-Date -Format o)" | Set-Content -LiteralPath $logPath -Encoding UTF8

    if (-not $AcceptWixTerms) {
        if ($NonInteractive) { throw 'Passa -AcceptWixTerms dopo aver verificato NOTICE-WIX.txt e i termini OSMF.' }
        Write-Host ''
        Get-Content -LiteralPath (Join-Path $docsDir 'NOTICE-WIX.txt') | ForEach-Object { Write-Host $_ }
        $answer = Read-Host 'Scrivi ACCETTO dopo aver verificato i termini applicabili alla tua organizzazione'
        if ($answer -cne 'ACCETTO') { throw 'Build annullata: termini WiX non accettati.' }
    }

    Write-Step 'Compilazione del runner Classic aggiornato / Compiling the updated Classic runner'
    $runnerSource = Join-Path $srcDir 'RobocopyDropRunner.cs'
    $runnerOutput = Join-Path $stagingCommon 'RobocopyDropRunner.exe'
    $runnerConfigOutput = Join-Path $stagingCommon 'RobocopyDropRunner.exe.config'
    $assemblyInfo = Join-Path $buildRoot 'RobocopyDrop.AssemblyInfo.cs'
    $runnerSourceText = Get-Content -LiteralPath $runnerSource -Raw

    # Regression guard for the 1.5.2 settings fix. The settings shortcut opens
    # the form with Application.Run, so DialogResult alone is not sufficient:
    # the Cancel button must explicitly call Close().
    if ($runnerSourceText -notmatch 'cancel\.Click\s*\+=\s*CancelClicked\s*;' -or
        $runnerSourceText -notmatch 'private\s+void\s+CancelClicked\s*\(' -or
        $runnerSourceText -notmatch 'DialogResult\s*=\s*DialogResult\.Cancel\s*;' -or
        $runnerSourceText -notmatch 'Close\s*\(\s*\)\s*;') {
        throw 'Fix Annulla impostazioni non rilevato nel sorgente del runner.'
    }
    if ($runnerSourceText -notmatch 'internal\s+static\s+class\s+UpdateManager' -or
        $runnerSourceText -notmatch 'releases/latest' -or
        $runnerSourceText -notmatch 'VerifyDownloadedPackage' -or
        $runnerSourceText -notmatch '--check-updates' -or
        $runnerSourceText -notmatch 'BeginAutomaticUpdateCheck') {
        throw 'Funzioni di aggiornamento GitHub non rilevate nel sorgente del runner.'
    }

    $assemblyVersion = $Version + '.0'
    @"
using System.Reflection;
[assembly: AssemblyTitle("Robocopy Drop")]
[assembly: AssemblyProduct("Robocopy Drop")]
[assembly: AssemblyCompany("$Publisher")]
[assembly: AssemblyCopyright("Copyright (c) $Publisher")]
[assembly: AssemblyVersion("$assemblyVersion")]
[assembly: AssemblyFileVersion("$assemblyVersion")]
[assembly: AssemblyInformationalVersion("$Version GitHub")]
"@ | Set-Content -LiteralPath $assemblyInfo -Encoding UTF8

    $csc = Resolve-CSharpCompiler
    Write-Host ("    csc: " + $csc) -ForegroundColor DarkGray
    Add-Content -LiteralPath $logPath -Value ("CSC: " + $csc) -Encoding UTF8
    Invoke-Checked `
        -FilePath $csc `
        -Arguments @(
            '/nologo',
            '/target:winexe',
            '/platform:x64',
            '/optimize+',
            '/debug-',
            ('/win32icon:' + (Join-Path $assetsDir 'RobocopyDrop.ico')),
            ('/out:' + $runnerOutput),
            '/reference:System.dll',
            '/reference:System.Core.dll',
            '/reference:System.Drawing.dll',
            '/reference:System.Windows.Forms.dll',
            '/reference:System.Management.dll',
            '/reference:System.Configuration.dll',
            '/reference:System.Web.Extensions.dll',
            $runnerSource,
            $assemblyInfo
        ) `
        -Description 'Compilazione RobocopyDropRunner.exe'

    if (-not (Test-Path -LiteralPath $runnerOutput -PathType Leaf)) {
        throw "Runner compilato non trovato: $runnerOutput"
    }
    Assert-X64Pe $runnerOutput
    $compiledVersion = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($runnerOutput).FileVersion
    if ([string]::IsNullOrWhiteSpace($compiledVersion) -or
        -not $compiledVersion.StartsWith($Version, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "FileVersion del runner inattesa: '$compiledVersion'; attesa '$Version.x'."
    }

    $githubUpdate = Get-GitHubUpdateConfiguration
    $configTemplate = Get-Content -LiteralPath (Join-Path $payloadDir 'RobocopyDropRunner.exe.config.template') -Raw
    $configTemplate = $configTemplate.Replace('__GITHUB_OWNER__', (ConvertTo-XmlAttributeValue ([string]$githubUpdate.owner)))
    $configTemplate = $configTemplate.Replace('__GITHUB_REPOSITORY__', (ConvertTo-XmlAttributeValue ([string]$githubUpdate.repository)))
    $configTemplate = $configTemplate.Replace('__GITHUB_API_VERSION__', (ConvertTo-XmlAttributeValue ([string]$githubUpdate.apiVersion)))
    $configTemplate = $configTemplate.Replace('__REQUIRE_SIGNED_UPDATES__', ([bool]$githubUpdate.requireSignedUpdates).ToString().ToLowerInvariant())
    $configTemplate = $configTemplate.Replace('__ALLOWED_SIGNER_THUMBPRINTS__',
        (ConvertTo-XmlAttributeValue ((@($githubUpdate.allowedSignerThumbprints) -join ';'))))
    [xml]$configRoundTrip = $configTemplate
    $configTemplate | Set-Content -LiteralPath $runnerConfigOutput -Encoding UTF8
    if ([string]::IsNullOrWhiteSpace([string]$githubUpdate.owner)) {
        Write-Warning "GitHub owner non configurato: il controllo aggiornamenti resta disattivato. Esegui Configura-GitHub.cmd prima della release pubblica."
    } else {
        Add-Content -LiteralPath $logPath -Value ("UPDATES: https://github.com/" + $githubUpdate.owner + "/" + $githubUpdate.repository) -Encoding UTF8
    }
    Set-Content -LiteralPath (Join-Path $stagingCommon 'Version.txt') -Value $Version -Encoding Ascii

    Copy-Item -LiteralPath (Join-Path $payloadDir 'it\RobocopyDropExtension.dll') -Destination $stagingItDll -Force
    Copy-Item -LiteralPath (Join-Path $payloadDir 'en\RobocopyDropExtension.dll') -Destination $stagingEnDll -Force

    if ((Get-FileHash -LiteralPath $stagingItDll -Algorithm SHA256).Hash -ne $expectedItDll) { throw 'Hash DLL italiana inatteso.' }
    if ((Get-FileHash -LiteralPath $stagingEnDll -Algorithm SHA256).Hash -ne $expectedEnDll) { throw 'Hash DLL inglese inatteso.' }
    Assert-X64Pe $stagingItDll
    Assert-X64Pe $stagingEnDll

    $script:SigningEnabled = -not [string]::IsNullOrWhiteSpace($CertificateThumbprint) -or -not [string]::IsNullOrWhiteSpace($PfxPath)
    $script:SignTool = $null
    if ($script:SigningEnabled) {
        if ($CertificateThumbprint -and $PfxPath) { throw 'Specifica CertificateThumbprint oppure PfxPath, non entrambi.' }
        $script:SignTool = Find-SignTool
        if (-not $script:SignTool) { throw 'signtool.exe non trovato. Installa Windows SDK.' }
        Write-Step 'Firma binari applicativi / Signing application binaries'
        Sign-File (Join-Path $stagingCommon 'RobocopyDropRunner.exe')
        Sign-File $stagingItDll
        Sign-File $stagingEnDll
    } else {
        Write-Warning 'Build non firmata: SmartScreen o policy aziendali possono mostrare avvisi o blocchi.'
    }

    # Run the executable test only after optional Authenticode signing. This
    # avoids testing an unsigned intermediate when the final release is signed.
    try {
        $runnerTest = Start-Process -FilePath $runnerOutput -ArgumentList '--self-test' -Wait -PassThru -WindowStyle Hidden
    }
    catch {
        if (Test-IsApplicationControlBlock -Exception $_.Exception) {
            throw 'Il runner compilato e stato bloccato da App Control/Smart App Control. Firma il binario con un certificato attendibile o usa un PC di build autorizzato.'
        }
        throw
    }
    if ($runnerTest.ExitCode -ne 0) {
        throw "Self-test del runner compilato non riuscito. Codice: $($runnerTest.ExitCode)"
    }
    Add-Content -LiteralPath $logPath -Value ("RUNNER: compiled from source; FileVersion=" + $compiledVersion) -Encoding UTF8

    $resolvedDotnet = @(Resolve-Dotnet8)
    if ($resolvedDotnet.Count -ne 1) {
        throw "Il rilevamento di dotnet ha restituito $($resolvedDotnet.Count) valori invece di un solo percorso. Dettagli: $($resolvedDotnet -join ' | ')"
    }
    $dotnet = [string]$resolvedDotnet[0]
    if ([string]::IsNullOrWhiteSpace($dotnet) -or -not (Test-Path -LiteralPath $dotnet -PathType Leaf)) {
        throw "Percorso dotnet non valido dopo il rilevamento: '$dotnet'"
    }
    Write-Host ("    dotnet: " + $dotnet) -ForegroundColor DarkGray
    Add-Content -LiteralPath $logPath -Value ("DOTNET: " + $dotnet) -Encoding UTF8
    $env:NUGET_PACKAGES = $packagesDir
    $env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
    $env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
    $env:DOTNET_NOLOGO = '1'
    $env:DOTNET_GENERATE_ASPNET_CERTIFICATE = 'false'
    Write-Step 'Ripristino WiX 6.0.2 da NuGet / Restoring WiX 6.0.2 from NuGet'
    Invoke-Checked -FilePath $dotnet -Arguments @('restore',$packageProject,'--configfile',$nugetConfig,'--packages',$packagesDir,'--force','--no-cache') -Description 'Ripristino pacchetti WiX'

    Write-Step 'Build MSI italiano e inglese / Building Italian and English MSI packages'
    $itMsi = Build-Msi -Language 'it' -Dotnet $dotnet
    $enMsi = Build-Msi -Language 'en' -Dotnet $dotnet

    Write-Step 'Preparazione release MSI / Preparing MSI-only release'
    Add-Content -LiteralPath $logPath -Value 'SETUP: omitted (MSI-only classic release)' -Encoding UTF8

    Copy-Item -LiteralPath (Join-Path $docsDir 'GUIDA-IT.pdf') -Destination (Join-Path $releaseDir 'GUIDA-IT.pdf') -Force
    Copy-Item -LiteralPath (Join-Path $docsDir 'GUIDE-EN.pdf') -Destination (Join-Path $releaseDir 'GUIDE-EN.pdf') -Force
    Copy-Item -LiteralPath (Join-Path $docsDir 'DEPLOY-IT.txt') -Destination (Join-Path $releaseDir 'DEPLOY-IT.txt') -Force
    Copy-Item -LiteralPath (Join-Path $docsDir 'NOTICE-WIX.txt') -Destination (Join-Path $releaseDir 'NOTICE-WIX.txt') -Force
    Copy-Item -LiteralPath (Join-Path $root 'LICENSE') -Destination (Join-Path $releaseDir 'LICENSE.txt') -Force
    Copy-Item -LiteralPath (Join-Path $root 'PRIVACY.md') -Destination (Join-Path $releaseDir 'PRIVACY.txt') -Force

    # Create the manifest before SHA256SUMS. The manifest intentionally lists
    # release artifacts but not itself or SHA256SUMS, avoiding recursive hashes.
    $manifestFiles = @(Get-ChildItem -LiteralPath $releaseDir -File | Sort-Object Name | Where-Object {
        $_.Name -notin @('release-manifest.json', 'SHA256SUMS.txt')
    })
    $manifest = [ordered]@{
        product='Robocopy Drop'; version=$Version; builtAt=(Get-Date).ToUniversalTime().ToString('o');
        publisher=$Publisher; runnerSourceBase=$runnerSourceBase; runnerBuild='compiled from source'; signed=$script:SigningEnabled;
        wixVersion='6.0.2'; distribution='MSI-only GitHub release'; setup='not included';
        startMenuEntries=@('settings','guide','reports','updates','uninstall');
        directUninstall='msiexec.exe /x [ProductCode] /norestart';
        updateFeed=[ordered]@{ provider='GitHub Releases'; owner=[string]$githubUpdate.owner; repository=[string]$githubUpdate.repository; automaticCheckHours=24; signedUpdatesRequired=[bool]$githubUpdate.requireSignedUpdates };
        automaticThreads=[ordered]@{ localFixed=32; usbSmallFiles=4; usbLargeFiles=8; network=8; optical=1; fallback=8 };
        files=@($manifestFiles | ForEach-Object {
            [ordered]@{ name=$_.Name; bytes=$_.Length; sha256=(Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant() }
        })
    }
    $manifest | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath (Join-Path $releaseDir 'release-manifest.json') -Encoding UTF8

    # SHA256SUMS covers every final file except SHA256SUMS itself, including
    # release-manifest.json.
    $hashLines = Get-ChildItem -LiteralPath $releaseDir -File | Sort-Object Name | Where-Object {
        $_.Name -ne 'SHA256SUMS.txt'
    } | ForEach-Object {
        '{0}  {1}' -f (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant(), $_.Name
    }
    $hashLines | Set-Content -LiteralPath (Join-Path $releaseDir 'SHA256SUMS.txt') -Encoding Ascii

    Write-Host ''
    Write-Host 'BUILD MSI COMPLETATA / MSI BUILD COMPLETED' -ForegroundColor Green
    Write-Host "Release: $releaseDir"
    Get-ChildItem -LiteralPath $releaseDir -File | Sort-Object Name | ForEach-Object { Write-Host ('  {0,-48} {1,12:N0} byte' -f $_.Name, $_.Length) }
    exit 0
}
catch {
    try { Add-Content -LiteralPath $logPath -Value ("FATAL: " + $_.Exception.ToString()) -Encoding UTF8 } catch { }
    Write-Host ''
    Write-Host 'BUILD NON RIUSCITA / BUILD FAILED' -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-DiagnosticTail
    Write-Host "Log completo / Full log: $logPath" -ForegroundColor Yellow
    exit 1
}
