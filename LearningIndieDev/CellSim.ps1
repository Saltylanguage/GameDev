[CmdletBinding()]
param(
    [ValidateSet('Help', 'Run', 'Test', 'Report', 'Compare', 'Baseline')]
    [string]$Command = 'Help',
    [ValidateSet('EditMode', 'PlayMode', 'All')]
    [string]$Mode = 'All',
    [ValidateRange(0, 10000)]
    [int]$SeedStart = 1,
    [ValidateRange(1, 10000)]
    [int]$SeedCount = 20,
    [string]$ScenarioPath,
    [string]$PlayerSpeciesId = 'herbivore',
    [string]$ReportPath,
    [string]$BaselinePath,
    [string]$TestArtifactDirectory,
    [string]$OutputPath,
    [string]$ProjectPath,
    [string]$UnityPath
)

function Show-Usage {
    @'
CellSim Help
CellSim Test [-Mode EditMode|PlayMode|All]
CellSim Run [-SeedStart 1] [-SeedCount 20] [-ScenarioPath Assets/...]
CellSim Report [-ReportPath artifacts/.../report.json]
CellSim Compare -BaselinePath artifacts/.../report.json -ReportPath artifacts/.../report.json
CellSim Baseline [-SeedStart 1] [-SeedCount 20] [-ScenarioPath Assets/...]

Unity must be closed before Test or Run.
'@ | Write-Output
}

switch ($Command) {
    'Help' { Show-Usage }
    'Test' {
        & (Join-Path $PSScriptRoot 'tools/Invoke-UnityTests.ps1') -Mode $Mode -ProjectPath $ProjectPath -UnityPath $UnityPath
    }
    'Run' {
        & (Join-Path $PSScriptRoot 'tools/Run-CellularExperiment.ps1') -SeedStart $SeedStart -SeedCount $SeedCount -ScenarioPath $ScenarioPath -PlayerSpeciesId $PlayerSpeciesId -ProjectPath $ProjectPath -UnityPath $UnityPath
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
    'Baseline' {
        $testOutput = & (Join-Path $PSScriptRoot 'tools/Invoke-UnityTests.ps1') -Mode All -ProjectPath $ProjectPath -UnityPath $UnityPath
        $testResult = @($testOutput | Where-Object { $_.PSObject.Properties.Name -contains 'ArtifactDirectory' } | Select-Object -Last 1)
        $runOutput = & (Join-Path $PSScriptRoot 'tools/Run-CellularExperiment.ps1') -SeedStart $SeedStart -SeedCount $SeedCount -ScenarioPath $ScenarioPath -PlayerSpeciesId $PlayerSpeciesId -ProjectPath $ProjectPath -UnityPath $UnityPath
        $runResult = @($runOutput | Where-Object { $_.PSObject.Properties.Name -contains 'Report' } | Select-Object -Last 1)
        if ($testResult.Count -ne 1 -or $runResult.Count -ne 1) {
            throw 'CellSim Baseline did not receive the expected test and experiment results.'
        }

        & (Join-Path $PSScriptRoot 'tools/New-CellSimReport.ps1') -ReportPath $runResult[0].Report -TestArtifactDirectory $testResult[0].ArtifactDirectory
    }
}
