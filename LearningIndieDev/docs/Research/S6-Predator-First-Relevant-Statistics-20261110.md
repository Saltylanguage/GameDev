# S6 predator-first: relevant Hare and Fox statistics

**Purpose:** A source-backed reference for the Hare/Rabbit versus Fox opening and the statistics that should guide its balance.
**Source revision:** `feature/s6-fun-pass`, commit `652c87c9` (Nov 10, 2026).
**Terminology:** The player-facing rabbit is the simulation species with ID `hare`. There is no separate `rabbit` species in the authored Forest Edge scenario.

> **Update, November 11:** This report preserves the November 10 source snapshot.
> Fox energy values in the table below were changed for the carcass-feeding
> first pass. See the [November 11 handoff](../handoffs/2026-11-11-carcass-feeding-first-pass.md)
> for current provisional values and verification status; do not use the old
> Fox energy rows as current tuning.

## Authored Forest Edge starting point

The production [ForestEdge scenario](../../Assets/Data/ProductionData/CellularSimulation/Scenarios/ForestEdge.asset) starts with **400 Plants, 55 Hares, and 20 Foxes** on a 36×20 grid. The recent Fox-count screens overrode the Fox count per arm; they did not change the scenario asset. Under the current node rule, the Hare count carries forward, while fresh Hares are created with default individual state and Foxes/Plants reset to their fixed starting counts.

## Base species rules

Values below come from the production [Hare asset](../../Assets/Data/ProductionData/CellularSimulation/Species/hare.asset) and [Fox asset](../../Assets/Data/ProductionData/CellularSimulation/Species/fox.asset). “Effective” energy values account for percentage fields that override the older fixed values.

| Stat | Hare / player Rabbit | Fox | Why it matters here |
|---|---:|---:|---|
| Role and food target | Herbivore; eats Plant | Carnivore; targets Hare | Defines the matchup and the food loop. |
| Movement speed / directions | 2.2; 4 cardinal directions | 1.0; 8 neighboring directions | Hare has higher speed; Fox has more movement directions. Actual movement also depends on terrain cost and behavior state. |
| Vision range | 5 | 12 | Fox has the larger authored perception range. Hare detection range is a key avoidance input. |
| Attack pattern / amount | Empty pattern / 1 | 8 neighboring cells / 2 | The Hare’s attack amount is unused with no attack offsets. Fox can attack an adjacent Hare. |
| Block amount | 0 | 0 | In opposed-roll combat this is the defense modifier. The pattern does not gate the roll. |
| Starting health | 1 | 1 | Every successful base Fox hit is lethal to a base Hare: attack damage is 2. |
| Starting / maximum energy | 12 / 48 | 120 / 240 | Controls feeding and reproduction eligibility. |
| Energy loss | 1 every 8 ticks | 1 every tick | Energy pressure and starvation should be tracked separately from predation. |
| Forage threshold | 6; forage until full | Effective 180 (75% of 240); fixed field is 120 | The Fox’s percentage threshold overrides its fixed threshold. |
| Energy value when eaten | 96 | 100 | Hare energy yield affects Fox feeding after a kill. |
| Reproduction chance | 0.50 per eligible attempt | 0.45 per eligible attempt | This is not a per-tick birth probability; mate, energy, group, and cooldown rules gate attempts. |
| Litter / gestation | 2–5 / 48 ticks | 1 / 48 ticks | Sets potential replacement speed after losses. Gestation delays births after a successful attempt. |
| Mating group and pattern | Requires 1 neighbor; 8 neighboring cells; max group size 7 | Requires 1 neighbor; 4 cardinal cells; max group size 3 | Local density and spacing can block reproduction. |
| Effective mating energy threshold / cost | 24 / 24 | 60 / 36 | Calculated from 50% / 50% of Hare max energy and 25% / 15% of Fox max energy. These fractions override fixed food requirements of 16 and 48. |

### What those energy values mean at a node start

A fresh Fox starts at 120 energy, below its effective forage threshold of 180, so its rules allow it to seek food immediately. A fresh Hare starts at 12, below its 24-energy mating threshold, so it must feed before it can reproduce. If a Fox at its starting energy kills a Hare, the Hare's 96 energy value raises the Fox to 216 (below its 240 cap). The Fox then stops seeking food while it is at or above 180; at one energy lost per tick, that is a 36-energy gap before it can seek food again if nothing else changes. This is a rules-based explanation for possible hunt/recovery cycles, not a measured average interval.

## Combat meaning for the current scenario

The corrected Fox sweep uses **opposed-roll** combat. Each attack rolls two d20s and hits when `attack roll + attack modifier > block roll + block modifier`. A Fox has attack modifier 2 and a Hare has block modifier 0, giving a **57.25% hit chance per valid attack roll** (229 of 400 roll pairs). A hit applies the Fox’s 2 damage against the Hare’s 1 health, so it kills the base Hare. These odds apply after a valid contact attack; they are not the chance that a Hare dies during a whole tick or node.

For the agreed strategies, the relevant levers are:

- **Avoidance:** Hare vision/perception, movement speed and direction, Fox detection/attack pattern, and whether a hunt reaches contact. Track encounters and attacks separately from deaths. The experimental pre-contact avoidance setting can remove an interaction before combat resolution.
- **Survival:** Hare health and block amount, Fox attack amount, opposed-roll hits, damage, lethal hits, and Hare deaths by cause. A block increase changes the opposed-roll modifier; a directional block pattern alone does not change the opposed-roll result.
- **Faster reproduction:** eligible mating attempts, failures due to mate/energy/group/location, chance-roll failures, litter size, gestation completion, births, and time from losses to replacement. The authored Hare’s `0.50` chance only applies after its eligibility checks.

These are separate Mutation responses. This reference describes the current species rules and does not define Genome builds or Mutation availability.

## Statistics to use in the balance report

Keep these outcomes separate by node and seed:

1. **Population and survival:** Hare and Fox opening/closing counts; Hare extinction by node; Hare age/energy at node opening where available; Fox reset count; Plant count at each boundary.
2. **Predator pressure:** Hare predator encounters and encountered-predator steps; Fox combat opportunities, attempts, hits, blocked attacks, kills, and attack timing. Report rates per opportunity as well as totals, since a larger number of kills can come from more encounters rather than a higher hit chance.
3. **Survival and other deaths:** Hare combat, starvation, crowding, and other deaths separately; nonlethal hits and damage where applicable.
4. **Reproduction and recovery:** Hare reproduction candidates and outcomes, litter size, births, births per Hare death, and time to first birth/replacement. Include Fox births and deaths because Foxes can reproduce within a node before they reset.
5. **Food as a control:** Plant counts, Hare food-action attempts/successes, food consumed, energy loss, and starvation. Do not combine Plant exhaustion with Fox pressure in one failure number.

The current report schema already contains population trajectories, combat/encounter counts, cause-specific deaths, births, feeding outcomes, reproduction outcomes, and phase-level windows. The recent [lower-count diagnostic](Experiments/S6-Fox-Soft-Opening-20261110/REPORT.md) showed why cause separation matters: across its 12 node-one runs per arm, Fox kills / Hare starvation deaths were 1,331 / 1,396 at 8 Foxes, 1,365 / 788 at 10, and 1,080 / 413 at 12. One 10-Fox run also reached zero Plants by node-two close. These are diagnostic totals, not stable population rates.

## Implementation sources

- [Species rules and effective energy thresholds](../../Assets/Scripts/Game/Species/SpeciesRules.cs)
- [Default starting health and energy](../../Assets/Scripts/Game/Simulation/SpeciesInitialGridFactory.cs)
- [Opposed-roll resolution, movement, and contact handling](../../Assets/Scripts/Game/Simulation/SpeciesSimulation.cs)
- [Combat, feeding, reproduction, and mortality telemetry](../../Assets/Scripts/Game/Simulation/SpeciesSimulationMetrics.cs)
- [S6 design target and node population rule](../Sprints/S6-control-record.md)
