[CmdletBinding()]
param(
    [string]$ReportPath,
    [string]$BaselinePath,
    [string]$TestArtifactDirectory,
    [string]$OutputPath,
    [string]$ProjectPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Resolve-CellSimPath {
    param(
        [Parameter(Mandatory)]
        [string]$Path,
        [Parameter(Mandatory)]
        [string]$ProjectRoot
    )

    if (Test-Path -LiteralPath $Path -PathType Leaf) {
        return (Resolve-Path -LiteralPath $Path).Path
    }

    $projectPath = Join-Path $ProjectRoot $Path
    if (Test-Path -LiteralPath $projectPath -PathType Leaf) {
        return (Resolve-Path -LiteralPath $projectPath).Path
    }

    throw "Could not find '$Path'."
}

function Get-LatestExperimentReport {
    param([string]$ProjectRoot)

    $artifactsRoot = Join-Path $ProjectRoot 'artifacts'
    $latest = Get-ChildItem -LiteralPath $artifactsRoot -Recurse -Filter 'report.json' -File -ErrorAction SilentlyContinue |
        Sort-Object LastWriteTime -Descending |
        Select-Object -First 1
    if ($null -eq $latest) {
        throw "No cellular experiment report was found under '$artifactsRoot'. Run 'CellSim Run' first."
    }

    return $latest.FullName
}

function Get-Population {
    param(
        [object]$Snapshot,
        [string]$SpeciesId
    )

    $entry = @($Snapshot.species | Where-Object { $_.speciesId -eq $SpeciesId } | Select-Object -First 1)
    if ($entry.Count -eq 1) {
        return [int]$entry[0].population
    }

    return 0
}

function Get-FinalPopulation {
    param(
        [object]$Run,
        [string]$SpeciesId
    )

    $history = @($Run.populationHistory)
    if ($history.Count -gt 0) {
        return Get-Population -Snapshot $history[-1] -SpeciesId $SpeciesId
    }

    return 0
}

function Get-SnapshotAtTick {
    param(
        [object]$Run,
        [int]$Tick
    )

    return @($Run.populationHistory | Where-Object { $_.tick -eq $Tick } | Select-Object -First 1)
}

function Get-Number {
    param([double]$Value)

    return $Value.ToString('0.##', [System.Globalization.CultureInfo]::InvariantCulture)
}

function Add-MarkdownTable {
    param(
        [System.Collections.Generic.List[string]]$Lines,
        [string[]]$Headers,
        [object[][]]$Rows
    )

    $Lines.Add('| ' + ($Headers -join ' | ') + ' |')
    $Lines.Add('| ' + (($Headers | ForEach-Object { '---' }) -join ' | ') + ' |')
    foreach ($row in $Rows) {
        $Lines.Add('| ' + (($row | ForEach-Object { "$($_)" }) -join ' | ') + ' |')
    }
}

function Add-TestResults {
    param(
        [System.Collections.Generic.List[string]]$Lines,
        [string]$ArtifactDirectory
    )

    if ([string]::IsNullOrWhiteSpace($ArtifactDirectory)) {
        return
    }

    $xmlFiles = Get-ChildItem -LiteralPath $ArtifactDirectory -Filter '*-results.xml' -File -ErrorAction SilentlyContinue |
        Sort-Object Name
    if ($xmlFiles.Count -eq 0) {
        $Lines.Add('## Test suite')
        $Lines.Add('')
        $Lines.Add(('No NUnit XML results were found in `{0}`.' -f $ArtifactDirectory))
        $Lines.Add('')
        return
    }

    $rows = [System.Collections.Generic.List[object[]]]::new()
    foreach ($xmlFile in $xmlFiles) {
        [xml]$xml = Get-Content -LiteralPath $xmlFile.FullName -Raw
        $node = if ($xml.DocumentElement.Name -eq 'test-run') { $xml.DocumentElement } else { $xml.SelectSingleNode('//test-run') }
        $rows.Add(@(
            ($xmlFile.BaseName -replace '-results$', ''),
            $node.GetAttribute('total'),
            $node.GetAttribute('passed'),
            $node.GetAttribute('failed'),
            $node.GetAttribute('skipped')
        ))
    }

    $Lines.Add('## Test suite')
    $Lines.Add('')
    Add-MarkdownTable -Lines $Lines -Headers @('Platform', 'Total', 'Passed', 'Failed', 'Skipped') -Rows $rows.ToArray()
    $Lines.Add('')
}

function Add-Comparison {
    param(
        [System.Collections.Generic.List[string]]$Lines,
        [object]$Report,
        [object]$Baseline
    )

    $baselineBySpecies = @{}
    foreach ($entry in @($Baseline.finalPopulationSummary)) {
        $baselineBySpecies[$entry.speciesId] = $entry
    }

    $reportBySpecies = @{}
    foreach ($entry in @($Report.finalPopulationSummary)) {
        $reportBySpecies[$entry.speciesId] = $entry
    }

    $species = @($baselineBySpecies.Keys + $reportBySpecies.Keys | Sort-Object -Unique)
    $rows = [System.Collections.Generic.List[object[]]]::new()
    foreach ($speciesId in $species) {
        $baselineEntry = $baselineBySpecies[$speciesId]
        $reportEntry = $reportBySpecies[$speciesId]
        $baselineAverage = if ($null -ne $baselineEntry) { [double]$baselineEntry.averageFinalPopulation } else { 0d }
        $reportAverage = if ($null -ne $reportEntry) { [double]$reportEntry.averageFinalPopulation } else { 0d }
        $baselineExtinction = if ($null -ne $baselineEntry) { [double]$baselineEntry.finalExtinctionRate } else { 0d }
        $reportExtinction = if ($null -ne $reportEntry) { [double]$reportEntry.finalExtinctionRate } else { 0d }
        $rows.Add(@(
            $speciesId,
            (Get-Number $baselineAverage),
            (Get-Number $reportAverage),
            (Get-Number ($reportAverage - $baselineAverage)),
            ((Get-Number ($baselineExtinction * 100)) + '%'),
            ((Get-Number ($reportExtinction * 100)) + '%'),
            ((Get-Number (($reportExtinction - $baselineExtinction) * 100)) + ' pp')
        ))
    }

    $Lines.Add('## Comparison to baseline')
    $Lines.Add('')
    $Lines.Add(('Baseline fingerprint: `{0}`' -f $Baseline.rulesetFingerprint))
    $Lines.Add(('Trial fingerprint: `{0}`' -f $Report.rulesetFingerprint))
    if ($Baseline.seedStart -eq $Report.seedStart -and $Baseline.seedCount -eq $Report.seedCount) {
        $Lines.Add('Comparison validity: controlled seed range.')
    }
    else {
        $Lines.Add('Comparison validity: seed ranges differ; treat deltas as descriptive only, not A/B balance evidence.')
    }
    $Lines.Add('')
    Add-MarkdownTable -Lines $Lines -Headers @('Species', 'Baseline avg.', 'Trial avg.', 'Delta', 'Baseline extinction', 'Trial extinction', 'Delta') -Rows $rows.ToArray()
    $Lines.Add('')
}

if ([string]::IsNullOrWhiteSpace($ProjectPath)) {
    $ProjectPath = Join-Path $PSScriptRoot '..'
}

$project = (Resolve-Path -LiteralPath $ProjectPath).Path
if ([string]::IsNullOrWhiteSpace($ReportPath)) {
    $ReportPath = Get-LatestExperimentReport -ProjectRoot $project
}

$resolvedReportPath = Resolve-CellSimPath -Path $ReportPath -ProjectRoot $project
$report = Get-Content -LiteralPath $resolvedReportPath -Raw | ConvertFrom-Json
if ($null -eq $report.runs -or $null -eq $report.finalPopulationSummary) {
    throw "'$resolvedReportPath' is not a CellSim experiment report."
}

$resolvedBaselinePath = $null
$baseline = $null
if (-not [string]::IsNullOrWhiteSpace($BaselinePath)) {
    $resolvedBaselinePath = Resolve-CellSimPath -Path $BaselinePath -ProjectRoot $project
    $baseline = Get-Content -LiteralPath $resolvedBaselinePath -Raw | ConvertFrom-Json
}

if ([string]::IsNullOrWhiteSpace($OutputPath)) {
    $OutputPath = Join-Path (Split-Path -Parent $resolvedReportPath) 'analysis.md'
}

$outputDirectory = Split-Path -Parent $OutputPath
if (-not [string]::IsNullOrWhiteSpace($outputDirectory)) {
    New-Item -ItemType Directory -Path $outputDirectory -Force | Out-Null
}

$species = @($report.finalPopulationSummary | ForEach-Object { $_.speciesId } | Sort-Object)
$lines = [System.Collections.Generic.List[string]]::new()
$lines.Add('# CellSim experiment report')
$lines.Add('')
$lines.Add("Generated: $([DateTime]::UtcNow.ToString('O'))")
$lines.Add(('Source: `{0}`' -f $resolvedReportPath))
$lines.Add('')
$lines.Add('## Scenario')
$lines.Add('')
$lines.Add(('- Ruleset fingerprint: `{0}`' -f $report.rulesetFingerprint))
$lines.Add(('- Scenario asset: `{0}`' -f $report.scenarioAssetPath))
$lines.Add("- Seeds: $($report.seedStart) through $($report.seedStart + $report.seedCount - 1) ($($report.seedCount) runs)")
$lines.Add("- Grid: $($report.gridWidth) x $($report.gridHeight); duration: $(Get-Number $report.runDurationSeconds)s; step: $(Get-Number $report.stepIntervalSeconds)s")
$lines.Add(('- Player species: `{0}`' -f $report.playerSpeciesId))
$lines.Add('')
$lines.Add('## Final population summary')
$lines.Add('')
$summaryRows = [System.Collections.Generic.List[object[]]]::new()
foreach ($entry in @($report.finalPopulationSummary | Sort-Object speciesId)) {
    $summaryRows.Add(@(
        $entry.speciesId,
        (Get-Number $entry.averageFinalPopulation),
        $entry.minimumFinalPopulation,
        $entry.maximumFinalPopulation,
        "$($entry.extinctFinalRuns)/$($report.seedCount)",
        ((Get-Number ([double]$entry.finalExtinctionRate * 100)) + '%')
    ))
}
Add-MarkdownTable -Lines $lines -Headers @('Species', 'Average final', 'Min', 'Max', 'Extinct runs', 'Extinction rate') -Rows $summaryRows.ToArray()
$lines.Add('')
$lines.Add('## Average population trajectory')
$lines.Add('')
$maximumTick = @($report.runs | ForEach-Object { $_.ticks } | Measure-Object -Maximum).Maximum
$trajectoryTicks = @(0, [int][Math]::Floor($maximumTick / 2), $maximumTick) | Select-Object -Unique
$trajectoryHeaders = [System.Collections.Generic.List[string]]::new()
$trajectoryHeaders.Add('Stage')
$trajectoryHeaders.Add('Tick')
foreach ($speciesId in $species) { $trajectoryHeaders.Add($speciesId) }
$trajectoryRows = [System.Collections.Generic.List[object[]]]::new()
foreach ($tick in $trajectoryTicks) {
    $stage = if ($tick -eq 0) { 'Start' } elseif ($tick -eq $maximumTick) { 'End' } else { 'Midpoint' }
    $row = [System.Collections.Generic.List[object]]::new()
    $row.Add($stage)
    $row.Add($tick)
    foreach ($speciesId in $species) {
        $total = 0
        foreach ($run in @($report.runs)) {
            $snapshot = @(Get-SnapshotAtTick -Run $run -Tick $tick)
            if ($snapshot.Count -eq 1) {
                $total += Get-Population -Snapshot $snapshot[0] -SpeciesId $speciesId
            }
        }

        $row.Add((Get-Number ($total / [double]$report.seedCount)))
    }

    $trajectoryRows.Add($row.ToArray())
}
Add-MarkdownTable -Lines $lines -Headers $trajectoryHeaders.ToArray() -Rows $trajectoryRows.ToArray()
$lines.Add('')
$lines.Add('## Per-seed outcomes')
$lines.Add('')
$headers = [System.Collections.Generic.List[string]]::new()
$headers.Add('Seed')
$headers.Add('Player final')
$headers.Add('Currency')
foreach ($speciesId in $species) { $headers.Add($speciesId) }
$runRows = [System.Collections.Generic.List[object[]]]::new()
foreach ($run in @($report.runs | Sort-Object seed)) {
    $row = [System.Collections.Generic.List[object]]::new()
    $row.Add($run.seed)
    $row.Add($run.playerPopulation)
    $row.Add($run.currencyEarned)
    foreach ($speciesId in $species) { $row.Add((Get-FinalPopulation -Run $run -SpeciesId $speciesId)) }
    $runRows.Add($row.ToArray())
}
Add-MarkdownTable -Lines $lines -Headers $headers.ToArray() -Rows $runRows.ToArray()
$lines.Add('')

Add-TestResults -Lines $lines -ArtifactDirectory $TestArtifactDirectory
if ($null -ne $baseline) {
    Add-Comparison -Lines $lines -Report $report -Baseline $baseline
}

Set-Content -LiteralPath $OutputPath -Value $lines -Encoding utf8
[pscustomobject]@{
    Report = $resolvedReportPath
    Analysis = (Resolve-Path -LiteralPath $OutputPath).Path
    Baseline = $resolvedBaselinePath
}
