# Broad CellSim screen - October 8, 2026

Bevin requested a larger executable sweep after authorizing standalone population,
grid, stat and skill-path diagnostics. Codex selected the bounded values below
within that request. Decision owner: Bevin. This is an exploratory candidate screen;
the selected levels and ranking metric are experimental inputs, not approved tuning.

Question: where do population, space, starting energy, awareness and available Hare
skill paths improve species survival and event pace, and where do paths depend on context?

Hypothesis: skill-path impact varies with density and starting stats. Evidence is the
matched path and held-axis differences, distributions and slash lines; a null result
is useful. No production success threshold or ecology viability definition is asserted.

| Axis | Levels |
| --- | --- |
| Plants / Hares / Foxes | 100/10/5; 250/20/10; 400/40/20 |
| Map | 30x18; 36x20; 42x24 |
| Fox starting energy | 80; 120; 160 |
| Hare vision | 5; 7; 9 |
| Hare paths | skip-all; Trailblazer; Warren; Gardeners |

81 contexts x 4 paths x 100 matched seeds = 32,400 runs. Seeds 30000-30099 are
shared across every arm. Six 100-tick phases give a planned 600-tick horizon;
early endings and unreached windows remain explicit. Wrap edges, opposed-roll combat,
Bev experimental features, Hare player species and 0.1-second steps come from the
frozen Unity export. Gardeners/Trailblazer/Warren use current offer-aware policies;
actual acquired choices are retained per run.

The independent capacity check uses the same 324 conditions at seeds 29990-29991
(648 runs). These rows are excluded from the main analysis.

The job started with eight workers. At Bevin's request it resumed with 32 workers
at 05:46:45 UTC on October 8, retaining completed chunks and live inherited workers.
It executes 1,296 chunks of 25 runs with a 3,600-second deadline per chunk.
All scientific inputs, snapshot, analyzer and compiled shared core are frozen here.
The source hash in snapshot.json identifies local shared-code contents; its source
commit alone does not describe the uncommitted implementation. plan.json and the
batch identity lock resolved settings, snapshot, assembly and runtime.

Primary exploratory shortlist ranking: all species alive at the planned horizon.
This endpoint is not the project's still-undefined ecology-viability score. All 26
declared metrics also cover player survival, population trajectories, herbivore and
predator slash lines, reproduction barriers and births per 100 ticks, overall and
per phase. Raw paired differences preserve same seeds, controls, held-axis comparisons
and status/window exclusions. Confidence intervals and p values are exploratory and
unadjusted across this broad screen. Interesting candidates require fresh-seed and
longer-horizon confirmation plus human gameplay review.

Execution succeeds when all 32,400 uniquely covered rows pass chunk validation,
the batch summary/hash verifies, and SQLite/CSV analysis completes. Failed chunks,
coverage errors, drift or analysis failure mark the pipeline failed and retain evidence.
The job does not edit production assets or approve balance.

run-workers32.ps1 -Resume launches the resumed batch and then analysis automatically.
The original run.ps1 is retained as the eight-worker launch record. pipeline-state.json
records Running/Analyzing/Completed/Failed; sweep/batch/status.json records completed
chunks and active workers. pipeline-workers32.log and pipeline-workers32.errors.log
retain current output; pipeline.log and pipeline.errors.log retain the first invocation.
Keep this machine awake for the background job.

Completion record: the job and automatic analysis finished October 8 at
06:12:13 UTC (02:12 EDT). All 32,400 runs and 1,296 chunks validated; independent
post-run hash/coverage/SQLite checks passed. See completion-review.md/json for
verified findings. Initial launch to completed analysis was 39.86 minutes.

Outputs after completion:
- sweep/batch/runs.jsonl and summary.json: complete raw evidence and provenance.
- analysis/analysis.md and analysis.json: exploratory ranking and analysis provenance.
- analysis/metrics.sqlite: run/phase metrics with unavailable-value statuses.
- analysis/metrics.csv and paired-deltas.csv: distributions and matched comparisons.
- analysis/shortlist-contexts.json: contexts suitable for fresh-seed confirmation.

To recover an interrupted simulation batch at the current worker limit, use
run-workers32.ps1 with -Resume. plan-workers32.json differs only in worker count;
the original plan and analyzer hashes remain frozen. worker-change.json and
launch-workers32.json record the requested execution change and new process.
Before recovery, inspect the process and logs; do not launch another coordinator
while this one is active. Existing analysis output requires inspection and a new
analysis output directory rather than overwriting.
