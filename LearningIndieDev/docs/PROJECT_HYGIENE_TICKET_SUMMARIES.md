# Project Hygiene Ticket Summaries

> Status: Proposed cleanup/refactor backlog  
> Date: 2026-09-03  
> Related: [`LOOSE_ENDS.md`](LOOSE_ENDS.md), [`WORKING_STATE.md`](WORKING_STATE.md)

These tickets turn the 2026-09-03 hygiene review into actionable work items.
The ticket IDs match the corresponding entries in `LOOSE_ENDS.md`.

No ticket authorizes an opportunistic Unity asset deletion, move, or rename.
Serialized asset work must preserve `.meta` files/GUIDs and include the focused
Unity validation required by the project standards.

## Recommended sequence

1. Resolve the two authority questions first: terrain tiling (`P1-022`) and
   current project/standards documentation (`P1-023`).
2. Make the explicit keep/remove decisions for the low-risk artifacts
   (`P2-010` through `P2-013` and `P2-017`/`P2-018`).
3. After Unity IPC and UI acceptance recover, perform the focused orphan
   cleanups (`P2-008` and `P2-009`).
4. Schedule compatibility and structural refactors only after the current
   simulation/UI contracts are stable (`P2-007`, `P2-014`, `P2-015`, `P2-016`).

## Ticket summaries

### P1-022 — Reconcile the terrain tiling contract

**Priority:** P1  
**Status:** Resolved in code and documentation; visual Unity acceptance remains open
**Owner:** Presentation/art owner + Sim  
**Size:** Medium  
**Evidence:** `TerrainTileResolver` and its tests implement 47 normalized
eight-neighbor blob masks; `TerrainTilePreviewWindow` loads the named
`Art/Terrain/Blob/128/{Grass,Desert}` families; the tiling plan and linked art
documents now describe that same model and path. The older 16-mask artifacts
remain historical.

**Goal:** Keep one authoritative terrain-mask model across the code, tests,
editor preview, art paths, atlas assumptions, and documentation.

**Acceptance criteria:**

- The authoritative mask model and sampling convention are stated once.
- Runtime resolver, tests, preview window, sprite names, and plan use the same
  mask count and art path.
- Any art/atlas change is validated in Unity; no renderer-only state enters the
  simulation model.
- The focused reconciliation records that the 47-mask implementation is
  authoritative and the superseded 15-piece plan is not used by the preview.

**Non-goals:** New terrain mechanics, Tilemap/RuleTile adoption, or an
unmeasured renderer rewrite.

### P1-023 — Refresh current project and engineering guidance

**Priority:** P1  
**Status:** Resolved for current guidance; historical Island claims remain scoped
**Owner:** Josh + repository maintainer  
**Size:** Small-medium  
**Evidence:** `FRAMEWORK.md` refers to a missing Bootstrap scene and validator;
Build Settings, the standards, the adoption plan, Noesis defines, and Play Mode
assembly facts no longer agree.

**Goal:** Establish which documentation is current guidance and clearly date or
scope historical Island Survivor/bootstrap assumptions.

**Acceptance criteria:**

- `FRAMEWORK.md`, `UNITY_ENGINEERING_STANDARDS.md`, and
  `UNITY_STANDARDS_ADOPTION_PLAN.md` name the current scene list, validator,
  Play Mode coverage, Noesis build state, and actual starter-file paths.
- Historical claims are labeled with their scope/date rather than presented as
  current instructions.
- The updated docs link to the current validation commands and relevant plans.
- No gameplay, scene, package, or asset changes are bundled into the doc pass.

**Non-goals:** Choosing a new UI technology, changing the build scene order, or
repairing the Unity machine state.

**Result:** `FRAMEWORK.md` remains explicitly historical for the deprecated
Bootstrap/Island slice. The standards and adoption plan now name the active
Main Menu → Lab → CellularAutomataPrototype flow, current ScriptableObject
authoring assets, focused Play Mode coverage, and the adopted Noesis ViewModel
path while preserving the deprecated validator's scope.

### P2-007 — Complete the `SpeciesArchetype` migration

**Priority:** P2  
**Status:** Deferred compatibility refactor  
**Owner:** Sim/domain owner  
**Size:** Large  
**Dependency:** A deliberate breaking-cleanup window and a green current test
baseline.

**Goal:** Move remaining runtime and test callsites to `SpeciesId`, then remove
the obsolete enum and compatibility surface.

**Acceptance criteria:**

- A repository-wide search finds no production or test use of
  `SpeciesArchetype`, its implicit conversion, or legacy properties/overloads.
- Serialized data and report compatibility are checked before removal.
- `SpeciesArchetype.cs`, `SpeciesId` conversion helpers, and obsolete API
  members are removed in one focused domain change.
- Edit Mode tests and relevant simulation/report checks pass with unchanged
  behavior.

**Non-goals:** Reworking species identity, adding a new abstraction, or changing
simulation rules.

### P2-008 — Remove orphan `CavePreview` and `LifeSimulationPreview`

**Priority:** P2  
**Status:** Ready after Unity validation is available  
**Owner:** Sim/domain owner  
**Size:** Small  
**Dependency:** Repair Unity IPC/preflight; confirm retained Edit Mode and
targeted cellular Play Mode tests can run.

**Goal:** Remove the two unreferenced presentation components while retaining
the deterministic cave/Life domain code and tests.

**Acceptance criteria:**

- Final GUID/name/reference scan finds no scene, prefab, asset, or code dependency
  on either preview component.
- Each `.cs` file is deleted together with its matching `.meta` file.
- Retained domain tests and the targeted cellular Play Mode test pass.
- The cleanup is isolated from unrelated simulation or scene changes.

**Non-goals:** Removing `CaveGenerator`, `LifeSimulation`, their tests, or the
active `SpeciesSimulationPreview`.

### P2-009 — Remove the copied UI starter scaffold

**Priority:** P2  
**Status:** Strong removal candidate; waiting for final UI acceptance  
**Owner:** UI owner  
**Size:** Small  
**Dependency:** Final GUID scan and confirmation that no design handoff still
uses the starter demo.

**Goal:** Delete the unreferenced Yoda/progress-bar demo code and duplicate
`TestUI.xaml` files.

**Scope:** `Assets/UI/MainMenu/Scripts/BaseViewModel.cs` plus its `.meta`, and
`TestUI.xaml` plus `.meta` under MainMenu, EcoSim, GalapagOS, and HUD.

**Acceptance criteria:**

- The final serialized reference scan has no references to the listed GUIDs.
- All listed source/XAML files and matching `.meta` files are removed together.
- Active Main Menu, Lab, GalapagOS, and simulation UI assets remain intact.
- Unity reimport/compile completes without references to `BaseViewModel` or
  `YodaIsGayCommand`.

**Non-goals:** Removing the active `MainMenu_Old` contract or changing the
Noesis package setup.

### P2-010 — Decide the fate of the `Assets/UI/EcoSim` placeholder shell

**Priority:** P2  
**Status:** Decision required  
**Owner:** UI/product owner  
**Size:** Small-medium  
**Dependency:** Confirm no future experiment, design review, or handoff owns
the initial EcoSim scaffold.

**Goal:** Either remove the unreferenced EcoSim placeholder tree or document it
as an explicitly retained prototype boundary.

**Acceptance criteria:**

- An owner records keep/remove intent for the folder.
- If removed, all five XAML assets and matching `.meta` files are handled in a
  focused cleanup and no GUID references remain.
- If retained, the folder has a short purpose/owner note and is excluded from
  “active production UI” assumptions.
- No active Lab/GalapagOS resource dictionary is changed accidentally.

**Non-goals:** Building the EcoSim UI or changing the current Lab flow.

### P2-011 — Remove or explicitly retain URP template onboarding

**Priority:** P2  
**Status:** Owner decision required  
**Owner:** Josh + repository maintainer  
**Size:** Small  
**Dependency:** Confirm the project no longer needs Unity’s onboarding Readme.

**Goal:** Resolve whether `Assets/Readme.asset` and `Assets/TutorialInfo/**`
remain useful project content.

**Acceptance criteria:**

- The owner records either “retain as template content” or “remove.”
- If removed, the Readme, complete TutorialInfo tree, and matching `.meta`
  files are deleted together with no broken references.
- If retained, the standards/adoption plan labels it as intentional template
  content and no cleanup ticket continues to treat it as orphaned.

**Non-goals:** Reformatting or modernizing Unity’s vendor/template scripts.

### P2-012 — Classify `_Recovery/0.unity`

**Priority:** P2  
**Status:** Owner decision required  
**Owner:** Josh  
**Size:** Small  
**Dependency:** Confirm whether the scene is a deliberate recovery checkpoint.

**Goal:** Stop treating the recovery scene as unexplained project baggage.

**Acceptance criteria:**

- Josh records whether the scene is retained for recovery or no longer needed.
- If retained, its purpose, date, and owner are documented and it is clearly
  excluded from the active scene list.
- If removed, the scene and `.meta` are deleted in a focused change after a
  final reference scan.
- No active cellular scene or serialized asset loses a required reference.

**Non-goals:** Reconstructing or merging the older cellular prototype.

### P2-013 — Remove or reserve `Assets/Scenes/Intro.unity`

**Priority:** P2  
**Status:** Archive/removal candidate  
**Owner:** Josh + repository maintainer  
**Size:** Small  
**Dependency:** Confirm it is not reserved for a future splash or entry flow.

**Goal:** Remove the unused camera/light template scene or explicitly reserve it
with a documented purpose.

**Acceptance criteria:**

- The owner records whether Intro is retained for a defined future use.
- If removed, `Intro.unity` and its `.meta` are deleted after a final reference
  scan and Build Settings check.
- `GalapagOSDesktopTest` and `TerrainPaintTest` remain untouched as manual
  acceptance scenes.
- No scene-order or startup behavior changes as a side effect.

### P2-014 — Rename `MainMenu_Old` through a GUID-preserving migration

**Priority:** P2  
**Status:** Deferred naming refactor; active path must be retained meanwhile  
**Owner:** UI owner  
**Size:** Medium  
**Dependency:** Graphics/UI acceptance complete and a Unity Editor migration
path available.

**Goal:** Replace the misleading `MainMenu_Old` directory name without breaking
the active Main Menu scene contract.

**Acceptance criteria:**

- The folder is renamed through Unity or an equivalent GUID-preserving
  migration; no asset receives a new GUID.
- `MainMenu.unity`, `MainMenuPlayModeTests`, docs, and any path-based tooling
  point to the new location.
- Unity reimport/compile and Main Menu Play Mode validation pass.
- The change is isolated from UI visual redesign.

**Non-goals:** Deleting the active Main Menu VM/host/XAML or changing scene
composition.

### P2-015 — Decompose the largest multi-responsibility files

**Priority:** P2  
**Status:** Deferred staged refactor  
**Owner:** Sim + UI/tooling owners  
**Size:** Large, split into multiple implementation tickets  
**Dependency:** P3/Unity and UI acceptance gates are green; current behavior is
captured by focused tests.

**Goal:** Reduce change coupling without destabilizing the active simulation,
Noesis shell, or experiment evidence pipeline.

**Initial seams:** simulation phases in `SpeciesSimulation.cs`; metrics DTOs vs
accumulation in `SpeciesSimulationMetrics.cs`; persistence/formatting vs
orchestration in `SpeciesSimulationPreview.cs` and `VM_SimulationShell.cs`;
report writers vs CLI execution in `CellularSimulationExperimentRunner.cs`;
behavior-based partitions in `SpeciesDomainTests.cs`.

**Acceptance criteria:**

- Each extraction has one concern, one focused diff, and a named validation
  target.
- Public names, serialized fields, report schema, and Unity scene wiring remain
  stable unless a separate migration is approved.
- Determinism, report fingerprints, targeted Edit Mode tests, and relevant
  Play Mode checks remain green.
- No “split everything” rewrite is attempted as one ticket.

**Non-goals:** New event buses, DI frameworks, performance claims, or gameplay
rule changes.

### P2-016 — Remove or document duplicated neighbor-pattern definitions

**Priority:** P2  
**Status:** Resolved; editor generation now consumes runtime defaults
**Owner:** Sim/tooling owner  
**Size:** Small  
**Dependency:** Confirm runtime/Editor assembly boundaries and asset-generation
requirements.

**Goal:** Prevent Cardinal/Moore pattern drift between runtime defaults and the
Editor scenario asset generator.

**Acceptance criteria:**

- Either a safe shared pure C# helper is introduced with parity coverage, or the
  intentional duplication and its maintenance rule are documented.
- Generated scenario assets retain the same serialized values and fingerprints.
- Editor-only code does not leak into runtime assemblies.
- No generic pattern framework is added for the sake of removing two literals.

**Non-goals:** Changing species behavior or regenerating unrelated scenarios.

**Result:** `CreateSpeciesScenarioAssets` copies the ordered offsets exposed by
`SpeciesRuleDefaults.CreateCardinalPattern()` and `CreateMoorePattern()` into
fresh arrays for serialization. The duplicate editor literals were removed;
runtime behavior and asset-generation boundaries are unchanged.

### P2-017 — Decide retention of `PrepareEx002Variants`

**Priority:** P2  
**Status:** Retention/archive decision required  
**Owner:** Sim/tooling owner  
**Size:** Small  
**Dependency:** EX-002 artifact retention and reproducibility policy.

**Goal:** Decide whether the one-shot generator remains a supported
reproducibility tool.

**Acceptance criteria:**

- The EX-002 policy names whether regeneration from source is still required.
- If retained, the menu command, inputs, outputs, and expected asset paths are
  documented and the script has an owner.
- If archived, a compact reproducibility record preserves the required source
  settings and generated artifact provenance before removal.
- No existing EX-002 evidence is overwritten or silently reinterpreted.

**Non-goals:** Re-running blocked experiments or changing the intervention
contract.

### P2-018 — Triage empty Unity directory shells

**Priority:** P2  
**Status:** Low-value structure cleanup  
**Owner:** Repository maintainer + feature owners  
**Size:** Small-medium  
**Dependency:** Each candidate has an explicit owner or confirmed no-use state.

**Goal:** Remove empty folders that provide no current ownership value while
preserving deliberate future feature boundaries.

**Candidate groups:** `Assets/Audio`, `Materials`, `ThirdParty`, unused
`Assets/Project/**` shells, `UI/Prefabs`, `PuzzleUI`, `Textures`,
`UI/EcoSim/Scripts`, `UI/GalapagOS/Art`, and `UI/MainMenu/XAML`.

**Acceptance criteria:**

- Each removed directory has no planned near-term owner or documented purpose.
- Folder `.meta` handling is deliberate and leaves no orphan metadata.
- No assets are moved as part of this cleanup.
- The resulting layout is documented so future empty-folder recreation is
  intentional rather than accidental.

**Non-goals:** Reorganizing populated feature folders or standardizing the whole
repository tree.

