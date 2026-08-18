# Sprint 0 C3 Main Menu technical readiness

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: Codex
- Branch: codex/cellular-sprite-tiling
- Baseline commit: b8da473
- Date: 2026-08-18

## Summary

C3 is a bounded repair, not a reusable Sprint 1 shell yet. The existing
`MainMenu.unity` scene provides a usable camera and Noesis host boundary, but it
currently points at a sample `TestUI.xaml` and the legacy `BaseViewModel`. The
scene can be retained without rebuilding its composition, while the XAML and
view-model content are replaced with the C2 screen-state fixture.

## Changes

- Inspected `Assets/Scenes/MainMenu.unity`: one Main Camera, enabled
  `NoesisView`, keyboard and mouse input enabled, and a `BaseViewModel`
  component on the camera.
- Confirmed the scene's `_xaml` reference resolves to
  `Assets/UI/HUD/TestUI.xaml`, whose content is a Yoda/progress-bar sample and
  is not Sprint 1 product UI.
- Confirmed `BaseViewModel` is a prototype test model with the `YodaIsGay`
  command and G/F progress-bar controls; it must not become the player-shell
  view-model.
- Confirmed `Assets/UI/HUD/XAML/SpeciesSimulationShell.xaml` and
  `SpeciesSimulationViewModel` belong to the simulation/Dev Lab surface and
  should not be reused as the player Main Menu view-model.
- Confirmed `EditorBuildSettings.asset` currently enables only
  `CellularAutomataPrototype.unity` and `IslandSurvivorPrototype.unity`;
  `MainMenu.unity` is not yet a bootstrap scene.
- Confirmed `MainMenu.unity.meta` is present. Its scene GUID and serialized
  references must be preserved during the bounded repair.

## Decisions and assumptions

- Retain `Assets/Scenes/MainMenu.unity` as the first shell scene; do not create
  a second menu scene or a general navigation framework.
- Add a dedicated player-shell XAML asset and a dedicated small view-model
  under the player UI area. The view-model should expose only the C2 states and
  commands: Main Menu, Lab Overview, Research preview, Back, focus-safe
  disabled actions, and representative fixture data.
- Keep `SpeciesSimulationShell.xaml` and `SpeciesSimulationViewModel` on the
  Dev Lab/simulation path. They expose tuning and simulation controls that are
  outside the C2 player contract.
- Promote `MainMenu.unity` to Build Settings index 0 only after the direct-scene
  smoke path passes at 1920×1080 and 1280×720. Preserve
  `CellularAutomataPrototype.unity` as the direct simulation development path
  until that smoke check succeeds.

## Validation

- Static scene/YAML inspection passed: the scene has a camera, NoesisView, and
  a stable MainMenu scene asset reference.
- XAML and C# reference inspection passed: the current Main Menu sample and
  the simulation shell are distinct paths with no existing player-shell
  ViewModel to accidentally reuse.
- Build Settings inspection passed: the current enabled order is explicit and
  MainMenu is absent, so no accidental bootstrap promotion is occurring.
- Unity Editor/Play Mode smoke validation was not run in this spike because the
  machine's Unity licensing/headless entitlement blocker remains unresolved.

## Risks and incomplete work

- The bounded repair still needs a new player-shell XAML and view-model; C3
  identifies the seam but does not implement Sprint 1 UI code.
- The existing Noesis package is an optional/guarded project dependency;
  Unity import and Play Mode behavior must be verified before Build Settings
  promotion.
- Hand-editing serialized scene GUIDs is unsafe. Use Unity Editor assignment
  for the new XAML reference and preserve the existing `.meta` files.

## Next useful step

Execute C4: create/confirm the Sprint 1 control record, assign owner/reviewer
and acceptance metadata, reconcile the extra UI/Dev Lab board card, and keep the
20-hour cap before starting the bounded player-shell implementation.
