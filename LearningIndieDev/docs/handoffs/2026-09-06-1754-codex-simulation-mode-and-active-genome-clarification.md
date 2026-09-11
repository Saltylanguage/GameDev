# Simulation modes and active Genome clarification

**Date:** 2026-09-06  
**Owner:** Josh  
**Work type:** Product direction, balance guidance, and planning alignment  
**Implementation changed:** No

## Outcome

The upgrade direction now distinguishes permanent Genome progress from the
Genome effects used in a particular simulation:

- purchasing a Genome node unlocks it permanently;
- the player can turn unlocked nodes on or off between simulations;
- the active Genome is frozen at launch and applies to every population of that
  species, including background populations;
- Species Simulations include temporary Mutations for the selected species;
- Biome Simulations never include Mutations;
- the two modes use different success measures.

An active-capacity or point budget is the leading configuration model, but its
size, node costs, growth, and any reallocation fee remain open. The current
recommendation is to allow reallocation between simulations and freeze the
choice after launch.

Capacity cost measures how large or flexible the species-level change is. It is
not a reward or penalty for helping or harming the biome; Biome impact is judged
separately.

## Balance consequence

Focal-species success is a valid Species-Simulation goal. It must not be used as
the sole measure of Genome or Biome health. A Genome node may help its species
while destabilizing a biome, and a useful Biome configuration may restrain one
species. Both can be intended outcomes.

Balance work therefore uses separate scorecards:

- Mutations: Species-Simulation goals and declared local interactions;
- Genomes: both focal-species and Biome effects;
- Biome configurations: scenario-specific diversity, stability, balance,
  recovery, and persistence of intended relationships.

Permanent unlocks remain earned when an active configuration performs poorly.

## Accepted concerns

The user accepted and clarified two Extreme planning concerns on 2026-09-06:

- `UPG-C07`: active Genome application must follow stable species identity, not
  player selection or broad ecological role;
- `UPG-C08`: Species and Biome goals must not be collapsed into one score.

Both are recorded as **Acknowledged** in
[`../Planning Concerns/upgrade-system.md`](../Planning%20Concerns/upgrade-system.md).

## Possible Biome progression

Biome projects are now recorded as a possible separate progression track.
These would change habitat conditions—such as water retention, corridors,
nesting space, soil recovery, or ecological niches—rather than act as hidden
species buffs. A limited restoration baseline could gate advanced Biome
challenges, but this is not yet an approved requirement.

## Main documents aligned

- [`../Studio Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md`](../Studio%20Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md)
- [`../UPGRADE_SYSTEM_DIRECTION.md`](../UPGRADE_SYSTEM_DIRECTION.md)
- [`../PROJECT_CONTEXT.md`](../PROJECT_CONTEXT.md)
- [`../PRODUCT_BRIEF.md`](../PRODUCT_BRIEF.md)
- [`../SCIENTIFIC_DATA_ECONOMY.md`](../SCIENTIFIC_DATA_ECONOMY.md)
- [`../../ROADMAP.md`](../../ROADMAP.md)

Supporting UI, architecture, technical-template, future-roadmap, and working
state documents were aligned with the same boundaries.

## Decisions still needed

- Exact active Genome capacity and how nodes consume it.
- Whether reallocation is free between simulations.
- Whether players can save named configurations per species or per mode.
- Whether Biome projects become a real progression track.
- Whether a small habitat-restoration baseline gates advanced Biome challenges.
- Final production treatment of Skip in the nine-Mutation Species Simulation.
