[CmdletBinding()]
param(
    [Parameter(Mandatory)] [string]$CalibrationBaselinePath,
    [Parameter(Mandatory)] [string]$CalibrationBlockPlusTwoPath,
    [Parameter(Mandatory)] [string]$HeldOutBaselinePath,
    [Parameter(Mandatory)] [string]$HeldOutBlockPlusTwoPath,
    [Parameter(Mandatory)] [string]$OutputDirectory,
    [string]$ProjectPath = (Join-Path $PSScriptRoot '..')
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Resolve-PathFromProject {
    param([string]$Path)
    if (Test-Path -LiteralPath $Path -PathType Leaf) { return (Resolve-Path -LiteralPath $Path).Path }
    $candidate = Join-Path $ProjectPath $Path
    if (Test-Path -LiteralPath $candidate -PathType Leaf) { return (Resolve-Path -LiteralPath $candidate).Path }
    throw "Could not find '$Path'."
}

function Read-Report([string]$Path) {
    return Get-Content -LiteralPath (Resolve-PathFromProject $Path) -Raw | ConvertFrom-Json
}

function Get-Number($Object, [string]$Property) {
    if ($null -eq $Object) { return 0d }
    $entry = $Object.PSObject.Properties[$Property]
    if ($null -eq $entry -or $null -eq $entry.Value) { return 0d }
    return [double]$entry.Value
}

function Get-State($Row, [string]$Arm) {
    if ($Arm -eq 'Baseline') { return $Row.baseline }
    return $Row.blockPlusTwo
}

function Get-Rows($Report, [string]$Group) {
    $rows = [System.Collections.Generic.List[object]]::new()
    foreach ($run in @($Report.runs)) {
        foreach ($row in @($run.opportunityControl.opportunityAudit)) {
            if ($null -eq $row) { continue }
            $rows.Add([pscustomobject]@{
                group = $Group
                seed = [int]$run.seed
                tick = [int]$row.tick
                occurrence = [int]$row.occurrence
                eventId = [string]$row.eventId
                identity = [string]$row.identity
                stratum = [string]$row.stratum
                baseline = $row.baseline
                blockPlusTwo = $row.blockPlusTwo
            })
        }
    }
    return $rows.ToArray()
}

function Add-TimeBand($Rows, [int]$MaximumTick) {
    foreach ($row in $Rows) {
        $progress = if ($MaximumTick -le 0) { 0d } else { [double]$row.tick / $MaximumTick }
        $band = [Math]::Min(5, [Math]::Max(1, [int][Math]::Floor($progress * 5d) + 1))
        Add-Member -InputObject $row -NotePropertyName normalizedTime -NotePropertyValue $progress
        Add-Member -InputObject $row -NotePropertyName timeBand -NotePropertyValue $band
    }
}

function Get-Values($Rows, [string]$Arm, [string]$Property) {
    $values = [System.Collections.Generic.List[double]]::new()
    foreach ($row in $Rows) {
        $state = Get-State $row $Arm
        if ($null -eq $state -or -not $state.present) { continue }
        $values.Add((Get-Number $state $Property))
    }
    return $values.ToArray()
}

function Get-Mean($Values) {
    if ($Values.Count -eq 0) { return 0d }
    return (($Values | Measure-Object -Average).Average)
}

function Get-Sd($Values) {
    if ($Values.Count -lt 2) { return 0d }
    $mean = Get-Mean $Values
    $sum = 0d
    foreach ($value in $Values) { $sum += [Math]::Pow($value - $mean, 2) }
    return [Math]::Sqrt($sum / ($Values.Count - 1))
}

function Get-Smd($Left, $Right) {
    if ($Left.Count -eq 0 -or $Right.Count -eq 0) { return 0d }
    $leftSd = Get-Sd $Left
    $rightSd = Get-Sd $Right
    $pooled = [Math]::Sqrt((($Left.Count - 1) * $leftSd * $leftSd + ($Right.Count - 1) * $rightSd * $rightSd) / [Math]::Max(1, $Left.Count + $Right.Count - 2))
    if ($pooled -eq 0) { return 0d }
    return ((Get-Mean $Left) - (Get-Mean $Right)) / $pooled
}

function Format-Number([double]$Value) { return $Value.ToString('0.###', [Globalization.CultureInfo]::InvariantCulture) }
function Format-Percent([double]$Value) { return (100d * $Value).ToString('0.###', [Globalization.CultureInfo]::InvariantCulture) + '%' }

$calibrationBaseline = Read-Report $CalibrationBaselinePath
$calibrationBlockPlusTwo = Read-Report $CalibrationBlockPlusTwoPath
$heldOutBaseline = Read-Report $HeldOutBaselinePath
$heldOutBlockPlusTwo = Read-Report $HeldOutBlockPlusTwoPath
$rows = @(
    Get-Rows $calibrationBaseline 'Calibration'
    Get-Rows $heldOutBaseline 'Held-out'
)
$maximumTick = (@($rows | ForEach-Object tick) | Measure-Object -Maximum).Maximum
Add-TimeBand $rows ([int]$maximumTick)

$stateProperties = @(
    'harePopulation', 'foxPopulation', 'plantPopulation',
    'localHareDensity', 'localFoxDensity', 'localPlantResourceDensity',
    'attackerAge', 'attackerEnergy', 'attackerFoodReserve',
    'targetAge', 'targetEnergy', 'targetFoodReserve', 'terrainEnergy'
)
$reconciliation = [System.Collections.Generic.List[object]]::new()
foreach ($group in @('Calibration', 'Held-out')) {
    $groupRows = @($rows | Where-Object group -eq $group)
    foreach ($seed in @($groupRows | Select-Object -ExpandProperty seed -Unique | Sort-Object)) {
        $seedRows = @($groupRows | Where-Object seed -eq $seed)
        $common = @($seedRows | Where-Object stratum -eq 'COMMON').Count
        $baselineOnly = @($seedRows | Where-Object stratum -eq 'BASELINE_ONLY').Count
        $blockOnly = @($seedRows | Where-Object stratum -eq 'BLOCK_ONLY').Count
        $baselineReport = if ($group -eq 'Calibration') { $calibrationBaseline } else { $heldOutBaseline }
        $baselineRun = @($baselineReport.runs | Where-Object seed -eq $seed)[0]
        $control = $baselineRun.opportunityControl
        $pass = (($common + $baselineOnly + $blockOnly) -eq [int]$control.unionCandidateCount) -and
            (($baselineOnly + $common) -eq [int]$control.baselineCandidateCount) -and
            (($blockOnly + $common) -eq [int]$control.blockPlusTwoCandidateCount)
        $reconciliation.Add([pscustomobject]@{
            group = $group; seed = [int]$seed
            common = $common; baselineOnly = $baselineOnly; blockOnly = $blockOnly
            union = $common + $baselineOnly + $blockOnly
            baselineCandidates = [int]$control.baselineCandidateCount
            blockPlusTwoCandidates = [int]$control.blockPlusTwoCandidateCount
            commonCandidates = [int]$control.commonCandidateCount
            pass = $pass
        })
    }
}

$coverage = [System.Collections.Generic.List[object]]::new()
$timeCoverage = [System.Collections.Generic.List[object]]::new()
$effectRows = [System.Collections.Generic.List[object]]::new()
foreach ($group in @('Calibration', 'Held-out')) {
    $groupRows = @($rows | Where-Object group -eq $group)
    $commonCount = @($groupRows | Where-Object stratum -eq 'COMMON').Count
    $baselineOnlyCount = @($groupRows | Where-Object stratum -eq 'BASELINE_ONLY').Count
    $blockOnlyCount = @($groupRows | Where-Object stratum -eq 'BLOCK_ONLY').Count
    $unionCount = $commonCount + $baselineOnlyCount + $blockOnlyCount
    $coverage.Add([pscustomobject]@{
        group = $group; common = $commonCount; baselineOnly = $baselineOnlyCount
        blockOnly = $blockOnlyCount; union = $unionCount
        commonUnion = if ($unionCount -eq 0) { 0d } else { $commonCount / $unionCount }
        reconciliationFailures = @($reconciliation | Where-Object { $_.group -eq $group -and -not $_.pass }).Count
    })

    for ($band = 1; $band -le 5; $band++) {
        $bandRows = @($groupRows | Where-Object timeBand -eq $band)
        $commonBand = @($bandRows | Where-Object stratum -eq 'COMMON').Count
        $baselineBand = @($bandRows | Where-Object stratum -eq 'BASELINE_ONLY').Count
        $blockBand = @($bandRows | Where-Object stratum -eq 'BLOCK_ONLY').Count
        $unionBand = $commonBand + $baselineBand + $blockBand
        $timeCoverage.Add([pscustomobject]@{
            group = $group; timeBand = $band; common = $commonBand
            baselineOnly = $baselineBand; blockOnly = $blockBand; union = $unionBand
            commonUnion = if ($unionBand -eq 0) { 0d } else { $commonBand / $unionBand }
        })
    }

    $comparisons = @(
        [pscustomobject]@{ name = 'COMMON vs UNION'; left = 'COMMON'; right = 'UNION'; leftArm = 'Baseline'; rightArm = 'Baseline' }
        [pscustomobject]@{ name = 'COMMON vs UNION (Block+2)'; left = 'COMMON'; right = 'UNION'; leftArm = 'Block+2'; rightArm = 'Block+2' }
        [pscustomobject]@{ name = 'COMMON vs BASELINE_ONLY'; left = 'COMMON'; right = 'BASELINE_ONLY'; leftArm = 'Baseline'; rightArm = 'Baseline' }
        [pscustomobject]@{ name = 'COMMON vs BLOCK_ONLY'; left = 'COMMON'; right = 'BLOCK_ONLY'; leftArm = 'Block+2'; rightArm = 'Block+2' }
        [pscustomobject]@{ name = 'BASELINE_ONLY vs BLOCK_ONLY'; left = 'BASELINE_ONLY'; right = 'BLOCK_ONLY'; leftArm = 'Baseline'; rightArm = 'Block+2' }
    )
    foreach ($comparison in $comparisons) {
        $leftRows = if ($comparison.left -eq 'UNION') { $groupRows } else { @($groupRows | Where-Object stratum -eq $comparison.left) }
        $rightRows = if ($comparison.right -eq 'UNION') { $groupRows } else { @($groupRows | Where-Object stratum -eq $comparison.right) }
        foreach ($property in $stateProperties) {
            $left = Get-Values $leftRows $comparison.leftArm $property
            $right = Get-Values $rightRows $comparison.rightArm $property
            $smd = Get-Smd $left $right
            $effectRows.Add([pscustomobject]@{
                group = $group; comparison = $comparison.name; variable = $property
                leftN = $left.Count; rightN = $right.Count
                leftMean = Get-Mean $left; rightMean = Get-Mean $right; smd = $smd
                material = [Math]::Abs($smd) -ge 0.25
            })
        }
    }
}

$historyRows = [System.Collections.Generic.List[object]]::new()
foreach ($group in @('Calibration', 'Held-out')) {
    $groupRows = @($rows | Where-Object group -eq $group | Sort-Object seed, tick, occurrence)
    foreach ($arm in @('Baseline', 'Block+2')) {
        $prior = @{}
        foreach ($row in $groupRows) {
            $state = Get-State $row $arm
            if ($null -eq $state -or -not $state.present) { continue }
            $key = "$($row.seed):$($state.attackerEntityId):$($state.targetEntityId)"
            $priorCount = if ($prior.ContainsKey($key)) { [int]$prior[$key] } else { 0 }
            $prior[$key] = $priorCount + 1
            $historyRows.Add([pscustomobject]@{
                group = $group; arm = $arm; stratum = $row.stratum
                firstOrRepeat = if ($priorCount -eq 0) { 'FIRST' } else { 'REPEAT' }
            })
        }
    }
}

$datasetPath = Join-Path $OutputDirectory 'encounter-dataset.json'
$analysisPath = Join-Path $OutputDirectory 'representativeness-analysis.md'
New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null
$rows | ConvertTo-Json -Depth 10 | Set-Content -LiteralPath $datasetPath -Encoding utf8

$lines = [System.Collections.Generic.List[string]]::new()
$lines.Add('# Common-contact representativeness audit')
$lines.Add('')
$lines.Add('- Primary population: naturally valid candidate contacts from the paired diagnostic reports.')
$lines.Add('- Pre-contact snapshot: source grid immediately before behavior/attack resolution; no roll, damage, or death fields are used for the primary comparison.')
$lines.Add('- Practical threshold: absolute SMD >= 0.25 is flagged material; this is a descriptive threshold, not a significance test.')
$lines.Add('')
$lines.Add('## Coverage and reconciliation')
$lines.Add('')
$lines.Add('| Group | Common | Baseline-only | Block-only | Union | Common / union | Reconciliation failures |')
$lines.Add('| --- | ---: | ---: | ---: | ---: | ---: | ---: |')
foreach ($row in $coverage) { $lines.Add("| $($row.group) | $($row.common) | $($row.baselineOnly) | $($row.blockOnly) | $($row.union) | $(Format-Percent $row.commonUnion) | $($row.reconciliationFailures) |") }
$lines.Add('')
$lines.Add('## Time-dependent censoring')
$lines.Add('')
$lines.Add('| Group | Quintile | Common | Baseline-only | Block-only | Union | Common / union |')
$lines.Add('| --- | ---: | ---: | ---: | ---: | ---: | ---: |')
foreach ($row in $timeCoverage) { $lines.Add("| $($row.group) | $($row.timeBand) | $($row.common) | $($row.baselineOnly) | $($row.blockOnly) | $($row.union) | $(Format-Percent $row.commonUnion) |") }
$lines.Add('')
$lines.Add('## Material pre-contact differences')
$lines.Add('')
$lines.Add('| Group | Comparison | Variable | Left N | Right N | Left mean | Right mean | SMD |')
$lines.Add('| --- | --- | --- | ---: | ---: | ---: | ---: | ---: |')
foreach ($row in @($effectRows | Where-Object material | Sort-Object group, comparison, variable)) { $lines.Add("| $($row.group) | $($row.comparison) | $($row.variable) | $($row.leftN) | $($row.rightN) | $(Format-Number $row.leftMean) | $(Format-Number $row.rightMean) | $(Format-Number $row.smd) |") }
if (@($effectRows | Where-Object material).Count -eq 0) { $lines.Add('| - | No variable crossed |SMD|0|0|0|0|0|') }
$lines.Add('')
$lines.Add('## Full effect-size table')
$lines.Add('')
$lines.Add('| Group | Comparison | Variable | Left N | Right N | Left mean | Right mean | SMD |')
$lines.Add('| --- | --- | --- | ---: | ---: | ---: | ---: | ---: |')
foreach ($row in $effectRows) { $lines.Add("| $($row.group) | $($row.comparison) | $($row.variable) | $($row.leftN) | $($row.rightN) | $(Format-Number $row.leftMean) | $(Format-Number $row.rightMean) | $(Format-Number $row.smd) |") }
$lines.Add('')
$lines.Add('## First versus repeat contact')
$lines.Add('')
$lines.Add('| Group | Arm | Stratum | First | Repeat | Repeat share |')
$lines.Add('| --- | --- | --- | ---: | ---: | ---: |')
foreach ($group in @('Calibration', 'Held-out')) {
    foreach ($arm in @('Baseline', 'Block+2')) {
        foreach ($stratum in @('COMMON', 'BASELINE_ONLY', 'BLOCK_ONLY')) {
            $subset = @($historyRows | Where-Object { $_.group -eq $group -and $_.arm -eq $arm -and $_.stratum -eq $stratum })
            $first = @($subset | Where-Object firstOrRepeat -eq 'FIRST').Count
            $repeat = @($subset | Where-Object firstOrRepeat -eq 'REPEAT').Count
            $total = $first + $repeat
            $lines.Add("| $group | $arm | $stratum | $first | $repeat | $(Format-Percent $(if($total -eq 0){0d}else{$repeat/$total})) |")
        }
    }
}
$lines.Add('')
$lines.Add('## Interpretation guardrail')
$lines.Add('')
$lines.Add('This artifact measures selection patterns only. It does not retest Block+2 mechanics, does not weight or match encounters, and does not treat population improvement as a success criterion.')
Set-Content -LiteralPath $analysisPath -Value $lines -Encoding utf8

[pscustomobject]@{ Dataset = $datasetPath; Analysis = $analysisPath; Rows = $rows.Count; ReconciliationFailures = @($reconciliation | Where-Object { -not $_.pass }).Count }
