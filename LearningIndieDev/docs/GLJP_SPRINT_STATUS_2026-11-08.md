# GLJP sprint status and change log

**As of:** November 9, 2026

**Scope:** GLJP-01 through GLJP-06 in the local `BevBranch` checkout. Changes are local and uncommitted; the checkout already contained substantial user and collaborator work before this status pass. No push was made.

## Short readout

GLJP-02 and GLJP-03 are Done on Trello. Population-rank progression and the Field Data shop economy are implemented and have focused coverage. GLJP-01, GLJP-04, GLJP-05 and GLJP-06 remain In Progress or Current Work. This sprint added the approved gene/Mutation distinctions, Burrowing and ancestry behavior, clearer Gene Lab and journey labels, the one-point equipment/rank experience, and Fox movement-scent pursuit defaulting off.

After Editor recovery, the full November baseline passed **762 EditMode tests** with one skip and **97 PlayMode tests** with two failures and two skips. Seed 10100 again lost its last Hare at tick 1210 with zero Plants and three Foxes. The Cave Preview test logged a Unity AI Assistant relay error and passed on a focused rerun. The later ancestry digest change passed focused Unity and standalone checks. A separate visual route run reproduced that stop and retained population/death evidence. This is route/balance evidence, not approval to tune ecology. Current captures show the Gene Lab and temporary journey choices; Bevin's visual/player acceptance is still needed. Estimated remaining work to close GLJP-01 through GLJP-05 is **about 1–2 focused engineering days**, plus owner decisions/review. GLJP-06 is redirected to the current November build by Bevin's Nov 9 direction. October comparison results are historical only; no further October-value comparisons are planned.

## Ticket status and remaining effort

| Ticket | Board status (checked Nov 8) | Implemented and verified | Still open | Estimated remaining effort |
|---|---|---|---|---|
| GLJP-01 Gene Lab and journey upgrade definitions | In Progress | Heightened Senses split into Long Vision and Keen Senses; Seed Dispersal consolidates the seed unlock; Escape Instinct replaces Threat Avoidance in the player catalog; reproduction chance, litter size and gestation remain separate. Keen Senses has its predator-detection effect. Larger Litters has three ranks and raises minimum litter at rank 3 when needed. Hare reproduction baseline is 0.5. Focused tests and full EditMode coverage pass. | Decide/implement Faster Gestation's temporary effect, minimum-litter Mutation rank limit, final prices/prerequisites and player-facing confirmation. Keep the current 5 Hare Data root prices labeled provisional. | **0.5–1 day** after the design decisions; longer if balance review changes effects. |
| GLJP-02 Population Reserves | Done | Five research ranks, rising Hare Data cost, persistent rank separate from one equipment point, and per-simulation Hare purchase allowance implemented. Starting allowance is 10 and each rank adds 5. Save migration and regression coverage included. | None known for ticket scope. Broader population-control refinement was explicitly deferred. | **0** for accepted ticket scope; future refinement excluded. |
| GLJP-03 Field Data shop economy | Done | One-time +1 Field Data per Hare purchase; three sequential income upgrades at 1.5×, 1.75×, and 2×; apply to next payouts only; duplicate species-effect shop offers removed. Idempotency, pricing, payout timing and graphics shop fixture covered. | None known for ticket scope. | **0** for accepted ticket scope. |
| GLJP-04 Burrowing | Current Work | Hares can dig once when all eight neighboring cells are empty, spending 24 energy (provisional); default capacity 3; lifespan 200 steps; creator's descendants through either parent can hide; Shared Burrows allows unrelated Hares; Expanded Burrows adds one capacity per rank up to 8. Hidden animals count toward population and persist through checkpoints. A plain brown circle marks the cell, offset so it does not obscure a Hare. Focused Unity tests and digging/hiding/emergence/collapse capture passed. | Final research prices and owner visual review. Open-ground Sprint terrain trigger/exclusion still needs its contract and wiring. The terrain dependency remains separate from the accepted Burrowing slice. | **0.25–0.5 day** for review/price application and any bounded fixes; Sprint is separate and unestimated pending design. |
| GLJP-05 Gene Lab / journey UI integration | Current Work | Research ownership and equipment state are distinguished (`OWNED · EQUIPPED` / `OWNED · UNEQUIPPED`); purchase action and acknowledgement explain equip behavior. Journey cards show `UNLOCKED IN GENE LAB` when the journey captured profile research and `THIS JOURNEY ONLY` for temporary effects. Burrowing and Seed Dispersal remain eligible after unequipping. Fresh-profile checks and inspected 720p and 1080p captures passed. Graphics-only shop fixture and current Gene Lab / journey captures pass visual test runs. | Owner review/acceptance; verify provisional costs and exclusions are clearly presented once decisions settle; close any small feedback fixes. Captures are automated/test-camera evidence, not human visual acceptance. | **0.25–0.5 day** including review fixes and final focused regression. |
| GLJP-06 route / November build diagnostic | In Progress | Ordinary Fox pursuit of Hare movement scent now defaults off per Bevin's decision; scent remains an optional experimental behavior. Approved early-tick comparisons were documented. Seed 10100 cleared two shops and stopped ten ticks into simulation 3, before shop 3. A focused visual rerun recorded the last plant consumed at tick 915 and the last Hare dying in combat at tick 1210. No ecology tuning was made. | Bevin directed on Nov 9 that November is the canonical build and October-value comparisons stop. Preserve completed October comparisons as historical evidence only. The observed depletion and starvation are documented. Which upstream ecology settings drove the collapse and whether the seed must survive without population interventions remain open. | **Not estimable yet.** Estimate the work once the November route investigation is scoped. Any balance/route changes need a separately approved experiment and may take longer. |

## Sprint changes

### Implemented in the current worktree

- Separated permanent Gene Lab research eligibility from equipping a Genome node and from a temporary journey Mutation.
- Clarified owned/equipped state and the buy-and-equip action in the Gene Lab; clarified journey-only duration on Mutation cards.
- Added/connected the approved Hare capability choices and effects: Long Vision, Keen Senses, Seed Dispersal, Escape Instinct, Crowding Tolerance, Reproductive Drive and litter progression. Open values remain provisional as marked above.
- Added the Burrowing runtime, ancestry tracking through both parents, hidden-population/checkpoint handling, Shared and Expanded Burrows, Born Burrower start grant, and the temporary brown-circle cell marker.
- Implemented Population Reserves ranks and persistent allowance; completed the Field Data income/reorder shop behavior.
- Set ordinary Fox movement-scent pursuit to off by default, while preserving the experimental toggle and other scent behavior.
- Added and repaired test fixtures around profile migration, wallet forfeiture, empty offers, shop progression, Burrowing, and visual state capture.
- Updated project handoffs and retained automated screenshots and route diagnostics under `artifacts/`.

### Not implemented, not decided, or not accepted

- Faster Gestation temporary Mutation and approved effect strength.
- Minimum-litter Mutation maximum rank; final Gene Lab prices and prerequisite paths.
- Open-ground Sprint's exact terrain trigger/effect and exclusion wiring. Broader terrain systems and Owls remain out of this ticket slice.
- Final player visual acceptance of the updated Gene Lab, journey choice and Burrow marker screens.
- A clean full PlayMode result: the current suite has one authored-route failure and two tests that require the visual runner. EditMode has one source-manifest-dependent parity skip.
- Further October-versus-November comparison (superseded by Bevin's Nov 9 direction); previously completed comparisons remain historical evidence.
- Proof that ordinary Fox scent was the sole cause of route viability changes, or a balance-approved fix for the seed-10100 plant/Hare extinction. No such causal or balance claim is made.
- Trello closeout for tickets still open, a commit, and a push.

## Validation evidence

- Full Unity suite: `artifacts/unity-tests-20261108-211957/` — EditMode **761 passed / 0 failed / 1 skipped**; PlayMode **98 passed / 1 failed / 2 skipped**.
- PlayMode failure: `SpeciesPresentationPlayModeTests.FieldDataTestRouteCompletesThroughAllShopsOnFixedSeed`; seed 10100, tick 1210. Captured test failure reports 0 plants / 1 Hare / 3 Foxes at tick 1209, then 0 / 0 / 3 at tick 1210.
- The November route visual fixture in `artifacts/visual-evidence-20261108-231609/` failed at tick 1210 and reproduced the earlier population and Hare death CSVs byte for byte. The new resource death CSV records grazing consuming the last plant at tick 915. The route visited shops 1 and 2. Shop 3 would open at tick 1800. See the [November resource diagnostic](handoffs/2026-11-07-gljp06-route-preflight.md#november-seed-10100-resource-depletion).
- Fresh-profile to Burrowing choice capture: `artifacts/visual-evidence-20261108-211649/`.
- Field Data economy/shop fixture: `artifacts/visual-evidence-20261108-211702/`.
- Burrow marker in-game fixture: `artifacts/visual-evidence-20261108-205110/`.
- Nov 9 current-source portable validation: `dotnet build tools/CellSim.Batch/CellSim.Batch.csproj --configuration Release --verbosity minimal` passed with 0 warnings/errors. `dotnet tools/CellSim.Batch/bin/Release/net8.0/CellSim.Batch.dll self-test` passed, including the Burrow hidden-state digest, sequential three-shop payout route, 18-round journey replay, and determinism/input checks. This is standalone validation, not Unity Editor verification.
- Unity-only execution of the matching Editor regression has not passed: the first clean Editor invocation exited before producing NUnit results after Unity Package Manager failed to connect to its local IPC server. Its failed Editor left `Temp/UnityLockfile`; `unity close` found no Editor associated with this project, so the stale lock was preserved. The project test wrapper now correctly refuses another run while it sees the lock. Do not present the standalone check as Unity test acceptance.
- Read the [GLJP finish plan](GLJP_FINISH_PLAN.md), [morning review and open decisions](GLJP_MORNING_REVIEW.md), and [Burrowing runtime handoff](handoffs/2026-11-08-gljp04-burrowing-runtime.md) for detailed decisions and evidence.

## Closeout estimate

For GLJP-01 through GLJP-05, plan on **1–2 focused engineering days** after prompt decisions on Faster Gestation, minimum-litter rank, costs/prerequisites, and any bounded Sprint scope. This includes implementation, focused regression, player review, small fixes and additive Trello evidence; it assumes no large balance revision. If those decisions wait, the tickets wait even if code time is available.

GLJP-06 is outside that estimate. Per Bevin's Nov 9 direction, November is the canonical build; stop October-value comparisons and retain prior October results as historical evidence. Continue with current-November route behavior when Unity can be safely recovered. Any route tuning still requires separately approved evidence.


## November build direction — November 9, 2026

Bevin set the November build as the canonical baseline. Stop further October-value testing and do not use October values to guide current tuning. Preserve already-completed October comparisons as historical records. GLJP-06 now focuses on the current November route failure and route behavior; no causal conclusion or balance change is approved. The recovered Editor reproduced the November route failure. The Trello GLJP-06 card remains In Progress with this direction recorded in a new comment.
