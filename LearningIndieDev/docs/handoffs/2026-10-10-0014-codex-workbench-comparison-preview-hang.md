# workbench-comparison-preview-hang

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-10-0014-codex-workbench-comparison-preview-hang
- Owner: codex
- Branch: BevBranch
- Baseline commit: 6da9ba06
- Date: 2026-10-10
- Supersedes: none

## Summary

Investigated Bevin's third-comparison hang and hardened the preview update path.
The OS confirms a hang; the exact freeze was not reproduced in a separate
test process. Do not describe this as a proven root-cause fix. Existing dropdown
readability changes remain intact and uncommitted.

## Changes

- Remove redundant condition-grid SizeChanged -> AutoResizeRows callback.
  Suspend layout/automatic row sizing while rebuilding the capped200-row preview,
  then restore automatic sizing once. Existing filtering and combinations stay.
- Invalid setups clear both old preview rows and headers. Budget messages show
  the actual requested conditions and runs instead of only a generic ceiling.
- Native regression adds both vision comparisons and Plant energy through the
  real controls, tries Show all while invalid, edits to a valid value and back,
  removes the third comparison and verifies recovery. Covers10/15-value ranges,
  twelve seeds, all four strategies/six policies at1280/1500/1920 widths.

## Decisions and assumptions

- Bevin clarified both vision lists were1-15 with12 seeds and all scenarios;
  remembered roughly58k runs. Full four-by-six policy selection computes64,800
  before Plant energy,194,400 afterwards (16,200 conditions). Exact original
  checkbox state was not recovered from the closed process.
- The10,000-condition ceiling exists in sweep.py and Workbench. It is a
  conservative engineering guardrail, not a measured hardware or ecology limit.
  Explained that a configurable budget could better suit this workflow; no
  change to budgets was requested/implemented in this fix.
- Preserve merged remote-worker work and the earlier local dropdown fixes.

## Validation

- Application log: Application Hang at00:06:16 on2026-10-10 for canonical
  CellSim.Workbench.exe, process5260(hex)=21088. Windows closed the process;
  no current managed exception/usable hang dump was found. Existing old dump
  fromOctober9 01:43 is unrelated. Did not terminate the user's process.
- Before changes, native tests crossed the reported budget and recovered;
  original freeze was not reproduced. Also checked narrower widths and a
  temporary96DPI harness. No unsupported root-cause assertion.
- After changes: Release publish zero warnings/errors. Fourteen native GUI
  groups passed; artifacts/cellsim-workbench-freeze-check/final.json. Third
  comparison updates14-86ms in the six default/native-width cases; reported
  over-limit cases14-23ms. Editing/removal restores the200-row valid preview.
- Temporary96DPI harness also passed14 groups (after.json), including all
  Add/edit/remove cases. Warning screenshot rendered/inspected. Diagnostic
  harness/builds/traces remain ignored local artifacts.
- Canonical EXE rebuilt and reopened. Its comparison cases passed, but the
  subsequent popup screenshot assertion failed when the dropdown lost desktop
  focus. Updated the test to check synchronous opening dimensions independently
  of foreground focus and capture only open popups. Final source check results:
  artifacts/cellsim-workbench-freeze-check/final-focus-independent.json.
  The open canonical binary has the same production fix; this last change is
  confined to test diagnostics/assertions.
- No actual simulations, research changes, source evidence edits or Unity tests.
  Source whitespace checks passed.

## Risks and incomplete work

- Original hang remains unexplained; human retry on the rebuilt app is needed.
  These changes remove redundant sizing and stale preview state, but automated
  responsiveness checks alone cannot prove the reported hang is eliminated.
- Fixed condition/run ceilings remain; configurable budgets would require
  coordinated compiler/Workbench validation and tests.
- Local source is uncommitted/unpushed.

## Next useful step

Retry the reported comparison setup in the rebuilt canonical app. If it hangs
again, capture the live process dump before Windows closes it. Consider a
separate configurable-budget change after agreeing on the desired controls.
