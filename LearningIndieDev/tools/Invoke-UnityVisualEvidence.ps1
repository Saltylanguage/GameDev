[CmdletBinding()]
param(
    [ValidateSet('Auto', 'Live', 'Clean')]
    [string]$Execution = 'Auto',
    [string]$ProjectPath,
    [string]$UnityPath,
    [string]$TestFilter,
    [string]$ReplayReportPath,
    [int]$ReplaySeed = -1,
    [ValidateRange(320, 7680)]
    [int]$ScreenWidth = 1280,
    [ValidateRange(240, 4320)]
    [int]$ScreenHeight = 720,
    [ValidateRange(30, 3600)]
    [int]$TimeoutSeconds = 900
)

. (Join-Path $PSScriptRoot 'UnityTooling.ps1')

if ([string]::IsNullOrWhiteSpace($ProjectPath)) {
    $ProjectPath = Join-Path $PSScriptRoot '..'
}

$project = Resolve-UnityProjectPath -ProjectPath $ProjectPath
$selection = Resolve-UnityExecutionLane -ProjectPath $project -Execution $Execution
$artifactDirectory = New-UnityArtifactDirectory -ArtifactsRoot (Join-Path $project 'artifacts') -Prefix 'visual-evidence'
$requestPath = Join-Path $project 'Temp/cellsim_visual_request.json'
$requestValues = @(
    [ordered]@{ name = 'CELLSIM_VISUAL_OUTPUT'; value = $artifactDirectory },
    [ordered]@{ name = 'CELLSIM_VISUAL_WIDTH'; value = [string]$ScreenWidth },
    [ordered]@{ name = 'CELLSIM_VISUAL_HEIGHT'; value = [string]$ScreenHeight }
)

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

    $requestValues += @(
        [ordered]@{ name = 'CELLSIM_REPLAY_SCENARIO'; value = [string]$replayReport.scenarioAssetPath },
        [ordered]@{ name = 'CELLSIM_REPLAY_PLAYER_SPECIES_ID'; value = [string]$replayReport.playerSpeciesId },
        [ordered]@{ name = 'CELLSIM_REPLAY_SEED'; value = [string]$ReplaySeed },
        [ordered]@{ name = 'CELLSIM_REPLAY_GRID_WIDTH'; value = [string]$replayReport.gridWidth },
        [ordered]@{ name = 'CELLSIM_REPLAY_GRID_HEIGHT'; value = [string]$replayReport.gridHeight }
    )
    [pscustomobject]@{
        sourceReport = $resolvedReportPath
        seed = $ReplaySeed
        scenarioAssetPath = $replayReport.scenarioAssetPath
        playerSpeciesId = $replayReport.playerSpeciesId
        rulesetFingerprint = $replayReport.rulesetFingerprint
        gridWidth = $replayReport.gridWidth
        gridHeight = $replayReport.gridHeight
        sourceRun = $selectedRun
    } | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath (Join-Path $artifactDirectory 'replay-manifest.json') -Encoding utf8
}

New-Item -ItemType Directory -Path (Split-Path -Parent $requestPath) -Force | Out-Null
[ordered]@{ schemaVersion = 1; values = $requestValues } |
    ConvertTo-Json -Depth 5 |
    Set-Content -LiteralPath $requestPath -Encoding utf8

$resultPath = $null
$summary = $null
try {
    if ($selection.Lane -eq 'Clean') {
        $unity = Resolve-UnityEditorPath -ProjectPath $project -UnityPath $UnityPath
        $resultPath = Join-Path $artifactDirectory 'PlayMode-results.xml'
        $unityLogPath = Join-Path $artifactDirectory 'PlayMode.log'
        $cliLogPath = Join-Path $artifactDirectory 'PlayMode-cli.json'
        $arguments = @(
            'test', $project,
            '--mode', 'PlayMode',
            '--output', $resultPath,
            '--report-format', 'nunit',
            '--timeout', [string]$TimeoutSeconds,
            '--editor-path', $unity,
            '--no-banner', '--non-interactive', '--format', 'json'
        )
        if (-not [string]::IsNullOrWhiteSpace($TestFilter)) {
            $arguments += @('--filter', $TestFilter)
        }
        $arguments += @('--', '-logFile', $unityLogPath, '-screen-width', [string]$ScreenWidth, '-screen-height', [string]$ScreenHeight, '-screen-fullscreen', '0')
        $invocation = Invoke-UnityCli -Arguments $arguments -LogPath $cliLogPath
        if ($invocation.ExitCode -ne 0 -and $invocation.ExitCode -ne 8) {
            throw "Unity could not execute the visual PlayMode tests (exit $($invocation.ExitCode)). See '$cliLogPath'."
        }
        if (-not (Test-Path -LiteralPath $resultPath -PathType Leaf)) {
            throw "Unity completed without writing expected test results to '$resultPath'."
        }
        $summary = Get-UnityNUnitSummary -ResultPath $resultPath
    }
    else {
        $resultPath = Join-Path $artifactDirectory 'PlayMode-results.json'
        $startLogPath = Join-Path $artifactDirectory 'PlayMode-command.json'
        $statusLogPath = Join-Path $artifactDirectory 'PlayMode-status.json'
        $commandArguments = @('--mode', 'playmode', '--async_tests', 'true')
        if (-not [string]::IsNullOrWhiteSpace($TestFilter)) {
            $commandArguments += @('--filter', $TestFilter, '--filter_type', 'testname')
        }
        Invoke-UnityLiveCommand -ProjectPath $project -Command 'run_tests' -CommandArguments $commandArguments -TimeoutSeconds $TimeoutSeconds -LogPath $startLogPath | Out-Null

        $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
        $payload = $null
        do {
            try {
                $status = Invoke-UnityLiveCommand -ProjectPath $project -Command 'test_status' -TimeoutSeconds 30 -LogPath $statusLogPath
                if ($null -ne $status.Payload -and $status.Payload.PSObject.Properties.Name -contains 'status') {
                    if ([string]$status.Payload.status -eq 'completed') { $payload = $status.Payload; break }
                    if ([string]$status.Payload.status -eq 'error') { throw "Visual tests failed to execute: $($status.Payload.message)" }
                }
            }
            catch {
                if ((Get-Date) -ge $deadline) { throw }
            }
            Start-Sleep -Seconds 1
        } while ((Get-Date) -lt $deadline)
        if ($null -eq $payload) { throw "Visual tests did not complete within $TimeoutSeconds seconds." }
        $payload | ConvertTo-Json -Depth 100 | Set-Content -LiteralPath $resultPath -Encoding utf8
        $summary = [pscustomobject]@{
            Total = [int]$payload.summary.total
            Passed = [int]$payload.summary.passed
            Failed = [int]$payload.summary.failed
            Skipped = [int]$payload.summary.skipped
            Inconclusive = [int]$payload.summary.inconclusive
            DurationSeconds = [double]$payload.duration
        }
    }
}
finally {
    Remove-Item -LiteralPath $requestPath -Force -ErrorAction SilentlyContinue
}

$screenshots = @(Get-ChildItem -LiteralPath $artifactDirectory -Filter '*.png' -File | Select-Object -ExpandProperty FullName)
$runResult = [pscustomobject]@{
    ArtifactDirectory = $artifactDirectory
    Execution = $selection.Lane
    InitialProjectState = $selection.State
    Results = $resultPath
    Summary = $summary
    Screenshots = $screenshots
}
$runResult
if ($summary.Failed -gt 0) {
    throw "$($summary.Failed) visual PlayMode test(s) failed. Artifacts: '$artifactDirectory'."
}
