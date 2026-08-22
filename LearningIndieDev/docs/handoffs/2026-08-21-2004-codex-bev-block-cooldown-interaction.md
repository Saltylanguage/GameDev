# Bev block + cooldown interaction test

Date: 2026-08-21
Branch: `codex/bev-experimental-features`
Seeds: `10300-10319` (20 paired seeds per arm)
Scenario: Forest Edge, 32x32, 20 seconds, 0.1 second step
Total: 18 arms / 360 simulations

## Test question

Does Fox cooldown specifically rescue the repeated-contact pressure that made Hare block upgrades unreliable when tested alone?

## Design

The test used same-seed reference arms for opposed control, block-only +2/+4/+6/+8/+10, and cooldown-only 6/8. It then tested every block level combined with cooldown 6 and 8. The primary comparison is combined versus the same block level without cooldown.

## Acceptance results

1. Combined cooldown should reduce Fox attack attempts relative to block-only. **Passed.** Attempts fell in all ten combined arms, by 0.8 to 10.8 per run.
2. Combined cooldown should reduce Fox kills relative to block-only. **Passed.** Fox kills fell in all ten combined arms, by 0.3 to 1.95 per run.
3. Combined cooldown should improve Hare mean population/AUC relative to block-only. **Passed with one near-zero exception.** Mean Hare population improved in 9/10 arms; AUC improved in 9/10 arms. The exception was block +8/cooldown 8, approximately flat versus block-only.
4. Combined cooldown should reduce danger-time consistently. **Partially passed.** Cooldown 8 improved time below 10 Hares for block +2/+4/+6/+10; cooldown 6 improved it for block +2/+4/+6. High block plus cooldown 6 remained unstable, so cooldown does not erase every cost of excessive block.

## Key combined results

| Arm | Mean Hare | AUC | Below 10 ticks | Fox attempts | Fox kills | Suppressions/run |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Block +2 / cooldown 6 | 19.83 | 3987 | 8.40 | 7.65 | 3.95 | 9.85 |
| Block +4 / cooldown 6 | 21.42 | 4305 | 7.70 | 8.60 | 3.15 | 14.10 |
| Block +6 / cooldown 6 | 21.71 | 4363 | 6.50 | 9.20 | 2.55 | 17.30 |
| Block +8 / cooldown 6 | 20.91 | 4203 | 12.90 | 10.65 | 2.05 | 23.85 |
| Block +10 / cooldown 6 | 21.32 | 4285 | 11.90 | 10.85 | 1.50 | 25.50 |
| Block +2 / cooldown 8 | 20.02 | 4025 | 8.15 | 8.15 | 4.15 | 12.50 |
| Block +4 / cooldown 8 | 21.47 | 4316 | 4.25 | 8.60 | 3.35 | 14.75 |
| Block +6 / cooldown 8 | 21.34 | 4290 | 3.55 | 8.40 | 2.65 | 16.55 |
| Block +8 / cooldown 8 | 20.56 | 4133 | 5.50 | 8.90 | 2.25 | 21.65 |
| Block +10 / cooldown 8 | 21.47 | 4316 | 3.80 | 10.10 | 1.85 | 26.00 |

## Interpretation

This is evidence of a real interaction: cooldown changes the ecological meaning of block. Block-only arms still generated many repeated Fox attempts, while adding cooldown cut those attempts and reduced kills. The strongest current stability candidates are block +6/cooldown 6 for mean/AUC and block +4 to +10/cooldown 8 for time below the danger threshold.

The result is not a promotion decision yet. It is one held-out 20-seed sweep, and high block can still create unstable tails. Final population remains secondary to the pressure and stability metrics.

## Artifacts

The 18 report directories are the newest `artifacts/cellular-experiment-*` folders timestamped from `20260821-194231` through `20260821-195337`; each contains `report.json`, `report.csv`, and generated `analysis.md`.

## Next step

Run a smaller confirmation sweep around block +4/+6 with cooldown 6/8, then split `AttackModifier` from `Damage` so future mutation paths can distinguish “avoid the hit” from “survive the hit.”
