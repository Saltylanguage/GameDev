# S6 lower Fox-count opening screen — experiment contract

- **Experiment ID:** `S6-FOX-SOFT-OPENING-20261110`
- **Decision owner and approval:** Josh approved trying a gentler Fox opening after reviewing the corrected fresh-node sweep. This screen tests lower fixed Fox counts; it does not approve a balance change.
- **Question:** Can a fixed starting Fox count below 16 reduce no-action Hare extinction by the end of node one toward the provisional 5% screen, while retaining roughly 50–70% cumulative Hare extinction by node two?
- **Hypothesis:** Lower Fox counts reduce first-node Hare losses. They may also make node two too safe, since the same fixed Fox count is restored at each fresh node.
- **Scope:** Forest Edge, Hare player, no-action/no-upgrade journey proxy, corrected fresh-node transition. Only the played Hare count carries between nodes; all Hares are recreated with default individual state, and Foxes and Plants reset to each arm's fixed starting counts. No authored balance asset is changed.
- **Changed variable:** Fox starting count, held fixed across both nodes within each arm: 8, 10, or 12.
- **Held inputs:** 400 Plants, 55 initial Hares, 36×20 nonwrapping grid, opposed-roll combat, `bev-experimental`, neutral Genome, 12 phases of 100 ticks (6 per node), no Mutations, purchases, population actions, or research. Observe to tick 1200 even after Hare extinction.
- **Diagnostic panel:** Matched seeds 10100–10111. Screen only; no final rate claim from 12 seeds.
- **Confirmation rule:** Confirm any diagnostic arm with at most 1/12 first-node failures and 4–10/12 cumulative node-two failures on a disjoint 100-seed panel, 10300–10399. If no arm qualifies, report the diagnostic and propose the next lower range instead of selecting a balance value.
- **Success screen:** Confirmation first-node failure at most 5/100 and cumulative node-two failure between 50–70/100. Report Wilson 95% intervals and conditional node-two failure among node-one survivors.
- **Failure screen:** Every confirmed arm misses either target; any incomplete run or mismatched node opening invalidates that arm pending correction.
- **Required telemetry:** Per-node opening and closing counts, Hare extinction by node, births and cause-specific Hare deaths, Fox attacks/kills, Plant counts, reset ledger, run status, and source/input/runner fingerprints.
- **Provenance:** Use the frozen Unity-exported v3 Forest Edge snapshot from the corrected sweep. Its shared source hash identifies the simulation source; verify the current worker against it before running. Record the snapshot and worker hashes, current Git revision, seed panels, plan identities, and raw output hashes in the factual report.
- **Interpretation limit:** The grid generator consumes shared shuffled placement cells in species order, so changing Fox count may also shift Hare/Plant starting positions. Matched-seed differences are comparisons of complete starting setups, not isolated Fox-count effects.
