# Forest Edge starts and Hare purchase accounting

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-09-28-1122-codex-forest-edge-starts-and-hare-purchase-accounting
- Owner: codex
- Branch: BevBranch
- Baseline commit: 63d1499
- Date: 2026-09-28
- Supersedes: none

## Summary

The production Forest Edge scenario now explicitly starts 400 Plants, 55 Hares,
and 35 Foxes on its existing 36x20 grid. Whole-run stat lines now account for
Hares purchased at phase boundaries as `ADD`, so purchases no longer make FPO
look unreconciled or count as biological growth.

## Changes

- Set the three Forest Edge starting populations and set Plant starting
  probability to zero without changing asset identity or grid dimensions.
- Record successful boundary additions in run state and its checkpoint.
- Include `ADD` in expected Herbivore/Predator FPO and survival denominators;
  exclude it from RFS. Show it in the in-game summary and JSON/human/CSV
  reports, and update the independent Hare validator and formula docs.
- Update `WORKING_STATE.md`. The matching DARWIN OR DIE workbook remains a
  separate local file outside this Git repository.

## Decisions and assumptions

- `ADD` is a successful phase-boundary addition, not a birth. A phase window
  begins with its post-purchase population; whole-run accounting uses the
  original opening population plus `ADD`.
- Existing `ProjectSettings` worktree modifications are not part of this push.

## Validation

- The directly launched local Unity 6000.4.6f1 Editor completed compilation:
  `compilationFailed=false`, `compiling=false`, and zero Console errors. It was
  closed after the check. No Unity test suite or live purchase run was run.
- Edited PowerShell report scripts parsed successfully; `git diff --check`
  reported no whitespace errors. The serialized Forest Edge diff preserves its
  references and changes only the intended starting fields.

## Risks and incomplete work

- The new starting ecology has not had a balance run. A live Hare purchase and
  the final stat line have not been observed in Play Mode.
- The workbook cannot be included in this Git push because it is outside the
  repository.

## Next useful step

Run one Forest Edge phase, buy one Hare, and check that `ADD=1` and expected
FPO matches the observed final population. Review the new 400/55/35 opening
ecology separately before calling its balance accepted.
