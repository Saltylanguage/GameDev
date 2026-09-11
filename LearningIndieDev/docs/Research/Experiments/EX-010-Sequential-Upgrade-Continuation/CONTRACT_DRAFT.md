# EX-010 execution contract

**Experiment:** `EXP-010` — Sequential upgrade continuation
**Contract:** `EX-010-DRAFT-1`
**Status:** Approved by Josh and Sim on 2026-09-06; executed and reported in [REPORT.md](REPORT.md); human decision recorded in [HUMAN_DECISION.md](HUMAN_DECISION.md)

The historical filename is retained so existing evidence links remain stable;
this is the approved execution contract, not an unapproved draft.
**Owner:** Josh
**Evidence rule:** No result from this draft is research evidence until the
contract is approved, the schedule is run through the same game/headless seam,
and every bundle passes validation.

The production upgrade authoring contract is summarized in the [Species
Per-Run Upgrade Acceptance Matrix](../../../UPGRADE_CATALOG_ACCEPTANCE_MATRIX.md).
EX-010's historical research fixtures remain separate from that production
catalog so their declared values do not change during this experiment.

## Question

Across one full ten-phase expedition, how do the phase-by-phase Stat-Lines and
the final expedition Stat-Line change as one fixed sequential upgrade history is
applied?

## Candidate fixture

| Field | Proposed value | Approval note |
| --- | --- | --- |
| Scenario | `Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset` | Confirm the scenario revision before execution. |
| Player species | `hare` | Confirm the species and starting state. |
| Upgrade schedule | The six production upgrades that can apply after a run starts, each used once, plus three explicit Skip decisions | Locked below; `gardeners-seed-pouches` is excluded because it is launch-only. |
| Combat/options | Same as EX-009 | Lock the ruleset and option values in the final contract. |
| Seed panel | Fresh development and held-out panels | Choose exact ranges without reusing a held-out panel as tuning data. |

## Candidate schedule

The approved first pass uses ten equal 200-tick phases. Phase 1 starts with no
upgrades. After each of phases 1 through 9, the player makes one preselected
decision. The fixed schedule uses all six upgrades that are eligible after run
start once and three Skips, and is used for every seed.

| Phase | Tick window | Decision before this phase |
| --- | --- | --- |
| 1 | `(0, 200]` | No added upgrade |
| 2 | `(200, 400]` | Skip |
| 3 | `(400, 600]` | `trailblazer-long-stride` |
| 4 | `(600, 800]` | Skip |
| 5 | `(800, 1000]` | `trailblazer-far-sight` |
| 6 | `(1000, 1200]` | `warren-guarded-burrow` |
| 7 | `(1200, 1400]` | `warren-room-to-breed` |
| 8 | `(1400, 1600]` | Skip |
| 9 | `(1600, 1800]` | `gardeners-careful-sowing` |
| 10 | `(1800, 2000]` | `familial-bond-large-litters` |

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
- Sequence identity proving the same seed and preselected schedule.

## Comparison and decision rules

1. Report the complete Stat-Line for every phase and the independently
   calculated final expedition Stat-Line.
2. Show chronological per-stat deltas against the immediately preceding phase
   or run; do not turn the full Stat-Line into a new combined score.
3. Keep `N/A`, invalid, partial, and no-data states visible; never replace them
   with zero for convenience.
4. Reject or leave unresolved when the checkpoint, schedule, report, or metric
   contract is incomplete, mixed, or not reproducible.

## Execution gate and disposition

- [x] Josh and Sim approve the scenario, values, seed panels, segment length,
      options, outcomes, and acceptance thresholds (2026-09-06).
- [x] Josh and Sim confirm the phase-aware Herbivore Stat-Line meanings and
      the independent phase/final reporting model (2026-09-06).
- [x] CF-4 phase-window serializer, validator, CSV, and Markdown outputs agree.
- [x] CF-5 checkpoint round trip and fork isolation pass.
- [x] The executed headless schedule produced the locked boundary ticks and
      acquisition timeline; the UI path uses the same boundary contract, but no
      separate UI-versus-headless equivalence arm is claimed here.
- [x] The source revision and artifact directory are recorded; unrelated
      working-tree edits are disclosed in the manifest.
- [x] The experiment was preregistered as an immutable contract before any
      held-out results were inspected.

The checklist and contract hash are preserved as the immutable execution
record. The report and human decision supersede the former draft-only status.
