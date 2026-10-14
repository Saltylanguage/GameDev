# Workbench large saved-plan metadata fix

[Working state](../WORKING_STATE.md) | Status: implemented locally; human retry pending

- Handoff schema: 1
- Handoff ID: 2026-10-10-2051-codex-workbench-large-saved-plan-metadata-fix
- Owner: Codex
- Branch: BevBranch
- Baseline commit: 612e65d5
- Date: 2026-10-10
- Supersedes: none

## Summary

Bevin's screenshot showed **Progress metadata is unexpectedly large** after
validating a 9,216-condition / 18,432-run gestation/mutation/purchase matrix.
Compilation had succeeded; Run failed before coordinator launch because
`RunTools.LoadRun` read the saved plan through a helper capped at 4 MB.
The plan is 5,017,684 bytes. This was a tooling defect within supported budgets,
not invalid experiment settings or a simulation crash.

The tested fix is installed in the canonical Workbench executable. Bevin saved
the current setup and explicitly authorized closing/replacing/reopening its
window. The new process is PID 26488. Source remains local/uncommitted on
BevBranch, on top of the preceding gestation-control changes.

## Changes

- `Tools.ReadSmall` accepts an explicit byte limit and checks the opened stream's
  length. The default remains 4 MB.
- Saved run plans, completed batch manifests and completed summaries use a
  bounded 64 MB allowance because these contain per-condition data. Status,
  snapshot and Workbench manifest files retain the compact-file limit.
- Oversize errors report the actual applicable MB limit. Frozen hashes, folder
  ownership/path checks and condition/run budgets are unchanged.
- Added regressions for large progress files, identity mismatch, compact-file
  limits, oversize rejection, and executing/reopening a valid padded plan above
  4 MB while still refusing later changes.
- Updated the Workbench guide and working-state index.

## Decisions and assumptions

- Preserve the user's frozen experiment and all eight pinned inputs byte-for-byte.
- Do not reduce the user's matrix or start its 18,432 research runs.
- Keep the existing 10,000-condition / 1,000,000-run budgets; this fix only aligns
  GUI metadata loading with condition-sized data already produced by the tooling.
- No new dependency, metadata format, CLI feature or production/Unity rule change.

## Validation

- Exact saved root:
  `artifacts/workbench-runs/Large-Gestation-Check-With-Skill-Choices-20261010-204819-8700e6`.
  Its plan is 5,017,684 bytes; sweep metadata is 8,688,085 bytes.
- A temporary console probe linking the actual Workbench helpers reproduced the
  original exception at `RunTools.LoadRun` before editing.
- The same probe passes after the fix: all eight frozen file hashes verified,
  18,432 requested runs and pinned-runner `batch --dry-run` preflight succeeds
  (9,216 chunks, 16 requested workers). No batch folder or simulation workers
  were created. Probe source lives in `artifacts/workbench-large-plan-check/`.
- Native GUI/export checks: 15 groups Passed, recorded in
  `artifacts/workbench-large-plan-check/ui-check.json`. Large batch/summary
  reads pass; wrong identities, oversized status and >64 MB inputs reject.
- Setup/run checks: 15 groups Passed, 47 fixture runs, including one tiny real
  run from a >4 MB plan plus the existing exact-coverage, independent gestation,
  all-32-controls preflight, stop/resume and changed-input checks. Original
  snapshot unchanged. See `artifacts/workbench-large-plan-check/run-check.json`.
- Workbench Release publish and canonical launcher build: zero warnings/errors.
- `git diff --check` passes. No Unity suite rerun: no Unity source or simulation
  behavior changed in this fix.

## Risks and incomplete work

- User's research batch is not started. Human retry of Open saved run and Run
  remains pending; the helper/preflight and automated execution paths pass.
- Large condition metadata is still bounded at 64 MB. This is not a claim of
  unlimited-size support or validated performance for a million-run batch.
- Generated applications/probe/test evidence are local ignored artifacts;
  source is not committed/pushed this turn.

## Next useful step

In the updated Workbench choose **Open saved run…**, select the exact saved root
above, and press **Run** when ready. Existing inputs are reusable; revalidation
or reducing the experiment is unnecessary for this error.
