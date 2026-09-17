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
    [ValidateSet('legacy-fixed-damage', 'opposed-roll')] [string]$CombatMode = 'opposed-roll',
    [ValidateSet('natural', 'fixed-rate-diagnostic', 'paired-lockstep-diagnostic')]
    [string]$AttackOpportunityMode = 'natural',
    [string]$ExperimentalFeatures = '',
    [ValidateRange(0, 1000000)] [int]$FoxAttackCooldownTicks = 0,
    [ValidateRange(0, 1)] [double]$PreContactAvoidanceChance = 0,
    [string]$QueueRoot = (Join-Path $PSScriptRoot '..\automation\CellSimQueue'),
    [string]$RequestedBy = $env:USERNAME
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if ([string]::IsNullOrWhiteSpace($RequestedBy)) { $RequestedBy = 'desktop-user' }
if (-not [string]::IsNullOrWhiteSpace($UpgradeId) -and -not [string]::IsNullOrWhiteSpace($UpgradeSequence) -and $UpgradeId -ne 'none') {
    throw 'Use either -UpgradeId or -UpgradeSequence, not both.'
}

$pending = Join-Path $QueueRoot 'Pending'
New-Item -ItemType Directory -Path $pending -Force | Out-Null
$jobId = '{0}-{1}' -f (Get-Date -Format 'yyyyMMdd-HHmmss'), ([guid]::NewGuid().ToString('N').Substring(0, 8))
$job = [ordered]@{
    schemaVersion = 1
    jobId = $jobId
    jobName = $JobName
    createdUtc = [DateTime]::UtcNow.ToString('O')
    requestedBy = $RequestedBy
    status = 'pending'
    parameters = [ordered]@{
        ScenarioPath = $ScenarioPath
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
    }
}
$path = Join-Path $pending "$jobId.json"
$job | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath $path -Encoding utf8
[pscustomobject]@{ JobId = $jobId; Path = $path; Status = 'pending' }
