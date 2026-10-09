# Standalone CellSim batch runner

Implemented locally on October 8, 2026. The console program links the production
simulation and existing experiment harness C# sources. It runs independent seeds
in bounded .NET 8 worker processes, without a Unity Editor, project import, player
build, rendering, Docker or third-party .NET packages in each worker.

Unity is needed to export an authored scenario and its resolved upgrade catalog,
and to produce a small reference batch. Keep consecutive ticks and phases of one
simulation in one worker. This tool does not change gameplay tuning or approve
the October 7 meeting's still-undefined tick-700 ecology viability criterion.

## Readable Excel worksheets

For the local Windows GUI, double-click `Open-CellSim-Workbench.cmd`. The
[Workbench](CELLSIM_WORKBENCH.md) now supports readable setup, saved presets,
run-count preview, validation, deliberate Run, progress, safe stop/resume and
Excel export. Setup is currently the neutral Plant/Hare/Fox six-round flow;
validation freezes a fresh experiment and exact runner without starting workers.

The Workbench launcher builds its runner into `artifacts/cellsim-workbench/runner`
and pins a copy per experiment, preserving historical Release binaries. Its
coordinator accepts `batch <plan> [--resume] [--stop-file <path>]`: creating that
invocation's unique stop file requests cancellation and owned-worker cleanup.
Completed chunks remain available for normal resume; unfinished chunks may run
again. This extends the existing Ctrl+C behavior without changing simulation
rules. The GUI waits for its coordinator to exit before closing. Resume after
restart uses **Open saved run...** and refuses changed/moved frozen evidence.

Double-click `Export-Simulation-Worksheet.cmd`, select saved `report.json` files
or a completed sweep's `sweep.json`, and the new workbook opens automatically.
From PowerShell, use `CellSim.ps1 Excel -ReportPath <report-or-sweep-folder> -Open`.
The exporter reads existing evidence, reports choices/populations/recovery in
Bevin's worksheet format, and leaves human notes blank. It runs no simulations
and never overwrites an existing workbook. See the
[worksheet guide](CELLSIM_EXCEL_EXPORT.md) for filters, bounded cohorts and runtime
requirements.

## Live progress dashboard

For a candidate/purchase sweep, start the read-only dashboard from LearningIndieDev:

```powershell
python .\tools\CellSim.Batch\dashboard.py .\artifacts\<run-directory>
```

Open the printed localhost URL, also recorded in the run's `dashboard-server.json`.
The page refreshes every five seconds and shows validated runs/chunks, reported
workers, elapsed simulation time, an average-throughput ETA, all candidate/path/
purchase matrix cells and active chunks with process IDs and assigned seeds.
It distinguishes simulation completion from analysis/report completion and flags
source status older than 30 seconds or disconnected refreshes. Progress advances
only when a chunk validates, not during partial runs. Worker records can be stale
if the controller stops; they are not independently probed for process liveness.
ETA is an estimate for simulation only and uses the runner's invocation elapsed
time; a resumed batch is not a fresh throughput benchmark.

The server binds to 127.0.0.1, exposes only the page and bounded status data, and
reads metadata rather than large raw reports. It does not modify scientific
inputs or control the running batch. Closing the tab or stopping this dashboard
process leaves the simulation running. Restart it if the computer/server stops;
no startup automation is installed. `check_dashboard.py` covers chunk accounting,
tail chunks, failures, stale status, identity rejection and analysis lifecycle.

## Sweep settings, then confirm candidates

The approved Hare-purchase panel is documented in
[`S4-03 protocol`](Research/Experiments/S4-03-Hare-Reinforcement-Candidate-Panel/PROTOCOL.md).
The scientific option `-harePurchasePolicy` supports `none`, `early-five`,
`middle-five`, `late-five`, `each-one`, `each-three`, `each-five` and
`restore-toward-start`. It requires a Hare player, six continuous phases and
normal boundary choices. Restoration requires an explicit starting Hare count.
Omitting the option preserves the historical experiment behavior; explicit
`none` adds a currency audit without purchases. Each reached window credits
living Hares once, buys individual Hares for 10 after successful production
placement, and retains unspent funds. `purchaseWindows` records currency,
population, requests, additions and stop reasons; additions reconcile as ADD.
Report schema is 35. Existing frozen historical builds/exports remain separate.

`compile-panel.py` freezes the exact accepted candidate arms and tools;
`validate-panel.py` checks reference/control parity and the full matrix smoke;
`run-panel.ps1` runs the validated panel and automatic analysis. The report tool
`purchase_report.py` adds per-candidate APS/AHS tables, paired purchase/path
references and diagnostics for all assigned windows, including those not reached.

Bevin clarified the goal on October 8: search combinations of populations,
grids, starting stats and skill paths, then investigate their slash lines and
matched deltas. The batch executor is the execution layer. `sweep.py` supplies
the matrix, disk-backed analysis and shortlist workflow using Python 3.11+ standard
library. These are local research tools; the example values are unapproved.

Export a neutral snapshot so different path conditions can supply their own
flow. From `LearningIndieDev`:

```powershell
.\tools\Export-CellSimSnapshot.ps1 `
    -OutputPath "$PWD\artifacts\cellsim-neutral.json" `
    -ScientificArguments @('-playerSpeciesId','hare','-stepIntervalSeconds','0.1',
        '-wrapEdges','true','-combatMode','opposed-roll',
        '-experimentalFeatures','bev-experimental')

python .\tools\CellSim.Batch\sweep.py compile `
    .\tools\CellSim.Batch\example-sweep.json .\artifacts\screen
dotnet .\tools\CellSim.Batch\bin\Release\net8.0\CellSim.Batch.dll batch `
    .\artifacts\screen\plan.json
python .\tools\CellSim.Batch\sweep.py analyze .\artifacts\screen `
    .\artifacts\screen-analysis --rank population.hare.final --top 10

# Retest the shortlisted settings with fresh seeds and every original path.
python .\tools\CellSim.Batch\sweep.py compile `
    .\tools\CellSim.Batch\example-sweep.json .\artifacts\confirmation `
    --contexts .\artifacts\screen-analysis\shortlist-contexts.json `
    --seed-start 500000 --seed-count 2000
dotnet .\tools\CellSim.Batch\bin\Release\net8.0\CellSim.Batch.dll batch `
    .\artifacts\confirmation\plan.json
```

Build the Release runner first as shown below. Start with `--seed-count 4` and
new output directories for a tooling pilot. Compilation preflights every
condition and launches no simulations. A full screen above has 16 parameter
contexts × 3 paths × 1,000 matched seeds = 48,000 runs. Changing `seedCount` to
20,000 makes 960,000 runs across varied settings. `maxRuns` bounds the declared
budget before Cartesian expansion; the compiler also caps conditions at 10,000
and selected metrics at 100. Supply `sampleContexts` plus `samplingSeed` in the
spec to sample a larger Cartesian space reproducibly without materializing it.
Each sampled context still includes every path. Shortlist compilation decodes
the selected contexts directly and rejects overlap with the screening seed range.

Each axis value supplies original option/value pairs. Independent species/stat
entries merge into one `-speciesStats` or `-startingPopulations` option; conflicting
entries or options are rejected. Specify all desired starting populations:
the original harness sets omitted species to zero. Frozen baseline entries in
those two compound options are retained unless explicitly replaced. Paths may
use the existing offer-aware Trailblazer, Warren or Gardeners policies, explicit
legacy skill schedules, frozen authored asset schedules, or launch loadouts.
Offer-aware policies record the choices actually available and selected, rather
than guaranteeing one upgrade order. Authored schedules retain the original
cumulative-loadout/boundary restrictions. All paths must plan the same declared
`horizonTicks`: explicit `-runTicks`, or phase length × schedule length; existing
policies have six phases. Short early endings remain measured outcomes.

Absolute starting-stat overrides use registered IDs, for example:

```text
-speciesStats fox:energy.starting=80,hare:awareness.vision-range=7
```

All 27 IDs in `SpeciesAttributeIds` are supported, including attack/block,
movement, metabolism, energy capacity/starting energy, awareness, reproduction,
litter size, digestion, crowding and resource stats. The registry defines integer
versus float values. Unknown species/IDs, duplicates, fractional integers,
nonfinite values and requests silently normalized by the rules constructor are
rejected. All edits apply together before validation and before path upgrades.
They alter each condition's immutable input data and fingerprints; scenario and
species assets are not edited. A new gameplay algorithm still needs a new source
build, export and reference check.

## Analyze the full sweep

The analyzer verifies frozen inputs, completed coverage and the combined JSONL
hash, then indexes observations in `metrics.sqlite`. It streams runs instead
of holding a million reports in memory. Selected overall/phase metrics occupy
numeric columns `m0`, `m1`, etc.; the `metrics` table maps those columns to names
and status columns. `cases` keeps axes, paths and references; `provenance` keeps
the batch/input hashes. Status codes are Valid=0, NotApplicable=1, Invalid=2,
Missing=3. Invalid/unavailable values remain SQL NULL, never measured zero.

- `metrics.csv`: per-condition/per-phase valid sample counts, mean, sample SD,
  approximate mean CI, min/max, p10/median/p90, N/A/invalid/missing counts and
  seeds that never reached a phase, including phases reached by no seeds in a
  condition. Overall phase is 0.
- `paired-deltas.csv`: candidate minus reference for the same seed and phase,
  paired sample count, mean/SD/CI, numeric wins/losses, unmatched/invalid pairs
  and different-window counts. `same-context-path` compares each path with that
  context's reference; `axis:<id>` changes one axis while holding other axes and
  the path fixed, against the axis's first declared value when present;
  `global-context` compares against the first selected context's reference and
  may change several parameters. Missing axis references in a sparse selection
  produce no comparison. Binary `outcome.*` measures also report exact McNemar p.
- `analysis.md` / `analysis.json`: a screen ranked by an explicitly chosen
  metric/direction. `shortlist-contexts.json` feeds the fresh-seed compile step.

Metric names include `population.<species>.final/min/max/mean`,
`herbivore.hare.PREY/pAVI/STRV/sAVI/CRWD/cAVI/BIR/RFS/APS`,
`predator.fox.KIL/huntAVG/STRV/BIR/AHS`,
`activity.fox.reproductionBlockedEnergy`, and
`activity.hare.births.per100Ticks`. The slash separators here enumerate metric
suffixes, not literal metric names. `population.<species>.mean` averages
post-tick populations over observed ticks; trajectory summaries also retain
initial/final counts, positive ticks, first-zero tick (-1 if absent), and the
population-tick integral. Trajectory metrics are overall only. Final populations,
activity and slash lines are available per observed phase. Per-100-tick activity
rates use actual window duration, so check `differentWindows` before interpreting
raw cumulative deltas across early-ending runs.

`outcome.playerAliveAtHorizon` and `outcome.allSpeciesAliveAtHorizon` require the
planned tick horizon to be reached plus positive ending populations. They are
diagnostic outcomes, not the undefined ecology-viability criterion. Use primary
measures tied to the hypothesis; APS/FPO alone should not select gameplay. The
default example rank illustrates the pipeline, not an agreed design objective.
CIs use normal approximations and p values are unadjusted for multiple searches.
Few-seed screening ranks can move substantially on fresh seeds. Inspect distributions,
path sensitivity and failure categories; confirm candidates and then play them
before deciding whether a scenario or mechanic is good.

Keep raw JSONL to select different metrics in a later analysis. Analysis sorting
uses disk; reserve space for SQLite and CSV as well as batch output. Analysis
tables scale with runs × observed phases × selected metrics. CSV summaries scale
with conditions × phases × metrics; the present script retains these smaller
summary rows in memory. Reduce selection breadth or analyze separate declared
batches if this becomes material. There is no automatic production promotion.

## Reuse existing cohort tools

Export a bounded cohort from measured rows without rerunning simulations:

```powershell
python .\tools\CellSim.Batch\sweep.py export-report .\artifacts\screen `
    CONDITION_ID .\artifacts\candidate-cohort.json --max-runs 100
```

The original build must match. Export takes the first ordered seeds (maximum
1,000), generates original report metadata without simulating, removes empty
header aggregate placeholders, and retains batch/source/assembly/input hashes.
Use the same context and seeds for a reference cohort and pass both reports to
the existing artifact-summarization script. It now preserves all-species slash
lines, Mutation choices, exact trajectory aggregates, and compact-input limits.
Its population `mean` includes the initial sample for legacy compatibility;
the sweep metric's post-tick mean explicitly excludes it. Omitted detail traces
cannot become measured zero-event evidence; replay a condition/seed for detail.
The cohort is bounded compatibility evidence; use SQLite/CSV for the full sweep.

## Build and freeze inputs

Run from the `LearningIndieDev` directory with .NET 8 SDK installed. Close this
project's Unity Editor before export; the exporter refuses a project lock.

```powershell
dotnet build .\tools\CellSim.Batch\CellSim.Batch.csproj -c Release
$runner = '.\tools\CellSim.Batch\bin\Release\net8.0\CellSim.Batch.dll'
dotnet $runner self-test

.\tools\Export-CellSimSnapshot.ps1 `
    -OutputPath "$PWD\artifacts\cellsim-input.json" `
    -ScientificArguments @(
        '-playerSpeciesId','hare','-runTicks','700',
        '-gridWidth','36','-gridHeight','20',
        '-startingPopulations','plant=400,hare=20,fox=10',
        '-wrapEdges','true','-combatMode','opposed-roll',
        '-experimentalFeatures','bev-experimental') `
    -SeedStart 100000 -ReferenceSeeds 4
```

400/20/10 is a tooling fixture, not an approved new starting population. The
snapshot contains resolved species rules, terrain, awareness, starting values,
alpha rules, upgrade definitions, scientific arguments and source/content hashes.
The reference runs use the asset-resolved data directly in Unity. Files are never
overwritten; export a new filename after source or input changes. The source hash
includes the linked harness, serializers and portable DTO code as well as the
simulation. HEAD is recorded for context; hashes identify uncommitted changes.

The .NET-only `UnityValues.cs` supplies the small value types, field JSON adapter
and inert authoring attributes that these files use. It does not recreate Unity's
lifecycle or asset system. Unity assets cannot be instantiated by this runner.

## Pilot before a large matrix

Copy `tools/CellSim.Batch/example-plan.json` to a new file in that same directory.
For the first run, set `seedCount` to 4, `seedStart` to the exported reference's
starting seed, `chunkSize` to 2, and give `output` a new pilot directory. Paths in
the plan resolve relative to the plan file, not the shell's directory.

```powershell
dotnet $runner batch .\tools\CellSim.Batch\pilot-plan.json --dry-run
dotnet $runner batch .\tools\CellSim.Batch\pilot-plan.json
dotnet $runner verify .\artifacts\cellsim-input.reference.jsonl .\artifacts\pilot\runs.jsonl
```

`verify` requires corresponding rows in the same order and fails on missing,
extra or mismatched runs. It compares final cell-state hashes, counters, numeric
statuses, timings, phase windows, loadouts and Mutation choices exactly at native
report precision. Only named derived stat-line rates allow `1e-6` relative or
absolute rounding differences: Mono can retain intermediate precision in these
report-only calculations. Unity's absent arrays/default stat-line objects are
normalized to their .NET equivalents. Omitted event arrays and intermediate
population history are outside compact-report parity. An exact final state hash
includes all cell fields, with live entity IDs represented by grid positions and
missing tracking targets represented by -1, to remove process-global labels.

For a policy fixture, export without `-runTicks` or `-runDurationSeconds`, and pass
`-phaseLengthTicks 100 -mutationPolicy trailblazer`. The original policy runner
uses six phases; this does not imply a seven-phase or tick-700 policy. Use an
explicit seven-entry schedule if seven phases are required. Existing parser
constraints and authored boundary restrictions apply unchanged.

## Conditions, progress and recovery

Each condition has a unique safe ID and an array of original harness option/value
pairs. Condition values replace matching snapshot arguments. All conditions use
the same seed range, enabling matched-seed comparisons. For example:

```json
"conditions": [
  { "id": "control", "arguments": [] },
  { "id": "fox-cooldown-2", "arguments": ["-foxAttackCooldownTicks", "2"] },
  { "id": "population-600-40-25", "arguments": ["-startingPopulations", "plant=600,hare=40,fox=25"] }
]
```

Arguments are case-sensitive and unknown, repeated and incompatible settings are
rejected before dispatch. Overrides do not remove unrelated base arguments: make
the exported base neutral when comparing run-length and policy families. Registered
starting-stat values can vary with `-speciesStats`; changes to rule structure or
gameplay code require their own authored export and source build.
The shared linked code supports flat runs, legacy skill/value overrides, frozen
authored upgrades, scheduled phases and the existing offer-aware policies. Paired
lockstep opportunities still require the existing linked-arm Unity runner.

```powershell
dotnet $runner status .\artifacts\cellsim-million
dotnet $runner batch .\tools\CellSim.Batch\example-plan.json --resume
dotnet $runner replay-batch .\artifacts\cellsim-million control 100000 .\artifacts\seed-100000-detail.json
```

Ctrl+C stops the coordinator's owned workers and retains validated completed
chunks. A killed coordinator can leave workers alive; their exclusive file claims
count against the resumed coordinator's worker limit, and each has its own chunk
deadline. Only owned child processes are terminated. Concurrent coordinators for
the same output are refused. Failed attempts remain available for diagnosis and
get at most one retry in an invocation. Never resume a batch after changing its
source build, snapshot, runtime, seed range, chunk size or scientific conditions.
Use a new output directory instead. Worker count can change on resume. An existing
chunk keeps the timeout recorded when it was first dispatched.

Completed chunks publish a hash and exact ordered seed coverage after validation.
Missing/damaged chunks cannot count as success. Corrupted completed evidence is
refused; investigate it and start a new output rather than silently replacing it.
Aggregation streams validated chunks into a staging file and publishes the final
JSONL plus summary only after complete coverage. An interrupted aggregate can be
rebuilt from the valid chunks on resume. Inputs and final outputs stay local.

## Output and million-run sizing

`batch.json` freezes resolved plan, source hash, assembly hash and batch identity.
`status.json` records progress. Each chunk has an immutable attempt directory,
`runs.jsonl`, status and completion record. Final `runs.jsonl` is ordered by
condition ID and seed. `summary.json` records coverage, throughput, output hash,
peak single-worker memory and per-condition population totals, player extinctions,
all-species presence at the run's end and total ticks. Those diagnostic totals are
not an ecology-viability decision; divide population/tick totals by condition run
counts for averages and use per-seed rows for paired analysis.

Compact rows retain activity, reproduction failure categories, behavior totals,
all herbivore/predator species' applicable stat lines, full trajectory aggregates,
phase boundaries, choices and final state digest. They keep
the first/final population snapshots and omit detailed event arrays. Death evidence
needed by stat lines and complete population history are still held during a run,
then discarded with that run. This bounds memory by concurrent runs, not total
batch size. A detailed replay records the original event arrays for one seed.
The legacy policy report emits an inapplicable predator stat line for a Hare;
the batch validates FPO accounting for the species' actual role and preserves the
legacy report for parity. Reproduction accounting is checked for all species.

The example plan schedules one million runs for one condition. Total runs are
`seedCount * number of conditions`; 100,000 matched seeds across ten conditions
also equals one million. Start with measured worker/chunk limits, then use a new
large plan. No million-run batch is launched by building or reading this example.
Retaining both chunk and combined JSONL roughly doubles raw-row disk consumption.
Reserve space for that plus failed attempts and detailed replays. Watch a pilot's
`outputBytes`, peak memory and actual completed runs/second before extrapolating.

This first implementation is one-host CPU processing. Distributed dispatch, GPU
execution, custom binary storage and a dashboard are deferred until measured
throughput or data-analysis needs justify them. Linux execution and cross-runtime
parity have not been validated. The coordinator scans an in-memory job list;
increase chunk size if very large job counts make scheduling overhead material.

## Runnable validation

After a flat export, run the standard-library Python integration check in a new
artifact directory. It writes no production assets and needs no Python packages.

```powershell
python .\tools\CellSim.Batch\check.py .\artifacts\cellsim-input.json .\artifacts\cellsim-check

python .\tools\CellSim.Batch\check_sweep.py .\artifacts\screen `
    .\artifacts\screen-analysis .\artifacts\sweep-check
```

It verifies serial/parallel byte-identical outputs, compact/detailed state parity,
condition replay, dry run, strict input/overflow rejection, resume identity,
controller exclusion, recovery with surviving workers, interrupted aggregation,
corrupt-chunk refusal, timeout refusal and parity mismatch refusal. The check's
flat length overrides require an export without a Mutation policy or schedule.
Unity's focused EditMode telemetry test also compares compact/detailed state and
activity counters on every tick. A console build alone is not Unity validation.

Standalone tool guard clauses use compact formatting; existing domain conventions
and serialized assets are preserved. See the dated validation record below for
the actual evidence, rather than treating this list of checks as a passing claim.
The sweep check uses a bounded Hare/Fox fixture to reconcile distributions,
matched deltas, held-axis references, SQLite coverage, invalid/N/A handling,
sampling, budgets, cohort summarization and corrupted/duplicate seed rejection.

## October 8 sweep extension validation

The current shared source hash is
`89c3792a277ff216d0e109ce3b784e3bc4c69de954c77b11b04b336a40bb55a9`.
Local evidence is in `artifacts/cellsim-sweep-validation-20261008/`:

- Release build/self-check pass; absolute stat edits and rejection cases are
  covered alongside compact/detailed state and trajectory parity.
- **259/259 focused Unity EditMode tests pass** again. Four current-source
  reference seeds pass exact state/counter/choice/timing parity: two neutral flat
  exports and two changed-grid/population/stat Trailblazer exports. Named derived
  report-rate rounding retains the documented tolerance.
- `pilot-final`: **192 runs = 16 contexts × 3 paths × 4 seeds** (23000–23003),
  changing 100/10/5 versus 150/15/8 populations, 20x12 versus 24x16 grids,
  Fox starting energy 80/120 and Hare vision 5/7. Paths are skip-all, Trailblazer
  and Warren, with six planned 20-tick phases. This is a short tooling fixture.
- `analysis-complete`: 26 metrics, 8,736 condition/phase/metric summaries and 31,850
  paired comparison rows (including completely unreached phases);
  `sweep-check-final-analysis/checks.json` passes hand-calculated
  deltas/statistics, integrity and compatibility checks.
- `confirmation`: **120 fresh-seed runs = 5 shortlisted contexts × 3 paths ×
  8 seeds** (25000–25007); `confirmation-analysis-complete` completes. Rankings change;
  neither this small confirmation fixture nor the initial rank approves tuning.
- `integration-final/checks.json` passes the batch recovery, locking, input,
  replay, corruption and timeout checks on the new source. A detailed Warren
  replay and four-seed paired legacy cohort summary also complete successfully.
- A **960,000-run varied matrix** (16 contexts × 3 paths × 20,000 seeds) passes
  compiler/batch dry-run validation with 9,600 chunks; no workers were launched.

The fresh 192-run invocation took 8.76 seconds on eight workers, about 21.9
short-fixture runs/second, with peak single-worker memory about 68.2 MiB. Phased
rows averaged **49.4 KB**, about 49.4 GB per million combined rows or 98.8 GB
retaining chunks plus aggregate, before analysis/failed-attempt/replay storage.
This demonstrates varied-condition throughput on small grids and short horizons;
it must not be extrapolated to a production tick-700 million-run sweep. A later
capacity pilot must use the intended matrix, horizon and diagnostics.

No production scenario/stat asset, gameplay default, balance criterion, external
board or remote worker was changed. All code/document changes remain local and
uncommitted. A million varied runs are not yet executed or capacity validated.

## Initial executor validation (before sweep telemetry extension)

Verified locally on Windows, Intel Core i7-13700K (16 cores / 24 logical processors),
approximately 32 GB RAM, Unity 6000.4.6f1 and .NET runtime 8.0.29. Source checkpoint is
`e7a6c602` plus the local tooling changes. Shared source SHA-256 is
`16c7d8ef1429fbb0a3638cd4148f328e2a58679678de003ba22efef87a775359`;
the tested Release assembly SHA-256 is
`59321d354cd88f2aa2d0423f720c83b6e3cc43abefcd5a0888e8a0b304df1925`.

- Release build: zero warnings/errors. Standalone self-check passed, including
  compact/detailed parity and the Mutation-policy digest regression.
- Unity CLI `test` passed **259/259 focused EditMode tests**, including the new
  per-tick telemetry/state regression. This is not a full PlayMode/graphics test.
- **38 final-source Unity reference seeds** passed: 32 flat 700-tick wrapped
  400/20/10 runs; four early-choice Trailblazer runs at 600/40/25 with six
  100-tick phases; two authored cumulative litter-upgrade runs with seven planned
  100-tick phases. Phase runs can end early; actual ticks are retained.
- The final integration check passed all listed recovery, replay, locking,
  integrity, timeout and input checks. An initial raw `unity run -runTests`
  invocation did not execute tests because `run` injects `-quit`; the passing
  result comes from the purpose-built `unity test` command and NUnit XML.
- A three-condition/four-matched-seed matrix passed 12-run coverage and resume
  revalidation. These short matrix fixtures establish tooling behavior, not
  balance. A one-million-run plan passed dry-run validation with no worker launch.

All baseline benchmark runs used identical settings: 36x20, wrapping enabled,
700 ticks, authored 0.2-second steps, opposed combat, experimental features,
400 Plants / 20 Hares / 10 Foxes. Fresh benchmark invocations include dispatch,
simulation, validation and aggregation. The three four-seed-chunk outputs have
the same SHA-256; the 32-seed single-chunk output and the first 32 rows of the
128-seed output are also byte-identical to them.

| Seeds | Workers | Seeds/chunk | Wall seconds | Runs/second | Peak MiB per worker |
|---:|---:|---:|---:|---:|---:|
| 32 | 1 | 4 | 58.28 | 0.55 | 68.3 |
| 32 | 4 | 4 | 15.09 | 2.12 | 63.9 |
| 32 | 8 | 4 | 8.86 | 3.61 | 65.3 |
| 32 | 1 | 32 | 54.16 | 0.59 | 62.6 |
| 128 | 8 | 16 | 34.37 | 3.72 | 66.3 |

Eight workers gave about **6.6x the one-worker throughput** in the matched
four-seed-chunk test. Larger chunks helped modestly in these pilots; they did
not remove the simulation's CPU cost. At the 128-seed rate, one million of this
flat fixture projects to **about 74.6 hours**. This is a small-pilot extrapolation,
not a measured million-run completion or a universal rate for other populations,
policies, grids, horizons or hardware. Long-running throughput, thermal behavior,
disk capacity and larger concurrency still need a bounded capacity test before a
production-scale job. The eight-worker limit is the measured starting point.

Before the additional trajectory and all-species telemetry, baseline rows
averaged about 5.6 KB: approximately 5.6 GB for a million combined
rows, or 11.2 GB retaining chunks plus aggregate. The four phased policy rows
averaged about 47.6 KB: approximately 47.6 GB combined, or 95.2 GB retaining both.
These decimal-GB estimates exclude attempt metadata, failed attempts and detailed
replays; reserve additional space. Phase-rich diagnostics can dominate storage.

Raw evidence is under the ignored local
[`artifacts/cellsim-portable-validation-20261007/`](../artifacts/cellsim-portable-validation-20261007/)
directory (created October 7; final checks completed October 8):
`editmode-final.xml`, `integration-final-build/checks.json`, frozen input/reference
files, `benchmark-{1,4,8}-workers/summary.json`,
`serial-32-one-chunk/summary.json`, `parallel-128/summary.json`,
`policy-final-build/summary.json`, `authored-verified/summary.json`,
`matrix-coverage/summary.json` and their manifests/attempts. Earlier failed export
and policy attempts are retained as debugging evidence, not passing results.
The earlier Docker build pack remains historical. No Docker deployment, Linux
parity, million-run execution, commit, push or new gameplay-tuning approval is
part of this verification record.
