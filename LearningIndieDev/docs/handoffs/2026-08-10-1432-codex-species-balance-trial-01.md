# species balance trial 01

[Working state](../WORKING_STATE.md) | [Control baseline](2026-08-10-1417-codex-species-balance-ab-baseline.md) | Status: trial-ready

- Owner: codex
- Branch: GridDesignWork
- Date: 2026-08-10
- Trial: Resource-coupled reproduction, crowding stress, and population reset

## Purpose

Address the baseline swarm/extinction pattern while keeping the changes
reproducible and attributable. This is the first variant to compare against the
control note; it is not considered balanced until fixed-seed runs are recorded.

## Variant settings

### Initial seeding

- Plants: `0.40`
- Herbivores: `0.16`
- Carnivores: `0.04`
- Same-species seed clump rejection: `0.65` with one or two nearby matches,
  `0.90` with more than two.

### Plants

- Reproduction chance: `0.10`
- Wilt chance per tick: `0.003`
- No movement, attack, or food requirement.

### Herbivores

- Movement speed: `1.5` effective movement passes per tick.
- Moore food-search pattern.
- Reproduction requires one current food reserve and one nearby herbivore.
- Reproduction consumes one food reserve.
- Maximum local reproduction group: `4`.
- Starting energy: `12`.
- Crowding energy penalty: `1` per excess local group member per tick.

### Carnivores

- Movement speed: `1.5` effective movement passes per tick.
- Starting energy increased to `18` so viable hunting pairs have time to form.
- Reproduction still requires one current food reserve and one adjacent carnivore.
- Reproduction consumes one food reserve.
- Maximum local reproduction group: `3`.
- Crowding energy penalty: `1` per excess local group member per tick.

## Behavioral changes

- Cells now track both lifetime `FoodEaten` telemetry and a consumable
  `FoodReserve`; reproduction uses and consumes the reserve.
- Oversized local groups drain energy, so pack limits create a population cost
  rather than only blocking new births.
- Movement speed now controls additional movement passes instead of being only a
  boolean movement gate.
- Each run records a per-tick `SpeciesPopulationSnapshot` history for plants,
  herbivores, carnivores, and empty tiles.

## Expected outcome

- Herbivores should no longer reproduce indefinitely after a single meal.
- Empty tiles should persist longer and provide recovery space for plants.
- Herbivores should spread more broadly instead of forming large stable clumps.
- Carnivores should survive long enough to hunt, find mates, and produce more
  than a single survivor in a meaningful portion of seeds.
- All three populations should fluctuate without routine extinction.

## Trial measurement

Run the control and this variant with the same fixed seeds, grid size, run
duration, and step interval. At each 25% checkpoint record the corresponding
entry from `SimulationRunState.PopulationHistory`. Also record:

- Final and minimum population for each species.
- First extinction tick for each species.
- Largest local same-species group.
- Number of births, feeding events, starvation deaths, wilt events, and crowding
  deaths when those counters are available.

## Validation

- `dotnet build LearningIndieDev.slnx --no-restore`: succeeded with 0 errors and
  the existing `DelegateCommand` warning.
- `git diff --check`: passed.
- Gameplay results are pending fixed-seed Unity playtests; this note deliberately
  does not claim the variant is balanced yet.

## Next useful step

Run at least five control/variant seed pairs and append the observed population
curves and extinction outcomes to a follow-up experiment note. If the variant
still produces herbivore swarms, tune the food reserve cost before changing
vision or pathfinding.
