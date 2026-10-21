# Field Data route map and income-source correction

- **Date:** 2026-10-17
- **Branch:** `BevBranch`
- **Status:** Local source and design updates made; Unity validation and human visual review remain outstanding.
- **Trigger:** Bevin reported that selecting a new Field Data journey immediately started simulation, the route map could not be opened or returned from at decision points, a generic attack upgrade appeared for Hares, and income modifiers appeared outside shop nodes.

## Findings

1. `StartFieldDataTestJourney` configured the definition and started the run in one click, leaving no Field Data route-preview step.
2. The Field Data definition deliberately did not enable the legacy `JourneyMap` node model. The route HUD toggle also excluded Field Data round decisions and shops, while the preview renderer had no Field Data route summary.
3. When the experimental Hare reward catalog was empty, `ResolveFieldDataOrdinaryRewardPool` fell back to generic movement, attack and block upgrades.
4. The authored starter definition marked +1 FD/Hare as eligible for round rewards and ×1.5 payout as eligible for node rewards. The asset validator and free-choice generator allowed those sources.

## Changes made

- The first route-button action now configures the route and leaves a five-node summary visible on the existing map. A second explicit Start action begins the simulation.
- Closing the selected route preview returns to simulation setup instead of closing the Desktop simulation window.
- During an active Field Data run, the map toggle remains available while running, paused, at round decisions, at shops and at results. The map summary labels all five nodes and their current status; returning from an open map resumes a run only if it was running before the map opened.
- Hare free reward pools now contain only the six approved Hare skill IDs: Tough Hide, Efficient Digestion, Seed Dispersal, Crowding Tolerance, Reproductive Drive and Keen Senses.
- Income offer eligibility is Shop-only in the starter data, authoring validation and free reward generation. Round and node rewards remain species-upgrade choices.
- Updated the gameplay baseline, process/solution drafts, checklist, ticket draft, project context and working state. The earlier T08/T15 source-policy checks remain historical evidence for the superseded rule.

## Validation and remaining work

No Unity compilation, tests, authored-asset load, or visual review was run for these edits. The code and XAML were read back for intended flow only. Verify in Unity that route preview, explicit launch, map open/close from every decision state, shop-only income offers, and Hare reward eligibility all behave as described. Reassess T08/T13/T15 evidence and Trello status after that verification; no Trello change or push was made here.
