# S4 Hare paired-seed screen - 600/40/25 with wrapping

## At a glance

The 13-arm screen completed all **1,300 scheduled runs** on 100 common fresh seeds (13000-13099) with **zero execution failures**. It produced 7,219 observed phase windows; later phases are absent when a run stopped after Hare extinction. The skip-all control had 56/100 positive Hare populations at tick 600. Seed Dispersal and both Gardeners orders had the highest number of runs with all three species present at tick 600 (43/100 each). These are observations from one population and wrapping configuration, not balance approval.

Run date: 2026-10-05 (local); completion: 2026-10-06 03:39 UTC. Unity 6000.4.6f1, connected Editor, four local workers. Source branch was dirty at baseline commit `6a0ab08e155b1a8a2962d19a6c5eca1bc8233118`; exact modified C# file hashes are in the local experiment contract.

## Outcomes

Survival means Hares > 0 at tick 600. `All three` means Plants, Hares and Foxes were all present then. The 95% intervals are Wilson intervals for each arm's observed survival rate. Paired W/T/L means the candidate alone survived / both had the same survival result / only the control survived.

| Arm | Hare survival /100 (95% Wilson CI) | All three /100 | Paired survival W/T/L vs control |
|---|---:|---:|---:|
| Skip-all control | 56 (46.2-65.3%) | 19 | 0 / 100 / 0 |
| General Movement | 49 (39.4-58.7%) | 21 | 15 / 63 / 22 |
| Threat Avoidance | 51 (41.3-60.6%) | 18 | 14 / 67 / 19 |
| Tough Hide | 55 (45.2-64.4%) | 25 | 14 / 71 / 15 |
| Crowding Tolerance | 56 (46.2-65.3%) | 21 | 5 / 90 / 5 |
| Efficient Digestion | 54 (44.3-63.4%) | 16 | 7 / 84 / 9 |
| Seed Dispersal | 61 (51.2-70.0%) | 43 | 18 / 69 / 13 |
| Trailblazer: Movement then Avoidance | 42 (32.8-51.8%) | 21 | 14 / 58 / 28 |
| Trailblazer: Avoidance then Movement | 46 (36.6-55.7%) | 13 | 8 / 74 / 18 |
| Warren: Hide then Crowding | 55 (45.2-64.4%) | 23 | 14 / 71 / 15 |
| Warren: Crowding then Hide | 60 (50.2-69.1%) | 18 | 8 / 88 / 4 |
| Gardeners: Digestion then Dispersal | 61 (51.2-70.0%) | 43 | 15 / 75 / 10 |
| Gardeners: Dispersal then Digestion | 60 (50.2-69.1%) | 43 | 18 / 68 / 14 |

Final Hare populations were strongly skewed. Median / mean FPO was 1 / 38.69 for control; 0 / 15.79 for Movement; 1 / 23.19 for Avoidance; 1 / 25.89 for Hide; 1 / 32.85 for Crowding; 1 / 35.49 for Digestion; 3.5 / 54.62 for Dispersal; 0 / 13.78 and 0 / 17.97 for Trailblazer orders; 1 / 24.92 and 3.5 / 28.00 for Warren orders; and 2.5 / 59.94 and 2 / 53.04 for Gardeners orders. The mean is pulled up by a small number of large survivors; it does not describe the typical run.

## Human-readable slashline and direct observations

Per-run mean slashlines use only runs where each ratio is `Valid`; this batch had 100 valid pAVI, eAVI, sAVI and bAVG values in every arm. The complete CSV preserves every raw count, other ratios, statuses, phase windows and paired deltas.

| Arm | pAVI | eAVI | sAVI | bAVG | Mean births | Mean prey deaths | Mean starvation deaths | Mean seed-drop successes |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| Skip-all control | 0.419 | 0.657 | 0.136 | 0.01184 | 264.07 | 123.37 | 142.01 | 0 |
| General Movement | 0.422 | 0.643 | 0.066 | 0.01181 | 275.37 | 139.35 | 160.23 | 0 |
| Threat Avoidance | 0.422 | 0.646 | 0.091 | 0.01182 | 252.11 | 127.40 | 141.52 | 0 |
| Tough Hide | 0.486 | 0.615 | 0.090 | 0.01153 | 264.51 | 130.23 | 148.39 | 0 |
| Crowding Tolerance | 0.417 | 0.657 | 0.110 | 0.01158 | 261.08 | 123.14 | 145.09 | 0 |
| Efficient Digestion | 0.420 | 0.659 | 0.121 | 0.01144 | 263.03 | 121.92 | 145.62 | 0 |
| Seed Dispersal | 0.418 | 0.653 | 0.174 | 0.01144 | 271.77 | 125.03 | 132.12 | 115.19 |
| Trailblazer: Movement then Avoidance | 0.416 | 0.638 | 0.048 | 0.01168 | 273.04 | 140.40 | 158.86 | 0 |
| Trailblazer: Avoidance then Movement | 0.419 | 0.647 | 0.062 | 0.01154 | 268.90 | 134.83 | 156.10 | 0 |
| Warren: Hide then Crowding | 0.484 | 0.618 | 0.086 | 0.01136 | 262.07 | 131.17 | 145.98 | 0 |
| Warren: Crowding then Hide | 0.465 | 0.644 | 0.124 | 0.01126 | 274.81 | 123.64 | 163.17 | 0 |
| Gardeners: Digestion then Dispersal | 0.422 | 0.658 | 0.186 | 0.01125 | 275.18 | 125.82 | 129.42 | 116.17 |
| Gardeners: Dispersal then Digestion | 0.418 | 0.647 | 0.171 | 0.01157 | 266.87 | 127.34 | 126.49 | 110.18 |

Seed Dispersal recorded 11,519 successful seed drops across its 100 runs; the two Gardeners orders recorded 11,617 and 11,018. Success/food-spend accounting reconciled in every run. Skill acquisition was reached at tick 100 in all 100 runs for each single and each pair. The second acquisition at tick 200 was reached by 99/100 Warren Hide-first runs and 99/100 Gardeners Dispersal-first runs; every other pair reached both choices in 100/100.

## Validation and evidence

- All 1,300 assigned arm/seed outputs exist; no run job failed. Hare extinctions and their actual stop ticks remain in the data.
- All starts were 600/40/25 on 36x20 with wrapping enabled. Same-seed ruleset fingerprints matched, and each arm's pre-choice phase matched its control after excluding process-scoped entity IDs.
- Ordered acquisitions and immutable upgrade snapshots matched the schedules. Registry fingerprints were consistent.
- All 1,300 whole-run and 7,219 observed phase slashlines passed independent denominator, formula, validity-status and FPO reconciliation checks. Histories cover every tick through each run's actual stop; observed populations stay within the 720-cell board.
- Unity's live `run_script` compiled the ignored dispatcher with no diagnostics. A two-run pilot validated shared inputs and the identical first phase. The Movement pilot ended at tick 300 from Hare extinction and is kept as a pilot, separate from the 1,300-run count.
- This is live Editor research evidence, not a standard NUnit run, Clean lane, build-readiness check, visual review or production balance result.

The generated CSVs, requests, run JSON, fingerprints, hashes, pilot, scripts and validation are stored locally under `artifacts/s4-paired-screen-20261005-600-40-25-wrapped-100/` (ignored by Git). `validation.json` lists per-run SHA256 values; `experiment-contract.json` freezes the approved inputs and source hashes.
