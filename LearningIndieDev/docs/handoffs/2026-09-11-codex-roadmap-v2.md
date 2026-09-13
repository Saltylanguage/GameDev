# Roadmap v2

[Working state](../WORKING_STATE.md) | Status: ready for review

- Owner: Codex
- Branch: `codex/simulation-window-production`
- Date: 2026-09-11

## Result

Replaced the canonical production roadmap with a version 2 baseline that shows
current milestone state, gives all twenty player-facing features stable IDs and
first-slice effort bands, assigns the proposed Sprint 3 capacity, and keeps
later sprint dates visibly provisional.

## Plan changes

- Marked M0 complete and M1 active.
- Made the GalapagOS Desktop the canonical player home while retaining the
  standalone Lab as a legacy/developer route.
- Reduced the open CF-6 gate to the outer ten-phase duration and peak-memory
  measurement; EX-010, target-resolution graphics acceptance, and the Windows
  smoke remain closed within their recorded bounds.
- Allocated the proposed Sprint 3 forty-hour envelope across the expedition
  contract, boundary/result route, Mutation evidence follow-up, CF-6, Fox
  telemetry, reconciliation, and a six-hour integration reserve.
- Added forecast dates and 32-hour feature envelopes plus eight-hour reserves
  for S4–S7.
- Scheduled the first Genome contract for S4 and persistence-backed
  implementation for S6, replacing the ambiguous “ASAP” wording.
- Split local profile persistence from later cloud sync and kept broad ecology,
  biome, collection, content-expansion, and predictive work behind explicit
  gates.

## Documents updated

- `ROADMAP.md`
- `docs/FUTURE_SPRINT_ROADMAP.md`
- `docs/GAME_FEATURE_ROADMAP_TRIAGE.md`
- `docs/Sprints/S2-control-record.md`
- `docs/Sprints/S3-control-record.md`
- `docs/WORKING_STATE.md`
- `docs/PROJECT_CONTEXT.md`
- `docs/UPGRADE_SYSTEM_DIRECTION.md`
- `docs/INCOMPLETE_FEATURES_ACTION_PLAN.md`
- `docs/MAIN_MENU_LAB_DELIVERY_PLAN.md`

## Validation

- `git diff --check` passes; Git reports only the checkout's normal future
  LF-to-CRLF conversion warnings.
- The feature ledger contains twenty unique IDs, F01 through F20.
- The Sprint 3 allocation totals 20 hours for Josh and 20 hours for Sim, with a
  six-hour shared reserve.
- Every local link in `ROADMAP.md` resolves.
- No Unity tests were run because this change modifies planning documentation
  only.

## Review focus

- Confirm the provisional S3 allocation at the Sprint 2 review rather than
  treating it as already committed.
- Name the currently role-based UI, art, audio, and platform reviewers when
  their features are promoted.
- Split S6 if its persistence and first-Genome tasks cannot fit inside the
  32-hour feature envelope; do not compress migration or recovery testing.
- The Loose Ends ledger still contains the pre-v2 Genome “ASAP” wording. It was
  intentionally left untouched because ledger entries change only when the
  user explicitly asks to record or resolve a finding.

## Next useful step

Run the Sprint 2 review, record actual effort and card disposition, and either
approve the proposed Sprint 3 allocation or adjust its rows while preserving
the 40-hour capacity and M1 outcome.
