# Handoff — Terrain art standard 64px

**Date:** 2026-09-17  
**Owner:** Codex  
**Status:** Grass runtime integration complete; Desert art remains pending

## Decision

New terrain tile source images are 64x64 pixels and imported at 64 pixels per
unit. This keeps each tile one world unit. The retained Island Chores atlases
and other explicitly legacy art remain at their existing resolutions unless a
separate migration is approved.

## Implementation

- Established 64x64 terrain sources at 64 PPU, keeping each tile one world unit.
- The resolver still uses the existing 47 normalized mask IDs and remains
  presentation-only.
- Generated `Assets/Art/Terrain/Blob/64/Grass/Grass_###.png` for all 47 valid
  masks from the 14 authored representatives in
  `Assets/UI/GalapagOS/Art/Biomes/Tilesets/Forest`. Each output is an exact
  rotation and/or reflection of its representative; no runtime image transform
  was added. The authored sources were left untouched.
- Outputs import as single 64x64 sprites at 64 PPU, point filtering, no mipmaps,
  uncompressed, with clamp wrapping.
- Recreated `Assets/Art/Terrain/Terrain_01.spriteatlasv2` at its existing GUID so
  scene references continue to resolve; it packs `Assets/Art/Terrain/Blob/64`.
- The simulation view model loads named Grass sprites by mask and requires all
  47 Grass entries. Missing Desert entries are optional and do not disable Grass
  rendering or get replaced with Grass art.
- The board applies the normalized mask through its existing batched Noesis
  renderer. The authored `Grass_000` is full dirt; `Grass_255` is full grass.
- ForestEdge's zero explicit plant count previously suppressed its authored
  Grass probability when the simulation applied other species' starting counts.
  Zero counts are now omitted from that override, so the ForestEdge grid contains
  Grass for the new terrain renderer while positive explicit counts are unchanged.

## Terrain art handoff status

The 14 source files map to representative masks as follows:

| Source | Mask |
| --- | ---: |
| `Biome_Grass_0` | 000 |
| `Biome_Grass_1` | 001 |
| `Biome_Grass_2` | 005 |
| `Biome_Grass_3` | 007 |
| `Biome_Grass_4` | 017 |
| `Biome_Grass_5` | 021 |
| `Biome_Grass_6` | 029 |
| `Biome_Grass_7` | 031 |
| `Biome_Grass_8` | 085 |
| `Biome_Grass_9` | 087 |
| `Biome_Grass_10` | 095 |
| `Biome_Grass_11` | 119 |
| `Biome_Grass_12` | 127 |
| `Biome_Grass_13` | 255 |

These representatives cover all 47 valid resolver masks under rotations and
reflections. Josh confirmed the semantics: `000` is full dirt (all grass
vertices off) and `255` is the entire texture filled with grass. The Desert
family is not present yet; missing Desert entries remain empty until that art is
authored.

## Validation

Unity batch generation confirmed exactly the 47 resolver filenames and checked
each Grass import for 64x64 dimensions, 64 PPU, point filtering, uncompressed
single-sprite import, no mipmaps, and stable sprite names. Focused Unity checks
passed: terrain asset/atlas EditMode tests 3/3 and the prototype-scene Grass
runtime initialization test 1/1. The atlas exposes every Grass mask; `000` and
`255` were visually checked against the confirmed full-dirt/full-grass meaning.
The full retained EditMode suite passed 251/251. The retained no-graphics
PlayMode run passed 28 tests with two expected graphics-only skips; its results
are under `artifacts/unity-tests-20260917-222442/`. The focused graphics-capable
ForestEdge scene/visual test passed 1/1 and captured setup, running, rewards, and
results at 1280x720 under `artifacts/visual-evidence-20260917-222658/`. The
board now visibly uses both Grass and Dirt shapes. The full graphics-only suite
was not rerun after the last scenario-population fix.

## Next safe step

Review all integrated masks in the editor preview and at gameplay scale on the
target resolutions. When Desert art is authored, map its representatives to the
same resolver masks, add it to the atlas, and run its asset and runtime contract
checks. Do not use Grass as a fallback for missing Desert sprites.
