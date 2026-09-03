[CmdletBinding()]
param(
    [Parameter(Mandatory)] [string]$JobName,
    [string]$ScenarioPath,
    [int]$SeedStart = 1,
    [ValidateRange(1, 10000)] [int]$SeedCount = 20,
    [string]$PlayerSpeciesId = 'hare',
    [string]$UpgradeId = 'none',
    [string]$UpgradeSequence = '',
    [ValidateRange(0, 1000000)] [double]$UpgradeValueOverride = 0,
    [ValidateSet('legacy-fixed-damage', 'opposed-roll')] [string]$CombatMode = 'legacy-fixed-damage',
    [ValidateSet('natural', 'fixed-rate-diagnostic', 'paired-lockstep-diagnostic')]
    [string]$AttackOpportunityMode = 'natural',
    [string]$ExperimentalFeatures = '',
    [ValidateRange(0, 1000000)] [int]$FoxAttackCooldownTicks = 0,
    [ValidateRange(0, 1)] [double]$PreContactAvoidanceChance = 0,
    [string]$RequestedBy = $env:USERNAME,
    [switch]$DryRun
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$workerBranch = 'codex/cellsim-worker'
$remoteName = 'origin'
$projectRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path

function Invoke-Git {
    param(
        [Parameter(Mandatory)] [string]$WorkingDirectory,
        [Parameter(Mandatory)] [string[]]$Arguments
    )

    # Git writes progress (including successful worktree setup) to stderr. With
    # the script-wide Stop preference, capture it without turning it into a
    # terminating PowerShell error; the native exit code remains authoritative.
    $previousErrorActionPreference = $ErrorActionPreference
    $ErrorActionPreference = 'Continue'
    try {
        $output = @(& git -C $WorkingDirectory @Arguments 2>&1)
        $exitCode = $LASTEXITCODE
    }
    finally {
        $ErrorActionPreference = $previousErrorActionPreference
    }
    if ($exitCode -ne 0) {
        $details = ($output | ForEach-Object { $_.ToString() }) -join [Environment]::NewLine
        throw "git $($Arguments -join ' ') failed with exit code $exitCode. $details"
    }

    if ($output.Count -eq 0) { return '' }
    return (($output | ForEach-Object { $_.ToString() }) -join [Environment]::NewLine)
}

function Assert-DesktopCheckoutUnchanged {
    $branch = (Invoke-Git -WorkingDirectory $repoRoot -Arguments @('branch', '--show-current')).Trim()
    $head = (Invoke-Git -WorkingDirectory $repoRoot -Arguments @('rev-parse', 'HEAD')).Trim()
    $status = Invoke-Git -WorkingDirectory $repoRoot -Arguments @('status', '--porcelain=v1', '--untracked-files=all')
    if ($branch -ne $initialBranch -or $head -ne $initialHead -or $status -ne $initialStatus) {
        throw 'The desktop checkout changed while submitting the remote CellSim job.'
    }
}

$repoRootText = (Invoke-Git -WorkingDirectory $projectRoot -Arguments @('rev-parse', '--show-toplevel')).Trim()
$repoRoot = (Resolve-Path -LiteralPath $repoRootText).Path
$initialBranch = (Invoke-Git -WorkingDirectory $repoRoot -Arguments @('branch', '--show-current')).Trim()
$initialHead = (Invoke-Git -WorkingDirectory $repoRoot -Arguments @('rev-parse', 'HEAD')).Trim()
$initialStatus = Invoke-Git -WorkingDirectory $repoRoot -Arguments @('status', '--porcelain=v1', '--untracked-files=all')

$tempRoot = [IO.Path]::GetFullPath((Join-Path ([IO.Path]::GetTempPath()) ('cellsim-remote-' + [guid]::NewGuid().ToString('N'))))
$tempBase = [IO.Path]::GetFullPath([IO.Path]::GetTempPath())
if (-not $tempRoot.StartsWith($tempBase, [StringComparison]::OrdinalIgnoreCase)) {
    throw 'Refusing to use a temporary path outside the system temporary directory.'
}
$worktreePath = Join-Path $tempRoot 'worktree'
$worktreeCreated = $false
$failure = $null
$cleanupFailure = $null
$jobId = $null
$pushedCommit = $null

try {
    Invoke-Git -WorkingDirectory $repoRoot -Arguments @(
        'fetch', $remoteName, "$workerBranch`:refs/remotes/$remoteName/$workerBranch"
    ) | Out-Null
    Assert-DesktopCheckoutUnchanged

    $workerCommit = (Invoke-Git -WorkingDirectory $repoRoot -Arguments @(
        'rev-parse', "refs/remotes/$remoteName/$workerBranch"
    )).Trim()

    New-Item -ItemType Directory -Path $tempRoot -Force | Out-Null
    Invoke-Git -WorkingDirectory $repoRoot -Arguments @(
        'worktree', 'add', '--detach', $worktreePath, "refs/remotes/$remoteName/$workerBranch"
    ) | Out-Null
    $worktreeCreated = $true

    $worktreeRoot = (Resolve-Path -LiteralPath $worktreePath).Path.TrimEnd('\')
    $actualWorktreeCommit = (Invoke-Git -WorkingDirectory $worktreeRoot -Arguments @('rev-parse', 'HEAD')).Trim()
    if ($actualWorktreeCommit -ne $workerCommit) {
        throw 'The temporary worktree was not created at the fetched worker commit.'
    }

    $worktreeProjectRoot = Join-Path $worktreeRoot 'LearningIndieDev'
    $submitScript = Join-Path $worktreeProjectRoot 'tools\Submit-CellSimJob.ps1'
    if (-not (Test-Path -LiteralPath $submitScript -PathType Leaf)) {
        throw 'The fetched worker branch does not contain tools\Submit-CellSimJob.ps1.'
    }

    $queueRoot = Join-Path $worktreeProjectRoot 'automation\CellSimQueue'
    $pendingRoot = Join-Path $queueRoot 'Pending'
    $beforeNames = @()
    if (Test-Path -LiteralPath $pendingRoot -PathType Container) {
        $beforeNames = @(Get-ChildItem -LiteralPath $pendingRoot -Filter '*.json' -File | Select-Object -ExpandProperty Name)
    }

    $submitParameters = @{
        JobName = $JobName
        SeedStart = $SeedStart
        SeedCount = $SeedCount
        PlayerSpeciesId = $PlayerSpeciesId
        UpgradeId = $UpgradeId
        UpgradeSequence = $UpgradeSequence
        UpgradeValueOverride = $UpgradeValueOverride
        CombatMode = $CombatMode
        AttackOpportunityMode = $AttackOpportunityMode
        ExperimentalFeatures = $ExperimentalFeatures
        FoxAttackCooldownTicks = $FoxAttackCooldownTicks
        PreContactAvoidanceChance = $PreContactAvoidanceChance
        RequestedBy = $RequestedBy
        QueueRoot = $queueRoot
    }
    if (-not [string]::IsNullOrWhiteSpace($ScenarioPath)) {
        $submitParameters.ScenarioPath = $ScenarioPath
    }

    & $submitScript @submitParameters | Out-Null
    if (-not $?) { throw 'Submit-CellSimJob.ps1 failed inside the temporary worktree.' }

    $afterFiles = @(Get-ChildItem -LiteralPath $pendingRoot -Filter '*.json' -File)
    $newFiles = @($afterFiles | Where-Object { $beforeNames -notcontains $_.Name })
    if ($newFiles.Count -ne 1) {
        throw "Expected exactly one new pending job JSON, found $($newFiles.Count)."
    }

    $jobFile = $newFiles[0]
    $jobData = Get-Content -LiteralPath $jobFile.FullName -Raw | ConvertFrom-Json
    if ($null -eq $jobData.PSObject.Properties['jobId'] -or
        [string]::IsNullOrWhiteSpace([string]$jobData.jobId) -or
        [string]$jobData.status -ne 'pending' -or
        [string]$jobData.jobId -ne $jobFile.BaseName) {
        throw 'The generated job JSON failed validation.'
    }
    $jobId = [string]$jobData.jobId

    $jobFullPath = (Resolve-Path -LiteralPath $jobFile.FullName).Path
    $relativeJobPath = $jobFullPath.Substring($worktreeRoot.Length).TrimStart('\', '/')
    $relativeJobPath = $relativeJobPath.Replace('\', '/')
    $expectedRelativePath = "LearningIndieDev/automation/CellSimQueue/Pending/$($jobFile.Name)"
    if ($relativeJobPath -ne $expectedRelativePath) {
        throw 'The generated job is outside the expected Pending queue path.'
    }

    if ($DryRun) {
        $pushedCommit = '(dry-run; no commit or push)'
    }
    else {
        $stagedBefore = (Invoke-Git -WorkingDirectory $worktreeRoot -Arguments @('diff', '--cached', '--name-only')).Trim()
        if (-not [string]::IsNullOrWhiteSpace($stagedBefore)) {
            throw 'The temporary worktree index was not clean before staging.'
        }

        Invoke-Git -WorkingDirectory $worktreeRoot -Arguments @('add', '--', $expectedRelativePath) | Out-Null
        $stagedNames = @((Invoke-Git -WorkingDirectory $worktreeRoot -Arguments @(
            'diff', '--cached', '--name-only'
        )) -split '\r?\n' | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
        $addedNames = @((Invoke-Git -WorkingDirectory $worktreeRoot -Arguments @(
            'diff', '--cached', '--name-only', '--diff-filter=A'
        )) -split '\r?\n' | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
        if ($stagedNames.Count -ne 1 -or $stagedNames[0] -ne $expectedRelativePath -or
            $addedNames.Count -ne 1 -or $addedNames[0] -ne $expectedRelativePath) {
            throw 'Refusing to commit anything other than the generated Pending job JSON.'
        }

        Invoke-Git -WorkingDirectory $worktreeRoot -Arguments @('commit', '-m', "Queue CellSim job $jobId") | Out-Null
        $pushedCommit = (Invoke-Git -WorkingDirectory $worktreeRoot -Arguments @('rev-parse', 'HEAD')).Trim()
        $commitChanges = @((Invoke-Git -WorkingDirectory $worktreeRoot -Arguments @(
            'diff-tree', '--no-commit-id', '--name-status', '--no-renames', '-r', $pushedCommit
        )) -split '\r?\n' | Where-Object { -not [string]::IsNullOrWhiteSpace($_) })
        if ($commitChanges.Count -ne 1 -or
            -not $commitChanges[0].StartsWith("A`t$expectedRelativePath")) {
            throw 'The created commit did not contain exactly the generated Pending job JSON.'
        }

        Invoke-Git -WorkingDirectory $worktreeRoot -Arguments @(
            'push', $remoteName, "HEAD:refs/heads/$workerBranch"
        ) | Out-Null
    }
}
catch {
    $failure = $_
}

if ($worktreeCreated) {
    try {
        Invoke-Git -WorkingDirectory $repoRoot -Arguments @('worktree', 'remove', '--force', $worktreePath) | Out-Null
    }
    catch {
        $cleanupFailure = $_
    }
}

if ((-not $worktreeCreated -or $null -eq $cleanupFailure) -and (Test-Path -LiteralPath $tempRoot)) {
    try {
        Remove-Item -LiteralPath $tempRoot -Recurse -Force -ErrorAction Stop
    }
    catch {
        if ($null -eq $cleanupFailure) { $cleanupFailure = $_ }
    }
}

try {
    Assert-DesktopCheckoutUnchanged
}
catch {
    if ($null -eq $failure) { $failure = $_ }
    elseif ($null -eq $cleanupFailure) { $cleanupFailure = $_ }
}

if ($null -ne $failure) {
    if ($null -ne $cleanupFailure) {
        throw "Remote CellSim submission failed: $($failure.ToString()) Cleanup also failed: $($cleanupFailure.ToString())"
    }
    throw "Remote CellSim submission failed: $($failure.ToString())"
}
if ($null -ne $cleanupFailure) {
    throw "Remote CellSim cleanup failed: $($cleanupFailure.ToString())"
}

[pscustomobject]@{
    JobId = $jobId
    PushedCommit = $pushedCommit
    Remote = "$remoteName/$workerBranch"
}
