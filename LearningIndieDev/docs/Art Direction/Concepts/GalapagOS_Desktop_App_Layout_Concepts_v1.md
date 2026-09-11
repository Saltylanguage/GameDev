# GalapagOS Desktop App Layout Concepts v1

> Status: Concept packet — ready for visual review  
> Date: 2026-09-06  
> Scope: Player-facing GalapagOS desktop applications other than Desktop Home and Simulation

> The simulation view is documented separately in [GalapagOS Simulation View —
> High-Fidelity Concept v1](GalapagOS_Simulation_View_High_Fidelity_v1.md). That
> concept is the current de-facto layout and visual north star for future
> generated GalapagOS views.

## Intent

This packet turns the approved GalapagOS pastel eco-desktop direction into
screen-level layout concepts. It is an information and composition guide, not
a production XAML specification or an asset list.

The screens should feel like tools in one small field station: each app has a
distinct job and a memorable focal object, but the player should never have to
learn a new navigation model for every destination.

The approved direction is:

- pale meadow greens and vanilla cream for the dominant surfaces;
- dark brown for readable text, outlines, separators, and anchors;
- restrained coral, tangerine, pollen, bronze, sky, petal, and lilac accents;
- crisp pixel clusters and sparse, authored decoration;
- a striped Windows 95 / Kingsway-inspired GalapagOS title bar;
- native-feeling app windows that sit on the desktop rather than blending into
  the background illustration.

## Shared shell contract

Every app uses the same shell family unless a concept below explicitly calls
for a contained sheet or tray.

```text
┌──────────────────────────────────────────────────────────────────────────┐
│ icon  APP TITLE                                      _  □  X              │
├──────────────────────────────────────────────────────────────────────────┤
│ app-local tabs / breadcrumbs / status                                      │
├───────────────┬───────────────────────────────────────────┬───────────────┤
│ optional      │                                           │ optional      │
│ category rail │              primary work surface         │ context /     │
│ or index      │                                           │ detail rail   │
│               │                                           │               │
├───────────────┴───────────────────────────────────────────┴───────────────┤
│ app-local action row / helper text / save or return affordance             │
└──────────────────────────────────────────────────────────────────────────┘
```

### Reference geometry

Use a 1280x720 design target and scale through the existing Noesis `Viewbox`.

| Region | Target | Rule |
| --- | ---: | --- |
| Window safe margin | 24px | Keep the shell away from the viewport edge. |
| Title bar | 36px | Keep the three light stripes and native close affordance. |
| Body inset | 4px | The cream body remains subtly inset from the shell border. |
| App header | 48–64px | Title, short purpose line, and one small status marker. |
| Category rail | 176–208px | Use only when browsing a collection or tree. |
| Context rail | 240–288px | Optional at wide layouts; collapse below 1280x720. |
| Primary action row | 52–64px | One dominant action; secondary actions stay visually quiet. |

### Shared visual tokens

Use the landed tokens rather than introducing per-app colors:

| Role | Token / value |
| --- | --- |
| Main body | `Chrono_LightCream` / `#FAF0D6` |
| Warm body sheet | `Chrono_VanillaCream` / `#FCEEC0` |
| Pale green header | `Chrono_Lime` / `#C5D370` |
| Green action / selection | `Chrono_Olive` / `#84A340` |
| Dark contrast | `Chrono_TreeBark` / `#372C15` |
| Quiet separator / card stroke | `Chrono_Sand` / `#D4A373` |
| Warm alert | `Chrono_Tangerine` / `#FF9770` |
| Discovery highlight | `Chrono_GoldenPollen` / `#FDCA47` |
| Water / cool evidence | `Chrono_AzureSky` / `#A2D2FF` |
| Soft category accent | `Chrono_FloralRose` / `#FFC8DD`, `Chrono_PaleLilac` / `#CDB4DB` |

### Reusable control placement

The existing control library should carry most of the visual identity:

- `GalapagOS.Button.Primary` for launch, purchase, equip, apply, and open;
- `GalapagOS.Button.Secondary` for back, cancel, reset, compare, and details;
- `GalapagOS.MetricStatRow` for compact ecology and history metrics;
- `GalapagOS.ProgressBar` for mastery, discovery, research, and collection;
- dotted separators for notebook-like grouping;
- the shared `GalapagOS_HeaderedWindowStyle` for every app shell.

New controls should be considered only when a repeated need appears in at
least two screens. The likely next reusable candidates are:

1. `CategoryTabs` — horizontal All / Plants / Herbivores / Carnivores tabs.
2. `FilterChips` — Unlocked / Favorites / In Progress / All.
3. `SpecimenCard` — icon, name, role, state, and progress.
4. `StatusStamp` — Locked / Ready / Owned / New with text and symbol.
5. `DetailSheet` — title, summary, metrics, links, and one primary action.
6. `TimelineEntry` — timestamp or phase, event label, value, and deep link.
7. `EmptyStateNote` — friendly explanation, next action, and optional icon.

These are concept-level candidates, not permission to build all seven before a
screen needs them.

## Three layout families

The apps share the same shell but can use three internal compositions.

### A — Field Notebook Window (recommended default)

Best for Settings, History, Field Guide, Research Inbox, and Quick Search.

```text
┌──────────────────────────────────────────────────────────────────────┐
│ title bar                                                             │
├───────────────────────┬──────────────────────────────────────────────┤
│ index / sections       │ one clean paper-like work sheet              │
│                        │ heading → grouped content → action row       │
└───────────────────────┴──────────────────────────────────────────────┘
```

This is the quietest family. It uses cream sheets, dotted lines, small icon
markers, and one clear selected section.

### B — Specimen Cabinet Window

Best for Species Collection, Gene Lab, and Biome / Ecology Lab.

```text
┌──────────────────────────────────────────────────────────────────────┐
│ title bar                                                             │
├──────────────────┬───────────────────────────────┬───────────────────┤
│ tabs / specimen   │ selected specimen, map, or    │ facts / node /    │
│ index             │ research board                │ action detail     │
└──────────────────┴───────────────────────────────┴───────────────────┘
```

The center is always the thing being studied. The side rails support it; they
must not become equal-weight dashboards.

### C — Desktop Utility Window

Best for My PC, Music Player, and small personalization tools.

```text
┌──────────────────────────────────────────────────────────────────────┐
│ title bar                                                             │
├─────────────────────────────┬────────────────────────────────────────┤
│ preview / identity           │ compact controls and choices           │
│                             │ explicit apply / equip / save row       │
└─────────────────────────────┴────────────────────────────────────────┘
```

These screens can be smaller and centered. Their preview area should show the
effect of a choice before the player commits it.

## Screen concepts

The following concepts cover the twelve non-simulation desktop applications.
Each one has a primary focal object, an intentional scan path, and explicit
links to the system that owns the underlying data.

---

## 1. Settings — “Make the station work for me”

**Family:** Field Notebook Window  
**Focal object:** A tidy settings sheet with section tabs  
**Primary action:** Apply Changes  
**Secondary actions:** Restore Defaults, Cancel

```text
┌──────────────────────────────────────────────────────────────────────────┐
│ SETTINGS                                                                  │
├──────────────────────────────────────────────────────────────────────────┤
│ SETTINGS · DISPLAY · AUDIO · CONTROLS · ACCESSIBILITY · DESKTOP           │
├───────────────────┬──────────────────────────────────────────────────────┤
│ current section   │ DISPLAY                                               │
│                   │ How GalapagOS fits your screen.                       │
│ Display            │ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─                   │
│ Audio              │ Window mode        [ Windowed              v ]        │
│ Controls           │ Resolution         [ 1280 x 720            v ]        │
│ Accessibility      │ UI scale           [ 100%                  v ]        │
│ Desktop behavior   │ Pixel scaling      [ x ] Preserve crisp pixels       │
│                    │                                                        │
│                    │ [Restore Defaults]                  [Apply Changes] │
└───────────────────┴──────────────────────────────────────────────────────┘
```

### Content and behavior

- Keep one section open at a time; do not make Settings a long scrolling wall.
- Audio controls link to Music Player for track choice, but volume and mix
  remain owned here.
- Desktop behavior links to My PC for themes, wallpapers, and icon placement.
- Accessibility options need text labels beside their toggles; never make a
  color-only preview the only explanation.
- Restoring defaults requires a small confirmation sheet, not a destructive
  full-screen modal.

### Art direction

Use a pale green section rail, cream sheet, dark-brown labels, and small toolbox
marks. Settings should be the least decorative app in the suite.

---

## 2. Species Collection — “Meet the organisms”

**Family:** Specimen Cabinet Window  
**Focal object:** Large selected-species card  
**Primary action:** Open in Gene Lab  
**Secondary actions:** Compare, Favorite, Open History

```text
┌──────────────────────────────────────────────────────────────────────────┐
│ SPECIES COLLECTION                                                        │
├──────────────────────────────────────────────────────────────────────────┤
│ ALL  PLANTS  HERBIVORES  CARNIVORES                 [Unlocked v] [☆]      │
├──────────────────┬──────────────────────────────┬────────────────────────┤
│ specimen index   │ HARE                           │ QUICK FACTS             │
│                  │        [large hare glyph]      │ Role       Herbivore    │
│ [Hare]           │                                │ Biome      Forest Edge │
│ [Fox]            │ HARE · HERBIVORE              │ Mastery     68%         │
│ [Grass]          │ A cautious grazer with sharp   │ First seen  Day 03      │
│ [locked slot]    │ hearing and a growing record.  │                        │
│                  │ [Open in Gene Lab] [History]  │ Known: grazing, flight  │
├──────────────────┴──────────────────────────────┴────────────────────────┤
│ discovery  ████████████░░░░  6 / 10 species catalogued                     │
└──────────────────────────────────────────────────────────────────────────┘
```

### Content and behavior

- The top category tabs are reusable across Collection, Gene Lab, and Biome
  Lab, but the labels should reflect the owning surface.
- The index supports All, Unlocked, Undiscovered, Mastery in Progress, and
  Favorites without changing the page layout.
- The selected card shows glyph, role, short identity, primary biome, mastery,
  discovery date, and known interactions at a glance.
- Compare opens a second selected column or a bounded comparison sheet; it
  should not replace the hero card by default.
- Locked species use a lock or missing-sample mark plus text, not opacity alone.

### Art direction

Use a specimen tray silhouette for the index, a clean cream card for the hero,
and role accents only on glyphs, labels, and status marks. The rabbit is the
first hero specimen; fox is the pressure contrast; plant entries can use
terrain/grass evidence until a dedicated plant glyph is authored.

---

## 3. Gene Lab — “Choose a biological direction”

**Family:** Specimen Cabinet Window  
**Focal object:** Branching upgrade tree  
**Primary action:** Preview in Expedition Setup  
**Secondary actions:** Switch Research Family, Open Species Collection

**Polish candidate:** [Gene Lab UI — High-Fidelity Concept v2](Gene_Lab_UI_High_Fidelity_v2.png)

This generated revision is the current visual candidate for review. It keeps
the approved information architecture while bringing the shell, spacing,
panel hierarchy, and research-tree presentation closer to the simulation-view
north star. The earlier v1 image remains preserved as the prior reference.

```text
┌──────────────────────────────────────────────────────────────────────────┐
│ GENE LAB · HARE                                                           │
├──────────────────────────────────────────────────────────────────────────┤
│ PLANT       HERBIVORE       CARNIVORE                    HARE              │
├───────────────────────────────────────────────┬──────────────────────────┤
│ RESEARCH TREE                                 │ SELECTED NODE             │
│ Hare root · owned                             │ Quick Feet                │
│                                               │ Available                 │
│ [owned]──[available]                          │ Cost · 180 data           │
│       └──[locked]                              │ Prereq · Hare root        │
│                                               │ Preview in Expedition     │
│                                               │ Setup                     │
└───────────────────────────────────────────────┴──────────────────────────┘
```

### Content and behavior

- Family selection sits in the top strip, keeping the main surface focused.
- The tree is readable left to right: root, prerequisite, choice, consequence.
- Each node combines shape, border, icon, and text state: Locked, Available,
  Affordable, Insufficient Data, Owned, or New.
- The selected detail sheet must distinguish permanent research from temporary
  simulation phase rewards.
- Preview in Expedition Setup is a presentation-only deep link placeholder until
  Gene Lab progression and economy wiring arrive.
- Open in Species Collection preserves the selected species context.

### Art direction

Use a notebook branching diagram with small pinned cards and dotted connectors.
Avoid a glowing sci-fi tech tree. The tree should feel like a field hypothesis
chart with pressed leaves, labeled samples, and stamped states.

---

## 4. My PC — “Make the desktop feel like mine”

**Family:** Desktop Utility Window  
**Focal object:** Live desktop preview  
**Primary action:** Equip Selection  
**Secondary actions:** Reset Layout, Randomize from Unlocked

```text
┌──────────────────────────────────────────────────────────────────────────┐
│ MY PC                                                                    │
├──────────────────────────────────────────────────────────────────────────┤
│ WALLPAPERS   THEMES   ICONS   TASKBAR   DECORATIONS                      │
├──────────────────────────────┬───────────────────────────────────────────┤
│ unlocked choices              │ LIVE DESKTOP PREVIEW                      │
│                              │  ┌─────────────────────────────────────┐  │
│ [Meadow Morning]             │  │ small desktop scene                  │  │
│ [Rainy Study]                │  │ icons + taskbar + selected theme    │  │
│ [Sunset Field Notes]         │  └─────────────────────────────────────┘  │
│ [locked wallpaper]           │                                           │
│                              │ Desktop icons       [Arrange]             │
│                              │ Taskbar pins         [Choose]              │
│                              │                         [Equip Selection]  │
└──────────────────────────────┴───────────────────────────────────────────┘
```

### Content and behavior

- Preview wallpaper, theme, icon treatment, and taskbar changes before apply.
- Icon arrangement is optional personalization; core apps remain reachable
  through Start and Quick Search.
- Keep profile identity here only as a small header card. Account/display
  settings stay in Settings.
- Habitat Gallery can send an unlocked wallpaper or ornament here to equip.

### Art direction

This is the most playful utility. Use miniature desktop previews, pinned
labels, and a few collectible decorations, but keep the actual controls clean.
No full-screen background painting inside the app window.

---

## 5. Music Player — “Set the field station's sound”

**Family:** Desktop Utility Window  
**Focal object:** Current-track card and playlist queue  
**Primary action:** Play / Pause  
**Secondary actions:** Choose Playlist, Shuffle, Repeat

```text
┌──────────────────────────────────────────────────────────────────────────┐
│ MUSIC PLAYER                                                              │
├──────────────────────────────────────────────────────────────────────────┤
│ DESKTOP  SIMULATION  RESULTS  AMBIENT                                     │
├────────────────────────────┬─────────────────────────────────────────────┤
│ TRACK LIST                  │ NOW PLAYING                                 │
│                              │ [small album / habitat tile]               │
│ > Fernlight Valley          │ Fernlight Valley                            │
│   Meadow Morning            │ Field station mix                           │
│   Rain on Canvas            │ ───────────────○──────  01:42 / 03:10       │
│   locked: Tide Pools       │ [◀] [ PLAY ] [▶]    [Shuffle] [Repeat]        │
│                              │                                             │
│                              │ Ambient layer: [Meadow                   v] │
└────────────────────────────┴─────────────────────────────────────────────┘
```

### Content and behavior

- Playlist tabs separate Desktop, Simulation, Results, and Ambient choices.
- The taskbar can show the current track, but volume/mix remains in Settings.
- Locked tracks show their unlock source, such as Habitat Gallery or a
  completed expedition.
- Ambient layers are additive choices, not a second music library.

### Art direction

Use a small cassette, shell, radio, or field-recorder motif as the app icon.
Track rows should look like labeled tape strips or notebook lines, not a modern
streaming-service card grid.

---

## 6. History / Data Record — “Understand what happened”

**Family:** Field Notebook Window  
**Focal object:** Selected expedition record with phase timeline  
**Primary action:** Open Record  
**Secondary actions:** Compare, Pin Record, Open Species / Biome

```text
┌──────────────────────────────────────────────────────────────────────────┐
│ HISTORY / DATA RECORD                                                     │
├──────────────────────────────────────────────────────────────────────────┤
│ EXPEDITIONS   PHASE TIMELINE   SPECIES   BIOMES   ACHIEVEMENTS             │
├───────────────────┬─────────────────────────────┬──────────────────────────┤
│ saved records     │ EXPEDITION 004              │ RECORD SUMMARY           │
│                   │ Fernlight Valley · Hare    │ Final population   28    │
│ [004] latest      │ ──●────●────●────●──        │ Collected         28/40  │
│ [003]             │ P1   P2   P3   P4           │ Rare genes          3/8  │
│ [002]             │                              │ Duration        04:12   │
│ [001]             │ upgrades at boundaries      │ [Compare] [Species]     │
│                   │ [Open Record]                │ [Biome Data]            │
├───────────────────┴─────────────────────────────┴──────────────────────────┤
│ NOTE: phase values describe one continuous expedition, not fresh runs.     │
└──────────────────────────────────────────────────────────────────────────┘
```

### Content and behavior

- Make Phase and Expedition explicit in labels and timeline grouping.
- The record view distinguishes valid recorded metrics from unavailable or
  representative data.
- Species comparison uses compatible records only and labels the comparison
  window clearly.
- Achievements can be a tab here while Habitat Gallery displays the earned
  presentation pieces; History remains the source of record.
- Deep links open the owning app without changing the historical record.

### Art direction

Use stamped log pages, dated tabs, tiny seed/phase marks, and restrained charts.
The app is analytical but should still look like a field notebook, not a BI
dashboard.

---

## 7. Biome / Ecology Lab — “Read the habitat”

**Family:** Specimen Cabinet Window  
**Focal object:** Biome map / terrain sample  
**Primary action:** Open in Expedition Planner  
**Secondary actions:** Inspect Ecology Research, View Species Fit

```text
┌──────────────────────────────────────────────────────────────────────────┐
│ BIOME / ECOLOGY LAB                                                       │
├──────────────────────────────────────────────────────────────────────────┤
│ FOREST EDGE  v       BIOME MAP   SPECIES FIT   ECOLOGY RESEARCH             │
├──────────────────┬───────────────────────────────┬────────────────────────┤
│ BIOME INDEX      │ [terrain / map sample]         │ HABITAT READOUT         │
│ Forest Edge      │                               │ Movement cost    Low    │
│ Fernlight Valley │   water · grass · canopy      │ Food pressure     42%   │
│ Tide Pools       │                               │ Risk              Mild   │
│ [locked biome]   │   [inspect hotspot]            │ Compatible: Hare       │
│                  │                               │ [Open in Planner]       │
├──────────────────┴───────────────────────────────┴────────────────────────┤
│ ecology research:  [owned]──[available]──[locked]    DATA  420             │
└──────────────────────────────────────────────────────────────────────────┘
```

### Content and behavior

- Separate biome/scenario access from environmental research purchases.
- Explain movement costs, resource availability, species fit, and ecological
  pressure in plain language with supporting metrics.
- The map is an authored terrain sample, not a second playable board.
- Open in Expedition Planner carries the biome context forward without
  silently selecting a species or launch option.

### Art direction

Use a simple terrain tile collage, map pins, pressed leaves, water marks, and a
small ecology legend. Keep the map readable at a glance; detailed landscape
illustration belongs in the desktop or gallery, not behind every data panel.

---

## 8. Expedition Planner / Launchpad — “Prepare the next expedition”

**Family:** Desktop Utility Window with field-case treatment  
**Focal object:** Launch permit / prepared expedition card  
**Primary action:** Launch Expedition  
**Secondary actions:** Back, Open Species, Open Biome, Review Rules

```text
┌──────────────────────────────────────────────────────────────────────────┐
│ EXPEDITION PLANNER                                                        │
├──────────────────────────────────────────────────────────────────────────┤
│ 1 BIOME  →  2 SPECIES  →  3 OPTIONS  →  4 LAUNCH                          │
├──────────────────────┬──────────────────────┬────────────────────────────┤
│ BIOME                 │ PLAYER SPECIES        │ LAUNCH PERMIT              │
│ [Forest Edge]         │ [hare glyph]          │ Forest Edge                │
│ terrain sample        │ Hare · Herbivore      │ Hare · Day 03              │
│ gentle pressure       │ starting traits       │ seed policy: randomized    │
│ [Change Biome]        │ [Change Species]      │ permanent research: 2      │
│                       │                       │ [Launch Expedition]        │
└──────────────────────┴──────────────────────┴────────────────────────────┘
```

### Content and behavior

- Treat the four steps as a visual progress path, not four separate windows.
- Show only valid launch options. Representative or unavailable values must be
  labelled.
- The permit summarizes the immutable launch request: scenario, species, seed
  policy, and eligible permanent choices.
- Launch begins the simulation flow; phase reward choices never route back
  through this screen.

### Art direction

Use a field case, passport, map fold, or stamped permit as the visual language.
This surface gets the strongest primary button and the least decorative noise.

---

## 9. Field Guide / Research Journal — “Learn what the station knows”

**Family:** Field Notebook Window  
**Focal object:** Selected illustrated entry page  
**Primary action:** Open Related Record  
**Secondary actions:** Previous / Next, Bookmark, Search Entries

```text
┌──────────────────────────────────────────────────────────────────────────┐
│ FIELD GUIDE / RESEARCH JOURNAL                                             │
├──────────────────────────────────────────────────────────────────────────┤
│ SPECIES   BIOMES   ECOLOGY TERMS   TUTORIALS                    [Search]   │
├──────────────────┬────────────────────────────────────────────────────────┤
│ contents          │ HARE · GRAZER                                          │
│                    │ [small illustration / glyph]                         │
│ Getting Started    │ Hares prefer open grass and avoid sustained pressure. │
│ Species            │                                                        │
│ Habitats           │ Known terms: forage · movement · reproduction          │
│ Interactions       │ Related: Species Collection · Forest Edge              │
│                    │ [Open Species Record]                                  │
└──────────────────┴────────────────────────────────────────────────────────┘
```

### Content and behavior

- Entries are short and skimmable; the guide teaches the player without
  becoming a lore-only wall of text.
- New entries show a small New stamp and remain discoverable through Research
  Inbox.
- Glossary links can open a definition sheet without leaving the guide.
- The guide explains rules in player language; raw developer tuning remains
  outside this surface.

### Art direction

Use a page-turn / notebook treatment with one illustration, a few pressed
specimen marks, and generous cream space. This is a calm reading surface.

---

## 10. Research Inbox / Bulletin — “See what changed”

**Family:** Field Notebook Window  
**Focal object:** Ordered notice stack  
**Primary action:** Open Selected Notice  
**Secondary actions:** Mark Read, Dismiss, Clear Read

```text
┌──────────────────────────────────────────────────────────────────────────┐
│ RESEARCH INBOX / BULLETIN                                                 │
├──────────────────────────────────────────────────────────────────────────┤
│ ALL  NEW  SPECIES  RESEARCH  EXPEDITIONS  PERSONALIZATION     3 UNREAD    │
├──────────────────────────────────────────────────────┬────────────────────┤
│ notice stack                                          │ NOTICE DETAIL      │
│ [NEW] Hare record added                    2 min ago  │ NEW SPECIES        │
│ [DATA] Expedition 004 complete             yesterday  │ Hare is now in your │
│ [UNLOCK] Rainy Study wallpaper              day 02    │ Species Collection. │
│ [READY] Research node available             day 01    │ [Open Collection]  │
│                                                        │ [Mark Read]        │
└──────────────────────────────────────────────────────┴────────────────────┘
```

### Content and behavior

- Notices are routing objects, not a second reward or progression system.
- Every notice names its destination app and offers one deep-link action.
- New / read state uses a symbol, label, and light emphasis—not color alone.
- The inbox can show recent results and unlocks without claiming them
  automatically.

### Art direction

Use pinned slips, clipped notes, small stamps, and a calm bulletin-board rhythm.
Keep the notice list compact enough that the right detail sheet remains useful.

---

## 11. Habitat Gallery / Museum — “Keep the discoveries visible”

**Family:** Specimen Cabinet Window with display-wall treatment  
**Focal object:** Equipped display / curated habitat set  
**Primary action:** Equip Display  
**Secondary actions:** Browse Collection, Open History, Send to My PC

```text
┌──────────────────────────────────────────────────────────────────────────┐
│ HABITAT GALLERY / MUSEUM                                                  │
├──────────────────────────────────────────────────────────────────────────┤
│ HABITATS   SPECIES SETS   ACHIEVEMENTS   DESKTOP DECOR                    │
├──────────────────────┬───────────────────────────────┬───────────────────┤
│ display shelf        │ CURRENT DISPLAY                │ ITEM DETAIL       │
│ [Fernlight]          │ ┌───────────────────────────┐ │ Fernlight plaque  │
│ [Forest Edge]        │ │ grass · water · hare      │ │ Earned: 6/10       │
│ [Hare set]           │ │ small framed habitat     │ │ Unlocks wallpaper │
│ [locked display]     │ └───────────────────────────┘ │ [Equip Display]   │
│                      │                               │ [Send to My PC]   │
└──────────────────────┴───────────────────────────────┴───────────────────┘
```

### Content and behavior

- The gallery displays earned content; it does not define achievement rules.
- A display can be an arranged set of species, a biome plaque, an expedition
  record, or a desktop ornament.
- Equip sends wallpaper and desktop ornament choices to My PC without making
  the gallery a settings screen.
- History remains the source for expedition evidence and achievement state.

### Art direction

Use a sparse museum shelf, framed cards, labels, and little plaque holders.
This is a place for the project's most charming illustrations, but the display
grid should remain calm and easy to browse.

---

## 12. Quick Search / Jump — “Find the thing I remember”

**Family:** Compact Field Notebook utility  
**Focal object:** Search field with categorized result list  
**Primary action:** Open Result  
**Secondary actions:** Filter, Clear, Recent Searches

```text
┌──────────────────────────────────────────────────────────────────────────┐
│ QUICK SEARCH / JUMP                                                       │
├──────────────────────────────────────────────────────────────────────────┤
│ [ Search species, biome, research, record...                    ] [Clear]  │
├──────────────────────────┬───────────────────────────────────────────────┤
│ FILTERS                  │ RESULTS                                         │
│ All                      │ SPECIES · Hare                                 │
│ Species                  │ Herbivore · Forest Edge             [Open]     │
│ Biomes                   │ RESEARCH · Keen Hearing                        │
│ Research                 │ Gene Lab · Hare                     [Open]     │
│ History                  │ RECORD · Expedition 004                        │
│ Music / Desktop          │ History · Fernlight Valley           [Open]     │
└──────────────────────────┴───────────────────────────────────────────────┘
```

### Content and behavior

- Search is navigation-only; it must not create content or apply changes.
- Results show type, title, short context, and owning app before the player
  opens them.
- Preserve context when opening a result: selected species, biome, record, or
  notice should already be selected in the destination app.
- Recent searches can remain local and lightweight.

### Art direction

Use a small magnifying-glass / field-lens icon, cream search sheet, and compact
result slips. This should be the fastest, quietest app to open.

## Cross-screen state language

Every app should support the same small set of readable states:

| State | Visual treatment | Required text |
| --- | --- | --- |
| Selected | olive border, small pin/marker | Selected / current item |
| Available | pale green accent | Available / Ready |
| Owned / equipped | stamped olive mark | Owned / Equipped |
| New | pollen or petal corner mark | New |
| Locked | muted art plus lock/missing-sample mark | Locked + why |
| Insufficient data | warm bronze/tangerine note | Need X more data |
| Empty | quiet cream note and next action | Nothing here yet + what to do |
| Representative | dotted note or small label | Representative data |
| Error | tangerine border and plain explanation | What failed + recovery |

Color is supportive only. A player should be able to understand state from the
label, icon, and shape treatment.

## Recommended design review order

For the next visual pass, review the concepts in this order:

1. Species Collection — validates the shared tabs, specimen card, detail
   sheet, and collection progress language.
2. Gene Lab — validates the branching tree, status stamps, and purchase detail.
3. Expedition Planner — validates the primary player loop and launch permit.
4. Settings — validates the quietest reusable form treatment.
5. Biome / Ecology Lab — validates map samples, ecology metrics, and deep links.
6. History / Data Record — validates timeline and phase-versus-expedition
   communication.
7. My PC and Music Player — validates personalization previews and utility
   density.
8. Field Guide, Research Inbox, Habitat Gallery, and Quick Search — fill out
   the supporting ecosystem using the established patterns.

## Implementation boundary

This concept packet does not authorize:

- new simulation rules or progression behavior;
- a second window manager or freeform stacking system;
- a new visual theme outside the approved light pastel direction;
- final production art assets;
- automatic purchases, claims, or state changes from deep links;
- developer tuning controls in player-facing apps.

The next concrete design artifact should be one annotated 1280x720 composition
for Species Collection, Gene Lab, and Expedition Planner using the same shell
and representative rabbit, fox, grass, and terrain references.
