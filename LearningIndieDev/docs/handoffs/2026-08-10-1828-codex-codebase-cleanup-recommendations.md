# Codebase cleanup recommendations

Status: recommendations-only audit  
Branch: `GridDesignWork`  
Baseline: `cf53ed27` (`Document species energy and terrain handoff`)  
Scope: first-party Unity runtime, tests, and project documentation. No runtime code, assets, scenes, packages, or tracked settings were changed for this audit.

## Executive recommendation

Spend the next cleanup effort on the species simulation vertical slice, in small passes:

1. Establish a presentation boundary around `SpeciesSimulationPreview`.
2. Extract the independent simulation stages from `SpeciesSimulation` while preserving its public stepping API.
3. Simplify the `SpeciesRules` and `SpeciesCell` construction seams after behavior stabilizes.
4. Only then audit and remove inactive Island Survivor/cave/life prototype code using reference and scene evidence.

This order addresses the code that is changing most often and has the highest coupling, while avoiding speculative framework work.

## Ranked recommendations

| Priority | Area | Evidence | Recommendation | Effort / risk |
| --- | --- | --- | --- | --- |
| R0 | Species preview boundary | `Assets/Scripts/Game/Presentation/SpeciesSimulationPreview.cs` is 51,215 bytes / 1,213 lines. One `MonoBehaviour` owns immediate-mode UI, settings editing, `PlayerPrefs` persistence, run/session state, seeding and population caps, rendering, and simulation controls. | Extract the smallest stable seams first: a settings persistence service/DTO, a seed/grid initializer, and a run presenter/controller. Keep the existing `MonoBehaviour` as the composition point until tests cover behavior. | Medium-high / medium. Highest expected payoff; serialized scene wiring and UI behavior need careful validation. |
| R0 | Simulation stage coupling | `Assets/Scripts/Game/Simulation/SpeciesSimulation.cs` is 32,432 bytes / 869 lines. `Step` sequences attacks, movement, metabolism, starvation, crowding, seed drops, wilt, reproduction, and population limiting through one static class. | Retain `SpeciesSimulation.Step` as the stable orchestration API, but move each independent stage into focused pure helpers or small rule-stage classes. Do not introduce a general simulation engine or event bus. | Medium / medium. Behavior-order regressions are the main risk; add generation-level tests before extraction. |
| R1 | Rules construction API | `SpeciesRules` has a 138-line positional constructor and a broad set of optional movement, combat, diet, reproduction, energy, wilt, crowding, and seed parameters. | Once baseline behavior is stable, replace the long call shape with a small configuration object or grouped immutable values. Migrate callers incrementally and keep defaults in `SpeciesRuleDefaults`. | Medium / medium. Improves readability and prevents argument-order mistakes, but should wait until the rule set stops changing weekly. |
| R1 | Cell semantic compatibility | `SpeciesCell` supports both the legacy `new SpeciesCell(Plant)` resource representation and explicit `Grass(...)` terrain resources, alongside creature occupancy and energy mutation helpers. | Migrate production/tests to explicit terrain-resource and entity construction. After call-site and scene coverage confirms safety, remove the legacy compatibility path. | Low-medium / medium. Good simplification, but deletion should follow a reference audit rather than happen during unrelated balancing work. |
| R1 | Parallel prototype models | The project currently contains generic `Grid<T>`, `GridPattern`, `LifeCell`/`LifeSimulation`, `CaveCell`/`CaveGenerator`, and `SpeciesCell`/`SpeciesSimulation` concepts. | Do not unify these models preemptively. Document which are active, experimental, or siloed; add a shared abstraction only when two active systems have a concrete duplicated requirement. | Low now / high if done speculatively. A premature common cell hierarchy would make the current experiments harder to change. |
| R1 | Legacy feature inventory | Island Survivor and earlier cave/life paths remain under `Activities`, `Camp`, `World`, `Survival`, `Progression`, `Inventory`, `CaveGeneration`, and `CellularAutomata`. | Run a separate dependency/scene/build-settings audit. Remove only code with no scene, assembly, test, or runtime references; preserve the siloed scenes until that evidence exists. | Medium / high if rushed. Unity references and `.meta`/GUIDs make blind deletion unsafe. |
| R2 | Test organization | `Assets/Tests/Runtime/SpeciesDomainTests.cs` is 22,984 bytes / 543 lines and covers many species behaviors in one file. | Split by behavior only where it improves navigation, and extract a small fixture/builder only for repeated setup. Keep tests close to the rule semantics; avoid creating a test framework. | Low / low. Useful maintenance pass after simulation-stage boundaries exist. |
| R2 | Result model size | `SimulationRunResult.cs` is already a multi-purpose result/snapshot/state file (7,923 bytes). | Leave it alone unless it continues to grow or callers need independent lifetimes. If that happens, split result status, immutable snapshot, and mutable run state with tests. | Low / low. Not worth interrupting the active simulation work today. |

## Suggested implementation passes

### Pass 0 - ownership and dependency map (read-only)

- Identify the scene object that owns `SpeciesSimulationPreview` and every serialized reference to it.
- Record all callers of `SpeciesSimulation.Step`, `SpeciesRules`, and `SpeciesCell` constructors.
- Mark each legacy prototype folder as active, experimental, or unreferenced.
- Capture a baseline run with a deterministic seed so later extractions can be compared.

### Pass 1 - presentation boundary

- Move persistence and settings DTO conversion out first because they have the least simulation knowledge.
- Move initial-grid seeding next.
- Leave UI layout and public button methods in the existing component until behavior is covered.

### Pass 2 - simulation stages

- Add focused tests for stage ordering and previous-generation reads.
- Extract movement, attack/feeding, resource/energy, reproduction, and population-limit stages one at a time.
- Compare deterministic snapshots after every extraction.

### Pass 3 - API simplification

- Replace the positional `SpeciesRules` constructor with an explicit configuration shape.
- Remove `SpeciesCell` compatibility only after all references use the explicit terrain/entity model.

### Pass 4 - legacy cleanup

- Use `rg` plus Unity scene/build-settings references to prove a candidate is unused.
- Delete in isolated commits, retaining `.meta` consistency and validating both prototype scenes.

### Pass 5 - test/navigation cleanup

- Split oversized test files by behavior if that improves failure locality.
- Add lightweight regression coverage for deterministic seeds, energy/resource consumption, reproduction gates, and pause/stop/restart state transitions.

## Deliberately out of scope

- Do not normalize `Assets/UI`, Noesis, or Unity template content as collateral cleanup. The project standards explicitly classify those paths as optional, guarded, vendor, or template code.
- Do not introduce DI, a gameplay event bus, a new cell inheritance hierarchy, ScriptableObjects for every rule, Addressables, DOTS/ECS, or a generalized simulation pipeline without a concrete requirement and a measured benefit.
- Do not claim a performance problem or optimize allocations until a representative run is profiled.
- Do not move or rename Unity assets in this pass; any such change needs a Unity-aware migration with `.meta`/GUID and scene validation.

## Outcome

The highest-value cleanup is a controlled reduction of coupling around the active species prototype, not a repo-wide rewrite. The recommended first implementation task is Pass 0 plus the first small extraction from `SpeciesSimulationPreview`; everything else should follow evidence from that boundary.

No runtime tests were run because this pass intentionally made no runtime changes. The only new artifact is this report.
