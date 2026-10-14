# Purchase screen score diagnostics

Gardeners only: D5/S25 and C2/S14; four policies; 100 assigned matched seeds per arm. Phase 0 uses all seeds and each run's actual observed duration. Phase 1-6 deltas include only matching observed start/end ticks; they are conditional diagnostics.

Terms sum to APS/AHS within 1e-6 for every included whole-run and phase observation. Purchased ADD is excluded from RFS by the production score formula; N/A terms contribute zero, matching production. Interaction means predAVG for APS and huntAVG for AHS. These decompositions explain the arithmetic, not a causal mechanism or enjoyment.

| Candidate | Policy | ΔAPS | ΔRFS | ΔpredAVG | Δstarvation term | Δcrowding term | ΔAHS | Different whole-run windows |
|---|---|---:|---:|---:|---:|---:|---:|---:|
| D5-S25 | late-five | -0.0005 | -0.0146 | -0.0006 | +0.0148 | +0.0000 | -0.0279 | 33 |
| D5-S25 | restore-toward-start | -0.0212 | -0.0339 | +0.0002 | +0.0126 | +0.0000 | -0.0439 | 67 |
| D5-S25 | each-five | -0.1050 | -0.1280 | +0.0001 | +0.0230 | +0.0000 | -0.0566 | 70 |
| C2-S14 | late-five | -0.0109 | -0.0200 | +0.0003 | +0.0089 | +0.0000 | -0.0063 | 15 |
| C2-S14 | restore-toward-start | +0.0839 | +0.0539 | -0.0011 | +0.0311 | +0.0000 | -0.0692 | 61 |
| C2-S14 | each-five | -0.0768 | -0.0724 | -0.0070 | +0.0027 | +0.0000 | +0.0286 | 74 |

Full component means, ecological counters and phase sample sizes are in means.csv and paired-components.csv. No purchased-entity lineage was tracked, so post-purchase population changes cannot identify which individual purchased Hares survived.

Source: F:\ForkBin\GameDev\LearningIndieDev\artifacts\cellsim-hare-purchase-20261008-132317. Source raw hash was verified when preparing the confirmation; these are earlier screen observations, not confirmation results.
