# Journey Field Data reset at run end

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-15-1250-codex-journey-field-data-reset-at-run-end
- Owner: Codex
- Branch: ProjectMain
- Baseline commit: a7684874
- Date: 2026-10-15
- Supersedes: none

## Summary

Starting a new Forest Edge journey no longer carries leftover Field Data from
the completed run. This closes one concrete leak across the journey boundary
while keeping the established standalone expedition behavior.

## Changes

- `SpeciesSimulationPreview.PlayNextSimulation` carries currency only for a
  non-journey expedition; the next journey starts with zero Field Data.
- Added assertions to the existing journey PlayMode flow that data exists at
  the completed result and is zero after **Start New Journey**.
- Preserved other contributors' local journey-map and presentation changes
  outside this focused commit.

## Decisions and assumptions

- Field Data is run currency and expires at the end of a journey, even when
  the journey succeeds. Temporary Mutations already reset on the new journey.
- The older standalone continuous expedition retains its existing currency
  carryover until its separate economy is redesigned.

## Validation

- `CellSim.ps1 -Command Doctor`: healthy; project closed and license ready.
- Clean PlayMode `JourneyNodeRewardsAndSimulationsContinueTheSameBiome`: 1/1
  passed, 0 failed/skipped.
- Clean PlayMode `ContinuousPhaseDecisionResumesTheSamePreviewRun`: 1/1
  passed, 0 failed/skipped. An earlier mistyped filter selected zero tests;
  it is not counted as validation.

## Risks and incomplete work

- The test ran against this shared checkout, which also has other local
  journey-map edits. This commit stages only the Field Data boundary fix and
  its assertions.
- No profile wallet payout, permanent unlock trigger, phase-pass gate,
  balance value, or new Field Data purchase was added.

## Next useful step

Define the success-time deposit into a separate permanent wallet and the
first permanent unlock rule after Bevin selects the corresponding content.
Use the existing run ledger and profile save tests to verify no double deposit
or loss of permanent progression on failure.
