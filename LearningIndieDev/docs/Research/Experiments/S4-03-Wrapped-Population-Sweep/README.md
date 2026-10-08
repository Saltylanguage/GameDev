# Wrapping-grid starting-population sweep

Experiment ID: **S4-03-20261004-wrapped-population-sweep**. Human decision owner: Bevin; team reviewer: Salty. Status: local screen and fresh-seed recheck complete; human candidate selection pending. Authorized question: find interesting starting populations after Bevin visually preferred wrapping, despite failures at 400/20/10. This is a population search with fixed species and skill rules, not approval of a new scenario default or a balanced ecosystem.

## Frozen screening inputs

- Production Forest Edge runtime rules, overridden in memory to 36×20, wrapping enabled, 0.1-second steps, six 100-tick phases, 600-tick horizon.
- Plants: 100, 200, 300, 400, 500, 600. Hares: 5, 10, 20, 30, 40, 60, 80. Foxes: 1, 2, 4, 6, 10, 15, 25, 40. All 336 combinations fit the 720-cell grid.
- Five common seeds per combination, 11000–11004: 1,680 screening runs. Every run starts a fresh world; phases carry the evolved world forward. Hare extinction stops the run early.
- Skip all Mutation decisions to isolate starting populations. No added Genome, reinforcements, or species-rule tuning. Natural opposed-roll combat, Bev experimental features, no coupled responses or additional Fox cooldown.
- Exploratory output goals: distinguish survival, all-three-species presence, late collapse, growth/collapse and recovery. These are selection aids, not pre-approved ecological acceptance thresholds. Five seeds are insufficient to establish reliability. Freeze a diverse shortlist before checking it on 20 new common seeds, 12000–12019.

## Execution and evidence

Local ignored evidence lives at `artifacts/s4-population-sweep-20261004/`. `experiment-contract.json` froze inputs, 73 source/production-asset SHA256 hashes, branch and dirty baseline commit before dispatch. `source.diff` records local changes; the source is not represented by the baseline commit alone. Unity version: 6000.4.6f1, Windows desktop; no remote workers.

The research-only `SweepWorker.cs` loads the production asset on the main thread, then calls the existing `CellularSimulationExperimentRunner.RunScheduledSimulation` with at most four isolated local workers. It does not implement new simulation rules or change project assets. Serial/parallel replay of four cases passed, and one case matched the earlier normal CLI full report across every retained field. Floating comparisons allow 1e-6 serializer precision; the two unused diagnostic arrays normalize JsonUtility's empty arrays versus Newtonsoft's nulls. See `pilot-validation.json` and the runnable `validate_pilot.py`.

Compact records retain every population tick, final and phase slashlines with validity statuses, cumulative/phase activity and behavior totals, rule fingerprints, loadouts and stop ticks. They omit individual combat/death/behavior-transition records, which are unnecessary for the screening question and would multiply file size. These are compact screening records, not full event replays. Per-run errors and stage progress remain explicit; existing reports are not overwritten. Runtime debug messages remain unchanged; no new per-tick console output is added for the sweep.

No source assembly reload should occur during the background worker. If interrupted, retained case files and progress make the incomplete stage visible; use a new stage and request identity rather than overwrite it. `cancel` in the stage directory cancels between runs. Do not change rules or resume an old stage under new source.

## Reading the results

Read the factual [report](REPORT.md), then the separate [analysis](ANALYSIS.md). Population summaries and per-run/phase CSVs retain extinction runs. Ratio means use only `Valid` values and show their valid and N/A counts; N/A is not zero. The slashline's cumulative `sAVI`, `RFS` and `APS` are not interchangeable with horizon survival or a judgment of fun. Crowding's `cAVI` measures deaths, not energy saved.

Fox and Plant counts are recorded **at the player's stop tick**. Counts from an early Hare extinction do not describe the ecosystem at tick 600. All-three-species presence is counted only for surviving runs that reach the complete horizon. Even presence at tick 600 does not establish resilience or long-term stability.

## Human decision

Bevin prefers the wrapping movement feel and authorized the population sweep on October 4. Candidate selection and visual comparison remain human decisions. The Inspector fixture stays 400/20/10 and bounded remains the authored default; this search does not silently select or publish new settings. Salty's journey/DNA and secondary ecology-mode planning remains separate.

## Completed outputs

The screen completed 1,680 runs; the frozen seven-preset recheck completed 140,
for 1,820 main runs plus eight pilot executions. Read REPORT.md and ANALYSIS.md
for the results and candidate recommendations. Local CSVs open in Excel; the
heatmap shows every population combination and trajectory plots show all 20
fresh histories per preset. Production presets remain as authored.
