# Bev confirmation sweep and combat stat split

Date: 2026-08-21
Branch: `codex/bev-experimental-features`

## Confirmation sweep

Seeds `10400-10419`, Forest Edge 32x32, 20 seconds, 0.1 second step. Nine arms / 180 simulations: opposed control, block +4/+6, cooldown 6/8, and the four combined block +4/+6 × cooldown 6/8 arms.

The interaction repeated partially:

- Cooldown 8 improved combined block +4 versus block-only by +0.52 mean Hare, +104 AUC, -1.95 danger-time ticks, -1.75 Fox attempts, and -0.75 Fox kills per run.
- Cooldown 8 improved combined block +6 versus block-only by +0.37 mean Hare, +75 AUC, -8.10 danger-time ticks, -5.70 Fox attempts, and -1.60 Fox kills per run.
- Cooldown 6 reduced danger-time and Fox kills for both block levels, but mean/AUC were mixed on this seed set: block +4/cooldown 6 was -0.62 mean/-125 AUC and block +6/cooldown 6 was -0.21 mean/-43 AUC versus block-only.

Conclusion: cooldown consistently reduces pressure and danger exposure when blocking is enabled, but cooldown 8 is the more repeatable candidate for mean/AUC improvement. This supports separating “avoid the hit” from “survive the hit,” without promoting a balance value yet.

## Implemented experimental stat split

- `SpeciesRules.AttackModifier` and `SpeciesRules.DamageAmount` now default to the legacy `AttackAmount` value.
- Bev experimental opposed-roll mode uses `AttackModifier` for the attack roll and `DamageAmount` for successful-hit damage.
- Legacy fixed-damage mode and non-Bev opposed-roll mode continue using `AttackAmount`.
- Added opt-in upgrade types/catalog entries: `stronger-attack-modifier` and `stronger-damage`.
- Existing `stronger-attack` remains a combined legacy-style upgrade and increments all three attack values.

## Validation

- EditMode: 154/154 passed (`artifacts/unity-tests-20260821-201652/EditMode-results.xml`).
- Focused tests prove independent modifier/damage upgrades and Bev-only opposed-roll separation.
- Confirmation runner: 9 arms / 180 simulations, no runner failures.

## Next step

Use the new split upgrade paths in a deterministic combat lab, then run a small confirmation around block +4/+6 with cooldown 8 before considering any balance promotion.
