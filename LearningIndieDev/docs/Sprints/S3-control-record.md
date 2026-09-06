# Sprint 3 Control Record — M1 Closeout and Project Hygiene

> **Status:** Proposed | **Dates:** 2026-09-17–2026-09-30 | **Cadence:** two weeks

Sprint 2 is not expected to close the M1 gate. Sprint 3 is therefore reserved
for finishing the first trustworthy upgrade loop, cleaning up the evidence and
task records, and removing the highest-friction seams before we begin the
species/build work that was previously planned for S3.

This is a planning record, not a kickoff. It becomes committed only after the
S2 review confirms the remaining work and capacity.

## Control fields

| Field | Value |
| --- | --- |
| Sprint ID | S3 |
| Goal | Close the remaining M1 implementation gates and leave the project easy to inspect and continue. |
| Capacity | Josh 20h; Sim 20h; 40h planning capacity. |
| Entry state | S2 remains active; CF-1/CF-2 runtime work and the controlled CF-3 preview path are implemented; CF-4 and generic CF-5 evidence integration are verified, with EX-010-specific closure remaining. |
| Primary outcome | One complete ten-phase Forest Edge run can be continued, upgraded, reported, and reviewed without silently rebuilding the world. |
| M2 relationship | This sprint prepares M2; species/build co-design moves to S4. |

## Proposed scope

### Josh — runtime, integration, and evidence handoff

1. Finish the CF-1 continuation gate: same world, absolute tick, prior state,
   explicit end, restart, and no extra tick at a decision boundary.
2. Add the smallest boundary upgrade/reward path needed to prove that one
   upgrade or Skip resumes the same run with the new rule effective on the next
   tick.
3. Align phase metadata, ordered loadout timing, report output, and the
   prediction-input adapter. Keep EX-010 execution gated until its checkpoint
   contract is actually ready.
4. Remove stale task/document references, split historical evidence from
   current evidence, and leave a reviewable branch checkpoint.

### Sim — Stat-Line and Forest Edge evidence

1. Complete the Forest Edge Stat-Line meaning review: fields, denominators,
   windows, and validity labels.
2. Reconcile Hare, Fox, and Plant population, feeding, combat, reproduction,
   and mortality totals on the agreed evidence set.
3. Predeclare and run only the next diagnostic arm needed to resolve a known
   telemetry or scenario question. Do not expand balance tuning from a single
   run.
4. Produce a concise evidence handoff that names remaining instrumentation
   gaps and the next safe action.

### Shared hygiene and polish

- Reconcile the S2 plan, control record, handoffs, branch state, and Trello
  cards so each active item has one owner, one estimate, and one acceptance
  check.
- Keep the existing seven-upgrade catalog unless evidence shows that a specific
  entry is inactive, misleading, or duplicated. More upgrades are not a goal
  of this sprint.
- Review tests, report validators, links, and generated artifacts for stale or
  misleading references.

## M1 risk register for the S2 review

| Priority | Risk | Why it matters | Owner | Exit evidence |
| --- | --- | --- | --- | --- |
| P0 | Phase-aware Stat-Line and report semantics still need final review. | Fresh-window evidence could be mistaken for continued-world evidence. | Josh, with Sim review | Phase/expedition windows, acquisition timing, and validity status agree across report, Stat-Line, adapter, and validator. |
| P0 | EX-010-specific checkpoint/schedule parity is not finished. | EX-010 could be run with a different schedule than the game or without a replayable boundary state. | Josh | Checkpoint round trip and the EX-010 schedule parity check pass before execution. |
| P1 | Sim's current board load is roughly 32h against the 20h planning budget. | The active evidence work cannot all finish inside S2 without re-scoping or carry-over. | Josh + Sim | S2 review explicitly keeps, splits, or carries each Sim card. |
| P1 | The continuation checkpoint is new and has not yet been promoted as M1 acceptance evidence. | A milestone decision should rely on a reproducible checkpoint and explicit verification, not only on smoke runs. | Josh | Review committed checkpoint `79423b4e` and its verification artifacts during S2 review; keep EX-010 evidence gated. |
| P1 | The new 12h upgrade-expansion card may expand catalog scope before the M1 gate is closed. | More content could consume capacity without improving the core loop. | Josh + Sim | Card is re-scoped, carried forward, or explicitly accepted as non-M1 work. |
| P2 | EX-010 remains blocked on checkpoint and telemetry contracts. | Research claims about sequential acquisition would be premature. | Josh | EX-010 stays preparatory until CF-5 prerequisites pass. |

## Acceptance gate

Sprint 3 is ready to close when:

- the M1 gate matrix has no unresolved P0 item;
- a complete ten-phase Forest Edge path can pause, Skip or apply upgrades,
  continue from the same state, and reach a result;
- the first changed rule is effective at the recorded tick and the ordered
  loadout is preserved;
- phase and expedition evidence are distinguishable and reconciled;
- the player path does not require raw developer fields;
- active docs, handoffs, tests, and board mappings agree with the branch; and
- all remaining work is named as an explicit carry-over, not left implicit.

## Out of scope

- Permanent Lab upgrades, wallet settlement, or player save/load.
- Broad species or scenario expansion.
- Full reactive-ecology or rubber-banding implementation.
- Final art/audio production or visual polish as a separate work track.
- Full EX-010 execution or predictive calibration.
- A generalized modifier, evolution, or plugin framework.

## Next sprint after S3

The former S3 species/build co-design plan becomes S4. Its outcome remains the
same: Trailblazer, Warren, and Gardeners must produce distinct strategies in
Forest Edge, supported by matched-seed evidence and an in-game review.

Related plans: [ROADMAP.md](../../ROADMAP.md),
[FUTURE_SPRINT_ROADMAP.md](../FUTURE_SPRINT_ROADMAP.md),
[CONTINUOUS_SIMULATION_FLOW_PLAN.md](../CONTINUOUS_SIMULATION_FLOW_PLAN.md),
and [VERTICAL_SLICE_SELECTION.md](../VERTICAL_SLICE_SELECTION.md).
