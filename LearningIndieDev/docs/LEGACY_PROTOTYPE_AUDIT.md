# Legacy prototype audit

Audit date: 2026-08-11

This audit checks whether the retained Island Survivor, cave, and Life paths
can be safely deleted. It does not change scenes, build settings, code, or
Unity GUIDs.

## Findings

| Area | Evidence | Decision |
| --- | --- | --- |
| `IslandSurvivorPrototype` | Enabled in `ProjectSettings/EditorBuildSettings.asset`; scene contains the `GameRuntime` composition root; `IslandSurvivorPlayModeTests` loads and exercises it. | Retain. It is an intentionally siloed vertical slice. |
| `CellularAutomataPrototype` | Enabled in Build Settings; scene contains `CellularAutomataPrototypeRuntime`; `CavePreviewPlayModeTests` loads it and verifies the species preview. | Retain. It is the active cellular simulation prototype. |
| `CaveGeneration` | `CaveGeneratorTests` directly exercise `CaveGenerator`, `CaveCell`, and deterministic generation. `CavePreview` remains a presentation path even though it is not currently attached to the prototype scene. | Retain code and tests; consider removing only the unused `CavePreview` presentation path after a separate scene/test decision. |
| `CellularAutomata/LifeSimulation` | `GridSimulationTests` directly exercise `LifeSimulation` and `LifeCell`; `LifeSimulationPreview` remains an experimental presentation path. | Retain code and tests; no safe deletion candidate. |
| Island Survivor gameplay folders | Scene and Play Mode tests reference the runtime composition root and its world/activity systems. | Retain in the siloed scene. |

## Safe-cleanup result

No candidate meets the project’s deletion bar. Every reviewed domain path is
either scene-backed, test-backed, or deliberately retained experimental code.
Deleting any of these now would remove active coverage or require a separate
product decision about the retained prototype scenes.

## Follow-up candidates

1. Decide whether the standalone `CavePreview` presentation component is still
   useful now that the species preview owns the cellular prototype scene.
2. Decide whether `LifeSimulationPreview` should remain as a code-only experiment
   or receive its own explicitly siloed scene.
3. If either component is removed later, delete its `.cs` and `.meta` together,
   remove or migrate its tests, and validate both enabled prototype scenes.
