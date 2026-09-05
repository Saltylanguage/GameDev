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
- `Species roster and scenario co-design` is not selected for S2 and returns to
  the backlog. The unrelated blocked bug-triage card remains blocked.
- No Sprint 1 task was carried: cards 51–53, 62, and 72 remain complete.

## Scope boundary

The 2026-09-04 [consecutive-phase review](../CONTINUOUS_SIMULATION_FLOW_PLAN.md)
identifies additional runtime and evidence work beyond the accepted launch-time
slice. It is proposed for explicit capacity planning, not automatically committed
to S2. Continuing a phase in memory is distinct from player file saves. The
[impact notice](../CONTINUOUS_SIMULATION_EVIDENCE_IMPACT.md) calls out required
Stat-Line/research retests; the S2.3 ownership is now aligned to Josh.

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
| P0 | Phase-aware Stat-Line and report semantics are not fully wired through the adapter and validator. | Record the gap and keep fresh-window evidence separate from continued-world evidence. |
| P1 | The current board lists more Sim work than the 20-hour S2 planning budget can hold. | Keep, split, or carry each Sim card during the S2 review; do not assume all can finish in this sprint. |
| P1 | Continuation work remains an uncommitted working-tree batch. | Create a reviewable checkpoint before using it as M1 acceptance evidence. |

The proposed extra sprint is recorded in
[`S3-control-record.md`](S3-control-record.md). The former species/build
co-design sprint moves to S4 in the product roadmap.
