[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$ReportPath,
    [string]$OutputDirectory,
    [double]$Tolerance = 0.00001,
    [string]$SpeciesId
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Get-RequiredProperty {
    param(
        [Parameter(Mandatory)] [object]$Object,
        [Parameter(Mandatory)] [string]$Name,
        [Parameter(Mandatory)] [string]$Context
    )

    $property = $Object.PSObject.Properties[$Name]
    if ($null -eq $property -or $null -eq $property.Value) {
        throw "Missing required property '$Name' in $Context."
    }

    return $property.Value
}

function Get-OptionalProperty {
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

function Get-PopulationAtEdge {
    param(
        [Parameter(Mandatory)] [object]$Run,
        [Parameter(Mandatory)] [string]$SpeciesId,
        [Parameter(Mandatory)] [ValidateSet('First', 'Last')] [string]$Edge
    )

    $history = @((Get-OptionalProperty -Object $Run -Name 'populationHistory'))
    if ($history.Count -eq 0) {
        return $null
    }

    $snapshot = if ($Edge -eq 'First') { $history[0] } else { $history[-1] }
    $species = @($snapshot.species | Where-Object { $_.speciesId -eq $SpeciesId } | Select-Object -First 1)
    if ($species.Count -ne 1) {
        return $null
    }

    return [int](Get-RequiredProperty -Object $species[0] -Name 'population' -Context "$Edge population snapshot for $SpeciesId")
}

function Get-ActivityCount {
    param(
        [Parameter(Mandatory)] [object]$Run,
        [Parameter(Mandatory)] [string]$SpeciesId,
        [Parameter(Mandatory)] [string]$PropertyName
    )

    $activity = @((Get-OptionalProperty -Object $Run -Name 'activity') | Where-Object { $_.speciesId -eq $SpeciesId } | Select-Object -First 1)
    if ($activity.Count -ne 1) {
        return $null
    }

    $value = Get-OptionalProperty -Object $activity[0] -Name $PropertyName
    if ($null -eq $value) {
        return $null
    }

    return [int]$value
}

function Get-DeathEventCount {
    param(
        [Parameter(Mandatory)] [object]$Run,
        [Parameter(Mandatory)] [string]$SpeciesId,
        [Parameter(Mandatory)] [string]$Cause
    )

    $events = @((Get-OptionalProperty -Object $Run -Name 'deathEvents') | Where-Object {
        $_.isCreature -eq $true -and $_.speciesId -eq $SpeciesId -and $_.cause -eq $Cause
    })
    return $events.Count
}

function New-RawCrossCheck {
    param(
        [Parameter(Mandatory)] [string]$Name,
        [Parameter(Mandatory)] [string]$Source,
        [AllowNull()] [object]$IndependentValue,
        [AllowNull()] [object]$ReportValue
    )

    if ($null -eq $IndependentValue) {
        return [pscustomobject]@{
            Name = $Name
            Source = $Source
            IndependentValue = $null
            ReportValue = $ReportValue
            Status = 'UNAVAILABLE'
        }
    }

    return [pscustomobject]@{
        Name = $Name
        Source = $Source
        IndependentValue = $IndependentValue
        ReportValue = $ReportValue
        Status = if ($IndependentValue -eq $ReportValue) { 'PASS' } else { 'MISMATCH' }
    }
}

function Get-RateResult {
    param(
        [int]$Numerator,
        [int]$Denominator
    )

    if ($Numerator -lt 0 -or $Denominator -lt 0 -or $Numerator -gt $Denominator) {
        return [pscustomobject]@{ Value = $null; Status = 'INVALID' }
    }

    if ($Denominator -eq 0) {
        return [pscustomobject]@{
            Value = $null
            Status = if ($Numerator -eq 0) { 'N/A' } else { 'INVALID' }
        }
    }

    return [pscustomobject]@{
        Value = 1d - ([double]$Numerator / [double]$Denominator)
        Status = 'VALID'
    }
}

function Get-BirthAverageResult {
    param(
        [int]$Births,
        [int]$Mating
    )

    if ($Births -lt 0 -or $Mating -lt 0 -or $Births -gt $Mating) {
        return [pscustomobject]@{ Value = $null; Status = 'INVALID' }
    }

    if ($Mating -eq 0) {
        return [pscustomobject]@{
            Value = $null
            Status = if ($Births -eq 0) { 'N/A' } else { 'INVALID' }
        }
    }

    return [pscustomobject]@{
        Value = [double]$Births / [double]$Mating
        Status = 'VALID'
    }
}

function Get-ApplicableAverageResult {
    param(
        [Parameter(Mandatory)] [object]$First,
        [Parameter(Mandatory)] [object]$Second
    )

    if ($First.Status -eq 'INVALID' -or $Second.Status -eq 'INVALID') {
        return [pscustomobject]@{ Value = $null; Status = 'INVALID' }
    }

    $values = @(@($First, $Second) | Where-Object { $_.Status -eq 'VALID' })
    if ($values.Count -eq 0) {
        return [pscustomobject]@{ Value = $null; Status = 'N/A' }
    }

    return [pscustomobject]@{
        Value = [double](($values | Measure-Object -Property Value -Average).Average)
        Status = 'VALID'
    }
}

function Get-GameMetricStatus {
    param(
        [Parameter(Mandatory)] [object]$Stat,
        [Parameter(Mandatory)] [string]$PropertyName
    )

    $value = Get-OptionalProperty -Object $Stat -Name $PropertyName
    if ($null -eq $value) {
        return 'MISSING'
    }

    return ([string]$value).ToUpperInvariant()
}

function Compare-Metric {
    param(
        [Parameter(Mandatory)] [string]$Name,
        [Parameter(Mandatory)] [object]$Independent,
        [Parameter(Mandatory)] [object]$Stat,
        [Parameter(Mandatory)] [string]$GameValueProperty,
        [Parameter(Mandatory)] [string]$GameStatusProperty,
        [double]$AllowedTolerance
    )

    $gameStatus = Get-GameMetricStatus -Stat $Stat -PropertyName $GameStatusProperty
    $gameValue = Get-OptionalProperty -Object $Stat -Name $GameValueProperty
    $difference = $null
    $pass = $gameStatus -eq $Independent.Status

    if ($Independent.Status -eq 'VALID') {
        if ($null -eq $gameValue) {
            $pass = $false
        } else {
            $difference = [math]::Abs([double]$gameValue - [double]$Independent.Value)
            $pass = $pass -and $difference -le $AllowedTolerance
        }
    }

    return [pscustomobject]@{
        Name = $Name
        IndependentValue = $Independent.Value
        IndependentStatus = $Independent.Status
        GameValue = $gameValue
        GameStatus = $gameStatus
        AbsoluteDifference = $difference
        Passed = $pass
    }
}

function Format-ValidationValue {
    param([object]$Value)

    if ($null -eq $Value) {
        return 'N/A'
    }

    if ($Value -is [double] -or $Value -is [single] -or $Value -is [decimal]) {
        return ([double]$Value).ToString('0.########', [Globalization.CultureInfo]::InvariantCulture)
    }

    return [string]$Value
}

function Add-MarkdownTableRow {
    param(
        [Parameter(Mandatory)] [object[]]$Values
    )

    $escaped = $Values | ForEach-Object { ([string]$_).Replace('|', '\|') }
    $script:lines.Add('| ' + ($escaped -join ' | ') + ' |')
}

$resolvedReportPath = (Resolve-Path -LiteralPath $ReportPath -ErrorAction Stop).Path
$report = Get-Content -Raw -LiteralPath $resolvedReportPath | ConvertFrom-Json
$runs = @($report.runs)

if ($runs.Count -eq 0) {
    throw "Report '$resolvedReportPath' contains no runs."
}

if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Split-Path -Parent $resolvedReportPath
}

New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null
$validationRuns = [System.Collections.Generic.List[object]]::new()

foreach ($run in $runs) {
    $stat = Get-OptionalProperty -Object $run -Name 'herbivoreStatLine'
    if ($null -eq $stat) {
        continue
    }

    $statSpeciesId = [string](Get-RequiredProperty -Object $stat -Name 'speciesId' -Context "run $($run.seed) herbivoreStatLine")
    if (-not [string]::IsNullOrWhiteSpace($SpeciesId) -and $statSpeciesId -ne $SpeciesId) {
        continue
    }

    $SPO = [int](Get-RequiredProperty $stat SPO "run $($run.seed) herbivoreStatLine")
    $HPS = [int](Get-RequiredProperty $stat HPS "run $($run.seed) herbivoreStatLine")
    $EHS = [int](Get-RequiredProperty $stat EHS "run $($run.seed) herbivoreStatLine")
    $ECN = [int](Get-RequiredProperty $stat ECN "run $($run.seed) herbivoreStatLine")
    $PREY = [int](Get-RequiredProperty $stat PREY "run $($run.seed) herbivoreStatLine")
    $STRV = [int](Get-RequiredProperty $stat STRV "run $($run.seed) herbivoreStatLine")
    $MAT = [int](Get-RequiredProperty $stat MAT "run $($run.seed) herbivoreStatLine")
    $BIR = [int](Get-RequiredProperty $stat BIR "run $($run.seed) herbivoreStatLine")
    $CRWD = [int](Get-RequiredProperty $stat CRWD "run $($run.seed) herbivoreStatLine")
    $FPO = [int](Get-RequiredProperty $stat FPO "run $($run.seed) herbivoreStatLine")

    $expectedFPO = $SPO + $BIR - $PREY - $STRV - $CRWD
    $fpoPass = $expectedFPO -eq $FPO
    $pAVI = Get-RateResult -Numerator $PREY -Denominator $ECN
    $eAVI = Get-RateResult -Numerator $EHS -Denominator $HPS
    $predAVG = Get-ApplicableAverageResult -First $pAVI -Second $eAVI
    $sAVI = Get-RateResult -Numerator $STRV -Denominator ($SPO + $BIR - $PREY)
    $cAVI = Get-RateResult -Numerator $CRWD -Denominator ($SPO + $BIR - $PREY - $STRV)
    $bAVG = Get-BirthAverageResult -Births $BIR -Mating $MAT

    if ($bAVG.Status -eq 'INVALID') {
        $rfs = [pscustomobject]@{ Value = $null; Status = 'INVALID' }
    } elseif ($bAVG.Status -eq 'N/A') {
        $rfs = [pscustomobject]@{ Value = $null; Status = 'N/A' }
    } else {
        $rfs = [pscustomobject]@{
            Value = ([double]($FPO - $SPO)) * [double]$bAVG.Value
            Status = 'VALID'
        }
    }

    $components = @($predAVG, $sAVI, $cAVI, $bAVG, $rfs)
    $hasInvalidComponent = @($components | Where-Object { $_.Status -eq 'INVALID' }).Count -gt 0
    if (-not $fpoPass -or $hasInvalidComponent) {
        $aps = [pscustomobject]@{ Value = $null; Status = 'INVALID' }
    } else {
        $apsValue = 0d
        if ($rfs.Status -eq 'VALID') { $apsValue += $rfs.Value }
        if ($predAVG.Status -eq 'VALID') { $apsValue += $predAVG.Value }
        if ($sAVI.Status -eq 'VALID') { $apsValue -= 1d - $sAVI.Value }
        if ($cAVI.Status -eq 'VALID') { $apsValue -= 1d - $cAVI.Value }
        $aps = [pscustomobject]@{ Value = $apsValue; Status = 'VALID' }
    }

    $comparisons = @(
        [pscustomobject]@{
            Name = 'FPO'
            IndependentValue = $expectedFPO
            IndependentStatus = if ($fpoPass) { 'VALID' } else { 'INVALID' }
            GameValue = $FPO
            GameStatus = if ($fpoPass) { 'VALID' } else { 'INVALID' }
            AbsoluteDifference = [math]::Abs($expectedFPO - $FPO)
            Passed = $fpoPass -and ([bool](Get-OptionalProperty -Object $stat -Name 'fpoReconciled'))
        },
        (Compare-Metric -Name 'pAVI' -Independent $pAVI -Stat $stat -GameValueProperty 'pAVI' -GameStatusProperty 'pAVIStatus' -AllowedTolerance $Tolerance),
        (Compare-Metric -Name 'eAVI' -Independent $eAVI -Stat $stat -GameValueProperty 'eAVI' -GameStatusProperty 'eAVIStatus' -AllowedTolerance $Tolerance),
        (Compare-Metric -Name 'predAVG' -Independent $predAVG -Stat $stat -GameValueProperty 'predAVG' -GameStatusProperty 'predAVGStatus' -AllowedTolerance $Tolerance),
        (Compare-Metric -Name 'sAVI' -Independent $sAVI -Stat $stat -GameValueProperty 'sAVI' -GameStatusProperty 'sAVIStatus' -AllowedTolerance $Tolerance),
        (Compare-Metric -Name 'cAVI' -Independent $cAVI -Stat $stat -GameValueProperty 'cAVI' -GameStatusProperty 'cAVIStatus' -AllowedTolerance $Tolerance),
        (Compare-Metric -Name 'bAVG' -Independent $bAVG -Stat $stat -GameValueProperty 'bAVG' -GameStatusProperty 'bAVGStatus' -AllowedTolerance $Tolerance),
        (Compare-Metric -Name 'RFS' -Independent $rfs -Stat $stat -GameValueProperty 'RFS' -GameStatusProperty 'RFSStatus' -AllowedTolerance $Tolerance),
        (Compare-Metric -Name 'APS' -Independent $aps -Stat $stat -GameValueProperty 'APS' -GameStatusProperty 'APSStatus' -AllowedTolerance $Tolerance)
    )

    $rawCrossChecks = @(
        (New-RawCrossCheck -Name 'SPO' -Source 'first populationHistory snapshot' -IndependentValue (Get-PopulationAtEdge -Run $run -SpeciesId $statSpeciesId -Edge First) -ReportValue $SPO),
        (New-RawCrossCheck -Name 'FPO' -Source 'last populationHistory snapshot' -IndependentValue (Get-PopulationAtEdge -Run $run -SpeciesId $statSpeciesId -Edge Last) -ReportValue $FPO),
        (New-RawCrossCheck -Name 'MAT' -Source 'activity.reproductionCandidates' -IndependentValue (Get-ActivityCount -Run $run -SpeciesId $statSpeciesId -PropertyName 'reproductionCandidates') -ReportValue $MAT),
        (New-RawCrossCheck -Name 'BIR' -Source 'activity.births' -IndependentValue (Get-ActivityCount -Run $run -SpeciesId $statSpeciesId -PropertyName 'births') -ReportValue $BIR),
        (New-RawCrossCheck -Name 'STRV' -Source 'creature deathEvents with cause Starvation' -IndependentValue (Get-DeathEventCount -Run $run -SpeciesId $statSpeciesId -Cause 'Starvation') -ReportValue $STRV),
        (New-RawCrossCheck -Name 'CRWD' -Source 'creature deathEvents with cause Crowding' -IndependentValue (Get-DeathEventCount -Run $run -SpeciesId $statSpeciesId -Cause 'Crowding') -ReportValue $CRWD),
        (New-RawCrossCheck -Name 'PREY' -Source 'creature deathEvents with cause Combat' -IndependentValue (Get-DeathEventCount -Run $run -SpeciesId $statSpeciesId -Cause 'Combat') -ReportValue $PREY),
        (New-RawCrossCheck -Name 'HPS' -Source 'no per-step predator-active population event list in current report schema' -IndependentValue $null -ReportValue $HPS),
        (New-RawCrossCheck -Name 'EHS' -Source 'no per-step encountered-herbivore event list in current report schema' -IndependentValue $null -ReportValue $EHS),
        (New-RawCrossCheck -Name 'ECN' -Source 'no per-encounter target event list in current report schema' -IndependentValue $null -ReportValue $ECN)
    )
    $hasRawCrossCheckMismatch = @($rawCrossChecks | Where-Object { $_.Status -eq 'MISMATCH' }).Count -gt 0
    $hasRawCrossCheckLimitation = @($rawCrossChecks | Where-Object { $_.Status -eq 'UNAVAILABLE' }).Count -gt 0

    $validationRuns.Add([pscustomobject]@{
        Seed = [int]$run.seed
        SpeciesId = $statSpeciesId
        RawCounts = [ordered]@{ SPO = $SPO; HPS = $HPS; EHS = $EHS; ECN = $ECN; PREY = $PREY; STRV = $STRV; MAT = $MAT; BIR = $BIR; CRWD = $CRWD; FPO = $FPO }
        ExpectedFPO = $expectedFPO
        Comparisons = $comparisons
        RawCrossChecks = $rawCrossChecks
        HasRawCrossCheckLimitation = $hasRawCrossCheckLimitation
        Passed = @($comparisons | Where-Object { -not $_.Passed }).Count -eq 0 -and -not $hasRawCrossCheckMismatch
    })
}

if ($validationRuns.Count -eq 0) {
    throw "Report '$resolvedReportPath' contains no herbivore stat lines matching the requested species."
}

$allPassed = @($validationRuns | Where-Object { -not $_.Passed }).Count -eq 0
$hasLimitations = @($validationRuns | Where-Object { $_.HasRawCrossCheckLimitation }).Count -gt 0
$result = [ordered]@{
    validation = if (-not $allPassed) { 'NOT_VALIDATED' } elseif ($hasLimitations) { 'VALIDATED_WITH_LIMITATIONS' } else { 'VALIDATED' }
    generatedUtc = [DateTime]::UtcNow.ToString('o')
    reportPath = $resolvedReportPath
    schemaVersion = Get-OptionalProperty -Object $report -Name 'schemaVersion'
    scenarioAssetPath = Get-OptionalProperty -Object $report -Name 'scenarioAssetPath'
    rulesetFingerprint = Get-OptionalProperty -Object $report -Name 'rulesetFingerprint'
    playerSpeciesId = Get-OptionalProperty -Object $report -Name 'playerSpeciesId'
    seedStart = Get-OptionalProperty -Object $report -Name 'seedStart'
    seedCount = Get-OptionalProperty -Object $report -Name 'seedCount'
    tolerance = $Tolerance
    independentCalculator = 'Validate-HerbivoreStatLine.ps1'
    runs = $validationRuns
}

$jsonPath = Join-Path $OutputDirectory 'herbivore-stat-validation.json'
$markdownPath = Join-Path $OutputDirectory 'herbivore-stat-validation.md'
$result | ConvertTo-Json -Depth 12 | Set-Content -LiteralPath $jsonPath -Encoding utf8

$lines = [System.Collections.Generic.List[string]]::new()
$lines.Add('')
$lines.Add('# Herbivore Slash-Line Forward Validation')
$lines.Add('')
$lines.Add("Status: **$($result.validation)**")
$lines.Add('')
$lines.Add('Source report: ' + $resolvedReportPath)
$lines.Add('Tolerance for exported floats: ' + $Tolerance)
$lines.Add('')
$lines.Add('## Raw counts used')
$lines.Add('')
Add-MarkdownTableRow -Values @('Seed', 'Species', 'SPO', 'HPS', 'EHS', 'ECN', 'PREY', 'STRV', 'MAT', 'BIR', 'CRWD', 'FPO', 'Expected FPO')
Add-MarkdownTableRow -Values @('---', '---', '---:', '---:', '---:', '---:', '---:', '---:', '---:', '---:', '---:', '---:', '---:')
foreach ($validationRun in $validationRuns) {
    $raw = $validationRun.RawCounts
    Add-MarkdownTableRow -Values @($validationRun.Seed, $validationRun.SpeciesId, $raw.SPO, $raw.HPS, $raw.EHS, $raw.ECN, $raw.PREY, $raw.STRV, $raw.MAT, $raw.BIR, $raw.CRWD, $raw.FPO, $validationRun.ExpectedFPO)
}

$lines.Add('')
$lines.Add('## Supporting raw evidence checks')
$lines.Add('')
Add-MarkdownTableRow -Values @('Seed', 'Raw field', 'Independent source', 'Report count', 'Status')
Add-MarkdownTableRow -Values @('---', '---', '---', '---:', '---')
foreach ($validationRun in $validationRuns) {
    foreach ($crossCheck in $validationRun.RawCrossChecks) {
        Add-MarkdownTableRow -Values @(
            $validationRun.Seed,
            $crossCheck.Name,
            $crossCheck.Source,
            (Format-ValidationValue $crossCheck.ReportValue),
            $crossCheck.Status
        )
    }
}

$lines.Add('')
$lines.Add('## Independent calculation comparison')
$lines.Add('')
Add-MarkdownTableRow -Values @('Seed', 'Statistic', 'Independent', 'Game export', 'Abs diff', 'Independent status', 'Game status', 'Pass')
Add-MarkdownTableRow -Values @('---', '---', '---:', '---:', '---:', '---', '---', '---')
foreach ($validationRun in $validationRuns) {
    foreach ($comparison in $validationRun.Comparisons) {
        Add-MarkdownTableRow -Values @(
            $validationRun.Seed,
            $comparison.Name,
            (Format-ValidationValue $comparison.IndependentValue),
            (Format-ValidationValue $comparison.GameValue),
            (Format-ValidationValue $comparison.AbsoluteDifference),
            $comparison.IndependentStatus,
            $comparison.GameStatus,
            $comparison.Passed
        )
    }
}

$lines.Add('')
$lines.Add('Fixed numeric fixtures are reserved for calculator edge-case tests. This report uses raw counts from the actual simulation report and does not reverse-solve or modify them.')
$lines | Set-Content -LiteralPath $markdownPath -Encoding utf8

[pscustomobject]@{
    Validation = $result.validation
    Runs = $validationRuns.Count
    Json = $jsonPath
    Markdown = $markdownPath
}

if (-not $allPassed) {
    throw "Independent herbivore stat validation failed. See '$markdownPath'."
}
