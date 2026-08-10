# three cell type grid proof

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: Codex-Josh
- Branch: codex/grid-expansion
- Baseline commit: 125932e6
- Date: 2026-08-10

## Summary

Proved that the existing generic grid can step three substantially different
cell-state shapes without cell interfaces, inheritance, or rule classes.

## Changes

- Added `GridSimulation.Step<T>` as the single reusable double-buffered stepping
  operation.
- Added binary `LifeCell`, continuous `HeatCell`, and multi-state `ElementCell`.
- Demonstrated Life-like evolution, heat averaging, and fire spreading through
  replaceable rule delegates in focused tests.
- Added explicit deterministic-seed coverage for Life initialization.
- Routed the existing cave simulation through the same generic step.
- Added a deterministic Life-like overlay to `CellularAutomataPrototype` that
  advances in real time alongside the retained cave preview.

## Decisions and assumptions

- `Grid<T>` remains storage-only.
- Cell types contain data only; rules remain caller-supplied behavior.
- A new grid is allocated per generation for correct previous-generation reads.
  Reusable buffers remain deferred until profiling demonstrates a need.

## Validation

- `SaltyGame.Tests.csproj`: build succeeded with zero warnings and zero errors.
- `SaltyGame.PlayMode.Tests.csproj`: build succeeded with zero warnings and zero
  errors.
- Runtime and test assemblies compile with zero warnings and zero errors.
- A batch Unity Edit Mode attempt was blocked by the existing Unity project lock
  and produced no test-results XML; the Unity Test Runner still needs to be run
  after closing the open editor session.

## Risks and incomplete work

- Heat and element behavior remain Edit Mode demonstrations; only Life has a
  scene preview so far.

## Next useful step

Open `CellularAutomataPrototype`, confirm the Life overlay advances beside the
cave, and run the Unity Edit Mode and Play Mode suites.
