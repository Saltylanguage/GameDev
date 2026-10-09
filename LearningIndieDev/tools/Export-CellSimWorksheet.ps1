[CmdletBinding()]
param(
    [string[]]$ReportPath,
    [string]$OutputPath,
    [ValidateRange(1, 1000)][int]$MaxRuns = 200,
    [int[]]$Checkpoints = @(400, 500),
    [string]$Match = '',
    [int[]]$Seeds = @(),
    [switch]$DetailedObservations,
    [switch]$Open,
    [string]$ProjectPath,
    [string]$PreviewPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
trap {
    Write-Host ('Export stopped: ' + $_.Exception.Message) -ForegroundColor Red
    exit 1
}
$projectRoot = if ($ProjectPath) { [IO.Path]::GetFullPath($ProjectPath) } else { Split-Path $PSScriptRoot }

if (-not $ReportPath) {
    Add-Type -AssemblyName System.Windows.Forms
    $picker = New-Object System.Windows.Forms.OpenFileDialog
    $picker.Title = 'Select simulation reports (report.json) or a completed sweep (sweep.json)'
    $picker.Filter = 'Simulation reports (*.json)|*.json'
    $picker.Multiselect = $true
    $picker.InitialDirectory = Join-Path $projectRoot 'artifacts'
    try {
        if ($picker.ShowDialog() -ne [System.Windows.Forms.DialogResult]::OK) {
            Write-Output 'Export cancelled. No files changed.'
            return
        }
        $ReportPath = $picker.FileNames
        $Open = $true
    } finally { $picker.Dispose() }
}

$resolvedInputs = @($ReportPath | ForEach-Object {
    $candidate = $_
    if (-not (Test-Path -LiteralPath $candidate)) { $candidate = Join-Path $projectRoot $candidate }
    if (-not (Test-Path -LiteralPath $candidate)) { throw "Cannot find simulation evidence: $_" }
    (Resolve-Path -LiteralPath $candidate).Path
})
if ($Checkpoints.Count -ne 2 -or @($Checkpoints | Where-Object { $_ -lt 0 }).Count) {
    throw 'Choose exactly two nonnegative checkpoint ticks, for example -Checkpoints 400,500.'
}
$runtimeRoot = if ($env:CELLSIM_WORKSPACE_DEPENDENCIES) { $env:CELLSIM_WORKSPACE_DEPENDENCIES } else {
    Join-Path $env:USERPROFILE '.cache/codex-runtimes/codex-primary-runtime/dependencies'
}
$python = Get-Command python -ErrorAction SilentlyContinue
$pythonPath = if ($python) { $python.Source } else { Join-Path $runtimeRoot 'python/python.exe' }
$node = Get-Command node -ErrorAction SilentlyContinue
$nodePath = if ($node) { $node.Source } else { Join-Path $runtimeRoot 'node/bin/node.exe' }
$modules = if ($env:CELLSIM_NODE_MODULES) { $env:CELLSIM_NODE_MODULES } else { Join-Path $runtimeRoot 'node/node_modules' }
if (-not (Test-Path -LiteralPath $pythonPath) -or -not (Test-Path -LiteralPath $nodePath) -or
    -not (Test-Path -LiteralPath (Join-Path $modules '@oai/artifact-tool'))) {
    throw 'Excel export needs Python 3, Node.js and @oai/artifact-tool. Use the installed Codex workspace runtime, or set CELLSIM_WORKSPACE_DEPENDENCIES / CELLSIM_NODE_MODULES. No AI session or Excel installation is needed.'
}
if (-not $OutputPath) {
    $outputDirectory = Join-Path $projectRoot 'artifacts/worksheets'
    $OutputPath = Join-Path $outputDirectory ('Simulation review-' + (Get-Date -Format 'yyyyMMdd-HHmmss-fff') + '.xlsx')
} else {
    $OutputPath = [IO.Path]::GetFullPath($OutputPath)
}
if ([IO.Path]::GetExtension($OutputPath) -ne '.xlsx') { throw 'OutputPath must end in .xlsx.' }
if (Test-Path -LiteralPath $OutputPath) { throw "This workbook already exists. Choose a new filename to protect your notes: $OutputPath" }
[IO.Directory]::CreateDirectory((Split-Path $OutputPath)) | Out-Null
$projectionPath = Join-Path ([IO.Path]::GetTempPath()) ('cellsim-worksheet-' + [Guid]::NewGuid().ToString('N') + '.json')
try {
    $arguments = @((Join-Path $PSScriptRoot 'CellSim.Batch/worksheet.py')) + $resolvedInputs +
        @('--output', $projectionPath, '--max-runs', "$MaxRuns", '--checkpoints', "$($Checkpoints[0])", "$($Checkpoints[1])")
    if ($Match) { $arguments += @('--match', $Match) }
    if ($Seeds) { $arguments += @('--seeds') + @($Seeds | ForEach-Object { "$_" }) }
    if ($DetailedObservations) { $arguments += '--detailed-observations' }
    & $pythonPath @arguments
    if ($LASTEXITCODE -ne 0) { throw 'Could not prepare worksheet rows. See the explanation above.' }
    $renderArguments = @((Join-Path $PSScriptRoot 'CellSim.Batch/worksheet.mjs'), $projectionPath, $OutputPath, $modules)
    if ($PreviewPath) { $renderArguments += [IO.Path]::GetFullPath($PreviewPath) }
    & $nodePath @renderArguments
    if ($LASTEXITCODE -ne 0) { throw 'Could not create the Excel workbook. Source reports were not changed.' }
    & $pythonPath (Join-Path $PSScriptRoot 'CellSim.Batch/worksheet.py') $projectionPath --output $OutputPath --link-evidence
    if ($LASTEXITCODE -ne 0) { throw 'Could not finish the source evidence links. Source reports were not changed.' }
    Write-Output "Worksheet ready: $OutputPath"
    if ($Open) { Start-Process -FilePath $OutputPath }
} finally {
    if (Test-Path -LiteralPath $projectionPath) { Remove-Item -LiteralPath $projectionPath }
}
