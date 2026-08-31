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

function Publish-WorkerResults {
    $unexpected = @(Get-WorkerStatus | Where-Object { $_ -notmatch 'LearningIndieDev/automation/CellSimQueue/' })
    if ($unexpected.Count -gt 0) {
        throw "Unexpected non-queue changes prevent result publication:`n$($unexpected -join [Environment]::NewLine)"
    }
    Invoke-WorkerGit @('add', '--', 'LearningIndieDev/automation/CellSimQueue')
    $staged = @(git -C (Split-Path $project -Parent) diff --cached --name-only)
    if ($staged.Count -eq 0) { return }
    Invoke-WorkerGit @('commit', '-m', 'Publish remote CellSim worker result')
    Invoke-WorkerGit @('push', 'origin', 'codex/cellsim-worker')
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
        $cleanAfterCleanup = (Get-WorkerStatus).Count -eq 0
        Assert-WorkerClean 'result packaging'
        $resultDirectory = Join-Path $completed $job.jobId
        New-Item -ItemType Directory -Path $resultDirectory -Force | Out-Null
        Copy-Item -LiteralPath $result.Report -Destination (Join-Path $resultDirectory 'report.json')
        Copy-Item -LiteralPath $result.Manifest -Destination (Join-Path $resultDirectory 'manifest.json')
        $job = Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
        $job.status = 'completed'
        $job | Add-Member -NotePropertyName sourceCommit -NotePropertyValue $sourceCommit -Force
        $job | Add-Member -NotePropertyName sourceTreeCleanBeforeRun -NotePropertyValue ($sourceStatusBefore.Count -eq 0) -Force
        $job | Add-Member -NotePropertyName sourceTreeCleanAfterCleanup -NotePropertyValue $cleanAfterCleanup -Force
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
        $job | Add-Member -NotePropertyName sourceTreeCleanAfterCleanup -NotePropertyValue ((Get-WorkerStatus).Count -eq 0) -Force
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
