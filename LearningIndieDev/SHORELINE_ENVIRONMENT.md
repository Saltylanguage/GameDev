# Shoreline Environment First Pass

## What was added

- A layered shoreline composition in `WorldRuntime`: ocean and foam at the bottom, a clear sandy playable band in the middle, and a horizontal jungle border across the top.
- Repeating wave marks and a shoreline foam strip to give the water a small amount of visual life without making the beach noisy.
- Sparse washed-up planks and a second beach rock for environmental storytelling.
- Washed-up planks use a fixed low render order, so characters always appear in front of them while the planks remain separate beach props.
- Foliage clusters and the existing tree/jungle-edge interactions moved into the back of the beach so the space reads as a tropical island rather than an abstract clearing.
- Separate `Background Layer`, `Playable Beach Layer`, and `Foreground Foliage Layer` roots so later jungle depth treatment can split background foliage, walkable ground, and foreground foliage.

## Assumptions

- The current orthographic camera and player movement clamp define the playable bounds for this prototype. Physical colliders are intentionally not added yet because the scene has no blocked routes, cliffs, or solid structures that need collision rules.
- The authored atlas remains optional; the runtime primitive fallback still produces the same layout if the atlas is unavailable.

## Next logical polish

Play the Bootstrap scene and tune the shoreline height, foliage density, and prop positions at gameplay scale. After the layout is approved, split the jungle edge into explicit background and foreground pieces, then add collision only where those pieces create a real traversal boundary.
