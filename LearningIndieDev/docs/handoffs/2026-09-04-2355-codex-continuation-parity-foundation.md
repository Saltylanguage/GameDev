# CF-1 continuation parity foundation

[Working state](../WORKING_STATE.md) | Status: in-progress

- Owner: Josh / Codex
- Branch: NF/ConsecutiveRuns
- Baseline commit: b8411526
- Date: 2026-09-04

## Summary

The first CF-1 runtime slice is implemented. A configured run can now stop at
an exact phase boundary, preserve the same evolving world and accumulated
evidence, continue without an upgrade, or end explicitly. Default fresh-run
behavior remains unchanged.

## Changes

- Added `AwaitingDecision` to the simulation lifecycle.
- Added opt-in continuous phases with absolute phase ticks and phase metadata.
- Preserved the runner's prior grid across a decision boundary and cleared the
  manager's leftover frame time when a boundary is reached.
- Added manager/helper forwarding for boundary notification, no-upgrade
  continuation, and explicit end.
- Allowed restart and stop from a frozen decision boundary.
- Updated paired-runner advancement guards for the new frozen state.
- Added parity, boundary-overshoot, and restart tests.

## Decisions and assumptions

- Continuous phases are opt-in through `SimulationRunState.ConfigureContinuousPhases`;
  existing non-continuous runs still complete using their existing duration or
  target-tick rules.
- A configured positive target tick remains the terminal tick. Intermediate
  phase boundaries freeze the run; the terminal tick completes it.
- Upgrade application, rewards/currency, player-facing phase UI, and telemetry
  window semantics remain deferred to their planned CF-2 through CF-4 packages.

## Validation

- `dotnet build LearningIndieDev/SaltyGame.Runtime.csproj` — passed; existing
  obsolete-API warnings only.
- `dotnet build LearningIndieDev/SaltyGame.Tests.csproj` — passed; existing
  obsolete-API warnings only.
- `dotnet build LearningIndieDev/SaltyGame.PlayMode.Tests.csproj` — passed;
  existing Unity API warnings only.
- Unity EditMode suite — 204 passed, 0 failed.

## Risks and incomplete work

- The new lifecycle is not yet wired into the current preview's reward/results
  flow; that is intentionally the next integration package, not part of this
  domain slice.
- Full A/B experiment evidence and the live-upgrade probe are not yet run.
- The working tree contains other uncommitted project changes; this slice has
  not been committed or pushed.

## Next useful step

Review this domain seam, then wire one controlled preview path to use the same
runner across `100 → Skip → 100` before adding live upgrade behavior or new
telemetry fields.
