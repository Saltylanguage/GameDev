# S6 starting Fox count and node-two pressure — factual report

- **Experiment:** `S6-FOX-START-20261110`
- **Report:** `S6-FOX-START-R1`
- **Feature:** Forest Edge introductory Hare journey
- **Task:** Test 16, 18, and 20 starting Foxes through the first two nodes
- **Status:** Complete; awaiting Josh's balance decision
- **Generated:** November 10, 2026
- **Contract:** [CONTRACT.md](CONTRACT.md)
- **Per-seed machine-readable results:** [paired-summary.json](paired-summary.json)

## Provenance and completion

The source was clean `feature/s6-fun-pass` at `b94929aa1f5cf2a16fc258c1ba77fc00d314f55b`. The frozen Forest Edge snapshot was `artifacts/s6-pf-diag-20261109/forest-edge-v2.json` (SHA-256 `429ea327fd8a6275cec833da710e04696bc64f01e94a6062b25fd38098daf455`, scenario fingerprint `e89a6ed32cdf87a9314dd86349ce8cca8607bb266908e9553f8dd88ad5d0d616`, shared-source hash `6fcc6913b758717217150094b7bdd9c8afca85cb96eb96484ab31669f3db3990`). The snapshot's `sourceCommit` field records its earlier pre-commit export base `6b6972d`; its shared-source hash matches the worker built from the now-published source. No authored scenario or simulation rule was edited for this experiment.

Both batch plans used the same frozen snapshot and 12 `none` phases of 100 ticks, with only `-startingPopulations` changing: `plant=400,hare=55,fox=16|18|20`. The map was 36×20 and nonwrapping; combat was opposed-roll with `bev-experimental`, ordinary Fox movement scent off, and neutral active Genome. The diagnostic panel used matched seeds 10100–10111; the disjoint confirmation used 10200–10299. The runner observed the full 1200 ticks even after Hare extinction. It did not simulate shop purchases, Mutations, or profile research.

| Run | Local path under `LearningIndieDev/artifacts/s6-fox-start-sweep-20261110/` | Result and identity |
| --- | --- | --- |
| Diagnostic | `diagnostic-plan.json`, `diagnostic/batch.json`, `diagnostic/runs.jsonl`, `diagnostic/summary.json` | 36/36 validated; identity `c75a16bdf93ddaf2c6fa5855b3be85d3c937cf798f76be4760433bc1cd27cf8f`; raw SHA-256 `34a5ff745d6e4f23931bc3af0da901698ff3f71b742b36fd42eab15a6c337040` |
| Confirmation | `confirmation-plan.json`, `confirmation/batch.json`, `confirmation/runs.jsonl`, `confirmation/summary.json` | 300/300 validated; identity `1f4bf18e5b4b8c98eca85ab38346f6f1c8b6e1a09069ab10868f2b6682e0e2a9`; raw SHA-256 `f03dfa3af4e7fe64712d2e64f5d8cd5c3b00bc2254bc2bf2ef7c7a4a7e334012` |

The plan SHA-256 values are `52d7c383d60a2d861702a8fac53aff5a83edd7a08b56b24f0d2918742cd0131c` and `660ff5f13e1b40950d8ffc605af709f1457bff43424515ff2b4d1e91d96dc80b` respectively. The worker assembly SHA-256 was `77a89a2fe155f7b17c5f633b3c8d604dfe952986007b18aa51d98ce1cfb85da7` in both batches. Commands were `CellSim.Batch batch <plan> --dry-run` followed by `CellSim.Batch batch <plan>` with `DOTNET_ROLL_FORWARD=Major` and the project-required elevated host permissions.

All 336 runs started with their exact declared Plant/Hare/Fox counts, had no effective Mutation loadout, and completed all 12 phase windows. Fox kills equaled Hare combat deaths in all 4,032 recorded phase windows. The 20-Fox diagnostic arm matched the prior 24-phase no-action trace on all 12 seeds through tick 1200: Hare extinction tick, every 100-tick population, phase activity, and species stat lines. The previous trace had already matched the authored journey's decision-boundary populations.

## Diagnostic panel, 12 matched seeds

| Starting Foxes | Hare extinct by 600 | Additional Hare extinctions in node 2 | Hare extinct by 1200 |
| ---: | ---: | ---: | ---: |
| 16 | 1/12 | 2/11 first-node survivors | 3/12 |
| 18 | 1/12 | 1/11 first-node survivors | 2/12 |
| 20 | 6/12 | 1/6 first-node survivors | 7/12 |

This panel includes previously inspected seeds. It was used for input and route-proxy validation, not for selecting a balance value. It differed substantially from the disjoint panel below, especially at 20 Foxes.

## Disjoint confirmation, 100 matched seeds per count

| Starting Foxes | Hare extinct by 600 (illustrative Wilson 95% interval) | Additional losses in node 2 | Hare extinct by 1200 (interval) | Plants at zero at 1200 | Foxes present at 600 |
| ---: | ---: | ---: | ---: | ---: | ---: |
| 16 | 19/100 (12.5–27.8%) | 6/81 first-node survivors | 25/100 (17.5–34.3%) | 63/100 | 41/100 |
| 18 | 15/100 (9.3–23.3%) | 11/85 first-node survivors | 26/100 (18.4–35.4%) | 62/100 | 36/100 |
| 20 | 20/100 (13.3–28.9%) | 11/80 first-node survivors | 31/100 (22.8–40.6%) | 54/100 | 20/100 |

All three counts were accepted by the runner and placed exactly. None met the contract's first-node screen of at most 5/100 failures or its 50–70/100 cumulative node-two band. The intervals are binomial approximations for a hypothetical independent seed sample; these fixed consecutive seeds do not establish a population-wide probability.

Matched against 20 Foxes, 16 Foxes prevented 13 first-node failures but created 12 failures on other seeds; 18 Foxes prevented 17 and created 12. By tick 1200, the corresponding rescued/new-failure pairs were 20/14 and 23/18. The raw per-seed outcomes and population checkpoints are in the linked summary. The first-node failure count was not monotonic across 16, 18, and 20 in the confirmation panel.

## Pressure and resource observations on confirmation seeds

| Starting Foxes | Hare combat / starvation deaths, ticks 1–600 | Fox kills in ticks 1–100 / 1–600 | Hare births, ticks 1–600 | Fox kills / Hare starvation deaths, ticks 601–1200 | Median Plants at 600 / 1200 |
| ---: | ---: | ---: | ---: | ---: | ---: |
| 16 | 6,885 / 1,283 | 2,568 / 6,885 | 4,379 | 461 / 25,144 | 689 / 0 |
| 18 | 6,246 / 999 | 2,960 / 6,246 | 3,795 | 320 / 25,428 | 688 / 0 |
| 20 | 5,777 / 878 | 3,248 / 5,777 | 3,023 | 91 / 22,878 | 701.5 / 0 |

No confirmation seed reached zero Plants before tick 600. By tick 1200, Plants were at zero in 63, 62, and 54 seeds at 16, 18, and 20 Foxes. For node-two Hare failures, Plants had first reached zero before Hare extinction in 6/6, 10/11, and 11/11 seeds respectively. These temporal observations do not identify the final Hare death cause. The Fox population was already zero at tick 600 in 59, 64, and 80 seeds respectively. The median Fox population at tick 600 was zero for all three counts.

## Limits and decision state

This is a deterministic matched-seed ecology comparison of an input override, not a fresh-profile Unity playthrough of an authored 16- or 18-Fox route. Its 1200-tick window cannot establish later journey success or player fun. First-zero timing describes when Plants or Foxes first reached zero; the per-node death totals and checkpoints are reported separately. The two seed panels are disjoint but consecutive, not randomly sampled. Raw artifacts remain local and ignored by Git; the compact summary is versioned.

**Human decision:** Pending. Josh can use these results to decide the next change; no starting population or other balance setting was changed.
