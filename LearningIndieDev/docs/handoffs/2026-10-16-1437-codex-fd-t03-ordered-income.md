# FD-T03 - Calculate ordered income with a complete breakdown

[Working state](../WORKING_STATE.md) | Status: complete

- Handoff schema: 1
- Handoff ID: 2026-10-16-1437-codex-fd-t03-ordered-income
- Owner: codex
- Branch: BevBranch
- Baseline commit: acc371a
- Date: 2026-10-16
- Depends on: FD-T02 (complete)

## Summary

Implemented deterministic population-based Field Data calculation in current
modifier-instance order. The calculator returns an immutable breakdown and
integer award without reading or changing a wallet. FD-T03 is complete with all
five Trello criteria checked.

## Changes

- Added `FieldDataIncome` and immutable modifier-instance, breakdown-step and
  payout-receipt types under the Simulation domain.
- Applied the approved +1 FD per Hare, ×1.5 and ×2 operations in list order,
  retained decimal intermediate values and floored only the final award.
- Kept the 10-FD node clear reward outside the population calculator.
- Reported invalid operation/identity/population data, decimal overflow and
  awards beyond the existing integer wallet range as errors without returning
  a partial receipt or inventing a gameplay cap.
- Renamed the operation value `AddPerSelectedPopulation` to `AddPerHare` while
  preserving its serialized numeric value 0. Added an enum-value regression
  assertion alongside the definition tests.
- Added eight focused Edit Mode tests for order, fractional arithmetic, empty
  populations, rejection paths, and frozen prior receipts.

## Validation

- Unity `SaltyGame.EditorTests.FieldDataIncomeTests`: **8 passed, 0 failed, 0
  skipped**. Result:
  `artifacts/unity-tests-20261016-143509/EditMode-results.xml`.
- Unity `SaltyGame.EditorTests.FieldDataJourneyDefinitionTests`: **7 passed, 0
  failed, 0 skipped**, including serialized income-operation and journey-node
  enum values. Result:
  `artifacts/unity-tests-20261016-143714/EditMode-results.xml`.
- Targeted `git diff --check` passed for the T03 implementation and updated
  Field Data/S5 documentation.
- Trello FD-T03 is Done with all five acceptance criteria checked. Live board
  read-back: 16 Done, 0 In Progress, 13 Current Work, 65 acceptance items open.

## Boundaries

- FD-T05 still owns once-only wallet settlement. This calculator does not
  settle income, node-clear bonuses, or purchases.
- Offer acquisition, source timing, reordering and UI remain later ticket work.
  The continuous journey remains FD-T04; Desktop/map integration remains
  FD-T13.
- No full suite, player build, visual/UI flow, headless replay, or balance
  comparison was run.
- Changes are local and uncommitted on `BevBranch`; nothing was pushed.
- Ten pre-existing terrain `.meta` edits and the previous FD-T01/T02 work were
  preserved.
