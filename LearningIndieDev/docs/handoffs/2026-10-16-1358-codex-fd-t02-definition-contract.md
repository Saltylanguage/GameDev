# FD-T02 - Freeze and validate starter definition

[Working state](../WORKING_STATE.md) | Status: complete

- Handoff schema: 1
- Handoff ID: 2026-10-16-1358-codex-fd-t02-definition-contract
- Owner: codex
- Branch: BevBranch
- Baseline commit: acc371a
- Date: 2026-10-16
- Depends on: FD-T01 (accepted and complete)

## Summary

Implemented the first Field Data authoring boundary and immutable resolved
definition. The starter fixture records the explicit Simulation/Shop route,
ForestEdge/Hare selection, population controls, income offers, reward cadence,
and reorder pricing. FD-T02 is complete with all five acceptance criteria
closed after the focused Editor tests passed 7/7.

## Changes

- Added `FieldDataJourneyDefinitionAsset` with validation for the exact
  five-node route, stable IDs, the six fixed population interventions, three
  source/visit-gated income offers, reorder settings, and supported signed Hare
  offer family/target mappings.
- Added a copied read-only `FieldDataJourneyDefinition` snapshot with a stable
  version string and SHA-256 fingerprint, including route, catalog and economy
  rule inputs.
- Added `FieldDataStarterDefinition.asset` under TestData. Its signed-species
  offer list remains empty until FD-T07 completes its runtime mappings.
- Pinned existing `JourneyNodeKind` values 0-5 and appended `Shop = 6`.
- Added seven Editor tests for valid resolution, route validation, bad costs and
  duplicate IDs, unsupported signed targets, asset-edit fingerprint isolation,
  serialized enum values, and preservation of the legacy 29-node map.

## Validation

- Unity 6000.4.6f1 compiled the project and ran
  `SaltyGame.EditorTests.FieldDataJourneyDefinitionTests`: **7 passed, 0
  failed, 0 skipped**. Result: `artifacts/fd-t02-editor-tests.xml`; log:
  `artifacts/fd-t02-editor-tests.log`.
- The new asset imported and resolved through the Editor tests.
- `git diff --check` passed after the final documentation updates.

## Boundaries

- This fixture is not wired to the existing map or Desktop launch. The larger
  map remains the entry; explicit test-route selection, map authoring and
  player-flow validation remain FD-T13.
- Runtime shop application of all twelve signed Hare variants remains FD-T07.
  The current resolver refuses unsupported family/attribute pairs.
- No full suite, player build, visual Desktop acceptance, headless run or
  balance comparison was performed.
- Ten pre-existing terrain `.meta` edits and earlier local documentation edits
  were preserved. Nothing was committed or pushed.
