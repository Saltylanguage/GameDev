# Discord agent collaboration protocol

This is the minimum message contract for a future Discord bridge between Josh's
and Sim's AI-assisted development workflows. It is intentionally transport-
agnostic: the same payload can be posted manually, by a webhook, or by a bot.

## Required fields

Every agent message should include:

```text
type: task-start | progress | decision | blocker | handoff | question
actor: josh | sim | automation
timestamp: ISO-8601 timestamp
task: short stable task name
branch: current branch or `none`
commit: current commit or `uncommitted`
status: active | waiting | complete | blocked
```

Implementation handoffs additionally include:

```text
changed_files: list of repository-relative paths
validation: commands or Unity test suites actually run
risks: known integration or behavioral risks
next_actions: concrete follow-up work
context_links: Markdown files and handoff notes to read
```

## Source-of-truth rules

- Git and checked-in project files are authoritative for implementation state.
- `PROJECT_CONTEXT.md` is authoritative for durable product direction.
- `WORKING_STATE.md` is the entry point to current context.
- `docs/handoffs/` is the historical record of independently reviewable tasks.
- Discord messages are coordination records and pointers, not a replacement for
  those files.
- If Discord and the repository disagree, trust the repository and record a
  corrective handoff.

## Message examples

### Task start

```text
type: task-start
actor: sim
task: terrain-registry-follow-up
branch: NF/TerrainIDs
commit: 7d042f7b
status: active
context_links: docs/PROJECT_CONTEXT.md, docs/CELLULAR_SIM_TODOS.md
```

### Completed handoff

```text
type: handoff
actor: josh
task: discord-collaboration-plan
branch: SaltysFirstBranch
commit: uncommitted
status: complete
changed_files: docs/DISCORD_AGENT_COLLABORATION_TODOS.md, docs/DISCORD_AGENT_COLLABORATION_PROTOCOL.md
validation: git diff --check
risks: no Discord connector is currently available in this Codex environment
next_actions: choose an authenticated Discord bot or integration path
context_links: docs/WORKING_STATE.md, docs/handoffs/<new-note>.md
```

## Automation guardrails

- Agents must not process their own messages as new work.
- A message should have one intended actor or explicitly target both agents.
- Repeated delivery must be idempotent using a stable task and message ID.
- Never post secrets, access tokens, or full private conversation transcripts.
- Do not mutate another contributor's uncommitted files without explicit
  ownership or a clean branch boundary.
- An automated bridge should fail closed when authentication, repository state,
  or message provenance cannot be verified.

## Current bridge utility

[`tools/Discord-AgentBridge.ps1`](../tools/Discord-AgentBridge.ps1) provides a
small transport adapter without storing credentials in the repository:

```powershell
# Validate a message locally without contacting Discord.
.\tools\Discord-AgentBridge.ps1 -Mode Publish `
  -PayloadPath docs/discord-agent-message.example.json -DryRun

# Publish through a webhook supplied outside the repository.
$env:DISCORD_AGENT_WEBHOOK_URL = 'https://discord.com/api/webhooks/...'
.\tools\Discord-AgentBridge.ps1 -Mode Publish `
  -PayloadPath docs/discord-agent-message.example.json

# Read recent messages through a bot token supplied outside the repository.
$env:DISCORD_BOT_TOKEN = '<token>'
$env:DISCORD_CHANNEL_ID = '<channel-id>'
.\tools\Discord-AgentBridge.ps1 -Mode Read
```

The example payload is intentionally checked in, but webhook URLs, bot tokens,
and channel identifiers used for access must remain in environment variables or
another approved secret store.

The bridge validates the required envelope fields locally before any network
request. Handoff messages must also include changed files, validation, risks,
next actions, and context links.
