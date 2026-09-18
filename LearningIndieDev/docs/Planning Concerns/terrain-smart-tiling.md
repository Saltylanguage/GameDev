# Planning concerns — Terrain smart tiling

**Scope:** Importing and presenting Chrono's Grass and Desert terrain tiles in
the 47-mask smart-tiling path. This record covers mask meaning, asset naming,
atlas integration, and biome presentation ownership. It excludes simulation
terrain rules and a general Tilemap or RuleTile migration.
**Canonical plan:** [`../CELLULAR_SPRITE_TILING_PLAN.md`](../CELLULAR_SPRITE_TILING_PLAN.md)
**Human owner:** Josh
**Status:** Active

## Active concerns

### TILE-C01 — Artist and resolver mask meanings differ

- **Severity:** Mild
- **Status:** Grass mapping reviewed; empty-cell renderer fix pending validation; recheck Desert when authored
- **Trigger:** A terrain family is imported before its representative shapes and rotations are checked against the resolver's promoted-cardinal mask convention.
- **Why it matters:** Isolated terrain can disappear into the base layer, or diagonal contacts can render bridges and edges that the artist did not intend.
- **Evidence:** Josh reviewed the Forest representatives individually and in batches against the 47 resolver masks, correcting the rotated-image interpretation before confirming the mappings. He confirmed `000` is full dirt (grass vertices off) and `255` is full grass. The generated Grass set is loaded through the runtime atlas and passed focused asset and scene tests. Desert art has not been authored/validated.
- **Smallest mitigation:** Repeat the representative-shape and rotation review for Desert before importing it. Keep each family's source mapping explicit and validate the resulting 47 named masks.
- **Progress:** The Grass family is integrated at 64x64 / 64 PPU. The terrain atlas preserves its scene-referenced GUID and packs the new `Blob/64` folder. A follow-up fixed Bare cells so they receive neighbor-derived Grass masks; regression tests were added, but the Unity preflight refused to run while the editor was open, so post-fix validation remains pending.
- **Owner:** Josh, with Chrono and the terrain integration owner
- **Recorded:** 2026-09-09, accepted by Josh in the planning conversation

### TILE-C02 — Desert is called integrated before it has a live biome owner

- **Severity:** Mild
- **Status:** Acknowledged
- **Trigger:** Desert is described as production-ready before authored Desert tiles and a selectable Desert scenario are available.
- **Why it matters:** Forty-six Desert variants can remain unused while screenshots and asset checks give the impression that Desert biome tiling is complete.
- **Evidence:** The resolver and renderer retain a Desert family slot, but no Desert tile images are authored or packed, and the default simulation terrain registry contains only Bare and Grass. Missing Desert sprites remain optional and are not replaced with Grass art.
- **Smallest mitigation:** During import, decide whether Desert is intentionally the base or a peer biome. If it is the base, document the limited contract; if it is a peer, add explicit scenario or board ownership of the visual base before calling the integration accepted.
- **Progress:** Josh chose Desert as a peer biome on 2026-09-09 and required a separate universal base. Bare uses the neutral dirt layer and now also receives the Grass neighbor mask in Forest Edge. Desert art, a selectable Desert scenario, and its visual import acceptance remain open.
- **Owner:** Josh
- **Recorded:** 2026-09-09, accepted by Josh in the planning conversation

## Closed concerns
