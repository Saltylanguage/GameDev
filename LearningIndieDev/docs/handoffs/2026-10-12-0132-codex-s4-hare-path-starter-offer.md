# S4 Hare Path Starter Offer

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-12-0132-codex-s4-hare-path-starter-offer
- Owner: codex
- Branch: codex/s4-gestation-checkpoint-20261010
- Baseline commit: 5f971011
- Date: 2026-10-12
- Supersedes: none

## Summary

The first Hare offer now exposes all three intended strategic paths. The canonical Forest Edge Desktop reward offers one starter for Trailblazer, Warren, and Gardeners, and an initial phase offer with no continuing upgrade uses the same one-per-path rule. Later phase offers retain the selected starter's partner-first behavior. This resolves the earlier 12/20 nominal Trailblazer access finding as an offer-access issue without changing upgrade effects or the balance experiment.

## Changes

- Added a one-per-path starter offer helper to `Assets/Scripts/Game/Species/SpeciesUpgrade.cs`, used for a first phase offer with no continuing upgrade. Seed-based ordering and starter choice remain deterministic; capped path starters fall back to other eligible upgrades.
- Updated `Assets/Data/ProductionData/CellularSimulation/Journey/ForestEdgeStart.asset` to offer `faster-movement`, `tough-hide`, and `efficient-digestion`.
- Updated `docs/WORKING_STATE.md` and the S4 control record with Josh's opening-access decision.

## Decisions and assumptions

- Josh approved the rule that the first three-card Hare offer has one starter from Trailblazer, Warren, and Gardeners. The authored Forest Edge opening uses Movement, Tough Hide, and Efficient Digestion as those three starters.
- When a player selects a starter, existing continuation behavior offers its partner at the next eligible phase boundary. Later offers retain their existing rotation.
- This changes offer access, not upgrade balance or the earlier spatial replay. The 12/20 finding remains an accurate historical result for the old rotating first-offer behavior.

## Validation

- Static review confirmed the new helper is limited to an initial phase offer without a continuing upgrade, and that the existing partner-first branch remains in place.
- Reviewed the existing PlayMode journey reward and offer-flow coverage. No Unity tests were run in this work block.
- `git diff --check` passed.

## Risks and incomplete work

- The one-per-path rule applies when all three paths have at least one eligible starter. If both starters for a path are capped, the offer fills from other eligible Hare upgrades.
- Unity execution and visual review of the opening reward remain outstanding.

## Next useful step

Review the opening reward and offer progression in the Desktop route during the next Unity review. Keep the earlier spatial report's measurements tied to its old offer flow and current crowding rule.
