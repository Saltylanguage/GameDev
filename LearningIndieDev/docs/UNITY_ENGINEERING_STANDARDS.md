# Salty GameDev Unity Engineering Standards

Status: authoritative project standard  
Scope: `LearningIndieDev` first-party code and assets  
Baseline audited: Unity `6000.4.6f1`, URP `17.4.0`, project revision `0b051c2e5d54`  
Last audited: 2026-08-02

This is a practical standard for the current island-survival prototype. It is intentionally small enough to use during feature work. Existing code is not automatically compliant; the adoption plan defines when it should change.

## How to read this document

- **MUST**: required for correctness, safety, or repository consistency.
- **SHOULD**: preferred unless a documented reason exists.
- **MAY**: useful in an appropriate context.
- **MEASURE FIRST**: do not assume this improves performance; capture evidence before adopting it.
- **AVOID**: generally harmful here, with exceptions only when documented.

Every rule below includes its intent, a project-specific example, enforcement status, and exceptions where they matter.

## 1. Project principles

1. **MUST keep the playable loop simple and explicit.** The current loop is `GameRuntime -> WorldRuntime -> InteractionController -> ActivityController -> InventoryState -> GameHud`. New work should fit a demonstrated boundary before introducing a new framework.
   - Why: the project is an early prototype and the existing vertical slice is easy to reason about.
   - Correct: add a new activity through `IActivity` and `IActivityTarget` before changing bootstrap or UI.
   - Discouraged: add a global `GameServices` registry so one feature can find inventory.
   - Automatic enforcement: documentation/review only.
   - Exceptions: a measured bottleneck or a new platform requirement may justify a boundary change.

2. **MUST apply KISS, YAGNI, incremental improvement, and "patterns solve demonstrated problems."** Prefer feature-local code over speculative reuse.
   - Why: the project has one compact vertical slice and several empty planned folders under `Assets/Project`.
   - Correct: keep a three-resource rule in its activity until a real shared definition need appears.
   - Discouraged: create a generic gameplay framework for hypothetical future activities.
   - Automatic enforcement: documentation/review only.

3. **SHOULD prefer composition over inheritance and explicit dependencies over global access.** Readability wins over theoretical extensibility; optimization follows measurements.
   - Correct: `PlayerController` receives `Transform`, `PlayerInputAdapter`, and `ActivityController` in its constructor.
   - Discouraged: `PlayerController.Instance` or `FindAnyObjectByType<InventoryState>()`.
   - Automatic enforcement: assembly references can be enforced; code choices require review.

## 2. Folder, namespace, and assembly structure

### Current intended structure

The Unity project is `LearningIndieDev`. First-party runtime code lives under `Assets/Scripts/Game` and uses namespace `SaltyGame`. The current feature areas are:

| Path | Ownership and rule |
|---|---|
| `Assets/Scripts/Game/Core` | Bootstrap, state, and clock. Owns startup sequencing. |
| `Assets/Scripts/Game/World` | World objects, interactables, and player-facing world construction. |
| `Assets/Scripts/Game/Input` | The only current layer that reads the Input System keyboard. |
| `Assets/Scripts/Game/Interaction` | Target selection and interaction intent. |
| `Assets/Scripts/Game/Activities` | Plain C# activity rules and results. |
| `Assets/Scripts/Game/Inventory` | Plain C# resource state. |
| `Assets/Scripts/Game/Presentation` | Runtime HUD and feedback. |
| `Assets/Scripts/Game/Debug` | Development diagnostics; no game rules. |
| `Assets/Editor/EditorTools` | Editor-only validation and menus; namespace `SaltyGame.EditorTools`. |
| `Assets/Tests/Runtime` | Current NUnit/Edit Mode tests; assembly `SaltyGame.Tests`. |
| `Assets/Scenes` | Scene assets. `Boostrap.unity` is the current composition scene. |
| `Assets/Settings` | URP and project settings assets. |
| `Assets/UI` | Optional/experimental UI code, currently outside `SaltyGame.Runtime`. |
| `Assets/ThirdParty` | Reserved third-party boundary. Do not edit vendor content without an explicit reason. |
| `Assets/Project` | Planned authoring structure; do not treat empty folders as implemented architecture. |
| `Assets/TutorialInfo` | Unity template content; preserve or remove only as a separate cleanup decision. |

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
| Types/enums | PascalCase | `ActivityController`, `TimeOfDay` |
| Interfaces | `I` + PascalCase | `IActivityTarget` |
| Methods/properties | PascalCase | `AdvanceActivity`, `IsActive` |
| Parameters/locals | camelCase | `deltaTime`, `rewardAmount` |
| Private fields | camelCase, no prefix | `readonly InventoryState inventory` |
| Serialized fields | private camelCase with `[SerializeField]` | ` [SerializeField] private Transform playerRoot;` |
| Static fields | camelCase; `readonly` where possible | `static bool visible` |
| Constants | PascalCase | `InteractionRange` |
| Events | PascalCase; event past tense when appropriate | `ActivityCompleted` |
| Event handlers | `On` + event name | `OnActivityCompleted` |
| Booleans | `is`, `has`, `can`, or `should` wording | `CanInteract`, `IsComplete` |
| Generic parameters | `T` or descriptive `TItem` | `TResult` |
| Test methods | descriptive behavior statement | `ThreeStrongHitsCompleteAndAwardThreeWood` |

**MUST** use explicit access control, private serialized fields instead of public mutable Inspector fields, braces for control flow, and one primary type per file. **SHOULD** use properties for simple access and methods for operations, side effects, or meaningful computation. **MUST NOT** introduce unexplained abbreviations, magic numbers, or magic strings; use named constants or a clearly named definition. Comments explain intent, constraints, or lifecycle assumptions, not syntax. XML documentation is **MAY** for public or non-obvious APIs that are reused outside the immediate feature.

Correct repository example: `public PlayerController(Transform transform, PlayerInputAdapter input, ActivityController activities)` makes dependencies visible. Discouraged example: `public InventoryState inventory;` on a scene component, which permits arbitrary mutation from the Inspector/runtime.

Enforcement: formatting whitespace and braces are enforced by `.editorconfig` for new/modified code; naming is warning/review guidance because legacy UI conflicts. Exceptions: vendor/template files and guarded Noesis experiment code may retain their existing style.

## 4. Unity component standards

- **MonoBehaviours MUST** coordinate Unity lifecycle, scene references, input-facing behavior, and presentation. `GameRuntime`, `WorldRuntime`, `PlayerInputAdapter`, interactables, `GameHud`, and `RuntimeDebugPanel` are valid examples.
- **Plain C# classes SHOULD** own domain/simulation rules that do not require Unity lifecycle. `GameClock`, `InventoryState`, `ActivityController`, and activities are the current examples and are directly testable.
- **ScriptableObjects MAY** hold authored definitions or configuration once the project has real shared data; they are not required for every feature.
- **Components MUST** have one clear responsibility and explicit ownership of references.
- **Prefabs and scene objects MUST** own composition and serialized references, not hidden game rules. Current prefab usage is TBD; no prefab is currently required by the bootstrap slice.
- **Editor tooling MUST** remain under `Assets/Editor` or an Editor-only assembly and must not leak into player assemblies.

The approved bootstrap path is `Boostrap.unity` -> exactly one active `GameRuntime` -> `GameRuntime.Initialize()` -> runtime construction. `Salty > Validate Bootstrap Scene` is the current editor validation entry point. **MUST NOT** add a competing bootstrap, `DontDestroyOnLoad` singleton, or implicit scene-order dependency without an architecture decision.

`Awake`, `OnEnable`, `Start`, and scene load timing are not interchangeable. Initialization and shutdown ownership must be documented at the component that owns it. Current runtime construction is explicit, but teardown behavior for generated world objects is incomplete and is an adoption risk, not a reason to refactor now.

Enforcement: bootstrap scene validator is available now; duplicate bootstrap policy and lifecycle ownership are review checks. Exception: Unity template/editor content is outside runtime standards.

## 5. ScriptableObject standards

Approved future uses: shared immutable definitions, configuration, item/activity/interaction/world definitions, catalogs, balancing curves, event channels with cleanup/debugging, runtime-set assets with explicit lifecycle, and intentionally pluggable behavior.

**MUST** separate definition data, runtime instance state, and persistent save data. Configuration assets are read-only at runtime unless mutation is intentional and documented. **AVOID** using ScriptableObjects as uncontrolled global mutable state, save files, hidden service locators, or scene-specific runtime state that has no reset/ownership path. Do not rely on Editor play-mode mutations being harmless.

Correct future example: `WoodChoppingDefinition` stores authored health, reward, and timing values; `WoodChoppingActivity` stores health/elapsed runtime state; a future save DTO stores persistent progress. Discouraged: mutate a shared `ResourceCatalog` to hold the current player's inventory.

Current usage: no first-party gameplay ScriptableObjects are present; template `Readme` and URP settings are not gameplay architecture. Event channels and runtime sets are **TBD** until a concrete cross-scene use exists.

Enforcement: documentation/review only now; a future asset validator may check naming, folder placement, and mutability conventions. Exception: Unity/package assets follow their owner.

## 6. SOLID and design-pattern policy

Use the smallest pattern that solves a demonstrated problem.

| Pattern | Appropriate here | Warning/signals to avoid | Current status |
|---|---|---|---|
| Factory | `IActivityTarget.CreateActivity` when target creation varies | Generic factory hierarchy for three activities | In use, intentionally small |
| Object Pool | Repeated transient objects after a measured allocation problem | Pooling one-off bootstrap sprites | MEASURE FIRST; not used |
| Singleton | Only one true process service with explicit lifecycle and tests | Convenience global access or hidden initialization | Avoid; not used |
| Service locator | No approved use in current architecture | Hidden dependencies and order coupling | Avoid; not used |
| Command | UI/input actions need queueing, undo, or replay | Wrapper around a direct method call | Optional; experimental `DelegateCommand` is isolated/guarded |
| State | Many explicit transitions make branching unreadable | Enum wrapper with no behavior benefit | `GameState` is a simple enum; no state framework |
| Observer | Decoupled notifications across a real boundary | Events replacing a direct call | Use sparingly; no gameplay event bus |
| MVP/MVVM | Complex UI with independent view state/testing | Imposing it on the current simple `OnGUI` HUD | Noesis path is experimental/TBD |
| Strategy | Multiple interchangeable rules with real variation | Interface for every class | `IActivity` is a valid boundary |
| Flyweight | Many shared immutable definitions | Premature data indirection | TBD |
| Dirty Flag | Expensive derived UI/world rebuilds | Flagging cheap direct reads | TBD |

Interfaces are **SHOULD** be used at real substitution or module boundaries, not automatically for every class. Deep inheritance is **AVOID**. Singleton/service-locator exceptions require a written responsibility, initialization owner, shutdown/test strategy, and reason explicit injection is insufficient.

Enforcement: review/documentation only. Assembly references can prevent some dependency mistakes.

### Pattern decision guide

This project follows the applicability-first view in Fireship's [10 Design Patterns Explained in 10 Minutes](https://www.youtube.com/watch?v=tv-_1er1mWI): a named pattern is a response to a repeated problem, not a feature checklist. Before adding one, name the concrete problem, identify the existing direct/simple alternative, and add a focused test for the new behavior.

| Pattern from the video | Project rule and current status |
|---|---|
| Singleton | **Avoid.** `GameRuntime` is the explicit Bootstrap composition root, not a global access point. A static debug visibility flag is development-only UI state, never a gameplay service pattern. |
| Prototype | **Defer.** Use Unity prefab/`Instantiate` copying only when multiple runtime instances genuinely derive from one authored base. Do not create a clone abstraction for the current hand-built world. |
| Builder | **Defer.** `WorldRuntime.Build` is explicit bootstrap composition, not a reusable Builder API. Add a builder only when a real object has many optional construction steps that make direct construction unreadable. |
| Factory | **Approved where already used.** `IActivityTarget.CreateActivity()` is the small factory boundary: different world targets create different activity rules while `ActivityController` stays unaware of their concrete types. Do not add factory hierarchies or registries until creation varies beyond this boundary. |
| Facade | **Use sparingly.** `GameRuntime` coordinates the runtime through explicit properties and direct calls. Add a narrow facade only when callers repeatedly need the same multi-system operation; it must not become a hidden service locator. |
| Proxy | **Defer.** Add only for a demonstrated access-control, lazy-load, or instrumentation boundary. Do not wrap ordinary game state just to intercept getters/setters. |
| Iterator | **Use language support.** `IReadOnlyList` plus `foreach` already expresses target/resource traversal. Do not write custom iterators until traversal has non-trivial rules that collection APIs cannot express. |
| Observer | **Defer.** A direct call is preferred while ownership is clear. Introduce a typed event only for a real one-to-many notification where the publisher must not know consumers; document subscribe/unsubscribe ownership. No global gameplay event bus. |
| Mediator | **Defer.** `GameRuntime` and `InteractionController` are explicit coordinators with visible dependencies. Add a mediator only when several peers need to communicate and direct calls create circular or repetitive coupling. |
| State | **Start simple.** `GameState` and `TimeOfDay` enums are appropriate while transitions are few. Promote to state objects only when each state owns distinct behavior and conditionals are demonstrably obscuring the transition rules. |

Pattern names MUST clarify the code's responsibility. A pattern that adds indirection without removing a demonstrated source of coupling, branching, or duplicate construction is rejected.

## 7. Dependency and event rules

Dependencies **MUST** be visible through constructor/method injection for plain C# types, serialized references for authored scene composition, explicit bootstrap composition, narrow interfaces, or documented event channels. The current dependency direction is from `GameRuntime` into world/input/domain/presentation, while activities and inventory remain Unity-independent.

**AVOID** `FindObjectOfType`/`FindAnyObjectByType` dependency discovery, convenience statics, hidden utility dependencies, and circular assembly references. Events are **MAY** be used only when the publisher must not know the consumer. A direct call is preferred when ownership is clear.

Events must use clear PascalCase names and payloads that describe the fact, not an opaque bag. The owner documents subscription timing and always unsubscribes at the matching lifecycle boundary. Event channels require subscriber cleanup and a debugging path. Current project event policy is otherwise **TBD** because no first-party gameplay events exist.

Enforcement: asmdef dependency cycles are machine-checkable; hidden discovery and event ownership are review checks.

## 8. Lifecycle, updates, and timing

- `Awake`: component-local references and bootstrap entry only when required before other `Start` calls.
- `OnEnable`/`OnDisable`: paired subscription ownership; no gameplay state reset without an explicit reason.
- `Start`: deferred initialization only when it genuinely depends on the loaded scene.
- `Update`: input sampling, presentation coordination, or measured per-frame work. Current `GameRuntime.Update` is the single game tick.
- `FixedUpdate`: physics integration only; the current activity loop is not physics-driven.
- `LateUpdate`: camera/follow/presentation correction only when ordering requires it.
- Coroutines/async: **MAY** represent asynchronous waits or I/O; ownership and cancellation are required. Do not use them to hide gameplay state transitions.
- Explicit tick interfaces: **SHOULD** be used for plain domain systems, as current `IActivity.Tick` demonstrates.
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

`Boostrap.unity` is the current composition scene and is enabled in Build Settings; `Intro` and `MainMenu` exist but are not currently enabled. Scene responsibilities and additive-scene policy are otherwise **TBD**. Do not assume Addressables: the manifest does not include Addressables, so no Addressables standard applies.

Assets belong in the owning feature folder; settings remain in `Assets/Settings`; third-party content remains isolated. Prefab variants are **SHOULD** be used only when the base/variant ownership is clear. Safe moves/renames require Unity Editor migration, `.meta` preservation, reference validation, and a separate commit/plan. Never bulk move/rename during feature work.

### Interactive terrain states

When an interaction gates a route or changes the world, its visual representation **MUST** be authored as a terrain state that shares the neighboring tile grid, scale, palette, and edge treatment. A closed state hides the route; a cleared state reveals the route through the interactable's explicit visual ownership. Do not layer a self-contained prop over unrelated terrain and call it a terrain transition.

Current example: `JungleEdgeInteractable` owns a closed 3x2 tile set and swaps it for the matching open 3x2 `Jungle Exit Route` tile set when chopped. The cells are rendered individually at the standard 128 pixels per unit, so the transition remains terrain rather than a full-scene texture. This is a small local state change, not a generic world-state framework. New terrain gates should follow that direct two-visual pattern until more than one shared rule proves a reusable abstraction is needed.

Save-data architecture and version migration are **TBD** because no save system exists. When introduced, persistent DTOs must be separate from runtime objects and have an explicit version/migration test.

Enforcement: `.meta` parity, YAML mode, enabled bootstrap, and forbidden generated paths can be checked now; asset ownership and serialized-reference correctness need Unity validation. Exception: package/vendor assets.

## 11. UI and presentation boundaries

The active bootstrap slice uses Unity built-in `OnGUI` in `GameHud` and `RuntimeDebugPanel`. uGUI is installed, and UI Toolkit support is present through Unity modules, but no first-party UI Toolkit runtime screen was found. The Noesis/XAML path under `Assets/UI/HUD` is guarded by `NOESIS` and is experimental; its machine-specific dependency concern is documented in `FRAMEWORK.md`.

Presentation **MUST** display state and send user intent without owning activity, inventory, or world rules. Refresh on meaningful state changes where practical; do not create unnecessary layout/canvas rebuilds. Use a presenter/view-model only when screen complexity or independent testing justifies it. Do not impose MVVM on the current simple HUD.

Correct: `GameHud` reads `GameRuntime` and renders status. Discouraged: `GameHud` awards inventory or depletes targets. Editor UI and runtime UI must remain separate.

Enforcement: assembly boundaries and code review; no UI architecture validator currently exists. Exception: the guarded Noesis experiment may retain its separate legacy convention until formally adopted.

## 12. Testing and validation

Plain C# domain systems **MUST** be testable without a production scene whenever practical. The current `WoodChoppingActivityTests` cover activities, controller/inventory delivery, world bootstrap construction, target reset, and clock progression. Tests use NUnit and live in `Assets/Tests/Runtime` under `SaltyGame.Tests`.

Test names describe behavior, not implementation. Fixtures must own setup/teardown and destroy created Unity objects. Add Edit Mode tests for pure rules, serialization/configuration, and migrations; add Play Mode tests for scene composition, lifecycle, input wiring, and presentation integration. Integration/regression tests belong at the narrowest useful boundary. Performance tests are required only for measured performance-sensitive systems.

Current gaps: no Play Mode test assembly, save-data tests, serialization tests, or performance tests were found. These are staged requirements, not immediate refactor work.

Enforcement: Unity Test Framework execution and bootstrap menu validation can be run now; coverage and Play Mode gates are planned. Exception: prototype-only features may start with a focused Edit Mode test or a documented manual validation path.

## 13. Productivity and Editor tooling

Use the existing `Salty > Validate Bootstrap Scene` validator before bootstrap/scene changes. Add custom inspectors, property drawers, validation menus, templates, or build scripts only for repeated, measurable friction. Development-only diagnostics such as F3 panel behavior must not become gameplay dependencies. Console logs use a stable `[Salty]` prefix for editor validation and should be removed, gated, or downgraded when noisy.

Editor code **MUST** stay out of runtime assemblies. Build scripts and CI checks **SHOULD** be deterministic and report actionable file paths. Project-local templates are **TBD**; do not add them until the naming/field patterns stabilize.

Enforcement: asmdef platform boundaries and the existing validator; tooling scope is review/documentation.

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
| Bootstrap scene composition | Enforced now by `SaltyBootstrapValidator` | Run in editor/CI where available |
| Tests | Enforced where existing tests apply | Add Play Mode/save/performance gates by phase |
| Performance claims | Documentation/review now | Add scenario captures and budgets |
| Asset moves/renames | Documentation/review only | Unity migration tooling when needed |
| DOTS adoption | Documentation/review gate | Benchmark before prototype |

## Decisions that remain TBD

Target hardware and frame/memory budgets; save format/versioning; additive scene policy; prefab ownership/variant policy; branch model in Plastic SCM; large-file policy; UI technology after the Noesis experiment; project script templates; gameplay event-channel policy; and whether a second runtime assembly is justified.
