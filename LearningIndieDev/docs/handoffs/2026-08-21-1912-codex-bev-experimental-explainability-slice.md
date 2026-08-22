# Bev experimental features: explainability slice

Date: 2026-08-21
Branch: `codex/bev-experimental-features`

## Decision

Keep the default/legacy mechanics unchanged. Continue building Bev's dice-roll, fixed-modifier, and cooldown ideas behind the existing `bev-experimental` opt-in. The first implementation slice adds observability so upgrade effects can be explained before we change the stat model.

## Implemented

- Opposed-roll events now expose attack total, block total, and the exact expected d20 hit probability for the recorded modifiers. Defender wins ties.
- Experimental fox cooldowns now record each eligible attack skipped because cooldown ticks remained, including attacker identity, position, tick, and remaining cooldown.
- Experiment JSON includes `expectedHitProbability` on `combatRolls` and `combatCooldownSuppressions` on each run.
- `New-CellSimReport.ps1` includes an Experimental combat diagnostics table with rolls/run, actual hit rate, expected hit rate, and cooldown suppressions/run.
- Added regression tests for probability math and cooldown suppression telemetry.

## Safety boundary

- `LegacyFixedDamage` still bypasses opposed-roll telemetry and uses its existing fixed-damage/block behavior.
- Cooldown suppression only exists when experimental options are supplied; no default settings or serialized default species rules were changed.
- The current `AttackAmount` remains the authored attack modifier/damage value. Splitting attack modifier from damage is intentionally deferred until the diagnostics establish the intended progression model.

## Validation

- EditMode: 152/152 passed (`artifacts/unity-tests-20260821-190805/EditMode-results.xml`).
- One experimental Forest Edge run (seed 10100, opposed roll, Bev opt-in, fox cooldown 4) emitted 6 combat rolls and 1 cooldown suppression event; report generation succeeded.

## Next step

Use the new telemetry in a held-out multi-seed comparison, then decide whether the next slice should split `AttackAmount` into separate attack modifier and damage fields or add a deterministic Combat Lab before introducing more mutation paths.
