# Quarter-million CellSim grid kickoff

October 8, 2026. Bevin explicitly requested another grid, 16 workers and approximately
250,000 runs. Codex selected a full five-level expansion retaining prior anchors.
Branch BevBranch / HEAD e7a6c602; existing implementation remains uncommitted.
No production tuning or new simulation logic changed.

| Axis | Values |
| --- | --- |
| Plants / Hares / Foxes | 100/10/5; 175/15/8; 250/20/10; 325/30/15; 400/40/20 |
| Map | 30x18; 36x20; 42x24; 48x28; 54x32 |
| Fox starting energy | 40; 60; 80; 120; 160 |
| Hare vision | 5; 6; 7; 8; 9 |
| Hare path | skip-all; Trailblazer; Warren; Gardeners |

625 contexts x four paths x 100 fresh matched seeds (40000-40099) = exactly
250,000 main runs. All 81 prior contexts / 324 path arms have identical arguments
and new seeds: 32,400 replication runs plus 217,600 runs in 544 added contexts.
Population is a compound bundle, not an isolated change to one species.

Six 100-tick phases retain the planned 600-tick horizon. The same frozen Unity export
sets Hare player species, wrap edges, opposed-roll combat, Bev experimental features
and 0.1-second steps. Actual offer-aware choices and early endings remain recorded.
Sixteen workers execute 5,000 chunks of 50 with 3,600-second per-chunk deadlines.

## Frozen evidence and launch

Run root:
`F:/ForkBin/GameDev/LearningIndieDev/artifacts/cellsim-250k-sweep-20261008-030316`.
Shared core, analyzer and snapshot copied from the completed/verified 32,400-run
experiment. Assembly and analyzer hashes match the prior frozen files. This explicit
reuse prevents content/source drift while replicating and extending the panel.

Source hash:
`89c3792a277ff216d0e109ce3b784e3bc4c69de954c77b11b04b336a40bb55a9`.
Main identity:
`ea092fa4ac396e4bf93b567e8aa7b1de886f5ef2303c3cb7d70b54aa509af12c`.
Copied runner self-test and strict full-grid compile/dry-run passed. Assertions
verify exact count, 16 workers, all prior arms unchanged and no prior seed overlap.

A separate full-grid execution check passed 2,500 runs at seed 39990, with
exact one-run coverage per condition and verified raw hash. It took 411.553 seconds
at 16 workers, generated 106,644,586 raw bytes and peaked at 80,433,152 bytes per
worker. One-seed chunks include startup/JIT costs: 6.0745 runs/second is not a
main-chunk throughput benchmark. This check is excluded from the 250,000-run evidence.
New-grid raw-size extrapolation is about 10.66 GB, with retained chunk copies and
analysis adding storage; approximately 284 GiB was free on F: during preflight.

Hidden background pipeline launched at 07:12:34 UTC / 03:12 EDT, PID 49536.
At the first verified checkpoint: Running, 8/5,000 validated chunks (400 runs),
16 active workers confirmed as actual processes, elapsed 25.47 seconds, error log empty.
Completion is pending; inspect current status rather than relying on this checkpoint.

## Outputs and recovery

`EXPERIMENT.md` defines the question, hypothesis, scope and evidence boundaries.
`launch.json` records source/build and launch provenance. `pipeline-state.json`
and `sweep/batch/status.json` show progress; `pipeline.log` /
`pipeline.errors.log` retain output. `run.ps1 -Resume` recovers interrupted chunks
using frozen inputs. Inspect live owned processes before recovery; existing analysis
output requires review and a new destination. Keep the machine awake while running.

After complete validated raw coverage, the wrapper automatically runs the original
26-metric SQLite/CSV analyzer. It retains phase-aware slash lines, populations,
survival, reproductive barriers, births per 100 ticks, status-aware distributions,
same-context path and held-axis paired differences. Expected files include
`sweep/batch/runs.jsonl` / `summary.json`, `analysis/metrics.sqlite`,
`metrics.csv`, `paired-deltas.csv`, `analysis.md/json` and
`shortlist-contexts.json`.

All-species survival at tick 600 is the exploratory shortlist endpoint, not an
approved ecology-viability definition. Inference remains unadjusted; pooled results
share correlated seeds and contexts. Fresh seeds allow independent replication of
previous selected arms, but tick-700 ecology, gameplay, event pace and mechanism
review remain separate. No further experiment or production decision is implied.

## Verified completion and first findings

The batch and automatic analysis completed at 14:09:02 UTC / 10:09 EDT on October 8.
All 250,000 runs / 5,000 chunks completed. First launch through completed analysis
took 6h 56m 29s; batch execution/aggregation took 6h 41m, with approximately
15m 25s remaining pipeline time. Fresh-batch throughput was 10.389 runs/second for
this expanded panel at 16 workers; this is not a matched 16-vs-32 performance test.
Peak individual worker working set was 103,989,248 bytes (99.17 MiB).
No owned batch/worker processes remain and the pipeline error log is empty.

Independent checks verified input and assembly identity, complete raw hash,
CSV/shortlist hashes, SQLite integrity, exact per-arm coverage of 100 unique seeds
40000-40099, and valid binary overall outcome statuses. Raw output is
10,883,166,732 bytes (10.88 GB), SHA-256:
`aed27d0b76deaa39c61e944e478407003786b16322ba5187c96bb909934e5edf`.
Analysis contains 455,000 metric-window summaries and 2,252,068 paired comparison rows.
`completion-review.md/json` retain verification, matrices, selected arms and replication.

New strongest selected context: 325 Plants / 30 Hares / 15 Foxes, 54x32,
Fox starting energy 80, Hare vision 9:

| Hare path | All species alive at tick 600 | Hare species alive at tick 600 |
| --- | --- | --- |
| Skip all | 16/100 | 32/100 |
| Trailblazer | 15/100 | 36/100 |
| Warren | 14/100 | 39/100 |
| Gardeners | 68/100 | 72/100 |

Gardeners improves the matched all-species endpoint by 52 percentage points:
56 seeds improved, four worsened and 40 tied. Exploratory paired normal 95% CI:
+40.7 to +63.3 points. All ten highest-ranked arms use Gardeners.
The largest map is the tested boundary; this does not locate a global optimum.

The prior winning context (250/20/10, 42x24, Fox energy 80, vision 9) reproduced
the Gardeners advantage on fresh seeds: 54/100 vs 18/100 skip-all (+36 points;
paired normal 95% CI +24.0 to +48.0). Previously it was 60/100 vs 19/100 (+41).
Cross-batch seeds are disjoint: these are replication rates, not paired old/new deltas.

The earlier vision hypothesis did not clearly reproduce in that prior context:
vision 9 vs 5 with Gardeners is now 54/100 vs 51/100 (+3 points; 95% CI -9.9 to
+15.9; exact McNemar p=.761), compared with 60 vs 38 (+22) in the selected first
screen. Treat this stat effect as context-dependent/unconfirmed.

Population/map pooled matrices show differing shapes across bundles: denser
bundles improve at larger maps, while the 100/10/5 bundle has zero all-species
survivals at 54x32 over its 10,000 pooled runs. Gardeners' pooled endpoint is
15.52%, versus 3.47% skip-all, 4.80% Trailblazer and 4.61% Warren, each covering
the same 625 contexts / 62,500 runs. These are correlated descriptive mixtures.

Every arm still has 100 seeds. Ranking among 2,500 arms and unadjusted inference
do not approve production tuning. Review acquired choices, phase/window-aware
failure causes and personal event pace; confirm selected contexts through tick 700
and human gameplay. No additional simulations or production tuning were performed
during the completion check.
