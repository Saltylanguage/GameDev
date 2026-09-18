# Cellular sprite and smart-tiling plan

## Current implementation

- `tools/Build-CellularSpriteSheets.ps1` is the legacy animal-sheet pipeline and
  still emits 128-pixel sprites. New authored pixel textures use the 64x64 art
  standard; retained Island Chores and other legacy art are not implicitly
  resized.
- Animal presentation is now scene-wired through a `SpriteAtlas` packed from
  `Assets/Art/Species/Animals/Standardized/32/`; standardized exports also
  exist under `Standardized/64/` and `Standardized/128/`. The board receives
  the atlas and direct sprite inputs through `SpeciesSimulationNoesisHost`;
  it no longer loads animal sheets from `Resources`.
- `Assets/Art/Terrain/Terrain_01_SpriteSheet.png` is a retained legacy source.
  The live terrain atlas packs the 47-mask Grass family under
  `Assets/Art/Terrain/Blob/64/Grass` and resolves sprites by stable name rather
  than pack order. Desert art is not currently present.
- `TerrainTileResolver` computes a normalized eight-neighbor blob bit mask from the
  simulation grid. It is presentation-only: it reads the immutable cell state
  and never changes simulation rules or determinism. The same mask table is
  shared by the runtime board, tests, and the editor preview window.
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
- Every passable cell resolves a tile mask. Bare cells use the Grass neighbor
  mask, so a dirt cell surrounded by Grass receives the right full or partial
  Grass shape instead of remaining an un-tiled brown square. Bare remains a
  distinct terrain when neighbor occupancy is sampled. Desert keeps its own
  family and never falls back to Grass art.

## Smart-tiling model

The Grass sprites represent the 47 normalized states of an eight-neighbor blob
mask. The eight-bit mask is:

```text
N = 1, NE = 2, E = 4, SE = 8, S = 16, SW = 32, W = 64, NW = 128
```

Mask `000` is the full-dirt image (no grass vertices); mask `255` is entirely
grass. Other raw masks are normalized for diagonal bridges, then resolved
through the 47 named variants in `TerrainTileResolver`. The board samples the
eight neighboring cells at every visual tile, including Bare dirt. Mask `000`
is used only when none of those neighbors are Grass; a Bare center surrounded
by Grass resolves to `255`. This stays presentation-only and does not alter
simulation determinism.

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
- Regression tests now cover a Bare center surrounded by Grass in both the
  resolver and board snapshot. Post-fix Unity validation is pending: the
  repository preflight found the Unity editor already open and refused to run
  tests, rather than closing it or risking its state.

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
