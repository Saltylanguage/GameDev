# GalapagOS Desktop UI Direction Plan

> Status: Superseded visual draft — retained for comparison  
> Date: 2026-09-05  
> Scope: Player-facing GalapagOS Lab desktop shell and its between-expedition surfaces

> The art-direction revision is now in
> [`GALAPAGOS_DESKTOP_UI_ART_DIRECTION_PASS.md`](GALAPAGOS_DESKTOP_UI_ART_DIRECTION_PASS.md).

## Purpose

Define the visual direction and layout contract for the GalapagOS Lab before
expanding the current Noesis implementation. This is a design document, not an
implementation commitment. It should settle the shell's visual language,
screen composition, and review gates before more XAML or Unity wiring is added.

The scope is the player Lab:

- Overview
- Research
- Species Archive
- Expedition Setup
- Settings and lightweight confirmation overlays

The scope excludes the developer Lab, the live simulation board, simulation
rules, persistence, and economy behavior.

## Evidence baseline

The direction below is grounded in the current project material:

- [`MAIN_MENU_LAB_DELIVERY_PLAN.md`](MAIN_MENU_LAB_DELIVERY_PLAN.md) defines the
  Lab as an operating-system-like home base between experiments and calls for
  a biology-workspace vocabulary: data, research projects, specimens, field
  notes, and analyzed samples.
- [`PROJECT_CONTEXT.md`](PROJECT_CONTEXT.md) establishes Noesis/XAML with
  ViewModels, a tentative 12px radius for major windows, separate player and
  developer surfaces, and stable role colors for plants, herbivores, and
  carnivores.
- [`ART_STYLE_GUIDE.md`](../ART_STYLE_GUIDE.md) and the current species assets
  establish compact, high-contrast geometric silhouettes with flat fills. The
  rabbit, fox, and terrain references are strong candidates for specimen cards
  and archive previews.
- [`FIGMA_NOESIS_PILOT.md`](FIGMA_NOESIS_PILOT.md) is a closed historical
  experiment. Its `FigmaNoesisPilotResources.xaml` remains a live Lab resource
  dictionary and preserves useful semantic keys, but its dark visual proposal is
  not the current art-direction authority.
- The canonical light pastel direction comes from `GlobalResources.xaml`, the
  GalapagOS window/control resources, the accepted concept image, and the
  graphics-tested desktop composition. New Lab work should converge on that
  direction while migrating live semantic resources deliberately rather than
  deleting or duplicating them.
- The current [`V_Panel_Lab.xaml`](../Assets/UI/Lab/V_Panel_Lab.xaml) is a
  functional two-column prototype with representative feature templates. It is
  the correct behavior baseline, but not yet the final composition.

## Design thesis

**GalapagOS is an instrument desk for observing living systems.**

The player should feel like they are returning to a research station, reviewing
evidence, choosing a hypothesis to pursue, and preparing the next expedition.
The desktop metaphor should provide orientation and personality without making
the player manage arbitrary windows just to reach the next action.

The visual mix should be:

- dark, quiet shell surfaces for long-session readability;
- warm paper, specimen, and amber accents for discoveries and decisions;
- pixel-art animal and terrain glyphs as the emotional identity layer;
- restrained biology motifs such as sample trays, field-note cards, measured
  bars, and phylogenetic connectors;
- explicit labels and status text so color and metaphor never carry meaning
  alone.

## Layout concepts

The first concept board explores three families:

1. **Expedition Workbench — recommended.** A fixed top status bar, a left
   navigation dock, one large active feature window, and an optional contextual
   details rail. This has the best scan path for the current Lab contract and
   scales cleanly to 1280x720.
2. **Specimen Archive Desk.** A catalog-first composition with a large selected
   specimen, evidence notes, and a horizontal species tray. This is the most
   characterful direction for Species Archive and could supply its internal
   page treatment.
3. **GalapagOS Window Stack.** A more literal retro desktop with overlapping
   research windows. It communicates the name strongly, but freeform stacking
   adds focus, z-order, close behavior, and small-screen risks. Use its chrome
   cues selectively, not as the default navigation model for the vertical
   slice.

### Recommended shell wireframe

```text
┌──────────────────────────────────────────────────────────────────────────┐
│ GALAPAGOS LAB     current profile        RESEARCH  PLANT  HERB  CARN  │
│ expedition status / last observation                         settings  │
├───────────────┬──────────────────────────────────────────────────────────┤
│               │  active window: OVERVIEW                 •  •  ×        │
│  OVERVIEW     │  ┌────────────────────────────────────────────────────┐  │
│  RESEARCH     │  │ current study / latest observation                 │  │
│  ARCHIVE      │  │                                                    │  │
│  EXPEDITION   │  │ primary evidence       recommended next action     │  │
│               │  │                                                    │  │
│  small status │  │ recent discoveries / specimen cards / notes       │  │
│  card         │  └────────────────────────────────────────────────────┘  │
│               │  contextual details appear inline or as a narrow rail  │
└───────────────┴──────────────────────────────────────────────────────────┘
```

The shell should visually read as a desktop and support multiple controlled
feature windows at once. The `C_GalapagOS_Window` treatment provides the title
bar, move surface, and close affordance; the desktop shell owns the open-window
collection while `VM_Lab` remains responsible for feature state only.

## Layout contract

Use a 1280x720 reference artboard and scale to 1920x1080 through the existing
Noesis `Viewbox` approach.

| Region | Reference size | Responsibility |
| --- | ---: | --- |
| Safe margin | 24px, reducing to 16px at the narrow target | Keep the shell away from the viewport edge. |
| Top bar | 64–72px | Lab identity, profile, persistent data totals, settings/menu access. |
| Navigation rail | 208–224px | Overview, Research, Species Archive, Expedition Setup, and active-state focus. |
| Main stage | Remaining width and height | One active feature window with a stable title/header treatment. |
| Context rail | Optional 240–288px | Only at wide widths; collapse into the main stage or an overlay at 1280x720. |
| Transient overlay | Centered, bounded | Confirmation, profile, purchase preview, and error states. |

Rules:

- The scientific-data bar remains visible on every Lab feature surface.
- Navigation, title, and Back behavior remain stable as the active feature
  changes.
- Do not use permanent overlapping windows for core Lab navigation.
- At 1280x720, the content must remain usable without a right rail.
- At 1920x1080, extra width may expose richer evidence cards or the context
  rail rather than simply enlarging text.
- Keep the current deterministic flow: feature pages return to Overview, and
  leaving the Lab or an expedition uses explicit commands and confirmation.

## Feature compositions

### Overview — “What changed, what matters, what next”

Three priorities in reading order:

1. Latest experiment return: scenario, player species, result, and data gained.
2. Recommended next research goal: reason, cost, prerequisites, and why it is
   available; recommendation must not purchase anything automatically.
3. Recent discoveries: compact species or behavior cards using the existing
   role-color language and small pixel silhouettes.

This surface should feel like the home desk, not a dashboard full of graphs.
One prominent study card is more useful than many equal-weight widgets.

### Research — “Choose a hypothesis to fund”

- Left: Plant / Herbivore / Carnivore category tabs or a compact category rail.
- Center: branching research nodes with locked, available, affordable,
  unaffordable, selected, purchased, and newly-unlocked states.
- Right or lower panel: project detail, prerequisite chain, data cost, and the
  resulting run content.
- Purchase preview is an explicit overlay or detail state; representative data
  remains clearly marked until the economy service exists.

The tree should use connectors and status marks, not color alone, to show
prerequisites and availability.

### Species Archive — “Meet the organisms”

- Left: species index or horizontal specimen tray.
- Center: large selected species glyph and identity card.
- Right: role, behavior summary, known interactions, mastery data, and unlock
  guidance.

Use the supplied rabbit/fox silhouettes and terrain swatches at a deliberate
UI scale. Preserve nearest-neighbor character and avoid enlarging the glyphs
until their pixel clusters blur.

### Expedition Setup — “Turn observation into a run”

- Scenario card: Forest Edge, biome preview, and short ecological premise.
- Species card: Hare as the current slice player species, with role and key
  starting information.
- Starting options: only options that are actually available or explicitly
  labelled representative.
- Launch summary: scenario ID, species ID, seed policy, unlocked run choices,
  and the single Launch Expedition action.

This is the most action-oriented page. It should have the strongest primary
button and the least decorative density.

### Settings and overlays

Keep Settings quiet and functional. Use the same window chrome for profile,
purchase, leave-run, and error overlays so the system feels coherent without
adding another navigation pattern.

## Visual system proposal

### Shell palette

Use the Figma/Noesis pilot as the shell baseline and keep the legacy lime/cream
palette as an alternate experiment, not a second live theme.

| Token | Proposed use | Value |
| --- | --- | --- |
| Canvas | outer desktop background | `#0A1720` |
| Header | top bar and navigation backing | `#152B36` |
| Window | active feature surface | `#1B2A38` |
| Window raised | selected cards and primary controls | `#263F4B` |
| Text primary | headings and key values | `#FFFFFF` |
| Text secondary | body copy and supporting labels | `#D9E7F0` |
| Accent | action, focus, and discovery emphasis | `#F1C27D` |

Role colors remain semantic and stable inside specimen cards and data marks:

- plant: green;
- herbivore: blue/sky;
- carnivore: coral/red.

Avoid tinting entire screens by species role. Role color should identify data,
not make the Lab feel like three unrelated themes.

### Shape, type, and texture

- Major windows: 12px radius.
- Controls and cards: 8px radius unless a sharper specimen-card treatment is
  intentionally selected.
- Use 8/12/16/24 spacing increments from the existing token set.
- Use Pixeloid Sans for display and controls; use Pixeloid Mono for seeds,
  IDs, telemetry values, and technical readouts.
- Prefer flat fills and thin borders. Treat gradients, shadows, and decorative
  pixel texture as optional polish after the layout reads correctly.
- Keep pixel-art texture concentrated in specimen glyphs, terrain samples, and
  small decorative objects; do not make every panel noisy.

## Work sequence before implementation

### Concept pass — current lane

- Review the three concept families and choose the shell direction.
- Lock the shell palette, role-color use, type hierarchy, window chrome, and
  1280x720 layout contract.
- Produce one annotated static composition for the recommended direction.
- Confirm that the existing rabbit, fox, and terrain references are sufficient
  for the first archive/overview cards.

### Shell pass

- Convert the selected direction into shared Noesis resources and one static
  desktop composition.
- Validate focus, hover, pressed, disabled, selected, and overlay states.
- Check 1280x720 and 1920x1080 before feature-specific detail work.

### Feature pass

Implement visual compositions in this order:

1. Overview
2. Species Archive
3. Expedition Setup
4. Research
5. Settings and confirmation/error overlays

This order gets the emotional identity and primary player loop visible before
the most information-dense research surface.

### Acceptance pass

- Noesis XAML parses cleanly.
- Keyboard and mouse can traverse every primary Lab destination.
- Focus remains visible and Back behavior remains deterministic.
- The data bar, role identity, locked/available states, and representative-data
  labels are readable without relying on color alone.
- The shell remains legible at 1280x720 and does not depend on overlapping
  windows to communicate the route.
- Unity visual evidence is captured before the design is treated as runtime-
  accepted. The prior GalapagOS control-library handoff explicitly records that
  this visual gate is still open.

## Open decisions for review

1. Accept the dark slate Figma/Noesis pilot as the primary shell palette, with
   the lime/cream treatment retired to exploration status.
2. Accept Expedition Workbench as the default composition, with Archive Desk
   patterns used inside Species Archive.
3. Keep the desktop metaphor as controlled, movable multi-window chrome and
   navigation framing. Window orchestration belongs to the desktop shell, not
   `VM_Lab`.
4. Keep the global data bar persistent and use contextual mastery data only on
   the selected species surface.

No runtime implementation is part of this draft. Once these decisions are
accepted, the next artifact should be the annotated static shell composition,
not a broad feature implementation batch.
