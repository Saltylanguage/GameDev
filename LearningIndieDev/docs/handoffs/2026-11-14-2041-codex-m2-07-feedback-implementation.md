# M2-07 feedback implementation

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-11-14-2041-codex-m2-07-feedback-implementation
- Owner: codex
- Branch: codex/s6-predator-first
- Baseline commit: f4c1ee34
- Date: 2026-11-14
- Supersedes: none

## Summary

Local implementation slice for M2-07 dense-activity feedback. The existing
board, right-hand reaction card, and birth chime now consume bounded cues and
grouped completed-tick activity. This is ready for review, not M2-07 acceptance:
Unity execution, crowded 1x/4x viewing, and player readability review remain
for the EOD/S6-S7 gates.

## Changes

- `VM_SimulationShell` counts recorded Hare combat/starvation/other deaths and
  Hare/Fox births since the previous rendered tick. A Hare loss takes priority
  over routine births or tracked Fox hunting; a zero-Hare completed population
  snapshot changes the title to `HARES GONE`. Births concurrent with a loss are
  included in the warning, while suppressed counts remain in five bounded
  counters until the panel can show them. A lower-priority cue cannot displace
  a live warning. A pause flushes a pending loss warning; routine births remain
  behind danger. The birth chime plays only when a birth card is presented.
- Both Noesis hosts use this one reaction path and clear its state on a new run
  or tick rollback. The board projection limits birth flourishes to eight per
  tick, preferring the played species; the card still uses the exact event
  counts. A tracked Fox hunting flourish uses its recorded transition location
  only while the same Fox is still hunting there on the rendered grid.
- `SpeciesPresentationPlayModeTests.RunFeedbackGroupsSkippedTicksAndPrioritizesRecordedHareLosses`
  covers grouped skipped ticks, mixed species, loss priority, extinction title,
  delayed routine births, reset, the board cue cap, and stale Fox positions.

## Decisions and assumptions

- Combat deaths are labelled `in combat`, not `killed by Fox`: the death record
  contains cause and location but does not by itself identify an attacker.
- No new simulation rule, Mutation balance, audio asset, source art, event bus,
  or result-screen explanation was added. M2-08 owns the final cause and
  contribution explanation.
- A small fixed number of board birth flourishes is deliberate; the card
  reports the full recorded counts when many births occur together.

## Validation

- Read-only caller and event-schema audit completed for both Noesis hosts,
  `VM_SimulationBoard`, `VM_SimulationShell`, birth/death records, tracked
  behavior transitions, and completed population snapshots.
- `git diff --check` passed on the implementation files during the pass.
- Unity compile, EditMode, PlayMode, visual capture, and seeded play were **not
  run**, following the implementation-first validation plan.

## Risks and incomplete work

- The implementation is local and uncommitted on `codex/s6-predator-first`,
  alongside pre-existing M2-04, art, XAML, documentation, scene, and artifact
  changes. No Trello card was modified.
- The board still has no new positional combat, feeding, or escape flourish;
  VF-01/VF-02 and later M2-07 acceptance can add only cues backed by precise
  resolved facts. The panel hierarchy at 1280x720 and 1920x1080 is unreviewed.
- Pending routine counts need natural pause/end-run review. The focused test
  exercises projection/reset logic but cannot establish player readability or
  actual Noesis import, audio behavior, or scene timing.

## Next useful step

In the next validation batch (target November 16), run these focused checks from
`LearningIndieDev`:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\CellSim.ps1 -Command Test -Mode PlayMode -TestFilter 'SaltyGame.PlayModeTests.SpeciesPresentationPlayModeTests.RunFeedbackGroupsSkippedTicksAndPrioritizesRecordedHareLosses'
powershell -NoProfile -ExecutionPolicy Bypass -File .\CellSim.ps1 -Command Visuals -TestFilter 'SaltyGame.PlayModeTests.GalapagOSVisualAcceptanceTests.GalapagOSDesktopAndSimulationCaptureGameViewEvidence'
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Invoke-UnityVisualEvidence.ps1 -TestFilter 'SaltyGame.PlayModeTests.GalapagOSVisualAcceptanceTests.GalapagOSDesktopAndSimulationCaptureGameViewEvidence' -ScreenWidth 1920 -ScreenHeight 1080
```

Inspect both captures and play a representative Hare route at 1x and 4x with
crowded births/losses, pause, resume, and reset. Check that loss warnings stay
readable, birth effects/chimes stay bounded, and no old hunting location is
highlighted. Add the agreed full suites and the focused M2-04 checks in the
shared batch. Do not mark M2-07 complete before player review and an
accept/revise decision.
