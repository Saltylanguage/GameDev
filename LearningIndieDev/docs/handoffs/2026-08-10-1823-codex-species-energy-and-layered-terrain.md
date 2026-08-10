# species energy and layered terrain

[Working state](../WORKING_STATE.md) | Status: in-progress

- Owner: codex
- Branch: GridDesignWork
- Baseline commit: 20b748c0
- Date: 2026-08-10

## Summary

The species prototype now distinguishes creature entities from map resources.
Grass is a consumable terrain layer rather than an occupied creature cell, which
lets herbivores and carnivores move through it while preserving food on the tile.
Energy and metabolism are now explicit rule values, making the simulation ready
for plant growth and resource-driven population experiments.

## Changes

- Added `EnergyValue`, `Metabolism`, and `CrowdingCost` rule concepts.
- Negative plant metabolism adds energy each tick; plants do not starve from
  creature metabolism.
- Eating transfers the target species' energy value and consumes one unit of
  plant/grass resource energy; the resource disappears at zero.
- Food/reproduction requirement now gates mating from current energy only; the
  neighbor requirement remains the nearby-mate gate.
- Added layered `SpeciesCell` terrain state with `SpeciesCell.Grass(...)`,
  `IsCreature`, `IsPlantResource`, and preservation helpers for movement.
- Updated seeding, movement, attacks, feeding, reproduction, wilt, population
  snapshots, and rendering for grass resources.
- Empty/bare tiles render as brown dirt. Live simulation controls are compact
  and anchored to the bottom-right of the view.
- Added runtime fields and tests for grass passability, plant metabolism, and
  resource/entity separation.

## Decisions and assumptions

- `IsOccupied` remains compatible with legacy explicit `new SpeciesCell(Plant)`
  values, but production seeding and plant offspring use non-occupied grass.
- `FoodReserve` remains the creature food telemetry/resource field; grass uses
  `TerrainEnergy` for its consumable tile energy.
- A negative metabolism means growth/addition for the plant prototype, matching
  the requested photosynthesis convention.

## Validation

- `dotnet build LearningIndieDev.slnx --no-restore`: succeeded with 0 errors and
  the existing `DelegateCommand` unused-event warning.
- `dotnet test LearningIndieDev.slnx --no-build --no-restore`: exited 0 without
  emitting test-run output; Unity Test Runner visual/runtime validation remains
  pending.
- `git diff --check`: passed.

## Risks and incomplete work

- The immediate-mode UI still needs visual confirmation at the target Game view
  resolutions.
- Energy values and metabolism rates are prototype defaults and need balance
  trials with recorded seeds/results.
- The terrain layer currently models grass only; additional terrain/resource
  types should be added when a real use case requires them.

## Next useful step

Run the pushed `GridDesignWork` branch in the prototype scene, verify movement
through grass and energy transfer visually, then record a deterministic balance
trial for the new metabolism/energy defaults.
