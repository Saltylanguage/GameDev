# vision-navigation

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: josh
- Branch: SaltysFirstBranch
- Date: 2026-08-11

## Summary

Added the first bounded sight, movement-target selection, and deterministic
navigation pass for species. The work intentionally stops before scent, a
general AI framework, or a global event bus.

## Changes

- `SpeciesAwarenessRules` adds immutable vision range and intelligence fields
  to `SpeciesRules`. The ruleset fingerprint is now v3 and includes both.
- Herbivores default to vision 5/intelligence 1; carnivores to 4/1; plants have
  none. The runtime settings UI and `CellularSimDataAsset` expose both values.
- `SpeciesPerception` selects the nearest visible food or mate from the
  read-only movement source grid, with seeded tie-breaking.
- `SpeciesNavigation` is a small seeded BFS that reuses a species's movement
  and interaction patterns, respects impassable terrain and creature blockers,
  and returns only the next legal step.
- `SpeciesSimulation` retains its source/next-grid movement claims and applies
  the initial policy: hungry creatures seek food; intelligence one can choose a
  viable visible mate after meeting the reproduction-energy threshold.

## Validation

- `dotnet build SaltyGame.Runtime.csproj --no-restore` passed.
- `dotnet build SaltyGame.Tests.csproj --no-restore` passed with the existing
  legacy obsolete-API warnings only.
- Added coverage for detecting food through the vision range, routing around an
  impassable tile, and the mate-over-food intelligence priority. Run these in
  Unity's Test Runner for execution coverage; the generated CLI project only
  compiles the Unity NUnit tests.

## Deferred deliberately

- Scent needs its own stateful/diffusing field; it is not a sight timer.
- Do not add a global message bus yet. If two systems later need the same
  outcome, emit a discrete post-tick event and keep all mutation inside the
  simulation step.
- A visibility overlay and richer priority scoring belong to a concrete design
  experiment, not this first mechanics pass.

## Next useful step

Use the new Inspector or settings fields to A/B fixed seeds and determine
whether sight ranges and the food/mate priority produce useful population
behavior. The planned ScriptableObject/preset authoring work can build directly
on the new serialized awareness fields.
