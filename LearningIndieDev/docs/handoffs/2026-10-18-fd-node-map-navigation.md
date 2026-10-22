# Field Data: settlement and explicit node entry

Date: October 18, 2026. Local `BevBranch`, based on `acc371a`.

Bevin approved keeping the node-end Field Data settlement while removing its
population controls, and returning to the map after every node so the player
chooses when to enter the next shop or simulation.

## Resulting player flow

1. Finish Simulation 1 or 2, review payout/clear bonus and resolve or skip the
   free node choice. No population controls or shop offers are available here.
2. Return to Map. Click the highlighted ready Shop card to shop.
3. Return to Map from the shop. Click the next ready Simulation card to resume.
4. After Simulation 3 succeeds, return to the completed map. A separate Start
   a New Journey action closes the old session and prepares a fresh route;
   the first simulation still requires explicit entry.

Ordinary round-1–5 decisions retain their six population controls. All other
nodes are disabled on the map. The retained board, tick, wallet, awards and
upgrades remain intact while waiting. A mandatory entry map has no Back action;
the optional Map view during a round/shop still returns to that same activity.

## Implementation

- `FieldDataJourneySession.CanUsePopulationControls` now permits only ordinary
  round decisions, rejecting node settlement and shop purchases without a charge.
- `VM_SimulationShell` uses a small feature-local enum for settlement, map to
  shop, shopping and map to simulation. The session stays at its existing Shop
  boundary. Returning to the map does not call Continue; the next simulation
  card calls it once. No new ecology or financial calculations were added.
- Shop purchase/reroll/reorder commands are guarded until explicit shop entry.
  Stale Continue commands cannot bypass waiting on the map. Leaving a shop
  cancels an unconfirmed reorder draft without a charge.
- Noesis route cards are buttons with explicit ready/cleared/upcoming states.
  Return to Map replaces Continue at node ends and completed-journey results.
- PDD, SDD, canonical journey rules, checklist, ticket draft, self-play guide
  and project context record the superseding October 18 decision.

## Verification

- Unity recompilation completed without errors.
- Population-control EditMode: **9/9 passed**,
  `artifacts/unity-tests-20261018-114314/`.
- Field Data PlayMode: **4 passed, 0 failed, 2 graphics checks skipped** for
  their separate visual runner, `artifacts/unity-tests-20261018-114829/`.
  This includes the authored 20-Fox seed-10100 route through all 18 rounds,
  both shops, explicit next-node clicks, repeated Continue, map wait/no-spend,
  final completed-map return and separate restart. Failure/abandon/reset pass.
- An old UI fixture ended from extinction before reaching shop assertions.
  Configuring a route reloads authored rules, so its synthetic stabilizing
  settings must be applied after selection. This was corrected in the UI and
  ownership fixtures; the authored route test retains real scenario rules.
- Initial rendered pass: **6/6**, 1280x720,
  `artifacts/visual-evidence-20261018-114904/`. Inspection found clipped Return
  to Map text and an unbound Back-button visibility; both were corrected.
- Final rendered pass after those layout fixes: **6/6 at 1280x720**,
  `artifacts/visual-evidence-20261018-115112/`; authored full route **1/1 at
  1920x1080**, `artifacts/visual-evidence-20261018-115159/`. Inspected settlement,
  ready shop, shopping, ready simulation and final completed-map captures.
  Return to Map is legible; mandatory entry maps hide Back. Unity was verified
  outside Play Mode on clean `MainMenu.unity` after the runs.

The existing 20-Fox default is unchanged by this work. Frozen older balance
panels remain historical; the new population-window rule changes which replay
actions are legal. No balance acceptance, player build, human acceptance,
Trello change, commit or push is claimed.
