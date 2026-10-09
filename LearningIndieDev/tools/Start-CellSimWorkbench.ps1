[CmdletBinding()]
param([switch]$BuildOnly)
Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$projectRoot = Split-Path $PSScriptRoot
$source = Join-Path $PSScriptRoot 'CellSim.Workbench'
$destination = Join-Path $projectRoot 'artifacts/cellsim-workbench'
$exe = Join-Path $destination 'CellSim.Workbench.exe'
$runnerDestination = Join-Path $destination 'runner'
$runnerProject = Join-Path $PSScriptRoot 'CellSim.Batch/CellSim.Batch.csproj'
$runnerDll = Join-Path $runnerDestination 'CellSim.Batch.dll'
$runnerInputs = @(Get-ChildItem -LiteralPath (Split-Path $runnerProject) -File | Where-Object { $_.Extension -in '.cs', '.csproj' })
$runnerXml = [xml](Get-Content -LiteralPath $runnerProject -Raw)
$runnerInputs += @($runnerXml.Project.ItemGroup.SharedSource | ForEach-Object { Get-Item -LiteralPath (Join-Path (Split-Path $runnerProject) $_.Include) })
$runnerLatest = $runnerInputs | Sort-Object LastWriteTimeUtc -Descending | Select-Object -First 1
# Keep the historical runner's bin directory intact; every prepared GUI run pins its own copy.
if ($BuildOnly -or -not (Test-Path -LiteralPath $runnerDll) -or $runnerLatest.LastWriteTimeUtc -gt (Get-Item -LiteralPath $runnerDll).LastWriteTimeUtc) {
    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) { throw 'Building the simulation runner needs the .NET 8 SDK. Install it or use a prebuilt Workbench folder.' }
    & dotnet build $runnerProject -c Release -o $runnerDestination
    if ($LASTEXITCODE -ne 0) { throw 'The Workbench simulation runner did not build.' }
}
$latest = Get-ChildItem -LiteralPath $source -File | Where-Object { $_.Extension -in '.cs', '.csproj' } | Sort-Object LastWriteTimeUtc -Descending | Select-Object -First 1
if ($BuildOnly -or -not (Test-Path -LiteralPath $exe) -or $latest.LastWriteTimeUtc -gt (Get-Item -LiteralPath $exe).LastWriteTimeUtc) {
    if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) { throw 'Building CellSim Workbench needs the .NET 8 SDK. Ask for a prebuilt copy, or install the SDK and run this launcher again.' }
    & dotnet publish (Join-Path $source 'CellSim.Workbench.csproj') -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -p:DebugType=None -o $destination
    if ($LASTEXITCODE -ne 0) { throw 'CellSim Workbench did not build. See the build output above.' }
}
if (-not $BuildOnly) {
    Start-Process -FilePath $exe -ArgumentList @('--project', ('"' + $projectRoot + '"')) -WindowStyle Normal
}
Write-Output "CellSim Workbench: $exe"
