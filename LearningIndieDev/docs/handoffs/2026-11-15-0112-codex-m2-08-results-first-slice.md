# M2-08 results first slice

[Working state](../WORKING_STATE.md) | Status: implementation in progress

- Handoff schema: 1
- Handoff ID: 2026-11-15-0112-codex-m2-08-results-first-slice
- Owner: codex
- Branch: codex/s6-predator-first
- Baseline commit: f4c1ee34
- Date: 2026-11-15
- Supersedes: none

## Summary

Local M2-08 first slice. The results panel now reports observed player-species
births, deaths, predator encounters and kills, a cause breakdown, and the
leading recorded loss. It appears on the canonical Field Data terminal results
as well as the prototype journey results. It does not yet meet the M2-08
contribution gate: there is no run-level evidence that isolates individual
Mutation effects.

## Changes

- `VM_SimulationShell` sums closed phase metric windows for the entire
  expedition. It does not rely on `run.Metrics`, which restarts empty after a
  checkpoint restore. Latest closing populations still come from the final
  closed phase, before a node reset can replace the live grid.
- The cause line counts creature death events and only names a leading cause
  when the event count reconciles with the aggregate death count. Combat,
  starvation, and other causes can tie; ties are called mixed. Predator kills
  use the simulation's carnivore-on-herbivore kill counter, rather than
  treating every combat death as a Fox kill. A missing event record shows
  `Cause detail unavailable` instead of guessing.
- Field Data success, failure, and player-ended results now show the existing
  journey observations panel. The existing run-record section explicitly says
  that individual upgrade contributions were not isolated. The prototype
  Mutation list carries the same qualification.
- `SpeciesPresentationPlayModeTests` adds a focused phase-aggregation and cause
  test and assertions in the existing Field Data success, natural-failure,
  and manual-end fixtures.

## Evidence and limits

- `SpeciesSimulationMetrics` records births, deaths, starvation deaths,
  carnivore-herbivore encounters, and predator kills. Closed
  `SimulationPhaseResult.Metrics` stores each phase's window and survives
  checkpoint restore. `SimulationRunState.Restore` does not reconstruct the
  live cumulative metrics accumulator, which is why the panel sums windows.
- The simulation currently does not record which pre-contact attacks Threat
  Exposure avoided, which reproduction rolls Reproductive Drive changed, or
  which Fox hits required Tough Hide's extra health. Carried upgrade names and
  aggregate outcomes cannot prove causal contributions. Add effect-level
  telemetry at those three resolution points, or an explicitly matched
  comparison, before claiming M2-08 acceptance.
- The UI readout describes the full expedition. It reports contact encounters
  after pre-contact avoidance, and predator kills from any carnivore, not
  necessarily Fox. It does not claim that births replaced specific losses or
  that a selected Mutation caused the observed outcome.

## Validation

- Read-only audit covered the result bindings, canonical Field Data and
  prototype states, phase closure and checkpoint restore, metric capture,
  and the three Mutation resolution paths.
- `git diff --check` passed. No Unity compile, PlayMode, visual capture, or
  representative route was run, following the implementation-first validation
  plan for the November 16 shared batch.
- The changes remain local and uncommitted alongside pre-existing M2-04,
  M2-07, art, XAML, docs, scene, and artifact edits. No Trello card changed.

## Next steps

1. Add direct, phase-retained effect evidence for the three introductory
   Mutations. Keep the applied-rule distinction separate from changed outcomes;
   check checkpoint restore and mutation acquisition boundaries.
2. In the November 16 validation batch, run the focused
   `JourneyOutcomeAggregatesPhaseEvidenceAndLeadingCause` PlayMode test and
   the three existing Field Data fixtures with the new result assertions.
   Inspect the scrollable results view at 1280×720 and 1920×1080 for success,
   extinction, and player-ended journeys.
3. Review one representative Hare route with a player before closing the S6
   causal-read or S7 result gate. Keep M2-08 In Progress until contribution
   evidence and that review pass.
