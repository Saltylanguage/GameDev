# Cellular-automata foundation

[Working state](../WORKING_STATE.md) | Status: shared

- Owner: team
- Branch: `SaltysFirstBranch`
- Baseline commit: `515bd0ae`
- Date: 2026-08-10

## Summary

Established the generic grid and cellular-automata cave prototype that preceded
the pivot to a cellular-automata roguelike. The retained island prototype now
includes a live cave preview in the `Boostrap` scene.

## Changes

- Added storage-only `Grid<T>` and immutable offset-based `GridPattern`.
- Added deterministic cave initialization and simulation through `CaveGenerator`.
- Added a point-filtered runtime cave preview composed by `GameRuntime`.
- Added focused Edit Mode and Play Mode test coverage.

## Decisions and assumptions

- Gameplay rules and traversal stay outside `Grid<T>`.
- Relative offsets are a reusable basis for neighborhoods and candidate player
  rule effects.
- Cave generation is retained experimentation, not the new core game loop.

## Validation

- Runtime, Edit Mode test, and Play Mode test projects compiled with zero warnings
  and zero errors in the originating session.
- The full Unity Test Runner suites were not executed because Unity already held
  the project lock.

## Risks and incomplete work

- Combat resolution, rule read/write semantics, turn cadence, progression, and
  the smallest playable loop remain undecided.

## Next useful step

Define the smallest interactive simulation that can prove whether buying and
combining directional cell rules is understandable and fun.
