# Project context

This document records durable product and design context that should carry across
Codex desktop and IDE conversations. Keep it concise, update it when a decision
changes, and do not treat research references as approved implementation work.

## Current game direction

- The project is pivoting toward an iterative roguelike centered primarily on
  cellular automata. The working elevator pitch is **"cellular automata as a
  roguelike."**
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

The turn cadence, player representation, opposing cell behavior, win/loss
conditions, economy, rule-combination semantics, and run structure remain open
design questions. Do not silently settle them in foundational grid code.

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
- Species awareness is immutable `SpeciesRules` data: vision is currently a
  Moore-range `GridPattern`, while intelligence is an initial priority tier.
  Perception reads a source grid, `SpeciesNavigation` uses seeded breadth-first
  search over the existing movement pattern, and the simulation applies the
  chosen one-cell move through its normal claim and crowding rules. This
  preserves deterministic source/next-grid stepping without embedding
  pathfinding in `Grid<T>`.
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
- Unity batch tooling now provides a closed-editor test entry point and a seeded
  `CellularSimData` experiment runner. It emits ignored `artifacts/` reports
  containing scenario path, seed range, ruleset fingerprint, population history,
  and final-population summaries. It is intentionally an evidence/automation
  seam rather than a custom editor-to-agent bridge; see
  [`UNITY_SIMULATION_TOOLING.md`](UNITY_SIMULATION_TOOLING.md).
- `CellSim` is the project-root command surface for this workflow: `Test`,
  `Run`, `Report`, `Compare`, and `Baseline`. The first 20-seed default baseline
  is evidence, not target balance: plants and herbivores reached final extinction
  in every run while carnivores survived all runs. See the dated tooling handoff
  for exact results before choosing the next balance hypothesis.
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

- Cellular automata is now central product direction, but the exact game loop is
  still exploratory. A high-level pivot does not authorize large speculative
  frameworks before the rules and feedback loop are playable.
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
