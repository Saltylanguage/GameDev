# multi-cell showcase

[Working state](../WORKING_STATE.md) | Status: local-complete

- Owner: codex
- Branch: unavailable (Git CLI is not installed in this environment)
- Baseline commit: unavailable (Git CLI is not installed in this environment)
- Date: 2026-08-10

## Summary

Refactored the Life prototype to one `Grid<LifeCell>` where each cell carries its
state and temperature, so all rules read and write one atomic generation.

## Changes

- Consolidated empty, Life, plant, and fire states plus temperature into `LifeCell`.
- Replaced the parallel simulation steps with one seeded `LifeSimulation.Step`.
- Added atomic interactions: fire burns out, ignites adjacent plants, and diffuses
  heat while Life follows neighbor rules and empty cells can grow plants.
- Removed the obsolete `HeatCell` and `ElementCell` assets.
- Updated Edit Mode and Play Mode coverage for the single-grid API.

## Decisions and assumptions

- All cell rules read the same previous-generation grid through the existing generic
  `GridSimulation.Step` delegate.
- Per-generation randomness is deterministic from the preview seed.
- The existing preview component and runtime property names remain unchanged to avoid
  a Unity asset rename and unnecessary integration churn.
- Simulation rules remain separate from presentation; no cell inheritance or new
  rules framework was introduced.

## Validation

- `dotnet build SaltyGame.Tests.csproj --no-restore`: succeeded with zero warnings
  and zero errors.
- `dotnet build SaltyGame.PlayMode.Tests.csproj --no-restore`: succeeded with zero
  warnings and zero errors.

## Risks and incomplete work

- The Unity Test Runner was not executed because the project is open in the editor.
- The combined colors and interaction cadence should be visually checked at the
  intended Game view size.
- The generated `SaltyGame.Runtime.csproj` was locally refreshed to remove deleted
  source entries; Unity may regenerate this file normally.

## Next useful step

Open `CellularAutomataPrototype`, confirm the mixed board animates clearly, then run
the Edit Mode and Play Mode suites from Unity Test Runner.
