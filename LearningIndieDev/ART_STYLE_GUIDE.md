# Island Chores Art Direction v1

## Direction

The prototype now uses crisp top-down pixel art inspired by the supplied references: hard pixel clusters, limited warm tropical colors, dark outlines, readable silhouettes, and no anti-aliased vector shapes.

## Grid contract

- Character and prop cells are 128x128 pixels.
- Tile cells are 128x128 pixels.
- Both atlases are 4x4 sheets and use nearest-neighbor filtering.
- Sprite atlases use 128 pixels per world unit.
- Backgrounds should use repeated tiles where a surface needs to scale; individual sprites remain appropriate for interactive props.
- Interactive terrain MUST be a tile state, not a floating prop over an unrelated ground tile. Its blocked and cleared states must share the same grid, scale, and edge treatment as their neighboring terrain.
- A blocked terrain state can conceal the underlying route; the cleared state reveals the route. Do not show a traversable-looking route before its gameplay gate is cleared.

## Current assets

- `Assets/Resources/Art/IslandChores_ArtAtlas128.png`: characters and props.
- `Assets/Resources/Art/IslandChores_TileAtlas128_SeamSafe.png`: seam-safe sand, water, shoreline, jungle, and path tiles used at runtime. `IslandChores_TileAtlas128.png` remains the untouched source atlas.
- `Assets/Resources/Art/IslandChores_JungleEntranceClosedTiles128.png` and `IslandChores_JungleEntranceOpenTiles128.png`: authored 3x2 tile sets for the blocked and cleared jungle entrance. The six cells use the existing canopy and beach pixel language; the opening is a center-cell state, not a floating prop or a full-scene texture.

## Scope boundary

This is a first-pass art foundation, not a final asset pipeline. The next useful step is a focused in-game review of scale, contrast, and tile seams; only then should we add animation frames or split jungle foreground pieces.

For the required authoring and preview loop for terrain transitions, see [`docs/TILE_AUTHORING_GUIDE.md`](docs/TILE_AUTHORING_GUIDE.md).

## Cellular simulation species glyphs

The cellular simulation board has a separate iconography direction from the
retained Island Chores pixel-art slice. Its target is the supplied colorized
animal reference: flat, geometric silhouettes with strong readability at small
sizes and a distinctive feature per species. Keep role colors stable (green
plants, blue herbivores, red carnivores) and use shape/accent differences for
species identity. Monochrome silhouette references may guide the contour, but
the colorized reference is the preferred presentation target.
