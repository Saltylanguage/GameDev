[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$OutputPath,
    [string]$ScenarioPath = 'Assets/Data/ProductionData/CellularSimulation/Scenarios/ForestEdge.asset',
    [string[]]$ScientificArguments = @('-playerSpeciesId', 'hare', '-runTicks', '700', '-combatMode', 'opposed-roll', '-experimentalFeatures', 'bev-experimental'),
    [ValidateRange(0, 2147483647)][int]$SeedStart = 1,
    [ValidateRange(1, 1000)][int]$ReferenceSeeds = 4,
    [string]$UnityPath
)

. (Join-Path $PSScriptRoot 'UnityTooling.ps1')
$project = Resolve-UnityProjectPath -ProjectPath (Join-Path $PSScriptRoot '..')
if (Test-Path -LiteralPath (Join-Path $project 'Temp/UnityLockfile')) {
    throw 'This project is open or locked. Save and close its Editor before exporting a frozen snapshot.'
}
$editor = Resolve-UnityEditorPath -ProjectPath $project -UnityPath $UnityPath
$output = [IO.Path]::GetFullPath($OutputPath)
$artifacts = [IO.Path]::GetFullPath((Join-Path $project 'artifacts')) + [IO.Path]::DirectorySeparatorChar
if (-not $output.StartsWith($artifacts, [StringComparison]::OrdinalIgnoreCase)) {
    throw 'Snapshot output must stay under this project artifacts directory.'
}
if (Test-Path -LiteralPath $output) { throw 'Snapshot already exists; use a new output path.' }
if ($ScientificArguments.Count % 2 -ne 0) { throw 'ScientificArguments must contain option/value pairs.' }
$allowed = @('-playerSpeciesId','-upgradeId','-upgradeSequence','-upgradeAssetSequence','-upgradeAssetCatalogPath',
    '-upgradeValueOverride','-combatMode','-attackOpportunityMode','-experimentalFeatures','-foxAttackCooldownTicks',
    '-preContactAvoidanceChance','-coupledSpeciesResponses','-wrapEdges','-gridWidth','-gridHeight','-startingPopulations',
    '-runTicks','-phaseLengthTicks','-phaseUpgradeSchedule','-phaseUpgradeAssetSchedule','-mutationPolicy',
    '-mutationPolicyEarlyFirstChoice','-harePurchasePolicy','-runDurationSeconds','-stepIntervalSeconds','-speciesStats')
$seen = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
for ($i = 0; $i -lt $ScientificArguments.Count; $i += 2) {
    if ($allowed -cnotcontains $ScientificArguments[$i] -or -not $seen.Add($ScientificArguments[$i]) -or
        [string]::IsNullOrWhiteSpace($ScientificArguments[$i + 1])) {
        throw "Unknown, duplicate or empty scientific option: $($ScientificArguments[$i])"
    }
}
if (Test-Path -LiteralPath ([IO.Path]::ChangeExtension($output, '.reference.jsonl'))) { throw 'Reference already exists; use a new output path.' }
[IO.Directory]::CreateDirectory([IO.Path]::GetDirectoryName($output)) | Out-Null
$log = [IO.Path]::ChangeExtension($output, '.unity.log')
$commit = (git -C $project rev-parse HEAD).Trim()
$arguments = @('run', $project, '--editor-path', $editor, '--timeout', '600',
    '--no-banner', '--non-interactive', '--format', 'json', '--', '-nographics',
    '-executeMethod', 'SaltyGame.EditorTools.CellularSimulationExperimentRunner.ExportPortableFromCommandLine',
    '-logFile', $log, '-scenarioPath', $ScenarioPath, '-portableOutput', $output,
    '-portableSourceCommit', $commit, '-seedStart', [string]$SeedStart, '-seedCount', [string]$ReferenceSeeds) + $ScientificArguments
$cli = Resolve-UnityCliPath
$cliOutput = & $cli @arguments
$cliExitCode = $LASTEXITCODE
$cliOutput | Set-Content -LiteralPath ([IO.Path]::ChangeExtension($output, '.unity-cli.log')) -Encoding utf8
if ($cliExitCode -ne 0 -or -not (Test-Path -LiteralPath $output) -or
    -not (Test-Path -LiteralPath ([IO.Path]::ChangeExtension($output, '.reference.jsonl')))) {
    throw "Portable export/reference failed. Inspect '$log'."
}
[pscustomobject]@{ Snapshot = $output; Reference = [IO.Path]::ChangeExtension($output, '.reference.jsonl'); UnityLog = $log }
