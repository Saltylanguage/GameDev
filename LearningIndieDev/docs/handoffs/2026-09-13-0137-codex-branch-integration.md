# branch-integration

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: Codex
- Branch: BevBranch
- Baseline commit: 06c4bc8
- Date: 2026-09-13

## Summary

Merged `origin/codex/simulation-window-production` at `993a844` into the shared
`BevBranch` baseline `06c4bc8`. This brings Salty's GalapagOS/UI, image-resource,
roadmap, and diagnostic-tooling work into the branch while retaining the current
Bev simulation and upgrade work.

## Changes

- Resolved the sole textual conflict in `V_Panel_SimulationShell.xaml`.
- Kept the newer in-board running controls from the incoming UI and retained
  the local reward-card text style.
- Removed the incoming obsolete `BevExperimentalFeaturesEnabled` checkbox. The
  active Bev behavior already enables those features through `ApplySettings`,
  so the incoming binding had no ViewModel property and caused Noesis errors.

## Decisions and assumptions

- Preserve `BevBranch`'s always-enabled experimental feature behavior; do not
  reintroduce the legacy toggle without a separate product decision.
- The incoming Unity `.meta` files and historical Markdown contain their own
  trailing-whitespace warnings. They were not reformatted during integration.

## Validation

- `tools/Test-StudioPolicy.ps1` passed: 6 guidelines, 6 rules, alert-only mode.
- Unity 6000.4.6f1 batch preflight completed successfully.
- EditMode completed 239/239 passing. Artifact:
  `artifacts/unity-tests-20260913-013155/EditMode-results.xml`.
- The first PlayMode run exposed a Noesis binding failure for the obsolete
  checkbox; after the narrow XAML repair, the binding error is absent.
- The repaired PlayMode run completed 14 passed, 12 failed, 1 skipped. Artifact:
  `artifacts/unity-tests-20260913-013501/PlayMode-results.xml`.
  The remaining failures assert the older Ready/Wetland setup while the shared
  scene/runtime starts Forest Edge. They are not compilation or conflict-marker
  failures and require a separate test-fixture/launch-contract decision.

## Risks and incomplete work

- PlayMode is not green. No visual runtime acceptance was performed.
- The remaining PlayMode expectation mismatch must be triaged before treating
  the integrated UI/scene behavior as release-ready.

## Next useful step

Review the merged Forest Edge launch contract, then update or isolate the
obsolete PlayMode fixtures under a separately approved test-maintenance task.
