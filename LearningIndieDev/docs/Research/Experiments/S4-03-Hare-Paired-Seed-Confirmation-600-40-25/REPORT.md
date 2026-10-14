# S4 Hare paired-seed threshold confirmation - 600/40/25 wrapped

## At a glance

All **800 assigned runs completed** on 200 common fresh seeds (14000-14199), across four arms. Under the all-species node-clear rule, control cleared 25/200 runs, Seed Dispersal 58/200, Gardeners Digestion-first 50/200, and Gardeners Dispersal-first 57/200. The separate analysis evaluates matched outcomes and uncertainty. These are bounded observations, not an approved balance decision.

Experiment ID: `S4-03-20261006-600-40-25-wrapped-threshold-confirmation`. Report date: 2026-10-06. Human decision owner: Bevin; team reviewer: Salty. Unity 6000.4.6f1. Source branch `BevBranch`, baseline commit `6a0ab08e155b1a8a2962d19a6c5eca1bc8233118` with unrelated/pre-existing working-tree changes. The frozen contract records exact C# file hashes and the Forest Edge asset hash.

## Inputs and scoring

All arms use the Production Forest Edge scenario at 36x20, wrap edges on, 600 Plants / 40 Hares / 25 Foxes, Hare player, 0.1-second steps, six 100-tick phases, maximum 600 ticks, natural opposed-roll combat, `bev-experimental`, no coupled responses, no extra Genome and no reinforcements.

The clear rule was fixed before execution: at tick 600, Plants > 0 and both Hares and Foxes >= 5. Exactly five Hares or Foxes qualifies. A run ending before tick 600 is a failed clear. This is stricter than merely recording positive populations and reflects Bevin's stated node objective.

| Arm | Phase choices after tick 100 | Main runs |
| --- | --- | ---: |
| Skip-all control | Skip every choice | 200 |
| Seed Dispersal | Seed Dispersal at tick 100; then retain it | 200 |
| Gardeners, Digestion-first | Efficient Digestion at 100; Seed Dispersal at 200 | 200 |
| Gardeners, Dispersal-first | Seed Dispersal at 100; Efficient Digestion at 200 | 200 |

## Outcomes

Counts include every assigned seed. Rate intervals are 95% Wilson intervals for the proportion of runs clearing the node.

| Arm | Clears / 200 | Clear rate (95% Wilson CI) | Ended before tick 600 | Median final Plants / Hares / Foxes | Median peak Hares |
| --- | ---: | ---: | ---: | ---: | ---: |
| Skip-all control | 25 | 12.5% (8.6-17.8%) | 70 | 298 / 1 / 8.5 | 99 |
| Seed Dispersal | 58 | 29.0% (23.2-35.6%) | 64 | 194.5 / 15 / 13.5 | 165 |
| Gardeners, Digestion-first | 50 | 25.0% (19.5-31.4%) | 67 | 320.5 / 4 / 12.5 | 105.5 |
| Gardeners, Dispersal-first | 57 | 28.5% (22.7-35.1%) | 66 | 256.5 / 6.5 / 14 | 140.5 |

Matched all-species clear outcomes (`candidate only / both clear / control only / neither clear`) were:

| Candidate vs control | Paired clear outcomes / 200 |
| --- | ---: |
| Seed Dispersal | 47 / 11 / 14 / 128 |
| Gardeners, Digestion-first | 35 / 15 / 10 / 140 |
| Gardeners, Dispersal-first | 47 / 10 / 15 / 128 |

Between the two Gardeners orders, there were 30 seeds where only Digestion-first cleared, 37 where only Dispersal-first cleared, 20 where both cleared, and 113 where neither cleared.

## Final-count failure descriptions

Of the runs that reached tick 600 but failed the clear rule, insufficient Foxes were frequent: 70/78 failures for Seed Dispersal, 81/83 for Gardeners Digestion-first, and 73/77 for Gardeners Dispersal-first had fewer than five Foxes (sometimes with another threshold missed too). The control's 105 horizon-reaching failures more often missed multiple parts of the threshold: 83 had fewer than five Foxes, 47 had fewer than five Hares, and 63 had no Plants. Categories overlap. A run can also stop before tick 600 after Hare extinction.

## Slashlines and counters

Slashline ratios below are medians of each run's whole-node ratio, using only `Valid` values. N/A is not treated as zero.

| Arm | pAVI | eAVI | sAVI | bAVG | cAVI Valid / 200 | Median Seed Dispersal successes / food |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Control | 0.427 | 0.636 | 0.050 | 0.01127 | 129 | 0 / 0 |
| Seed Dispersal | 0.425 | 0.642 | 0.115 | 0.01134 | 136 | 59 / 590 |
| Gardeners, Digestion-first | 0.426 | 0.641 | 0.124 | 0.01146 | 132 | 41 / 410 |
| Gardeners, Dispersal-first | 0.430 | 0.637 | 0.104 | 0.01126 | 134 | 59.5 / 595 |

All 800 final Hare populations reconciled against the player FPO and `SPO + ADD + BIR - PREY - STRV - CRWD`; the analyzer also checked each observed phase line. Dispersal successes reconciled with reserve spent in every run. Full metric values, statuses, counters, final counts, phases and histories are retained in the raw artifacts.

## Execution and validation

- Four one-seed pilots (one per arm) used seed 14000 and are excluded from the 800 main runs.
- Each of the four main arm reports contains exactly 200 unique runs covering 14000-14199. All start counts, grid dimensions, edge wrapping, scenario identity and maximum ticks match the frozen contract.
- The independent analyzer validated complete per-tick histories, phase-window coverage through each run's stop, and every expected distinct skill acquisition that was reached before extinction.
- All 800 final and observed phase Hare slashlines passed independent denominator/status/formula checks; FPO reconciled in every run. Seed dispersal reserve and food accounting reconciled.
- The 14 frozen source-file SHA256 values and Forest Edge scenario SHA256 match after execution. No runtime/editor source or authored simulation asset changed during this confirmation.
- Unity batch commands completed successfully. Unity logged licensing-client access-token/entitlement messages at startup; the runs produced complete reports and successful process exit codes. No compiler errors or simulation exceptions were found in the run logs.
- `git diff --check` passed. No NUnit suite, editor visual review, build, or production-balance acceptance was run; this research batch did not edit gameplay source.

Raw JSON, CSV, logs, the frozen contract, and `analyze_confirmation.py` are under the ignored local artifact path `artifacts/s4-paired-confirmation-20261006-600-40-25-wrapped-200/`. `validated-summary.json` and the `*-validated.csv` files are generated by the independent analyzer. The analyzer asserts its data checks and uses only Python's standard library.
