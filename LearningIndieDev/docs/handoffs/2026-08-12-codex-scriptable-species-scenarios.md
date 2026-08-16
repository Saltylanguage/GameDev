# Handoff: Scriptable species and scenario pipeline

## Status

Implemented and validated locally on `codex/xaml-migration`. Existing unrelated
working-tree changes were preserved.

## What changed

- Added reusable `SpeciesDefinitionAsset` authoring data with shared rules and
  three role-specific subclasses: plant, herbivore, and carnivore.
- Added `ScenarioDefinitionAsset`, which composes a subset of species assets into
  a fresh immutable `CellularSimData` snapshot at run start.
- Added explicit `SpeciesRole` data so plant/resource behavior is no longer
  identified only by the legacy `SpeciesIds.Plant` key.
- Updated initialization, perception, metabolism, seed drops, reproduction, and
  fingerprinting to preserve custom plant identities and role data.
- Added Unity menu tooling to create the ten requested species assets and three
  scenarios under `Assets/Data/CellularSimulation/`.
- Added a parity command that compares the authored baseline to the legacy
  defaults by fingerprint and final grid for seeds 10100-10104.

## Parity validation

`Validate Baseline Parity` passed: matching ruleset fingerprints and identical
final grids for all five deterministic seeds.

## 20-seed reports

Reports were generated with `CellSim Run -SeedStart 10100 -SeedCount 20` and
converted with `CellSim Report`. The JSON/Markdown artifacts are intentionally
ignored; these are the paths for review:

| Scenario | Player | Player survival | Other final populations | Report |
| --- | --- | ---: | --- | --- |
| ForestEdge | hare | 20/20 (100%) | fern 23.4 avg; fox 5.15 avg, 15% extinct | Superseded schema-2 artifact; source removed after current-code evidence was recorded |
| Wetland | snail | 20/20 (100%) | beetle 246.9 avg; reed 13.65 avg; owl/stoat 5% extinct | `artifacts/cellular-experiment-20260812-211950/analysis.md` |
| OpenRange | deer | 20/20 (100%) | beetle 174.25 avg; fern 87 avg; wolf 20% extinct | `artifacts/cellular-experiment-20260812-212058/analysis.md` |

The first balance pass was rejected because the authored player species went
extinct in 100% of ForestEdge and Wetland runs. The second pass reduced predator
density/reproduction and increased plant reserve/regrowth; all three player
species then survived all 20 seeds. This is a baseline candidate, not a final
balance claim.

## Validation caveats

- Unity batch mode required elevated access because the local Unity cache was
  permission-blocked. Unity also emitted a licensing access-token warning, but
  the compile, asset generation, parity check, and experiments completed.
- The simulation still has a small number of built-in assumptions around grass
  terrain and the first plant species. These are acceptable for this increment;
  general multi-resource support should be a separate task once a concrete
  scenario requires it.
