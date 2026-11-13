# S6 predator-first Hare diagnostic — experiment contract

- **Experiment ID:** S6-PF-DIAG-20261109
- **Decision owner and approval:** Josh approved the published [S6 forecast](../../2026-11-09-S6-predator-first-forecast.md)'s test plan and push in the November 9 design chat. This record fixes the diagnostic implementation of that scope before new runs.
- **State:** Approved diagnostic; no result or balance decision yet.
- **Question:** On the current authored Forest Edge four-simulation route, when do unassisted Hares fail, and do one early avoidance, survival, or reproduction Mutation produce the expected local signal on matched seeds?
- **Hypotheses:** S6-P1 through S6-P5 in the forecast. In particular, Fox kills should be visible before Plant collapse if the opening pressure is already predator-led; survival should reduce successful attacks, avoidance should reduce contact or successful hunts, and reproduction should increase births before any claimed route benefit.

## Frozen inputs

| Input | Value |
| --- | --- |
| Source | `6b6972dbe0216ff27ab968cad9be105b2cedc830`, `feature/s6-fun-pass`, plus an uncommitted portable journey replay fix: it now passes the snapshot's experimental options to the shared runner. The only source edits are `CellSimPortable.cs` and `CellSim.Batch/Program.cs`; no simulation rules or assets were changed. Freeze the source hash from the new export in the report. |
| Scenario | `Assets/Data/ProductionData/CellularSimulation/Scenarios/ForestEdge.asset`, SHA-256 `0ece03f19616942f052b16f07fe171d9bd73663cc1deea26604c75438024f69e` |
| Route | `Assets/Data/TestData/FieldDataStarterDefinition.asset`, SHA-256 `f56ee2855aff01907bd37e36d1dffc43f15947dcdfa91fdcd41a56030b8c1d1f` |
| Ecology | 36×20 nonwrapping map, 400 Plants, 55 Hares, 20 Foxes, 0.2 seconds/tick, opposed-roll combat, `bev-experimental`, default Fox movement scent off; neutral active Genome |
| Seeds | 10100–10111 inclusive, identical across arms |
| Window | Four continuous simulation nodes, six 100-tick rounds each; node boundaries at ticks 600, 1200, 1800, 2400 |
| Baseline route | `journey-replay`, empty action schedule; all reward choices auto-skipped, no shop purchases or population controls |
| Mechanics comparison | Portable continuous 24-phase observation with 100 ticks/phase, starting from the same snapshot and seeds. Control has `none` in every phase. Candidate arms hold one explicit Mutation from phase 2 onward, with no later choices: `threat-exposure` (avoidance), `tough-hide` (survival), `larger-minimum-litter` (reproduction). |

The Mutation arms use a deliberate mechanics proxy. `journey-replay` cannot force a Mutation that is absent from an offer, and its current report lacks per-tick mortality and activity telemetry. The proxy bypasses the player research and offer gate. Before comparing route outcomes, compare the no-action proxy and journey replay on the same seeds and boundary ticks. If their board/population states disagree materially, label all Mutation results as mechanics-only and do not use them as route win-rate evidence. If the route ends earlier on extinction, compare only shared ticks.

An initial read-only replay using the exported snapshot but the pre-fix portable worker failed seed 10100 at tick 236. This was a comparability failure: the worker silently omitted the snapshot's `bev-experimental` ruleset, whereas the authored Unity route has it enabled. Keep that raw output as rejected diagnostic evidence. A new snapshot and worker built from the fixed source are required for all outcomes below; do not combine their results with the pre-fix run.

## Outcomes and report contract

- **Opening safety:** Hare survival to tick 600; record early failure seeds and cause.
- **Second-node target diagnostic:** Hare survival at tick 1200, or extinction by then. Twelve seeds give a rough screen only, not a calibrated 60% estimate.
- **Predator-first mechanism:** Hare combat versus starvation deaths by round and node; Fox kills, attacks, Plant stock and consumption; the first sustained Plant shortage relative to the Hare decline.
- **Mutation signal:** Compare matched-seed post-tick-100 combat losses, Fox hunt/contact measures, Hare births and reproduction attempts, minimum Hare population, Plant stock/consumption, and Hare survival at ticks 600/1200/1800/2400. Confirm that the scheduled Mutation was applied and its relevant events had opportunities to trigger.
- **Falsification:** Predator-first is not supported by this configuration if food exhaustion typically precedes decisive Hare losses, or if combat is a minority of early Hare deaths. A candidate is not a viable introductory answer if its local mechanism rarely acts, fails to improve its intended local metric, or improves that metric only by accelerating food-led collapse on these seeds.
- **Status/provenance:** Retain exact commands, snapshot and worker hashes, scenario/definition fingerprints, per-seed raw outputs, completion statuses, and a factual report separate from AI analysis. State any route/proxy mismatch and estimate variation without turning this diagnostic into a definitive balance claim.

No production balance value or upgrade catalog is changed in this experiment. A larger disjoint held-out panel and any scenario tuning require a new frozen contract after these diagnostics.
