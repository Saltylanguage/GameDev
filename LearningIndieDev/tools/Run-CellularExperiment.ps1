[CmdletBinding()]
param(
    [string]$ScenarioPath,
    [int]$SeedStart = 1,
    [ValidateRange(1, 10000)]
    [int]$SeedCount = 20,
    [ValidateRange(0, 4096)]
    [int]$GridWidth = 0,
    [ValidateRange(0, 4096)]
    [int]$GridHeight = 0,
    [ValidateRange(0, 1000000)]
    [double]$RunDurationSeconds = 0,
    [ValidateRange(0, 1000000)]
    [double]$StepIntervalSeconds = 0,
    [string]$PlayerSpeciesId = 'herbivore',
    [string]$UpgradeId = 'none',
    [string]$UpgradeSequence = '',
    [string]$UpgradeAssetSequence = '',
    [string]$UpgradeAssetCatalogPath = '',
    [ValidateRange(0, 1000000)]
    [double]$UpgradeValueOverride = 0,
    [ValidateSet('legacy-fixed-damage', 'opposed-roll')]
    [string]$CombatMode = 'opposed-roll',
    [ValidateSet('natural', 'fixed-rate-diagnostic', 'paired-lockstep-diagnostic')]
    [string]$AttackOpportunityMode = 'natural',
    [string]$ExperimentalFeatures = '',
    [ValidateRange(0, 1000000)]
    [int]$FoxAttackCooldownTicks = 0,
    [ValidateRange(0, 1)]
    [double]$PreContactAvoidanceChance = 0,
    [string]$ProjectPath,
    [string]$UnityPath
)

. (Join-Path $PSScriptRoot 'UnityTooling.ps1')

if ([string]::IsNullOrWhiteSpace($ProjectPath)) {
    $ProjectPath = Join-Path $PSScriptRoot '..'
}

function ConvertTo-UnityAssetPath {
    param(
        [string]$Path,
        [string]$ProjectRoot,
        [string]$ParameterName = 'Path'
    )

    if ([string]::IsNullOrWhiteSpace($Path)) {
        return $null
    }

    $normalizedPath = $Path.Replace('\', '/')
    if ($normalizedPath.StartsWith('Assets/', [System.StringComparison]::Ordinal)) {
        return $normalizedPath
    }

    $absolutePath = (Resolve-Path -LiteralPath (Join-Path $ProjectRoot $Path)).Path
    $assetsPath = (Resolve-Path -LiteralPath (Join-Path $ProjectRoot 'Assets')).Path
    $assetsPrefix = $assetsPath.TrimEnd([System.IO.Path]::DirectorySeparatorChar, [System.IO.Path]::AltDirectorySeparatorChar) + [System.IO.Path]::DirectorySeparatorChar
    if (-not $absolutePath.StartsWith($assetsPrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "$ParameterName must point inside this project's Assets folder."
    }

    return 'Assets/' + $absolutePath.Substring($assetsPrefix.Length).Replace('\', '/')
}

function Get-FileSha256 {
    param([string]$Path)

    $text = [System.IO.File]::ReadAllText($Path, [System.Text.UTF8Encoding]::new($false, $true))
    $canonicalText = $text.Replace("`r`n", "`n").Replace("`r", "`n")
    $bytes = [System.Text.UTF8Encoding]::new($false).GetBytes($canonicalText)
    $sha256 = [System.Security.Cryptography.SHA256]::Create()
    try {
        return ([System.BitConverter]::ToString($sha256.ComputeHash($bytes))).Replace('-', '').ToLowerInvariant()
    }
    finally {
        $sha256.Dispose()
    }
}

function Get-SourceState {
    param([string]$ProjectPath)

    $commit = ''
    $dirty = $false
    try {
        $repositoryRoot = (& git -C $ProjectPath rev-parse --show-toplevel 2>$null | Select-Object -First 1)
        if (-not [string]::IsNullOrWhiteSpace([string]$repositoryRoot)) {
            $repositoryRoot = $repositoryRoot.Trim()
            $commitValue = (& git -C $repositoryRoot rev-parse HEAD 2>$null | Select-Object -First 1)
            if ($null -ne $commitValue) {
                $commit = $commitValue.Trim()
            }

            $dirty = @(& git -C $repositoryRoot status --porcelain --untracked-files=no 2>$null).Count -gt 0
        }
    }
    catch {
        # A report may still be useful outside a Git checkout; leave provenance
        # blank rather than inventing a source identity.
    }

    return [pscustomobject]@{
        Commit = $commit
        Dirty = $dirty
    }
}

$project = Resolve-UnityProjectPath -ProjectPath $ProjectPath
$unity = Resolve-UnityEditorPath -ProjectPath $project -UnityPath $UnityPath
$sourceStateBeforeRun = Get-SourceState -ProjectPath $project
$preflight = Invoke-UnityPreflight -ProjectPath $project -UnityPath $unity -ArtifactsRoot (Join-Path $project 'artifacts')
$assetPath = ConvertTo-UnityAssetPath -Path $ScenarioPath -ProjectRoot $project
$artifactDirectory = New-UnityArtifactDirectory -ArtifactsRoot (Join-Path $project 'artifacts') -Prefix 'cellular-experiment'
$reportPath = Join-Path $artifactDirectory 'report.json'
$logPath = Join-Path $artifactDirectory 'unity.log'
$manifestPath = Join-Path $artifactDirectory 'manifest.json'

$arguments = @(
    '-batchmode',
    '-nographics',
    '-projectPath', $project,
    '-executeMethod', 'SaltyGame.EditorTools.CellularSimulationExperimentRunner.RunFromCommandLine',
    '-seedStart', $SeedStart,
    '-seedCount', $SeedCount,
    '-playerSpeciesId', $PlayerSpeciesId,
    '-upgradeId', $UpgradeId,
    '-combatMode', $CombatMode,
    '-attackOpportunityMode', $AttackOpportunityMode,
    '-outputPath', $reportPath,
    '-logFile', $logPath
)

if ($UpgradeValueOverride -gt 0) {
    $arguments += @('-upgradeValueOverride', $UpgradeValueOverride.ToString([Globalization.CultureInfo]::InvariantCulture))
}

if (-not [string]::IsNullOrWhiteSpace($UpgradeSequence)) {
    if ($UpgradeId -ne 'none') {
        throw 'Use either -UpgradeId or -UpgradeSequence, not both.'
    }

    $arguments += @('-upgradeSequence', $UpgradeSequence)
}

if (-not [string]::IsNullOrWhiteSpace($UpgradeAssetSequence)) {
    if ($UpgradeId -ne 'none' -or -not [string]::IsNullOrWhiteSpace($UpgradeSequence)) {
        throw 'Use either -UpgradeAssetSequence or the legacy -UpgradeId/-UpgradeSequence arguments, not both.'
    }

    $arguments += @('-upgradeAssetSequence', $UpgradeAssetSequence)

    if (-not [string]::IsNullOrWhiteSpace($UpgradeAssetCatalogPath)) {
        $catalogPath = ConvertTo-UnityAssetPath -Path $UpgradeAssetCatalogPath -ProjectRoot $project -ParameterName 'UpgradeAssetCatalogPath'
        $arguments += @('-upgradeAssetCatalogPath', $catalogPath)
    }
}
elseif (-not [string]::IsNullOrWhiteSpace($UpgradeAssetCatalogPath)) {
    throw 'Use -UpgradeAssetCatalogPath together with -UpgradeAssetSequence.'
}

if ($null -ne $assetPath) {
    $arguments += @('-scenarioPath', $assetPath)
}

if ($GridWidth -gt 0) {
    $arguments += @('-gridWidth', $GridWidth)
}

if ($GridHeight -gt 0) {
    $arguments += @('-gridHeight', $GridHeight)
}

if ($RunDurationSeconds -gt 0) {
    $arguments += @('-runDurationSeconds', $RunDurationSeconds.ToString([Globalization.CultureInfo]::InvariantCulture))
}

if ($StepIntervalSeconds -gt 0) {
    $arguments += @('-stepIntervalSeconds', $StepIntervalSeconds.ToString([Globalization.CultureInfo]::InvariantCulture))
}

if (-not [string]::IsNullOrWhiteSpace($ExperimentalFeatures)) {
    $arguments += @('-experimentalFeatures', $ExperimentalFeatures)
}

if ($FoxAttackCooldownTicks -gt 0) {
    $arguments += @('-foxAttackCooldownTicks', $FoxAttackCooldownTicks)
}

if ($PreContactAvoidanceChance -gt 0) {
    $arguments += @('-preContactAvoidanceChance', $PreContactAvoidanceChance.ToString([Globalization.CultureInfo]::InvariantCulture))
}

Invoke-UnityBatch -UnityPath $unity -Arguments $arguments
if (-not (Test-Path -LiteralPath $reportPath -PathType Leaf)) {
    throw "Unity completed without writing expected report to '$reportPath'."
}

$scenarioGuid = ''
if ($null -ne $assetPath) {
    $scenarioMetaPath = Join-Path $project ($assetPath.Replace('/', [System.IO.Path]::DirectorySeparatorChar) + '.meta')
    if (Test-Path -LiteralPath $scenarioMetaPath -PathType Leaf) {
        $guidLine = Select-String -LiteralPath $scenarioMetaPath -Pattern '^guid:\s*(\S+)\s*$' | Select-Object -First 1
        if ($null -ne $guidLine) {
            $scenarioGuid = $guidLine.Matches[0].Groups[1].Value
        }
    }
}

$sourceStateAfterRun = Get-SourceState -ProjectPath $project
$manifest = [ordered]@{
    schemaVersion = 1
    createdUtc = [DateTime]::UtcNow.ToString('O')
    reportFile = [System.IO.Path]::GetFileName($reportPath)
    reportSha256 = Get-FileSha256 -Path $reportPath
    sourceCommit = $sourceStateBeforeRun.Commit
    sourceTreeDirty = $sourceStateBeforeRun.Dirty
    sourceTreeDirtyBeforeRun = $sourceStateBeforeRun.Dirty
    sourceTreeDirtyAfterRun = $sourceStateAfterRun.Dirty
    scenarioAssetPath = if ($null -eq $assetPath) { '' } else { $assetPath }
    scenarioAssetGuid = $scenarioGuid
    unityExecutable = $unity
    unityArguments = $arguments
}
$manifest | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $manifestPath -Encoding utf8

[pscustomobject]@{
    ArtifactDirectory = $artifactDirectory
    Manifest = $manifestPath
    Preflight = $preflight
    Report = $reportPath
    UnityLog = $logPath
}
