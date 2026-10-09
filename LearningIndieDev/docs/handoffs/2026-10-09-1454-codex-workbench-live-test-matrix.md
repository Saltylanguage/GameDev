# workbench-live-test-matrix

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-09-1454-codex-workbench-live-test-matrix
- Owner: codex
- Branch: BevBranch
- Baseline commit: 87a456b0
- Date: 2026-10-09
- Supersedes: none

## Summary

Reworked Workbench setup around Bevin's request for better presentation,
interactive matrix building, descriptions and more knobs. Four setup sections
sit beside a live strategy/purchase matrix and per-condition input list.
Thirty runner-supported controls can use a single override or multiple values
crossed with all selected strategies/policies/seeds. Saved presets retain these
settings. The canonical local EXE is updated; previous work and evidence remain.
Local/uncommitted/unpushed on BevBranch at87a456b0.

## Changes

- `RunSetupView.cs`: setup presentation, described strategy/purchase choices,
  parameter picker with frozen snapshot values, editable comparison lists,
  live counts/matrix and clickable pairing filter. Condition list caps at200
  while full counts remain explicit. Input combinations have separate columns.
- `SetupParameters.cs`: thirty explicitly supported absolute overrides, labels,
  descriptions, runner IDs, snapshot fields and input parsing/bounds. Covers
  populations/map size; Hare/Fox vision, energy, movement, reproduction/combat;
  Plant wilt/food energy and Hare seed-drop chance. No new ecology implementation.
- `RunTools.cs`: optional comparison arrays preserve old presets/runs, checked
  Cartesian counts/capacity/budgets and compiler spec generation. Axis overrides
  replace the matching baseline component, avoiding conflicting duplicate args.
- `RunPage.cs`: selection/change events invalidate validation and rebuild preview;
  existing deliberate Run/owned stop/resume lifecycle retained. Loading a frozen
  run refreshes its preview even while controls are busy. Batch state/signatures
  remain tied to frozen evidence.
- `WorkbenchForm.cs`: larger DPI-aware bounds, clearer heading/status and setup
  tab identity. Checks now distinguish outer tabs from setup section tabs.
- `worksheet.py`: separate numeric map dimensions/wrap and explicit stat override
  fields connect new comparisons to pivots. Legacy missing inputs stay blank.
- Guides, shared context, focused native/run/worksheet checks updated.

## Decisions and assumptions

- Every comparison axis crosses every other axis; common consecutive seeds
  retain matched comparisons. One value is a fixed override. Up to8 axes,
  1–20 distinct values each,10,000 conditions and1,000,000 runs. Populations must
  fit the smallest requested map. Whole-number/probability inputs are checked
  locally; pinned-runner preflight refuses resolved-rule normalization/conflicts.
- The existing six-round contract, neutral Plant/Hare/Fox source and automatic
  strategies remain. No upgrade-level/Genome builder or alternate simulation
  engine was added. Absolute research overrides do not edit production assets.
- User did not answer optional priorities question; proceeded with the stated
  population/stat/map controls already supported by the runner. Upgrade-level
  authoring remains a possible later iteration.
- Bevin explicitly saved desired presets and authorized closing the two older
  Workbench windows. Both closed gracefully with no active child operations;
  the canonical build was then replaced. Preview builds/diagnostics are local
  ignored artifacts, not a second supported application path.

## Validation

- Final Release runner build/Workbench publish:0 warnings/errors.
- `artifacts/cellsim-workbench/matrix-run-checks.json`: Passed13 groups,42 final
  fixture rows. Covers new counts/parser/bounds/duplicate/capacity limits,
  comparison preset round trips, every offered parameter through pinned-runner
  preflight,16-condition two-axis compilation/execution, exact seeds/populations,
  baseline-overlap removal, immutable inputs, safe stop/resume/chunk preservation
  and native Validate → Run → Export. Source snapshot hash unchanged.
- `matrix-ui-checks.json`: Passed13 GUI groups including real legacy workbook
  export and updated layout. Final `matrix-ui-final-checks.json`: Passed12
  non-simulation GUI groups after display-only polish. Matrix checkbox changes,
  pairing cell filtering/reset and default/compact preview visibility checked.
- All4 sections and compact layout rendered/inspected at144DPI. Default1920×1350
  and compact1500×1200 physical pixels; bounds are DPI-aware/capped to work area.
  Smaller views use scrollbars for long descriptions or wide matrices.
- Worksheet suite:21 tests passed, including separated numeric executed inputs.
  Independently prepared the final16-run comparison export projection: actual
  8/12 starting Hares,120/160 Fox energy,16×12 wrapped map retained as typed fields.
- `git diff --check` passed. Handoff checker still reports28 older issues and
  none naming this handoff. Existing research batches/workbooks were not rerun
  or rewritten. No Unity/domain/production data/board/Git-sharing changes.
- An initial native check caught missed preview-handler wiring; corrected at
  the change handler and revalidated. Initial scaled layout squeezed previews;
  DPI bounds/flexible rows were corrected before final render checks.

## Risks and incomplete work

- Human acceptance and ordinary mouse/keyboard usability feedback are pending;
  native form interaction/render checks are automated. Wide/full matrices can
  require horizontal scrolling at smaller sizes; long setup sections scroll.
- This is a curated thirty-control catalogue, not every possible runner flag.
  Upgrade levels, custom Mutation sequences/Genome configurations, fresh Unity
  snapshot authoring, other species and independent runtime packaging remain.
- Optional priority clarification remained unanswered; no unsupported knobs or
  speculative simulation behavior were invented. Old presets load in the new
  app; newly saved comparison presets require the updated app.
- Historical global handoff-check failures remain outside this change.

## Next useful step

Open the updated Workbench, load the saved preset and try selecting strategies,
purchase policies and two comparison lists. Confirm the matrix is understandable
before extending the curated parameter catalogue or adding upgrade authoring.
