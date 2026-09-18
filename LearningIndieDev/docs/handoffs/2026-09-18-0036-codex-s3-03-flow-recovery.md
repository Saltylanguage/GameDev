# S3-03 flow recovery

[Working state](../WORKING_STATE.md) | Status: complete

- Handoff schema: 1
- Handoff ID: 2026-09-18-0036-codex-s3-03-flow-recovery
- Owner: Codex
- Branch: ProjectMain
- Baseline commit: 52ce0430
- Date: 2026-09-18
- Supersedes: none

## Summary

S3-03 flow and recovery changes align the simulation with the accepted
six-round expedition contract. Unity imported and compiled the project, and
the focused End/cancel regression plus the full no-graphics PlayMode suite
passed in an isolated copy of the current project.

## Changes

- Continuous player expeditions now run six phases, with UI copy updated to
  match.
- End now opens a confirmation dialog. Opening it pauses a running simulation;
  cancelling resumes only if the run was previously running.
- Early endings and player-species extinction forfeit available field data;
  extinction ends a continuous run immediately. Results distinguish failure,
  early end, and completion.
- Starting another expedition clears temporary Mutations while keeping earned
  field data and the frozen Genome snapshot. Reopening Simulation after Results
  starts one new run; opening it again during the active run does not replace
  that run.
- Added PlayMode coverage for end/cancel, phase-boundary forfeiture, immediate
  extinction, new-expedition reset, repeated app opening, and cancelling while
  already paused. Existing Results-window and host changes remain in the local
  change set.
- Fixed the End command's `CanExecuteChanged` notification so it becomes
  available when a run starts.
- Corrected both serialized XAML references in `CellularAutomataPrototype`:
  the scene had assigned `RD_SimulationShellResources.xaml` as the view root
  instead of `V_Window_CellSimulation.xaml`.

## Acceptance coverage

- Lab launch and return: `LabPlayModeTests.ExpeditionLaunchLoadsSimulationWithImmutableRequest`
  and `LabPlayModeTests.SimulationWindowCloseIsDisabledDuringRunAndReturnsToLabAfterResults`.
- Choice cadence and result states: `SpeciesPresentationPlayModeTests` covers
  five decision boundaries, purchase/continue, successful completion, early
  End, and immediate extinction failure.
- Recovery: `SpeciesPresentationPlayModeTests.ResultsActionsStartTheNextExpedition`
  and `GalapagOSVisualAcceptanceTests.ReopeningSimulationAfterResultsStartsANewRun`
  cover reset/new expedition, Mutation reset, currency carry/forfeiture, and
  repeated opening. The End prompt test covers running, paused, and phase
  decision cancellation.
- The focused End/cancel test passed 1/1. The complete PlayMode suite passed
  31/33, with 0 failures and two expected graphics-only sprite tests skipped.
- The suite includes the Lab launch/return, Results re-entry, phase cadence,
  early End, extinction, and new-expedition reset cases.
- This S3-03 validation covers flow and recovery, not the separate offer-count
  requirement: S3-02 specifies three Mutation options at each of five decision
  boundaries, while the current offer path and an existing test expose two.
  S3-04 records that as an open product/implementation decision; S3-03 does not
  close that gap or the Sprint 3 gate.

## Decisions and assumptions

- S3-02's six-phase player contract is authoritative; the old ten-phase preview
  count was inconsistent with it.
- Mutations are expedition-scoped. Awarded field data carries to the next
  in-memory expedition; profile persistence remains out of scope.
- Results re-entry starts a new run only through the explicit
  `PlayNextSimulation()` lifecycle call. Active-run view changes retain the
  same simulation state.

## Validation

- `git diff --check` passed.
- PowerShell XML parsing confirmed `V_Window_CellSimulation.xaml` is
  well-formed XML.
- `dotnet build SaltyGame.PlayMode.Tests.csproj` compiled the Unity-generated
  Runtime and PlayMode test assemblies with 0 errors.
- Unity PlayMode reports are retained at
  `artifacts/s3-03-test-results-20260918/focused-after-scene.xml` and
  `artifacts/s3-03-test-results-20260918/PlayMode-full.xml`.
- The initial focused run exposed the stale scene-to-resource-dictionary XAML
  reference. The scene link was fixed, then the focused test passed and the
  full PlayMode suite completed with 31 passes, 0 failures, and two expected
  graphics-only skips.
- Unity ran against an isolated copy because three editor processes were
  already open. The copy used the same source files and the same scene
  reference correction now present in the project.
- The new handoff passed the handoff validator in isolation with no warnings.
- The repository-wide handoff validator reports one pre-existing error:
  `2026-09-17-1613-codex-sprint-3-kickoff.md` uses unsupported status
  `complete`; that historical note was left unchanged.
- Two sprite initialization tests require a graphics-capable player and were
  skipped by the `-nographics` run. No graphics review is claimed here.

## Risks and incomplete work

- The repository also has a separate S3-01 post-terrain-fix Unity validation
  gate; that evidence predates the current local terrain correction.
- The S3-03 Trello card was already in `✅ Done` and marked complete.

## Next useful step

Push the validated changes and update the completed S3-03 Trello card with the
commit and test evidence. Keep the separate S3-01 post-terrain validation
follow-up open.
