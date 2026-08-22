# Bev upgrade cooldown 20 seed sweep

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: codex
- Branch: BevLaptopBranch
- Baseline commit: 118f9ca
- Date: 2026-08-21

## Summary

Ran a controlled 20-seed Forest Edge sweep to test whether fixed attack/block
modifiers and Fox attack cooldowns create useful species upgrade paths under
opposed-roll combat.

## Changes

- Ran 12 arms total, all on seeds `10100-10119`, with Hare as the player
  species, 32x32 grid, 20-second runs, and natural attack opportunities:
  - Legacy default.
  - Opposed-roll control with no block upgrade and no cooldown.
  - Hare Block `+2`, `+4`, `+6`, `+8`, `+10`, cooldown off.
  - Fox cooldown `2`, `4`, `6`, `8`, `10`, Hare block upgrade off.
- Generated a JSON report and `analysis.md` for every arm under
  `artifacts/cellular-experiment-*`.

## Decisions and assumptions

- Block and cooldown sweeps were separated so their effects are not confounded.
- The legacy default is useful as the product baseline; the opposed-roll
  no-upgrade/no-cooldown arm is the correct control for Bev upgrade deltas.
- These are descriptive 20-seed results, not a promotion decision or a
  statistical proof.

## Validation

- All 12 experiment arms completed successfully: **240 seeded runs**.
- Legacy default: average final Hare `27.15`; Fox `2.40`.
- Opposed-roll control: average final Hare `23.45`; Fox `2.35`; Fox hit rate
  `54.5%`.
- Block sweep, compared with opposed-roll control:

| Arm | Final Hare | Delta | Fox hit rate |
| --- | ---: | ---: | ---: |
| Block +2 | 24.10 | +0.65 | 48.0% |
| Block +4 | 25.65 | +2.20 | 38.7% |
| Block +6 | 23.95 | +0.50 | 31.6% |
| Block +8 | 22.90 | -0.55 | 23.6% |
| Block +10 | 22.05 | -1.40 | 18.8% |

- Cooldown sweep, compared with opposed-roll control:

| Arm | Final Hare | Delta | Fox attempts/run |
| --- | ---: | ---: | ---: |
| Cooldown 2 | 25.35 | +1.90 | 6.75 |
| Cooldown 4 | 27.30 | +3.85 | 6.30 |
| Cooldown 6 | 26.15 | +2.70 | 5.60 |
| Cooldown 8 | 25.75 | +2.30 | 6.45 |
| Cooldown 10 | 25.55 | +2.10 | 5.80 |

- The universal opposed-roll rule is confirmed in the reports: opposed arms
  record rolls for every resolved creature combat attempt, not only attacks
  covered by directional block.

## Risks and incomplete work

- Opposed roll alone reduced the Hare average by `3.70` versus legacy default;
  this is a mode effect, not evidence that any upgrade is bad by itself.
- Block strength reduces Fox hit rate monotonically, but the population result
  is not monotonic; higher block values are not automatically better ecological
  outcomes.
- Cooldown 4 is the strongest result in this sample, but the differences are
  still vulnerable to natural-world variance and the sample is only 20 seeds.

## Next useful step

Use the opposed-roll control and the strongest candidate arms (`Block +4` and
`Cooldown 4`) for a larger held-out seed range, then decide whether the upgrade
values should be tuned independently from the combat-mode baseline.
