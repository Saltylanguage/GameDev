[CmdletBinding()]
param(
    [ValidateSet('Development', 'HeldOut')]
    [string]$Panel = 'Development',
    [ValidateSet('Original', 'Alternate')]
    [string]$Sequence = 'Original',
    [string]$ProjectPath,
    [string]$UnityPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if ([string]::IsNullOrWhiteSpace($ProjectPath)) {
    $ProjectPath = Join-Path $PSScriptRoot '..'
}

$project = (Resolve-Path -LiteralPath $ProjectPath).Path
$seedStart = if ($Panel -eq 'Development') { 1 } else { 106 }
$seedCount = if ($Panel -eq 'Development') { 20 } else { 5 }
$phaseSchedule = if ($Sequence -eq 'Original') {
    @(
        'none',
        'none',
        'trailblazer-long-stride',
        'trailblazer-long-stride',
        'trailblazer-long-stride,trailblazer-far-sight',
        'trailblazer-long-stride,trailblazer-far-sight,warren-guarded-burrow',
        'trailblazer-long-stride,trailblazer-far-sight,warren-guarded-burrow,warren-room-to-breed',
        'trailblazer-long-stride,trailblazer-far-sight,warren-guarded-burrow,warren-room-to-breed',
        'trailblazer-long-stride,trailblazer-far-sight,warren-guarded-burrow,warren-room-to-breed,gardeners-careful-sowing',
        'trailblazer-long-stride,trailblazer-far-sight,warren-guarded-burrow,warren-room-to-breed,gardeners-careful-sowing,familial-bond-large-litters'
    )
}
else {
    @(
        'none',
        'none',
        'familial-bond-large-litters',
        'familial-bond-large-litters',
        'familial-bond-large-litters,gardeners-careful-sowing',
        'familial-bond-large-litters,gardeners-careful-sowing,warren-room-to-breed',
        'familial-bond-large-litters,gardeners-careful-sowing,warren-room-to-breed,warren-guarded-burrow',
        'familial-bond-large-litters,gardeners-careful-sowing,warren-room-to-breed,warren-guarded-burrow',
        'familial-bond-large-litters,gardeners-careful-sowing,warren-room-to-breed,warren-guarded-burrow,trailblazer-far-sight',
        'familial-bond-large-litters,gardeners-careful-sowing,warren-room-to-breed,warren-guarded-burrow,trailblazer-far-sight,trailblazer-long-stride'
    )
}
$schedule = $phaseSchedule -join ';'
$contractFileName = if ($Sequence -eq 'Original') { 'CONTRACT_DRAFT.md' } else { 'ALTERNATE_ORDER_CONTRACT.md' }
$runScript = Join-Path $PSScriptRoot 'Run-CellularExperiment.ps1'
$bundleScript = Join-Path $PSScriptRoot 'Test-CellSimArtifactBundle.ps1'
$reportScript = Join-Path $PSScriptRoot 'New-EX010Report.ps1'
$analysisScript = Join-Path $PSScriptRoot 'New-CellSimReport.ps1'

$runArguments = @{
    ScenarioPath = 'Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset'
    SeedStart = $seedStart
    SeedCount = $seedCount
    PlayerSpeciesId = 'hare'
    PhaseLengthTicks = 200
    PhaseUpgradeAssetSchedule = $schedule
    UpgradeAssetCatalogPath = 'Assets/Data/CellularSimulation/Upgrades/Production'
    CombatMode = 'opposed-roll'
    AttackOpportunityMode = 'natural'
    ExperimentalFeatures = 'bev-experimental'
    ProjectPath = $project
}
if (-not [string]::IsNullOrWhiteSpace($UnityPath)) {
    $runArguments.UnityPath = $UnityPath
}

$runOutput = & $runScript @runArguments
$runResult = @($runOutput | Where-Object {
        $null -ne $_.PSObject.Properties['ArtifactDirectory']
    } | Select-Object -Last 1)
if ($runResult.Count -ne 1) {
    throw 'The EX-010 run did not return an artifact directory.'
}

$artifactDirectory = [string]$runResult[0].ArtifactDirectory
$reportPath = Join-Path $artifactDirectory 'report.json'
$contractPath = Join-Path $project (Join-Path 'docs/Research/Experiments/EX-010-Sequential-Upgrade-Continuation' $contractFileName)
$contractHash = (Get-FileHash -LiteralPath $contractPath -Algorithm SHA256).Hash.ToLowerInvariant()
$executionRecord = [ordered]@{
    experiment = 'EXP-010'
    panel = $Panel
    sequence = $Sequence
    scenario = 'Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset'
    playerSpeciesId = 'hare'
    seedStart = $seedStart
    seedCount = $seedCount
    phaseLengthTicks = 200
    phaseCount = 10
    schedule = $phaseSchedule
    acquisitionTicks = @(400, 800, 1000, 1200, 1600, 1800)
    upgradeCatalogPath = 'Assets/Data/CellularSimulation/Upgrades/Production'
    contractPath = $contractPath
    contractSha256 = $contractHash
    reportPath = $reportPath
}
$executionRecord | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath (Join-Path $artifactDirectory 'ex010-execution.json') -Encoding utf8

$bundleValidationPath = Join-Path $artifactDirectory 'bundle-validation.json'
& $bundleScript -ArtifactDirectory $artifactDirectory -RequireUnityLog | Set-Content -LiteralPath $bundleValidationPath -Encoding utf8
if ($LASTEXITCODE -ne 0) {
    throw "EX-010 artifact validation failed. See '$bundleValidationPath'."
}

& $reportScript -ReportPath $reportPath -OutputDirectory $artifactDirectory -ProjectPath $project -Sequence $Sequence
if ($LASTEXITCODE -ne 0) {
    throw "EX-010 phase report generation failed for '$reportPath'."
}

& $analysisScript -ReportPath $reportPath -OutputPath (Join-Path $artifactDirectory 'analysis.md') -ProjectPath $project
if ($LASTEXITCODE -ne 0) {
    throw "General report generation failed for '$reportPath'."
}

[pscustomobject]@{
    Panel = $Panel
    ArtifactDirectory = $artifactDirectory
    Report = $reportPath
    ExecutionRecord = Join-Path $artifactDirectory 'ex010-execution.json'
    Validation = Join-Path $artifactDirectory 'ex010-validation.json'
    ReportMarkdown = Join-Path $artifactDirectory 'ex010-report.md'
    Analysis = Join-Path $artifactDirectory 'analysis.md'
}
