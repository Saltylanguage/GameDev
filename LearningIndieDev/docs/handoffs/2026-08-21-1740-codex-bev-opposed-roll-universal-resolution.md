# Bev opposed roll universal resolution

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: codex
- Branch: BevLaptopBranch
- Baseline commit: 118f9ca
- Date: 2026-08-21

## Summary

Bev opposed-roll combat now resolves every valid creature-versus-creature
attack with seeded d20 rolls. This makes attack and block upgrades meaningful
fixed modifiers across the whole interaction instead of allowing directional
block coverage to bypass the dice system.

## Changes

- Moved opposed-roll resolution outside the directional-block gate in
  `Assets/Scripts/Game/Simulation/SpeciesSimulation.cs`.
- The attack modifier is the attacker's authored `AttackAmount`; the block
  modifier is the target's authored `BlockAmount`, including when its block
  pattern does not cover the attack direction.
- Preserved legacy fixed-damage behavior when opposed-roll mode is disabled.
- Updated the domain regression test to cover an unguarded attack direction,
  authored modifiers, deterministic replay, and legacy behavior.

## Decisions and assumptions

- In Bev/opposed-roll mode, a valid creature attack always rolls; block
  patterns no longer decide whether dice are used.
- Attack patterns still determine which cells can be targeted.
- Plant feeding is not creature combat and remains outside opposed-roll
  resolution.
- The existing opt-in Bev toggle and Fox cooldown behavior are unchanged.

## Validation

- Unity EditMode suite: **151/151 passed**.
- PlayMode suite: **4/7 passed, 3 failed** because the existing Noesis
  `TextureSource` native-pointer exception occurs during cellular preview
  startup, including before the Bev toggle test assertions. The failures are
  unrelated to this combat change.
- Artifacts:
  - `artifacts/unity-tests-20260821-173804/EditMode-results.xml`
  - `artifacts/unity-tests-20260821-173858/PlayMode-results.xml`

## Risks and incomplete work

- The implementation currently reuses `AttackAmount` and `BlockAmount` as
  both damage/stat values and roll modifiers, as intended for this prototype.
- No balance conclusion has been drawn; seeded experiments should be rerun
  after human review of the universal-roll rule.
- PlayMode remains limited by the pre-existing Noesis texture-pointer issue.

## Next useful step

Run the Bev baseline and opposed-roll variants with the same seeds, then
compare whether stronger block/attack upgrade paths produce stable combat and
population differences.
