# GalapagOS Desktop UI — Art Direction Pass 02

> Status: Superseded historical exploration  
> Date: 2026-09-05  
> Scope: Player-facing GalapagOS Lab desktop shell and its between-expedition surfaces

> This pass is retained for comparison only. Its dark/wood field-station
> direction is superseded by the official pastel eco-desktop concept in
> [`ART_STYLE_GUIDE.md`](../ART_STYLE_GUIDE.md) and the approved concept board
> at [`docs/Art Direction/Concepts/GalapagOS_Desktop_UI_Concept_Options_v1.png`](Art%20Direction/Concepts/GalapagOS_Desktop_UI_Concept_Options_v1.png).

## Intent

This pass resets the visual direction around the project's actual art language.
It is intentionally implementation-agnostic: no UI framework, resource
dictionary, control template, or existing UI styling is treated as a visual
constraint.

The information architecture remains the existing Lab contract:

- Overview
- Research
- Species Archive
- Expedition Setup
- Settings and lightweight confirmations

The live simulation board, developer Lab, persistence, economy behavior, and
runtime implementation remain outside this concept pass.

## Visual north star

**GalapagOS is a hand-built tropical field station for studying living systems.**

The “desktop” is a research work surface: a wooden bench, pinned field notes,
specimen drawers, jars, maps, pressed leaves, stones, rope, brass fasteners,
and small pieces of the island. The player is not operating a sterile computer
dashboard. They are returning to a warm, curious, slightly improvised station
where the ecosystem has left evidence behind.

The interface should feel:

- cute and tactile;
- authored from the same pixel world as the island and its creatures;
- warm, readable, and a little messy at the edges;
- grounded in observation, collection, and preparation;
- clear enough that material decoration never obscures a decision.

## Art evidence this pass follows

- [`ART_STYLE_GUIDE.md`](../ART_STYLE_GUIDE.md) establishes crisp top-down pixel
  art, hard pixel clusters, limited warm tropical color, dark outlines,
  readable silhouettes, and no anti-aliased vector shapes.
- The retained island atlas provides the strongest material vocabulary: wood,
  campfire, tent, palm, jungle foliage, baskets, shells, stones, and shoreline.
- The current cellular species references provide compact silhouettes and stable
  role colors: green plants, blue herbivores, and red carnivores.
- The grass terrain tile provides a useful authored sample for biome cards and
  observation thumbnails.
- [`ART_PRODUCTION_SETUP_PLAN.md`](ART_PRODUCTION_SETUP_PLAN.md) keeps the UI and
  Lab visual language as an explicit art-direction work item rather than a
  consequence of the implementation framework.

## Concept families

### 1. Field Station Wall — recommended shell

A large central observation board is pinned into a wooden station wall. A
compact field-kit rail on the left opens the Lab destinations. A specimen tray
or note stack sits on the right. The persistent data readout is a row of jars,
tags, or stamped labels along the top or bottom edge.

This gives the Lab a strong identity while preserving a simple scan path:
choose a station tool, inspect one main surface, then take one clear action.

### 2. Specimen Cabinet — recommended Species Archive treatment

The screen reads like a cabinet of drawers and pinned study sheets. A selected
species occupies the largest card, with smaller neighboring specimens showing
what has been discovered. The archive becomes the emotional center of the Lab,
while research and expedition preparation appear as attached notebooks and
map sheets.

This is the strongest direction for species identity and collection, but it
should not become the navigation model for every Lab surface.

### 3. Camp Table / Expedition Map — recommended Expedition Setup treatment

The player prepares a field case on a camp table: a biome map, a species card,
starting options, and a clearly marked launch permit. Small props—compass,
backpack, seed pouch, notebook, specimen jar—make preparation feel like a
physical act.

This direction is the most action-oriented and should have the least decorative
density around the primary launch decision.

## Recommended layout

Use a 1280x720 composition as the design target and let 1920x1080 provide more
breathing room rather than simply scaling every object up.

```text
┌──────────────────────────────────────────────────────────────────────────┐
│ GALAPAGOS FIELD STATION     profile      data jars / field tags           │
├──────────┬───────────────────────────────────────────────┬───────────────┤
│ field    │                                               │ specimen tray │
│ kit      │              pinned observation board          │ notes /       │
│          │                                               │ quick facts   │
│ overview │       primary evidence + one next action      │               │
│ research │                                               │               │
│ archive  │       recent finds / map / research sheet      │               │
│ prepare  │                                               │               │
│          │                                               │               │
├──────────┴───────────────────────────────────────────────┴───────────────┤
│ field log / last observation / current expedition status                  │
└──────────────────────────────────────────────────────────────────────────┘
```

### Spatial rules

- Outer safe margin: approximately 32px at the reference resolution.
- Header/signboard: approximately 72–88px.
- Left field-kit rail: approximately 88–112px; it is icon-led but always has
  a readable label or tooltip state.
- Main board: the visual priority, with enough room for one large specimen,
  map, or research sheet.
- Right tray: approximately 240–288px at wide layouts; at 1280x720 it may
  collapse into the main board or become a slide-out note stack.
- Bottom field log: a compact status strip, not a second dashboard.
- The main board remains one coherent work surface. Avoid a pile of equal-sized
  modern cards competing for attention.

## Material and shape language

### Frames

- Wood planks, dark carved outlines, canvas, parchment, stone, and brass pins.
- Chunky pixel corners and stepped edges are preferable to smooth rounded
  rectangles.
- Use shadow as a hard pixel offset or a dark outline, not a soft floating
  drop-shadow effect.
- A panel can look attached, pinned, clipped, or inset into the work surface.

### Controls

- Navigation tabs resemble field-kit plates, drawer labels, or painted signs.
- Primary actions resemble a stamped permit, brass latch, or large wooden
  button with a clear inset/pressed state.
- Secondary actions resemble paper tabs or notebook marks.
- Locked states use a visible lock, crossed-out stamp, missing-sample mark, or
  hatch pattern plus text; never rely on opacity alone.

### Decoration

Use decoration to establish place and collection, not to fill every gap:

- small jungle leaves at the edges;
- a compass, hand lens, jars, shells, stones, seed packets, and field pencils;
- pinned notes and specimen labels;
- tiny grass or biome fragments as evidence samples.

The island atlas props are appropriate motifs. They should remain subordinate to
the player decision and should not be mistaken for interactive world objects.

## Palette direction

These are working concept swatches, not locked production values.

| Family | Direction | Example swatch |
| --- | --- | --- |
| Ink | dark brown-green outline and text | `#2D2015` |
| Jungle | deep foliage backing | `#24452B` |
| Leaf | foliage and plant evidence | `#6F963D` |
| Sand | warm paper and dry terrain | `#D8A34D` |
| Parchment | readable study sheets | `#F0D6A0` |
| Wood | frames and workbench | `#6B3B21` |
| Clay | warm panel accent and danger | `#B95A3C` |
| Ocean | water and map contrast | `#2C8190` |
| Herbivore blue | player/species identity | `#5D9FC1` |
| Carnivore red | threat/species identity | `#C84E3E` |

The palette should remain limited. Large surfaces should be sand, wood,
jungle, or parchment; saturated role colors belong to species, status marks,
and important interaction points.

## Pixel-art rules for the UI

- Use hard pixel clusters and nearest-neighbor scaling.
- Keep outlines dark and consistent so small specimens remain readable over
  parchment, grass, or wood.
- Prefer a small number of strong silhouettes over detailed but noisy drawings.
- Use 128px source art for hero specimens, 64px for feature cards, and 32px
  for trays and compact lists when the asset supports that scale.
- Keep the rabbit and fox visually distinct at a glance; the animal references
  already provide that contrast.
- Use actual terrain samples for biome/observation thumbnails where possible.
- Do not invent a final plant glyph by borrowing an animal silhouette. Until a
  dedicated fern presentation exists, use grass/terrain samples and clear text.
- Do not anti-alias, blur, or smooth-scale the UI art.

## Feature treatments

### Overview — field log

The Overview is a pinned field log showing:

1. The latest expedition result as the largest evidence sheet.
2. The next useful research action as a pinned recommendation.
3. Recent discoveries as small specimen tags or jars.

It should feel like a page the player has just returned to, not a generic
analytics dashboard.

### Research — branching notebook

Research is a notebook or wall chart with branching lines, pinned projects,
and stamped states:

- Locked
- Ready to inspect
- Affordable
- Insufficient data
- Selected
- Collected / purchased
- Newly available

Every state needs a symbol and label in addition to its color. A selected
project opens a readable detail sheet with cost, prerequisites, and result.

### Species Archive — specimen cabinet

The archive uses drawers or a specimen tray for Plant, Herbivore, and Carnivore
groups. The selected species gets the largest treatment: silhouette, role,
known behavior, observed interactions, mastery, and unlocked content.

The rabbit should be the first hero specimen, with fox as the contrasting
pressure specimen and grass/fern evidence supporting the environment.

### Expedition Setup — field case

Preparation should read as packing and approving an expedition:

- biome map or terrain sample;
- scenario premise;
- player species specimen;
- starting options and eligible upgrades;
- a launch permit or expedition seal.

The launch action must be visually unmistakable and the page should not bury
it under decorative props.

### Settings — camp toolbox

Settings can be a small toolbox or pinned utility sheet. Keep it visually
consistent but quieter than the archive and expedition surfaces.

## Interaction language

- Hover/focus: a bright outline, pin highlight, raised label, or small cursor
  marker—not a neon glow.
- Pressed: a 1–2px pixel inset or a visibly depressed wood/metal plate.
- Selected: pinned marker, colored border, or stamped corner seal plus text.
- Disabled: muted art, lock/stamp icon, and an explanation when useful.
- Transitions: short paper-slide, drawer-open, map-unroll, or pin-pop motions;
  avoid glossy fades and elastic modern UI motion.
- Sound direction can later follow the same language: paper, wood, glass, light
  metal, insects, and soft field ambience.

## Review gates before implementation

1. Approve the Field Station Wall as the shell family.
2. Approve Specimen Cabinet as the Species Archive treatment.
3. Approve Camp Table / Expedition Map as the Expedition Setup treatment.
4. Create one annotated 1280x720 static composition using the actual rabbit,
   fox, grass, and two or three island props.
5. Review that composition at 1920x1080 and 1280x720 for hierarchy, contrast,
   and readable states.
6. Only then translate the approved visual language into the runtime UI layer.

This pass replaces the previous dark-console recommendation. The earlier plan
is retained only for comparison and information-architecture traceability.
