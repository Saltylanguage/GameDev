# Main Menu technical readiness spike

[Working state](../WORKING_STATE.md) | Status: in-progress

- Owner: Codex
- Branch: BevLaptopBranch
- Baseline commit: f1a585c
- Date: 2026-08-15

## Summary

`MainMenu.unity` is reusable as the Sprint 1 player-shell host. Its camera already has a configured `NoesisView` with keyboard and mouse input enabled. The required repair is limited to replacing the retired placeholder XAML and placeholder `BaseViewModel`; no simulation or Dev Lab code needs to move into the scene.

## Readiness findings

- `Assets/Scenes/MainMenu.unity` contains only a Main Camera, a directional light, `NoesisView`, and the legacy `BaseViewModel` component.
- The view currently references `Assets/UI/HUD/TestUI.xaml`; that file is a test screen and cannot be shipped as player-facing UI.
- `Assets/UI/HUD/Scripts/BaseViewModel.cs` is a matching test component. Replace it rather than extending it.
- The established Noesis pattern is `SpeciesSimulationNoesisHost` plus `SpeciesSimulationViewModel`: bind one explicit ViewModel to `NoesisView.Content.DataContext` and expose UI commands through `DelegateCommand`.
- `MainMenu.unity` is not in Build Settings. Promote it only after the direct-scene smoke path passes. Do not overwrite the pre-existing local changes in `ProjectSettings/EditorBuildSettings.asset`.

## Sprint 1 change set

- Create a player-facing root XAML asset for Main Menu, Lab Overview, and the Herbivore Research preview. Preserve the current test XAML until the new asset has been imported and the scene reference can be changed through Unity.
- Add a focused `MainMenuViewModel` and small representative-data fixture. It owns only the three known screen states and commands: Enter Lab, Open Research, and Back. It has no wallet, profile, persistence, scene-load, or simulation logic.
- Replace the `BaseViewModel` component on the Main Camera with the new ViewModel/host and update the scene's `NoesisView` XAML reference in Unity, preserving scene and asset GUIDs.
- Add a Play Mode navigation smoke check for Main Menu -> Lab Overview -> Research -> Back. Confirm the test assembly boundary before choosing whether the test drives the ViewModel directly or the Noesis controls.
- After direct-scene smoke passes, add `MainMenu.unity` as the first Build Settings scene in a separate serialized-settings change, then run the Windows development-build launch check.

## Risks and incomplete work

- The existing menu XAML/ViewModel is legacy test content, so it supplies wiring evidence only; it is not a code template for the new shell.
- The existing Play Mode test assembly references `SaltyGame.Runtime`, while Noesis UI currently compiles in the default Assembly-CSharp assembly. The smallest test-access seam must be confirmed during implementation; do not add a generic UI framework to solve it.
- Build Settings and several other project settings have unowned local edits. They are outside this spike and must remain intact.
- No Unity Editor or test run was performed in this investigation. Recent handoffs note that project-locking Unity processes can block headless validation.

## Next useful step

Implement the bounded Main Menu/Lab shell only after the UX contract names the representative balances and first Herbivore research states. Start with the new XAML and ViewModel, then use a direct-scene smoke before any Build Settings change.
