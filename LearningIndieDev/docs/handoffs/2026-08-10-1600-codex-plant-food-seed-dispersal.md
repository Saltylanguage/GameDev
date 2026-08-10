# plant food and seed dispersal trial

[Working state](../WORKING_STATE.md) | [Previous trial](2026-08-10-1432-codex-species-balance-trial-01.md) | Status: trial-ready

- Owner: codex
- Branch: GridDesignWork
- Date: 2026-08-10
- Trial: Fractional plant food reserves and herbivore seed dispersal

## Purpose

Extend the balance trial so plants support multiple herbivore meals instead of
being destroyed by the first feeding, while giving fed herbivores a limited way
to replenish the plant population.

## Changes

- Plants now start with `3.25` food reserve units.
- A feeding consumes up to one food unit from the plant and gives that amount to
  the herbivore's current food reserve.
- A plant remains in place while it has food remaining, allowing multiple
  herbivores to feed from it.
- A partial final meal is preserved as a fractional reserve rather than being
  rounded away.
- Herbivores with a positive food reserve have a `0.05` chance per tick to drop a
  seed into an adjacent empty tile.
- Dropped and reproduced plants use the configured plant starting food reserve.
- `FoodEaten` remains lifetime feeding telemetry; `FoodReserve` is now the
  consumable resource used for reproduction and partial meals.

## Expected outcome

- Plants should support roughly 3.25 herbivore reproduction-food units before
  being consumed.
- Herbivore populations should have a food-dependent recovery path after local
  plant depletion.
- Seed drops should create spatially separated plant patches instead of relying
  only on adjacent plant reproduction.
- Plant persistence should improve without making the plant population
  permanently dominant.

## Measurement

Compare this trial against Trial 01 using the same fixed seeds and checkpoints.
In addition to population snapshots, record:

- Average meals per plant before depletion.
- Fractional meals and remaining plant reserves.
- Seed-drop count and distance from the parent herbivore.
- Plant births from reproduction versus herbivore seed drops.
- Herbivore starvation and extinction timing.

## Scratchpad update

The shared design scratchpad is now available at
[`docs/SPECIES_IDEAS_SCRATCHPAD.md`](../SPECIES_IDEAS_SCRATCHPAD.md). The first
recorded idea is the alpha-offspring upgrade: special dietary or behavioral
requirements produce an alpha child with significant stat bonuses.

## Validation

- `dotnet build LearningIndieDev.slnx --no-restore`: succeeded with 0 errors and
  the existing `DelegateCommand` warning.
- `git diff --check`: passed.
- Fixed-seed gameplay results are pending Unity playtesting; this note records
  the variant and expected outcomes without claiming balance success.

## Next useful step

Run the same seed set used for Trial 01, then append observed curves and event
counts to a results note. If plants still collapse, adjust wilt and seed-drop
rates separately rather than changing the food reserve again immediately.
