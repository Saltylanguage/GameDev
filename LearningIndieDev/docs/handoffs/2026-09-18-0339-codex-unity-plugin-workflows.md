# Unity plugin workflows

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-09-18-0339-codex-unity-plugin-workflows
- Owner: Codex
- Branch: ProjectMain
- Baseline commit: 34d16ce4
- Date: 2026-09-18
- Supersedes: none

## Summary

Established the project routing and verification workflow for the installed
Unity plugin. The project runners and graphics acceptance path worked, while a
direct full-suite run exposed one EditMode failure that must remain visible to
the relevant feature work.

## Changes

- Added [`UNITY_PLUGIN_WORKFLOWS.md`](../UNITY_PLUGIN_WORKFLOWS.md) and linked it
  from [`AGENTS.md`](../../AGENTS.md), `PROJECT_CONTEXT.md`, and `WORKING_STATE.md`.
- Recorded the Unity skill routing, Noesis and terrain constraints, audio and
  service gates, test/visual commands, and live-Editor CLI dependency boundary.
- No runtime code, Unity assets, or project dependencies were changed.

## Decisions and assumptions

- Noesis/XAML remains the player UI, and the cellular board remains the custom
  batched Noesis renderer. Tilemap/RuleTile procedures do not apply to it.
- `com.unity.pipeline` is absent. Do not add it solely for live-Editor plugin
  automation; reconsider only for a concrete recurring task and accepted
  dependency change.
- Use `CellSim.ps1` and the project visual runner as the canonical verification
  route. A separately passing PlayMode suite does not convert a failed combined
  suite into a pass.

## Validation

- `unity --version`: `1.0.0-beta.6`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\CellSim.ps1 -Command Test -Mode All`:
  EditMode produced 253 tests, 252 passed and 1 failed. The failure is
  `SaltyGame.Tests.SimulationManagerTests.ContinuousSkipPreservesWorldHistoryAndMetricsUntilTheSameAbsoluteTick`
  at `Assets/Tests/Runtime/SimulationManagerTests.cs:201`; expected
  `AwaitingDecision`, received `Complete`. Results and log:
  `artifacts/unity-tests-20260918-032007/EditMode-results.xml` and
  `artifacts/unity-tests-20260918-032007/EditMode.log`. The combined runner
  stopped after this failing EditMode suite, so this command did not run
  PlayMode.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\CellSim.ps1 -Command Test -Mode PlayMode`:
  31 passed, 0 failed, 2 expected graphics-only skips (33 total). Results and
  log: `artifacts/unity-tests-20260918-032319/PlayMode-results.xml` and
  `artifacts/unity-tests-20260918-032319/PlayMode.log`.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\CellSim.ps1 -Command Visuals -TestFilter 'SaltyGame.PlayModeTests.GalapagOSVisualAcceptanceTests.GalapagOSDesktopAndSimulationCaptureGameViewEvidence'`
  passed 1/1 at 1280x720. The same test passed 1/1 at 1920x1080 using
  `powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Invoke-UnityVisualEvidence.ps1 -TestFilter 'SaltyGame.PlayModeTests.GalapagOSVisualAcceptanceTests.GalapagOSDesktopAndSimulationCaptureGameViewEvidence' -ScreenWidth 1920 -ScreenHeight 1080`.
  Screenshots and logs are in
  `artifacts/visual-evidence-20260918-032623/` and
  `artifacts/visual-evidence-20260918-032914/`; captures were inspected.
- The current Editor process check found no running Unity Editor before using
  the guarded runners. They completed their preflight and retained reports.

## Risks and incomplete work

- The EditMode failure is real evidence, but its root cause is not established
  by this workflow pass. The test concerns continuous-skip state/history; inspect
  its fixture and the current completion transition before changing code.
- Two graphics-only PlayMode tests remain skipped in the no-graphics suite; the
  separate visual acceptance test covers the desktop/simulation capture only.
- The full combined Unity suite is not green. Existing S3-03 isolated PlayMode
  evidence remains bounded to its own handoff and does not resolve this failure.

## Next useful step

Triage the failing EditMode case in the relevant simulation-flow work, preserve
the current artifact evidence, then rerun `CellSim.ps1 -Command Test -Mode All`.
Keep the workflow guide's full-suite status red until that combined run passes.
