[CmdletBinding()]
param(
    [ValidateSet('Help', 'Run', 'Test')]
    [string]$Command = 'Help',
    [Parameter(ValueFromRemainingArguments)]
    [string[]]$Arguments
)

function Show-Usage {
    @'
CellSim Help
CellSim Test [-Mode EditMode|PlayMode|All]
CellSim Run [-SeedStart 1] [-SeedCount 20] [-ScenarioPath Assets/...]

Unity must be closed before Test or Run.
'@ | Write-Output
}

switch ($Command) {
    'Help' { Show-Usage }
    'Test' { & (Join-Path $PSScriptRoot 'tools/Invoke-UnityTests.ps1') @Arguments }
    'Run' { & (Join-Path $PSScriptRoot 'tools/Run-CellularExperiment.ps1') @Arguments }
}
