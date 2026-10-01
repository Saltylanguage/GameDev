Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Resolve-UnityProjectPath {
    param([Parameter(Mandatory)][string]$ProjectPath)

    $resolved = (Resolve-Path -LiteralPath $ProjectPath).Path
    if (-not (Test-Path -LiteralPath (Join-Path $resolved 'Assets')) -or
        -not (Test-Path -LiteralPath (Join-Path $resolved 'ProjectSettings'))) {
        throw "'$resolved' is not a Unity project directory."
    }

    return $resolved
}

function Resolve-UnityCliPath {
    $command = Get-Command unity -CommandType Application -ErrorAction SilentlyContinue | Select-Object -First 1
    if ($null -eq $command) {
        throw 'Unity CLI was not found on PATH. Install it before running this workflow.'
    }

    return $command.Source
}

function Resolve-UnityEditorPath {
    param(
        [Parameter(Mandatory)][string]$ProjectPath,
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
    $versionLine = Select-String -LiteralPath $projectVersionPath -Pattern '^m_EditorVersion:\s*(.+)$' | Select-Object -First 1
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
        throw "Unity project lockfile exists at '$lockFile'. Close the project Editor or investigate the stale lock before running batch commands; this tooling will not remove it."
    }
}

function New-UnityArtifactDirectory {
    param(
        [Parameter(Mandatory)][string]$ArtifactsRoot,
        [Parameter(Mandatory)][string]$Prefix
    )

    $timestamp = Get-Date -Format 'yyyyMMdd-HHmmss'
    $directory = Join-Path $ArtifactsRoot "$Prefix-$timestamp"
    $suffix = 1
    while (Test-Path -LiteralPath $directory) {
        $directory = Join-Path $ArtifactsRoot "$Prefix-$timestamp-$suffix"
        $suffix++
    }

    New-Item -ItemType Directory -Path $directory -Force | Out-Null
    return $directory
}

function Get-ProcessIdsByName {
    param(
        [Parameter(Mandatory)]
        [string]$Name
    )

    return @(Get-Process -Name $Name -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Id)
}

function Stop-UnityProcessTree {
    param(
        [Parameter(Mandatory)]
        [int]$ProcessId
    )

    # taskkill must see the live parent to terminate its descendants. Stopping
    # the parent first would leave child processes running after a timeout.
    if ($null -eq (Get-Process -Id $ProcessId -ErrorAction SilentlyContinue)) { return }
    $output = & taskkill.exe /PID $ProcessId /T /F 2>&1
    if ($LASTEXITCODE -eq 0 -or $null -eq (Get-Process -Id $ProcessId -ErrorAction SilentlyContinue)) {
        return
    }

    try {
        Stop-Process -Id $ProcessId -Force -ErrorAction Stop
    }
    catch {
        throw "Could not terminate Unity PID ${ProcessId}: $($_.Exception.Message). taskkill: $($output -join ' ')"
    }

    throw "Could not verify cleanup of Unity descendants for PID ${ProcessId}. taskkill: $($output -join ' ')"
}

function Stop-ProcessIds {
    param(
        [Parameter(Mandatory)]
        [int[]]$ProcessIds
    )

    foreach ($processId in ($ProcessIds | Sort-Object -Unique)) {
        if ($processId -eq $PID) {
            continue
        }

        try {
            Stop-Process -Id $processId -Force -ErrorAction Stop
        }
        catch {
            $output = & taskkill.exe /PID $processId /T /F 2>&1
            if ($LASTEXITCODE -ne 0 -and ($output -join ' ') -notmatch 'not found|no running instance') {
                Write-Verbose "Could not terminate child process PID ${processId}: $($_.Exception.Message)"
            }
        }
    }
}

function Invoke-UnityBatch {
    param(
        [Parameter(Mandatory)][string[]]$Arguments,
        [string]$LogPath
    )

    $argumentLine = ($Arguments | ForEach-Object {
        if ($_ -match '[\s"]') {
            '"' + $_.Replace('"', '\"') + '"'
        }
        else {
            $_
        }
    }) -join ' '
    $trackedChildNames = @('UnityPackageManager', 'Unity.Licensing.Client')
    $trackedChildrenBefore = @{}
    foreach ($name in $trackedChildNames) {
        $trackedChildrenBefore[$name] = @(Get-ProcessIdsByName -Name $name)
    }

    $process = Start-Process -FilePath $UnityPath -ArgumentList $argumentLine -PassThru
    try {
        if (-not $process.WaitForExit($TimeoutSeconds * 1000)) {
            throw "Unity batch command timed out after $TimeoutSeconds seconds (PID $($process.Id))."
        }

        if ($process.ExitCode -ne 0) {
            throw "Unity batch command failed with exit code $($process.ExitCode). See the command's log file for details."
        }
    }
    finally {
        $treeCleanupError = $null
        try {
            Stop-UnityProcessTree -ProcessId $process.Id
        }
        catch {
            $treeCleanupError = $_.Exception.Message
        }

        # Unity can start or re-parent the Package Manager/licensing client as
        # the editor is shutting down. Keep a short, bounded cleanup window so
        # those late children cannot survive a failed batch invocation.
        $cleanupDeadline = (Get-Date).AddSeconds(10)
        do {
            $newChildIds = @()
            foreach ($name in $trackedChildNames) {
                $before = @($trackedChildrenBefore[$name])
                $newChildIds += @(Get-ProcessIdsByName -Name $name | Where-Object { $before -notcontains $_ })
            }

            if ($newChildIds.Count -gt 0) {
                Stop-ProcessIds -ProcessIds $newChildIds
            }

            if ((Get-Date) -ge $cleanupDeadline) {
                break
            }

            Start-Sleep -Milliseconds 250
        } while ($true)

        if ($null -ne $treeCleanupError) { throw $treeCleanupError }
    }
}

function Invoke-UnityPreflight {
    param(
        [Parameter(Mandatory)][string]$ProjectPath,
        [string]$UnityPath,
        [Parameter(Mandatory)][string]$ArtifactsRoot,
        [ValidateRange(30, 360)][int]$TimeoutSeconds = 180
    )

    Assert-UnityProjectNotOpen -ProjectPath $ProjectPath

    $artifactDirectory = New-UnityArtifactDirectory -ArtifactsRoot $ArtifactsRoot -Prefix 'unity-preflight'
    $contextLogPath = Join-Path $artifactDirectory 'licensing-context.log'
    $licensingClientPath = Join-Path (Split-Path -Parent $UnityPath) 'Data/Resources/Licensing/Client/Unity.Licensing.Client.exe'
    if (Test-Path -LiteralPath $licensingClientPath -PathType Leaf) {
        $previousErrorActionPreference = $ErrorActionPreference
        try {
            # The client reports restricted WMI access on stderr; capture it as
            # diagnostic output instead of allowing the shell's Stop policy to
            # mask the actionable preflight message below.
            $ErrorActionPreference = 'Continue'
            $contextOutput = @(& $licensingClientPath '--showContext' 2>&1)
        }
        finally {
            $ErrorActionPreference = $previousErrorActionPreference
        }
        $contextOutput | Set-Content -LiteralPath $contextLogPath
        if (($contextOutput -join "`n") -match '(?i)access denied') {
            throw "Unity licensing cannot read the host identity from this restricted process context. Run Unity validation from a normal host-permission terminal (or approve the elevated Unity preflight). Context log: '$contextLogPath'."
        }
    }

    $licenseDirectory = Join-Path $env:LOCALAPPDATA 'Unity/licenses'
    if (-not (Get-ChildItem -LiteralPath $licenseDirectory -Filter '*.xml' -File -ErrorAction SilentlyContinue)) {
        throw "No local Unity entitlement file was found under '$licenseDirectory'. Open Unity Hub, sign in, and activate the editor before running validation."
    }

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
        DoctorLog = $doctorLog
        LicenseLog = $licenseLog
        DoctorExitCode = $doctor.ExitCode
        LicenseExitCode = $license.ExitCode
        Healthy = $doctor.ExitCode -eq 0 -and $license.ExitCode -eq 0
        ProjectState = Get-UnityProjectState -ProjectPath $project
    }
}
