# Bev Ticket 3 organic coastlines

[Working state](../WORKING_STATE.md) | Status: in-progress

- Handoff schema: 1
- Handoff ID: 2026-11-19-1525-codex-bev-ticket-3-organic-coastlines
- Owner: Codex
- Branch: codex/bev-ticket-3-irregular-maps
- Baseline commit: f9e3af5
- Date: 2026-11-19
- Supersedes: irregular-land generator behavior from the [first-slice handoff](2026-11-19-1447-codex-bev-ticket-3-connected-irregular-land.md)

## Summary

Bevin confirmed that the first Irregular Land map ran and wrapped. The two edges changed independently for each row, creating a striped silhouette. The follow-up sketch shows broad lobes, bays spanning multiple rows and a small enclosed gap. The new generator produces this type of outline through the existing Irregular Land preset.

## Changes

- Seeded variation around a continuous contour replaces row-by-row offsets. A cardinal flood fill keeps one connected mainland after rasterization. Edge Roughness 0 produces an oval, and higher values strengthen the bays and lobes.
- **Interior Hole** is an opt-in gap when Width and Height are at least 16. The gap has no playable cells. The existing topology wraps outer row and column endpoints and blocks steps into the gap.
- The generator records `irregular-land-v2` and the applied hole setting. Existing Irregular Land authoring settings generate the updated outline at next launch. Portable snapshots carry their concrete geometry and seam pairs.

## Decisions and assumptions

- Interpreted the sketch's inner circle as an impassable gap. Water rules are tracked in [VS-A1](https://trello.com/c/wsmjVY20).
- The work is on `codex/bev-ticket-3-irregular-maps` in the existing Unity Hub folder. The user's Forest Edge Inspector settings are local playtest edits.

## Validation

- Unity compiled with no errors. The six new or updated Irregular Land EditMode cases passed. They verify a bay spanning adjacent rows, repeatability, cardinal connectivity, reciprocal seams, the optional gap, and its minimum dimensions. The broader live fixture uses the local Forest Edge preset and reported 20 failures in tests that resize an authored rectangular map. Results are in `artifacts/unity-tests-20261119-152526`.
- In a separate validation copy using the committed rectangular Forest Edge asset, all 58 `PopulationConfigurationTests` passed. The first attempt timed out during import. The retry with the imported files passed in `artifacts/organic-coast-validation-20261119-152618/artifacts/unity-tests-20261119-153306`.
- All 14 shared topology EditMode tests passed in `artifacts/unity-tests-20261119-152633`.
- The live MainMenu PlayMode test launched Forest Edge at 68×48, map seed 17, roughness 65, and Interior Hole enabled. The board rendered the outline and enclosed gap, and the controlled crossing kept tracking Hare #553 from displayed cell (34, 9) to (51, 9). Visually inspected 1920×1080 and 1280×720 captures and passing test results are in `artifacts/visual-evidence-20261119-152602` and `artifacts/visual-evidence-20261119-152638`.
- The existing Hare Journey launch passed with Bevin's 45×55 Irregular Land settings. The fixed Cross rendering and inspection test also passed. Results are in `artifacts/unity-tests-20261119-152908` and `artifacts/unity-tests-20261119-152928`.
- Batch and Workbench Release builds and the Batch self-test passed.

## Risks and incomplete work

- [Bev-Ticket-3](https://trello.com/c/RTBOG9So/161-bev-ticket-3-irregular-map-shapes) remains In Progress while disconnected islands and their seam rules are open.

## Next useful step

Open the existing LearningIndieDev project. In Forest Edge, select Irregular Land and set Width 68, Height 48, Map Seed 17, Edge Roughness 65, and Interior Hole on to reproduce the capture. Launch MainMenu, continue to Desktop, open Simulation, choose Creek Bend, claim the opening Mutation, and enter Forest Edge.
