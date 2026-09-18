# Main Game Flow Cleanup Candidates

**Status:** The Island Survivor candidate was removed on 2026-09-18. The other
rows remain review-only candidates; no other files were deleted for this work.

For Unity assets listed below, their `.meta` sidecars are part of the same
review bundle.

The player route is **Main Menu → GalapagOS Desktop → desktop-hosted
Simulation**. Being outside that route makes something a review candidate; it
does not by itself make it unused. Test coverage, developer workflows, and
historical value count as uses too.

## Candidates to review

| Candidate | Why it is on the list | Current guardrail |
| --- | --- | --- |
| **Island Survivor prototype slice — removed 2026-09-18** | The user approved retiring the off-flow prototype as one connected slice. | Removed its scene/build entry, runtime modules, dedicated tests, validator, and Island Chores textures with `.meta` files. The historical handoffs and audit remain. See [legacy prototype audit](LEGACY_PROTOTYPE_AUDIT.md). |
| **Terrain Paint diagnostic bundle** — `Assets/Scenes/TerrainPaintTest.unity`; `Assets/Scripts/Game/Presentation/TerrainPaintPreview.cs`; `Assets/Tests/Runtime/TerrainPaintPreviewTests.cs` | The scene is not in Build Settings or the player route. It is a manual developer diagnostic. Its helper still has a focused test, so the scene, script, and test should be considered together. | Existing item [P2-022 in Loose Ends](LOOSE_ENDS.md) says to decide after the terrain asset/presentation workflow settles: keep as a bounded diagnostic, migrate, or explicitly remove. The terrain atlas itself is used elsewhere and is not part of this candidate. |
| **Empty asset-folder placeholders** — `Assets/Audio/`, `Assets/Materials/`, `Assets/ThirdParty/` | Each directory currently contains no files. | Low-priority housekeeping only. Confirm nobody intends these as reserved import locations before removing the empty directories; this is not a runtime cleanup. |

## Keep out of the removal list

- `Assets/Scenes/MainMenu.unity` and `Assets/Scenes/GalapagOSDesktopTest.unity`
  are on the main route. “Test” in the desktop scene name is misleading, not
  evidence that the scene is disposable.
- `Assets/UI/MainMenu_Old/` is still serialized into the active Main Menu and
  required by its tests. It may merit a future rename, not removal.
- `Assets/Scenes/CellularAutomataPrototype.unity` and `Assets/Scenes/Lab.unity`
  are outside the player route, but remain intentional developer/test routes.
  The Lab loads the standalone simulation scene, and Play Mode tests exercise
  both. Keep them unless those workflows are separately retired.
- Test files are not production-flow assets, but are deliberate project
  dependencies. Retain them unless the feature or test contract is retired.
- The active cellular simulation’s terrain/species art and scenario data are
  not candidates merely because some are loaded indirectly or exercised by
  tests. Island Survivor art was removed only as part of the explicitly
  approved slice retirement; this does not authorize broader art cleanup.

## Evidence checked

- [Main Menu and Home-Base Delivery Plan](MAIN_MENU_LAB_DELIVERY_PLAN.md) defines
  the player route and distinguishes it from the legacy/developer Lab route.
- [Helper_SceneTransition.cs](../Assets/Scripts/Game/Scene/Helper_SceneTransition.cs)
  and its call sites show Main Menu loading the desktop; the separate
  `LoadSimulation` scene transition is used by `VM_Lab`.
- Before removal, `WorldRuntime` loaded the Island Chores textures by resource
  name. That dependency was removed as part of the approved slice retirement;
  this is historical evidence, not a live source-file reference.
- [Legacy prototype audit](LEGACY_PROTOTYPE_AUDIT.md), [Loose Ends](LOOSE_ENDS.md),
  and [Unity engineering standards](UNITY_ENGINEERING_STANDARDS.md) record the
  existing keep/defer decisions.

## Before any cleanup

No removals beyond the Island Survivor slice are approved by this inventory. If
another candidate is later selected, make that a separate scoped cleanup: check code and serialized references,
Build Settings, tests, editor tools, resource-name loads, and Unity `.meta` GUIDs
as a unit. Handle `.meta` sidecars as part of the same review, then rerun the
affected tests and player-flow checks. Delete or retain each Unity asset and its
`.meta` sidecar together so no orphan metadata or unintended GUID changes are
introduced. Historical Island Survivor handoffs and decisions remain preserved
as records; they are not live project dependencies.
