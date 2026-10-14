# human-readable-excel-exporter

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-09-0104-codex-human-readable-excel-exporter
- Owner: codex
- Branch: BevBranch
- Baseline commit: 87a456b0
- Date: 2026-10-09
- Supersedes: none

## Summary

Implemented the first human-readable CellSim Excel exporter using Bevin's
manual worksheet as the reference. The double-click launcher and `CellSim Excel`
read existing evidence and create a fresh workbook without Unity, simulations,
AI calls or overwriting prior notes. The longer-term direction is self-service
simulation setup/run/review; this change implements export only.

## Changes

- `Export-Simulation-Worksheet.cmd`: Windows file-picker entry point.
- `CellSim.ps1 Excel` and `tools/Export-CellSimWorksheet.ps1`: input selection,
  filters/checkpoints/row budget, runtime checks, progress, safe new output and open.
- `tools/CellSim.Batch/worksheet.py`: existing sweep validation/streaming, legacy
  reports, pre-purchase checkpoints, choice/purchase timeline, factual narrative,
  all-matching totals and lowest-seed bounded examples. Native source links use
  standard-library Open XML packaging after rendering.
- `worksheet.mjs`: artifact-tool presentation in the user's column order, filters,
  frozen headers/seed column, blank shaded human notes and optional batch summary.
- `check_worksheet.py`: 12 focused fixtures. Worksheet guide and runbook updated.

## Decisions and assumptions

- No gameplay settings, production assets, simulation rules or frozen run files changed.
- The source report, not an AI-generated compact summary, supplies exact checkpoints
  and purchase ledgers. The existing summarizer currently omits purchase windows.
- For checkpoint Hares, use purchase `populationBefore`; phase closing snapshots
  precede reinforcement and take priority over history at the same boundary.
- Post-purchase change starts from `populationAfter`, excluding immediate ADD.
  Highest recorded Hare count includes post-purchase observations.
- Zero, missing data and an unreached checkpoint are distinct. Reaching the horizon
  does not establish viability. Human experience notes remain blank.
- Default 200 rows, configurable 1..1000. Lowest seeds are selected round-robin
  across conditions, not called statistically representative. Batch totals include
  every matching run, even when only examples are displayed. Filters narrow totals.
- Standalone JSON reports are limited to 128 MB; large completed sweeps stream JSONL
  and use existing frozen-input/raw-hash checks plus full seed-coverage validation.
- Every export uses a new file; no merge/update of existing human notes is attempted.

## Validation

- `python tools/CellSim.Batch/check_worksheet.py`: 12/12 passed, including timing,
  missing/zero data, early extinction, truthful recovery, ledgers, selection,
  duplicate/coverage rejection and native-link preservation/failure safety.
- PowerShell parser checks, `CellSim.ps1 Help`, `node --check`, `git diff --check` passed.
- Existing completed 96,000-run confirmation: verified frozen inputs/raw hash,
  streamed all runs and checked full seed coverage. Filtered D5/S25 Gardeners:
  6,000 runs across six purchase policies, 24 example rows (lowest four seeds).
- Workbook summary Hare-alive-at-horizon counts independently matched existing
  `analysis/metrics.csv`: none382, late495, each-one423, each-three472,
  each-five493, restoration545 (each denominator1000).
- Readback confirmed native links, frozen `B6`, blank human notes, numeric summary
  counts, filters, no cached/formula errors; both sheet views rendered/inspected.
- Direct `CellSim.ps1 Excel` exported two existing run records in a legacy-format
  compatibility fixture without a new simulation.
- An existing-output export was rejected before source processing. Workbook SHA256
  was identical before/after rejection. Invalid input paths produce a clear error.
- Example: `artifacts/worksheets/outputs/2026-10-09-simulation-worksheet/D5-Gardeners-purchase-review.xlsx`.
  This contains automated Gardeners research choices, not Bevin's manual choices.

## Risks and incomplete work

- Python3.11+, Node.js and `@oai/artifact-tool` are prerequisites. The current Windows
  launcher uses installed PATH runtimes and the Codex workspace dependency bundle;
  environment overrides allow an independently provisioned runtime. It does not
  launch Codex or need an AI session. Independent distribution/setup is unfinished.
- Native Windows picker, opening in desktop Excel and clicking its links were not
  manually exercised. The command, XLSX readback and structural native links passed.
- `@oai/artifact-tool` cannot calculate HYPERLINK and exposes no documented native
  link API. Renderer writes literal evidence labels; Python adds native relationships.
  Openpyxl was used read-only for independent output checks, never for authoring.
- No new simulations, Unity compile/tests, player build, full sweep regression,
  setup/launch UI, automated extreme-case selection or causality/viability claim.
- All work is local on BevBranch/87a456b0, uncommitted/unpushed. Earlier local
  inspection/display changes were preserved. No shared board update was made.

## Next useful step

Review the generated workbook with Bevin before changing the presentation further.
Then choose a bounded self-service setup/run interface using the existing sweep
compiler and execution layer. Preserve experiment ownership and reproducibility;
do not infer a new gameplay role or balance threshold from the reporting format.
