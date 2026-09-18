[CmdletBinding()]
param(
    [ValidateSet('EditMode', 'PlayMode', 'All')]
    [string]$Mode = 'All',
    [ValidateSet('Auto', 'Live', 'Clean')]
    [string]$Execution = 'Auto',
    [string]$TestFilter,
    [ValidateSet('TestName', 'Assembly', 'Category')]
    [string]$FilterType = 'TestName',
    [ValidateRange(30, 3600)]
    [int]$TimeoutSeconds = 900,
    [string]$ProjectPath,
    [string]$UnityPath
)

. (Join-Path $PSScriptRoot 'UnityTooling.ps1')

if ([string]::IsNullOrWhiteSpace($ProjectPath)) {
    $ProjectPath = Join-Path $PSScriptRoot '..'
}

function Get-LiveTestPayload {
    param(
        [Parameter(Mandatory)]$CommandResult,
        [Parameter(Mandatory)][string]$Platform,
        [Parameter(Mandatory)][string]$Project,
        [Parameter(Mandatory)][int]$Timeout,
        [Parameter(Mandatory)][string]$PollLogPath
    )

    if ($Platform -eq 'EditMode') {
        return $CommandResult.Payload
    }

    $deadline = (Get-Date).AddSeconds($Timeout)
    $lastError = $null
    do {
        try {
            $status = Invoke-UnityLiveCommand -ProjectPath $Project -Command 'test_status' -TimeoutSeconds 30 -LogPath $PollLogPath
            $payload = $status.Payload
            if ($null -ne $payload -and $payload.PSObject.Properties.Name -contains 'status') {
                switch ([string]$payload.status) {
                    'completed' { return $payload }
                    'error' { throw "Live $Platform tests failed to execute: $($payload.message)" }
                    'cancelled' { throw "Live $Platform tests were cancelled." }
                }
            }
            $lastError = $null
        }
        catch {
            # A PlayMode domain reload can briefly drop the Pipeline connection.
            # Keep polling until the bounded test deadline expires.
            $lastError = $_
        }

        Start-Sleep -Seconds 1
    } while ((Get-Date) -lt $deadline)

    $detail = if ($null -eq $lastError) { 'No completed status was returned.' } else { $lastError.Exception.Message }
    throw "Live $Platform tests did not complete within $Timeout seconds. $detail"
}

$project = Resolve-UnityProjectPath -ProjectPath $ProjectPath
$selection = Resolve-UnityExecutionLane -ProjectPath $project -Execution $Execution
$artifactDirectory = New-UnityArtifactDirectory -ArtifactsRoot (Join-Path $project 'artifacts') -Prefix 'unity-tests'
$platforms = if ($Mode -eq 'All') { @('EditMode', 'PlayMode') } else { @($Mode) }
$resultPaths = @()
$summaries = @()
$testFailures = 0

if ($selection.Lane -eq 'Clean' -and -not [string]::IsNullOrWhiteSpace($TestFilter) -and $FilterType -ne 'TestName') {
    throw "Clean execution supports TestName filtering. Use -Execution Live for $FilterType filters."
}

$unity = $null
if ($selection.Lane -eq 'Clean') {
    $unity = Resolve-UnityEditorPath -ProjectPath $project -UnityPath $UnityPath
}

foreach ($platform in $platforms) {
    if ($selection.Lane -eq 'Clean') {
        $resultPath = Join-Path $artifactDirectory "$platform-results.xml"
        $unityLogPath = Join-Path $artifactDirectory "$platform.log"
        $cliLogPath = Join-Path $artifactDirectory "$platform-cli.json"
        $arguments = @(
            'test', $project,
            '--mode', $platform,
            '--output', $resultPath,
            '--report-format', 'nunit',
            '--timeout', [string]$TimeoutSeconds,
            '--editor-path', $unity,
            '--no-banner', '--non-interactive', '--format', 'json'
        )
        if (-not [string]::IsNullOrWhiteSpace($TestFilter)) {
            $arguments += @('--filter', $TestFilter)
        }
        $arguments += @('--', '-nographics', '-logFile', $unityLogPath)

        $invocation = Invoke-UnityCli -Arguments $arguments -LogPath $cliLogPath
        if ($invocation.ExitCode -ne 0 -and $invocation.ExitCode -ne 8) {
            throw "Unity could not execute $platform tests (exit $($invocation.ExitCode)). See '$cliLogPath' and '$unityLogPath'."
        }
        if (-not (Test-Path -LiteralPath $resultPath -PathType Leaf)) {
            throw "Unity completed without writing expected test results to '$resultPath'. See '$cliLogPath'."
        }

        $summary = Get-UnityNUnitSummary -ResultPath $resultPath
        $resultPaths += $resultPath
        $summaries += [pscustomobject]@{
            Platform = $platform
            Total = $summary.Total
            Passed = $summary.Passed
            Failed = $summary.Failed
            Skipped = $summary.Skipped
            Inconclusive = $summary.Inconclusive
            DurationSeconds = $summary.DurationSeconds
            ResultPath = $resultPath
        }
        $testFailures += $summary.Failed
        continue
    }

    $resultPath = Join-Path $artifactDirectory "$platform-results.json"
    $commandLogPath = Join-Path $artifactDirectory "$platform-command.json"
    $pollLogPath = Join-Path $artifactDirectory "$platform-status.json"
    $modeArgument = if ($platform -eq 'EditMode') { 'editor' } else { 'playmode' }
    $commandArguments = @('--mode', $modeArgument)
    if (-not [string]::IsNullOrWhiteSpace($TestFilter)) {
        $commandArguments += @('--filter', $TestFilter, '--filter_type', $FilterType.ToLowerInvariant())
    }
    if ($platform -eq 'PlayMode') {
        $commandArguments += @('--async_tests', 'true')
    }

    $commandResult = Invoke-UnityLiveCommand `
        -ProjectPath $project `
        -Command 'run_tests' `
        -CommandArguments $commandArguments `
        -TimeoutSeconds $TimeoutSeconds `
        -LogPath $commandLogPath
    $payload = Get-LiveTestPayload `
        -CommandResult $commandResult `
        -Platform $platform `
        -Project $project `
        -Timeout $TimeoutSeconds `
        -PollLogPath $pollLogPath
    $payload | ConvertTo-Json -Depth 100 | Set-Content -LiteralPath $resultPath -Encoding utf8

    if ($payload.PSObject.Properties.Name -contains 'success' -and -not [bool]$payload.success) {
        throw "Live $platform tests failed to execute: $($payload.error)"
    }
    if (-not ($payload.PSObject.Properties.Name -contains 'summary')) {
        throw "Live $platform tests returned no summary. See '$resultPath'."
    }

    $summary = $payload.summary
    $resultPaths += $resultPath
    $summaries += [pscustomobject]@{
        Platform = $platform
        Total = [int]$summary.total
        Passed = [int]$summary.passed
        Failed = [int]$summary.failed
        Skipped = [int]$summary.skipped
        Inconclusive = [int]$summary.inconclusive
        DurationSeconds = if ($payload.PSObject.Properties.Name -contains 'duration') { [double]$payload.duration } else { 0 }
        ResultPath = $resultPath
    }
    $testFailures += [int]$summary.failed
}

$runResult = [pscustomobject]@{
    ArtifactDirectory = $artifactDirectory
    Execution = $selection.Lane
    InitialProjectState = $selection.State
    Results = $resultPaths
    Summaries = $summaries
    Failed = $testFailures
}
$runResult

if ($testFailures -gt 0) {
    throw "$testFailures Unity test(s) failed. Both requested test modes completed. Artifacts: '$artifactDirectory'."
}
