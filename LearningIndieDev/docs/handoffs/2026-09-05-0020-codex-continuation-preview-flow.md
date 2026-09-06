# CF-1 continuation preview flow

## Status

The controlled preview path is now wired on top of the CF-1 same-world
continuation lifecycle. Manual play testing is explicitly an additional
inspection pass; it does not replace the automated validation gate.

## What changed

- Added opt-in continuous-phase controls to the Developer Mode settings panel:
  total run ticks, phase length, and a continuous-phase toggle.
- Added a phase-boundary screen that keeps the board visible and offers
  Continue Without Upgrade, Restart, and Stop.
- Continue resumes the same `SimulationRunState`; the phase boundary does not
  award currency or apply an upgrade. Terminal completion still uses the
  existing reward/results flow.
- Added a PlayMode integration test covering phase pause, UI visibility, same-run
  continuation, and terminal completion.

## Validation

- Managed runtime build: passed.
- Managed test build: passed with existing obsolete-API warnings.
- Unity EditMode: 204 passed, 0 failed.
- Unity PlayMode: 15 passed, 0 failed, 1 graphics-only test skipped under
  `-nographics`.

Artifacts:

- `artifacts/unity-continuation-editmode-20260905-000809/EditMode-results.xml`
- `artifacts/unity-continuation-playmode-20260905-001049/PlayMode-results.xml`

## Manual check

In the open Unity project, load `CellularAutomataPrototype` and enter Play
mode. Continuous phases are the default player flow: keep Developer Mode off,
keep Randomize seed off for a repeatable check, and start the run. At tick 100
the board should remain visible while the phase-complete screen waits. Continue
Without Upgrade should resume that same run, and the normal terminal Rewards
screen should appear at tick 200.

To inspect the uninterrupted control, turn on Developer Mode, uncheck `Use
continuous phases (default)`, set Run ticks to `200`, and start again.

## Deferred

The boundary currently has no phase reward, currency settlement, live behavior
upgrade, or phase-specific Stat-Line semantics. Those remain later CF packages;
this slice is for flow and same-world validation only.
