# remaining-roadmap-audit

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: josh
- Branch: SaltysFirstBranch
- Baseline commit: e4c15b39
- Date: 2026-08-11

## Summary

Reviewed the three remaining roadmap items after ruleset fingerprints. The
legacy prototype audit is complete; custom rule logic and ScriptableObject
authoring remain intentionally trigger-based rather than speculative work.

## Changes

- Added `docs/LEGACY_PROTOTYPE_AUDIT.md` with scene, test, build-setting, and
  deletion evidence.
- Updated `CELLULAR_SIM_TODOS.md` to record the audit result.
- Added the audit to `WORKING_STATE.md`.

## Decisions and assumptions

- Do not delete the Island Survivor or cellular prototype scenes: both are
  enabled and covered by Play Mode tests.
- Do not delete cave or Life domain code: runtime tests cover both paths.
- CS-04 should activate only when a real mechanic exceeds the current data and
  stage model; no universal rule plugin or event bus is justified yet.
- CS-06 should activate only when reusable designer-authored scenarios are a
  demonstrated workflow need; current code-authored immutable data remains the
  simplest source-controlled path.

## Validation

- `git diff --check` passed.
- Reference audit covered enabled scenes, scene script GUIDs, runtime tests,
  Play Mode tests, and build settings.
- No Unity asset or scene was modified.

## Risks and incomplete work

- No legacy files were deleted because no candidate met the deletion bar.
- CS-04 and CS-06 remain open deferred items and need concrete triggers.

## Next useful step

Choose a concrete behavior such as sight/pathfinding or alpha offspring to
activate CS-04, or define a real designer preset workflow to activate CS-06.
