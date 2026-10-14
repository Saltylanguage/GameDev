# Workbench large Excel export memory fix

[Working state](../WORKING_STATE.md) | Status: ready-for-review (local)

- Handoff schema: 1
- Handoff ID: 2026-10-10-2209-codex-workbench-large-excel-export-memory-fix
- Owner: Codex
- Branch: BevBranch
- Baseline commit: 612e65d5
- Date: 2026-10-10
- Supersedes: none

## Summary

Bevin's completed 24,192-run Forest Edge export prepared its requested 1,000
review rows but Node failed at its default roughly 4 GB JavaScript heap limit.
The full workbook now exports successfully with an explicit bounded 8 GB child
heap allowance and batched layout sizing. All requested detail is retained.
Simulation evidence and old workbooks were not changed or rerun. The open
Workbench uses the updated scripts directly; no EXE replacement was needed.

## Changes

- `tools/Export-CellSimWorksheet.ps1` passes `--max-old-space-size=8192` only to
  its Excel renderer child. It does not change global Node settings. Failures
  use plain output instead of `Write-Host`, avoiding host-information CLIXML
  in the Workbench log while preserving failure exit codes.
- `tools/CellSim.Batch/worksheet.mjs` logs workbook/table-building stages and
  Excel-file preparation. Consecutive rows with equal heights are formatted
  together; row heights and column widths target only the cells needed to set
  those whole-row/whole-column dimensions. Cell values and styling are preserved.
- Export/Workbench guides document large-workbook memory use and progress.

## Decisions and assumptions

- Preserve Bevin's requested 1,000 review rows and all full-cohort summary and
  comparison rows. Do not lower the row count or silently omit statistics to
  avoid the crash. Re-export only saved evidence; do not run new simulations.
- This exact source has 8,064 conditions, 12,768 comparison rows and a
  1,000 × 698 Run data table: about 1.2 million table cells. Its three seeds per
  condition remain discovery evidence, not confirmed production tuning.
- Readable formatting remains the same. The 8 GB cap is an allocation ceiling,
  not a reservation. A sample during successful export observed 7,746,797,568
  bytes of process working set (about 7.2 GiB); this is an observed sample,
  not a claim of the exact final peak. The machine has about 32 GB RAM.

## Validation

- **Reproduced:** the default-heap export builds all tables, then fails during
  XLSX preparation at about 4,066 MB heap / 149 seconds. Saved trace:
  `artifacts/workbench-large-export-check/batched-memory.log`. An earlier
  unbatched profiling run with an 8 GB allowance was intentionally stopped
  after about 131 seconds to test layout batching; it is not a successful run.
- **Passed:** full current PowerShell entry point with the 8 GB cap, all
  24,192 matching runs, 8,064 summary rows, 12,768 comparisons, 1,000 review rows
  and 698 data columns. The file was saved at 22:16 America/New_York, about
  3 minutes 14 seconds after starting the export. All four views rendered and
  were visually inspected. No simulation was rerun.
- New workbook:
  `artifacts/worksheets/Forest Edge statistical review-20261010-221318-988.xlsx`
  (6,031,760 bytes). `saved-workbook-check.json` independently reads **1,240,658
  table cells**, including numeric types, and matches them to the prepared
  projection. All four native table ranges, 1,000 evidence links, ZIP integrity
  and no Excel error cells are verified. Frozen inputs and raw hash reverify
  against completed batch identity
  `a77bca8693ece380b82ec2599a4e32e8b1c2d9e741391dae7c70e1afa9c583e3`.
- **31 worksheet checks** and **17 native GUI/export checks** pass. The latter
  uses the actual Create Excel worksheet button and updated exporter scripts.
  A before/after fixture compares **13,166 cells**, numeric formats, fonts,
  fills, alignment, row heights, column widths, table ranges, panes and links.
  Existing-file refusal preserves the workbook hash, returns exit 1 and emits
  plain text without CLIXML. Node syntax and `git diff --check` pass.
- Evidence and read-only saved-file verification script are under
  `artifacts/workbench-large-export-check/`; `export.log` is the successful
  complete export, `final-gui-export-check.json` the final native check.

## Risks and incomplete work

- Large exports still require several GB and take minutes. The whole workbook
  remains an in-memory artifact-tool export; this does not claim bounded-memory
  streaming XLSX support or coverage of every maximum-sized future matrix.
- Desktop Excel opening/PivotTable interaction has not been manually exercised.
  No Unity code changed for this fix, so Unity suites were not rerun. Source is
  local/uncommitted/unpushed along with the earlier session work.

## Next useful step

Open the completed workbook for human review. Future Workbench exports use the
fix immediately. If larger exports reach the new bounded ceiling, profile that
case and consider streaming workbook creation rather than unbounded heap growth.
