# S4-02 Seed Dispersal and Gardeners

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-02-1319-codex-s4-02-seed-dispersal-and-gardeners
- Owner: Codex
- Branch: BevBranch
- Baseline commit: 3eea4e8
- Date: 2026-10-02
- Supersedes: none

## Summary

Seed Dispersal is implemented as the next S4 Hare basic skill. Each level adds
one percentage point to planting chance per eligible simulation step, capped at
level 10. A plant is created only on a nearby passable empty tile; one stored
food is spent only after successful creation. Ordinary plant births remain
separate from the direct planting counters.

## Changes

- Added `seed-dispersal` to the shared upgrade catalog, normal and experimental
  Hare pools, and the Efficient Digestion partner path.
- Added effective chance, attempt/success, food-created, and reserve-spent
  counters to activity snapshots, windows, reports, and the preview Inspector.
- Added the passable-space eligibility guard while preserving the existing
  movement-pattern selection for deterministic placement.
- Added domain, editor batch, and PlayMode coverage for caps, snapshot parity,
  eligibility, food accounting, counters, and the five-choice Gardeners path.

## Decisions and assumptions

- Planting is checked per eligible simulation step, not per feeding action.
- Empty passable terrain is required; blocked, occupied, existing-resource, and
  out-of-bounds candidates do not consume a chance roll or stored food.
- `SpeciesRules` now rejects non-finite seed-drop chance and starting food.
- The existing saved prototype scene diff is preserved separately and was not
  changed by this slice.

## Validation

- Direct Unity EditMode batch: **219 passed, 0 failed** (including new
  Seed Dispersal domain/editor cases), report:
  `artifacts/s4-seed-dispersal-20261002/editmode-results.xml`.
- Compiler/log output:
  `artifacts/s4-seed-dispersal-20261002/editmode.log`.
- `git diff --check` passed for feature files with the saved scene excluded.
  The scene SHA256 remained
  `0720CED6F17A6D6FB29FE64D6F3D28106A99F1F6A994F923B14038D1306A3879`.

## Risks and incomplete work

- Focused PlayMode execution passed: 1 passed, 0 failed for
  `GardenersPathCanBeChosenAcrossFivePhaseDecisions`.
- Manual visual acceptance is complete for the Seed Dispersal card and results
  presentation. Bevin's 2026-10-02 screenshot shows the 3% chance, 6,769
  planting attempts, 172 plants created, and 172/172 food-created/spent values
  clearly in the Inspector alongside the completed slashline results. The
  screenshot shows a separate run's offer choices; the five-choice Gardeners
  sequence remains covered by the focused PlayMode test.
- No matched-seed balance claim is made. The manual S4 fixture now defaults to
  400/20/10 per Bevin's follow-up; the existing automated Gardeners test keeps
  its explicit 400/25/15 setup until a balance baseline is deliberately
  reselected.

## Next useful step

Perform the short manual card/results check in Unity, then send this handoff to
Salty and his agent for review. Follow with matched-seed balance comparisons;
the current implementation makes no balance claim.
