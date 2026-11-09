# GLJP Codex pickup — November 6, 2026

Local uncommitted work on `BevBranch` at `8659a1b` (`Clarify Sim handoff does not transfer ownership`). The GLJP implementation is in the working tree, not in that commit. Do not push. Do not commit unless Nick asks. Do not move or close Trello cards. Preserve every other dirty file and every existing `.meta` GUID.

## Unity is free again

The full live suite started at 21:38 UTC did not finish. EditMode completed: 744 passed / 0 failed / 1 skipped (`artifacts/unity-tests-20261106-163829`). PlayMode hit the 900-second live timeout, and the status poll then found no Pipeline instance. That is a dropped Editor connection, not a recorded assertion failure. There is no PlayMode result file for this run.

Run `CellSim.ps1 -Command Doctor` before another Unity command. If the Editor is closed or the Pipeline is gone, use `-Execution Clean` for the PlayMode rerun rather than assuming Live is still attached. Do not treat this timeout as a product failure.

The only expected remaining failure from the last completed full PlayMode run is the authored seed-10100 route, which dies at tick 239. That failure is retained ecology evidence. Do not tune it away. Last completed full run, from before the route-length change: EditMode 742 / 0 / 1 and PlayMode 94 / 1 / 2. PlayMode has not completed since the route-length change or the catalog slice.

## What just landed

Nick approved one GLJP-01 catalog slice. It is written and focused-tested.

Three provisional Hare research roots, 5 Hare Data, no prerequisite, empty modifiers:

| Research node | Unlocks journey Mutation | Does not unlock |
| --- | --- | --- |
| `hare.escape-instinct` | `threat-exposure` (display `ESCAPE INSTINCT`) | either litter Mutation, or Reproductive Drive |
| `hare.increased-minimum-litter` | `larger-minimum-litter` (display `INCREASED MINIMUM LITTER`) | the other litter Mutation, Escape Instinct, or Reproductive Drive |
| `hare.larger-litters` | `larger-maximum-litter` (display `LARGER LITTERS`) | the other litter Mutation, Escape Instinct, or Reproductive Drive |

Legacy research ids still count: `escape-instinct`, `increased-minimum-litter`, and `poc.hare.larger-litters`. New assets and GUIDs:

- `Assets/Data/TestData/Genome/HareGenome/EscapeInstinct.asset` — `7e4a91c2b6d84f0a9c3e15d8a2f647b0`
- `Assets/Data/TestData/Genome/HareGenome/IncreasedMinimumLitter.asset` — `8f5b02d3c7e94a1bad4f26e9b30758c1`
- `Assets/Data/TestData/Genome/HareGenome/LargerLitters.asset` — `906c13e4d8fa4b2cbe5037fac41869d2`

They are appended on `Assets/Data/TestData/Genome/HareGenome.asset`. `FieldDataPermanentProgressionDefinition.CreateStarter()` appends the same three nodes after `hare.reproductive-drive`, so existing `Take(8)` and `all[4]` tests stay valid. The lab shows 13 nodes with the locked concept included, and 12 after `RefreshGenomeCatalog(Empty)`. Combinatorial counts for 12 nodes at capacity 8 are `validCount = 3797` and `terminalCount = 1816`.

Faster Gestation stays `CONCEPT · LOCKED` (`fast-gestation` only, in `VM_GenomeLab.PermanentConceptNodeIds`). `GenomeContract.FastGestationNodeId` still applies a permanent 15% reduction inside `GenomeUpgradeNodeSnapshot.Apply`. There is no `SpeciesUpgrade` journey Mutation for gestation. Do not copy that 15% formula into a purchasable node or the shop.

`increased-minimum-litter` was removed from the concept list so it does not duplicate the new purchasable node. The old POC assets (`Assets/Data/Genome/...` and `RabbitSelectionPOC`) were not retargeted. `GenomeAuthoringAssetTests` still expects the POC increased-minimum-litter and fast-gestation nodes.

A litter rank that would make minimum greater than maximum is refused by `SpeciesUpgradeCatalog.PreservesLitterBounds`, used from `SpeciesProgression.TryApplyRunUpgrade` and `FieldDataJourneySession.IsOrdinaryRewardEligible`. Natural Hare litter is 1–2, so the second minimum rank needs a maximum rank first. `FieldDataHareSkillIds` appends `threat-exposure`, then `larger-maximum-litter`, then `larger-minimum-litter`, so an automatic pass that maxes each skill in pool order can apply both.

## What was already in the tree before this slice

Journey length follows the resolved route. `RouteSimulationCount`, `RouteShopCount`, and `OpensShopAfterSimulation` drive the session, ledger, preview, and portable replay. `WithSimulationCount` builds a test-only alternating route (`simulation-N` / `shop-N`). The authored starter is still exactly five nodes, Simulation → Shop → Simulation → Shop → Simulation, and rebuilding that three-simulation shape keeps the starter fingerprint. A four-simulation test buys +1 and 1.5× at shop 1, 1.75× at shop 2, and 2× at shop 3. Stable 100-Hare payouts are 300, 350, then 400. The production route stays 1800 ticks / 18 rounds. See [third-shop handoff](2026-11-06-gljp-third-shop-route.md).

GLJP-03 acceptance is met in local code: allowance, +1 FD/Hare on later payouts only, sequential replacing tiers 30/40/50, and duplicates removed from the live shop. Shop placement for a 7–10 node route is still Nick's decision. Do not author that route.

## Evidence already recorded

- EditMode `FieldDataPermanent`: 29/29 (`artifacts/unity-tests-20261106-163531`), including `AuthoredCatalogMatchesTheSingleRuntimeDefinition`.
- EditMode `FieldDataJourney`: 46/46 (`artifacts/unity-tests-20261106-163548`).
- `EscapeAndLitterResearchUnlockSeparateJourneyMutationsWithoutPermanentStats`: 1/1 (`artifacts/unity-tests-20261106-163610`).
- Gene Lab capture: 1/1 (`artifacts/visual-evidence-20261106-163739`). `gljp-provisional-litter-research.png` shows Larger Litters with `BUY · 5 DATA (TEST)` and the provisional-path note. `genome-permanent-concept-locked.png` shows Faster Gestation locked. The tree viewport does not show all 13 cards at once; the header says 13 genome options and the detail pane is the check.

Unity test filters are name substrings, not regular expressions. `FieldDataJourney(Session|Definition)Tests` matches nothing. If the Editor has not imported script changes, `recompile` through `tools/UnityTooling.ps1` (`Invoke-UnityLiveCommand`) and poll `recompile_status` until `status` is `completed`. Do not treat the JSON field `"failed": false` as a failure.

## Leave these decisions to Nick

- Final prices and prerequisites. The six research roots at 5 Hare Data are test values. See [morning review](../GLJP_MORNING_REVIEW.md) item 2.
- Keen Senses. It tracks creature food, and Hares eat plants. Do not retune it.
- Reproductive Drive against the 100% Hare reproduction baseline. The offer is suppressed at 100%. Do not change the baseline.
- Burrows, before any GLJP-04 code: whether a threat has passed immediately or after a grace period; what happens when every emergence cell is occupied; one burrow or many. Sprint, terrain, and Owls stay out.
- Exact 7–10 node shop placement.
- GLJP-01's Trello description had unsaved edits. Save or discard them in the browser. Cards: [GLJP-01](https://trello.com/c/dR2z6ENf) In Progress, [GLJP-02](https://trello.com/c/o4q7Necn) Done, [GLJP-03](https://trello.com/c/h4wxvWAp), [GLJP-04](https://trello.com/c/DQwFKLSF), and [GLJP-05](https://trello.com/c/Htt7PJBY) still Current Work. Board: https://trello.com/b/Z14AeSnU

## Next step

Rerun PlayMode after Doctor shows a reachable Editor. If the only failure is seed 10100 at tick 239, record the counts in `WORKING_STATE.md` and this note, and stop. New failures need a cause before any balance or catalog change. After that, the remaining product work is Nick's open decisions above, not another unlock node.
