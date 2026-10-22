# Population decisions: first playable slice

Status: local implementation, October 21, 2026. These encounter quantities are
provisional playtest content, not accepted ecology balance. Josh approved building
refresh/event choices and food-pressure feedback after the population-pool work.
No new ecology experiment or Trello update is part of this change.

## What the player can do

At an uncommitted round decision, the habitat is frozen. Six repeatable controls
follow the existing Field Data design:

| Control | Cost | Consequence |
| --- | --- | --- |
| +1 Hare | 10 Field Data | More Hares and more food demand; ordinary refill cap applies |
| -1 Hare | 10 Field Data | Less food demand; cannot remove the final player individual |
| +1 Fox | 10 Field Data | More hunting pressure |
| -1 Fox | 10 Field Data | Less hunting pressure; Hare food demand can subsequently rise |
| +5 Plants | 10 Field Data | Restores food on suitable bare ground |
| -5 Plants | 10 Field Data | Reduces food supply |

These controls are unavailable during running, an ordinary pause, after committing
a Mutation, or when funds/space/targets are insufficient. A rejected action
changes no population, time, or currency. Removals choose matching individuals or
resources with a seeded draw. Only the player-selected species has a one-survivor
removal floor. Removing a Plant under an animal preserves the animal; removing an
animal on a Plant preserves the Plant. Planting never overwrites another terrain,
resource or creature. A Plant configuration with no usable food reserve/resource
is rejected instead of recording a nonexistent refill. Placement obeys the world's separate physical/global limits.

The live ledger shows current Plant count and its net change since the round
opened, with EMPTY when none remain. Round decisions label food as available,
declining, or exhausted. The decision and encounter views also show the current
Hare population, refill limit, and cumulative player starvation deaths. Counts
are explicitly described as supply observations: they do not estimate nutrients,
carrying capacity, or time until extinction. Player additions/removals affect the
net count change. An increase does not prove sustainable renewal.

## Two new choices between simulations

All three opening routes connect to the existing two habitat conditions and two
new encounters. Choosing one branch commits that path for the current journey.
Each still leads into one of the existing second simulations; later map nodes
remain placeholders.

- **Rest Stop:** choose up to 10 Hares, 10 Plants, or continue without a refresh.
  Hare restoration clamps to remaining pool capacity; its label shows the actual
  grant. Full-pool and unsuitable-space choices are disabled with a reason.
- **Seed Cache:** open a cache with disclosed 50% odds of +10 Plants and 50% odds
  of -5 Plants, or leave it alone. It costs no Field Data in this first slice.
  Both outcomes must be feasible before opening, so availability cannot reveal
  the hidden roll. If either is impossible, leaving remains available.

The quantities and odds are serialized on the node assets and frozen into plain
immutable values when a journey is prepared. The roll depends on the run seed
and stable node ID; cancelling, reopening, and taking other boundary actions
cannot reroll it. Accepting changes the live world once without advancing time
or reseeding it. The map summary reports the actual result before continuation.

## Population and failure rules

The base replenishment pool remains 50; active capacity Genome nodes add 10 each.
The separate starting Genome adds 5 only when seeding a fresh world. Natural
births can exceed the pool and above-cap survivors remain. These controls provide
agency without rescuing unchecked growth: Hares can still exhaust food and die
of starvation. Zero Plants is feedback, not a new instant-failure trigger.

## Ownership and accounting

- `PopulationIntervention` plans placement/removal without consuming random state
  or entity IDs during availability checks. `SpeciesSimulationRunner` commits it.
- `SpeciesSimulationPreview` coordinates the existing run wallet, round state,
  frozen encounter options and route transitions. The ViewModel exposes text and
  commands; XAML binds to that state.
- Accepted actions retain ordered species, signed actual amount, source, tick,
  phase and placement seed in run state and checkpoints. Report schema 41 emits
  these actions. Rejected attempts and leave-without-change choices do not add
  population-action records.
- Added totals and explicit removed totals keep whole-run stat lines reconciled.
  Paid removals do not count as combat, starvation, births or simulation deaths.
  `REM` exposes removals separately. Historical reports are unchanged.
- These new decisions affect the live world. A runner restart discards them and
  their audit entries. Replication fitness excludes accepted additions and removals
from its net population change; mortality rate denominators retain the existing
starting-plus-added-plus-births convention. The pre-existing reinforcement API retains its prior
  restart-baseline semantics; explicit special-rule pool bypass remains there.

## Verification

Verification receipts and inspected visual states are recorded in the associated
October 21 handoff. The verification uses short phases and no initial Foxes to
isolate feature behavior. It does not establish that the production scenario is
balanced, that any Genome build is viable, or that the resulting choices are fun.

## Next design work

Play the actual route and tune the encounter quantities, prices, placement rules
and decision timing from feedback. Nutrient-aware forecasts and a larger authored
event catalog remain future work. The controls expose the consequences needed to
study player agency without changing natural simulation balance values.
