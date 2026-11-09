# Working state

This file is the stable doorway into current collaboration context. It should not
become a master changelog.

**November 9 planning update:** Josh approved [S6 — The Fun Pass](Sprints/S6-control-record.md)
for November 16–29. It is the official next-sprint direction, with kickoff and
task allocation pending; [S5](Sprints/S5-control-record.md) remains active through
November 15. The [publication handoff](handoffs/2026-11-09-codex-s6-fun-pass-plan.md)
records the planning baseline. The November implementation and verification
are now documented in the [GLJP handoff](handoffs/2026-11-09-gljp-salty-handoff.md).
Integration into the Sprint 6 working branch is pending.

## Current focus

**November 9 GLJP handoff for Salty:** The November implementation is recorded on `BevBranch` in `de26d3b`. The [handoff](handoffs/2026-11-09-gljp-salty-handoff.md) links the current behavior, test evidence, and decisions that still need review.

**November 9 GLJP full Unity validation:** EditMode passed 763 with one replay-manifest skip. The full PlayMode rerun passed 100 with two visual-runner skips. The only PlayMode failure was authored Forest Edge seed 10100 at tick 1210. Cave Preview passed after an Editor tooling log failed the first run. See the [closeout evidence and remaining decisions](GLJP_FINISH_PLAN.md#november-9-closeout-evidence).

**November 9 GLJP-06 route flow:** The controlled November PlayMode case passed all four simulations and three shops, reached successful results at tick 2400, and restarted with a fresh wallet. The authored seed 10100 case still failed at tick 1210 after two shops. The authored survival expectation and ecology follow-up remain open. See the [route verification and Trello evidence](handoffs/2026-11-07-gljp06-route-preflight.md#november-9-controlled-route-completion).

**November 8 GLJP-05 research source caption:** Profile-researched Field Data cards now identify the Gene Lab unlock and temporary lifetime. The existing fresh-profile flow buys research, resets the loadout, and proves that owned, unequipped Burrowing or Seed Dispersal still appears and can be chosen. The profileless route fixture shows only the lifetime caption. Both resolutions passed and were inspected. See [GLJP-05 caption evidence](handoffs/2026-11-06-gljp-trello-evidence.md#november-8-gljp-05-research-source-caption).

**GLJP status snapshot — November 9, 2026:** The sprint change log and ticket-by-ticket remaining LOE are in [GLJP sprint status](GLJP_SPRINT_STATUS_2026-11-08.md). Live Trello state last checked today: GLJP-02/03 Done; GLJP-01/06 In Progress; GLJP-04/05 Current Work. After the Editor recovered, the November baseline suite passed 762 EditMode tests (1 skip) and 97 PlayMode tests (2 failures, 2 skips) in `artifacts/unity-tests-20261108-230644`. Seed 10100 again failed at tick 1210. The Cave Preview failure logged a Unity AI Assistant relay connection error and passed in isolation in `artifacts/unity-tests-20261108-231227`. GLJP-01/04/05 still need bounded design/owner review and acceptance; GLJP-06 follows Bevin's Nov 9 direction: November is canonical, no further October-value comparisons; completed October evidence remains historical. The November seed 10100 diagnostic captured the final plant consumed at tick 915 and the terminal Hare combat death at tick 1210, after two shops. See the [GLJP-06 resource diagnostic](handoffs/2026-11-07-gljp06-route-preflight.md#november-seed-10100-resource-depletion). Estimate for GLJP-01 through 05: 1–2 focused engineering days after open decisions and visual review; GLJP-06 effort depends on the bounded November route scope. The portable journey digest now includes Hare ancestry before Burrowing is acquired. Current-source Unity compilation, 11 Burrowing tests, 3 portable provenance tests, and focused fresh-profile and Burrow marker captures passed. The standalone Release build and self-test passed. See the [Burrowing runtime handoff](handoffs/2026-11-08-gljp04-burrowing-runtime.md). No commit or push.

**November 8 GLJP-04 portable digest regression:** Added a test that hides a Hare in a Burrow and proves `PortableRunStateDigest` differs from a control run with the identical visible grid but no Burrow state. The `CellSim.Batch` Release build and portable self-test both passed, including the new assertion and existing journey replay checks. The matching Unity Editor regression is not verified: the clean Editor exited before NUnit results when Unity Package Manager failed its local IPC connection, leaving an unreachable `Temp/UnityLockfile`. The project-targeted `unity close` found no Editor associated with this project; the lock remains untouched, and the runner refuses another clean attempt until the Unity environment is recovered. Detailed evidence is appended to the [burrowing runtime handoff](handoffs/2026-11-08-gljp04-burrowing-runtime.md#portable-hidden-state-digest-regression-november-8-2026-later).

**November 8 design-context cleanup:** Updated the “Current game direction” section in `PROJECT_CONTEXT.md` to replace the stale October 29 permanent Genome POC costs/effects with the later GLJP decisions and current open questions. This is documentation only; no Unity tests were run for this edit.

**November 8 GLJP Unity suite rerun:** The full PlayMode suite completed **97 passed / 2 failed / 2 skipped** (`artifacts/unity-tests-20261108-210949`). The new Burrowing player flow, existing Seed Dispersal flow, and repaired wallet forfeiture fixture all passed. The empty-offer fixture then passed its focused rerun (`artifacts/unity-tests-20261108-211323`) after using the same research predicate as the live session when exhausting the pool. The other failure was GLJP-06's default seed-10100 route, which lost its last Hare at tick 1210 with no Plants and three Foxes. The two skipped tests require the visual runner. See the [validation handoff](handoffs/2026-11-08-gljp04-burrowing-runtime.md#unity-suite-rerun-november-8-2026).

**November 8 GLJP-04/05 fresh profile flow:** Fixed the desktop launch validator rejecting purchased Burrow nodes when equipped. The launch now accepts all four Burrow nodes and leaves their effects to research and simulation. A PlayMode fixture went from a fresh profile with zero offers to a Gene Lab purchase and the visible Burrowing CHOOSE button in the next Field Data journey. The burrow system initialized on the following tick. The focused run passed 1/1. Full EditMode passed 757/0/1, with the replay parity case requiring a retained manifest. See the [launch and capture handoff](handoffs/2026-11-08-gljp04-burrowing-runtime.md#fresh-profile-to-burrowing-choice-november-8-2026).

**November 8 GLJP-05 wallet fixture:** The full PlayMode attempt exposed a loop in the test driver at the first shop. Continue returned the UI to the map, but the fixture never entered the shop or the next simulation. The fixture now enters both nodes, asserts simulation progress, and checks that the actual extinction round earns no payout. Focused PlayMode passed 1/1 (`artifacts/unity-tests-20261108-210858`). This wallet regression uses seed 10100 with Fox movement scent explicitly enabled. The default route survival check remains separate under GLJP-06.

**November 8 GLJP-05 permanent Gene Lab:** The inspector now identifies Born Burrower as a starter gene, uses the existing Burrowing illustration for the research node, and says that buying equips the Genome node. The permanent Gene Lab PlayMode fixture checks all catalog IDs, Burrowing's provisional research offer, Born Burrower's prerequisite, and purchase, save, and launch behavior. It passed 1/1. See the [rendered research and starter states](handoffs/2026-11-08-gljp04-burrowing-runtime.md#permanent-gene-lab-captures-november-8-2026). Full GLJP-05 acceptance remains open.

**November 8 GLJP-04 runtime:** Field Data Hare journeys can acquire Burrowing, Shared Burrows and Expanded Burrows, or start with Burrowing via an equipped Born Burrower. Hares dig, hide and emerge, including after blocked expiry. Hidden Hares count toward the population and persist through checkpoints. A simple brown circle now marks each Burrow on the simulation board. The seven focused burrow tests, journey reward handoff, authored catalog check, and targeted PlayMode marker-render test pass in Unity; focused Gene Lab and journey-choice UI fixtures also pass. The PlayMode board fixture captured digging, hiding, emergence, and collapse. GLJP-04 remains open for player review and final research prices. See the [Burrowing runtime handoff](handoffs/2026-11-08-gljp04-burrowing-runtime.md).

**November 8 GLJP-04 first slice:** Continuous Hare journeys now record both parents at immediate and gestating births and retain the ancestry across checkpoints. Restart clears the ancestry for the discarded run. Both lineage cases passed against the shared standalone runner, and its Release build and portable self-test passed. Unity's focused EditMode process timed out during startup without a result. Burrow creation, hiding, emergence, and Mutation wiring are the next implementation work. See the [lineage handoff](handoffs/2026-11-08-gljp04-hare-lineage.md).

**November 8 GLJP validation update:** The full PlayMode run after the approved 0.5 Hare reproduction baseline reported **93 passed / 4 failed / 2 skipped**. Two baseline-sensitive assertions were updated; focused reruns had passed for the herbivore progression boundary and Fox offer cap behavior before the latest restart. The natural-failure wallet fixture now advances through the configured route, but that run left the Unity Editor unresponsive before producing a result; cancellation timed out. Bevin authorized closing the frozen Editor, so only the exact project Editor process was stopped. A restarted Editor briefly reconnected; a focused rerun request stalled and the Editor process exited without a test result. The fixed-seed route-survival failure remains unresolved; do not tune ecology values without a balance decision. See the [GLJP-01 capability handoff](handoffs/2026-11-08-gljp01-capability-pass.md).

**November 8 GLJP-01 capability pass (local, uncommitted):** Implemented Keen Senses as a distinct +1 predator-detection tile Mutation per rank, separate from Long Vision's general vision range. Larger Litters now caps at rank 3 and rank 3 also raises minimum litter to 2 when needed; its applied snapshot records both effects. The production Hare's reproduction chance is now 0.5. Full EditMode passed **747/0/1** (`artifacts/unity-tests-20261108-110527`), including the new Keen Senses range, litter rank-3, cap, and production baseline checks. The focused Heightened Senses offer PlayMode check passed **1/1** (`artifacts/unity-tests-20261108-110628`). GLJP-01 remains In Progress: minimum-litter Mutation rank limit, Faster Gestation implementation, final costs/prerequisites, and visual/player acceptance remain open. Details in [the capability handoff](handoffs/2026-11-08-gljp01-capability-pass.md).

**November 8 GLJP-03 closed:** The card moved to Done after its explicit allowance, sequential income tier, payout timing, pricing, and idempotency acceptance tests passed in the full EditMode suite (744 passed / 0 failed / 1 parity skip), and the focused current-revision graphics shop run passed 1/1. Its evidence and the unrelated GLJP-06 ecology failure are documented in the [Trello evidence handoff](handoffs/2026-11-06-gljp-trello-evidence.md#november-8-gljp-03-closeout-evidence).

**November 8 GLJP-06 direction and implementation:** Bevin decided ordinary Fox pursuit of Hare movement scent should default off. An independent experimental runtime option now gates Fox acquisition and defaults false; Hare movement markers and dropping scent remain intact. Focused Unity `Simulation` EditMode passed 518/518. The full PlayMode run completed **96 passed / 1 failed / 2 skipped**; a focused rerun confirms its sole failure is ecological extinction in the fixed seed-10100, four-simulation test route at tick 1130: no Plants or Foxes remain, then the final Hares die. This is not a shop/map transition failure. The test now retains recent population counts in its failure message. The pinned scratch full-route A/B is seed-sensitive: seed 10100 failed at tick 237 with pursuit on / closed at tick 1800 with it off, while seed 10108 closed at tick 1800 with pursuit on / failed at tick 236 with it off. Those older diagnostic-runner results are not current Unity journey acceptance. Keep route survival and balance open; no ecological values changed. See the [diagnostic handoff](handoffs/2026-11-07-gljp06-route-preflight.md#november-8-default-decision-and-route-result).

**November 7 GLJP-06 Fox scent result:** The approved November-only test used one diagnostic build and snapshot, with ordinary Fox pursuit of movement scent enabled or disabled. The enabled arm reproduced the previous November results byte for byte. With pursuit disabled, Hare, Fox, and Plant counts matched October at every tick from 0 through 100 for both seeds. This isolates the measured early population divergence. The November 8 decision to default ordinary Fox pursuit off is recorded in the updated handoff. See the [diagnostic handoff](handoffs/2026-11-07-gljp06-route-preflight.md#approved-november-fox-scent-comparison).

**November 7 GLJP-06 follow-up executed:** Bevin approved the first 100 ticks for seeds 10100 and 10108 on both frozen runners. All four detailed standalone traces reached tick 100. Species counts first differed at tick 17 for 10100 and tick 41 for 10108. All four compact batch jobs failed Hare population accounting because the no-preset stat line omits combat deaths. The detailed Hare counts reconcile independently against combat and starvation events. The full route cause remains open. See the [preflight and execution handoff](handoffs/2026-11-07-gljp06-route-preflight.md).

**November 7 GLJP-06 route preflight:** A fresh standalone Release build matched all 55 embedded shared source files to the live worktree. The runner rejected the verified October snapshot at the source hash check and wrote a failure record before simulation. The retained seed-10100 journey reports used the same empty action schedule and already differ at tick 100, before the first reward decision. The seed-10108 Unity fixture reaches Shop, but there is no equivalent current no-action report to localize the divergence. See [preflight results and proposed comparison](handoffs/2026-11-07-gljp06-route-preflight.md) before further GLJP feature work.

**November 7 retained scent telemetry:** The separate current-source seed-10100 Unity reference records 271 Fox ordinary-scent detections and 2,558 scent-pursuit decisions over 1,800 ticks, with zero dropping detections and an empty upgrade loadout (`artifacts/gljp-finite-pool-unity-20261106/artifacts/gljp-parity-current.reference.jsonl`). Ordinary scent was active in that reference run. These aggregate, non-journey counters do not establish detection timing or the cause of the tick-237 journey failure. No experiment run or tuning changed. See [route evidence comparison](handoffs/2026-11-06-gljp-route-evidence-comparison.md).

**November 7 exact-source comparison:** The historical shared source lacks scent processing; the current code adds ordinary Hare scent and carnivore scent memory/pursuit independently of the conditional Fertile Droppings bonus. Git attributes the movement-scent addition to `be98b03`. This is a candidate mechanism for earlier predator-prey divergence, not a demonstrated cause: retained journey reports inspected do not provide scent counters and multiple code paths changed. No tuning or experiment. See [route evidence comparison](handoffs/2026-11-06-gljp-route-evidence-comparison.md).

**November 7 historical-source recovery:** The frozen October runner embeds 50 shared source files. Extracted copies in `artifacts/gljp-historical-embedded-source-20261107/` reproduce the manifest/snapshot source hash; the runner DLL also matches its manifest hash. This corrects the earlier assumption that exact historical shared source was unavailable. Current `LoadSnapshot` rejects a snapshot with a different source hash, so the proposed old-snapshot/current-runner experiment cannot run unchanged. No experiment, historical-file modification or guard bypass was performed. See [route evidence comparison](handoffs/2026-11-06-gljp-route-evidence-comparison.md).

**November 7 route test history:** Git commit `e6cadcd` introduced the authored seed-10100 completion test in the accepted five-node journey work; its original version already used a new profile, the authored scenario and skipped upgrades. The current tick-237 failure contradicts that existing test expectation, but its cause is still unknown. See [route evidence comparison](handoffs/2026-11-06-gljp-route-evidence-comparison.md). The morning review's obsolete eight-failure summary was updated to the latest recorded results.

**November 7 full PlayMode after shop UI changes:** The first isolated graphics run returned **95 passed / 2 failed / 2 skipped** (`Full-after-shop-results.xml`): the known seed-10100 route failure and an obsolete assertion expecting `No copy cap`. The assertion was updated to check the one-time wording and correct reroll visibility, and its focused rerun passed **1/1** (`Shop-assertion-update-results.xml`). The subsequent full run returned **96 passed / 1 failed / 2 skipped** (`Full-after-shop-assertion-results.xml`); only the same seed-10100 failure at tick 237 remains. The two visual fixtures require capture output and have separate enabled runs, one passing synthetic shop fixture and one failing authored route. This is not player visual acceptance.

**November 7 income shop reroll visibility:** Income-only offers no longer show disabled REROLL buttons; species-effect offers retain their existing visibility and enablement rules. The isolated graphics shop fixture passed **1/1**, asserting both rendered income reroll buttons collapsed, purchasing +1 and 1.5x, and exercising paid reorders. Result `artifacts/gljp-finite-pool-unity-20261106/Income-shop-reroll-results.xml`; inspected capture in `income-shop-reroll-capture/`. The fixture uses synthetic populations and the direct test scene's legacy research policy; this is not balance or player acceptance. Full PlayMode predates this local UI edit.

**November 7 authored-route visual failure:** The previously skipped visual capture fixture was enabled in isolated graphics PlayMode. It failed at seed 10100, tick 237, `Failed` before reaching the first shop. Its assertion now reports tick and journey state, and the test captures the unexpected-stop screen before failing. The inspected screenshot shows the expedition failure and 18 Field Data lost. Result `artifacts/gljp-finite-pool-unity-20261106/Authored-route-stop-results.xml`; capture `authored-route-stop-capture/gljp-authored-route-unexpected-stop.png`. This direct test-scene fixture is not a fresh-profile acceptance or balance result. No tuning changed.

**November 7 income shop label and capture:** An enabled visual fixture revealed the shop's incorrect `No copy cap` label for one-time income offers. The label now says one-time purchase; multiplier tiers say they replace the previous tier. The formerly skipped fixture was updated from removed signed-species offers to the current income-only shop. Its latest isolated graphics PlayMode run passed **1/1**, buying both +1 FD per Hare and 1.5x through the UI, then checking paid reorders and later controls. It uses synthetic stable populations and the direct test scene's legacy research policy, so it is neither balance evidence nor fresh-profile acceptance. Result `artifacts/gljp-finite-pool-unity-20261106/Income-shop-both-results.xml`; inspected captures in `income-shop-both-capture/`. The preceding full PlayMode result predates this label edit.

**November 7 checkpoint layout:** The Field Data checkpoint no longer renders an empty bordered `PhaseRewardText` box: its visibility tracks whether the text is nonblank. The isolated graphics rank-five test passed **1/1** checking collapsed and injected nonempty visible states; the fresh-profile checkpoint test passed **1/1** and its inspected capture still shows the no-Mutations guidance and Skip. Results: `artifacts/gljp-finite-pool-unity-20261106/No-empty-message-final-results.xml` and `No-empty-fresh-results.xml`; the fresh-state capture and scoped evidence were added and read back on GLJP-05. The subsequent full graphics PlayMode suite completed **96 passed / 1 failed / 2 skipped** (`Full-after-layout-results.xml`): the sole failure remains the seed-10100 route at tick 237. Player visual acceptance remains open.

**November 7 rank-five allowance integration:** The extended `PopulationReservesRanksArePurchasedInGeneLabForOneSlot` graphics PlayMode test passed **1/1** in the isolated project. The test-funded profile acquired rank 1 through the profile API and ranks 2–5 in the Gene Lab UI; the one-slot node produced 35 allowed Hare purchases, displayed as 35 then 34 left after one successful purchase in Field Data. `Rank-5-journey-results.xml` and `rank-5-journey-capture/` are under `artifacts/gljp-finite-pool-unity-20261106/`. The capture and scoped result were added and read back on GLJP-03; this is not player or balance acceptance.

**November 7 route evidence inspection:** Historical and current retained portable snapshots have equal authored species, terrain, grid and simulation argument payloads, but their no-action seed-10100 reports already differ at tick 100: historical 25 Hares versus current 16, before the first auto-skip. The snapshots have different source hashes, income catalogs and ordinary rewards; equal authored payloads do not prove identical initialization or runtime behavior. This read-only comparison narrows the investigation but does not identify a cause or approve balance. See [route evidence comparison](handoffs/2026-11-06-gljp-route-evidence-comparison.md).

**November 7 research snapshot timing:** `ResearchAcquiredDuringFieldDataIsDeferredUntilTheNextJourney` passed **1/1** in isolated graphics PlayMode. The test changes a test-funded profile directly after a Field Data journey starts; the first checkpoint stays empty, and after abandoning, closing and reopening Simulation the next journey offers the newly researched Seed Dispersal. The injected profile change checks snapshot timing and does not claim the lab is accessible during a running simulation. Results: `artifacts/gljp-finite-pool-unity-20261106/Research-frozen-results.xml`; the result was posted and read back on GLJP-01.

**November 7 Gene Lab to journey integration:** The graphics PlayMode test `GeneLabResearchPurchaseFlowsIntoFieldDataJourneyChoice` passed **1/1** in the isolated project. A new test-funded profile buys the license and Seed Dispersal through the Gene Lab UI, opens Field Data without manually binding research, receives Seed Dispersal as its sole first-checkpoint choice and applies it. The test does not establish natural earning, every research option, later checkpoints, player acceptance, or balance. Evidence: `artifacts/gljp-finite-pool-unity-20261106/Research-to-journey-results.xml` and `research-to-journey-capture/gljp-research-to-journey-choice.png`. The capture and a scoped comment were added and read back on GLJP-05.

**November 7 focused replay parity:** The previously skipped Unity-to-standalone Field Data parity test passed **1/1** in the isolated project using a newly exported current-source snapshot and two newly generated standalone replay fixtures (no actions and population actions). Both fixtures reached natural failure at ticks 237 and 246; later shops and successful completion are outside this evidence. Results: `artifacts/gljp-finite-pool-unity-20261106/Parity-current-results.xml`; manifest and inputs are in its `artifacts/gljp-parity-*` files. The full EditMode run below still records its original skip. The focused test is correctness verification, not a balance experiment; historical frozen files were not changed. The result and limitations were posted and read back on GLJP-05.

**November 6 GLJP-05 empty state:** The Field Data empty-reward message now covers capped or excluded choices as well as unresearched choices and clarifies that further Gene Lab research applies to a future journey. An isolated broad PlayMode run failed on a relay error. A shorter one-round test initially checked before the next frame refresh, then passed **1/1** after yielding a frame; its two inspected captures show the empty message, Skip, and the Hare allowance decreasing to 9. The isolated test does not replace full PlayMode or player acceptance. See [handoff](handoffs/2026-11-06-gljp-empty-reward-message.md).

**November 6 full PlayMode rerun:** The full suite in `artifacts/gljp-isolated-unity-20261106-173144` was deliberately stopped at Editor PID 34852 after its working set grew from roughly 3.9 GB to over 8 GB while the log remained quiet and its fixture had a potential billions-of-iterations loop. There is no completed full-suite result; do not classify it as a pass or assertion failure. Before launch, every file in source Assets and Packages matched the copy by hash; source ProjectSettings matched except `ProjectSettings.asset` (intentional copied product-name isolation) and `ShaderGraphSettings.asset` (line-ending-only diff). A reverse inventory while running found only an `InitTestScene` and its meta in copied Assets, created at suite startup; no copy-only files in Packages or ProjectSettings.

The last logged PlayMode activity traces to `FieldDataTestRouteCanBeSelectedFromTheJourneyMapBeforeFirstSimulation` at `SpeciesPresentationPlayModeTests.cs:330`, inside the seventh round's 100 simulation steps. The last tracked Hare transition reports age 641, the same age as the final tracked transition in the earlier isolated broad run that ended on the relay error. The Editor was still responding and accumulating CPU after log output stopped; this identifies the last logged work, not a completed test or proof that simulation itself is stuck.

Investigation found a concrete subsequent test-fixture hazard: the same test loops until it exhausts each eligible reward, while both litter Mutations currently have `int.MaxValue` rank limits and the default unbound research policy permits them. The local test now supplies a finite researched pool excluding those two unlocks; it still checks a real exhausted-pool/Skip state. The revised filtered test passed **1/1 in 3.98 seconds** in `artifacts/gljp-finite-pool-unity-20261106` (`Finite-pool-results.xml`, `Finite-pool.log`). Its copied product name isolates its profile and the revised test hash was verified. The subsequent full batch PlayMode run completed **90 passed / 5 failed / 2 skipped** (`Full-playmode-results.xml`, `Full-playmode.log`): a Gene Lab test could not find the rendered `hare.escape-instinct` button; the fixed-seed natural-failure test reached Shop instead of Failed; the seed-10100 success route failed at tick 237; and two terrain tests failed because `WaitForEndOfFrame` is unavailable in batch mode. The fixture fix passing does not settle those failures or imply user acceptance. Litter rank limits are parked in [morning review](GLJP_MORNING_REVIEW.md); no player-facing cap was invented. The old stopped run's last logged line alone does not prove it reached the loop.

The Gene Lab case passed **1/1** in a graphics-enabled, non-batch filtered rerun in the same isolated project (`GeneLab-graphic-results.xml`, `GeneLab-graphic.log`, `genelab-rerun-capture/`). The subsequent full non-batch PlayMode run completed **92 passed / 3 failed / 2 skipped** (`Full-graphic-results.xml`, `Full-graphic.log`, 129.6 seconds): Gene Lab still failed to find the rendered Escape Instinct button, natural-extinction seed 10108 reached Shop, and fixed route seed 10100 failed at tick 237. The two batch-only `WaitForEndOfFrame` failures disappeared. The focused Gene Lab pass versus full-suite failure suggests an execution-context or ordering difference but does not establish the cause. No ecology tuning has been changed to force a pass.

The focused Gene Lab test without capture reproduced the missing rendered button (`GeneLab-no-capture-results.xml`, 0/1); the passing visual capture helper waits at least ten frames for layout. The local test now waits up to 60 frames for each rendered new research node before asserting and clicking it. A non-batch no-capture focused rerun passed **1/1** (`GeneLab-wait-results.xml`, `GeneLab-wait.log`); the wait still asserts if a button never appears. The full graphics-enabled PlayMode rerun with this change completed **93 passed / 2 failed / 2 skipped** (`Full-graphic-wait-results.xml`, `Full-graphic-wait.log`, 131.7 seconds): Gene Lab passes, while the seed-10108 natural-failure expectation and seed-10100 route completion remain failures. This is isolated testing, not balance or player acceptance.

The natural-failure fixture now asserts the actual `Run.Seed` after journey start, because `PrepareNextRun` uses the base seed plus a run counter. A focused diagnostic (`Natural-seed-check-results.xml`, `Natural-seed-check.log`) confirmed the actual seed was 10108 but still reached Shop rather than extinction. The October 19 report used 10108 for natural failure under an older reward setup; the current fresh-profile fixed-route test separately observes seed 10100 failing at tick 237. The local wallet-forfeit fixture now uses 10100 to exercise that observed round-three extinction and passed **1/1** (`Natural-seed-10100-results.xml`, `Natural-seed-10100.log`). The subsequent full graphics-enabled PlayMode run finished **94 passed / 1 failed / 2 skipped** (`Full-final-results.xml`, `Full-final.log`, 130.3 seconds): wallet forfeiture passes, while the independent route-completion test still fails for seed 10100 at tick 237. No behavior or ecology value has been changed; selecting a fixture is not balance approval.

The October matched spending comparison included a policy that skipped all rewards, so research gating alone is not a demonstrated explanation for changed seed outcomes. The earlier exact source identity includes uncommitted code; the cause remains unproven. Current isolated test copies of the three changed PlayMode files and simulation shell match the worktree by SHA-256. See [route evidence comparison](handoffs/2026-11-06-gljp-route-evidence-comparison.md).

**November 7 EditMode refresh:** The first full isolated run returned **743 passed / 1 failed / 1 skipped** (`EditMode-current-results.xml`): the failure was `PortableManifestCoversEveryBatchSharedSource` because the copied project omitted `tools/CellSim.Batch/CellSim.Batch.csproj`; the skipped parity test requires an explicit retained manifest. The provenance test reads that project XML, so the exact worktree csproj was copied and hash-verified into the isolated project. The full rerun completed **744 passed / 0 failed / 1 skipped** in 35.5 seconds (`EditMode-with-manifest-results.xml`, `EditMode-with-manifest.log`). The skipped test is `FieldDataReplayParityTests.FrozenJourneyActionsMatchUnityAtEveryDecisionAndTerminalState`, which requires `CELLSIM_FIELD_DATA_PARITY_MANIFEST`. The three changed PlayMode files and simulation shell were hash-checked before the first run. The copied project uses a separate product name/profile; this result does not prove player acceptance or Unity-to-standalone parity.

The historical October snapshot and frozen runner are locally available; the snapshot SHA-256 matched its parity manifest on November 6. A proposed, unapproved two-seed comparison and its compatibility/identity gates are in [GLJP route seed diagnostic](Research/Experiments/GLJP-Route-Seed-Diagnostic-20261106.md). No new experiment was run and no historical evidence was modified.

**November 6 GLJP-04 investigation:** Mapped the two birth paths, conception-time mate identity, movement copies, checkpoint state, grid-only population counts and portable digest constraints for the approved either-parent descendant rule. Existing birth telemetry is optional and lacks authoritative mate identity; tagging only future births would miss descendants already alive when a burrow is made. No burrow behavior or edge-condition decision was implemented. See [investigation](handoffs/2026-11-06-gljp-burrow-lineage-investigation.md).

**November 6 GLJP board evidence:** Updated and read back all five GLJP cards with dated implementation, test and outstanding-work evidence; attached inspected screenshots to GLJP-01, 02, 03 and 05, preserving descriptions and list positions. GLJP-04 records the unimplemented burrow behavior and open decisions. GLJP-02 remains Done; the others remain open. See [handoff](handoffs/2026-11-06-gljp-trello-evidence.md). Latest-revision full PlayMode remains unavailable after the Editor pipeline disconnected; GLJP is incomplete and unpushed.

**November 6 GLJP standalone integration:** Restored the standalone runner build after research gating introduced a profile dependency, kept the profile inputs in the portable source hash, and corrected the replay self-test to check rejection before the reward is resolved. Release build and expanded standalone self-test pass, including a four-simulation portable journey with three successive tier purchases; the result was added to GLJP-03 and read back. Unity PlayMode and cross-runner parity remain unverified on this revision. See [handoff](handoffs/2026-11-06-gljp-standalone-build.md). GLJP remains incomplete and unpushed.

**November 6 GLJP Codex pickup:** Escape Instinct, Increased Minimum Litter, and Larger Litters are provisional 5 Hare Data research roots. Faster Gestation stays locked. Full EditMode is 744 / 0 / 1 (`artifacts/unity-tests-20261106-163829`). The following PlayMode run timed out when the Editor Pipeline disconnected, so it has no result. See [Codex pickup](handoffs/2026-11-06-gljp-codex-pickup.md). No push. GLJP-01's Trello description still has unsaved edits.

**November 6 GLJP third-shop route:** Journey length now follows the route. A test-only four-simulation route buys the 1.5×, 1.75×, and 2× payout tiers at three separate shops. The authored starter route is still five nodes, and its fingerprint is unchanged. Focused EditMode `FieldDataJourney` is 46/46 (`artifacts/unity-tests-20261106-143107`). The seed-10100 survival failure and exact production shop placement remain open. See [handoff](handoffs/2026-11-06-gljp-third-shop-route.md). No push.

**November 6 GLJP testing and route investigation:** Fixed standalone expedition currency being erased at restart, updated stale shop/profile/population UI test setup, and made permanent-results tests use the stable acceptance scenario. At that checkpoint full EditMode was 742 / 0 / 1 and PlayMode 94 / 1 / 2; the authored route seed failed at tick 239. The updated delayed-start test and results visual tests pass focused; saved, pending, and failed results captures were inspected. The newer route-length foundation is described in the third-shop handoff above. See [handoff](handoffs/2026-11-06-gljp-playmode-and-route.md) and [morning review](GLJP_MORNING_REVIEW.md). GLJP remains incomplete and unpushed.

**November 6 GLJP research capacity:** Crowding Tolerance and Reproductive Drive are provisional-price research unlocks; buying a ninth research node with 8/8 equipped preserves ownership without equipping it. Crowding and full-loadout purchase have inspected passing in-game captures. The default Hare reproduction chance is already 100%; Field Data suppresses the no-effect Reproductive Drive offer at that cap, while a test-only 50% scenario confirms the Mutation works. Balance choice remains open. Full EditMode 742 / 0 / 1; full PlayMode 87 / 8 / 2, with the same eight failures as the earlier November 6 run. See [handoff](handoffs/2026-11-06-gljp-crowding-reproduction.md). GLJP remains incomplete and unpushed.

**November 6 GLJP continuation:** Founding Hares is a permanent one-point starter gene (5 Hare Data, +5 at launch), and Seed Dispersal is one research unlock with the older Fertile Droppings concept removed from the permanent lab; Seed's 5-data price and root path are explicitly provisional. Long Stride is available as a temporary Field Data choice. Each has an inspected, passing in-game capture. Full EditMode 738 / 0 / 1 and PlayMode 84 / 8 / 2; the eight failure names match the earlier run. GLJP-01 and the wider slice remain incomplete. See [handoff](handoffs/2026-11-06-gljp-starter-seed.md) and [open decisions](GLJP_MORNING_REVIEW.md). No push.

**November 5 GLJP-02 complete:** Ranked Population Reserves, one-point equipment, per-node purchase allowance and version 9 migration are implemented locally. The generic and FD2 paths count one point per active node while preserving legacy effect metadata; oversized legacy loadouts are preserved but cannot launch or be re-created. Full EditMode was 732 passed / 0 failed / 1 skipped (`artifacts/unity-tests-20261105-203710`); Gene Lab rank-5 visual passed (`artifacts/visual-evidence-20261105-204015`). PlayMode previously reported 79 / 8 / 2 with eight previously recorded failure names. GLJP-02 was marked complete and moved to Done on Trello, read back November 5. Local work is uncommitted; no push. See [handoff](handoffs/2026-11-05-gljp02-rank-foundation.md).

**November 5 GLJP-01 continuation:** Existing Tough Hide, Heightened Senses, Long Stride and Efficient Digestion research nodes no longer grant permanent stats; their stable IDs, ownership, prices and prerequisite paths remain. Heightened Senses offers Long Vision and Keen Senses; Long Vision's former two-tile test effect and one rank are provisional, not balance-approved. Both choices and descriptions were captured in-game, and Long Vision was selected; see [handoff](handoffs/2026-11-05-gljp-capacity-senses.md). Full EditMode 735 / 0 / 1; final full PlayMode 80 / 8 / 2, with the same eight failure names as the prior run. Keen Senses currently tracks only creature food targets, so its practical Hare behavior needs review. GLJP-01 is in Trello In Progress; the rest of the catalog, starter genes and Burrowing remain incomplete.

**November 5 human acceptance:** Bevin confirmed the GLJP-01 research gate is working as intended after testing it. This accepts the gate and its fresh-profile flow; the remaining catalog work stays open. See [acceptance record](handoffs/2026-11-05-gljp01-research-gate.md).

**November 5 GLJP-01 research gate:** Player-profile journey offers now require permanent owned research, independently of equipment. Opening/phase/Field Data and authored reward paths are gated, application checks reject locked Mutations, and fresh profiles can continue/skip empty rewards. Verified full EditMode 708/0/1, real-profile empty/owned opening checks 1/1 each, and inspected empty-research Field Data capture 1/1. See [handoff](handoffs/2026-11-05-gljp01-research-gate.md). Current permanent stat bonuses and unauthored research nodes remain catalog work; GLJP-01 is partially complete. `Bev Test 11/5/2026` remains zero-research. Local uncommitted changes; no push.

**November 1 overnight GLJP implementation:** Per-node Hare purchase allowance, remaining-count UI, one-time additive income, replacement payout tiers including 1.75x/40 FD, active-shop duplicate removal, Founding Hares price 5, Escape Instinct naming, and portable maximum-rank preservation are implemented locally. Full EditMode is 705 passed / 0 failed / 1 skipped (`artifacts/unity-tests-20261101-233137`). Final full PlayMode is 76 passed / 8 failed / 2 skipped (`artifacts/unity-tests-20261101-233903`); focused Gene Lab is 9/9 and three inspected visual runs pass 1/1 each. Remaining runtime failures are enumerated in the handoff. See [handoff](handoffs/2026-11-01-gljp-overnight-progress.md) and [morning questions](GLJP_MORNING_REVIEW.md). Ranked research/save migration, full capability catalog, and burrowing remain incomplete. Trello browser controls are unavailable; no card closed and no progress comment posted. Existing local UI/recovery work is preserved.

**November 1 Gene Lab/Journey progression design:** Created [PDD-GLJP-001](GENE_LAB_JOURNEY_PROGRESSION_PROCESS_DESIGN.md) and [SDD-GLJP-001](GENE_LAB_JOURNEY_PROGRESSION_SDD.md) from Bevin's item-by-item decisions. Five implementation tickets are in Trello Current Work: [GLJP-01](https://trello.com/c/dR2z6ENf/149-gljp-01-replace-hare-genome-stat-nodes-with-researched-journey-unlocks), [GLJP-02](https://trello.com/c/o4q7Necn/150-gljp-02-enforce-one-point-genome-capacity-and-ranked-population-reserves), [GLJP-03](https://trello.com/c/h4wxvWAp/151-gljp-03-add-hare-purchase-allowance-and-journey-shop-income-tiers), [GLJP-04](https://trello.com/c/DQwFKLSF/152-gljp-04-implement-burrowing-family-hiding-and-burrow-upgrades), and [GLJP-05](https://trello.com/c/Htt7PJBY/153-gljp-05-integrate-gene-lab-and-journey-upgrade-ui-tests-and-captures). All five cards include acceptance criteria and are in Current Work. The final capacity decision is 1 point per equipped node, max 8; existing 2-point Genome asset values and FD2 default constant are now changed to 1. A focused Unity run found one stale FD2 capacity assertion; it has been updated and the focused rerun is in progress. This is local uncommitted work; do not push without an explicit request.

**November 1 Salty coverage sync:** Fast-forwarded `BevBranch` from `b95941e`
to `8659a1b`, matching `origin/codex/audio-direction-and-sourcing`; local
`BevBranch` is 12 commits ahead of `origin/BevBranch` and has not been pushed.
Salty's three November 1 commits add the weekly context handoff and three
project skills, without new gameplay/runtime code. The handoff says coverage
does not transfer ownership or assign Salty's open design items. See
[the November 1 handoff](handoffs/2026-11-01-codex-sim-weekly-handoff.md).
The user's request in this session is to review and package context for Salty's
return; unresolved prices, loadout rules, avoidance integration, and Gardener
food-model dependencies remain open.

**November 1 local UI slice:** Local, uncommitted Noesis changes apply the
concept-art presentation to Gene Lab and temporary journey upgrade cards while
preserving options and mechanics. Three focused visual tests passed 1/1 each.
Gene Lab capture: `artifacts/visual-evidence-20261030-131448`; paused journey
upgrade capture: `artifacts/visual-evidence-20261030-131748/journey-upgrade-choice.png`.
The full suite was not rerun for this visual slice; earlier full-suite failures
remain separately documented. Existing local `WORKING_STATE.md` edits, Unity
metadata, and `_Recovery` files were preserved while integrating the handoff.

**October 29 permanent Gene Lab concept preview:** The Sim/FD2 Gene Lab now
shows Faster Gestation, Fertile Droppings, High Crowding, and Increased Minimum
Litter alongside the permanent five-node tree. These four nodes are visibly
locked concepts; purchasing and loadout controls remain disabled, and the panel
states that their permanent prices and loadout rules are undecided. The legacy
POC source catalog supplies their descriptions and branch labels. The connected
tree now uses larger illustrated cards with a title, description, status, and
selection footer, styled after the journey-choice cards. A scroll area preserves
the larger node size as the tree grows. Twelve transparent trait sprites use
Chrono's in-game rabbit as their art reference. At the October 29 checkpoint,
an updated Unity capture was pending. The October 30 follow-up captured and
visually reviewed the Gene Lab panel and journey upgrade choice; see the local
UI slice above and the dated project log.

**October 29 Rabbit Genome POC:** The production test catalog now offers eleven
selectable nodes, including the existing POC stats/lifecycle effects, Faster
Gestation, and Increased Minimum Litter. Existing POC IDs are preserved. The
minimum-litter node costs 1 Rabbit Data and 1 capacity point; Forest Edge's
minimum rises from 2 to 3. The expanded Genome authoring suite passes 5/5 in an
isolated Unity project copy; no desktop Editor or active game was controlled.
These values are provisional playtest baselines, not production balance targets.
High Crowding uses the existing +0.1 reduction to extra crowding energy loss.
Avoidance still needs a Genome-to-simulation integration path; Gardener food
abilities remain behind the food-model gate. See the
[implementation plan](RABBIT_GENOME_IMPLEMENTATION_PLAN.md#rg-03--finish-stat-only-offers-before-adding-structural-mechanics).

**October 29 Field Data route verification:** Sim's accepted five-node Field
Data journey, shop and reroll flow, ordered income controls, interventions, and
APS accounting are merged with the Rabbit Genome and population-control work.
Forest Edge remains at 400/20/10. The player wallet is unavailable and cannot
be spent during the opening six-round simulation; it becomes available at the
first shop boundary. The audit ledger still records those round receipts. A
shop-route PlayMode test now passes through the first shop, purchase, and next
simulation (1/1), and the population-control EditMode class passes (16/16).
The accounting failure was caused by paid removals being deducted twice from
expected final population; commit `bc864346` fixes both herbivore and predator
stat lines. Test fixtures now earn spendable Field Data through the opening
simulation and shop. The original Editor checkout remains locked, so a hands-on
Editor pass and shop screenshot were still open at the time of this note. On
October 29, Josh reported completing the journey manually and said the flow made
sense. No specific choice, confusion point, or fairness judgment was described.
Exact run settings, choices, and wallet receipts were not supplied, so this is
qualitative route feedback, not a reproducible route record. No new ecology
experiment ran. See the
[RG-02 route review](Research/2026-10-22-RG-02-normal-route-review.md).

**October 28 owner review:** Bevin said the current state visually seems to
work and pass and he is fine with it as it stands. Noesis expiry interrupted
result-screen and uninterrupted-return review; the saved settlement and reopened
Gene Lab balances were verified. Fox ecology remains an open balance question.
Bevin identified future design work to define Gene Lab behavior and its
distinction from run Mutations and Field Data-funded choices. No new mechanics,
balance or release approval was implied.

**Latest October 28 playtest choice:** Bevin reported that 400/20/10 led to
Hare collapse too quickly to test or play and chose to return to 400 Plants /
55 Hares / 20 Foxes for continued playtesting. This is a current playtest choice,
not balance acceptance. Per-round Field Data spending is enabled. Bevin confirmed
a live round-one population purchase worked; 16 FD became 6 with one intervention.
The earlier population correction below is historical; the journey continues.

**October 28 FD2 integration and gameplay validation:** FD2 is integrated into
local unpushed `BevBranch`. The original 43 dirty/untracked files were
hash-backed up and retained in stash `520cc9cb`; recovery files and earlier
working-state content remain. The older 400 Plants / 55 Hares / 20 Foxes
starting population was mistakenly restored in `a2035b3` and the test was
aligned in `28de090`; an October 22 decision selected 400/20/10 for launch.
Bevin chose to stop the 55/20 playthrough and correct the setting. Corrected full EditMode: 701 passed, zero failed, one
existing skip; focused graphics PlayMode: 8/8. Related PlayMode: 21/25 pass,
the same four failing test names as the earlier baseline (the fixed-seed failure
now occurs at tick 237). Portable build and self-test passed.

Bevin approved a 200-seed matched natural balance screen; 400/400 journeys
completed and were independently audited. Baseline versus four selected nodes:
31/200 versus 30/200 successes; 71 versus 42 had living Hares at tick 600.
Five seeds succeeded in both, 26 baseline only and 25 four-node only. This 55/20 screen is not the October 22 launch setting, the exact T12 live
loadout, or balance approval. The factual report and
per-seed evidence are in `artifacts/fd2-integration-20261028/natural-screen-200x2/`.
Unity is open in Edit mode; interactive play awaits profile choice. No push,
player-build, tuning or production balance acceptance. See the
[integration handoff](handoffs/2026-10-28-1310-codex-fd2-integration-and-gameplay-validation.md).

**October 28 Field Data timing update:** Bevin chose to make each settled
round's Field Data spendable at its pause, superseding the October 22 delay for
the Field Data route. Mutation choice still resolves before 10-FD population
controls. Corrected 400/20/10 launch settings and the new timing are applied
locally; 41/41 focused journey/population EditMode and 1/1 route UI PlayMode
passed in isolation, and the original Editor compiled cleanly. Live manual
purchase and complete gameplay remain pending. Legacy POC timing is separate.

**October 27 evening T12 complete after live review:** Bevin explicitly accepted
T12 result/Gene Lab clarity after success,manual purchases,paid reset/free
replacement and failure. Live review exposed a purchase trap: buying a new node
after reset immediately activated it and ended free selection. Local code
`330d94d` requires applying the replacement first and explains why. Full
EditMode701 passed/0 failed/1 existing skip;focused graphics8/8 and final guard
capture1/1 pass. The actual user-selected build reloads and launches;confirmed
End forfeits20 newFD and retains172Hare/0RD,fiveowned,fouractive,five receipts.
Demo Editor closed;temporary storage/helper/import residue cleaned;original
exact profile keys/four source hashes unchanged. No push,merge,original-source
integration,player-build or balance acceptance. T12 is marked complete in live Done; final acceptance comment
`#comment-6ae1567563aaa80267e1df6a` posted/read back,description preserved.
Proof:`artifacts/fd2-board-20261027/t12-done.png`. Preserve earlier dated
evidence below as history. See the
[reset-correction handoff](handoffs/2026-10-27-2147-codex-fd2-t12-protect-reset-selection.md).


**October 27 FD2-T12 automated acceptance complete; human review pending:**
Four end-to-end cases pass: zero-wallet earned progression plus v2, local v3
and incoming Rabbit v3 migration. Three real18-round successes fund Research,
license, four nodes, a20 RD reset and free replacement; reload/next launch
retain the selection and confirmed End forfeits only20 new FD. Duplicate
callbacks/reloads never pay twice. A real reopen auto-start defect was fixed:
configured Field Data journeys now wait at the map for explicit start.
Full EditMode700 passed/zero failed/one existing parity skip. All29 focused
scene cases pass within33 total; the same4 wider baseline failures remain.
Final graphics4/4 and ten1280x720 captures inspected. Portable build/self-test
pass. Exact original preferences/four source hashes unchanged; original Editor
ready. Controlled survival settings are documented, with no ecology/balance,
player-build or full PlayMode green claim. Code `ada1759` is local/unpushed.
T12 stays incomplete in live In Progress until Bevin confirms screen clarity;
evidence comment `#comment-6ae10ab7d93167a216017b9b` was posted/read back,
original description preserved. Proof: `artifacts/fd2-board-20261027/t12-awaiting-review.png`.
See the [acceptance review](FD2_PERMANENT_PROGRESSION_ACCEPTANCE.md) and
[T12 handoff](handoffs/2026-10-27-1613-codex-fd2-t12-permanent-progression-end-to-end-acceptance.md).

**October 27 FD2-T11 Gene Lab controls verified:** MainMenu profile launch
now uses the permanent Hare catalog and shared Research purchases. Wallets,
prices, resulting balances, prerequisites, owned/active/draft states and capacity
are shown. Each replacement requires a fresh20 RD reset with no refund; select
owned nodes from empty and apply once for free. Save errors/retry, stale profile
and reset generation, insufficient funds and over-capacity states are explained.
Legacy refundable Hare allocations retain labeled POC controls until their
legacy reset; direct authoring-scene entry stays POC. Running T09 snapshots
remain frozen. New domain10/10 and scene4/4 pass; focused T11/POC8/8. Full
EditMode700 passed/zero failed/one existing parity skip. Final graphics4/4 and
seven1280x720 captures inspected, including scrolling and pinned save errors.
Portable build/self-test pass. Related PlayMode25/29 pass; all4 failures exactly
match retained baseline messages (POC7-vs-9 options, population114-vs-90,
natural failure reaches Shop, seed10100 fails tick882). Exact original prefs and
four source hashes unchanged; original Editor ready. Code `f2a8565` is local,
no push or original-source integration. T11 is complete in live Done, comment
`#comment-6ae0ff1fafaa9b0d7ede7fef` posted/read back, description preserved.
Proof: `artifacts/fd2-board-20261027/t11-done.png`. Next T12 end-to-end acceptance.
See the [T11 handoff](handoffs/2026-10-27-1522-codex-fd2-t11-gene-lab-permanent-economy-ui.md).

**October 27 FD2-T10 settlement results verified:** The accepted18-round Field
Data journey now shows the persisted permanent receipt: leftover FD, rate,
frozen research, species deposit, +10RD and before/after wallets. Failure shows
forfeiture and permanent retention. Pending results explicitly confirm no paid
deposit and guard exits/start/reset until retry saves; pending reload and result
reentry preserve exactly-once payment. Older POC rewards remain unchanged.
New domain11/11 and scene3/3 pass, including a repeated final scene run. Full
EditMode690 passed/zero failed/one existing parity skip. Rendered saved/pending/
failure views inspected at1280x720; portable build/self-test pass. Related
PlayMode21/24 pass: the population-control baseline and two older fixed-seed
route failures remain. Both route failures reproduce exactly against pre-T10
HEAD26ca438, including tick882. Exact original preferences/four source hashes
are unchanged; original Editor is ready. Code `1b7d9ab` remains local with no
push or original-source integration. T10 is complete in live Done; evidence
comment `#comment-6ae0c7bb937b17df3d1fc85d` was posted/read back, description
preserved. Proof: `artifacts/fd2-board-20261027/t10-done.png`.
Next T11 wires visible Gene Lab controls; T12 retains end-to-end acceptance.
See the [T10 handoff](handoffs/2026-10-27-1137-codex-fd2-t10-permanent-settlement-results-and-retry.md).

**October 27 FD2-T09 launch freezing verified:** Permanent Hare ownership uses
the canonical FD2 resolver; POC/other-species catalogs retain their own rules.
Desktop explicitly captures the owning profile at first simulation start,
including direct next-journey commands. Opening Mutations/map choices are retained,
and every participating species gets its own Genome effects. Running journeys,
phases/checkpoints/results retain frozen values after profile changes. Field
Data route configuration retains the prepared Genome. All13 new domain cases
pass; full clean EditMode679 passed/zero failed/one parity skip. Related
PlayMode17/18 pass, including all4 new T09 cases and existing reward/unlock
journeys; the sole failure is the unchanged incoming population-control
expected114/actual90 baseline. Portable build/self-test pass. Exact original
preferences and four source hashes are unchanged; original Editor reports ready.
Work remains local on `codex/fd2-t01-integration`, with no push/source integration.
T09 code commit `8d9cf30` is local. Its card is complete in live Done with
verification comment `#comment-6ae0bed4662be98c593cf2a8` posted/read back;
proof: `artifacts/fd2-board-20261027/t09-done.png`. T01-T08 remain Done. Next T10 wires
settlement/failure results; visible Lab controls remain T11. See the
[T09 handoff](handoffs/2026-10-26-2315-codex-fd2-t09-freeze-permanent-genome-at-launch.md).

**October 26 FD2-T08 reset/arrangement verified:** Bevin confirmed a fresh
20 RD reset for each replacement loadout. Reset clears active Hare nodes without
refund or ownership loss; free full selection applies from empty, and a changed
selection after application requires another paid reset. Identical selection
is a no-op. Schema v7 preserves a per-Genome reset generation for stale-callback
protection; v6/older migrate at zero, while corrupt v7 records are recovered.
All36 new cases/all132 profile transaction cases pass. Full clean EditMode666
passed/zero failed/one parity skip; independent Editor write/read checks,
focused PlayMode8/8 and portable build/self-test pass. Exact original preferences
and four source hashes are unchanged; original Editor reports ready. Work is
local on `codex/fd2-t01-integration`, with no push or original-source integration.
T08 code commit `d90c2f5` is local. Its card is complete in live Done, with
verification comment `#comment-6ae013d687320fb2409a962e` posted and read back;
proof: `artifacts/fd2-board-20261026/t08-done.png`. T01-T07 remain Done. Next T09 freezes
the FD2 loadout into the next launch; visible controls remain T11. See the
[T08 handoff](handoffs/2026-10-26-2254-codex-fd2-t08-reset-and-arrange-permanent-loadouts.md).

**October 26 FD2-T07 permanent Hare nodes verified:** The new FD2 purchase
charges canonical 5 Hare Data, permanently unlocks and activates in one saved
transaction. Matching profile, Hare path license, owned ancestors and the full
8-point catalog capacity are required. Permanent purchases create no refundable
POC allocations. All32 new node cases/all96 profile transaction cases pass;
full clean EditMode630 passed/zero failed/one parity skip. Independent Editor
write/read checkpoints, focused PlayMode7/7 and portable build/self-test pass.
Original profile/recovery preferences and all four original source hashes are
unchanged; original Editor is ready/stopped. Work is local on
`codex/fd2-t01-integration`, with no push or source integration. Code commit
`c58f7b5` is local. Live T07 is complete in Done, with verification comment
`#comment-6adffac4ab95c50adf0bc641` posted and visibly read back; T01-T06 remain
Done. Proof: `artifacts/fd2-board-20261026/t07-done.png`. Next T08 resets/arranges
the permanent loadout; T09/T11 own launch/UI wiring. See the
[T07 handoff](handoffs/2026-10-26-2109-codex-fd2-t07-permanent-hare-node-purchases.md).

**October 26 FD2-T06 Hare license verified:** The FD2 purchase charges canonical
10 Hare Data once, saves the fee/license flag together, preserves all other
wallets/research/Genomes/settlement state and grants no node effect. No schema
change beyond existing v6 is needed. Focused license16/16; full clean EditMode
598 passed/zero failed/one parity skip, with all64 profile transaction cases
passing. Distinct-process write/read checks and focused PlayMode6/6 pass,
including real PlayerPrefs ownership reload and an unlicensed fresh journey
advancing its first simulation. Exact profile/recovery values are unchanged;
portable build/self-test pass. T06 code commit `2e6fcd8` remains local; its card
is complete in live Done with comment `#comment-6adff66df4b9147e647dac68` posted
and read back. Proof: `artifacts/fd2-board-20261026/t06-done.png`. T01-T05 remain
Done. No push or active-checkout source integration occurred. Next T07 buys permanent nodes; visible
Gene Lab wiring remains T11. See the
[T06 handoff](handoffs/2026-10-26-2050-codex-fd2-t06-hare-genome-path-license.md).

**October 26 FD2-T05 verified and Trello closed:** Local code commit
`e32367b5cd43275606a6a6edd629404fb1c2f131` adds schema-v6 conversion research:
10/20/30 RD purchases, cap three, atomic level/deduction and expected-level/profile
request identity. Fresh settlement uses owned research; pending/prior receipts
stay frozen. The isolated branch `codex/fd2-t01-integration` is clean at docs
closeout `2f77e4c0ff80c15eefb39b91164aed547dba50c9`. Focused research25/25,
T04 regression23/23, full clean EditMode582 passed/zero failed/one parity skip,
distinct-process write/read and focused PlayMode4/4 pass. Profile/recovery values
are unchanged; portable build/self-test pass. Live T05 is complete in Done with
verification comment posted/read back, joining T01-T04. Proof in the isolated
checkout is `artifacts/fd2-board-20261026/t05-done.png`. No push or active-checkout
source integration occurred. Next: T06 Hare path license; T10/T11 still wire player
results and Gene Lab controls. See the [T05 handoff](handoffs/2026-10-26-1351-codex-fd2-t05-conversion-research-purchases.md).

**October 26 FD2-T04 verified and T01-T04 board closeout:** Local code commit
`ec30f70005b571181ea6badec2454ad39033d533` adds schema-v5 pending successes,
lifetime receipt history, atomic deposit/receipt updates and failure-safe retry.
It is in the isolated `codex/fd2-t01-integration` checkout; docs closeout commit
is `7e6d659ba7d4b6be72b7e47435b4cf41c97fb7d8`. Full clean EditMode: 557 passed,
zero failed, one parity skip, including 23 T04 cases. Two distinct Editor-process
write/read checks pass; focused PlayMode 3/3 passes including real PlayerPrefs
scene reload/no repayment and both migrations. Profile/recovery values match
the pre-validation capture. Portable Release build/self-test pass. Bevin
instructed that Trello be updated as tickets finish: T01, T03 and T04 were marked
complete and moved to Done with comments posted and read back; T02 remains Done.
Proof in the isolated checkout is `artifacts/fd2-board-20261026/fd2-t01-t04-done.png`.
No push or active-checkout source integration occurred; this checkout receives
documentation only. Next: T05 conversion research purchases; T10 still wires
player results. See the [T04 handoff](handoffs/2026-10-26-1254-codex-fd2-t04-exactly-once-settlement-persistence.md).

**October 26 FD2-T03 calculator verified locally:** Continued from the T01
checkpoint in the isolated checkout. Commit
`72a95432c5d3f4a5d55224f2c8552126ffb7dceb` on `codex/fd2-t01-integration`
adds the immutable success receipt/calculator, approved two-stage conversion,
fixed success RD and checked before/after balances without wallet mutation.
Focused Unity EditMode passes 37/37; full clean EditMode passes 534, fails zero
and skips one retained-manifest parity fixture. Fixed the existing profile test
fixture's missing input isolation and verified profile/recovery values unchanged
across rerun. Portable Release build/self-test pass. The isolated worktree is
clean; this active checkout receives documentation only. No push or live Trello
update occurred. T04 exactly-once persistence/retry and T10 results UI remain.
See the [T03 handoff](handoffs/2026-10-26-1229-codex-fd2-t03-deterministic-success-settlement.md).

**October 24 approved T01 implementation checkpoint:** Bevin approved the five
supported effects and display names. The isolated branch
`codex/fd2-t01-integration` now contains local commit
`03cd29e0725196d8d3c6f9ee31aafc99cfbbb009`, based on incoming `7ac2a54`, at
`C:\Users\bevin\.codex\worktrees\fd2-t01-integration\GameDev`. The executable
FD2 definition authors the existing assets without changing their GUIDs; its
adapter supports owned prerequisites within eight active points. Schema v4
reconciles both v3 profile shapes and preserves incoming allocations/history.
Full clean EditMode: 497 passed, zero failed, one parity-manifest skip. Full
PlayMode: 51 passed, seven failed, six graphics skips; untouched incoming base
reproduces all seven failure names/messages exactly, and both added migration
tests pass. Focused definition 5/5 and profile 33/33 pass; portable Release build
and self-test pass. Original active source is unchanged. T01 is technically
ready for scoped closeout; last verified Trello remains In Progress. No push or
Trello update occurred in this implementation turn. Later FD2 transactions and
UI remain separate tickets. See the [implementation handoff](handoffs/2026-10-24-2158-codex-fd2-t01-approved-definition-and-profile-integration.md).
The proposal/reconciliation and T02 checkpoints below are historical.

**October 24 T01 reconciliation proposal ready:** The [T01 review](FIELD_DATA_PERMANENT_PROGRESSION_T01_REVIEW.md)
and [tuning sheet v0.2](FIELD_DATA_PERMANENT_PROGRESSION_TUNING_SHEET.md) propose
five supported modifiers and truthful display names while preserving all FD2
IDs, prerequisites, prices, permanent ownership and no-refund reset. Incoming
`7ac2a54` portable Release compiles; an isolated review passes 36 assertions,
including all effect applications and eight exact rounding examples. The
incoming active-prerequisite rule leaves no legal eight-point loadout using
Warren Network; FD2 owned prerequisites need a launch/purchase adaptation.
Both branches label different profile shapes v3; a compatible next migration
is needed before integration. Isolated merge previews identify one conflict
each in both profile classes and GenomeContractTests, preserving source hashes.
T01 stays In Progress pending effect/name approval and the validated integrated
definition. No runtime asset, Unity scene, save, checkout/index merge, commit,
push, or Trello mutation occurred. See the [handoff](handoffs/2026-10-24-2101-codex-fd2-t01-content-and-integration-review.md).

**October 24 approved T02 board closeout and T01 refresh:** Bevin approved T02
closeout. Its live Trello card is now complete in Done with the verification
comment read back; proof is `artifacts/fd2-t02-20261024/trello-t02-done.png`.
The 26 focused passes apply to the local BevBranch implementation, which remains
uncommitted/unpushed. T01 is still incomplete in In Progress. A fresh fetch finds
Salty's Rabbit Genome work on origin/ProjectMain at `7ac2a54` (including
`be98b03`); the mechanics dependency is now available remotely. Reconcile its
different node IDs, prices and reset/ownership behavior with the agreed FD2
tuning sheet, map supported effects and review against SG-005 before T01 closes.
The incoming branch also edits both profile classes and uses schema v3; merge
and save-format reconciliation are pending. No incoming code was merged or
validated here. Earlier statements that the mechanics push was still pending
describe their dated checkpoints.

**October 24 FD2-T02 verified:** Unity startup is resolved. The current local
profile migration compiled and passed 17/17 GenomeContractTests EditMode and
9/9 MainMenuPlayModeTests, zero failures/skips. Both migration/roundtrip
PlayMode tests executed. Console compilation is healthy; the retained error is
expected by the missing-profile recovery test. Editor is ready with Play Mode
stopped. T02 is technically ready for review/board closeout; no board update,
commit or push occurred. No full clean suite or player build is claimed. See
the [verified handoff](handoffs/2026-10-24-2040-codex-fd2-t02-verified-wallet-migration.md).
The blocked checkpoint immediately below is historical and superseded.

**October 24 FD2-T02 continuation:** Resumed the local wallet migration. Recovery
JSON is flushed before the v3 replacement, Currency now projects ResearchData,
and six EditMode test cases cover migration replay, recovery preservation,
snapshot copies and invalid wallets. A linked-source .NET check passes 30 model
assertions. Fresh Unity compilation/tests remain pending: the responding project
Editor is at "Recovering Scene Backups" and Pipeline is unreachable. The user
was asked to finish startup or save and close. T02 remains open; live Trello
still lists it in Upcoming Work. No board change, commit or push was made.
See the [continuation handoff](handoffs/2026-10-24-2025-codex-fd2-t02-migration-recovery-and-validation.md).

**October 21 FD2 planning and Trello publication:** Bevin approved the permanent
progression direction after accepting FD1. The FD2 process design, solution
design and 12-card actionable ticket breakdown are documented in the three
linked FD2 files. All 12 cards were created in the board's **🎯 Upcoming Work**
list, with Bevin (Nick Nevins) assigned and recorded as owner and reviewer; no
sprint placement was made. The initial numeric settings are in the [FD2 tuning sheet](FIELD_DATA_PERMANENT_PROGRESSION_TUNING_SHEET.md). FD2-T01 is now in Trello In Progress. Hare effects remain placeholders pending Salty's core-mechanics push, so the ticket stays open. This is design and ticket planning only; no FD2 implementation or runtime verification is claimed. The document package is published to both branches; latest shared commit before this T01 update was `9f9ed11` (2026-10-21). See the [Trello ticket index](FIELD_DATA_PERMANENT_PROGRESSION_TICKET_DRAFT.md#published-trello-cards-2026-10-21).

**October 21 publication and closeout:** All FD-T01–FD-T16 are verified in
Trello Done, with complete flags and all 79 current checklist resolutions.
Bevin accepted the initial five-node slice and confirmed APS fixed. Accepted
implementation `e6cadcd` and integration `dc4f6c5` are published to BevBranch and
ProjectMain, preserving Salty's incoming `6df406c` planning update. T16 explicitly
defers uncollected human timing/matched-path notes; no measurements are invented.
Sprint 5 stays Active and WTR-01 stays In Progress. Unrelated terrain metadata
and Unity recovery files remain local and unchanged. See the
[publication handoff](handoffs/2026-10-21-0153-codex-field-data-publication-and-ticket-closeout.md).
Earlier dated entries below describe their own checkpoints, including then-local work.

**October 22 FD2-T02 local implementation:** Began the profile wallet migration
on `BevBranch`. The local source now defines profile-store v3, maps legacy v2
currency into Research Data, stores species-keyed wallet balances, and keeps
the v2 JSON under the recovery key before migration when that key is free.
Existing `Currency` callers remain compatible. Two focused PlayMode migration
and roundtrip tests were added. The live test command returned zero discovered
tests; the existing Unity test assemblies predate these edits (last compiled
October 20), so compile and test verification remain pending. No commit or push
was made; do not mark T02 done until the tests compile and pass. Existing
unrelated terrain `.meta` edits and `_Recovery` files remain untouched.

**October21 first-slice acceptance:** Bevin explicitly accepts the initial
five-node Field Data implementation as a good starting point and confirms APS
fixed after further play. The new screenshot is a valid failed run atseed10100/
tick1168: ADD6/REM7, FPO0/expected0, ReconciledTrue, APS-1.04. This is separate
from the prior completed1800-tick journey. See the [acceptance record](FIELD_DATA_SELF_PLAY_REVIEW.md).
Human acceptance for the initial slice is satisfied; more content/polish and
balance development remain future scope. Missing research timing metadata is
not fabricated or used to deny acceptance. This turn changes documentation
only; no tests, tuning, live Trello changes, commit or push were performed.

**October 20 human playthrough and APS fix:** Bevin completed the full route,
reported that it felt good and described near population misses for both Foxes
and Hares. The captured seed10100/tick1800 run exposed missing paid-intervention
accounting:8 Hares added/9 removed were omitted, making expected144 disagree
with actual143 and invalidating APS. Run counters, live/snapshot metric factories,
UI and reports now include separate ADD/REM for all controls; natural births/deaths
stay separate. Corrected captured APS is about1.19; Fox accounting also reconciles.
Unity passes159 focused EditMode and4 Field Data PlayMode tests (2 graphics skips);
portable build/self-test pass. See the [handoff](handoffs/2026-10-20-0007-codex-field-data-human-playthrough-and-aps-accounting-fix.md)
and [self-play record](FIELD_DATA_SELF_PLAY_REVIEW.md). Positive full-route human
feedback is now recorded; missing human timing/matched notes and next-content
decisions are not invented. No ecology tuning, board changes or push occurred.

**October 19 current parity/cohort refresh:** The authored 20-Fox setup and
current rules have a fresh export, eight-fixture direct Unity/headless parity
pass (1/1 test, zero skips), and **600/600 validated journeys** across 200 matched
seeds and three v3 schedules. Survival: Remove Hare 58/200, Add Hare 1/200,
save for ×2 52/200. The first payout affords a 10-FD action in 180/200 seeds,
but all removal survivors and 51/52 saving survivors finish with one Hare.
No ecology values or prices changed; T13 human acceptance and T16 recorded
self-play/accept-revise remain open. See the [refresh handoff](handoffs/2026-10-19-1601-codex-field-data-current-parity-and-matched-cohort-refresh.md)
and [report](Research/Experiments/FD-Current-Loop-Refresh-20261019/REPORT.md).
October 17 frozen artifacts remain historical. No Trello changes, commit or
push were made for this refresh.

**October 19 round-7 reward exhaustion:** Taking all six free mutations in the
first simulation exhausted the ordinary reward pool because every skill was
treated as single-use. Round 7 then displayed disabled legacy Attack/Data-cost
cards. A regression reproduced the empty pool exactly at round 7. Field Data
now reoffers the six Hare skills up to their existing level caps (10), retaining
the separate shop cap and legacy authored-upgrade rules. Missing choices are
hidden and an empty pool explains Skip. Focused EditMode passes 91 tests with
one dedicated parity skip; focused PlayMode passes 4 with two separate graphics
skips. Clean rendered checks pass 6/6 at 1280x720 and the round-7 ownership
fixture 1/1 at 1920x1080; captures were inspected. Evidence is recorded in the
[round-7 handoff](handoffs/2026-10-19-fd-round7-reward-exhaustion.md).

**October 18 explicit node entry:** Bevin keeps node-end Field Data settlement
but removes its population controls. Settlement now returns to the map before
explicit shop entry, and leaving each shop returns to the map before the next
simulation can start. Final success offers the completed map and a separate
new-journey action. Ordinary rounds 1–5 retain controls. Focused population
tests pass 9/9; Field Data PlayMode passes 4/4 with two separate graphics checks.
Final graphics pass 6/6 at 1280x720 and the authored route 1/1 at 1920x1080;
captures were reviewed. See the
[node-navigation handoff](handoffs/2026-10-18-fd-node-map-navigation.md).
Earlier pre-shop round-6 control rules are superseded. Human acceptance remains open.

**October 18 approved playtest default:** Bevin approved switching Forest Edge
from 35 to **20 starting Foxes**; 55 Hares and 400 Plants remain. The authored
asset now resolves to the tested 20-Fox candidate. This is the next playtest
setting, with later population crashes still open. See the
[change record](handoffs/2026-10-18-fd-20-fox-playtest-default.md).

**October 18 opening-population diagnostic:** Bevin reports Hares collapse too
quickly. An 80-run current-source screen (20 matched seeds x four settings)
finds 3.8 Hares at the first baseline payout on average; 0/20 can afford the
10-FD intervention. Starting 20 Foxes instead of 35 gives 16.55 Hares and
18/20 affordability, but only 5/20 no-action journeys survive to tick 1800
(four with one Hare). Ten Foxes gives a gentler opening followed by worse
full-route survival; raising Hare starting energy shifts losses to starvation.
Forty actual FD replays match the baseline/candidate screen, and two Unity
session cases match the standalone boundaries. Recommend temporary 20-Fox
playtests on matched seeds 10100/10102, not a production balance approval.
Production assets were unchanged during that diagnostic; the approval above
subsequently adopts its 20-Fox candidate. The authored map is nonwrapping; freeze
candidate values into resolved data because `journey-replay` ignores research
argument overrides. See the [diagnostic report](Research/Experiments/FD-Opening-Hare-Collapse-20261018/REPORT.md).

**October 18 validated Field Data correction:** The selected test route now
shows five visible node cards before explicit Start, with the correct route
heading and accessible Map buttons in round/shop and results panels. Map
round trips preserve the session, tick and wallet; Reproductive Drive is now
bounded at reproduction chance 1.0 instead of throwing. Shop-only income and
the six-skill Hare pool have regression coverage. Clean suites passed 399
EditMode / 49 PlayMode (1 dedicated parity and 6 graphics skips); final graphics
passed 6/6 at 1280x720 and the authored route 1/1 at 1920x1080. Screenshots were
reviewed. The final graphics runs include the panel Map buttons added after
the broad suites. Portable Release build and self-test pass. Human visual
acceptance and team self-play remain open; no fresh parity/panel, player build,
Trello change or push is claimed. See the [validation handoff](handoffs/2026-10-18-fd-route-and-reward-validation.md).

**October 17 Field Data UX/economy correction:** Bevin reported that choosing
the test route immediately launched simulation, the active Field Data journey
had no usable route map at round/shop decisions, and a generic attack upgrade
appeared for Hares while income modifiers appeared outside shops. Source review
confirmed: route selection configured and started in one action; the map toggle
was hidden during Field Data decisions and the Field Data path had no mapped
JourneyMap nodes; the ordinary Hare fallback included generic attack; the
starter definition allowed +1/Hare at round rewards and ×1.5 at node rewards.
Local changes now show a five-node route preview before an explicit Start,
expose an in-journey map summary/toggle through round and shop decisions, limit
Hare free rewards to the six approved Hare skills, and restrict all income
modifiers to shop offers in both authoring validation and reward generation.
Unity compile/tests and human visual review have **not** been run for this
follow-up. Earlier T08/T15 evidence used the superseded income-source rule and
does not validate this correction. See the [correction handoff](handoffs/2026-10-17-fd-route-map-and-shop-income-correction.md).

**October 17, 02:02 delivery checkpoint:** T11/T12 are verified complete;
T13's authored Desktop route, all 18 rounds/two shops, natural failure,
abandonment and reset have runtime/rendered evidence, while independent human
visual acceptance remains open. T14 now passes five authored action fixtures
against Unity at every decision/board boundary; an ordinary reward-pool mismatch
was corrected with a shared resolver and explicit portable pool. T15 completed
and validated all 600 current-source runs (200 matched seeds × three approved
policies), preserving earlier frozen artifacts. Full checks: EditMode 397
passed/1 dedicated-fixture skip; PlayMode 49 passed/6 graphics skips; dedicated
parity 1/1; final 1280×720 graphics 2/2, earlier 1920×1080 graphics 2/2.
T16 still needs genuine team self-play, decision-time observations and
accept/revise. No tuning/content decision or implementation push is claimed.
See the [current handoff](handoffs/2026-10-17-0202-codex-field-data-rendering-parity-and-cohort.md)
and [play/review instructions](FIELD_DATA_SELF_PLAY_REVIEW.md).

**October 17 verified T04–T06 closeout:** T04, T05 and T06 are Done on Trello,
5/5 criteria each. Clean Unity verification passed 49/49 FieldData EditMode
tests at `artifacts/unity-tests-20261017-010128` and 5/5 focused PlayMode tests
at `artifacts/unity-tests-20261017-010227`. This adds repeat Continue/results,
shop re-entry, legacy entry and seeded species/Genome unlock preservation to
the current route/lifecycle evidence. T06 fixes prevent Plant additions from
overwriting existing resources and count both population layers at capacity.
T07 also passed its strengthened signed-effect checks in the full clean
EditMode suite (390/390 at `artifacts/unity-tests-20261017-010934`) and is Done,
5/5 on Trello. T08 is also Done, 5/5, after the expanded clean session fixture
passed 13/13 at `artifacts/unity-tests-20261017-011229`. T09 is Done after its
16/16 session-fixture pass at `artifacts/unity-tests-20261017-011558`; T10 is
Done after the full clean EditMode suite passed 395/395 at
`artifacts/unity-tests-20261017-011725`. Each has 5/5 checked on Trello with
read-back completion evidence. T11 is active: review reproduced shop actions
being accepted before the free node reward (`artifacts/unity-tests-20261017-011920`).
The domain/UI gate is corrected; focused PlayMode passed 4/4 at
`artifacts/unity-tests-20261017-013124`. Two older second-shop fixtures needed
to resolve the node choice before their actions and have been updated; the
focused EditMode rerun passed 56/56 at `artifacts/unity-tests-20261017-013258`.
T11/T12 rendering review then exposed oversized paper-button rows, a zero-based
shop heading, a premature discard message and missing offer eligibility/effect
details. These are corrected locally. Clean 1280×720 graphics passed 2/2 at
`artifacts/visual-evidence-20261017-014514`: the actual authored 18-round route
and a separately labeled preloaded-ownership fixture for capped offers, free
reroll exhaustion and paid random/chosen reorders. A fresh 1920×1080 run and
integrated suites are in progress; human acceptance remains open. T14 reports
now also retain board digests before/after actions and actual charged FD;
current-source cross-runtime verification is still pending. See the
[T04–T06 closeout handoff](handoffs/2026-10-17-0106-codex-fd-t04-t06-verified-closeout.md)
and [T07 completion](FIELD_DATA_TICKET_DRAFT.md#fd-t07--apply-the-six-signed-hare-shop-effect-families).

**Earlier October 17 Field Data continuation:** `CellSim.Batch` Release build and full
self-test passed earlier in this work block. The FieldData-named EditMode suite
passed 46/46 at `artifacts/unity-tests-20261017-002151`. A clean Desktop
PlayMode run passed 4/4 at
`artifacts/unity-tests-20261017-004043`, covering authored-settings route
success, natural failure, manual abandonment through End confirmation, and
the route-entry check. Success and failure/abandonment each preserve seeded
Desktop profile currency and feature unlocks across a fresh journey. The route
test now forces the authored 0.2-second cadence; seed 10100 completed all 18
rounds and two shops. The previous seed 10125 pass inherited a 0.1-second
editor setting and was not evidence for the authored asset.

T14's authored Forest Edge snapshot and 200-seed Unity reference export now
exist under `artifacts/field-data-t14/`. A no-purchase headless journey on seed
10100 completed 18 population awards and three node rewards, then discarded
52 leftover FD on success. Its final Hare/Fox/Plant populations (1/0/717)
match the Unity reference. Full Unity/headless receipt/order parity for
scheduled purchases and reorders remains open.

T15's v2 policies ran all 600 matched replays for seeds 10100–10299; all 600
reports completed and each seed appears once in all three arms. The baseline
Unity reference had 45/200 Hare survivors; removal had 50/200, additive Hare
intervention 0/200, and save-for-second-shop ×2 45/200. The detailed aggregate
and every replay are under `artifacts/field-data-t15/run-20261017/`.
Automated replay runtime totals 872,424.8 ms across the 600 runs (1,454 ms
mean). Human setup/analysis time is not captured. Treat the survival counts as
descriptive evidence only; no numeric acceptance gate was approved.

T13's repeat/re-entry lifecycle checks now pass; fresh graphics captures
and independent human review remain open. T14 needs action/receipt/order parity and report
compatibility checks. T15 needs a separately recorded active team setup/analysis
time. T16 is not complete: internal self-play, real decision-time notes, and
the team's accept/revise judgment require team participation. See the [Oct 17
ticket checkpoint](FIELD_DATA_TICKET_DRAFT.md#current-execution-checkpoint---2026-10-17)
and [T14/T15 SDD evidence](FIELD_DATA_SOLUTION_DESIGN.md#20-fd-t14-and-t15-authored-replay-evidence---2026-10-17).

**Historical October 16 FD-T04/T05 checkpoint (superseded above):** Added `FieldDataJourneySession` to
coordinate one ready 1,800-tick run with 100-tick rounds, decision pauses and
shops at ticks 600/1,200. It retains the same runner/run and progression,
rejects runner replacement, treats early completion as failure, and completes
only at tick 1,800 with the selected species alive. `SpeciesSimulationPreview`
now has an explicit configuration seam that prepares 18 rounds, binds the
session to the preview's manager, bypasses legacy reward handlers for that
mode, and bridges decision/shop/result state; the preview remains the only
simulation clock forwarder. The focused `FieldData` EditMode suite passed
24/24 in the connected Editor (`artifacts/unity-tests-20261016-204629`). A
session test exposed and fixed disposal leaving the state at Failed; the route
fixture now uses a stable Hare rule to test flow only, not ecology balance.
Rendered preview/runtime behavior remains unverified. FD-T04 remains In
Progress and all five criteria remain open. FD-T05 now has a journey-local
wallet ledger with idempotent population and node-clear receipts, failure
forfeiture, successful-result remainder discard, and focused Unity coverage.
After the verified T05 move, the board is at 16 Done,
2 In Progress, 11 Current Work, and 65 acceptance items open (whole-board counts
derived from the verified post-T04 snapshot and T05 move). See the [T05 SDD checkpoint](FIELD_DATA_SOLUTION_DESIGN.md#18-fd-t05-wallet-settlement---2026-10-16),
[T04 SDD checkpoint](FIELD_DATA_SOLUTION_DESIGN.md#17-fd-t04-retained-journey-session---2026-10-16),
[T03 checkpoint](FIELD_DATA_SOLUTION_DESIGN.md#16-fd-t03-ordered-income-checkpoint---2026-10-16),
[T02 checkpoint](FIELD_DATA_SOLUTION_DESIGN.md#15-fd-t02-implementation-checkpoint---2026-10-16)
and [ticket status](FIELD_DATA_TICKET_DRAFT.md#current-execution-checkpoint---2026-10-16).

**October 16 FD-T01 accepted and complete:** Source review is prepared at `acc371a`. Bevin
confirmed that the Desktop Simulation icon should retain the larger map as
entry, with the five-node test selected from it. The
[entry/compatibility review](FIELD_DATA_ENTRY_REVIEW.md) records the route,
development inputs and concrete composition proposal. PDD/SDD launch sections
are updated. Bevin accepted the plan and waived further review, including the
additional Salty review. FD-T01 is Done with all four criteria closed under
that explicit exception. See the
[FD-T01 handoff](handoffs/2026-10-16-1335-codex-fd-t01-route-entry-and-compatibility-review.md).

**October 16 Sprint 5 kickoff:** S5 is Active with the approved four-week work
window **October 19-November 15, 2026**. All 16 Field Data tickets are in
Current Work with 79 unchecked acceptance items; owners/reviewers stay
unassigned for team allocation as Bevin requested. Estimates total 57-95h;
the 80h combined planning floor protects 16h reserve and leaves 64h feature
capacity, so the upper estimate exceeds that allowance by 31h. See the
[S5 control](Sprints/S5-control-record.md),
[published ticket index](FIELD_DATA_TICKET_DRAFT.md#published-trello-index---2026-10-16)
and [kickoff handoff](handoffs/2026-10-16-0125-codex-s5-kickoff-trello-publication-and-projectmain-sync.md).
No gameplay implementation or new test/seed run was performed at kickoff. At
that time, next was team allocation and FD-T01's route entry/compatibility
review; both are now resolved, and FD-T02 is complete.

### October16 incoming planning/capacity checkpoint (historical)

**October 16 S5 product goalpost:** Josh wants a much more solid Forest Edge
simulation and a playable demonstration that the loop could become a fun,
engaging game. The [playability goalpost](Sprints/S5-playability-goalpost-2026-10-16.md)
sets a two-simulation, one-shop first review gate and a meeting agenda. Keep
all 16 Field Data cards visible for tonight's priority meeting with Sim;
the final committed subset, owners and changed acceptance checks await that
meeting.

**October 16 Sprint 5 kickoff and planning update:** S5 is Active with the
approved four-week window **October 19-November 15, 2026**. At kickoff all 16
Field Data tickets were in Current Work with 79 unchecked acceptance items.
The later board check has FD-T01–03 Done, FD-T04 In Progress and FD-T05–16 in
Current Work. Owners/reviewers remain unassigned for team allocation as Bevin
requested. Estimates total 57–95h; Josh removed the 16h protected review
reserve and will handle review in separate dedicated time outside sprint
effort. Against the 80h conservative planning floor, the upper Field Data
estimate is 15h over before unestimated ecology/playability work. See the
[S5 control](Sprints/S5-control-record.md),
[published ticket index](FIELD_DATA_TICKET_DRAFT.md#published-trello-index---2026-10-16)
and [kickoff handoff](handoffs/2026-10-16-0125-codex-s5-kickoff-trello-publication-and-projectmain-sync.md).
No gameplay implementation or new test/seed run was performed at kickoff.
Next: agree priorities, actual remaining estimates, owners and review gate
with Sim; reconcile FD-T02/03's completion comments and unchecked due flags.

### Earlier checkpoints (historical)

**October 16 Field Data ticket planning:** The
[local ticket draft](FIELD_DATA_TICKET_DRAFT.md) breaks the PDD/SDD into 16
proposed tickets with dependencies, acceptance checks and rough estimates.
Bevin explicitly requested unassigned owners/reviewers for team review. The
57–95 active-hour feature backlog is not a sprint commitment; FD-T01–FD-T05
are a proposed first foundation package. No Trello cards, gameplay changes or
new test runs were made during this planning pass.
See the [ticket-planning handoff](handoffs/2026-10-16-0029-codex-field-data-actionable-ticket-draft.md)
for coverage and draft-validation evidence.

**October 16 publication, merge and plan alignment:** Bevin's Field Data work
was published as `d178a62`; Salty's `ProjectMain` through `341cc702` is merged
at `0928359`. The [combined planning draft](FIELD_DATA_AND_SIMULATION_ALIGNMENT.md)
maps overlap, implemented foundations, retained simulation work and decisions
needed before tickets. S4 is Closed; S5 remains Proposed. The reviewed Field
Data rules remain the first-test baseline; implementation and permanent economy
are not approved merely by integrating these documents.
Merged Clean checks: 340 EditMode passed; 45 PlayMode passed, 0 failed and four
graphics-dependent tests skipped. Fresh visual/build acceptance and the 200-seed
economy panel remain unperformed. See the
[integration handoff](handoffs/2026-10-16-0016-codex-field-data-publication-salty-merge-and-planning-alignment.md).

**October 16 PDD readability revision:** Section 4 of the
[Process Design](FIELD_DATA_PROCESS_DESIGN.md#4-end-to-end-player-process) now
uses a rendered journey/round overview and a separate round-6 table. The product
rules and implementation scope are unchanged; the diagram's editable SVG lives
alongside its PNG in `docs/diagrams/`.

**October 15 Field Data implementation design drafts:**
[Process Design, PDD-FD-001](FIELD_DATA_PROCESS_DESIGN.md) and
[Solution Design, SDD-FD-001](FIELD_DATA_SOLUTION_DESIGN.md) are draft 0.1 for
review. They map the approved baseline to the player process, seven delivery
stages, existing simulation/wallet/Noesis seams and future verification. They
propose one retained progression wallet, one journey session/settlement path,
atomic population changes, signed shop overlays and shared headless evidence.
No gameplay code, assets, runtime tests or 200-seed runs were changed/executed.
Implementation owners and scheduling remain unassigned by these drafts.
See the [draft handoff](handoffs/2026-10-15-2352-codex-field-data-implementation-design-drafts.md)
for source review and documentation-validation scope.

**October 15 Field Data design review:** The Step 1–5 design decisions are
consolidated in [Field Data and Journey Loop Design v1.0](FIELD_DATA_JOURNEY_DESIGN.md).
The [design/delivery checklist](FIELD_DATA_DESIGN_PLAN.md) preserves all original
IDs and distinguishes design selection from implementation and test results.
The full five-node route, starter economy, reward schedule, ordering/reordering,
UI requirements, correctness checks and internal self-play protocol are defined.

The final review confirmed 10-FD starting random reorders, chosen at 10× current
rounded random charge, 10% escalation per paid reorder of either kind/reset at
next shop; 200 fixed matched seeds; three free choices or Skip after rounds
1–5 and the first two simulation clears; a provisional 10-FD clear bonus at all
three simulation nodes; protection for the final selected-species individual;
and population controls at nonterminal round-6 results before shops.

Implementation and focused runtime validation have not begun. Internal
comparisons follow that delivery, using a frozen scenario and actual 200-seed
list. Paper budgets are synthetic arithmetic, not ecology evidence. Success
conversion, external-player research and follow-up content selection remain
explicitly deferred. No new sprint, board change or publication follows from
this documentation review. See the [review handoff](handoffs/2026-10-15-2335-codex-field-data-design-baseline-review.md)
for scope and validation.

**October 15 focus change:** Bevin put the initial
[Sprint 5 planning draft](Sprints/S5-planning-draft.md) on hold and intends to
leave sprint planning to Salty. Bevin's immediate focus is the currency/journey
loop and how the simulation plays out, with that work feeding into Salty's
planning. The draft's dates, budget and candidate list are historical discussion
material, not a current commitment. No S5 kickoff or Trello changes occurred.

**October 14 meeting record approved for publication:** The
[Word meeting record](Meeting%20Briefs/TEAM%20MEETING%20%E2%80%94%20OCTOBER%2014,%202026,%20AT%209%20PM.docx)
now contains the nine decisions, clarified journey-loss rule, verified S4
card closeouts and all nine Ready for Art requests with Trello links.
All four pages were rendered and visually checked. Bevin reviewed the
document, accepted it and authorized the documentation push. See the
[document handoff](handoffs/2026-10-14-2337-codex-october-14-meeting-brief-review.md).

**October 14 S4 card closeout:** At Bevin's request, S4-01, S4-02, S4-03
and S4-07 now have refreshed Trello descriptions and are complete in Done.
The board was reloaded and each change verified: Current Work has only
P1-032; Done has 12 cards. At that October 14 checkpoint, the sprint control remained Active and no
next sprint was opened; the October 15 closeout below supersedes that status. Historical plans and evidence are retained; ongoing
crowding, balance and player-review work is recorded as follow-up rather
than reported as newly validated. See the
[four-card closeout](handoffs/2026-10-14-2254-codex-s4-four-card-closeout.md).

**S4 closed; next work block proposed, 2026-10-15.** Josh approved the
[S4 closeout](Sprints/S4-control-record.md#closeout--2026-10-15): four selected
cards are Done, P1-032 returned to Backlog, and S4-04/05 remain Backlog. The
[S5 simulation and upgrades proposal](Sprints/S5-proposal-2026-10-15.md)
puts Forest Edge behavior, authored upgrade choices, matched evidence, and
player review first, but S5 has no kickoff commitment yet. The canonical
Desktop profile save/launch foundation is published; Sim's launch review and
real application exit/relaunch check remain open on P1-032. See the
[S4 closeout handoff](handoffs/2026-10-15-2007-codex-s4-closeout-and-s5-proposed-handoff.md)
and the
[profile acceptance handoff](handoffs/2026-10-15-1835-codex-profile-feature-unlock-storage-and-desktop-launch-acceptance.md).

**Journey art alignment, 2026-10-15.** The published map is at `4d3d7734`.
Its scrolling UI now targets Chrono's 5,760 × 1,080 panorama brief using
normalized node positions; the current 4,700 × 1,080 artwork is a placeholder.
The follow-up 1920 × 1080 visual check passed and captures are retained. See
the [alignment handoff](handoffs/2026-10-15-1903-codex-journey-panorama-art-alignment-and-s5-proposal.md)
and [art coordinates](JOURNEY_PROTOTYPE.md#replacing-the-placeholder-artwork).

**October 14 team meeting: journey rules for the next playtest.** Bevin will
guide phase-clear criteria as balance evidence develops. Failure ends the
whole journey without cash-out: run-earned currency and temporary upgrades
are lost, while permanent Desktop upgrades and unlocked upgrade or feature
availability remain. Field Data carries through a journey; other currency
types can reach the Desktop after success. Population controls and
a random-event gamble are intended Field Data purchases. At that meeting, prices, exact
wallets and the first slice were open. The October 15 Field Data baseline
now defines the first slice; permanent wallet content remains open. See the
[meeting decisions](Meeting%20Briefs/2026-10-14-Simulation-Focus-and-Journey-Economy-Notes.md#meeting-decisions)
and [clarification handoff](handoffs/2026-10-15-1232-codex-journey-failure-persistence-clarification.md).

**Journey Field Data reset: 2026-10-15, ready for review.** Starting a new
Forest Edge journey now clears leftover Field Data along with its temporary
upgrades. Standalone expedition currency carryover remains unchanged. Two
focused Clean PlayMode checks passed. The separate permanent-currency deposit
and permanent unlock rules remain design work. See the
[boundary handoff](handoffs/2026-10-15-1250-codex-journey-field-data-reset-at-run-end.md).

**Scrollable Forest Edge journey: 2026-10-14, ready for visual review.**
The map now follows Josh's horizontal layout study: three playable openings,
29 circular markers, 35 connections, and 13 visual stages over a quiet forest
panorama. Mouse-wheel travel and hover cards are implemented. The two-cycle
gameplay remains the playable scope; later markers await content. See the
[journey handoff](handoffs/2026-10-14-2105-codex-scrollable-forest-journey-map.md)
for verification and Chrono's art replacement coordinates.

**October 14 ProjectMain and BevBranch integration:** Sim's terrain, artwork,
simulation, and tooling commits are merged with the local S4/journey checkpoint
and profile persistence. The merged Unity suites and Release tooling builds pass.
See the [integration handoff](handoffs/2026-10-14-1707-codex-projectmain-bevbranch-integration.md)
for validation, the standalone runner check limitation, and the visual review
still useful before balance work.

**Profile persistence foundation: 2026-10-14, ready for review.**
Multiple profiles now keep separate currency, species/Genome unlock IDs,
Genome node state, cumulative simulation ticks, and active playtime. The
canonical Desktop launch uses the selected profile's frozen Genome state;
focused EditMode and PlayMode checks passed. Run reward settlement and data
categories remain design decisions. See the
[profile persistence handoff](handoffs/2026-10-14-1424-codex-profile-persistence-foundation.md).

**October 14 terrain selection correction accepted:** The board and paint preview
now sample four cells around shared vertices instead of drawing center-excluding
neighbor masks on each terrain cell. Existing artwork now forms one straight
transition and preserves islands/holes. 18 focused EditMode checks, 27 presentation
PlayMode tests and one Desktop visual capture pass; temporarily restoring the
old drawing path fails the new rendered regression. Bevin tested the correction
himself, confirmed the improvement and authorized publication. See the
[shared-vertex handoff](handoffs/2026-10-14-codex-shared-vertex-terrain-fix.md).

**October 14 Forest Edge flourish replacement accepted:** Replaced the eight old
images with Bevin's ten supplied mushroom/shrub/flower PNGs, preserving existing
asset identities. Noesis labels plus explicit uncompressed V2 atlas defaults
preserve transparent edges; exact sprite-name matching keeps the three mushroom
colors distinct. 12 distinct focused EditMode checks, 26 presentation PlayMode
tests and one 1920x1080 Desktop visual capture pass. GPU alpha and source hashes
verify. Bevin confirmed the rendered result and authorized publication. See
the [replacement handoff](handoffs/2026-10-14-codex-forest-edge-flourish-replacement.md).

**October 13 intermittent tile seams accepted:** The shared board now
aligns tile boundaries in screen pixels and refreshes after pan/layout scale
changes. 26 presentation PlayMode tests pass, including a rendered regression
with a failing negative control; actual art inspected at three viewport sizes.
No terrain art/atlas/scenario changes. Bevin confirmed the visual result and
authorized publication with the Chrono artwork on October 13; see the
[seam handoff](handoffs/2026-10-13-codex-terrain-seam-rendering-fix.md).

**October 13 Chrono Forest Edge flourishes accepted:** Added the eight supplied
mushroom/shrub/flower sprites to the existing terrain atlas and shared board.
They decorate full grass beneath creatures without changing simulation state.
Corrected their Noesis import labels so grass shows through transparent areas;
desktop Play Mode visual check and focused tests pass. The original fox sprite's
missing Noesis label is corrected too, with fresh Desktop Play Mode visual and
GPU transparent-corner verification. Prototype final visual check/full suite/
player build remain unverified. Bevin visually accepted the artwork and fixes
and authorized publication on October 13; see the
[flourish handoff](handoffs/2026-10-13-codex-forest-edge-chrono-flourishes.md).

**October 12 tooling publication and Salty integration:** The Workbench/export
work and compact confirmation review were pushed to BevBranch in `07d78de9`.
Salty's checkpoint `876f3cf7` is integrated with Hare spatial telemetry and
guaranteed first-offer access to all three paths; BevBranch's existing avoidance
fix and portable records are retained. Merged checks pass: 160 focused EditMode,
25 presentation PlayMode, 31 worksheet tests, runner self-test, 16 GUI groups,
both Release builds and four-seed Unity/standalone parity including spatial
records. The PlayMode check first caught four obsolete card-order
assumptions; their coverage now selects the intended skill and checks the new
authored opening rewards. The saved 8,000-run review remains historical to its
frozen runner. See the [integration handoff](handoffs/2026-10-12-1622-codex-bevbranch-tooling-publication-and-salty-offer-spatial-integration.md)
and [preserved results](Research/Results/2026-10-12-Forest-Edge-Confirmation/README.md).
Earlier local/uncommitted statements below describe their original checkpoints.

**October 12 fresh-seed confirmation reviewed locally:** Bevin completed the
five presets and supplied cumulative exports ending at 8,000 runs/80 conditions.
The best observed condition is preset 04 (64x48 H4/F16), Gardeners + Restore:
10/100 ecology goals; no reliable baseline is established. Overall 261 goals,
2,167 Hare survivors and 1,438 all-three survivors. Matched choices show useful
effects; 60 no-purchase goals include 15 fully unassisted successes. All five
completed raw hashes, 40 frozen files and 7,720 workbook metrics verify.
Analysis: `artifacts/confirmation-review-20261012/ai-analysis-v1.md`.
No source/runtime changes or additional workloads; recommendations need review.
See the [confirmation results handoff](handoffs/2026-10-12-1609-codex-forest-edge-fresh-seed-confirmation-results-review.md).

**October 12 confirmation presets prepared locally:** The reviewed gestation/map
screen now has five focused fresh-seed presets (10001-10100; 1,600 runs each,
8,000 total) and six original-seed comparison presets (24 runs total). All eleven
are also frozen as saved Workbench runs; application loading/preparation/reload
and pinned-runner preflight pass. All 104 resolved conditions match the discovery
settings, with byte-identical snapshot/runner and original inputs preserved.
No simulations started. The guide is
`artifacts/workbench-presets/Forest Edge confirmation 2026-10-12/README.md`.
No app rebuild or production tuning; local/uncommitted. See the
[preset pack handoff](handoffs/2026-10-12-1447-codex-forest-edge-confirmation-and-replay-preset-pack.md).

**October 10 large Excel export fixed locally:** Bevin's completed Forest Edge
batch has 24,192 runs, 8,064 conditions and 12,768 matched comparisons. Exporting
1,000 review rows exceeded Node's default 4 GB heap during XLSX creation.
The exporter now has a bounded 8 GB child heap allowance, batches layout sizing
and logs table/file preparation stages. The full export succeeds; 1,240,658
saved cells, four native tables, 1,000 evidence links and frozen source/raw hashes
verify. 31 worksheet and 17 native GUI/export checks pass. No simulation reran;
the open Workbench uses the updated scripts without an EXE replacement.
Local/uncommitted; see the
[large export handoff](handoffs/2026-10-10-2209-codex-workbench-large-excel-export-memory-fix.md).

**October 10 statistical worksheets and promising seeds, local:** The exporter
now uses all matching runs for goal/Hare-survival rates with 95% Wilson bounds,
endpoint/birth/purchase/decision-access statistics, and matched Mutation/purchase
controls on a new Choice impact sheet. Review defaults to promising seeds;
Lowest seeds remains available, and Run data still matches the selected rows.
Editable export floors default to 5 Hares / 5 Foxes / 100 Plants, inferred from
Bevin's latest experiment notes rather than approved as production tuning.
31 worksheet checks, 16 GUI groups and 15 setup/run groups (47 fixture runs) pass;
raw-count/Wilson and saved-workbook readbacks reconcile. Re-exporting the existing
1,000-run baseline yields 0 goal successes; it cannot show choice effects because
it has only Skip all / No purchases. Canonical EXE rebuilt with fresh save/close
authorization and reopened; the installed EXE's 17 checks pass, including a real
native-button export. No research
batch started; local/uncommitted. See the
[statistical export handoff](handoffs/2026-10-10-2106-codex-workbench-statistical-results-and-promising-seed-review.md).

**October 10 large saved-plan metadata fix, local:** Bevin's validated 9,216-
condition / 18,432-run setup compiled successfully but Run refused its 5,017,684-
byte plan through the unrelated 4 MB progress-file reader. Condition-sized
plans and completed batch manifests/summaries now have a bounded 64 MB allowance;
status/snapshot/small-manifest limits, input hashes and run budgets remain in
place. The exact saved setup reopens, verifies all eight hashes and passes a
read-only pinned-runner preflight without creating a batch folder. Fifteen GUI
and fifteen setup/run check groups (47 fixture runs) pass, including large-file
regressions and changed-input rejection. Canonical EXE rebuilt/reopened with
Bevin's save-and-close permission. Research batch remains unstarted; source
local/uncommitted. See the [metadata fix handoff](handoffs/2026-10-10-2051-codex-workbench-large-saved-plan-metadata-fix.md).

**October 10 Workbench gestation controls and baseline review, local:** Bevin's
1,000-run Gestation Baseline had 53 Hares-surviving runs at tick 600 (5.3%);
836 runs recorded no Hare births. All conditions used Hare/Fox gestation 48,
so this is a poor baseline, not an isolated test of gestation's effect.
Compare values now offers independent Hare/Fox gestation ticks, including zero
for immediate births, with readable help, frozen values, presets and numeric
Excel exports. Production defaults remain 48. A prepared, unstarted preset
compares Hare 0/12/24/48 with Fox fixed at 48 across the original 1,000 seeds.
All 326 EditMode tests, 21 worksheet checks, 14 GUI checks and 14 setup/run
groups (46 fixture runs) pass. The canonical executable was rebuilt and
reopened after Bevin saved and authorized closing the previous window.
See the [baseline and controls handoff](handoffs/2026-10-10-2035-codex-workbench-gestation-controls-and-baseline-review.md).
This supersedes the earlier proposal to add gestation comparison controls;
the 4,000-run comparison and any production tuning remain open.

**October 10 BevBranch gestation integration:** Our Workbench readability and
comparison-preview fixes were pushed as `a82d1cd4`. Salty's gestation checkpoint
(`65e47386`) is merged as `10c13f4`, preserving both contributors' working-state
entries. Portable snapshots now preserve gestation, and parity checks include
pending-litter state with normalized mate identity. Production Hare/Fox use a
provisional 48-tick delay; earlier frozen batches retain their original model.
See the [integration and testing-impact handoff](handoffs/2026-10-10-2004-codex-bevbranch-salty-gestation-integration-and-testing-impact.md)
for current validation, new-snapshot requirements, and proposed comparison controls.

**S4-03 Trailblazer diagnostic: 2026-10-11, needs design review.** A
five-arm replay of the existing 20 matched seeds added Hare spatial observations
without changing any of the 100 archived run outcomes. Both Trailblazer orders
increase post-choice movement, but population-wide map coverage gains are small
and inconsistent. The current crowding rule only triggers at seven Hare
neighbors; Josh's approved four-neighbor density surcharge is still pending,
so the replay cannot judge intended crowding pressure. The first rotating
three-card offer nominally contained a Trailblazer starter in 12/20 seeds under
the old rotation. On 2026-10-12 Josh approved guaranteed first-offer access to
one starter from each path; the Forest Edge opening reward now uses Movement,
Tough Hide, and Efficient Digestion, and later phase offers keep the selected
starter's partner available. This changes player access, not the earlier
balance evidence. See the
[factual report](Sprints/S4-03-trailblazer-spatial-20261011/report.md) and
[analysis](Sprints/S4-03-trailblazer-spatial-20261011/ai-analysis-v1.md), plus
the [offer-path handoff](handoffs/2026-10-12-0132-codex-s4-hare-path-starter-offer.md).

**S4-03 Hare strategy screen: 2026-10-11, needs in-game review.** The
predeclared 13-arm × 20-seed screen on 400/20/10 is complete. A scheduled
Threat Avoidance bug and a stale combat gate were fixed and the affected arms
replayed on the same seeds; 319 EditMode and 42 PlayMode tests passed. Movement
activity and seed-drop accounting are observable, but patch revisits and
Warren crowding pressure remain insufficient for strategy
acceptance. See the [screen report](Sprints/S4-03-screen-20261011/report.md) and
its [analysis](Sprints/S4-03-screen-20261011/ai-analysis-v1.md). Josh and Sim's
player-flow review and accept/revise decision remain open.

**S4 gestation checkpoint: 2026-10-10, ready for review on codex/s4-gestation-checkpoint-20261010.**
Hare and Fox now use a provisional 48-tick gestation period, with pending litters
preserved through movement and whole-litter births waiting for space. Full
isolated-worktree Unity suites passed (317 EditMode and 42 PlayMode). The S4
record also captures Trailblazer, Warren, and Gardeners identities and
Gardeners' one-unit reserve cost per successful seed drop. Matched-seed
balance and team review remain open. See the
[checkpoint handoff](handoffs/2026-10-10-1853-codex-s4-gestation-checkpoint.md).

**S4-03 exploratory pilot: 2026-10-10; main fixture selected 2026-10-11.** A
five-seed matched control/Gardeners pilot after gestation found poor
choice reach at 400/25/15 and more observable seed renewal at the existing
400/20/10 manual fixture. Both fixtures still had frequent Hare extinction.
Josh selected 400/20/10 for the main screen, with 400/25/15 retained as a
stress comparison. The inputs are frozen in the
[screen plan](Sprints/S4-03-screen-plan-20261011.md). See the
[pilot handoff](handoffs/2026-10-10-1902-codex-s4-03-gestation-baseline-pilot.md).


**October 10 comparison-preview hang investigated locally:** Windows logged an
Application Hang for the canonical Workbench. Bevin reported Hare/Fox vision
1-15, twelve seeds and all strategies/policies; adding Plant energy1,2,3 crosses
the inherited10,000-condition limit. Automated checks reproduce the warning but
did not reproduce the original hang. Preview row updates now batch layout and
avoid the redundant resize callback; invalid previews clear rows and columns,
with explicit requested condition/run counts. Fourteen GUI check groups pass,
including sequential Add/edit/remove recovery at three window widths. See
[handoff](handoffs/2026-10-10-0014-codex-workbench-comparison-preview-hang.md).
Shared in `a82d1cd4`; original hang cause and human retry remain open. Budget
configuration is a proposed follow-up, not an implemented change.

**October 9 dropdown readability fix added locally:** The Compare values picker
and value entry now fill the available column instead of collapsing under the
auto-sized layout. The open list measures every label with the current font and
uses a twelve-entry scrollable height. Read-only setting cells no longer show
misleading dropdown arrows and allow wrapping. Thirteen native GUI checks pass,
including a regression that fails on the previous picker and checks all thirty
labels at default/compact sizes and larger text. Open lists rendered/inspected
at 144 DPI. See [handoff](handoffs/2026-10-09-2356-codex-workbench-readable-dropdowns.md).
Shared in `a82d1cd4`; this UI fix changes no simulation rules or evidence.

**October 9 publication package:** Bevin authorized committing and pushing this
session's D5 inspection setup, live Plant count, Excel exporter and interactive
Workbench to `origin/BevBranch`. The publication commit includes the source,
launchers, focused checks and guides; generated applications, workbooks and run
evidence remain local ignored artifacts. The earlier "local/uncommitted" entries
below describe their original handoff state. See the
[publication handoff](handoffs/2026-10-09-1600-codex-cellsim-workbench-publication.md)
and Git history for the shared revision. Human UI acceptance remains pending.

**October 9 interactive Workbench setup added locally:** Setup now has Scenario,
Choices, Compare values and Run settings sections beside a live strategy ×
purchase matrix. Cell counts include every setting variant and matched seed;
click a cell to inspect its combinations. Thirty described controls expose
runner-supported population/map/stat overrides, with loaded snapshot values,
editable comparison lists and saved presets. The existing sweep compiler and
pinned runner validate every condition; safe run/stop/resume remains in place.
New input settings also export as separate numeric Run data fields. Final
validation:13 setup/run check groups (42 fixture rows), all30 controls preflight,
13 GUI/export checks,21 worksheet tests and clean Release builds. All four
sections and compact/default layouts rendered/inspected. Older windows closed
with Bevin's permission after preset saving; canonical EXE updated. No research
batch, production data or Unity changes this turn. See
[guide](CELLSIM_WORKBENCH.md) and
[handoff](handoffs/2026-10-09-1454-codex-workbench-live-test-matrix.md).
Local/uncommitted/unpushed on BevBranch; earlier work preserved.

**October 9 pivot-ready worksheet iteration added locally:** A third **Run data**
sheet automatically exports the same selected examples as Run review, with one
run per row in the named RunData Excel table. Separate typed columns cover
populations, whole-run/per-round slash-line metrics and statuses, decisions,
purchases and loadout counts. Bevin explicitly chose the same 200 examples;
Batch summary still covers all 4,800 matching runs. The fresh workbook preserves
the existing review content and summary results; 110,108 values independently
reconcile with raw evidence, including 77,172 slash-line fields. Twenty Python
checks, twelve published-GUI checks and a zero-warning build passed. Saved
workbook sheets and native export form rendered/inspected. No new simulations.
See [guide](CELLSIM_EXCEL_EXPORT.md) and
[handoff](handoffs/2026-10-09-1432-codex-workbench-pivot-run-data.md).
Local/uncommitted/unpushed on BevBranch; prior work and workbooks preserved.

**October 9 Workbench readability iteration added locally:** New exports open
on Batch summary, showing experiment context and actual settings, Hare and
all-three-species survival counts/percentages, and explicit missing data.
Run review keeps its familiar layout with concise observations; detailed
purchase-change narratives are optional in the GUI/CLI. The versioned report
projection owns scientific interpretation/calculations; Excel layout and
Windows process/UI remain separate. Bevin's first 4,800-run workbook was
re-exported to a fresh readable file without new simulations or original edits.
All 24 summaries and 200 examples/native links independently checked; average
example row height fell from 182.79 to 90 points. Fifteen Python checks,
twelve published-GUI checks and zero-warning Release publish passed. See
[worksheet guide](CELLSIM_EXCEL_EXPORT.md) and
[handoff](handoffs/2026-10-09-1400-codex-workbench-readable-reports.md).
Local/uncommitted/unpushed on BevBranch; earlier work is preserved.

**October 9 Workbench setup/run controls added locally:** **Set up & run** now
provides readable experiment settings/notes, saved presets, a run-count preview,
validation and a deliberate Run button. Validation freezes a fresh experiment
and pinned runner without workers; progress, safe stop, resume/recheck and
Open saved run connect to the existing executor. Completed chunks survive stop;
changed/moved frozen evidence is refused. Export this run completes the
setup → run → review → export workflow. Load D5 example proposes 24 runs and
starts nothing. Scope is the neutral Plant/Hare/Fox six-round flow, not arbitrary
scenario authoring or a definition of human Gardeners play. Nine setup/run check
groups (26 final fixture rows), twelve portable runner checks and native
review/export regressions passed. Historical confirmation runner hash remains
unchanged; no Unity/domain/tuning changes or large research batch this turn.
See the [Workbench guide](CELLSIM_WORKBENCH.md) and
[setup/run handoff](handoffs/2026-10-09-0201-codex-workbench-setup-run-stop-resume.md).
Local/uncommitted/unpushed on BevBranch; prior work is preserved.

**October 9 local CellSim Workbench added:** `Open-CellSim-Workbench.cmd`
opens a native Windows GUI for the existing worksheet exporter and saved batch
progress. The built EXE is under `artifacts/cellsim-workbench/`; basic export
uses defaults, with optional filters/checkpoints, a visible background log and
safe new-file output. Batch monitoring reads small status/summary metadata,
flags stale Running state and checks summary identity. This first slice does
not start simulations; the setup/run entry above supersedes that original
limitation. Release publish and focused validation, including real
existing-evidence export and native rendering, passed. See the
[Workbench guide](CELLSIM_WORKBENCH.md) and
[handoff](handoffs/2026-10-09-0141-codex-cellsim-local-workbench.md).
Local/uncommitted/unpushed on BevBranch; earlier work is preserved.

**October 9 readable simulation worksheet exporter added locally:** Bevin's
manual Excel layout is now a reusable export format. Double-click
`Export-Simulation-Worksheet.cmd` or run `CellSim.ps1 Excel` to select existing
reports/completed sweeps, generate a fresh workbook and optionally open it.
Run rows show pre-purchase checkpoints, endpoints, choices, purchase limits,
factual recovery/death summaries, blank human notes and evidence links. Large
exports bound displayed rows and retain all-matching condition totals. The
goal is human-operated setup/run/review without AI prompts; this slice adds
export only. The existing 96,000-run batch was read/validated, not rerun, to
produce a 24-row example and independent 6,000-run summary cross-check.
Twelve focused checks passed. Runtime dependencies and untested native picker/
Excel interaction remain explicit. Read the [worksheet guide](CELLSIM_EXCEL_EXPORT.md)
and [handoff](handoffs/2026-10-09-0104-codex-human-readable-excel-exporter.md).
This work and the preceding inspection/display changes remain local and uncommitted.

**October 8 live Plant-total display added:** The Field Ledger now shows
`PLANTS: current total` beside Population, using the latest simulation snapshot.
Unity recompile and live binding checks passed at initial/decision/terminal
states; screenshots confirm visibility beside the Mutation/purchase overlay.
Play Mode is stopped after checks. Source/docs remain local and uncommitted.
Read the [display handoff](handoffs/2026-10-08-2242-codex-live-plant-population-display.md).

**October 8 D5 visual review setup prepared locally:** Copied inspection scene
`ForestEdge_D5_VisualReview` and scenario/Hare/Fox assets configure wrapped 54x32,
325/30/15, Hare vision 9, Fox energy 160 and six 100-tick phases with Journey off.
All eight supplied seed/purchase-policy examples reproduced their stated Unity
preview outcomes; the inspection-only runtime Inspector reset control passed
all four first-boundary seed checks, Unity recompile and the existing S4 fixture
regression. The legacy Game-view setup panel is hidden, so the Inspector handles
seed/reset/start and Game view handles purchases/Mutations. Human visual/player
review and tick-700 viability remain pending; production defaults are unchanged.
Read the [inspection handoff](handoffs/2026-10-08-2157-codex-d5-visual-inspection-setup.md)
and [run guide](Research/Experiments/S4-04-Hare-Purchase-Confirmation/VISUAL_REVIEW.md).
This work is local, uncommitted and unpushed.

**October 8 Hare-purchase confirmation complete:** All 96,000 runs, paired
analysis and purchase reports finished at 19:37:50 EDT. The 96 arms each have
1,000 fresh matched seeds 60000-60999; all 1,920 chunks passed. Completion review
verified frozen input/tool hashes, the 4.84 GB raw aggregate and analysis hashes,
exact seed coverage, Valid APS/AHS n=1,000 in every arm, and all 480,000 assigned
purchase windows with reconciled reached-window ledgers. Simulation took 4h 50m
34s at 16 workers. The live dashboard now reports Completed / 96,000 / zero workers.
Restoration and late-five improve Hare and all-species survival versus same-path
no-buy controls on all twelve skill arms. Restoration also beats same-policy
Skip all on both observed endpoints in all twelve arms. D5/S25 Gardeners
restoration leads this panel at 52.2% all-species survival versus 35.8% no-buy,
with mean APS -0.654, AHS -0.102 and spend 63.16 Field Data. The earlier C2/S14
Gardeners restoration decline did not reproduce; the each-five 59% screen
maximum did not repeat. Read the
[completion/results handoff](handoffs/2026-10-08-codex-hare-confirmation-results.md)
and [focused review](../artifacts/cellsim-hare-confirmation-20261008-143725/purchase-analysis/focused-review.md).
Keep all four candidates; prioritize reviewing restoration and late-five.
New thresholds/caps, tick-700 validation and production changes remain proposals.
This completion review is local; the preceding publication is already pushed.

**October 8 publication completed before confirmation finished:** Bevin requested committing and pushing the
completed standalone tooling, purchase experiment work, progress dashboard and
research without disturbing the running confirmation. Fresh isolated Release
build (zero warnings/errors), portable self-test, sweep-analysis regression and
dashboard fixtures passed. The rebuilt DLL is byte-identical to the live run's
copy; all frozen input/tool hashes remain unchanged. Completed evidence and
immutable confirmation inputs are packaged in
[the shared research bundle](Research/Results/2026-10-08-CellSim-Sweeps/README.md).
Raw/active output remains local. Main commit `bada43ad` was pushed to
`origin/BevBranch`; local and remote SHAs were verified equal. The active batch
continued to 30,000 validated runs with 16 workers and all pinned hashes intact.
Read the [publication handoff](handoffs/2026-10-08-codex-cellsim-publication.md).
Trello synchronization remains outstanding. The completion review above supersedes
the running status at this publication checkpoint. The final handoff commit
`e250d32f` was also pushed and verified against the remote.

**October 8 Hare-purchase confirmation launch checkpoint (now complete):** Bevin accepted the recommended
first step with "okay lets begin step by step". The approved follow-up runs the
same four candidates and four paths with six purchase policies (none, late-five,
each-one, each-three, each-five, restore-toward-start), 1,000 fresh matched seeds
60000-60999: 96,000 runs at 16 workers. It started at 14:37 EDT in
`artifacts/cellsim-hare-confirmation-20261008-143725/`; all 96 scientific argument
sets match the corresponding screen arms, and the runner and snapshot are
byte-identical to the validated screen. Portable self-test and frozen input
checks passed. Prior Unity/reference/smoke validation is retained evidence,
not rerun validation. First 16 chunks / 800 runs validated with 16 workers active.
Automatic paired analysis and purchase reports follow completion. Read the
[confirmation protocol](Research/Experiments/S4-04-Hare-Purchase-Confirmation/PROTOCOL.md)
and [launch handoff](handoffs/2026-10-08-codex-hare-confirmation-launch.md).
Bounded earlier-screen diagnostics reconcile APS/AHS components on 800 whole-run
Gardeners observations plus phases; different observed windows and conditional
phase sample sizes remain explicit. These are screen findings, not confirmation
results. Threshold/cap extensions, tick-700 tests and production tuning remain
future proposed scope, pending review.

Bevin subsequently requested a visual progress dashboard. A read-only localhost
dashboard is running at `http://127.0.0.1:59057/`, opened in Codex's browser.
It refreshes every five seconds and shows validated chunk/run totals, workers,
elapsed/estimated remaining simulation time, 96 matrix cells and active chunks.
`tools/CellSim.Batch/dashboard.py` / `dashboard.html` use existing status and
completion metadata; no runner or frozen input changes. See the
[dashboard handoff](handoffs/2026-10-08-codex-cellsim-progress-dashboard.md).

**October 8 completed Hare-purchase panel:** Bevin retained
D4/S19, D4/S14, C2/S14 and D5/S25 from the quarter-million grid as candidates to
develop, then requested studying the upgrade-window Buy Hare choice. The current
player purchase costs 10 Field Data per Hare, separate from the Mutation choice;
the legacy catalog's five-Data reinforcement price must not be substituted.
The [purchase protocol](Research/Experiments/S4-03-Hare-Reinforcement-Candidate-Panel/PROTOCOL.md)
and verified machine-readable panel cover eight purchase policies across those
four contexts and all four paths, with 100 fresh matched seeds 50000-50099 /
12,800 runs at 16 workers. Bevin approved execution with "yes run it".
The shared harness now handles wallet-aware purchases and window audits, validated
against Unity and historical controls before launch. The batch started October 8
at 13:34 EDT in `artifacts/cellsim-hare-purchase-20261008-132317/`, with automatic
paired analysis and purchase reports. All 12,800 runs and analysis completed at
14:15 EDT; simulation time was 39.35 minutes. Completion review verified frozen
input/raw/analysis hashes, 100 seeds per arm, APS/AHS Valid n=100 throughout and
64,000 assigned windows. Restoration improved both survival endpoints on all
three paths in D4/S19, D4/S14 and D5/S25, but hurt C2/S14 Gardeners. Late-five
had positive observed purchase effects across all twelve candidate/path arms.
D5/S25 Gardeners each-five had the highest all-species survival, 59% versus 39%
without purchases, with higher spending and lower APS. These are exploratory
observations; production defaults and follow-up selection remain human decisions.
Read the [results handoff](handoffs/2026-10-08-codex-hare-purchase-results.md) and
the [focused review](../artifacts/cellsim-hare-purchase-20261008-132317/purchase-analysis/focused-review.md).

**October 8 standalone CellSim tooling:** Bevin authorized the implementation of
a .NET 8 batch runner linking the production simulation and original experiment
harness, then clarified that the goal is sweeping populations, grids, starting
stats and skill paths to find candidate scenarios/mechanics. Unity exports frozen
authored inputs; bounded console workers execute Cartesian or reproducibly sampled
conditions with matched seeds, recovery, provenance and detailed replay. The sweep
compiler supports 27 registered starting-stat overrides; SQLite/CSV analysis retains
all-species slash lines, trajectories, status-aware distributions, path/held-axis
paired deltas and shortlist confirmation. A 192-run/48-condition tooling pilot and
120-run fresh-seed shortlist fixture complete, with analysis/integrity regressions
passing. Existing cohort summarization uses the exact retained trajectories.
Current-source Unity checks pass 259 focused EditMode tests and four reference
seeds, including changed grid/populations/stats plus Trailblazer choices.
The million-run plan passed dry-run validation; execution at that scale remains
unmeasured. Read the [standalone runbook](CELLSIM_STANDALONE_RUNBOOK.md) for the
commands, measured throughput and exact validation limits. These changes were
subsequently published on BevBranch in the October 8 publication checkpoint above.
This is shared-core diagnostic
tooling; the short fixtures and illustrative ranking do not approve balance.
The meeting's population and gameplay decisions still need balance work.

**October 8 quarter-million grid complete:** All 250,000 runs and automatic
analysis completed at 10:09 EDT after 6h 56m, using 16 workers. Independent hashes,
SQLite integrity and exact coverage of 2,500 arms x 100 fresh seeds passed.
The 625-context panel retains all 81 previous contexts and adds 544, at the same
planned 600-tick horizon. Raw/SQLite/CSV evidence and compact completion reviews
are in `artifacts/cellsim-250k-sweep-20261008-030316`.
The new selected top arm (325/30/15, 54x32, Fox energy 80, Hare vision 9,
Gardeners) retained all species in 68/100 seeds versus 16/100 for skip-all.
The earlier winning context replicated the Gardeners advantage (54/100 vs 18/100);
its earlier vision-9 advantage did not clearly replicate (54 vs 51 at vision 5).
All top ten arms use Gardeners. Selection/inference remain exploratory; tick-700
ecology, acquired-choice mechanisms, event pace and gameplay review are pending.
Read the [quarter-million handoff and completion](handoffs/2026-10-08-0303-codex-250k-grid-kickoff.md)
for inputs, verification and evidence limits.

**October 8 larger sweep complete:** All 32,400 runs and automatic analysis completed
at 02:12 EDT: 81 population/grid/starting-stat contexts, four Hare path policies,
100 matched seeds and six 100-tick phases. Execution began at eight workers and
resumed at Bevin's requested 32; total launch-to-analysis time was 39.86 minutes.
Independent checks confirm all 1,296 chunks, exact per-condition seed coverage,
raw/CSV hashes and SQLite integrity. Outputs and compact completion review are in
`artifacts/cellsim-large-sweep-20261008-012921`. The strongest screened arm
(250/20/10, 42x24, Fox energy 80, Hare vision 9, Gardeners) retained all species at
tick 600 in 60/100 seeds versus 19/100 for its matched skip-all control. This is an
exploratory selected result; fresh-seed, tick-700 ecology and gameplay review remain
pending. Read the [handoff and completion record](handoffs/2026-10-08-0135-codex-large-sweep-kickoff.md)
for the frozen inputs, verified outputs and evidence limits.

**October 7 meeting decisions:** success means reaching tick 600 with an ecology viable through tick 700; viability is still undefined. Upgrades carry into the next simulation in the journey. Keep 600/40/25 as historical research and seek a new starting population with a more personal pace of kills, births, and other events. SimMasterBev owns the next balance work; DNA/secondary simulator work is deferred. No new threshold, population, or gameplay tuning has been implemented. Read the [meeting outcomes](Meeting%20Briefs/2026-10-07-Journey-and-Ecology-Balance-Agenda.md#meeting-decisions) and [handoff](handoffs/2026-10-07-2239-codex-journey-meeting-decisions-and-population-pacing.md).

**October 7 publication and journey integration:** S4 checkpoint `64a8e3b5` is pushed to BevBranch, and Salty's ProjectMain `fea55926` is integrated. The only content conflict was this document; both research and journey entries were retained. Fresh merged-revision checks passed 158 focused EditMode tests and one PlayMode journey continuity test with wrapping enabled. No fresh graphics review, full suite, build, or journey balance batch was run. The seven research packages below retain their original experiment dates and frozen-source provenance; older statements about local/uncommitted status describe those historical runs. See the [publication and integration handoff](handoffs/2026-10-07-2049-codex-s4-balance-publication.md). No new gameplay tuning is approved by this checkpoint.

**S4-03 Trailblazer early-choice timing diagnostic: 2026-10-07, complete locally; human balance review pending.** Moving the first of five Trailblazer choices from tick 100 to tick 0 did not improve the matched all-species clear rate: 28/200 (14%) versus 30/200 (15%) on the same seeds, with a paired risk difference of -1 percentage point (95% CI -7.3 to +5.3; exact McNemar p=.878). The first-phase median Hare population ended higher (26 vs. 21), while median combat deaths changed only from 39 to 37.5; fewer candidate runs reached tick 600 (102 vs. 121). No gameplay values, defaults, or player-facing UX changed. See [protocol](Research/Experiments/S4-03-Trailblazer-Early-Choice-Timing-600-40-25/PROTOCOL.md), [report](Research/Experiments/S4-03-Trailblazer-Early-Choice-Timing-600-40-25/REPORT.md), [interpretation](Research/Experiments/S4-03-Trailblazer-Early-Choice-Timing-600-40-25/ANALYSIS.md), and [handoff](handoffs/2026-10-07-0024-codex-s4-03-trailblazer-early-choice-timing.md). Raw run artifacts and validated summary remain under the ignored `artifacts/` directory.

**S4-03 offer-aware strategy policy screen at 600/40/25: 2026-10-06, complete locally; balance review pending.** Four arms ran on 200 fresh common seeds (14200–14399) each. All-species clear rates were 16.0% skip-all, 15.0% Trailblazer, 17.0% Warren, and 25.5% Gardeners. Gardeners improved over skip-all on matched seeds (+9.5 percentage points, 95% CI +3.2 to +15.8); none reached the rough 40–50% target. Median Fox starvation deaths were 47–48 and energy-blocked reproduction candidates about 6,700–7,700 per run, flagging Fox persistence for diagnosis rather than immediate tuning. No gameplay values or defaults changed. See [protocol](Research/Experiments/S4-03-Strategy-Choice-Policy-Screen-600-40-25/PROTOCOL.md), [report](Research/Experiments/S4-03-Strategy-Choice-Policy-Screen-600-40-25/REPORT.md), [interpretation](Research/Experiments/S4-03-Strategy-Choice-Policy-Screen-600-40-25/ANALYSIS.md), and [handoff](handoffs/2026-10-06-2326-codex-s4-03-strategy-choice-policy-screen.md). Raw artifacts remain under the ignored `artifacts/` path.

**S4-03 all-species threshold confirmation at 600/40/25: 2026-10-06, complete locally; human balance decision pending.** On 200 fresh common seeds per arm (14000-14199), the strict node-clear rate was 25/200 skip-all control, 58/200 Seed Dispersal, 50/200 Gardeners Digestion-first, and 57/200 Gardeners Dispersal-first. Each strategy cleared on more matched seeds than control alone; the two Gardeners orders did not differ decisively. Many horizon-reaching intervention failures miss the minimum-Fox threshold. No skill values, scenario defaults, or authored population settings were changed. See the [confirmation package](Research/Experiments/S4-03-Hare-Paired-Seed-Confirmation-600-40-25/README.md) and [handoff](handoffs/2026-10-06-1432-codex-s4-03-200-seed-threshold-confirmation.md). Full reports are local under the ignored experiment-artifact root.

**S4-03 Hare paired-seed screen at 600/40/25: 2026-10-05, complete locally; human review pending.** Bevin selected 100 fresh common seeds per arm. The corrected 13-arm strategy matrix was run with 600 Plants / 40 Hares / 25 Foxes, wrapping on, 36x20, six 100-tick phases, 0.1-second steps, coupled responses off, and no reinforcements or added Genomes. Seeds 13000-13099 produced 1,300 runs and 7,219 observed phase windows; all run and slashline accounting checks passed. Seed Dispersal and both Gardeners orders retained all three species at tick 600 in 43/100 each, versus 19/100 control, while Hare survival was 60-61/100 versus 56/100. Both Trailblazer orders survived less often (42/100 and 46/100). These are contextual screening observations; medians remained low, means were skewed, and no tuning or balance decision is approved. See the [screen package](Research/Experiments/S4-03-Hare-Paired-Seed-Screen-600-40-25/README.md) and [handoff](handoffs/2026-10-05-2348-codex-s4-600-40-25-paired-seed-screen.md). Raw evidence and exact dirty-source hashes are local under the ignored artifact root. No source or authored simulation asset was changed during the batch.


**Wrapping population search: 2026-10-04, complete locally; visual candidate selection pending.**
Bevin prefers wrapping and authorized a broad starting-population sweep with fixed
species/skill values. The 336-combination, five-seed screen and seven-preset,
20-fresh-seed recheck completed 1,820 main runs plus eight pilots. All inputs,
7,310 phase windows, FPO accounting, slashline arithmetic and source hashes pass.
600/40/25 survives in 10/20 and retains all three species in 2/20; 600/5/10
survives in 13/20 but often narrowly after a huge boom; 400/20/10 survives in 1/20.
Recommend Bevin visually compare 600/40/25 seeds 12000, 12011 and 12003 first.
No source or authored defaults changed; existing local wrapping/avoidance work
remains uncommitted. Read the [experiment package](Research/Experiments/S4-03-Wrapped-Population-Sweep/README.md)
and [handoff](handoffs/2026-10-04-0127-codex-s4-wrapped-population-sweep.md).

**Optional wrapping-grid experiment: 2026-10-04, complete locally; Bevin prefers the visual feel.**
Bevin authorized connecting top/bottom and left/right edges with an Inspector
toggle. `Species Simulation Preview > Wrap Edges` applies to both species and
their movement, perception, interactions and neighbor counts; bounded remains
the default. During Play, use Reset to Start before changing the toggle under
Manual Simulation Testing. Six focused live assertion checks, matched initial
layouts, and 80 matched 400/20/10 runs pass verification. Wrapping survives in
0/20 control and 0/20 Movement-first Trailblazer runs versus 7/20 and 2/20
bounded; this does not establish a balanced or more survivable default. Bevin visually
preferred wrapping despite the failures and authorized a broad starting-population
sweep with wrapping on. The authored default remains bounded. Earlier bounded results
replay unchanged. Read the [wrapping handoff](handoffs/2026-10-04-0022-codex-s4-wrapping-grid-experiment.md)
and [experiment package](Research/Experiments/S4-03-Wrapping-Grid-Screen/README.md).
Source changes remain local and uncommitted with the prior paired-screen work.

**S4-03 paired-seed screen: 2026-10-03, complete locally; human decision pending.** Bevin selected
400 Plants / 20 Hares / 10 Foxes for the 13-arm, 20-seed comparison of six
single Hare skills and both acquisition orders of the three core pairs. The
first screen exposed scheduled Threat Avoidance not reaching experimental
options; follow-up exposed an obsolete flee-speed gate and process-wide entity
IDs in avoidance hashing. Local fixes and three focused assertion checks pass. The corrected 260-run
screen and avoidance replay validate: control survives in 7/20, Crowding
Tolerance alone in 9/20, and pairs in 2/20 to 5/20; all median final Hare
populations are zero. No skill values or production assets were tuned.
Read the [screen handoff](handoffs/2026-10-03-1200-codex-s4-paired-seed-development-screen.md)
and [experiment package](Research/Experiments/S4-03-Hare-Paired-Seed-Screen/README.md).
Results and fixes are not committed or pushed. The S4 Inspector fixture already
uses 400/20/10; authored scenario defaults remain separate.
**Forest Edge journey prototype: integrated 2026-10-06, layout updated 2026-10-14.**
The Desktop Simulation icon opens the scrolling journey map. A choice of three
trailheads leads through the opening reward, first simulation, connected habitat
condition, and second simulation. The same run, ecosystem, history, and accumulated
Mutations continue through phase 12. The visual map has 29 markers across 13
stages; the first ten markers are playable and later content remains provisional.
See [the prototype contract](JOURNEY_PROTOTYPE.md) for current effects, approved
upgrade carryover, art coordinates, and remaining content and balance decisions.

**Chrono species artwork: 2026-10-02, accepted for BevBranch publication.** Both handmade source
sprites now supply the standardized 32/64/128 icons and retained compatibility
exports. Existing asset names and GUIDs remain stable, covering the animal atlas,
Species Catalog and Noesis icon consumers. Both simulation scenes use the Chrono
fox/rabbit board sprites. Rebuild from the originals with **Salty Game > Art >
Rebuild Chrono Species Icons**. Export validation and two focused PlayMode checks
passed; Field Notes and simulation screenshots were inspected. Bevin visually
accepted the integration and authorized pushing it with additive Trello updates.
Separate local prototype population and fox species edits are excluded from the
art checkpoint. See
[art ownership and export instructions](Species%20Design/CHRONO_SPECIES_ART.md)
and the [validation handoff](handoffs/2026-10-02-chrono-species-art-standardization.md).

**S4-01 Hare strategy workbook: 2026-10-01, ready for Salty review.** Bevin
approved starting values, costs and example paths for Trailblazer, Warren
and Gardeners. The workbook is now a repository review artifact; team review,
implementation and balance evidence remain. Read the
[workbook handoff](handoffs/2026-10-01-1711-codex-s4-01-hare-workbook-review.md)
for the decisions and a review prompt for Salty and his agent. The intended
400/20/10, 0.1-second manual S4 fixture differs from committed scenario defaults.

**S4-02 first implementation slice: 2026-10-01, local and ready for review.**
Bevin authorized reversible implementation without waiting for Salty's explicit
approval; team review remains pending. General Movement and Threat Avoidance
now support the Trailblazer path, and the preview Inspector has an in-memory
S4 fixture and paused single-tick controls. Focused validation passed 180
EditMode and two PlayMode tests. Bevin closed the first manual visual-testing
pass on October 1 after checking the controls, Movement acquisition and results.
The seed-5, 400/20/10 run ended in Hare extinction at tick 180; Threat Avoidance
and the full five-pick path were not reached in that manual run. A clipped
Movement card footer and stale Game-view setup display remain UI follow-ups.
Bevin reports preferring 400 Plants / 20 Hares / 10 Foxes during population
exploration. This is the next comparison candidate; the implemented S4 fixture
and authored production defaults have not been changed. Seed Dispersal is now
implemented locally with direct planting counters and a Gardeners offer path;
automated and manual card/results acceptance are complete, while balance
validation remains open. The `CellSim` wrapper was repaired and its Clean test
and experiment lanes passed focused smoke checks on 2026-10-14; the Live lane
still needs a ready Editor check. See the
[tooling handoff](handoffs/2026-10-14-1533-codex-cellsim-wrapper-restoration.md).
Also read the
[implementation handoff](handoffs/2026-10-01-2216-codex-s4-02-trailblazer-and-editor-manual-testing-first-slice.md)
and the original [six-skill checklist](handoffs/2026-10-01-1947-codex-s4-02-implementation-checklist-and-editor-testing-preflight.md).

**Player slashline display: 2026-10-02, local and ready for review.** The existing
Field Notes button now opens cumulative species metrics without Developer Mode,
and both completed and failed results show the same slashline. Existing domain
calculations, ADD accounting and N/A/INVALID statuses are reused. Live Unity
checks covered Hare/Fox, paused updates, decisions, reinforcement and results;
rendered prototype panels were inspected. Bevin reported the new display looks
good on October 2. The NUnit regression was updated but
not executed, to preserve the open scene's unsaved edits. Read the
[slashline handoff](handoffs/2026-10-01-2358-codex-player-slashline-field-notes-and-results.md).

**Warren crowding energy reduction: 2026-10-02, local and ready for review.**
Bevin selected this next slice. Crowding Tolerance now removes 10% of the
original extra crowding cost per level (cap 10), with a separate fractional
loss accumulator and unchanged ordinary metabolism/crowd eligibility. The
Inspector and reports expose eligible animal-ticks and actual energy lost.
25 focused assertion cases passed in place, compilation passed, and a live
six-tick diagnostic completed the five Warren choices through normal offers;
the offer card was rendered and inspected. This is mechanics/presentation
evidence, not balance or a standard-suite result. The modified-scene dialog
was closed by Bevin; the newly saved prototype scene diff is preserved for
separate review. Read the
[Warren handoff](handoffs/2026-10-02-0143-codex-s4-02-warren-crowding-energy-reduction.md).
Read the [Seed Dispersal handoff](handoffs/2026-10-02-1319-codex-s4-02-seed-dispersal-and-gardeners.md).

The September planning notes below are dated snapshots. Use the
[S4 Trello control card](https://trello.com/c/Zn4UpBYc) for current scope,
ownership and capacity; those older notes are not a fresh kickoff decision.

**S4 active: 2026-10-01.** Sim accepts 20h (14h feature, 3h protected
reserve, and 3h unallocated). Josh confirmed his 20h
availability and responsibilities. Josh chose the active profile plus current
defaults for P1-032 (Forest Edge, Hare, seed 10100), accepted its 6h estimate,
deferred local profile-choice save/restore, and approved 400/25/15 with a
0.1-second step as an experiment variant; the authored production asset
remains 400/55/35 at 0.2 seconds. Missing or invalid profile context must fail
closed to visible profile selection. The control/candidate pilot was attempted
but Unity licensing and Package Manager failures stopped it before simulation;
no experiment result or S4-02/03 re-estimate exists. Josh accepts the current
S4-02/03 estimates as planning assumptions. The pilot remains an early
checkpoint before the full screen, not a kickoff blocker. S4 is Active from
2026-10-01; P1-032 and S4-01/02/03/07 are in Trello Current Work, with the
S4 control card in Roadmap & Milestones. S4-04/05 remain in Backlog. The 8h
reserve is protected.
See the [S4 control record](Sprints/S4-control-record.md), [kickoff handoff](handoffs/2026-10-01-codex-s4-kickoff.md), and [accepted Sim
review](handoffs/2026-09-30-2018-codex-s4-sim-review-and-stat-sheet-strategy-plan.md).

**Loose Ends disposition: 2026-10-01.** P2-005 raw-artifact retention and
P2-022 Terrain Paint diagnostic retention are decided. CF-6 remains optional
Backlog work, and P2-015 is no longer an unbounded cleanup ticket. No raw
reports or Unity assets were removed. See the [ledger](LOOSE_ENDS.md) and
[review handoff](handoffs/2026-10-01-0112-codex-loose-ends-retention-decisions.md).

**P1-029 contract accepted: 2026-10-01.** Josh approved the Fertile Droppings
direction and progression defaults: Gene Lab after three completed runs, a
permanent Rabbit tree license, Rabbit Data-funded active upgrades, a refunded
resettable allocation with a flat Research Data fee, and the existing 8-point
capacity. The node's scent behavior and numeric balance are not implemented or
validated. P1-029 is closed as a design gap; implementation remains future
work. See the [accepted contract](handoffs/2026-10-01-0154-codex-p1-029-first-hare-genome-contract.md)
and [Loose Ends](LOOSE_ENDS.md).

**S4 planning basis: 2026-09-29.** M1 and S3 are closed. The M1 evidence and
cap-aware offer fix are recorded in pushed commits `25642440` and `974827f2`.
Sim's proposed work builds on his recent Forest Edge, Mutation, telemetry, and
report changes. The M1 Trello card is COMPLETE, and S3-05 remains Backlog
stretch work. See the [Sim kickoff handoff](handoffs/2026-09-29-1241-codex-s4-sim-kickoff-approvals.md)
and [M1 closeout](handoffs/2026-09-29-codex-m1-closeout.md).

**Hare purchase stat line: 2026-09-28, shared on BevBranch.** Successful phase-boundary
population additions are now tracked as `ADD` in the run and checkpoint. The
whole-run Herbivore and Predator Stat-Lines include `ADD` in expected FPO and
survival denominators and exclude it from birth-based RFS. The in-game summary,
JSON report, human report, CSV, and independent Hare validator expose or account
for the new field; phase windows still start with their post-purchase opening
population. Static diff inspection and script parsing passed. The locally
installed Unity 6000.4.6f1 Editor compiled the project with zero Console
errors; gameplay behavior remains unverified because no test or live purchase
run was requested. The Editor instance used for compilation was closed.

**Forest Edge population cleanup: 2026-10-01, ready for review on BevBranch.**
See the [cleanup handoff](handoffs/2026-10-01-1916-codex-population-configuration-cleanup.md) for scope and verification. The shared
production default remains 36x20, 400 Plants / 55 Hares / 35 Foxes, zero
starting probabilities and a 0.2-second step. The proposed S4 fixture uses
400/25/15 and an explicit 0.1-second step through experiment inputs; it does
not replace the production asset. `CellSim Run -StartingPopulations` now passes
those inputs through the existing runner. Saved explicit preview presets are
preserved; preview settings without an override follow the selected asset.
Unity 6000.4.6f1 clean focused validation passed 10/10
`PopulationConfigurationTests` EditMode cases and 2/2 existing developer-settings
PlayMode cases. A one-seed, two-tick `CellSim Run` smoke check at 0.1 seconds
recorded 400/25/15 at tick zero; the production asset still matches HEAD.
The checks exercise combined grid/count overrides, invalid inputs, saved v4/v5
presets, defaults following authored counts, and edit/Undo/save/reload.
Local results are under `artifacts/unity-tests-20261001-180224/`,
`artifacts/unity-tests-20261001-180800/` and
`artifacts/cellular-experiment-20261001-180623/`. This is focused functional
validation, not a full-suite, visual Catalog interaction or balance acceptance.

**Historical Forest Edge no-choice run: 2026-09-29, local on BevBranch.** This
run used a locally edited 36x20 scenario with 400 Plants, 25 Hares and 15
Foxes, all starting probabilities zero. It did not establish a shared baseline.
A matched 200-seed no-choice run (10100–10299) through the locally installed
Unity Editor, using the saved desktop 0.1-second step, observed Hare extinction
in 178/200 runs (median tick 211) and Fox extinction in 196/200 (median tick
351); only 4 runs reached tick 600. Plants averaged 402.05 at tick 200, while
Hare combat deaths outnumbered starvation deaths, consistent with predation
being the main early pressure in that configuration. Later checkpoint counts
are censored by runs ending when the selected Fox player species goes extinct. The scenario
asset still authors a 0.2-second step; this evidence is for the live saved
0.1-second setting. See the [shared historical evidence summary](Research/Reports/2026-10-01-historical-forest-edge-populations/report.md).
Raw reports and the original analysis remain in ignored local artifacts.

**Historical composite-Mutation pilot: 2026-09-29, local.** The catalog Mutation,
`warren-guarded-burrow` (block +2, movement speed -0.25), was tested alone
against a same-seed no-Mutation control on seeds 10100-10119 using a local
400/25/15 Forest Edge start, a 0.2-second step and Hare as the player species.
This differs from the 0.1-second, Fox-player historical baseline above.
It was acquired after phase 1 and retained through
the six 100-tick phases. Control had 2/20 Hare extinctions and mean terminal
Hare 119.05; Guarded Burrow had 0/20 and mean terminal Hare 137.10. Mean APS
was 1.35887 versus 1.56838. All 20 slash lines per arm independently validated
with the documented HPS/EHS/ECN raw-event limitations. This is directional pilot
evidence only. Guarded Burrow combines block and a movement penalty; it does
not validate the proposed S4 basic Hide skill. Reconcile the earlier full
200-seed follow-up with the current S4 pilot-first plan before scheduling more
work. The [shared historical summary and per-seed slash lines](Research/Reports/2026-10-01-historical-forest-edge-populations/report.md)
preserve the comparison; raw reports and validators remain local artifacts.

**Previous Forest Edge baseline: 2026-09-29.** The 400 Plants / 55 Hares / 5
Foxes run remains a historical comparison. Its report is local-only at
`artifacts/forest-edge-startpop-screen-20260929/report.md`; it is not part of
the shared evidence package.

**Scenario population authoring: 2026-09-29, local.** The Species Catalog's
Scenario Roster exposes editable starting-population fields on each species
card. Edits use Unity's serialized-object Undo path; click **Save Assets** to
persist them, then start or restart a preview without an explicit population
override to use the selected scenario's authored values. The historical run
loaded the locally edited scenario asset without a starting-population
override and verified 400 Plants / 25
Hares / 15 Foxes at tick zero in all 200 seeds. The screenshot/run values are
recorded locally. This authoring tool does not approve a production balance change.

**S3-08 Fox telemetry clarification: 2026-09-27, validated.** The report
already distinguishes pre-resolution FSM state ticks from resolver outcomes;
the reproduction funnel now also exposes eligible attempts (chance failures,
no-location outcomes, and successful attempts) alongside the blocked gates.
This is a derived metric and does not change Fox behavior. The new Editor test
initially caused Safe Mode because the project's NUnit version does not support
`Assert.Multiple`; separate `Assert.That` calls fixed the compile error. The
direct installed Unity CLI EditMode runs passed the report serializer and
runtime reproduction funnel tests (1/1 each). A five-seed, 600-tick Forest
Edge report on the authored 36x20 grid contains
`reproductionEligibleAttempts`; the generated Markdown includes the Eligible
column. The current Editor log has no compiler errors. The 177 warnings in the
user's screenshot are not the Safe Mode blocker. Artifacts are under
`artifacts/s3-08-reproduction-telemetry/`. See the [S3-08 telemetry
handoff](handoffs/2026-09-27-codex-s3-08-fox-telemetry.md).

**S3-04 Mutation readability and first bounded review: 2026-09-27.** The
phase-choice path now offers three distinct free Hare Mutations plus Skip,
allows repeat picks to increase the level, and keeps the same expedition
running. Tough Hide was approved as the first evidence candidate. A matched
20-seed, six-phase Forest Edge/Hare comparison supports the directional copy
“Block more incoming attacks,” but does not establish a broad population
benefit. The player-facing copy and generic A/B/C markers are accepted for this
slice. Focused EditMode 1/1 and three PlayMode checks passed in the installed
Unity Editor. See the
[S3-04 Tough Hide handoff](handoffs/2026-09-27-codex-s3-04-tough-hide.md).

**Forest Edge board visual pass: 2026-09-27.** The visual pass was merged into
`BevBranch`, based on the birth-poof pilot. The board now
has a pixel canopy edge, low ground details tied to grass/resource state, and
prototype side rock clusters that sit outside the traversable field. Birth
presentation now gathers all children placed in the latest birth tick, enlarges
the poof, and retains the heart and sparkles. A short code-synthesized litter
chime is rate-limited and uses the desktop volume/mute controls. The view model
tracks up to four actual adjacent-cell steps from one Fox that is currently
hunting; their marks fade over nine simulation ticks and persist through pause.
The retained baseline and first changed 1280×720 views are in
[`visual-evidence-20260927-095257`](../artifacts/visual-evidence-20260927-095257/)
and [`visual-evidence-20260927-095849`](../artifacts/visual-evidence-20260927-095849/).
The first changed image shows a clear wooded edge, but the center still reads
as the original grass/bare grid. Later variation and rock placement edits have
not been captured. Unity crashed during a later live visual run; the clean lane
then reported the project lock as unreachable. Doctor also reported the Unity
licensing client and Unity services endpoint unreachable. The merged project
compiled in the directly installed Unity 6000.4.6f1 Editor on 2026-09-27;
full visual acceptance remains open. See the
**Forest Edge Hare metabolism first pass: 2026-09-28.** The production Hare
energy-loss interval is now 8 ticks instead of 10; grass, reproduction, and
other Hare values are unchanged. This is a provisional first pass to intensify
the no-predator famine, not a Fox–Hare equilibrium decision. The matched
no-Fox 20-seed test showed a smaller mean Hare peak (155 vs. 171), grass
depletion about 25 ticks earlier, and extinction by tick 600 in 20/20 runs
(baseline 11/20). The same in-progress Hare dispersal code was present in both
arms, so its effect is not isolated. See the [comparison analysis](../artifacts/cellular-experiment-20260928-034040/ai-analysis-v1.md),
[candidate report](../artifacts/cellular-experiment-20260928-034040/report-summary.md),
and [baseline report](../artifacts/cellular-experiment-20260928-033949/report-summary.md).

**Crowded Hare dispersal: 2026-09-27.** The fallback Hare movement path now
allows an overcrowded Hare to move into an adjacent passable, empty cell above
the local group-size cap when that move reduces nearby Hare density. Normal
mate-seeking, feeding, threat escape, and non-Hare movement retain their
existing priorities. A focused regression test was added, but the project test
runner did not execute it because the connected Unity Editor is currently in
Play Mode. No map-spread or starvation experiment has been run, so this is a
provisional behavior change, not a validated population outcome. See the
[crowded-Hare dispersal handoff](handoffs/2026-09-27-1124-codex-crowded-hare-dispersal.md).

**Forest Edge board visual pass: 2026-09-27–28.** Work continues on
`codex/forest-edge-visual-pass`, based on the birth-poof pilot. The board has a
pixel canopy edge, low ground details tied to grass/resource state, and
prototype side rock clusters outside the traversable field. Birth presentation
gathers all children placed in the latest birth tick, enlarges the poof, and
retains the heart and sparkles. A short code-synthesized litter chime is
rate-limited and uses the desktop volume/mute controls. The board view model
tracks up to four actual adjacent-cell steps from one Fox that is currently
hunting; their marks fade over nine simulation ticks and persist through pause.
The compact phase tracker now gives its active pixel bunny a soft double
heartbeat pulse. A small Field Ledger reaction card appears for a new birth or
Fox hunt, stays hidden between events, uses existing rabbit/fox pixel art, and
lets births take priority over hunt notices. The reviewed 1280×720 and
1920×1080 captures are in
[`visual-evidence-20260928-163621`](../artifacts/visual-evidence-20260928-163621/)
and [`visual-evidence-20260928-163450`](../artifacts/visual-evidence-20260928-163450/).
The center still reads mainly as a grass/bare grid; trees and other habitat
variation remain future work. Unity crashed during a later visual attempt on
2026-09-27, but the Editor reconnected on 2026-09-28 and the focused visual
acceptance test passed at both resolutions. The captures manually trigger the
reaction presentation; a natural birth/hunt through the full event route and a
full suite have not yet been verified. A clean full PlayMode attempt on
2026-09-28 stopped before tests began because the Unity licensing client threw
an `ObjectDisposedException` and Package Manager IPC timed out; it produced no
test verdict. See the
[field-pass handoff](handoffs/2026-09-27-forest-edge-visual-pass.md).

**Crowding and starvation: 2026-09-26.** Production Hares and Foxes use a
2× metabolism multiplier while their local group exceeds its size limit and
tolerance. The crowding pass no longer kills creatures directly; lethal energy
loss is classified as starvation. This is implemented in the
[simulation step](../Assets/Scripts/Game/Simulation/SpeciesSimulation.cs). No
post-change simulation or tests have been run, so the balance effect is
unverified.

**Time-based grass spread: 2026-09-24.** Grass now spreads on a per-tile
simulation-time interval instead of a per-tick chance roll. The provisional
17.4-second interval preserves the previous expected wait at Forest Edge's
0.2-second tick interval. Each plant tile tracks elapsed simulation ticks,
converted using the scenario step duration; the timer pauses while an animal
occupies the tile and resets after successful spread. Initial plant timers are
seeded at staggered points in the cycle to avoid a synchronized first wave.
This changes spreading; passive food-reserve regrowth keeps its current
behavior. No tests or matched balance runs have been run for this change, so
the interval remains unverified. Hare seed-drop chance is zero in the
production definition, diagnostic definition, and scenario generator, so
Hares consume grass locally without reseeding it. See the
[time-based grass growth handoff](handoffs/2026-09-24-codex-time-based-grass-growth.md).

**Hare mate-seeking and reproductive drive: 2026-09-24.** The production Hare
now has a 100% reproduction chance and can seek a mate at 50% of its 48-energy
cap (24 energy), matching the existing 50%-cap mating cost. Once eligible, a
Hare paths toward the nearest same-species individual without a reproduction
cooldown anywhere on the board before foraging; this social search is separate
from vision so Fox-detection range stays unchanged. The local group limit is
raised from 3 to 7 so a pair and its maximum five-offspring litter can use the
same patch; at that time, direct crowding deaths were still active. The
2026-09-26 update replaced them with doubled metabolism under crowding. The
distant-mate and gathered-three regressions passed, and the final full EditMode suite passed
259/259 at
[`unity-tests-20260924-150134`](../artifacts/unity-tests-20260924-150134/).
No matched Forest Edge balance run has measured the ecological effect yet, so
these remain provisional.

**Population Reinforcements phase Mutation (historical snapshot): 2026-09-23.**
At that point the experimental offer used its third slot for a repeatable `+1` individual of
the player species. A seeded placement chooses an unoccupied, passable cell for
the next phase, respects the population cap, and is retained if the expedition
restarts. This is one selection per phase decision. Phase-boundary Mutations
are free; the fixed option's separate legacy catalog cost is not charged. No
balance batch has been run. See the
[Population Reinforcements handoff](handoffs/2026-09-23-codex-population-reinforcement-mutation.md)
and its [2026-09-24 free-choice follow-up](handoffs/2026-09-24-1341-codex-free-mutations-and-near-term-desktop-migration.md).

**Fox hunting and mating energy: 2026-09-23.** Foxes now prioritize
hunting when below 75% of maximum energy and prey is available; that priority
interrupts the short Mating state and defers a reproduction outcome while the
Fox is hunting, eating, or attacking. They can seek a mate at 25% maximum
energy. Each reproduction attempt has a 45% success chance. A successful birth
costs both Fox parents 15% of maximum energy per
offspring (36 each at the authored 240-energy cap); offspring still start with
the authored 48 energy. A pair at exactly 25% when it enters Mating can still
resolve that attempt when metabolism runs first during the tick. The thresholds
and cost scale when maximum energy changes. Focused Fox tests passed 10/10.
The full Unity suite passed EditMode 256/256 and PlayMode 33/33 on 2026-09-24;
see [retained results](../artifacts/unity-tests-20260924-134934/). No matched
Forest Edge balance batch has been run, so treat these as provisional values. See the
[Fox tuning change](handoffs/2026-09-23-2054-codex-fox-hunt-mating-energy.md).

**Forest Edge hare energy behavior: 2026-09-23.** Hare energy loss now occurs
once every 10 simulation ticks. Reserve feeding uses a 6-energy trigger and a
24-energy refill target: dropping below 6 starts refilling; after reaching 24,
the hare stops eating until it falls below 6 again. Other species retain their
existing per-tick metabolism. Focused tests cover the refill cycle, metabolism
cadence, and maximum-energy upgrade behavior. The full Unity suite passed
256/256 EditMode and 33/33 PlayMode on 2026-09-24; see
[retained results](../artifacts/unity-tests-20260924-134934/).
See the [hare feeding and mating handoff](handoffs/2026-09-23-codex-hare-full-energy-mate-seeking.md).

**Field observation board pan and zoom: 2026-09-22.** The custom board now
handles captured left-drag panning and cursor-anchored wheel zoom (0.75x–4x),
with bounds based on the visible board area. This moves the behavior into
`SpeciesSimulationBoard` so shell XAML rewrites cannot silently drop it again.
Static diff checks and a scratch C# compile passed (0 errors; one external System.Net.Http version warning). Unity's live Pipeline connection was unavailable, so Editor compilation and runtime interaction still need confirmation. See the
[pan and zoom handoff](handoffs/2026-09-22-2355-codex-board-pan-zoom.md).

**Forest Edge current authored values: 2026-09-22 diagnostic batches.** Two
Clean CellSim runs used seeds 10100-10119 at 600 and 1,200 ticks on commit
`c6b3282`, with the authored 20x20 / 0.2-second Forest Edge setup, opposed-roll
combat, natural attack opportunities, and no upgrades. Foxes ended extinct in
20/20 seeds by tick 600; Hares ended extinct in 10/20 at both horizons and
averaged 0.80 / 0.85 final individuals. Mean Fox combat kills and starvation
deaths were 15.65 and 15.85 per run, while mean Hare births were 0.60 / 0.80.
Bevin reports an exploratory two-stage concept with Salty: species survival to
earn data first, where collapse without intervention is expected, then data
investment in upgrades to build a healthy, collapse-resistant environment.
These no-upgrade runs are only a first-stage pressure baseline; they do not test
upgraded play or the second-stage goal. This concept is not a finalized spec or
success gate. See the [diagnostic batch
handoff](handoffs/2026-09-22-2200-codex-forest-edge-current-values-diagnostic-batches.md)
and its raw reports and summaries. The September 21 42x20 Fox-6 comparison and
mating-fix evidence remain in the [first balance handoff](handoffs/2026-09-21-codex-forest-edge-first-balance-pass.md).
**Forest Edge balance iteration: 2026-09-21.** The first matched 600-tick
Hare/Fox pass provisionally moves Forest Edge's explicit starting Fox population
from 4 to 6. Across seeds 10100–10119 this increased direct Fox pressure: mean
Fox kills rose from 20.35 to 24.25 and Hare combat deaths rose from 20.35 to
24.25, with no Hare extinction. Keep Fox 6 as the working comparison baseline.
Fox/Hare population equality and final population are descriptive only, not the
balance score. The current direct measures are predation encounters and kills,
Hare post-contact survival, starvation pressure, and whether phase 3 remains
weakened. A paired 300-tick continuation also found the existing Tough Hide →
Threat Exposure path improved Hare post-contact survival (`pAVI` 0.39→0.46)
while increasing phase-3 starvation pressure (31.4→35.5 deaths/run). Evidence
and the rejected Hare starting-energy trial are recorded in the [first balance
handoff](handoffs/2026-09-21-codex-forest-edge-first-balance-pass.md).
The same handoff records the subsequent Fox mating-state fix: mutually ready
adjacent Foxes take mating priority over foraging and do not move while mating.
The six-phase follow-up reduced phase-3 `Mating↔Wandering` transitions from
4.8/5.2 per run to 0.2/0.2, with the live EditMode suite passing 236/236.
The latest follow-up adds a shared 24-tick reproduction cooldown after an
eligible attempt and prevents full reproduction groups from staying in
`Mating`. A 2026-09-22 review found that the cooldown blocked state selection
and vision-based mate pursuit, but a local movement fallback still pulled
cooldown or low-energy Foxes toward each other. The current correction gates
both mate-pursuit paths on both animals being eligible. A focused regression
was added and passed; the full live EditMode suite now passes 240/240. Earlier
mating coverage passed 4/4 focused and 237/237 before this correction. The
latest live five-seed
continuation recorded three Fox births, but zero aggregate `Mating` state
ticks; that telemetry/eligibility discrepancy remains a follow-up, not a
Fox/Hare population-equality target.

**Historical Hare starvation comparison: 2026-09-22.** The grass reserve increase from
10 to 11.5 did not change matched Hare starvation deaths, so it was not kept.
Using the same Forest Edge scenario, seeds 1–20, 600 ticks, and the current
Hare bite value of 5, grass reproduction `0.0015` produced 283 Hare
starvation deaths. Grass reproduction `0.0115` produced 236, a provisional
16.61% reduction. This chance-based value was superseded by the time-based
spread interval on 2026-09-24; the measured result remains historical evidence,
not a validation of the new interval. Evidence is recorded in the
[grass-starvation candidate](../artifacts/cellular-experiment-20260922-075959/report.json)
and [matched control](../artifacts/cellular-experiment-20260922-074858/report.json).

**Fox hunt / Hare escape follow-up: 2026-09-22.** Hungry Foxes now use their
visible Hare target for vision-based pursuit while in `Hunting`, instead of
falling back to wandering unless prey is already adjacent. Herbivores now enter
`Threatened` when a predator is visible, including a stationary Fox outside
attack range, and the existing escape movement chooses an available cell that
increases distance from that Fox. Focused EditMode checks cover visible-prey
hunting, stationary-threat perception, stationary-Fox escape, and the existing
Fox-pursuit fixture. This is a capability fix, not a claim that Fox and Hare
populations should match; a matched Forest Edge run is still needed to assess
its ecological effect.

**GalapagOS simulation field fit: 2026-09-23.** The desktop starts with the
legacy Lab placeholder collapsed. Forest Edge's production scenario now uses
36×20 (720 cells, about 14.3% fewer than 42×20) while retaining its authored
starting animal counts. The production asset is the gameplay source of truth;
the editor generator writes to the separate legacy `CellularSimulation`
scenario path, so changing that generated asset did not change gameplay. The
board resolves square cells from its available width and height, then applies
the user's zoom, so the default view fits the whole grid. The existing visual
acceptance assertions now expect 36×20. Unity PlayMode acceptance was not
rerun for this change.

**Saved-grid override follow-up: 2026-09-24.** The connected Editor had a
64×64 v3 saved default, which was overriding the Forest Edge production asset
and matching the square board in the screenshot. Startup now migrates older
saved settings once, preserving their other values and replacing only width
and height with the selected scenario's authored dimensions. Newer saved
defaults still retain custom dimensions. The connected Editor is not in Play
Mode, so the visual result awaits the next runtime launch.

**Saved-grid migration follow-up: 2026-09-26.** The v4 migration only handled
v3 keys; an existing v4 saved default could still override the 36×20 scenario.
Startup now migrates both v3 and v4 dimensions into v5 from the selected
scenario while preserving other saved values. A v5 custom grid remains intact.
The v4 64×64 migration regression passed 1/1 in the locally installed Unity
6000.4.6f1 Editor. The Forest Edge visual acceptance test also passed 1/1;
both the runtime preview and board snapshot reported 36×20. Its rendered field
capture is [here](../artifacts/direct-unity-grid-check-20260926/03-galapagos-simulation.png).

**S3-08 telemetry distinction: 2026-09-26.** FSM behavior ticks are recorded
before attack resolution; a successful Fox attack can set the persisted cell to
Eating afterward. The latest retained 130-tick Play Mode report shows 47 Fox
food successes with no Fox Eating decision ticks, while its reproduction funnel
classifies all 4,550 Fox candidates and reconciles. These are separate
decision/outcome measures, not contradictory counts. The Markdown report now
labels FSM decision ticks explicitly, and a focused regression asserts the
post-resolution Eating state. That targeted EditMode test passed 1/1 in the
locally installed Unity 6000.4.6f1 Editor. This does not replace the longer
six-phase reproduction evidence requested by P1-035; no balance values changed.

**Combat default reconciliation: 2026-09-18.** Opposed-roll combat is now the
default across `SpeciesSimulation`, runner construction, checkpoint restore,
the `CellSim` wrappers, and job submission. `RestoreCheckpoint` was the last
runtime API that still defaulted to legacy fixed damage. Legacy fixed damage
remains available when explicitly requested for compatibility or historical
replay. Checkpoints do not record combat mode, so a historical legacy run must
pass that mode when restoring. The focused EditMode regression passed 1/1 in an
isolated project copy; the full suite was not rerun for this correction. See
the [combat default handoff](handoffs/2026-09-18-1458-codex-opposed-roll-checkpoint-default.md)
and [test artifacts](../artifacts/legacy-combat-default-20260918/).

**Unity automation lanes and acceptance: 2026-09-18.** `CellSim` now routes
tests, visual checks, and experiments through `Auto`, `Live`, and `Clean`.
`Live` reuses this project's ready Pipeline Editor for focused feedback;
`Clean` is the reproducible acceptance lane; `Auto` selects between them and
reports busy, Safe Mode, or unreachable locked states without broad process or
lock cleanup. `Doctor` provides fast CLI and license diagnostics. Test filters
support test name, assembly, and the `Core`, `Simulation`, `Graphics`, `UI`,
`Authoring`, and `Tooling` categories. The obsolete 200 tick
`ContinuousSkipPreservesWorldHistoryAndMetricsUntilTheSameAbsoluteTick` test was
removed because it encoded a superseded final-phase decision flow. The exact
full clean command then passed EditMode 234/234 and PlayMode 28/30 with zero
failures and two expected graphics-only skips. Evidence:
[full clean test artifacts](../artifacts/unity-tests-20260918-144956/).

**Unity plugin workflow, Pipeline, and MCP: 2026-09-18.** The project routes
Unity work through [`UNITY_PLUGIN_WORKFLOWS.md`](UNITY_PLUGIN_WORKFLOWS.md).
`com.unity.pipeline` 0.7.0-exp.1 is installed and resolved. Live category tests,
a live seeded experiment, and live visual evidence were exercised; the visual
test passed 1/1 and its 1280x720 screenshot was reviewed. A clean 600 tick
experiment also produced a complete report bundle. The Codex MCP configuration
now uses the installed Unity CLI pinned to this project; the previous
`unity_mcp` user-relay entry was retired. A Codex restart or new task is needed
to load the new MCP server. One warm `UI` category run passed 10/11 while the
same tests passed in the clean full PlayMode suite, so the warm-only failure is
recorded as an Editor-state/test-isolation issue rather than a product failure.
See [`UNITY_MCP_RELAY_OPERATIONS.md`](UNITY_MCP_RELAY_OPERATIONS.md) and the
latest workflow handoff for operational details and artifact paths.

**Island Survivor retirement: 2026-09-18.** The scene/build entry, runtime
slice, dedicated tests and validator, and six Island Chores textures plus Unity
metadata were removed. Historical handoffs remain preserved. Bare-cell resolver
and board-snapshot regressions pass in the current EditMode suite. The repeated
`Terrain_01` sprite-atlas warning also appears in the pre-retirement no-graphics
baseline and is not caused by this cleanup.

**Handoff artifact validation: 2026-09-18.** The previous 102 unavailable
artifact warnings came from legacy handoffs that predate schema 1. Their
original run paths remain as historical provenance; the validator now checks
local Markdown links in those notes without requiring their machine-local
artifacts to remain present. Current schema-1 handoffs still check artifact
availability and pass with zero warnings. See the
[validation handoff](handoffs/2026-09-18-1257-codex-historical-artifact-reference-validation.md).

**Context refresh: 2026-09-18.** The pushed `ProjectMain` baseline includes the
terrain art migration, Bare-cell neighbor-mask correction and regression tests,
and simulation-shell/UI integration. Josh closed S3-01 after integration.
S3-02's player contract is complete. The original S3-03 closeout run passed its
focused End/cancel test 1/1 and no-graphics PlayMode 31/33 with 0 failures and
two expected graphics-only skips. That run also caught and fixed the prototype
scene's stale Noesis resource-dictionary reference. Tests ran in an isolated
copy because editor processes were open; reports are retained under
`artifacts/s3-03-test-results-20260918/`. The later post-retirement results
above supersede the old full-suite totals. See
[`handoffs/2026-09-18-0036-codex-s3-03-flow-recovery.md`](handoffs/2026-09-18-0036-codex-s3-03-flow-recovery.md).
Treat test claims as bounded by the retained evidence below. The S3-03 card is
already complete; see the closeout handoff for the validation record. The
separate three-option Mutation gap is tracked for S3-04, so the Sprint 3 gate
remains open.

**Terrain art standard update: 2026-09-17.** New terrain source tiles are
64x64 pixels at 64 PPU and live under `Assets/Art/Terrain/Blob/64/`; this keeps
each tile one world unit. The terrain atlas retains its scene-referenced GUID.
All 47 Grass masks generated from the 14 authored Forest
representatives are now packed by `Terrain_01.spriteatlasv2` and loaded by the
simulation runtime. `Grass_000` is full dirt and `Grass_255` is full grass.
Desert art is not present yet; its slots remain optional and no Grass art is
used as a substitute. The Island Chores atlases were removed with the retired
Island Survivor slice on 2026-09-18. At that earlier terrain-art checkpoint,
EditMode passed 251/251 and the no-graphics PlayMode run passed 28 with two
expected graphics-only skips. A graphics-capable ForestEdge
board capture also passed 1/1 and shows the Grass/Dirt tiles in context. See
[`handoffs/2026-09-17-codex-terrain-art-standard-64px.md`](handoffs/2026-09-17-codex-terrain-art-standard-64px.md).

Follow-up review found that empty Bare cells were not receiving Grass masks.
Snapshots and both board/paint-preview renderers now resolve a Grass neighbor
mask for Bare cells too, so dirt cells inside a Grass field can display their
Grass vertices. Resolver and snapshot regression tests were added and pass in
the current clean EditMode suite recorded at the top of this file.

**Historical S3 closeout context (Roadmap v2.2 at the time).** M0 and M1 are complete; Josh closed M1 on
2026-09-29 after accepting the distributed exit-gate evidence. Sprint 2 closed on 2026-09-17 with Fox telemetry as
its sole carry-over. Sprint 3 closed on 2026-09-29 after manual review. The
committed S3 cards and control card are in Trello Done; S3-05 remains in Backlog
as uncommitted stretch work. The latest merged-baseline Clean result is recorded
at the top of this file and in the closeout handoff. S3-02's contract is
complete, S3-03 is Unity-validated, and S3-04 V1 was accepted within its bounded
scope. Chrono is producing the selected Main Menu art, with further polish as
iterative work. At S3 closeout, the S4 plan still needed refinement; S4 was
subsequently activated on 2026-10-01. Retesting may be needed as later work
changes the accepted baseline. Local profile-choice save/restore was deferred beyond S4
at that kickoff; its foundation was published separately on 2026-10-14.
The GalapagOS Desktop is the canonical
player home; the standalone Lab remains a legacy/developer route.
The S3-04 working plan is now recorded. Josh confirmed that Mutation copy will
translate repeatable, predictable Stat-Line impacts into concise qualitative
player guidance, with simpler directional language when the evidence cannot
support a precise claim; raw statistics remain off the player surface. The
earlier S3 bridge used two rotating Hare Mutations plus fixed repeatable
Reinforcements. S3-04 replaced that offer with three distinct free Hare
Mutations from the five-effect pool; repeat selections at later boundaries
increase the selected level. Skip has no S3-04 reward. The earlier population
addition and its balance remain provisional historical work.

**CF-0 through CF-5 are implemented and verified.** This includes continuation
parity, boundary upgrades, the controlled preview path, phase/final Stat-Lines,
checkpoint replay, the headless schedule, and the accepted EX-010 execution.
CF-6 duration/memory measurement is stretch work rather than an S3 closeout
gate; the Windows player smoke and corrected scenario run are complete, and
graphics acceptance is complete.
The current Desktop acceptance contract directly starts Forest Edge with Hare
when the player opens Simulation. This preserves the merged Bev/Sim simulation
experience. The initial S3-01 baseline on 2026-09-17 had EditMode 247/249 with
two terrain failures and PlayMode 28/29 with one justified graphics-only skip.
The latest retained rerun is `artifacts/unity-tests-20260917-222442/`: EditMode
251/251 passed; no-graphics PlayMode 28 passed with two expected graphics-only
skips. The graphics-capable ForestEdge visual test passed 1/1 and captured the
board under `artifacts/visual-evidence-20260917-222658/`. The original terrain
failures are resolved locally. Josh subsequently closed S3-01 in Trello;
post-fix validation remains tracked separately.
Desert art remains absent. Verifying the end-to-end state/recovery route is S3
work; profile saving is scheduled for S4.
The same-world lifecycle, phase/expedition evidence meaning, initialization-only
upgrade policy, above-cap energy behavior and a versioned fresh-run fixture are
locked in the [consecutive simulation plan](CONTINUOUS_SIMULATION_FLOW_PLAN.md).

The project-owned Noesis/XAML image pipeline is now migrated: every XAML image
and icon consumer uses a `StaticResource` from
[`Assets/UI/ImageResources.xaml`](../Assets/UI/ImageResources.xaml), with
`GlobalResources.xaml` providing the shared merge and Main Menu loading the
dictionary directly. The custom simulation-board sprite atlas remains a
runtime renderer input rather than an XAML image consumer.
The game-design feature sequence is now triaged in
[GAME_FEATURE_ROADMAP_TRIAGE.md](GAME_FEATURE_ROADMAP_TRIAGE.md), starting with
the Expedition Decision Loop. S3-02 is complete: the expedition has six phases,
ten seconds of simulation time per phase, one minute total, and five upgrade
decision moments with three temporary Mutations or Skip at each, one after
each of phases 1–5. Permanent currency purchases in
the Gene Lab are Genome Upgrades that fill a Genome skill tree. Letting the
player skip for extra currency is undecided and non-blocking; if adopted, it
uses the same currency as Genome Upgrades. At Hare phase decisions, a separate
repeatable reinforcement purchase adds one Hare to the next phase for 10 Field
Data while space and Data remain; it does not use the Mutation choice. The
phase screen shows the available Data and labels free Mutation offers. S3-04
Mutation choices are free and
should be readable at a glance through an icon, identity, and evidence-backed
qualitative direction (for example, “Hunter Lv2 — better tracking and sharper
teeth”); no Stat-Line breakdown is shown. Restart is removed; End abandons
the run after confirmation, forfeits rewards if used before round 6, and Pause
remains available. Round 1 has no upgrade;
rounds 1–5 each lead to a three-Mutation choice or Skip; round 6 ends in results
with no upgrade. Victory is survival to the end of round 6; rewards use a
performance measure currently in development (not simply final population).
Extra bonus-event rewards are possible but undecided. Extinction ends
immediately as a failed run with no rewards. The Forest Edge production default
is 36×20; playable plants (including Fern) remain on hold. Older engineering
and research records still contain ten-phase/200-tick values; those are historical
configurations, not the current player contract. The upgrade direction now
separates temporary per-run Species-Simulation **Mutations** from permanent
**Genome Upgrades**, bought with currency in the Gene Lab application and
organized in a species' Genome skill tree, and
configurable **active Genome**. Mutations never enter Biome Simulations. The
active Genome is frozen at launch and applies to every population of that
species, including when it is not player-controlled. Species and Biome
Simulations use different success scorecards. Scalable balance work uses shared
capabilities and provisional Adaptation Value estimates, but requires
mode-appropriate direct-effect, matchup, and ecosystem evidence under
[`SG-005`](Studio%20Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md). The seven
existing Hare assets are provisional Mutation candidates. The implemented
Genome foundation carries stable per-species unlocked and active node IDs
through the local profile, launch request, run, checkpoint, and result. Generic
ScriptableObject authoring types resolve metadata into an immutable catalog for
the Gene Lab. The visualization-only five-node Hare and seven-node Fox maps,
their debug selector, and the player-scene provider were removed on 2026-09-17;
the simulation's seven-item Mutation catalog and Bev experimental behavior were
not changed. No production Genome map is currently assigned. Genome effects,
cost validation, economy, buying/activation actions, rule application,
versioned migration, and recovery remain planned. Named Genome loadouts are
deferred and non-blocking, and Species Mastery remains
deferred and non-gating. Skip's possible currency bonus is undecided and
non-blocking; if adopted it uses Genome Upgrade currency, with amount and limits
not yet defined. Provisional scientific-data settlement remains open pending feature-owner
approval.

The integrated branch also contains a Main Menu polish/refinement pass, broader
GalapagOS Desktop/Simulation/Lab XAML and ViewModel updates, and new concept
art for Settings, Species Collection, Expedition Setup, and Field Notes. The
Main Menu handoff is **Needs Review**: the meadow background, CRT-style boot and
desktop handoff, focus behavior, and procedural chime were verified in Unity,
but title/brand and promotion of generated art remain human decisions. The
concept images are references, not approved production assets. A focused
Settings/Collection PlayMode invocation on 2026-09-12 exited without producing
results, so it was not acceptance evidence. This gap is superseded by the
retained 2026-09-24 full suite and focused My Collection result linked in Loose
Ends P1-031.

The 2026-09-09 artifact-retention audit and cleanup removed only approved,
verified duplicates and old clean logs (about 1.35 GB). Compact summaries and
cited raw evidence remain; five semantic duplicate bundles are still review
candidates under the retention handoff.
The preview now supports phase survivor data, live/legacy upgrade choices,
same-run resume, explicit End, and manual inspection. Continuous terminal
completion is results-only; upgrades are offered at phase boundaries, and a
new expedition is an explicit next action. Continuous phases remain the
default player flow; uninterrupted single-run mode is Developer Mode-only.
Phase result/telemetry windows and ordered acquisition timing are now captured
by the runtime and report serializers. Boundary checkpoints can be copied,
restored, and resumed with deterministic runner output. The opt-in headless
schedule applies cumulative per-phase loadouts and emits the same phase
contract. EX-010 has now executed on the approved ten-phase schedule and was
accepted by Josh and Sim as bounded evidence. P3 is closed under its revised
bounded gate. EX-011 has executed successfully under its preregistered
Open Range/Deer contract; Josh accepted its narrow ordered-combination finding
on 2026-09-17. Reuse remains limited to that tested setup and does not approve
individual-upgrade effects, production balance, or generalized transfer. No
further experiment is selected.
The `bev-experimental` Coupled Hare/Fox response path is enabled in the current
preview: it applies a deterministic free counterpart legacy upgrade at the same
Expedition boundary and records both species' immutable snapshots, origins, and
trigger IDs. Unity EditMode passed 239/239 and the focused same-boundary
PlayMode test passed 1/1 on 2026-09-11. The matched Forest Edge 100-seed
three-arm check passed its direct Fox-hit-conversion gate; the factual result
and the broad PlayMode-suite limitation are recorded in the coupled-response
handoff before any mapping expansion.

Latest retained S3-01 test artifacts are
`artifacts/unity-tests-20260917-174307/EditMode-results.xml` (247 passed, 2
failed, 0 skipped; 249 total) and
`artifacts/unity-tests-20260917-174422/PlayMode-results.xml` (28 passed, 0
failed, 1 skipped; 29 total). Profile persistence and Genome asset-change
tests now pass. In that retained run, both remaining EditMode failures were
terrain checks against the absent `Assets/Art/Terrain/Blob/64` set and atlas.
The Grass family and atlas are now integrated, and focused terrain checks pass
(EditMode 3/3; PlayMode 1/1). The earlier full S3-01 rerun, before the final
population initialization fix, recorded:
EditMode 251/251, no-graphics PlayMode 28 passed with two expected
graphics-only skips, and graphics-capable PlayMode 30/30. The artifact is
`artifacts/unity-tests-20260917-220612/`. PlayMode covers the direct-start
Forest Edge/Hare route, Lab launch/return, and settings-rejection/run-
preservation path. The earlier terrain blocker is resolved locally.
The older paragraph above describes the 2026-09-17 validation snapshot. The
Trello S3-01 card was subsequently marked Done; see the [S3 control
record](Sprints/S3-control-record.md). Existing graphics acceptance evidence
is also retained at 1280×720 and 1920×1080.

The latest retained bundle after the population initialization fix, before the
empty-cell tiling follow-up, is `artifacts/unity-tests-20260917-222442/`:
EditMode 251/251 passed; no-graphics PlayMode 28 passed with two expected
graphics-only skips. The focused graphics-capable ForestEdge scene test passed
1/1 and captured setup, running, rewards, and results under
`artifacts/visual-evidence-20260917-222658/`. A separate full graphics-suite
attempt exited during Unity startup without producing a result file and is
inconclusive.

- Durable product direction: [`PROJECT_CONTEXT.md`](PROJECT_CONTEXT.md)
- Vertical-slice product brief: [`PRODUCT_BRIEF.md`](PRODUCT_BRIEF.md)
- Consecutive simulation phases — review and migration plan: [`CONTINUOUS_SIMULATION_FLOW_PLAN.md`](CONTINUOUS_SIMULATION_FLOW_PLAN.md)
- Stat-Line, predictive AI and telemetry applicability: [`CONTINUOUS_SIMULATION_EVIDENCE_IMPACT.md`](CONTINUOUS_SIMULATION_EVIDENCE_IMPACT.md)
- Simulation-flow documentation coverage: [`CONTINUOUS_SIMULATION_DOCUMENTATION_AUDIT.md`](CONTINUOUS_SIMULATION_DOCUMENTATION_AUDIT.md)
- Vertical-slice scenario, roster, and builds: [`VERTICAL_SLICE_SELECTION.md`](VERTICAL_SLICE_SELECTION.md)
- Future scientific-data economy: [`SCIENTIFIC_DATA_ECONOMY.md`](SCIENTIFIC_DATA_ECONOMY.md)
- Mutation, Genome, and balance delivery plan: [`UPGRADE_SYSTEM_DIRECTION.md`](UPGRADE_SYSTEM_DIRECTION.md)
- Coupled Hare/Fox implementation goal pack: [`COUPLED_SPECIES_RESPONSE_GOAL_PACK.md`](COUPLED_SPECIES_RESPONSE_GOAL_PACK.md)
- Mutation authoring workflow: [`UPGRADE_AUTHORING_GUIDE.md`](UPGRADE_AUTHORING_GUIDE.md)
- Hare Mutation acceptance matrix: [`UPGRADE_CATALOG_ACCEPTANCE_MATRIX.md`](UPGRADE_CATALOG_ACCEPTANCE_MATRIX.md)
- Official upgrade and ecology balance guideline: [`Studio Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md`](Studio%20Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md)
- Official Noesis image resource pipeline: [`Studio Guidelines/SG-006-NOESIS-IMAGE-RESOURCE-PIPELINE.md`](Studio%20Guidelines/SG-006-NOESIS-IMAGE-RESOURCE-PIPELINE.md)
- Upgrade-system planning concerns: [`Planning concerns/upgrade-system.md`](Planning%20concerns/upgrade-system.md)
- Main Menu, Lab, and progression delivery plan: [`MAIN_MENU_LAB_DELIVERY_PLAN.md`](MAIN_MENU_LAB_DELIVERY_PLAN.md)
- GalapagOS desktop art direction: [`../ART_STYLE_GUIDE.md`](../ART_STYLE_GUIDE.md)
- GalapagOS desktop app ecosystem: [`GALAPAGOS_DESKTOP_APP_ECOSYSTEM_PLAN.md`](GALAPAGOS_DESKTOP_APP_ECOSYSTEM_PLAN.md)
- GalapagOS desktop feature set: [`GALAPAGOS_DESKTOP_FEATURE_SET_PLAN.md`](GALAPAGOS_DESKTOP_FEATURE_SET_PLAN.md)
- Unity MVVM and GalapagOS UI architecture: [`UNITY_MVVM_ARCHITECTURE_PLAN.md`](UNITY_MVVM_ARCHITECTURE_PLAN.md)
- Unity MVVM UI contracts: [`UNITY_MVVM_UI_CONTRACTS.md`](UNITY_MVVM_UI_CONTRACTS.md)
- Sprint 0 closeout plan: [`SPRINT_0_CLOSEOUT_PLAN.md`](SPRINT_0_CLOSEOUT_PLAN.md)
- Sprint 1 authoritative execution plan: [`SPRINT_1_PLAN.md`](SPRINT_1_PLAN.md)
- Sprint Kickoff and carry-over workflow: [`SPRINT_KICKOFF_WORKFLOW.md`](SPRINT_KICKOFF_WORKFLOW.md)
- Active production roadmap and sprint plan: [`../ROADMAP.md`](../ROADMAP.md)
- Roadmap v2 review handoff: [`handoffs/2026-09-11-codex-roadmap-v2.md`](handoffs/2026-09-11-codex-roadmap-v2.md)
- Genome contract slice: [`handoffs/2026-09-12-codex-genome-contract-slice.md`](handoffs/2026-09-12-codex-genome-contract-slice.md)
- Genome catalog responsiveness: [`handoffs/2026-09-12-1756-codex-genome-catalog-responsiveness.md`](handoffs/2026-09-12-1756-codex-genome-catalog-responsiveness.md)
- Dummy Genome visualization fixture: [`handoffs/2026-09-12-1847-codex-dummy-genome-visualization-fixture.md`](handoffs/2026-09-12-1847-codex-dummy-genome-visualization-fixture.md)
- Genome tree tile visual pass: [`handoffs/2026-09-12-1938-codex-genome-tree-tile-visual-pass.md`](handoffs/2026-09-12-1938-codex-genome-tree-tile-visual-pass.md)
- Genome map swap debug fixture: [`handoffs/2026-09-12-1959-codex-genome-map-swap-debug.md`](handoffs/2026-09-12-1959-codex-genome-map-swap-debug.md)
- Genome fixture and document reconciliation: [`handoffs/2026-09-17-codex-genome-fixture-and-doc-reconciliation.md`](handoffs/2026-09-17-codex-genome-fixture-and-doc-reconciliation.md)
- Main Menu polish and refinement handoff: [`handoffs/2026-09-09-codex-main-menu-polish-first-pass.md`](handoffs/2026-09-09-codex-main-menu-polish-first-pass.md)
- Artifact retention audit: [`handoffs/2026-09-09-artifact-retention-audit.md`](handoffs/2026-09-09-artifact-retention-audit.md)
- Closed Sprint 3 closeout and M1 gate review: [`Sprints/S3-control-record.md`](Sprints/S3-control-record.md)
- S3-04 Mutation readability and bounded review plan: [`Sprints/S3-04-mutation-readability-plan.md`](Sprints/S3-04-mutation-readability-plan.md)
- S3-02 expedition contract complete: [`Sprints/S3-02-expedition-contract.md`](Sprints/S3-02-expedition-contract.md)
- S3-03 flow and recovery work: [`handoffs/2026-09-18-0036-codex-s3-03-flow-recovery.md`](handoffs/2026-09-18-0036-codex-s3-03-flow-recovery.md)
- Sprint 3 kickoff handoff: [`handoffs/2026-09-17-1613-codex-sprint-3-kickoff.md`](handoffs/2026-09-17-1613-codex-sprint-3-kickoff.md)
- Stable-but-incomplete feature action plan: [`INCOMPLETE_FEATURES_ACTION_PLAN.md`](INCOMPLETE_FEATURES_ACTION_PLAN.md)
- Proposed next work bucket: [`NEXT_WORK_BUCKET_PLAN.md`](NEXT_WORK_BUCKET_PLAN.md)
- Sprint 1 species stat-line tickets: [`SPRINT_1_SPECIES_STAT_LINE_TICKETS.md`](SPRINT_1_SPECIES_STAT_LINE_TICKETS.md)
- Fox/Rabbit art foundation lane: [`FOX_RABBIT_ART_FOUNDATION_PLAN.md`](FOX_RABBIT_ART_FOUNDATION_PLAN.md)
- Design scratchpad: [`SPECIES_IDEAS_SCRATCHPAD.md`](SPECIES_IDEAS_SCRATCHPAD.md)
- Fun/design values scratchpad: [`WHAT_IS_FUN.md`](WHAT_IS_FUN.md)
- Hunting-strategy ideation: [`Species Design/HUNTING_STRATEGIES_IDEATION.md`](Species%20Design/HUNTING_STRATEGIES_IDEATION.md)
- Reactive species/ecology arms-race plan: [`REACTIVE_SPECIES_ECOLOGY_PLAN.md`](REACTIVE_SPECIES_ECOLOGY_PLAN.md)
- Cellular simulation deferred work: [`CELLULAR_SIM_TODOS.md`](CELLULAR_SIM_TODOS.md)
- Unity simulation execution and experiment tooling: [`UNITY_SIMULATION_TOOLING.md`](UNITY_SIMULATION_TOOLING.md)
- Proposed custom report dashboard developer tooling: [`CUSTOM_REPORT_DASHBOARD_TOOLING_PLAN.md`](CUSTOM_REPORT_DASHBOARD_TOOLING_PLAN.md)
- Cellular sprite sheets and smart-tiling: [`CELLULAR_SPRITE_TILING_PLAN.md`](CELLULAR_SPRITE_TILING_PLAN.md)
- Future AI workflow skills: [`AI_WORKFLOW_SKILLS_PLAN.md`](AI_WORKFLOW_SKILLS_PLAN.md)
- AI-assisted ecology laboratory research plan: [`Research/AI_ASSISTED_ECOLOGY_LAB_RESEARCH_PLAN.md`](Research/AI_ASSISTED_ECOLOGY_LAB_RESEARCH_PLAN.md)
- Discord agent collaboration goal: [`DISCORD_AGENT_COLLABORATION_TODOS.md`](DISCORD_AGENT_COLLABORATION_TODOS.md)
- Discord message contract: [`DISCORD_AGENT_COLLABORATION_PROTOCOL.md`](DISCORD_AGENT_COLLABORATION_PROTOCOL.md)
- Legacy prototype audit: [`LEGACY_PROTOTYPE_AUDIT.md`](LEGACY_PROTOTYPE_AUDIT.md)
- Next architecture batch: [`NEXT_ARCHITECTURE_BATCH.md`](NEXT_ARCHITECTURE_BATCH.md)
- One-note-per-task handoff journal: [`handoffs/`](handoffs/)
- Handoff process: [`COLLABORATION_WORKFLOW.md`](COLLABORATION_WORKFLOW.md)
- Loose Ends ledger and review protocol: [`LOOSE_ENDS.md`](LOOSE_ENDS.md)
- Project hygiene ticket summaries: [`PROJECT_HYGIENE_TICKET_SUMMARIES.md`](PROJECT_HYGIENE_TICKET_SUMMARIES.md)

## How to get current

1. Read `PROJECT_CONTEXT.md`.
2. Read the newest handoff notes and any notes relevant to the area being changed.
   Filenames sort chronologically and include the contributor and topic.
3. Confirm the notes against the checked-out branch, `git status`, recent commits,
   code, and tests.

Create a new note instead of editing a running history here:

```powershell
.\tools\New-Handoff.cmd -Owner "your-name" -Topic "short feature name"
```

Each generated note links back here. Notes may be corrected while their work is
still local, but once shared they should normally remain historical records; add
a newer note when status or conclusions change.
