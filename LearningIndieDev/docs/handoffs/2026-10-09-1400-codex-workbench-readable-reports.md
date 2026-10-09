# workbench-readable-reports

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-09-1400-codex-workbench-readable-reports
- Owner: codex
- Branch: BevBranch
- Baseline commit: 87a456b0
- Date: 2026-10-09
- Supersedes: none

## Summary

The next Workbench iteration prioritizes Bevin's readability, ease of use,
understanding and modularity goals. New workbooks open on Batch summary with
experiment context, actual settings and survival counts/percentages. Per-run
observations are concise by default; detailed purchase-change narratives remain
an explicit GUI/CLI option. Re-exported Bevin's first real 4,800-run batch into
a fresh workbook without rerunning simulations or editing the original.
All work is local/uncommitted/unpushed on BevBranch at 87a456b0. Earlier changes
are preserved; no Unity assets/domain rules or external boards changed this turn.

## Changes

- `worksheet.py`: schema-2 projection owns summary counts/rates, actual setting
  descriptions, verified frozen experiment context and concise/detailed stories.
  Native source links resolve the Run review sheet by name rather than index.
- `worksheet.mjs`: summary first, context block, typed percentages, compact
  summary table and shorter review rows. Population/choice/purchase columns and
  blank human notes remain. Renderer consumes prepared facts, not ecology logic.
- `Export-CellSimWorksheet.ps1`, `CellSim.ps1`, `Tools.cs`, `WorkbenchForm.cs`:
  optional DetailedObservations flag propagated end to end, default false.
  Windows export screen explains summary versus examples and the new option.
- Focused Python/native checks, guides and durable project direction updated.
- Fresh output: `artifacts/worksheets/Simulation review-readable-20261009.xlsx`.

## Decisions and assumptions

- Survival percentages divide by all matching runs, not displayed examples or
  survivors. Missing required evidence produces Not recorded for affected
  metrics, preserving valid zeros. Extinction exactly at the horizon can count
  as reached while not alive. All-three means Hare/Fox/Plant present there;
  it is not an approved ecology viability threshold.
- Context uses executed arguments. Workbench name/owner/question come from
  hash-verified experiment notes. Multiple sources/settings stay distinguishable;
  legacy omissions remain explicit. Recorded experiment questions are not
  rewritten merely because the user selected more strategies.
- Reuse existing boundaries: RunTools/RunPage for execution/UI, Python for
  scientific report facts, JavaScript for Excel presentation and adapters for
  invocation. No new generic plugin/framework is needed for this slice.
- Frozen simulations, original workbook and human notes are preserved. Every
  exporter invocation still refuses an existing destination. Only a temporary
  newly generated candidate was replaced during this session's layout QA.

## Validation

- Python worksheet suite: all15 passed. Includes missing-data denominators,
  exact-horizon extinction, concise/detailed output, actual/frozen context,
  sampling/coverage, purchase timing/ledgers and sheet-name evidence links.
- Python syntax and JavaScript syntax checks passed.
- Release Workbench build/publish: zero warnings/errors. Published EXE self-test
  `artifacts/cellsim-workbench/readable-gui-checks.json`: Passed all12 checks,
  including explicit detailed flag through a real PowerShell process, actual
  native export with legacy evidence, async completion and close guard.
- Real batch re-export verified the existing frozen inputs and raw output hash.
  `artifacts/worksheet-review-20261009/readable-checks.json`: Passed, independently
  reconciled24 condition summaries against4,800 raw runs, preserved200 example
  populations/choices/purchases, all200 native source links and original SHA256.
  No Excel error cells; numeric rates retain 0.0% formatting.
- Both saved workbook sheets and the native export form were rendered and
  inspected. Mean review-row height in this export decreased182.79→90 points;
  concise content/choice/limit lines fit the inspected rows. Native links remain
  on Run review after summary-first reordering. No simulations this turn.
- `git diff --check` passed. Handoff checker retains the same28 older errors,
  with none naming this handoff. Historical confirmation runner SHA256 remains
  unchanged. Updated Workbench launched after final checks.

## Risks and incomplete work

- Default examples still use lowest seeds, not statistical sampling. Summary
  covers all matching runs; filters narrow that population explicitly.
- Long technical source paths remain to the right of the primary summary;
  use native per-run source-folder links for evidence navigation.
- Desktop Excel/native pickers/default app interaction was not manually tested.
  Unity/run-control suites were not repeated for unchanged simulation/execution
  code. Existing local runtime dependencies/distribution limits remain.
- Human acceptance of this presentation is pending. A saved-experiment library,
  in-app result comparisons and preset discovery are useful future iterations,
  not implemented or separately approved here.

## Next useful step

Review the new workbook with Bevin and collect concrete readability/navigation
feedback before expanding the interface. Continue the same evidence/presentation
boundaries for future result views. Read the [worksheet guide](../CELLSIM_EXCEL_EXPORT.md)
and [Workbench guide](../CELLSIM_WORKBENCH.md).
