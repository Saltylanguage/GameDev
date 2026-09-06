# EX-010 execution contract — draft for human approval

**Experiment:** `EXP-010` — Sequential upgrade continuation
**Contract:** `EX-010-DRAFT-1`
**Status:** Prepared; Stat-Line meaning review confirmed with Sim on 2026-09-06; schedule details and human approval remain; not executed
**Owner:** Josh
**Evidence rule:** No result from this draft is research evidence until the
contract is approved, the schedule is run through the same game/headless seam,
and every bundle passes validation.

The production upgrade authoring contract is summarized in the [Species
Per-Run Upgrade Acceptance Matrix](../../../UPGRADE_CATALOG_ACCEPTANCE_MATRIX.md).
EX-010's historical research fixtures remain separate from that production
catalog so their declared values do not change during this experiment.

## Question

Across one full ten-phase expedition, how do different preselected sequential
upgrade histories affect the later phase Stat-Lines and the final expedition
Stat-Line?

## Candidate fixture

| Field | Proposed value | Approval note |
| --- | --- | --- |
| Scenario | `Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset` | Confirm the scenario revision before execution. |
| Player species | `hare` | Confirm the species and starting state. |
| Upgrade schedule | All six currently available upgrades, each used once, plus three explicit Skip decisions | Choose the exact stable IDs, order, and Skip positions before execution. |
| Combat/options | Same as EX-009 | Lock the ruleset and option values in the final contract. |
| Seed panel | Fresh development and held-out panels | Choose exact ranges without reusing a held-out panel as tuning data. |

## Candidate schedule

The agreed first pass uses ten equal 200-tick phases. Phase 1 starts with no
upgrades. After each of phases 1 through 9, the player makes one preselected
decision. Each research sequence contains all six upgrades once and three
Skips. The exact nine decisions are fixed before the run and used for every
seed. If EX-010 compares alternate sequences, each full sequence must be
written down before execution; the meeting did not choose a second sequence.

| Phase | Tick window | Decision before this phase |
| --- | --- | --- |
| 1 | `(0, 200]` | No added upgrade |
| 2 | `(200, 400]` | Decision 1 |
| 3 | `(400, 600]` | Decision 2 |
| 4 | `(600, 800]` | Decision 3 |
| 5 | `(800, 1000]` | Decision 4 |
| 6 | `(1000, 1200]` | Decision 5 |
| 7 | `(1200, 1400]` | Decision 6 |
| 8 | `(1400, 1600]` | Decision 7 |
| 9 | `(1600, 1800]` | Decision 8 |
| 10 | `(1800, 2000]` | Decision 9 |

At a boundary, the simulation stops at the completed tick. The selected
upgrade becomes active at that boundary, and the next tick uses it. A Skip
keeps the current upgrade list unchanged.

## Required evidence per seed and arm

- scenario, ruleset, option, and lifecycle fingerprints;
- initial state and seed;
- each checkpoint's opening/closing state and absolute tick;
- ordered upgrade snapshot, fingerprint, and effective tick;
- phase-window population snapshots and raw metric deltas;
- event ledgers, validity status, and terminal outcome;
- replayable checkpoint lineage and the exact report schema versions;
- Sequence identity proving the same seed and preselected schedule; if an
  alternate research sequence is added, its full schedule must also be fixed
  before execution.

## Comparison and decision rules

1. Report the complete Stat-Line for every phase and the independently
   calculated final expedition Stat-Line.
2. Show chronological per-stat deltas against the immediately preceding phase
   or run; do not turn the full Stat-Line into a new combined score.
3. If alternate sequences are compared, compare their complete reports under
   the declared research design and have Josh and Sim review the findings
   directly.
4. Keep `N/A`, invalid, partial, and no-data states visible; never replace them
   with zero for convenience.
5. Reject or leave unresolved when the checkpoint, schedule, report, or metric
   contract is incomplete, mixed, or not reproducible.

## Gate before execution

- [ ] Human approves the scenario, values, seed panels, segment length, options,
      outcomes, and acceptance thresholds.
- [x] Josh and Sim confirm the phase-aware Herbivore Stat-Line meanings and
      the independent phase/final reporting model (2026-09-06).
- [ ] CF-4 phase-window serializer, validator, CSV, and Markdown outputs agree.
- [ ] CF-5 checkpoint round trip and fork isolation pass.
- [ ] Gameplay and headless schedule commands produce the same boundary ticks
      and acquisition timeline.
- [ ] A clean branch/revision and artifact directory are recorded.
- [ ] The experiment is then preregistered as an immutable contract before any
      held-out results are inspected.

Until every box is checked, this document is planning material only.
