---
name: playable-route-review
description: Review a playable game route with reproducible evidence and player feedback. Use for internal playtests or playability gates.
---

# Playable Route Review

Review whether a player can complete and understand an intended route, where choices change the next state, and which moments invite or discourage another attempt. This skill structures observation and evidence; it cannot certify that a game is fun.

## Define the route being reviewed

Read the current goal and acceptance record. For Forest Edge S5, start with the [playability goalpost](../../../LearningIndieDev/docs/Sprints/S5-playability-goalpost-2026-10-16.md); use the current ticket/control record and implementation to resolve its actual scope. The S5 two-simulation/one-shop route is a first review gate, not a permanent definition of this skill or a replacement for the full journey.

Record the build or source revision, branch and dirty-state disclosure, profile and initial world state, scenario and seed where controlled, entry and exit points, required choices, and the expected result. Name the human reviewer or player when known. If a review depends on a build or play state that cannot be reached, establish that as a finding rather than inventing the remainder of the route.

## Gather evidence

- Follow the route in the running game or a suitable test fixture when available. `unity-cli` and the [Unity plugin workflows](../../../LearningIndieDev/docs/UNITY_PLUGIN_WORKFLOWS.md) cover Editor, test, and visual capture operations. Inspect the actual screenshots and results; a passing capture test proves the capture state, not visual quality.
- Record a short action-and-response timeline at the decision and result moments: what the player saw, chose, paid or earned, what changed in the world and wallet, and what explanation the game gave. Retain seed, input sequence, receipts, run status, relevant logs, and screenshot paths for reproduction. Keep ecology and economy outcomes separate.
- If comparing plans or a follow-up attempt, hold the declared starting inputs fixed where possible and identify every meaningful difference. Use `cellsim-experiment` only when a controlled simulation comparison is actually needed.
- Collect the player's own observations separately from telemetry and the agent's interpretation. Ask about promising, confusing, tedious, and unfair moments when a human play review is in scope; do not invent player feedback. If direct interaction or observation is unavailable, report what can be verified and what needs a human play session.

## Review packet

Give the route outcome against each applicable gate; an evidence-linked timeline of important moments; reproducible defects or unclear interactions ranked by impact; the observed effects of choices; and a concise next review or fix. Mark each conclusion as observed, inferred, or still untested. A named human decides accept, revise, or inconclusive for playability. Keep that decision distinct from automated test results and AI recommendations under [SG-001](../../../LearningIndieDev/docs/Studio%20Guidelines/AI_GENERATED_REPORTS.md).

Create a durable report or handoff when the task or project workflow calls for one. Do not silently change product criteria, ticket scope, or balance values while preparing a review.
