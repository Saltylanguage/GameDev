# RG-02 — normal route and shop/session review

**Report ID:** RG-02-ROUTE-2026-10-22-01  
**Status:** Partial route review; baseline and Field Data timing decisions implemented, shop/session integration remains open  
**Run date:** October 22, 2026  
**Source:** `6ffb7b109b63683e5863bc84e2c48e50794924a0`  
**Branch:** `codex/audio-direction-and-sourcing` (dirty before and after review)  
**Editor:** Unity 6000.4.6f1; `LearningIndieDev` Editor reported Ready, no compile
failure, Play Mode available.

## Scope

Follow RG-02 in the Rabbit Genome implementation plan: inspect the normal
Desktop launch, route length and first decision; locate the existing Field Data
journey/session/shop implementation; identify what route evidence can be
exported. This was a single live route observation, not a balance experiment.
No production balance values, authored assets, or profile wallet balances were
changed.

## Run record

- Entry: `GalapagOSDesktopTest` profile launch, through the normal Desktop
  simulation route. Route-node and Mutation selections used the same commands
  bound to the UI because this session had no native mouse input.
- Profile: `Researcher 01`, ID prefix `16b29f48`; currency, Rabbit Data,
  Research Data, and completed journeys were all 0 before play.
- Scenario/species: Forest Edge / Hare, 36×20 grid.
- Serialized base seed: 10100. Active route run seed: 10103.
- Starting world: 400 Plants, 55 Hares, 35 Foxes.
- Active permanent Genome: none.
- Opening temporary offer chosen: free General Movement. No paid Mutation or
  population intervention was purchased. Wallet remained 0.
- Normal settings: one simulation phase is 100 ticks at 0.2 seconds per tick;
  two six-phase cycles are 1200 ticks / 240 simulated seconds. The UI showed
  1× speed.
- The journey was ended in the Editor without completing or settling it.

## Observations

1. At tick 60, the live run had 22 Hares, 41 Foxes and 518 Plants. The
   recorded start was 55 / 35 / 400. Hare metrics showed 33 deaths, all from
   predation, zero births, and zero starvation at that point.
2. At the first 100-tick decision boundary (20 simulated seconds), one Hare
   remained. Hare deaths were 54: 49 predation and 5 starvation; births were
   still zero. Foxes rose from 35 to 44 and Plants from 400 to 562.
3. The interface model exposed six Field Data population controls, all disabled
   at the zero wallet. The phase Mutation offers were also unaffordable. Thus
   the first paid response could not address the opening losses. The start-of-run
   free Mutation was available and had already been chosen.
4. The live run reached the decision state through the shared simulation
   helper, but Unity's Game View did not refresh after the route was advanced
   while the Editor was unfocused. A tick-60 screen capture was returned in the
   session; no image was saved in the repository. The tick-100 state was
   verified from runtime and view-model values, not a fresh decision-screen
   capture. Wall-clock decision timing and on-screen readability remain
   unverified.

## Route and integration findings

- `ForestEdgeJourney.asset` contains a 31-node authored graph. The runtime
  prototype is capped at two cycles of six phases each (12 phases / 1200 ticks).
  The map header says “13 encounters per route.” These route descriptions do
  not establish the S5 two-simulation-plus-one-shop acceptance path.
- `JourneyNodeKind` has Simulation, Condition, Upgrade, Event, Finale, Reward,
  and Refresh kinds; it has no Shop kind. The runtime exposes Rest Stop and Seed
  Cache population encounters, not a Field Data shop. Searches of the current
  worktree, available local branches, and the separate `CrossAgent/S4Pilot`
  checkout found no journey shop/session implementation to integrate.
- The actual route export seam is incomplete for player-action review. Existing
  portable research reports are not a receipt for this Desktop run; they do not
  establish its complete ordered route choices, wallet receipts, encounter
  outcomes, and interventions together.
- The live Desktop start (55 Hares / 35 Foxes) differs from the previously
  screened ecology setup (20 Hares / 10 Foxes). RG-02 did not change either.

## Prioritized blockers and next choices

**P0 — Confirm the normal starting population.** At the observed Desktop start,
54 of 55 Hares died by the first decision, with 49 attributed to predation and
5 to starvation. This is one seed, not a balance conclusion. Bevin should guide
whether this is the intended pressure or whether the normal-route baseline
should use the previously screened 20-Hare / 10-Fox start.

**P0 — Integrate the agreed Field Data shop/session path.** No shop node,
journey wallet path, or matching implementation was found in the available
local source. Preserve the existing journey and node rules when the operative
FD-T04/09/12/13 implementation is located; the Rest Stop/Seed Cache prototype
is not that integration.

**P1 — Make an early response affordable and visibly usable.** The first paid
decision arrived after the opening loss and every paid option was disabled at
0 Field Data. Decide where the first spendable Field Data enters the route and
whether an early population recovery action is intended before implementing or
tuning prices.

**P1 — Complete route receipts.** RG-02 did not find one export that captures
the actual Desktop journey’s choices, spend/earn receipts, outcomes and
interventions in sequence. Identify FD-T14's owner/path and the smallest gap
before treating this run as reproducible route evidence.

**P2 — Repeat with human reviewers after the route is connected.** This run
does not establish whether the route is fun, fair, or readable. Josh, Bevin,
and Salty must review the intended route; this report records no human play
feedback.

## Verification and limits

- Unity Editor status: Ready before play; compilationFailed=false; zero compile
  errors.
- Live route: started through Desktop, observed through tick 100, then stopped
  without cashout.
- No tests or new ecological experiments were run.
- The Editor bridge did not keep the Game View advancing between tool actions.
  Auto-tick was already on; its session-only interval was set to 16 ms. No
  persistent Editor setting or game source was changed.

## Follow-up decisions and implementation — October 22, 2026

Josh selected **400 Plants / 20 Hares / 10 Foxes** for the Forest Edge launch,
confirmed the opening upgrade remains free, and said players should not receive
Field Data until after the first simulation. The Forest Edge scenario asset now
uses 400/20/10. In a journey, Field Data earned across phases 1–6 is banked and
released at the first six-phase cycle boundary; subsequent phase income keeps
its existing behavior. Restarting the run clears the pending amount, and the
existing phase-settlement guard prevents duplicate release at a repeated
boundary event.

Focused verification passed: the Forest Edge authored-population test (1/1) and
the profile journey reward / Gene Lab integration test (1/1). The latter now
asserts there is no spendable Field Data at phases 1–5 and that phase 6 releases
the accumulated first-simulation amount. This did not add or integrate a shop
node; the existing route/session integration and full-route receipt gap above
remain open. No new ecology experiment or balance tuning was performed.
- One seed and one opening Mutation cannot determine balance or fun.

## RG-02 disposition

The route and integration review is complete enough to expose dependencies, but
the two-simulation-plus-shop route is not demonstrated and the normal population
baseline needs a human decision. Stop before RG-03 values, prices, or production
offers until the starting-condition and economy choices are resolved.
