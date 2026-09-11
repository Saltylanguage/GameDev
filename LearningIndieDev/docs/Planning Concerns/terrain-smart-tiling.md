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
- **Status:** Acknowledged
- **Trigger:** A terrain family is imported when its `000` tile or diagonal-only states use different meanings from the resolver's empty-overlay and promoted-cardinal convention.
- **Why it matters:** Isolated terrain can disappear into the base layer, or diagonal contacts can render bridges and edges that the artist did not intend.
- **Evidence:** `TerrainTileResolver` promotes a diagonal contact by adding its adjacent cardinal directions; the current generated `000` sprite is transparent; Chrono's replacement art has not yet been inspected against those states.
- **Smallest mitigation:** Before replacing production images, compare the delivered isolated, one-cardinal, diagonal-only, corner, and full-surround examples with the resolver and approve one mapping for both families.
- **Owner:** Josh, with Chrono and the terrain integration owner
- **Recorded:** 2026-09-09, accepted by Josh in the planning conversation

### TILE-C02 — Desert is called integrated before it has a live biome owner

- **Severity:** Mild
- **Status:** Acknowledged
- **Trigger:** The Desert set is accepted as a peer smart-tiled biome while the board still draws `Desert_255` as the universal base, ignores computed Desert masks, and treats every non-Grass terrain as Desert.
- **Why it matters:** Forty-six Desert variants can remain unused while screenshots and asset checks give the impression that Desert biome tiling is complete.
- **Evidence:** The current Noesis board and terrain diagnostic draw a full Desert base and apply computed transition masks only for Grass; the simulation terrain registry requires Bare and Grass rather than a selectable Desert biome.
- **Smallest mitigation:** During import, decide whether Desert is intentionally the base or a peer biome. If it is the base, document the limited contract; if it is a peer, add explicit scenario or board ownership of the visual base before calling the integration accepted.
- **Progress:** Josh chose Desert as a peer biome on 2026-09-09 and required a separate universal base. Bare now owns the neutral base layer; Grass and Desert resolve and draw their own mask families. The temporary base is a plain fallback until its production art arrives. Scenario-level Desert rules and visual import acceptance remain open.
- **Owner:** Josh
- **Recorded:** 2026-09-09, accepted by Josh in the planning conversation

## Closed concerns
