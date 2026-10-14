# CellSim wrapper restoration

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-14-1533-codex-cellsim-wrapper-restoration
- Owner: Codex
- Branch: ProjectMain
- Baseline commit: 089a8522
- Date: 2026-10-14
- Supersedes: none

## Summary

Restored the CellSim command path needed for repeatable S4 balance work. A merge
had removed the shared Unity CLI, Pipeline, execution-lane, and NUnit helpers
while leaving their callers intact. The developer diagnostics and the worker
batch preflight now have separate entry points.

## Changes

- Restored the missing helpers from their original `37f01ddb` implementation:
  CLI invocation and JSON parsing, Pipeline command payloads, project state and
  lane selection, and NUnit result summary.
- Kept the worker process-tree cleanup. Restored its batch invocation parameters
  and preflight result fields; `Test-UnityPreflight.ps1` calls that batch path.
- `CellSim Doctor` again uses fast CLI and license diagnostics. The worker guard
  ignores background Unity CLI processes while still rejecting Editor
  executables or processes whose path cannot be inspected.

## Decisions and assumptions

- `Auto` chooses a ready Pipeline Editor or a closed-project Clean run; locked,
  busy, Safe Mode, and unreachable states still stop without deleting locks.
- This change repairs execution tooling only. It does not approve an S4 balance
  fixture, Mutation schedule, or production value.

## Validation

- PowerShell parsed `UnityTooling.ps1` and found every restored function.
- The real `Auto` probe chose `Clean` for the closed project; focused lane
  assertions covered ready, closed, and unreachable routing.
- Elevated `CellSim Doctor`: CLI doctor and license exit 0, `Healthy=True`,
  project state `Closed`; logs in `artifacts/unity-doctor-20261014-153048/`.
- Elevated Clean `CellSim Run`: seed 10100, Forest Edge, Hare, five ticks;
  report and provenance manifest in
  `artifacts/cellular-experiment-20261014-153112/`.
- Elevated Clean `CellSim Test`: the focused JSON/CSV EditMode test passed 1/1;
  NUnit XML in `artifacts/unity-tests-20261014-153143/`.
- Elevated worker `Test-UnityPreflight.ps1`: passed with a license probe log in
  `artifacts/unity-preflight-20261014-153250/`.
- `git diff --cached --check` passed. The handoff validator still reports 11
  existing errors in older notes, with none in this note.

## Risks and incomplete work

- The Live Pipeline lane and visual evidence wrapper were not exercised because
  no ready Editor was connected. Run those checks when an Editor is available.
- The smoke experiment's manifest records a dirty source tree. It proves tool
  execution, not balance outcomes; existing unrelated S4 and journey edits in
  the shared checkout were left untouched.

## Next useful step

Use the restored wrapper for a named S4 fixture and matched-seed comparison
once the authored content and schedule are approved; review the retained
manifest and report before drawing balance conclusions.
