# Weekly handoff to Sim

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-11-01-codex-sim-weekly-handoff
- Owner: codex
- Branch: codex/audio-direction-and-sourcing
- Baseline commit: b95941eb
- Date: 2026-11-01
- Supersedes: none

## Summary

This week's work integrates Sim's Field Data shop/economy with the Rabbit
Genome playtest work, then improves the Gene Lab presentation so the planned
Genome branches and traits can be reviewed in-game. The feature branch includes
the latest fetched `ProjectMain` and `BevBranch` tips. Its origin was fetched
and confirmed even with the local branch before this handoff was prepared.

## Changes

- Merged Sim's `BevBranch` at `edbf4560`, including the Field Data journey,
  shop/reroll flow, settlements, permanent progression, migration handling,
  and results/UI work.
- Added Rabbit Genome playtest nodes and increased minimum litter at
  `092428d4`; showed four additional permanent-tree ideas as explicitly locked
  concepts at `9e2cba87`.
- Restored and authored twelve transparent trait sprites, then matched the
  existing Chrono rabbit art direction and card layout in
  `5cd4dc8e`, `8ea7d3ff`, `b45df5a0`, and `b95941eb`.
- Preserved the Forest Edge 400 Plants / 20 Hares / 10 Foxes setup. Spendable
  Field Data remains unavailable during the opening simulation and becomes
  available at the first shop; the opening receipts remain in the audit ledger.
- Josh reported completing the journey and said the flow made sense. The
  record does not include exact choices or a reproducible route transcript.

## Decisions and assumptions

- Faster Gestation, Fertile Droppings, High Crowding, and Increased Minimum
  Litter are concept-only in the permanent Gene Lab view. The concepts cannot
  be purchased or loaded there. Their permanent prices and loadout rules remain
  undecided.
- Playtest catalog values and node costs remain provisional. Avoidance still
  lacks a Genome-to-simulation input path; Gardener food upgrades depend on the
  food model being ready.
- This handoff records the week of October 25 through November 1, 2026. It is a
  summary of project state, not a replacement for detailed ticket handoffs.

## Validation

- After fetching `origin`, `HEAD` was 0 commits ahead and 0 behind the feature
  branch's upstream. The latest fetched `ProjectMain` and `BevBranch` commits
  were ancestors of this branch.
- Recorded in the October 29 working state: the shop-route PlayMode check
  passed 1/1, population-control EditMode checks passed 16/16, and the expanded
  Genome authoring suite passed 5/5 in an isolated Unity project copy.
- The updated illustrated-card layout has not received a new Unity capture;
  the earlier focused Gene Lab checks predate that styling change.
- No tests were rerun for this documentation and sync task.

## Risks and incomplete work

- Capture and inspect the updated Gene Lab at the target resolutions.
- Decide how permanent Genome prices, licenses, capacity, and loadouts should
  work before enabling purchase of the four concepts.
- Continue Genome implementation from
  `RABBIT_GENOME_IMPLEMENTATION_PLAN.md`; resolve the avoidance input path and
  food-model prerequisites before claiming those branches are gameplay-ready.
- Two October 10 S4 XML reports and a Unity-generated `InitTestScene` remain
  local and were not included in the push. They are not part of this week's
  implementation handoff.

## Next useful step

Have Sim review the illustrated Gene Lab direction and current locked concepts,
then capture the updated view and agree which Genome mechanics are next to
implement. Keep permanent economy rules explicit before enabling purchases.
