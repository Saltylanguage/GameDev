# Quarter-million sweep: verified completion and first findings

All 250,000 runs and automatic analysis completed October 8, 2026 at 14:09:02 UTC / 10:09 EDT.
Launch to completed analysis: 6.94 hours (6h 56m). Batch execution/aggregation: 6.68 hours; remaining pipeline time approximately 15.42 minutes. Measured fresh-batch throughput: 10.39 runs/second with 16 workers for this panel. Raw data: 10,883,166,732 bytes. Peak individual worker: 99.2 MiB.

Verified frozen input and assembly identities, complete raw hash, analysis CSV/shortlist hashes, SQLite integrity, all 5,000 chunks, 2,500 conditions x exactly 100 unique seeds (40000-40099), and valid binary overall outcome statuses. No batch/worker process remains and the pipeline error log is empty.

Raw SHA-256: aed27d0b76deaa39c61e944e478407003786b16322ba5187c96bb909934e5edf

## New strongest screened context

325 Plants / 30 Hares / 15 Foxes; 54x32; Fox starting energy 80; Hare vision 9. Same 100 matched seeds per path.

| Hare path | All three species alive at tick 600 | Hare species alive at tick 600 |
|---|---:|---:|
| skip-all | 16% | 32% |
| trailblazer | 15% | 36% |
| warren | 14% | 39% |
| gardeners | 68% | 72% |

Gardeners vs skip-all: +52 percentage points; 56 matched wins, 4 losses, 40 ties. Exploratory paired normal 95% CI +40.7 to +63.3 points. Selected from 2,500 arms; inference is unadjusted. All ten highest-ranked arms use Gardeners.

## Population / map results grid

Percent of runs retaining all three species at tick 600. Each cell pools 10,000 runs across 25 Fox-energy/Hare-vision combinations and four paths. These describe the complete mixture, not a selected best arm, and repeated shared seeds are correlated.

| Plants / Hares / Foxes | 30x18 | 36x20 | 42x24 | 48x28 | 54x32 |
|---|---:|---:|---:|---:|---:|
| 100-10-5 | 3.09% | 1.30% | 0.27% | 0.02% | 0.00% |
| 175-15-8 | 11.02% | 11.25% | 7.40% | 2.90% | 0.62% |
| 250-20-10 | 9.15% | 13.10% | 13.12% | 9.45% | 4.91% |
| 325-30-15 | 3.10% | 6.43% | 9.69% | 15.04% | 15.38% |
| 400-40-20 | 2.69% | 3.59% | 6.59% | 11.27% | 16.14% |

The population/map relationship has a clear shape: larger maps improve the denser bundles; the low population bundle loses all-species survival as space increases. The 250/20/10 bundle has its strongest pooled results at the middle maps. This is descriptive evidence; population is a compound bundle and the mechanism remains unproven.

## Best observed arm per grid cell

Each percentage is the maximum among that cell's 100 energy/vision/path arms, each measured on 100 seeds. Selection makes these optimistic screening values; they require confirmation.

| Plants / Hares / Foxes | 30x18 | 36x20 | 42x24 | 48x28 | 54x32 |
|---|---:|---:|---:|---:|---:|
| 100-10-5 | 27% | 23% | 9% | 1% | 0% |
| 175-15-8 | 36% | 45% | 55% | 47% | 21% |
| 250-20-10 | 36% | 48% | 54% | 56% | 66% |
| 325-30-15 | 14% | 22% | 44% | 61% | 68% |
| 400-40-20 | 18% | 19% | 27% | 37% | 65% |

## Replication of the previous winning context

250 Plants / 20 Hares / 10 Foxes; 42x24; Fox starting energy 80; Hare vision 9. Source, inputs and horizon are unchanged. Old/new seed batches are disjoint; cross-batch rates are independent-seed replication observations, not paired old/new deltas.

| Hare path | Previous 100 seeds: all species | Fresh 100 seeds: all species |
|---|---:|---:|---:|
| skip-all | 19% | 18% |
| trailblazer | 20% | 18% |
| warren | 17% | 17% |
| gardeners | 60% | 54% |

The Gardeners advantage reproduced: 54% vs 18% control on fresh seeds (+36 points; paired exploratory normal 95% CI +24.0 to +48.0). The previous batch was 60% vs 19% (+41 points).

A narrower earlier hypothesis did not clearly reproduce: at the previous winning context, vision 9 vs 5 changes fresh-seed Gardeners survival by only +3 points (54% vs 51%; 95% CI -9.9 to +15.9; exact McNemar p=0.761). The previous screen was +22 points (60% vs 38%). Treat this stat effect as context-dependent/unconfirmed, rather than generalizing the selected vision-9 arm.

## Pooled path outcomes

Each path covers the same 625 contexts / 62,500 runs. Descriptive mixtures only: these observations share seeds and contexts.

| Hare path | All species at tick 600 | Hare species at tick 600 |
|---|---:|---:|
| skip-all | 3.47% | 10.73% |
| trailblazer | 4.80% | 13.66% |
| warren | 4.61% | 13.34% |
| gardeners | 15.52% | 50.55% |

## Limits and next review

- Every arm still has 100 seeds; 250,000 total runs increase grid coverage rather than making every arm a 250,000-seed estimate.
- This remains a 600-tick survival screen. Tick-700 ecology, recovery and human gameplay/event pace are untested.
- New winners are selected among 2,500 arms and remain exploratory. CIs and p values are unadjusted; shared-seed pooled results are correlated.
- The largest map (54x32) is the tested boundary. A winner there does not locate a global optimum or establish the desired gameplay map size.
- Gardeners' repeated survival advantage warrants examining acquired choices, phase-specific failure causes, food/reproduction dynamics and tradeoffs before production decisions.
- Whole-run death and event counts rise with exposure and reproduction; use phase/window-aware denominators before attributing effects.

Evidence: completion-review.json; analysis/analysis.md/json; analysis/metrics.csv; analysis/paired-deltas.csv; analysis/metrics.sqlite; sweep/batch/runs.jsonl and summary.json. No production tuning or additional simulations were launched during this completion check.
