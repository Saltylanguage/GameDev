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
    $unityProcesses = @(Get-Process -Name Unity -ErrorAction SilentlyContinue | Where-Object {
        $processPath = $_.Path
        [string]::IsNullOrWhiteSpace($processPath) -or
        $processPath -match '[\\/]Editor[\\/]Unity\.exe$'
    })
    if ($unityProcesses.Count -gt 0) {
        throw "Unity Editor is already running (PID $($unityProcesses[0].Id)). Close the editor, save your work, and run this command again. This tooling never closes Unity for you."
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

function Invoke-UnityCli {
    param(
        [Parameter(Mandatory)][string[]]$Arguments,
        [string]$LogPath
    )

    $cli = Resolve-UnityCliPath
    $previousErrorActionPreference = $ErrorActionPreference
    try {
        $ErrorActionPreference = 'Continue'
        $output = @(& $cli @Arguments 2>&1 | ForEach-Object { [string]$_ })
        $exitCode = $LASTEXITCODE
    }
    finally {
        $ErrorActionPreference = $previousErrorActionPreference
    }

    if (-not [string]::IsNullOrWhiteSpace($LogPath)) {
        $parent = Split-Path -Parent $LogPath
        if (-not [string]::IsNullOrWhiteSpace($parent)) {
            New-Item -ItemType Directory -Path $parent -Force | Out-Null
        }
        $output | Set-Content -LiteralPath $LogPath -Encoding utf8
    }

    return [pscustomobject]@{
        ExitCode = $exitCode
        Output = $output
        Text = ($output -join "`n")
        Arguments = $Arguments
    }
}

function ConvertFrom-UnityCliJson {
    param(
        [Parameter(Mandatory)]$Invocation,
        [string]$Operation = 'Unity CLI command'
    )

    try {
        return $Invocation.Text | ConvertFrom-Json
    }
    catch {
        throw "$Operation did not return valid JSON (exit $($Invocation.ExitCode)). Output: $($Invocation.Text)"
    }
}

function Get-UnityCommandPayload {
    param([Parameter(Mandatory)]$Envelope)

    $payload = if ($Envelope.PSObject.Properties.Name -contains 'data') { $Envelope.data } else { $Envelope }
    if ($null -ne $payload -and $payload.PSObject.Properties.Name -contains 'result') {
        $payload = $payload.result
    }
    if ($payload -is [string] -and $payload.TrimStart().StartsWith('{')) {
        try { return $payload | ConvertFrom-Json } catch { return $payload }
    }
    return $payload
}

function Invoke-UnityLiveCommand {
    param(
        [Parameter(Mandatory)][string]$ProjectPath,
        [Parameter(Mandatory)][string]$Command,
        [string[]]$CommandArguments = @(),
        [ValidateRange(5, 3600)][int]$TimeoutSeconds = 300,
        [string]$LogPath
    )

    $arguments = @(
        'command',
        '--caller', 'plugin',
        '--skill', 'unity-cli',
        '--project-path', $ProjectPath,
        '--timeout', [string]$TimeoutSeconds,
        '--no-banner', '--non-interactive', '--format', 'json',
        $Command
    ) + $CommandArguments
    $invocation = Invoke-UnityCli -Arguments $arguments -LogPath $LogPath
    $envelope = ConvertFrom-UnityCliJson -Invocation $invocation -Operation "Unity Pipeline command '$Command'"
    $reportedSuccess = -not ($envelope.PSObject.Properties.Name -contains 'success') -or [bool]$envelope.success
    if ($invocation.ExitCode -ne 0 -or -not $reportedSuccess) {
        throw "Unity Pipeline command '$Command' failed (exit $($invocation.ExitCode)). Output: $($invocation.Text)"
    }

    return [pscustomobject]@{
        Invocation = $invocation
        Envelope = $envelope
        Payload = Get-UnityCommandPayload -Envelope $envelope
    }
}

function Get-UnityProjectState {
    param([Parameter(Mandatory)][string]$ProjectPath)

    $project = Resolve-UnityProjectPath -ProjectPath $ProjectPath
    $lockFile = Join-Path $project 'Temp/UnityLockfile'
    $hasLock = Test-Path -LiteralPath $lockFile -PathType Leaf
    $statusInvocation = Invoke-UnityCli -Arguments @(
        'command', '--caller', 'plugin', '--skill', 'unity-cli',
        '--project-path', $project, '--timeout', '5',
        '--no-banner', '--non-interactive', '--format', 'json',
        'editor_status'
    )

    if ($statusInvocation.ExitCode -eq 0) {
        $statusEnvelope = ConvertFrom-UnityCliJson -Invocation $statusInvocation -Operation 'Unity Editor status probe'
        $payload = Get-UnityCommandPayload -Envelope $statusEnvelope
        $editorStatus = if ($null -ne $payload -and $payload.PSObject.Properties.Name -contains 'status') { [string]$payload.status } else { 'ready' }
        $state = if ($editorStatus -eq 'ready') { 'Ready' } else { 'Busy' }
        return [pscustomobject]@{
            State = $state
            ProjectPath = $project
            PipelineReachable = $true
            EditorStatus = $editorStatus
            HasLockFile = $hasLock
            Detail = "Pipeline is reachable; Editor status is '$editorStatus'."
        }
    }

    $listInvocation = Invoke-UnityCli -Arguments @('pipeline', 'list', '--no-banner', '--non-interactive', '--format', 'json')
    $matching = $null
    if ($listInvocation.ExitCode -eq 0) {
        try {
            $listEnvelope = ConvertFrom-UnityCliJson -Invocation $listInvocation -Operation 'Unity Pipeline discovery'
            $matching = @($listEnvelope.data.instances | Where-Object {
                -not [string]::IsNullOrWhiteSpace([string]$_.projectPath) -and
                [System.IO.Path]::GetFullPath([string]$_.projectPath).TrimEnd('\') -eq $project.TrimEnd('\')
            }) | Select-Object -First 1
        }
        catch { $matching = $null }
    }

    if ($null -ne $matching -and $null -ne $matching.safeMode -and [bool]$matching.safeMode.detected) {
        return [pscustomobject]@{
            State = 'SafeMode'; ProjectPath = $project; PipelineReachable = $false
            EditorStatus = 'safe-mode'; HasLockFile = $hasLock
            Detail = 'The project Editor is in safe mode. Fix compile errors and restart the Editor before automation.'
        }
    }
    if ($hasLock) {
        return [pscustomobject]@{
            State = 'Unreachable'; ProjectPath = $project; PipelineReachable = $false
            EditorStatus = 'unknown'; HasLockFile = $true
            Detail = 'The project lock file exists, but Pipeline is unreachable. The Editor may be starting, compiling, or hidden by the current process context.'
        }
    }

    $detail = if ($null -ne $matching -and [bool]$matching.isRunning) {
        'No project lock or reachable Editor was found; a stale Pipeline discovery record was ignored.'
    } else {
        'No project lock or reachable Pipeline Editor was found.'
    }
    return [pscustomobject]@{
        State = 'Closed'; ProjectPath = $project; PipelineReachable = $false
        EditorStatus = 'closed'; HasLockFile = $false; Detail = $detail
    }
}

function Resolve-UnityExecutionLane {
    param(
        [Parameter(Mandatory)][string]$ProjectPath,
        [ValidateSet('Auto', 'Live', 'Clean')][string]$Execution = 'Auto'
    )

    $state = Get-UnityProjectState -ProjectPath $ProjectPath
    if ($Execution -eq 'Live') {
        if ($state.State -ne 'Ready') {
            throw "Live execution requires a ready Pipeline Editor. Current state: $($state.State). $($state.Detail)"
        }
        return [pscustomobject]@{ Lane = 'Live'; State = $state }
    }
    if ($Execution -eq 'Clean') {
        if ($state.State -ne 'Closed') {
            throw "Clean execution requires this project to be closed. Current state: $($state.State). $($state.Detail)"
        }
        return [pscustomobject]@{ Lane = 'Clean'; State = $state }
    }
    if ($state.State -eq 'Ready') { return [pscustomobject]@{ Lane = 'Live'; State = $state } }
    if ($state.State -eq 'Closed') { return [pscustomobject]@{ Lane = 'Clean'; State = $state } }
    throw "Automatic execution could not choose a safe lane. Current state: $($state.State). $($state.Detail)"
}

function Get-UnityNUnitSummary {
    param([Parameter(Mandatory)][string]$ResultPath)

    [xml]$xml = Get-Content -LiteralPath $ResultPath -Raw
    $root = $xml.'test-run'
    return [pscustomobject]@{
        Total = [int]$root.total
        Passed = [int]$root.passed
        Failed = [int]$root.failed
        Skipped = [int]$root.skipped
        Inconclusive = [int]$root.inconclusive
        DurationSeconds = [double]$root.duration
    }
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
        [Parameter(Mandatory)][string]$UnityPath,
        [Parameter(Mandatory)][string[]]$Arguments,
        [ValidateRange(30, 3600)][int]$TimeoutSeconds = 900
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

function Invoke-UnityBatchPreflight {
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
        Log = $logPath
        LicenseEntitlementFile = (Get-ChildItem -LiteralPath $licenseDirectory -Filter '*.xml' -File | Select-Object -First 1 -ExpandProperty FullName)
    }
}

function Invoke-UnityPreflight {
    param(
        [Parameter(Mandatory)][string]$ProjectPath,
        [string]$UnityPath,
        [Parameter(Mandatory)][string]$ArtifactsRoot,
        [ValidateRange(30, 360)][int]$TimeoutSeconds = 180
    )

    $project = Resolve-UnityProjectPath -ProjectPath $ProjectPath
    $artifactDirectory = New-UnityArtifactDirectory -ArtifactsRoot $ArtifactsRoot -Prefix 'unity-doctor'
    $doctorLog = Join-Path $artifactDirectory 'doctor.json'
    $licenseLog = Join-Path $artifactDirectory 'license.json'
    $doctor = Invoke-UnityCli -Arguments @('doctor', '--ci', '--no-banner', '--non-interactive', '--format', 'json') -LogPath $doctorLog
    $license = Invoke-UnityCli -Arguments @('license', 'status', '--no-banner', '--non-interactive', '--format', 'json') -LogPath $licenseLog

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
