# Cellular sprite and smart-tiling plan

## Current implementation

- `tools/Build-CellularSpriteSheets.ps1` converts the supplied reference sheets
  into transparent, nearest-neighbor atlases at 128 pixels per tile.
- Animal presentation is now scene-wired through a `SpriteAtlas` packed from
  `Assets/Art/Species/Animals/Standardized/32/`; standardized exports also
  exist under `Standardized/64/` and `Standardized/128/`. The board receives
  the atlas and direct sprite inputs through `SpeciesSimulationNoesisHost`;
  it no longer loads animal sheets from `Resources`.
- `Assets/Art/Terrain/Terrain_01_SpriteSheet.png` is a 4x8 atlas: grass
  variants occupy rows 0-3 and desert variants rows 4-7. The current board uses
  the grass half for grass and temporarily maps bare terrain to the desert half
  until a dedicated bare-ground atlas is authored.
- `TerrainTileResolver` computes an eight-neighbor bit mask (N/E/S/W plus the
  four diagonals) from the simulation grid. It is presentation-only: it reads
  the immutable cell state and never changes simulation rules or determinism.
  The current 16-tile atlas still uses the cardinal projection as its explicit
  fallback, while open diagonal corners are reported for the next art pass.
- The species preview no longer has a legacy IMGUI board or settings fallback;
  the Noesis shell is now the single runtime presentation path.

## Smart-tiling model

For each grass cell, compute a mask with `N=1`, `E=2`, `S=4`, and `W=8`.
The resolver maps that mask to the current atlas order. Because the source art
is arranged visually rather than in bit-mask order, the lookup table is the
single art-layout seam. Replacing or reordering the atlas only requires changing
that table; simulation code and cell data stay untouched.

The resolver now performs the eight-neighbor pass at this presentation
boundary. The 16 base variants remain the deliberate fallback because the
current atlas cannot represent every diagonal combination. `GetOpenDiagonalCorners`
identifies the corner states that require authored diagonal variants; do not
silently treat those states as fully connected when evaluating new art.

## Completed in this pass

- Desert-family rendering now uses the same resolver and atlas offset 16;
  `Bare` currently shares that family as a temporary visual mapping.
- `Salty Game > Simulation > Preview Terrain Smart Tiles` renders all 16 masks
  in a labelled 4x4 grid and can switch between grass and desert families.
- Edit Mode coverage now checks all 16 unique mappings, invalid masks, and the
  shared resolver behavior for a desert terrain family.
- Diagonal mask extraction and open-corner detection are covered by focused
  resolver tests. The existing atlas output remains cardinal-projected until
  diagonal corner art is authored.

## Remaining validation and art work

1. Update `TerrainTilePreviewWindow` from the deleted `Resources` path to the
   current terrain asset/SpriteAtlas, then run all 16 masks in Unity.
2. Verify stable species mapping and merge direct Fox/Rabbit overrides with the
   atlas fallback, or explicitly scope the scene to those two species and test
   every authored species that can appear.
3. Author and wire diagonal corner variants (a 47/48-state blob atlas or an
   equivalent corner-overlay solution), then update the resolver's art seam.
4. Open the preview and cellular prototype in Unity at gameplay scale. If an
   edge or corner is wrong, adjust only the presentation lookup/overlay seam.
5. Replace the temporary bare-to-desert mapping with an authored bare-ground
   tileset when that art is available.
6. Add authored plant sprites to the animal atlas or a dedicated plant atlas;
   until then, plant-resource terrain is represented by grass and avoids an
   unrelated animal icon.
7. Add a Play Mode screenshot check once Unity is available in the development
   environment. This should verify atlas loading, terrain seams, and icon scale
   in the actual Noesis view rather than only in the editor preview.

## Non-goals

- Do not put neighbor masks, atlas indices, or renderer-only state into
  `SpeciesCell` or `CellularSimData`.
- Do not introduce Unity's `RuleTile` package for this board yet. The board is a
  batched Noesis control and a small deterministic lookup is sufficient until
  the simulation becomes a scene-authored Tilemap.
