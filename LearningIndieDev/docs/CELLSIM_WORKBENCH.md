# CellSim Workbench

CellSim Workbench is the local Windows home for simulation tooling. It supports
setting up a Forest Edge experiment, saving presets, validating/fixing inputs,
running or safely stopping/resuming its frozen batch, checking progress, and
exporting readable Excel worksheets without AI prompts.

## Open it

Double-click **Open-CellSim-Workbench.cmd** in `LearningIndieDev`. It builds the
app if missing or its source changed, then launches it. The first build needs
the .NET 8 SDK; this machine already has it.

The built application is **artifacts/cellsim-workbench/CellSim.Workbench.exe**.
You can double-click that executable directly; it finds the enclosing project.
If moved elsewhere, it asks you to select the `LearningIndieDev` project folder.
The app opens with no command prompt and does not need Codex running.

## Set up and run

Start on **Set up & run**. **Load D5 example** fills a proposed 24-run setup:
one automatic Gardeners strategy × six purchase policies × four matched seeds.
It uses the existing local frozen confirmation snapshot and inspection settings
(54×32 wrapped map, 325 Plants / 30 Hares / 15 Foxes, Hare vision 9, Fox starting
energy 160). This is a reusable research example, not a production default or
an automated recreation of Bevin's manual choices. It launches nothing.

1. Load that example or **Load preset...**, or choose a neutral frozen scenario
   snapshot and enter settings. **Choose snapshot...** loads its starting grid
   and populations; explicit stat overrides are optional.
2. On **Run settings**, review the name, owner, question, expectation and
   success/failure criteria. These record what you
   intend to learn; the app does not invent an ecology viability threshold.
3. On **Scenario**, set starting Plant/Hare/Fox totals, grid, wrapping and ticks
   per round. On **Choices**, select automatic Mutation strategies and Hare
   purchase policies; each has a description. Use **Compare values** for
   additional overrides or several values of the same setting. First seed,
   seeds per condition, workers, chunk size and timeout are on **Run settings**.
   The live matrix shows strategies × policies × setting variants × seeds,
   with six rounds per run. Workers/chunks/timeout control execution, not the
   scientific test combinations.
4. **Save preset...** saves editable setup for later. A saved preset does not
   launch workers or change a previous experiment. Existing preset replacement
   uses the normal save-dialog confirmation and atomic file replacement.
5. Click **Validate setup**. The existing sweep compiler and runner preflight
   check the configuration. A fresh timestamped folder under
   `artifacts/workbench-runs/` freezes the snapshot, compiled sweep, experiment
   notes and an exact runner copy. No simulation workers run during validation.
6. Review the preview and validation log, then click **Run** deliberately. Any
   setting change disables Run and requires validation into a new folder.
7. Watch chunk/worker progress and the live activity log. On completion,
   **Export this run** sends the saved sweep to **Excel worksheets**, where
   **Create Excel worksheet** creates the report. Export is a separate action.

Automatic strategies are the existing Skip all / Trailblazer / Warren / Gardeners
research policies. They do not define a player role or model subjective play.
Purchases are capped by the recorded wallet/rules, not guaranteed additions.
Late five requests up to five at decision 5; restoration requests up to five
toward the starting Hare total at each reached decision. Unreached windows and
blocked purchases stay explicit in the exported evidence.

## Build the test matrix interactively

The right side stays visible while you change setup sections. Rows are selected
Mutation strategies, columns are purchase policies, and each cell shows the
number of runs for that pairing, including comparison values and seeds. Hover
short matrix headings for the full policy description. Click a cell to filter
the combination list below; **Show all combinations** clears that filter.
Each comparison setting gets its own column in the list. The list shows up to
200 conditions; the total and matrix counts always include every condition.

On **Compare values**, choose a setting to see its description and value in the
loaded frozen snapshot. Enter one value to override it, or a comma-separated
list to compare it, then click **Add comparison**. Double-click a values cell
to edit it, or **Remove selected** to remove that setting. Comparison values
replace the corresponding single Scenario setting or optional D5 override.
They do not modify the source snapshot or production assets.

For example, Hare population `20, 30` and Fox starting energy `120, 160` create
four setting variants. With four strategies, six purchase policies and four
matched seeds, that requests **4 × 6 × 4 × 4 = 384 runs**. Each condition uses
the same consecutive seed range. Changing a value immediately rebuilds the
preview and invalidates any previous validation.

Thirty runner-supported controls are available: starting populations, map
width/height, Hare/Fox vision, starting/maximum energy, metabolism, movement
speed, reproduction chance/food requirement/litter limits, attack modifier and
defense, Plant wilt/food energy, and Hare seed-drop chance. These are absolute
stat overrides; they are separate from Mutation choices or Genome allocations.
The pinned runner checks the resolved rules before Run, including combinations
whose energy/litter values would otherwise be normalized.

Use a decimal point for fractions (`0.25, 0.5`). Values must be distinct and
within the setting's integer/probability limits. Up to eight settings with
1–20 values each are supported, subject to the existing 10,000-condition and
1,000,000-run limits. Population totals must fit the smallest requested map.
Malformed comparisons keep Validate disabled; other input problems appear
during Validate. Presets include the comparison lists; earlier presets and
frozen saved runs remain readable by the updated application.

## Stop and resume

**Stop safely** sends a uniquely scoped request to this window's coordinator.
It stops dispatch, follows the runner's cancellation cleanup and retains validated
completed chunks. In-flight unfinished chunks may be interrupted and are rerun
on resume. It does not stop arbitrary PIDs or unrelated jobs. The app remains
open until its coordinator exits. Closing during a run requests the same safe
stop and waits; closing during validation waits for validation to finish.

After a stop, **Resume / recheck** continues the same frozen batch. After a
restart, **Open saved run...** selects its folder containing `workbench-run.json`;
the app checks frozen file hashes and the pinned runner's preflight before
enabling resume. Completed runs are revalidated without simulating them again.
Changing settings creates a new experiment; it never edits the stopped batch.
Keep frozen run folders in their original location, because compiled evidence
contains absolute paths. Changed/moved inputs are refused rather than repaired.

If the GUI or computer crashes, use Open saved run and Resume after restart.
The existing controller lock refuses a second coordinator while one is alive.
Surviving workers retain their exclusive chunk claims; resume respects those
claims. A normal Stop cleans up only the current coordinator's owned workers.
It does not forcibly stop inherited workers from a crashed invocation. Keep
that distinction when investigating interrupted or stale jobs.

Older standalone batches remain available in Batch progress. Workbench resume
requires its own frozen run manifest and pinned runner; it does not silently
upgrade or rerun historical batches. Validation failures retain their folder
and diagnostics for inspection; no automatic deletion of saved runs occurs.

## Create an Excel worksheet

1. On **Excel worksheets**, use **Add reports...** for one or more `report.json`
   files or a completed sweep's `sweep.json`. **Add sweep folder...** accepts a
   completed sweep or its experiment folder. Remove selected items if needed.
2. Optionally enable **Customize examples, filters and checkpoint ticks**.
   Condition words filter readable condition names; seeds accept comma- or
   space-separated nonnegative numbers. Default: 200 examples, ticks 400/500.
   Summary totals cover every matching run, even when examples are limited.
   Concise observations are the default. Enable **Include detailed
   purchase-by-purchase observations** here when you want the longer population
   change narrative; choices and purchase limits are included either way.
3. Optionally choose **Save as...**, otherwise use a fresh timestamped workbook
   in `artifacts/worksheets/`. Existing workbooks are refused to protect notes.
4. Click **Create Excel worksheet**. The activity log stays visible while the
   exporter works. Large sweeps can take several minutes to verify and stream.
5. The workbook opens automatically unless unchecked. **Open workbook** and
   **Open output folder** refer to the last successful export in this session.

The GUI uses the existing [worksheet exporter](CELLSIM_EXCEL_EXPORT.md); it does
not recalculate ecology results or use AI to invent explanations. Zero, missing
data and unreached checkpoints remain distinct, and human notes stay blank.
The workbook opens on **Batch summary**, with experiment context, counts and
percentages for Hare and all-three-species survival across all matching runs.
**Run review** keeps the familiar per-run layout with shorter observations.
**Run data** automatically splits those same examples into individual numeric
and category columns, including whole-run/per-round slash-line metrics,
population checkpoints, mutations and purchases. Its named **RunData** Excel
table is ready for **Insert → PivotTable** and charts. The
[worksheet guide](CELLSIM_EXCEL_EXPORT.md) includes a first-pivot example and
explains missing values/statuses. Run data follows the review limit and filters;
it does not silently include the entire batch.
Export settings reset on restart. During an export, closing is deferred until
completion; there is no cancel button in this first version. A failed export
keeps its explanation in the activity log. Choose a fresh destination if it
left a partial workbook; do not treat a failure as a complete report.

## Check batch progress

On **Batch progress**, choose a batch folder containing `batch.json` and
`status.json`, or its parent experiment/sweep folder. The app refreshes every
five seconds and displays the recorded state, completed chunks, reported active
workers and the final run total when a matching completed summary is available.
Running status older than 30 seconds is marked stale. Missing worker counts say
Not recorded rather than assuming zero. Missing/damaged metadata shows an error
and is retried, without retaining a misleading success display.

**Open evidence folder** opens that batch. **Use for Excel export** adds the
associated `sweep.json` to the export screen. Exporting requires a completed
sweep; partial or invalid evidence is refused by the existing exporter. An
ordinary standalone batch without its compiled `sweep.json` can be monitored,
but needs a legacy report or compiled sweep for worksheet export.

This lightweight monitor reads small metadata, never the raw run archive. Its
status is a recorded coordinator report, not a fresh integrity check or proof
that a stale worker is still running. Export validates its source separately.

## Runtime and build requirements

- Windows x64 and .NET 8 Desktop Runtime (installed on this machine).
- Setup validation needs Python 3.11+ for the existing sweep compiler. The
  launcher builds the runner separately under `artifacts/cellsim-workbench/runner`;
  each validated experiment pins its own copy. Historical Release binaries and
  saved experiments are not rebuilt in place.
- The source project and existing export tools remain required; this is a local
  project app, not a standalone distributable simulation suite.
- Excel exports retain Python 3.11+, Node.js and `@oai/artifact-tool` prerequisites
  and the existing runtime overrides described in the worksheet guide. Excel is
  only required if it is your chosen viewer, not for workbook creation.
- No new third-party .NET packages or network service are added. The GUI is a
  framework-dependent single-file EXE using the installed Desktop Runtime.
  .NET's publishing options are documented by
  [Microsoft](https://learn.microsoft.com/en-us/dotnet/core/deploying/single-file/overview).

Rebuild without launching:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Start-CellSimWorkbench.ps1 -BuildOnly
```

Focused self-check (writes its result and native form images; no simulations):

```powershell
$app = (Resolve-Path .\artifacts\cellsim-workbench\CellSim.Workbench.exe).Path
$result = Join-Path (Split-Path $app) 'checks.json'
Start-Process $app -ArgumentList @('--self-test', ('"' + $result + '"')) -WindowStyle Hidden -Wait
Get-Content $result
```

An optional third argument is an existing report path, which also verifies real
Excel export through the GUI's process adapter. It writes a new timestamped
integration workbook alongside the result file, exercising the native export
button, background log, completion and close guard. Fixtures test validation,
PowerShell quoting/arrays/progress/errors, overwrite refusal, batch discovery,
stale/completed/corrupt status, summary identity and native form rendering.

Setup/run integration check (runs a small fixture batch, including stop/resume;
use the existing neutral confirmation snapshot, not a production research batch):

```powershell
$snapshot = (Resolve-Path .\artifacts\cellsim-hare-confirmation-20261008-143725\snapshot.json).Path
$result = Join-Path (Split-Path $app) 'run-checks.json'
Start-Process $app -ArgumentList @('--run-check', ('"' + $snapshot + '"'), ('"' + $result + '"')) -WindowStyle Hidden -Wait
Get-Content $result
```

The October 9 published application passed all nine setup/run check groups,
with 26 final fixture rows, unchanged source snapshot, exact seed/condition
coverage, frozen-input refusal, retained completed chunk bytes after stop/resume,
and native Validate → Run → Export interaction. Unfinished interrupted work may
run again on resume. The updated runner also passed all twelve portable checks,
including controller exclusion, orphan recovery and aggregate interruption.
These are tooling checks, not ecology balance findings.

## Current scope and next additions

The current setup supports the neutral Plant/Hare/Fox frozen scenario, thirty
explicit comparison controls and the existing six-round flow. Creating a fresh
snapshot from Unity, other species, custom upgrade-level/Genome loadouts,
arbitrary scientific axes/flat horizons, wider preset discovery and independent
runtime packaging remain future work. Reuse the existing scientific input and
evidence contracts as those grow; do not introduce a second simulation or
implicit production tuning.
