# S6 starting Fox count with fresh node populations — experiment contract

- **Experiment ID:** `S6-FOX-NODE-RESET-20261110`
- **Human decision owner and scope approval:** Josh approved the 16/18/20 starting-Fox sweep and node-two pressure check, then clarified that only the played species' population count carries between nodes and said to continue. This revision applies that clarification to the approved comparison; it does not authorize a balance change.
- **Status:** Approved for execution; results and Josh's balance decision are separate.
- **Question:** With fresh Foxes and Plants at node two, which of 16, 18, or 20 starting Foxes gives an unlikely first-node Hare failure and approximately 60% cumulative no-action seed failure by the end of node two?
- **Hypothesis:** Restoring the fixed Fox count at node two raises predator pressure among surviving Hare seeds relative to the previous continuous-world sweep.

## Frozen inputs and arms

| Input | Value |
| --- | --- |
| Source | `codex/s6-predator-first`, corrected node-boundary implementation; record commit, worker assembly hash, source hash, snapshot hash, and runtime in the run report |
| Scenario | Authored Forest Edge, 36×20 nonwrapping, opposed-roll combat, `bev-experimental`, neutral Genome |
| Starting populations | 400 Plants, 55 Hares, and 16, 18, or 20 Foxes; fixed nonplayed counts on every node |
| Node transition | At tick 600, carry only the Hare count; recreate all Hares with default individual state; reset Foxes and Plants to their fixed starting counts; preserve active progression and upgrades |
| Journey proxy | Twelve 100-tick phases, six per node, no Mutations, shop purchases, population actions, or player research; observe all 1200 ticks even after Hare extinction |
| Diagnostic | Matched seeds 10100–10111, three Fox counts; mechanism and parity screen only |
| Confirmation | Disjoint matched seeds 10200–10299, three Fox counts, regardless of diagnostic outcome |

## Validation and report contract

- Export a fresh Unity snapshot and verify two matched portable seeds against its Unity reference before the sweep. The corrected source hash must match the worker.
- For **every seed**, verify node-one opening is exactly 400 Plants, 55 Hares, and the declared Fox count. Verify node-two opening has the same fixed Plant and Fox counts and the Hare count from node-one close. Reject any incomplete or mismatched run.
- Report Hare extinction by node-one close and by node-two close, and conditional node-two extinction among node-one survivors. The first-node screen is at most 5/100 failures; the approximate 60% cumulative target is screened at 50–70/100. These are decision aids, not approved balance values.
- Report per-node Hare combat and starvation deaths, births, Fox kills and attacks, Plant populations, and the population distribution at each node boundary. Compare 16/18/20 using matched seed outcomes and uncertainty intervals.
- Keep raw runs and manifests immutable under `artifacts/s6-fox-node-reset-20261110/`. Version a factual report, compact per-seed summary, and separate AI analysis here. Do not rewrite the earlier continuous-world sweep or edit authored balance values in this experiment.
