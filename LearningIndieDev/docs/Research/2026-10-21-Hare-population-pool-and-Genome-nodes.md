# Hare population pool and starting-colony Genomes

**Owner:** Josh  
**Date:** October 21, 2026, America/Toronto  
**Status:** Implemented and verified locally as playtest content

## Accepted design

Josh clarified the interpretation of the
[feeding/Fox comparison](2026-10-21-Feeding-and-Fox-introduction-results.md):
unchecked Hares must be able to exhaust the food supply. Food exhaustion and
subsequent starvation are intended potential failure outcomes. The balance
goal is understandable consequences and useful player choices. Uncontrolled
population growth does not need to sustain itself indefinitely.

This supersedes the earlier analysis's suggestion that every food-collapse
pattern necessarily requires stronger natural population controls or renewable
food. The raw observations and the historical analysis remain unchanged.
Review food access for correctness and player agency; do not automatically
add food, restrain births or rescue a population because the report shows
exhaustion. No new instant failure on zero Plants is introduced here.

Josh's latest values replace the earlier provisional discussion of a 20-Hare
carry-forward cap:

| Rule | Value and timing |
| --- | --- |
| Starting-population Genome | +5 Hares when a new journey world is seeded |
| Base player population pool | 50 |
| Population-pool Genome | +10 capacity per active node; multiple nodes add |
| Ordinary refresh/population grants | Restore only up to available pool space |
| Special rules | May explicitly bypass the pool limit; physical placement rules still apply |

The pool is the player's replenishable population/HP allowance. It does not
cull survivors at a Mutation boundary, does not refill the population for free,
and does not cap natural births. A population above the replenishment limit
remains alive, can consume food and can starve. Ordinary grants cannot add more
until it falls below its pool limit. Starting quantity and pool capacity are
separate investments; +5 starting Hares does not raise capacity.

With a scenario start of 20 Hares, the starting node seeds 25. Without a
capacity node the pool is 50; with one it is 60; with two it is 70. These are
provisional values for continued design and playtesting.

## Existing Gene Lab POC offers

Both nodes are Warren roots in the existing explicit playtest catalog:
`Assets/Data/TestData/Genome/RabbitSelectionPOC/RabbitSelectionPOC.asset`.
They follow its current provisional convention of **1 Rabbit Data and 2
allocation points** each. Production catalog and final economy remain separate.

| Name | Stable node ID | Effect |
| --- | --- | --- |
| Founding Hares | `poc.hare.starting-population` | `population.starting` +5 |
| Population Reserves | `poc.hare.population-pool` | `population.pool-capacity` +10 |

Existing license, unlock gate, eight-point allocation, profile save/load and
reset/refund behavior apply. Both effects are frozen with the active Genome
before launch. A profile or authored-value change during a run cannot change
that run's capacity. Starting bonuses compose from the scenario's natural
starting counts once per new world preparation, never from the already-bonused
previous count and never as a refill between rounds.

## Runtime contract

The stable species attribute registry v7 distinguishes population modifiers
from animal stat modifiers. Genome nodes can contain them; Mutation stat
snapshots reject them. The normal species rules remain unchanged by these
population-only modifiers. The active Genome still applies by explicit species
identity, including starting-population bonuses for background species.

Starting bonuses currently require an explicit authored/overridden starting
count for their species. Unsupported probability-only starts fail validation
instead of inventing a base count. Resulting counts retain existing nonnegative,
grid-capacity and scenario-cap validation.

`SimulationRunState` owns the frozen player pool capacity and current space.
The runner's population-addition operation clamps ordinary player grants to:

```text
allowed = min(requested, max(0, pool capacity - living player population))
```

At full capacity it returns false without a charge or population change.
Partial fills expose the actual count through the `out added` overload and
record that count in boundary accounting. Other species' additions are
independent of the player's pool. `ignorePopulationPool: true` is the explicit
special-rule override; it does not bypass placement, phase or scenario limits.
Manager/helper wrappers expose the same operation for future refresh/event
implementations. No new random-event content or refresh node was created.

The existing Buy Hare and population-reinforcement paths use this gate. The
purchase control disables at the limit and displays `POPULATION POOL FULL` with
the maximum. Headless purchase policy audits report `population-pool-full`
when capacity stops them.

Report schema 40 adds `populationPoolCapacity` and per-species
`boundaryPopulationAdded` totals to each run. Those totals include all actual
boundary grants, including separately audited Fox introductions/purchases;
do not add those sources twice. Genome input already preserves ordered node
modifiers and their values. Portable snapshots retain these same nodes and
run outputs; old raw schema-39 experiment reports stay unchanged.

## Verification

- **112/112** focused EditMode checks passed:
  `artifacts/unity-tests-20261021-143501/EditMode-results.xml`.
- Desktop interaction **1/1 at 720p** and **1/1 at 1080p** passed:
  `artifacts/visual-evidence-20261021-143253/PlayMode-results.xml` and
  `artifacts/visual-evidence-20261021-143348/PlayMode-results.xml`.
- Four captures were inspected: Gene Lab and visible full-pool controls in
  `artifacts/population-genome-visuals-720p/` and
  `artifacts/population-genome-visuals-1080p/`.
- Standalone build: **zero warnings/errors**. Portable regression self-test
  passed with the final shared-source build.
- The [verification receipt](../../artifacts/population-genome-verification-20261021-01/verification.json)
  retains test/capture paths and hashes, source hashes and the commands.

Coverage includes registry scope, multiple additive capacity nodes, launch-once
semantics, frozen values, checkpoint restore, partial/full refills, above-cap
continuation, explicit exceptions, portable report fields and actual Gene Lab
purchase/save/load/launch/refill behavior. At a full pool a rejected purchase
leaves currency unchanged. This is feature verification, not a new balance
experiment or proof of an engaging failure loop.

## Remaining design work

- Final names, tree placement, prices and capacity costs.
- Concrete refresh/random-event content and any event-specific exceptions.
- Player-readable food-pressure feedback and choices that make depletion feel
  justified, plus human playtesting of those decisions.
- Balance evidence for these new population nodes under a separately approved
  scope. No additional ecology panel was run in this work.
