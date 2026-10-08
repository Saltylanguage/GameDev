# Quarter-million CellSim grid - October 8, 2026

Bevin requested another grid using 16 workers and approximately 250,000 simulations.
Codex selected a full five-level expansion of the previous four axes. Decision
owner: Bevin. These levels are diagnostic inputs, not approved production tuning.

Question: do the previous population/map/path survival patterns reproduce on fresh
seeds, and where do intermediate populations, greater space, lower Fox energy and
intermediate Hare vision expose better scenarios or path-dependent mechanics?

Hypothesis: the Gardeners advantage and middle-population region found in the
first screen depend on starting conditions. Retain contrasting outcomes and inspect
phase-aware slash lines, event pace, reproductive barriers and failures rather than
equating all-species survival with biome stability or fun.

| Axis | Levels |
| --- | --- |
| Plants / Hares / Foxes | 100/10/5; 175/15/8; 250/20/10; 325/30/15; 400/40/20 |
| Grid | 30x18; 36x20; 42x24; 48x28; 54x32 |
| Fox starting energy | 40; 60; 80; 120; 160 |
| Hare vision | 5; 6; 7; 8; 9 |
| Hare path | skip-all; Trailblazer; Warren; Gardeners |

5 x 5 x 5 x 5 = 625 contexts. Four paths and 100 matched seeds (40000-40099)
produce exactly 250,000 main simulations / 2,500 conditions. All 81 prior contexts
are included on fresh seeds: 32,400 replication runs plus 217,600 runs in 544 added
contexts. Holding the same frozen shared core, Unity snapshot and 600-tick horizon
allows direct replication of the prior endpoint without source/content drift.
No prior runs are imported into this evidence batch.

Six 100-tick phases; Hare player species; wrap edges; opposed-roll combat; Bev
experimental features; 0.1-second steps. Offer-aware policies record actual acquired
choices. Early endings and unreached planned phases remain explicit. The compound
population axis changes a bundle, so its differences do not isolate one species.

Sixteen workers process 5,000 chunks of 50 runs, with a 3,600-second chunk deadline.
A separate execution check covers all 2,500 conditions with seed 39990 at 16 workers
before launch. This one-seed check is execution/capacity evidence, not balance evidence,
and is excluded from the main analysis. Its small chunks include process-start costs;
linear timing extrapolation is a coarse estimate, not a 16-worker performance benchmark.

The frozen tool and snapshot were copied from the verified 32,400-run experiment
at ../cellsim-large-sweep-20261008-012921. Assembly and analyzer copy hashes match.
The runner self-test and full scientific configuration dry run must pass. Source
hash: 89c3792a277ff216d0e109ce3b784e3bc4c69de954c77b11b04b336a40bb55a9.
Main identity: ea092fa4ac396e4bf93b567e8aa7b1de886f5ef2303c3cb7d70b54aa509af12c.
HEAD e7a6c602 labels the base commit; local shared-code changes are represented by
the frozen source hash, not solely that commit.

The 26 declared metrics and original analyzer retain overall/per-phase status-aware
slash lines, survival, population trajectories, births per 100 ticks, reproductive
barriers, distributions, same-context path and held-axis paired comparisons.
The first exploratory shortlist ranks allSpeciesAliveAtHorizon. Normal CIs and
McNemar p values are unadjusted; repeated seeds across contexts correlate pooled
observations. A selected arm is a candidate for confirmation and gameplay review.

Execution success: all 250,000 uniquely covered rows validate, the raw/output
hashes reconcile and SQLite/CSV analysis completes. Corrupt coverage, identity drift,
failed chunks or analysis failure retain diagnostics and mark the pipeline failed.
No production asset editing or automatic balance approval occurs.

The background run.ps1 executes batch then analysis. Status: pipeline-state.json and
sweep/batch/status.json. Logs: pipeline.log / pipeline.errors.log. Launch/build/input
provenance: launch.json, spec.json, sweep/sweep.json and sweep/batch/batch.json.
Keep the host awake while running. Inspect owned processes before using
run.ps1 -Resume after interruption; existing analysis output requires review and a
new output directory rather than overwriting.

Expected outputs: sweep/batch/runs.jsonl and summary.json; analysis/metrics.sqlite,
metrics.csv, paired-deltas.csv, analysis.md/json and shortlist-contexts.json.
Tick-700 ecology, human gameplay and mechanism validation remain separate follow-ups.

Completion: all 250,000 main runs / 5,000 chunks and automatic analysis finished
October 8 at 14:09:02 UTC (10:09 EDT). Independent identity/hash, exact seed coverage,
SQLite integrity and binary outcome status checks passed. Launch to completed
analysis took 6h 56m 29s. See completion-review.md/json for selected arms,
population/map grids, prior-context replication and evidence limits.
