# Gene Lab and Journey Progression — Process Design Document

- **Document:** PDD-GLJP-001
- **Status:** Approved design direction with provisional balance values and open content details
- **Owner:** Bevin
- **Updated:** 2026-11-11
- **Technical companion:** [Software Design Document](GENE_LAB_JOURNEY_PROGRESSION_SDD.md)
- **Source inventory:** [Gene Lab and Journey Upgrade Inventory](GENE_LAB_AND_JOURNEY_UPGRADE_INVENTORY.md)

Implementation evidence and remaining work are in the [overnight handoff](handoffs/2026-11-01-gljp-overnight-progress.md). The [morning review list](GLJP_MORNING_REVIEW.md) records decisions parked during unattended implementation; these are not silently resolved by this PDD.

## 1. Purpose

Define how permanent Gene Lab research, temporary journey Mutations, paid journey shop offers, and population interventions fit together. The player should understand what is permanent, what is only active for one journey, and which purchases change the route's economy or population.

This is a feature design for the current Hare-focused slice. It does not redefine the whole Field Data journey, predator progression, or future terrain systems.

## 2. Product goals

- Make Gene Lab research unlock capabilities and choices, rather than small permanent flat-stat bonuses.
- Keep permanent research, equipped Genome nodes, journey Mutations, and Field Data purchases visibly distinct.
- Give each upgrade a clear effect and acquisition source, avoiding duplicate options across systems.
- Make population purchases predictable and prevent repeated Hare purchases from bypassing the node allowance.
- Support a longer journey of roughly 7–10 simulation nodes; the current short route remains a proof of concept.

## 3. Player-facing progression model

| System | Player action | Lifetime | Rule |
|---|---|---|---|
| Gene Lab research | Permanently researches a capability or unlock | Permanent profile state | Ownership unlocks the node for equipping; it does not by itself add a temporary Mutation to journey offers. |
| Active Genome | Equips researched nodes before a journey | Frozen for that journey | Equipped research controls which Gene Lab Mutations can appear. Every equipped node costs **1 capacity point**. Total active capacity remains **8**. A ranked node uses one slot at its chosen rank. |
| Journey Mutation | Selects an offered Hare adaptation | Current journey | Temporary; clears at journey end. A Gene Lab Mutation is eligible only when its research is owned and that node is equipped. |
| Field Data shop | Buys route economy or other distinct shop offer | Current journey | One-time and tiered offer rules are stated below. |
| Population intervention | Adds/removes population or plants at an allowed boundary | Immediate route state | These are paid controls, not upgrades. Existing controls remain as authored for now, except Add Hare is limited by Population Reserves. |

**Eligibility rule approved November 11, 2026:** a Gene Lab Mutation enters the journey offer pool only when both its Gene Lab research is owned and its node is in the active Genome loadout. Buying research permanently unlocks the node; equipping it enables its journey Mutation; choosing the Mutation applies it for that journey. This supersedes the earlier implementation rule that owned research alone enabled offers. Starter genes remain explicit launch grants, separate from journey choices.

## 4. Genome capacity and research

Maximum active Genome capacity is **8**. Each equipped Gene Lab node costs **1 point**, regardless of effect or rank. Research rank does not increase capacity cost. Existing authored 2-point allocations must be reconciled to this rule. The player equips the highest researched rank of Population Reserves rather than equipping each rank separately.

Existing prices and values in test assets are not treated as balance-approved unless called out below. Research prices and dependency details for newly defined nodes remain open only where the inventory and approved diagram do not set them.

## 5. Gene Lab and journey Mutation map

“Research unlocks” means the relevant temporary Mutation becomes eligible for journey offers. These options do not also grant a permanent stat bonus.

| Gene Lab research | Journey option / result | Current product direction |
|---|---|---|
| Tough Hide | Tough Hide | Unlock temporary Block Mutation; remove Reinforced Hide shop duplicate. |
| Heightened Senses | Long Vision; Keen Senses | One research unlock connects both choices. Long Vision extends general vision range; Keen Senses extends predator-detection range. They remain distinct Mutations. |
| Long Stride | Faster Movement / Long Stride | Unlock temporary movement Mutation; remove signed Long Stride shop duplicate. |
| Efficient Digestion | Efficient Digestion | Unlock temporary digestion Mutation; remove Deep Digestion shop duplicate. |
| Crowding Tolerance | Crowding Tolerance | Unlock temporary reduction to extra crowding energy cost; remove Warren Comfort duplicate. Research cost is 10 Hare Data (decided 2026-11-11). |
| Reproductive Drive | Reproductive Drive | Improves chance of successful reproduction. Distinct from litter size and gestation time. |
| Seed Dispersal | Seed Dispersal | The single Gene Lab unlock for the seed-planting Mutation. Fertile Droppings is merged into this option; no second Gene Lab node or shop purchase. A successful seed drop costs 1 stored food. Defer its separate scent-marker concept. |
| Faster Gestation | Faster Gestation | Temporary reduction to Hare gestation time by 15% per rank, up to three ranks. Reuses the existing Genome test value provisionally; distinct from reproduction chance and litter size. |
| Increased Minimum Litter | Increased Minimum Litter | Add as a regular Hare journey choice; raises minimum litter only. |
| Larger Litters | Larger Litters | Add as a regular Hare journey choice; each of three ranks raises maximum litter by 1. At rank 3, minimum litter is automatically raised to at least 2. |
| Reproductive baseline | Reproductive Drive | The production Hare starts at 0.5 reproduction chance; Reproductive Drive increases chance from that baseline. |
| Threat Avoidance research, renamed **Escape Instinct** | Escape Instinct | Unlock the existing temporary pre-contact avoidance Mutation. No permanent avoidance stat or duplicate Sharper Instinct shop offer. |
| Burrowing | Burrowing | Unlock temporary journey Mutation that enables the burrow behavior below. Research cost is 10 Hare Data (decided 2026-11-11). |
| Shared Burrows | Shared Burrows | Separate Gene Lab research unlock for a temporary Mutation that permits unrelated Hares to use available burrows for that journey. |
| Expanded Burrows | Expanded Burrows | Separate Gene Lab research unlock for a temporary Mutation. Five ranks; each adds +1 capacity to every burrow for that journey, from base 3 up to 8. |
| Open-ground Sprint | Open-ground Sprint | Unlock a one-rank temporary Mutation that adds +0.5 movement speed on exposed/open terrain (provisional balance value). Current simulation terrain, Bare and Grass, is exposed; Hares hidden in burrows do not move. Active Born Burrower excludes it. Research costs 10 Hare Data. If covered terrain is added, covered tiles must suppress this bonus. |

### Approved Gene Lab catalog prices and paths

On November 11, 2026, Bevin confirmed the supplied progression diagram as authoritative for every listed research price, rank, and prerequisite arrow. Each displayed tier is the Hare Data price for that node's unlock or each rank purchase. Apply the arrows as research prerequisites:

| Hare Data price | Gene Lab nodes |
|---:|---|
| 10 | Open-ground Sprint; Crowding Tolerance; Burrowing; Tough Hide |
| 20 | Heightened Senses; Long Stride; Reproductive Drive; Shared Burrows; Efficient Digestion |
| 30 | Escape Instinct; Seed Dispersal; Gestation Upgrade; Expanded Burrows; Population Reserves |
| 40 per rank | Litter Upgrade ranks 1–3; Founding Hares ranks 1–3; Born Burrower |

The diagram's directed paths are: Sprint → Heightened Senses → Escape Instinct → Seed Dispersal → Gestation Upgrade → Expanded Burrows → Population Reserves; Heightened Senses → Long Stride; Crowding Tolerance → Reproductive Drive joins at the Seed Dispersal/Gestation segment; Burrowing → Shared Burrows joins at the Gestation Upgrade/Expanded Burrows segment; and Tough Hide → Efficient Digestion joins at the Expanded Burrows/Population Reserves segment. The tier-40 branches are Seed Dispersal → Litter Upgrade 1–3, Gestation Upgrade → Founding Hares 1–3, and Expanded Burrows → Born Burrower. Treat every connecting arrow as a prerequisite. The Efficient Digestion branch is read as an additional Population Reserves prerequisite because its arrow joins immediately before that node.

This approved catalog replaces the older provisional Founding Hares price of 5 and Population Reserves rank prices of 5/10/15/20/25. It also replaces the earlier 5-Hare-Data placeholders for the listed nodes. Sprint now uses a provisional +0.5 movement value over current exposed terrain; balance approval remains open. If covered terrain is added, covered tiles must suppress the bonus. This catalog decision does not approve balance values.

Existing Mutation strengths are starting test values and remain provisional. The source-backed inventory records current values and offer pools; it is not evidence of final balance.

## 6. Starting genes

### Founding Hares

A separate starter Gene Lab option, not a journey Mutation. It has three research ranks at **40 Hare Data per rank**. When equipped, it adds Hares at the start of a new journey; the currently authored effect is +5 Hares and the per-rank effect still needs implementation review. It costs 1 Genome capacity under the global rule.

### Born Burrower

A separate starter gene that grants Burrowing at journey launch. It costs **40 Hare Data** and follows Expanded Burrows in the approved research path. It excludes Open-ground Sprint while equipped. Researching Burrowing alone does not grant it at launch.

## 7. Population Reserves and Hare purchase allowance

Population Reserves limits only player-purchased Hare additions. It does not cap births or surviving population.

- Base allowance: **10 Hare additions per simulation node**.
- Five research ranks at **30 Hare Data each**; each equipped rank adds **5** to that node's allowance.
- Rank 1 = 15; rank 3 = 25; rank 5 = 35 total additions per simulation node.
- The selected highest researched rank is equipped as one Genome node and costs 1 capacity point.
- Allowance resets at the beginning of each simulation node.
- Count accepted Add Hare purchases cumulatively. Removing a purchased Hare does not restore allowance; adding again counts again.
- Existing Add Hare price remains 10 Field Data for now. Other population controls retain current authored behavior and price.

The previous +10 refill/pool-capacity effect is replaced by this purchase allowance.

## 8. Field Data shop

### Income offers

- **+1 Field Data per Hare:** one-time purchase per journey, provisional price 20 FD. It affects payouts calculated after purchase only; it does not recalculate prior payouts.
- **Population payout multiplier:** three sequential, one-time tiers: **1.5× → 1.75× → 2×**. Each replaces the previous tier and the maximum is 2×. Starting prices are 30/40/50 FD and remain provisional.
- Make the tiers available early, middle, and late across the planned longer route. Exact simulation-node placement remains open.

### Population controls and duplicate shop effects

Keep the existing Add/Remove Hare, Add/Remove Fox, and Add/Remove 5 Plants controls as currently authored for now. The +1 Hare purchase is subject to the per-node Population Reserves allowance. Refine other controls later.

Remove the paid shop modifiers that duplicate the Gene Lab/Mutation direction: Reinforced Hide, Long Stride, Deep Digestion, Warren Comfort, Abundant Seedfall, and Sharper Instinct. The user selected +1 FD/Hare and population payout multipliers as shop-only income progression.

## 9. Burrowing behavior

- A Hare may automatically create a burrow only when all eight neighboring cells are empty of creatures/species.
- Creation costs 24 energy (starting test value, based on current Hare energy of 48).
- Burrows last 200 simulation steps and have base capacity 3, including the digging Hare.
- Descendants qualify when either parent belongs to that burrow family.
- A qualifying family member at the burrow or an adjacent cell automatically hides when a Fox is within attack range, if capacity is available.
- Hares emerge when no Fox is in attack range or the burrow collapses. At expiry, if all emergence cells are occupied, occupants remain protected until a cell frees, then emerge.
- Shared Burrows allows unrelated Hares to use open capacity during that journey. Expanded Burrows increases capacity by rank as specified above.

Lineage must be tracked for descendants through either parent. A Hare may create one burrow; Shared Burrows research permits access to other Hares' burrows. The high creation energy cost (24 energy) is a starting test value, not balance approval.

## 10. Explicitly out of scope

This feature does not define Owls or broad terrain interactions for bushes, trees, rocks, or water. Those ideas are parked. Current Bare and Grass terrain are open ground for Sprint. If covered terrain is added later, covered tiles must suppress the Sprint bonus. Population controls beyond the Add Hare allowance remain at their current proof-of-concept behavior.

## 11. Open design items

- Player review of the updated catalog presentation and its locked/prerequisite states.
- Exact longer-route node placements for payout tiers and how research unlocks are paced across 7–10 simulation nodes.
- Final Mutation magnitudes, rank caps, offer probabilities, and route-specific offer eligibility.
- Balance review Open-ground Sprint's provisional +0.5 movement speed; if covered terrain is added, enforce the documented covered-tile suppression without expanding this delivery into terrain or Owl systems.
- Exact burrow-collapse cause and edge ordering when a burrow expires while occupied; lineage implementation must preserve descendant access after either parent dies.
- Final failure/transition behavior for an allowance consumed across an interrupted or restarted simulation node, consistent with the existing Field Data session contract.

## 12. Acceptance summary

The feature is ready for implementation when the runtime distinguishes permanent research from temporary Mutation eligibility, applies the 1-point/8-point Genome rule, enforces the per-node Hare purchase allowance, exposes the approved Gene Lab/Mutation relationships, and preserves the chosen shop-only income progression. Provisional values require later balance review and are not a launch-balance approval.
