[CmdletBinding()]
param(
    [string]$RepoPath = (Get-Location).Path,
    [string]$OutputPath = '',
    [ValidateRange(1, 500)]
    [int]$MaxChangedFiles = 120
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Get-RelativePath {
    param([string]$Root, [string]$Path)

    $rootUri = [System.Uri]((Resolve-Path -LiteralPath $Root).Path.TrimEnd('\') + '\')
    $pathUri = [System.Uri](Resolve-Path -LiteralPath $Path).Path
    return [System.Uri]::UnescapeDataString($rootUri.MakeRelativeUri($pathUri).ToString()).Replace('/', '\')
}

function Invoke-GitText {
    param([string]$Root, [string[]]$Arguments)

    # Git can emit benign repository warnings (for example line-ending
    # normalization) on stderr. PowerShell treats native stderr as an error
    # record under `Stop`, so quiet it only for this read-only probe and keep
    # the exit code as the actual success signal.
    $previousErrorAction = $ErrorActionPreference
    $ErrorActionPreference = 'SilentlyContinue'
    $value = & git -C $Root @Arguments 2>$null
    $exitCode = $LASTEXITCODE
    $ErrorActionPreference = $previousErrorAction
    if ($exitCode -ne 0) {
        return @()
    }

    return @($value | ForEach-Object { [string]$_ })
}

function Add-Finding {
    param(
        [System.Collections.Generic.List[object]]$List,
        [ValidateSet('Attention', 'Finger wag', 'Clean')]
        [string]$Severity,
        [string]$Topic,
        [string]$Message,
        [string]$Evidence
    )

    $List.Add([pscustomobject]@{
        Severity = $Severity
        Topic = $Topic
        Message = $Message
        Evidence = $Evidence
    })
}

$repo = (Resolve-Path -LiteralPath $RepoPath).Path
$gitRootLine = Invoke-GitText -Root $repo -Arguments @('rev-parse', '--show-toplevel') | Select-Object -First 1
if ([string]::IsNullOrWhiteSpace($gitRootLine)) {
    throw "'$repo' is not inside a Git repository."
}

$gitRoot = (Resolve-Path -LiteralPath $gitRootLine.Trim()).Path
$branch = (Invoke-GitText -Root $gitRoot -Arguments @('branch', '--show-current') | Select-Object -First 1).Trim()
$commit = (Invoke-GitText -Root $gitRoot -Arguments @('rev-parse', '--short', 'HEAD') | Select-Object -First 1).Trim()
$statusLines = @(Invoke-GitText -Root $gitRoot -Arguments @('status', '--short', '--untracked-files=all'))
$diffCheckLines = @(Invoke-GitText -Root $gitRoot -Arguments @('diff', '--check'))
$findings = [System.Collections.Generic.List[object]]::new()
$changed = [System.Collections.Generic.List[object]]::new()

foreach ($line in $statusLines) {
    if ([string]::IsNullOrWhiteSpace($line)) { continue }

    $code = if ($line.Length -ge 2) { $line.Substring(0, 2) } else { $line }
    $pathText = if ($line.Length -gt 3) { $line.Substring(3) } else { '' }
    if ($pathText -match ' -> ') { $pathText = $pathText.Split(' -> ')[-1] }
    $changed.Add([pscustomobject]@{
        Code = $code
        Path = $pathText
        Staged = $code[0] -ne ' ' -and $code -ne '??'
        Unstaged = $code[1] -ne ' ' -and $code -ne '??'
        Untracked = $code -eq '??'
    })
}

$changedPaths = @($changed | Select-Object -First $MaxChangedFiles | ForEach-Object { $_.Path })
$codeChanges = @($changed | Where-Object { $_.Path -match '(^|[\\/])(Assets|Packages|ProjectSettings|tools)[\\/]' })
$testChanges = @($changed | Where-Object { $_.Path -match '(Tests|test|spec|\.tests\.)' })
$docChanges = @($changed | Where-Object { $_.Path -match '(^|[\\/])docs?[\\/]|\.md$' })

if ([string]::IsNullOrWhiteSpace($branch)) {
    Add-Finding $findings 'Attention' 'Branch' 'The checkout is detached or has no readable branch name.' 'git branch --show-current returned an empty value.'
}
if ($changed.Count -gt 0) {
    Add-Finding $findings 'Finger wag' 'Dirty tree' 'There are local changes that still need an owner and a final disposition.' "$($changed.Count) changed path(s) are present; DirtyBoy will not stage or clean them for you."
}
if ($diffCheckLines.Count -gt 0) {
    Add-Finding $findings 'Attention' 'Diff hygiene' 'Git found whitespace errors in the diff.' (($diffCheckLines -join ' | '))
}
if ($codeChanges.Count -gt 0 -and $testChanges.Count -eq 0) {
    Add-Finding $findings 'Finger wag' 'Verification' 'Code-like files changed without a matching test change.' "$($codeChanges.Count) code/project path(s) changed; no changed test path was detected. Run the relevant existing tests or explain why this is documentation/configuration only."
}
if ($codeChanges.Count -gt 0 -and $docChanges.Count -eq 0) {
    Add-Finding $findings 'Finger wag' 'Handoff' 'Code-like files changed without a documentation or handoff update.' 'If the behavior or workflow changed, record the new truth, verification command, or remaining uncertainty.'
}

$hasRg = $null -ne (Get-Command rg -ErrorAction SilentlyContinue)
$conflictFiles = @()
if ($hasRg) {
    $conflictFiles = @(rg --files-with-matches --hidden --glob '!.git/**' --glob '!Library/**' --glob '!Temp/**' '^(<<<<<<<|>>>>>>>)' $gitRoot 2>$null)
}
if ($conflictFiles.Count -gt 0) {
    Add-Finding $findings 'Attention' 'Conflict markers' 'Conflict-marker text remains in project files.' "$($conflictFiles.Count) file(s): $($conflictFiles -join ', ')"
}

$markerFiles = @()
if ($hasRg -and $changedPaths.Count -gt 0) {
    $existingChanged = @($changedPaths | Where-Object {
        (($_ -notmatch '(^|[\\/])\.agents[\\/]skills[\\/]dirtyboy[\\/]') -and
            ($_ -match '\.(cs|ps1|py|js|ts|json|yaml|yml)$'))
    } | ForEach-Object {
        $candidate = Join-Path $gitRoot $_
        if (Test-Path -LiteralPath $candidate -PathType Leaf) { $candidate }
    })
    if ($existingChanged.Count -gt 0) {
        $markerFiles = @(rg --files-with-matches '(TODO|FIXME|HACK|TEMP)' @existingChanged 2>$null)
    }
}
if ($markerFiles.Count -gt 0) {
    Add-Finding $findings 'Finger wag' 'Deferred work' 'A changed file contains an explicit TODO/FIXME/HACK/TEMP marker.' "$($markerFiles -join ', '); either track the deferral or finish the shortcut before calling the block done."
}

$largeFiles = @($changedPaths | ForEach-Object {
    $candidate = Join-Path $gitRoot $_
    if (Test-Path -LiteralPath $candidate -PathType Leaf) {
        $item = Get-Item -LiteralPath $candidate
        if ($item.Length -gt 5MB) { "$($_) ($([math]::Round($item.Length / 1MB, 1)) MB)" }
    }
})
if ($largeFiles.Count -gt 0) {
    Add-Finding $findings 'Finger wag' 'Generated-looking change' 'A changed file is larger than 5 MB; confirm that a generated artifact was not accidentally made part of the feature diff.' ($largeFiles -join ', ')
}

$unityProjects = [System.Collections.Generic.List[object]]::new()
$candidateRoots = @($gitRoot)
$candidateRoots += @(Get-ChildItem -LiteralPath $gitRoot -Directory -Force | ForEach-Object { $_.FullName })
foreach ($candidate in $candidateRoots | Select-Object -Unique) {
    if ((Test-Path -LiteralPath (Join-Path $candidate 'Assets')) -and
        (Test-Path -LiteralPath (Join-Path $candidate 'ProjectSettings')) -and
        (Test-Path -LiteralPath (Join-Path $candidate 'Packages'))) {
        $unityProjects.Add([pscustomobject]@{
            Root = (Get-RelativePath -Root $gitRoot -Path $candidate)
            Assets = (Get-ChildItem -LiteralPath (Join-Path $candidate 'Assets') -Force -ErrorAction SilentlyContinue | Measure-Object).Count
            HasUnityTests = Test-Path -LiteralPath (Join-Path $candidate 'tools/Invoke-UnityTests.ps1')
            HasCellSim = Test-Path -LiteralPath (Join-Path $candidate 'tools/Run-CellularExperiment.ps1')
        })
    }
}

$relevantDocs = @(
    'AGENTS.md',
    'LearningIndieDev/AGENTS.md',
    'LearningIndieDev/docs/Studio Guidelines/README.md',
    'LearningIndieDev/docs/PROJECT_CONTEXT.md',
    'LearningIndieDev/docs/WORKING_STATE.md',
    'LearningIndieDev/docs/LOOSE_ENDS.md',
    'LearningIndieDev/docs/NEXT_WORK_BUCKET_PLAN.md',
    'LearningIndieDev/docs/Planning Concerns'
)
$docRows = foreach ($doc in $relevantDocs) {
    $path = Join-Path $gitRoot $doc
    [pscustomobject]@{ Path = $doc; Present = Test-Path -LiteralPath $path }
}
$missingDocs = @($docRows | Where-Object { -not $_.Present })
if ($missingDocs.Count -gt 0) {
    Add-Finding $findings 'Finger wag' 'Project guidance' 'Some expected project guidance was not found.' (($missingDocs.Path) -join ', ')
}

$testCommands = [System.Collections.Generic.List[string]]::new()
foreach ($project in $unityProjects) {
    $prefix = if ($project.Root -eq '.') { '' } else { "$($project.Root)/" }
    if ($project.HasUnityTests) {
        $testCommands.Add("powershell -File ${prefix}tools/Invoke-UnityTests.ps1 -Mode EditMode")
        $testCommands.Add("powershell -File ${prefix}tools/Invoke-UnityTests.ps1 -Mode PlayMode")
    }
    if ($project.HasCellSim) {
        $testCommands.Add("powershell -File ${prefix}tools/Run-CellularExperiment.ps1 -ScenarioPath Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset")
    }
}
if ($testCommands.Count -eq 0) {
    Add-Finding $findings 'Finger wag' 'Verification commands' 'No known Unity test or CellSim command was discovered.' 'Check the project tools folder before starting implementation.'
}

$artifactRows = foreach ($project in $unityProjects) {
    $projectRoot = if ($project.Root -eq '.') { $gitRoot } else { Join-Path $gitRoot $project.Root }
    $artifactRoot = Join-Path $projectRoot 'artifacts'
    if (Test-Path -LiteralPath $artifactRoot -PathType Container) {
        Get-ChildItem -LiteralPath $artifactRoot -Directory -ErrorAction SilentlyContinue |
            Where-Object { $_.Name -match '^(unity-tests|cellular-experiment)-' } |
            Sort-Object LastWriteTime -Descending |
            Select-Object -First 8 |
            ForEach-Object { [pscustomobject]@{ Project = $project.Root; Name = $_.Name; LastWriteTime = $_.LastWriteTime.ToString('s') } }
    }
}
if ($artifactRows.Count -eq 0 -and $codeChanges.Count -gt 0) {
    Add-Finding $findings 'Finger wag' 'Test evidence' 'No recent Unity test or experiment artifact was found.' 'Run the smallest relevant verification command before handoff, or record why the change is not executable yet.'
}

$topLevel = @(Get-ChildItem -LiteralPath $gitRoot -Directory -Force -ErrorAction SilentlyContinue | Sort-Object Name | ForEach-Object {
    [pscustomobject]@{ Name = $_.Name; Kind = if (Test-Path -LiteralPath (Join-Path $_.FullName 'Assets')) { 'Unity project' } else { 'directory' } }
})

$lines = [System.Collections.Generic.List[string]]::new()
$lines.Add('# DirtyBoy context snapshot')
$lines.Add('')
$lines.Add("Generated: $([DateTime]::UtcNow.ToString('O'))")
$branchDisplay = if ([string]::IsNullOrWhiteSpace($branch)) { '(detached or unknown)' } else { $branch }
$lines.Add(('Repository: `{0}`' -f $gitRoot))
$lines.Add(('Branch: `{0}`' -f $branchDisplay))
$lines.Add(('Commit: `{0}`' -f $commit))
$lines.Add('')
$lines.Add('DirtyBoy is read-only. It gently flags evidence-backed cleanup or verification gaps; it does not stage, commit, reset, delete, or push.')
$lines.Add('')
$lines.Add('## Findings')
$lines.Add('')
if ($findings.Count -eq 0) {
    $lines.Add('Clean snapshot: no evidence-backed process warning was found.')
} else {
    $lines.Add('| Level | Topic | Finding | Evidence |')
    $lines.Add('| --- | --- | --- | --- |')
    foreach ($finding in $findings) {
        $lines.Add("| $($finding.Severity) | $($finding.Topic) | $($finding.Message.Replace('|', '\|')) | $($finding.Evidence.Replace('|', '\|')) |")
    }
}
$lines.Add('')
$lines.Add('## Changed files')
$lines.Add('')
if ($changed.Count -eq 0) {
    $lines.Add('Working tree is clean.')
} else {
    $lines.Add('| Status | Path |')
    $lines.Add('| --- | --- |')
    foreach ($file in $changed | Select-Object -First $MaxChangedFiles) {
        $lines.Add(('| `{0}` | `{1}` |' -f $file.Code, $file.Path.Replace('|', '\|')))
    }
    if ($changed.Count -gt $MaxChangedFiles) { $lines.Add("| … | $($changed.Count - $MaxChangedFiles) more path(s) omitted; raise `-MaxChangedFiles` to inspect them. |") }
}
$lines.Add('')
$lines.Add('## Project map')
$lines.Add('')
$lines.Add('| Path | Kind |')
$lines.Add('| --- | --- |')
foreach ($entry in $topLevel) { $lines.Add(('| `{0}` | {1} |' -f $entry.Name, $entry.Kind)) }
if ($unityProjects.Count -eq 0) { $lines.Add('| — | No Unity project root detected. |') }
$lines.Add('')
$lines.Add('## Relevant guidance')
$lines.Add('')
$lines.Add('| Path | Present |')
$lines.Add('| --- | --- |')
foreach ($doc in $docRows) { $lines.Add(('| `{0}` | {1} |' -f $doc.Path, $doc.Present)) }
$lines.Add('')
$lines.Add('## Test and experiment commands')
$lines.Add('')
foreach ($command in $testCommands) { $lines.Add(('- `{0}`' -f $command)) }
if ($testCommands.Count -eq 0) { $lines.Add('- No known command discovered.') }
$lines.Add('')
$lines.Add('## Recent evidence artifacts')
$lines.Add('')
if ($artifactRows.Count -eq 0) { $lines.Add('No Unity test or experiment artifact found.') }
else {
    $lines.Add('| Project | Artifact | Last write |')
    $lines.Add('| --- | --- | --- |')
    foreach ($artifact in $artifactRows) { $lines.Add(('| `{0}` | `{1}` | {2} |' -f $artifact.Project, $artifact.Name, $artifact.LastWriteTime)) }
}
$lines.Add('')
$lines.Add('## Next gentle check')
$lines.Add('')
if ($findings.Count -gt 0) {
    $lines.Add('Before calling the block complete, resolve or explicitly record each Finger wag and Attention finding. Keep the smallest useful test and handoff update.')
} else {
    $lines.Add('Keep the snapshot beside the next handoff if the work spans another session.')
}

$markdown = $lines -join "`n"
if (-not [string]::IsNullOrWhiteSpace($OutputPath)) {
    $output = Join-Path $gitRoot $OutputPath
    $parent = Split-Path -Parent $output
    if (-not [string]::IsNullOrWhiteSpace($parent)) { New-Item -ItemType Directory -Path $parent -Force | Out-Null }
    Set-Content -LiteralPath $output -Value $markdown -Encoding utf8
    Write-Output $output
} else {
    Write-Output $markdown
}
