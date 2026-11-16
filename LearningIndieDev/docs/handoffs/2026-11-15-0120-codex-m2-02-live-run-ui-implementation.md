# M2-02 live-run UI implementation

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-11-15-0120-codex-m2-02-live-run-ui-implementation
- Owner: codex
- Branch: codex/s6-predator-first
- Baseline commit: f4c1ee34
- Date: 2026-11-15
- Supersedes: none

## Summary

The local M2-02 implementation now lets a player click a board cell to inspect
the current Hare, Fox, food, or terrain state. The Field Ledger replaces its
static collection mockup with that readout and states the Hare survival goal.
The previously decorative speed controls now request 0.5x, 1x, 2x, or 4x
simulation cadence. This is implementation only: Noesis import, actual pointer
mapping, layout at target resolutions, and player comprehension are unverified.

## Changes

- `SpeciesSimulationBoard` maps a click through the current fitted/zoomed board
  to a cell and draws an outline; dragging still pans. Both Noesis hosts connect
  this event to the existing `VM_SimulationBoard` selection state and set its
  data context for the readout.
- `VM_SimulationBoard` formats the selected cell's current health/energy/state
  or food/terrain contents, updates it with each snapshot, and clears selection
  on a new run or tick rollback.
- `V_Window_CellSimulation.xaml` replaces placeholder collection counts with
  cell inspection, displays the current goal and next action, and binds all four
  speed buttons. `VM_SimulationShell` supplies their command and selected states.
  `SpeciesSimulationPreview` scales time passed to the existing fixed-step
  simulation manager; no simulation rules or step size changed.
- `GalapagOSVisualAcceptanceTests.LiveBoardInspectionAndSpeedControlsUseCurrentRun`
  covers selection text, its Noesis binding, new-run reset, objective text, and
  supported/unsupported speed requests. The test is authored but not run.

## Validation and limits

- Static XAML XML parsing and `git diff --check` passed. Unity compilation,
  Noesis import, PlayMode, screenshots, 1x/4x play, and human readability review
  were not run in this implementation-first pass.
- The result-screen and dense-feedback edits in the shared checkout belong to
  the separate M2-08/M2-07 task. This pass preserved them and did not change
  `.meta` files, GUIDs, serialized fields, art, or authored balance.
- The click coordinate conversion under Viewbox scaling, pan, and wheel zoom
  needs actual runtime inspection. The ledger's longest reaction/inspection
  combinations need review at 1280x720 and 1920x1080. Keyboard inspection and
  broader accessibility acceptance remain in M2-10.

## Next useful step

In the November 16 shared validation batch, run the focused
`LiveBoardInspectionAndSpeedControlsUseCurrentRun` PlayMode case and the existing
desktop visual capture. Click cells at default and high zoom, pan, pause, and
restart. Check the readout and selection outline against the rendered cell,
check 0.5x/1x/2x/4x elapsed-tick cadence, and inspect crowded warning states at
both target resolutions. Keep M2-02 open until a player can identify the Fox
pressure, inspect relevant state, and understand the current objective and
next action without developer guidance.
