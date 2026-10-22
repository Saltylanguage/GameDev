# mortality-attribution-and-early-hare-diagnosis

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-21-0431-codex-mortality-attribution-and-early-hare-diagnosis
- Owner: codex
- Branch: codex/audio-direction-and-sourcing
- Baseline commit: 6ffb7b10
- Date: 2026-10-21
- Supersedes: none

## Summary

Fixed the shared-tile mortality attribution defect and used existing screening
evidence plus one isolated feeding fixture to diagnose the early Hare decline.
No production balance values changed and no new ecological panel ran.

## Changes

- SpeciesSimulationMetrics.RecordDeath accepts an explicit resource-layer flag.
  Wilt, resource-consumption and population-limit callers pass the removed layer.
  Resource events no longer borrow an occupant's species/entity/age; actual
  creature deaths retain their identity and context.
- Added six mortality cases and one food-rich feeding-onset case to
  SpeciesDomainTests. Existing standalone-plant tests remain in the same class.
- Added `docs/Research/2026-10-21-Hare-early-loss-diagnosis.md`, updated tooling
  guidance and working state, and linked the follow-up from the original report.
- Derived diagnosis artifacts/script are under
  `artifacts/hare-loss-diagnosis-20261021/`; original reports remain immutable.

## Decisions and assumptions

- Josh authorized fixing the reporting issues and diagnosing early Hare losses.
  This includes targeted verification. No new balance-panel approval was inferred.
- Empty Genome's blank catalog fingerprint is intentional; checked but unchanged.
- Diagnosis analyzes the already approved 48-run panel. Per-tick births are
  reconstructed from population changes and actual death events; totals reconcile
  with each run's Hare births and age-derived dead cohorts.
- Source/data values put starting Hare energy at 12, feeding onset below 6,
  mating threshold/cost at 24, metabolism 1/8 ticks and gestation at 48 ticks.
  No field shape, report-schema or production-value change was required.

## Validation

- Pre-fix targeted regressions reproduced 3/3 failures:
  `artifacts/unity-tests-20261021-041806/EditMode-results.xml`.
- Corrected simulation class initially passed 151/151:
  `artifacts/unity-tests-20261021-042211/EditMode-results.xml`.
- Final class, including the isolated feeding fixture, passed **152/152**:
  `artifacts/unity-tests-20261021-043010/EditMode-results.xml`. No PlayMode or
  full-suite acceptance claim is made.
- Test-only compilation failures were corrected: internal API access now uses
  the existing reflection convention; a nonexistent pattern helper was replaced
  with the verified GridPattern constructor. Failure logs are retained in
  `unity-tests-20261021-041904/` and `unity-tests-20261021-042937/`.
- Aborted compiler batches left zero-byte lock markers. Initial cleanup was
  rejected by auto-review because CLI MCP processes were mistaken for an active
  Editor. Read-only exact executable/version evidence, process identities,
  failed-batch marker timestamps and exclusive file access established no Editor
  or file owner; reviewed recovery removed only those task-created stale markers.
  No active Editor, MCP server or Hub process was stopped.
- Standalone project build passed with zero warnings/errors and the portable
  regression self-test passed (`dotnet --roll-forward Major` using installed
  newer runtime; no dependency/runtime install).
- Retained-report analysis checks all 48 Hare birth totals and cohorts;
  zero births occur before tick 101. First birth is tick 106 or later. The
  predator-free 3×3 fixture verifies no successful feeding in ticks 1–56 and
  successful feeding at tick 57 with the current energy thresholds.

## Risks and incomplete work

- Old raw reports still contain the mortality bug. Use their documented
  derivedDeaths correction; this patch does not rewrite old evidence.
- Predation causes most early deaths while the feeding/reproduction gates delay
  replacement. Improving outcome by changing those gates remains an untested
  intervention, not a production recommendation already accepted by Josh.
- A separate source-level targeting asymmetry rejects creatures sharing plant
  resource tiles in IsDietTarget. Its ecological magnitude and intended design
  remain unconfirmed; it was not changed or claimed to cause early Hare deaths.
- No new ecological runs, asset changes, player playtest, commit/push, branch
  switch, external messages or Trello mutations. Preserve all pre-existing edits.

## Next useful step

Josh reviews a bounded one-variable comparison of feeding onset (current 6
versus an isolated candidate above the 24-energy mating threshold), keeping
starting energy, gestation and Fox pressure fixed. Resolve shared-tile target
intent with a focused regression before broad balance tuning. See the diagnosis
for observed facts, verified mechanism and limits.
