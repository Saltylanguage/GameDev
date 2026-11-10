# S6 predator-first diagnostic

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-11-09-2007-codex-s6-predator-first-diagnostic
- Owner: codex
- Branch: codex/s6-predator-first
- Baseline commit: 6b6972db
- Date: 2026-11-09
- Supersedes: none

## Summary

The [approved S6 diagnostic](../Research/Experiments/S6-Predator-First-Diagnostic-20261109/REPORT.md)
tested the published November Forest Edge route and three matched first-choice
Mutation mechanics on seeds 10100–10111. Fox pressure is already the opening
problem, but six of twelve unassisted seeds lose Hares before the first node
ends. Seven lose by the second node. Current `feature/s6-fun-pass` Unity and
portable route checks both fail seed 10100 at tick 236, with Plants still
present. The previous tick-1210 handoff came from a different dirty checkout.

## Changes

- Fixed portable Field Data journey replay so it passes the exported
  experimental options to `SpeciesSimulationRunner`; added a small self-test.
- Kept the [experiment contract](../Research/Experiments/S6-Predator-First-Diagnostic-20261109/CONTRACT.md),
  [factual report](../Research/Experiments/S6-Predator-First-Diagnostic-20261109/REPORT.md),
  [paired per-seed summary](../Research/Experiments/S6-Predator-First-Diagnostic-20261109/paired-summary.json),
  and [separate AI analysis](../Research/Experiments/S6-Predator-First-Diagnostic-20261109/AI_ANALYSIS.md).
- Updated `PROJECT_CONTEXT.md` and `WORKING_STATE.md` with the approved
  predator-first direction, integration state, and current-source result.
- No production scenario, Mutation value, serialized gameplay asset, or
  player-facing flow was changed.

## Decisions and assumptions

- Josh approved the test plan and push in the design chat; the scenario's
  primary pressure and three strategy families are already locked in the
  [S6 control record](../Sprints/S6-control-record.md#november-9-predator-first-design-decision).
- The 12 consecutive seeds are diagnostic, including known seeds 10100 and
  10108. Their 7/12 second-node loss count must not be accepted as a calibrated
  60% rate. Forced Mutation arms bypass offer and profile research eligibility.
- The report contains observations; AI analysis recommends the next test.
  Josh has not accepted a tuning value or revised Mutation design.

## Validation

- Unity 6000.4.6f1 snapshot export and one-seed reference completed with source
  hash `6fcc6913b758717217150094b7bdd9c8afca85cb96eb96484ab31669f3db3990`.
- `dotnet build tools/CellSim.Batch/CellSim.Batch.csproj -c Release` passed;
  `CellSim.Batch self-test` passed with project-local temp and .NET 9 roll-forward.
- The focused clean Unity PlayMode authored-route test ran 1/1 and failed its
  completion assertion at tick 236. The current ecology terminated normally.
- Twelve no-action portable journeys completed; route and control mechanics
  shared-boundary populations matched on every seed. Two successful journeys
  matched final board digests. Three real first-offer choice replays succeeded
  and matched their proxy boundary populations.
- The matched mechanics batch completed 48/48 validated runs across four arms;
  raw output and commands remain under ignored
  `LearningIndieDev/artifacts/s6-pf-diag-20261109/`.

## Risks and incomplete work

- First-node failures are too frequent for the agreed onboarding target. The
  exact player offer at tick 100 lacks all three strategy families in 9/12
  portable seeds; a fresh profile's research gate is a separate barrier.
- `larger-minimum-litter` did not raise aggregate early Hare births. Avoidance
  has no direct pre-contact trigger counter. Small sample and chaotic
  trajectories limit route-level Mutation rankings.
- Raw artifacts are local and ignored by Git. The compact summary and exact
  hashes are versioned. Unity generated line-ending-only `ProjectSettings`
  changes during export; they are excluded from the intended commit.

## Next useful step

Josh and Sim should review the [analysis](../Research/Experiments/S6-Predator-First-Diagnostic-20261109/AI_ANALYSIS.md),
then freeze a separate small Fox-start-pressure sweep and a faster-reproduction
mechanism check. A fresh-profile offer/playtest check should follow before any
claim that the three responses are obtainable and understandable.
