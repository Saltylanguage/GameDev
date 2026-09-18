# Tile authoring workflow

Use this guide when adding or changing terrain for the cellular simulation board. It applies to the authored blob-tile families and the simulation's atlas-backed renderer.

## Non-negotiable rule

The neighboring terrain and active simulation atlas define the art contract. A new tile is acceptable only when its edge, interior density, and palette read consistently in the board at gameplay scale.

## Before making art

1. Inspect the current simulation presentation and how it selects terrain sprites.
2. Inspect the exact neighboring source tiles and identify the terrain represented on each edge.
3. Record the visual family, valid neighbor-mask coverage, atlas name, and any fallback behavior before drawing.
4. Choose the smallest source cell or family change that preserves the shared atlas contract.

## Authoring contract

- New terrain cells are 64x64 pixels at 64 pixels per unit. This keeps one cell at one world unit.
- The current `Terrain_01.spriteatlasv2` uses named terrain sprites in `Assets/Art/Terrain/Blob/64/`. Preserve its naming and GUID contract; add source art to the correct family and verify atlas packing rather than introducing runtime resource-name loads.
- Build terrain as individual source cells and valid blob-mask variants, not as a full-board screenshot or a floating prop.
- Use the existing terrain palette and material language first. Generated images may be used only as private concept references; never paste their unrelated texture into a production tile.
- Preserve the established mask and edge language across every required variant; no one sprite should create an unintended seam at a board boundary.
- Do not use a hard geometric mask, rectangular tint, or one-off patch as a shortcut. Judge each variant beside its real neighboring tiles.

## Required preview loop

1. Create the tile sheet outside `Assets/` first, under local-only `artifacts/`.
2. Compose it into a preview with the real neighboring tiles on every side.
3. Inspect the preview at native scale and at the actual game-camera scale.
4. Reject it if any boundary reads as a rectangle, pasted texture, cone, or isolated prop.
5. Only then add accepted source files under `Assets/Art/Terrain/Blob/64/<family>/`, preserve `.meta` files, and confirm the sprite atlas packs the required names.
6. Run the focused terrain asset contract and inspect the current simulation board/terrain preview before calling the art accepted.

## Unity implementation pattern

- Keep terrain presentation in the existing atlas/resolver path; `TerrainTileResolver` owns the normalized 47-mask contract.
- Verify source sprite names, atlas packing, and the board renderer together. Do not add a second importer or a separate runtime loading convention for a new family.

## Verification checklist

- [ ] Every production image has a matching `.meta` file.
- [ ] Sheet dimensions are exact multiples of 64 pixels.
- [ ] The sheet's row order is correct for Unity's lower-left sprite-rect origin.
- [ ] The preview includes the real surrounding tiles, not a neutral background.
- [ ] The required neighbor-mask variants are visible and readable in the actual simulation presentation.
- [ ] `git diff --check` and `.meta` parity pass.
- [ ] Rejected concepts stay under `artifacts/` and are not referenced by runtime code.

## Current example

The Grass terrain family is authored as named 64x64 sprites and packed by
`Terrain_01.spriteatlasv2`. `TerrainTileResolver` selects among the 47 normalized
eight-neighbor masks. The Island Survivor jungle-gate example and its
`WorldRuntime` loader were removed with that slice on 2026-09-18; they are not
part of this workflow.
