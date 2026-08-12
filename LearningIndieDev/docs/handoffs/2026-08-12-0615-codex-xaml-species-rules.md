# Handoff: Noesis species-rule authoring

**Branch:** `codex/xaml-migration`

## Delivered locally

- Added a plain `SpeciesRuleEditValues` contract in the runtime presentation
  layer. It keeps the UI bridge separate from the private preview draft and
  from Noesis types.
- Added ViewModel bindings for all current species-rule values: movement,
  attack, block, diet target/pattern, reproduction, energy, metabolism,
  perception, intelligence, crowding, wilt, food reserve, and seed drops.
- Added species selection for Plant, Herbivore, and Carnivore, with an explicit
  `Apply Species Rules` command.
- Applying rules is restricted to the ready/pre-session state. Values are
  parsed, clamped to the existing rule constraints, rebuilt into the next
  `SpeciesRules` snapshot, and never mutate an active run.
- The legacy IMGUI editor remains available only as a fallback through its
  explicit legacy-panel command.

## Verification

- `SpeciesSimulationShell.xaml` XML parse passed.
- `NoesisXaml.Load()` probe passed for the authored shell.
- `dotnet build LearningIndieDev.slnx --no-restore`: passed with 0 errors.
- Full Edit Mode and Play Mode Unity validation will run again after the slice
  is committed; Unity batch mode requires elevated local access on this machine
  because its machine-level cache is permission-restricted.
