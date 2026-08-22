[CmdletBinding()]
param(
    [string]$ProjectPath,
    [string]$UnityPath,
    [string]$TestFilter,
    [string]$ReplayReportPath,
    [int]$ReplaySeed = -1
)

. (Join-Path $PSScriptRoot 'UnityTooling.ps1')

if ([string]::IsNullOrWhiteSpace($ProjectPath)) {
    $ProjectPath = Join-Path $PSScriptRoot '..'
}

$project = Resolve-UnityProjectPath -ProjectPath $ProjectPath
$unity = Resolve-UnityEditorPath -ProjectPath $project -UnityPath $UnityPath
$preflight = Invoke-UnityPreflight -ProjectPath $project -UnityPath $unity -ArtifactsRoot (Join-Path $project 'artifacts')
$artifactDirectory = New-UnityArtifactDirectory -ArtifactsRoot (Join-Path $project 'artifacts') -Prefix 'visual-evidence'
$resultPath = Join-Path $artifactDirectory 'PlayMode-results.xml'
$logPath = Join-Path $artifactDirectory 'PlayMode.log'
$previousEnvironment = @{}
foreach ($name in @('CELLSIM_VISUAL_OUTPUT', 'CELLSIM_REPLAY_SCENARIO', 'CELLSIM_REPLAY_PLAYER_SPECIES_ID', 'CELLSIM_REPLAY_SEED', 'CELLSIM_REPLAY_GRID_WIDTH', 'CELLSIM_REPLAY_GRID_HEIGHT')) {
    $previousEnvironment[$name] = [Environment]::GetEnvironmentVariable($name)
}

$env:CELLSIM_VISUAL_OUTPUT = $artifactDirectory
if (-not [string]::IsNullOrWhiteSpace($ReplayReportPath)) {
    if ($ReplaySeed -lt 0) {
        throw 'Replay visuals require -ReplaySeed.'
    }

    $resolvedReportPath = (Resolve-Path -LiteralPath $ReplayReportPath).Path
    $replayReport = Get-Content -LiteralPath $resolvedReportPath -Raw | ConvertFrom-Json
    $selectedRun = @($replayReport.runs | Where-Object { $_.seed -eq $ReplaySeed }) | Select-Object -First 1
    if ($null -eq $selectedRun) {
        throw "Seed $ReplaySeed was not found in replay report '$resolvedReportPath'."
    }

    foreach ($required in @('scenarioAssetPath', 'playerSpeciesId', 'gridWidth', 'gridHeight', 'rulesetFingerprint')) {
        if ([string]::IsNullOrWhiteSpace([string]$replayReport.$required)) {
            throw "Replay report '$resolvedReportPath' is missing '$required'."
        }
    }

    $env:CELLSIM_REPLAY_SCENARIO = $replayReport.scenarioAssetPath
    $env:CELLSIM_REPLAY_PLAYER_SPECIES_ID = $replayReport.playerSpeciesId
    $env:CELLSIM_REPLAY_SEED = [string]$ReplaySeed
    $env:CELLSIM_REPLAY_GRID_WIDTH = [string]$replayReport.gridWidth
    $env:CELLSIM_REPLAY_GRID_HEIGHT = [string]$replayReport.gridHeight
    [pscustomobject]@{
        sourceReport = $resolvedReportPath
        seed = $ReplaySeed
        scenarioAssetPath = $replayReport.scenarioAssetPath
        playerSpeciesId = $replayReport.playerSpeciesId
        rulesetFingerprint = $replayReport.rulesetFingerprint
        gridWidth = $replayReport.gridWidth
        gridHeight = $replayReport.gridHeight
        sourceRun = [pscustomobject]@{
            seed = $selectedRun.seed
            ticks = $selectedRun.ticks
            durationSeconds = $selectedRun.durationSeconds
            playerPopulation = $selectedRun.playerPopulation
            currencyEarned = $selectedRun.currencyEarned
        }
    } | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath (Join-Path $artifactDirectory 'replay-manifest.json') -Encoding utf8
}
$unityArguments = @(
    '-batchmode',
    '-projectPath', $project,
    '-runTests',
    '-testPlatform', 'PlayMode',
    '-testResults', $resultPath,
    '-logFile', $logPath,
    '-screen-width', '1280',
    '-screen-height', '720',
    '-screen-fullscreen', '0'
)
if (-not [string]::IsNullOrWhiteSpace($TestFilter)) {
    $unityArguments += @('-testFilter', $TestFilter)
}

try {
    Invoke-UnityBatch -UnityPath $unity -Arguments $unityArguments
}
finally {
    foreach ($name in $previousEnvironment.Keys) {
        if ($null -eq $previousEnvironment[$name]) {
            Remove-Item "Env:$name" -ErrorAction SilentlyContinue
        }
        else {
            [Environment]::SetEnvironmentVariable($name, $previousEnvironment[$name])
        }
    }
}

if (-not (Test-Path -LiteralPath $resultPath -PathType Leaf)) {
    throw "Unity completed without writing expected test results to '$resultPath'."
}

[pscustomobject]@{
    ArtifactDirectory = $artifactDirectory
    Preflight = $preflight
    Results = $resultPath
    Screenshots = @(Get-ChildItem -LiteralPath $artifactDirectory -Filter '*.png' -File | Select-Object -ExpandProperty FullName)
}
