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

## Next useful step

Review the local branch for publication, then continue the authored-map work with Bev-Ticket-3.
