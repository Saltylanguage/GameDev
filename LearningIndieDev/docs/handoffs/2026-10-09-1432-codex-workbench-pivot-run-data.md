# workbench-pivot-run-data

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-09-1432-codex-workbench-pivot-run-data
- Owner: codex
- Branch: BevBranch
- Baseline commit: 87a456b0
- Date: 2026-10-09
- Supersedes: none

## Summary

Added an automatic third worksheet, Run data, for Bevin to build Excel pivots
and graphs from the readable Run review cohort. Bevin explicitly selected the
same 200 examples, rather than every matching run. Exported the existing real
4,800-run batch into a fresh workbook without new simulations. Existing readable
review content, summary results and previous workbooks remain intact.
All changes are local/uncommitted/unpushed on BevBranch at 87a456b0.

## Changes

- `worksheet.py`: schema-3 projection adds scalar Run data records after the
  existing sample selection. Includes separate condition/seed/policy/strategy,
  initial/checkpoint/final populations, horizon flags, all available whole-run
  and per-round slash-line counters/rates/statuses, actual observation windows,
  mutation choices/offers/path levels, acquisitions, purchase ledgers and
  per-upgrade final loadout counts. Reuses population/peak helpers with review.
- `worksheet.mjs`: renders the native RunData table, header row, filters, frozen
  seed/strategy columns and numeric formats. Original summary/review layouts
  remain; summary subtitle explicitly identifies the shared example cohort.
- `WorkbenchForm.cs`: explains the automatic third sheet; no new setting.
  Rebuilt local application and updated usage guides/context.
- `check_worksheet.py`: five focused flat-data regressions added (20 total).
- Fresh workbook: `artifacts/worksheets/Simulation review-pivot-ready-20261009.xlsx`.

## Decisions and assumptions

- Exactly one run per row and one scalar per column; this is a deliberately
  wide analysis table, while Run review remains the reading surface. It shares
  review ordering, limit and filters. `Run review row` maps back to readable data.
- Valid numbers including zero remain numeric. Source N/A/INVALID rate numeric
  placeholders become blanks, with source statuses preserved. Missing/unreached
  rounds and checkpoints stay explicit. Known empty loadouts get zero counts;
  unknown loadouts stay blank. Numeric flags use 0/1 for pivot aggregation.
- Per-round metrics retain actual windows. No synthetic full-round results or
  causal purchase-effect claims. Legacy singular stat lines only fall back for
  their expected species/role. Duplicate metrics and nested fields are refused.
- No prebuilt pivots/charts, new dependencies, simulation rules or run-control
  changes. All scientific interpretation remains in the Python projection;
  JavaScript owns presentation and the GUI invokes the same exporter.

## Validation

- Python worksheet suite: 20 checks passed; JS syntax check passed.
- Release runner build and Workbench publish: zero warnings/errors. Published
  EXE `artifacts/cellsim-workbench/pivot-gui-checks.json`: all12 checks passed,
  including native button/async completion/close guard with a real legacy export.
  Independently read that saved legacy workbook: all3 sheets,2 review/data rows
  and native RunData table present. Updated Workbench reopened after checks.
- Production wrapper re-export verified frozen inputs and raw report hash.
  `artifacts/worksheet-review-20261009/pivot-checks.json`: Passed; streamed4,800
  raw runs, verified all200 selected runs and110,108 source values, including
  77,172 slash-line fields. Native RunData table is A1:ZK201 (687 columns).
  Numeric cell types checked;300 unavailable numeric rates remain blank with
  source statuses. All24 selected-cohort survival groupings reconcile.
- Independently read saved XLSX using openpyxl without authoring. Review values
  are identical to the earlier readable workbook; summary results unchanged
  except subtitle; original workbook hash and200 native evidence links preserved.
  No formula/error cells on Run data. All three saved sheets and native export
  form rendered and inspected. No Unity or new simulations during this work.
- `git diff --check` passed. Full handoff check still reports28 older errors;
  none names this new handoff. Existing unrelated failures were not edited.

## Risks and incomplete work

- Excel desktop PivotTable interaction has not been manually tested. Native
  table metadata/types and independent aggregations verified instead.
- Default examples are lowest seeds across conditions, not a statistical sample;
  pivots describe those examples. Use Batch summary for full matching-run rates.
- The flat table is wide because round/decision fields are separated. A future
  long-format per-round view may help cross-round charts but is not implemented.
- Existing runtime packaging and standalone distribution limits remain.
- No shared commit/push or external board changes. Human presentation acceptance
  is pending; simulation/run-control suites were not repeated for unchanged code.

## Next useful step

Open the fresh workbook, select Run data and Insert → PivotTable using RunData.
The worksheet guide includes a Strategy × Purchase policy survival pivot example.
Collect Bevin's feedback before adding other analysis views or automation.
