[CmdletBinding()]
param(
    [string]$QueueRoot = (Join-Path $PSScriptRoot '..\automation\CellSimQueue'),
    [int]$PollSeconds = 10,
    [switch]$Once,
    [switch]$AutoSync,
    [switch]$AutoPublish
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$project = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$pending = Join-Path $QueueRoot 'Pending'
$running = Join-Path $QueueRoot 'Running'
$completed = Join-Path $QueueRoot 'Completed'
$failed = Join-Path $QueueRoot 'Failed'
$runtimeRoot = Join-Path ([IO.Path]::GetTempPath()) 'SaltyGame-CellSimWorker'
$generatedUnityPaths = @(
    'LearningIndieDev/Assets/UI/DesignSystem/FigmaNoesisPilot.xaml.meta',
    'LearningIndieDev/Assets/UI/DesignSystem/FigmaNoesisPilotResources.xaml.meta',
    'LearningIndieDev/LearningIndieDev.slnx',
    'LearningIndieDev/ProjectSettings/EditorBuildSettings.asset',
    'LearningIndieDev/ProjectSettings/Packages/com.unity.probuilder/Settings.json',
    'LearningIndieDev/ProjectSettings/ShaderGraphSettings.asset'
)
foreach ($directory in @($pending, $running, $completed, $failed)) {
    New-Item -ItemType Directory -Path $directory -Force | Out-Null
}
New-Item -ItemType Directory -Path $runtimeRoot -Force | Out-Null

function Get-WorkerStatus {
    @(git -C (Split-Path $project -Parent) status --short --untracked-files=all 2>$null)
}

function Assert-WorkerClean([string]$Phase) {
    $status = @(Get-WorkerStatus)
    if ($status.Count -gt 0) {
        throw "Worker checkout is not clean before ${Phase}:`n$($status -join [Environment]::NewLine)"
    }
}

function Invoke-WorkerGit([string[]]$Arguments) {
    & git -C (Split-Path $project -Parent) @Arguments
    if ($LASTEXITCODE -ne 0) { throw "Git command failed: git $($Arguments -join ' ')" }
}

function Sync-WorkerRepository {
    Assert-WorkerClean 'Git synchronization'
    Invoke-WorkerGit @('pull', '--ff-only')
    Assert-WorkerClean 'job discovery'
}

function Restore-UnityGeneratedChanges {
    foreach ($relativePath in $generatedUnityPaths) {
        $absolutePath = Join-Path (Split-Path $project -Parent) $relativePath
        if ((Get-WorkerStatus) -contains (" M " + $relativePath)) {
            & git -C (Split-Path $project -Parent) restore --source=HEAD -- $relativePath
            if ($LASTEXITCODE -ne 0) { throw "Could not restore Unity-generated path '$absolutePath'." }
        }
    }
}

function Get-FileSha256([string]$Path) {
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

function ConvertTo-CellSimCsvScalar {
    param([object]$Value)

    if ($null -eq $Value) { return '' }
    if ($Value -is [bool]) { return $Value.ToString().ToLowerInvariant() }
    if ($Value -is [System.IFormattable]) {
        return $Value.ToString($null, [Globalization.CultureInfo]::InvariantCulture)
    }

    return [string]$Value
}

function ConvertTo-CellSimCsvField {
    param([object]$Value)

    $text = ConvertTo-CellSimCsvScalar $Value
    return '"' + $text.Replace('"', '""') + '"'
}

function Get-StatLineMetricCsvValue {
    param(
        [object]$Stat,
        [string]$ValueProperty,
        [string]$StatusProperty
    )

    $status = $Stat.PSObject.Properties[$StatusProperty]
    if ($null -ne $status -and -not [string]::IsNullOrWhiteSpace([string]$status.Value) -and
        [string]$status.Value -ne 'Valid') {
        return [string]$status.Value
    }

    $value = $Stat.PSObject.Properties[$ValueProperty]
    if ($null -eq $value -or $null -eq $value.Value) { return 'N/A' }
    return ConvertTo-CellSimCsvScalar $value.Value
}

function Get-StatLinePropertyValue {
    param(
        [object]$Stat,
        [string]$PropertyName,
        [object]$Default = 'N/A'
    )

    $property = $Stat.PSObject.Properties[$PropertyName]
    if ($null -eq $property -or $null -eq $property.Value) { return $Default }
    return $property.Value
}

function Get-StatLineStatusCsvValue {
    param(
        [object]$Stat,
        [string]$StatusProperty
    )

    $status = $Stat.PSObject.Properties[$StatusProperty]
    if ($null -eq $status -or [string]::IsNullOrWhiteSpace([string]$status.Value)) { return 'N/A' }
    return [string]$status.Value
}

function Export-HerbivoreStatLineCsv {
    param(
        [Parameter(Mandatory)] [string]$ReportPath,
        [Parameter(Mandatory)] [string]$OutputPath
    )

    $report = Get-Content -LiteralPath $ReportPath -Raw | ConvertFrom-Json
    $headers = @(
        'seed', 'speciesId', 'SPO', 'HPS', 'EHS', 'ECN', 'PREY', 'STRV', 'MAT', 'BIR', 'CRWD',
        'FPO', 'expectedFPO', 'fpoReconciled',
        'pAVI', 'pAVIStatus', 'eAVI', 'eAVIStatus', 'predAVG', 'predAVGStatus',
        'sAVI', 'sAVIStatus', 'cAVI', 'cAVIStatus', 'bAVG', 'bAVGStatus', 'RFS', 'RFSStatus',
        'APS', 'APSStatus'
    )
    $lines = [System.Collections.Generic.List[string]]::new()
    $lines.Add((@($headers | ForEach-Object { ConvertTo-CellSimCsvField $_ }) -join ','))
    $rowCount = 0

    foreach ($run in @($report.runs | Sort-Object seed)) {
        $statProperty = $run.PSObject.Properties['herbivoreStatLine']
        if ($null -eq $statProperty -or $null -eq $statProperty.Value) { continue }

        $stat = $statProperty.Value
        $values = @(
            $run.seed,
            (Get-StatLinePropertyValue -Stat $stat -PropertyName 'speciesId' -Default ''),
            (Get-StatLinePropertyValue -Stat $stat -PropertyName 'SPO'),
            (Get-StatLinePropertyValue -Stat $stat -PropertyName 'HPS'),
            (Get-StatLinePropertyValue -Stat $stat -PropertyName 'EHS'),
            (Get-StatLinePropertyValue -Stat $stat -PropertyName 'ECN'),
            (Get-StatLinePropertyValue -Stat $stat -PropertyName 'PREY'),
            (Get-StatLinePropertyValue -Stat $stat -PropertyName 'STRV'),
            (Get-StatLinePropertyValue -Stat $stat -PropertyName 'MAT'),
            (Get-StatLinePropertyValue -Stat $stat -PropertyName 'BIR'),
            (Get-StatLinePropertyValue -Stat $stat -PropertyName 'CRWD' -Default 0),
            (Get-StatLinePropertyValue -Stat $stat -PropertyName 'FPO'),
            (Get-StatLinePropertyValue -Stat $stat -PropertyName 'expectedFPO'),
            (Get-StatLinePropertyValue -Stat $stat -PropertyName 'fpoReconciled'),
            (Get-StatLineMetricCsvValue -Stat $stat -ValueProperty 'pAVI' -StatusProperty 'pAVIStatus'),
            (Get-StatLineStatusCsvValue -Stat $stat -StatusProperty 'pAVIStatus'),
            (Get-StatLineMetricCsvValue -Stat $stat -ValueProperty 'eAVI' -StatusProperty 'eAVIStatus'),
            (Get-StatLineStatusCsvValue -Stat $stat -StatusProperty 'eAVIStatus'),
            (Get-StatLineMetricCsvValue -Stat $stat -ValueProperty 'predAVG' -StatusProperty 'predAVGStatus'),
            (Get-StatLineStatusCsvValue -Stat $stat -StatusProperty 'predAVGStatus'),
            (Get-StatLineMetricCsvValue -Stat $stat -ValueProperty 'sAVI' -StatusProperty 'sAVIStatus'),
            (Get-StatLineStatusCsvValue -Stat $stat -StatusProperty 'sAVIStatus'),
            (Get-StatLineMetricCsvValue -Stat $stat -ValueProperty 'cAVI' -StatusProperty 'cAVIStatus'),
            (Get-StatLineStatusCsvValue -Stat $stat -StatusProperty 'cAVIStatus'),
            (Get-StatLineMetricCsvValue -Stat $stat -ValueProperty 'bAVG' -StatusProperty 'bAVGStatus'),
            (Get-StatLineStatusCsvValue -Stat $stat -StatusProperty 'bAVGStatus'),
            (Get-StatLineMetricCsvValue -Stat $stat -ValueProperty 'RFS' -StatusProperty 'RFSStatus'),
            (Get-StatLineStatusCsvValue -Stat $stat -StatusProperty 'RFSStatus'),
            (Get-StatLineMetricCsvValue -Stat $stat -ValueProperty 'APS' -StatusProperty 'APSStatus'),
            (Get-StatLineStatusCsvValue -Stat $stat -StatusProperty 'APSStatus')
        )
        $lines.Add((@($values | ForEach-Object { ConvertTo-CellSimCsvField $_ }) -join ','))
        $rowCount++
    }

    $outputDirectory = Split-Path -Parent $OutputPath
    New-Item -ItemType Directory -Path $outputDirectory -Force | Out-Null
    Set-Content -LiteralPath $OutputPath -Value $lines -Encoding utf8
    return $rowCount
}

function Publish-WorkerResults {
    $unexpected = @(Get-WorkerStatus | Where-Object { $_ -notmatch 'LearningIndieDev/automation/CellSimQueue/' })
    if ($unexpected.Count -gt 0) {
        throw "Unexpected non-queue changes prevent result publication:`n$($unexpected -join [Environment]::NewLine)"
    }
    Invoke-WorkerGit @('add', '--', 'LearningIndieDev/automation/CellSimQueue')
    $staged = @(git -C (Split-Path $project -Parent) diff --cached --name-only)
    if ($staged.Count -eq 0) { return }
    Invoke-WorkerGit @('commit', '-m', 'Publish remote CellSim worker result')
    # The worker commonly runs from a detached temporary worktree. Push the
    # checked-out commit explicitly instead of relying on a local branch ref.
    Invoke-WorkerGit @('push', 'origin', 'HEAD:refs/heads/codex/cellsim-worker')
}

function Update-JobFile([string]$Path, [hashtable]$Changes) {
    $job = Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
    foreach ($key in $Changes.Keys) { $job | Add-Member -NotePropertyName $key -NotePropertyValue $Changes[$key] -Force }
    $job | ConvertTo-Json -Depth 12 | Set-Content -LiteralPath $Path -Encoding utf8
}

function Invoke-Job([string]$Path, [string]$PendingPath) {
    $job = Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
    $parameters = @{}
    foreach ($property in $job.parameters.psobject.Properties) {
        if ($null -ne $property.Value -and -not ($property.Value -is [string] -and [string]::IsNullOrWhiteSpace($property.Value))) {
            $parameters[$property.Name] = $property.Value
        }
    }
    $parameters.ProjectPath = $project
    $parameters.PlayerSpeciesId = [string]$parameters.PlayerSpeciesId
    $parameters.SeedStart = [int]$parameters.SeedStart
    $parameters.SeedCount = [int]$parameters.SeedCount
    $workerRoot = Split-Path $project -Parent
    $sourceCommit = (& git -C $workerRoot rev-parse HEAD).Trim()
    $sourceStatusBefore = @(Get-WorkerStatus)
    $started = [DateTime]::UtcNow
    Update-JobFile $Path @{ status = 'running'; startedUtc = $started.ToString('O'); worker = $env:COMPUTERNAME }
    try {
        $result = & (Join-Path $project 'tools\Run-CellularExperiment.ps1') @parameters
        Restore-UnityGeneratedChanges
        $cleanAfterCleanup = (@(Get-WorkerStatus)).Count -eq 0
        Assert-WorkerClean 'result packaging'
        $reportCsvPath = [IO.Path]::ChangeExtension($result.Report, '.csv')
        if (-not (Test-Path -LiteralPath $reportCsvPath -PathType Leaf)) {
            throw "Unity completed without writing expected CSV to '$reportCsvPath'."
        }
        $statLineCsvPath = [string]$result.StatLine
        if ([string]::IsNullOrWhiteSpace($statLineCsvPath) -or
            -not (Test-Path -LiteralPath $statLineCsvPath -PathType Leaf)) {
            $statLineCsvPath = Join-Path $result.ArtifactDirectory 'statline.csv'
            $statLineCount = Export-HerbivoreStatLineCsv -ReportPath $result.Report -OutputPath $statLineCsvPath
        }
        else {
            $statLineCount = @(Import-Csv -LiteralPath $statLineCsvPath).Count
        }
        if ($parameters.ContainsKey('ExperimentalFeatures') -and
            [string]$parameters.ExperimentalFeatures -eq 'bev-experimental' -and
            $statLineCount -ne $parameters.SeedCount) {
            throw "Expected one Hare stat-line row per seed, found $statLineCount of $($parameters.SeedCount)."
        }
        if (-not (Test-Path -LiteralPath $result.UnityLog -PathType Leaf)) {
            throw "Unity completed without writing expected log to '$($result.UnityLog)'."
        }
        $manifest = Get-Content -LiteralPath $result.Manifest -Raw | ConvertFrom-Json
        $manifestReportHash = [string]$manifest.reportSha256
        if ([string]::IsNullOrWhiteSpace($manifestReportHash) -or
            $manifestReportHash.ToLowerInvariant() -ne (Get-FileSha256 -Path $result.Report)) {
            throw "Manifest reportSha256 does not match '$($result.Report)'."
        }
        $resultDirectory = Join-Path $completed $job.jobId
        New-Item -ItemType Directory -Path $resultDirectory -Force | Out-Null
        Copy-Item -LiteralPath $result.Report -Destination (Join-Path $resultDirectory 'report.json')
        Copy-Item -LiteralPath $reportCsvPath -Destination (Join-Path $resultDirectory 'report.csv')
        Copy-Item -LiteralPath $result.Manifest -Destination (Join-Path $resultDirectory 'manifest.json')
        Copy-Item -LiteralPath $statLineCsvPath -Destination (Join-Path $resultDirectory 'statline.csv')
        Copy-Item -LiteralPath $result.UnityLog -Destination (Join-Path $resultDirectory 'unity.log')
        $job = Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
        $job.status = 'completed'
        $job | Add-Member -NotePropertyName sourceCommit -NotePropertyValue $sourceCommit -Force
        $job | Add-Member -NotePropertyName sourceTreeCleanBeforeRun -NotePropertyValue ($sourceStatusBefore.Count -eq 0) -Force
        $job | Add-Member -NotePropertyName sourceTreeCleanAfterCleanup -NotePropertyValue $cleanAfterCleanup -Force
        $job | Add-Member -NotePropertyName reportHashVerified -NotePropertyValue $true -Force
        $job | Add-Member -NotePropertyName packagedFiles -NotePropertyValue @('report.json', 'report.csv', 'statline.csv', 'manifest.json', 'unity.log') -Force
        $job | Add-Member -NotePropertyName completedUtc -NotePropertyValue ([DateTime]::UtcNow.ToString('O')) -Force
        $job | Add-Member -NotePropertyName result -NotePropertyValue $result -Force
        $destination = Join-Path $completed (Split-Path $Path -Leaf)
        $job | ConvertTo-Json -Depth 16 | Set-Content -LiteralPath $destination -Encoding utf8
        Remove-Item -LiteralPath $PendingPath -Force
        return $true
    }
    catch {
        Restore-UnityGeneratedChanges
        $job = Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
        $job.status = 'failed'
        $job | Add-Member -NotePropertyName sourceCommit -NotePropertyValue $sourceCommit -Force
        $job | Add-Member -NotePropertyName sourceTreeCleanBeforeRun -NotePropertyValue ($sourceStatusBefore.Count -eq 0) -Force
        $job | Add-Member -NotePropertyName sourceTreeCleanAfterCleanup -NotePropertyValue ((@(Get-WorkerStatus)).Count -eq 0) -Force
        $job | Add-Member -NotePropertyName completedUtc -NotePropertyValue ([DateTime]::UtcNow.ToString('O')) -Force
        $job | Add-Member -NotePropertyName error -NotePropertyValue $_.Exception.Message -Force
        $destination = Join-Path $failed (Split-Path $Path -Leaf)
        $job | ConvertTo-Json -Depth 16 | Set-Content -LiteralPath $destination -Encoding utf8
        Remove-Item -LiteralPath $PendingPath -Force
        return $false
    }
}

do {
    if ($AutoSync) { Sync-WorkerRepository }
    Assert-WorkerClean 'job execution'
    $jobs = @(Get-ChildItem -LiteralPath $pending -Filter '*.json' -File | Sort-Object Name)
    foreach ($job in $jobs) {
        $claimed = Join-Path $runtimeRoot $job.Name
        try {
            Copy-Item -LiteralPath $job.FullName -Destination $claimed -ErrorAction Stop
            Invoke-Job $claimed $job.FullName | Out-Null
            Remove-Item -LiteralPath $claimed -Force
            if ($AutoPublish) { Publish-WorkerResults }
        }
        catch { Write-Warning "Could not claim job '$($job.Name)': $($_.Exception.Message)" }
    }
    if (-not $Once) { Start-Sleep -Seconds $PollSeconds }
} while (-not $Once)
