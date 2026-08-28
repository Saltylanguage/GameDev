[CmdletBinding()]
param(
[ValidateSet('Help', 'Run', 'Test', 'Visuals', 'Report', 'Compare', 'Baseline', 'Validate')]
    [string]$Command = 'Help',
    [ValidateSet('EditMode', 'PlayMode', 'All')]
    [string]$Mode = 'All',
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
    [string]$ScenarioPath,
[string]$PlayerSpeciesId = 'herbivore',
[string]$UpgradeId = 'none',
[string]$UpgradeSequence = '',
[ValidateSet('legacy-fixed-damage', 'opposed-roll')]
[string]$CombatMode = 'legacy-fixed-damage',
[ValidateSet('natural', 'fixed-rate-diagnostic', 'paired-lockstep-diagnostic')]
[string]$AttackOpportunityMode = 'natural',
[string]$ExperimentalFeatures = '',
[ValidateRange(0, 1000000)]
[int]$FoxAttackCooldownTicks = 0,
[string]$ReportPath,
    [string]$BaselinePath,
    [string]$TestArtifactDirectory,
    [string]$OutputPath,
    [string]$TestFilter,
    [string]$ReplayReportPath,
    [int]$ReplaySeed = -1,
    [string]$ProjectPath,
    [string]$UnityPath
)

function Show-Usage {
    @'
CellSim Help
CellSim Test [-Mode EditMode|PlayMode|All]
CellSim Visuals [-TestFilter SaltyGame.PlayModeTests.SomeTest]
CellSim Visuals [-ReplayReportPath artifacts/.../report.json] -ReplaySeed 10100
CellSim Run [-SeedStart 1] [-SeedCount 20] [-GridWidth 64] [-GridHeight 64] [-RunDurationSeconds 20] [-StepIntervalSeconds 0.1] [-ScenarioPath Assets/...]
             [-PlayerSpeciesId hare] [-UpgradeId tough-hide] [-UpgradeSequence tough-hide,tough-hide]
             [-ExperimentalFeatures bev-experimental] [-CombatMode opposed-roll]
CellSim Report [-ReportPath artifacts/.../report.json]
CellSim Compare -BaselinePath artifacts/.../report.json -ReportPath artifacts/.../report.json
CellSim Baseline [-SeedStart 1] [-SeedCount 20] [-GridWidth 64] [-GridHeight 64] [-ScenarioPath Assets/...]
CellSim Validate -ReportPath artifacts/.../report.json

Unity must be closed before Test or Run.
Visuals also requires Unity to be closed and a graphics-capable editor run.
'@ | Write-Output
}

switch ($Command) {
    'Help' { Show-Usage }
    'Test' {
        & (Join-Path $PSScriptRoot 'tools/Invoke-UnityTests.ps1') -Mode $Mode -ProjectPath $ProjectPath -UnityPath $UnityPath
    }
    'Visuals' {
        & (Join-Path $PSScriptRoot 'tools/Invoke-UnityVisualEvidence.ps1') -ProjectPath $ProjectPath -UnityPath $UnityPath -TestFilter $TestFilter -ReplayReportPath $ReplayReportPath -ReplaySeed $ReplaySeed
    }
    'Run' {
        & (Join-Path $PSScriptRoot 'tools/Run-CellularExperiment.ps1') -SeedStart $SeedStart -SeedCount $SeedCount -GridWidth $GridWidth -GridHeight $GridHeight -RunDurationSeconds $RunDurationSeconds -StepIntervalSeconds $StepIntervalSeconds -ScenarioPath $ScenarioPath -PlayerSpeciesId $PlayerSpeciesId -UpgradeId $UpgradeId -UpgradeSequence $UpgradeSequence -CombatMode $CombatMode -AttackOpportunityMode $AttackOpportunityMode -ExperimentalFeatures $ExperimentalFeatures -FoxAttackCooldownTicks $FoxAttackCooldownTicks -ProjectPath $ProjectPath -UnityPath $UnityPath
    }
    'Report' {
        & (Join-Path $PSScriptRoot 'tools/New-CellSimReport.ps1') -ReportPath $ReportPath -BaselinePath $BaselinePath -TestArtifactDirectory $TestArtifactDirectory -OutputPath $OutputPath -ProjectPath $ProjectPath
    }
    'Compare' {
        if ([string]::IsNullOrWhiteSpace($BaselinePath) -or [string]::IsNullOrWhiteSpace($ReportPath)) {
            throw 'CellSim Compare requires -BaselinePath and -ReportPath.'
        }

        & (Join-Path $PSScriptRoot 'tools/New-CellSimReport.ps1') -ReportPath $ReportPath -BaselinePath $BaselinePath -OutputPath $OutputPath -ProjectPath $ProjectPath
    }
    'Validate' {
        if ([string]::IsNullOrWhiteSpace($ReportPath)) {
            throw 'CellSim Validate requires -ReportPath.'
        }

        & (Join-Path $PSScriptRoot 'tools/Validate-HerbivoreStatLine.ps1') -ReportPath $ReportPath -OutputDirectory $OutputPath
    }
    'Baseline' {
        $testOutput = & (Join-Path $PSScriptRoot 'tools/Invoke-UnityTests.ps1') -Mode All -ProjectPath $ProjectPath -UnityPath $UnityPath
        $testResult = @($testOutput | Where-Object { $_.PSObject.Properties.Name -contains 'ArtifactDirectory' } | Select-Object -Last 1)
        $runOutput = & (Join-Path $PSScriptRoot 'tools/Run-CellularExperiment.ps1') -SeedStart $SeedStart -SeedCount $SeedCount -GridWidth $GridWidth -GridHeight $GridHeight -RunDurationSeconds $RunDurationSeconds -StepIntervalSeconds $StepIntervalSeconds -ScenarioPath $ScenarioPath -PlayerSpeciesId $PlayerSpeciesId -ProjectPath $ProjectPath -UnityPath $UnityPath
        $runResult = @($runOutput | Where-Object { $_.PSObject.Properties.Name -contains 'Report' } | Select-Object -Last 1)
        if ($testResult.Count -ne 1 -or $runResult.Count -ne 1) {
            throw 'CellSim Baseline did not receive the expected test and experiment results.'
        }

        & (Join-Path $PSScriptRoot 'tools/New-CellSimReport.ps1') -ReportPath $runResult[0].Report -TestArtifactDirectory $testResult[0].ArtifactDirectory
    }
}
