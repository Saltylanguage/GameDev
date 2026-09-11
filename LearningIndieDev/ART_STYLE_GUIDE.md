# Darwin UI Art Direction v1

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

## GalapagOS desktop UI concept direction

**Status: Official concept inspiration baseline — approved 2026-09-05.**

The player-facing GalapagOS desktop should follow the light, pastel eco-OS
direction shown in the official concept board:

- [GalapagOS Desktop UI concept options v1](docs/Art%20Direction/Concepts/GalapagOS_Desktop_UI_Concept_Options_v1.png)
- [GalapagOS Desktop App Layout Concepts v1](docs/Art%20Direction/Concepts/GalapagOS_Desktop_App_Layout_Concepts_v1.md)
- [GalapagOS Desktop App Layout Studies v1](docs/Art%20Direction/Concepts/GalapagOS_Desktop_App_Layout_Studies_v1.svg)

The concept board is inspiration and layout direction, not a production UI
asset. It establishes the following visual priorities:

- vanilla cream and pale meadow greens as the dominant surfaces;
- light olive-green window chrome and navigation framing;
- dark brown for text, outlines, separators, and high-contrast anchors;
- pastel coral, tangerine, golden pollen, light bronze, sky blue, pastel petal,
  and pale lilac as restrained accents;
- bright ecology backgrounds, desktop icons, taskbars, and soft pixel-art
  window framing;
- a blend of Meadow Desktop, Lab Notebook, and controlled Classic Eco OS
  layouts;
- crisp pixel clusters, readable silhouettes, and hard edges rather than dark
  dashboards, neon, glassmorphism, or smooth vector UI.

### Current simulation-view north star

The current de-facto screen layout for the simulation, and the quality bar for
future generated GalapagOS views, is
[GalapagOS Simulation View — High-Fidelity Concept v1](docs/Art%20Direction/Concepts/GalapagOS_Simulation_View_High_Fidelity_v1.md),
with its preserved reference image at
[GalapagOS_Simulation_View_High_Fidelity_v1.png](docs/Art%20Direction/Concepts/GalapagOS_Simulation_View_High_Fidelity_v1.png).

![GalapagOS simulation view north-star concept](docs/Art%20Direction/Concepts/GalapagOS_Simulation_View_High_Fidelity_v1.png)

This is a high-fidelity concept and visual north star, not a production asset
or a gameplay specification. Future generated views should match its pale
green-and-cream GalapagOS shell, dark-brown contrast, board-first hierarchy,
small uniform square-cell game board, Field Ledger, bottom control dock,
phase timeline, and restrained field-notebook flourish. The interactive board,
state, and production art remain runtime- and artist-authored.

The intended shell direction is **Meadow Desktop**. Research and Species
Collection may use the **Lab Notebook** treatment, while **Classic Eco OS**
window stacking is reserved for controlled utility surfaces and should not make
the core Lab route difficult to scan.

### Window shell token baseline

The current `HeaderedContentControl` shell uses a cream body with a pale
green accent and a darker same-hue outline. These are the landed baseline
values for the first XAML treatment:

| Token | Value | Use |
| --- | --- | --- |
| Body top | `#FFF9E8` | Subtle upper body tone |
| Body bottom | `#FCEEC0` | Vanilla cream body surface |
| Header / accent fill | `#C5D370` | Pale green header and inset accent |
| Header stripes | `#E8EDB2` | Light Windows 95/Kingsway stripe treatment |
| Shell stroke | `#6F7E3A` | Slightly darker same-hue border |
| Shell text | `#372C15` | Dark-brown title and control contrast |

The cream body is inset by `4px` on the left, right, and bottom. It remains
flush to the header at the top, with rounded lower corners. Custom variants
should keep this cream body and adjust the header/accent fill plus its darker
same-hue stroke rather than returning to saturated full-window gradients.
The reusable three-line header treatment is exposed as
`GalapagOS.Brush.TopStripes` for title bands and panel headers.

### Approved GalapagOS control library entries

The first reusable controls are available from
`Assets/UI/GlobalResources.xaml`:

| Control | Resource key | Direction |
| --- | --- | --- |
| Primary Action Button | `GalapagOS.Button.Primary` | Olive fill `#84A340`, vanilla-cream label, darker olive outline, soft pale-green top highlight, and rounded corners |
| Secondary Action Button | `GalapagOS.Button.Secondary` | Light cream fill, dark-brown label, same shell outline, soft cream top highlight, and rounded corners |
| Metric / Stat Row | `GalapagOS.MetricStatRow` | Optional icon, readable label, and right-aligned value for compact ecology metrics |
| Progress Bar | `GalapagOS.ProgressBar` | Thick rounded cream track, pronounced shell stroke, olive fill with a pale-green linear shine, and a flat fill edge |

The metric row uses the attached properties on `GalapagOSMetricRow`:
`IconSource`, `Label`, and `Value`. It is intentionally a row-level control;
cards, progress bars, and separators remain composable screen-level pieces.

Primary action buttons also accept the optional `GalapagOSButton.IconSource`
attached property. Icons collapse completely when absent, can sit after the
label by default, or move before it with `GalapagOSButton.IconBefore="True"`.
This supports icon plus text, text plus icon, icon-only, and text-only actions
without creating separate button styles. The current window shell baseline is
a `4px` border. Screen-level separators use short, low-contrast dotted dark
sand strokes to preserve the light desktop feel.

The first GalapagOS icon files live in `Assets/UI/GalapagOS/Art/Icons/` as
concept/prototype assets. They establish the intended small pixel-art language
for ecology, expedition, gene, and action symbols while remaining replaceable
by final artist-authored assets.

The shared vector primitives live in `Assets/UI/GlobalResources.xaml` as
`Geometry` resources: `GalapagOS.Icon.Play`, `Pause`, `Stop`, `ZoomIn`,
`ZoomOut`, `Checkmark`, `Arrow`, and `Goalpost`. Use them with
`GalapagOS.Icon.Path` for simple monochrome controls; use authored raster art
when an icon needs species identity, texture, or a larger illustrative role.
