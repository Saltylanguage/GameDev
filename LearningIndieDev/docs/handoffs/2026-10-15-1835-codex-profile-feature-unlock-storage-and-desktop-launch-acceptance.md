# profile feature unlock storage and desktop launch acceptance

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-15-1835-codex-profile-feature-unlock-storage-and-desktop-launch-acceptance
- Owner: Codex
- Branch: ProjectMain
- Baseline commit: 64076af4
- Date: 2026-10-15
- Supersedes: none

## Summary

Prepared the non-simulation profile boundary for the next work block. A profile
can now retain a generic permanent feature-availability ID across save/load,
independently of run currency and temporary Mutations. The canonical Desktop
launch test now verifies the scenario, pre-start state, and frozen Genome.

## Changes

- Added `UnlockedFeatureIds` to `ProfileSessionSnapshot` and the version 2
  PlayerPrefs profile record. Version 0 and 1 saves still load with an empty
  feature list; a save writes the version 2 shape.
- Added `Helper_ProfileSession.UnlockFeature(string)` as an idempotent profile
  mutation. No accomplishment trigger, purchase rule, or gameplay gate is
  connected to it.
- Strengthened the existing Main Menu to Desktop PlayMode route to assert
  Forest Edge, Hare, seed 10100, a ready run at tick 0, and that a later
  profile Genome edit does not change the launch snapshot.

## Decisions and assumptions

- Feature IDs are stable opaque identifiers until content names and unlock
  conditions are approved. The test ID `test-feature` is not playable content.
- Permanent unlock availability belongs to the profile. Run Field Data and
  temporary Mutations remain separate and are not written by this field.
- The Desktop launch still uses the currently selected profile and current
  setup defaults; no new setup screen or active-run resume was added.

## Validation

- `CellSim.ps1 -Command Doctor`: healthy; project closed and license ready.
- Clean PlayMode `ProfileCreationEnablesContinueAndLoadsDesktop`: 1/1 passed
  after the launch assertions were added.
- Clean PlayMode `MainMenuPlayModeTests`: 7/7 passed after profile schema v2.
- Clean EditMode `GenomeContractTests`: 11/11 passed after profile schema v2.
- Clean EditMode `ProfileSessionKeepsCurrencyProgressAndTimeSeparatePerProfile`:
  1/1 passed after the final test ID edit.
- Focused `git diff --check`: passed for the four touched source/test files.

## Risks and incomplete work

- A real application exit/relaunch smoke check and Sim's launch-input review
  are still needed before treating P1-032 or the deferred S4-04/05 save work
  as fully accepted.
- Permanent reward wallets, journey success settlement, accomplishment
  triggers, and feature gating need content/economy decisions. The generic
  profile currency remains a prototype field.
- S4 still reads Active in Trello and the control record after its October 14
  planned end. This work did not change Trello or sprint status.
- The shared checkout contains unrelated journey-map, art importer, settings,
  and test files. They were preserved outside this focused change.

## Next useful step

Review P1-032 launch inputs with Sim and smoke check save/load across a real
application exit and relaunch. Then decide S4 closeout and a proposed S5 goal
that puts simulation content and balance first; leave unspecified reward and
unlock values for the balance work block.
