# Context for Sim while Josh is away

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-11-01-codex-sim-weekly-handoff
- Owner: codex
- Branch: codex/audio-direction-and-sourcing
- Baseline commit: 6d66b4d8
- Date: 2026-11-01
- Supersedes: none

## Summary

Josh will be away for a week and won't be available to address these open items
or work on upcoming changes during that time. This note provides context on
his recent work so it is easier to distinguish completed work, current
implementation, and undecided ideas.

This note does not transfer ownership of Josh's work, assign its open items to
Sim, or change Sim's priorities. Sim can continue his own work as planned; the
items below are context, not a task list. If future work runs into a question
that needs Josh's input, record it for his return rather than assuming
responsibility for deciding or implementing it on his behalf.

The pushed `codex/audio-direction-and-sourcing` branch at `6d66b4d8` contains
the recent work and the latest fetched `ProjectMain` and `BevBranch` tips,
including Sim's Field Data shop/economy work. The branch was fetched and
confirmed even with origin at handoff time.

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

- Settled direction to preserve during coverage: Forest Edge starts at 400
  Plants / 20 Hares / 10 Foxes; Field Data receipts from the opening
  simulation remain auditable, but the spendable wallet opens at the first
  shop. Josh manually completed the journey and said the flow made sense.
- Keep the four new permanent Gene Lab concepts visibly locked until their
  permanent prices and loadout rules are explicitly decided.
- Faster Gestation, Fertile Droppings, High Crowding, and Increased Minimum
  Litter are concept-only in the permanent Gene Lab view. The concepts cannot
  be purchased or loaded there. Their permanent prices and loadout rules remain
  undecided.
- Playtest catalog values and node costs remain provisional. Avoidance still
  lacks a Genome-to-simulation input path; Gardener food upgrades depend on the
  food model being ready.
- This handoff covers the week beginning November 1, 2026. It supplements the
  detailed ticket and feature handoffs; those remain the source for exact
  acceptance evidence.

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

- The updated Gene Lab still needs a Unity capture at target resolutions; the
  earlier focused checks predate the illustrated-card styling.
- Permanent Genome prices, licenses, capacity, and loadouts for the four
  concepts are undecided, so those concepts remain locked.
- The Genome implementation plan remains at
  `RABBIT_GENOME_IMPLEMENTATION_PLAN.md`. Avoidance lacks a Genome-to-simulation
  input path, and Gardener food upgrades depend on the food model. Neither
  branch should be described as gameplay-ready yet.

## Next useful step

This note assigns no work to Sim. The items above are context only; Sim can
follow his existing priorities. Josh can resume the unresolved Genome and UI
items when he returns, using this note and the linked plan as context.
