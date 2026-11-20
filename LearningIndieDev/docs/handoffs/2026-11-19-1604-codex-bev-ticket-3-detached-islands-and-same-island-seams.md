# Bev Ticket 3 detached islands and same island seams

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-11-19-1604-codex-bev-ticket-3-detached-islands-and-same-island-seams
- Owner: Codex
- Branch: codex/bev-ticket-3-irregular-maps
- Baseline commit: b55c62c
- Date: 2026-11-19
- Supersedes: island status from the [coastline handoff](2026-11-19-1525-codex-bev-ticket-3-organic-coastlines.md)

## Summary

Fixed-shape [PR #9](https://github.com/Saltylanguage/GameDev/pull/9) merged into `ProjectMain` at `0cefc64`. The Bev-Ticket-3 branch includes that merge. Irregular Land can now add a detached island, and a wrapped step always ends on the same landmass.

## Changes

- **Detached Island** adds one small seeded island beside the main landmass when Width is at least 24 and Height at least 16. It defaults off. The same settings reproduce the same cells and seams.
- Generated terrain and authored maps use outer row and column endpoints from each connected landmass. Landmass detection uses direct movement adjacency, including diagonals. The generator separates the mainland and island with at least one empty column. Explicit seam definitions also require both endpoints to be on the same landmass.
- The mainland can retain an **Interior Hole**. Missing cells are excluded from the board and simulation. Portable snapshots carry the generated cells and seam pairs.
- Hare spatial reporting visits playable cells and uses the wrapped topology for neighbor queries. Its per-cell visit arrays use the grid storage size.

## Decisions and assumptions

- Bevin clarified that a wrap must stay on the island it began on. Water topology is tracked under [VS-A1](https://trello.com/c/wsmjVY20).
- Forest Edge Inspector edits belong to the local playtest. The shared scenario asset retains its committed settings.

## Validation

- The clean island implementation suite passed 827 EditMode tests with one skip and 105 PlayMode tests with seven skips. The sole PlayMode failure was `FieldDataAuthoredRouteReachesFirstShopAndResetsNode` at seed 10101, tick 288. Its failure message and recorded populations exactly match the fixed-shapes baseline. Both suites are in `artifacts/organic-coast-validation-20261119-152618/artifacts/unity-tests-20261119-160530`.
- Live Unity passed all 15 topology tests, the authoring test for deterministic islands, and all five portable provenance tests. Results are in `artifacts/unity-tests-20261119-160354`, `artifacts/unity-tests-20261119-160400`, and `artifacts/unity-tests-20261119-205144`.
- MainMenu live tests and inspected captures at 1920×1080 cover the rectangle and the irregular island board. The same controlled hare wrapped on the mainland while the detached island shared that row. Both the board snapshot and the movement topology excluded the separating gap. Results are in `artifacts/visual-evidence-20261119-205146` and `artifacts/visual-evidence-20261119-205243`.
- Batch and Workbench Release builds and the Batch self-test passed.
- The Unity export and standalone Batch replay of the same island layout matched the frozen snapshot fingerprint and all three final-state digests for seeds 10100 through 10102. The broader run comparator also passed after normalizing Unity's unset research milestone fields, all `-1`, to the standalone `null` form. The original outputs and the normalized comparison are retained under `artifacts/bev-island-parity-20261119/candidate`. The final-source live portable tests passed 5/5 in `artifacts/unity-tests-20261119-205702`.

## Risks and incomplete work

- The pre-existing Forest Edge ecology failure remains tracked with the authored Journey work.

## Next useful step

For the island playtest, set Forest Edge to Irregular Land at Width 68, Height 48, Map Seed 17, Edge Roughness 65, with Interior Hole and Detached Island enabled. From MainMenu continue to Desktop, open Simulation, choose Creek Bend, claim the first Mutation, and enter Forest Edge. The [Trello checklist](https://trello.com/c/RTBOG9So/161-bev-ticket-3-irregular-map-shapes) calls for regular and irregular map captures on the integrated `ProjectMain` revision before closeout.
