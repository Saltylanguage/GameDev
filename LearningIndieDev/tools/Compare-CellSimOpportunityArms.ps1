[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$BaselinePath,
    [Parameter(Mandatory)]
    [string]$TrialPath,
    [Parameter(Mandatory)]
    [string]$OutputPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Read-Report {
    param([string]$Path)
    return Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
}

function Get-Activity {
    param([object]$Run, [string]$SpeciesId)
    return @($Run.activity | Where-Object speciesId -eq $SpeciesId | Select-Object -First 1)
}

function Get-Value {
    param([object]$Object, [string]$Property)
    if ($null -eq $Object) { return 0d }
    $entry = $Object.PSObject.Properties[$Property]
    if ($null -eq $entry -or $null -eq $entry.Value) { return 0d }
    return [double]$entry.Value
}

function Get-Control {
    param([object]$Run, [string]$Property)
    return Get-Value -Object $Run.opportunityControl -Property $Property
}

function Get-Population {
    param([object]$Snapshot, [string]$SpeciesId)
    $entry = @($Snapshot.species | Where-Object speciesId -eq $SpeciesId | Select-Object -First 1)
    if ($entry.Count -eq 1) { return [double]$entry[0].population }
    return 0d
}

function Get-Trajectory {
    param([object]$Run, [string]$SpeciesId)
    $values = @($Run.populationHistory | ForEach-Object { Get-Population -Snapshot $_ -SpeciesId $SpeciesId })
    if ($values.Count -eq 0) { return [pscustomobject]@{ Start = 0d; Minimum = 0d; Maximum = 0d; Mean = 0d; Auc = 0d; Final = 0d } }
    return [pscustomobject]@{
        Start = $values[0]
        Minimum = ($values | Measure-Object -Minimum).Minimum
        Maximum = ($values | Measure-Object -Maximum).Maximum
        Mean = ($values | Measure-Object -Average).Average
        Auc = ($values | Measure-Object -Sum).Sum
        Final = $values[-1]
    }
}

function Get-DeathCounts {
    param([object]$Run, [string]$SpeciesId)
    $events = @($Run.deathEvents | Where-Object { $_.speciesId -eq $SpeciesId -and $_.isCreature })
    $combat = @($events | Where-Object cause -eq 'Combat').Count
    $starvation = @($events | Where-Object cause -eq 'Starvation').Count
    return [pscustomobject]@{
        Total = $events.Count
        Combat = $combat
        Starvation = $starvation
        Other = $events.Count - $combat - $starvation
    }
}

function Get-Summary {
    param([object]$Report)
    $fox = [ordered]@{ opportunities = 0d; attempts = 0d; hits = 0d; blocked = 0d; damage = 0d; nonlethal = 0d; lethal = 0d; starvation = 0d; start = 0d; final = 0d; extinct = 0 }
    $hare = [ordered]@{ totalDeaths = 0d; combatDeaths = 0d; starvationDeaths = 0d; otherDeaths = 0d; mean = 0d; auc = 0d; final = 0d }
    $control = [ordered]@{
        scheduled = 0d; baselineValid = 0d; blockPlusTwoValid = 0d; commonValid = 0d
        baselineOnly = 0d; blockPlusTwoOnly = 0d; pairedAttempts = 0d; mismatches = 0d; invalidated = 0d
        baselineCandidateCount = 0d; blockPlusTwoCandidateCount = 0d; commonCandidateCount = 0d; unionCandidateCount = 0d
    }
    foreach ($run in @($Report.runs)) {
        $foxActivity = Get-Activity -Run $run -SpeciesId 'fox'
        $hareActivity = Get-Activity -Run $run -SpeciesId 'hare'
        $fox.opportunities += Get-Value $foxActivity 'combatOpportunities'
        $fox.attempts += Get-Value $foxActivity 'combatAttempts'
        $fox.hits += Get-Value $foxActivity 'combatHits'
        $fox.blocked += Get-Value $foxActivity 'combatBlocked'
        $fox.damage += Get-Value $foxActivity 'combatDamageApplications'
        $fox.nonlethal += Get-Value $foxActivity 'combatNonLethalHits'
        $fox.lethal += Get-Value $foxActivity 'combatLethalHits'
        $fox.starvation += Get-Value $foxActivity 'starvationDeaths'
        $foxTrajectory = Get-Trajectory -Run $run -SpeciesId 'fox'
        $fox.start += $foxTrajectory.Start
        $fox.final += $foxTrajectory.Final
        if ($foxTrajectory.Final -eq 0) { $fox.extinct++ }
        $hareDeaths = Get-DeathCounts -Run $run -SpeciesId 'hare'
        $hare.totalDeaths += $hareDeaths.Total
        $hare.combatDeaths += $hareDeaths.Combat
        $hare.starvationDeaths += $hareDeaths.Starvation
        $hare.otherDeaths += $hareDeaths.Other
        $hareTrajectory = Get-Trajectory -Run $run -SpeciesId 'hare'
        $hare.mean += $hareTrajectory.Mean
        $hare.auc += $hareTrajectory.Auc
        $hare.final += $hareTrajectory.Final
        $control.scheduled += Get-Control $run 'scheduled'
        $control.baselineValid += Get-Control $run 'baselineValid'
        $control.blockPlusTwoValid += Get-Control $run 'blockPlusTwoValid'
        $control.commonValid += Get-Control $run 'commonValid'
        $control.baselineOnly += Get-Control $run 'baselineOnly'
        $control.blockPlusTwoOnly += Get-Control $run 'blockPlusTwoOnly'
        $control.baselineCandidateCount += Get-Control $run 'baselineCandidateCount'
        $control.blockPlusTwoCandidateCount += Get-Control $run 'blockPlusTwoCandidateCount'
        $control.commonCandidateCount += Get-Control $run 'commonCandidateCount'
        $control.unionCandidateCount += Get-Control $run 'unionCandidateCount'
        $control.pairedAttempts += Get-Control $run 'pairedAttempts'
        $control.mismatches += Get-Control $run 'pairedMismatches'
        $control.invalidated += Get-Control $run 'unfulfilledInvalidated'
    }

    $count = [double]$Report.seedCount
    return [pscustomobject]@{
        Report = $Report
        Fox = $fox
        Hare = $hare
        Control = $control
        FoxHitRate = if ($fox.attempts -eq 0) { 0d } else { $fox.hits / $fox.attempts }
        FoxKillRate = if ($fox.attempts -eq 0) { 0d } else { $fox.lethal / $fox.attempts }
        Lethality = if ($fox.hits -eq 0) { 0d } else { $fox.lethal / $fox.hits }
        AverageHare = $hare.mean / $count
        HareAuc = $hare.auc / $count
        FinalHare = $hare.final / $count
        AverageFoxStart = $fox.start / $count
        AverageFoxFinal = $fox.final / $count
        FoxExtinctionRate = $fox.extinct / $count
    }
}

function Add-Table {
    param([System.Collections.Generic.List[string]]$Lines, [string[]]$Headers, [object[][]]$Rows)
    $Lines.Add('| ' + ($Headers -join ' | ') + ' |')
    $Lines.Add('| ' + (($Headers | ForEach-Object { '---' }) -join ' | ') + ' |')
    foreach ($row in $Rows) { $Lines.Add('| ' + (($row | ForEach-Object { "$($_)" }) -join ' | ') + ' |') }
}

function Format-Number { param([double]$Value) return $Value.ToString('0.###', [Globalization.CultureInfo]::InvariantCulture) }
function Format-Rate { param([double]$Value) return ((100d * $Value).ToString('0.###', [Globalization.CultureInfo]::InvariantCulture) + '%') }

$baseline = Read-Report $BaselinePath
$trial = Read-Report $TrialPath
if ($baseline.seedStart -ne $trial.seedStart -or $baseline.seedCount -ne $trial.seedCount) { throw 'Baseline and trial seed ranges differ.' }
if ($baseline.attackOpportunityMode -ne 'PairedLockstepDiagnostic' -or $trial.attackOpportunityMode -ne 'PairedLockstepDiagnostic') { throw 'Both reports must use PairedLockstepDiagnostic.' }

$base = Get-Summary $baseline
$arm = Get-Summary $trial
$lines = [System.Collections.Generic.List[string]]::new()
$lines.Add('# Controlled opportunity arm comparison')
$lines.Add('')
$lines.Add(('Baseline: `{0}`' -f $BaselinePath))
$lines.Add(('Trial: `{0}`' -f $TrialPath))
$lines.Add("Seeds: $($baseline.seedStart) through $($baseline.seedStart + $baseline.seedCount - 1) ($($baseline.seedCount) paired runs)")
$lines.Add('')
$lines.Add('## Exposure control')
$lines.Add('')
Add-Table $lines @('Metric', 'Baseline', 'Block +2', 'Delta') @(
    @('Scheduled slots', $base.Control.scheduled, $arm.Control.scheduled, ($arm.Control.scheduled - $base.Control.scheduled)),
    @('Baseline-valid opportunities', $base.Control.baselineValid, $arm.Control.baselineValid, ($arm.Control.baselineValid - $base.Control.baselineValid)),
    @('Block+2-valid opportunities', $base.Control.blockPlusTwoValid, $arm.Control.blockPlusTwoValid, ($arm.Control.blockPlusTwoValid - $base.Control.blockPlusTwoValid)),
    @('Common valid opportunities', $base.Control.commonValid, $arm.Control.commonValid, ($arm.Control.commonValid - $base.Control.commonValid)),
    @('Baseline-only opportunities', $base.Control.baselineOnly, $arm.Control.baselineOnly, ($arm.Control.baselineOnly - $base.Control.baselineOnly)),
    @('Block+2-only opportunities', $base.Control.blockPlusTwoOnly, $arm.Control.blockPlusTwoOnly, ($arm.Control.blockPlusTwoOnly - $base.Control.blockPlusTwoOnly)),
    @('Baseline candidate contacts', $base.Control.baselineCandidateCount, $arm.Control.baselineCandidateCount, ($arm.Control.baselineCandidateCount - $base.Control.baselineCandidateCount)),
    @('Block+2 candidate contacts', $base.Control.blockPlusTwoCandidateCount, $arm.Control.blockPlusTwoCandidateCount, ($arm.Control.blockPlusTwoCandidateCount - $base.Control.blockPlusTwoCandidateCount)),
    @('Common candidate contacts', $base.Control.commonCandidateCount, $arm.Control.commonCandidateCount, ($arm.Control.commonCandidateCount - $base.Control.commonCandidateCount)),
    @('Union candidate contacts', $base.Control.unionCandidateCount, $arm.Control.unionCandidateCount, ($arm.Control.unionCandidateCount - $base.Control.unionCandidateCount)),
    @('Paired baseline attempts', $base.Control.pairedAttempts, $arm.Control.pairedAttempts, ($arm.Control.pairedAttempts - $base.Control.pairedAttempts)),
    @('Paired Block+2 attempts', $base.Control.pairedAttempts, $arm.Control.pairedAttempts, ($arm.Control.pairedAttempts - $base.Control.pairedAttempts)),
    @('Paired mismatches', $base.Control.mismatches, $arm.Control.mismatches, ($arm.Control.mismatches - $base.Control.mismatches)),
    @('Invalidated common slots', $base.Control.invalidated, $arm.Control.invalidated, ($arm.Control.invalidated - $base.Control.invalidated))
)
$lines.Add('')
$lines.Add('The paired runner intersects abstract contact identities at each scheduled tick. One deterministic common contact slot is resolved in both worlds; baseline-only and Block+2-only slots are excluded from the causal sample. Candidate-contact counts are retained separately to quantify intersection censoring.')
$lines.Add('')
$lines.Add('## Opposed-roll and mortality conversion')
$lines.Add('')
Add-Table $lines @('Metric', 'Baseline', 'Block +2', 'Delta') @(
    @('Fox hit rate', (Format-Rate $base.FoxHitRate), (Format-Rate $arm.FoxHitRate), (Format-Rate ($arm.FoxHitRate - $base.FoxHitRate))),
    @('Successful hits', $base.Fox.hits, $arm.Fox.hits, ($arm.Fox.hits - $base.Fox.hits)),
    @('Fox-caused Hare deaths', $base.Fox.lethal, $arm.Fox.lethal, ($arm.Fox.lethal - $base.Fox.lethal)),
    @('Kill rate per attempt', (Format-Rate $base.FoxKillRate), (Format-Rate $arm.FoxKillRate), (Format-Rate ($arm.FoxKillRate - $base.FoxKillRate))),
    @('Lethality per hit', (Format-Rate $base.Lethality), (Format-Rate $arm.Lethality), (Format-Rate ($arm.Lethality - $base.Lethality))),
    @('Hare total deaths', $base.Hare.totalDeaths, $arm.Hare.totalDeaths, ($arm.Hare.totalDeaths - $base.Hare.totalDeaths)),
    @('Hare starvation deaths', $base.Hare.starvationDeaths, $arm.Hare.starvationDeaths, ($arm.Hare.starvationDeaths - $base.Hare.starvationDeaths)),
    @('Hare other deaths', $base.Hare.otherDeaths, $arm.Hare.otherDeaths, ($arm.Hare.otherDeaths - $base.Hare.otherDeaths))
)
$lines.Add('')
$lines.Add('## Population trajectory')
$lines.Add('')
Add-Table $lines @('Metric', 'Baseline', 'Block +2', 'Delta') @(
    @('Mean Hare population', (Format-Number $base.AverageHare), (Format-Number $arm.AverageHare), (Format-Number ($arm.AverageHare - $base.AverageHare))),
    @('Hare population AUC', (Format-Number $base.HareAuc), (Format-Number $arm.HareAuc), (Format-Number ($arm.HareAuc - $base.HareAuc))),
    @('Final Hare population', (Format-Number $base.FinalHare), (Format-Number $arm.FinalHare), (Format-Number ($arm.FinalHare - $base.FinalHare))),
    @('Mean Fox start', (Format-Number $base.AverageFoxStart), (Format-Number $arm.AverageFoxStart), (Format-Number ($arm.AverageFoxStart - $base.AverageFoxStart))),
    @('Mean Fox final', (Format-Number $base.AverageFoxFinal), (Format-Number $arm.AverageFoxFinal), (Format-Number ($arm.AverageFoxFinal - $base.AverageFoxFinal))),
    @('Fox extinction rate', (Format-Rate $base.FoxExtinctionRate), (Format-Rate $arm.FoxExtinctionRate), (Format-Rate ($arm.FoxExtinctionRate - $base.FoxExtinctionRate)))
)
$lines.Add('')
$lines.Add('## Paired exposure and outcome deltas')
$lines.Add('')
$pairRows = [System.Collections.Generic.List[object[]]]::new()
$baseRuns = @($baseline.runs | Sort-Object seed)
$trialRuns = @($trial.runs | Sort-Object seed)
for ($index = 0; $index -lt $baseRuns.Count; $index++) {
    $b = $baseRuns[$index]
    $t = $trialRuns[$index]
    if ($b.seed -ne $t.seed) { throw "Pair seed mismatch at index $index." }
    $ba = Get-Activity $b 'fox'; $ta = Get-Activity $t 'fox'
    $bh = Get-DeathCounts $b 'hare'; $th = Get-DeathCounts $t 'hare'
    $bt = Get-Trajectory $b 'hare'; $tt = Get-Trajectory $t 'hare'
    $baseHitRate = if ((Get-Value $ba 'combatAttempts') -eq 0) { 0d } else { (Get-Value $ba 'combatHits') / (Get-Value $ba 'combatAttempts') }
    $trialHitRate = if ((Get-Value $ta 'combatAttempts') -eq 0) { 0d } else { (Get-Value $ta 'combatHits') / (Get-Value $ta 'combatAttempts') }
    $pairRows.Add(@(
        $b.seed,
        (Get-Control $b 'scheduled'),
        (Get-Control $t 'scheduled'),
        (Get-Control $b 'baselineValid'),
        (Get-Control $b 'blockPlusTwoValid'),
        (Get-Control $b 'commonValid'),
        (Get-Control $b 'baselineOnly'),
        (Get-Control $b 'blockPlusTwoOnly'),
        (Get-Control $b 'pairedAttempts'),
        (Get-Control $b 'pairedMismatches'),
        (Format-Rate ($trialHitRate - $baseHitRate)),
        ($th.Combat - $bh.Combat),
        ($th.Starvation - $bh.Starvation),
        (Format-Number ($tt.Mean - $bt.Mean)),
        (Format-Number ($tt.Auc - $bt.Auc)),
        (Format-Number ($tt.Final - $bt.Final))
    ))
}
Add-Table $lines @('Seed', 'Base sched.', 'Trial sched.', 'Base valid', 'Block+2 valid', 'Common', 'Base-only', 'Block+2-only', 'Paired attempts', 'Mismatches', 'Delta hit rate', 'Delta Fox deaths', 'Delta starvation', 'Delta mean Hare', 'Delta Hare AUC', 'Delta final Hare') $pairRows.ToArray()
$lines.Add('')
$lines.Add('SC-1 passes only when paired attempts match exactly for every seed and paired mismatches remain zero. Scheduled equality alone is not treated as exposure isolation.')
$lines.Add('')

$outputDirectory = Split-Path -Parent $OutputPath
if (-not [string]::IsNullOrWhiteSpace($outputDirectory)) { New-Item -ItemType Directory -Path $outputDirectory -Force | Out-Null }
Set-Content -LiteralPath $OutputPath -Value $lines -Encoding utf8
Resolve-Path -LiteralPath $OutputPath
