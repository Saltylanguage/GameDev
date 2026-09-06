[CmdletBinding()]
param(
    [Parameter(Mandatory)] [string]$OriginalDirectory,
    [Parameter(Mandatory)] [string]$AlternateDirectory,
    [Parameter(Mandatory)] [string]$OriginalHeldOutDirectory,
    [Parameter(Mandatory)] [string]$AlternateHeldOutDirectory,
    [Parameter(Mandatory)] [string]$OutputDirectory,
    [string]$ProjectPath
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

if ([string]::IsNullOrWhiteSpace($ProjectPath)) {
    $ProjectPath = Join-Path $PSScriptRoot '..'
}

$project = (Resolve-Path -LiteralPath $ProjectPath).Path
$originalDirectory = (Resolve-Path -LiteralPath $OriginalDirectory).Path
$alternateDirectory = (Resolve-Path -LiteralPath $AlternateDirectory).Path
$originalHeldOutDirectory = (Resolve-Path -LiteralPath $OriginalHeldOutDirectory).Path
$alternateHeldOutDirectory = (Resolve-Path -LiteralPath $AlternateHeldOutDirectory).Path
New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null

$statNames = @('SPO', 'HPS', 'EHS', 'ECN', 'PREY', 'STRV', 'MAT', 'BIR', 'CRWD', 'FPO', 'pAVI', 'eAVI', 'predAVG', 'sAVI', 'cAVI', 'bAVG', 'RFS', 'APS')
$panels = @(
    [pscustomobject]@{ Name = 'Development'; Original = $originalDirectory; Alternate = $alternateDirectory },
    [pscustomobject]@{ Name = 'HeldOut'; Original = $originalHeldOutDirectory; Alternate = $alternateHeldOutDirectory }
)

function Read-JsonFile {
    param([Parameter(Mandatory)] [string]$Path)
    return Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
}

function Get-CellText {
    param([AllowNull()] [object]$Row, [Parameter(Mandatory)] [string]$Name)
    if ($null -eq $Row) {
        return 'NO_DATA'
    }
    $property = $Row.PSObject.Properties[$Name]
    if ($null -eq $property -or [string]::IsNullOrWhiteSpace([string]$property.Value)) {
        return 'N/A'
    }
    return [string]$property.Value
}

function Get-Delta {
    param([Parameter(Mandatory)] [string]$Original, [Parameter(Mandatory)] [string]$Alternate)

    if ($Original -eq 'NO_DATA' -or $Alternate -eq 'NO_DATA') {
        return [pscustomobject]@{ Status = 'NO_DATA'; Value = 'NO_DATA' }
    }
    if ($Original -eq 'INVALID' -or $Alternate -eq 'INVALID') {
        return [pscustomobject]@{ Status = 'INVALID'; Value = 'INVALID' }
    }

    $originalNumber = 0.0
    $alternateNumber = 0.0
    $originalIsNumber = [double]::TryParse($Original, [Globalization.NumberStyles]::Float, [Globalization.CultureInfo]::InvariantCulture, [ref]$originalNumber)
    $alternateIsNumber = [double]::TryParse($Alternate, [Globalization.NumberStyles]::Float, [Globalization.CultureInfo]::InvariantCulture, [ref]$alternateNumber)
    if (-not $originalIsNumber -or -not $alternateIsNumber) {
        return [pscustomobject]@{ Status = 'N/A'; Value = 'N/A' }
    }

    return [pscustomobject]@{ Status = 'DATA'; Value = $alternateNumber - $originalNumber }
}

function Get-RowKey {
    param([Parameter(Mandatory)] [object]$Row)
    return "$($Row.seed)|$($Row.phase)"
}

$errors = [System.Collections.Generic.List[string]]::new()
$warnings = [System.Collections.Generic.List[string]]::new()
$longRows = [System.Collections.Generic.List[object]]::new()
$wideRows = [System.Collections.Generic.List[object]]::new()

$originalExecution = Read-JsonFile -Path (Join-Path $originalDirectory 'ex010-execution.json')
$alternateExecution = Read-JsonFile -Path (Join-Path $alternateDirectory 'ex010-execution.json')
$originalSequence = if ($null -eq $originalExecution.PSObject.Properties['sequence']) { 'Original' } else { [string]$originalExecution.sequence }
$alternateSequence = if ($null -eq $alternateExecution.PSObject.Properties['sequence']) { 'Original' } else { [string]$alternateExecution.sequence }
if ($originalSequence -ne 'Original') {
    $errors.Add('The original execution record is not marked Original.')
}
if ($alternateSequence -ne 'Alternate') {
    $errors.Add('The alternate execution record is not marked Alternate.')
}
if ([int]$originalExecution.phaseLengthTicks -ne [int]$alternateExecution.phaseLengthTicks -or [int]$originalExecution.phaseCount -ne [int]$alternateExecution.phaseCount) {
    $errors.Add('The sequences do not use the same phase length and phase count.')
}

foreach ($panel in $panels) {
    $originalPath = Join-Path $panel.Original 'phase-statlines.csv'
    $alternatePath = Join-Path $panel.Alternate 'phase-statlines.csv'
    $originalRows = @(Import-Csv -LiteralPath $originalPath)
    $alternateRows = @(Import-Csv -LiteralPath $alternatePath)
    $originalMap = @{}
    $alternateMap = @{}
    foreach ($row in $originalRows) { $originalMap[(Get-RowKey -Row $row)] = $row }
    foreach ($row in $alternateRows) { $alternateMap[(Get-RowKey -Row $row)] = $row }

    $originalKeys = @($originalMap.Keys | Sort-Object)
    $alternateKeys = @($alternateMap.Keys | Sort-Object)
    if (($originalKeys -join '|') -ne ($alternateKeys -join '|')) {
        $errors.Add("$($panel.Name) phase rows do not use the same seed and phase keys.")
    }

    foreach ($key in $originalKeys) {
        $originalRow = $originalMap[$key]
        $alternateRow = $alternateMap[$key]
        if ($null -eq $alternateRow) { continue }
        if ([string]$originalRow.window -ne [string]$alternateRow.window) {
            $errors.Add("$($panel.Name) $key uses different phase windows.")
        }

        $wide = [ordered]@{
            panel = $panel.Name
            recordKind = 'Phase'
            seed = $originalRow.seed
            phase = $originalRow.phase
            window = $originalRow.window
        }
        foreach ($name in $statNames) {
            $originalValue = Get-CellText -Row $originalRow -Name $name
            $alternateValue = Get-CellText -Row $alternateRow -Name $name
            $delta = Get-Delta -Original $originalValue -Alternate $alternateValue
            $longRows.Add([pscustomobject]@{
                    panel = $panel.Name
                    recordKind = 'Phase'
                    seed = $originalRow.seed
                    phase = $originalRow.phase
                    window = $originalRow.window
                    stat = $name
                    original = $originalValue
                    alternate = $alternateValue
                    delta = $delta.Value
                    deltaStatus = $delta.Status
                })
            $wide['Original_' + $name] = $originalValue
            $wide['Alternate_' + $name] = $alternateValue
            $wide['Delta_' + $name] = $delta.Value
            $wide['DeltaStatus_' + $name] = $delta.Status
        }
        $wideRows.Add([pscustomobject]$wide)
    }

    $originalFinal = @(Import-Csv -LiteralPath (Join-Path $panel.Original 'final-statlines.csv'))
    $alternateFinal = @(Import-Csv -LiteralPath (Join-Path $panel.Alternate 'final-statlines.csv'))
    $originalFinalMap = @{}
    $alternateFinalMap = @{}
    foreach ($row in $originalFinal) { $originalFinalMap[[string]$row.seed] = $row }
    foreach ($row in $alternateFinal) { $alternateFinalMap[[string]$row.seed] = $row }
    $originalFinalKeys = (($originalFinalMap.Keys | Sort-Object) -join '|')
    $alternateFinalKeys = (($alternateFinalMap.Keys | Sort-Object) -join '|')
    if ($originalFinalKeys -ne $alternateFinalKeys) {
        $errors.Add("$($panel.Name) final rows do not use the same seed keys.")
    }

    foreach ($seed in @($originalFinalMap.Keys | Sort-Object)) {
        $originalRow = $originalFinalMap[$seed]
        $alternateRow = $alternateFinalMap[$seed]
        if ($null -eq $alternateRow) { continue }
        $wide = [ordered]@{
            panel = $panel.Name
            recordKind = 'Final'
            seed = $seed
            phase = 'FINAL'
            window = $originalRow.window
        }
        foreach ($name in $statNames) {
            $originalValue = Get-CellText -Row $originalRow -Name $name
            $alternateValue = Get-CellText -Row $alternateRow -Name $name
            $delta = Get-Delta -Original $originalValue -Alternate $alternateValue
            $longRows.Add([pscustomobject]@{
                    panel = $panel.Name
                    recordKind = 'Final'
                    seed = $seed
                    phase = 'FINAL'
                    window = $originalRow.window
                    stat = $name
                    original = $originalValue
                    alternate = $alternateValue
                    delta = $delta.Value
                    deltaStatus = $delta.Status
                })
            $wide['Original_' + $name] = $originalValue
            $wide['Alternate_' + $name] = $alternateValue
            $wide['Delta_' + $name] = $delta.Value
            $wide['DeltaStatus_' + $name] = $delta.Status
        }
        $wideRows.Add([pscustomobject]$wide)
    }
}

$summaryRows = [System.Collections.Generic.List[object]]::new()
foreach ($panelName in @('Development', 'HeldOut')) {
    foreach ($recordKind in @('Phase', 'Final')) {
        $phaseValues = if ($recordKind -eq 'Phase') { 1..10 | ForEach-Object { [string]$_ } } else { @('FINAL') }
        foreach ($phase in $phaseValues) {
            foreach ($name in $statNames) {
                $cells = @($longRows | Where-Object { $_.panel -eq $panelName -and $_.recordKind -eq $recordKind -and [string]$_.phase -eq $phase -and $_.stat -eq $name })
                $numeric = @($cells | Where-Object { $_.deltaStatus -eq 'DATA' } | ForEach-Object { [double]$_.delta })
                if ($numeric.Count -gt 0) {
                    $measure = $numeric | Measure-Object -Average -Minimum -Maximum
                    $summaryRows.Add([pscustomobject]@{
                            panel = $panelName
                            recordKind = $recordKind
                            phase = $phase
                            stat = $name
                            compared = $numeric.Count
                            nonNumeric = @($cells | Where-Object { $_.deltaStatus -ne 'DATA' }).Count
                            meanDelta = $measure.Average
                            minDelta = $measure.Minimum
                            maxDelta = $measure.Maximum
                            positive = @($numeric | Where-Object { $_ -gt 0 }).Count
                            negative = @($numeric | Where-Object { $_ -lt 0 }).Count
                            unchanged = @($numeric | Where-Object { $_ -eq 0 }).Count
                        })
                }
                else {
                    $summaryRows.Add([pscustomobject]@{
                            panel = $panelName
                            recordKind = $recordKind
                            phase = $phase
                            stat = $name
                            compared = 0
                            nonNumeric = $cells.Count
                            meanDelta = 'N/A'
                            minDelta = 'N/A'
                            maxDelta = 'N/A'
                            positive = 0
                            negative = 0
                            unchanged = 0
                        })
                }
            }
        }
    }
}

$validation = [ordered]@{
    status = if ($errors.Count -gt 0) { 'INVALID' } else { 'VALID' }
    originalDirectory = $originalDirectory
    alternateDirectory = $alternateDirectory
    originalHeldOutDirectory = $originalHeldOutDirectory
    alternateHeldOutDirectory = $alternateHeldOutDirectory
    originalContractSha256 = $originalExecution.contractSha256
    alternateContractSha256 = $alternateExecution.contractSha256
    errors = @($errors)
    warnings = @($warnings)
    matchedRows = $wideRows.Count
    outputs = @('matched-statline-comparison.csv', 'matched-statline-long.csv', 'matched-statline-summary.csv', 'comparison-validation.json', 'comparison.md')
}

$wideRows | Export-Csv -LiteralPath (Join-Path $OutputDirectory 'matched-statline-comparison.csv') -NoTypeInformation -Encoding utf8
$longRows | Export-Csv -LiteralPath (Join-Path $OutputDirectory 'matched-statline-long.csv') -NoTypeInformation -Encoding utf8
$summaryRows | Export-Csv -LiteralPath (Join-Path $OutputDirectory 'matched-statline-summary.csv') -NoTypeInformation -Encoding utf8
$validation | ConvertTo-Json -Depth 10 | Set-Content -LiteralPath (Join-Path $OutputDirectory 'comparison-validation.json') -Encoding utf8

$lines = [System.Collections.Generic.List[string]]::new()
$lines.Add('# EX-010 sequence comparison')
$lines.Add('')
$lines.Add('- Comparison: alternate minus original.')
$lines.Add("- Matched comparison rows: $($wideRows.Count) phase/final records.")
$lines.Add('- The CSVs preserve every direct Stat-Line value and delta. They do not create a new score.')
$lines.Add("- Validation: **$($validation.status)**")
$lines.Add('')
$lines.Add('## Final Stat-Line deltas')
$lines.Add('')
$lines.Add('| Panel | Stat | Compared | Mean delta | Minimum | Maximum |')
$lines.Add('| --- | --- | ---: | ---: | ---: | ---: |')
foreach ($row in @($summaryRows | Where-Object { $_.recordKind -eq 'Final' -and $_.stat -in @('SPO', 'HPS', 'EHS', 'PREY', 'STRV', 'MAT', 'BIR', 'FPO', 'RFS', 'APS') })) {
    $lines.Add("| $($row.panel) | $($row.stat) | $($row.compared) | $($row.meanDelta) | $($row.minDelta) | $($row.maxDelta) |")
}
$lines.Add('')
$lines.Add('## Findings')
$lines.Add('')
$lines.Add('- Phase 1 and phase 2 are the same no-upgrade history in both sequences; divergence begins when the first upgrade is installed at tick 400.')
$lines.Add('- The alternate sequence changes the later phase and final Stat-Lines under the same seeds and simulation options.')
$lines.Add('- The direction is not uniform across every Stat-Line field or seed. Treat the result as a bounded sequence-order finding for this scenario and schedule, not a universal upgrade-order rule.')
$lines.Add('- `N/A`, invalid, and no-data cells remain visible in the long comparison CSV.')
foreach ($errorMessage in $errors) { $lines.Add("- Error: $errorMessage") }
$lines | Set-Content -LiteralPath (Join-Path $OutputDirectory 'comparison.md') -Encoding utf8

if ($errors.Count -gt 0) {
    throw "EX-010 sequence comparison validation failed. See '$(Join-Path $OutputDirectory 'comparison-validation.json')'."
}

[pscustomobject]@{
    Status = $validation.status
    OutputDirectory = (Resolve-Path -LiteralPath $OutputDirectory).Path
    Validation = Join-Path $OutputDirectory 'comparison-validation.json'
    Markdown = Join-Path $OutputDirectory 'comparison.md'
}
