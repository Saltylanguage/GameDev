# Blob terrain tiles

This folder contains the first generated 47-mask blob terrain set for the
runtime auto-tiler.

- `128/Grass/` contains 47 grass masks.
- `128/Desert/` contains 47 desert masks.
- Each sprite is 128 x 128 pixels at 128 pixels per unit.
- Masks use the Wang/blob weights `N=1, NE=2, E=4, SE=8, S=16, SW=32, W=64, NW=128`.
- The 47 outputs are generated from the 15 rotationally canonical masks:
  `0, 1, 5, 7, 17, 21, 23, 29, 31, 85, 87, 95, 119, 127, 255`.

The source and generation contract is documented by
`tools/Generate-BlobTerrainTiles.ps1`. These are presentation assets only;
`TerrainTileResolver` and the runtime atlas consume the same normalized 47-mask
contract.
