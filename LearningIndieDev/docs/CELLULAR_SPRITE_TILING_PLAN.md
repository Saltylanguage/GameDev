# Cellular sprite and smart-tiling plan

## Current implementation

- `tools/Build-CellularSpriteSheets.ps1` is the legacy animal-sheet pipeline and
  still emits 128-pixel sprites. New authored pixel textures use the 64x64 art
  standard. Island Chores art was removed with the retired prototype on
  2026-09-18; other legacy art is not implicitly resized.
- Animal presentation is now scene-wired through a `SpriteAtlas` packed from
  `Assets/Art/Species/Animals/Standardized/32/`; standardized exports also
  exist under `Standardized/64/` and `Standardized/128/`. The board receives
  the atlas and direct sprite inputs through `SpeciesSimulationNoesisHost`;
  it no longer loads animal sheets from `Resources`.
- `Assets/Art/Terrain/Terrain_01_SpriteSheet.png` is a retained legacy source.
  The live terrain atlas packs the 47-mask Grass family under
  `Assets/Art/Terrain/Blob/64/Grass` and resolves sprites by stable name rather
  than pack order. Desert art is not currently present.
- `TerrainTileResolver.ResolveVertexMask` samples the four terrain cells around
  each shared vertex. The immutable board snapshot captures those masks; the
  renderer draws their existing named sprites half a cell offset from the
  simulation grid. The same vertex sampling drives the terrain paint preview.
  This is presentation-only and never changes simulation rules or determinism.
- The species preview no longer has a legacy IMGUI board or settings fallback;
  the Noesis shell is now the single runtime presentation path.
- `TerrainTilePreviewWindow` reads the 64-pixel Grass family. The view model
  loads Grass by stable sprite name; absent Desert names remain empty optional
  slots rather than substituting Grass art.
- New terrain art uses 64x64 source images at 64 PPU. The imported Grass family
  has 47 named sprites matching the normalized resolver masks. `Grass_000` is
  the full-dirt image and `Grass_255` is fully grass, as confirmed by Josh.
- `Terrain_01.spriteatlasv2` packs `Assets/Art/Terrain/Blob/64`. Its existing
  GUID is retained for scene references. The Noesis view model resolves names
  directly, so atlas packing order is not simulation or presentation state.
- Shared vertices give adjacent rendered tiles the same two control points
  along their common edge. A Bare cell remains a dirt control point even when
  surrounded by Grass; an isolated Grass cell remains a grass control point.
  Non-wrapped borders extend the edge cell's terrain to the board clip; wrapped
  boards sample the opposite edge. Animals, input and flourishes stay on the
  simulation-cell grid. Desert retains its separate optional art slots.

## Smart-tiling model

The authored Grass sprites retain their 47-mask naming contract. Its bits are:

```text
N = 1, NE = 2, E = 4, SE = 8, S = 16, SW = 32, W = 64, NW = 128
```

Mask `000` is full dirt and `255` is full grass. The live renderer supplies the
NW/NE/SE/SW corner bits from the four cells surrounding a shared vertex.
Normalization supplies each occupied corner's adjacent cardinal bits to select
the compatible authored sprite. There are 16 four-corner states, all present in
the existing 47-image atlas; no replacement art or runtime image rotation is
required. The images span cell centers and are clipped to the board rectangle.

The earlier center-excluding eight-neighbor mask is retained as per-cell
coverage data for cosmetic flourish eligibility and legacy callers. It no
longer selects the live board or paint-preview terrain image. That old drawing
model duplicated straight boundaries and could hide isolated terrain; see the
October 14 shared-vertex handoff for current validation.

## Planning concerns

The accepted import and biome-ownership triggers are recorded in
[`Planning Concerns/terrain-smart-tiling.md`](Planning%20Concerns/terrain-smart-tiling.md).
Check that record before replacing a family or changing the renderer.

## Validation

- `TerrainTileResolverTests` covers normalization to all 47 variants, diagonal
  promotion, representative rotations, family parity, and neighbor sampling.
- `TerrainTileAssetContractTests` checks all 47 Grass files, 64x64 dimensions,
  64 PPU, point filtering, no mipmaps, uncompressed sprite import settings,
  atlas packing, and every mask name. Full EditMode passed 251/251; the
  focused terrain tests passed 3/3.
- The prototype-scene runtime check confirmed all 47 valid Grass masks load
  from the atlas, ForestEdge provides Grass cells, and missing Desert art remains
  optional. The retained no-graphics PlayMode run passed 28 with two expected
  graphics-only skips. The graphics-capable ForestEdge visual test passed 1/1
  and captured the board at gameplay scale. Results are under
  `artifacts/unity-tests-20260917-222442/` and
  `artifacts/visual-evidence-20260917-222658/`.
- `TerrainTilePreviewWindow` previews all 47 Grass masks from the named files.
- The runtime still uses one batched Noesis board; no Tilemap or `RuleTile`
  dependency was added.
- October 14 adds center-preserving island/hole and straight-boundary tests,
  edge-clamp/wrap tests, immutable vertex-snapshot coverage, actual-art checks
  for all compatible horizontal/vertical vertex edges, and rendered topology
  coverage. The seam regression now checks the shifted terrain edges as well
  as simulation-cell edges. The dated handoff records executed results.

## Remaining validation

1. Review the integrated Grass family in `Salty Game > Simulation > Preview
   Terrain Smart Tiles` and in the prototype at gameplay scale, especially
   isolated, diagonal, edge, and mixed boundaries.
2. Obtain and name the separate universal base tile. Until then, the plain
   fallback remains in use for cells without a terrain-family overlay.
3. Import and validate Desert as its own 64x64 family when authored; do not
   substitute Grass for missing Desert states.
4. Before a production scenario uses Desert, author its simulation definition
   and rules explicitly; smart-tiling does not invent biome behavior.

## Non-goals

- Do not put neighbor masks, atlas indices, or renderer-only state into
  `SpeciesCell` or `CellularSimData`.
- Do not introduce Unity's `RuleTile` package while this remains a batched
  Noesis board.
