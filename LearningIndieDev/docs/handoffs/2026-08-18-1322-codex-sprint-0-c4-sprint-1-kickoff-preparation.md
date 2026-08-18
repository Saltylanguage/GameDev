# Sprint 0 C4 Sprint 1 kickoff preparation

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: Codex
- Branch: codex/cellular-sprite-tiling
- Baseline commit: f1bf0e3
- Date: 2026-08-18

## Summary

C4 closes Sprint 0 kickoff preparation and activates the dated Sprint 1 control
record. The committed scope is four work packages totaling 20 hours, with
explicit owners, reviewers, dependencies, acceptance evidence, and midweek and
final review points.

## Changes

- Added [`../Sprints/S1-control-record.md`](../Sprints/S1-control-record.md)
  with the Sprint ID, dates, goal, capacity, task register, checkpoints, cut
  order, and known blockers.
- Confirmed the task split: S1.1 shell 10h, S1.2 research preview 4h, S1.3
  verification 3h, and integration reserve 3h.
- Assigned owners/reviewers: Sim owns S1.1/S1.2 implementation, Josh owns
  product/integration and S1.3 verification, with reciprocal review coverage.
- Confirmed the midweek checkpoint on 2026-08-20 and final review on
  2026-08-23.
- Reconciled the extra Player UI/Dev Lab separation card as deferred backlog
  work because it is explicitly outside Sprint 1 and lacks ready metadata.
- Marked `SPRINT_1_PLAN.md` active while retaining its authoritative execution
  role.

## Decisions and assumptions

- The four existing Upcoming Work cards are the complete Sprint 1 commitment;
  no new feature card is added to the 20-hour cap.
- Build Settings promotion remains conditional on the C3/C4 smoke gate and may
  be cut while preserving direct-scene verification.
- The Unity licensing/headless entitlement failure is an explicit integration
  risk; it is not silently assigned as implementation work.

## Validation

- Read each Sprint 1 Upcoming Work card and the C4 card from the board.
- Confirmed the four committed cards total 20h with Josh 10h and Sim 10h.
- Confirmed the extra UI/Dev Lab card has no description, estimate, owner,
  reviewer, or due date and conflicts with the explicit Sprint 1 deferral.
- `git diff --check` is pending after the C4 documentation changes.

## Risks and incomplete work

- The board still needs the Sprint 1 control card and card-level metadata
  comments to mirror this record.
- Unity Editor/Play Mode and Windows-build smoke evidence remains blocked by
  the licensing issue.

## Next useful step

Start S1.1 from the bounded C3 repair seam, keeping the uncommitted simulation
and tooling batches outside the player-shell commit.
TODO
