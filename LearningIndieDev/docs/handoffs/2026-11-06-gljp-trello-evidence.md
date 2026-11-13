# GLJP Trello evidence checkpoint - November 6, 2026

The live Trello board was read back on November 6 after the user requested GLJP ticket updates. Existing descriptions and list placement were preserved. All work remains local and uncommitted; no push.

- [GLJP-01](https://trello.com/c/dR2z6ENf): added and read back a research-gate/catalog progress comment, including provisional prices, focused tests, missing current PlayMode result and remaining content. Attached the inspected `gljp-provisional-litter-research.png` from `artifacts/visual-evidence-20261106-163739`.
- [GLJP-02](https://trello.com/c/o4q7Necn): added and read back the November 5 rank/migration/capacity evidence and attached `gljp-population-reserves-rank-5.png` from `artifacts/visual-evidence-20261105-204015`. The card remains complete in Done as previously recorded.
- [GLJP-03](https://trello.com/c/h4wxvWAp): added and read back allowance, payout-tier and test-route evidence; attached the inspected November 1 `gljp-hare-purchases-remaining.png`. Its comment dates the older screenshot and states it is not current-revision visual acceptance. The card remains open in Current Work.
- [GLJP-04](https://trello.com/c/DQwFKLSF): added and read back a status comment identifying the missing mechanic and the unresolved threat, emergence, and multiple-burrow decisions. No burrow implementation or runtime acceptance is claimed.
- [GLJP-05](https://trello.com/c/Htt7PJBY): added and read back integration test/capture evidence and remaining validation, and attached `gljp-crowding-choice.png` from `artifacts/visual-evidence-20261106-112451`. The card remains open.

The four uploaded images were inspected and their attachment names read back in Trello. Trello automatically set each uploaded image as a card cover; the automatic covers were removed without removing the attachments. The board screenshot `artifacts/gljp-trello-evidence-20261106.png` records the completed GLJP-02 card and attached image. A further GLJP-03 comment records the passing four-simulation standalone replay; its read-back appears in `artifacts/gljp-trello-shop-20261106.png`. No other card was closed or moved. Full PlayMode on the latest catalog revision is still missing because the Editor pipeline disconnected; the latest completed full run was on an earlier revision and is described on the cards.

Later November 6: [GLJP-03](https://trello.com/c/h4wxvWAp) received a read-back comment with the isolated one-round allowance test and newer capture on GLJP-05, the full graphics PlayMode **94 / 1 / 2** result, and the remaining seed-10100 failure and evidence limits. The card remains in Current Work. `artifacts/gljp-trello-shop-latest-20261106.png` records the update; the earlier paragraph above is retained as its dated historical checkpoint.

The newer inspected allowance capture was also attached directly to GLJP-03 as `gljp-hare-allowance-after-purchase-20261106.png`, verified on the card on November 7. Its local copy matches the original focused-test capture by SHA-256 `9b36a4764a7c38623613e177a84ab92637be20f7ec78101d8ca6dfa5271448fd`; the attachment menu offered “Make cover,” confirming it was not automatically used as a cover. `artifacts/gljp-trello-allowance-capture-20261107.png` records the card. The capture remains isolated test evidence, not player acceptance.

## November 8, 2026 update

After the user decided to keep ordinary Fox movement-scent pursuit off by default, additive progress comments were posted and read back on [GLJP-03](https://trello.com/c/h4wxvWAp), [GLJP-05](https://trello.com/c/Htt7PJBY), and [GLJP-06](https://trello.com/c/eo0iqoRL). All three remain open in their existing lists. The comments record the four-simulation/three-shop authored test route, the 1.75x payout tier, focused Unity evidence, and the route survival issue. Current full PlayMode was 96 passed, 1 failed, 2 skipped; the sole failure is the fixed-seed 10100 route survival check at tick 1130 with movement-scent defaulted off. No successful completion or balance conclusion is claimed. GLJP-05 also records the four November 8 Gene Lab and journey captures under `artifacts/visual-evidence-20261108-101001/` and `artifacts/visual-evidence-20261108-101016/`. No card was moved or closed.

## November 8 GLJP-03 closeout evidence

The current full EditMode suite passed **744 / 0 / 1** (`artifacts/unity-tests-20261108-102811/EditMode-results.json`). GLJP-03 acceptance coverage passed for gross Hare allowance and next-node reset, rank-derived limits, one-time sequential tiers at separate shops, and future-payout-only modifier timing. The focused PlayMode visual case for the Hare allowance passed separately; the current income-only shop run passed **1/1** on November 8 (`artifacts/visual-evidence-20261108-104330/PlayMode-results.json`). Its inspected offer and purchased screens are `gljp-income-shop-offer.png` and `gljp-income-shop-purchased.png`. The only full PlayMode failure remains the independent authored-route ecology issue tracked under GLJP-06; it does not fail the shop acceptance tests. Based on the documented card acceptance, GLJP-03 was marked complete, moved to Done, and read back there; its closeout comment is [recorded on Trello](https://trello.com/c/h4wxvWAp#comment-6af09908bb0aaa9bb78d350f). The 7–10 node production placement and final balance remain open and are not approved by this closeout.

## November 8 GLJP-05 research source caption

Field Data Mutation cards now show UNLOCKED IN GENE LAB above THIS JOURNEY ONLY when the journey uses research captured from the player's profile. The retained progression carries that source with its research filter. Profileless and synthetic filter runs show only the lifetime caption.

The fresh-profile fixture starts with no Mutation offers, buys the research through Gene Lab, then resets the loadout. It checks that the node remains owned and unequipped, that the eligible card renders both labels, and that choosing the Mutation activates it in that journey. The same flow passed for Burrowing and Seed Dispersal. The synthetic filter in the existing route regression verifies the lifetime caption without a Gene Lab attribution.

- Unity compilation completed with zero errors and warnings.
- Burrowing visual PlayMode passed at 1280×720 and 1920×1080. The inspected capture sets are `artifacts/visual-evidence-20261108-232451` and `artifacts/visual-evidence-20261108-232503`.
- Seed Dispersal integration and the test route entry, economy and empty reward regression passed 1/1 each in `artifacts/unity-tests-20261108-232349` and `artifacts/unity-tests-20261108-232401`.
- Field Data journey session EditMode passed 28/28 in `artifacts/unity-tests-20261108-232515`.

Player review and the remaining catalog decisions are open. The fixed-seed full route survival result stays tracked under GLJP-06.

## November 9 shop UI and payout tiers

Shop summaries show the offer count, with singular wording when only one tier is available. The visual PlayMode fixture uses a stable test population and buys 1.5×, 1.75× and 2× through the shop commands at the first, second and third shops. Each later purchase charges its displayed cost, keeps the settled awards, and replaces the active tier. The payout receipts verify that the new tier applies from the following round. The route reaches the Expedition complete screen at tick 2400.

The visual fixture passed in `artifacts/visual-evidence-20261109-110626`. The 1600×900 captures include `gljp-income-shop-offer.png`, offer and purchased states at shops two and three, and `gljp-income-route-success.png`. The separate journey-map and shop entry regression passed in `artifacts/unity-tests-20261109-110649`. Player review and the remaining catalog decisions are open on GLJP-05. Authored route survival is tracked separately on GLJP-06. [Trello readback](https://trello.com/c/Htt7PJBY#comment-6af1efcb3082d0d54a229e1c)
