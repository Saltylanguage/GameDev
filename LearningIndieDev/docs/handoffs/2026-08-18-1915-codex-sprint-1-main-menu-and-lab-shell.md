# Sprint 1 main menu and lab shell

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: Codex
- Branch: codex/cellular-sprite-tiling
- Baseline commit: a8d89aa
- Date: 2026-08-18

## Summary

Implemented the bounded S1.1 player-facing shell in the existing `MainMenu.unity`
scene. The route now has an explicit Main Menu, Lab Overview, and Research
preview state using deterministic representative data.

## Changes

- Added `Assets/UI/MainMenu/MainMenuShell.xaml` with the three-screen route,
  scientific-data bar, representative project states, disabled prototype
  actions, responsive Viewbox layout, focus targets, and quit confirmation.
- Added `Assets/UI/MainMenu/MainMenuViewModel.cs` with explicit page state,
  commands, deterministic Back/Esc behavior, project selection, and focus
  restoration. It contains no wallet, persistence, or simulation logic.
- Added matching Unity metadata and changed `Assets/Scenes/MainMenu.unity` to
  bind the new XAML and ViewModel instead of the temporary test screen.

## Decisions and assumptions

- The first implementation keeps balances and project data static and clearly
  labels them representative, matching the C2 UX contract.
- Settings, Credits, Species Archive, Expedition Setup, and Purchase remain
  disabled prototype actions; no fake state mutation was introduced.
- `MainMenu.unity` was not added to Build Settings because the documented smoke
  gate has not passed yet.

## Validation

- Parsed `MainMenuShell.xaml` as XML successfully.
- Verified every ViewModel focus target has a matching named XAML control.
- Scanned new Unity metadata for duplicate GUIDs successfully.
- Ran `git diff --check` on the scene and new shell assets.

## Risks and incomplete work

- Unity Editor Play Mode/build smoke could not be run in this environment due
  to the known Unity licensing/headless entitlement blocker.
- The shell is not yet wired to real profile, wallet, persistence, or simulation
  services by design; those are later S1/S5 boundaries.
- Final art direction, audio, and broad Lab navigation remain deferred.

## Next useful step

Run the shell in Unity at 1920×1080 and 1280×720 once licensing is available,
then record focus/input evidence and promote `MainMenu.unity` to Build Settings
only if the smoke path passes.
