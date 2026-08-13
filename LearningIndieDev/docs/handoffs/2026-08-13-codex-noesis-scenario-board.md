# Handoff: Runtime scenarios and species iconography

## Current branch

`codex/xaml-migration`

## What is now available

- The Cellular Automata Prototype scene exposes a runtime scenario dropdown in
  the Noesis settings screen.
- The scene list contains `ForestEdge`, `Wetland`, `OpenRange`, and
  `BaselineParity`, plus a `Legacy Defaults` option.
- Selecting a scenario resets the ready state and applies its authored
  `ScenarioDefinitionAsset` data before the next Start Simulation action.
- `Legacy Defaults` remains the scene default so the original simulation is
  not silently replaced by an authored scenario.
- Scenario assets compose reusable species assets, so each scenario can choose
  a different roster and diet/predator chain.

## Noesis board

- `SpeciesSimulationBoard` remains a single batched custom `FrameworkElement`;
  it does not create one UI visual per cell.
- Terrain is rendered first (green resource terrain or brown dirt), then the
  entity/resource icon is drawn on top.
- Each known species has its own cached vector silhouette. Examples include a
  fern frond, reed stalk, hare ears, deer antlers, snail shell, beetle shell,
  fox ears, owl tufts, stoat body, and wolf head.
- The board keeps one transform per grid cell. Reusing one mutable transform was
  a rendering bug that collapsed the entire board to one moving icon.
- Role colors remain consistent: plants green, herbivores blue, carnivores red.

## Important files

- `Assets/UI/HUD/Scripts/SpeciesSimulationBoard.cs`
- `Assets/UI/HUD/Scripts/SpeciesSimulationViewModel.cs`
- `Assets/UI/HUD/XAML/SpeciesSimulationShell.xaml`
- `Assets/Scripts/Game/Presentation/SpeciesSimulationPreview.cs`
- `Assets/Scripts/Game/CaveGeneration/CellularAutomataPrototypeRuntime.cs`
- `Assets/Data/CellularSimulation/Species/`
- `Assets/Data/CellularSimulation/Scenarios/`

## How to try it

1. Open `Assets/Scenes/CellularAutomataPrototype.unity`.
2. Enter Play Mode and wait for the Noesis settings screen.
3. Pick a scenario from the `Scenario` dropdown.
4. Press `START` and observe the selected roster and iconography.
5. Return to the settings screen and select another scenario to compare.

## Validation notes

- `git diff --check` passes.
- The earlier ScriptableObject parity check and 76-test Edit Mode run passed
  before the final iconography-only edits.
- Unity batch validation is currently blocked by headless Unity processes that
  are still holding the local project database. Do not delete Unity lock files
  while the editor or those processes are running.

## Suggested follow-up

- Replace hard-coded vector path strings with authored icon data only if visual
  iteration becomes frequent enough to justify an icon asset/tooling layer.
- Add a small in-game legend once more species are introduced; the current
  silhouettes are intentionally minimalist and role colors remain the fastest
  recognition cue.
