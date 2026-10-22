# Fertile Droppings Gene Lab playtest offer — decision preview

[Working state](../WORKING_STATE.md) | Status: approved playtest offer, implemented locally

The [mechanics handoff](../handoffs/2026-10-17-1545-codex-fertile-droppings-scent-first-slice.md) and [matched-seed pilot](2026-10-17-fertile-droppings-pilot.md) make the effect reviewable. Josh approved these provisional terms and the shared production asset and scene wiring on October 17. The current Gene Lab offer uses them; they are playtest values, not a final balance decision.

| Offer element | Approved playtest value |
| --- | --- |
| Species and branch | Hare, Gardener root |
| Stable node ID | `fertile-droppings` |
| Prerequisite | Rabbit Genome tree access; no other node |
| Active capacity | 1 of the species' 8 points |
| Node purchase | 1 Rabbit Data, refundable on reset |
| Tree access | 1 Rabbit Data after three completed runs |
| Reset | 1 Research Data flat fee; return active node spend |
| Player copy | “Grow new grass from stored food. Droppings can draw hungry predators, who may follow your scent.” |

The node effect would be the already tested +0.10 seed-drop chance using one stored-food reserve on a successful drop, plus the agreed 20/16 ordinary scent, 40/24 dropping scent, and 36-tile post-sniff tracking. Every Hare in the next run receives the frozen active node. The pilot shows strong food renewal and lower mean Fox counts, so these terms are deliberately cheap for interface and balance testing, not a claim of fair final price.

The production Hare map now contains the Gardener root, and the Desktop scene binds it to Gene Lab. Profile transactions back the tree, node, and reset buttons; wallet, completion gate, and active capacity update from the loaded profile. A purchase made after Desktop initialization is copied into the simulation snapshot when the player opens a ready journey. An already started journey keeps its frozen Genome. The [implementation handoff](../handoffs/2026-10-17-1719-codex-fertile-droppings-gene-lab-offer.md) records validation and the remaining gameplay payout seam.

The current game still has no production call that records a completed journey or grants Rabbit/Research Data. Test profiles can exercise the whole purchase and save route, while ordinary play cannot yet earn the offer. A separate payout rule and completion hook must be agreed and wired before treating the offer as naturally unlockable.
