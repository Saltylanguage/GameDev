# Salty GameDev Unity Engineering Standards

Status: authoritative project standard  
Scope: `LearningIndieDev` first-party code and assets  
Baseline audited: Unity `6000.4.6f1`, URP `17.4.0`, project revision `0b051c2e5d54`  
Last reviewed: 2026-09-18 (Island Survivor retirement)

This is a practical standard for current first-party code and assets. The player-facing route is `MainMenu -> GalapagOSDesktopTest -> desktop-hosted Simulation`; `Lab` and `CellularAutomataPrototype` remain separate development/test entry points. The Island Survivor slice was removed on 2026-09-18, so its old examples and validator references are no longer applicable. Existing code is not automatically compliant; the adoption plan defines when it should change.

## How to read this document

- **MUST**: required for correctness, safety, or repository consistency.
- **SHOULD**: preferred unless a documented reason exists.
- **MAY**: useful in an appropriate context.
- **MEASURE FIRST**: do not assume this improves performance; capture evidence before adopting it.
- **AVOID**: generally harmful here, with exceptions only when documented.

Every rule below includes its intent, a project-specific example, enforcement status, and exceptions where they matter.

## 1. Project principles

1. **MUST keep the playable loop simple and explicit.** The active product flow is `MainMenu -> GalapagOS Desktop -> desktop-hosted Simulation`, with explicit simulation and Noesis presentation composition. The standalone `Lab -> CellularAutomataPrototype` route is retained for development and testing, not as the main player flow. New work should fit a demonstrated boundary before introducing a new framework.
   - Why: the project is an early prototype and the existing vertical slice is easy to reason about.
   - Correct: extend the simulation through its existing domain and presentation boundaries before changing startup or UI composition.
   - Discouraged: add a global `GameServices` registry so one feature can find inventory.
   - Automatic enforcement: documentation/review only.
   - Exceptions: a measured bottleneck or a new platform requirement may justify a boundary change.

2. **MUST apply KISS, YAGNI, incremental improvement, and "patterns solve demonstrated problems."** Prefer feature-local code over speculative reuse.
   - Why: the project has one compact vertical slice and several empty planned folders under `Assets/Project`.
   - Correct: keep a three-resource rule in its activity until a real shared definition need appears.
   - Discouraged: create a generic gameplay framework for hypothetical future activities.
   - Automatic enforcement: documentation/review only.

3. **SHOULD prefer composition over inheritance and explicit dependencies over global access.** Readability wins over theoretical extensibility; optimization follows measurements.
   - Correct: a component receives its required collaborators through explicit composition or injection.
   - Discouraged: hidden global access or scene-wide object searches for required dependencies.
   - Automatic enforcement: assembly references can be enforced; code choices require review.

## 2. Folder, namespace, and assembly structure

### Current intended structure

The Unity project is `LearningIndieDev`. First-party runtime code lives under `Assets/Scripts/Game` and uses namespace `SaltyGame`. The current feature areas are:

| Path | Ownership and rule |
|---|---|
| `Assets/Scripts/Game/Simulation` | Simulation rules, orchestration, snapshots, and run results. |
| `Assets/Scripts/Game/Species` | Species identity, rules, progression, and authored upgrades. |
| `Assets/Scripts/Game/Presentation` | Simulation and terrain presentation adapters/resolvers. |
| `Assets/Scripts/Game/Scene` | Scene-transition helpers. |
| `Assets/Scripts/Game/Profile` | Profile-session state and snapshots. |
| `Assets/Scripts/Game/Grid`, `CellularAutomata`, `CaveGeneration`, `Terrain` | Grid, cellular, cave-preview, and terrain domain code. |
| `Assets/Editor` | Editor-only simulation tools, asset authoring, and validation windows. |
| `Assets/Tests/Runtime`, `Editor`, `PlayMode` | NUnit/Edit Mode, asset/editor, and Play Mode coverage in separate assemblies. |
| `Assets/Scenes` | Main Menu, Desktop, Simulation, and separate development/test scenes. |
| `Assets/Settings` | URP and project settings assets. |
| `Assets/UI` | Optional/experimental UI code, currently outside `SaltyGame.Runtime`. |
| `Assets/ThirdParty` | Reserved third-party boundary. Do not edit vendor content without an explicit reason. |
| `Assets/Project` | Removed 2026-09-07 as an unused placeholder tree; create owned feature folders only when real assets require them. |
| `Assets/TutorialInfo` | Removed 2026-09-06 as stale Unity template content; do not treat it as a project architecture boundary. |

`SaltyGame.Runtime` currently references `Unity.InputSystem` and has no other first-party assembly dependency. `SaltyGame.Tests` references only `SaltyGame.Runtime` and includes the Editor platform. **MUST** preserve this acyclic direction:

```text
SaltyGame.Tests -> SaltyGame.Runtime -> Unity.InputSystem
SaltyGame.EditorTools -> SaltyGame.Runtime + UnityEditor
Presentation -> core/domain APIs; domain does not depend on presentation
```

New feature assemblies are **SHOULD** be added only when a real compile-time boundary or test isolation need exists. Avoid assembly-per-folder.

**MUST** keep one primary MonoBehaviour, ScriptableObject, or editor type per matching file. File and type names use PascalCase. Namespace segments map to ownership, not merely to the physical folder.

Safe migration map, if structure changes later: `Assets/Scripts/Game/*` remains the source of truth; move code only in a Unity-aware migration with its `.meta` file, asmdef references, GUID validation, and compile/test check. Do not move or rename assets in this documentation task.

Enforcement: assembly dependency direction and file/meta checks can be CI scripts now; folder ownership and namespace mapping are review/documentation until a validator exists. Exception: third-party and Unity template content may retain vendor conventions.

## 3. C# naming and formatting

The dominant first-party runtime convention is namespace `SaltyGame`, PascalCase types, camelCase private fields without a prefix, expression-bodied read-only properties, four-space indentation, braces on control flow, and `var` for obvious local types. Legacy `Assets/UI` uses `_PascalCase` fields and has spelling/convention drift; preserve it until that optional path is deliberately adopted or replaced.

| Item | Standard | Example |
|---|---|---|
| Types/enums | PascalCase | `SimulationManager`, `SpeciesId` |
| Interfaces | `I` + PascalCase | `IReadOnlyList<T>` |
| Methods/properties | PascalCase | `RunSimulation`, `IsComplete` |
| Parameters/locals | camelCase | `deltaTime`, `rewardAmount` |
| Private fields | camelCase, no prefix | `readonly SimulationManager simulation` |
| Serialized fields | private camelCase with `[SerializeField]` | ` [SerializeField] private Transform playerRoot;` |
| Static fields | camelCase; `readonly` where possible | `static bool visible` |
| Constants | PascalCase | `InteractionRange` |
| Events | PascalCase; event past tense when appropriate | `SimulationCompleted` |
| Event handlers | `On` + event name | `OnSimulationCompleted` |
| Booleans | `is`, `has`, `can`, or `should` wording | `CanInteract`, `IsComplete` |
| Generic parameters | `T` or descriptive `TItem` | `TResult` |
| Test methods | descriptive behavior statement | `ThreeStrongHitsCompleteAndAwardThreeWood` |

**MUST** use explicit access control, private serialized fields instead of public mutable Inspector fields, braces for control flow, and one primary type per file. **SHOULD** use properties for simple access and methods for operations, side effects, or meaningful computation. **MUST NOT** introduce unexplained abbreviations, magic numbers, or magic strings; use named constants or a clearly named definition. Comments explain intent, constraints, or lifecycle assumptions, not syntax. XML documentation is **MAY** for public or non-obvious APIs that are reused outside the immediate feature.

Correct: pass required collaborators explicitly through composition or constructors. Discouraged: expose mutable runtime state as a public scene-component field or discover required collaborators through a global search.

Enforcement: formatting whitespace and braces are enforced by `.editorconfig` for new/modified code; naming is warning/review guidance because legacy UI conflicts. Exceptions: vendor/template files and guarded Noesis experiment code may retain their existing style.

## 4. Unity component standards

- **MonoBehaviours MUST** coordinate Unity lifecycle, scene references, and Unity-facing presentation. `CellularAutomataPrototypeRuntime` is an existing scene-runtime example.
- **Plain C# classes SHOULD** own domain/simulation rules that do not require Unity lifecycle. Simulation, species, grid, and terrain rules should remain directly testable where practical.
- **ScriptableObjects MAY** hold authored definitions or configuration once the project has real shared data; they are not required for every feature.
- **Components MUST** have one clear responsibility and explicit ownership of references.
- **Prefabs and scene objects MUST** own composition and serialized references, not hidden game rules. Current prefab usage is TBD; no prefab is currently required by the bootstrap slice.
- **Editor tooling MUST** remain under `Assets/Editor` or an Editor-only assembly and must not leak into player assemblies.

The player-facing composition is `MainMenu.unity` -> `GalapagOSDesktopTest.unity` -> a simulation hosted by the Desktop. `Lab.unity` and `CellularAutomataPrototype.unity` remain separate development/test entry points. **MUST NOT** couple independent scene roots, add a `DontDestroyOnLoad` singleton, or introduce an implicit scene-order dependency without an architecture decision.

`Awake`, `OnEnable`, `Start`, and scene load timing are not interchangeable. Initialization and shutdown ownership must be documented at the component that owns it. Keep generated-object teardown explicit whenever a runtime feature creates Unity objects.

Enforcement: scene composition, duplicate-runtime policy, and lifecycle ownership are covered by relevant tests and review; there is no general bootstrap scene validator. Exception: Unity template/editor content is outside runtime standards.

## 5. ScriptableObject standards

Approved future uses: shared immutable definitions, configuration, item/activity/interaction/world definitions, catalogs, balancing curves, event channels with cleanup/debugging, runtime-set assets with explicit lifecycle, and intentionally pluggable behavior.

**MUST** separate definition data, runtime instance state, and persistent save data. Configuration assets are read-only at runtime unless mutation is intentional and documented. **AVOID** using ScriptableObjects as uncontrolled global mutable state, save files, hidden service locators, or scene-specific runtime state that has no reset/ownership path. Do not rely on Editor play-mode mutations being harmless.

Correct future example: `WoodChoppingDefinition` stores authored health, reward, and timing values; `WoodChoppingActivity` stores health/elapsed runtime state; a future save DTO stores persistent progress. Discouraged: mutate a shared `ResourceCatalog` to hold the current player's inventory.

Current usage: first-party `CellularSimDataAsset`, `ScenarioDefinitionAsset`, species-definition assets, and `SpeciesUpgradeAsset` hold authored definitions; runtime state remains in plain C# objects. Template `Readme` and URP settings are not gameplay architecture. Event channels and runtime sets are **TBD** until a concrete cross-scene use exists.

Enforcement: documentation/review only now; a future asset validator may check naming, folder placement, and mutability conventions. Exception: Unity/package assets follow their owner.

## 6. SOLID and design-pattern policy

Use the smallest pattern that solves a demonstrated problem.

| Pattern | Appropriate here | Warning/signals to avoid | Current status |
|---|---|---|---|
| Factory | A small creation boundary when product variation requires it | Generic factory hierarchy for hypothetical future types | Use only when demonstrated |
| Object Pool | Repeated transient objects after a measured allocation problem | Pooling one-off bootstrap sprites | MEASURE FIRST; not used |
| Singleton | Only one true process service with explicit lifecycle and tests | Convenience global access or hidden initialization | Avoid; not used |
| Service locator | No approved use in current architecture | Hidden dependencies and order coupling | Avoid; not used |
| Command | UI/input actions need queueing, undo, or replay | Wrapper around a direct method call | Optional; experimental `DelegateCommand` is isolated/guarded |
| State | Many explicit transitions make branching unreadable | Enum wrapper with no behavior benefit | `GameState` is a simple enum; no state framework |
| Observer | Decoupled notifications across a real boundary | Events replacing a direct call | Use sparingly; no gameplay event bus |
| MVP/MVVM | Complex UI with independent view state/testing | Reintroducing a runtime UI path outside Noesis | Active Noesis screens use ViewModels; editor-only diagnostic UI remains separate |
| Strategy | Multiple interchangeable rules with real variation | Interface for every class | Add only for demonstrated variation |
| Flyweight | Many shared immutable definitions | Premature data indirection | TBD |
| Dirty Flag | Expensive derived UI/world rebuilds | Flagging cheap direct reads | TBD |

Interfaces are **SHOULD** be used at real substitution or module boundaries, not automatically for every class. Deep inheritance is **AVOID**. Singleton/service-locator exceptions require a written responsibility, initialization owner, shutdown/test strategy, and reason explicit injection is insufficient.

Enforcement: review/documentation only. Assembly references can prevent some dependency mistakes.

### Pattern decision guide

This project follows the applicability-first view in Fireship's [10 Design Patterns Explained in 10 Minutes](https://www.youtube.com/watch?v=tv-_1er1mWI): a named pattern is a response to a repeated problem, not a feature checklist. Before adding one, name the concrete problem, identify the existing direct/simple alternative, and add a focused test for the new behavior.

| Pattern from the video | Project rule and current status |
|---|---|
| Singleton | **Avoid.** A scene composition root is not a global access point. Do not use static debug or gameplay state as a substitute for explicit ownership. |
| Prototype | **Defer.** Use Unity prefab/`Instantiate` copying only when multiple runtime instances genuinely derive from one authored base. Avoid clone abstractions without a demonstrated need. |
| Builder | **Defer.** Explicit composition is not automatically a reusable Builder API. Add a builder only when many optional construction steps make direct construction unreadable. |
| Factory | **Use when creation varies behind a real boundary.** Keep the factory small and avoid hierarchies or registries until variation requires them. |
| Facade | **Use sparingly.** Add a narrow facade only when callers repeatedly need the same multi-system operation; it must not become a hidden service locator. |
| Proxy | **Defer.** Add only for a demonstrated access-control, lazy-load, or instrumentation boundary. Do not wrap ordinary game state just to intercept getters/setters. |
| Iterator | **Use language support.** `IReadOnlyList` plus `foreach` expresses ordinary traversal. Do not write custom iterators until traversal has non-trivial rules that collection APIs cannot express. |
| Observer | **Defer.** A direct call is preferred while ownership is clear. Introduce a typed event only for a real one-to-many notification where the publisher must not know consumers; document subscribe/unsubscribe ownership. No global gameplay event bus. |
| Mediator | **Defer.** Keep coordinators' dependencies visible. Add a mediator only when several peers need to communicate and direct calls create circular or repetitive coupling. |
| State | **Start simple.** Enums are appropriate while transitions are few. Promote to state objects only when each state owns distinct behavior and conditionals obscure the transition rules. |

Pattern names MUST clarify the code's responsibility. A pattern that adds indirection without removing a demonstrated source of coupling, branching, or duplicate construction is rejected.

## 7. Dependency and event rules

Dependencies **MUST** be visible through constructor/method injection for plain C# types, serialized references for authored scene composition, explicit scene composition, narrow interfaces, or documented event channels. Keep domain and simulation rules independent of presentation where a clear boundary exists.

**AVOID** `FindObjectOfType`/`FindAnyObjectByType` dependency discovery, convenience statics, hidden utility dependencies, and circular assembly references. Events are **MAY** be used only when the publisher must not know the consumer. A direct call is preferred when ownership is clear.

Events must use clear PascalCase names and payloads that describe the fact, not an opaque bag. The owner documents subscription timing and always unsubscribes at the matching lifecycle boundary. Event channels require subscriber cleanup and a debugging path. Current project event policy is otherwise **TBD** because no first-party gameplay events exist.

Enforcement: asmdef dependency cycles are machine-checkable; hidden discovery and event ownership are review checks.

## 8. Lifecycle, updates, and timing

- `Awake`: component-local references and bootstrap entry only when required before other `Start` calls.
- `OnEnable`/`OnDisable`: paired subscription ownership; no gameplay state reset without an explicit reason.
- `Start`: deferred initialization only when it genuinely depends on the loaded scene.
- `Update`: input sampling, presentation coordination, or measured per-frame work; do not assume every simulation needs a MonoBehaviour-owned per-frame tick.
- `FixedUpdate`: physics integration only; simulation timing should follow its explicit run contract.
- `LateUpdate`: camera/follow/presentation correction only when ordering requires it.
- Coroutines/async: **MAY** represent asynchronous waits or I/O; ownership and cancellation are required. Do not use them to hide gameplay state transitions.
- Explicit tick/run boundaries: **SHOULD** be used for plain domain systems when they clarify simulation timing and ownership.
- Central schedulers/custom update managers: **MEASURE FIRST**; do not add one for style.

Empty lifecycle methods **MUST** be removed. Avoid per-frame polling when an input edge, event, or explicit command is clearer. Separate physics, simulation, and presentation timing when a feature introduces those distinctions.

Enforcement: simple static checks can flag empty lifecycle methods and `Find*` calls; behavior and timing remain review/test checks. Exception: vendor/template code.

## 9. Performance and profiling policy

No project target frame-time or memory budget is established yet: **TBD**. Before an optimization claim, record target hardware, build/configuration, representative scenario, baseline capture, CPU/GPU bottleneck, memory observations where relevant, and before/after results. Use Unity Profiler first; use Profile Analyzer for captures, Memory Profiler for memory, Project Auditor for configuration/code findings, Frame Debugger for render submission, and `ProfilerMarker` around non-obvious measured regions. Platform-native tools are required when platform evidence is needed.

The following are **MEASURE FIRST**: pooling, custom update managers, ECS conversion, Burst/Jobs rewrites, data-oriented rewrites, replacing readable APIs, complex caches/invalidation, manual memory management, and broad abstraction removal. Avoid recurring managed allocations in confirmed hot paths, but do not rewrite cold or unmeasured code to satisfy a slogan.

Current evidence: no first-party profiler markers, performance tests, or captured budgets were found. The runtime creates a small placeholder world at bootstrap, which is acceptable for the current slice but not evidence for future scale.

Enforcement: performance evidence is required in review for optimization changes; automated performance thresholds are planned, not current. Exception: a release/platform requirement may set a temporary budget with recorded hardware and scenario.

## 10. Asset, prefab, scene, and serialization safety

**MUST** preserve every Unity `.meta` file and serialized GUID. The current tracked asset inventory has matching `.meta` files for all non-meta assets. Text/YAML serialization is enabled (`EditorSettings.m_SerializationMode: 2`) and must remain enabled for reviewable scene/prefab changes.

`MainMenu.unity`, `Lab.unity`, `GalapagOSDesktopTest.unity`, and
`CellularAutomataPrototype.unity` are enabled in Build Settings. Main Menu is
the player entry; the Desktop hosts the player-facing Simulation. Lab and the
standalone Cellular Automata scene remain development/test entry points. The
Island Survivor scene and its validator were removed on 2026-09-18. The unused
starter `Intro.unity` scene and unreferenced `_Recovery/0.unity` snapshot were
removed on 2026-09-07. There is no general bootstrap validator for the current
UI/simulation flow.
Additive-scene policy is otherwise **TBD**. Do not assume Addressables: the
manifest does not include Addressables, so no Addressables standard applies.

Assets belong in the owning feature folder; settings remain in `Assets/Settings`; third-party content remains isolated. Prefab variants are **SHOULD** be used only when the base/variant ownership is clear. Safe moves/renames require Unity Editor migration, `.meta` preservation, reference validation, and a separate commit/plan. Never bulk move/rename during feature work.

### Interactive terrain states

When an interaction gates a route or changes the world, its visual representation **MUST** be authored as a terrain state that shares the neighboring tile grid, scale, palette, and edge treatment. A closed state hides the route; a cleared state reveals the route through the interactable's explicit visual ownership. Do not layer a self-contained prop over unrelated terrain and call it a terrain transition.

For any future interactive terrain gate, keep blocked and cleared visuals on the neighboring terrain grid and let the owning feature control the transition. Current terrain source cells use 64x64 pixels at 64 pixels per unit. Prefer a small local state change over a generic world-state framework until multiple features demonstrate a shared rule.

Save-data architecture and version migration are **TBD** because no save system exists. When introduced, persistent DTOs must be separate from runtime objects and have an explicit version/migration test.

Enforcement: `.meta` parity, YAML mode, enabled bootstrap, and forbidden generated paths can be checked now; asset ownership and serialized-reference correctness need Unity validation. Exception: package/vendor assets.

## 11. UI and presentation boundaries

The active player-facing flow uses Noesis/XAML under `Assets/UI` with direct
Noesis imports and the current generated/editor package resolution. The
terrain diagnostic scene is a temporary exception pending an explicit removal
or Noesis migration decision. Unity Editor utility windows may use editor-only
IMGUI, but no player-facing runtime screen may use it. uGUI and UI Toolkit modules are installed, but no first-party UI
Toolkit runtime screen was found. No scripting define currently guards the
first-party Noesis files; package-resolution and editor analytics remain
documented risks.

Presentation **MUST** display state and send user intent without owning activity, inventory, or world rules. Refresh on meaningful state changes where practical; do not create unnecessary layout/canvas rebuilds. Use the established Noesis presenter/view-model path for runtime screens; do not reintroduce a runtime IMGUI HUD.

Editor UI and runtime UI must remain separate. Runtime UI belongs in Noesis/XAML; editor-only diagnostic windows may use Unity IMGUI.

Enforcement: assembly boundaries and code review; no UI architecture validator currently exists. Exception: the guarded Noesis experiment may retain its separate legacy convention until formally adopted.

## 12. Testing and validation

Plain C# domain systems **MUST** be testable without a production scene whenever practical. The current `WoodChoppingActivityTests` cover activities, controller/inventory delivery, world bootstrap construction, target reset, and clock progression. Tests use NUnit and live in `Assets/Tests/Runtime` under `SaltyGame.Tests`.

Test names describe behavior, not implementation. Fixtures must own setup/teardown and destroy created Unity objects. Add Edit Mode tests for pure rules, serialization/configuration, and migrations; add Play Mode tests for scene composition, lifecycle, input wiring, and presentation integration. Integration/regression tests belong at the narrowest useful boundary. Performance tests are required only for measured performance-sensitive systems.

The project has a Play Mode assembly at
`Assets/Tests/PlayMode/SaltyGame.PlayMode.Tests.asmdef`. Current gaps include
save-data tests (no save system exists), broader serialized-asset validation,
and performance baselines/tests. These are staged requirements, not immediate
refactor work. Historical test totals must cite their source commit and artifact;
do not present a rolling count as current evidence.

Enforcement: Unity Test Framework execution is available when the Unity host is available; current UI/simulation work uses focused Edit Mode and Play Mode suites, while broader coverage and performance gates remain planned. Exception: prototype-only features may start with a focused Edit Mode test or a documented manual validation path.

## 13. Productivity and Editor tooling

For current Main Menu/Desktop/simulation changes, use the focused Play Mode
and Edit Mode checks documented in the relevant plan. Add
custom inspectors, property drawers, validation menus, templates, or build
scripts only for repeated, measurable friction. Development-only diagnostics
such as F3 panel behavior must not become gameplay dependencies. Console logs
use a stable `[Salty]` prefix for editor validation and should be removed,
gated, or downgraded when noisy.

Editor code **MUST** stay out of runtime assemblies. Build scripts and CI checks **SHOULD** be deterministic and report actionable file paths. Project-local templates are **TBD**; do not add them until the naming/field patterns stabilize.

Enforcement: asmdef platform boundaries and existing asset validators; general scene-composition tooling remains a review/documentation concern.

## 14. Version-control workflow

The repository tracks Unity source, settings, scenes, assets, and `.meta` files. `Library`, `Temp`, `Obj`, builds, logs, user settings, IDE output, and generated project files are ignored by the root `.gitignore`; `LearningIndieDev/ignore.conf` contains a compatible Plastic SCM ignore list. Keep both aligned when a new generated directory is introduced.

Use small coherent commits. Commit messages should state the behavior or project concern changed. Keep functional work, broad cleanup, asset migration, package updates, and third-party updates in separate commits. Branch expectations are **TBD** for this Plastic SCM checkout; follow the active team workflow and do not invent a Git-only process. Avoid concurrent edits to the same scene/prefab. Generated files are not committed unless the project explicitly treats them as source.

Package updates require the Unity version and lockfile to be reviewed together; do not update packages as part of ordinary feature work. Large-file policy is **TBD**; confirm whether Plastic SCM or a future remote imposes a limit before importing large media.

Enforcement: ignore rules, meta parity, and diff checks can run automatically; commit/branch/scene conflict policy is review/documentation.

## 15. DOTS adoption gate

Current status: **Not used**. Burst, Collections, and Mathematics appear transitively in `packages-lock.json`, but no first-party DOTS/ECS code or assembly was found. Conventional GameObject/plain C# architecture remains the default.

Jobs and Burst MAY be adopted without full ECS when a measured workload benefits. ECS requires a benchmarked workload, explicit migration boundary, ownership of native collection allocation/disposal, intentional job dependencies/synchronization, and managed Unity object access outside Burst-compatible jobs. Minimize and measure structural changes. Data components contain data. Hybrid GameObject/ECS is valid. **MUST NOT** introduce DOTS for theoretical future scale.

Enforcement: package/code search can flag first-party DOTS introduction for review; benchmark and architecture decisions are documentation/review.

## Enforcement matrix

| Rule family | Status now | Adoption path |
|---|---|---|
| Unity version/package lock | Enforced by committed project files | Review changes together |
| `.meta` parity and ignored generated folders | Enforced by repository checks/ignore files | Add CI execution |
| Runtime/editor assembly direction | Enforced by asmdefs and Unity compile | Add cycle validator if boundaries grow |
| Formatting/braces/whitespace | Enforced now for editor-aware new/modified code via `.editorconfig` | Normalize legacy files gradually |
| Naming/access/serialization conventions | Warning for new or modified code | Migrate touched files only |
| Scene composition | Covered by focused tests/review; no general bootstrap validator | Add a validator only if repeated setup errors justify it |
| Tests | Enforced where existing tests apply | Add Play Mode/save/performance gates by phase |
| Performance claims | Documentation/review now | Add scenario captures and budgets |
| Asset moves/renames | Documentation/review only | Unity migration tooling when needed |
| DOTS adoption | Documentation/review gate | Benchmark before prototype |

## Decisions that remain TBD

Target hardware and frame/memory budgets; save format/versioning; additive scene policy; prefab ownership/variant policy; branch model in Plastic SCM; large-file policy; UI technology after the Noesis experiment; project script templates; gameplay event-channel policy; and whether a second runtime assembly is justified.
