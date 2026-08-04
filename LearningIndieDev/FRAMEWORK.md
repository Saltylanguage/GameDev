# Salty GameDev framework

The Bootstrap scene is the composition root. `GameRuntime` creates the runtime systems in one explicit startup path:

```text
Bootstrap -> GameRuntime -> WorldRuntime -> Player -> InteractionController -> ActivityController -> InventoryState/SurvivalState/CampState -> HUD
```

## Layers

- `Assets/Scripts/Game/Core`: game state, startup ownership, and time flow.
- `Assets/Scripts/Game/World`: runtime entities, placeholder world construction, and player movement.
- `Assets/Scripts/Game/Input`: the only layer that reads Unity's Input System keyboard state.
- `Assets/Scripts/Game/Interaction`: target discovery and activity startup.
- `Assets/Scripts/Game/Activities`: plain C# activity contracts and rules. `WoodChoppingActivity` is the first client.
- `Assets/Scripts/Game/Inventory`: plain C# resource state and reward delivery.
- `Assets/Scripts/Game/Survival`: plain C# hunger, energy, and shared activity-cost rules.
- `Assets/Scripts/Game/Camp`: persistent camp state plus campfire and shelter actions.
- `Assets/Scripts/Game/Progression`: current objective progression and the authored Day 2 storm scenario.
- `Assets/Scripts/Game/Presentation`: the runtime HUD and player feedback.
- `Assets/Scripts/Game/Debug`: the F3 runtime diagnostics panel.

The Bootstrap slice intentionally uses Unity's built-in GUI for its HUD and
does not depend on the pulled Noesis package. The original Noesis manifest
entry referenced a machine-specific download path, so that optional UI
experiment remains isolated behind its existing compile guard until it has a
portable package setup.

## Adding another activity

Implement `IActivity` for the rules, then add an `IActivityTarget` component that creates it and applies its `ActivityResult`. Register that target from `WorldRuntime.Build()`. Player movement, input adapters, interaction discovery, activity lifecycle, inventory, bootstrap, and debug UI do not need to change.

The first activities are wood chopping, berry gathering, mining, campfire building, shelter building, and cooking. Gathering, chopping, and mining use timing input; cooking and building progress automatically. The shared `SurvivalState` charges each activity's configured hunger and energy cost. Activities advance Morning to Afternoon to Night, but a new day begins only when the player intentionally sleeps at the campfire. After the campfire is built, the current progression scenario forecasts a Day 2 storm. A four-wood shelter prevents the storm's otherwise recoverable energy/hunger cost; this is an authored scenario, not a general weather system.

## Current controls

- `WASD`: move
- `E`: start an available activity or cancel the active one
- `Space`: submit the current activity timing hit
- `Escape`: cancel the active activity
- `F`: eat cooked food first, then raw berries, while at a built campfire
- `R`: sleep at a built campfire and begin the next morning
- `F3`: toggle diagnostics

## Editor validation

Use `Salty > Validate Bootstrap Scene` from the Unity editor. It checks that
the Bootstrap scene exists, is enabled in Build Settings, has one active
`GameRuntime` composition root, no longer contains the removed giant
prototype, and has the required survival/camp runtime scripts.
