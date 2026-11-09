# GLJP-01 capability pass

November 8, 2026. This is a local, uncommitted implementation checkpoint for [GLJP-01](https://trello.com/c/dR2z6ENf/149-gljp-01-replace-hare-genome-stat-nodes-with-researched-journey-unlocks). It records the newest accepted Gene Lab and Mutation decisions, their implementation, validation, and remaining work. It does not close the Trello card or publish the branch.

## Implemented

- Heightened Senses continues to unlock separate Long Vision and Keen Senses Mutations. Long Vision increases general vision range. Keen Senses adds predator-detection range without changing food-target tracking persistence.
- Threat Avoidance is presented as Escape Instinct; Seed Dispersal remains the only seed-planting unlock and maps legacy Fertile Droppings research to the same capability.
- The production Hare reproduction-chance baseline is 0.5. Reproductive Drive remains a distinct chance increase.
- Larger Litters adds one maximum litter per rank and is capped at rank 3. Rank 3 also raises minimum litter to at least 2 when needed; the rank-3 applied upgrade snapshot records both modifiers. Minimum-litter research remains a separate effect.
- The production design, SDD, inventory, and morning-review ledger now reflect these decisions.

## Validation

- Full EditMode: **748 total, 747 passed, 0 failed, 1 skipped**. The skipped test requires a retained parity manifest. Artifact: `artifacts/unity-tests-20261108-110527/EditMode-results.json`.
- New and relevant EditMode checks present in that result: `KeenSensesDetectsPredatorsOneTileBeyondVisionRange`, `ThirdLargerLitterRankRaisesMinimumToTwoAndIsCapped`, `LitterMutationRanksHaveApprovedCaps`, and `ProductionHareUsesDoubledEnergyAndLitterMatingRules`.
- Focused graphics-capable PlayMode offer check: **1/1 passed** for `HeightenedSensesOffersBothMutationsInFieldData`. Artifact: `artifacts/unity-tests-20261108-110628/PlayMode-results.json`.
- Unity CLI was updated to `1.0.0-beta.17` after Bevin approved the update. The live Unity Editor recompiled the latest source with no errors before the final validation run.

## Still open on GLJP-01

- Define a rank cap for Increased Minimum Litter. It remains uncapped until Bevin defines the intended limit.
- Faster Gestation remains a locked concept until its journey Mutation and effect are approved and implemented.
- Final prices and prerequisite mapping remain unresolved; current new roots are labeled provisional test values.
- The focused offer check verifies the two choices and eligibility, not player visual acceptance or balance. Mutation strengths remain provisional.

## PlayMode regression follow-up

- A full PlayMode run after the approved 0.5 Hare reproduction baseline reported **93 passed / 4 failed / 2 skipped**. Two failures were assertions tied to the former 1.0 baseline; one wallet-forfeiture fixture stopped at the first 600-tick boundary before the journey's natural-failure state; and the fixed-seed success route became ecologically extinct. The Fox-offer cap test also encoded the old baseline.
- Updated the two baseline-sensitive assertions and extended the wallet-forfeiture fixture to allow the configured route to finish. Focused reruns passed for `ApplicableHerbivoreSkillsRetainLevelsAndAcquisitionsAcrossDecisionBoundaries` and `FoxOfferKeepsBroodDriveWhenItsHareResponseIsNotCapped`.
- The extended wallet-forfeiture test was still running after more than six minutes. Unity's test cancellation request timed out after 60 seconds, and the connected Unity Editor process was CPU-bound and reported unresponsive. Bevin authorized closing the frozen Editor; the exact `LearningIndieDev` Editor process was stopped. No result artifact was produced for this attempt. A restarted Editor briefly reconnected, but the focused rerun request stalled and the Editor process exited before returning a test result. The attempt is **inconclusive**, not a pass or failure.
- The fixed-seed route completion failure remains unresolved and must not be fixed by tuning ecology values without a separate approved balance decision. Full PlayMode has not been rerun after the assertion edits.

No push was made.
