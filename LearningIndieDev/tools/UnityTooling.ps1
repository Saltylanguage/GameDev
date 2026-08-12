Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Resolve-UnityProjectPath {
    param(
        [Parameter(Mandatory)]
        [string]$ProjectPath
    )

    $resolved = (Resolve-Path -LiteralPath $ProjectPath).Path
    $hasAssets = Test-Path -LiteralPath (Join-Path $resolved 'Assets')
    $hasProjectSettings = Test-Path -LiteralPath (Join-Path $resolved 'ProjectSettings')
    if (-not $hasAssets -or -not $hasProjectSettings) {
        throw "'$resolved' is not a Unity project directory."
    }

    return $resolved
}

function Resolve-UnityEditorPath {
    param(
        [Parameter(Mandatory)]
        [string]$ProjectPath,
        [string]$UnityPath
    )

    if (-not [string]::IsNullOrWhiteSpace($UnityPath)) {
        $resolved = (Resolve-Path -LiteralPath $UnityPath).Path
        if (-not (Test-Path -LiteralPath $resolved -PathType Leaf)) {
            throw "Unity editor executable was not found at '$resolved'."
        }

        return $resolved
    }

    $projectVersionPath = Join-Path $ProjectPath 'ProjectSettings/ProjectVersion.txt'
    $versionLine = Select-String -LiteralPath $projectVersionPath -Pattern '^m_EditorVersion:\s*(.+)$' |
        Select-Object -First 1
    if ($null -eq $versionLine) {
        throw "Could not determine the Unity version from '$projectVersionPath'."
    }

    $editorVersion = $versionLine.Matches[0].Groups[1].Value.Trim()
    $candidates = @()
    if (-not [string]::IsNullOrWhiteSpace($env:ProgramFiles)) {
        $candidates += Join-Path $env:ProgramFiles "Unity\Hub\Editor\$editorVersion\Editor\Unity.exe"
    }

    if (-not [string]::IsNullOrWhiteSpace(${env:ProgramFiles(x86)})) {
        $candidates += Join-Path ${env:ProgramFiles(x86)} "Unity\Hub\Editor\$editorVersion\Editor\Unity.exe"
    }

    foreach ($candidate in $candidates) {
        if (Test-Path -LiteralPath $candidate -PathType Leaf) {
            return $candidate
        }
    }

    throw "Could not find Unity $editorVersion. Supply -UnityPath explicitly."
}

function Assert-UnityProjectNotOpen {
    param(
        [Parameter(Mandatory)]
        [string]$ProjectPath
    )

    $lockFile = Join-Path $ProjectPath 'Temp/UnityLockfile'
    if (Test-Path -LiteralPath $lockFile -PathType Leaf) {
        throw "Unity appears to be open for '$ProjectPath' (found Temp/UnityLockfile). Close the editor, save your work, and run this command again. This tooling never closes Unity for you."
    }
}

function New-UnityArtifactDirectory {
    param(
        [Parameter(Mandatory)]
        [string]$ArtifactsRoot,
        [Parameter(Mandatory)]
        [string]$Prefix
    )

    $timestamp = Get-Date -Format 'yyyyMMdd-HHmmss'
    $directory = Join-Path $ArtifactsRoot "$Prefix-$timestamp"
    New-Item -ItemType Directory -Path $directory -Force | Out-Null
    return $directory
}

function Invoke-UnityBatch {
    param(
        [Parameter(Mandatory)]
        [string]$UnityPath,
        [Parameter(Mandatory)]
        [string[]]$Arguments
    )

    $argumentLine = ($Arguments | ForEach-Object {
        if ($_ -match '[\s"]') {
            '"' + $_.Replace('"', '\"') + '"'
        }
        else {
            $_
        }
    }) -join ' '
    $process = Start-Process -FilePath $UnityPath -ArgumentList $argumentLine -Wait -PassThru
    if ($process.ExitCode -ne 0) {
        throw "Unity batch command failed with exit code $($process.ExitCode). See the command's log file for details."
    }
}
