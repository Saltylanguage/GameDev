# Forest Edge shared-vertex terrain correction

[Working state](../WORKING_STATE.md) | Status: visually accepted; publication authorized

- Handoff schema: 1
- Handoff ID: 2026-10-14-codex-shared-vertex-terrain-fix
- Owner: codex
- Branch: BevBranch
- Baseline commit: c7724a530ba9709305dbc6d7ae1933a35a0b4a13
- Date: 2026-10-14
- Supersedes: live per-cell terrain sampling described in the September 17 art handoff

## Problem and resulting behavior

The previous board chose each cell's image from its eight neighbors without
considering that cell. Grass and adjacent Bare could select the same edge
image, duplicating a boundary. Isolated Grass selected full dirt at its own
location; a Bare hole surrounded by Grass selected full grass.

Terrain now draws at shared vertices between four simulation-cell centers.
Each of those cells supplies the corresponding corner bit of the image. The
existing normalization resolves those corner bits into the compatible authored
sprite. Adjacent images therefore share the same endpoints along their common
edge. The 16 four-corner states are already present in the 47-image atlas.
No art regeneration or runtime image rotation is needed.

The images are offset half a cell and clipped to the board rectangle. Both the
simulation grid and shifted terrain grid retain screen-pixel-aligned edges.
Non-wrapped borders extend their own terrain to the clip; wrapped boards sample
the opposite edge. Isolated cells and holes remain visible, and straight
boundaries have one transition. Existing art still has a grid-based shape; this
does not redesign the terrain artwork or generate a new biome map.

## Implementation scope

- `TerrainTileResolver.ResolveVertexMask`: four-cell shared-vertex sampling.
- `SimulationBoardSnapshot`: captures immutable Grass/Desert vertex masks;
  later live-grid changes cannot change a captured terrain render.
- `SpeciesSimulationBoard`: draws vertex-aligned terrain in the existing batched
  Noesis control before cell-aligned flourishes and animals. Impassable cells
  retain their black overlay. Atlas references, source PNGs and GUIDs are intact.
- `TerrainPaintPreview`: uses the same shared-vertex sampling and clipping.
- Existing per-cell `TerrainVariantMask` is retained for cosmetic flourish
  eligibility and legacy callers; it no longer chooses the live terrain image.
- No simulation rules, scenario values, creature placement or input mapping change.
  Desert remains optional and unvalidated as an authored biome.

## Verification

- Unity 6000.4.6f1, Direct3D12: compilation completed without errors.
- Focused EditMode: TerrainTile 15/15, BoardSnapshot 2/2 and TerrainPaintPreview
  1/1 (18 distinct checks). Covers straight boundaries, islands/holes, edge
  extension/wrapping and snapshot immutability. The actual-art contract compares
  all 64 compatible horizontal and 64 compatible vertical edge pairings across
  the 16 corner states, with no mismatched grass/dirt edge pixels.
- Presentation PlayMode: 27/27. A new actual-art rendered regression checks
  terrain at cell centers for a straight boundary, isolated Grass and a Bare hole.
  The seam regression checks shifted terrain edges plus original cell edges at
  four zooms, fractional pan and UI scaling.
- Desktop visual capture: 1/1 at 1920x1080. Inspected the normal Forest Edge view
  and controlled-pattern captures. The normal before/after screenshots both show
  seed -1628811672, tick 1 and the same displayed population counts. Terrain
  changes while animals/flourishes stay aligned to the simulation cells.
- Negative control: temporarily restoring the old per-cell drawing path causes
  the new rendered test to fail at Bare center (4,2) in the straight-boundary
  case (expected dirt, observed grass). The corrected renderer is restored
  byte-for-byte from its already-passing copy; the focused regression passes 1/1
  again after restoration.
- Initial diagnostic run: cached discovery ran the previous 26 tests and the
  expanded seam sampling hit a decorative pebble. The failure image confirms
  opaque green probe coverage. The test now excludes only the known occupied
  meadow/pebble regions, preserving coverage of terrain edges. Forced compilation
  picked up the new test; the subsequent 27-test suite passed. Initial and
  negative-control failures are retained as diagnostic evidence.
- Final Console ground truth reports zero errors, zero warnings and no compile
  failure. Visual environment overrides are cleared; Unity returns to stopped
  MainMenu and the validation Editor is closed afterward.

Local ignored evidence: `artifacts/terrain-fix-20261014/`, including EditMode
results, `presentation-playmode.json`, `desktop-visual.json`, `negative-control.json`,
`negative-control.png`, restored regression results, `candidate-comparison.png`,
`desktop-before-after.png`, normal Desktop captures and controlled topology images.
The earlier read-only diagnosis remains in `artifacts/terrain-audit-20261014/`.

## Limits and handoff

Bevin tested the change himself on October 14, confirmed that it looks much
better, and explicitly authorized pushing it. Git history identifies the
publication commit; the related Trello art-card comment and Obsidian records
receive the verified remote checkpoint after pushing.

No full Unity suite, player build, performance profile, new balance experiment
or Desert visual acceptance was performed. The source grid still determines
the angular layout; this corrects sampling and continuity, not landscape design.

Validation uses the connected Pipeline commands directly because the existing
`CellSim.ps1` wrapper still lacks `Resolve-UnityExecutionLane`; no tooling repair
was included. Production art, scenes, project settings and simulation data are
outside this change. Unity-generated metadata whitespace is normalized without
changing GUIDs or importer values before handoff.
