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

1. **TODO-CS-04** is activated by the alpha-offspring prototype.
2. **TODO-CS-06** now has a first Inspector-authored scenario path.
3. **TODO-CS-07** is the lowest-priority item for now.

TODO-CS-01, TODO-CS-02, TODO-CS-03, and TODO-CS-05 are complete.

The remaining items should be handled in that order unless a concrete use case
changes the dependency or an experiment provides a stronger trigger.

## Current vision and navigation experiment (2026-08-11)

- [x] Add bounded sight as immutable species-rule data: a non-negative vision
  range and an initial intelligence tier.
- [x] Use a reusable Moore-range pattern for sight, read perception exclusively
  from the movement pass's source grid, and use seeded breadth-first search to
  choose a legal one-step route toward visible food or a viable mate.
- [x] Apply the initial priority rule: hungry creatures seek food; a creature
  with intelligence at least one may prioritize a visible mate once it exceeds
  its reproduction energy-transfer requirement.
- [ ] Add scent only as a separate, stateful field/diffusion experiment. It must
  not be treated as delayed vision.
- [ ] Add simulation event output only when two concrete consumers need the
  same discrete event. Such output must be observed after a tick, never mutate
  the simulation through a global bus during the tick.

## Reproducible experiment tooling (2026-08-11)

- [x] Add closed-editor Unity batch commands for Edit Mode/Play Mode tests and
  seeded cellular-simulation reports. The reports record ruleset fingerprints,
  seed ranges, full population histories, final-population summaries, and
  cumulative per-species activity under ignored `artifacts/` output.
- [x] Establish the corrected reproducible default baseline: seeds 1–20 finish
  with herbivores extinct in 20/20 runs, plants surviving in 20/20, and
  carnivores extinct in 3/20. This is an observed imbalance to investigate, not
  an approved target state; use the activity-metrics handoff for exact figures.
- [ ] Add visual captures, per-tick event telemetry, or report charts only when
  a balance/design question needs more than population trajectories and
  cumulative activity. Keep generated artifacts outside the repository unless a
  curated design result is explicitly chosen for source control.

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

### TODO-CS-04 - Extensible custom rule logic

- [x] Add the first focused custom mechanic: `AlphaOffspringRule` applies during
  reproduction, marks a newborn creature as alpha, and adds configured health
  and energy bonuses.
- Implementation: alpha rules live in immutable `CellularSimData`, are keyed by
  `SpeciesId`, use the seeded simulation random source, and are included in the
  ruleset fingerprint. `SpeciesCell.IsAlpha` is preserved through movement,
  feeding, damage, metabolism, crowding, and reproduction-cost updates.
- Boundary: the current prototype is chance plus starting stat bonuses only.
  Special-diet qualification, inheritance, unique-per-pack limits, and vision
  remain separate experiments.
- Guardrail retained: do not introduce a universal event bus, callback registry,
  scripting language, or general plugin framework until another concrete rule
  requires a shared abstraction.

### TODO-CS-05 - Ruleset fingerprints and comparison metadata (Later)

- [x] Record a stable ruleset/data fingerprint alongside each run's seed and
  results.
- Trigger: we begin systematic A/B experiments, replay bug reports, or saved
  run comparisons.
- Implementation: `CellularSimData.Fingerprint` uses the versioned
  `cellular-sim-data-v4` canonical representation and SHA-256. Dictionary
  entries are sorted by stable IDs; numeric values use invariant round-trip
  formatting; patterns preserve offset order; movement speed and the forage
  energy threshold are included.
  Data-backed runs carry the fingerprint into `SimulationRunState` and
  `SimulationRunResult`.
- Validation: fingerprints are stable across dictionary insertion order and
  change when scenario data changes. Runtime and test assemblies build cleanly.

### TODO-CS-06 - Data asset/editor authoring

- [x] Add `CellularSimDataAsset`, an Inspector-authored `ScriptableObject` that
  serializes global values, arbitrary species definitions, grid-pattern offsets,
  full species rules, and alpha-offspring values.
- Implementation: `CreateRuntimeData()` validates/converts the serialized values
  into a new immutable `CellularSimData` snapshot. The asset is not used as
  mutable run state, and the existing code-authored/settings-UI path remains
  intact for rapid iteration.
- Reusable `SpeciesDefinitionAsset` instances own behavior only. Each
  `ScenarioDefinitionAsset.SpeciesEntry` owns the species reference and its
  scenario-specific starting probability; existing authored scenarios were
  migrated without changing their probabilities.
- Existing cell/terrain fields now have concrete behavior: entity age advances
  per tick, destination movement cost scales effective movement speed, and
  resource terrain regrows by `RegrowthPerTick`. Creature/resource layers are
  preserved independently through lifecycle operations.
- Boundary: the first asset path uses the established bare/grass terrain
  defaults. Add serialized custom terrain and a scene-level scenario selector
  only when reusable terrain presets become an actual workflow.

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
