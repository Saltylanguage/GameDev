# Production roadmap

[Working state](../WORKING_STATE.md) | Status: ready for review

- Owner: codex
- Branch: codex/xaml-migration
- Baseline commit: 9d7d5300
- Date: 2026-08-13

## Summary

Replaced the retired Island Chores roadmap with an outcome-based production
roadmap for the cellular-automata roguelike. The plan connects upgrades,
species, player/dev UI separation, art, audio, tools, and meta-progression to a
single vertical-slice gate instead of treating them as independent feature
backlogs.

## Changes

- `ROADMAP.md` now defines seven parallel workstreams, milestones M0-M4, six
  initial delivery sprints plus a short Sprint 0, and explicit exclusions from
  the first slice.
- `PROJECT_CONTEXT.md` records the transition from broad prototyping to focused
  production planning.
- `WORKING_STATE.md` links directly to the active roadmap and sprint plan.

## Decisions and assumptions

- The default planning cadence is two weeks; Sprint 0 is shorter. Calendar dates
  remain unset until owner capacity and the product brief are known.
- The first slice uses a deliberately small subset of the existing species
  library and roughly 6-10 explicit upgrades.
- The current simulation settings become a Dev Lab workflow, while normal play
  hides raw tuning fields.
- Art and audio begin during the slice because both are required for simulation
  readability.
- Custom tooling is justified by repeated use cases. The existing scenario
  assets and `CellSim` commands remain the first option.
- Colony construction remains a future research experiment, not slice scope.

## Validation

- `git diff --check` passed for the working tree.
- Documentation diffs were reviewed against the current branch, recent history,
  project context, current Noesis handoff, upgrade implementation, and existing
  authoring/telemetry direction.
- No Unity tests were run because this task changed documentation only.

## Risks and incomplete work

- The exact player action cadence, run-ending conditions, reward timing, target
  platform, slice roster, and three target builds still require the Sprint 0
  product brief.
- Sprint estimates are sequencing assumptions, not delivery commitments.
- The working tree already contained unrelated art, board-rendering, project
  context, and scratch-pad edits; they were preserved.

## Next useful step

Complete Sprint 0 by writing the one-page product brief and naming the selected
scenario, species roster, three intended builds, run-end conditions, and reward
cadence.
