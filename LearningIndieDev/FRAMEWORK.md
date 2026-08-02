# Salty GameDev framework

The Bootstrap scene is the composition root. `GameRuntime` creates the runtime systems in one explicit startup path:

```text
Bootstrap -> GameRuntime -> WorldRuntime -> Player -> InteractionController -> ActivityController -> InventoryState -> HUD
```

## Layers

- `Assets/Scripts/Game/Core`: game state, startup ownership, and time flow.
- `Assets/Scripts/Game/World`: runtime entities, placeholder world construction, and player movement.
- `Assets/Scripts/Game/Input`: the only layer that reads Unity's Input System keyboard state.
- `Assets/Scripts/Game/Interaction`: target discovery and activity startup.
- `Assets/Scripts/Game/Activities`: plain C# activity contracts and rules. `WoodChoppingActivity` is the first client.
- `Assets/Scripts/Game/Inventory`: plain C# resource state and reward delivery.
- `Assets/Scripts/Game/Presentation`: the runtime HUD and player feedback.
- `Assets/Scripts/Game/Debug`: the F3 runtime diagnostics panel.

The Bootstrap slice intentionally uses Unity's built-in GUI for its HUD and
does not depend on the pulled Noesis package. The original Noesis manifest
entry referenced a machine-specific download path, so that optional UI
experiment remains isolated behind its existing compile guard until it has a
portable package setup.

## Adding another activity

Implement `IActivity` for the rules, then add an `IActivityTarget` component that creates it and applies its `ActivityResult`. Register that target from `WorldRuntime.Build()`. Player movement, input adapters, interaction discovery, activity lifecycle, inventory, bootstrap, and debug UI do not need to change.

The first three activities are wood chopping, berry gathering, and mining. Each uses the same timing input but owns its own rules and reward: gathering completes after three gathers, while mining breaks a six-health rock and awards stone.

## Current controls

- `WASD`: move
- `E`: start an available activity or cancel the active one
- `Space`: submit the current activity timing hit
- `Escape`: cancel the active activity
- `F3`: toggle diagnostics

## Editor validation

Use `Salty > Validate Bootstrap Scene` from the Unity editor. It checks that
the Bootstrap scene exists, is enabled in Build Settings, has one active
`GameRuntime` composition root, and no longer contains the removed giant
prototype.
