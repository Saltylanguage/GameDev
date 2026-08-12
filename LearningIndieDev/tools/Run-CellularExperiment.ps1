[CmdletBinding()]
param(
    [string]$ScenarioPath,
    [ValidateRange(0, 10000)]
    [int]$SeedStart = 1,
    [ValidateRange(1, 10000)]
    [int]$SeedCount = 20,
    [string]$PlayerSpeciesId = 'herbivore',
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
        [string]$ProjectRoot
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
        throw "ScenarioPath must point inside this project's Assets folder."
    }

    return 'Assets/' + $absolutePath.Substring($assetsPrefix.Length).Replace('\', '/')
}

$project = Resolve-UnityProjectPath -ProjectPath $ProjectPath
Assert-UnityProjectNotOpen -ProjectPath $project
$unity = Resolve-UnityEditorPath -ProjectPath $project -UnityPath $UnityPath
$assetPath = ConvertTo-UnityAssetPath -Path $ScenarioPath -ProjectRoot $project
$artifactDirectory = New-UnityArtifactDirectory -ArtifactsRoot (Join-Path $project 'artifacts') -Prefix 'cellular-experiment'
$reportPath = Join-Path $artifactDirectory 'report.json'
$logPath = Join-Path $artifactDirectory 'unity.log'

$arguments = @(
    '-batchmode',
    '-nographics',
    '-projectPath', $project,
    '-executeMethod', 'SaltyGame.EditorTools.CellularSimulationExperimentRunner.RunFromCommandLine',
    '-seedStart', $SeedStart,
    '-seedCount', $SeedCount,
    '-playerSpeciesId', $PlayerSpeciesId,
    '-outputPath', $reportPath,
    '-logFile', $logPath
)

if ($null -ne $assetPath) {
    $arguments += @('-scenarioPath', $assetPath)
}

Invoke-UnityBatch -UnityPath $unity -Arguments $arguments
if (-not (Test-Path -LiteralPath $reportPath -PathType Leaf)) {
    throw "Unity completed without writing expected report to '$reportPath'."
}

[pscustomobject]@{
    ArtifactDirectory = $artifactDirectory
    Report = $reportPath
    UnityLog = $logPath
}
