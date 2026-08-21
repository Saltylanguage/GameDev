# Future sprint roadmap planning horizon

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: Codex
- Branch: codex/cellular-sprite-tiling
- Baseline commit: a253a09
- Date: 2026-08-18

## Summary

Created a draft planning horizon for the next two-week sprints without
expanding the active S1 commitment. The map covers save/load, species and
scenario design, iteration tooling, upgrade design and implementation, the art
bible, and UI/UX design.

## Changes

- Added `docs/FUTURE_SPRINT_ROADMAP.md` with theme outcomes, dependencies,
  candidate S2–S6 horizons, a post-slice content-alpha bucket, and a future-card
  promotion checklist.
- Linked the draft horizon from `docs/WORKING_STATE.md` and `ROADMAP.md`.
- Explicitly split save/load design from implementation and made tooling an
  accelerator lane with a manual fallback, so tools cannot silently block
  feature work.

## Decisions and assumptions

- S2 remains focused on the first trustworthy upgrade loop; S3 covers slice
  species/scenario co-design; S4 covers art direction and UI/UX readability;
  S5 covers profile/save/load and settlement; S6 validates the integrated
  vertical slice.
- Permanent research and per-run evolution remain separate progression layers.
- Active-run save/resume is a separate decision from profile/settings saves.
- The document is a draft planning aid. It does not create or move Trello
  cards, and future scope will be promoted only after the S1 review.

## Validation

- Read the existing `ROADMAP.md`, `MAIN_MENU_LAB_DELIVERY_PLAN.md`,
  `UPGRADE_SYSTEM_DIRECTION.md`, `WORKING_STATE.md`, and current S1 handoffs.
- Confirmed the new dependency sketch agrees with the existing E2–E6 delivery
  epics and the two-week sprint cadence.
- Ran `git diff --check` on the planning changes.

## Risks and incomplete work

- Exact S2 card breakdown, owners, estimates, and acceptance evidence remain
  intentionally open until the S1 review.
- Save schema and migration details require the eventual profile and settlement
  contracts before implementation can be committed.
- Tooling candidates still need repeated-friction evidence and measured inputs/
  outputs before promotion.

## Next useful step

At the S1 review, select one S2 primary outcome and convert only its smallest
ready slices into Trello cards with owners, estimates, dependencies, and
acceptance evidence.
