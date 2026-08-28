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

    # Some machines keep Unity editors on F: rather than under the Hub default.
    if (Test-Path -LiteralPath 'F:\Editor' -PathType Container) {
        $candidates += Join-Path 'F:\Editor' "$editorVersion-x86_64\Editor\Unity.exe"
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
    $unityProcesses = @(Get-Process -Name Unity -ErrorAction SilentlyContinue)
    if ($unityProcesses.Count -gt 0) {
        throw "Unity is already running (PID $($unityProcesses[0].Id)). Close the editor, save your work, and run this command again. This tooling never closes Unity for you."
    }

    if (Test-Path -LiteralPath $lockFile -PathType Leaf) {
        Remove-Item -LiteralPath $lockFile -Force
        Write-Verbose "Removed stale Unity lockfile '$lockFile'."
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
        ,
        [ValidateRange(30, 3600)]
        [int]$TimeoutSeconds = 900
    )

    $argumentLine = ($Arguments | ForEach-Object {
        if ($_ -match '[\s"]') {
            '"' + $_.Replace('"', '\"') + '"'
        }
        else {
            $_
        }
    }) -join ' '
    $process = Start-Process -FilePath $UnityPath -ArgumentList $argumentLine -PassThru
    if (-not $process.WaitForExit($TimeoutSeconds * 1000)) {
        Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
        throw "Unity batch command timed out after $TimeoutSeconds seconds (PID $($process.Id))."
    }

    if ($process.ExitCode -ne 0) {
        throw "Unity batch command failed with exit code $($process.ExitCode). See the command's log file for details."
    }
}

function Invoke-UnityPreflight {
    param(
        [Parameter(Mandatory)]
        [string]$ProjectPath,
        [Parameter(Mandatory)]
        [string]$UnityPath,
        [Parameter(Mandatory)]
        [string]$ArtifactsRoot,
        [ValidateRange(30, 360)]
        [int]$TimeoutSeconds = 180
    )

    Assert-UnityProjectNotOpen -ProjectPath $ProjectPath

    $licenseDirectory = Join-Path $env:LOCALAPPDATA 'Unity/licenses'
    if (-not (Get-ChildItem -LiteralPath $licenseDirectory -Filter '*.xml' -File -ErrorAction SilentlyContinue)) {
        throw "No local Unity entitlement file was found under '$licenseDirectory'. Open Unity Hub, sign in, and activate the editor before running validation."
    }

    $artifactDirectory = New-UnityArtifactDirectory -ArtifactsRoot $ArtifactsRoot -Prefix 'unity-preflight'
    $logPath = Join-Path $artifactDirectory 'license-probe.log'
    try {
        Invoke-UnityBatch -UnityPath $UnityPath -TimeoutSeconds $TimeoutSeconds -Arguments @(
            '-batchmode',
            '-nographics',
            '-quit',
            '-projectPath', $ProjectPath,
            '-logFile', $logPath
        )
    }
    catch {
        throw "Unity preflight failed: $($_.Exception.Message) Log: '$logPath'."
    }

    if (-not (Test-Path -LiteralPath $logPath -PathType Leaf)) {
        throw "Unity preflight completed without writing '$logPath'."
    }

    $lines = @(Get-Content -LiteralPath $logPath)
    $lastReady = -1
    $lastFailure = -1
    for ($index = 0; $index -lt $lines.Count; $index++) {
        if ($lines[$index] -match 'Licensing is initialized|Product:\s+Unity\s+|Successfully updated license') {
            $lastReady = $index
        }

        if ($lines[$index] -match 'LicenseClient-[^\"]+ refused|Licensing initialization failed') {
            $lastFailure = $index
        }
    }

    if ($lastReady -lt 0 -or $lastFailure -gt $lastReady) {
        throw "Unity license preflight did not reach a stable licensing handshake. Log: '$logPath'."
    }

    return [pscustomobject]@{
        ArtifactDirectory = $artifactDirectory
        Log = $logPath
        LicenseEntitlementFile = (Get-ChildItem -LiteralPath $licenseDirectory -Filter '*.xml' -File | Select-Object -First 1 -ExpandProperty FullName)
    }
}
