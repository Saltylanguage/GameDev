$ErrorActionPreference = 'SilentlyContinue'

# Alert-only Codex hook for the studio workflow policy.
# This prototype intentionally never returns a deny, block, or rewrite decision.

try {
    $payload = [Console]::In.ReadToEnd() | ConvertFrom-Json
    $policyPath = Join-Path $PSScriptRoot '..\studio-policy.json'
    $policy = Get-Content -Raw $policyPath | ConvertFrom-Json
    $toolName = [string]$payload.tool_name
    $toolInput = $payload.tool_input | ConvertTo-Json -Depth 50 -Compress
    $matchedRules = @()

    foreach ($rule in @($policy.rules)) {
        $toolIsCovered = $false
        foreach ($toolPattern in @($rule.tools)) {
            if ($toolName -match [string]$toolPattern) {
                $toolIsCovered = $true
                break
            }
        }

        if (-not $toolIsCovered) {
            continue
        }

        foreach ($pattern in @($rule.patterns)) {
            if ($toolInput -match [string]$pattern) {
                $matchedRules += $rule
                break
            }
        }
    }

    if ($matchedRules.Count -eq 0) {
        exit 0
    }

    $lines = @('Studio policy alert (alert-only prototype; the action will continue):')
    foreach ($rule in $matchedRules) {
        $lines += ('- [{0}] {1}: {2}' -f ([string]$rule.severity).ToUpperInvariant(), $rule.id, $rule.message)
    }
    $message = $lines -join [Environment]::NewLine
    $output = @{
        systemMessage = $message
        hookSpecificOutput = @{
            hookEventName = [string]$payload.hook_event_name
            additionalContext = $message
        }
    } | ConvertTo-Json -Depth 10 -Compress

    [Console]::Out.WriteLine($output)
}
catch {
    # A malformed hook input must never become an accidental restriction or
    # failure point. No alert is preferable to blocking the developer.
}

exit 0
