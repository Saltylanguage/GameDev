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

- S2 work uses the sprint workflow lists: `🛠️ In Progress` for work being
  actively done and `Current Work` for current-sprint work not yet started.
- The S2 control card is in `🛠️ In Progress` while the sprint is active.
- CF-1 continuation foundation, S2.3B report/Stat-Line integration, and the
  EX-010 preparation card are complete and sit in `✅ Done`. The next-sprint
  EX-010 schedule/approval card is in `🎯 Upcoming Work`.
- `Species roster and scenario co-design` is not selected for S2 and returns to
  the backlog. The unrelated blocked bug-triage card remains blocked.
- No Sprint 1 task was carried: cards 51–53, 62, and 72 remain complete.

### Register-to-board mapping

The repository register uses suffixes where Trello has separate cards with the
same S2 work-package prefix. Links below are the current board records.
The repository owner column remains authoritative for the local plan; the
parallel evidence cards for S2.3A and S2-QA are currently assigned to Sim on
the board and must be reconciled at review rather than silently reassigned.

| Repository ID | Work package | Board card | List at review |
| --- | --- | --- | --- |
| S2.1 | Upgrade contract and boundary | [772a1U6k](https://trello.com/c/772a1U6k) | ✅ Done |
| S2.2A | Catalog design and acceptance matrix | [7e5KhUen](https://trello.com/c/7e5KhUen) | Current Work |
| S2.2B | First catalog slice implementation | [KhHBo2tf](https://trello.com/c/KhHBo2tf) | ✅ Done |
| S2.3A | Deterministic application and contribution evidence | [JORLMXG3](https://trello.com/c/JORLMXG3) | ✅ Done |
| S2.3B | Upgrade loadout report/stat-line integration | [pZ4qG2DM](https://trello.com/c/pZ4qG2DM) | ✅ Done |
| S2.4 | Review and balance evidence | [SSwZkrI1](https://trello.com/c/SSwZkrI1) | ✅ Done |
| EX-002 | Intervention surface and causal-gate preparation | [PVqz2g5n](https://trello.com/c/PVqz2g5n) | ✅ Done |
| S2-QA | Catalog fixtures and invalid-combination tests | [OLZYtNS9](https://trello.com/c/OLZYtNS9) | ✅ Done |
| S2-UI | Player-facing upgrade preview and result summary | [iKcmYkuy](https://trello.com/c/iKcmYkuy) | ✅ Done |
| S2-CORR-FOX | Fox mating/eating telemetry discrepancy | [BkJwxhkw](https://trello.com/c/BkJwxhkw) | Current Work |
| S2-CORR-BOARD | BoardSnapshot fixture repair | [Cy2TOMOh](https://trello.com/c/Cy2TOMOh) | ✅ Done |
| CF-1 | Continuous domain lifecycle parity foundation | [cPlWlTfr](https://trello.com/c/cPlWlTfr) | ✅ Done |
| EX-010-PREP | Continuation contract and prediction-input preparation | [qftxVtnX](https://trello.com/c/qftxVtnX) | ✅ Done |
| EX-010-SCHEDULE | Contract-specific schedule parity and human approval | [LyMlLztN](https://trello.com/c/LyMlLztN) | 🎯 Upcoming Work |

## Scope boundary

The 2026-09-04 [consecutive-phase review](../CONTINUOUS_SIMULATION_FLOW_PLAN.md)
identifies additional runtime and evidence work beyond the accepted launch-time
slice. It is proposed for explicit capacity planning, not automatically committed
to S2. Continuing a phase in memory is distinct from player file saves. The
[impact notice](../CONTINUOUS_SIMULATION_EVIDENCE_IMPACT.md) calls out required
Stat-Line/research retests. The S2.3B report/stat-line integration card is
Josh-owned and complete; final phase-aware Stat-Line meaning review remains in
Sim's active lane.

S2 is a temporary per-run upgrade slice. It does not implement file saves,
wallet mutation, permanent Lab research, Archive or Expedition flows, a
general modifier/plugin framework, broad roster expansion, or final art/audio
production.

## Review cadence

- **Mid-sprint:** verify contract/catalog decisions and one reproducible
  baseline-versus-upgrade run before expanding the catalog.
- **Final review:** run the fixed-seed comparison, record accepted behavior,
  tuning questions, and explicit carry-over. Font, layout, and visual-feedback
  review are a separate work track and are not part of S2.

## M1 delivery risk snapshot

M1 is not expected to close at the end of S2. The following items must be
reviewed explicitly rather than being treated as silently complete:

| Priority | Risk | S2 end-state needed |
| --- | --- | --- |
| P0 | The continuous flow can pause and continue, but phase upgrades and rewards are not yet applied to the same world. | Carry the boundary upgrade/reward work into the proposed S3 M1 closeout sprint, with a matched continuation test. |
| P0 | Phase-aware Stat-Line meaning still needs final review. | Keep fresh-window evidence separate from continued-world evidence until Sim confirms field and denominator meaning. |
| P1 | The current board lists more Sim work than the 20-hour S2 planning budget can hold. | Keep, split, or carry each Sim card during the S2 review; do not assume all can finish in this sprint. |
| P1 | The continuation checkpoint is new and has not yet been promoted as M1 acceptance evidence. | Use committed checkpoint `79423b4e` and its verification artifacts during the S2 review; do not treat smoke runs as EX-010 evidence. |

The proposed extra sprint is recorded in
[`S3-control-record.md`](S3-control-record.md). The former species/build
co-design sprint moves to S4 in the product roadmap.
