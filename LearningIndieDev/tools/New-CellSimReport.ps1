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

function Get-ActivityValue {
    param(
        [object]$Run,
        [string]$SpeciesId,
        [string]$Property
    )

    $entry = @($Run.activity | Where-Object { $_.speciesId -eq $SpeciesId } | Select-Object -First 1)
    if ($entry.Count -ne 1) {
        return 0d
    }

    $propertyValue = $entry[0].PSObject.Properties[$Property]
    if ($null -eq $propertyValue -or $null -eq $propertyValue.Value) {
        return 0d
    }

    return [double]$propertyValue.Value
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
$lines.Add('## Average activity per run')
$lines.Add('')
$activityRows = [System.Collections.Generic.List[object[]]]::new()
foreach ($speciesId in $species) {
    $births = 0d
    $foodConsumed = 0d
    $movementSteps = 0d
    $damageDealt = 0d
    $combatKills = 0d
    foreach ($run in @($report.runs)) {
        $births += (Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'births')
        $foodConsumed += (Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'foodConsumed')
        $movementSteps += (Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'movementSteps')
        $damageDealt += (Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'damageDealt')
        $combatKills += (Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'combatKills')
    }

    $activityRows.Add(@(
        $speciesId,
        (Get-Number ($births / [double]$report.seedCount)),
        (Get-Number ($foodConsumed / [double]$report.seedCount)),
        (Get-Number ($movementSteps / [double]$report.seedCount)),
        (Get-Number ($damageDealt / [double]$report.seedCount)),
        (Get-Number ($combatKills / [double]$report.seedCount))
    ))
}
Add-MarkdownTable -Lines $lines -Headers @('Species', 'Births', 'Food consumed', 'Movement steps', 'Damage dealt', 'Combat kills') -Rows $activityRows.ToArray()
$lines.Add('')
$lines.Add('Births include successful plant seed drops.')
$lines.Add('')
$lines.Add('Food consumed is the resource amount actually withdrawn; one consumed creature counts as one unit.')
$lines.Add('')
$lines.Add('## Average reproduction funnel per run')
$lines.Add('')
$reproductionRows = [System.Collections.Generic.List[object[]]]::new()
foreach ($speciesId in $species) {
    $candidates = 0d
    $blockedEnergy = 0d
    $blockedMate = 0d
    $blockedGroup = 0d
    $failedChance = 0d
    $blockedNoSpace = 0d
    $successfulAttempts = 0d
    $births = 0d
    $allRunsReconciled = $true
    foreach ($run in @($report.runs)) {
        $candidateValue = Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'reproductionCandidates'
        $energyValue = Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'reproductionBlockedEnergy'
        $mateValue = Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'reproductionBlockedMateRequirement'
        $groupValue = Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'reproductionBlockedGroupLimit'
        $chanceValue = Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'reproductionFailedChanceRoll'
        $spaceValue = Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'reproductionBlockedNoBirthLocation'
        $successValue = Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'reproductionSuccessfulAttempts'
        $candidates += $candidateValue
        $blockedEnergy += $energyValue
        $blockedMate += $mateValue
        $blockedGroup += $groupValue
        $failedChance += $chanceValue
        $blockedNoSpace += $spaceValue
        $successfulAttempts += $successValue
        $births += Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'births'
        if ($candidateValue -ne ($energyValue + $mateValue + $groupValue + $chanceValue + $spaceValue + $successValue)) {
            $allRunsReconciled = $false
        }
    }

    $reproductionRows.Add(@(
        $speciesId,
        (Get-Number ($candidates / [double]$report.seedCount)),
        (Get-Number ($blockedEnergy / [double]$report.seedCount)),
        (Get-Number ($blockedMate / [double]$report.seedCount)),
        (Get-Number ($blockedGroup / [double]$report.seedCount)),
        (Get-Number ($failedChance / [double]$report.seedCount)),
        (Get-Number ($blockedNoSpace / [double]$report.seedCount)),
        (Get-Number ($successfulAttempts / [double]$report.seedCount)),
        (Get-Number ($births / [double]$report.seedCount)),
        $allRunsReconciled
    ))
}
Add-MarkdownTable -Lines $lines -Headers @('Species', 'Candidates', 'Energy', 'Mate', 'Group cap', 'Chance', 'No space', 'Successes', 'Births', 'Reconciled') -Rows $reproductionRows.ToArray()
$lines.Add('')
$lines.Add('Each reproduction candidate is classified once by the first resolver gate that prevents offspring, or as a successful attempt when at least one birth is created. Mating behavior ticks are decision intent and are not expected to equal candidate evaluations.')
$lines.Add('')
$lines.Add('## Average mortality per run')
$lines.Add('')
$mortalityRows = [System.Collections.Generic.List[object[]]]::new()
foreach ($speciesId in $species) {
    $deaths = 0d
    $starvationDeaths = 0d
    $crowdingDeaths = 0d
    $wiltDeaths = 0d
    $populationLimitRemovals = 0d
    foreach ($run in @($report.runs)) {
        $deaths += (Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'deaths')
        $starvationDeaths += (Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'starvationDeaths')
        $crowdingDeaths += (Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'crowdingDeaths')
        $wiltDeaths += (Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'wiltDeaths')
        $populationLimitRemovals += (Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'populationLimitRemovals')
    }

    $mortalityRows.Add(@(
        $speciesId,
        (Get-Number ($deaths / [double]$report.seedCount)),
        (Get-Number ($starvationDeaths / [double]$report.seedCount)),
        (Get-Number ($crowdingDeaths / [double]$report.seedCount)),
        (Get-Number ($wiltDeaths / [double]$report.seedCount)),
        (Get-Number ($populationLimitRemovals / [double]$report.seedCount))
    ))
}
Add-MarkdownTable -Lines $lines -Headers @('Species', 'Deaths', 'Starvation', 'Crowding', 'Wilt', 'Population cap') -Rows $mortalityRows.ToArray()
$lines.Add('')
$lines.Add('The JSON report also contains one `deathEvents` record per resolved removal, including proximate cause, tick, position, and entity/resource identity. Root-cause links such as preceding resource state or attacker identity are not inferred.')
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
