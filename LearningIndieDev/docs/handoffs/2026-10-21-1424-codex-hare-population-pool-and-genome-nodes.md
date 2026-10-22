# hare-population-pool-and-genome-nodes

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-21-1424-codex-hare-population-pool-and-genome-nodes
- Owner: codex
- Branch: codex/audio-direction-and-sourcing
- Baseline commit: 6ffb7b10
- Date: 2026-10-21
- Supersedes: none

## Summary

Implemented Josh's two population Genome effects and shared replenishment
capacity rule. Food exhaustion from unchecked growth remains possible and is
now recorded as an intended failure condition. The new nodes are playable in
the existing Gene Lab POC. No new ecology panel or production rebalance ran.

[Contract and design decisions](../Research/2026-10-21-Hare-population-pool-and-Genome-nodes.md).

## Changes

- Registry v7 adds integer `population.starting` and `population.pool-capacity`
  targets, explicitly separated from animal stat/Mutation application.
- Genome composition applies starting bonuses to fresh authored counts before
  seeding. Runtime pool capacity is frozen with the active Genome and retained
  by checkpoint restore. Validation rejects invalid capacity and unsupported
  probability-only starting-count bonuses before launch.
- Runner/manager/helper population grants clamp ordinary player additions,
  return actual added counts, and expose an explicit special-rule bypass.
  Non-player additions stay independent. Placement remains atomic and checked.
- Buy Hare/Mutation reinforcement availability follows the same rule. Full
  pool disables purchase without charging; the phase control states the limit.
  Node descriptions distinguish food demand from replenishment capacity.
- Added Founding Hares (+5) and Population Reserves (+10) as Warren roots in
  the explicit TestData RabbitSelectionPOC map. Unity AssetDatabase generated
  the assets/metas. Both follow the existing provisional 1 Data / 2-point cost.
  The production Hare map remains separate. The existing scenario-asset utility
  has an idempotent menu/batch command to create these offers without replacing
  existing authored values.
- Report schema 40 records pool capacity and actual per-species boundary
  additions; purchase audits distinguish `population-pool-full`. Portable
  Genome nodes preserve the same registered modifiers and values.
- Added focused domain/portable/UI verification and updated shared context.

## Decisions and assumptions

- Latest human direction is base pool 50 and +10 per active capacity node.
  This supersedes the earlier 20-base/+5-capacity discussion. Starting bonus
  remains +5 and does not also increase capacity.
- The pool governs refresh/restoration/population grants. It is not a cull at
  Mutation rounds or journey map nodes, a birth cap, or an automatic refill.
  Surplus Hares can still exhaust food. Special-rule overrides are explicit.
- Existing profile allocation/refund/license rules and permanent unlocks remain
  in effect. Values, names, placement and pricing are still playtest content.
- Do not rewrite the prior schema-39 reports or treat uncontrolled extinction
  alone as a defect. The new design record supersedes that interpretation of
  the historical AI recommendation; raw experiment observations are unchanged.

## Validation

- Final focused EditMode: **112/112 passed**, covering Genome contracts,
  population research input/reporting, authoring, manager boundaries and
  Mutation compatibility. XML:
  `artifacts/unity-tests-20261021-143501/EditMode-results.xml`.
- Final actual Desktop route: **1/1 passed at each resolution**, 1280×720 and
  1920×1080. XMLs:
  `artifacts/visual-evidence-20261021-143253/PlayMode-results.xml` and
  `artifacts/visual-evidence-20261021-143348/PlayMode-results.xml`.
  Four inspected captures are in `artifacts/population-genome-visuals-720p/`
  and `artifacts/population-genome-visuals-1080p/` (Gene Lab / full pool).
- Final .NET build: **0 warnings/errors**. Portable regression self-test:
  **passed**. Scoped `git diff --check` passes. Test/capture/source hashes and
  command identities are in
  `artifacts/population-genome-verification-20261021-01/verification.json`.
- Development checks exposed test-fixture species-ID/funding/frame-refresh
  errors and an identity-only snapshot compatibility regression, all corrected.
  A later missing LINQ import in report accounting was fixed and the shared
  .NET build passes with zero warnings/errors. Portable self-test passes.
- The failed compiler run left a stale project lock. Process inspection proved
  only CLI connector servers remained; none was terminated and no lock was
  deleted. An idempotent Unity batch reopen recovered the project normally.
- Capture configuration needed explicit supported visual environment values
  because the Clean launch lost its Temp request. Earlier passing runs without
  images are interaction evidence only. Capture fixture corrections select the
  simulation camera and close the map overlay before photographing the phase
  controls. Retained failed attempts remain under artifacts/visual-evidence-*.

## Risks and incomplete work

- New refresh/random-event content is not authored here. Future population
  grants must use the common gate and actual-added count; arbitrary mid-phase
  event timing remains part of that content's implementation.
- Starting-population modifiers currently require explicit starting counts;
  probability-only scenarios are rejected rather than given an invented base.
- No human fun/balance acceptance or new ecology comparison was performed.
- Local shared checkout already contains earlier Genome, simulation, UI and
  art changes. This task did not commit, push, switch branches or update Trello.

## Next useful step

Design the player-facing refresh/event choices and food-pressure feedback that
make overpopulation decisions readable. Use the shared pool rule for ordinary
grants, declare exceptions explicitly, and agree a scoped balance comparison
before running further ecology evidence.
