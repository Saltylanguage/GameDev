[CmdletBinding()]
param(
    [ValidateSet('Publish', 'Read')]
    [string]$Mode = 'Publish',

    [string]$PayloadPath,
    [string]$WebhookUrl = $env:DISCORD_AGENT_WEBHOOK_URL,
    [string]$BotToken = $env:DISCORD_BOT_TOKEN,
    [string]$ChannelId = $env:DISCORD_CHANNEL_ID,
    [string]$AfterMessageId,
    [switch]$DryRun
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Require-Value {
    param(
        [string]$Name,
        [string]$Value
    )

    if ([string]::IsNullOrWhiteSpace($Value)) {
        throw "Missing $Name. Supply it as a parameter or environment variable."
    }
}

function Read-Payload {
    if ([string]::IsNullOrWhiteSpace($PayloadPath)) {
        throw 'Publish mode requires -PayloadPath pointing to a JSON message payload.'
    }

    if (-not (Test-Path -LiteralPath $PayloadPath -PathType Leaf)) {
        throw "Payload file was not found: $PayloadPath"
    }

    $raw = Get-Content -LiteralPath $PayloadPath -Raw
    if ($raw.Length -gt 1800) {
        throw 'Payload is too large for a single Discord message. Keep handoffs concise and link to repository files.'
    }

    try {
        $message = $raw | ConvertFrom-Json
    }
    catch {
        throw "Payload is not valid JSON: $($_.Exception.Message)"
    }

    $requiredFields = 'type', 'actor', 'timestamp', 'task', 'branch', 'commit', 'status'
    foreach ($field in $requiredFields) {
        if ($null -eq $message.$field -or [string]::IsNullOrWhiteSpace([string]$message.$field)) {
            throw "Payload is missing required field '$field'."
        }
    }

    if ($message.type -eq 'handoff') {
        foreach ($field in 'changed_files', 'validation', 'risks', 'next_actions', 'context_links') {
            if ($null -eq $message.$field) {
                throw "Handoff payload is missing required field '$field'."
            }
        }
    }

    return $raw
}

if ($Mode -eq 'Publish') {
    $payload = Read-Payload
    Require-Value -Name 'DISCORD_AGENT_WEBHOOK_URL' -Value $WebhookUrl

    $content = '```json' + [Environment]::NewLine + $payload + [Environment]::NewLine + '```'
    $body = @{ content = $content } | ConvertTo-Json -Compress

    if ($DryRun) {
        [pscustomobject]@{
            mode = 'Publish'
            dryRun = $true
            endpoint = 'configured webhook (redacted)'
            payload = ($payload | ConvertFrom-Json)
        } | ConvertTo-Json -Depth 10
        exit 0
    }

    Invoke-RestMethod -Method Post -Uri $WebhookUrl -ContentType 'application/json' -Body $body | ConvertTo-Json -Depth 10
    exit 0
}

Require-Value -Name 'DISCORD_BOT_TOKEN' -Value $BotToken
Require-Value -Name 'DISCORD_CHANNEL_ID' -Value $ChannelId

$uri = "https://discord.com/api/v10/channels/$ChannelId/messages?limit=100"
if (-not [string]::IsNullOrWhiteSpace($AfterMessageId)) {
    $uri = "$uri&after=$AfterMessageId"
}

if ($DryRun) {
    [pscustomobject]@{
        mode = 'Read'
        dryRun = $true
        channelId = $ChannelId
        afterMessageId = $AfterMessageId
    } | ConvertTo-Json
    exit 0
}

$headers = @{ Authorization = "Bot $BotToken" }
Invoke-RestMethod -Method Get -Uri $uri -Headers $headers | ConvertTo-Json -Depth 20
