# M2-04 Hare response implementation

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-11-14-1204-codex-m2-04-hare-response-implementation
- Owner: codex
- Branch: codex/s6-predator-first
- Baseline commit: f4c1ee34
- Date: 2026-11-14
- Supersedes: none

## Summary

Local implementation pass for M2-04. The three Forest Edge opening Mutation
choices now represent avoidance (Escape Instinct), attack survival (Tough Hide),
and more successful reproduction attempts (Reproductive Drive). A fresh profile
can stack each response through its existing cap on the introductory route. The Quick Escape player-description exception
seen in the November 14 baseline is addressed in code. This is not M2-04
acceptance: Unity checks, legitimate journeys, matched comparisons, and player
review are reserved for the EOD validation and later S6 work.

## Changes

- `Assets/Data/ProductionData/CellularSimulation/Species/Journey/{FernGladeStart,ForestEdgeStart,PineVergeStart}.asset` now offer the same three introductory responses. Their GUIDs and serialized fields are unchanged.
- `Assets/Scripts/Game/Species/SpeciesUpgrade.cs` uses those responses for the first phase offer when no earlier Mutation was chosen.
- `Assets/Scripts/Game/Species/SpeciesProgression.cs` allows all eligible ranks of the three without research only in the Forest Edge journey; existing level and reproduction-chance caps still apply. Gene Lab research enables these choices in other routes and Field Data. `Assets/Scripts/Game/Presentation/SpeciesSimulationPreview.cs` selects this journey rule and uses the same eligibility check for offers and claims. `Assets/Scripts/Game/Simulation/FieldDataJourneySession.cs` shares the eligibility check, but the separate Field Data route does not enable introductory access.
- `Assets/Scripts/Game/Presentation/SpeciesSimulationPreview.cs` explains each opening effect and its current ecological obligation, and handles Quick Escape in the player description switch.
- `Assets/Scripts/Game/Species/FieldDataPermanentProgressionDefinition.cs` maps Quick Escape to Escape Instinct research. The three affected Gene Lab node descriptions are synchronized with their authored assets in `Assets/Data/TestData/Genome/HareGenome/{EscapeInstinct,GuardedBurrow,ReproductiveDrive}.asset`.
- Focused assertions were added or updated in `Assets/Tests/Runtime/{FieldDataPermanentProgressionTests,SpeciesDomainTests}.cs` and `Assets/Tests/PlayMode/{GalapagOSVisualAcceptanceTests,SpeciesPresentationPlayModeTests}.cs`.

## Decisions and assumptions

- This uses existing signed-stat and pre-contact avoidance rules. No Fox, calorie, price, or production Mutation value changed. The three response Mutations are introductory journey entitlements, not permanent Genome unlocks; research ownership and equipped Genome effects remain separate.
- Reproductive Drive raises success chance only for already eligible attempts; it does not accelerate gestation. The choice text says so. Tough Hide and additional births can increase food demand; these are obligations to observe, not proven balance costs for this source revision.
- Escape Instinct was selected over general movement for the opening avoidance response because its pre-contact rule directly avoids some Fox attacks. Existing November panels did not establish a viable avoidance or reproduction value. Quick Escape remains a researched candidate and is now display-safe.

## Validation

- Read-only source and caller audit completed. `git diff --check` reported no whitespace errors; it emitted line-ending warnings for three edited Unity assets and an unrelated pre-existing roadmap file.
- Unity compilation, EditMode, PlayMode, visual inspection, and seeded simulation were **not run** in this implementation task, per the EOD validation plan. The November 14 pre-change full-suite baseline was EditMode 764 passed / 8 failed / 1 skipped and PlayMode 90 passed / 6 failed / 7 skipped; five PlayMode failures shared the Quick Escape description exception.

## Risks and incomplete work

- The choice text communicates the rule, but direct player-visible counts of avoided Fox attacks, prevented damage, and replacement births are still needed to establish observable contribution. This overlaps M2-07/08; do not call M2-04 complete based on descriptions or loadout labels.
- The three responses remain eligible for later phase decisions on a fresh profile, up to their existing caps. Reproductive Drive becomes invalid at 100% reproduction chance (or its level cap); at that point a phase may show fewer than three Mutations if the profile has not researched other options. Review that cap case against the six-round offer contract in the EOD pass.
- Earlier 100-seed work found Tough Hide promising but did not confirm avoidance or reproduction viability. The November carcass and node-reset findings also require new matched evidence; no production tuning is approved here.
- The edited Gene Lab descriptions change the catalog fingerprint. Saved research is keyed by stable node IDs, but profile load/relaunch and authored-map validation still need the planned Unity pass. The whole implementation is local and uncommitted; unrelated art, XAML, documentation, scenes, and artifacts in the shared tree were not edited.

## Next useful step

At EOD, run targeted `FieldDataPermanentProgressionTests`,
`SpeciesDomainTests.FirstPhaseMutationOfferIncludesEachHarePathAndRespectsCaps`,
`FieldDataPermanentProgressionAssetTests`,
`GalapagOSVisualAcceptanceTests.FreshProfileCanChooseAnOpeningResponse`,
`GalapagOSVisualAcceptanceTests.OwnedResearchKeepsFurtherToughHideRanksAvailable`,
`SpeciesPresentationPlayModeTests.QuickEscapeHasAPlayerFacingMutationDescription`,
and `SpeciesPresentationPlayModeTests.JourneyNodeRewardsCarryPopulationIntoFreshBiome`
through `CellSim.ps1 Test` with `-TestFilter` and the appropriate mode. Then run
the agreed full suites and representative authored route checks once, triage
against the retained baseline, and hand the source/seed/action schedule to the
matched comparison and player review. Keep M2-04 open until the three strategies
are observed and an accept/revise decision is recorded.
