# Bev experimental held-out dose-response test

Date: 2026-08-21
Branch: `codex/bev-experimental-features`
Seeds: `10200-10219` (20 paired seeds per arm)
Scenario: Forest Edge, 32x32, 20 seconds, 0.1 second step
Total: 12 arms / 240 simulations

## Test question

Do the experimental stats produce the mechanical effects a player would expect, and do those effects translate into improved Hare fitness?

## Hypotheses and acceptance criteria

1. Increasing Hare block should lower the expected and observed Fox hit rate. **Passed.** Expected hit rate fell from 57% in the opposed control to 47%, 38%, 30%, 23%, and 17% for block +2/+4/+6/+8/+10. Observed rates were 46%, 41%, 27%, 26%, and 18%.
2. Increasing Fox cooldown should increase cooldown-suppressed attacks and reduce Fox attack pressure. **Partially passed.** Suppressions rose monotonically: 2.8, 8.0, 9.45, 12.65, and 15.25 per run for cooldown 2/4/6/8/10. Fox attempts fell modestly from 8.35 in control to 7.95, 8.45, 7.70, 7.45, and 7.80; suppression and attempts are now separately visible.
3. Lower pressure should improve Hare stability. **Partially passed.** Cooldown 6 and 8 improved mean Hare population by +1.39/+1.49 and AUC by +279/+299 versus opposed control. Cooldown 2 and 4 improved mean/AUC but had lower average final population. Cooldown 10 regressed from the 6–8 window.
4. Higher block should improve Hare fitness. **Failed for this ecology and implementation.** Block upgrades lowered mean/AUC versus opposed control by -0.33 to -1.18/-238 and increased Fox attempts to 10.10–20.15 per run. Lower hit chance did not prevent repeated-contact pressure.

## Key aggregate results

| Arm | Final Hare | Mean Hare | AUC | Below 10 ticks | Fox attempts | Fox kills | Actual hit | Expected hit | Cooldown suppressions |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Opposed control | 23.00 | 16.80 | 3376 | 11.20 | 8.35 | 4.50 | 54% | 57% | 0.00 |
| Block +2 | 20.30 | 16.32 | 3281 | 12.30 | 10.10 | 4.65 | 46% | 47% | 0.00 |
| Block +4 | 19.95 | 16.23 | 3262 | 13.70 | 11.10 | 4.50 | 41% | 38% | 0.00 |
| Block +6 | 20.45 | 16.47 | 3310 | 14.40 | 16.50 | 4.40 | 27% | 30% | 0.00 |
| Block +8 | 18.15 | 15.61 | 3138 | 24.05 | 15.00 | 3.90 | 26% | 23% | 0.00 |
| Block +10 | 20.00 | 16.85 | 3387 | 21.50 | 20.15 | 3.55 | 18% | 17% | 0.00 |
| Cooldown 2 | 22.25 | 17.42 | 3501 | 22.75 | 7.95 | 4.45 | 56% | 57% | 2.80 |
| Cooldown 4 | 21.85 | 17.78 | 3573 | 11.45 | 8.45 | 4.60 | 54% | 57% | 8.00 |
| Cooldown 6 | 25.40 | 18.19 | 3655 | 13.65 | 7.70 | 4.40 | 57% | 57% | 9.45 |
| Cooldown 8 | 25.30 | 18.28 | 3675 | 12.85 | 7.45 | 4.10 | 55% | 57% | 12.65 |
| Cooldown 10 | 24.15 | 17.72 | 3562 | 12.65 | 7.80 | 4.35 | 56% | 57% | 15.25 |

## Interpretation

The opposed-roll telemetry is trustworthy: actual hit rates track the exact probabilities implied by the modifiers. The ecological result is the important surprise: block changes the probability of each contact, but surviving targets can create more future contact attempts. Cooldown 6–8 currently gives the clearest stability benefit without requiring a lower roll probability.

This is evidence for keeping final Hare population as one outcome metric, but pairing it with mean population, AUC, time below a danger threshold, Fox attempts/kills, actual-vs-expected hit rate, and cooldown suppressions.

## Artifacts

The 12 report directories are the newest `artifacts/cellular-experiment-*` folders timestamped from `20260821-191630` through `20260821-192324`; each contains `report.json`, `report.csv`, and generated `analysis.md`.

## Next decision

Do not promote block +8/+10 as fitness upgrades yet. Treat cooldown 6–8 as the current experimental candidate window, then run a targeted follow-up that separates `AttackModifier` from `Damage` before adding more mutation paths.
