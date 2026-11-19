# Bev-Ticket-2 fixed map shapes

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-11-19-0949-codex-bev-ticket-2-fixed-map-shapes
- Owner: Codex
- Branch: codex/bev-ticket-2-map-shapes
- Baseline commit: 0ea5118
- Date: 2026-11-19
- Supersedes: none

## Summary

The [fixed map shapes card](https://trello.com/c/9c6kJfkg/160-bev-ticket-2-new-map-shapes) adds deterministic, author-selected layouts on top of the mandatory wrapping already present in ProjectMain. The live board showed Bev-Ticket-1 in Done at 5/5 and Bev-Ticket-2 in Upcoming Work at 0/5 on November 19. This work is local to the feature branch in `F:/ForkBin/GameDev-Bev-Ticket-2`.

## Changes

- The scenario and simulation-data asset inspectors expose `Map Layout > Shape`. Select a preset, then set `Height` to the desired size. `Authored Layout` uses both existing dimensions and preserves any authored cells.
- Square, circle, diamond and cross use equal dimensions. Oval and short rectangle are approximately 3:2, with width rounded down to a whole cell. Long rectangle is 2:1. Starting populations must fit the playable cells at the chosen size.
- Each preset uses the shared layout and wrap contract. The playable row and column endpoints have reciprocal seam connections. Movement, placement, distance, snapshots and replay use the same topology. Preview dimension overrides yield to the authored fixed layout.
- The Noesis terrain layer clips its base fill and border-straddling sprite tiles to playable cell spans. Pointer and keyboard inspection select playable cells.

## Decisions and assumptions

- Height controls fixed-map scale. Existing scenario assets use their authored layout by default.
- Sparse presets reuse the cardinal seam pairing introduced in Bev-Ticket-1. Diagonal steps require a valid adjacent cell or an explicit diagonal connection. Procedural irregular maps and the water topology decision are tracked under [Bev-Ticket-3](https://trello.com/c/RTBOG9So/161-bev-ticket-3-irregular-map-shapes) and [VS-A1](https://trello.com/c/wsmjVY20).

## Validation

- Clean EditMode: 818 passed, zero failed, one skipped. This includes all seven authored geometry, seam, placement and board projection cases, plus both saved-preview settings cases. [EditMode results](../../artifacts/unity-tests-20261119-095604/EditMode-results.xml)
- Clean PlayMode: 102 passed, one failed, seven skipped. The authored Forest Edge route test still fails at seed 10101, tick 288. Its message and population history exactly match the retained ProjectMain integration baseline. The shaped-map PlayMode test passes. [PlayMode results](../../artifacts/unity-tests-20261119-095604/PlayMode-results.xml)
- Focused graphics PlayMode: one test passed at each resolution. The test advanced an authored cross in the prototype scene and verified keyboard navigation and the playable-cell selection guard. Both captures were inspected. [1920x1080](../../artifacts/visual-evidence-20261119-095509/bev-ticket-2-wrapped-cross.png) and [1280x720](../../artifacts/visual-evidence-20261119-095546/bev-ticket-2-wrapped-cross.png).
- Both CellSim portable tools built in Release with zero warnings and errors. The Batch self-test passed and included portable topology round-tripping.

## Risks and incomplete work

- Publication and Trello closeout require approval under the studio workflow.

## MainMenu playtest follow-up

- Bevin chose to use the existing LearningIndieDev entry in Unity Hub. The primary checkout at `F:/ForkBin/GameDev` now uses local branch `codex/bev-map-work`, based on the ticket commit `f5bf9df`. The original ticket worktree records the earlier implementation and validation.
- ForestEdge at Cross Height 20 had 204 playable cells and 475 starting entities. The scenario failed to initialize before the Desktop could open Simulation. Cross Height 40 fits the authored population in 816 playable cells. The Cross 40 scenario is a local working-tree playtest setting.
- The first MainMenu route test reached the running simulation and exposed a Fox footprint lookup at the off-map coordinate (0, 0). Hunt tracking now skips nonplayable cells. The shaped-board regression advances across two frames to exercise that update.
- Follow-up live Unity checks passed 2/2. The Hare journey launched from MainMenu with the local 40x40 cross and advanced through at least two ticks. The focused cross test verified later board refresh and cell inspection. [MainMenu results](../../artifacts/unity-tests-20261119-104509/PlayMode-results.json) and [cross results](../../artifacts/unity-tests-20261119-104521/PlayMode-results.json).

## Next useful step

Review the local branch for publication, then continue the authored-map work with Bev-Ticket-3.

## Creature tracking and seam verification

- In Cell Inspection, select a living creature and choose **Track Creature**. A cyan ring and the inspection coordinates follow its entity ID on each board update. **Stop Tracking** returns to cell inspection at the current location. Choosing a different cell or losing the tracked creature ends tracking.
- The controlled Cross PlayMode route applies an actual leftward movement step. Hare #451 moves from cell (1, 21) to (40, 21) between ticks 3 and 4, and the ring and inspection follow the same ID. The test also exercises tracking availability for an empty cell, stopping tracking, and replacement by a creature with a different ID.
- Reviewed captures at [1280x720 before](../../artifacts/visual-evidence-20261119-110227/creature-tracking-before-wrap.png), [1280x720 after](../../artifacts/visual-evidence-20261119-110227/creature-tracking-after-wrap.png), [1920x1080 before](../../artifacts/visual-evidence-20261119-110306/creature-tracking-before-wrap.png), and [1920x1080 after](../../artifacts/visual-evidence-20261119-110306/creature-tracking-after-wrap.png).
- Live Unity validation passed all 14 [topology tests](../../artifacts/unity-tests-20261119-110316/EditMode-results.json), all seven [authored preset tests](../../artifacts/unity-tests-20261119-110318/EditMode-results.json), the [MainMenu Hare launch check](../../artifacts/unity-tests-20261119-110319/PlayMode-results.json), the [existing cell inspection and speed control check](../../artifacts/unity-tests-20261119-110424/PlayMode-results.json), and the focused graphics check at both resolutions.

## Field Ledger scrolling

- The Field Ledger has a vertical scrolling area that ends above the pinned Zoom controls. The event card, population, Cell Inspection, and Field Notes remain reachable when the ledger overflows.
- The focused PlayMode regression displays the Hare Loss card on the Oval scenario, sends a mouse wheel event, and verifies that the full tracking button can be scrolled into view and used above Zoom. Passed and inspected at [1280x720](F:/ForkBin/GameDev-Bev-Ticket-2/LearningIndieDev/artifacts/visual-evidence-20261119-142557/field-ledger-scrolled-with-event.png) and [1920x1080](../../artifacts/visual-evidence-20261119-142823/field-ledger-scrolled-with-event.png). The existing [Cross tracking regression](../../artifacts/unity-tests-20261119-142832/PlayMode-results.json) also passed.
