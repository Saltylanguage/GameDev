# prototype scene separation

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: Codex-Josh
- Branch: SaltysFirstBranch
- Baseline commit: c413db49
- Date: 2026-08-10

## Summary

Separated the retained island-survivor vertical slice from the active cellular-
automata prototype so each can run and evolve without composing the other.

## Changes

- Migrated `Boostrap.unity` to `IslandSurvivorPrototype.unity` while preserving
  its scene GUID.
- Added `CellularAutomataPrototype.unity` as the first enabled build scene.
- Removed cave-preview ownership from `GameRuntime` and introduced the dedicated
  `CellularAutomataPrototypeRuntime` composition root.
- Updated Play Mode tests, the island scene validator, project context, and Unity
  engineering documentation to use the new scene boundaries.

## Decisions and assumptions

- `GameRuntime` and `WorldRuntime` belong only to the retained island prototype.
- The cave preview belongs only to the cellular-automata prototype.
- The prototypes may share generic grid and simulation foundations, but neither
  scene should initialize the other's runtime for convenience.

## Validation

- `SaltyGame.Tests.csproj`: build succeeded with zero warnings and zero errors.
- `SaltyGame.PlayMode.Tests.csproj`: build succeeded with zero warnings and zero
  errors.
- `Assembly-CSharp-Editor.csproj`: build succeeded with zero warnings and zero
  errors.
- Unity Test Runner execution was not available because an existing Unity process
  held the project lock.

## Risks and incomplete work

- The new minimal cellular-automata scene has not yet been opened and resaved by
  the Unity Editor or exercised through the Unity Test Runner.
- `Intro` and `MainMenu` remain outside Build Settings, matching the prior setup.

## Next useful step

Open `CellularAutomataPrototype` in Unity, confirm the animated cave preview, then
run the Edit Mode and Play Mode suites. Open `IslandSurvivorPrototype` separately
to confirm that no cave preview is created there.
