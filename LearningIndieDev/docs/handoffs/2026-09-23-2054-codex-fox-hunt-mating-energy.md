# Fox hunting and mating energy

[Working state](../WORKING_STATE.md) | Status: complete

- Handoff schema: 1
- Handoff ID: 2026-09-23-2054-codex-fox-hunt-mating-energy
- Owner: Codex
- Branch: NF/FoxHuntingImprovements
- Baseline commit: 919f2306
- Date: 2026-09-23
- Supersedes: none

## Summary

Foxes now seek available prey while below 75% of maximum energy. This priority
can interrupt the short Mating state, and a Fox that is hunting, eating, or
attacking cannot resolve a birth that tick. If prey is unavailable, the
existing mate-seeking behavior can begin once the Fox has at least 25% of its
maximum energy. A successful birth costs both Fox parents 15% of maximum
energy per offspring, and each reproduction attempt has a 45% success chance.
A pair that entered the Mating state at exactly 25% can
resolve that attempt even when metabolism is applied before the birth. The
Fox's authored litter is one, and newborns still start with the authored 48
energy.

These are ratios in the species rules, so the thresholds and cost scale with a
maximum-energy change. Fractional energy amounts round up to whole energy
units. A zero ratio keeps the previous absolute or role-based behavior.

## Implementation

- Authored the Fox ratios in [`fox.asset`](../../Assets/Data/CellularSimulation/Species/fox.asset).
- Applied forage precedence in behavior selection, vision movement, and the
  reproduction resolver. Other species with no forage ratio keep their old
  priority.
- Kept offspring energy separate from the configured reproduction cost.
- Preserved the ratios through simulation snapshots, attribute upgrades,
  simulation-preview drafts, and rule fingerprints.
- Added focused behavior, reproduction-cost, maximum-energy-scaling, and
  fingerprint regressions.

## Validation

- Unity recompile completed without errors; the Editor console reports no
  compile errors.
- Focused Fox EditMode filter passed 10/10 tests.
- Full EditMode passed 248/249. The unrelated existing
  `EfficientDigestionAccumulatesFractionalEnergyDeterministically` failure
  expects 23 and receives 0. The same failure was present before these changes.
- `git diff --check` and `git diff --cached --check` passed.
- No matched Forest Edge balance batch was run; these values remain provisional.

## Next step

If the player-facing behavior feels right, run a matched Forest Edge comparison
with fixed seeds and the current authored Fox rules before treating the tuning
as a balance conclusion.
