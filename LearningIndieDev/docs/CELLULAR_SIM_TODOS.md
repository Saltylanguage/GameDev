# Cellular simulation deferred work

These are intentional deferrals for the `CellularSimData` direction. They are
tracked here so we can keep the first implementation small without losing the
larger plan. Each item has a trigger that should bring it back into scope.

## Active now

- [x] Create the first `CellularSimData` aggregate for the current species game:
  global settings, starting population settings, species rules, validation, and
  copy-on-edit behavior. General terrain data remains deferred in TODO-CS-03.
- [x] Inject the data snapshot into `SpeciesInitialGridFactory` and the
  simulation runner so the same seed can be compared under different rulesets.
- [x] Keep runtime state (`Grid`, tick, elapsed time, population history,
  progression, and currency) outside the scenario data.

## Roadmap priority (2026-08-11)

1. **TODO-CS-04 and TODO-CS-06** remain later roadmap work and require a
   concrete mechanic or authoring workflow to activate them.
2. **TODO-CS-07** is the lowest-priority item for now.

TODO-CS-01, TODO-CS-02, TODO-CS-03, and TODO-CS-05 are complete.

The remaining items should be handled in that order unless a concrete use case
changes the dependency or an experiment provides a stronger trigger.

## Completed foundational work

### TODO-CS-01 - Replace enum species identity (Priority 1)

- [x] Replace `SpeciesArchetype` with stable data-driven species IDs.
- Trigger: the project needs to add or remove a species without changing and
  recompiling the enum and its `switch` statements.
- Implementation: `SpeciesId` is now the primary identity for rules, cells,
  simulation, initialization, results, and UI settings. `SpeciesArchetype`
  remains only as an obsolete compatibility shim so existing callers can
  migrate incrementally.
- Boundary: population snapshots still expose the original three aggregate
  counters; that is intentionally deferred to TODO-CS-02. Resource/terrain
  semantics still special-case the plant ID until TODO-CS-03.

## Deferred until a concrete use case requires it

### TODO-CS-02 - Generalize population metrics (Priority 3)

- [x] Replace hardcoded plant/herbivore/carnivore counters in run snapshots with
  species-keyed metrics while preserving useful aggregate counts such as empty
  cells.
- Trigger: a fourth species or species deletion is required, or analysis needs
  arbitrary per-species graphs.
- Implementation: `SpeciesPopulationSnapshot.Counts` now stores read-only
  `SpeciesId` keyed counts, `GetCount` returns zero for missing species, and the
  existing Plants/Herbivores/Carnivores properties remain compatibility accessors.
  Resource terrain cells are counted under their resource species, including
  custom species, while empty cells remain a separate aggregate.
- Validation: runtime and test assemblies build successfully; coverage includes
  a custom creature, custom resource species, built-in plant/carnivore cells,
  and an empty cell.

### TODO-CS-03 - Data-driven terrain registry (Priority 2)

- [x] Move beyond the current bare/grass assumptions to a terrain definition
  registry with stable IDs, color/presentation data, passability, resource
  values, and regrowth settings.
- Implementation: `TerrainId` and `TerrainDefinition` now live in
  `CellularSimData`; `SpeciesCell` stores terrain identity separately from its
  occupant and exposes passability, movement cost, and terrain-resource state.
  Existing Bare and Grass behavior is preserved, and movement honors terrain
  passability.
- Boundary: Sand is not added as gameplay content yet. Its movement-cost shape
  is supported for a later definition, but movement-cost slowing is not active
  until a real terrain needs it.

### TODO-CS-04 - Extensible custom rule logic (Later)

- [ ] Add composable rule/stage code for mechanics that cannot be represented by
  data values and patterns alone.
- Trigger: a new mechanic needs behavior that cannot be expressed by the current
  simulation stages and parameters.
- Reason deferred: delegates, serialized callbacks, or a general rule plugin
  framework would make determinism, testing, and Unity serialization harder.
- Guardrail: add one focused rule seam for the real mechanic; do not introduce a
  universal event bus or scripting system.

### TODO-CS-05 - Ruleset fingerprints and comparison metadata (Later)

- [x] Record a stable ruleset/data fingerprint alongside each run's seed and
  results.
- Trigger: we begin systematic A/B experiments, replay bug reports, or saved
  run comparisons.
- Implementation: `CellularSimData.Fingerprint` uses the versioned
  `cellular-sim-data-v1` canonical representation and SHA-256. Dictionary
  entries are sorted by stable IDs; numeric values use invariant round-trip
  formatting; patterns preserve offset order. Data-backed runs carry the
  fingerprint into `SimulationRunState` and `SimulationRunResult`.
- Validation: fingerprints are stable across dictionary insertion order and
  change when scenario data changes. Runtime and test assemblies build cleanly.

### TODO-CS-06 - Data asset/editor authoring (Later)

- [ ] Decide whether `CellularSimData` should be authored as a Unity asset,
  runtime code, or both, with a clear separation between definitions and run
  state.
- Trigger: designers need reusable named scenarios, source control-friendly
  presets, or cross-scene sharing beyond the runtime settings screen.
- Reason deferred: the current settings UI and plain C# defaults are faster to
  iterate while the schema is unsettled.
- Guardrail: do not introduce ScriptableObjects as mutable global runtime state.

### TODO-CS-07 - Legacy prototype cleanup (Lowest priority)

- [x] Audit and, where proven unused, remove or further isolate earlier Island
  Survivor, cave, and Life prototype paths.
- Trigger: a dependency/scene/build-settings audit confirms a candidate has no
  active references.
- Result: no safe deletion candidate was found. The Island Survivor and cellular
  prototype scenes are enabled and covered by Play Mode tests; cave and Life
  domain code is covered by runtime tests. The audit is recorded in
  [`LEGACY_PROTOTYPE_AUDIT.md`](LEGACY_PROTOTYPE_AUDIT.md).
- Required follow-up: delete in isolated commits and validate both retained
  prototype scenes.

## Rules for revisiting this list

- A trigger is required before promoting a deferred item into implementation.
- When an item is started, add a dated handoff note describing the decision and
  update or close the item here.
- Do not solve multiple deferred items opportunistically in a balancing or UI
  change unless their dependencies are explicitly recorded.
