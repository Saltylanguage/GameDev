# Gene Lab and Journey Upgrade Inventory

**Status:** exploratory inventory, checked November 1, 2026. This records current authored systems and unresolved design; it does not approve new options, costs, balance values, or shop rules.

**Scope note (November 1):** Bevin asked to keep this review focused on Gene Lab, journey upgrades, population controls, and Field Data shop purchases. Broader terrain and Owl design is parked; do not expand it during this walkthrough unless asked. The current three-simulation/two-shop Field Data route is proof of concept. The anticipated real journey is much longer, probably 7-10 simulation nodes; that range is provisional, not a finalized route contract.

## Decisions confirmed in the walkthrough

**Genome capacity rule:** all equipped Gene Lab nodes cost 1 capacity point for now, regardless of effect or selected rank. Maximum active capacity stays 8. This supersedes the earlier authored 2-point costs and walkthrough discussion of 2 points for Population Reserves and Founding Hares. Existing authored 2-point Genome assets and the FD2 catalog constant have now been changed to 1; broader catalog semantics remain in progress.

| Item | Gene Lab | Journey | Field Data shop |
|---|---|---|---|
| Tough Hide | Research unlocks the ability for journey offers; no permanent +Block stat | Keep Tough Hide as a temporary Mutation (+2 Block per level in current authored data) | Remove Reinforced Hide as a duplicate stat purchase |
| Heightened Senses | One research unlocks both sensory adaptations | Long Vision extends general vision range; Keen Senses extends predator-detection range. They are related choices, not synonyms or prerequisites. | No matching shop item currently |
| Long Stride | Research unlocks the journey offer; no permanent movement bonus | Keep as temporary Mutation and use the shared name Long Stride | Remove signed Long Stride offer as a duplicate speed purchase |
| Efficient Digestion | Research unlocks the journey offer; no permanent digestion bonus (confirmed as current direction, “for now”) | Keep as a temporary Mutation | Remove Deep Digestion as a duplicate modifier |
| Crowding Tolerance | Research unlocks the journey offer; no permanent crowding bonus | Keep temporary Mutation; reduce extra crowding energy cost, not area capacity | Remove Warren Comfort as a duplicate modifier |
| Reproductive Drive | Research unlocks the journey offer; no permanent reproduction-chance bonus | Keep temporary Mutation for chance of successful reproduction | No matching shop item; keep distinct from litter size and gestation |
| Seed Dispersal | Sole research unlock for the seed-planting journey Mutation; no permanent seed-drop chance | Keep temporary Mutation; a successful seed-drop spends 1 stored food. Fertile Droppings is merged here. | Remove Abundant Seedfall as a duplicate seed-chance modifier |
| Fertile Droppings | Merge into the single Seed Dispersal research unlock; no second Gene Lab node | Fold seed-planting behavior into Seed Dispersal; defer the separate scent-marker effect | No separate purchase |
| Faster Gestation | Research unlocks the journey offer; no permanent gestation reduction | Keep as a temporary Mutation; test catalog currently reduces gestation 15% per level, up to 3 | No matching shop item; distinct from reproduction chance and litter size |
| Increased Minimum Litter | Research unlocks a journey Mutation; no permanent litter-size change | Add to regular Hare Mutation choices; +1 minimum litter as a starting test effect | No matching shop item; distinct from maximum litter and reproduction chance |
| Larger Litters | Research unlocks a journey Mutation; no permanent litter-size change | Three journey ranks add +1 maximum litter per rank; rank 3 raises minimum litter to at least 2 | No matching shop item; distinct from minimum litter |
| Population Reserves | One Gene Lab node, researched through rank 1-5 at costs 5/10/15/20/25 Hare Data (75 total) | Equip once for 1 Genome capacity point at any rank. Per simulation node, base allowance is 10 Hare purchases; each rank adds 5, so rank 3 allows 25 and rank 5 allows 35. Limits purchased Hares only, not natural births or survivor carryover; resets each node. | Applies to paid Hare additions during the node; replaces replenishment-capacity increase |
| Founding Hares | Separate starter gene; +5 Hares starting value; price 5 Hare Data | Adds Hares at the start of a new journey; not a journey Mutation | No matching shop purchase; equip cost 1 capacity point |
| Escape Instinct | Research unlocks the journey offer; no permanent avoidance bonus (current direction, “for now”) | Player-facing name for the existing Threat Avoidance Mutation; +0.08 pre-contact avoidance per level in current data | Remove Sharper Instinct as a duplicate avoidance modifier; do not create a second option |
| Burrowing / Born Burrower | Burrowing research unlocks the journey Mutation; it is a prerequisite for the separate Born Burrower starter gene | Burrowing lets a Hare hide with its children in a burrow. Each burrow lasts 200 steps and starts with a maximum capacity of 3. Born Burrower, when active, grants Burrowing at journey launch and excludes Open-ground Sprint. | Capacity expansion and allowing unrelated Hares are future upgrade directions; detailed rules TBD |
| Shared Burrows | Separate research unlock for a journey Mutation | When acquired, lets unrelated Hares use an eligible burrow with capacity for that journey | Future upgrade; name and capacity interactions TBD |
| Expanded Burrows | Separate research unlock for a journey Mutation | Five ranks; each rank adds +1 capacity to every burrow created during that journey, from base 3 up to 8 | Future upgrade; rank acquisition cadence and research price TBD |
| Open-ground Sprint | Research unlocks the journey offer; price TBD | Temporary Mutation: stronger movement on exposed/open terrain; no bonus under cover or underground. Active Born Burrower blocks it for that journey. Exact value and terrain definition TBD. | No shop version; distinct from general Long Stride; depends on expanded terrain interactions |
| +1 Field Data per Hare | No Gene Lab counterpart | One-time-per-journey shop purchase at provisional 20 FD; adds +1 FD per Hare only to payouts calculated after purchase | Shop only; first-shop placement remains provisional |
| Population payout multiplier | No Gene Lab counterpart | Three sequential shop tiers, each purchasable once per journey: 1.5x -> 1.75x -> 2x; each replaces the prior multiplier, capped at 2x | Provisional prices 30/40/50 FD; unlock early, middle, and late in the longer journey; exact nodes TBD |
| Add 1 Hare | No Gene Lab counterpart | Keep as paid in-journey action at 10 FD; every Hare bought counts cumulatively toward the simulation node allowance, even if later removed | Existing round-boundary population control |
| Remove 1 Hare | No Gene Lab counterpart | Keep as paid in-journey control at current test price 10 FD; does not count toward purchase allowance | Population control, not an upgrade |
| Add/remove 1 Fox | No Gene Lab counterpart | Keep existing +/-1 controls at 10 FD each for now | Population controls; refine later |
| Add/remove 5 Plants | No Gene Lab counterpart | Keep existing +/-5 controls at 10 FD each for now | Population controls; refine later |

## The three layers

| Layer | What the player gets | When it applies | Current status |
|---|---|---|---|
| Gene Lab / Genome | Permanent ownership or access to a species capability, plus an active pre-journey configuration in the FD2 implementation | Frozen for a journey; affects that species across its populations | The FD2 Hare catalog is a five-node stat map. The richer production test catalog and locked concepts are playtest/design material, not the same catalog. |
| Journey Mutation | A free, temporary Hare adaptation selected from a reward offer | Acquired during the current journey and carried through its later phases; cleared when the journey ends | The six-round Mutation flow has five later choice moments; offer pools vary by route/contract. |
| Field Data shop | A paid route/economy or signed species modifier offer | Current Field Data journey, subject to the authored shop route and offer rules | The starter definition is test/playtest content; its signed values are provisional. |

Ordinary round reward choices and Field Data shop purchases are separate systems. Population controls are paid interventions, not genes or species upgrades.

## Gene Lab / permanent Genome inventory

### FD2 first Hare catalog

This is the catalog used by `FieldDataPermanentProgressionDefinition.CreateStarter()` and the FD2 Gene Lab flow.

| Node | Active effect | Prerequisite | Price / capacity |
|---|---|---|---|
| Tough Hide | +1 Block (current test map; being replaced by unlock direction) | None | 5 Hare Data / 1 capacity |
| Long Vision | +2 vision range (current test map; being replaced by unlock direction) | Tough Hide | 5 / 1 |
| Long Stride | +0.25 movement speed (current test map; being replaced by unlock direction) | Tough Hide | 5 / 1 |
| Efficient Digestion | +0.25 digestion energy bonus (current test map; being replaced by unlock direction) | Long Vision and Long Stride | 5 / 1 |
| Population Reserves | +10 ordinary replenishment capacity (current test map; being replaced by per-node purchase allowance) | Tough Hide and Efficient Digestion | 5 / 1 |

FD2 allows 8 active capacity points. This catalog models permanent owned nodes and a saved active loadout. The current effect list is the existing implementation baseline, not the intended Gene Lab fantasy: the approved direction is capability unlocks rather than small permanent flat-stat gains. Population Reserves' +10 refill-capacity effect is to be replaced by a per-simulation-node purchased-Hare allowance; this behavior is not implemented yet.

### Broader Hare Genome test assets and concepts

There are multiple catalogs here, which must not be collapsed into one product list. `HareGenome.asset` contains 10 node assets, including the older stat/lifecycle nodes plus Faster Gestation, Fertile Droppings, High Crowding, and Increased Minimum Litter. The separate `RabbitSelectionPOC.asset` has 12 references (11 selectable and one non-activatable Burrows preview). Its selectable set includes Long Stride, Long Vision, Tough Hide, Efficient Digestion, Larger Litters (+1 maximum litter), Founding Hares (+5 Hares at journey start), Population Reserves (currently +10 replenishment capacity; proposed replacement is the purchased-Hare allowance above), Faster Gestation (up to three ranks, each -15% gestation), High Crowding (+0.1 crowding-energy reduction), and Increased Minimum Litter (+1 minimum litter, maximum unchanged). These are POC/test catalog contents and provisional values, not proof that each is purchasable in the player-facing Gene Lab.

The November 1 handoff explicitly says Faster Gestation, Fertile Droppings, High Crowding, and Increased Minimum Litter remain locked concepts in the permanent Gene Lab view. Their permanent Gene Lab prices and loadout rules are undecided. Fertile Droppings depends on the unresolved food model. Avoidance still lacks a complete Genome-to-simulation input path. Keep UI concept status, test catalog availability, and implemented permanent progression as three separate facts.

## Temporary journey Mutation inventory

Current Hare pools are not identical across route contracts:

| Mutation | Current effect per level | Player-facing meaning |
|---|---|---|
| Faster Movement | +0.5 movement speed | Escape predators and reach fresh food |
| Tough Hide | +2 Block | Block more incoming attacks |
| Efficient Digestion | +0.1 digestion energy bonus | Gain more energy from food |
| Seed Dispersal | +0.01 seed-drop chance; spends 1 stored food on success | Plant food during the run |
| Crowding Tolerance | +0.1 crowding-energy reduction | Reduce extra crowding cost |
| Reproductive Drive | +0.005 reproduction chance | Make successful reproduction more likely |
| Threat Exposure / Threat Avoidance | +0.08 pre-contact avoidance chance | Evade predator attacks more often |
| Keen Senses | +1 predator-detection tile per rank (provisional) | Detect predators from farther away; currently in the Field Data ordinary reward pool |

The Field Data ordinary reward pool contains Tough Hide, Efficient Digestion, Seed Dispersal, Crowding Tolerance, Reproductive Drive, and Keen Senses. The phase-based Mutation pool contains Tough Hide, Efficient Digestion, Crowding Tolerance, Reproductive Drive, Threat Exposure, Faster Movement, and Seed Dispersal. The opening Forest Edge route offers Faster Movement, Tough Hide, or Efficient Digestion before its first cycle. Many repeatable skills currently allow up to ten levels; actual offer generation and cap eligibility depend on route state.

## Field Data shop inventory

The authored `FieldDataStarterDefinition` is a five-node route: simulation, shop, simulation, shop, simulation.

### Paid Hare modifiers

Each signed offer costs 30 Field Data and targets Hare. Both positive and negative variants exist in the test definition; this is not evidence that every shop displays every variant.

| Offer family | Positive / negative value | Closest permanent or Mutation overlap |
|---|---|---|
| Long Stride | +/-1.5 movement speed | Genome Long Stride (+0.25); Mutation Faster Movement (+0.5/level) |
| Sharper Instinct | +/-0.24 pre-contact avoidance chance | Mutation Threat Exposure (+0.08/level); Genome path not wired |
| Reinforced Hide | +/-6 Block | Genome Tough Hide (+1); Mutation Tough Hide (+2/level) |
| Warren Comfort | +/-0.3 crowding energy reduction | Genome High Crowding (+0.1 concept); Mutation Crowding Tolerance (+0.1/level) |
| Deep Digestion | +/-0.3 digestion energy bonus | Genome Efficient Digestion (+0.25); Mutation Efficient Digestion (+0.1/level) |
| Abundant Seedfall | +/-0.03 seed-drop chance | Mutation Seed Dispersal (+0.01/level, with food cost); not identical behavior |

These signed amounts are provisional playtest data. “Same fantasy” does not mean interchangeable rules: the price, duration, stacking, and presentation need to make the difference legible.

### Paid income offers

| Offer | Cost | Earliest shop |
|---|---:|---:|
| +1 Field Data per Hare | 20 | 1 |
| x1.5 population payout (tier 1) | 30 (provisional) | Early journey, exact node TBD |
| x1.75 population payout (tier 2) | 40 (provisional) | Mid journey, exact node TBD |
| x2 population payout (tier 3) | 50 (provisional) | Late journey, exact node TBD |

The three payout tiers were confirmed as the desired shop progression. The current short route is only proof of concept and does not constrain the longer target journey; the tiers should unlock early, middle, and late across a probable 7-10 simulation nodes. Exact nodes remain to define. Prices are provisional, not balance-validated.

### Paid population interventions

Each currently costs 10 Field Data: add/remove 1 Hare, add/remove 1 Fox, or add/remove 5 Plants. These change the route state; they are not persistent upgrades.

Bevin confirmed on November 1 that all current population controls stay as authored for now, with one exception: +1 Hare purchases count against the Population Reserves allowance. Further population-control refinement is deferred.

### Burrowing ability detail

Bevin defined the Burrowing core effect on November 1, 2026: a Hare can hide in a burrow, and its descendants can hide there too. A baby counts as part of the burrow family if either parent is already in that family. Burrows last 200 simulation steps and start with a maximum capacity of 3, including the Hare that dug it (space for up to two descendants). A Hare creates a burrow automatically only when all 8 surrounding cells are empty of creatures/species; creation costs 24 energy (current max Hare energy is 48). Eligible descendants at the burrow or in an adjacent cell automatically hide when a Fox is within attack range if capacity is available; they emerge when the threat passes or the burrow collapses. Future upgrades may allow unrelated Hares to use a burrow or increase its capacity. Current simulation cells do not persist parentage/ancestry after birth, so implementing descendant access requires adding and preserving lineage data. Exact expiry behavior remains open. The effect is design direction, not implemented behavior.

### Terrain and cover dependency

Bevin wants Open-ground Sprint and future environmental features defined against terrain interactions such as bushes, trees, rocks, and water. The current game does not have an exposed-versus-covered terrain contract. Terrain-specific cover, movement, blocking, and water interactions remain to define before Sprint can have a precise trigger. No behavior is inferred for any named terrain feature yet.

**Bush (rules defined so far):** a bush is a tile prey-role creatures can pass through and predator-role creatures cannot. A prey creature on a bush tile is hidden from predator sight. Predators may pursue its last seen location briefly, but cannot enter or attack prey hidden in the bush. The attack restriction is the starting rule and may change with future Fox upgrades. The duration of pursuit remains open. Open-ground Sprint should account for bushes when its terrain trigger is finalized.

**Trees and rocks (first rule defined):** these are solid obstacle tiles that neither Hares nor Foxes can occupy. Future Owls are expected to take advantage of trees, but their tree interaction is not yet defined. Other species' traversal and sight interactions remain open.

## Overlap and differentiation to resolve

| Theme | Current options that sound related | Design distinction to make explicit |
|---|---|---|
| Movement / escape | Long Stride, Faster Movement, shop Long Stride | Permanent baseline configuration vs temporary level growth vs a paid large signed swing |
| Defense | Tough Hide, Reinforced Hide | Block value, duration, and whether a paid negative bargain is a deliberate tradeoff |
| Feeding | Efficient Digestion, Deep Digestion | Persistent food-energy baseline vs run adaptation vs paid route effect |
| Crowding | High Crowding, Crowding Tolerance, Warren Comfort | Exact same stat family today, but different acquisition and lifetimes |
| Plants / food | Seed Dispersal (including the merged Fertile Droppings concept), Abundant Seedfall | Use one Gene Lab unlock and one temporary Mutation for the seed-planting capability; remove the duplicate shop modifier. Defer the separate scent-marker effect. |
| Reproduction | Increased Minimum Litter, Larger Litters, Reproductive Drive, Faster Gestation | Minimum floor, maximum ceiling, chance of success, and time between litters are distinct levers |
| Senses / danger | Long Vision, Keen Senses, Threat Exposure, Sharper Instinct | General vision range, predator-detection range, and pre-contact avoidance are separate capabilities |
| Population | Population Reserves, Founding Hares, population interventions | Refill limit, launch population, and paid encounter population change must not read as the same effect |

## Next design pass

For every candidate option, fill in: player promise; exact trigger and effect; target species/populations; acquisition source; when it becomes active; duration/reset; currency and price; rank/cap; prerequisites; stacking; exclusions/tradeoffs; visible feedback; and what a player should learn from choosing it. Then choose one canonical player-facing name per effect and explicitly label concepts, test values, and approved content.

Sources: `Assets/Scripts/Game/Species/FieldDataPermanentProgressionDefinition.cs`, `Assets/Scripts/Game/Species/SpeciesUpgrade.cs`, `Assets/Scripts/Game/Presentation/SpeciesSimulationPreview.cs`, `Assets/Data/TestData/FieldDataStarterDefinition.asset`, `docs/PROJECT_CONTEXT.md`, `docs/RABBIT_GENOME_IMPLEMENTATION_PLAN.md`, and `docs/handoffs/2026-11-01-codex-sim-weekly-handoff.md`.
