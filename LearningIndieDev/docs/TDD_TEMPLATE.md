# GalapagOS — Technical Design Document

> Status: Working engineering baseline; implementation truth is separated from roadmap intent | Owner: Josh Campbell | Last updated: 2026-09-12 | Engine: Unity 6000.4.6f1

## How to read this document

This is the engineering source of truth for runtime ownership, data contracts, deterministic behavior, scene composition, verification, and known disconnections. The [Game Design Document](GDD_TEMPLATE.md) owns player intent. [`ROADMAP.md`](../ROADMAP.md) owns scheduling; roadmap v2 is the working baseline, but proposed Sprint 3 work is not yet an approved sprint commitment.

- **Implemented** means executable in the current project within the stated route and limits.
- **Connected** means reachable through the canonical player flow, not merely present in code or a legacy scene.
- **Planned** means named in roadmap v2 or an active implementation plan.
- **Open** means no authoritative contract or accepted decision exists yet.

## 1. Technical goals and constraints

### Goals

- Preserve deterministic, same-world simulation across ten phase windows.
- Keep authored Unity assets separate from plain runtime data and mutable run state.
- Keep the dependency direction `View -> ViewModel -> Helper/composition -> Domain` for player UI.
- Present the simulation through Noesis/XAML without making the domain depend on Unity UI.
- Produce phase and expedition evidence that records seed, rules, schedule, ordered Mutations, and validity.
- Connect one player route from profile selection through Desktop, expedition launch, phase choices, results, and return.
- Keep future Genome persistence and migration compatible with immutable launch snapshots.

### Non-goals for the current slice

- Active-expedition disk save/resume.
- Cloud sync, multiplayer, live services, or platform account integration.
- A generalized gameplay-effect framework beyond validated Mutation and Genome needs.
- Broad content authoring or port work before Windows slice acceptance.
- Domain dependence on `MonoBehaviour`, Noesis, scene objects, or mutable `ScriptableObject` state.

### Constraints and current values

| Concern | Contract or current value | Status |
| --- | --- | --- |
| Engine | Unity 6000.4.6f1 | **Current** |
| Player presentation | Noesis/XAML plus Unity composition and a custom board renderer | **Implemented foundation** |
| Target platform | Windows 64-bit Steam, keyboard/mouse | **Committed slice target** |
| Resolution | 1920×1080 target; 1280×720 functional | **Graphics acceptance recorded** |
| Determinism | Seeded run; deterministic for a fixed scenario/ruleset/version/schedule | **Implemented within recorded evidence bounds** |
| Phase schedule | 10 phases × 200 completed ticks, 9 decision boundaries | **Committed target** |
| Preview default | `SpeciesSimulationPreview.phaseLengthTicks = 100` | **Implementation mismatch** |
| Forest Edge size | Checked-in asset 42×20; generator and architecture map 36×20 | **Source conflict** |
| Performance | Outer ten-phase wall duration and peak-memory budget/measurement | **Not closed; CF-6 remainder** |

## 2. Architecture and ownership

### Dependency direction

```text
Noesis View / Unity board surface
        -> ViewModel commands and observable state
            -> scene helper / composition root
                -> immutable launch and authored runtime data
                    -> simulation domain and run state
                        -> snapshots, checkpoints, telemetry, reports
```

Higher layers may project lower-layer state. Lower layers must not reach into views, scene objects, or mutable authoring assets.

### Layer ownership

| Layer | Owns | Must not own | Representative entry points |
| --- | --- | --- | --- |
| Domain/simulation | Tick rules, run state, cells, species behavior, deterministic transitions | Unity UI, scene navigation, mutable assets | `SpeciesSimulation`, `SpeciesSimulationRunner`, `SimulationRunState`, `SpeciesProgression` |
| Runtime data | Immutable scenario/species/upgrade values and stable IDs | Scene objects or live asset mutation | `CellularSimData`, `SpeciesId`, `TerrainId`, `SpeciesUpgradeSnapshot`, `SimulationLaunchRequest` |
| Authoring/assets | Inspector-authored scenarios, species, upgrades, validation inputs | Per-run state or applied progression | `ScenarioDefinitionAsset`, `SpeciesDefinitionAsset`, upgrade assets |
| Composition/helpers | Asset conversion, dependency wiring, scene transitions, profile/launch handoff | Ecological rules or view layout | `SpeciesSimulationPreview`, `Helper_SceneTransition`, `Helper_ProfileSession` |
| Presentation | Commands, display state, board snapshots, result and phase panels | Mutation of domain internals | `VM_MainMenu`, `VM_SimulationBoard`, simulation shell ViewModels, XAML views |
| Tools/reports/tests | Batch runs, evidence export, validation, regression and graphics acceptance | Player-only assumptions | CellSim commands, EditMode/PlayMode tests, report validators |

The project’s detailed dependency and scene diagrams remain in [Game Architecture and Flows](Architecture%20Maps/GAME_ARCHITECTURE_AND_FLOWS.md). If this document and an executable route conflict, record the conflict before changing either one.

## 3. Scene composition and executable routes

### Scenes in build settings

| Scene | Intended role | Current route status |
| --- | --- | --- |
| `MainMenu` | Launch and local profile selection | **Executable** |
| `Lab` | Legacy/developer simulation launcher | **Executable legacy route** |
| `GalapagOSDesktopTest` | Canonical player home and embedded app host | **Executable shell; incomplete context handoff** |
| `CellularAutomataPrototype` | Standalone simulation/prototype | **Executable through legacy Lab route** |
| `IslandSurvivorPrototype` | Historical prototype/reference | **Deprecated; not part of the main flow** |

### Target player route

```text
MainMenu
  -> persistent ProfileSessionSnapshot
  -> GalapagOSDesktopTest
  -> Expedition Planner produces SimulationLaunchRequest
  -> embedded Simulation consumes the request
  -> phase/reward loop
  -> terminal Results + settlement
  -> Desktop using the same profile session
```

### Current canonical route and disconnect

`VM_MainMenu` validates the current profile and asks `Helper_SceneTransition.LoadDesktop` to load `GalapagOSDesktopTest`. The transition validates the supplied profile, but only the scene change survives. The Desktop scene has no `Helper_ProfileSession` owner.

`GalapagOSDesktopNoesisHost.OpenSimulation` then swaps the Desktop to its local simulation view and starts a locally composed preview. It does not consume a `SimulationLaunchRequest`, selected scenario, seed, schedule, or frozen profile progression. This makes the screen executable but disconnects it from profile selection and expedition preparation.

The legacy `Lab -> CellularAutomataPrototype` route does use the immutable `SimulationLaunchRequest` contract. It proves the launch object can work, but it is not the intended player home. The fix should establish one request/session owner and reuse the contract in the Desktop route rather than creating a third launch path.

### Main-flow availability matrix

| Flow segment | Code exists | Player reachable | Correctly connected |
| --- | :---: | :---: | :---: |
| Main Menu -> profile ID/name | Yes | Yes | Limited to ID/name |
| Profile -> Desktop | Yes | Yes | **No; snapshot is not retained** |
| Desktop -> Expedition Planner | Visual concepts/shell elements | No complete planner | No |
| Planner -> immutable launch request | Legacy contract exists | No | No |
| Desktop -> embedded simulation | Yes | Yes | **Local defaults; bypasses request/profile** |
| Running -> pause/resume | Yes | Yes | Yes |
| Running -> speed/zoom | Controls visible | Yes | **No commands bound** |
| Board -> selected cell details | ViewModel method exists | No | **No pointer-to-cell call** |
| Simulation -> Field Notes | Control visible | Yes | **No command/route** |
| Boundary -> choose/Skip/Continue | Yes | Yes in preview | Partial catalog reachability |
| Terminal -> authored outcome/results | Generic shell exists | Yes | **No authored outcome/settlement** |
| Results -> same-profile Desktop | Close route exists | Yes | **No retained session/progression** |
| Desktop -> Gene Lab/history/details | UI affordances exist | Some appear enabled | **Handlers/routes absent** |

## 4. Runtime data model

### Core types

| Type | Responsibility | Mutability/identity |
| --- | --- | --- |
| `SpeciesId` | Stable species identity used by rules and evidence | Value identity; do not replace casually with display strings |
| `TerrainId` | Stable terrain identity | Value identity |
| `SpeciesCell` | Terrain/resource/creature values for one cell | Read-only value copied into next-state grids |
| `CellularSimData` | Plain runtime scenario and species rules | Created from authored assets; independent of live `ScriptableObject` mutation |
| `SimulationRunState` | Absolute tick, current cells, lifecycle, seed, and accumulated run state | Mutable only through the run lifecycle/domain |
| `SpeciesSimulationRunner` | Owns stepping, seed derivation, previous/current cells, checkpoints, and continuation | One runner per continuing expedition |
| `SpeciesProgression` | Ordered applied Mutation state | Changes atomically at valid frozen boundaries |
| `SpeciesUpgradeSnapshot` | Validated immutable effect data | Safe to retain without referencing mutable authoring assets |
| `SimulationLaunchRequest` | Immutable launch context for a new simulation | Must include the chosen scenario/species/schedule/seed/frozen progression contract |
| `ProfileSessionSnapshot` | Current local profile identity | Presently ID/name only; persistence shape is incomplete |
| `SimulationBoardSnapshot` | Immutable presentation projection of a committed board | Consumed by the custom renderer and UI |

### Cell and grid contract

- A simulation step reads from the previous committed cell array and writes to a copied next-state array.
- Systems must not mutate the source grid during decision resolution.
- Terrain, resource/plant, and creature data have distinct semantics even when co-located in one cell value.
- Legal occupancy, depletion, regrowth, feeding, attack, reproduction, and population-limit transitions belong to the domain, not the renderer.
- Presentation receives an immutable snapshot after commit; it must not inspect mutable internals as its primary data source.

### Authoring conversion

`ScenarioDefinitionAsset`, `SpeciesDefinitionAsset`, and upgrade assets are authoring inputs. Composition converts them into plain runtime data or immutable snapshots before a run begins or before a boundary Mutation is applied.

Required invariants:

- A run never mutates a source asset.
- Stable IDs, not asset instance identity or display names, define species and terrain equality.
- Scenario, natural-rule, Genome, Mutation loadout, schedule, and report versions/fingerprints are recorded wherever they affect replay, comparison, or migration.
- Invalid or initialization-only Mutation effects are rejected before purchase/continue, not silently ignored.
- Explicitly placed initial creatures and factory-created cells must agree on initialization effects; the current Seed Pouches behavior has a known explicit-placement inconsistency and must not be described as a newborn or mid-run benefit.

### Effective rules and progression boundaries

Species Simulation:

```text
scenario-authored natural rules
  -> that SpeciesId's frozen active Genome
    -> selected player species' ordered expedition Mutations
```

Biome Simulation:

```text
scenario-authored natural rules
  -> every participating SpeciesId's frozen active Genome
```

Biome Simulations reject Mutation loadouts. Permanent Genome unlocks must be stored separately from active Genome allocations. Neither Genomes nor Mutations may rewrite authored base assets.

Genome application and persistence are **planned, not implemented**. The current executable rule composition includes natural rules and ordered runtime Mutation snapshots only.

### Persistence boundary

The current local profile serialization stores an ID and display name through PlayerPrefs/JSON. It has no explicit save-schema version, wallet, settings contract, unlock collection, active Genome, accomplishments, expedition history, or migration/recovery behavior.

The first production persistence format must:

- be versioned;
- use stable IDs;
- separate permanent unlocks from active allocations;
- define corrupt-save fallback and safe reset;
- migrate or reject incompatible data explicitly;
- save deterministic settlement results without saving an active run unless that separate feature is approved.

## 5. Simulation pipeline

### Exact completed-tick order

`SpeciesSimulation.Step` currently resolves one tick in this order:

1. Copy the committed source cells into the next-state buffer.
2. Resolve aging.
3. Resolve attack cooldowns.
4. Run `SpeciesBehaviorSystem.Update` to determine behavior/intents.
5. Record herbivore exposure for this step.
6. Resolve attacks.
7. Resolve movement.
8. Resolve metabolism.
9. Resolve terrain/resource regrowth.
10. Resolve starvation.
11. Resolve crowding stress.
12. Resolve seed drops.
13. Resolve wilt.
14. Resolve reproduction.
15. Resolve the population limit.
16. Commit the next grid by advancing the run.

Changing this order is a rules change. It requires updated deterministic tests, fingerprints/versions where applicable, and an evidence-impact review.

### Lifecycle

```text
Ready -> Running <-> Paused
            |
            +-> AwaitingDecision -> Running (Continue after Mutation or Skip)
            |
            +-> Complete (terminal result, extinction, or explicit end)
```

Invariants:

- A non-terminal phase boundary is resumable; terminal completion is not.
- Continue retains the same runner, absolute tick, grid, progression, and accumulated evidence.
- A Mutation is validated and applied atomically while frozen, before the next tick.
- Restart/new expedition owns initialization and creates a new run.
- The phase clock is derived from completed ticks, not presentation frames or wall time.
- A terminal event after a completed tick takes precedence over opening another boundary.

### Randomness and deterministic replay

`SpeciesSimulationRunner` derives each step’s seed from `Run.Seed + Run.Tick` and retains the previous cells needed for step resolution. Reproduction requires, at minimum:

- scenario and runtime-data identity/fingerprint;
- base seed;
- simulation/rules version;
- actual board dimensions;
- phase schedule;
- ordered Mutation snapshots and acquisition ticks;
- frozen Genome snapshots once that system exists;
- simulation mode and terminal reason;
- any validity flags or compatibility decisions applied during report loading.

Determinism is contract-bounded. EX-010 does not prove arbitrary scenarios, schedules, upgrade orders, or future rules remain equivalent.

### Consecutive-phase implementation status

CF-1 through CF-5 implement and verify:

- one retained domain run across boundaries;
- explicit lifecycle states and Continue semantics;
- phase-aware accumulated telemetry;
- continuation checkpoints;
- atomic ordered boundary upgrades;
- generic schedule/report support;
- phase and expedition evidence required by EX-010.

CF-6 is partially complete. Documentation, automated lifecycle/evidence coverage, graphics checks, Windows smoke, and the corrected ten-phase Forest Edge/Hare run are recorded. The outer wall-duration and peak-memory measurement remains the open product-acceptance item.

## 6. Mutation, Genome, and offer execution

### Current Mutation runtime

At a valid boundary, the composition layer prepares validated `SpeciesUpgradeSnapshot` values. The player can purchase one visible option or Skip, then Continue. Ordered snapshots affect only later ticks. Initialization-only effects remain launch-only.

Seven Hare upgrade assets are authored and loaded by the preview, but the XAML surface has only three purchase commands. Because the current list is exposed in asset order, only the first three catalog entries are reachable through the player preview. This is executable code with incomplete product reachability, not a completed three-build selection system.

Before the slice claims Trailblazer/Warren/Gardeners support, the offer system must define and test:

- eligibility and exclusion rules;
- how three offers are selected from the catalog;
- duplicate/stacking behavior;
- acquisition cost and insufficient-funds behavior if currency is used;
- the handling of launch-only upgrades;
- whether offers are seeded and reproducible;
- how the UI previews benefits, costs, obligations, and the first effective tick;
- what happens when fewer than three valid offers exist.

### Genome execution boundary

No production Genome state, active allocation, launch freezing, Gene Lab editor, persistence, or migration exists yet. The intended sequence is:

1. Define the smallest Genome contract in S4 planning.
2. Stabilize profile ownership, wallet/settlement, schema versioning, and stable node IDs.
3. Implement one Hare Genome unlock that survives restart and can be activated/deactivated between runs.
4. Freeze active Genome snapshots for every participating species at launch.
5. Prove that authored natural species assets remain unchanged and that replay/report identity includes the frozen configuration.

## 7. Presentation and interaction boundaries

### Noesis/MVVM contract

- XAML owns layout, styling, bindings, and visual states.
- ViewModels expose commands and observable display state.
- Scene hosts/helpers own Unity wiring, view lifetime, and navigation.
- Domain objects are not bound directly to the view.
- Commands must validate lifecycle state and delegate to the owning helper/domain boundary.
- Enabled-looking controls must have a command or be disabled/labeled as unavailable.

### Board rendering and selection

`SpeciesSimulationBoard` renders the immutable `SimulationBoardSnapshot` as a batched custom surface. `VM_SimulationBoard.SelectCell` can project a selected cell, but the board currently handles rendering only; no pointer position is mapped to grid coordinates and no caller invokes selection. Cell inspection is therefore non-executable from the player UI.

The completed connection must define:

- pointer-to-cell mapping under zoom and scaling;
- out-of-bounds and empty-cell behavior;
- selected/hover visual states;
- which current and historical values are player-facing;
- focus and keyboard behavior;
- whether opening detailed Field Notes pauses the run.

### Currently visible but inert UI

The current XAML/Desktop surface includes enabled-looking controls without connected commands or routes:

- simulation speed radio controls;
- simulation zoom buttons;
- Field Notes;
- “Go to Gene Lab”;
- “View Expedition” and “All Expeditions”;
- species collection filters.

These controls should be connected within an approved feature slice or presented as clearly unavailable. Leaving them enabled creates a false executable contract.

### Results and return route

The simulation can enter a terminal state and show a representative results shell, then close back to the Desktop. It does not yet compute/present the authored Forest Edge victory, narrow-survival, defeat, immediate-extinction, accomplishment, unlock, or scientific-data settlement contract. The return route also lacks a retained profile/progression owner.

## 8. Reporting, tooling, and evidence

### Evidence separation

Reports must keep these levels distinct:

- **direct effect:** whether the changed rule executed and when;
- **species outcome:** population, births, deaths, feeding, energy, exposure, attacks, and reproduction effects for the affected species;
- **ecosystem outcome:** resource sustainability, predator/prey pressure, biodiversity, stability, and recovery;
- **phase evidence:** the bounded interval since the last frozen decision;
- **expedition evidence:** the accumulated run across all phases.

An Adaptation Value estimate may support authoring, but it cannot replace recorded local and ecosystem measurements required by [`SG-005 — Upgrade and Ecology Balance`](Studio%20Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md).

### Tooling boundary

CellSim batch/report tools, prediction-input adapters, and research artifacts support deterministic diagnosis and comparison. They are not prerequisites for the player executable unless a sprint explicitly promotes them. Every balance or feature conclusion must name its scenario, ruleset, seed set, schedule, acquisition order, report/schema version, and known limitations.

## 9. Verification status

### Current recorded evidence

| Area | Latest recorded result | Scope caveat |
| --- | --- | --- |
| Unity compilation | Current editor command compiled without errors on 2026-09-12 | Two obsolete-API warnings remain in `LabPlayModeTests` |
| EditMode | 212/212 passed on 2026-09-09 | Not rerun for this documentation update |
| General PlayMode | 21 passed, 1 intentional visual-capture skip, 0 failures on 2026-09-09 | Recorded suite only |
| Graphics | 22/22 passed at 1280×720 plus focused 1920×1080 acceptance | Does not prove interaction or accessibility |
| Consecutive simulation | CF-1–CF-5 and bounded EX-010 accepted | Does not generalize beyond recorded contract |
| Windows build | Development-player smoke recorded complete | Not a release/certification pass |
| Performance | Outer duration/peak memory not recorded | Remaining CF-6 gate |

Current warnings:

- `Assets/Tests/PlayMode/LabPlayModeTests.cs` uses obsolete `FindObjectsByType<Component>(FindObjectsSortMode.None)` API calls in two locations.

This document-only update does not justify claiming fresh Unity test results. The next runtime change touching the launch flow, schedule, phase decisions, or UI commands must run the proportionate EditMode/PlayMode and player-flow checks.

### Required integrated acceptance

The M1 player-flow gate needs evidence that:

1. A selected profile remains owned through Desktop, launch, results, and return.
2. A Forest Edge/Hare expedition launches from the canonical Desktop route without developer-only fields.
3. The actual runtime runs ten 200-tick phases or the design target is explicitly revised.
4. At every non-terminal boundary, Mutation or Skip followed by Continue preserves the same world.
5. Every intended Mutation build has a reachable, valid offer path and a visible effective tick.
6. Extinction and final-phase completion route to an honest outcome and results state.
7. Results return to Desktop without losing the selected profile or applied settlement.
8. Duration, peak working set, and peak private memory are captured for the accepted run.

## 10. Technical risks and open decisions

| Risk or decision | Impact | Required action |
| --- | --- | --- |
| Desktop bypasses `SimulationLaunchRequest` | Player choices, profile, seed, schedule, and future Genome may not reach the run | Establish one Desktop-owned session/launch boundary and remove local-default authority |
| Profile snapshot is discarded on scene load | Results, wallet, Genome, history, and settings have no durable owner | Define session lifetime before adding progression |
| 100-tick preview versus 200-tick target | Product cadence and acceptance evidence disagree | Choose the authority and add a configuration assertion |
| 42×20 asset versus 36×20 generator/map | Regeneration can silently change gameplay and invalidate comparisons | Select one dimension source and test generated asset parity |
| Fern versus `plant` runtime identity | IDs, telemetry, art, authored diets, and future saves may diverge | Decide alias versus migration before persistence |
| Only first three of seven Mutation assets are reachable | Warren/Gardeners builds cannot be evaluated through the player loop | Implement deterministic offer selection/eligibility and coverage |
| Inert enabled-looking controls | UI advertises features that cannot execute | Bind and test them or present them as unavailable |
| Generic terminal result | No reliable win/loss, accomplishment, or settlement contract | Author outcome evaluator and result DTO before persistence |
| Unversioned minimal profile JSON | Future data cannot migrate or recover safely | Introduce schema/version/migration/corrupt fallback with S6 persistence |
| Seed Pouches initialization inconsistency | Placement path changes whether the authored effect applies | Resolve before offering or promoting the upgrade |
| No performance number for full expedition | M1 cannot close its outer runtime gate | Measure duration and memory on the accepted Windows path |
| Obsolete PlayMode API use | Warnings can hide future diagnostic signal | Replace during a scoped test-maintenance change |

## 11. Architecture decisions

| Date | Decision | Consequence |
| --- | --- | --- |
| 2026-09-12 | Treat GalapagOS Desktop as the canonical player home; standalone Lab remains legacy/developer. | New flow work must connect Desktop to the existing launch/domain contracts. |
| 2026-09-12 | Record executable code separately from player-reachable and correctly connected behavior. | Inert controls and bypassed handoffs cannot be reported as completed features. |
| 2026-09-12 | Keep roadmap v2 as a working baseline without treating proposed S3 as approved. | Documentation may describe planned ownership while preserving the review gate. |
| 2026-09-06 | Retain one runner/run across phase boundaries and apply validated immutable upgrades only while frozen. | Continue preserves all ecological state and accumulated evidence. |
| 2026-09-04 | Keep initialization-only effects at launch and active-run disk resume out of the slice. | Boundary rewards cannot silently rebuild/refill the world; persistence remains separate. |

## 12. Authoritative references

- [Game Design Document](GDD_TEMPLATE.md)
- [`ROADMAP.md`](../ROADMAP.md)
- [Game Architecture and Flows](Architecture%20Maps/GAME_ARCHITECTURE_AND_FLOWS.md)
- [Unity MVVM UI Contracts](UNITY_MVVM_UI_CONTRACTS.md)
- [`SG-003 — UI MVVM Architecture`](Studio%20Guidelines/SG-003-UI-MVVM-ARCHITECTURE.md)
- [Consecutive Simulation Flow Plan](CONTINUOUS_SIMULATION_FLOW_PLAN.md)
- [Continuous Simulation Evidence Impact](CONTINUOUS_SIMULATION_EVIDENCE_IMPACT.md)
- [Upgrade System Direction](UPGRADE_SYSTEM_DIRECTION.md)
- [Upgrade Authoring Guide](UPGRADE_AUTHORING_GUIDE.md)
- [Unity Simulation Tooling](UNITY_SIMULATION_TOOLING.md)

## 13. Change log

| Date | Change | Reason |
| --- | --- | --- |
| 2026-09-12 | Replaced the placeholder template with the current architecture, exact tick pipeline, data boundaries, route matrix, and verification state. | Engineering status must distinguish present code from connected player behavior. |
| 2026-09-12 | Recorded profile/launch bypass, phase-length, board-size, Fern/Plant, offer reachability, inert controls, generic results, and persistence gaps. | These are current execution or source-of-truth failures that were missing from the TDD. |
| 2026-09-12 | Aligned planning language with roadmap v2 while preserving the S3 review gate. | Avoid turning provisional planning into an implementation claim. |
