[CmdletBinding()]
param(
    [string]$HandoffDirectory,
    [switch]$ShowWarnings,
    [switch]$TreatWarningsAsErrors
)

$ErrorActionPreference = 'Stop'
$projectRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
if ([string]::IsNullOrWhiteSpace($HandoffDirectory)) {
    $HandoffDirectory = Join-Path $PSScriptRoot '..\docs\handoffs'
}
$handoffRoot = Resolve-Path $HandoffDirectory
$errors = [System.Collections.Generic.List[string]]::new()
$warnings = [System.Collections.Generic.List[string]]::new()
$ids = @{}
$allowedStatuses = @('planned', 'in-progress', 'blocked', 'ready-for-review', 'shared', 'complete')

function Read-Field([string]$Text, [string]$Name) {
    $match = [regex]::Match($Text, "(?m)^- $([regex]::Escape($Name)):\s*(.+?)\s*$")
    if ($match.Success) { return $match.Groups[1].Value.Trim() }
    return $null
}

function Add-LinkIssue([System.IO.FileInfo]$File, [string]$Message, [bool]$Strict) {
    $fullMessage = "$($File.Name): $Message"
    if ($Strict) { $errors.Add($fullMessage) } else { $warnings.Add($fullMessage) }
}

$files = @(Get-ChildItem -LiteralPath $handoffRoot -File -Filter '*.md' |
    Where-Object { $_.Name -ne 'README.md' } |
    Sort-Object Name)

foreach ($file in $files) {
    $text = Get-Content -Raw -LiteralPath $file.FullName
    $schema = Read-Field $text 'Handoff schema'
    $strict = $schema -eq '1'

    if ($null -ne $schema -and -not $strict) {
        $errors.Add("$($file.Name): unsupported handoff schema '$schema'.")
        continue
    }

    if ($strict) {
        $id = Read-Field $text 'Handoff ID'
        $statusMatch = [regex]::Match($text, '(?m)^\[Working state\]\([^\r\n]+\)\s*\|\s*Status:\s*(.+?)\s*$')
        $status = if ($statusMatch.Success) { $statusMatch.Groups[1].Value.Trim() } else { $null }
        $required = @{
            'Handoff ID' = $id
            'Status' = $status
            'Owner' = Read-Field $text 'Owner'
            'Branch' = Read-Field $text 'Branch'
            'Baseline commit' = Read-Field $text 'Baseline commit'
            'Date' = Read-Field $text 'Date'
            'Supersedes' = Read-Field $text 'Supersedes'
        }

        foreach ($name in $required.Keys) {
            $value = [string]$required[$name]
            if ([string]::IsNullOrWhiteSpace($value) -or $value -match '^TODO\b') {
                $errors.Add("$($file.Name): missing required field '$name'.")
            }
        }

        if (-not [string]::IsNullOrWhiteSpace($id)) {
            if ($id -ne $file.BaseName) {
                $errors.Add("$($file.Name): Handoff ID '$id' must match the filename.")
            }
            if ($ids.ContainsKey($id)) {
                $errors.Add("$($file.Name): duplicate Handoff ID '$id'.")
            } else {
                $ids[$id] = $file.Name
            }
        }

        if (-not [string]::IsNullOrWhiteSpace($status) -and $allowedStatuses -notcontains $status) {
            $errors.Add("$($file.Name): unsupported status '$status'.")
        }

        $supersedes = [string]$required['Supersedes']
        if (-not [string]::IsNullOrWhiteSpace($supersedes) -and $supersedes -ne 'none') {
            foreach ($target in $supersedes.Split(',')) {
                $targetName = $target.Trim().Trim('`')
                if (-not (Test-Path -LiteralPath (Join-Path $handoffRoot $targetName))) {
                    $errors.Add("$($file.Name): superseded handoff is missing: $targetName")
                }
            }
        }
    }

    foreach ($match in [regex]::Matches($text, '\[[^\]]+\]\((?!https?://|mailto:|#)([^)#]+)(?:#[^)]+)?\)')) {
        $target = [uri]::UnescapeDataString($match.Groups[1].Value)
        if ($target -match '[<>*]' -or $target -match '\.\.\.') { continue }
        $resolved = Join-Path $file.DirectoryName $target
        if (-not (Test-Path -LiteralPath $resolved)) {
            Add-LinkIssue $file "local link is missing: $target" $strict
        }
    }

    if ($strict) {
        foreach ($match in [regex]::Matches($text, '`(artifacts/[^`\r\n]+)`')) {
            $target = $match.Groups[1].Value
            if ($target -match '[<>*$]' -or $target -match '\.\.\.') { continue }
            $resolved = Join-Path $projectRoot ($target -replace '/', '\')
            if (-not (Test-Path -LiteralPath $resolved)) {
                $warnings.Add("$($file.Name): local artifact reference is unavailable: $target")
            }
        }
    }
}

if ($ShowWarnings) {
    foreach ($warning in $warnings) { Write-Warning $warning }
}

if ($errors.Count -gt 0) {
    throw "Handoff validation failed with $($errors.Count) error(s):`n- $($errors -join "`n- ")"
}
if ($TreatWarningsAsErrors -and $warnings.Count -gt 0) {
    throw "Handoff validation found $($warnings.Count) warning(s)."
}

Write-Output ("Handoffs valid: {0} note(s), {1} schema-1 note(s), {2} warning(s)." -f $files.Count, $ids.Count, $warnings.Count)
if ($warnings.Count -gt 0 -and -not $ShowWarnings) {
    Write-Output 'Re-run with -ShowWarnings to list unavailable links and artifacts.'
}
