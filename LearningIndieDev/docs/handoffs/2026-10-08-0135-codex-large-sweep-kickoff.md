# Larger standalone CellSim sweep kickoff

Date: October 8, 2026. User/decision owner: Bevin. Branch: BevBranch;
HEAD e7a6c6022ffbc5c72a7269e4428a36a8d340b869. Existing implementation remains
uncommitted. No production balance values changed.

Bevin requested actual execution of a larger varied-parameter sweep. Codex selected
81 contexts and four current Hare path policies within that authorization:

| Axis | Values |
| --- | --- |
| Plants / Hares / Foxes | 100/10/5; 250/20/10; 400/40/20 |
| Map | 30x18; 36x20; 42x24 |
| Fox starting energy | 80; 120; 160 |
| Hare vision | 5; 7; 9 |
| Path | skip-all; Trailblazer; Warren; Gardeners |

100 matched seeds (30000-30099) per arm give 32,400 runs. Six 100-tick phases
give a planned 600-tick horizon; early endings are retained. Eight console workers
process 1,296 chunks of 25 with a 3,600-second deadline per chunk. Inputs inherit
Hare player species, wrap edges, opposed-roll combat, Bev experimental features,
and 0.1-second steps from the neutral Unity export. Actual choices remain recorded.

## Location and execution state

Run root:
`F:/ForkBin/GameDev/LearningIndieDev/artifacts/cellsim-large-sweep-20261008-012921`

The run root freezes the Unity snapshot, compiled shared core, sweep compiler/analyzer,
specifications, and experiment protocol. Source hash:
`89c3792a277ff216d0e109ce3b784e3bc4c69de954c77b11b04b336a40bb55a9`.
Main batch identity:
`2d33776b894b557be56a2f854424470112386e5b43176c3b3e14cada6f3068ec`.

The hidden background PowerShell pipeline started at 05:32:22 UTC (01:32 EDT),
PID 43220. At the first recorded checkpoint: Running, 16/1,296 validated chunks
(400 runs), eight active workers, 20.3 seconds elapsed. This is a dated checkpoint;
inspect live status before relying on it. Full main-sweep completion is pending.

- `launch.json`: process, source/build/analyzer hashes, scope and calibration.
- `pipeline-state.json`: Running, Analyzing, Completed or Failed.
- `pipeline.log` / `pipeline.errors.log`: coordinator and analysis output.
- `sweep/batch/status.json`: validated chunks, active workers and elapsed time.
- `run.ps1`: executes the main batch, then analysis automatically. `-Resume`
  recovers interrupted simulation chunks using frozen inputs. Inspect live processes
  before recovery; do not create a competing coordinator. Existing analysis outputs
  require inspection and a new analysis directory.

The job continues after the Codex reply; the host must stay awake.

## Validation and analysis contract

Frozen build: zero warnings/errors; self-test passed. Two neutral existing Unity
references match exact states, counters, choices and timing; only named derived report
rates use the established 1e-6 relative/absolute comparison tolerance.

Independent capacity run covered all 324 conditions at seeds 29990-29991: 648 validated
runs, 107.663 seconds, 6.0188 runs/second, 24,405,853 raw bytes, 80,457,728-byte peak
worker working set. Its completed analysis has 58,968 metric-window summaries and
260,260 paired comparison rows. These capacity seeds are excluded from main evidence.
Linear calibration suggests roughly 90 simulation minutes plus analysis and 1.22 GB
combined raw data; chunk copies and analysis add storage. This is an estimate, with
larger main chunks reducing process startup costs and seeds changing actual workload.

After complete raw coverage, analysis verifies hashes and produces `analysis/metrics.sqlite`,
`metrics.csv`, `paired-deltas.csv`, `analysis.md/json`, and
`shortlist-contexts.json`. Twenty-six declared metrics cover player/all-species
survival, population trajectories, slash lines, reproductive barriers and births
per 100 ticks, overall and in each planned phase. Paired controls include path
and held-axis comparisons with unavailable/different-window exclusions.

Shortlisting by all-species survival is exploratory. It does not define the project's
ecology viability criterion, validate tick-700 recovery, or approve gameplay/balance.
Confidence intervals and p values are unadjusted across the broad screen. After
completion, inspect extinction causes, personal event pace and path sensitivity, then
confirm candidates with fresh seeds and longer horizons before human review.

## User-requested increase to 32 workers

At 05:46:45 UTC (01:46 EDT), Bevin requested 32 workers. The original owned wrapper
and coordinator were stopped without terminating their worker processes. The new
coordinator resumed the same batch, inherited live claims and retained completed
chunks. Before the transition, 392 chunks (9,800 runs) were validated.

Current wrapper: PID 30412, script `run-workers32.ps1 -Resume`. The alternate
`sweep/plan-workers32.json` differs only in workers=32; the original frozen plan,
its analysis hash, snapshot, assembly and scientific batch identity remain intact.
Both the dry run and the resumed manifest confirm the same batch identity.
`worker-change.json` and `launch-workers32.json` retain transition provenance.
Current output is `pipeline-workers32.log` and `pipeline-workers32.errors.log`;
the original logs remain available.

At the verified checkpoint 43.3 seconds after resume: 423/1,296 validated chunks
(10,575 runs), 32 active workers confirmed as actual processes, error log empty.
Worker working sets total approximately 1.76 GiB, with 10.61 GiB free system RAM.
The host has 16 physical cores / 24 logical processors. A throughput improvement
at 32 workers has not been established by a matched benchmark; RAM headroom does
not imply a proportional speedup. The eight-worker calibration above is historical.
Main completion and automatic analysis remain pending.

## Verified completion and initial findings

The main job and automatic analysis completed at 06:12:13 UTC / 02:12 EDT on
October 8. All 1,296 chunks / 32,400 runs finished. First launch through completed
analysis took 39.86 minutes, including the eight-to-32-worker transition. The
resumed invocation's 1,427.01 seconds excludes the earlier invocation; its reported
22.70 runs/second includes retained chunks and must not be presented as a fresh
32-worker speed benchmark.

Independent post-completion checks verified the frozen plan/input and batch identity,
complete raw SHA-256, derived output hashes, SQLite integrity, exact coverage of
324 conditions x 100 distinct seeds (30000-30099), and valid binary overall outcomes.
Raw output is 1,245,257,609 bytes; the raw SHA-256 is
`46b2ba06d06bf1f1c1b64c1ecbe97c3b359357b7524397856c4358d4140a3c99`.
The analysis has 58,968 metric-window summaries and 260,260 paired comparison rows.
`completion-review.json` and `completion-review.md` retain verification,
top arms, pooled path results, a population/map matrix and limitations.

The strongest selected context is 250 Plants / 20 Hares / 10 Foxes, 42x24,
Fox starting energy 80 and Hare vision 9. On the same 100 seeds:

| Path | All species alive at tick 600 | Hare alive at tick 600 |
| --- | --- | --- |
| Skip all | 19/100 | 47/100 |
| Trailblazer | 20/100 | 43/100 |
| Warren | 17/100 | 43/100 |
| Gardeners | 60/100 | 70/100 |

Gardeners improves the paired all-species endpoint by 41 percentage points:
48 seeds improved, seven worsened, 45 tied. The exploratory paired normal 95% CI
is +28.8 to +53.2 points. This arm was selected from 324 arms; inference is
unadjusted, and no balance approval follows. Nine of the ten highest-ranked
arms use the middle population bundle. Pooled population/map and path results
are descriptive mixtures with shared seeds, not independent-trial inference.

Review contrasting shortlisted contexts with fresh seeds, extended tick-700
ecology observation, actual choices, event pace and phase-aware failure causes.
Whole-run counts can differ because survival windows differ. No production
tuning, new experiment or human gameplay acceptance was performed in this check.
