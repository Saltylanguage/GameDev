# Automated journey rewards and Gene Lab unlock

[Working state](../WORKING_STATE.md) | Status: complete

- Handoff schema: 1
- Handoff ID: 2026-10-20-1205-codex-automated-journey-rewards-and-gene-lab-unlock
- Owner: Codex
- Branch: ProjectMain
- Baseline commit: 6ffb7b10
- Date: 2026-10-20
- Supersedes: [October 17 Gene Lab handoff](2026-10-17-1719-codex-fertile-droppings-gene-lab-offer.md) for the unverified payout/unlock status

## Summary

The journey completion, permanent Data payout, profile persistence, and
three-success Gene Lab gate are now covered by a passing automated PlayMode
test through the Main Menu and Desktop profile route. Three successful
journeys earn the Data used to buy tree access and Fertile Droppings. The test
does not seed currencies or completion counts to reach the gate.

## Changes

- Added `ProfileJourneyRewardsPayOncePersistAndUnlockGeneLab` to
  `Assets/Tests/PlayMode/LabPlayModeTests.cs`, using the existing profile save
  backup/restore fixture.
- Covers early stopping and extinction paying nothing, no premature payout
  at phase/cycle boundaries, one of each permanent Data and one completion
  per success, distinct journey IDs, duplicate rejection after profile reload,
  the locked-to-unlocked Gene Lab transition, and buying the production offer
  with earned Data.
- Recreates Main Menu and Desktop scene components to check actual saved
  balances and allocation. A subsequent failed journey preserves the earlier
  rewards, permanent tree access, and active node.
- Updated Working State and the prerequisite checklist. No production runtime
  behavior or serialized scene/asset definition changed in this pass.

## Decisions and assumptions

- This is an integration fixture: fixed seed 10100, an 8x8 map, 20 plants,
  four Hares, no Foxes, and one tick per phase. It follows both simulation
  nodes (`first-cycle` and `burrow-study`) through all 12 phases, takes the
  opening Mutation and `seedfall` condition, and skips optional phase Mutations.
  Public production controllers advance the simulation and trigger its normal
  completion event; success is not manufactured by calling the profile reward
  method.
- Extinction is forced by removing the player population from the live test
  grid, then advancing a real simulation tick. Duplicate settlement is the
  sole deliberate call to `RecordSuccessfulJourney`, and it must return false.
- The fixture shortens time to test reward wiring independently of ecological
  balance. It does not establish a 600-tick ecology margin or player enjoyment.

## Validation

- Live Unity 6000.4.6f1 script recompile: completed, zero compilation errors.
- Focused new PlayMode test: 1/1 passed (4.81s).
- Final `LabPlayModeTests` group: **10/10 passed**, zero failures or skips
  (13.71s); includes the new test and the existing purchase/save/reset/frozen
  next-journey test.
- Retained evidence under `artifacts/unity-tests-20261020-journey-profile/`:
  `compile-results.json`, `focused-results.json`, and `PlayMode-results.json`.
- The first focused attempt exposed a test timing assumption: live Main Menu
  transitions animate before loading Desktop. Replaced the two-frame assumption
  with a bounded scene wait. The initial failure is retained in
  `initial-failure.json`; no production code repair was needed.
- Used the live Unity MCP `recompile` and `run_tests` commands against the
  matching project, with filter `ProfileJourneyRewardsPayOncePersistAndUnlockGeneLab`
  and then `LabPlayModeTests`, mode `playmode`, filter type `testName`.
  The project wrapper's CLI discovery reported the locked Editor unreachable,
  while direct MCP `editor_status` confirmed this exact project ready. Elevated
  Doctor confirmed healthy licensing; the installed CLI is 1.0.0-beta.6 and
  does not expose the skill's newer `self-update` command. No tooling/package
  upgrade was made.

## Risks and incomplete work

- This work remains local on ProjectMain alongside the existing uncommitted
  Fertile Droppings/Gene Lab work and unrelated local edits.
- Profile persistence was exercised by `Reload` and scene reconstruction in
  one Editor process. An OS-level application relaunch, a standalone build,
  human play acceptance, and broad ecology/balance runs were not performed.
- The general permanent Genome effect mapping and multi-node Gene Lab
  selection remain open; this task only closes the automated reward/unlock
  verification step.

## Next useful step

Implement the explicit permanent Genome-to-species-rule mapping and selectable
multiple Gene Lab nodes, then agree Efficient Digestion's offer and effect
before exposing a second production node.
