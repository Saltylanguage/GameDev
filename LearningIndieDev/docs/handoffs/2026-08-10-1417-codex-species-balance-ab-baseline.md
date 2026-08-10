# species balance A/B baseline

[Working state](../WORKING_STATE.md) | Status: baseline-recorded

- Owner: codex
- Branch: GridDesignWork
- Date: 2026-08-10
- Experiment: Species ecosystem balance, iteration 1

## Purpose

Record the first observed balance problems before changing the rules. This is the
control description for the next A/B comparison, so future tuning can be judged
against the same symptoms instead of memory or a single attractive run.

## Observed baseline issues

1. Plants begin with a large population but wilt/die too quickly.
2. Herbivores are seeded too heavily and later form groups that are too large.
3. Carnivores starve too often and fail to keep herbivore numbers in check.
4. Plant population can deplete completely while herbivores survive too long
   without finding food.

## Working hypotheses

- Plant loss is currently driven by a high effective wilt rate relative to plant
  reproduction and available empty neighbors.
- Herbivore movement and reproduction allow local groups to remain too dense once
  food becomes scarce.
- Carnivore starting energy, prey encounter rate, and mate requirement are too
  restrictive together, causing extinction before meaningful reproduction.
- Herbivore starvation is not sufficiently coupled to food availability or search
  behavior, allowing long survival after plants disappear.

These are hypotheses, not confirmed causes. The next iteration should change as
few independent variables as possible so the result remains interpretable.

## A/B protocol

- **Control (A):** current checked-out rules, same seed, grid dimensions, run
  duration, and simulation step interval.
- **Variant (B):** one focused rule adjustment, with all other settings held
  constant.
- Run at least five fixed seeds per side and record the same checkpoints at 25%,
  50%, 75%, and 100% of the run.
- Record population counts for plants, herbivores, carnivores, empty cells,
  extinct species, and the player's species.
- Also record first extinction tick, peak population, final population, and the
  number of successful feeding events where available.
- Keep the seed and rule values in the experiment note so a regression can be
  reproduced exactly.

## First measurements to add or capture

- Population counts per species at each checkpoint.
- Plant births, wilts, and remaining empty-tile count.
- Herbivore starvation deaths and successful plant feeds.
- Carnivore starvation deaths, successful herbivore feeds, and reproductions.
- Largest local same-species group observed.

## Next experiment candidate

Start with plant persistence and herbivore food pressure, because those issues
propagate into carnivore survival. Avoid changing seeding, wilt, reproduction,
and energy values simultaneously. Compare one plant-persistence change first,
then a separate herbivore-search/starvation change, before tuning carnivore
parameters.

## Current implementation context

- `SpeciesRules` now supports reproduction food requirements, local group caps,
  starting energy, and wilt chance.
- `SpeciesSimulation` supports feeding, starvation, plant wilting, mate-seeking,
  and lower-crowding movement.
- `SpeciesSimulationPreview` seeds with a plant-heavy, herbivore-present,
  carnivore-sparse distribution and a local clump penalty.

## Validation

- `dotnet build LearningIndieDev.slnx --no-restore`: succeeded with 0 errors.
- No balance values were changed while creating this baseline note.
- Unity batch test execution remains unavailable in this environment because the
  editor exits on its cache database initialization before running tests.

## Next useful step

Add a lightweight deterministic population telemetry snapshot to the simulation
run, then run the control and first variant across fixed seeds and append results
as new experiment notes rather than overwriting this baseline.
