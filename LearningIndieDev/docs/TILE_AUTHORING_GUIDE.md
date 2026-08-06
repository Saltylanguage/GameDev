# Tile authoring workflow

Use this guide when adding or changing terrain that must visually join existing sprites, textures, or tiles. It applies especially to progression gates such as the jungle entrance.

## Non-negotiable rule

The surrounding tiles define the art contract. A new tile is acceptable only when it reads as part of that terrain in context at gameplay scale. Matching an image's outer pixels is necessary, but it is not sufficient if the interior has a different density, palette, or silhouette.

## Before making art

1. Inspect the live scene composition: target position, visual root, sort order, and what changes when the interaction completes.
2. Inspect the exact neighboring atlas cells and identify the terrain on every edge of the new work.
3. Write down the states before drawing: for example, `closed = overgrown`, `open = route visible`, and `reset = closed again`.
4. Choose the smallest tile footprint that can carry the change. A route crossing a terrain boundary normally needs side, center, and lower transition cells - not one special square.

## Authoring contract

- Keep the established cell size and pixels-per-unit. Island Chores terrain cells are 128x128 pixels at 128 PPU.
- Terrain cells rendered beside one another MUST use the shared one-pixel sprite mesh extrusion in `WorldRuntime`; otherwise camera sampling can expose seams even when the source pixels are correct. If a seam is baked into a tile's outer pixel columns, create a versioned seam-safe atlas that repairs only those edge pixels; preserve the original atlas unchanged.
- Build a multi-cell terrain feature as a tile sheet, not as a full-scene screenshot or a floating prop.
- Use the existing terrain palette and material language first. Generated images may be used only as private concept references; never paste their unrelated texture into a production tile.
- Preserve the exact outer edge pixels from the neighboring source cells where the feature touches repeated terrain.
- Make side cells carry the transition. The center cell alone must not be responsible for making a path, doorway, or shoreline feel natural.
- Do not use a hard geometric mask, a rectangular tint, or a single dirt triangle as a shortcut. If the center state still reads as a stamp, expand the tile footprint or redraw the transition.
- Keep blocked and open states on the same grid and with the same outer edges. The state change may affect the center and local fringe, never the entire surrounding world.

## Required preview loop

1. Create the tile sheet outside `Assets/` first, under local-only `artifacts/`.
2. Compose it into a preview with the real neighboring tiles on every side.
3. Inspect the preview at native scale and at the actual game-camera scale.
4. Reject it if any boundary reads as a rectangle, pasted texture, cone, or isolated prop.
5. Only then copy the accepted tile sheet into `Assets/Resources/Art/`, add its `.meta`, and wire the individual cells into `WorldRuntime`.
6. Run a clean Play Mode check for every state transition and reset before calling the art accepted.

## Unity implementation pattern

- The interactable owns two local visual roots: blocked and cleared.
- Build each terrain state from individual cell sprites. `WorldRuntime.MakeTextureTileField` is the current project helper for a fixed-size tile sheet.
- Keep the blocked visual depth-sorted with the world interaction root. Keep the cleared route in the background layer unless gameplay needs it to occlude characters.
- Use the same cell origin, dimensions, and sorting contract for both states.
- Preserve the interaction mechanic while iterating art, but do not leave an invisible gate or an unverified placeholder as a final state.

## Verification checklist

- [ ] Every production image has a matching `.meta` file.
- [ ] Sheet dimensions are exact multiples of 128 pixels.
- [ ] The sheet's row order is correct for Unity's lower-left sprite-rect origin.
- [ ] The preview includes the real surrounding tiles, not a neutral background.
- [ ] Closed, open, and reset states are all visible and readable in Play Mode.
- [ ] `git diff --check` and `.meta` parity pass.
- [ ] Rejected concepts stay under `artifacts/` and are not referenced by runtime code.

## Current example

`JungleEdgeInteractable` swaps a closed terrain tile set for an open `Jungle Exit Route` set. The mechanic and terrain art stay local to that feature. Do not generalize this into a world-state framework until another terrain gate demonstrates a shared need.
