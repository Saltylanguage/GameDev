# Workbench gestation controls and baseline review

[Working state](../WORKING_STATE.md) | Status: implemented locally; comparison not run

- Handoff schema: 1
- Handoff ID: 2026-10-10-2035-codex-workbench-gestation-controls-and-baseline-review
- Owner: Codex
- Branch: BevBranch
- Baseline commit: 612e65d5
- Date: 2026-10-10
- Supersedes: none

## Summary

Bevin reported that the Hare gestation baseline looked too harsh and that the
Workbench offered no duration control. Independent **Hare gestation ticks** and
**Fox gestation ticks** are now available in Compare values, using the existing
matrix, preset, validation and export paths. One value overrides that species;
several values form a comparison. Zero means immediate births. Production
Hare/Fox defaults remain 48 ticks. Changes are local and uncommitted on BevBranch.

The canonical Workbench executable was rebuilt and reopened after Bevin saved
the current setup and explicitly authorized closing its window. No research
comparison was started and no existing workbook or run evidence was changed.

## Baseline evidence

Source workbook: `artifacts/worksheets/Simulation review-20261010-202730-273.xlsx`.
Batch summary row 14 agrees with all 1,000 raw records under
`artifacts/workbench-runs/Gestation-Baseline-20261010-202434-d473da/sweep/batch/runs.jsonl`.
The workbook's 200 Run review/Run data examples are the same lowest-seed examples,
not a statistical sample; the findings below use the entire batch.

- Seeds 2-1001; 36 x 20 closed map; Plants/Hares/Foxes 400/55/35;
  six 100-tick rounds; Skip all Mutations; no purchases.
- Frozen Hare and Fox gestation are both 48 ticks.
- 53/1,000 runs retained Hares through tick 600 (5.3%); 947 ended with Hare
  extinction. No run retained all three species through tick 600.
- 836 runs recorded zero Hare births; 680 Hare births occurred across the batch.
- Median extinction endpoint among the 947 extinct runs was tick 125.
- Whole-run Hare stat lines record 51,202 PREY deaths and 4,376 STRV deaths.
- Successful reproduction attempts count conceptions, not delivered litters.
  The 1,821 successful attempts cannot establish an exact pregnancy-loss count.

A shorter Hare duration is a plausible hypothesis, not a demonstrated cause or
approved balance change. Predation dominates the recorded Hare deaths, and this
batch has no alternative gestation duration to isolate the delay's effect.

## Changes

- Appended GestationTicks to the attribute enum, added the registry ID
  `reproduction.gestation-ticks`, and advanced registry version v5 to v6.
  Applying a modifier now changes that rule while preserving the other fields.
- Mapped that ID to portable `RuleData.gestationTicks` so absolute overrides
  reach the existing runner. Existing enum numbers and Unity metadata are preserved.
- Added two described comparison controls using the current catalogue pattern
  (32 controls total). Existing integer/minimum-zero validation, snapshot values,
  preview, preset serialization and Run workflow are reused.
- Added friendly Hare/Fox gestation labels to the worksheet projection. Run data
  already supports separate numeric stat-override columns; tests now cover zero.
- Extended domain, runner, worksheet, comparison/preflight and native UI checks.
- Updated `docs/CELLSIM_WORKBENCH.md` and this working-state handoff.

## Decisions and assumptions

- Keep the production defaults unchanged until matched-seed results are reviewed.
- Compare Hare duration independently with Fox held at 48, retaining the baseline
  map, populations, strategy, purchase policy and seeds.
- Prepared `artifacts/workbench-presets/Hare gestation comparison.json`: Hare
  0/12/24/48 x Fox 48 x 1,000 seeds = 4,000 requested runs. It contains proposed
  comparison criteria, not an approved acceptance threshold. It has not run.
- Use the new matching snapshot at
  `artifacts/cellsim-gestation-controls-check/snapshot.json`. Registry changes
  alter runner provenance, so historical frozen batches retain their old runner.
  Snapshot source commit honestly identifies the uncommitted working tree as
  `612e65d5+gestation-controls-working-tree`; source hash is
  `62d30f34dc7c3b531a710f26234b8091c4d0bfbc858b8dcb7adfb3eead7ea08d`.

## Validation

- Runner Release build and Workbench Release publish: zero warnings/errors;
  canonical application and its runner rebuilt with `Start-CellSimWorkbench.ps1 -BuildOnly`.
- Runner self-test passes, including absolute Hare/Fox durations, independent
  overrides, zero, negative rejection and fractional rejection.
- Python worksheet suite: 21 checks pass.
- Unity EditMode: 326 passed, zero failed/skipped, recorded in
  `artifacts/cellsim-gestation-controls-check/EditMode-final-results.xml`.
  An earlier new fixture attempted a zero-delta modifier, which the existing
  modifier constructor rejects. Corrected that test input; production code was
  not changed to weaken the contract. The earlier failed result is preserved.
- Native GUI checks: 14 groups pass, including all 32 readable dropdown labels
  and oversized-matrix recovery; `ui-check.json` in the same evidence folder.
- Setup/run checks: 14 groups pass, 46 actual fixture runs, all 32 offered
  controls preflight, independent durations, preset round trips and safe
  stop/resume. Source snapshot remained unchanged; see `run-check.json`.
- Rendered and visually inspected `workbench-gestation-controls.png`: frozen
  48-tick value, help and four Hare duration rows with Fox fixed at 48 are readable.
  This screenshot shows a tiny test fixture, not research comparison results.
- Exported the actual four-run gestation fixture and asserted numeric pairs
  (Hare,Fox) = (0,0), (0,48), (48,0), (48,48) in Run data. See
  `gestation-worksheet-check.json` in the evidence folder.
- `git diff --check` passes; no production asset or metadata changes.

## Risks and incomplete work

- The prepared 4,000-run comparison has not run; a new duration is not approved.
- No fresh PlayMode suite this turn. This change exposes a rule through the
  shared registry/tooling; it does not change the existing gestation lifecycle.
- Canonical Workbench reopened successfully; Bevin's human UI acceptance remains
  open. Load the prepared preset before validating/running the comparison.
- Generated applications, preset, snapshot and test evidence are local ignored
  artifacts. Source changes are not committed or pushed this turn.

## Next useful step

In the updated Workbench, choose Load preset and select
`artifacts/workbench-presets/Hare gestation comparison.json`. Review the 4,000-run
matrix, then Validate setup and deliberately Run when ready. Compare matched-seed
Hare survival at 600, births and extinction timing, and review the wider ecology
before changing any production setting.
