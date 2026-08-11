# Terrain IDs

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: Codex
- Branch: NF/TerrainIDs
- Baseline commit: 75379113
- Date: 2026-08-11

## Summary

Terrain identity is now separate from species/entity identity. The existing
Bare and Grass behavior is preserved while scenario data can carry terrain
definitions and cells expose the metadata needed for future terrain mechanics.

## Changes

- Added stable `TerrainId` values and `TerrainDefinition` metadata for
  passability, movement cost, resource behavior, regrowth, and presentation.
- Added default Bare and Grass definitions and injected the terrain registry
  into `CellularSimData` snapshots.
- Refactored `SpeciesCell` to store terrain identity separately from its
  occupant, while retaining compatibility properties for the old enum APIs.
- Updated initial seeding and simulation-generated grass to use the configured
  Grass definition.
- Movement now honors terrain passability.
- Added regression coverage for custom slower passable terrain and custom
  terrain definitions in simulation data.

## Decisions and assumptions

- Sand is intentionally not added as gameplay content yet. Its definition
  shape is supported through movement cost, but movement-cost slowing remains
  inactive until a real terrain requires it.
- Bare and Grass remain required baseline definitions so existing simulation
  behavior and authored content remain safe.
- Terrain presentation color is defined in the registry, while the current
  preview retains its existing player-configurable color overrides.

## Validation

- Runtime and test projects compiled successfully through the generated
  project files with the new terrain sources included (0 errors; existing
  obsolete-enum compatibility warnings remain).
- Unity Edit Mode execution was not rerun because the Unity editor instance is
  still active; run the full Unity suite after closing it.

## Risks and incomplete work

- The generated `SaltyGame.Runtime.csproj` was only patched temporarily for
  local compilation and restored afterward; Unity should regenerate it when it
  imports the new scripts.
- Movement cost is metadata only in this pass; a future Sand experiment should
  add cost-aware movement and balancing tests together.

## Next useful step

Next planned work is TODO-CS-02: replace the fixed plant/herbivore/carnivore
population counters with species-keyed metrics while retaining useful
aggregate counts such as empty cells.
