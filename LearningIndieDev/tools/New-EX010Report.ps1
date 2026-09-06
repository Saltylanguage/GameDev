[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$ReportPath,
    [string]$OutputDirectory,
    [string]$ProjectPath,
    [ValidateSet('Original', 'Alternate')]
    [string]$Sequence = 'Original'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Resolve-ProjectFile {
    param(
        [Parameter(Mandatory)] [string]$Path,
        [Parameter(Mandatory)] [string]$ProjectRoot
    )

    if (Test-Path -LiteralPath $Path -PathType Leaf) {
        return (Resolve-Path -LiteralPath $Path).Path
    }

    $candidate = Join-Path $ProjectRoot $Path
    if (Test-Path -LiteralPath $candidate -PathType Leaf) {
        return (Resolve-Path -LiteralPath $candidate).Path
    }

    throw "Could not find report '$Path'."
}

function Get-PropertyValue {
    param(
        [Parameter(Mandatory)] [object]$Object,
        [Parameter(Mandatory)] [string]$Name
    )

    $property = $Object.PSObject.Properties[$Name]
    if ($null -eq $property) {
        return $null
    }

    return $property.Value
}

function Get-StatCell {
    param(
        [Parameter(Mandatory)] [object]$Stat,
        [Parameter(Mandatory)] [string]$Name
    )

    $status = Get-PropertyValue -Object $Stat -Name ($Name + 'Status')
    if ($null -ne $status -and [string]$status -ne 'Valid') {
        return [string]$status
    }

    $value = Get-PropertyValue -Object $Stat -Name $Name
    if ($null -eq $value) {
        return 'N/A'
    }

    return $value
}

function Get-StatStatus {
    param(
        [Parameter(Mandatory)] [object]$Stat,
        [Parameter(Mandatory)] [string]$Name
    )

    $status = Get-PropertyValue -Object $Stat -Name ($Name + 'Status')
    if ($null -eq $status) {
        return 'Valid'
    }

    return [string]$status
}

function Get-DeltaCell {
    param(
        [AllowNull()] [object]$Current,
        [AllowNull()] [object]$Previous,
        [Parameter(Mandatory)] [string]$Name
    )

    if ($null -eq $Current -or $null -eq $Previous) {
        return 'NO_DATA'
    }

    $currentStatus = Get-StatStatus -Stat $Current -Name $Name
    $previousStatus = Get-StatStatus -Stat $Previous -Name $Name
    if ($currentStatus -eq 'INVALID' -or $previousStatus -eq 'INVALID') {
        return 'INVALID'
    }

    if ($currentStatus -ne 'Valid' -or $previousStatus -ne 'Valid') {
        return 'N/A'
    }

    $currentValue = Get-PropertyValue -Object $Current -Name $Name
    $previousValue = Get-PropertyValue -Object $Previous -Name $Name
    if ($null -eq $currentValue -or $null -eq $previousValue) {
        return 'N/A'
    }

    return [double]$currentValue - [double]$previousValue
}

function Get-UpgradeIds {
    param([AllowNull()] [object]$Loadout)

    if ($null -eq $Loadout) {
        return @()
    }

    return @($Loadout | ForEach-Object { [string]$_.upgradeId })
}

function Join-UpgradeIds {
    param([AllowNull()] [object]$Loadout)

    $ids = @(Get-UpgradeIds -Loadout $Loadout)
    if ($ids.Count -eq 0) {
        return 'none'
    }

    return $ids -join '+'
}

function New-StatLineRow {
    param(
        [Parameter(Mandatory)] [int]$Seed,
        [Parameter(Mandatory)] [string]$Phase,
        [Parameter(Mandatory)] [string]$Window,
        [Parameter(Mandatory)] [string]$RecordStatus,
        [Parameter(Mandatory)] [string]$EffectiveUpgrades,
        [AllowNull()] [object]$Stat
    )

    $row = [ordered]@{
        seed = $Seed
        phase = $Phase
        window = $Window
        recordStatus = $RecordStatus
        effectiveUpgrades = $EffectiveUpgrades
    }

    foreach ($name in $script:AllStatNames) {
        $row[$name] = if ($null -eq $Stat) { 'NO_DATA' } else { Get-StatCell -Stat $Stat -Name $name }
    }

    foreach ($name in $script:DerivedStatNames) {
        $row[$name + 'Status'] = if ($null -eq $Stat) { 'NO_DATA' } else { Get-StatStatus -Stat $Stat -Name $name }
    }

    return [pscustomobject]$row
}

function New-DeltaRow {
    param(
        [Parameter(Mandatory)] [int]$Seed,
        [Parameter(Mandatory)] [int]$Phase,
        [Parameter(Mandatory)] [int]$PreviousPhase,
        [Parameter(Mandatory)] [string]$Window,
        [Parameter(Mandatory)] [string]$DeltaStatus,
        [Parameter(Mandatory)] [string]$EffectiveUpgrades,
        [AllowNull()] [object]$Current,
        [AllowNull()] [object]$Previous
    )

    $row = [ordered]@{
        seed = $Seed
        phase = $Phase
        previousPhase = $PreviousPhase
        window = $Window
        deltaStatus = $DeltaStatus
        effectiveUpgrades = $EffectiveUpgrades
    }

    foreach ($name in $script:AllStatNames) {
        $row[$name + 'Delta'] = Get-DeltaCell -Current $Current -Previous $Previous -Name $name
    }

    return [pscustomobject]$row
}

function Add-MarkdownRow {
    param(
        [Parameter(Mandatory)] [AllowEmptyString()] [System.Collections.Generic.List[string]]$Lines,
        [Parameter(Mandatory)] [object[]]$Values
    )

    $escaped = @($Values | ForEach-Object { ([string]$_).Replace('|', '\|') })
    $Lines.Add('| ' + ($escaped -join ' | ') + ' |')
}

if ([string]::IsNullOrWhiteSpace($ProjectPath)) {
    $ProjectPath = Join-Path $PSScriptRoot '..'
}

$project = (Resolve-Path -LiteralPath $ProjectPath).Path
$resolvedReportPath = Resolve-ProjectFile -Path $ReportPath -ProjectRoot $project
$report = Get-Content -LiteralPath $resolvedReportPath -Raw | ConvertFrom-Json
if ($null -eq $report.runs) {
    throw "'$resolvedReportPath' is not a CellSim report."
}

if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Split-Path -Parent $resolvedReportPath
}

New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null

$script:RawStatNames = @('SPO', 'HPS', 'EHS', 'ECN', 'PREY', 'STRV', 'MAT', 'BIR', 'CRWD', 'FPO')
$script:DerivedStatNames = @('pAVI', 'eAVI', 'predAVG', 'sAVI', 'cAVI', 'bAVG', 'RFS', 'APS')
$script:AllStatNames = @($script:RawStatNames + $script:DerivedStatNames)
$expectedSchedule = if ($Sequence -eq 'Original') {
    @(
        'none',
        'none',
        'trailblazer-long-stride',
        'trailblazer-long-stride',
        'trailblazer-long-stride,trailblazer-far-sight',
        'trailblazer-long-stride,trailblazer-far-sight,warren-guarded-burrow',
        'trailblazer-long-stride,trailblazer-far-sight,warren-guarded-burrow,warren-room-to-breed',
        'trailblazer-long-stride,trailblazer-far-sight,warren-guarded-burrow,warren-room-to-breed',
        'trailblazer-long-stride,trailblazer-far-sight,warren-guarded-burrow,warren-room-to-breed,gardeners-careful-sowing',
        'trailblazer-long-stride,trailblazer-far-sight,warren-guarded-burrow,warren-room-to-breed,gardeners-careful-sowing,familial-bond-large-litters'
    )
}
else {
    @(
        'none',
        'none',
        'familial-bond-large-litters',
        'familial-bond-large-litters',
        'familial-bond-large-litters,gardeners-careful-sowing',
        'familial-bond-large-litters,gardeners-careful-sowing,warren-room-to-breed',
        'familial-bond-large-litters,gardeners-careful-sowing,warren-room-to-breed,warren-guarded-burrow',
        'familial-bond-large-litters,gardeners-careful-sowing,warren-room-to-breed,warren-guarded-burrow',
        'familial-bond-large-litters,gardeners-careful-sowing,warren-room-to-breed,warren-guarded-burrow,trailblazer-far-sight',
        'familial-bond-large-litters,gardeners-careful-sowing,warren-room-to-breed,warren-guarded-burrow,trailblazer-far-sight,trailblazer-long-stride'
    )
}

$errors = [System.Collections.Generic.List[string]]::new()
$warnings = [System.Collections.Generic.List[string]]::new()
$phaseRows = [System.Collections.Generic.List[object]]::new()
$deltaRows = [System.Collections.Generic.List[object]]::new()
$finalRows = [System.Collections.Generic.List[object]]::new()
$completedPhaseCounts = [System.Collections.Generic.List[object]]::new()

$reportPhaseCount = [int](Get-PropertyValue -Object $report -Name 'phaseCount')
if ($reportPhaseCount -ne 10) {
    $errors.Add("Report phaseCount is $reportPhaseCount; EX-010 requires 10.")
}

if ([int](Get-PropertyValue -Object $report -Name 'phaseLengthTicks') -ne 200) {
    $errors.Add('Report phaseLengthTicks is not 200.')
}

$reportSchedule = @($report.phaseUpgradeSchedule | ForEach-Object { [string]$_ })
if (($reportSchedule -join ';') -ne ($expectedSchedule -join ';')) {
    $errors.Add("Report phase schedule does not match the locked $Sequence sequence.")
}

$runIndex = 0
foreach ($run in @($report.runs)) {
    $seed = [int]$run.seed
    $phases = @($run.phaseResults)
    $phaseByIndex = @{}
    foreach ($phase in $phases) {
        $phaseByIndex[[int]$phase.phaseIndex] = $phase
    }

    $previousStat = $null
    $previousPhase = 0
    $completed = 0
    for ($phaseNumber = 1; $phaseNumber -le 10; $phaseNumber++) {
        $phase = if ($phaseByIndex.ContainsKey($phaseNumber)) { $phaseByIndex[$phaseNumber] } else { $null }
        $expectedStart = ($phaseNumber - 1) * 200
        $expectedEnd = $phaseNumber * 200
        $window = "${expectedStart}:${expectedEnd}"
        if ($null -eq $phase) {
            $phaseRows.Add((New-StatLineRow -Seed $seed -Phase ([string]$phaseNumber) -Window $window -RecordStatus 'NO_DATA' -EffectiveUpgrades 'NO_DATA' -Stat $null))
            $deltaRows.Add((New-DeltaRow -Seed $seed -Phase $phaseNumber -PreviousPhase $previousPhase -Window $window -DeltaStatus 'NO_DATA' -EffectiveUpgrades 'NO_DATA' -Current $null -Previous $previousStat))
            continue
        }

        $completed++
        if ([int]$phase.phaseIndex -ne $phaseNumber) {
            $errors.Add("Seed $seed has phase index $($phase.phaseIndex) where phase $phaseNumber was expected.")
        }
        if ([int]$phase.windowStartTickExclusive -ne $expectedStart -or [int]$phase.windowEndTickInclusive -ne $expectedEnd) {
            $errors.Add("Seed $seed phase $phaseNumber does not use the expected $window tick window.")
        }

        $opening = @($phase.openingPopulation)
        $closing = @($phase.closingPopulation)
        if ($opening.Count -ne 1 -or $closing.Count -ne 1) {
            $errors.Add("Seed $seed phase $phaseNumber does not have one opening and one closing population snapshot.")
        }
        else {
            if ([int]$opening[0].tick -ne $expectedStart -or [int]$closing[0].tick -ne $expectedEnd) {
                $errors.Add("Seed $seed phase $phaseNumber population edges do not match $window.")
            }
        }

        $stat = Get-PropertyValue -Object $phase -Name 'herbivoreStatLine'
        if ($null -eq $stat) {
            $errors.Add("Seed $seed phase $phaseNumber is missing its Herbivore Stat-Line.")
        }

        $loadout = Get-PropertyValue -Object $phase -Name 'effectiveUpgradeLoadout'
        $loadoutText = Join-UpgradeIds -Loadout $loadout
        $expectedLoadout = $expectedSchedule[$phaseNumber - 1] -replace ',', '+'
        if ($loadoutText -ne $expectedLoadout) {
            $errors.Add("Seed $seed phase $phaseNumber loadout is '$loadoutText'; expected '$expectedLoadout'.")
        }
        $phaseRows.Add((New-StatLineRow -Seed $seed -Phase ([string]$phaseNumber) -Window $window -RecordStatus 'DATA' -EffectiveUpgrades $loadoutText -Stat $stat))
        $deltaStatus = if ($null -eq $stat -or $null -eq $previousStat) { if ($null -eq $previousStat) { 'NO_PREVIOUS' } else { 'NO_DATA' } } else { 'DATA' }
        $deltaRows.Add((New-DeltaRow -Seed $seed -Phase $phaseNumber -PreviousPhase $previousPhase -Window $window -DeltaStatus $deltaStatus -EffectiveUpgrades $loadoutText -Current $stat -Previous $previousStat))
        if ($null -ne $stat) {
            $previousStat = $stat
            $previousPhase = $phaseNumber
        }
    }

    if ($completed -lt 10) {
        $warnings.Add("Seed $seed has data for $completed of 10 phases; later phases are marked NO_DATA.")
    }
    $completedPhaseCounts.Add([pscustomobject]@{ seed = $seed; completedPhases = $completed })

    $finalStat = Get-PropertyValue -Object $run -Name 'herbivoreStatLine'
    if ($null -eq $finalStat) {
        $errors.Add("Seed $seed is missing the independent final Herbivore Stat-Line.")
    }
    else {
        $finalRows.Add((New-StatLineRow -Seed $seed -Phase 'FINAL' -Window "0:$($run.ticks)" -RecordStatus 'DATA' -EffectiveUpgrades (Join-UpgradeIds -Loadout $run.upgradeLoadout) -Stat $finalStat))
    }

    $acquisitions = @($run.upgradeAcquisitionTimeline)
    $acquisitionIds = @(Get-UpgradeIds -Loadout @($acquisitions | ForEach-Object { $_.upgrade }))
    $expectedAcquisitionIds = if ($Sequence -eq 'Original') {
        @('trailblazer-long-stride', 'trailblazer-far-sight', 'warren-guarded-burrow', 'warren-room-to-breed', 'gardeners-careful-sowing', 'familial-bond-large-litters')
    }
    else {
        @('familial-bond-large-litters', 'gardeners-careful-sowing', 'warren-room-to-breed', 'warren-guarded-burrow', 'trailblazer-far-sight', 'trailblazer-long-stride')
    }
    if (($acquisitionIds -join ',') -ne ($expectedAcquisitionIds -join ',')) {
        $errors.Add("Seed $seed acquisition order is '$($acquisitionIds -join ',')', expected '$($expectedAcquisitionIds -join ',')'.")
    }

    $expectedTicks = @(400, 800, 1000, 1200, 1600, 1800)
    if ($acquisitions.Count -eq $expectedTicks.Count) {
        for ($index = 0; $index -lt $expectedTicks.Count; $index++) {
            if ([int]$acquisitions[$index].effectiveTick -ne $expectedTicks[$index]) {
                $errors.Add("Seed $seed acquisition $($index + 1) occurred at tick $($acquisitions[$index].effectiveTick), expected $($expectedTicks[$index]).")
            }
        }
    }
    else {
        $errors.Add("Seed $seed has $($acquisitions.Count) acquisitions; expected 6.")
    }
    $runIndex++
}

$phaseCsvPath = Join-Path $OutputDirectory 'phase-statlines.csv'
$deltaCsvPath = Join-Path $OutputDirectory 'phase-statline-deltas.csv'
$finalCsvPath = Join-Path $OutputDirectory 'final-statlines.csv'
$validationJsonPath = Join-Path $OutputDirectory 'ex010-validation.json'
$markdownPath = Join-Path $OutputDirectory 'ex010-report.md'

$phaseRows | Export-Csv -LiteralPath $phaseCsvPath -NoTypeInformation -Encoding utf8
$deltaRows | Export-Csv -LiteralPath $deltaCsvPath -NoTypeInformation -Encoding utf8
$finalRows | Export-Csv -LiteralPath $finalCsvPath -NoTypeInformation -Encoding utf8

$validation = [ordered]@{
    status = if ($errors.Count -gt 0) { 'INVALID' } elseif ($warnings.Count -gt 0) { 'VALID_WITH_WARNINGS' } else { 'VALID' }
    sequence = $Sequence
    reportPath = $resolvedReportPath
    expectedPhaseCount = 10
    expectedPhaseLengthTicks = 200
    runCount = @($report.runs).Count
    completedPhaseCounts = @($completedPhaseCounts)
    errors = @($errors)
    warnings = @($warnings)
    outputs = @(
        [System.IO.Path]::GetFileName($phaseCsvPath),
        [System.IO.Path]::GetFileName($deltaCsvPath),
        [System.IO.Path]::GetFileName($finalCsvPath),
        [System.IO.Path]::GetFileName($markdownPath)
    )
}
$validation | ConvertTo-Json -Depth 10 | Set-Content -LiteralPath $validationJsonPath -Encoding utf8

$lines = [System.Collections.Generic.List[string]]::new()
$lines.Add('# EX-010 phase and final report')
$lines.Add('')
$lines.Add("Source report: ``$resolvedReportPath``")
$lines.Add('')
$lines.Add('## Locked schedule')
$lines.Add('')
Add-MarkdownRow -Lines $lines -Values @('Phase', 'Ticks', 'Cumulative loadout')
Add-MarkdownRow -Lines $lines -Values @('---', '---', '---')
for ($index = 0; $index -lt $expectedSchedule.Count; $index++) {
    Add-MarkdownRow -Lines $lines -Values @(($index + 1), "$(($index) * 200):$(($index + 1) * 200)", $expectedSchedule[$index])
}
$lines.Add('')
$lines.Add('## Validation')
$lines.Add('')
$lines.Add("- Status: **$($validation.status)**")
$lines.Add("- Sequence: **$Sequence**")
$lines.Add("- Runs: $($validation.runCount)")
$lines.Add("- Phase Stat-Lines: ``phase-statlines.csv``")
$lines.Add("- Chronological deltas: ``phase-statline-deltas.csv``")
$lines.Add("- Independent final Stat-Lines: ``final-statlines.csv``")
foreach ($warning in $warnings) {
    $lines.Add("- Warning: $warning")
}
foreach ($error in $errors) {
    $lines.Add("- Error: $error")
}
$lines.Add('')
$lines.Add('The phase CSV contains one row per expected phase and visibly marks phases after an early terminal result as `NO_DATA`. Derived Stat-Line deltas remain `N/A` when either side is `N/A`; they are not converted to zero.')
$lines | Set-Content -LiteralPath $markdownPath -Encoding utf8

if ($errors.Count -gt 0) {
    throw "EX-010 report validation failed. See '$validationJsonPath'."
}

[pscustomobject]@{
    Status = $validation.status
    Validation = $validationJsonPath
    Report = $markdownPath
    PhaseStatLines = $phaseCsvPath
    PhaseDeltas = $deltaCsvPath
    FinalStatLines = $finalCsvPath
}
