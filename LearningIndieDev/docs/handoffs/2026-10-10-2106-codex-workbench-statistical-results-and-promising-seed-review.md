# Workbench statistical results and promising seed review

[Working state](../WORKING_STATE.md) | Status: implemented and tested locally

- Handoff schema: 1
- Handoff ID: 2026-10-10-2106-codex-workbench-statistical-results-and-promising-seed-review
- Owner: Codex
- Branch: BevBranch
- Baseline commit: 612e65d5
- Date: 2026-10-10
- Supersedes: none

## Summary

Bevin wants useful seeds, trustworthy full-batch results and evidence that
ecologies support continued play and that choices affect outcomes. The exporter
now separates full matching-cohort statistics, an outcome-ranked seed shortlist
and same-seed policy/control comparisons. The readable review and its pivot
data still contain exactly the same selected runs. Existing workbooks, frozen
experiments and production ecology settings are preserved.

## Changes

- Batch summary has a native **BatchSummary** table: final-horizon goal success,
  95% Wilson bounds, Hare survival bounds, median endpoint, mean endpoint
  populations, Hare births, purchases, no-purchase/unassisted success and
  recorded decision-window access. Missing numeric evidence stays blank.
- New **Choice impact** sheet pairs Mutation policies with Skip all at the same
  purchase policy, and purchase policies with No purchases at the same Mutation
  policy. Source batch, all other executed arguments, horizon and seed must match.
  It reports wins/losses/ties, paired goal-rate change, endpoint/Hare differences,
  recorded non-skip choices and additions. Missing/ambiguous controls stay visible.
  Controls can be outside the condition text filter; seed filters affect both arms.
- **Promising seeds** is the default review selection: goal success, success
  without purchased Hares, endpoint tick, final Hares/Foxes/Plants, then lowest
  seed. Rows show goal status; **Lowest seeds (legacy)** remains available.
- The Excel screen exposes review selection and three positive final-horizon
  success floors. Large-font/DPI dropdown sizing is covered. Both PowerShell
  entry points pass the same options. `-SuccessFloors 5,5,100` and
  `-ReviewSelection promising|lowest-seeds` are supported.
- `worksheet_stats.py` uses temporary standard-library SQLite compact outcomes
  for statistics and pairing; no new dependency. Projection schema is now 4.
  Existing layout/reader/export boundaries remain in place.

## Decisions and assumptions

- Pending clarification, “play autonomy” is supported in both senses: ecology
  success without reinforcement, and matched automated-choice impact. The goal
  floors **5 Hares / 5 Foxes / 100 Plants** come from Bevin's latest experiment
  screenshot. They are editable export criteria, not an approved production target.
- Success requires all three floors at the condition's configured horizon.
  Unknown goal outcomes leave the condition's success count/rate/bounds blank;
  they are counted explicitly, not dropped from denominators. Endpoint means
  include early extinctions at their actual observed final tick.
- No-purchase success may include Mutations. Unassisted success is restricted
  to Skip all / No purchases with known zero additions. Missing ledgers are
  never inferred to mean zero purchases. Decision access uses recorded windows.
- Two-sided 95% Wilson bounds follow
  [NIST](https://www.itl.nist.gov/div898/handbook/prc/section2/prc241.htm).
  Seeds are treated as independent trials within a condition. Shared-seed
  conditions are paired observations, not independent repeats to pool.
  Comparisons are descriptive; there is no significance test or multiple-search
  correction. Selected good results require confirmation on new seeds.

## Validation

- Release app/runner builds: zero warnings/errors. **31 worksheet checks**,
  **16 native GUI check groups**, **15 setup/run groups / 47 fixture runs** pass.
  The native checks include actual export option forwarding, large fonts,
  existing-file protection, and the full setup/run/export path. No new Unity
  changes were made for this export iteration; no Unity suite was rerun.
- Independent raw-count and 50-digit Decimal Wilson checks pass for the saved
  baseline and 16 actual paired fixture comparisons. Saved-XLSX readback checks
  all 200 ranked endpoint rows, matching Run data order/ranks, 200 native evidence
  links, full-cohort counts, births/decision access, all 16 paired table rows and
  zero formula errors. All four sheet previews and the GUI were visually inspected;
  a clipped control-condition column and a large-font selector were fixed.
- Evidence: `artifacts/workbench-statistical-export-check/` contains
  `independent-check.json`, `saved-workbook-check.json`, GUI/run result files,
  the read-only verification script and previews. Early failed GUI checks remain
  separately named; `ui-check-final.json` is the passing result.
- New workbook:
  `artifacts/worksheets/Gestation baseline statistical review-20261010-211732-050.xlsx`.
  It re-exports existing seeds 2–1001: **0/1,000 goal successes**, **53 Hare
  survivors**, **median endpoint 128**, **836 zero-Hare-birth runs / 680 births**.
  Top ranked seed 78 reaches 600 with 34 Hares / 0 Foxes / 353 Plants, explicitly
  below floors. The only condition is Skip all / No purchases, so its Choice
  impact sheet correctly reports that there is no comparison to make.
- Canonical executable rebuilt after Bevin saved and authorized closing the old
  window, then reopened as PID 16764. Its **17 installed GUI checks**, including
  a real export through the native button, pass in `installed-ui-check.json`.
  An initial installed-check invocation incorrectly passed the project directory
  as its optional report argument and blocked on an error dialog; only that
  newly launched test process was stopped, and the corrected invocation passed.
  Research batches were not started by this work; fixture runs above test tooling,
  not balance.

## Risks and incomplete work

- These automated-policy results support finite-horizon ecology and choice-impact
  assessment; they cannot prove human enjoyment or longer-term stability.
- Two seeds per condition yield wide intervals (even 2/2 success has a lower
  bound near 34%). Rank-based discovery across many conditions can select luck;
  confirmation on new seeds remains necessary before tuning.
- Run data pivots cover the same selected review rows, not the full cohort.
  Use BatchSummary for full condition-level statistics.
- Standalone reports without executed arguments cannot establish matching
  controls. Incomplete ledgers/goals remain unknown. Desktop Excel PivotTable
  interaction was not manually tested. Source remains local/uncommitted/unpushed.

## Next useful step

Export the deliberately completed comparison batch with the chosen criteria.
Review full-cohort success/uncertainty and matched policy effects, inspect the
shortlist, then choose settings/seeds for a separately authorized confirmation.
The 18,432-run saved setup and 4,000-run gestation preset remain separate from
this export-only iteration. Do not infer they ran from these tooling checks.
