# GLJP morning review — November 1 overnight work

Bevin requested unattended implementation, with questions parked for morning review. No clarification is required to continue the independent work below.

## Decisions needed

1. **Resolved November 5 — research versus equipment:** permanent owned research gates journey Mutation eligibility independently of equipment. Owned research is captured at journey launch. Fresh profiles get a continue action at the opening reward and may skip empty Field Data checkpoints. Bevin confirmed the fresh-profile gate.
   - **Resolved November 7 — route-seed diagnostic scope:** the approved first-100-tick comparison for seeds 10100 and 10108 completed. Both seed traces differed between the October and November runners; disabling ordinary Fox movement-scent pursuit matched October's Hare/Fox/Plant counts at every tick in the measured window. This does not close current route survival or balance. See [GLJP-06 route preflight](handoffs/2026-11-07-gljp06-route-preflight.md).
2. **New research prices and prerequisites:** choose costs and prerequisite paths for Heightened Senses, Crowding Tolerance, Reproductive Drive, Seed Dispersal, gestation/litter nodes, Escape Instinct, Burrowing, Born Burrower, Shared Burrows, Expanded Burrows, and Sprint. Founding Hares and Population Reserves already have approved prices. Heightened Senses, Long Vision, Keen Senses, Seed Dispersal, Escape Instinct and litter unlock relationships are now represented locally; root costs and paths remain test values.
   - Seed Dispersal, Crowding Tolerance, Reproductive Drive, Escape Instinct, Increased Minimum Litter, and Larger Litters are being tested as 5 Hare Data roots with no prerequisites. These are explicitly provisional in the Gene Lab and are not final design decisions. Faster Gestation stays locked: the permanent 15% preview is not a journey Mutation, and no approved journey value was invented.
   - **Resolved maximum-litter rank limit:** Larger Litters adds +1 maximum litter per rank, capped at rank 3; rank 3 automatically raises minimum litter to at least 2. The third-rank combined effect is implemented locally with a focused regression test. **Minimum-litter Mutation rank limit remains undecided** and stays on the morning list; do not infer a cap from the maximum-litter decision.
3. **Resolved November 6 — Burrowing behavior:** a Hare may create one burrow only when all eight neighboring cells are free of creatures; creation costs a high amount of energy (exact value remains provisional). A starting burrow holds up to 3 Hares and lasts 200 simulation steps. The creator's descendants through either parent can hide there. Threat protection ends when no Fox is in attack range or when the burrow collapses. If all emergence cells are occupied at expiry, inhabitants stay protected until a cell frees and then emerge. Shared Burrows research permits use of other Hares' burrows; Expanded Burrows research increases capacity. The user accepted tracking descendant lineage if feasible. The implementation still needs a deterministic ancestry and hidden-population contract through counts, movement, checkpoints and exports; see the [burrow lineage investigation](handoffs/2026-11-06-gljp-burrow-lineage-investigation.md).
4. **Open-ground Sprint:** exposed/covered terrain detection and effect strength remain open. Terrain and Owl work stays parked.
5. **Long journey shops:** choose exact shop placements for the 1.5x / 1.75x / 2x tiers. The authored route is still three simulations and two shops. A separate test route now exercises the three tier purchases. The implementation preserves the short production route rather than inventing a longer one.
   - November 6 investigation: the starter asset still requires exactly five alternating nodes. Session, ledger, preview, and portable replay now take their simulation and shop boundaries from that route instead of a fixed three-simulation horizon. A test-only four-simulation route buys 1.5×, then 1.75×, then 2× at three separate shops; rebuilding the authored three-simulation shape keeps the starter fingerprint. This does not choose the production 7–10 node layout, and it does not define transitions for route kinds other than Simulation and Shop. Do not treat a direct modifier injection as proof of a third shop purchase.
6. **Resolved Keen Senses behavior:** Bevin chose longer-range predator detection. Keen Senses now adds a separate predator-detection range bonus; Long Vision remains a general vision-range Mutation. Focused simulation and offer-eligibility coverage passed on November 8. Mutation strength is provisional and still needs balance review.
7. **Resolved Reproductive Drive baseline:** Bevin chose a 0.5 Hare reproduction-chance baseline. The production Hare asset and authoring assertion now use 0.5; the existing Reproductive Drive chance bonus remains provisional and is separate from litter and gestation.

## Implementation dependencies

- Population Reserves rank research and save migration now use independent researched-rank data; the November 5 GLJP-02 handoff records the implementation and tests. Preserve the distinction between owned rank and the one equipment point.
- The Field Data allowance is a session-owned count of successful purchased Hares per simulation node. It does not limit births or survivors. Existing Population Pool capacity still serves legacy non-Field-Data replenishment and will need reconciliation when Population Reserves' full catalog migration lands.
- Duplicate signed species offers remain available to historical diagnostics and serialization fixtures, but are removed from active shop candidate generation. New shop candidates contain eligible income offers only.

## Evidence and handoff

Current results and ticket status are recorded in [WORKING_STATE.md](WORKING_STATE.md) and the dated GLJP handoffs. As of the November 7 shop UI changes, the latest isolated graphics PlayMode suite has 96 passed, 1 failed and 2 capture-dependent skips; the failure is the authored seed-10100 route stopping at tick 237. The captures were run separately: the synthetic shop case passed and the authored route case reproduced the failure. The latest full EditMode run was 744 passed and 1 skipped before the shop UI edits; the skipped parity case passed separately on two short current-source replays. A focused test or capture is not player visual or balance acceptance.

## November 9 build baseline direction

Bevin designated the November build as the canonical baseline. The October comparisons remain historical. GLJP-06 follows the current November route. After the Editor recovered, the controlled route completed all three shops at tick 2400 and the authored seed 10100 still failed at tick 1210. The survival requirement for the authored route remains open. [November route results](handoffs/2026-11-07-gljp06-route-preflight.md#november-9-controlled-route-completion)

## Catalog choices for closeout

The diagram supplied in the [morning review answers](handoffs/GLJP_Morning_Review_Answers.docx) groups research by the prices below. The [GLJP-01 review note](https://trello.com/c/dR2z6ENf) records these as proposed prices and asks whether the arrows represent required ownership or related upgrades.

| Hare Data in diagram | Research shown |
| ---: | --- |
| 10 | Sprint, Crowding Tolerance, Burrowing |
| 20 | Heightened Senses, Reproductive Drive, Shared Burrows |
| 30 | Escape Instinct, Seed Dispersal, Gestation Upgrade, Expanded Burrows, Population Reserves |
| 40 | Litter Upgrade ranks 1–3, Founding Hares ranks 1–3, Born Burrower |

- Founding Hares currently costs 5 Hare Data for one starter gene that adds five Hares at launch. Population Reserves already has five approved research prices of 5, 10, 15, 20 and 25 Hare Data. The diagram shows different prices and a ranked Founding Hares node.
- Treating the arrows as prerequisites would make Sprint a gate for Heightened Senses and Expanded Burrows a gate for Born Burrower. The current catalog requires Burrowing for Born Burrower. The Sprint terrain rule is still open.
- Faster Gestation still needs a temporary effect value. Increased Minimum Litter still needs its own rank limit. The approved three-rank Larger Litters behavior is implemented.
