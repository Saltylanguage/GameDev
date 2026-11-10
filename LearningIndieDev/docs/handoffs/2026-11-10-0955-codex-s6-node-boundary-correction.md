# S6 node boundary correction

[Working state](../WORKING_STATE.md) | Status: in-progress

- Handoff schema: 1
- Handoff ID: 2026-11-10-0955-codex-s6-node-boundary-correction
- Owner: codex
- Branch: codex/s6-predator-first
- Baseline commit: 5160b798
- Date: 2026-11-10
- Supersedes: none

## Summary

Josh corrected the cross-node population design: the played species' population carries forward; every other species and resource population starts from its fixed authored count at each new simulation node. The current Field Data route, portable replay, and published Fox-count sweep retain one entire world instead. The [new analysis](../Research/Experiments/S6-Fox-Start-Sweep-20261110/AI_ANALYSIS_V2.md) withdraws the earlier node-two balance recommendation while preserving the factual retained-world results.

## Changes

- Recorded the clarified rule and implementation gap in [S6 control](../Sprints/S6-control-record.md#november-10-node-boundary-design-correction), `PROJECT_CONTEXT.md`, and `WORKING_STATE.md`.
- Added a separate correction analysis; original contract, factual report, raw results, and v1 analysis remain intact as historical retained-world evidence.
- Added an S5 transition note so its earlier one-ecosystem scope is not mistaken for the current design. No runtime behavior or experiment result changed in this work block.

## Decisions and assumptions

- Josh's November 10 message supersedes the earlier cross-node retained-world design for future work. Within-node rounds still continue normally.
- The exact carried state of the played species (headcount alone versus individual creature state) remains open and affects implementation and corrected testing.
- Static nonplayed starting counts should generalize to any number of species; the rule is not Hare/Fox-specific.

## Validation

- Inspected `SpeciesSimulationPreview.PrepareNextRun`, `FieldDataJourneySession` construction and `Continue`, portable `JourneyReplay`, the Fox-sweep plan, and the Editor test `AuthoredRoutePausesAtAllShopsAndCompletesOnTheSameRun`. All establish one retained run across the current route.
- Verified the old sweep's first node begins with its declared fixed 400 Plants, 55 Hares, and 16/18/20 Foxes; no new simulation was run for this correction.
- `git diff --check` passed for the documentation changes.

## Risks and incomplete work

- The current game is not yet compliant with the clarified node transition. Editing only research documents does not fix the player route or headless runner.
- The earlier tick-1200 rates and node-two Fox/Plant interpretation cannot select a balance value for the intended design. First-node results remain relevant.
- Replacing the run or reseeding its grid affects ledger boundaries, upgrades, retained Hare state, placement capacity, deterministic seeds, telemetry, and Unity/headless parity. The smallest correct implementation depends on the carry-state decision.

## Next useful step

Resolve the played-species carry-state detail, implement a generic node-boundary transition with a focused multi-species regression, verify Unity/headless parity, and approve a new experiment contract before rerunning the starting-Fox comparison. Keep Fox escalation and Plant tuning decisions pending until the corrected node-two results exist.
