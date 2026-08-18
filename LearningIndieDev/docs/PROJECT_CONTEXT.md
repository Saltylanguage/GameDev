# Project context

This document records durable product and design context that should carry across
Codex desktop and IDE conversations. Keep it concise, update it when a decision
changes, and do not treat research references as approved implementation work.

## Current game direction

- The living design and engineering document templates are [`GDD_TEMPLATE.md`](GDD_TEMPLATE.md)
  and [`TDD_TEMPLATE.md`](TDD_TEMPLATE.md). Use them to capture player-facing
  decisions separately from implementation contracts; completed sections should
  replace placeholders rather than becoming a second set of informal notes.
- The first coupled species treatment is [`HARE_FOX_ITERATIVE_TREATMENT.md`](Species%20Design/HARE_FOX_ITERATIVE_TREATMENT.md);
  its execution plan is [`HARE_FOX_IMPLEMENTATION_PLAN.md`](Species%20Design/HARE_FOX_IMPLEMENTATION_PLAN.md);
  use it as the working design reference for the Forest Edge hare/fox balance
  fixture until the interaction experiments produce evidence for promotion.
- The repeatable shorthand for this workflow is **Species Design Treatment** or
  **Iterate Species Design**; [`SPECIES_DESIGN_TREATMENT_TEMPLATE.md`](Species%20Design/SPECIES_DESIGN_TREATMENT_TEMPLATE.md)
  is the reusable treatment format.

- The project is pivoting toward an iterative roguelike centered primarily on
  cellular automata. The working elevator pitch is **"cellular automata as a
  roguelike."**
- Broad prototyping is giving way to production planning and a focused vertical
  slice. The active workstreams are the upgrade-driven core loop, a deliberately
  small species/scenario roster, separate player and developer experiences,
  visual direction, audio feedback, and the first persistent roguelike unlock
  loop. [`ROADMAP.md`](../ROADMAP.md) records their dependencies and gates.
- The initial vertical-slice content selection is Forest Edge with hare as the
  player species, fern as support, fox as opposition, and Trailblazer, Warren,
  and Gardeners as the three intended builds. The rationale and validation gaps
  are recorded in [`VERTICAL_SLICE_SELECTION.md`](VERTICAL_SLICE_SELECTION.md).
- The player develops a cell and its ruleset over the course of a run. Levels,
  currency, or both may purchase new rules and improve existing ones.
- Candidate upgrades act on relative grid positions. Examples include adding
  `+1 block` to the cell directly above the player or gaining an attack effect
  on horizontally adjacent cells. These examples communicate the direction;
  they are not a finalized combat model or rules API.
- *Digseum* is a high-level reference for the intended iterative progression
  loop. It is inspiration for product direction, not a specification to copy.
- The existing island-survival vertical slice, generic grid, grid patterns, and
  cave-generation prototype are being retained. The island content is no longer
  assumed to be the primary product direction, while the grid and cellular-
  automata work are foundations for the new concept.
- The current island, shoreline, and jungle entrance use an authored pixel-art
  tile workflow. Preserve those retained assets unless a feature explicitly
  replaces or repurposes them.
- Each retained prototype has its own scene and composition root.
  `CellularAutomataPrototypeRuntime` owns the cellular-automata preview scene,
  while `GameRuntime` and `WorldRuntime` remain isolated in the
  `IslandSurvivorPrototype` scene. Do not reconnect the cave preview to the island
  runtime merely for convenience.

## Cellular-automata roguelike concept

The central design opportunity is to make cellular-automata rules the player's
build, progression, and interaction language rather than using cellular automata
only as a behind-the-scenes map generator. A cell's offsets, effects, thresholds,
and other rule parameters may become upgradeable game content.

Keep the early architecture flexible enough to explore:

- Directional offense, defense, support, movement, and resource effects.
- Rules represented through reusable relative-offset `GridPattern` data.
- Rules gained, upgraded, combined, or replaced during a run.
- Deterministic simulation where seeds and rule loadouts can reproduce bugs.
- Clear presentation of which cells a rule reads, writes, attacks, or protects.

The vertical-slice player agency, phase cadence, win/loss conditions, reward
timing, persistence boundary, launch target, and non-goals are defined in
[`PRODUCT_BRIEF.md`](PRODUCT_BRIEF.md). Opposing-cell behavior, detailed economy,
and rule-combination semantics remain open design questions. Do not silently
settle them in foundational grid code.

## Noesis presentation direction

- Noesis/XAML with ViewModels is the intended presentation stack for menus,
  HUD, settings, rewards, results, and other player-facing controls.
- Simulation and domain code remain UI agnostic. A Unity composition/controller
  layer projects completed simulation state into UI-ready ViewModels and applies
  player commands through explicit methods; neither `Grid<T>` nor simulation
  rules reference Noesis, XAML, or Unity UI types.
- The live cellular board is a high-throughput view, not a conventional list of
  controls. Compose it through XAML, but render its cells through one dedicated
  Noesis custom-rendering control backed by a presentation snapshot. This keeps
  the visual tree small while preserving Noesis styling and overlays.
- A templated `ItemsControl` is appropriate for paused cell inspection,
  debugging, and authoring tools. Do not use a `ListBox` or one XAML element per
  cell as the primary full-screen simulation renderer: its selection/layout
  overhead and virtualization do not help when every cell is visible.
- Profile the board in a real Noesis view before optimizing further. Prefer
  shared resources, solid brushes, and redraws only when a simulation tick or
  relevant presentation state changes.
- The `CellularAutomataPrototype` scene now composes a Noesis shell through
  `SpeciesSimulationNoesisHost` and `SpeciesSimulationViewModel`. The shell
  owns the running, paused, rewards, and results controls, while the existing
  IMGUI panel remains the fallback authoring surface for species-specific rules.
- The shell also edits global run settings (grid size, seed mode, population
  bounds, duration, step interval, and starting probabilities) through an
  explicit apply command. Settings are validated and applied only before a
  session starts, so an active run cannot be mutated underneath the simulation.
- Species-rule authoring remains available through an explicit legacy-panel
  handoff from the shell; it is hidden during normal Noesis settings flow so
  the two authoring surfaces do not overlap.
- The Noesis shell now also edits the complete current species-rule surface:
  movement, attack, block, diet, reproduction, energy, perception, crowding,
  wilt, food reserve, and seed-drop settings. The edit contract is plain data
  and applies only to the next run; it does not expose the private draft or
  mutate an active simulation.
- The cellular species preview uses the Noesis shell as its single runtime UI
  path. The former IMGUI board/settings fallback has been removed, so terrain
  and species presentation cannot silently diverge between two renderers.

## CellularSimData direction

- The next architecture step is a `CellularSimData` scenario definition that
  groups global settings, starting population settings, species rules, and
  terrain data for one simulation ruleset.
- A simulation receives a run-start snapshot of this data. Editing, replacing,
  adding, or removing definitions affects the next run and never mutates an
  active run's state.
- Initial-grid creation remains a factory concern; simulation stepping remains a
  domain concern; `Grid<T>` remains a generic data container.
- `SpeciesId` is now the primary stable identity for species rules, cells,
  simulation, initialization, results, and preview settings. The old
  `SpeciesArchetype` enum remains only as an obsolete compatibility shim.
- `TerrainId` and `TerrainDefinition` now provide a small data-driven terrain
  registry owned by `CellularSimData`. `SpeciesCell` keeps terrain identity and
  occupant identity separate, with passability, movement cost, presentation
  color, and resource metadata available to the simulation.
- Population snapshots now use read-only `SpeciesId`-keyed metrics while
  preserving compatibility accessors for the original three species.
- `CellularSimData` now produces a versioned, deterministic SHA-256 fingerprint;
  data-backed runs and results carry it for A/B comparison and replay metadata.
- Alpha offspring is the first focused custom rule: configured species can
  produce chance-based alpha newborns with health and energy bonuses. Alpha
  configuration is part of the immutable ruleset and its fingerprint; special
  qualification and inheritance are still exploratory.
- `CellularSimDataAsset` is the first Inspector authoring path. It converts
  serialized scenario values into a fresh immutable `CellularSimData` at run
  start and is never mutable run state. It currently uses bare/grass terrain
  defaults; the existing runtime settings UI remains the fastest experiment path.
- `SpeciesDefinitionAsset` and `ScenarioDefinitionAsset` now provide the
  reusable authoring pipeline: species assets share one rule surface through
  plant/herbivore/carnivore role subclasses, and scenarios compose three to six
  species assets into immutable runtime data. Starting probability belongs to
  each scenario/species entry rather than the reusable species asset. The first
  authored library contains fern, reed, hare, deer, snail, beetle, fox, wolf,
  owl, and stoat, plus a compatibility-only baseline scenario.
- The authored baseline was proven equivalent to the legacy defaults by matching
  the ruleset fingerprint and final grids for deterministic seeds 10100-10104.
  Three 20-seed scenario reports are recorded in the dated handoff journal; they
  are balance experiments, not product-tuned defaults.
- Species awareness is immutable `SpeciesRules` data: vision is currently a
  Moore-range `GridPattern`, while intelligence is an initial priority tier.
  Perception reads a source grid, `SpeciesNavigation` uses seeded breadth-first
  search over the existing movement pattern, and the simulation applies the
  chosen one-cell move through its normal claim and crowding rules. This
  preserves deterministic source/next-grid stepping without embedding
  pathfinding in `Grid<T>`.
- Movement speed is an expected number of move attempts per tick: its whole
  portion is guaranteed and its fractional portion is a seeded chance for one
  additional attempt. The effective speed for a move is divided by the
  destination terrain's movement cost. Movement speed is included in the v4
  ruleset fingerprint; terrain movement cost was already fingerprinted.
- Occupied entities gain one age at the beginning of each tick, so newborns
  finish their birth tick at age zero. Resource terrain applies its authored
  regrowth every tick, including while a creature occupies the same cell.
- Creature and resource layers remain independent through movement, mortality,
  feeding, reproduction, wilt, and population-limit removal. Depleting a terrain
  resource sets its energy to zero without erasing the terrain or an occupant;
  later regrowth can restore the resource layer.
- Creature hunger is represented by `Energy`. Each species has an authored
  `ForageBelowEnergy` threshold; creatures only attack or seek their diet target
  at or below that value. The threshold survives runtime edits and upgrades and
  is included in the v4 ruleset fingerprint.
- A creature's `FoodReserve` is finite carried material for seed dispersal, not
  its hunger state. A successful seed drop consumes one reserve. Creature
  reproduction similarly conserves ordinary energy: the configured reproduction
  amount is transferred from the parent to the newborn. Plant propagation and
  alpha bonuses remain intentional environmental/special-rule energy sources.
- Rebuilding rules through the runtime editor or an upgrade must preserve the
  species role. Role is authored identity, not a default inferred by those paths.
- The current priority policy is deliberately small: hungry creatures seek
  visible food; intelligence tier one or higher may prefer a viable visible mate
  after the reproduction energy threshold is met. It is a prototype policy, not
  a general AI system. Scent remains deferred because it needs a separate
  stateful field/diffusion model rather than a delayed sight check.
- If a future cross-system message mechanism becomes necessary, restrict it to
  discrete output events after a completed tick. A global bus must not mutate
  simulation state mid-tick; direct calls remain preferable for a single local
  consumer.
- Future custom rule work should stay focused on a concrete mechanic such as
  sight; editor work should add terrain presets or preview asset selection only
  when that authoring workflow is needed.

## Cellular simulation iconography direction

- The preferred species visual language is the supplied colorized reference:
  bold, compact geometric silhouettes with flat fills, high contrast, and one
  or two immediately identifiable features per species (ears, antlers, beak,
  mane, tail, shell, and so on).
- The monochrome reference sheets are useful for silhouette and pose research;
  the colorized sheet is the stronger target for in-game presentation.
- Keep the existing role colors for readability: plants green, herbivores blue,
  and carnivores red. Species-specific shape and a restrained accent color can
  provide identity without weakening the role language.
- The Noesis board now consumes transparent raster atlases derived from the
  supplied animal and terrain references instead of the previous hard-coded
  vector silhouettes. The atlas build is deterministic and preserves the
  source's compact, high-contrast visual language.
- Terrain presentation is separate from simulation state: `TerrainTileResolver`
  derives a four-cardinal-neighbor mask at render time and selects one of the
  16 variants for the grass or desert family. Neighbor masks and atlas indices
  do not belong in `SpeciesCell` or `CellularSimData`.
- The reference set contains animal symbols but no dedicated plant symbol.
  Plant-resource terrain currently uses the grass tile family; add a dedicated
  plant atlas before displaying a separate plant glyph rather than borrowing an
  animal icon. See [`CELLULAR_SPRITE_TILING_PLAN.md`](CELLULAR_SPRITE_TILING_PLAN.md).
- Unity batch tooling now provides a closed-editor test entry point and a seeded
  `CellularSimData` experiment runner. It emits ignored `artifacts/` reports
  containing scenario path, seed range, ruleset fingerprint, population history,
  final-population summaries, and per-species activity totals (births, food
  consumed, movement, combat, and directly resolved mortality causes). It is
  intentionally an evidence/automation seam rather than a custom editor-to-agent bridge; see
  [`UNITY_SIMULATION_TOOLING.md`](UNITY_SIMULATION_TOOLING.md).
- The serious research program for turning this evidence seam into an
  auditable AI-assisted ecology laboratory is defined in
  [`Research/AI_ASSISTED_ECOLOGY_LAB_RESEARCH_PLAN.md`](Research/AI_ASSISTED_ECOLOGY_LAB_RESEARCH_PLAN.md).
  It is proposed research, not an approved replacement for the production
  roadmap; promotion requires reproducible evidence and a human decision.
- `CellSim` is the project-root command surface for this workflow: `Test`,
  `Run`, `Report`, `Compare`, and `Baseline`. The first population-only baseline
  was superseded after correcting terrain-resource identity and layered population
  counts. The current schema-4 BaselineParity reference over seeds 10100-10119
  averaged 10.25 final herbivores with one extinction; its recorded herbivore
  deaths were predominantly starvation deaths. Treat the older schema-2
  all-extinction result as superseded, not current balance evidence.
- Deferred generalization work and its triggers are tracked in
  [`CELLULAR_SIM_TODOS.md`](CELLULAR_SIM_TODOS.md). Do not introduce dynamic
  terrain registries or arbitrary rule plugins until a concrete use case
  activates the corresponding TODO.

## Procedural cave generation research

Sebastian Lague's video **"Procedural Cave Generation (E01. Cellular Automata)"**
is a design and engineering reference for the existing cave prototype and for
understanding cellular-automata simulation fundamentals:

- Video: https://www.youtube.com/watch?v=v7yyZZjF1z4
- Companion source: https://github.com/SebLague/Procedural-Cave-Generation

The relevant technique represents a map as a two-dimensional binary grid, fills
it from a reproducible random seed, keeps its boundary solid, and repeatedly
smooths it by counting the eight cells surrounding each position. Later processing
should remove unusably small regions, ensure important areas are connected, and
translate the logical grid into project-native tiles and collision data.

For cave-shaped generation experiments, prefer this project-shaped design:

```text
Generation settings
    -> seeded random grid
    -> cellular-automata smoothing
    -> region cleanup
    -> connectivity validation
    -> tile and collision presentation
```

Keep the responsibilities separate:

- `CaveGenerationSettings`: dimensions, seed, fill percentage, and smoothing passes.
- `CaveMapData`: engine-independent logical grid.
- `CellularAutomataGenerator`: deterministic generation rules.
- `CaveRegionProcessor`: region cleanup and connectivity guarantees.
- `CaveTilePresenter`: conversion into authored tiles, visuals, and collisions.

Add Edit Mode coverage for deterministic seeds, solid boundaries, smoothing rules,
region cleanup, and connectivity. Add focused Play Mode acceptance coverage for
the presented entrance, traversal, collisions, and regeneration lifecycle.

## Guardrails

- Cross-developer AI collaboration may use Discord as a coordination and
  handoff layer, but the repository and Markdown context remain authoritative.
  A shared channel cannot guarantee identical private conversation context or
  local editor state. The staged integration plan is tracked in
  [`DISCORD_AGENT_COLLABORATION_TODOS.md`](DISCORD_AGENT_COLLABORATION_TODOS.md).

- Cellular automata is now the central product direction. Follow the player
  cadence, run-ending conditions, reward timing, persistence boundary, and
  target recorded in [`PRODUCT_BRIEF.md`](PRODUCT_BRIEF.md); revise that brief
  explicitly when playtest evidence changes a product decision.
- Procedural cave generation remains a useful prototype and possible supporting
  feature; do not assume that cave generation itself is the new core game loop.
- Do not replace the authored shoreline or jungle entrance with procedural output
  without an explicit design decision.
- Do not copy the tutorial into one large `MonoBehaviour`; adapt the concepts to
  the project's explicit, testable architecture.
- Generated layouts must be reproducible from a recorded seed so regression bugs
  can be recreated.
- Validate traversability and connectivity rather than assuming organic-looking
  output is playable.

## Noesis migration status

- Global settings, species-rule authoring, run controls, rewards, and results are
  now presented by `SpeciesSimulationShell.xaml` with
  `SpeciesSimulationViewModel`. The former IMGUI species editor is no longer
  part of the runtime path; older handoffs that describe it as a fallback are
  historical.
- `SpeciesSimulationBoard` is the first XAML game-board attempt. It is a single
  `FrameworkElement` custom renderer that receives the current
  `Grid<SpeciesCell>` and batches all cell rectangles through `DrawingContext`.
  It intentionally does not create one XAML element per cell.
- The board is presentation-only: simulation stepping still happens in
  `SpeciesSimulationPreview`/`SpeciesSimulationRunner`, and the custom control
  only reads the current grid and invalidates its visual when the tick changes.
- Elevated Unity validation on 2026-08-12 passed 75/75 Edit Mode tests and 4/4
  Play Mode tests after the custom board was added. Unity licensing handshake
  warnings remain machine-environment noise; they did not fail the suites.
- The same pass completed a 20-seed `CellSim Baseline` for seeds 10100-10119;
  the ignored report is `artifacts/cellular-experiment-20260812-061503/analysis.md`.
