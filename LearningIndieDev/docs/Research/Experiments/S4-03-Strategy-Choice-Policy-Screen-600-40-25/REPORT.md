# S4-03 strategy-choice policy screen — 600/40/25 wrapped

## At a glance

None of the three offer-aware Hare strategies reached the agreed rough 40–50% node-clear target. Gardeners cleared 51/200 runs (25.5%); Trailblazer cleared 30/200 (15.0%), Warren 34/200 (17.0%), and skip-all cleared 32/200 (16.0%). On matched seeds, Gardeners cleared 31 runs that skip-all did not, while skip-all alone cleared 12; its clear-rate difference was +9.5 percentage points (95% CI +3.2 to +15.8, exact McNemar p=0.0054). Trailblazer and Warren did not separate from control in this screen. The result points to Gardeners as a follow-up candidate, not as a balanced or accepted build.

This tests one 600-tick Forest Edge node at wrapped 36x20 and 600 Plants / 40 Hares / 25 Foxes. It does not test the later journey map or a stable long-running ecology. No gameplay values, scenario defaults, population settings, or authored assets were changed for the experiment.

Experiment ID: `S4-03-20261006-600-40-25-wrapped-strategy-choice-policy-screen`. Bevin approved the protocol on 2026-10-06. Unity 6000.4.6f1; branch `BevBranch`; source commit `6a0ab08e155b1a8a2962d19a6c5eca1bc8233118` with existing dirty working-tree changes. The frozen contract records the exact relevant source-file and Forest Edge asset hashes.

## Inputs and scoring

All 800 main runs used Production Forest Edge, 36x20, wrapping on, 600 Plants / 40 Hares / 25 Foxes, Hare player, 0.1-second steps, natural opposed-roll combat, six continuous 100-tick phases, `bev-experimental`, and coupled species responses off. The four arms shared fresh seeds 14200–14399. The three strategy arms made up to five choices after ticks 100, 200, 300, 400, and 500. The policy generated each offer with the production experimental Hare offer function, chose only an offered skill on its assigned two-skill path, favored the lower-level skill, broke ties by the path's first-pick order, and skipped when neither path skill was offered.

The clear rule was fixed before execution: the run must reach tick 600 with Plants > 0, Hares >= 5, and Foxes >= 5. Exactly five Hares or Foxes qualifies. A run ending before tick 600 fails the clear. The rough 40–50% target is a review band, not an automatic pass/fail or tuning instruction.

| Arm | Clears / 200 | Clear rate (95% Wilson CI) | Reached tick 600 | Median final Plants / Hares / Foxes |
|---|---:|---:|---:|---:|
| Skip-all control | 32 | 16.0% (11.6–21.7%) | 122 | 353 / 1 / 19 |
| Trailblazer | 30 | 15.0% (10.7–20.6%) | 121 | 189 / 1 / 20 |
| Warren | 34 | 17.0% (12.4–22.8%) | 135 | 237 / 2 / 15.5 |
| Gardeners | 51 | 25.5% (20.0–32.0%) | 138 | 311 / 2.5 / 14 |

Matched all-species clear outcomes (candidate only / both clear / skip-all only / neither clear) were:

| Candidate vs skip-all | Outcomes / 200 | Risk difference, candidate minus skip-all (95% CI) | Exact McNemar p |
|---|---:|---:|---:|
| Trailblazer | 18 / 12 / 20 / 150 | -1.0 pp (-7.0 to +5.0) | 0.8714 |
| Warren | 15 / 19 / 13 / 153 | +1.0 pp (-4.2 to +6.2) | 0.8506 |
| Gardeners | 31 / 20 / 12 / 137 | +9.5 pp (+3.2 to +15.8) | 0.0054 |

The three strategy paths were offered on 76–81% of recorded decision points. Trailblazer selected a path skill on 698 of 908 decisions and skipped 210 times because neither path skill was offered. Warren selected on 693 of 911 and skipped 218; Gardeners selected on 737 of 905 and skipped 168. Selected skill totals were Trailblazer: Faster Movement 414, Threat Avoidance 284; Warren: Tough Hide 414, Crowding Tolerance 279; Gardeners: Efficient Digestion 436, Seed Dispersal 301. Offer and level audits confirmed every selected skill was present in that run's offer, stayed on path, and followed the lower-level / tie-break rule.

## Phase boundary and slashline evidence

Median Plants / Hares / Foxes among runs that actually reached each exact boundary were:

| Arm | Tick 100 (n) | Tick 300 (n) | Tick 500 (n) | Tick 600 (n) |
|---|---|---|---|---|
| Skip-all | 584 / 21 / 31 (200) | 545 / 22 / 13 (179) | 166 / 103 / 3 (138) | 17.5 / 23.5 / 2 (122) |
| Trailblazer | 584 / 21 / 31 (200) | 515.5 / 32.5 / 16.5 (192) | 84.5 / 71.5 / 9.5 (148) | 6 / 10 / 3 (121) |
| Warren | 584 / 21 / 31 (200) | 542 / 19 / 15 (187) | 230 / 79 / 4 (157) | 45 / 29 / 1 (135) |
| Gardeners | 584 / 21 / 31 (200) | 551 / 21 / 14 (187) | 294 / 101 / 5 (153) | 96 / 104.5 / 3 (138) |

Median Fox activity over each run was:

| Arm | Births | Total deaths | Starvation deaths | Food consumed | Reproduction candidates blocked by energy | Blocked by mate requirement |
|---|---:|---:|---:|---:|---:|---:|
| Skip-all | 40 | 67 | 48 | 108.5 | 6,683.5 | 671 |
| Trailblazer | 45 | 67 | 47 | 132 | 7,733 | 687.5 |
| Warren | 40 | 69 | 48.5 | 112.5 | 6,952.5 | 659.5 |
| Gardeners | 41 | 69 | 48 | 112 | 6,693.5 | 688 |

These are per-run medians, not counts from a single representative run. Every strategy ended with median Fox population below the five-Fox clear threshold at tick 600; Gardeners' median Plants and Hares were above their thresholds among runs that reached tick 600.

Slashline ratio medians use only `Valid` values; N/A values are not treated as zero.

| Arm | pAVI | eAVI | predAVG | sAVI | cAVI valid / 200 | bAVG | RFS | APS |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| Skip-all | 0.426 | 0.627 | 0.523 | 0.037 | 121 | 0.0112 | -0.318 | -0.693 |
| Trailblazer | 0.423 | 0.617 | 0.514 | 0.013 | 121 | 0.0114 | -0.345 | -0.756 |
| Warren | 0.485 | 0.612 | 0.545 | 0.072 | 135 | 0.0112 | -0.263 | -0.598 |
| Gardeners | 0.427 | 0.646 | 0.530 | 0.100 | 137 | 0.0117 | -0.176 | -0.548 |

All 800 whole-run Hare FPO values reconciled. Every observed phase slashline passed the denominator, validity-status, and formula checks. Hare seed-drop successes reconciled with reserve spent in every run.

## Execution and validation

- Four one-seed diagnostics used seed 14200, one for each arm. The selected choices and five phase decisions were validated before the main panel; the pilot was excluded from all main-run counts.
- Each main arm report contains 200 unique runs for exactly 14200–14399. All frozen scenario, population, wrapping, grid, combat, step, feature, and phase inputs match.
- An independent Python standard-library analyzer validated the complete per-tick histories; exact-boundary phase populations; all five-choice offer, selection, level, and acquisition audits; final counts; slashline formulas and statuses; FPO; seed-drop accounting; matched seed outcomes; and frozen source/scenario hashes. All checks passed.
- All four Unity CLI arm commands exited successfully. Run logs contain Unity Licensing Client validation / entitlement 404 notices at startup, but no C# compiler errors or simulation exceptions. The `CellSim.ps1` wrapper could not be used because its `Resolve-UnityExecutionLane` helper is absent from this checkout; the same editor experiment entry point was invoked through `unity run` with the frozen arguments.
- No NUnit suite, player build, visual review, long-run stability test, or human balance acceptance was performed. This screen changed no simulation values.

Raw reports, CSV files, Unity logs, the frozen contract, independent analyzer, validated per-run CSVs, and machine-readable summary are under the ignored local path `artifacts/s4-03-strategy-choice-policy-screen-20261006-600-40-25-wrapped-200/`.
