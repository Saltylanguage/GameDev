# Opposed-roll checkpoint restore default

[Working state](../WORKING_STATE.md) | Status: complete

- Handoff schema: 1
- Handoff ID: 2026-09-18-1458-codex-opposed-roll-checkpoint-default
- Owner: Codex
- Branch: Production/ProjectCleanup
- Baseline commit: 963c16a9
- Date: 2026-09-18
- Supersedes: none

## Summary

The opposed-roll migration had left one runtime API defaulting to legacy fixed
damage: `SpeciesSimulationRunner.RestoreCheckpoint`. Its default now matches
the other simulation entry points. Legacy fixed damage remains supported as
an explicit compatibility and historical replay mode.

## Changes

- Changed the optional combat mode on `RestoreCheckpoint` to
  `SpeciesCombatResolutionMode.OpposedRoll`.
- Added a regression test that checks the method's optional default, so a future
  default regression fails directly.
- Updated the parallel-build research pack to record the current default and
  the explicit legacy replay path.
- Confirmed that `SimulationRunCheckpoint` does not retain combat mode. Callers
  replaying historical legacy runs must pass `LegacyFixedDamage` explicitly.

## Validation

- Focused EditMode test `RestoreCheckpointDefaultsToOpposedRollCombat`: 1/1
  passed in a temporary isolated Unity project copy. The original project was
  unreachable/locked, so it was left untouched.
- Retained test output: [EditMode test artifacts](../../artifacts/legacy-combat-default-20260918/).
- The full test suite was not rerun for this one-parameter default correction.

## Risks and next step

This is ready to be included before integration back to `ProjectMain`. Existing
historical checkpoint data has no serialized combat-mode field; replay code
must select the mode explicitly when it needs legacy fixed-damage behavior.
