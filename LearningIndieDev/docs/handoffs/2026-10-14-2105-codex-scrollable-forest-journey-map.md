# scrollable-forest-journey-map

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-14-2105-codex-scrollable-forest-journey-map
- Owner: Codex
- Branch: ProjectMain
- Baseline commit: f146e33a
- Date: 2026-10-14
- Supersedes: none

## Summary

Implemented Josh's horizontal journey layout study in the Unity/Noesis screen.
The map now scrolls through a quiet forest panorama, with three playable
trailheads, 29 circular icon markers, and 35 branching trail connections across
13 visual stages. The background is a temporary image behind live UI geometry,
ready for Chrono's authored panorama. Changes are local on ProjectMain.

**Publication update, 2026-10-15:** The map is included in the ProjectMain
publication work below. Josh's visual review and Chrono's final art remain open.

## Changes

- Expanded the authored Journey ScriptableObjects and normalized placements to
  match the asymmetric layout. Existing node GUIDs and the canonical
  `forest-edge-start` / `first-cycle` route are preserved.
- Added Fern Glade and Pine Verge reward starts and their first simulation
  nodes. Claiming a reward records the chosen start; only that start's next
  simulation can be selected.
- Replaced the fixed map and right journal with a full-width horizontal
  ScrollViewer, dirt-trail geometry, circular type icons, and hover cards.
  Wheel input travels horizontally. Reopening or resetting the map positions
  it at the relevant node. Hover cards move to the other side near an edge.
- Added `GalapagOS.Image.Journey.Panorama` to the shared image dictionary.
  Its current 4700 × 1080 placeholder uses point filtering, no mipmaps,
  clamp wrapping, an 8192 maximum import size, and no compression.
- Corrected the visual test capture helper to let Noesis measure its render
  target before taking the image, avoiding stale Game-view aspect ratios.
- Updated the journey contract, project context, and Working State index.

## Decisions and assumptions

- All three opening rewards use the existing three Hare Mutation offers and
  first-cycle scenario; new effects or balance values were not introduced.
- The upper start leads toward Seedfall, the middle can choose either authored
  condition, and the lower leads toward Fox Tracks, matching the accepted
  branch layout. Both conditions still lead to their existing second cycle.
- The 13-stage graph describes the visual journey. Gameplay still ends after
  the second simulation; nineteen later markers remain non-playable content
  placeholders.
- The source panorama is separate from route geometry, icons, text, state
  highlights, and hover information. Replacement dimensions and coordinate
  conversion are recorded in `docs/JOURNEY_PROTOTYPE.md`.

## Validation

- Unity Editor graph test: 1 passed, 0 failed
  (`artifacts/unity-tests-20261014-205942/EditMode-results.xml`).
- Journey Play Mode filter: 5 passed, 0 failed
  (`artifacts/unity-tests-20261014-210025/PlayMode-results.xml`).
- After strengthening the three-opening check to actually start each
  simulation, that targeted test passed 1/1
  (`artifacts/unity-tests-20261014-210639/PlayMode-results.xml`).
- 1280 × 720 graphics capture and scrolling check: 1 passed, 0 failed
  (`artifacts/visual-evidence-20261014-210811/PlayMode-results.xml`).
- Final 1920 × 1080 graphics fixture: 5 passed, 0 failed
  (`artifacts/visual-evidence-20261014-211512/PlayMode-results.xml`). It verifies
  wheel travel, opening/middle/right captures, hover, report-to-map continuity,
  reopening after exploration, and returning to the trailheads after reset.
- XML syntax and `git diff --check` passed. Unrelated importer, solution, and
  resolution changes produced by the test Editor were restored to baseline.
- Captures are under `artifacts/journey-map-runtime-20261014/` (1920 × 1080)
  and `artifacts/journey-map-runtime-1280-20261014/` (1280 × 720).

## Risks and incomplete work

- Chrono's final art and the later encounter content are pending. This pass
  establishes layout and interaction, not journey pacing or ecology balance.
- Full Unity suites were not rerun; the checks above target this map change.
- The graphics wrapper's temporary request file was cleared during clean
  Editor startup. Passing the visual output and dimensions through the
  environment produced the retained captures. The wrapper itself was not
  changed.
- No commit, push, or Trello publication was performed.

## Publication recheck — 2026-10-15

- Clean Editor graph test: 1 passed, 0 failed
  (`artifacts/unity-tests-20261015-185248/EditMode-results.xml`).
- Clean journey PlayMode filter: 5 passed, 0 failed
  (`artifacts/unity-tests-20261015-185311/PlayMode-results.xml`).
- Clean focused visual acceptance test: 1 passed, 0 failed
  (`artifacts/visual-evidence-20261015-185440/PlayMode-results.xml`). The
  wrapper did not retain screenshots in this run; use the October 14 captures
  above for visual review.
- Scoped `git diff --check` passed. Unity-generated sprite import settings,
  project settings, solution, and root checkpoint XML files were excluded from
  the map publication.

## Next useful step

Review the actual Unity opening, middle, right, and hover captures with Josh.
Give Chrono the panorama dimensions and current placement guide before final
trail painting. Replace the panorama through its shared resource key after
the artwork is accepted.
