# Rabbit Genome prerequisite checklist

**Status:** Implementation-readiness audit; open items are not approvals for
values, unlock rules, or sprint scope.  
**Date:** 2026-10-17; readiness refreshed 2026-10-21. **Scope:** The proposed
Trailblazer, Warren, and Gardener permanent Rabbit Genome branches (8/10/9
candidate slots including the two later population nodes).

The current POC supports eight activatable nodes plus a Burrows preview,
including seven stat/lifecycle nodes and Fertile Droppings. Selection, allocation,
profile persistence and the stat bridge work. The full 27-slot candidate inventory
still needs new mechanics, design choices and balance evidence. Use the
[remaining-work implementation plan](../RABBIT_GENOME_IMPLEMENTATION_PLAN.md) for
sequencing and the first Luna packets; this checklist records readiness.

## Status key

- `[x]` Present in the current implementation.
- `[~]` A related primitive or a narrow first slice exists; the proposed Genome
  behavior is not complete.
- `[ ]` Missing, or its rule is too unclear to implement and balance.

## Shared prerequisites

- [x] **Profile and allocation foundation.** Rabbit/Research Data, permanent
  Hare-tree access, node allocation receipts, an eight-point active capacity,
  reset/refund, and profile save/load are implemented. One production node is
  currently authored. See `Assets/Scripts/Game/Profile/Helper_ProfileSession.cs`
  and `Assets/Scripts/Game/Species/SpeciesGenomeContract.cs`.
- [x] **Freeze the allocation for a journey.** Desktop copies the profile's
  active Genome into a ready journey; the run receives that snapshot. The
  Gene Lab purchase/reload/reset and frozen-next-journey path passed the checks
  recorded in the [Gene Lab handoff](../handoffs/2026-10-17-1719-codex-fertile-droppings-gene-lab-offer.md).
- [x] **Successful-run settlement and automated verification.** The Desktop profile route records a
  successful journey once and grants 1 Rabbit Data plus 1 Research Data;
  failures and early endings pay nothing. Unity recompiled and all 10 Lab
  PlayMode tests passed on October 20. The new test completes three journeys,
  rejects duplicate settlement after reload, verifies the Gene Lab gate and
  purchase using earned Data, and recreates scene components to check storage.
  See the [automation handoff](../handoffs/2026-10-20-1205-codex-automated-journey-rewards-and-gene-lab-unlock.md)
  for its shortened fixture and evidence boundary.
- [x] **General stat-node effect bridge.** Genome assets now author ordered
  signed additions through the existing species attribute registry. Desktop
  resolves active nodes into immutable values and applies them to every
  participating species before initial populations and ordered Mutations.
  New journeys rebuild from natural rules; phase continuation preserves the
  frozen effects. Unknown attributes, unresolved stat selections, invalid rule
  bounds, and active placeholders without implemented effects are rejected.
  The October 20 checks passed: 285 runtime tests, 23 focused Genome checks,
  18 population checks, and 11 Lab PlayMode tests. See the
  [implementation handoff](../handoffs/2026-10-20-1237-codex-genome-stat-node-runtime-bridge.md).
  This completes delivery for existing stat targets; new abilities still need
  explicit simulation rules under [SG-005](../Studio%20Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md).
- [x] **Select and allocate among multiple nodes (POC).** Branch filtering,
  clickable node selection, selected/active/available/preview states, named
  prerequisite feedback, specific Data/capacity blockers, and filled capacity
  slots are implemented. Selection survives profile refreshes; allocations
  persist across session reload and scene reconstruction and apply to the
  next journey. Josh approved a separate test catalog of five stat nodes,
  existing Fertile Droppings, and a non-purchasable Burrows preview. The full
  Lab regression passes 12/12, with captures inspected at 720p and 1080p.
  See the [POC catalog and limits](2026-10-20-Gene-Lab-selection-POC.md) and
  [handoff](../handoffs/2026-10-20-2032-codex-gene-lab-node-selection-poc.md).
  This closes selection plumbing; it does not promote the test content or
  establish the rest of the production tree/economy.
- [ ] **Set the remaining production economy and unlock contract.** Multi-node
  reset/refund and the eight-point budget are implemented. The POC's 1 Rabbit
  Data offers and one-point Fertile / two-point stat costs remain provisional.
  Final prices, point costs, production prerequisites, branch access and any
  milestone conditions for the remaining content are open. The “100 rabbits
  unlock Burrow” example remains hypothetical. Replacing POC IDs requires an
  explicit saved-allocation migration or reset plan.
- [x] **Make implemented Genome builds reproducible in balance runs.** The
  public command accepts an explicit Rabbit map and an ordered allocation,
  validates prerequisites/availability/capacity/bounds, applies Genome before
  Mutations, and records resolved values and versioned provenance. Generic
  phase schedules and Mutation policies retain the snapshot. Portable exports
  freeze the catalog and reproduce the selected build. Focused checks pass
  28/28; both technical-smoke seeds pass Unity/portable parity. Paired-lockstep
  diagnostics remain excluded, and this is not a full Desktop route export.
  See the [delivery handoff](../handoffs/2026-10-20-2245-codex-multi-node-genome-research-runner.md).
- [x] **Starting population and refill capacity.** +5 starting Hares apply once
  per fresh journey. The pool is 50 plus 10 per active capacity node and limits
  ordinary replenishment; births may exceed it. Frozen snapshots, profile saves,
  checkpoints and actual grant counts are covered by the October 21 work.
- [~] **Player agency and observed food feedback.** Six paid population controls,
  Plant-count trends, Rest Stop and Seed Cache are implemented locally. The
  nonterminal round-6 result still needs paid controls (RG-01), and the complete
  Field Data shop/session route remains an integration dependency. Raw population
  deltas do not reproduce the full journey wallet and decision sequence.
- [~] **Complete portable provenance.** The batch project includes the new
  PopulationIntervention source, but the portable source-fingerprint manifest
  omits it. RG-00 fixes this concrete gap before relying on further frozen runs.
- [ ] **Add evidence for new abilities as they are implemented.** Existing
  seed-drop/scent counters do not establish trigger denominators for burrows,
  migration, plant preference, patrols or future mechanics. Those belong with
  each new mechanic, rather than with the completed stat-build input plumbing.
- [ ] **Establish the S5 ecology reference before tuning the trees.** Agree the
  actual route, starting conditions, health measures, and review bands. The
  600/40/25 research panels in the forecast are historical references, not the
  accepted S5 baseline.

## Trailblazer

Long Stride and Long Vision are authored, selectable POC Genome nodes and use
the runtime bridge. Other stat targets exist but need accepted node definitions;
signature behaviors still need their own mechanics.

- [x] **Faster movement (POC):** Long Stride applies +0.25 movement speed.
  Per-move energy charging remains deferred; decide its intended cost before
  final balance, without treating that decision as missing stat delivery.
- [~] **Avoidance:** Hares can detect visible threats and flee; flee-speed
  support also exists. Specify the added Genome effect and make its benefit
  measurable beyond the existing baseline response.
- [~] **Fast gestation:** gestation timers, the gestation-tick stat and the
  permanent stat bridge exist. Approve and author a node; test safe bounds and
  stacking. No additional delivery framework is needed.
- [x] **Long vision (POC):** Long Vision applies +2 sight range through the
  permanent bridge. Its provisional value still needs balance review alongside
  the separate 10-tick scent sample.
- [~] **High crowding:** local crowding, tolerance, and energy penalties exist.
  Define which aspect “high crowding” changes (tolerance, cost, or both) and
  how its effect combines with Warren's crowding bonus.
- [ ] **Wanderlust / migration:** add the proposed map chunks, density measure,
  threshold accumulation/roll, destination and failed-migration behavior. The
  existing local dispersal when crowded is not chunk-based migration.
- [ ] **2× interactions:** define which actions count (foraging, mating,
  combat, or another interaction), what is doubled, and its opportunity/energy
  cost; no interaction multiplier exists.
- [ ] **Skip a node:** clarify whether this skips a journey-map node or a Genome
  tree node, then specify path/prerequisite behavior and cost. No skip action
  exists in the relevant flow.

## Warren

- [ ] **Burrows:** implement a persistent shelter with occupancy and access
  rules, construction/maintenance cost, safe mating/hiding behavior, a defined
  crowding tradeoff, and Fox counterplay. No burrow entity or terrain exists.
- [x] **Tough hide (POC):** the active node applies +1 block through the
  permanent bridge. Final value/counterplay remain balance work.
- [~] **Good scent:** scent sampling every 10 ticks and Hare trail tracking
  after a Fox sniffs Fertile Droppings exist. This only covers the current
  predator-to-prey tracking behavior; define what Hare scent should detect
  (food, mates, threats) and implement the relevant Hare perception.
- [~] **Increased minimum litter:** bounds, reproduction rules and the bridge
  exist; a node definition/value is still needed. Test every legal composition
  so minimum never exceeds maximum.
- [x] **Increased maximum litter (POC):** Larger Litters applies +1 maximum.
  Its final price/value and interactions with minimum litter remain open.
- [x] **Starting population / pool capacity (POC):** Founding Hares and Population
  Reserves implement the accepted +5 start / +10 refill-limit effects.
- [ ] **Crowding bonus based on minimum litter:** define `X = minimum litter × n`
  precisely: its denominator/area, caps, and whether it affects tolerance,
  energy cost, or allowable group size. No linked rule currently exists.
- [ ] **Burrow speed:** decide what this means (construction, entering,
  movement while sheltered, or another action) before implementing it.
- [ ] **Offspring use and inherited burrows:** define when newborns can enter,
  who owns a burrow, and how it survives the parent's death. Depends on the
  burrow lifecycle above.

## Gardener

- [ ] **Pass through bushes:** Forest Edge shrub art is currently decorative;
  simulation terrain has no bush terrain or bush-specific movement rule. Add
  meaningful blocked/slow terrain and define whether predators can traverse it.
- [~] **Fertile Droppings:** the first node is authored and has a special-case
  runtime effect: +0.10 seed-drop chance, one food-reserve unit spent per seed,
  and a Hare-identified dropping scent. Base scent is 20 ticks/16 tiles;
  Droppings is 40 ticks/24 tiles; Fox tracking after sniffing reaches 36 tiles;
  scent sampling is every 10 ticks. The approved profile payout and unlock
  route now has passing automated coverage. These are provisional first-pass
  values, not a complete Gardener branch.
- [ ] **Picky eater:** add distinguishable food quality/nutrient values and a
  bounded reachable-food ranking rule. Current feeding rules target one diet
  species and cannot choose among ranked plant varieties.
- [ ] **Tier-2 / tier-3 droppings:** add plant tiers, seed selection, growth and
  reserve caps, and the cost/tradeoff that prevents the highest tier from
  becoming a strictly better free choice.
- [x] **Efficient digestion (POC):** the active node adds +0.25 digestion energy
  bonus through the bridge, with Fertile Droppings as prerequisite. Final balance
  and interaction evidence remain open.
- [ ] **Favorite food:** requires multiple edible plant identities and a named
  food target with a real availability tradeoff; neither the target nor the
  Genome preference effect exists.
- [~] **At One with Nature:** food reserves already support partial consumption
  and resources can regrow. The upgrade's preserve chance, “50% energy” meaning,
  per-plant growth-speed effect, spatial cap, and interaction with Fertile
  Droppings are not implemented or defined.
- [ ] **Patrols:** still an open concept. Define whether patrols revisit food,
  protect a patch, or perform another job, and specify their cost and Fox
  counterplay before keeping this as a node.

## Next implementation boundary

Shared selection, persistence and the permanent stat bridge are already present.
The immediate Luna queue is RG-00 (portable provenance) and RG-01 (nonterminal
round-6 controls), followed by a review of the actual normal-length route and
its shop/session integration. New stat-only offers reuse existing infrastructure
after their values are accepted. The remaining Gardener food nodes need distinct
food definitions and feeding/seed selection changes; burrows and migration need
explicit behavior contracts. See the
[implementation plan](../RABBIT_GENOME_IMPLEMENTATION_PLAN.md) for the complete
candidate inventory, design gates, acceptance checks and test budget.
