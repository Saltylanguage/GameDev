[CmdletBinding()]
param(
    [ValidateSet('EditMode', 'PlayMode', 'All')]
    [string]$Mode = 'All',
    [string]$ProjectPath,
    [string]$UnityPath
)

. (Join-Path $PSScriptRoot 'UnityTooling.ps1')

if ([string]::IsNullOrWhiteSpace($ProjectPath)) {
    $ProjectPath = Join-Path $PSScriptRoot '..'
}

$project = Resolve-UnityProjectPath -ProjectPath $ProjectPath
Assert-UnityProjectNotOpen -ProjectPath $project
$unity = Resolve-UnityEditorPath -ProjectPath $project -UnityPath $UnityPath
$artifactDirectory = New-UnityArtifactDirectory -ArtifactsRoot (Join-Path $project 'artifacts') -Prefix 'unity-tests'
$platforms = if ($Mode -eq 'All') { @('EditMode', 'PlayMode') } else { @($Mode) }
$results = foreach ($platform in $platforms) {
    $resultPath = Join-Path $artifactDirectory "$platform-results.xml"
    $logPath = Join-Path $artifactDirectory "$platform.log"
    Invoke-UnityBatch -UnityPath $unity -Arguments @(
        '-batchmode',
        '-nographics',
        '-projectPath', $project,
        '-runTests',
        '-testPlatform', $platform,
        '-testResults', $resultPath,
        '-logFile', $logPath
    )

    if (-not (Test-Path -LiteralPath $resultPath -PathType Leaf)) {
        throw "Unity completed without writing expected test results to '$resultPath'."
    }

    $resultPath
}

[pscustomobject]@{
    ArtifactDirectory = $artifactDirectory
    Results = $results
}
