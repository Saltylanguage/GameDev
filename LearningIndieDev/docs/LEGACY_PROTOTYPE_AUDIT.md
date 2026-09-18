# Legacy prototype audit

Original audit: 2026-08-11
Closure update: 2026-09-18

**Closure:** The Island Survivor slice was retired and removed on 2026-09-18.
Its scene, runtime code, dedicated tests, validator, and Island Chores runtime
art are no longer in the Unity project. This supersedes the earlier decision to
preserve that slice. The historical findings below explain the prior state;
they are not instructions to restore it.

The original 2026-08-11 audit checked whether the then-retained Island
Survivor, cave, and Life paths could be safely deleted. Its findings below
record that earlier state; the 2026-09-18 closure supersedes the original
Island Survivor retention decision.

## Findings

| Area | Evidence | Decision |
| --- | --- | --- |
| `IslandSurvivorPrototype` | At the original audit, it was enabled in Build Settings, held the `GameRuntime` composition root, and was exercised by Play Mode tests. | **Retired and removed (2026-09-18).** Scene/build entry, runtime slice, dedicated tests, validator, and Island Chores art were deleted together. |
| `CellularAutomataPrototype` | Enabled in Build Settings; scene contains `CellularAutomataPrototypeRuntime`; `CavePreviewPlayModeTests` loads it and verifies the species preview. | Retain as a standalone development/test route; it is not the canonical player-facing Desktop-hosted Simulation. |
| `CaveGeneration` | `CaveGeneratorTests` directly exercise `CaveGenerator`, `CaveCell`, and deterministic generation. The orphan `CavePreview.cs` had no scene, prefab, asset, or code dependency beyond its own test filename. | Retain domain code and tests. The orphan `CavePreview.cs` and `.meta` were removed in the focused 2026-09-06 cleanup. |
| `CellularAutomata/LifeSimulation` | `GridSimulationTests` directly exercise `LifeSimulation` and `LifeCell`. The orphan `LifeSimulationPreview.cs` has now been removed; no scene, prefab, or asset depended on it. | Retain domain/reference code and tests; do not create a preview scene without a concrete experiment. |
| Island Survivor gameplay folders | At the original audit, the scene and tests referenced the runtime composition root and its world/activity systems. | Removed with the retired prototype slice on 2026-09-18. |

## Safe-cleanup result

At the original audit, no domain candidate beyond the presentation orphan met
the deletion bar. `CavePreview` and `LifeSimulationPreview` were removed after
their reference-inspection gates passed. The later Island Survivor retirement
was a separate project decision; cave/Life domain code and tests remain
retained.

## Follow-up candidates

1. Run Edit Mode plus the cellular Play Mode smoke after the next normal Unity
   reimport/graphics validation pass.
2. Keep Cave and Life domain code/tests until a separate product decision says
   the reference infrastructure is no longer valuable.

## Dormant experiment

Alpha offspring remains a bounded dormant capability. All authored species
assets currently configure zero alpha chance/bonuses, while the rule is threaded
through simulation data, fingerprints, paired runners, and focused tests. Do not
expand it without a named scenario or upgrade, and do not remove it as hygiene;
removal would be a separate architecture change.
