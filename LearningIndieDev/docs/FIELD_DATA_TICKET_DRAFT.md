# Field Data implementation tickets - Sprint 5

- **Prepared:** 2026-10-16
- **Status:** published to Trello; all 16 selected for Active S5, October 19-November 15, 2026; implementation not started
- **Planning baseline:** merged `BevBranch` at `dcd6bb0`
- **Owner / reviewer:** unassigned on every ticket, as Bevin requested
- **Estimates:** proposed active team-hours, including focused tests and ordinary review fixes; selected planning ranges; capacity risk recorded in the S5 control

## Outcome and sources

Deliver the five-node Field Data journey from the existing Desktop, preserving
one ecosystem and one wallet through three six-round simulations and two shops.
The player can earn, intervene, buy journey upgrades and reorder income, then
see correct success or failure results.

Use the [reviewed gameplay baseline](FIELD_DATA_JOURNEY_DESIGN.md) for rules,
the [PDD](FIELD_DATA_PROCESS_DESIGN.md) for process/delivery stages, the
[SDD](FIELD_DATA_SOLUTION_DESIGN.md) for proposed integration and the
[combined planning draft](FIELD_DATA_AND_SIMULATION_ALIGNMENT.md) for Salty's
related work and scope boundaries. The proposed type names are implementation
guidance, not a requirement to create one class for every ticket.

`FD-T01` through `FD-T16` are stable task references on the S5 Trello cards.
Every ticket starts Not started with no owner/reviewer, as requested. Its
acceptance boxes below describe future evidence; none is checked by writing
this document. A receipt means a recorded award or purchase with its identity,
amount and resulting balance.

## Ticket index and dependencies

| Draft ID | Actionable outcome | PDD stage | Depends on | Proposed hours |
| --- | --- | --- | --- | ---: |
| FD-T01 | Agree the test-route entry and first implementation boundaries | FD-I01 | Team review | 1–2 |
| FD-T02 | Freeze and validate the starter journey definition | FD-I01 | FD-T01 | 3–5 |
| FD-T03 | Calculate ordered income with a complete breakdown | FD-I04 | FD-T02 | 3–5 |
| FD-T04 | Run all 18 rounds in one retained journey session | FD-I02 | FD-T02 | 4–7 |
| FD-T05 | Settle awards once and enforce the journey wallet lifecycle | FD-I02 | FD-T03, FD-T04 | 4–6 |
| FD-T06 | Add the six atomic population controls | FD-I03 | FD-T05 | 4–7 |
| FD-T07 | Apply the six signed Hare shop-effect families | FD-I04 | FD-T02, FD-T04 | 5–8 |
| FD-T08 | Offer and resolve free round/node rewards | FD-I04 | FD-T05 | 3–5 |
| FD-T09 | Generate and purchase deterministic shop offers | FD-I04 | FD-T05, FD-T07 | 4–7 |
| FD-T10 | Buy random or chosen income reorders | FD-I04 | FD-T09 | 3–5 |
| FD-T11 | Show round income, rewards and population actions in Noesis | FD-I05 | FD-T06, FD-T08 | 4–6 |
| FD-T12 | Show shops and reorder controls in Noesis | FD-I05 | FD-T10, FD-T11 | 4–7 |
| FD-T13 | Author the test route and verify the complete Desktop flow | FD-I05 | FD-T01, FD-T02, FD-T11, FD-T12 | 3–5 |
| FD-T14 | Run the same journey domain headlessly and export replay evidence | FD-I06 | FD-T13 | 6–10 |
| FD-T15 | Freeze and run the matched 200-seed comparison | FD-I07 | FD-T14 | 3–5 |
| FD-T16 | Review internal self-play and record accept/revise | FD-I07 | FD-T15 | 3–5 |

The selected tickets total **57–95 active team-hours**, before unknown machine
time or a new tuning iteration. Bevin selected all 16 for a four-week S5 after
the over-capacity warning. The [S5 control](Sprints/S5-control-record.md)
records the 80–120h combined availability, protected 16h reserve, conservative
64h feature allowance and 31h upper-estimate overage. Re-estimate after FD-T01
and each implementation review.

The original **FD-T01–FD-T05** recommendation remains a useful 15–25h foundation
sequence within the full selected scope.
It produces a domain fixture that can exercise the continuous route and its
awards. It does not deliver the complete player-facing shop journey. FD-T03 and
FD-T04 can proceed independently after FD-T02; FD-T07 can follow FD-T04 while
other domain work proceeds. Agree shared-file ownership before parallel edits.

## Published Trello index - 2026-10-16

[Active Sprint 5 control](https://trello.com/c/hqQczLQ9/135-sprint-5-control-record-field-data-journey-implementation) |
[Repository control and capacity](Sprints/S5-control-record.md)

| Card | Workflow list at kickoff | Execution state |
| --- | --- | --- |
| [FD-T01](https://trello.com/c/kl9F3o2l/119-fd-t01-agree-the-test-route-entry-and-first-implementation-boundaries) | Current Work | Not started |
| [FD-T02](https://trello.com/c/3ZTf7hmC/120-fd-t02-freeze-and-validate-the-starter-journey-definition) | Current Work | Not started |
| [FD-T03](https://trello.com/c/kdncn6mH/121-fd-t03-calculate-ordered-income-with-a-complete-breakdown) | Current Work | Not started |
| [FD-T04](https://trello.com/c/LKzm9j7r/122-fd-t04-run-all-18-rounds-in-one-retained-journey-session) | Current Work | Not started |
| [FD-T05](https://trello.com/c/3cGXubBh/123-fd-t05-settle-awards-once-and-enforce-the-journey-wallet-lifecycle) | Current Work | Not started |
| [FD-T06](https://trello.com/c/CMld0U0x/124-fd-t06-add-the-six-atomic-population-controls) | Current Work | Not started |
| [FD-T07](https://trello.com/c/LFKRoQpN/125-fd-t07-apply-the-six-signed-hare-shop-effect-families) | Current Work | Not started |
| [FD-T08](https://trello.com/c/XB4jwxit/126-fd-t08-offer-and-resolve-free-round-node-rewards) | Current Work | Not started |
| [FD-T09](https://trello.com/c/Hrl7GNAo/127-fd-t09-generate-and-purchase-deterministic-shop-offers) | Current Work | Not started |
| [FD-T10](https://trello.com/c/C9D3evir/128-fd-t10-buy-random-or-chosen-income-reorders) | Current Work | Not started |
| [FD-T11](https://trello.com/c/GZFJ6QPo/129-fd-t11-show-round-income-rewards-and-population-actions-in-noesis) | Current Work | Not started |
| [FD-T12](https://trello.com/c/J2EqiGv3/130-fd-t12-show-shops-and-reorder-controls-in-noesis) | Current Work | Not started |
| [FD-T13](https://trello.com/c/FGFDVXcQ/131-fd-t13-author-the-test-route-and-verify-the-complete-desktop-flow) | Current Work | Not started |
| [FD-T14](https://trello.com/c/XB3Yc2j7/132-fd-t14-run-the-same-journey-domain-headlessly-and-export-replay-evidence) | Current Work | Not started |
| [FD-T15](https://trello.com/c/nnujHAoK/133-fd-t15-freeze-and-run-the-matched-200-seed-comparison) | Current Work | Not started |
| [FD-T16](https://trello.com/c/osmzcZVf/134-fd-t16-review-internal-self-play-and-record-accept-revise) | Current Work | Not started |

## Ticket descriptions

### FD-T01 — Agree the test-route entry and first implementation boundaries

**Outcome:** A developer knows how the five-node Field Data test is launched
from the canonical Desktop and which existing behaviors to preserve.

Record the selected map/definition entry and review the SDD's explicit opt-in
composition. Identify the current ecology revision used during development.
Keep the 29-marker content map and standalone six-round route usable. Resolve
how the first-test journey is named in planning without rewriting the overall
roadmap contract by implication.

- [ ] Record the launch path and map/definition selection with Bevin and Salty's review.
- [ ] Confirm the five-node/18-round acceptance route and the existing map/standalone compatibility boundary.
- [ ] Record Field Data as this ticket set's scope; permanent rewards, Genome effects and ecology corrections remain separate unless explicitly selected.
- [ ] Identify any technical proposal needing revision and update the PDD/SDD accordingly; no code is needed to close this decision ticket.

**Trace:** FD-I01; PDD P-01/P-02; SDD §§3–5/11; checklist 4.06.

### FD-T02 — Freeze and validate the starter journey definition

**Outcome:** One validated, immutable definition contract supplies route,
catalog, price, quantity, reward-cadence and eligibility data for a run.

Implement the definition/authoring boundary and stable identities described in
the SDD. Resolve values once at launch; keep runtime balance, offers and owned
upgrades out of shared assets. Author the current six interventions and three
income entries, with the explicit data shape for signed Hare offers. FD-T07
finishes the twelve signed entries after their runtime mappings are available;
an unmapped entry cannot enter a run in the meantime.

- [ ] Capture the exact starter rules, 100-tick rounds and six-round nodes, including shop eligibility from visit 1 or 2.
- [ ] Reject invalid routes, costs, duplicate IDs and unsupported effects/targets before a journey starts.
- [ ] Produce versioned/fingerprinted resolved inputs; later asset edits cannot alter an active run.
- [ ] Preserve serialized identities and existing enum values; append Shop only where the selected integration requires it.
- [ ] Add focused authoring/definition tests and preserve legacy definitions when the new mode is not selected.

**Trace:** FD-I01; SDD §§4/11; baseline Steps 1–2 and checklist 4.09–4.11.

### FD-T03 — Calculate ordered income with a complete breakdown

**Outcome:** Given closing populations and an ordered income list, calculate
the exact award and numerical steps used to explain it.

Implement +1 FD per Hare, ×1.5 and ×2 as population-payout operations. Preserve
instance order and compound multipliers; floor only the final amount. Keep the
10-FD node bonus outside this calculation. Do not award currency in the UI.

- [ ] With 20 Hares, +1/Hare then ×2 yields 80 FD; reversing those operations yields 60 FD.
- [ ] Verify one final floor with a fractional fixture, rather than rounding every step or carrying a remainder.
- [ ] Return the base count, ordered operations, intermediate totals and final award without changing a wallet.
- [ ] Reject out-of-range/invalid arithmetic without wrapping a balance or adding an unapproved gameplay cap.
- [ ] Add focused calculation tests; taking a modifier never recalculates an already-settled payout.

**Trace:** FD-I04; PDD P-03; SDD §6; checklist 4.09, 4.13, 4.25–4.26.

### FD-T04 — Run all 18 rounds in one retained journey session

**Outcome:** One session coordinates the complete route without recreating the
ecosystem, wallet or progression at shops.

Add the feature-local session and its allowed flow states around the existing
SimulationManager/runner and SpeciesProgression. Keep the current preview as
the sole Unity clock forwarder. Make boundaries and terminal completion reach
the session through explicit composition, bypassing competing legacy flow in
the selected mode.

- [ ] Traverse rounds 1–18 at ticks 100–1800; pause for both shops at 600 and 1200.
- [ ] Preserve the runner, board, identities, resource/creature state, history, Genome snapshot and retained progression through both shops.
- [ ] Tick 1800 reaches the same final-round handling even if the runner reports Complete instead of a decision boundary.
- [ ] Decisions/shops do not advance time; illegal or stale transitions cannot resume or create another journey.
- [ ] Verify subscription cleanup/rebinding and legacy route compatibility with focused lifecycle fixtures.

**Trace:** FD-I02; PDD P-01/P-02; SDD §§3–5; checklist 4.20–4.21.

### FD-T05 — Settle awards once and enforce the journey wallet lifecycle

**Outcome:** A successful round pays once; failure, success exit and a fresh
journey handle Field Data correctly without changing permanent progression.

Use one session-owned settlement path for boundary and completion callbacks.
Retain separate population and node-clear receipts. Reuse the merged reset
behavior where compatible and bypass the old preview's competing award paths.

- [ ] New journeys start at 0 FD; population income settles once and each of the three successful node clears adds a separate 10 FD.
- [ ] Reopening results/repeating Continue does not repay; a retry after population settlement can finish a missing node bonus without resetting choices.
- [ ] Natural extinction at ticks 100, 600 or 1800 fails before that round's payout/choice/bonus; incomplete-route End/abandon forfeits the remaining balance.
- [ ] Success shows leftover FD and discards it on results exit; no permanent cash-out or conversion is added.
- [ ] Test that banked currency, species/Genome unlocks and feature IDs are unchanged; legitimate existing profile time accounting remains allowed.

**Trace:** FD-I02; PDD P-03/P-06/P-08; SDD §§5–6/11; checklist 4.07, 4.18, 4.22–4.24.

### FD-T06 — Add the six atomic population controls

**Outcome:** Between rounds, a paid add/remove request either changes the full
requested population and wallet together, or changes neither.

Add/remove 1 Hare, add/remove 1 Fox and add/remove 5 Plants, each for 10 FD.
Use deterministic random valid positions/targets. Respect separate creature
and plant-resource cell contents and update the boundary/previous grid safely.

- [ ] All six operations support repeat purchases in legal windows, including nonterminal round-6 results before shops.
- [ ] Reject insufficient FD, too few removal targets, insufficient full-batch space and removal of the last selected-species individual without partial changes or charges.
- [ ] Plant removal preserves a creature sharing that cell; creature removal preserves its underlying resource/terrain.
- [ ] Rejected actions leave IDs/random ordinals unchanged; repeated deliberate clicks are distinct purchases while retries of one request cannot charge twice.
- [ ] Preserve the closing population used for settled income; record interventions separately from natural births/deaths/kills.

**Trace:** FD-I03; PDD P-05; SDD §7; checklist 4.08, 4.19; baseline purchase rules.

### FD-T07 — Apply the six signed Hare shop-effect families

**Outcome:** Positive and negative shop species effects persist through the
journey, cancel correctly and coexist with ordinary upgrades.

Implement net signed overlays for Long Stride, Sharper Instinct, Reinforced
Hide, Warren Comfort, Deep Digestion and Abundant Seedfall using the baseline's
exact amounts. Resolve against ordinary progression before final bounds. Add
the explicit signed avoidance registry/options mapping required by the SDD.

- [ ] All twelve variants affect the intended attribute; no display-name/reflection mapping or negative value through the old positive-only constructor.
- [ ] Opposing copies cancel against the base even at bounds; current energy, cooldowns and creature state are not reset/refilled.
- [ ] A shared three-copy family cap spans signs and future target metadata, separately from ordinary upgrade levels; cancellation does not refund a copy.
- [ ] Shop effects begin in the next simulation, remain active for later nodes and reset at journey end.
- [ ] Registry, runtime applier, fingerprint and telemetry mapping agree in focused tests before assets enter a run, preserving [UPG-C01–C04/C06](Planning%20Concerns/upgrade-system.md).

**Trace:** FD-I04; PDD P-07; SDD §8; checklist 4.11, 4.21.

### FD-T08 — Offer and resolve free round/node rewards

**Outcome:** Each eligible break presents a stable pick-one free choice with
Skip, using the approved source pool and activation timing.

After rounds 1–5 of each simulation, mix ordinary upgrades with eligible
+1 FD/Hare. After Simulation 1 and 2 clear, mix ordinary upgrades with eligible
×1.5. Keep ×2 shop-only. Reuse existing ordinary upgrade behavior and caps.

- [ ] Draw three distinct eligible options or fewer if needed, deterministically; income remains eligible rather than guaranteed.
- [ ] Choosing one resolves the source once; Skip grants no award or penalty. Reopening does not generate a fresh choice.
- [ ] Source rewards cost no FD; newly acquired income appends to current execution order and first affects the next unsettled payout.
- [ ] Income has no copy cap; ordinary upgrades retain their existing cap and separate state.
- [ ] No free reward is offered after terminal success; test round/node timing and replay-safe resolution.

**Trace:** FD-I04; PDD P-04/P-06; SDD §§4/9; checklist 4.10, 4.26.

### FD-T09 — Generate and purchase deterministic shop offers

**Outcome:** Each shop retains its seeded offers, handles purchases once per
offer, and lets the player optionally reroll capped species offers.

Draw five distinct equal-weight legal offers from the starter pool: signed
Hare effects and income, with ×2 eligible only at shop 2. Price species offers
at 30 FD and income at 20/30/50 FD. Keep population controls outside shops.

- [ ] A visit never displays both signs of the same family; re-entering preserves offers, purchases and exclusions.
- [ ] Allow multiple distinct purchases per visit; later distinct offers can stack within their applicable cap.
- [ ] Capped species offers may appear locked; buying is disabled and one optional free reroll is available per capped offer/visit.
- [ ] Replacements exclude capped, duplicate, conflicting or ineligible items, including conflicts with already-purchased same-visit offers; use fewer offers if necessary.
- [ ] Test funds/stale-intent rejection and receipt-based exactly-once spending; preserve independent deterministic offer/action streams.

**Trace:** FD-I04; PDD P-07; SDD §9; checklist 2.13a–2.13h, 4.10–4.11.

### FD-T10 — Buy random or chosen income reorders

**Outcome:** Shop actions change future income execution order while retaining
owned instances and the original acquisition history.

Implement immediate random shuffle without preview and chosen full-permutation
preview/confirm/cancel. After either successful paid reorder, increment the
visit's price step: random is ceil(10 × 1.1^k), chosen is exactly 10 times it.

- [ ] Random requires at least two instances and changes their order; same-effect copies remain distinct instances.
- [ ] Chosen preview leaves committed state untouched; confirmation validates a complete current permutation and funds; cancel costs nothing.
- [ ] Verify random/chosen charges 10/100, 11/110, 13/130 and 14/140, with reset at the next shop.
- [ ] Rejections/cancellation do not advance prices; retrying one accepted intent does not charge twice.
- [ ] Keep acquisition and effective/reorder histories separately; later acquisitions append to current order and no prior payout changes.

**Trace:** FD-I04; PDD P-07; SDD §§6/9; checklist R.01–R.09, 4.27–4.28.

### FD-T11 — Show round income, rewards and population actions in Noesis

**Outcome:** The player can understand the payout and use the round-break
actions from the canonical simulation screen.

Compose the journey helper explicitly and project read-only facts into the
existing shell ViewModel. Show all species counts, selected count, ordered
income steps, separate node bonus and current balance. Bind reward/Skip,
population controls, Continue and terminal results to authoritative commands.

- [ ] Display cost/effect before clicking; a population purchase applies on one click and balance updates after every transaction.
- [ ] Population controls appear only in allowed windows, including pre-shop round-6 results, and disappear after terminal success.
- [ ] The free choice resolves before population buying; selected income effects are clear for the next payout.
- [ ] Failure shows FD lost; success shows leftover FD until exit. A stale/disabled command is rejected in the domain too.
- [ ] Add focused Noesis integration and rendered checks; preserve board rendering/zoom/pan and one clock forwarder. No economy calculations move into XAML/ViewModel.

**Trace:** FD-I05; PDD P-03–P-06/P-08; SDD §10; checklist 4.12–4.17.

### FD-T12 — Show shops and reorder controls in Noesis

**Outcome:** The player can buy eligible offers, reroll capped items and use
both reorder modes without losing their place or accidentally spending twice.

Add the shop presentation and an uncommitted chosen-reorder draft to the
existing shell. Show prices, effects, ownership/cap state and effective income
order. Let the session decide which commands are currently allowed.

- [ ] Purchase/reroll state and the visible wallet update immediately; population controls are not shop cards.
- [ ] Random reorder has no preview or confirmation; chosen shows full order, preview/cost and confirm/cancel.
- [ ] Cancellation/reopening preserves the committed sequence, wallet, offers and current shop price step.
- [ ] Repeated binding/open/close cannot add duplicate subscriptions, purchases or clock advancement.
- [ ] Focused runtime/visual checks cover both shops, affordability, capped offers, reroll exhaustion and reorder confirmation/rejection.

**Trace:** FD-I05; PDD P-07; SDD §10; checklist 4.13, 4.15, 4.27.

### FD-T13 — Author the test route and verify the complete Desktop flow

**Outcome:** The agreed entry launches the real Simulation → Shop →
Simulation → Shop → Simulation route from the Desktop.

Wire the reviewed definition/map and explicit helper composition into the
player entry. Preserve the accepted scrolling map/panorama and old route when
not selected. Exercise a complete journey plus failure and a fresh-journey reset.

- [ ] Play all 18 rounds and both shops through the actual UI with the same board/progression; no developer fixture substitutes for the final acceptance route.
- [ ] Verify ordinary round, nonterminal round-6 and final-round order, repeated results/Continue, shop re-entry and lifecycle cleanup.
- [ ] Check natural failure, manual abandonment, success exit and a new journey against protected profile currency/unlock/feature state.
- [ ] Run the relevant integrated suites and graphics checks; retain captures and record independent human accept/revise without counting automated tests as human approval.
- [ ] Update the PDD/SDD, checklist and handoff from actual delivery/evidence; keep unperformed checks open.

**Trace:** FD-I05; PDD P-01–P-08; SDD §§10–11/13; checklist 4.06, 4.18–4.28.

### FD-T14 — Run the same journey domain headlessly and export replay evidence

**Outcome:** The existing standalone tool can execute the same journey and
action schedule, producing evidence that matches bounded Unity fixtures.

Add the required shared sources to compilation and embedded provenance, then
extend current runner entry points with opt-in definition/actions. Export a
versioned Field Data section rather than copying economy logic into a new tool.

- [ ] Replay stable offer/action/purchase/reorder identities with matching receipts, execution order and board outcomes in Unity and headless fixtures.
- [ ] Export inputs/fingerprints, all-species payout counts, income breakdowns, offers, buy/skip/cost/balance, shop-arrival balance, terminal status and acquisition/effective histories.
- [ ] Record first affordability by node/round/tick/balance and decision timing units; distinguish automated runtime from human deliberation time.
- [ ] Preserve rejected/crashed/incomplete statuses, separate interventions from natural ecology counters and retain old report meanings.
- [ ] Demonstrate deterministic replay and report-schema compatibility before claiming support for the 200-seed economy panel.

**Trace:** FD-I06; SDD §§9/12/13; checklist 4.28, 5.04–5.07.

### FD-T15 — Freeze and run the matched 200-seed comparison

**Outcome:** A reproducible comparison bundle supplies actual evidence for the
first Field Data review, with matched inputs across strategy arms.

Before execution, freeze 200 explicit seed IDs, scenario/Genome/ecology/build/
definition snapshots and declared action policies. Specify weaker/stronger
paths and the 10-FD intervention versus saving-for-50-FD ×2 comparison. Keep
natural offer availability rather than silently forcing a desired shop draw.

- [ ] Run the same 200 seeds per arm from fresh matched inputs; preserve the continuous world inside each journey.
- [ ] Record real offers, unavailable strategy opportunities, receipts and complete/stopped/crashed statuses without silently replacing missing runs.
- [ ] Retain a manifest, objective per-run records and paired summaries using the approved affordability/spending/timing measures.
- [ ] Keep ecology fixed across economy arms. Any chosen ecology revision is separately validated and frozen before the comparison.
- [ ] Report active setup/analysis effort separately from measured machine time; no assumed throughput or survival acceptance threshold.

**Trace:** FD-I07; PDD §7; SDD §12; checklist 5.01–5.07, 5.12, 5.14.

### FD-T16 — Review internal self-play and record accept/revise

**Outcome:** The team can explain whether the loop works well enough to keep
or needs a bounded revision, based on repeatable evidence and direct self-play.

Play the matched weaker/stronger choices, review earning versus saving and
look for income purchases crowding out useful species/population actions.
Combine observations with FD-T15's records. This is internal testing, without
external interviews, teach-back requests or behavioral-research requirements.

- [ ] Record team observations, real decision time and simulation ticks; do not substitute automated runtime for player deliberation.
- [ ] Review intervention versus saving, payout/effect clarity and concrete opportunity-cost examples without inventing a numeric pass gate.
- [ ] Record accept/revise and its evidence. If revise, identify the related price/effect bundle and a bounded follow-up rather than leaving an open-ended tuning ticket.
- [ ] A revision requires matched replays through FD-T15 on both weaker/stronger paths; do not claim isolated causality for a multi-value bundle.
- [ ] Choose the next content addition only after findings exist; permanent economy, Fox/Plant/multispecies shop effects and random events remain separate decisions.

**Trace:** FD-I07; PDD §7; baseline Step 5; checklist 5.02–5.03, 5.08–5.16.

## Shared completion and scheduling rules

Each implementation ticket includes its focused correctness tests and a concise
handoff. FD-T13 verifies integrated behavior; FD-T14 proves replay/report parity;
FD-T15/16 evaluate the economy. Passing one layer does not complete the others.
Use the [documented validation workflow](UNITY_PLUGIN_WORKFLOWS.md) and record
actual passed/failed/skipped results, revision, artifacts and remaining limits.

Reuse the merged profile storage, journey reset and scrolling map. Retain
P1-032's separate launch-input/real exit-relaunch acceptance; it is not a new
profile implementation hidden in this backlog. Do not add the deferred
crowding/movement/litter changes or permanent reward economy to these tickets.

All 16 tickets are selected for S5. Allocate owners/reviewers during team
review; leaving them unassigned at kickoff is Bevin's explicit exception.
Resolve FD-T01, review technical proposals against the current code, and retain
the recorded reserve and capacity risk. Kickoff and card creation do not satisfy
any implementation, runtime, balance or self-play acceptance criterion.

## Requirement coverage

| Requirement group | Draft tickets |
| --- | --- |
| Scope and immutable inputs | FD-T01, FD-T02 |
| Continuous journey, wallet and protected permanent state | FD-T04, FD-T05, FD-T13 |
| Ordered income and source activation | FD-T03, FD-T08, FD-T09 |
| Six population controls and rejection | FD-T06, FD-T11 |
| Signed shop effects, caps and offers | FD-T07, FD-T09, FD-T12 |
| Reorder pricing, interaction and histories | FD-T10, FD-T12, FD-T14 |
| Payout/results display and full route | FD-T11, FD-T12, FD-T13 |
| Checklist 4.07–4.28 correctness/delivery | FD-T03–FD-T14, with item traces in each body |
| Checklist 5.01–5.16 comparison/review | FD-T14, FD-T15, FD-T16 |

## Historical draft review evidence — 2026-10-16, before publication

The draft was checked for unique IDs, valid dependencies without cycles,
estimate sums, links and coverage of the seven PDD stages. It preserves the
original numbered design checklist. No implementation, test suite, comparison
run, board edit, commit or push is claimed by this ticket-planning work.
