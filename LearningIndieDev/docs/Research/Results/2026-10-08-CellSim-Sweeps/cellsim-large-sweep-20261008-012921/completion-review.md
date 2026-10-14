# Verified completion and first results

All 32,400 runs, all 1,296 chunks and automatic analysis completed. Pipeline finished October 8, 2026 at 06:12:13 UTC (02:12 EDT).
First launch to completed analysis: 39.86 minutes. Raw JSONL: 1,245,257,609 bytes. The resumed runner's 22.70 runs/second summary includes reused chunks and is not a measured 32-worker throughput benchmark.

Independent checks: frozen batch/input identity, raw/output hashes, SQLite integrity, 324 conditions x exactly 100 unique seeds (30000-30099), and valid binary overall outcome statuses. Primary raw and derived files remain preserved.

## Strongest screened context

250 Plants / 20 Hares / 10 Foxes; 42x24 map; Fox starting energy 80; Hare vision 9. Same 100 seeds per path.

| Path | All species alive at tick 600 | Hare alive at tick 600 |
|---|---:|---:|
| skip-all | 19/100 | 47/100 |
| trailblazer | 20/100 | 43/100 |
| warren | 17/100 | 43/100 |
| gardeners | 60/100 | 70/100 |

Gardeners vs skip-all: +41 percentage points; paired normal 95% CI +28.8 to +53.2 points. 48 matched seeds improved, 7 worsened, 45 tied. This selected result requires fresh-seed confirmation; it was identified among 324 arms, with unadjusted inference.

## Population and map grid

Each cell pools 3,600 runs across all nine Fox-energy/Hare-vision combinations and four paths. Percentages are descriptive mixtures, not the best available path or independent-trial confidence estimates.

| Plants / Hares / Foxes | 30x18 | 36x20 | 42x24 |
|---|---:|---:|---:|
| 100-10-5 | 4.36% | 2.17% | 0.28% |
| 250-20-10 | 6.44% | 8.89% | 15.42% |
| 400-40-20 | 1.83% | 1.08% | 3.00% |

The middle population bundle produced nine of the ten highest-ranked arms. Map impact depends on the population bundle: more space did not uniformly improve survival.

## Pooled path results

Each path covers the same 81 contexts and 8,100 runs. These are descriptive pooled outcomes; shared seeds and contexts correlate observations.

| Path | All species alive at tick 600 | Hare alive at tick 600 |
|---|---:|---:|---:|
| skip-all | 2.37% | 11.23% |
| trailblazer | 3.85% | 14.28% |
| warren | 3.86% | 13.75% |
| gardeners | 9.23% | 37.65% |

## Evidence limits and next review

- The screen planned 600 ticks. Tick-700 ecology viability, recovery and human gameplay acceptance remain untested.
- All-species survival does not define biome stability or a satisfying personal event pace.
- Actual eligible upgrades/choices are recorded; policies do not guarantee identical acquired nodes across seeds.
- Whole-run counts can rise simply because a path survives longer. Use the planned-phase metrics, denominators and different-window counts before attributing a mechanism.
- Confirm several contrasting contexts on fresh seeds and review extended survival, personal event pace, death causes and chosen nodes before production tuning.

Outputs: analysis/analysis.md and analysis.json; analysis/metrics.csv; analysis/paired-deltas.csv; analysis/metrics.sqlite; analysis/shortlist-contexts.json. completion-review.json preserves this compact verification packet. Raw source: sweep/batch/runs.jsonl and summary.json.
