# discord-agent-bridge

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: josh
- Branch: SaltysFirstBranch
- Baseline commit: 7d042f7b
- Date: 2026-08-11

## Summary

Defined the repository-side protocol for coordinating Josh's and Sim's AI
workflows through Discord and added a small credential-free PowerShell bridge.
This moves the goal from discussion into an executable integration shape without
putting secrets or private conversation history in the project.

## Changes

- Added `docs/DISCORD_AGENT_COLLABORATION_TODOS.md` with staged priorities.
- Added `docs/DISCORD_AGENT_COLLABORATION_PROTOCOL.md` with message fields,
  source-of-truth rules, examples, and automation guardrails.
- Added `tools/Discord-AgentBridge.ps1` with Publish/Read modes and `-DryRun`.
- Added `docs/discord-agent-message.example.json`.
- Linked the goal and protocol from `PROJECT_CONTEXT.md` and `WORKING_STATE.md`.

## Decisions and assumptions

- Git and checked-in Markdown remain authoritative; Discord is a coordination
  layer only.
- Webhook URLs, bot tokens, and channel identifiers must be supplied through
  environment variables or an approved secret store.
- A real two-way bridge requires an authenticated Discord bot or equivalent;
  this Codex environment currently exposes no Discord connector.

## Validation

- `git diff --check` passed.
- Publish dry-run passed using the checked-in example payload.
- Read dry-run passed with test credentials and no network call.
- Live Discord delivery was not attempted because no credentials were supplied.
- This handoff is intended for Sim to continue on a branch based on the pushed
  commit; no Unity code or gameplay behavior was changed.

## Risks and incomplete work

- The bridge has not yet been tested against a real Discord channel.
- Agent wake-up/orchestration, deduplication, and channel permissions remain
  TODO-COLLAB-04 and TODO-COLLAB-05.
- The working tree contains an unrelated ProBuilder settings modification that
  was intentionally left untouched.

## Next useful step

Choose and authorize a Discord bot or webhook path, then run one real publish
and read handoff through a restricted test channel. Start with:

```powershell
cd LearningIndieDev
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Discord-AgentBridge.ps1 `
  -Mode Publish -PayloadPath .\docs\discord-agent-message.example.json -DryRun
```

For live testing, provide `DISCORD_AGENT_WEBHOOK_URL` for publishing, and
`DISCORD_BOT_TOKEN` plus `DISCORD_CHANNEL_ID` for reading. Keep all values out
of Git and replace the example payload with a real task handoff.
