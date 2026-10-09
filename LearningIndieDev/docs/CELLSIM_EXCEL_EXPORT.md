# Simulation worksheets

Export saved simulations into the readable comparison format used in Bevin's
October 9 playtest worksheet. No AI prompt, Unity session, Excel installation or
new simulation is needed to create the file. Excel or another compatible viewer
is needed to open it.

For a full local window with filters, checkpoint settings, progress and output
buttons, double-click `Open-CellSim-Workbench.cmd`. See the
[Workbench guide](CELLSIM_WORKBENCH.md). The simpler picker below remains available.

## Start with the file picker

Double-click **Export-Simulation-Worksheet.cmd** in `LearningIndieDev`.

1. Select one or more `report.json` files. To export a completed large sweep,
   select its `sweep/sweep.json` instead.
2. Wait for the printed progress. A large sweep first verifies its frozen inputs
   and raw-report hash, then streams its saved runs. This can take several minutes.
3. The new worksheet opens in your default spreadsheet application. It is saved
   under `artifacts/worksheets/` with a timestamp in its filename.

Cancelling the picker changes nothing. An existing workbook is never overwritten;
your original manual worksheet, source reports and notes remain intact.

## Repeat an export from PowerShell

Run from `LearningIndieDev`:

```powershell
# One existing report; omit ReportPath to use the picker.
.\CellSim.ps1 Excel -ReportPath .\artifacts\my-run\report.json -Open

# Compare two reports in the same worksheet.
.\CellSim.ps1 Excel -ReportPath .\artifacts\arm\report.json `
    -BaselinePath .\artifacts\control\report.json -Open

# Existing completed sweep; filter by words in the condition label.
.\CellSim.ps1 Excel -ReportPath .\artifacts\my-sweep `
    -Match 'D5-S25 gardeners' -Seeds 60000,60001,60002,60005 `
    -OutputPath .\artifacts\my-review.xlsx -Open

# Other observation points and a larger readable cohort.
.\CellSim.ps1 Excel -ReportPath .\artifacts\my-run\report.json `
    -Checkpoints 200,400 -MaxRuns 300

# Optional purchase-by-purchase population-change explanations.
.\CellSim.ps1 Excel -ReportPath .\artifacts\my-run\report.json -DetailedObservations
```

`ReportPath` accepts a full report, its containing folder, a completed sweep
folder, `sweep.json`, or that sweep's `batch/runs.jsonl`. A sweep must pass the
existing completion, input identity and raw hash checks. Missing/duplicate seeds
are rejected. Standalone reports need a nonempty `runs` array; compact summaries,
the manual playtest worksheet and ad hoc checkpoint JSON are not full reports.
Large legacy JSON files over 128 MB are rejected; select the streaming sweep
instead. This first version does not parse arbitrary JSONL without sweep metadata.

## Read the worksheet

**Batch summary opens first.** It shows the experiment name/owner/question when
recorded, actual starting settings, seed selection, timing, automatic strategies
and purchase policies. Per-condition settings and provenance remain to the
right of the results. Combined exports retain context for each source;
settings that vary say so rather than showing one condition as universal.
Unavailable legacy settings say Not recorded. Workbench experiment notes must
match their frozen hash; they do not override the executed scientific arguments.

The summary covers every matching run, including examples not displayed. Hare
and all-three-species survival each have a count and percentage at the stated
horizon. Rates divide by all matching runs, including early extinctions. Missing
required population/horizon data makes the affected count/rate Not recorded;
it is not converted to zero or dropped from the denominator. Reaching the
horizon and remaining alive there differ when extinction happens on the final
tick. Missing endpoint counts stay visible. A seed filter narrows this whole
population, not just the example rows.

**Run review** preserves the familiar sequence: seed/choice/purchase condition,
two pre-purchase population checkpoints, actual endpoint tick and populations,
choices and purchases, purchase limits, a short factual explanation, your notes,
and source evidence. Each decision has its own line. Use the header filters and
frozen seed column to compare runs while scrolling.

Population order is **Hares / Foxes / Plants**. A phase's closing snapshot is used
before the next phase's reinforcement. The purchase ledger supplies the exact
pre-purchase Hare count when available. Zero is a measured count; `Not recorded`
is missing evidence; `Not reached` means the run ended before that checkpoint.
An early extinction retains all available endpoint populations.

The concise default explanation reports the endpoint, highest recorded Hare
count and whole-run births/deaths when recorded. Choices, spending and purchase
limits remain in their own columns. Enable **Include detailed purchase-by-purchase
observations** under the Workbench's Customize settings, or pass
`-DetailedObservations`, to also report population change after purchases.
That change starts from `populationAfter`, excluding the immediate addition. It does
not identify the survivors as purchased individuals or infer causal effectiveness.
Completing the configured horizon is not a claim of ecological viability.
Mutation names follow the report; levels are not invented from a final loadout.

**Run data** contains exactly the same selected runs, in the same order, as
Run review. Each run occupies one row in the named Excel table **RunData**.
Headers are on the first row, with no merged cells or spacer rows. Seed,
strategy, purchase policy and condition are separate fields. Initial, checkpoint
and final Hare/Fox/Plant populations each have their own numeric column.
`Run review row` identifies the corresponding readable row.
Executed map dimensions/wrapping and each explicit species stat override also
have individual numeric fields, so Workbench comparisons can become pivot
fields directly. Omitted stat overrides stay blank: this export does not invent
unrecorded baseline stat values from a condition label.

Whole-run and per-round Hare herbivore and Fox predator slash-line fields are
exported individually, including counters, derived rates, reconciliation flags
and source statuses. Prefixes such as `Run Hare herbivore APS` and
`Round 2 Fox predator KIL` identify the species, role and observation window.
Round start/end ticks and opening/closing populations stay separate; an early
extinction uses its actual observed window rather than implying a full round.
Decisions also have separate chosen/offered mutation IDs, path levels, acquired
upgrades and purchase ledger fields, including requested/added Hares, spending,
balances and stop reasons. Final loadouts expose one count column per recorded
upgrade. This is a wide table intended for selecting fields in pivots and charts;
Run review remains the place to read a run from left to right.

Numbers remain numeric. A valid zero stays zero; a numeric placeholder whose
source rate status is `N/A` or `INVALID` becomes blank, with its exact status in
the adjacent column. Missing/unreached populations and rounds also stay blank
with explicit status fields. Known empty loadouts get zero counts; missing
loadouts stay blank. Rates display four decimal places without rounding the
stored values. Survival/reconciliation flags use numeric 0/1; averaging a known
survival flag gives the survival proportion for the selected example cohort.

To build a first pivot in Excel:

1. Select any cell on **Run data**, then **Insert → PivotTable**. The source is
   the table **RunData**.
2. Put **Strategy** in Rows and **Purchase policy** in Columns.
3. Put **Hares alive at horizon (0/1)** in Values, choose **Average**, and format
   it as a percentage. Or use **Final Hare population** as an average/count.
4. For rate comparisons, filter the corresponding status to **Valid**; for
   example average **Run Hare herbivore APS** with its status set to Valid.
5. Use **Insert → PivotChart** to visualize the pivot.

These pivots cover the selected examples only (200 by default). They must not
be presented as the full batch's rates; **Batch summary** remains the full
matching-run result. No prebuilt pivot or chart is added automatically.

**Your playtest notes** is blank and shaded for optional human input. No generated
text pretends that the run felt meaningful, readable or enjoyable. Source cells
open the evidence folder and identify the file, condition and seed. Screenshots
are not generated or inferred; any existing captures can be linked in your notes.

By default at most **200 rows** are shown (configurable from 1 to 1,000). The
selection takes lowest seeds round-robin across matching conditions, retaining
same-seed comparisons where the budget allows. It is explicitly labelled as
examples, not a statistically representative sample. Use `Seeds` to choose your
own review cohort. Batch totals are computed from all matching runs, not the
displayed examples. A too-small row budget can omit conditions from Run review;
Batch summary still includes them. Extreme/typical-case selection is future work.

## Runtime and extension points

`tools/CellSim.Batch/worksheet.py` uses the existing Python 3.11+ standard-library
CellSim helpers to read/validate evidence and prepare rows. It makes no model or
network calls. `worksheet.mjs` renders those rows through `@oai/artifact-tool`.
The Python helper adds native Open XML evidence links after rendering because
the renderer cannot calculate `HYPERLINK`; no formula error is left in the file.
`tools/Export-CellSimWorksheet.ps1` supplies the picker, runtime checks, fresh output
path and optional open action. `CellSim Excel` and the double-click launcher use
that same path.

The launcher uses Python and Node.js on PATH, falling back to the installed Codex
workspace runtime under `%USERPROFILE%/.cache/codex-runtimes/codex-primary-runtime/dependencies`.
It does not launch Codex or require an AI session. On another machine, install the
workspace runtime or set `CELLSIM_WORKSPACE_DEPENDENCIES` to its dependency root;
`CELLSIM_NODE_MODULES` can point to another installation containing
`@oai/artifact-tool`. Dependencies are not downloaded or installed automatically.
This runtime packaging remains a prerequisite for sharing the tool independently.

Keep the existing boundaries when iterating:

- `RunTools.cs` owns setup validation, frozen experiments and coordinator
  lifecycle; `RunPage.cs` presents those actions in the Windows app.
- `worksheet.py` owns evidence interpretation, summary calculations and the
  versioned JSON projection (currently schema 3). A new output/view should reuse
  these facts rather than independently reinterpret simulation results.
- `worksheet.mjs` owns Excel layout and formatting. It consumes prepared counts
  and numeric rates; scientific calculations do not belong in the renderer.
- PowerShell and the Windows export screen pass explicit options to the same
  exporter. Defaults remain useful without opening advanced settings.

No new dependencies are required by this readability iteration. Preserve frozen
inputs, old workbooks and optional human notes; every export creates a new file.
The [Workbench](CELLSIM_WORKBENCH.md) also provides setup/run/stop/resume.

## Focused verification

```powershell
python .\tools\CellSim.Batch\check_worksheet.py
node --check .\tools\CellSim.Batch\worksheet.mjs
```

The fixture checks include pre/post-purchase timing, recovery excluding ADD,
zero versus missing populations, early extinction, missing purchase ledgers,
each stop reason, unknown horizons, selection independent of input order,
filters, duplicate seeds, full matching totals and incomplete sweep coverage.
Twenty checks pass, including flat numeric/status fields, whole-run/per-round
metrics, legacy role fallback, sampled-cohort correspondence, loadout omissions,
duplicate/nested metric refusal, concise/detailed output, missing-data rate
denominators, extinction exactly at the horizon, actual/frozen context and native
links located by sheet name after summary-first ordering. The October 9 real
4,800-run re-export independently reconciled all 24 summaries and preserved
200 example populations/choices/purchases, 200 native links and the original
workbook hash. Its average example-row height fell from 182.79 to 90 points.
The subsequent pivot-ready export independently checked 110,108 source values,
including 77,172 slash-line fields across the same 200 runs, against saved raw
evidence. It contains a native 687-column RunData table (A1:ZK201); 300 unavailable
numeric rate values are blank with statuses retained. All 24 selected-cohort
groupings reconcile. The earlier workbooks, readable review values, summary
results and 200 native evidence links are preserved. Saved sheets were rendered
and inspected; desktop Excel PivotTable interaction has not been manually tested.
