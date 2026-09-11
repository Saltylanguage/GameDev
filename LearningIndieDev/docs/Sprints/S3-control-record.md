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
| Entry state | CF-1 through CF-5 are implemented and verified; EX-010's approved ten-phase original and reverse-order sequences are executed, validated, and accepted as bounded evidence. |
| Primary outcome | The implemented ten-phase Forest Edge flow passes integrated product acceptance and can be completed through the player-facing route without developer-only fields. |
| M2 relationship | This sprint prepares M2; species/build co-design moves to S4. |

## Proposed scope

### Josh — runtime, integration, and evidence handoff

1. Preserve the completed CF-1 through CF-5 lifecycle, boundary, reporting,
   checkpoint, and schedule contracts.
2. Preserve the completed target-resolution graphics evidence, then complete a
   current Windows development-build smoke and ten-phase duration/memory
   measurement.
3. Connect the smallest player-facing Mutation/result route to the existing
   domain boundary without duplicating lifecycle or reward ownership.
4. Preserve the aligned phase metadata, ordered loadout timing, report output,
   and prediction-input adapter. Treat EX-010 as completed evidence and keep
   new predictive work on its own approved contract.
5. Remove stale task/document references, split historical evidence from
   current evidence, and leave a reviewable branch checkpoint.

### Sim — Stat-Line and Forest Edge evidence

1. Preserve the completed Forest Edge Stat-Line meaning review: fields,
   denominators, windows, and validity labels.
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
| P0 | Build/performance acceptance is incomplete. | Automated domain and graphics evidence alone do not prove the ten-phase player experience is shippable. | Josh | Current Windows build smoke and ten-phase duration/memory evidence are recorded. |
| P0 | EX-010-specific checkpoint/schedule parity is complete. | The evidence remains bounded to its approved schedule; new schedules could be mistaken for the same result. | Josh + Sim | Keep new schedules under a separate approved contract. |
| P1 | Sim's current board load is roughly 32h against the 20h planning budget. | The active evidence work cannot all finish inside S2 without re-scoping or carry-over. | Josh + Sim | S2 review explicitly keeps, splits, or carries each Sim card. |
| P1 | The continuation checkpoint is new and is now supported by the EX-010 evidence package. | A later schedule could silently exceed the accepted evidence envelope. | Josh + Sim | Keep the EX-010 scope and human decision linked to any reuse. |
| P1 | The new 12h upgrade-expansion card may expand catalog scope before the M1 gate is closed. | More content could consume capacity without improving the core loop. | Josh + Sim | Card is re-scoped, carried forward, or explicitly accepted as non-M1 work. |
| P2 | Broader predictive calibration remains untested after EX-010. | The bounded continuation result could be overgeneralized into a general AI claim. | Josh + Sim | Keep EX-003 deferred; reopen it only with a new approved workflow-validity contract. |

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
- New predictive calibration or generalized AI recommendation validation.
- A generalized modifier, evolution, or plugin framework.

## Next sprint after S3

The former S3 species/build co-design plan becomes S4. Its outcome remains the
same: Trailblazer, Warren, and Gardeners must produce distinct strategies in
Forest Edge, supported by matched-seed evidence and an in-game review.

Related plans: [ROADMAP.md](../../ROADMAP.md),
[FUTURE_SPRINT_ROADMAP.md](../FUTURE_SPRINT_ROADMAP.md),
[CONTINUOUS_SIMULATION_FLOW_PLAN.md](../CONTINUOUS_SIMULATION_FLOW_PLAN.md),
and [VERTICAL_SLICE_SELECTION.md](../VERTICAL_SLICE_SELECTION.md).
