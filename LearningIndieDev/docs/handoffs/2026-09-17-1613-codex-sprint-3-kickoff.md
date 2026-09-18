# Sprint 3 kickoff

[Working state](../WORKING_STATE.md) | Status: complete

- Handoff schema: 1
- Handoff ID: 2026-09-17-1613-codex-sprint-3-kickoff
- Owner: Codex
- Branch: ProjectMain
- Baseline commit: cab5838d
- Date: 2026-09-17
- Supersedes: none

## Summary

Sprint 3 was confirmed and kicked off on 2026-09-17. The plan prioritizes a
safe, uninterrupted Forest Edge game loop and M1 closeout, with bounded
Mutation readability, UI polish, a protected integration reserve, and Fox
telemetry as the sole S2 carry-over. Profile saving stays in S4; duration and
memory measurement is stretch-only.

## Changes

- Updated the S3 control record, roadmap, project context, working state, S2
  historical control record, and Loose Ends status.
- Promoted S3-01, S3-02, S3-03, S3-04, S3-06, S3-07, and the existing Fox card
  (S3-08) to Trello `Current Work`; created S3-05 in Backlog as stretch-only.
- Added the S3 control card to Trello `Roadmap & Milestones`.
- Trello operation: `S3-KICKOFF-20260917-01`.

## Decisions and assumptions

- S3-03 is the Josh-owned 8-hour safe-flow and recovery priority.
- S3-06 is a 4-hour polish/UI package split 2h Josh / 2h Sim.
- Committed effort is Josh 20h and Sim 18h; Sim has 2h capacity unallocated.
- CF-6 is not a committed M1 gate and requires a scope trade or added capacity
  before it can start. Local profile save/restore is scheduled for S4.

## Validation

- Re-read Trello after card updates: seven committed cards in `Current Work`,
  S3-05 in Backlog, and Upcoming/In Progress/Blocked empty.
- `✅ Done` contained 0 cards and `Archived` contained 35 at verification;
  this differed from the earlier S2 closeout snapshot. No archived cards were
  moved or altered.
- Owners and hour estimates are recorded in card descriptions; the available
  card-write connection did not set Trello member assignments.
- No Unity tests were run during this planning/kickoff update.

## Risks and incomplete work

- The Done/Archived count differs from the earlier S2 closeout snapshot; the
  discrepancy is recorded in the S2 and S3 control records rather than
  rewriting archived history.
- Remaining 2h of Sim capacity is intentionally unallocated. Trello owner
  fields are descriptive metadata, not actual member assignments.

## Next useful step

The kickoff-time suggestion to rerun the consolidated baseline is complete.
The latest retained results pass: EditMode 251/251, no-graphics PlayMode 28
passed with two expected graphics-only skips, and graphics-capable PlayMode
30/30. Review the artifacts and reconcile the S3-01 Trello card status before
using this baseline as accepted evidence; see [Working State](../WORKING_STATE.md)
and the [S3 control record](../Sprints/S3-control-record.md).
