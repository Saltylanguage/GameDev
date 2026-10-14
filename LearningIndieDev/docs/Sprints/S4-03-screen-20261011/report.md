# S4-03 Hare strategy screen — factual report

**Status:** Needs Review (2026-10-11). **Experiment:** S4-03-HARE-STRATEGY-SCREEN-20261011. This report records observations; interpretation is in [AI analysis](ai-analysis-v1.md).

## Run contract

The [predeclared plan](../S4-03-screen-plan-20261011.md) fixed Forest Edge at 36×20, 400 Plants / 20 Hares / 10 Foxes, 0.1-second step, six 100-tick phases, opposed-roll combat, natural attack opportunities, coupled Fox response off, and seeds 11100–11119. Thirteen arms produced 260 seeded runs. The phase schedules, source commits, report hashes, and fingerprints are in [metadata](metadata.json); all per-seed outcomes are in [per-seed.csv](per-seed.csv). Raw Unity JSON/CSV and logs remain in ignored `LearningIndieDev/artifacts/s4-screen-20261011/` on the isolated worktree. The tracked [metrics](metrics.json) contain the compact arm totals.

The first screen exposed two Threat Avoidance faults: scheduled phases kept a zero avoidance option, and combat required a flee-speed bonus that Threat Avoidance no longer grants. Commit `a7667e77c68607fc832bc6412ba03c36eccc887c` corrected both. Control plus the three affected arms were replayed on the same seeds. The nine unaffected arms retain their original reports. The corrected control reproduced every archived original per-seed summary. Two original raw files were regenerated from the original source commit after a replay output-path error; their run summaries matched the archived originals, and both original and corrected manifests now validate every report hash.

## Observed outcomes

`Alive 600` means at least one Hare remained after all 600 ticks. `Reach 200` means the tick-200 decision boundary was reached. Paired gains/losses compare Hare survival at tick 600 against the same seed in control. Movement is steps divided by Hare population observations; it does not measure map coverage.

| Arm | Reach 200 | Alive 600 | Paired gains/losses | Mean stop tick | Hare move rate | Crowding energy lost | Seed drops |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| control | 19/20 | 6/20 | 0/0 | 398.6 | 1.4286 | 13 | 0 |
| movement | 15/20 | 7/20 | 4/3 | 369.3 | 1.7632 | 41 | 0 |
| avoidance | 19/20 | 4/20 | 2/4 | 367.7 | 1.4344 | 5 | 0 |
| hide | 18/20 | 5/20 | 2/3 | 357.6 | 1.4592 | 13 | 0 |
| crowding | 19/20 | 5/20 | 0/1 | 395.6 | 1.4279 | 15 | 0 |
| digestion | 19/20 | 6/20 | 0/0 | 398.4 | 1.4326 | 12 | 0 |
| dispersal | 19/20 | 5/20 | 4/5 | 392.9 | 1.4749 | 9 | 270 |
| trailblazer-movement-first | 15/20 | 6/20 | 5/5 | 378.8 | 1.8203 | 14 | 0 |
| trailblazer-avoidance-first | 19/20 | 7/20 | 2/1 | 395.6 | 1.6174 | 6 | 0 |
| warren-hide-first | 18/20 | 5/20 | 2/3 | 357.6 | 1.4594 | 7 | 0 |
| warren-crowding-first | 19/20 | 6/20 | 2/2 | 399.5 | 1.4619 | 0 | 0 |
| gardeners-digestion-first | 19/20 | 5/20 | 2/3 | 373.6 | 1.4884 | 2 | 152 |
| gardeners-dispersal-first | 19/20 | 5/20 | 4/5 | 390.4 | 1.4797 | 36 | 301 |

## Direct counters and limits

- The no-skill control reached tick 600 with Hares alive in 6/20 seeds. Nineteen seeds reached the second decision.
- General Movement raised the Hare movement rate from 1.4286 to 1.7632 steps per population observation. The two corrected Trailblazer orders recorded 1.8203 (Movement first) and 1.6174 (Avoidance first). No report field measures distinct map cells, home-range size, or patch revisits.
- Corrected single-level Threat Avoidance recorded 1,096 Fox combat attempts and 608 hits across its runs, versus 1,155 and 686 in control. Exposure differs because runs can end early, so these totals alone are not a contact-rate estimate. The avoidance ability was separately verified in a deterministic combat regression with zero flee-speed bonus.
- Control recorded only 13 Hare crowding-energy units lost across all 20 seeds. Crowding Tolerance and the Warren pairs therefore received little measured crowding pressure. Burrows is a 1.0 design target and is absent from this S4 screen.
- Seed Dispersal alone made 270 successful drops from 25,009 attempts, created 2,700 food units, and spent 270 stored-food units. The Gardeners pairs made 152 (Digestion first) and 301 (Dispersal first) successful drops, spending the same number of reserve units as drops. The report does not show a spatial network of patches or revisits.
- The phase schedule installs cumulative loadouts at tick 100 and tick 200 without using the player offer UI. This screen cannot substitute for the required in-game decision review. No skill values, production scenario asset, or confirmation thresholds were changed during this screen.

## Validation

- Unity 6000.4.6f1: 319/319 EditMode and 42/42 PlayMode tests passed at the fixed source commit. The new tests cover scheduled avoidance state and zero-flee-speed combat.
- All 13 original and four corrected raw report hashes validate against their manifests. Opening populations, seeds, schedules, and common launch ruleset fingerprint were checked per arm.
- This is a 20-seed exploratory screen. The separate 400/25/15 stress fixture, in-game review, spatial behavior measures, and predeclared 200-seed confirmation remain open.
