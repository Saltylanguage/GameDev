# Project sprint cadence change to two weeks

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: Codex
- Branch: codex/cellular-sprite-tiling
- Baseline commit: 5bb8cc5
- Date: 2026-08-18

## Summary

The project sprint baseline is now two weeks, with approximately 20 hours of
planning capacity per developer. Sprint 1 remains narrowly committed to the
existing 20-hour Main Menu/Lab foundation scope while its execution window is
extended from August 17–23 to August 17–30, 2026.

## Changes

- Updated `ROADMAP.md` and `docs/SPRINT_KICKOFF_WORKFLOW.md` to define the
  two-week cadence and 20-hour per-developer planning capacity.
- Updated `docs/SPRINT_1_PLAN.md` and
  `docs/Sprints/S1-control-record.md` with the August 17–30 window, Aug 27
  checkpoint, Aug 30 review, and explicit uncommitted-capacity boundary.
- Updated `docs/LOOSE_ENDS.md` to resolve the S1 readiness/control-record
  finding and revised the Sprint 0 closeout wording to reference the durable
  control record.
- Updated Trello cards 51–54 and the S1 control card 62 with Aug 30 due dates,
  current sprint-window metadata, and a cadence-change comment. Six stale
  Sprint 0 cards were triaged into `🗂️ Backlog & Ideas` with notes; none were
  duplicates or fully covered, so none were deleted or relabeled.

## Decisions and assumptions

- The two-week cadence changes planning windows and baseline capacity; it does
  not automatically add features to an active sprint. S1's committed work and
  20-hour estimate remain unchanged, with the additional capacity left
  uncommitted until a later decision.
- Sprint 0 dates remain historical evidence and are not rewritten.

## Validation

- Re-read the Trello board after mutation: `🎯 Upcoming Work` contains four S1
  task cards, each due Aug 30; `🛠️ In Progress` contains the two genuine
  current work cards plus the S1 control card, also due Aug 30; backlog count is
  21 after the six-card triage.
- Verified the control card title, description, due date, and cadence comment.
- Reviewed the documentation diff and schedule/date references with `rg` and
  `git diff`.

## Risks and incomplete work

- The four S1 tasks remain in `🎯 Upcoming Work` until the sprint kickoff
  operation promotes them; this change only updates cadence and metadata.
- Fox telemetry and species-roster work remain active and were intentionally
  left untouched.

## Next useful step

At the Aug 27 checkpoint, compare actual hours with the 20-hour committed
scope and decide whether any additional two-week capacity should be explicitly
committed or remain reserve.
