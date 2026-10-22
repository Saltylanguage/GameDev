# Gene Lab selection proof of concept

**Owner:** Josh. **Date:** October 20, 2026. **Status:** Implemented and verified as a POC.

Josh approved testing selection with unfinished Genome content. This catalog
tests the purchase and build flow; its stat values, prices, point costs and
prerequisites are provisional and are not balance recommendations.

## Where it appears

Open **Gene Lab** from GalapagOS Desktop. The canonical `GalapagOSDesktopTest`
scene now explicitly uses
`Assets/Data/TestData/Genome/RabbitSelectionPOC/RabbitSelectionPOC.asset`.
The separate production Hare catalog still contains only Fertile Droppings.
The POC includes that existing approved node by reference.

The current profile gate, Rabbit-tree license, wallets, eight-point capacity,
reset fee/refund, save/load, and next-journey activation remain in effect.
No development currency is granted to player profiles by this feature.

## Test offers

The October 21 [population extension](2026-10-21-Hare-population-pool-and-Genome-nodes.md)
adds the two population nodes listed below. Earlier evidence in this document
describes the original seven-option catalog; current extension verification is
recorded separately.

All activatable nodes below cost **1 Rabbit Data**. Test stat nodes cost two
capacity points, making capacity conflicts easy to exercise with this small
catalog. Fertile Droppings retains its approved one-point cost.

| Branch | Node | Prerequisite | Test effect | Capacity |
| --- | --- | --- | --- | ---: |
| Gardener | Fertile Droppings | None | Existing grass renewal and predator scent ability | 1 |
| Gardener | Efficient Digestion | Fertile Droppings | Digestion energy bonus +0.25 | 2 |
| Trailblazer | Long Stride | None | Movement speed +0.25 | 2 |
| Trailblazer | Long Vision | Long Stride | Vision range +2 tiles | 2 |
| Warren | Tough Hide | None | Block +1 | 2 |
| Warren | Larger Litters | Tough Hide | Maximum litter size +1 | 2 |
| Warren | Founding Hares | None | Starting population +5 per new journey | 2 |
| Warren | Population Reserves | None | Replenishment pool capacity +10 (base 50) | 2 |
| Warren | Burrows (Preview) | Tough Hide | No effect; inspectable, cannot be activated | — |

New IDs use the `poc.hare.` prefix and assets remain under `TestData`. This is
an explicit small test catalog, not the full proposed 25-node tree. Existing
older metadata-only Hare/Fox test maps remain separate.

## Expected player flow

1. Open the first authored branch (currently Gardener), select another branch,
   or show all branches. The all-branches overview is dense at 720p; branch
   filters provide the readable working view for the current window.
2. Click a node to inspect it. A selected node has a thicker border and filled
   background; selection does not spend Data.
3. Read the node state, prerequisite names, price/point cost, and specific
   purchase blocker. Locked and preview nodes can still be inspected.
4. Activate an eligible node. The profile saves the allocation and receipt;
   active nodes and occupied capacity points update immediately. Selection
   survives profile refreshes and wallet changes.
5. Reset to refund the allocation's recorded Rabbit Data spend and pay the
   existing one-Research-Data fee. Permanent tree access stays unlocked.
6. Start a new journey. It receives the resolved saved build. Profile changes
   cannot rewrite an already running journey.

UI selection and branch filter are window state; the active build is profile
state. Reopening a scene may select the first node while retaining the saved
allocation.

## Limits and removal condition

- Rabbit allocation only. Other species' allocation economies are not designed.
- Burrows, migration, plant tiers, and other new abilities are not implemented
  by these stat fixtures. Burrows is deliberately a non-purchasable preview.
- Legal POC allocations are now selectable through the CellSim research CLI
  with an explicit `-RabbitGenomeCatalogPath`. See
  [simulation tooling](../UNITY_SIMULATION_TOOLING.md#rabbit-genome-build-input-october-20).
- Before removing this catalog from Desktop or promoting/replacing its IDs,
  reset POC allocations or provide an explicit save migration. A profile with
  active POC IDs requires this catalog to resolve them at launch.
- Replace the POC banner, values and scene catalog only after the corresponding
  production offers and balance evidence are agreed. Do not silently promote
  the provisional numbers.

## Verification

The clean Unity host passed the full Lab PlayMode regression (12/12), the
focused selection test with captures at 720p and 1080p, and the Genome
authoring/contract checks (24/24). The test covers Noesis branch and node
bindings, blockers, allocation/reset/refund, session reload, scene
reconstruction, next-journey effects and frozen running journeys. Five UI
states were captured and inspected at both resolutions. See the
[handoff](../handoffs/2026-10-20-2032-codex-gene-lab-node-selection-poc.md)
for exact reports, visual fixes and evidence limits.
