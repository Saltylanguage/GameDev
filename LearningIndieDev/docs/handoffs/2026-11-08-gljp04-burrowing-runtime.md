# GLJP-04 Burrowing runtime

November 8, 2026. Local implementation checkpoint for [GLJP-04](https://trello.com/c/DQwFKLSF).

## Journey behavior

- Hares with Burrowing automatically dig at most once per journey when all eight neighboring spaces exist and have no occupying species. A dig spends the provisional 24 energy. Burrows hold three Hares and expire after 200 steps.
- The digger and its descendants through either parent can hide from a Fox whose attack pattern covers their tile. Shared Burrows admits unrelated Hares. Expanded Burrows adds one place per acquired rank, up to five ranks and eight places.
- A hidden Hare leaves the surface grid, so Foxes cannot attack it. It still counts for extinction checks, payouts, result totals, and population limits. Hidden Hares retain their creature state while underground. Aging, metabolism, and gestation timers pause during that time in this first implementation.
- Hares emerge when the burrow and an available exit are outside Fox attack range. Expiry is the first collapse trigger. At expiry, occupants use a free passable exit even if a Fox remains nearby. If all exits are blocked, occupants stay protected until a tile opens. Expired burrows admit no new occupants.
- Checkpoints copy the burrow state and its occupants. Restart clears it. Portable journey digests include occupants, burrow positions and expiry, digger history, and normalized ancestry.

## Gene Lab and Field Data rewards

Burrowing, Shared Burrows, Expanded Burrows, and Born Burrower are in the authored FD2 Hare map. Research unlocks the three journey Mutations. Shared and Expanded require Burrowing to be active during the journey. Equip Born Burrower to start with Burrowing. It requires owned Burrowing research. The four prices are provisional 5 Hare Data and the Shared and Expanded research links to Burrowing are provisional. Open-ground Sprint still needs its terrain and exclusion implementation.

## Validation

- The CellSim.Batch Release build completed with zero warnings and zero errors. Its portable self-test passed.
- Seven focused NUnit cases passed against the standalone assembly. They cover ancestry from before acquisition, threat and expiry emergence, blocked exits, counts, checkpoint copies, Shared Burrows, expansion, and the Born Burrower grant. The Field Data reward and rank-cap assertions also passed using a standalone journey fixture.
- The focused Unity EditMode attempt timed out during startup after 150 seconds without a test report. See `artifacts/gljp04-burrowing-20261108-120152/Unity.log`.

The player can research and acquire the abilities through the Field Data flow. The temporary brown circle and the current Unity evidence are recorded below. GLJP-04 remains open for player review and final research prices.

## Unity Editor verification and UI captures (November 8, 2026)

After reopening the responsive Unity 6.4 Editor, these focused checks passed in the current checkout:

- EditMode `HareBurrowTests`: 7 passed, 0 failed.
- EditMode `FieldDataJourneySessionTests.BurrowingRewardEnablesDiggingOnTheNextJourneyStep`: 1 passed, 0 failed.
- EditMode `FieldDataPermanentProgressionAssetTests`: 1 passed, 0 failed.
- PlayMode `GeneLabOfferCapturesLockedAndActiveStates`: 1 passed, 0 failed.
- PlayMode `LabPlayModeTests.GeneLabResearchPurchaseFlowsIntoFieldDataJourneyChoice`: 1 passed, 0 failed.

The two UI fixtures restored the saved profile store on teardown. PNGs are in `artifacts/gljp04-visual-20261108/`: `gene-lab-locked.png`, `gene-lab-fertile-droppings-active.png`, and `gljp-research-to-journey-choice.png`. These are test-camera UI captures, not player visual acceptance. They cover the current Gene Lab layout and Field Data choice flow, but do not show a Hare entering or leaving a Burrow.

Hidden Hares leave the surface grid. The temporary brown circle identifies each Burrow site while it exists.

## Burrow marker placeholder (November 8, 2026)

A plain brown circle now renders on each active Burrow cell, offset toward the lower-right so it does not cover the Hare's face. The marker is projected from the simulation's burrow locations into `SimulationBoardSnapshot` and drawn by the existing batched `SpeciesSimulationBoard`; it requires no art asset or scene wiring.

Validation after the marker change:

- Unity Editor recompile completed with no errors.
- The seven `HareBurrowTests` passed, including the snapshot projection assertion.
- PlayMode `BurrowMarkerRendersThroughHidingAndEmergence` passed 1/1. The seeded fixture advances the actual Burrowing logic and draws each resulting snapshot in the prototype board. It checks the Hare's surface presence and samples the marker pixel through digging, hiding, emergence, and collapse.
- Current 1600x900 captures and Unity results are in `artifacts/visual-evidence-20261108-205110/`. `01-dug.png` shows the Hare and marker. `02-hidden.png` shows the marker beside the Fox while the Hare is underground. `03-emerged.png` matches the dug image as the Hare returns to the same tile. `04-collapsed.png` shows the Hare without the marker. The ledger at right belongs to the host scene. The fixture supplies the board snapshots.

The circle is the approved temporary shape. Player visual review and final research prices remain open.

## Permanent Gene Lab captures (November 8, 2026)

The permanent Gene Lab inspector uses the existing Burrowing icon and distinguishes its research offer from Born Burrower's starter gene label. The buy quote describes equipping a Genome node. The research description explains that Burrowing becomes a journey Mutation option, and Born Burrower's description explains the launch grant. The Sprint exclusion text describes the planned restriction. Sprint itself still needs its terrain rule and exclusion wiring.

PlayMode `FD2GeneLabPurchasesArrangesAndLaunchesTheSavedPermanentBuild` passed 1/1 after replacing old fixed counts with an exact catalog ID comparison and checking both Burrowing entries. It also verifies purchase, loadout arrangement, save, and launch. The retained result and the inspected `gljp-burrowing-research.png` and `gljp-born-burrower-prerequisite.png` are in `artifacts/visual-evidence-20261108-205549/`.

## Fresh profile to Burrowing choice (November 8, 2026)

The main menu to desktop PlayMode flow found a launch failure. Purchasing Burrowing equipped its Genome node, but the natural rules application rejected the node because it had no stat modifier. The launch contract now recognizes Burrowing, Shared Burrows, Expanded Burrows, and Born Burrower as explicit Hare nodes with effects handled by research or the simulation ability path. Unknown effect-free nodes still fail validation.

`FreshProfileCanResearchAndChooseBurrowingInFieldData` passed 1/1. A fresh profile saw an empty first checkpoint and could skip it. The fixture supplied 15 Hare Data to buy the license and research using the Gene Lab buttons. In the next journey Burrowing started inactive, appeared as the sole choice at the first checkpoint, and entered the run through the visible CHOOSE button. Burrow state was initialized on the next simulation tick. The inspected screenshots and test result are in `artifacts/visual-evidence-20261108-210105/`. This establishes the player UI path to the first checkpoint with fixture funds. Full route survival remains under GLJP-06.

## Unity suite rerun (November 8, 2026)

- Full EditMode passed 757, failed 0, skipped 1. The skipped parity test requires `CELLSIM_FIELD_DATA_PARITY_MANIFEST`. Results are in `artifacts/unity-tests-20261108-210121/EditMode-results.json`.
- The first full PlayMode attempt stalled in the wallet forfeiture fixture at a shop boundary. The fixture repeatedly invoked Continue while the map required a node click. Cancellation timed out. The project Editor was closed and reopened. The fixture now enters the shop and next simulation through the existing node commands, requires tick progress, and checks that the actual extinction round paid nothing. Focused PlayMode passed 1/1 in `artifacts/unity-tests-20261108-210858/`.
- Full PlayMode then completed 97 passed, 2 failed, 2 skipped in `artifacts/unity-tests-20261108-210949/`. The fresh-profile Burrowing and Seed Dispersal flows and the repaired wallet fixture all passed. Both skips require the visual runner.
- The empty-offer assertion failed because the exhaustion setup omitted Burrow rewards even though the journey's research policy admitted them. The setup now uses the same research predicate as the journey, and that test passed 1/1 in `artifacts/unity-tests-20261108-211323/`. The complete suite result above predates this fixture correction.
- GLJP-06's default seed-10100 route still failed. It reached tick 1210 with no Plants or Hares and three Foxes. Ecology values were unchanged. This is the remaining reproduced failure from the suite.

## Full suite and visual follow-up (November 8, 2026, later rerun)

The earlier result above predates the latest fixture corrections. The current full run is `artifacts/unity-tests-20261108-211957`: EditMode **761 passed / 0 failed / 1 skipped**; PlayMode **98 passed / 1 failed / 2 skipped**. The parity skip requires `CELLSIM_FIELD_DATA_PARITY_MANIFEST`; two visual captures are intentionally skipped in the non-visual suite and were run separately.

The only PlayMode failure is `FieldDataTestRouteCompletesThroughAllShopsOnFixedSeed`, seed 10100. It clears the second shop at tick 1200, then the last Hare dies at tick 1210. The third shop is scheduled at tick 1800. The route evidence in `artifacts/visual-evidence-20261108-211940/` records plants reaching zero after tick 914, 357 later Hare starvation deaths, and a combat event for the terminal Hare death. The event does not identify an attacker; no single cause is inferred and no balance values were changed.

The updated Gene Lab and journey language has a fresh-profile capture at `artifacts/visual-evidence-20261108-211649/`; its fresh profile starts with no Mutation offer, buys the Burrowing research, then sees Burrowing as the next journey choice. `artifacts/visual-evidence-20261108-211702/` retains the graphics economy-shop fixture, which passed 1/1. These are test-camera captures, not Bevin's visual acceptance.

Ticket-by-ticket current status, sprint changes, open decisions and remaining estimate are in [GLJP sprint status](../GLJP_SPRINT_STATUS_2026-11-08.md). GLJP-02/03 are Done; GLJP-01/04/05 remain open for scoped decisions and owner review. GLJP-06's source-identical historical comparison remains blocked by the snapshot source-hash guard; do not bypass it or treat the single Unity route as balance approval.

## Portable hidden-state digest regression (November 8, 2026, later)

Added `PortableRunDigestIncludesHiddenHareAndBurrowState` to `Assets/Tests/Editor/CellSimPortableProvenanceTests.cs` and the equivalent case to the `CellSim.Batch self-test`. Both create a Burrow, hide a Hare, and compare a run with a control having the identical visible cell grid but no Burrow state. The portable digest must distinguish them. The Release build completed with zero warnings/errors and `CellSim.Batch.dll self-test` passed, including the new assertion and existing replay tests.

The Unity Editor version of this regression is unverified. A clean Unity CLI run exited before creating an NUnit result because Package Manager could not connect to its local IPC server. The failed Editor left `Temp/UnityLockfile`; the project-targeted graceful `unity close` found no Editor attached to this project, so the lock was not deleted. The test wrapper consequently refuses a second clean run while it sees the lock. Do not claim this as Unity validation.

## Current-source standalone verification — November 9, 2026

With no Unity Editor running and the existing `Temp/UnityLockfile` left untouched, the current shared-source standalone runner was rebuilt:

- `dotnet build tools/CellSim.Batch/CellSim.Batch.csproj --configuration Release --verbosity minimal` — succeeded, 0 warnings and 0 errors.
- `dotnet tools/CellSim.Batch/bin/Release/net8.0/CellSim.Batch.dll self-test` — passed. It verified the portable digest distinguishes identical visible boards with and without hidden Hare/Burrow state; the four-simulation, three-shop 1.5×/1.75×/2× route; the full 18-round replay and its receipts; and snapshot, determinism, purchase, wallet, and research-eligibility checks.

This strengthens the portable validation only. It does not substitute for the still-unverified Unity Editor test for the latest hidden-state digest change or for player visual acceptance.

## Editor recovery and ancestry export verification

The November Editor became reachable again on November 8 at 11:06 PM Eastern. The previously blocked hidden-state digest regression passed in Unity in `artifacts/unity-tests-20261108-230633`.

The portable journey digest now includes recorded Hare ancestry before Burrowing is acquired. Two identical surface grids can represent different families, which can later determine who can hide. The existing pre-acquisition birth regression verifies the ancestry contribution and checkpoint restoration. Older digests retain their original source provenance.

- The full pre-change November baseline in `artifacts/unity-tests-20261108-230644` passed 762 EditMode tests with one skip and 97 PlayMode tests with two failures and two skips. Seed 10100 again failed at tick 1210. Cave Preview failed on a Unity AI Assistant relay log and passed its focused rerun in `artifacts/unity-tests-20261108-231227`.
- After the ancestry change, Unity recompiled with zero errors and warnings. Burrowing tests passed 11/11 in `artifacts/unity-tests-20261108-231141`. Portable provenance tests passed 3/3 in `artifacts/unity-tests-20261108-231146`. The CellSim.Batch Release build and self-test passed.
- Fresh-profile research and journey choice passed in `artifacts/visual-evidence-20261108-231203`. Digging, hiding, emergence and collapse marker checks passed in `artifacts/visual-evidence-20261108-231218`. These fixtures provide current captures for player review.

## Family access through an absent parent — November 9, 2026

Added `GestatedHareCanUseAbsentParentsBurrowAfterCheckpointRestore`. A Hare digs a burrow, mates, and leaves the board before its mate delivers. After restoring a checkpoint, the offspring retains both parents in its ancestry and hides in the active burrow when threatened. A nearby unrelated Hare stays above ground without Shared Burrows.

The November Editor passed all 12 `HareBurrowTests` in `artifacts/unity-tests-20261109-105707/EditMode-results.json`. This was a test-only change. The [GLJP-04 verification comment](https://trello.com/c/DQwFKLSF#comment-6af1eda703b6f10f6a7b4d70) was posted and read back in Current Work. Player review and final research prices remain open.

## Born Burrower at journey launch — November 9, 2026

The November desktop PlayMode fixture buys the Hare path, Burrowing research and Born Burrower through the Gene Lab. The equipped starter and research node together use two of eight capacity points. The next Field Data journey captures Born Burrower in its active Genome. A single Hare with room and energy digs on tick one, before the player has taken any Burrowing Mutation. The inspector labels Open-ground Sprint as a planned exclusion while its terrain contract is open.

`EquippedBornBurrowerCanDigBeforeTheFirstJourneyReward` passed. The inspected 1600×900 Gene Lab and first-tick board captures are in `artifacts/visual-evidence-20261109-111149/`. The authored catalog EditMode test passed in `artifacts/unity-tests-20261109-111113/`. GLJP-04 remains in Current Work for final research prices and player review. [Trello readback](https://trello.com/c/DQwFKLSF#comment-6af1f0f4e4618bae65ba821b)
