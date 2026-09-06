# Legacy prototype audit

Audit date: 2026-08-11

This audit checks whether the retained Island Survivor, cave, and Life paths
can be safely deleted. It does not change scenes, build settings, code, or
Unity GUIDs.

## Findings

| Area | Evidence | Decision |
| --- | --- | --- |
| `IslandSurvivorPrototype` | Enabled in `ProjectSettings/EditorBuildSettings.asset`; scene contains the `GameRuntime` composition root; `IslandSurvivorPlayModeTests` loads and exercises it. | **Deprecated (2026-08-25).** Preserve without further feature work until an explicit archival or deletion task is approved. |
| `CellularAutomataPrototype` | Enabled in Build Settings; scene contains `CellularAutomataPrototypeRuntime`; `CavePreviewPlayModeTests` loads it and verifies the species preview. | Retain. It is the active cellular simulation prototype. |
| `CaveGeneration` | `CaveGeneratorTests` directly exercise `CaveGenerator`, `CaveCell`, and deterministic generation. Repository reference inspection on 2026-08-25 found `CavePreview.cs` referenced only by its own `.meta`, not by a scene, prefab, or asset. | Retain domain code and tests. Remove only the orphan `CavePreview.cs` and `.meta` in a focused cleanup after Unity can run validation. |
| `CellularAutomata/LifeSimulation` | `GridSimulationTests` directly exercise `LifeSimulation` and `LifeCell`. The orphan `LifeSimulationPreview.cs` has now been removed; no scene, prefab, or asset depended on it. | Retain domain/reference code and tests; do not create a preview scene without a concrete experiment. |
| Island Survivor gameplay folders | Scene and Play Mode tests reference the runtime composition root and its world/activity systems. | Deprecated with the prototype; preserve in place and do not extend. |

## Safe-cleanup result

No domain candidate meets the project’s deletion bar. `CavePreview` remains
deferred pending focused Unity validation. The orphan `LifeSimulationPreview`
component has been removed after its reference-inspection gate passed.

## Follow-up candidates

1. When Unity is closed, delete the remaining orphan `CavePreview` with its
   `.meta` in a focused change and run Edit Mode plus the cellular Play Mode
   smoke.
2. Keep Cave and Life domain code/tests until a separate product decision says
   the reference infrastructure is no longer valuable.

## Dormant experiment

Alpha offspring remains a bounded dormant capability. All authored species
assets currently configure zero alpha chance/bonuses, while the rule is threaded
through simulation data, fingerprints, paired runners, and focused tests. Do not
expand it without a named scenario or upgrade, and do not remove it as hygiene;
removal would be a separate architecture change.
