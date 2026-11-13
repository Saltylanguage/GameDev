# Gene Lab and Journey Progression — Software Design Document

- **Document:** SDD-GLJP-001
- **Status:** Proposed implementation design for the approved direction in [PDD-GLJP-001](GENE_LAB_JOURNEY_PROGRESSION_PROCESS_DESIGN.md)
- **Updated:** 2026-11-01
- **Source inventory:** [Gene Lab and Journey Upgrade Inventory](GENE_LAB_AND_JOURNEY_UPGRADE_INVENTORY.md)

## 1. Technical outcome

Integrate Gene Lab unlocks, active Genome configuration, journey Mutation eligibility, Field Data shop progression, Hare purchase allowances, and burrow-family behavior into the existing profile, journey-session, simulation, and Noesis UI boundaries. Keep deterministic gameplay rules in existing domain types and use existing serialized assets and test assemblies. Do not introduce a generic modifier framework or a second source of truth.

The PDD is the product contract. Values explicitly marked provisional remain data/configuration values and must not be described as balanced.

## 2. Existing architecture seams

Observed code provides these extension points:

| Existing source | Current responsibility | Change required |
|---|---|---|
| `Assets/Scripts/Game/Species/SpeciesGenomeContract.cs` | Genome node definitions, capacity validation, rank allocation, active snapshot | Make every purchasable node's base capacity cost 1; keep max capacity 8; verify rank allocation remains one equipped node and reports selected rank. |
| `Assets/Scripts/Game/Species/FieldDataPermanentProgressionDefinition.cs` | FD2 starter catalog and current five-node stat map | Replace obsolete stat-bonus mapping with capability unlock definitions and Population Reserves allowance ranks; include new researched options and starter-gene constraints. |
| `Assets/Scripts/Game/Species/SpeciesUpgrade.cs` | Mutation catalog, effect rules, levels/caps and eligibility | Add/rename/map the approved journey Mutations, litter choices, and genome-unlock eligibility without granting them as permanent stats. |
| `Assets/Scripts/Game/Simulation/FieldDataJourneySession.cs` and related shop/ledger types | Journey-local economy and state | Own the per-simulation-node accepted Add Hare count and reset it only when entering a fresh simulation node. |
| `Assets/Scripts/Game/Simulation/FieldDataPopulationIntervention.cs` | Validates/applies paid population interventions | Gate Add Hare atomically against researched/equipped allowance, deducting FD and incrementing count only on accepted purchase. Removal must not refund count. |
| `Assets/Scripts/Game/Simulation/SimulationRunResult.cs`, `SpeciesSimulationPreview.cs` | Snapshot/runtime projection for active Genome and population space | Replace old population-pool-capacity interpretation where it currently gates purchasable Hares; pass the node allowance to the intervention boundary. |
| `Assets/Scripts/Game/Simulation/FieldDataShopOffers.cs` and starter assets | Authored offers/prices/route | Keep +1 FD/Hare and replaceable payout multiplier tiers; remove overlapping paid species modifiers. Keep short route marked test-only. |
| `Assets/Scripts/Game/Simulation/SpeciesSimulation.cs` | Deterministic cell simulation and seed drop behavior | Consolidate Seed Dispersal/Fertile Droppings to one effect; add burrow state and rules only through simulation-owned deterministic transitions. |
| `Assets/UI/GalapagOS/Scripts/VM_GenomeLab.cs` and its XAML | Gene Lab presentation and commands | Show unlock versus active rank versus journey Mutation eligibility; display capacity at 1 point per equipped node; use canonical names and remove obsolete stat labels. |

Exact type members, asset IDs, serializers, and assembly definitions must be rechecked immediately before each implementation ticket because the working checkout may have changed since this document was prepared.

## 3. State ownership and lifecycle

| State | Owner | Lifetime / persistence |
|---|---|---|
| Researched Gene Lab nodes and researched ranks | Existing permanent profile Genome state | Persist across sessions and journeys. |
| Equipped node IDs and selected rank allocations | Existing per-species Genome profile | Persist as loadout; snapshot/freeze at journey launch as current architecture requires. Each equipped node consumes 1 capacity. |
| Mutation eligibility derived from owned research | Journey launch/offer-generation boundary | Capture permanent owned node IDs independently of equipment for that journey; do not mutate permanent research during a run. |
| Acquired Mutations and their levels | Existing journey progression/session state | Carry across phases as current route does; discard at journey end. |
| Hare additions used in current simulation node | Field Data journey session | Reset when a new simulation node begins; never reset on removal or UI re-entry. Not profile-persisted unless the existing session persistence contract requires it. |
| Income offers and current payout tier | Journey-local shop/session state | One +1 FD/Hare purchase and highest purchased multiplier tier per journey; payouts already settled remain unchanged. |
| Burrows and family links | Simulation state/cell representation | Deterministic current world; preserve through existing checkpoints, cloning, previews, portable definitions, replay/export paths that represent a live simulation. |

## 4. Genome catalog and capacity

Keep `GenomeContract.ActiveCapacity` at 8. Change node capacity data/defaults and validation so every equipped node consumes one. For ranked nodes, the selected rank is represented by the existing allocation/rank contract while capacity remains one. Avoid representing each rank as separately equipped nodes.

Preserve stable node IDs when the same concept remains. For removed or repurposed nodes, add explicit profile compatibility handling: old two-point allocations for existing nodes must normalize to one point for retained nodes; obsolete nodes must not silently grant new capabilities. Choose migration behavior using the profile snapshot's current validation/load path and add tests for old saved allocations before editing profile serialization.

Researching a node and equipping it remain separate. Retain owned-prerequisite semantics. The journey receives an immutable equipped Genome snapshot for launch effects and a separate frozen ownership policy for Mutation eligibility. November 5 implementation gates opening, phase, and Field Data rewards by owned research; no equipment requirement is imposed for Mutation access.

## 5. Mutation eligibility and offer composition

Use the existing `SpeciesUpgrade` IDs and reward-pool construction where possible. Map permanent owned research IDs to eligible Mutation IDs at journey launch. Mutation selection remains temporary and uses existing progression rules. `CaptureMutationResearch` supplies the frozen profile policy; profileless domain/research harnesses keep their explicitly unrestricted pools.

Requirements:

- Heightened Senses enables both Long Vision and Keen Senses as distinct choices. Long Vision increases general vision; Keen Senses increases predator-detection range.
- Seed Dispersal is the only seed-planting Gene Lab unlock; avoid duplicate Fertile Droppings node behavior and duplicate seed Mutation offers.
- Escape Instinct maps to the existing Threat Avoidance behavior and does not create a second Mutation.
- Increased Minimum Litter and Larger Litters are separate regular Hare choices. Larger Litters increases maximum by one per rank through rank 3; its third rank also raises minimum litter to at least 2 when needed.
- Reproductive Drive and Faster Gestation remain separate dimensions.
- Gene Lab research alone must not apply any Mutation or permanent flat bonus.
- Existing route/phase offer filtering, caps, determinism, and reward receipts must remain intact.

## 6. Population Reserves enforcement

Represent Population Reserves as one equipped ranked node. Starting add allowance is 10 per simulation node; each selected rank adds five. The effective allowance is therefore `10 + (rank * 5)` for ranks 1–5 when a rank is equipped, and 10 when absent. Confirm against the PDD's rank convention before coding because “rank 1 = 15” is the chosen interpretation.

Enforcement belongs at the authoritative population-intervention transaction, not in UI alone:

1. Resolve the current node allowance from the frozen active Genome snapshot.
2. For Add Hare, validate cost, placement capacity, and `acceptedAdds < allowance` before mutation.
3. Apply the population change and spend FD atomically.
4. Increment `acceptedAdds` exactly once only after success.
5. Reject with no charge, no board change, and no counter change when invalid.
6. Remove Hare does not reduce the counter.
7. Reset counter on transition into the next simulation node, not shop visit, screen open, results refresh, or repeated command.

Preserve all other control quantities, costs, and boundary availability. Remove or stop using `PopulationPoolCapacity` as a proxy for the purchase allowance; do not make it cap births or survivors.

## 7. Shop and payout calculation

Keep the existing one-time +1 FD/Hare shop offer. Its purchase applies only to payout calculations executed after purchase. Implement population payout multiplier as a monotonic tier index/value where buying 1.75x replaces 1.5x and buying 2x replaces 1.75x. Validate sequential eligibility and one-time purchase at the session/shop boundary. Keep the provisional 30/40/50 prices in authored data.

Delete the six signed duplicate shop species modifiers from the active offer catalog and any stale references safely. Do not delete unrelated historical notes or test evidence. Keep current shop/purchase controls unchanged except Hare allowance. Do not derive access from the current three-simulation route; exact 7–10 node pacing is still open.

## 8. Burrowing simulation model

Burrows need explicit simulation-owned state: stable burrow identity, location, creation/expiry step, capacity, and family/occupancy representation sufficient for deterministic hide/emerge behavior. Extend the creature lineage state to retain parent IDs or a compact family ancestry identifier. On birth, a Hare offspring joins a burrow family if either parent is a member. Preserve lineage across cell copies, snapshots, previews, serialization/checkpoints, and deterministic exports.

Creation and transition order must be deterministic:

- Check all eight neighboring cells for any creature/species occupancy.
- Check energy >=24 before creation; deduct energy and create once.
- Capacity defaults to 3 including the digger; Expanded Burrows adds one per active rank, maximum 8.
- A qualifying Hare in the burrow/adjacent cell hides only if a Fox is within the established attack-range query and a slot is free.
- Shared Burrows adds unrelated Hares to eligible occupants for the journey.
- Expire at the 200-step boundary and emerge when no Fox is in attack range or the burrow collapses. If no emergence cell is free at expiry, occupants remain protected until a cell becomes free, then emerge.

Expiry at 200 steps is the first collapse trigger. The journey tracks ancestry and hidden inhabitants separately from the grid. Underground inhabitants retain their creature state and pause aging, metabolism and gestation in the first implementation. They count toward population totals, and checkpoints and portable journey digests include burrow state. Further collapse triggers, terrain behavior, and Owls remain open. See the [runtime handoff](handoffs/2026-11-08-gljp04-burrowing-runtime.md) for validation status.

## 9. UI and data assets

Update the existing Gene Lab view model and XAML rather than adding a parallel screen. Each option needs visible state for: locked/researchable, researched, active, rank, capacity, journey Mutation unlocked, starter-gene launch effect, incompatibility/exclusion, and provisional value where shown in test UI. Use the approved canonical names from the PDD. Keep runtime view state derived from domain snapshots.

Update existing ScriptableObject/test assets and their `.meta` files without changing GUIDs for retained assets. Avoid direct serialization migrations in UI code. Any removed offer/node references must be checked in prefabs, scenes, test fixtures, and catalog providers.

## 10. Delivery sequence and tests

1. **Catalog and capacity:** schema/profile compatibility, one-point cost, eight-point maximum, rank semantics, Gene Lab labels.
2. **Mutation unlocks:** canonical IDs/names, eligibility mapping, regular offer pools, litter effects, duplicate consolidation.
3. **Shop and reserves:** per-node Hare-add accounting, one-time +1 FD/Hare, sequential payout tiers, duplicate shop removal.
4. **Burrowing:** lineage, burrow lifecycle, capacity upgrades, family/shared membership, preview/export/checkpoint parity.
5. **Integration:** Unity compile, focused EditMode/domain tests, existing Field Data tests, Unity play capture of Gene Lab and journey choice screens, then review open values.

Focused tests should cover:

- Eight active capacity permits eight one-point nodes; the ninth is rejected; ranked Population Reserves remains one equipped node.
- Legacy saved capacity allocations migrate or reject with a clear deterministic rule; unlocks and wallets are preserved.
- Research unlocks offers but does not apply Mutation; equipped state and route pools gate eligibility.
- Heightened Senses enables two distinct choices; merged seed concept appears once; Escape Instinct aliases existing behavior without duplicate effects.
- Minimum and maximum litter changes remain independent; maximum-litter Mutation ranks add +1 each through rank 3, and reaching rank 3 automatically raises minimum litter to at least 2. Gestation and success chance remain independent.
- The production Hare reproduction-chance baseline is 0.5; Reproductive Drive remains a separate temporary chance increase.
- Add Hare allowance counts successful additions cumulatively per node; removal gives no refund; invalid purchases are atomic; next simulation node resets count.
- Shop income purchase affects later payouts only; multipliers replace prior tier and cannot exceed 2x or be bought out of order.
- Burrow spacing, energy, 200-step expiry, capacity, lineage, shared membership, threat-triggered hiding and emergence are deterministic and survive relevant clones/checkpoints.
- Existing population controls and unrelated profile state retain their behavior.

Do not claim balance acceptance from implementation tests. Balance values and route pacing remain provisional until a separate fixed-seed and player review.

## 11. Risks and decision gates

- Existing profile schema and capacity rank allocations may encode two-point nodes; migration needs explicit compatibility tests.
- Mutation pools vary by route; eligibility changes could accidentally alter offer probability, repeated levels, or deterministic replay.
- The current `PopulationPoolCapacity` contract may be used outside Hare intervention and must be traced before replacement.
- Burrow lineage touches copy/preview/export paths and can alter deterministic simulation identity; establish parity fixtures before broad edits.
- The long journey route is not authored. Keep shop tiers data-driven and avoid pretending exact node pacing is settled.
- Open-ground Sprint cannot be fully specified until exposed/covered terrain is defined. Do not expand this delivery into terrain or Owl systems.
