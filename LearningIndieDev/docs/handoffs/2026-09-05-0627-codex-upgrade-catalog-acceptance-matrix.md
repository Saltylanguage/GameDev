# Upgrade catalog acceptance matrix

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: Codex
- Branch: NF/ConsecutiveRuns
- Baseline commit: f1a923ec
- Date: 2026-09-05

## Summary

The first production upgrade catalog now has an explicit, row-by-row
acceptance matrix. The matrix separates authoring correctness from gameplay
mechanic evidence and balance approval, so a valid asset is not mistaken for a
balanced player reward.

## Changes

- Added `docs/UPGRADE_CATALOG_ACCEPTANCE_MATRIX.md` with all seven production
  Hare upgrades, exact modifier order and values, costs, lifecycle rules,
  evidence needs, and open decisions.
- Expanded `SpeciesUpgradeAssetCatalogTests` to verify every production row,
  including scope, relationship lists, modifier values, and whether the
  snapshot can apply after a run starts.
- Fixed strict JSON output when Stat-Line is omitted and added a bounded report
  read retry after Unity batch exit. The fix was found while running the
  EX-010-shaped schedule smoke check.
- Linked the matrix from the upgrade direction, authoring guide, and working
  state; marked S2.2A complete in the S2 control record.
- Corrected the stale S2 risk wording so it reflects the implemented
  same-world boundary upgrade/reward path while keeping the final M1 review
  open.

## Decisions and assumptions

- The seven production assets remain starting hypotheses for balance; contract
  acceptance does not promote any balance claim.
- Seed Pouches is launch-only because its modifiers describe initial energy and
  starting reserve. Making it a live boundary or newborn effect would require a
  separate design decision.
- EX-010 keeps its historical research fixtures (`faster-movement` and
  `crowding-tolerance`) and is not replaced by production assets through this
  matrix work.
- The old card wording asked for spatial and conditional examples, but the
  approved V1 contract is numeric signed/additive only. Those effect types are
  explicitly deferred to a future contract package.
- M1 closeout remains deferred until the end of the current pre-EX-010 work
  block.

## Validation

- `git diff --check` passed.
- Elevated Unity EditMode suite passed: **210/210**, artifact
  `artifacts/unity-tests-20260905-064231/EditMode-results.xml`.
- EX-010-shaped schedule smoke passed after the serializer fix; the strict
  artifact bundle is `VALID` at
  `artifacts/cellular-experiment-20260905-064045` for seeds 106–107 and three
  40-tick phases. It remains smoke validation, not EX-010 evidence.

## Risks and incomplete work

- Mechanic and balance evidence for the seven rows is still provisional.
- EX-010 still needs its authored schedule, phase/stat-line review, parity
  gate, and human approval before execution.
- The Trello S2.2 card is now synchronized with this repository result: its
  description records the approved V1 scope, evidence, and non-goals, and it
  is in `✅ Done`.

## Next useful step

Complete the EX-010-specific schedule and human approval gate. Keep M1
closeout as the final review after this block.
