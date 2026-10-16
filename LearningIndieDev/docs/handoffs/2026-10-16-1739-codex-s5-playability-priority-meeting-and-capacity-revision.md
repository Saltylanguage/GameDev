# s5-playability-priority-meeting-and-capacity-revision

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-16-1739-codex-s5-playability-priority-meeting-and-capacity-revision
- Owner: codex
- Branch: ProjectMain
- Baseline commit: acc371ac
- Date: 2026-10-16
- Supersedes: none

## Summary

Recorded Josh's stronger S5 playability goal, removed the protected review
reserve from the current capacity model, and kept the 16 Field Data cards in
place for tonight's priority meeting with Sim. The first quality gate is a
polished two-simulation/one-shop route; the final S5 ticket commitment awaits
team review.

## Changes

- Updated the [S5 playability goalpost](../Sprints/S5-playability-goalpost-2026-10-16.md)
  as a meeting draft with observable simulation, player-choice and review
  evidence, plus a dependency-aware discussion order.
- Updated the [repository S5 control](../Sprints/S5-control-record.md),
  [roadmap](../../ROADMAP.md), generated `ROADMAP.html`,
  [Working State](../WORKING_STATE.md), [Project Context](../PROJECT_CONTEXT.md),
  [Field Data ticket index](../FIELD_DATA_TICKET_DRAFT.md),
  [alignment](../FIELD_DATA_AND_SIMULATION_ALIGNMENT.md) and
  [Loose Ends](../LOOSE_ENDS.md) to reflect the current decision.
- Updated the [Trello S5 control](https://trello.com/c/hqQczLQ9/135-sprint-5-control-record-field-data-journey-implementation)
  name, product direction, capacity and current board snapshot. No ticket was
  moved, renamed, rescoped or assigned.

## Decisions and assumptions

- Josh removed the 16h protected integration/review reserve. Review uses
  separate dedicated time outside sprint effort. The conservative sprint
  planning floor is now 80h, against 57–95h in the published Field Data
  estimates: a 15h upper overage before unestimated ecology/playability work.
- Keep the completed FD-T01–03 evidence and FD-T04's ongoing 18-round session
  work. A two-simulation route is the first playability check, not a decision
  to discard the third simulation or second shop. All 16 cards remain visible
  for Sim's review. Owners, final priorities, any changed acceptance and a
  simulation diagnosis task are pending that meeting.

## Validation

- Re-read the Trello S5 control after update: Active, revised capacity, 15h
  overage, and current goal direction present.
- Re-read all 16 FD cards: FD-T01–03 Done; FD-T04 In Progress; FD-T05–16 in
  Current Work. All remain on the same board, with no card movement.
- Ran `git diff --check` on the scoped documentation (passed) and regenerated
  `ROADMAP.html` from `ROADMAP.md`. No Unity run, player build, or seed
  comparison was performed for this planning change.

## Risks and incomplete work

- FD-T02 and FD-T03 have completion comments while their Trello due-complete
  flags remain unset; ask the implementer to reconcile the bookkeeping.
- Existing estimates may change after FD-T04 remaining-work review; ecology
  diagnosis and new playability work have no agreed estimate yet. All 16 cards
  do not fit the 80h floor at their current upper range, even without reserve.
- Unrelated Unity-generated working-tree edits remain outside this handoff.

## Next useful step

Meet with Sim to rank the full card list, estimate remaining work and ecology
diagnosis, assign owners/reviewers, decide exact S5 exit checks, and schedule
the first playable review. Then reconcile the Trello cards, control and current
documents as one decision.
