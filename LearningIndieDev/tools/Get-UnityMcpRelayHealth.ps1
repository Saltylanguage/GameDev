[CmdletBinding()]
param(
    [int]$WarnUserRelayCount = 3,
    [switch]$Json
)

$ErrorActionPreference = 'Stop'

$profileRoot = [Environment]::GetFolderPath('UserProfile')
if ([string]::IsNullOrWhiteSpace($profileRoot)) {
    $profileRoot = $env:USERPROFILE
}
if ([string]::IsNullOrWhiteSpace($profileRoot)) {
    throw 'Could not resolve the current user profile path.'
}

$userRelayRoot = Join-Path $profileRoot '.unity\relay'
$relays = @(Get-Process -Name 'relay_win' -ErrorAction SilentlyContinue | ForEach-Object {
    $path = $_.Path
    $source = if ($path -like "$userRelayRoot\*") {
        'Codex user relay'
    }
    elseif ($path -match 'com\.unity\.ai\.assistant') {
        'Unity AI Assistant package relay'
    }
    else {
        'Unknown relay'
    }

    [pscustomobject]@{
        Pid = $_.Id
        Source = $source
        StartTime = $_.StartTime
        WorkingSetMB = [math]::Round($_.WorkingSet64 / 1MB, 1)
        Path = $path
    }
})

$userRelays = @($relays | Where-Object Source -eq 'Codex user relay')
$packageRelays = @($relays | Where-Object Source -eq 'Unity AI Assistant package relay')
$workingSetMB = [math]::Round(($relays | Measure-Object WorkingSetMB -Sum).Sum, 1)
$status = if ($userRelays.Count -gt $WarnUserRelayCount) { 'WARNING' } else { 'OK' }

$report = [pscustomobject]@{
    Status = $status
    TotalRelayProcesses = $relays.Count
    CodexUserRelayProcesses = $userRelays.Count
    UnityAssistantPackageRelays = $packageRelays.Count
    TotalWorkingSetMB = $workingSetMB
    Processes = @($relays | Sort-Object StartTime)
}

if ($Json) {
    $report | ConvertTo-Json -Depth 4
    exit $(if ($status -eq 'WARNING') { 1 } else { 0 })
}

Write-Output "Unity MCP relay health: $status"
Write-Output "Total relay processes: $($report.TotalRelayProcesses)"
Write-Output "Codex user relays: $($report.CodexUserRelayProcesses)"
Write-Output "Unity Assistant package relays: $($report.UnityAssistantPackageRelays)"
Write-Output "Combined working set: $($report.TotalWorkingSetMB) MB"

if ($status -eq 'WARNING') {
    Write-Warning "More than one Codex user relay is running. Keep one active Codex-to-Unity connection and close stale Codex sessions before reconnecting. This script does not terminate processes."
}

if ($relays.Count -gt 0) {
    $relays | Sort-Object StartTime | Format-Table Pid,Source,StartTime,WorkingSetMB,Path -AutoSize
}

exit $(if ($status -eq 'WARNING') { 1 } else { 0 })
