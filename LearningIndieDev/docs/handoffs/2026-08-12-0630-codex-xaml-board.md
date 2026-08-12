# Handoff: Noesis board renderer prototype

**Branch:** `codex/xaml-migration`

## Delivered

- Added `SpeciesSimulationBoard`, a single Noesis `FrameworkElement` that
  renders the simulation grid with `DrawingContext.DrawRectangle`.
- Added the board to `SpeciesSimulationShell.xaml` and wired it to the current
  `SimulationRunState.Cells` through the ViewModel.
- Kept simulation logic UI-agnostic. The board only reads `Grid<SpeciesCell>`;
  stepping, rules, and metrics remain in the simulation/preview layers.
- Used one batched visual rather than one `ListBox` item or XAML element per
  cell, leaving a practical path for larger grids.

## Verification

- `dotnet build LearningIndieDev.slnx --no-restore`: passed with 0 errors.
- Unity Edit Mode: 75/75 passed.
- Unity Play Mode: 4/4 passed.
- The Noesis XAML asset imported successfully with the custom board namespace.
- End-to-end `CellSim Baseline -SeedStart 10100 -SeedCount 20` completed. The
  report is at
  `artifacts/cellular-experiment-20260812-061503/analysis.md` (20/20 runs,
  75/75 Edit Mode tests, and 4/4 Play Mode tests).

## Follow-up

- Add visual profiling once board dimensions or tick rates increase.
- If the board needs richer interaction, add hit testing/commands to this
  control rather than reintroducing per-cell visual trees.
