# CF-2 boundary upgrades and CF-3 End route

## Status

The controlled continuous-phase preview now supports a real decision at the
frozen boundary. Phase survivor data is awarded once, a live upgrade can be
selected, the same retained run resumes at the next absolute tick, and an
explicit End route finalizes the current expedition. The player-facing
continuous expedition now has ten phases; manual inspection remains additional
to automated validation.

## What changed

- Added phase settlement keyed to the current phase index. The phase reward is
  the current player-survivor count, added once before either Skip or Purchase.
- Added boundary upgrade installation through Preview → Helper → Manager →
  Runner. The runner keeps its current grid, absolute tick and prior source;
  only the effective rules/options/loadout snapshot changes.
- Converted the existing legacy reward choices into the immutable snapshot
  contract when they are selected at a boundary, so the default scene can use
  the same provenance path as authored assets.
- Marked starting-energy and starting-food-reserve snapshots as launch-only;
  they remain visible but unavailable at a mid-run boundary.
- Added one-decision guards for phase purchases and Skip, plus runtime,
  contract and PlayMode coverage for same-run upgrade continuation and repeated
  clicks.
- Added an explicit End command to the Preview/ViewModel/XAML path. Stop still
  means discard and reset; End finalizes through the normal result flow.
- Continuous terminal completion now goes straight to expedition results. The
  old terminal reward screen is retained only for Developer Mode single runs;
  starting another expedition is an explicit results-screen action.

## Validation

- Managed runtime build: passed.
- Managed runtime test build: passed with the repository's existing obsolete
  API warnings.
- Managed PlayMode test build: passed with the repository's existing Unity API
  warnings.
- Full Unity EditMode/PlayMode rerun is still pending because the Unity Editor
  is open for manual inspection. Earlier artifacts remain historical and do not
  validate these latest boundary-upgrade changes:
  `artifacts/unity-continuation-editmode-20260905-000809` and
  `artifacts/unity-continuation-playmode-20260905-001049`.

## Manual check

In the open `CellularAutomataPrototype` scene, keep Developer Mode off and
start the default continuous flow. At the configured phase boundary the board
should freeze and the phase panel should show the survivor-data award, upgrade
buttons, Skip / Continue, Restart, Stop and End. Select a non-launch-only live
upgrade; the panel should close, the same board/run should continue, and the
upgrade should affect ticks after the boundary. Try clicking the same option
again after resuming; it must not charge or apply twice.

When the continuous run reaches its terminal tick, the panel should show
expedition results with no terminal upgrade buttons. The action to start again
should be labeled `START NEW EXPEDITION` and should only run after it is
selected.

Developer Mode can still disable continuous phases for the uninterrupted
single-run control.

## Remaining gate

CF-4 still owns versioned phase/expedition telemetry windows, phase result
serialization and Stat-Line meaning. Do not use this preview output as EX-010
research evidence until that contract and its producer/consumer checks are
implemented. Close the Unity Editor before running the full automated suite.
