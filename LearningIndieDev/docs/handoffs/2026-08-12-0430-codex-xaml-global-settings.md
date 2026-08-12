# Handoff: Noesis shell global settings slice

**Branch:** `codex/xaml-migration`

## Delivered locally

- `SpeciesSimulationNoesisHost` attaches the authored
  `SpeciesSimulationShell.xaml` to the prototype camera and binds a
  `SpeciesSimulationViewModel`.
- The Noesis shell now exposes global run settings: grid dimensions, seed,
  deterministic/random mode, population bounds, duration, step interval, and
  starting species probabilities.
- `Apply Settings` validates and applies those values only while the preview is
  ready and before a session has started. It rebuilds the next run snapshot;
  active simulation state is not mutated.
- `Save Default` and `Start` are available from the shell. Running, paused,
  rewards, results, and reset controls remain command-bound through the
  ViewModel.
- Species-specific rule authoring remains in the existing IMGUI surface until
  its data contract is migrated deliberately; the shell has an explicit
  `Edit Species Rules (Legacy Panel)` handoff so the two authoring surfaces do
  not overlap.

## Verification

- `dotnet build LearningIndieDev.slnx --no-restore`: passed with 0 errors.
- `SpeciesSimulationShell.xaml`: XML parse passed.
- `git diff --check`: passed.
- Elevated closed-editor Unity validation passed: Edit Mode and Play Mode both
  completed with `Passed` and exit code 0.
- A temporary scene probe also passed: `CellularAutomataPrototype` resolves
  `SpeciesSimulationNoesisHost` and the `SpeciesSimulationShell` XAML asset.
- The first non-elevated run exposed a machine-level Unity cache permission
  issue (`CurlRequestCache.db`); elevated access resolved it without project
  changes. Licensing handshake warnings remain environmental and did not fail
  either test run.
