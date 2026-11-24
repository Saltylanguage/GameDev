# M2 Q21 test route visibility

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-11-18-1215-codex-m2-q21-test-route-visibility
- Owner: Codex
- Branch: codex/m2-species-scenario-select
- Baseline commit: 6f672b14
- Date: 2026-11-18
- Supersedes: none

## Summary

Josh chose to hide the Field Data test route from normal players after Sim's
Q21 navigation finding. The normal Journey remains the player path; the test
route remains available in Developer Mode for research and PlayMode fixtures.

## Changes

- Gate the journey-map test-route button on `DeveloperMode` in
  `VM_SimulationShell`.
- Check player and developer visibility in the existing PlayMode route test.
- Record the decision and remaining manual check in the M2 comparison and
  working-state notes.

## Decisions and assumptions

- The underlying test-route command remains callable by developer fixtures;
  only its normal player entry is hidden. No Field Data save or route reset
  behavior changed.
- The independent questionnaire answers remain historical evidence. Q21 is
  ready for a fresh player check, not accepted or closed.

## Validation

- Clean PlayMode test
  `SpeciesPresentationPlayModeTests.FieldDataTestRouteOnlyAppearsInDeveloperModeBeforeFirstSimulation`:
  1/1 passed, `artifacts/unity-tests-20261118-121413/PlayMode-results.xml`.
  It checks hidden player visibility, visible developer access, and the existing
  route start flow.
- `git diff --check` passed.

## Risks and incomplete work

- A fresh-profile player check is still needed to confirm the test-route
  button is absent and the authored Journey remains reachable.
- A developer who intentionally enters the test route still lacks an in-place
  return to the authored Journey. The user chose to hide that route from
  normal players, so this does not block the player path.

## Next useful step

Have Josh or Sim check the fresh-profile Journey entry on this branch and
record the Q21 result. Keep M2 and M2-10 open until human validation.
