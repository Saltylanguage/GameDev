# EX-010 execution contract — draft for human approval

**Experiment:** `EXP-010` — Sequential upgrade continuation
**Contract:** `EX-010-DRAFT-1`
**Status:** Prepared, not approved, not executed
**Owner:** Josh
**Evidence rule:** No result from this draft is research evidence until the
contract is approved, the schedule is run through the same game/headless seam,
and every bundle passes validation.

The production upgrade authoring contract is summarized in the [Species
Per-Run Upgrade Acceptance Matrix](../../../UPGRADE_CATALOG_ACCEPTANCE_MATRIX.md).
EX-010's historical research fixtures remain separate from that production
catalog so their declared values do not change during this experiment.

## Question

When two per-run upgrades are acquired during one evolving expedition, does
acquisition order change the later trajectory or final outcome?

## Candidate fixture

| Field | Proposed value | Approval note |
| --- | --- | --- |
| Scenario | `Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset` | Confirm the scenario revision before execution. |
| Player species | `hare` | Confirm the species and starting state. |
| Upgrade 1 | `faster-movement` | Use the resolved runtime snapshot, not a live asset reference. |
| Upgrade 2 | `crowding-tolerance` | Use the resolved runtime snapshot, not a live asset reference. |
| Combat/options | Same as EX-009 | Lock the ruleset and option values in the final contract. |
| Seed panel | Fresh development and held-out panels | Choose exact ranges without reusing a held-out panel as tuning data. |

## Candidate schedule

The proposed first pass uses three equal segments. The first arm acquires
Upgrade 1 at the first boundary and Upgrade 2 at the second. The second arm
swaps those identities while keeping the initial state, boundary ticks, and
seed identical.

| Segment | Tick window | Arm A | Arm B |
| --- | --- | --- | --- |
| 1 | `(0, 200]` | No added upgrade | No added upgrade |
| 2 | `(200, 400]` | `faster-movement` effective after tick 200 | `crowding-tolerance` effective after tick 200 |
| 3 | `(400, 600]` | Both upgrades | Both upgrades |

The segment length is a proposal only. If the final contract uses another
length, the same value must be used in both arms and recorded before any run.
The first changed rule is effective on the next tick after the boundary; the
boundary tick itself belongs to the preceding window.

## Required evidence per seed and arm

- scenario, ruleset, option, and lifecycle fingerprints;
- initial state and seed;
- each checkpoint's opening/closing state and absolute tick;
- ordered upgrade snapshot, fingerprint, and effective tick;
- phase-window population snapshots and raw metric deltas;
- event ledgers, validity status, and terminal outcome;
- replayable checkpoint lineage and the exact report schema versions;
- A/B pair identity proving the same seed and same schedule.

## Comparison and decision rules

1. Compare Arm A and Arm B pairwise on the same seed and same phase windows.
2. Report per-seed deltas before any panel summary; do not average unlike
   windows or silently turn missing/invalid values into zero.
3. Attribute a difference to order only within this matched schedule. A timing,
   scenario, upgrade-value, or ruleset change requires a new experiment.
4. Accept a bounded order finding only when all required bundles validate and the
   direction is reported with its seed-level consistency and limitations.
5. Reject or leave unresolved when the checkpoint, schedule, report, or metric
   contract is incomplete, mixed, or not reproducible.

## Gate before execution

- [ ] Human approves the scenario, values, seed panels, segment length, options,
      outcomes, and acceptance thresholds.
- [ ] CF-4 phase-window serializer, validator, CSV, and Markdown outputs agree.
- [ ] CF-5 checkpoint round trip and fork isolation pass.
- [ ] Gameplay and headless schedule commands produce the same boundary ticks
      and acquisition timeline.
- [ ] A clean branch/revision and artifact directory are recorded.
- [ ] The experiment is then preregistered as an immutable contract before any
      held-out results are inspected.

Until every box is checked, this document is planning material only.
