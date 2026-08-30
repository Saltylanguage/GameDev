[CmdletBinding()]
param(
    [string]$QueueRoot = (Join-Path $PSScriptRoot '..\automation\CellSimQueue'),
    [int]$PollSeconds = 10,
    [switch]$Once
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$project = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$pending = Join-Path $QueueRoot 'Pending'
$running = Join-Path $QueueRoot 'Running'
$completed = Join-Path $QueueRoot 'Completed'
$failed = Join-Path $QueueRoot 'Failed'
foreach ($directory in @($pending, $running, $completed, $failed)) {
    New-Item -ItemType Directory -Path $directory -Force | Out-Null
}

function Update-JobFile([string]$Path, [hashtable]$Changes) {
    $job = Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
    foreach ($key in $Changes.Keys) { $job | Add-Member -NotePropertyName $key -NotePropertyValue $Changes[$key] -Force }
    $job | ConvertTo-Json -Depth 12 | Set-Content -LiteralPath $Path -Encoding utf8
}

function Invoke-Job([string]$Path) {
    $job = Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
    $parameters = @{}
    foreach ($property in $job.parameters.psobject.Properties) {
        if ($null -ne $property.Value -and -not ($property.Value -is [string] -and [string]::IsNullOrWhiteSpace($property.Value))) {
            $parameters[$property.Name] = $property.Value
        }
    }
    $parameters.ProjectPath = $project
    $parameters.PlayerSpeciesId = [string]$parameters.PlayerSpeciesId
    $parameters.SeedStart = [int]$parameters.SeedStart
    $parameters.SeedCount = [int]$parameters.SeedCount
    $started = [DateTime]::UtcNow
    Update-JobFile $Path @{ status = 'running'; startedUtc = $started.ToString('O'); worker = $env:COMPUTERNAME }
    try {
        $result = & (Join-Path $project 'tools\Run-CellularExperiment.ps1') @parameters
        $job = Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
        $job.status = 'completed'
        $job | Add-Member -NotePropertyName completedUtc -NotePropertyValue ([DateTime]::UtcNow.ToString('O')) -Force
        $job | Add-Member -NotePropertyName result -NotePropertyValue $result -Force
        $destination = Join-Path $completed (Split-Path $Path -Leaf)
        $job | ConvertTo-Json -Depth 16 | Set-Content -LiteralPath $destination -Encoding utf8
        Remove-Item -LiteralPath $Path -Force
        return $true
    }
    catch {
        $job = Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
        $job.status = 'failed'
        $job | Add-Member -NotePropertyName completedUtc -NotePropertyValue ([DateTime]::UtcNow.ToString('O')) -Force
        $job | Add-Member -NotePropertyName error -NotePropertyValue $_.Exception.Message -Force
        $destination = Join-Path $failed (Split-Path $Path -Leaf)
        $job | ConvertTo-Json -Depth 16 | Set-Content -LiteralPath $destination -Encoding utf8
        Remove-Item -LiteralPath $Path -Force
        return $false
    }
}

do {
    $jobs = @(Get-ChildItem -LiteralPath $pending -Filter '*.json' -File | Sort-Object Name)
    foreach ($job in $jobs) {
        $claimed = Join-Path $running $job.Name
        try {
            Move-Item -LiteralPath $job.FullName -Destination $claimed -ErrorAction Stop
            Invoke-Job $claimed | Out-Null
        }
        catch { Write-Warning "Could not claim job '$($job.Name)': $($_.Exception.Message)" }
    }
    if (-not $Once) { Start-Sleep -Seconds $PollSeconds }
} while (-not $Once)
