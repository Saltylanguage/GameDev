# S5 playability goalpost — meeting draft

> **Direction from Josh, October 16, 2026:** S5 should prove that Forest Edge
> can become a fun, engaging game through a much more solid simulation and a
> playable route. Josh wants the full Field Data card list available for the
> priority meeting with Sim. This is the proposed product exit goal; the final
> committed ticket subset, owners and acceptance changes await that meeting.

## The playability proof

The first quality gate is a polished route from the normal Desktop through two
connected six-round simulations and one shop, using one retained world and
journey wallet. The player can see why plants, Hares and Foxes change, make a
Field Data spend-or-save decision, and try different temporary upgrades. A
success or failure result suggests a different plan for the next attempt.

The existing five-node, 18-round route and two shops remain in FD-T02/04 and
the published Field Data plan. In particular, FD-T04 is already in progress;
do not rewrite or discard its working session code. The shorter route is the
**first playability review gate**, not a board decision to remove the third
simulation or second shop. The team will decide tonight whether those and the
other broad ticket outcomes remain S5 exit requirements or follow-on work.

## Evidence for an S5 pass

1. **Reliable route:** A player can enter from Desktop, finish two simulations
   and one shop, see a clear result, and begin another attempt. World, wallet,
   upgrade and profile boundaries behave consistently. Awards and purchases
   cannot settle twice.
2. **Simulation pressure:** On declared inputs, plant renewal, Hare food and
   reproduction, and Fox predation or starvation create understandable recovery
   and failure paths. Diagnose and correct the largest agreed run-shaping
   defect before tuning values. Bevin guides provisional clear and viability
   judgments; no fixed population quota is invented here.
3. **Meaningful choices:** Compare at least two plans from the same start,
   including spending versus saving Field Data and different Hare upgrade
   choices. Show their effects on t he world, wallet and outcome. Trailblazer,
   Warren and Gardeners are comparison lenses, not locked builds.
4. **Readable play:** At the decisions and result, players can understand the
   pressure, price, effect and reason for the outcome. Bevin and Salty each
   play the route and a different follow-up attempt, recording promising,
   confusing, tedious and unfair moments. Someone other than the tuning author
   reviews at least one route.
5. **Reproducible judgment:** Record revision, scenario, seeds, actions,
   receipts, run statuses and separate ecology/economy outcomes. Freeze inputs
   within each matched comparison. If ecology changes, freeze a new baseline
   and rerun both arms. Record an explicit accept/revise decision and prioritized
   problems. Automated comparisons support judgment; they do not certify fun.

The team should be able to name specific moments where a simulation problem
appeared, a player choice changed the next state, and the result invited another
attempt. If it cannot, the playability goal is still open even if infrastructure
works.

## Capacity and current board — October 16

S5 runs October 19–November 15. Bevin and Salty each have 10–15h/week of
planning availability: **80–120h combined** over four weeks. Josh removed the
16h protected integration/review reserve; review will use separate dedicated
time outside the sprint effort. Use **80h** as the conservative sprint planning
floor. The 16 published Field Data tickets still estimate **57–95 active
team-hours**, so their upper range is **15h above** that floor. Ecology
diagnosis/correction and playability work have no agreed estimate yet. Removing
the reserve does not establish that all work fits.

Keep the 16 cards visible for tonight's meeting: FD-T01–03 are Done, FD-T04 is
In Progress, and FD-T05–16 remain in Current Work. No cards have been moved or
rescoped for this goalpost. Owners and reviewers remain unassigned by request.
The current Trello control and original ticket checks remain the operative
execution record until the team confirms a revised selection.

## Priority discussion with Sim

Use this order to expose dependencies and make choices, **not** as a committed
ranking or instruction to move cards:

| Discussion block | Cards / work | Decision to make |
| --- | --- | --- |
| Foundations already underway | FD-T01–04 | Confirm FD-T04's remaining work and whether the two-simulation review can run through its existing definition/session path. |
| Player choice spine | FD-T05, FD-T07, FD-T09, FD-T11–13 | Identify the minimum wallet, visible upgrade/shop effects and Desktop presentation needed for the first playable review; estimate any acceptance change with implementers. |
| Simulation and judgment | Ecology diagnosis/correction; FD-T16 internal play review | Name the first defect to investigate, freeze a baseline, set a bounded comparison and agree when play review happens without reinstating a protected reserve. |
| Remaining full-route breadth | FD-T06, FD-T08, FD-T10, FD-T14–15 and the third simulation/second shop | Decide what can be completed inside S5 after actual remaining estimates, dependencies and the playability gate are known. Keep the work visible until that decision. |

Meeting outputs: a ranked S5 ticket subset, named owners/reviewers, remaining
hour estimates, precise acceptance changes if any, a simulation diagnosis
task/owner if selected, and a date for the first playable review. Reconcile the
Trello control, affected cards, roadmap and capacity ledger together after the
team decides. Preserve completed evidence and distinguish a two-simulation
review from full five-node acceptance.
