# cellsim-workbench-publication

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-09-1600-codex-cellsim-workbench-publication
- Owner: codex
- Branch: BevBranch
- Baseline commit: 87a456b0
- Date: 2026-10-09
- Supersedes: none

## Summary

Publication package requested by Bevin for origin/BevBranch. Includes this
session's inspection, visible population totals and standalone simulation
setup/run/review/export tooling. Git history identifies the publication commit;
remote SHA equality is checked after pushing.

## Changes

- Separate D5 visual-review scene/assets and reset helper; live Plant total in
  the simulation HUD. Production scenario/species assets remain unchanged.
- Human-readable Excel export with Batch summary, Run review and pivot-ready
  Run data. The latter uses the same selected examples as Run review (default
  200), as requested, while Batch summary covers all matching evidence.
- Native Windows Workbench, readable setup, presets, interactive test matrix,
  thirty described comparison controls, frozen validation, deliberate Run,
  progress, owned safe stop/resume and export.
- Launchers, focused regression checks, guides and individual handoffs included.

## Decisions and assumptions

- Publish the accumulated session source together on BevBranch. Generated EXEs,
  worksheets, simulation results and build caches are intentionally ignored.
- Preserve previous handoffs as dated records; their original local status is
  superseded by this publication package and Git history.

## Validation

- Before publication: fetched origin/BevBranch; local and remote baseline match.
- Worksheet suite rerun for publication: 21 tests passed.
- Prior implementation validation: Release builds/publish had zero warnings or
  errors; matrix-run-checks.json passed 13 groups with 42 fixture rows; final
  matrix-ui-final-checks.json passed 12 non-simulation GUI groups. See the
  [matrix handoff](2026-10-09-1454-codex-workbench-live-test-matrix.md) for details.
- Staged file inventory and source/document whitespace checks reviewed before
  commit. Unity's serialized empty-field trailing spaces are retained. Generated
  outputs and dependencies excluded. No research simulation rerun for this push.

## Risks and incomplete work

- Human acceptance of the updated Workbench remains pending. No fresh full
  Unity suite was run for this publication.
- Independent runtime packaging is still open: other machines need the
  documented .NET SDK/runtime and Excel-export dependencies.
- Collaboration workflow requests a Trello update after pushes. No Trello
  connector is available in this session; board synchronization remains open.
- Historical global handoff-check errors remain outside this change.

## Next useful step

Pull BevBranch, open the Workbench launcher and review the updated matrix using
a saved preset. Update the matching Trello card when board access is available.
