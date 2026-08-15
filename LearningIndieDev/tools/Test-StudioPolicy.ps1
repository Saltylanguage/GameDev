[CmdletBinding()]
param(
    [switch]$SkipHookSmokeTest
)

$ErrorActionPreference = 'Stop'

function Fail([string]$Message) {
    throw "Studio policy validation failed: $Message"
}

$repoRoot = Resolve-Path (Join-Path $PSScriptRoot '..\..')
$policyPath = Join-Path $repoRoot '.codex\studio-policy.json'
$hooksPath = Join-Path $repoRoot '.codex\hooks.json'
$hookPath = Join-Path $repoRoot '.codex\hooks\studio_policy.ps1'
$guidelinesPath = Join-Path $PSScriptRoot '..\docs\Studio Guidelines'

foreach ($path in @($policyPath, $hooksPath, $hookPath, $guidelinesPath)) {
    if (-not (Test-Path $path)) {
        Fail "Required path is missing: $path"
    }
}

try {
    $policy = Get-Content -Raw $policyPath | ConvertFrom-Json
    $hooks = Get-Content -Raw $hooksPath | ConvertFrom-Json
}
catch {
    Fail "Policy or hook configuration is not valid JSON. $($_.Exception.Message)"
}

if ($policy.mode -ne 'alert-only') {
    Fail "Policy mode must remain alert-only; found '$($policy.mode)'."
}

$guidelineIds = @(
    Get-ChildItem -File -Filter '*.md' $guidelinesPath |
        ForEach-Object {
            [regex]::Matches((Get-Content -Raw $_.FullName), '(?m)^\*\*Guideline ID:\*\*\s*(SG-\d+)\b') |
                ForEach-Object { $_.Groups[1].Value }
        }
)

$rules = @($policy.rules)
if ($rules.Count -eq 0) {
    Fail 'At least one policy rule is required.'
}

$ruleIds = @()
$allowedSeverities = @('info', 'low', 'medium', 'high')
foreach ($rule in $rules) {
    foreach ($field in @('id', 'guideline', 'severity', 'tools', 'patterns', 'message')) {
        if ($null -eq $rule.$field -or [string]::IsNullOrWhiteSpace([string]$rule.$field)) {
            Fail "Rule is missing required field '$field'."
        }
    }

    if ($ruleIds -contains $rule.id) {
        Fail "Duplicate rule id '$($rule.id)'."
    }
    $ruleIds += $rule.id

    if ($allowedSeverities -notcontains [string]$rule.severity) {
        Fail "Rule '$($rule.id)' has unsupported severity '$($rule.severity)'."
    }
    if ($guidelineIds -notcontains [string]$rule.guideline) {
        Fail "Rule '$($rule.id)' references missing guideline '$($rule.guideline)'."
    }
    if (@($rule.tools).Count -eq 0 -or @($rule.patterns).Count -eq 0) {
        Fail "Rule '$($rule.id)' must define at least one tool and pattern."
    }

    foreach ($pattern in @($rule.patterns)) {
        try {
            [regex]::new([string]$pattern) | Out-Null
        }
        catch {
            Fail "Rule '$($rule.id)' contains invalid regex '$pattern'."
        }
    }
}

$hookText = Get-Content -Raw $hookPath
foreach ($forbiddenPattern in @(
    'permissionDecision\s*=',
    'updatedInput\s*=',
    'decision\s*=\s*[''\"](?:deny|block|allow)',
    'continue\s*=\s*\$false'
)) {
    if ($hookText -match $forbiddenPattern) {
        Fail "Alert-only hook contains a blocking or rewriting decision: $forbiddenPattern"
    }
}

$preToolHooks = @($hooks.hooks.PreToolUse)
if ($preToolHooks.Count -eq 0) {
    Fail 'No PreToolUse hook is registered.'
}
if ((Get-Content -Raw $hooksPath) -match 'permissionDecision|updatedInput|decision') {
    Fail 'Hook configuration contains a decision field while policy mode is alert-only.'
}

if (-not $SkipHookSmokeTest) {
    $dangerousInput = '{"hook_event_name":"PreToolUse","tool_name":"Bash","tool_input":{"command":"git reset --hard HEAD"}}'
    $safeInput = '{"hook_event_name":"PreToolUse","tool_name":"Bash","tool_input":{"command":"git status --short"}}'
    $dangerousOutput = $dangerousInput | & powershell -NoProfile -ExecutionPolicy Bypass -File $hookPath
    $safeOutput = $safeInput | & powershell -NoProfile -ExecutionPolicy Bypass -File $hookPath

    if ([string]::IsNullOrWhiteSpace(($dangerousOutput -join ''))) {
        Fail 'Dangerous hook smoke test produced no alert.'
    }
    if (($dangerousOutput -join '') -notmatch 'alert-only prototype') {
        Fail 'Dangerous hook smoke test did not identify the alert-only mode.'
    }
    if (-not [string]::IsNullOrWhiteSpace(($safeOutput -join ''))) {
        Fail 'Safe hook smoke test produced an unexpected alert.'
    }
}

Write-Output ("Studio policy valid: {0} guideline(s), {1} rule(s), mode={2}." -f $guidelineIds.Count, $rules.Count, $policy.mode)
