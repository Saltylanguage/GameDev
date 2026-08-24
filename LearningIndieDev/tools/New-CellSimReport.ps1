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

function Get-OpportunityControlValue {
    param(
        [object]$Run,
        [string]$Property
    )

    $control = $Run.opportunityControl
    if ($null -eq $control) {
        return 0d
    }

    $propertyValue = $control.PSObject.Properties[$Property]
    if ($null -eq $propertyValue -or $null -eq $propertyValue.Value) {
        return 0d
    }

    return [double]$propertyValue.Value
}

function Get-OpposedHitProbability {
    param(
        [int]$AttackModifier,
        [int]$BlockModifier
    )

    $winningRolls = 0
    for ($attackRoll = 1; $attackRoll -le 20; $attackRoll++) {
        for ($blockRoll = 1; $blockRoll -le 20; $blockRoll++) {
            if (($attackRoll + $AttackModifier) -gt ($blockRoll + $BlockModifier)) {
                $winningRolls++
            }
        }
    }

    return $winningRolls / 400d
}

function Get-Number {
    param([double]$Value)

    return $Value.ToString('0.##', [System.Globalization.CultureInfo]::InvariantCulture)
}

function Get-StatMetricDisplay {
    param(
        [object]$Stat,
        [string]$ValueProperty,
        [string]$StatusProperty
    )

    $status = $Stat.PSObject.Properties[$StatusProperty]
    if ($null -ne $status -and -not [string]::IsNullOrWhiteSpace([string]$status.Value)) {
        if ([string]$status.Value -ne 'Valid') {
            return [string]$status.Value
        }
    }

    $value = $Stat.PSObject.Properties[$ValueProperty]
    if ($null -eq $value -or $null -eq $value.Value) {
        return 'N/A'
    }

    return Get-Number ([double]$value.Value)
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
$lines.Add(('- Attack opportunity mode: `{0}`' -f $report.attackOpportunityMode))
$lines.Add('')

$scheduledOpportunities = 0d
$eligibleOpportunities = 0d
$unfulfilledNoTarget = 0d
$unfulfilledInvalidated = 0d
$baselineValidOpportunities = 0d
$blockPlusTwoValidOpportunities = 0d
$commonValidOpportunities = 0d
$baselineOnlyOpportunities = 0d
$blockPlusTwoOnlyOpportunities = 0d
$pairedAttempts = 0d
$pairedMismatches = 0d
$baselineCandidateContacts = 0d
$blockPlusTwoCandidateContacts = 0d
$commonCandidateContacts = 0d
$unionCandidateContacts = 0d
foreach ($run in @($report.runs)) {
    $scheduledOpportunities += Get-OpportunityControlValue -Run $run -Property 'scheduled'
    $eligibleOpportunities += Get-OpportunityControlValue -Run $run -Property 'eligible'
    $unfulfilledNoTarget += Get-OpportunityControlValue -Run $run -Property 'unfulfilledNoTarget'
    $unfulfilledInvalidated += Get-OpportunityControlValue -Run $run -Property 'unfulfilledInvalidated'
    $baselineValidOpportunities += Get-OpportunityControlValue -Run $run -Property 'baselineValid'
    $blockPlusTwoValidOpportunities += Get-OpportunityControlValue -Run $run -Property 'blockPlusTwoValid'
    $commonValidOpportunities += Get-OpportunityControlValue -Run $run -Property 'commonValid'
    $baselineOnlyOpportunities += Get-OpportunityControlValue -Run $run -Property 'baselineOnly'
    $blockPlusTwoOnlyOpportunities += Get-OpportunityControlValue -Run $run -Property 'blockPlusTwoOnly'
    $pairedAttempts += Get-OpportunityControlValue -Run $run -Property 'pairedAttempts'
    $pairedMismatches += Get-OpportunityControlValue -Run $run -Property 'pairedMismatches'
    $baselineCandidateContacts += Get-OpportunityControlValue -Run $run -Property 'baselineCandidateCount'
    $blockPlusTwoCandidateContacts += Get-OpportunityControlValue -Run $run -Property 'blockPlusTwoCandidateCount'
    $commonCandidateContacts += Get-OpportunityControlValue -Run $run -Property 'commonCandidateCount'
    $unionCandidateContacts += Get-OpportunityControlValue -Run $run -Property 'unionCandidateCount'
}
if ($scheduledOpportunities -gt 0) {
    $lines.Add('## Controlled opportunity exposure')
    $lines.Add('')
    if ($report.attackOpportunityMode -eq 'PairedLockstepDiagnostic') {
        $pairRows = [System.Collections.Generic.List[object[]]]::new()
        $pairRows.Add(@(
            $baselineValidOpportunities,
            $blockPlusTwoValidOpportunities,
            $commonValidOpportunities,
            $baselineOnlyOpportunities,
            $blockPlusTwoOnlyOpportunities,
            $pairedAttempts,
            $pairedMismatches,
            (($commonValidOpportunities -eq $pairedAttempts) -and ($pairedMismatches -eq 0))
        ))
        Add-MarkdownTable -Lines $lines -Headers @('Baseline valid', 'Block+2 valid', 'Common valid', 'Baseline-only', 'Block+2-only', 'Paired attempts', 'Mismatches', 'Reconciled') -Rows $pairRows.ToArray()
        $commonCandidateFraction = if ($unionCandidateContacts -eq 0) { 0d } else { $commonCandidateContacts / $unionCandidateContacts }
        $candidateRows = [System.Collections.Generic.List[object[]]]::new()
        $candidateRows.Add(@(
            $baselineCandidateContacts,
            $blockPlusTwoCandidateContacts,
            $commonCandidateContacts,
            $unionCandidateContacts,
            $commonCandidateFraction
        ))
        Add-MarkdownTable -Lines $lines -Headers @('Baseline candidate contacts', 'Block+2 candidate contacts', 'Common candidate contacts', 'Union candidate contacts', 'Common / union') -Rows $candidateRows.ToArray()
        $lines.Add('Paired lockstep uses coordinate/species/contact identities and executes one shared common contact slot in both worlds. Candidate-contact counts quantify the intersection censoring separately from the exact paired-attempt gate.')
    }
    else {
        $controlRows = [System.Collections.Generic.List[object[]]]::new()
        $controlRows.Add(@(
            $scheduledOpportunities,
            $eligibleOpportunities,
            $unfulfilledNoTarget,
            $unfulfilledInvalidated,
            (($scheduledOpportunities - $eligibleOpportunities) -eq ($unfulfilledNoTarget + $unfulfilledInvalidated))
        ))
        Add-MarkdownTable -Lines $lines -Headers @('Scheduled', 'Eligible', 'Unfulfilled: no target', 'Unfulfilled: invalidated', 'Reconciled') -Rows $controlRows.ToArray()
        $lines.Add('')
        $lines.Add('Scheduled slots are deterministic fixed-rate diagnostic exposure; eligible slots had a live Fox-to-diet-target candidate in the current arm. Unfulfilled slots are never silently counted as attack attempts.')
    }
    $lines.Add('')
}
if ($report.experimentalFeatures -eq 'bev-experimental') {
    $statRuns = @($report.runs | Where-Object {
        $statProperty = $_.PSObject.Properties['herbivoreStatLine']
        $null -ne $statProperty -and $null -ne $statProperty.Value
    } | Sort-Object seed)
    if ($statRuns.Count -gt 0) {
        $lines.Add('## Experimental herbivore stat line')
        $lines.Add('')
        $statHeaders = @('Seed', 'Species', 'SPO', 'ECN', 'PREY', 'STRV', 'MAT', 'BIR', 'CRWD', 'FPO', 'Expected FPO', 'FPO reconciled', 'pAVI', 'sAVI', 'cAVI', 'bAVG', 'RFS', 'APS')
        $statRows = [System.Collections.Generic.List[object[]]]::new()
        foreach ($run in $statRuns) {
            $stat = $run.herbivoreStatLine
            $crowdingProperty = $stat.PSObject.Properties['CRWD']
            $crowding = if ($null -eq $crowdingProperty -or $null -eq $crowdingProperty.Value) {
                0
            } else {
                $crowdingProperty.Value
            }
            $statRows.Add(@(
                $run.seed,
                $stat.speciesId,
                $stat.SPO,
                $stat.ECN,
                $stat.PREY,
                $stat.STRV,
                $stat.MAT,
                $stat.BIR,
                $crowding,
                $stat.FPO,
                $stat.expectedFPO,
                $stat.fpoReconciled,
                (Get-StatMetricDisplay -Stat $stat -ValueProperty 'pAVI' -StatusProperty 'pAVIStatus'),
                (Get-StatMetricDisplay -Stat $stat -ValueProperty 'sAVI' -StatusProperty 'sAVIStatus'),
                (Get-StatMetricDisplay -Stat $stat -ValueProperty 'cAVI' -StatusProperty 'cAVIStatus'),
                (Get-StatMetricDisplay -Stat $stat -ValueProperty 'bAVG' -StatusProperty 'bAVGStatus'),
                (Get-StatMetricDisplay -Stat $stat -ValueProperty 'RFS' -StatusProperty 'RFSStatus'),
                (Get-StatMetricDisplay -Stat $stat -ValueProperty 'APS' -StatusProperty 'APSStatus')
            ))
        }
        Add-MarkdownTable -Lines $lines -Headers $statHeaders -Rows $statRows.ToArray()
        $lines.Add('')
        $lines.Add('This opt-in stat line is emitted only for a herbivore player species. N/A means zero exposure or opportunity with a zero numerator. INVALID means positive deaths with zero exposure, negative exposure, over-counted deaths, or an FPO reconciliation failure. APS treats N/A as neutral contribution (RFS and pAVI contribute 0; sAVI and cAVI penalties contribute 0) but remains INVALID when a component or FPO reconciliation is invalid.')
        $lines.Add('')
    }
}
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
    $foodActionAttempts = 0d
    $foodActionSuccesses = 0d
    $foodActionFailures = 0d
    $foodActionsReconciled = $true
    $movementSteps = 0d
    $damageDealt = 0d
    $combatKills = 0d
    $combatOpportunities = 0d
    $combatAttempts = 0d
    $combatHits = 0d
    $combatBlocked = 0d
    $combatDamageApplications = 0d
    $combatNonLethalHits = 0d
    $combatLethalHits = 0d
    foreach ($run in @($report.runs)) {
        $births += (Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'births')
        $foodConsumed += (Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'foodConsumed')
        $attempts = Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'foodActionAttempts'
        $successes = Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'foodActionSuccesses'
        $failures = Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'foodActionFailures'
        $foodActionAttempts += $attempts
        $foodActionSuccesses += $successes
        $foodActionFailures += $failures
        if ($attempts -ne ($successes + $failures)) {
            $foodActionsReconciled = $false
        }
        $movementSteps += (Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'movementSteps')
        $damageDealt += (Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'damageDealt')
        $combatKills += (Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'combatKills')
        $combatOpportunities += (Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'combatOpportunities')
        $combatAttempts += (Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'combatAttempts')
        $combatHits += (Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'combatHits')
        $combatBlocked += (Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'combatBlocked')
        $combatDamageApplications += (Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'combatDamageApplications')
        $combatNonLethalHits += (Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'combatNonLethalHits')
        $combatLethalHits += (Get-ActivityValue -Run $run -SpeciesId $speciesId -Property 'combatLethalHits')
    }

    $activityRows.Add(@(
        $speciesId,
        (Get-Number ($births / [double]$report.seedCount)),
        (Get-Number ($foodConsumed / [double]$report.seedCount)),
        (Get-Number ($foodActionAttempts / [double]$report.seedCount)),
        (Get-Number ($foodActionSuccesses / [double]$report.seedCount)),
        (Get-Number ($foodActionFailures / [double]$report.seedCount)),
        $foodActionsReconciled,
        (Get-Number ($movementSteps / [double]$report.seedCount)),
        (Get-Number ($damageDealt / [double]$report.seedCount)),
        (Get-Number ($combatKills / [double]$report.seedCount)),
        (Get-Number ($combatOpportunities / [double]$report.seedCount)),
        (Get-Number ($combatAttempts / [double]$report.seedCount)),
        (Get-Number ($combatHits / [double]$report.seedCount)),
        (Get-Number ($combatBlocked / [double]$report.seedCount)),
        (Get-Number ($combatDamageApplications / [double]$report.seedCount)),
        (Get-Number ($combatNonLethalHits / [double]$report.seedCount)),
        (Get-Number ($combatLethalHits / [double]$report.seedCount)),
        (($combatHits + $combatBlocked) -eq $combatAttempts)
    ))
}
Add-MarkdownTable -Lines $lines -Headers @('Species', 'Births', 'Food consumed', 'Food attempts', 'Food successes', 'Food failures', 'Food actions reconciled', 'Movement steps', 'Damage dealt', 'Combat kills', 'Combat opportunities', 'Combat attempts', 'Combat hits', 'Combat blocked', 'Combat damage applications', 'Combat non-lethal hits', 'Combat lethal hits', 'Combat reconciled') -Rows $activityRows.ToArray()
$lines.Add('')
$lines.Add('Births include successful plant seed drops.')
$lines.Add('')
$lines.Add('Food consumed is the resource amount actually withdrawn; one consumed creature counts as one unit.')
$lines.Add('')
$lines.Add('Food attempts are eligible diet-target resolutions; successes must reconcile with failures as attempts = successes + failures.')
$lines.Add('')
$lines.Add('Combat opportunities are creature diet-targets found in the attack pattern; combat attempts are targets still present when the attack resolves. Hits and blocked rolls reconcile against attempts for opposed-roll diagnostics.')
$lines.Add('')
$lines.Add('## Experimental combat diagnostics')
$lines.Add('')
$diagnosticRows = [System.Collections.Generic.List[object[]]]::new()
foreach ($speciesId in $species) {
    $rollCount = 0d
    $hitCount = 0d
    $expectedProbabilityTotal = 0d
    $suppressionCount = 0d
    foreach ($run in @($report.runs)) {
        foreach ($roll in @($run.combatRolls | Where-Object { $_.attackerSpeciesId -eq $speciesId })) {
            if ($null -eq $roll) {
                continue
            }

            $rollCount++
            if ($roll.hit) {
                $hitCount++
            }

            $probabilityProperty = $roll.PSObject.Properties['expectedHitProbability']
            if ($null -ne $probabilityProperty) {
                $expectedProbabilityTotal += [double]$probabilityProperty.Value
            }
            else {
                $expectedProbabilityTotal += Get-OpposedHitProbability -AttackModifier $roll.attackModifier -BlockModifier $roll.blockModifier
            }
        }

        $suppressionEventsProperty = $run.PSObject.Properties['combatCooldownSuppressions']
        if ($null -ne $suppressionEventsProperty) {
            $suppressionCount += @($suppressionEventsProperty.Value | Where-Object { $_.attackerSpeciesId -eq $speciesId }).Count
        }
    }

    $actualHitRate = if ($rollCount -eq 0) { 0d } else { $hitCount / $rollCount }
    $expectedHitRate = if ($rollCount -eq 0) { 0d } else { $expectedProbabilityTotal / $rollCount }
    $diagnosticRows.Add(@(
        $speciesId,
        (Get-Number ($rollCount / [double]$report.seedCount)),
        ((Get-Number ($actualHitRate * 100)) + '%'),
        ((Get-Number ($expectedHitRate * 100)) + '%'),
        (Get-Number ($suppressionCount / [double]$report.seedCount))
    ))
}
Add-MarkdownTable -Lines $lines -Headers @('Attacker', 'Rolls/run', 'Actual hit rate', 'Expected hit rate', 'Cooldown suppressions/run') -Rows $diagnosticRows.ToArray()
$lines.Add('')
$lines.Add('Expected hit rate is the exact d20 probability implied by the recorded attack and block modifiers, with defender wins on ties. Cooldown suppressions are eligible experimental attacks skipped because the attacker still had cooldown ticks remaining. Legacy runs should produce no roll or suppression records.')
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
