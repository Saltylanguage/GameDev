# GLJP unattended implementation checkpoint

- Date: 2026-11-01
- Branch: BevBranch; local uncommitted work, no push
- Direction: Bevin requested as many GLJP tickets as possible without clarification prompts. Questions are parked in [morning review](../GLJP_MORNING_REVIEW.md).
- Design: [PDD](../GENE_LAB_JOURNEY_PROGRESSION_PROCESS_DESIGN.md), [SDD](../GENE_LAB_JOURNEY_PROGRESSION_SDD.md)

## Delivered changes

- Field Data sessions count successful purchased Hares by simulation node. Base allowance is 10; an existing resolved Population Reserves node adds 5 per selected rank. Add Hare rejects at the authoritative transaction boundary before board/wallet changes. Retried request IDs do not consume more slots; removals never refund slots. Node indexing preserves the previous node's history while giving the next node its own allowance.
- The Add Hare control shows remaining purchases and disables at zero.
- The session permits one additive FD/Hare upgrade and sequential payout tiers of 1.5x, 1.75x, 2x. A tier replaces the previous multiplier at the same execution-order position. Previously settled receipts remain immutable. Duplicate/skipped tiers are rejected before spending.
- Authored the 1.75x/40-FD income offer and its validation. Active shop candidates exclude signed species modifiers and already-owned income upgrades. Only the next payout tier is eligible. Historical signed-offer definitions and diagnostic APIs remain available for legacy tests and exports.
- Founding Hares authored price is 5 Hare Data. Escape Instinct is the display name for the existing avoidance Mutation; stable IDs and effect are retained.
- Existing assets and FD2 default use one-point base Genome costs. Updated obsolete two-point tests to assert the approved one-point behavior.
- Fixed portable Genome serialization to preserve MaximumRanks; the previous omission caused ranked node fingerprint mismatch on round trip. Existing portable tests exercise this path.

## Verification

- Initial focused purchase-control checks: 16/16, `artifacts/unity-tests-20261101-232502`.
- Full live EditMode on recompiled assemblies: 705 passed, 0 failed, 1 skipped; `artifacts/unity-tests-20261101-233137/EditMode-results.json`.
- Prior failed intermediate runs are retained. They exposed obsolete two-point expectations, the missing serialized maximum rank, stale live assemblies before explicit recompile, and shop tests that depended on removed duplicate offers. Those tests now express the approved shop contract; legacy species-effect arithmetic tests remain.
- PlayMode and visual results are recorded below when complete. No balance acceptance is claimed.

Full live PlayMode initial run: 69 passed / 14 failed / 2 skipped, `artifacts/unity-tests-20261101-233232/PlayMode-results.json`. Several Gene Lab/Main Menu checks still assert two-point capacity and were updated; focused reruns follow. Other failures remain explicitly unresolved: permanent-results route failures at tick 256, population-control live-refresh count mismatch, invalid `hare.guard` profile launch in Main Menu, continuous-phase currency/reset expectations, and fixed-seed route extinction at tick 239. This run does not prove those failures predate this work.

Focused live Gene Lab PlayMode after correcting the capacity fixtures: **9/9 passed**, `artifacts/unity-tests-20261101-233617`.

Visual runs passed and generated inspected 1280x720 captures:

- Desktop and opening journey reward: 1/1, `artifacts/visual-evidence-20261101-233655`.
- FD2 Gene Lab purchase/reset/save/launch flow: 1/1, `artifacts/visual-evidence-20261101-233713`; `t11-saved-loadout.png` shows four one-point equipped nodes and four remaining slots. Permanent stat labels remain until GLJP-01's catalog redesign.
- New focused round Mutation and Hare allowance rendering regression: 1/1, `artifacts/visual-evidence-20261101-233818`. `gljp-journey-mutation.png` shows the three temporary choices. `gljp-hare-purchases-remaining.png` shows 11 FD remaining and `ADD 1 HARE · 10 FD · 9 LEFT` after one accepted purchase. The test verifies the session counter and displayed count together. No relevant control clipping observed at 1280x720.

## Remaining work by card

Final full live PlayMode: **76 passed / 8 failed / 2 skipped** (86 total), `artifacts/unity-tests-20261101-233903/PlayMode-results.json`. All corrected Gene Lab capacity/price checks and the new rendered allowance test pass. Remaining failures are:

- `FD2PermanentResultsPendingSaveBlocksEveryExitAndRetryPaysOnlyOnce` and `FD2PermanentResultsShowSavedReceiptAndReturnToDesktopWithoutAnotherPayment`: route ends at tick 256 rather than reaching round 3.
- `PopulationControlsRefreshAndSeedCacheUseTheLiveJourney`: expected 114, actual 90. The same failure is documented in the October 26 T09 handoff against its incoming baseline; this historical match does not establish current full-suite parity.
- `ProfileCreationEnablesContinueAndLoadsDesktop`: profile references missing `hare.guard` node at launch.
- `ContinuousPhaseDecisionResumesTheSamePreviewRun`: expected currency 287, actual 0.
- `FieldDataTestRouteCanBeSelectedFromTheJourneyMapBeforeFirstSimulation`: expected FD text, actual No offer.
- `FieldDataTestRouteCompletesThroughBothShopsOnFixedSeed`: seed 10100 extinct at tick 239.
- `PhaseDecisionCanPurchaseAuthoredUpgradeAndResumeSameRun`: expected currency 240, actual 0.

No tests were ignored to hide these failures. No clean full-PlayMode, player-build, or balance claim is made.

| Card | Checkpoint |
|---|---|
| GLJP-01 | Escape Instinct rename done; full research-to-Mutation catalog waits on the research/equipment eligibility distinction and new node costs/prerequisites. |
| GLJP-02 | Base capacity changes and related tests done. Independent researched ranks, one-point equipment at every rank, and old saved-allocation migration remain. The existing POC rank model still spends extra capacity on higher ranks. |
| GLJP-03 | Allowance enforcement and income/shop backend implemented. Full five-rank Population Reserves depends on GLJP-02. Three-tier placement on the longer route remains a product decision. Current short POC route has two shops, so it cannot exercise a third sequential shop visit. |
| GLJP-04 | Parked burrow occupancy/emergence decisions; no speculative hidden-cell/lineage implementation. |
| GLJP-05 | Remaining-count UI done; wider Gene Lab/Mutation redesign and new captures depend on the catalog work. |

## Shared state and board

Preserved pre-existing Noesis/XAML, Genome metadata, recovery files, and working-state edits. No commit or push performed. The Trello browser tab is present, but the browser control returns `browser.user is unavailable` on navigation/rebinding. Card progress comments could not be posted; this handoff is the draft for the next board sync. No card has been marked Done.
