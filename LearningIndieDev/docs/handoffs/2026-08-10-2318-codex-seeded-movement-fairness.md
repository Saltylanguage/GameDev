# Seeded Movement Fairness

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: Codex
- Branch: SpeciesBalanceWork
- Baseline commit: b3ea53f5
- Date: 2026-08-10

## Summary

Removed the remaining deterministic directional drift in species movement while
preserving reproducibility from a run seed. This is ready for Sim to review and
merge into the primary `SaltysFirstBranch` line.

## Changes

- Offset traversal for movement, diet seeking, mate seeking, attacks, and
  reproduction placement now starts at a seeded random offset instead of always
  preferring the first pattern entry.
- Each movement pass now uses a seeded shuffled cell-processing order, preventing
  row-major destination claims from creating a persistent spatial flow.
- Species and Life preview rendering now maps positive grid Y upward on screen,
  matching `Vector2Int.up` semantics.
- Added a regression test covering same-seed determinism and multiple Moore
  movement destinations.

## Decisions and assumptions

- The simulation remains deterministic for the same seed and ruleset; changing
  the seed is expected to change tie-breaking and processing order.
- The existing `GridPattern` offset definitions remain unchanged. Fairness is
  handled by seeded traversal rather than encoding a preferred direction into
  each pattern.
- The row-major iteration used by independent metabolism/starvation stages is
  unchanged because those stages do not claim destinations or select neighbors.

## Validation

- Unity 6000.4.6f1 Edit Mode suite: **65/65 passed**.
- `git diff --check`: passed.

## Risks and incomplete work

- Movement now allocates a shuffled index array per movement pass. This is
  appropriate for the current prototype grid sizes; profile before optimizing
  if larger grids or shorter step intervals become a real requirement.
- The current prototype still uses enum-backed species and hardcoded terrain
  semantics tracked in `CELLULAR_SIM_TODOS.md`.

## Next useful step

Review the directional behavior across several seeds in the prototype scene,
then merge this branch into `SaltysFirstBranch` if the visual distribution is
acceptable.
