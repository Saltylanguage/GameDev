# ProjectMain BevBranch integration

[Working state](../WORKING_STATE.md) | Status: complete

- Handoff schema: 1
- Handoff ID: 2026-10-14-1707-codex-projectmain-bevbranch-integration
- Owner: Codex
- Branch: codex/sim-merge-20261014
- Baseline commit: 3086bfb5
- Date: 2026-10-14
- Supersedes: none

## Summary

Merged Sim's `BevBranch` tip `bf0e53a6` with `ProjectMain`, including the
local S4 and journey checkpoint `3086bfb5` and the previously pushed profile
persistence and CellSim wrapper work. The integration is intended for
`ProjectMain` so the team can focus on simulation content and balance.

## Changes

- Preserved both sides of the shared working state and S4 control record.
- Resolved the gestation attribute registry against Sim's computed gestation
  value.
- Removed a byte-identical duplicate block of five gestation domain tests
  introduced when the branches combined.
- Updated the journey launch PlayMode test to create a loaded profile and
  restore the previous profile save after the test. The production profile
  guard remains active.

## Decisions and assumptions

- The local S4 and journey edits were included at the user's direction.
- Unity's regenerated importer and solution-file changes from the validation
  run were discarded from the worktree. The original staged asset files and
  GUIDs were preserved.
- The existing profile save format remains version 1; future progression
  fields can extend the current skeleton.

## Validation

- `CellSim Doctor`: healthy, with the integration project closed before tests.
- Clean Unity EditMode: 340 passed, 0 failed.
- Clean Unity PlayMode: 44 passed, 0 failed, 4 skipped.
- The focused journey launch PlayMode test passed independently before the
  complete rerun.
- `CellSim.Batch` and `CellSim.Workbench` Release builds passed with zero
  warnings and errors.
- The standalone runner self-test could not start from this Codex worktree:
  this host lacks the .NET 8 runtime, and process-local major-version
  roll-forward reached Windows Application Control, which blocked the DLL.
  Its Release build completed successfully.

## Risks and incomplete work

- Automated tests and builds do not replace a human visual pass of the merged
  terrain, artwork, journey, and profile flows in the Editor or player.
- The four skipped PlayMode cases remain skipped by their own test conditions.
- No new balance matrix or player build was run for this integration.

## Next useful step

After pulling `ProjectMain`, review the journey and terrain visually, then
start the next simulation content and balance pass using the merged tooling.
