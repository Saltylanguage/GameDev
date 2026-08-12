[CmdletBinding()]
param(
    [ValidateSet('help', 'test', 'run')]
    [string]$Command = 'help',
    [Parameter(ValueFromRemainingArguments)]
    [string[]]$Arguments
)

function Show-Usage {
    @'
Usage:
  .\tools\sim.cmd test [-Mode EditMode|PlayMode|All]
  .\tools\sim.cmd run [-SeedStart 1] [-SeedCount 20] [-ScenarioPath Assets/...]

Unity must be closed before test or run.
'@ | Write-Output
}

switch ($Command) {
    'help' { Show-Usage }
    'test' { & (Join-Path $PSScriptRoot 'Invoke-UnityTests.ps1') @Arguments }
    'run' { & (Join-Path $PSScriptRoot 'Run-CellularExperiment.ps1') @Arguments }
}
