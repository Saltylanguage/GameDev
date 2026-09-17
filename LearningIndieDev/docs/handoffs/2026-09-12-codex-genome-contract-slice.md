# Genome contract and authoring skeleton slice

[Working state](../WORKING_STATE.md) | Status: implemented, needs Unity
verification

- Owner: Codex
- Branch: codex/simulation-window-production
- Date: 2026-09-12

## Summary

The first minimal Genome data contract is implemented and connected across the
profile, launch request, and immutable simulation-run boundary. A separate
metadata-only ScriptableObject authoring skeleton now resolves upgrade nodes
and species maps into an immutable catalog snapshot consumed by the Gene Lab
ViewModel. This preserves the approved product decisions without inventing
node effects, costs, or gameplay behavior.

## Contract

- `GenomeContract.Version` is `species-genome-v1`.
- The agreed active-capacity policy is recorded as eight points per species.
- `SpeciesGenomeProfile` stores permanent `UnlockedNodeIds` separately from
  reassignable `ActiveNodeIds`.
- `SpeciesGenomeSnapshot` freezes one species' active IDs and provides a stable
  fingerprint.
- `GenomeSimulationSnapshot` freezes the participating species map and provides
  a deterministic aggregate fingerprint. Missing profile entries become empty
  species configurations when the scenario roster is known.
- `GenomeUpgradeAsset` authors one node's identity, display metadata, target
  species, and prerequisite IDs.
- `SpeciesGenomeMapAsset` authors one species map by supplying an explicit list
  of Genome upgrade assets. The resolved `SpeciesGenomeMapSnapshot` validates
  duplicate IDs, missing prerequisites, target-species mismatches, and cycles.
- `GenomeCatalogProvider` captures those assets into an asset-free
  `GenomeCatalogSnapshot`; `GenomeLabViewModel` binds the selected map and node
  metadata to the Gene Lab surface.

## Connected path

`ProfileSessionSnapshot` exposes Genome profiles and `Helper_ProfileSession`
persists them. `VM_ExpeditionSetup` places the profile snapshot into the
`SimulationLaunchRequest`. `SpeciesSimulationPreview` carries the immutable
snapshot into `SimulationRunState`, `SimulationRunCheckpoint`, and
`SimulationRunResult`, preserving it through run provenance and checkpoint
restore.

## Preserved decisions and deferrals

- Stable `SpeciesId` owns Genome state; player selection does not substitute for
  species identity.
- Active configuration is frozen at launch and is not changed between phases.
- Mutations remain a separate Species-Simulation-only layer.
- Species and Biome scorecards remain separate.
- Node effects, authored node-cost semantics, economy, profile interaction,
  broad role-based behavior, tree-layout polish, and applying Genome effects to
  simulation rules are deferred.
- Named loadouts and Species Mastery remain deferred and non-gating.

## Validation

- `git diff --check` passed.
- Focused EditMode and PlayMode coverage was added for profile separation,
  persistence, deterministic fingerprints, and launch/run propagation.
- Unity batch verification was not available because the Unity editor was
  already open (PID 4760); no new Unity test result is claimed.
- A temporary source-inclusion check compiled the Runtime Genome sources with
  0 errors and 7 existing obsolete-species warnings. A separate temporary
  check compiled the desktop Genome Lab ViewModel and the focused Genome test
  sources with 0 errors; all temporary project files were removed afterward.
- The generated Unity assemblies remain stale because the editor was already
  open, so these supplementary checks do not replace Unity's authoritative
  import, Test Runner, and visual verification.

The new focused coverage includes pure catalog topology tests and Editor tests
for the Hare ScriptableObject fixtures/provider. The scene PlayMode test now
asserts that the Gene Lab window receives the authored Hare node rather than a
hardcoded `GuardedBurrow` state.

## Next step

Run the focused Unity tests with the editor closed and inspect the Gene Lab
scene once Unity has reimported the new assets. Then review and approve the
first authored node/effect catalog and cost semantics before wiring Genome
effects into simulation rules.
