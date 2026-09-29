# M1 exit-gate pre-review

[Working state](../WORKING_STATE.md) | Status: ready-for-owner-review; M1 remains Active

- Date: 2026-09-29
- Precondition: Sprint 3 was manually closed on 2026-09-29.
- Decision question: Review the M1 exit gate before refining the Proposed S4
  plan, or after?

## Recommendation

Review M1 before locking S4 scope. Keep the existing S4 draft Proposed. M1 is
the release gate for the first playable upgrade loop; its result determines
whether a small correction belongs before S4 or whether S4 can start with a
trusted baseline. S4 planning can resume immediately after the gate decision.

## M1 exit-gate evidence

The active gate in `ROADMAP.md` asks for one six-round, one-minute-simulation
Forest Edge expedition; five understandable Mutation decisions; expected
effects and a result; and reproducibility in the developer evidence path without
raw tuning fields.

| Gate item | Evidence reviewed | Assessment |
| --- | --- | --- |
| Six-round route and five decisions | Accepted S3-02 contract; S3-03 flow/recovery coverage; passing `PlayerModeUsesContinuousPhasesAndDeveloperModeCanSelectSingleRun` and `ContinuousPhaseDecisionResumesTheSamePreviewRun`. | Supported. |
| Understandable, valid Mutation offers and effects | S3-04 offers three applicable choices from the existing five-Hare pool or Skip; capped effects are hidden. Josh accepted Tough Hide's bounded directional copy and its matched six-phase evidence. | Supported within S3 V1; no broad balance, catalog, or player-fun claim. |
| Expected outcomes and recovery | Passing End/cancel, extinction/no-reward, results/next-run, and return-to-Lab PlayMode coverage; S3-03 completion record. | Supported. |
| Developer reproduction and separation from raw tuning fields | CF-0–CF-5 are implemented and verified; CF-4 emits validated JSON/Markdown/CSV/Stat-Line outputs and CF-5 supports deterministic schedules/checkpoint replay. `ExperimentalDiagnosticsStayHiddenUntilDeveloperModeEnabled` passes. EX-010 is accepted within its declared bounds. | Supported by the recorded developer workflow and tests. |
| Integrated clean baseline | Merged commit `51abd41d` plus the current working diff: EditMode 263/263; PlayMode 34 passed, 2 expected graphics-only skips, 0 failures. | Supported; artifacts are retained under `artifacts/unity-tests-20260929-033047/`. |
| No known blocking P0 | Current Loose Ends review records no verified P0. | Supported. |

## Items to carry into the owner review

- The cap fix and related integration/documentation changes are still
  uncommitted in `codex/forest-edge-visual-pass`. M1 should not be marked
  complete until this tested state is preserved in a reviewable commit or
  equivalent shared checkpoint.
- The evidence is distributed across the full-suite report, focused player-flow
  tests, CF-0–CF-5 validation, and the Tough Hide comparison. No single artifact
  combines one complete six-round player run with its matching developer report.
  Decide whether the distributed evidence satisfies the gate or whether to
  capture that paired example during M1 sign-off.
- S3's accepted Forest Edge evidence remains bounded. Wider ecology balance,
  additional Mutation catalog review, and future UI/art iteration are not M1
  blockers unless the owner elects to broaden the gate.
- CF-6 duration and memory measurement is explicitly stretch work, not an M1
  exit requirement.

## State

This is a pre-review, not an M1 closeout decision. M1 remains Active. The S4
control record remains Proposed and should be refined after the owner decides
the M1 gate; no S4 cards have been promoted or started.
