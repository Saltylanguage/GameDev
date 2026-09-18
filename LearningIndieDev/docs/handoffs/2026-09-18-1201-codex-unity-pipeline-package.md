# Unity Pipeline package

[Working state](../WORKING_STATE.md) | Status: complete

- Handoff schema: 1
- Handoff ID: 2026-09-18-1201-codex-unity-pipeline-package
- Owner: Codex
- Branch: ProjectMain
- Baseline commit: 34d16ce4
- Date: 2026-09-18
- Supersedes: 2026-09-18-0339-codex-unity-plugin-workflows.md

## Summary

Added Unity CLI's Pipeline package to this project and reran the guarded
validation. Per Josh's direction, the existing EditMode failure is documented
but does not block the package/workflow work; the separate PlayMode suite passes
with two expected graphics-only skips.

## Changes

- Installed `com.unity.pipeline` version `0.7.0-exp.1` with
  `unity --no-banner --non-interactive pipeline install --project-path "D:\GameDev\GameDev\LearningIndieDev"`.
- Unity resolved the package into `Packages/manifest.json`,
  `Packages/packages-lock.json`, and `Library/PackageCache`.
- Updated [`UNITY_PLUGIN_WORKFLOWS.md`](../UNITY_PLUGIN_WORKFLOWS.md) and
  [`WORKING_STATE.md`](../WORKING_STATE.md) to describe the installed package
  and current test state.

## Decisions and assumptions

- The package was explicitly requested and is now a project dependency. It
  enables live Editor commands only while the target Editor is running.
- Josh directed that the EditMode failure not block progress because current
  game flow no longer waits for a decision after the final phase. Keep the
  observed test result visible without treating it as a blocker or claiming the
  test suite passed.
- The test runners use batch Unity with the Editor closed; do not run them
  against a GUI Editor session.
- Live GUI Editor connection was not verified in this run. Batch Unity loaded
  the package and shut down its Pipeline server normally.

## Validation

- Package install exited 0 and reported `com.unity.pipeline 0.7.0-exp.1`.
  The manifest and lockfile both resolve that version from the Unity registry.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\CellSim.ps1 -Command Test -Mode All`:
  EditMode 252/253, one failed. The failure is
  `SaltyGame.Tests.SimulationManagerTests.ContinuousSkipPreservesWorldHistoryAndMetricsUntilTheSameAbsoluteTick`
  at `Assets/Tests/Runtime/SimulationManagerTests.cs:201`; expected
  `AwaitingDecision`, received `Complete`. Artifact:
  `artifacts/unity-tests-20260918-115732/EditMode-results.xml` and
  `artifacts/unity-tests-20260918-115732/EditMode.log`. The combined runner
  stopped before PlayMode because EditMode failed.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\CellSim.ps1 -Command Test -Mode PlayMode`:
  31 passed, 0 failed, 2 expected graphics-only skips (33 total). Artifact:
  `artifacts/unity-tests-20260918-115904/PlayMode-results.xml` and
  `artifacts/unity-tests-20260918-115904/PlayMode.log`.
- Both commands passed the guarded Unity preflight. No GUI Editor was open at
  the start of either run.

## Risks and incomplete work

- The EditMode failure is unchanged from the run before this package was added;
  package installation did not resolve it. This assertion occurs at tick 100
  with internal `phaseLengthTicks: 100` and `targetTicks: 200` fixture settings;
  these values do not represent the current player-facing game flow. It remains
  recorded, but is non-blocking by user direction. The test stops at the status
  assertion before checking world history or metrics.
- Live GUI Editor commands (`unity status`, `unity command`, or `eval`) were not
  exercised; only package resolution and batch-mode loading were confirmed.
- The two graphics-only PlayMode tests remain skipped in the no-graphics suite.

## Next useful step

No further work is required to close the package/workflow setup. If simulation
test maintenance enters scope later, reconcile this assertion with the current
phase flow and then rerun `CellSim.ps1 -Command Test -Mode All`. Smoke-test
`unity status` against the project Editor the next time a task needs live Editor
control; use the project-specific skill and `--caller plugin --skill <name>`
for commands.
