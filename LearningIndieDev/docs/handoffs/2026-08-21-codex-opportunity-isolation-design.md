# Controlled Fox attack-opportunity isolation design

## Decision

Use **Method C: fixed-rate diagnostic opportunity mode**. The mode is named
`fixed-rate-diagnostic`, is disabled by default, and polls for one global combat
slot every `3` simulation ticks. The three-tick cadence is a diagnostic sampling
interval chosen before the accepted rerun to avoid missing one-tick contact
windows in the 0.1-second simulation; it is not a production attack rate and is
not selected from a preferred ecological outcome.

## Runtime flow audited

In natural mode, `SpeciesBehaviorSystem.ChooseState` sees a visible diet target
and marks an adjacent Fox as `Attacking`. `SpeciesSimulation.ResolveAttacks`
then scans the authored attack pattern, records a creature opportunity when a
diet target is present in the source grid, rechecks that the target still exists
in the next grid, and resolves at most one target for that attacker in the tick.
Movement, hunger, target visibility, and target contention can therefore change
later opportunities after a combat outcome changes the source grid. There is no
separate attack cooldown; the effective cadence is behavior state plus tick-level
target availability.

## Controlled behavior

- The slot schedule is derived from the per-tick seed (`seed % 3 == 0`) and does
  not consume the simulation RNG, so `UpgradeId` cannot change scheduled slots.
- On a scheduled slot, the resolver enumerates current valid creature attackers
  and contacts, then selects one with a seed-indexed deterministic index. This
  exposes cardinal and diagonal contacts without using arm outcomes or inventing
  a target outside the authored contact pattern.
- The diagnostic mode bypasses only the natural `Attacking`/`ShouldForage`
  gate for that selected slot. Movement, aging, metabolism, resource regrowth,
  starvation, reproduction, target validity, opposed-roll randomness, and all
  death handling remain natural.
- Reports separate scheduled slots, eligible candidates, executed attempts, and
  unfulfilled slots (`no target` or `invalidated`). Scheduled equality is the
  control guarantee; eligible/attempt equality is measured rather than assumed.

## Known limitations

The two arms still evolve separate world states after their first differing
combat outcome. A later slot can therefore be scheduled in both arms but have no
eligible Fox-to-Hare contact in one arm. Those slots are explicitly counted as
unfulfilled; the experiment must be classified `PARTIALLY ISOLATED`, `NOT
ISOLATED`, or `BLOCKED` if residual exposure differences are large enough to
confound mortality.

## Frozen experiment

- Scenario: `Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset`
- Grid: `32 x 32`; duration `20.0 s`; step `0.1 s`
- Combat: `opposed-roll`
- Arms: `UpgradeId none` vs `stronger-block-2`
- Opportunity mode: `fixed-rate-diagnostic`
- Calibration seeds: `10100–10119`
- Held-out seeds: `10125–10144`
- Required runs: `80`
- No balance, damage, reproduction, resource, capacity, or starting-population
  changes are part of this experiment.

## First-pass control correction

The initial implementation always selected the first valid contact. Its 80-run
check (`cellular-experiment-20260821-013330` through `013509`) overrepresented
unblocked diagonal contacts: calibration produced no Block+2 hit-rate change.
Those runs are retained as a failed control check and are not ecological
evidence. The scheduler was corrected before the accepted rerun to select a
seed-indexed candidate across the full valid-contact set, so authored cardinal
and diagonal contacts are exposed without using arm outcomes. The corrected
implementation must be rerun across all 80 seeds.

The corrected seed-indexed selector with the original 30-tick cadence then
produced exact exposure equality but only 16 calibration attempts across 20
runs, and every sampled opposed roll had the same outcome in both arms. Those
reports (`20260821-013752` through `013929`) are also non-evidence for SC-2.
The accepted rerun uses the frozen three-tick sampling cadence.
