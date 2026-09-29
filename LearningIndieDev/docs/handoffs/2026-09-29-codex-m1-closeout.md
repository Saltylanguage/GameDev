# M1 Exit-Gate Closeout

[Working state](../WORKING_STATE.md) | Status: complete; M1 closed 2026-09-29

- Decision owner: Josh Campbell
- Branch: `codex/forest-edge-visual-pass`
- Tested source checkpoint: `974827f2` (`fix: hide inapplicable mutation offers and close S3`)
- Remote: `origin/codex/forest-edge-visual-pass`
- Decision: accept the M1 exit gate and close the milestone.

## Evidence accepted

Josh accepted the distributed evidence package as sufficient for the M1 gate:

- The retained Clean run passed EditMode 263/263 and PlayMode 34 passed, 2
  expected graphics-only skips, 0 failed. Results are under
  `LearningIndieDev/artifacts/unity-tests-20260929-033047/`.
- PlayMode coverage exercises the six-round player route, continuous-phase
  decisions, same-run continuation, recovery, extinction/results, and return
  paths. The developer evidence path remains reproducible through CF-0–CF-5;
  the report and replay checks keep raw tuning fields out of player-facing UI.
- Mutation V1 offers three distinct applicable choices or Skip. Reproductive
  Drive is hidden when its increase would exceed the 1.0 cap; coupled Fox Brood
  Drive is hidden when its Hare response is capped. Josh accepts the bounded
  Tough Hide direction and Forest Edge evidence for this gate.
- The player route and results are supported by the integrated tests and
  retained artifacts; no unresolved P0 issue is recorded.

The evidence is distributed across test results, focused flow checks, developer
tooling validation, and the bounded Forest Edge comparison. There is no single
artifact pairing a complete six-round player run with its matching developer
report. Josh explicitly accepts the distributed package and understands that
later changes may require retesting. No new Unity run was performed during this
closeout; the tested code state was committed and pushed as `974827f2`.

## Scope and follow-up

- M1 is **Complete** as of 2026-09-29.
- CF-6 duration/memory measurement remains stretch work, not an M1 requirement.
- Broader ecology balance, Mutation catalog iteration, art production, and
  visual polish remain future work and are not implied to be complete by M1.
- S4 remains Proposed. Refine its plan and confirm its owners, estimates, and
  acceptance checks before a separate kickoff.
- Trello's `M1 - Playable upgrade loop` card is marked complete with the
  `COMPLETE` label. The S3 control card description now records the M1
  decision and pushed implementation checkpoint.

## Linked records

- [S3 control record](../Sprints/S3-control-record.md)
- [S4 control record](../Sprints/S4-control-record.md)
- [M1 exit-gate pre-review](2026-09-29-codex-m1-exit-gate-prereview.md)
- [S3 merged-baseline cap review](2026-09-29-0341-codex-s3-merged-cap-review.md)
- [M1 Trello card](https://trello.com/c/yU4KboDa/18-m1-playable-upgrade-loop)
- [S3 Trello control card](https://trello.com/c/zfzJkUnj/109-sprint-3-control-record-safe-game-loop-and-m1-closeout)
