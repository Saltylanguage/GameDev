# Sprint 2 Control Record — First Trustworthy Upgrade Loop

> Status: Active | Start: 2026-09-03 | End: 2026-09-16

This control record activates Sprint 2. The authoritative execution detail is
[`../NEXT_WORK_BUCKET_PLAN.md`](../NEXT_WORK_BUCKET_PLAN.md); this record fixes
the identity, capacity, ownership, board disposition, and exit gate.

## Control fields

| Field | Value |
| --- | --- |
| Sprint ID | S2 |
| Status | Active |
| Start | 2026-09-03 |
| End | 2026-09-16 |
| Goal | A player can inspect, choose, and verify a temporary upgrade in a deterministic Forest Edge run. |
| Capacity | Josh 20h; Sim 20h; 40h committed. |
| Entry state | Sprint 1 is closed; its Windows build/review gate is accepted. |
| Exit criteria | A documented catalog covers numeric, spatial, conditional, and tradeoff effects; one temporary selection is applied reproducibly; the effective loadout/fingerprint and contribution evidence are reported; focused tests and a fixed-seed comparison pass; remaining balance questions are documented. |

## Board disposition

- The S2 cards listed in the active task register are in Trello `Current Work`.
- The S2 control card is in `🛠️ In Progress` while the sprint is active.
- `Species roster and scenario co-design` is not selected for S2 and returns to
  the backlog. The unrelated blocked bug-triage card remains blocked.
- No Sprint 1 task was carried: cards 51–53, 62, and 72 remain complete.

## Scope boundary

The 2026-09-04 [consecutive-phase review](../CONTINUOUS_SIMULATION_FLOW_PLAN.md)
identifies additional runtime and evidence work beyond the accepted launch-time
slice. It is proposed for explicit capacity planning, not automatically committed
to S2. Continuing a phase in memory is distinct from player file saves. The
[impact notice](../CONTINUOUS_SIMULATION_EVIDENCE_IMPACT.md) calls out required
Stat-Line/research retests and the repository/live S2.3 ownership mismatch.

S2 is a temporary per-run upgrade slice. It does not implement file saves,
wallet mutation, permanent Lab research, Archive or Expedition flows, a
general modifier/plugin framework, broad roster expansion, or final art/audio
production.

## Review cadence

- **Mid-sprint:** verify contract/catalog decisions and one reproducible
  baseline-versus-upgrade run before expanding the catalog.
- **Final review:** run the fixed-seed comparison, inspect the player-facing
  preview/result language at the target resolutions, record accepted behavior,
  tuning questions, and explicit carry-over.
