# Cellular sprite and smart-tiling plan

## Current implementation

- `tools/Build-CellularSpriteSheets.ps1` converts the supplied reference sheets
  into transparent, nearest-neighbor atlases at 128 pixels per tile.
- Animal presentation is now scene-wired through a `SpriteAtlas` packed from
  `Assets/Art/Species/Animals/Standardized/32/`; standardized exports also
  exist under `Standardized/64/` and `Standardized/128/`. The board receives
  the atlas and direct sprite inputs through `SpeciesSimulationNoesisHost`;
  it no longer loads animal sheets from `Resources`.
- `Assets/Art/Terrain/Terrain_01_SpriteSheet.png` is the retained 4x8 legacy
  source. The live terrain atlas now packs the 47-mask families under
  `Assets/Art/Terrain/Blob/128/{Grass,Desert}` and resolves them by stable name
  rather than pack order.
- `TerrainTileResolver` computes a normalized eight-neighbor blob bit mask from the
  simulation grid. It is presentation-only: it reads the immutable cell state
  and never changes simulation rules or determinism. The same mask table is
  shared by the runtime board, tests, and the editor preview window.
- The species preview no longer has a legacy IMGUI board or settings fallback;
  the Noesis shell is now the single runtime presentation path.
- `TerrainTilePreviewWindow` loads the named terrain families from
  `Assets/Art/Terrain/Blob/128/{Grass,Desert}`, and animal atlas entries are
  resolved by stable sprite names before optional Fox/Rabbit scene overrides
  are layered on top.
- Terrain art is now a named 47-mask blob set under
  `Assets/Art/Terrain/Blob/128/{Grass,Desert}/`. Each family uses stable
  `Grass_` or `Desert_` names matching the normalized resolver masks.
- `Terrain_01.spriteatlasv2` packs the blob terrain folder. The Noesis
  view model resolves the named sprites directly, so atlas packing order is not
  simulation or presentation state.
- Grass and Desert are peer visual families. The cell's explicit `TerrainId`
  selects the family and both use the computed mask. Bare owns the neutral
  layer underneath them and does not borrow either family's tiles.

## Smart-tiling model

The current sprites represent the 47 normalized states of an eight-neighbor
blob mask. The eight-bit mask is:

```text
N = 1, NE = 2, E = 4, SE = 8, S = 16, SW = 32, W = 64, NW = 128
```

The current placeholder mask `0` is transparent. Chrono's delivered `000` tile
must be checked before replacement because it may instead represent the filled
isolated member of the family. Other raw masks are normalized for diagonal
bridges, then resolved through the 47 named variants in `TerrainTileResolver`.
The board samples the eight neighboring cells around each visual tile and keeps
the mask presentation-only, so it does not alter simulation determinism.

Both families use the same naming and mask table. The live board first draws a
neutral universal base, then draws the matching Grass or Desert mask for that
cell. The base is currently a plain brown fallback; its production tile is a
separate art delivery and must not be taken from either 47-mask family.

## Planning concerns

The accepted import and biome-ownership triggers are recorded in
[`Planning Concerns/terrain-smart-tiling.md`](Planning%20Concerns/terrain-smart-tiling.md).
Check that record before replacing a family or changing the renderer.

## Validation

- `TerrainTileResolverTests` covers normalization to all 47 variants, diagonal
  promotion, representative rotations, family parity, and neighbor sampling.
- `TerrainTileAssetContractTests` checks both 47-file families, exact runtime
  names, 128x128 dimensions, sprite import settings, and the atlas packable.
- `TerrainTilePreviewWindow` previews all 47 masks from the named files and can
  switch between `Grass_` and `Desert_` families.
- The runtime still uses one batched Noesis board; no Tilemap or `RuleTile`
  dependency was added.

## Remaining validation

1. Confirm whether Chrono's `000` and diagonal states match the resolver's mask
   meaning before replacing production images.
2. Obtain and name the separate universal base tile. Until then, the plain
   fallback keeps presentation ownership correct but is not final art.
3. Import the two peer families while preserving their existing `.meta` files, then
   let Unity rebuild the atlas and run the asset-contract tests.
4. Open `Salty Game > Simulation > Preview Terrain Smart Tiles` and inspect all
   masks in both families.
5. Run the cellular prototype at 1920x1080 and 1280x720 and inspect isolated,
   diagonal, edge, and mixed boundaries at normal gameplay speed.
6. Before a production scenario uses Desert, author its simulation definition
   and rules explicitly; the smart-tiling change does not invent movement or
   resource behavior for the biome.

## Non-goals

- Do not put neighbor masks, atlas indices, or renderer-only state into
  `SpeciesCell` or `CellularSimData`.
- Do not introduce Unity's `RuleTile` package while this remains a batched
  Noesis board.
