[CmdletBinding()]
param(
    [string]$ProjectPath,
    [string]$UnityPath,
    [ValidateRange(30, 360)]
    [int]$TimeoutSeconds = 180
)

. (Join-Path $PSScriptRoot 'UnityTooling.ps1')

if ([string]::IsNullOrWhiteSpace($ProjectPath)) {
    $ProjectPath = Join-Path $PSScriptRoot '..'
}

$project = Resolve-UnityProjectPath -ProjectPath $ProjectPath
$unity = Resolve-UnityEditorPath -ProjectPath $project -UnityPath $UnityPath
Invoke-UnityPreflight -ProjectPath $project -UnityPath $unity -ArtifactsRoot (Join-Path $project 'artifacts') -TimeoutSeconds $TimeoutSeconds
