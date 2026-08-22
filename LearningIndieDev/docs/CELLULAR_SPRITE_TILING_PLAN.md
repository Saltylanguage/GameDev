# Cellular sprite and smart-tiling plan

## Current implementation

- `tools/Build-CellularSpriteSheets.ps1` converts the supplied reference sheets
  into transparent, nearest-neighbor atlases at 128 pixels per tile.
- Animal presentation is now scene-wired through a `SpriteAtlas` packed from
  `Assets/Art/Species/Animals/Standardized/32/`; standardized exports also
  exist under `Standardized/64/` and `Standardized/128/`. The board receives
  the atlas and direct sprite inputs through `SpeciesSimulationNoesisHost`;
  it no longer loads animal sheets from `Resources`.
- `Assets/Art/Terrain/Terrain_01_SpriteSheet.png` remains the 4x8 authored source
  sheet: grass variants occupy rows 0-3 and desert variants rows 4-7. The sheet
  is split into named per-tile exports under
  `Assets/Art/Terrain/Standardized/` at source, 32, 64, and 128 pixels. The
  runtime `Terrain_01` SpriteAtlas packs the 128-pixel folder and resolves tiles
  by stable names rather than pack order. The current board uses the grass half
  for grass and temporarily maps bare terrain to the desert half until a
  dedicated bare-ground atlas is authored.
- `TerrainTileResolver` computes a four-cardinal-neighbor bit mask from the
  simulation grid. It is presentation-only: it reads the immutable cell state
  and never changes simulation rules or determinism. The same mask table is
  shared by the runtime board, tests, and the editor preview window.
- The species preview no longer has a legacy IMGUI board or settings fallback;
  the Noesis shell is now the single runtime presentation path.
- `TerrainTilePreviewWindow` loads the current terrain sheet from
  `Assets/Art/Terrain`, and animal atlas entries are resolved by stable sprite
  names before optional Fox/Rabbit scene overrides are layered on top.
- Animal presentation remains scene-wired through a `SpriteAtlas` and stable
  sprite names; the board does not load art from `Resources`.
- Terrain art is now a named 15-piece dual-grid set under
  `Assets/Art/Terrain/Standardized/128/`. The grass family uses `Grass_` names
  and the matching desert family uses `Desert_` names.
- `Terrain_01.spriteatlasv2` packs the standardized terrain folder. The Noesis
  view model resolves the named sprites directly, so atlas packing order is not
  simulation or presentation state.
- `TerrainTileResolver` is presentation-only. It reads the simulation grid and
  never changes `SpeciesCell` or the deterministic simulation rules.

## Smart-tiling model

The new sprites represent the 15 non-empty combinations of four surrounding
simulation cells. The four-bit mask is:

```text
SW = 1, SE = 2, NW = 4, NE = 8
```

Mask `0` draws no transition tile. Masks `1` through `15` map directly to the
named variants (`DiagBottomLeft` through `Full`) in `TerrainTileResolver`.
The board samples the four cells around each visual tile, with the current cell
as the north-east corner, so the species icon and terrain remain centered in
the same board cell while the edge shape comes from the dual-grid combination.

Both grass and desert use the same mask rules. `Bare` continues to use the
desert family as its temporary visual family until a dedicated bare set exists.

## Validation

- `TerrainTileResolverTests` covers empty masks, all 15 named variants, corner
  sampling, grass/desert parity, sprite naming, and invalid masks.
- `TerrainTilePreviewWindow` previews all 16 masks from the named files and can
  switch between `Grass_` and `Desert_` families.
- The runtime still uses one batched Noesis board; no Tilemap or `RuleTile`
  dependency was added.

## Remaining validation

1. Let Unity reimport the renamed desert sprites and rebuild the SpriteAtlas.
2. Open `Salty Game > Simulation > Preview Terrain Smart Tiles` and confirm
   each mask shows the expected named texture.
3. Run the cellular prototype and inspect mixed grass/bare boundaries at normal
   gameplay scale. If a shape is wrong, change the named mask table in the
   resolver, not simulation state.
4. Add a dedicated bare-ground family when that art is available.

## Non-goals

- Do not put neighbor masks, atlas indices, or renderer-only state into
  `SpeciesCell` or `CellularSimData`.
- Do not introduce Unity's `RuleTile` package while this remains a batched
  Noesis board.
