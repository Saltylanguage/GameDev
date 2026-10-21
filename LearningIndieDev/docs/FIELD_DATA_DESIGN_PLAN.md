# Field Data design coverage and delivery checklist

**Reviewed:** 2026-10-15
**Canonical design:** [Field Data and Journey Loop Design v1.1](FIELD_DATA_JOURNEY_DESIGN.md)

**October 18 revision:** Node-end settlement has no population controls. Return
to the map after each simulation or shop and explicitly click the next ready
node to enter it. The PDD/SDD carry this approved update; earlier round-6
population-window decisions are superseded. Ordinary rounds 1–5 retain controls.

The Step 1–5 design review is complete for the first slice, including the five
settings confirmed during hardening. This file preserves every numbered item
from the decision process as a traceable work index. Read the standalone design
for the complete rules, content catalog, paper ledger and internal test protocol.

Checkboxes describe the action as written. A selected rule or deliberate
scope deferral can be checked; implementation, correctness tests and self-play
remain unchecked until actually performed. A complete design is not a completed
feature or a balance result. The delivery work is now selected for Active
[Sprint 5](Sprints/S5-control-record.md), October 19-November 15, 2026;
the original checklist IDs and evidence states remain unchanged.

Implementation planning now has two draft companions:
[Process Design](FIELD_DATA_PROCESS_DESIGN.md) for player/delivery flow and
[Solution Design](FIELD_DATA_SOLUTION_DESIGN.md) for proposed code/data/UI
integration. Their FD-I01–FD-I07 stages organize delivery without replacing the
original item IDs below. Neither draft marks delivery complete.

The [published ticket specification](FIELD_DATA_TICKET_DRAFT.md) maps pending delivery
to 16 selected S5 tickets; it preserves these IDs and leaves their checkboxes open
until the work and required evidence exist.

| Step | Design state | Delivery/evidence state |
| --- | --- | --- |
| 1 | Complete | Rule and example review only |
| 2 | Complete with provisional settings | Starter content not implemented/balanced |
| 3 | Complete | Synthetic arithmetic checked; no ecology validation |
| 4 | Scope and correctness requirements complete | Source audit complete; implementation/tests pending |
| 5 | Internal protocol complete | 200-seed runs and team acceptance pending; next content intentionally deferred |

## Step 1 — Round and purchase rules

- [x] **1.01** Start each new journey with **0 Field Data**. Bevin confirmed 2026-10-15.
- [x] **1.02** Allow population purchases **only between rounds**. Bevin selected option 1 on 2026-10-15.
- [x] **1.03** Apply purchased population changes **immediately upon purchase**, while paused between rounds. Bevin selected option 1 on 2026-10-15.
- [x] **1.04** Make **Add/Remove Hares, Add/Remove Foxes and Add/Remove Plants** available from the start of a journey **for now**, subject to Field Data and valid targets/space. Bevin confirmed 2026-10-15.
- [x] **1.05** Require **no unlocks for the current six controls for now**, as established by Bevin's decision that all six are available from journey start. Future controls or restrictions remain undecided.
- [x] **1.06** Automatically remove random individuals; reject overlarge removals and block removal of the final selected-species individual.
- [x] **1.07** Place additions at random valid available locations; reject the entire purchase without charge if the full addition cannot fit.
- [x] **1.08** Apply a newly acquired income upgrade from the **next round-end payout after acquisition**, without recalculating earlier payouts. Income upgrades are shop-only under the October 17 correction; the original October 15 multi-source decision is superseded.
- [x] **1.09** For ordinary rounds 1–5: pause and evaluate failure, award income, present the free reward choice, allow population purchases, then Continue. Nonterminal round-6 and terminal flow are defined in the standalone design.
- [x] **1.10** Evaluate selected-species extinction before payout. Failure skips the incomplete round payout/rewards, discards journey FD and preserves banked Desktop state.
- [x] **1.11** Award each successful final round its ordinary modified payout before node completion and the 10-FD clear bonus, including Simulation 3.
- [x] **1.12** Automatically award the provisional 10-FD bonus at each successful simulation clear, once, after its final-round payout; show a separate reward line.
- [x] **1.13** Require each distinct round payout and node-clear bonus to be awarded once for its event within the journey; repeated screens or Continue actions must not add it again. This is a correctness requirement derived from event-based awards, not a new reward mechanic. Runtime verification remains in 4.18.
- [x] **1.14** Apply per-individual bonuses and income multipliers to population-based round payouts, including successful final rounds, using the agreed activation timing. This follows the existing Field Data direction and 1.08/1.11; node-bonus scope is defined in 1.15, and future effect types need explicit scope.
- [x] **1.15** Allow modifiers for both population-based round payouts and node-clear Field Data bonuses. Each modifier must explicitly state which reward type it affects; a population-only modifier does not implicitly boost node bonuses. Bevin confirmed 2026-10-15.
- [x] **1.16** Flat per-individual bonuses are additive. Apply each at its position in the upgrade sequence, as clarified in 1.18; do not gather all flat bonuses ahead of interleaved multipliers. The earlier example base 1 + bonuses 1 and 2 = 4 per individual remains valid when those additions are consecutive. Example strengths are not approved balance values.
- [x] **1.17** Stack income multipliers by multiplying their effects together for compounding growth. Example: ×1.5 and ×2 produce ×3 when applied consecutively. Bevin confirmed 2026-10-15; example strengths are illustrative.
- [x] **1.18** Execute income operations in acquisition order until a paid shop reorder changes it. Keep the exact order; do not regroup operations by type. Reorder rules and prices are defined in R.01–R.09.
- [x] **1.19** Round down once at the end of each Field Data payout, after applying its full modifier sequence. Do not round intermediate upgrade operations; discard any fractional remainder after awarding that payout. Bevin confirmed 2026-10-15.
- [x] **1.20** Ordinary example: after a 100-tick round, 20 living Hares with no income upgrades award 20 FD; a starting wallet of 0 becomes 20.
- [x] **1.21** Example, upgraded payout: 20 Hares start at 1 Field Data each; a +1-per-Hare bonus gives 20 + 20 = 40, then ×1.5 gives 60 Field Data. Round down once after the full sequence; result is 60. Upgrade strengths are illustrative, not balance decisions.
- [x] **1.22** Successful-node example: final-round payout 20 plus the provisional 10-FD clear bonus adds 30 FD in total. More varied node rewards remain deferred.
- [x] **1.23** Example: a journey holds 30 Field Data and the Desktop already has 50 banked permanent currency. Failure clears the journey's 30 Field Data and gives no cash-out; the 50 already banked on the Desktop remains. Values are illustrative. Bevin confirmed 2026-10-15.

## Reordering — Shop-only income sequence edits

- [x] **R.01** Make both random and player-chosen income-upgrade reordering available **only at shop nodes**. Bevin confirmed 2026-10-15. Interaction and timing are defined in the standalone design.
- [x] **R.02** Start random reorder price at 10 FD. Escalate its unrounded basis by 10% after every successful paid reorder of either kind; round the random charge up. Reset at each new shop.
- [x] **R.03** Price a chosen reorder at exactly 10 times the current rounded random charge. The standalone design includes the formula and worked prices.
- [x] **R.04** Randomly reorder all acquired income-upgrade nodes in the sequence, and do not allow a shuffle that leaves the order unchanged. The option is available only when at least two upgrade nodes can be reordered. Bevin confirmed 2026-10-15.
- [x] **R.05** Let the player arrange the full sequence of acquired income-upgrade nodes at a shop, rather than limiting the action to a move or swap. Bevin confirmed 2026-10-15.
- [x] **R.06** Append each newly acquired income upgrade to the end of the current effective sequence, preserving the order already set. Bevin confirmed 2026-10-15.
- [x] **R.07** Apply an accepted reorder to the next payout that has not yet been settled; never recalculate past payouts. Bevin confirmed 2026-10-15.
- [x] **R.08a** Show a preview only for a player-chosen reorder; random reorders have no preview. Bevin confirmed 2026-10-15.
- [x] **R.08b** Execute and charge for a random reorder immediately when selected. For a player-chosen reorder, show the preview and cost, then charge only after the player confirms; cancellation does not charge. Bevin confirmed 2026-10-15.
- [x] **R.09** Label each income upgrade with the reward type it affects: population payouts, node-clear bonuses, or both. Bevin confirmed 2026-10-15.

## Step 2 — Starter purchases and rewards

- [x] **2.01** Source review: the current phase-decision control buys one Hare for a fixed 10 Currency per click, only while a Hare player is awaiting a phase decision; it requires enough balance and valid population space, adds the Hare immediately, then charges. The UI says it can be bought again while Data and space remain. This is existing prototype behavior, not an approved future Field Data price or quantity decision. Bevin reviewed 2026-10-15.
- [x] **2.02** Start with one Hare per purchase for now. Bevin confirmed 2026-10-15. Future idea: seed-deterministic purchase variations could add/remove Hares or Foxes, or multiply/divide their counts; exact operations and when they apply remain open.
- [x] **2.03** Start with a provisional price of **10 Field Data per Hare**. Bevin confirmed 2026-10-15. Current price is fixed; seed-deterministic sliding-scale pricing is deferred future work.
- [x] **2.04** Include both **Remove Fox** and **Add Plants** as ecological interventions in the first comparison. Bevin confirmed 2026-10-15.
- [x] **2.05** Test removing 1 random Fox or adding 5 Plants at random valid locations. These are initial quantities/effects for comparison, not final balance. Bevin confirmed 2026-10-15.
- [x] **2.06** Start both ecological interventions at a provisional **10 Field Data per purchase**. Bevin confirmed 2026-10-15. Future tuning or seed-determined sliding-scale prices remain an idea, not a settled rule.
- [x] **2.07** Use a provisional **+1 Field Data per Hare per payout** income upgrade. Bevin confirmed 2026-10-15; the value is a starting test value, not approved balance.
- [x] **2.08** Use a provisional ×1.5 income upgrade that affects future population payouts. Bevin confirmed 2026-10-15; the value is a starting test value, not approved balance.
- [x] **2.09** Include a larger journey upgrade that multiplies future population payouts by ×2, as an initial save-worthy comparison. Bevin confirmed 2026-10-15; effect strength is provisional.
- [x] **2.10** Price the larger ×2 upgrade at a provisional 50 Field Data for the first comparison. Bevin confirmed 2026-10-15; revisit with the budget examples.
- [x] **2.11** Allow repeat purchases of population interventions while funds and valid targets/space remain. Bevin confirmed 2026-10-15.
- [x] **2.12** Limit each distinct income-upgrade offer to one purchase; allow the same upgrade to stack when presented as a later distinct offer, including from a different reward source. Bevin confirmed 2026-10-15.
- [x] **2.13a** Offer five randomized, non-duplicated items at each shop, using equal weighting among eligible items; let players buy multiple distinct offers in one visit; make the ×2 income upgrade eligible starting at the second shop. Bevin confirmed 2026-10-15.
- [x] **2.13b** Define the first-playtest journey-persistent shop pool: six existing Hare-skill families with positive and negative versions, plus Field Data modifiers. Defer Fox/Plant-targeted and multispecies offers, and new effect types, until after testing the existing-skill variants. Population controls are not shop items. Additional content is later work. Bevin confirmed 2026-10-15.
- [x] **2.13c** Price +1 Field Data per Hare at 20 FD and ×1.5 population income at 30 FD; keep the ×2 upgrade at provisional 50 FD. These are comparison prices, not balanced values. Bevin confirmed 2026-10-15.
- [x] **2.13d** Use roughly 3× the original effect for stronger shop variants, keep the proposed names, and make negative versions reverse the same amount, respecting valid floors/caps. Include all six existing Hare-skill variations, each with a negative counterpart, in the first-playtest pool; Bevin confirmed on 2026-10-15. Values remain unbalanced sketches.
- [x] **2.13e** Let the same species-buff family stack additively up to 3 copies total across positive and negative versions; track this shared cap separately from the in-phase upgrade cap. Allow the opposite version to appear at a later shop, and apply both owned versions so their opposing amounts cancel. Do not offer both versions together in one visit; price them equally. If a capped version appears, allow a reroll. Bevin confirmed 2026-10-15.
- [x] **2.13f** Price each initial single-species buff at a provisional 30 FD, keep the price fixed across repeat purchases, and scale multispecies offers linearly by the number of targets. One multispecies purchase counts as one stack. Bevin confirmed 2026-10-15; these are comparison prices, not balanced values.
- [x] **2.13g** Let the player choose to reroll a capped buff offer, for free, once per capped offer in that shop visit. Draw replacements only from currently eligible items, excluding capped items and same-visit positive/negative conflicts. If fewer than five items are eligible, show fewer shop offers. Bevin confirmed 2026-10-15.
- [x] **2.13h** Make +1 FD per Hare and ×1.5 population-payout modifiers eligible in shops from the first visit. Keep ×2 shop-only, starting at the second visit. Field Data modifiers are positive-only in the first playtest and have no copy cap for now; the once-per-distinct-offer rule still applies. Bevin confirmed 2026-10-15.
- [x] **2.14** Offer +1 Field Data per Hare per payout in shops only, from visit one at 20 FD. The October 17 correction supersedes the October 15 round-reward source.
- [x] **2.15** Offer the ×1.5 population-payout multiplier in shops only, from visit one at 30 FD. The October 17 correction supersedes the October 15 node-reward source.
- [x] **2.16** Free round/node choices contain species upgrades only, with Skip available. Income and species offers compete for the same shop wallet; this October 17 correction supersedes the original mixed free-reward pool.
- [x] **2.17** Keep the complete six-control, income and signed species-offer catalogs in the standalone design, with effects, source, lifetime, repeatability and provisional prices.
- [x] **2.18** Use add/remove 1 Hare, add/remove 1 Fox and add/remove 5 Plants, each for 10 FD. Reject invalid purchases without charge/partial effect; block removal of the final selected-species individual.

## Step 3 — Paper journey budget

- [x] **3.01** Use the illustrative five-node sequence: Simulation → Shop → Simulation → Shop → Simulation.
- [x] **3.02** Place shops after the first and second simulation nodes so the example reaches the second-visit ×2 offer.
- [x] **3.03** Create the round ledger preserved in the standalone design Appendix A, including populations, payouts, node rewards, purchases and balances.
- [x] **3.04** Include a struggling-population path.
- [x] **3.05** Include a stable-population path.
- [x] **3.06** Include a growing-population path.
- [x] **3.07** Compare early purchase of ×1.5 at the first shop.
- [x] **3.08** Compare saving for ×2 at the second shop.
- [x] **3.09** Compare buying +1 FD/Hare at the first shop and ×2 at the second.
- [x] **3.10** Check whether the struggling path can afford a useful 10-FD intervention after its first payout.
- [x] **3.11** Check whether intervening still leaves a path to the 50-FD second-shop purchase.
- [x] **3.12** Estimate payback periods for the three income modifiers.
- [x] **3.13** Compare the same modifiers when offered at different remaining payout counts.
- [x] **3.14** Flag the mixed +1/Hare then ×2 path as a potential dominant compound-income strategy in stable/growing paths.
- [x] **3.15** Keep current prices as comparison inputs for a playable test; this paper-only ledger is not sufficient evidence to rebalance them.

## Step 4 — Implementation and correctness requirements

- [x] **4.01** Compare the current round-payout code with the agreed rules sheet.
- [x] **4.02** Compare the current currency balance and purchase code with the agreed rules sheet.
- [x] **4.03** Compare the current node-reward code with the agreed rules sheet.
- [x] **4.04** Compare the current failure, journey-transition and reset paths with the agreed rules sheet.
- [x] **4.05** List the specific implementation gaps found in those comparisons.
- [x] **4.06** Select a playable segment containing multiple payouts, intervention choices and a shop. Bevin selected the full five-node route in the standalone design for the first implementation slice on 2026-10-15.
- [ ] **4.07** Implement the agreed payout and wallet corrections needed by that segment.
- [ ] **4.08** Implement the six fixed-amount 10-FD population controls, random valid selection, atomic rejection and final-selected-individual guard.
- [ ] **4.09** Implement +1 FD per Hare, ×1.5 population payout and ×2 population payout upgrades.
- [ ] **4.10** Restrict all income modifiers to randomized shop offers; free round and node rewards contain species upgrades only.
- [ ] **4.11** Implement journey-persistent shop species buffs/nerfs so purchases take effect in the next simulation.
- [ ] **4.12** Display population counts for all species on each payout summary.
- [ ] **4.13** Display base payout, each modifier in execution order with a running total, and the final rounded award.
- [ ] **4.14** Display node-clear Field Data rewards separately from population income, then show the combined balance.
- [ ] **4.15** Update the visible Field Data balance after every income award and purchase.
- [ ] **4.16** Show each population purchase's cost and effect before selection; apply it immediately on click without a second confirmation.
- [ ] **4.17** Show the amount of journey Field Data lost on failure.
- [ ] **4.18** Verify round payouts and node-clear rewards are each granted once, including reopened results and repeated Continue actions.
- [ ] **4.19** Verify rejected population purchases leave both Field Data and the board unchanged.
- [ ] **4.20** Verify one Field Data balance carries across the complete five-node route and changes only through awards/purchases.
- [ ] **4.21** Verify income upgrades and shop species buffs/nerfs stay active through the current journey's end.
- [ ] **4.22** Verify each new journey starts at 0 FD and resets journey-local upgrades while retaining banked Desktop currency/unlocks.
- [ ] **4.23** Verify failure discards only remaining Field Data from the failed journey.
- [ ] **4.24** Verify Field Data operations leave banked Desktop currency and permanent unlocks unchanged.
- [ ] **4.25** Compare at least two modifier orders with identical population/upgrades to verify order-dependent payouts.
- [ ] **4.26** Verify upgrades from shop, round-reward and node-reward sources affect the next unsettled payout, never prior payouts.
- [ ] **4.27** Verify reorders preserve owned upgrades; random escalation/reset, exact 10× chosen price, preview/confirmation/cancel rules and exactly-once charges.
- [ ] **4.28** Preserve and verify acquisition history separately from effective execution order for reproducible reordered payouts.

## Step 5 — Internal self-play and refinement

- [ ] **5.01** Freeze a starting scenario and 200 explicit seeds; reuse the same setup and seed panel across all comparison arms.
- [ ] **5.02** Play a weaker-population path from the matched starting scenario and seeds.
- [ ] **5.03** Play a stronger-population path from the same starting scenario and seeds.
- [ ] **5.04** Record the route node, round/tick boundary and Field Data balance when each purchase first becomes affordable.
- [ ] **5.05** For every offer, record buy/skip, cost and remaining Field Data.
- [ ] **5.06** Record the Field Data balance on arrival at each shop, before purchases.
- [ ] **5.07** Record time between meaningful decision opportunities in real elapsed time and simulation ticks.
- [x] **5.08** Defer asking external players why they spend or save; this first playtest is internal team testing.
- [x] **5.09** Defer player teach-back interviews; assess payout and purchase clarity through internal self-play.
- [x] **5.10** Defer external-player purchase-behavior tracking, including whether Hare purchases dominate, until that research is in scope.
- [ ] **5.11** During team self-play, note examples where an income upgrade leads us to pass on a species upgrade or population intervention; use as a qualitative signal without a preset threshold.
- [ ] **5.12** Compare a 10-FD intervention path with saving toward the 50-FD ×2 upgrade on the same setup and seeds.
- [ ] **5.13** Select the next adjustment from findings. Multiple related economy values may change together; record the bundle and do not claim isolated per-variable causality.
- [ ] **5.14** Replay both weaker and stronger population paths for each adjustment, using the same starting setup and seed set.
- [ ] **5.15** Make the first-loop accept/revise decision from repeatable team self-play evidence and notes, without a numeric survival/population threshold or external-player research gate.
- [x] **5.16** Defer choosing the next content addition until the team completes self-play and reviews its findings.

## Later design work — Deliberately deferred

- [ ] **L.01** Design additional income-upgrade effects based on the first playtests.
- [ ] **L.02** Decide how random journey challenges award Field Data.
- [ ] **L.03** Design the separate permanent-progression reward system.
- [ ] **L.04** Decide whether successful journeys convert leftover Field Data.
- [ ] **L.05** If conversion is adopted, design its base calculation.
- [ ] **L.06** If conversion upgrades are adopted, design their effects and acquisition rules.
- [ ] **L.07** Design future handling of unconverted FD if success conversion is added. The current slice already discards leftover FD on success-result exit.

## Current handoff

The design phase through 5.16 is complete. Actual seed IDs and scenario/build
snapshots are execution-preparation records; the approved panel size is 200.
The next practical phase is to implement the defined route and pass its focused
correctness checks, then run internal comparisons. These activities are not
started or authorized by this documentation review alone.

The earlier source audit and complete synthetic paper ledger are preserved in
the standalone design appendices. Permanent reward currency, success conversion,
random challenges, broader species offers and the next content choice remain
explicit future work. Bevin leads design; Salty handles sprint planning.
