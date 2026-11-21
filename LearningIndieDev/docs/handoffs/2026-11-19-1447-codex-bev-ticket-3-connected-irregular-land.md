# Bev-Ticket-3 connected irregular land

[Working state](../WORKING_STATE.md) | Status: in-progress

- Handoff schema: 1
- Handoff ID: 2026-11-19-1447-codex-bev-ticket-3-connected-irregular-land
- Owner: Codex
- Branch: codex/bev-ticket-3-irregular-maps
- Baseline commit: 746ec85
- Date: 2026-11-19
- Supersedes: none

## Summary

Bev-Ticket-2 is published for review as [PR #9](https://github.com/Saltylanguage/GameDev/pull/9) against ProjectMain. Its [Trello card](https://trello.com/c/9c6kJfkg/160-bev-ticket-2-new-map-shapes) is complete in Done with five of five items checked. [Bev-Ticket-3](https://trello.com/c/RTBOG9So/161-bev-ticket-3-irregular-map-shapes) is In Progress. The first slice generates one connected land layout in the existing scenario and board workflow.

## Changes

- Choose **Map Layout > Shape > Irregular Land** in the scenario or simulation-data Inspector. **Width** and **Height** set the layout dimensions. **Map Seed** controls the outline independently of the simulation seed. **Edge Roughness** controls how much the edge varies between rows.
- The generator tapers the outer rows and perturbs each row's left and right edges from the seed. Every row retains its center cell or pair of center cells. Playable coordinates and reciprocal seams are produced by the existing `MapTopology`.
- The topology stores the generator version, seed, dimensions and roughness. The portable snapshot retains the generated cells, seams and input string.

## Decisions and assumptions

- Use the existing Unity Hub project folder, `F:/ForkBin/GameDev/LearningIndieDev`, on local `codex/bev-ticket-3-irregular-maps`. The fixed-shape review branch is frozen at `746ec85`. Bevin's Oval remains a working-tree playtest edit.
- The roughness range is 0 to 100, with a default of 35. The map seed defaults to 1. A value of 0 produces the tapered outline with no seeded variation.

## Validation

- Batch and Workbench Release builds passed. The Batch self-test passed, including portable topology round-tripping.
- Unity compiled with zero errors. The focused EditMode runs passed 22 cases covering the seeded layout, the seven fixed presets and the shared topology. Results are in local `artifacts/unity-tests-20261119-144844`, `artifacts/unity-tests-20261119-144850` and `artifacts/unity-tests-20261119-144854`.
- The MainMenu PlayMode test launched Forest Edge with the representative Irregular Land settings. After the run began, the test isolated Hare #553 for a controlled step. Cell Inspection retained its ID and the tracking ring moved from displayed cell (6, 7) to (34, 7). This passed and was viewed at 1280×720 and 1920×1080. Before-and-after screenshots and test results are in local `artifacts/visual-evidence-20261119-145153` and `artifacts/visual-evidence-20261119-145224`.
- The existing Hare Journey launch and fixed Cross rendering and inspection tests each passed. Results are in local `artifacts/unity-tests-20261119-145304` and `artifacts/unity-tests-20261119-145316`.

## Risks and incomplete work

- Generator controls and topology rules for holes and disconnected islands remain open. Reconcile the water topology under [VS-A1](https://trello.com/c/wsmjVY20) before integrating water.

## Next useful step

For a representative playtest through MainMenu, set Forest Edge to Irregular Land at Width 45, Height 55, Map Seed 17 and Edge Roughness 65. From MainMenu, Continue to the Desktop, open Simulation, choose Creek Bend, claim the opening Mutation, and enter Forest Edge. Use Track Creature on a hare near an uneven edge and resume at 0.5× to watch for a crossing.
