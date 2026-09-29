# S3 merged cap review

[Working state](../WORKING_STATE.md) | Status: closed

- Handoff schema: 1
- Handoff ID: 2026-09-29-0341-codex-s3-merged-cap-review
- Owner: Codex
- Branch: codex/forest-edge-visual-pass
- Baseline commit: 51abd41d
- Date: 2026-09-29
- Supersedes: none

## Summary

Integrated the approved invalid-Mutation offer rule against the merged
`codex/forest-edge-visual-pass` baseline (`51abd41d`). Reproductive Drive is
now hidden when its chance increase would exceed 1.0; Fox Brood Drive is hidden
when its coupled Hare response cannot apply. The merged Clean suites pass with
the fix and its integration repairs.

## Changes

- `SpeciesProgression` checks reproduction chance eligibility for paid and free
  upgrades, so an invalid purchase returns false without changing currency or
  rules.
- Experimental offers filter invalid upgrades while preserving three distinct
  Hare phase choices. The phase offer uses the accepted five-Mutation pool;
  paired response copy identifies the responding species.
- Resizing with explicit starting-population overrides now applies the new grid
  and population map atomically. Settings reject authored populations that do
  not fit before building a run. Small-grid tests now use explicit test
  populations where they expect a successful run.
- Updated regression tests for Hare caps, Fox paired responses, population
  validation, and the current Tough Hide copy.

## Decisions and assumptions

- Josh approved hiding a Mutation when it is invalid at the current species
  stats. This does not change the Hare's authored 1.0 reproduction chance.
- Josh accepts the current Forest Edge evidence for this S3 closeout; further
  balance work remains iterative and is not treated as broadly approved.
- Main Menu generated-art review is complete. The direction is selected and
  Chrono is working on the art; further interface polish remains future work.
- Josh manually closed S3 on 2026-09-29 after reviewing the current conversation
  and retained evidence. Trello now has committed S3 cards and the control card
  in Done; S3-05 remains Backlog as uncommitted stretch work. M1 remains Active
  for a separate exit-gate review before S4 planning.

## Validation

- `CellSim Test -Mode All -Execution Clean` ran from a closed managed worktree
  at merged commit `51abd41d`, with the current source diff applied. EditMode:
  263/263 passed. PlayMode: 34 passed, 2 expected graphics-only skips, 0
  failures. The run includes the Hare offer and Fox response cap regressions.
- Results are retained in
  [`unity-tests-20260929-033047`](../../artifacts/unity-tests-20260929-033047/).
- `git diff --check` passed.

## Risks and incomplete work

- Source and documentation changes remain uncommitted on
  `codex/forest-edge-visual-pass`.
- Forest Edge balance values remain provisional for future iterations, and
  Chrono's art and further UI polish continue. The current source and
  documentation changes remain uncommitted on
  `codex/forest-edge-visual-pass`.

## Next useful step

Review the M1 exit gate next. If it passes, close M1, then refine the Proposed
S4 plan; if it exposes a gap, decide whether it belongs in the M1 correction or
the S4 backlog before kickoff.
