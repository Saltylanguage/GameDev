# s4-trailblazer-spatial-diagnostic

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-11-1040-codex-s4-trailblazer-spatial-diagnostic
- Owner: codex
- Branch: codex/s4-gestation-checkpoint-20261010
- Baseline commit: ad6588ee
- Date: 2026-10-11
- Supersedes: none

## Summary

The S4-03 Trailblazer follow-up is pushed on `codex/s4-gestation-checkpoint-20261010` at `ad6588ee`. Five existing screen arms were replayed on the same 20 seeds with read-only Hare spatial telemetry. Movement is measurably faster after the Trailblazer choices, while population-wide coverage is only modest and inconsistent. The player decision flow offers a Trailblazer starter on 12/20 nominal first offers; a selected starter puts its partner in the next offer when eligible. The current game still uses a seven-neighbor crowding trigger, so the approved four-neighbor density surcharge is a separate pending implementation and must not be inferred from these rates. Josh and Sim have not accepted or revised the path yet.

## Changes

- Added the Hare-only spatial observer and report schema 31 in `Assets/Editor/SimulationTools/CellularSimulationExperimentRunner.cs`. It records distinct cells, per-Hare travel, current-rule crowding, Fox adjacency, and per-phase observations without affecting the simulation.
- Added one Editor test for exact known-grid telemetry and one PlayMode test for the Movement → Threat Avoidance offer flow and text.
- Predeclared the replay in [the plan](../Sprints/S4-03-trailblazer-spatial-plan-20261011.md). The [factual report](../Sprints/S4-03-trailblazer-spatial-20261011/report.md), [separate analysis](../Sprints/S4-03-trailblazer-spatial-20261011/ai-analysis-v1.md), [metadata](../Sprints/S4-03-trailblazer-spatial-20261011/metadata.json), and compact metrics are tracked. Raw JSON, CSV, Unity logs, and test XML are local ignored artifacts under `artifacts/s4-trailblazer-spatial-20261011/` in this managed worktree.
- Updated `docs/WORKING_STATE.md`, the S4 control record, and the S4-03 and S4 Trello cards after pushing.

## Decisions and assumptions

- Josh's existing 400/20/10 screen inputs and matched seeds 11100–11119 were reused. No production scenario, skill value, or balance rule was changed.
- Fox adjacency is proximity in the eight neighboring cells, not an attack opportunity. Crowding observations reflect the current seven-neighbor rule; the approved density surcharge begins at four neighbors and remains unimplemented.
- The offered-build access decision and balance interpretation remain human-owned. The scheduled replay bypasses the offer UI; the focused PlayMode test checks one actual pair route, but not visual comprehension.

## Validation

- Unity 6000.4.6f1 one-seed smoke run compiled and wrote schema 31 telemetry. Post-commit smoke reproduced its full seed 11100 run record exactly.
- Five 20-seed arm replays completed. Comparing all 100 run records with the archived S4-03 screen after removing only `spatial` gave 100 exact matches. All five raw SHA-256 hashes match the tracked metadata.
- Focused Unity EditMode test `SpatialObserverCountsTravelCrowdingAndFoxProximityWithoutChangingGrid`: 1/1 passed. Focused Unity PlayMode test `TrailblazerPairCanBeChosenThroughPhaseOffers`: 1/1 passed.
- `git diff --check` passed before commits; the pushed branch was clean at `ad6588ee`. Full suites were not rerun for this observer-only change.
- `tools/Test-Handoffs.ps1` was run after this note. It failed on 24 existing historical handoff errors (missing old local artifacts or unsupported old statuses); it listed no error for this new note.

## Risks and incomplete work

- Mean population-wide distinct cells rose from 524.4/720 in control to 538.4 and 537.2 in the two pair orders, with mixed matched-seed outcomes. Do not claim broad map control from this screen.
- The current crowding rule is materially different from Josh's approved provisional density rule. The old screen is evidence for the old rule only.
- Raw 20-seed reports are ignored local files in the attached worktree; the tracked metadata and summary are the shareable compact evidence. Preserve the worktree if a reviewer needs raw traces.
- Human visual review, the 400/25/15 stress comparison, final thresholds, and fresh 200-seed confirmation remain open.

## Next useful step

Ask Josh whether the first Hare mutation offer should guarantee access to one Trailblazer starter or keep its rotating three-card design. Then implement the already-approved four-neighbor density surcharge as a separate, explicitly labeled balance change and replay the matched control/Trailblazer comparison before using crowding as acceptance evidence. Coordinate the in-game visual review and later frozen confirmation with Sim.
