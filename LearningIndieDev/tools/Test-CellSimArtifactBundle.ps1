[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [string]$ArtifactDirectory,
    [switch]$RequireUnityLog
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$artifact = (Resolve-Path -LiteralPath $ArtifactDirectory -ErrorAction Stop).Path
$errors = [System.Collections.Generic.List[string]]::new()
$warnings = [System.Collections.Generic.List[string]]::new()

function Add-Error([string]$Message) {
    $errors.Add($Message)
}

function Add-Warning([string]$Message) {
    $warnings.Add($Message)
}

function Get-ManifestProperty {
    param(
        [Parameter(Mandatory)] [object]$Manifest,
        [Parameter(Mandatory)] [string]$Name
    )

    $property = $Manifest.PSObject.Properties[$Name]
    if ($null -eq $property) { return $null }
    return $property.Value
}

function Get-FileSha256([string]$Path) {
    $text = [System.IO.File]::ReadAllText($Path, [System.Text.UTF8Encoding]::new($false, $true))
    $canonicalText = $text.Replace("`r`n", "`n").Replace("`r", "`n")
    $bytes = [System.Text.UTF8Encoding]::new($false).GetBytes($canonicalText)
    $sha256 = [System.Security.Cryptography.SHA256]::Create()
    try {
        return ([System.BitConverter]::ToString($sha256.ComputeHash($bytes))).Replace('-', '').ToLowerInvariant()
    }
    finally {
        $sha256.Dispose()
    }
}

$requiredFiles = [System.Collections.Generic.List[string]]::new()
$requiredFiles.Add('report.json')
$requiredFiles.Add('report.csv')
$requiredFiles.Add('manifest.json')
if ($RequireUnityLog) { $requiredFiles.Add('unity.log') }

foreach ($fileName in $requiredFiles) {
    if (-not (Test-Path -LiteralPath (Join-Path $artifact $fileName) -PathType Leaf)) {
        Add-Error "Missing required artifact '$fileName'."
    }
}

$reportPath = Join-Path $artifact 'report.json'
$manifestPath = Join-Path $artifact 'manifest.json'
$report = $null
$manifest = $null

if (Test-Path -LiteralPath $reportPath -PathType Leaf) {
    try {
        $report = Get-Content -LiteralPath $reportPath -Raw | ConvertFrom-Json
    }
    catch {
        Add-Error "report.json is not valid JSON: $($_.Exception.Message)"
    }
}

if (Test-Path -LiteralPath $manifestPath -PathType Leaf) {
    try {
        $manifest = Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json
    }
    catch {
        Add-Error "manifest.json is not valid JSON: $($_.Exception.Message)"
    }
}

if ($null -ne $report) {
    $seedCountProperty = $report.PSObject.Properties['seedCount']
    $runsProperty = $report.PSObject.Properties['runs']
    if ($null -eq $seedCountProperty -or $null -eq $runsProperty) {
        Add-Error 'report.json is missing seedCount or runs.'
    }
    else {
        $seedCount = [int]$seedCountProperty.Value
        $runCount = @($runsProperty.Value).Count
        if ($seedCount -le 0) { Add-Error "Report seedCount must be positive; found $seedCount." }
        if ($runCount -ne $seedCount) {
            Add-Error "Report contains $runCount runs but seedCount is $seedCount."
        }

        $reportCsvPath = Join-Path $artifact 'report.csv'
        if (Test-Path -LiteralPath $reportCsvPath -PathType Leaf) {
            try {
                $csvCount = @(Import-Csv -LiteralPath $reportCsvPath).Count
                if ($csvCount -ne $seedCount) {
                    Add-Error "report.csv contains $csvCount data rows but seedCount is $seedCount."
                }
            }
            catch {
                Add-Error "report.csv could not be parsed: $($_.Exception.Message)"
            }
        }

        $isExperimentalHare = [string]$report.playerSpeciesId -eq 'hare' -and
            [string]$report.experimentalFeatures -eq 'bev-experimental'
        $statLinePath = Join-Path $artifact 'statline.csv'
        if ($isExperimentalHare -and -not (Test-Path -LiteralPath $statLinePath -PathType Leaf)) {
            Add-Error 'Experimental Hare report is missing statline.csv.'
        }
        if (Test-Path -LiteralPath $statLinePath -PathType Leaf) {
            try {
                $statLineCount = @(Import-Csv -LiteralPath $statLinePath).Count
                if ($statLineCount -ne $seedCount) {
                    Add-Error "statline.csv contains $statLineCount data rows but seedCount is $seedCount."
                }
            }
            catch {
                Add-Error "statline.csv could not be parsed: $($_.Exception.Message)"
            }
        }
    }
}

if ($null -ne $manifest) {
    $reportFile = [string](Get-ManifestProperty -Manifest $manifest -Name 'reportFile')
    if ($reportFile -ne 'report.json') { Add-Error "Manifest reportFile is '$reportFile', expected 'report.json'." }

    foreach ($name in @('sourceCommit', 'scenarioAssetPath', 'scenarioAssetGuid', 'reportSha256')) {
        if ([string]::IsNullOrWhiteSpace([string](Get-ManifestProperty -Manifest $manifest -Name $name))) {
            Add-Error "Manifest is missing '$name'."
        }
    }

    $expectedHash = [string](Get-ManifestProperty -Manifest $manifest -Name 'reportSha256')
    if ((Test-Path -LiteralPath $reportPath -PathType Leaf) -and -not [string]::IsNullOrWhiteSpace($expectedHash)) {
        $actualHash = Get-FileSha256 -Path $reportPath
        if ($actualHash -ne $expectedHash.ToLowerInvariant()) {
            Add-Error 'Manifest reportSha256 does not match report.json.'
        }
    }

    if ($null -eq $manifest.PSObject.Properties['sourceTreeDirtyBeforeRun'] -or
        $null -eq $manifest.PSObject.Properties['sourceTreeDirtyAfterRun']) {
        Add-Warning 'Manifest does not expose explicit before/after source-tree state; rerun with current worker tooling.'
    }
}

$status = if ($errors.Count -gt 0) { 'INVALID' } elseif ($warnings.Count -gt 0) { 'VALID_WITH_WARNINGS' } else { 'VALID' }
$result = [pscustomobject]@{
    status = $status
    artifactDirectory = $artifact
    requiredFiles = @($requiredFiles)
    errors = @($errors)
    warnings = @($warnings)
}
$result | ConvertTo-Json -Depth 8
if ($errors.Count -gt 0) { exit 1 }
