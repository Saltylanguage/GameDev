# S4 closeout and S5 proposed handoff

[Working state](../WORKING_STATE.md) | Status: shared

- Handoff schema: 1
- Handoff ID: 2026-10-15-2007-codex-s4-closeout-and-s5-proposed-handoff
- Owner: Codex
- Branch: ProjectMain
- Baseline commit: 239024de
- Date: 2026-10-15
- Supersedes: none

## Summary

Josh approved S4 administrative closeout on 2026-10-15 after reviewing the
unfinished work. The board and repo now show S4 Closed, while the next sprint
remains Proposed so its simulation and upgrade work can receive a separate
capacity and acceptance review.

## Changes

- Moved the [S4 control card](https://trello.com/c/Zn4UpBYc/118-sprint-4-control-record-build-identity-and-evidence)
  to Done and marked it Closed. Verified S4-01/02/03/07 in Done.
- Returned [P1-032](https://trello.com/c/Enm8Avxu/111-p1-032-launch-from-the-active-profile)
  from Current Work to Backlog; verified S4-04/05 remain Backlog and Current
  Work is empty.
- Synchronized the [S4 control record](../Sprints/S4-control-record.md),
  [roadmap](../../ROADMAP.md), [working state](../WORKING_STATE.md),
  [project context](../PROJECT_CONTEXT.md), [loose ends](../LOOSE_ENDS.md),
  and [S5 proposal](../Sprints/S5-proposal-2026-10-15.md).

## Decisions and assumptions

- The closeout records delivered research and Desktop/profile foundation; it
  does not count the original three-build in-game acceptance or independent
  review as passed.
- P1-032 retains Josh ownership and Sim launch-input review. It is unscheduled
  for S5. The S5 proposal has no selected cards, final estimates, or kickoff.
- Josh confirmed 5,760 × 1,080 as the final Journey panorama size, matching
  Chrono's art card and the already published normalized map contract.

## Validation

- Re-read the Trello board and both changed cards after the moves. The control
  card is Done/Closed; S4-01/02/03/07 are Done; P1-032 and S4-04/05 are
  Backlog; Current Work is empty.
- `git diff --check` on the selected documentation paths passed. No Unity
  tests were rerun for this Trello/documentation closeout. Earlier test and
  Windows build evidence is in the [profile acceptance handoff](2026-10-15-1835-codex-profile-feature-unlock-storage-and-desktop-launch-acceptance.md)
  and [journey art handoff](2026-10-15-1903-codex-journey-panorama-art-alignment-and-s5-proposal.md).

## Risks and incomplete work

- Sim's launch-input review and a real application save/exit/relaunch check
  remain on P1-032. Three-build spatial/in-game acceptance and balance remain
  simulation work; the existing matched-seed screen alone does not prove them.
- Unrelated Unity-generated local asset/settings changes and root test XMLs
  were present before this closeout and were left out of the documentation
  commit.

## Next useful step

Plan S5 as a separate kickoff: choose a bounded Forest Edge simulation and
upgrade slice, assign owners and reviewers, estimate it within capacity, freeze
playtest inputs, and record explicit accept/revise checks. Keep P1-032
unscheduled unless deliberately selected later.
