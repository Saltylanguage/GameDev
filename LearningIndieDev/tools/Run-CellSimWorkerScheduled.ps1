[CmdletBinding()]
param()

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$project = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$logDirectory = Join-Path $project 'artifacts'
$logPath = Join-Path $logDirectory 'cellsim-worker-scheduled.log'
New-Item -ItemType Directory -Path $logDirectory -Force | Out-Null

try {
    Start-Transcript -LiteralPath $logPath -Append | Out-Null
    & (Join-Path $PSScriptRoot 'Start-CellSimWorker.ps1') -AutoSync -AutoPublish -PollSeconds 15
    if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
}
catch {
    Write-Error $_
    exit 1
}
finally {
    Stop-Transcript | Out-Null
}
