[CmdletBinding()]
param([string]$RunRoot = $PSScriptRoot, [switch]$Resume)
$ErrorActionPreference = 'Stop'
$runDirectory = (Resolve-Path -LiteralPath $RunRoot).Path
$dotnetPath = (Get-Command dotnet -CommandType Application | Select-Object -First 1).Source
$pythonPath = (Get-Command python -CommandType Application | Select-Object -First 1).Source
$tool = Join-Path $runDirectory 'tool/bin/Release/net8.0/CellSim.Batch.dll'
function Set-PipelineState([string]$State, [string]$Message = '') {
    $path = Join-Path $runDirectory 'pipeline-state.json'
    $temporary = "$path.tmp-$PID"
    @{ state=$State; message=$Message; updatedUtc=[DateTime]::UtcNow.ToString('O'); pid=$PID;
       runs=12800; workers=16; seedStart=50000; seedCount=100 } |
        ConvertTo-Json | Set-Content -LiteralPath $temporary -Encoding utf8
    Move-Item -LiteralPath $temporary -Destination $path -Force
}
try {
    $checks = Get-Content (Join-Path $runDirectory 'validation/checks.json') -Raw | ConvertFrom-Json
    [xml]$tests = Get-Content (Join-Path $runDirectory 'validation/EditMode-results.xml') -Raw
    $plan = Get-Content (Join-Path $runDirectory 'sweep/plan.json') -Raw | ConvertFrom-Json
    if ($checks.state -ne 'Passed' -or $tests.'test-run'.result -ne 'Passed' -or
        [int]$tests.'test-run'.total -lt 1 -or $plan.workers -ne 16 -or $plan.seedStart -ne 50000 -or
        $plan.seedCount -ne 100 -or $plan.conditions.Count -ne 128) { throw 'Approved panel launch checks do not match.' }
    Set-PipelineState 'Running'
    $arguments = @($tool, 'batch', (Join-Path $runDirectory 'sweep/plan.json'))
    if ($Resume) { $arguments += '--resume' }
    & $dotnetPath @arguments
    if ($LASTEXITCODE -ne 0) { throw "Batch failed with exit code $LASTEXITCODE" }
    Set-PipelineState 'Analyzing'
    if (-not (Test-Path (Join-Path $runDirectory 'analysis/analysis.json'))) {
        & $pythonPath (Join-Path $runDirectory 'tool/sweep.py') analyze (Join-Path $runDirectory 'sweep') `
            (Join-Path $runDirectory 'analysis') --rank outcome.allSpeciesAliveAtHorizon --top 10
        if ($LASTEXITCODE -ne 0) { throw "Sweep analysis failed with exit code $LASTEXITCODE" }
    }
    if (-not (Test-Path (Join-Path $runDirectory 'purchase-analysis/windows.csv'))) {
        & $pythonPath (Join-Path $runDirectory 'tool/purchase_report.py') $runDirectory
        if ($LASTEXITCODE -ne 0) { throw "Purchase report failed with exit code $LASTEXITCODE" }
    }
    Set-PipelineState 'Completed' 'Simulation, paired analysis and purchase reports completed.'
}
catch { Set-PipelineState 'Failed' $_.Exception.Message; throw }
