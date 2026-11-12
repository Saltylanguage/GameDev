# Superseded: S6 starting-population curve draft

**Status:** Superseded before execution after Josh clarified the question as
“how many Hares is a Fox worth?” No runs were started. The 1,500-run, two-node
failure-rate matrix below is not approved and must not be executed. The new
single-node carcass-value question is drafted at
[S6 Fox carcass-value diagnostic](../S6-Fox-Carcass-Value-20261111/CONTRACT-DRAFT.md).

**Experiment ID:** `S6-POPULATION-CURVE-20261111`  
**Decision owner:** Josh  
**Status:** Draft; awaiting Josh's approval before any runs  
**Feature:** Introductory Hare journey under Fox pressure  
**Parent evidence:** [S6 Mutation tuning](../S6-Mutation-Tuning-20261111/CONTRACT.md), [corrected Fox node-reset sweep](../S6-Fox-Node-Reset-20261110/REPORT.md), [lower-Fox diagnostic](../S6-Fox-Soft-Opening-20261110/REPORT.md)

## Question

At fixed Forest Edge map size, how do the starting Hare count and Fox-to-Hare ratio affect Hare extinction by Node 1 and Node 2? At a fixed ratio, does increasing the absolute starting population produce a roughly linear change in failure probability, or a more curved, multiplicative response? Which tested starting conditions keep Node 1 safe while making Node 2 a challenging but fair test for a no-upgrade run?

## Hypotheses

1. Raising the Node 1 starting Fox-to-Hare ratio will raise Hare extinction risk, particularly by the end of Node 2.
2. At the same Fox-to-Hare ratio, changing absolute population scale on a fixed-size map will change local density and therefore may change outcomes beyond the change predicted by ratio alone.
3. Any selected balance point will be a tested range of conditions for Forest Edge, not a universal Fox:Hare ratio for every species, biome, or map.

These are hypotheses, not accepted balance decisions.

## Proposed scope and fixed inputs

| Input | Proposed value |
| --- | --- |
| Scenario and map | Authored Forest Edge, 36×20, nonwrapping |
| Player species | Hare (`hare`) |
| Simulation | Two fresh nodes; six phases of 100 ticks per node; observe through tick 1200 |
| Node transition | Carry only the Node 1 closing Hare count; recreate Hares with default individual state; reset Foxes and Plants to that arm's authored Node 2 counts |
| Rules | Opposed-roll combat; `bev-experimental`; natural attack opportunities; Fox movement scent off |
| Genome and actions | Neutral Genome; no Mutations, purchases, population actions, or research |
| Resources | Scale Plant count with initial Hare count to keep the starting Plant:Hare ratio near the current 500:55 reference |
| Seed panel | Matched seeds 30300–30399 for all 15 conditions |

Only the starting Hare count and nominal Node 1 Fox:Hare ratio vary. For every condition, Node 2 Foxes reset to four times that condition's Node 1 Fox count, preserving the current 12→48 Fox pressure step. Plants reset to the same count as Node 1. Only Hare population count carries to Node 2.

The no-upgrade arm isolates the population curve and represents the user's low-level/no-effective-upgrade difficulty target. Actual suboptimal Mutation choices are a later comparison; they are not mixed into this count-only experiment.

Plant count is rounded to the nearest whole number, with half values rounded up, using `500 × starting Hares / 55`. Node 1 Fox count is rounded the same way using `starting Hares × nominal ratio`. Node 2 Fox count is exactly four times the resulting Node 1 Fox count. This keeps the existing 55 Hare / 12 Fox / 500 Plant setup as the center reference (actual ratio 12/55 = 0.218).

## Proposed population matrix

The fixed-map factorial grid uses five Hare population scales and three Fox:Hare ratios. `F1` and `F2` are the static Fox counts at Node 1 and Node 2; `P` is the Plant reset count in both nodes.

| Starting Hares | Plants | Nominal Fox:Hare ratios to test | Node 1 Fox counts (`F1`) | Node 2 Fox counts (`F2 = 4 × F1`) |
| ---: | ---: | --- | --- | --- |
| 30 | 273 | 0.14, 0.22, 0.30 | 4, 7, 9 | 16, 28, 36 |
| 36 | 327 | 0.14, 0.22, 0.30 | 5, 8, 11 | 20, 32, 44 |
| 44 | 400 | 0.14, 0.22, 0.30 | 6, 10, 13 | 24, 40, 52 |
| 55 | 500 | 0.14, 0.22, 0.30 | 8, 12, 17 | 32, 48, 68 |
| 60 | 545 | 0.14, 0.22, 0.30 | 8, 13, 18 | 32, 52, 72 |

The matrix contains 15 conditions and 1,500 runs. The baseline reference condition is 55 Hares, 12 Node 1 Foxes, 48 Node 2 Foxes, and 500 Plants. Shared seeds support paired comparisons, but changing starting counts also changes shuffled cell placement; paired effects therefore compare complete starting setups and do not isolate count from exact initial coordinates.

The dry-run plan is prepared at `artifacts/S6-Population-Curve-20261111/population-ratio-scale-100-plan.json`. Its CellSim batch identity is `20ce44cda46e91c61aad7ef72dd498b02ca7bf8ba64baa8aae6a67e0730eecba`; the plan SHA-256 is `7e17b4c769a724258474c36a0c9aae21dedc32cc9ada16b4e13c12b5a5a1d069`. Dry-run validated 1,500 runs across 150 chunks. The snapshot source hash is `cbe74a675ed62726d4a1a758ee22fdd2b3ae58d22068df6e1cfbfce92c1d17b7` and its Forest Edge data fingerprint is `b7cac6867848cf29338350381e5d03122339bc8304ff7d399091d3c6aa17f8b3`. Recheck hashes and dry-run after approval before execution.

## Measures and analysis

Primary outcomes:

- Hare failure by Node 1 close and cumulative Hare failure by Node 2 close, reported as seed counts, rates, and 95% Wilson intervals.
- Node 2 failure among seeds that survived Node 1.
- Hare counts at both node closes and the actual Fox:Hare ratio at each node opening.

Mechanism and guardrail measures:

- Fox attacks and kills; Hare births and cause-specific deaths, including starvation.
- Minimum Plant population at recorded phase checkpoints and any zero-Plant checkpoint.
- Every node opening, Hare carry, Fox/Plant reset, reset ledger, loadout, seed, source hash, snapshot fingerprint, and batch identity.

Report empirical rates over both nominal ratio and absolute population scale. Compare a linear probability fit with a logistic fit (linear change in log-odds, or exponential change in failure odds), including ratio-by-scale interaction. Use a paired-seed bootstrap for uncertainty. Call one curve form better only when the evidence clearly separates the fits; otherwise report the shape as unresolved. Do not extrapolate beyond the tested range.

The proposed provisional “good balance” screen for a no-upgrade condition is at most 5% Node 1 failure and 45–55% cumulative Node 2 failure. A condition where Plants reach zero at recorded checkpoints or starvation exceeds Fox-caused Hare deaths is flagged as resource-dominated rather than treated as a clean Fox:Hare balance point. The range is for seed outcomes under this fixed scenario, not a claim about individual player odds.

## Report contract

Expected artifacts after the approved runs:

- `REPORT.md`: factual setup, per-condition outcomes, confidence intervals, ecology guardrails, and provenance.
- `AI_ANALYSIS.md`: separate interpretation, model comparison, uncertainty, and next-step recommendation.
- `paired-summary.json` and `summarize.py`: seed-level outcomes, population-reset assertions, paired differences, fitted curve outputs, and source/run hashes.
- Raw `runs.jsonl` and batch manifests in the ignored experiment artifacts directory.

No authored balance asset or runtime value changes in this experiment. No manual review starts as part of the run. The existing portable CellSim snapshot and worker must pass a fresh source-hash preflight before execution; record the exact commit/source hash and assembly hash with the run. Report any Unity export or Editor verification limitation.

## Map-size follow-up

After this population matrix identifies a useful ratio/scale region, create a separate approved experiment for map size. That follow-up should compare fixed absolute populations with populations scaled by map area. The pair distinguishes a map expansion that dilutes density from a larger map that preserves density. Freeze the chosen count ratio, dimensions, resource-scaling rule, seeds, and success criteria in a new contract before running that phase.

## Approval record

**Approval record:** Superseded before execution; no approval is requested for
this draft. The generated plan and snapshot are stale preparation and must not
be executed or reused for the carcass-value question.
