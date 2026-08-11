# CellularSimData first integration

Status: local implementation in progress  
Branch: `GridDesignWork`  
Previous pushed planning commit: `a4426806` (`Document cellular simulation architecture plan`)

## What changed locally

- Added `CellularSimData` as an immutable scenario definition for the current
  species simulation: dimensions, run timing, population limits, starting
  probabilities, and species rules.
- Added copy-on-edit helpers for changing rules, adding a species definition,
  changing starting probabilities, and removing a species without mutating the
  source data.
- Added `SpeciesInitialGridFactory` so deterministic initial population seeding
  is no longer owned by the UI `MonoBehaviour`.
- Added a `SpeciesSimulation.Step` overload and a `SpeciesSimulationRunner`
  constructor that consume `CellularSimData`.
- Migrated `SpeciesSimulationPreview` to create a run-start data snapshot and
  use the factory/runner. The existing dictionary-based runner and step API
  remain for compatibility.
- Added Edit Mode coverage for copy-on-edit behavior, runner injection, and
  same-seed initial-grid determinism.

## Decisions and boundaries

- Runtime state remains in `SimulationRunState` and `SpeciesProgression`; it is
  not stored in `CellularSimData`.
- The first pass still uses `SpeciesArchetype` and the current plant/grass
  semantics. Dynamic species IDs, generalized terrain definitions, arbitrary
  rule plugins, and ruleset fingerprints remain tracked in
  [`CELLULAR_SIM_TODOS.md`](../CELLULAR_SIM_TODOS.md).
- The existing public simulation overloads are preserved to reduce integration
  risk while callers migrate.

## Validation

- `git diff --check` passes for the changed implementation files.
- Unity Edit Mode tests were retried with Unity `6000.4.6f1` after the project
  unlocked, but Unity crashed before tests started while opening the shared
  `CurlRequestCache.db` under the user cache directory. No test result file was
  produced; no existing Unity process was terminated.

## Next step

Run the Edit Mode suite with the project editor closed or use the existing Unity
editor's test runner. After compilation/tests pass, commit this implementation
separately from unrelated UI, scene, and ProBuilder changes.
