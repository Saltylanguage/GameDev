# Field Data and Journey Loop Design

- **Version:** 1.1
- **Design reviewed:** 2026-10-17
- **Design owner:** Bevin
- **Scope:** first playable Field Data journey and its internal test protocol

This is the standalone design baseline produced by the Step 1–5 decision
review. It defines the rules, starter content, paper budget, implementation
requirements and internal test plan in one place. The decisions and provisional
test settings are complete for this slice. Implementation evidence exists for
the approved route and October 17 correction, with fresh Unity regression and
rendered checks on October 18. Independent human review remains open, and the
new loop is not yet balance-approved.

### October 17 behavior correction

Selecting the Field Data test route from the larger Desktop map opens its own
five-node route preview with visible connected node cards and current-node
highlighting. Simulation ticks begin only after a separate Start
action. During the active journey, the route map can be opened from simulation,
round-decision and shop screens, then closed to return to the same run state.
Map controls are also available inside the decision and results panels.
All Field Data income modifiers are shop-only. Free round and node rewards may
offer species upgrades, but never income modifiers. Hare free species rewards
use the six approved Hare skill traits; generic attack upgrades are excluded.
This correction supersedes the earlier design text that allowed income
modifiers as round or node rewards.

The original item IDs remain in the [design and delivery checklist](FIELD_DATA_DESIGN_PLAN.md).
Use this document for current Field Data behavior. The checklist tracks work
and the [economy history](SCIENTIFIC_DATA_ECONOMY.md) retains earlier proposals.
Sprint planning remains with Salty; this design assigns no sprint or deadline.

## Implementation design drafts

The [October 16 combined planning draft](FIELD_DATA_AND_SIMULATION_ALIGNMENT.md)
maps this baseline to Salty's simulation/upgrade proposal and merged foundations.
It keeps sprint selection and broader product decisions separate from these
first-test rules.

The [Process Design, PDD-FD-001](FIELD_DATA_PROCESS_DESIGN.md) describes the
player process, implementation stages and evidence needed for delivery. The
[Solution Design, SDD-FD-001](FIELD_DATA_SOLUTION_DESIGN.md) proposes the code,
data and UI integration against the inspected checkout. Both are draft 0.1 for
review as of 2026-10-15. This v1.0 document remains the product-rule authority;
authoring those drafts does not complete implementation or testing.

## Review status across Steps 1–5

| Step | Design completion | Evidence or delivery status |
| --- | --- | --- |
| 1. Round, wallet and purchase rules | Defined, including reorder prices and boundary cases | Requirements; not runtime-verified |
| 2. Starter purchases and rewards | Defined with provisional values and reward schedule | Catalog and source comparison; not balanced |
| 3. Journey budgets | Complete as a synthetic paper comparison | Arithmetic verified; no ecology runs |
| 4. Playable slice and correctness checks | Scope and acceptance checks defined | Implementation evidence exists; October 17 follow-up awaits Unity verification |
| 5. Internal test and refinement protocol | Defined, including 200 fixed seeds | Automated cohort complete; team judgment and follow-up content choice pending |

Checking a design decision does not complete an implementation or a playtest.
The next content addition is deliberately deferred until internal findings
exist. Actual seed IDs, scenario snapshot and build fingerprint are frozen at
test preparation, rather than invented as gameplay settings here.

## Step 1 — Round, wallet and simulation rules

### The first journey

Use this route:

```text
Simulation 1 → Shop 1 → Simulation 2 → Shop 2 → Simulation 3 → Results
```

Each simulation node contains six 100-tick rounds. A completed journey therefore
contains 18 round-end payouts and 1,800 simulation ticks. Local round numbers
restart at each simulation node; the underlying simulation continues.

Preserve the board, populations, seed, random-generator state, accumulated
simulation state and journey upgrades through both shops. Shops and decision
screens pause simulation ticks. Entering a shop does not reseed, restart the
ecosystem or regenerate its populations.

Starting populations and other ecology settings come from the selected scenario.
The paper example's 55 Hares is not an approved replacement starting population
or balance target. The first content comparison uses Hares; the base income rule
still refers to the selected species.

### One journey wallet

- Start every new journey with **0 Field Data (FD)** and no carried journey-local
  upgrades from a previous journey.
- Carry one balance through every round, simulation and shop in that journey.
  Payouts, node-clear awards and accepted purchases change that balance.
- Keep permanent Desktop currency and unlocks separate. Field Data operations
  neither spend nor credit them.
- Losing or abandoning a journey discards its remaining Field Data. Previously
  banked Desktop currency and unlocks remain intact.
- Success shows leftover Field Data on final results, then discards it on exit.
  This slice awards no permanent currency and performs no conversion.
- Journey upgrades end with that journey. A new journey resets its balance and
  local upgrades while retaining permanent Desktop state.

### Passing, failing and settling a round

A simulation node clears after all six rounds finish with at least one member
of the selected species alive. No extra population, reproduction, predator-kill
or ecological-viability threshold applies to this slice. This does not redefine
the pass rule for every future scenario.

If the selected species becomes extinct, fail immediately, including extinction
on the last tick of a round. Failure wins over settlement: do not award that
round's payout, its reward choice or its node-clear bonus. Do not undo payouts
already earned in earlier rounds; discard their remaining journey balance as
part of failure. Manually ending or abandoning the incomplete route also fails.

Use this order at a successful ordinary boundary, after local rounds 1–5:

1. Finish the tick, evaluate failure and pause.
2. Capture ending populations; calculate and award that round's Field Data.
3. Present the free round-reward choice, or Skip.
4. Allow population purchases, applying each accepted change immediately.
5. Resume only when the player selects Continue.

After local round 6 of Simulation 1 or 2:

1. Evaluate failure, then award the normal final-round population payout.
2. Complete the node and automatically award its 10-FD clear bonus.
3. Present the free node-reward choice, or Skip.
4. Show settlement without population controls, then Return to Map.
5. Wait for the player to click the ready shop node before shopping.
6. Leaving the shop returns to the map. Resume only after the player clicks
   the ready simulation node; preserve the same board, tick, balance and upgrades.

After local round 6 of Simulation 3, award the successful final-round payout and
10-FD clear bonus, then show final journey results. Offer no further population
purchases or upgrade choices after terminal success. Return to the completed
map without starting a new journey; starting a new journey is a separate action.

Round payouts and node-clear awards each happen **once per event**. Reopening
results, repeated Continue actions or returning to a screen cannot pay again.

### Population purchases

All six controls are unlocked for this slice. They are available at the round
breaks above, not during a running round or an ordinary mid-round pause.

| Control | Quantity per purchase | Provisional price |
| --- | ---: | ---: |
| Add Hares | +1 Hare | 10 FD |
| Remove Hares | −1 Hare | 10 FD |
| Add Foxes | +1 Fox | 10 FD |
| Remove Foxes | −1 Fox | 10 FD |
| Add Plants | +5 Plants | 10 FD |
| Remove Plants | −5 Plants | 10 FD |

Repeat purchases are allowed while funds and valid targets or space remain.
Removals choose random individuals of the requested species. Additions choose
random valid available locations for that species. The same seed, scenario and
ordered actions must reproduce those choices.

Validate the whole purchase before committing it. Reject with **no charge and
no partial board change** if funds are insufficient, a removal exceeds the
population, or the full addition cannot fit. Also **block a purchase that would
remove the final individual of the selected species**. This protection does not
prevent natural extinction. It does not establish a survival floor for other
species.

Show cost and effect before selection. A click executes the purchase immediately
without a second confirmation. Rejected purchases and accepted purchases must
remain distinguishable in correctness evidence.

### Calculating income

At a successful round boundary, begin with:

```text
base payout = surviving selected-species population
```

Process applicable income upgrades one at a time in their current execution
order. Initially this is acquisition order. A flat per-Hare upgrade adds the
ending Hare count at that position; a multiplier multiplies the current running
amount. Consecutive flat bonuses add and multipliers compound. Do not gather
all additions first or reinterpret the sequence as a conventional arithmetic
expression.

Round down **once**, after the full sequence. Do not round intermediate values
or carry a fractional remainder into the next payout. Add the final whole-FD
award to the existing wallet.

With 20 Hares:

| Execution sequence | Calculation | Award |
| --- | --- | ---: |
| No upgrades | 20 | 20 FD |
| +1/Hare, then ×1.5 | (20 + 20) × 1.5 | 60 FD |
| +1/Hare, then ×2 | (20 + 20) × 2 | 80 FD |
| ×2, then +1/Hare | (20 × 2) + 20 | 60 FD |

An acquired income upgrade first affects the next round-end payout after its
shop purchase. It cannot recalculate a settled payout. New income upgrades
append to the end of the current effective sequence.

Each income modifier declares whether it affects population payouts, node-clear
FD bonuses, or both. The three starter income upgrades below are
**population-payout-only**. The flat 10-FD clear bonus is therefore unaffected
by them. Node-clear modifier types and challenge income remain future content.

### Paid reordering at shops

Reordering edits the income calculation sequence, not the journey-map route.
Both modes are shop-only and use the full owned income-upgrade sequence.

- Random reorder requires at least two upgrade instances. It must change their
  order, has no preview, and executes and charges immediately.
- Chosen reorder lets the player arrange the full sequence, shows a preview and
  price, and charges only on confirmation. Cancel is free.
- Reordering preserves every owned upgrade. It changes only future unsettled
  payouts. Equal-effect copies remain distinct acquired instances; changing
  their order need not change the numerical payout.
- Keep original acquisition history separately from the effective execution
  sequence and reorder history.

For this first test, use:

```text
k = number of successful paid reorders of either kind in the current shop
unrounded random basis = 10 × 1.1^k FD
random charge = ceil(unrounded random basis)
chosen charge = 10 × current random charge
```

Increase `k` only after a completed paid reorder. Cancelling or rejecting an
operation does not increase it. Reset `k` to 0 at the next shop. Escalate from
the unrounded basis, rather than repeatedly multiplying a rounded previous
charge. Chosen reorder remains exactly 10 times the displayed random charge.

| Successful reorders already bought at this shop | Random charge | Chosen charge |
| ---: | ---: | ---: |
| 0 | 10 FD | 100 FD |
| 1 | 11 FD | 110 FD |
| 2 | 13 FD | 130 FD |
| 3 | 14 FD | 140 FD |
| Next shop | 10 FD | 100 FD |

## Step 2 — Starter content and reward schedule

All prices and strengths in this section are **first-test values**, not balance
findings. The shop contains persistent species changes and income upgrades;
population controls remain separate simulation actions.

### Income offers

| Offer | Explicit effect | Source eligibility | Shop price |
| --- | --- | --- | ---: |
| +1 FD per Hare | Add ending Hare count at this point in a population payout | Shop only, from first visit | 20 FD |
| ×1.5 population payout | Multiply current population payout by 1.5 | Shop only, from first visit | 30 FD |
| ×2 population payout | Multiply current population payout by 2 | Shop only, from second visit | 50 FD |

Income upgrades are positive-only and have no copy cap for now. A distinct
offer can be taken once per shop visit. The same upgrade can stack when it is
offered at a later shop. Eligible does not mean guaranteed: shop draws need not
contain an income item. Free round and node rewards contain species upgrades,
not income modifiers.

### Shop species changes

Include six Hare skill families, each with a positive and an equal-magnitude
negative variant: twelve starter variants total. They use roughly three times
the existing ordinary skill amount. Each costs **30 FD**, fixed across repeat
offers, and affects the rest of the journey starting with the next simulation.

| Family | Signed amount per copy | Existing skill reference |
| --- | --- | --- |
| Long Stride | +1.5 / −1.5 movement speed | Faster Movement: +0.5 |
| Sharper Instinct | +0.24 / −0.24 avoidance probability | Threat Exposure / avoidance: +0.08 |
| Reinforced Hide | +6 / −6 block | Tough Hide: +2 |
| Warren Comfort | +0.3 / −0.3 crowding energy reduction | Crowding Tolerance: +0.1 |
| Deep Digestion | +0.3 / −0.3 digestion bonus | Efficient Digestion: +0.1 |
| Abundant Seedfall | +0.03 / −0.03 seed-drop probability | Seed Dispersal: +0.01 |

Probability amounts are **percentage-point additions**, not percentage
multipliers: +0.24 means +24 percentage points. Species stat changes retain
explicit signed additive semantics; income multipliers do not alter the
existing species-stat authoring model.

Each family has a **three-copy total cap shared across positive and negative
variants**, separate from the ordinary in-phase upgrade cap. An opposite bought
at a later shop still consumes one of the three copies. Apply net opposing
amounts so they cancel; cancellation does not refund an acquisition slot.
Compute the net signed contribution against the species' base and other valid
upgrades before enforcing existing attribute bounds. Avoid irreversible
per-purchase clamping that would prevent later opposites from cancelling.
Use explicit attribute constraints and runtime mappings, not display names or
reflection. Boundary behavior requires focused tests before content delivery.

Only Hare-targeted variants enter this slice. Fox/Plant-targeted variants,
multispecies versions and new effect families are deferred. Preserve the agreed
future direction: a multispecies purchase applies its stated full effect to
each target, costs 30 FD per target at these provisional prices, and counts as
one copy against the same family cap across targets.

### Shop generation and purchases

Each shop draws five distinct offers with equal weighting within its available
starter pool. Use seed-deterministic draws and preserve the visit/offer identity
for replay. The second shop adds ×2 to its candidate pool. No positive/negative
pair from one family can appear together in the same visit. Players can buy
multiple distinct offers, once each; later visits may offer the same upgrade
again and it can stack under its cap.

A capped species offer may appear locked. The player can choose to reroll it,
free, once for that offer in that visit. A replacement must be currently
purchasable under the family cap, avoid same-visit duplicate offers and
positive/negative conflicts, and meet source/target eligibility. Do not reroll
automatically or charge FD. If no valid replacement exists, leave fewer offers
rather than inventing a fallback. Draw fewer than five when the offer pool
cannot supply five distinct legal display offers. Other shop offers remain
unchanged by the reroll.

### Free rewards and node-clear income

| Moment | First-slice reward |
| --- | --- |
| Local rounds 1–5, each simulation | Three randomized distinct eligible choices, take one or Skip, free |
| Simulation 1 and 2 clear | Three randomized distinct eligible node choices, take one or Skip, free |
| Every successful simulation clear, including Simulation 3 | Automatically award 10 FD after the normal final-round payout |
| Final journey success | Results only after settlement; no additional upgrade choice |

Round and node choices contain the six ordinary Hare skills; income modifiers
are shop-only. Each ordinary Hare skill can repeat up to its existing catalogue
level cap (currently 10), independently of the shop's shared three-copy family
cap. Taking a free skill once does not remove it for the rest of the journey.
Journey carryover follows the agreed journey upgrade direction; do not restart
the simulation or discard acquired journey effects at shops. Skip gives no
reward or penalty in this slice. When fewer than three distinct choices are
eligible, show the available choices and Skip. If none remain, explain that no
eligible mutations remain and allow Skip; never substitute legacy priced offers.

Shop 3× variants do not silently become ordinary free rewards. Taking one free
skill passes on the other offered skills. Rewards are not a second wallet or
a permanent Genome purchase.

## Step 3 — Paper budget and what it proves

The complete synthetic ledger is in Appendix A. It compares struggling, stable
and growing population paths on the same route. Each starts with an
illustrative 55 Hares, uses the six-round nodes, and includes all three 10-FD
clear bonuses. It assumes the purchased income offer appears; it does not
predict randomized shop inventory or simulate ecological intervention effects.

| Population path | No buys | ×1.5 at Shop 1 | ×2 at Shop 2 | +1/Hare at Shop 1, then ×2 |
| --- | ---: | ---: | ---: | ---: |
| Struggling | 324 FD | 336 FD | 295 FD | 384 FD |
| Stable | 1,020 FD | 1,314 FD | 1,300 FD | 2,270 FD |
| Growing | 1,785 FD | 2,427 FD | 2,500 FD | 4,595 FD |

The compound path ends richest in these constructed examples. That is a
specific candidate for internal comparison, not proof of dominance in the
live ecology. The paper paths establish arithmetic affordability and payback;
they do not establish fun, pacing, survival probability or validated prices.
The struggling path ends at one Hare and meets this slice's survival-only
rule mathematically, without predicting that real runs will follow that path.

## Step 4 — Implementation scope and acceptance checks

The new slice must integrate with the existing simulation, progression,
journey flow and Noesis presentation. Use the existing deterministic domain
logic and explicit upgrade mappings. Preserve authored asset identity and
historical experiment inputs. Add no general-purpose modifier framework just
to support these three explicit income operations.

### Presentation requirements

- Show every species' ending population, and identify the selected species used
  for base income.
- Show the base amount, applicable modifiers in execution order, running totals
  and final whole-FD payout.
- Show population income separately from the node-clear FD bonus, then the
  combined wallet balance. Update it after every award and purchase.
- Show population purchase price/effect before the single execution click.
- Show effective income order. Only chosen reorder gets a preview and a
  confirmation step; random reorder has neither.
- Failure results show the FD amount lost. They need no additional banked-
  currency message; banked-state protection is still a system requirement.
- Success results show leftover FD before its discard on exit.

### Required correctness evidence

| Checklist coverage | Verify before calling delivery complete |
| --- | --- |
| 4.07, 4.18 | Successful rounds and node bonuses settle once; final-tick extinction prevents both; reopened screens/repeated Continue never duplicate awards |
| 4.08, 4.19 | Six controls obey amount/price, pause window, random valid targets and all-or-nothing charging; block final-selected-individual removal |
| 4.09–4.11, 4.21 | All three income types are offered only in eligible shops; species shop rules work; upgrades survive both shops and later simulations |
| 4.12–4.17 | Payout populations, ordered math, separate award lines, immediate balances, one-click purchase and loss/results displays match this design |
| 4.20–4.24 | One journey wallet persists through nodes; new journey resets only local state; failure/success exit preserve banked currency and unlocks |
| 4.25–4.26 | Compare order-dependent sequences on identical inputs; test shop acquisition timing at each shop; settle/round once |
| 4.27–4.28 | Reorders preserve instances; random must change order; chosen confirm/cancel is correct; escalation/reset/10× costs charge once; both histories reproduce the calculation |

Also cover capped free rerolls, positive/negative exclusion, net cancellation
near stat bounds, second-shop ×2 eligibility, and deterministic replay of
seeded offers and population interventions. Random income offers are not
guaranteed in live play; focused fixtures may deliberately exercise each source
without changing live offer rules.

Appendix B records the source audit and known gaps. No new runtime, Unity test,
player build or balance acceptance is established by this design review.

## Step 5 — Internal self-play and refinement

Use **200 fixed seeds** for every matched comparison arm; the number of runs
depends on the number of arms, not a 200-run total cap. Freeze and record
the actual IDs, scenario, species/upgrade inputs, source/build revision and
ordered choices before the comparison. Reuse the same starting scenario and
seed panel for weaker and stronger population paths and their baseline/changed
arms. The paths differ through decisions, not unrecorded starting-population
edits. Matching seeds does not promise identical later ecology after different
actions; retain the actions and resulting populations needed to explain it.

This is internal team testing. It does not commission 200 external playtesters
or require player interviews, teach-back prompts or external purchase-behavior
tracking. It authorizes no run in the current documentation task.

### What to record and review

| Item | Required internal record or decision |
| --- | --- |
| 5.01 | Freeze the selected starting setup and 200-seed list before execution |
| 5.02–5.03 | Compare weaker and stronger population outcomes on that matched panel |
| 5.04 | For first affordability, record node, round/tick boundary and FD balance |
| 5.05–5.06 | Record offered item, buy/skip, cost, remaining FD and pre-purchase balance on shop arrival |
| 5.07 | Record both real elapsed time and simulation ticks between meaningful decision opportunities |
| 5.08–5.10 | External rationale interviews, teach-back and purchase-behavior research are deferred |
| 5.11 | Note qualitative examples where income choices displace species upgrades or interventions, without a preset threshold |
| 5.12 | Compare a 10-FD intervention path with saving toward the 50-FD ×2 offer on matched inputs; label offer availability |
| 5.13–5.14 | Allow multiple related prices/effects in one documented adjustment bundle; replay both population paths with the same scenario and seeds |
| 5.15 | Record the team's accept/revise judgment and the evidence supporting it; use no numeric survival/population gate |
| 5.16 | Choose the next content addition only after the internal findings are available |

For timing, keep real time and simulation ticks distinct. Deliberation on a
paused choice can take real time while the simulation tick stays fixed; do not
interpret zero ticks as zero decision time. Keep raw outcomes separately from
team explanations and approval notes.

Hold ecology values stable while comparing economy adjustments. A bundle of
related economy changes is allowed, but its result supports the bundle, not a
claim about the isolated effect of each changed number. Compare actual offer
availability rather than assuming that ×2 appears in every second shop. Do not
generate survival or viability acceptance thresholds from the paper examples.

## Explicit deferrals

- Separate permanent progression currency, Genome purchase economy and success
  conversion, including conversion upgrades.
- Random journey challenges/events and more varied node rewards.
- Seed-randomized population purchase quantities/prices and multiply/divide
  population controls; current six controls use fixed quantities and prices.
- Fox/Plant-targeted and multispecies shop variants, new species effect
  families, negative income modifiers and node-bonus income modifiers.
- External participant research and the exact next content addition.
- Broader map shapes, wrapping/obstacles, plant/mushroom roles, animation
  smoothness and other meeting topics outside this Field Data slice.

## Review findings resolved in this baseline

1. Separated design completion from unperformed implementation and self-play.
2. Replaced stale open notes with the confirmed six controls, atomic rejection,
   source rules, result displays, new-journey reset and success discard.
3. Closed the reorder base price, rounding/escalation and 200-seed test-panel
   gaps through Bevin's direct review answers on 2026-10-15.
4. Fixed the missing reward cadence and approved provisional clear-bonus value.
5. Defined nonterminal round-6 purchase windows and the final-individual guard.
6. Removed the stale universal pass-rule question from the paper example.
7. Distinguished current Hare-only shop content from deferred broader targets.
8. Preserved shared family caps, optional capped-offer rerolls, opposing
   cancellation, ordered income and separate acquisition/execution histories.
9. Reconciled the 200-seed internal protocol with the deferred external-player
   research and the permitted multi-value economy adjustment bundles.

## Appendix A — Complete paper ledger

The following calculations are synthetic arithmetic examples, not simulation
evidence. The clear bonus began as an illustrative paper assumption and was
confirmed as a provisional first-slice setting during this review.

### Paper-only scenario assumptions

The segment has five nodes: Simulation 1 → Shop 1 → Simulation 2 → Shop 2 →
Simulation 3. Each simulation node contains six 100-tick rounds. The routes and
economy paths below are matched examples, not ecology forecasts. They start
with 55 Hares, matching the currently authored production Forest Edge starting
population (`Assets/Data/ProductionData/CellularSimulation/Scenarios/ForestEdge.asset`,
`startingPopulation: 55` for the Hare definition); that source value is a
reference for this paper exercise, not a decision that this journey must use
55. The three Hare-count paths are
deliberately constructed to illustrate different budgets. They are not
user-approved targets or evidence of ecological viability. Add a provisional
10-FD clear reward after each successfully completed simulation node. No
intervention's ecological effect, optional challenge reward or randomized
shop draw is modeled. Free income reward choices are skipped in this arithmetic
example. Each strategy assumes its purchased shop item appears and buys no paid
reorders. Prices and clear rewards remain provisional.

### Round-by-round base ledger

Each population payout is 1 FD per Hare before upgrades. Add a 10-FD clear
reward after rounds 6, 12 and 18. Balances are unspent and shown immediately
before any purchase at that shop.

| Round | Node | Struggling Hares / base FD | Stable Hares / base FD | Growing Hares / base FD | Clear reward |
| --- | --- | ---: | ---: | ---: | ---: |
| 1 | Simulation 1 | 55 / 55 | 55 / 55 | 55 / 55 | - |
| 2 | Simulation 1 | 45 / 45 | 55 / 55 | 60 / 60 | - |
| 3 | Simulation 1 | 36 / 36 | 55 / 55 | 65 / 65 | - |
| 4 | Simulation 1 | 29 / 29 | 55 / 55 | 70 / 70 | - |
| 5 | Simulation 1 | 23 / 23 | 55 / 55 | 75 / 75 | - |
| 6 | Simulation 1 | 18 / 18 | 55 / 55 | 80 / 80 | +10 FD |
| 7 | Simulation 2 | 16 / 16 | 55 / 55 | 85 / 85 | - |
| 8 | Simulation 2 | 14 / 14 | 55 / 55 | 90 / 90 | - |
| 9 | Simulation 2 | 12 / 12 | 55 / 55 | 95 / 95 | - |
| 10 | Simulation 2 | 10 / 10 | 55 / 55 | 100 / 100 | - |
| 11 | Simulation 2 | 8 / 8 | 55 / 55 | 105 / 105 | - |
| 12 | Simulation 2 | 7 / 7 | 55 / 55 | 110 / 110 | +10 FD |
| 13 | Simulation 3 | 6 / 6 | 55 / 55 | 115 / 115 | - |
| 14 | Simulation 3 | 5 / 5 | 55 / 55 | 120 / 120 | - |
| 15 | Simulation 3 | 4 / 4 | 55 / 55 | 125 / 125 | - |
| 16 | Simulation 3 | 3 / 3 | 55 / 55 | 130 / 130 | - |
| 17 | Simulation 3 | 2 / 2 | 55 / 55 | 135 / 135 | - |
| 18 | Simulation 3 | 1 / 1 | 55 / 55 | 140 / 140 | +10 FD |
| **Total before purchases** |  | **324 FD** | **1,020 FD** | **1,785 FD** | **30 FD** |

Balances on arrival at Shop 1 are 216 / 340 / 415 FD for struggling / stable /
growing paths. The corresponding balances on arrival at Shop 2, with no earlier
purchase, are 293 / 680 / 1,010 FD.

### Spending-strategy comparison

The early strategy buys ×1.5 for 30 FD at Shop 1. The saving strategy saves
until Shop 2 and buys ×2 for 50 FD. The mixed strategy buys +1 FD per Hare for
20 FD at Shop 1, then ×2 for 50 FD at Shop 2. Modifiers affect only later
population payouts, execute in acquisition order and do not modify the
illustrative node-clear rewards. All figures include the three 10-FD clear
rewards and subtract the stated purchase costs.

| Population path | No purchases | Early: ×1.5 at Shop 1 | Save: ×2 at Shop 2 | Mixed: +1/Hare, then ×2 |
| --- | ---: | ---: | ---: | ---: |
| Struggling | 324 FD | 336 FD | 295 FD | 384 FD |
| Stable | 1,020 FD | 1,314 FD | 1,300 FD | 2,270 FD |
| Growing | 1,785 FD | 2,427 FD | 2,500 FD | 4,595 FD |

The mixed strategy has the highest ending balance in these constructed paths
because the later ×2 also multiplies the earlier +1 FD/Hare effect. This is a
reason to test acquisition order and compounding in play, not a balance
verdict. The struggling example is still alive at one Hare at the final
round and mathematically meets the scoped survival-only clear rule. It is not
a simulated survival forecast or an ecology-balance result.

### Affordability and upgrade payback

After round 1, the 55-Hare reference path earns 55 FD and can spend 10 FD on a
provisional population intervention, retaining 45 FD. If it then buys the
30-FD species buff at Shop 1 after round 6, the no-upgrade balance of 216 FD is
reduced to 176 FD (216 − 10 − 30). The next six rounds plus the 10-FD clear reward contribute
77 FD on the struggling path, leaving 253 FD at Shop 2 before another purchase;
it can still afford ×2 for 50 FD. These numbers demonstrate paper affordability
only; an intervention would change the simulated population and invalidate the
fixed trajectory used here.

| Income offer | Cost | Population payouts remaining | Struggling incremental FD / payback | Stable incremental FD / payback | Growing incremental FD / payback |
| --- | ---: | ---: | --- | --- | --- |
| +1 FD per Hare at Shop 1 | 20 FD | 12 | 88 / 2 payouts | 660 / 1 payout | 1,350 / 1 payout |
| ×1.5 at Shop 1 | 30 FD | 12 | 42 / 5 payouts | 324 / 2 payouts | 672 / 1 payout |
| ×2 at Shop 2 | 50 FD | 6 | 21 / not repaid | 330 / 1 payout | 765 / 1 payout |

Payback uses per-payout round-down after the full modifier sequence and assumes
the example populations persist. A modifier may fail to repay in the remaining
rounds on the struggling path, while earlier or higher-population investments
repay sooner. The +1/Hare then ×2 combination is worth observing because the
later multiplier compounds the flat bonus. Keep current prices as comparison
inputs for play; paper estimates are not sufficient evidence to rebalance them.

These population paths are illustrations, not predictions of the real ecology.
Actual birth, food, crowding and predation outcomes will change income. Do not
price the first loop around an undecided permanent-currency conversion.

**Step result:** a five-node, eighteen-payout paper comparison ready to inform a playable test.

## Appendix B — Source audit and remaining implementation

Audit date: 2026-10-15. Source baseline: `735b189c`; relevant code was
inspected locally. These observations do not establish runtime acceptance.

**Integration refresh, 2026-10-16:** `ProjectMain` through `341cc702` is now
merged. `PlayNextSimulation` clears carried Field Data for a new journey while
retaining standalone carryover; the profile now stores permanent feature IDs,
and the scrolling map has 29 markers/35 connections. The dated audit below
describes the earlier revision. The complete five-node economy and settlement
contract remains implementation work; see the
[combined planning draft](FIELD_DATA_AND_SIMULATION_ALIGNMENT.md).


This is a source inspection of the current prototype, not runtime or test
evidence. The agreed rules above describe the intended loop; current code has
not yet been changed to implement it.

- `SimulationRunResults.Create` sets eligible currency equal to the current
  player population. `HandlePhaseBoundaryReached` awards that amount once per
  phase using `lastSettledPhaseIndex`. It has no Field Data modifiers or
  configurable reward breakdown yet.
- `SpeciesProgression` has one journey-local integer `Currency`,
  add/spend/purchase methods, and an authored Hare purchase cost of 10. A
  separate persistent profile-currency store exists in
  [ProfileSessionSnapshot.cs](../Assets/Scripts/Game/Profile/ProfileSessionSnapshot.cs)
  and [Helper_ProfileSession.cs](../Assets/Scripts/Game/Profile/Helper_ProfileSession.cs);
  no inspected call site connects it to journey currency or genome purchases.
  Confirm its intended use before wiring any success conversion. The journey
  progression still lacks the new income modifiers and shop effects.
- The existing `SpeciesUpgrade` constructor accepts only positive finite
  values. Negative shop variants need an explicit signed runtime application
  path and bounds/cancellation tests; do not feed negative values into the
  existing positive-only constructor or infer effects from display text.
- `ClaimJourneyNodeReward` applies an authored mutation or condition; a
  condition's `DataReward` is granted immediately when claimed. This is not
  yet an automatic reward for successful simulation-node completion, nor does
  it implement the provisional clear reward used in the paper ledger.
- `PlayNextSimulation` explicitly carries the current currency into a reset
  progression instance. `ResetToStart` creates a fresh progression and clears
  the run balance. Failure, early termination and player-ending paths mark
  rewards ineligible and spend the remaining progression currency. Those paths
  do not call the separate profile-currency helper, but focused failure and
  profile-persistence tests are still needed to verify that banked currency is
  preserved through the complete runtime flow.
- The preview currently defines six continuous phases and a two-cycle journey
  prototype. The planned five-node route with three six-round simulation nodes
  and two shops is not represented by the current journey-node flow. There is
  no shop-node purchase flow for the planned journey-persistent species buffs or Field
  Data modifiers.
- Phase settlement and terminal run completion both call `AddCurrency` on
  eligible results, with separate duplicate guards. Verify the final-boundary
  case with a focused test before relying on “exactly once” for the last
  successful round; source inspection alone does not establish whether a
  terminal population can be settled twice in the actual run lifecycle.

**Audit result:** the existing prototype provides a useful population-payout,
journey-local wallet and phase-boundary foundation, alongside a separate
persistent profile-currency store whose intended genome-purchase connection
was not found in the inspected call sites. The planned journey economy still
requires reward-type modifiers, shop nodes, the three-node simulation segment,
successful node-clear awards, and focused exactly-once and wallet-preservation
verification. No implementation, gameplay tests, or balance claims were made
in this audit.

### Source references

- [Simulation results](../Assets/Scripts/Game/Simulation/SimulationRunResult.cs)
- [Simulation and journey presentation](../Assets/Scripts/Game/Presentation/SpeciesSimulationPreview.cs)
- [Journey progression](../Assets/Scripts/Game/Species/SpeciesProgression.cs)
- [Ordinary upgrade catalog](../Assets/Scripts/Game/Species/SpeciesUpgrade.cs)
- [Species stat constraints](../Assets/Scripts/Game/Species/SpeciesRules.cs)
- [Profile snapshot](../Assets/Scripts/Game/Profile/ProfileSessionSnapshot.cs)
- [Profile session helper](../Assets/Scripts/Game/Profile/Helper_ProfileSession.cs)
- [Production Forest Edge scenario](../Assets/Data/ProductionData/CellularSimulation/Scenarios/ForestEdge.asset)
- [Existing upgrade planning concerns](Planning%20Concerns/upgrade-system.md)

Income ordering and multiplication are explicit Field Data operations. They do
not authorize implicit multiplication syntax in existing additive species
upgrade assets. Keep stable IDs, resolved effect values, order and source
fingerprints in deterministic evidence. No concern status or waiver was changed
by this review.

## Document review evidence — 2026-10-15

- Preserved all 124 original checklist IDs in their original order.
- Checked all five design sections, local document/source links, Markdown
  whitespace and code-fence balance.
- Recomputed 18 paper payout boundaries, 12 strategy outcomes, nine payback
  cases and the displayed reorder charges using exact arithmetic.
- Confirmed historical economy material and unrelated project/working-state
  sections were preserved during consolidation.
- Source inspection identified existing foundations and implementation gaps;
  no Unity compilation, runtime tests, ecology runs or self-play were performed.
- See the [review handoff](handoffs/2026-10-15-2335-codex-field-data-design-baseline-review.md)
  for the local change and validation record.
