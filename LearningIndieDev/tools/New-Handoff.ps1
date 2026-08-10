param(
    [Parameter(Mandatory = $true)]
    [string]$Owner,

    [Parameter(Mandatory = $true)]
    [string]$Topic,

    [ValidateSet("planned", "in-progress", "blocked", "ready-for-review", "shared")]
    [string]$Status = "in-progress"
)

$projectRoot = Split-Path -Parent $PSScriptRoot
$handoffDirectory = Join-Path $projectRoot "docs\handoffs"
$timestamp = Get-Date -Format "yyyy-MM-dd-HHmm"
$date = Get-Date -Format "yyyy-MM-dd"
$ownerSlug = ($Owner.ToLowerInvariant() -replace "[^a-z0-9]+", "-").Trim("-")
$topicSlug = ($Topic.ToLowerInvariant() -replace "[^a-z0-9]+", "-").Trim("-")

if ([string]::IsNullOrWhiteSpace($ownerSlug) -or [string]::IsNullOrWhiteSpace($topicSlug))
{
    throw "Owner and Topic must contain at least one letter or number."
}

$branch = (git -C $projectRoot rev-parse --abbrev-ref HEAD).Trim()
$commit = (git -C $projectRoot rev-parse --short HEAD).Trim()
$fileName = "$timestamp-$ownerSlug-$topicSlug.md"
$filePath = Join-Path $handoffDirectory $fileName

if (Test-Path -LiteralPath $filePath)
{
    throw "A handoff note already exists at $filePath"
}

New-Item -ItemType Directory -Path $handoffDirectory -Force | Out-Null

$content = @"
# $Topic

[Working state](../WORKING_STATE.md) | Status: $Status

- Owner: $Owner
- Branch: $branch
- Baseline commit: $commit
- Date: $date

## Summary

TODO: Explain the outcome and why this work matters.

## Changes

- TODO

## Decisions and assumptions

- TODO

## Validation

- TODO: Record only checks that actually ran and their results.

## Risks and incomplete work

- TODO

## Next useful step

TODO
"@

Set-Content -LiteralPath $filePath -Value $content -Encoding utf8
Write-Output $filePath
