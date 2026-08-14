# Cellular Automata Roguelike production roadmap

This roadmap replaces the completed Island Chores prototype roadmap. The
retained island work remains useful reference material, but current production
is centered on **cellular automata as a roguelike**.

The roadmap is organized around playable outcomes. Technical systems, content,
art, audio, and tools are scheduled only when they help reach one of those
outcomes.

## Product goal

Deliver a run-based game in which the player develops a species by choosing
cellular-automata upgrades, watches those rules interact with other species and
the environment, and earns persistent scenario, species, and upgrade unlocks
from accomplishments during the run.

Before feature production accelerates, capture the exact player action cadence,
run-ending conditions, reward cadence, and launch target in a one-page product
brief. These details remain decisions to make, not assumptions for foundational
code.

The active player-shell delivery sequence, including Main Menu, the Lab home
base, profile persistence, scientific data, permanent research, branching run
upgrades, and species mastery, is defined in
[`docs/MAIN_MENU_LAB_DELIVERY_PLAN.md`](docs/MAIN_MENU_LAB_DELIVERY_PLAN.md).
That plan supplies the implementation epics, dependency order, and workflow;
this roadmap remains the product-level milestone source of truth.

## Production principles

- Make the upgrade decision and its visible consequence the center of play.
- Prefer a small roster of distinct species over many lightly differentiated
  species.
- Keep simulation runs deterministic and record the seed, scenario, ruleset,
  and upgrade loadout used for comparisons and bug reports.
- Separate pleasant player-facing flows from complete developer authoring and
  diagnostics.
- Treat art, audio, and causal feedback as simulation readability, not final
  polish.
- Add a tool only after a repeated production task identifies its inputs,
  outputs, and friction. Reuse the existing scenario assets and `CellSim`
  commands first.
- Promote research ideas such as colony construction only through bounded
  experiments after the core loop is proven.

## Parallel workstreams

### 1. Core loop and upgrades

Define the run cadence and build an upgrade vocabulary from simulation values
with known behavior. Begin with a small explicit catalog rather than a general
rule scripting or modifier framework.

Required outcomes:

- Base species rules plus ordered run upgrades produce one immutable,
  fingerprinted effective ruleset.
- Each upgrade has a visible effect, valid range, stacking rule, preview, and
  measurable activation or contribution.
- Seeded baseline-versus-upgraded comparisons expose dead values, balance
  cliffs, dominant choices, and interactions.
- At least three builds create understandable and measurably different play
  styles in the same scenario.

### 2. Species and scenario content

Co-design species with the upgrade vocabulary. A species earns its place by
changing which upgrades and strategies are valuable, not merely by having
different starting numbers.

Required outcomes:

- Select a vertical-slice roster from the existing authored species rather than
  balancing the entire library at once.
- Give each selected species a concise identity: resource relationship,
  movement or spatial behavior, pressure, strength, weakness, and upgrade
  affinity.
- Maintain a small scenario matrix that pressures different strategies without
  prescribing one solution.
- Add or remove species from the production roster based on playtest evidence.

### 3. Player UI and Dev Lab

Split the current all-purpose simulation shell into two experiences:

- **Player UI:** scenario context, readable board, run controls, cell/species
  inspection, upgrade choices, rewards, and results. It must not expose raw
  tuning fields.
- **Dev Lab scene:** scenario and species selection, seed controls, all tuning
  fields, runtime simulation controls, metrics, and comparison diagnostics.

Noesis remains the player-facing presentation stack. Simulation and scenario
domain code remain independent of both views.

### 4. Art direction and readability

Iterate at actual board scale until terrain, role, species, selection, danger,
and upgrade effects are readable and visually coherent.

Required outcomes:

- Compare a small number of complete visual directions in context rather than
  polishing isolated icons.
- Keep role recognition immediate while making selected species silhouettes
  distinct.
- Establish a shared palette, typography, spacing, panel, icon, animation, and
  effects language for the board and surrounding UI.
- Lock a vertical-slice direction before producing a large species asset set.

### 5. Audio and simulation feedback

Audio begins during the vertical slice. Dense cellular activity must be
aggregated, prioritized, and rate-limited so that the simulation does not
become noise.

Required outcomes:

- Define an audio palette for UI, upgrades, rewards, ambience, and major
  simulation state changes.
- Provide clear feedback for selection, confirmation, danger, success, failure,
  and meaningful rule activation.
- Test an event-aggregation policy for frequent births, deaths, attacks, and
  resource events before adding a large sound catalog.
- Include music and ambience direction in the slice, even if the first assets
  are temporary.

### 6. Roguelike shell and persistence

Once one run and its rewards are proven, add the surrounding game structure:

- Main menu and continue/new-run flow.
- A player-facing Lab home base with Overview, Research, Species Archive, and
  Expedition Setup surfaces.
- Run results and accomplishment evaluation.
- Persistent unlocks for scenarios, species, and eligible upgrade content.
- Versioned save data for settings and meta-progression; active-run persistence
  is a separate decision.
- A clear next-run flow that demonstrates why the previous run mattered.

Future progression will use scientific data collected from meaningful simulation
observations. Players may spend it on current-run evolution or bank it for
permanent research in the Lab, including ecological data categories and
species-specific mastery. This direction is defined in
[`docs/SCIENTIFIC_DATA_ECONOMY.md`](docs/SCIENTIFIC_DATA_ECONOMY.md) and remains
deferred until the vertical-slice upgrade loop establishes earning rates,
spending pressure, and useful permanent unlocks.

Permanent Lab progression will use Plant, Herbivore, and Carnivore research
trees, while upgrades selected during a run form temporary branching paths that
produce distinct playstyles. See
[`docs/UPGRADE_SYSTEM_DIRECTION.md`](docs/UPGRADE_SYSTEM_DIRECTION.md).

### 7. Production tools and quality

Likely use cases include species/scenario authoring, seeded A/B comparisons,
parameter sweeps, representative replay or visual capture, definition
validation, and build verification. Implement each only when its repeated
manual workflow is understood.

Maintain focused tests for deterministic runs, occupancy and resource
invariants, upgrade application, victory/reward conditions, and save migration.
Set board-size, tick-time, entity-count, and UI redraw budgets before optimizing.

## Milestones and gates

### M0 - Production definition

Exit criteria:

- A one-page product brief defines player agency, run cadence, victory/defeat,
  reward timing, persistence, target platform, and explicit non-goals.
- The vertical-slice species roster, scenario, and intended three build styles
  are named.
- The player UI and Dev Lab responsibilities are agreed.

### M1 - Playable upgrade loop

Exit criteria:

- A player can begin a curated scenario, observe the simulation, earn and choose
  upgrades, see their effects, and reach a result without raw developer fields.
- The Dev Lab can reproduce the same run and compare its base and upgraded
  rulesets.
- A first catalog of roughly 6-10 upgrades includes numeric, spatial,
  conditional, and tradeoff examples without a generalized plugin framework.
- The UI-only Main Menu and Lab shell demonstrates the intended home-base and
  expedition flow with representative data.

### M2 - Vertical slice

Exit criteria:

- One scenario supports at least three understandable builds across a small,
  visually distinct species roster.
- The complete main-menu-to-run-to-reward-to-next-run flow works with versioned
  meta-progression.
- Player UI, selected art direction, initial audio language, onboarding, and
  results presentation are coherent enough for external playtesting.
- Players can explain what their upgrades changed and the main cause of their
  outcome.

### M3 - Content alpha

Exit criteria:

- Several scenarios and species create distinct strategic pressures using the
  proven upgrade grammar and authoring pipeline.
- No universally correct upgrade path dominates representative seeded runs or
  structured playtests.
- Performance and save compatibility meet the agreed production budgets.
- The feature set is locked; unproven ideas remain research or post-launch
  candidates.

### M4 - Beta and release preparation

Exit criteria:

- Content is complete and the focus is defects, balance, onboarding,
  accessibility, input, performance, compatibility, store/platform work, and
  release operations.
- Representative runs, save upgrades, and supported hardware have repeatable
  validation coverage.

## Initial sprint plan

Default cadence is two weeks. Sprint 0 is a shorter planning sprint. Do not put
calendar dates on later work until owner capacity and the product brief are
known. Each sprint has one primary playable outcome; exploratory art and audio
work may run alongside it without displacing that outcome.

### Sprint 0 - Lock the slice

Primary outcome: the team can describe exactly what will be demonstrated by the
vertical slice.

- Write the one-page product brief.
- Name one slice scenario, the player species, opposing/supporting roster, three
  target build styles, run-end conditions, and reward cadence.
- Inventory existing upgrades and simulation parameters; identify useful ranges
  and missing telemetry rather than designing a large catalog.
- Make a screen-flow sketch covering main menu, run, upgrade choice, results,
  unlock, and next run.
- Write the Dev Lab use cases and choose the minimum useful controls.
- Gather a compact art and audio reference board and define evaluation criteria.

Exit: M0 criteria are met and Sprint 1 has no unresolved product-level blocker.

### Sprint 1 - Separate play from authoring

Primary outcome: a player can run the selected scenario without seeing the
tuning interface.

- Create the Dev Lab scene around the existing runtime settings, scenario
  assets, board, and diagnostics.
- Reduce the player scene to scenario context, start/pause/speed controls,
  essential inspection, and results placeholders.
- Keep both scenes on the same simulation-domain APIs and immutable run-start
  data.
- Establish the first visual and audio spike inside the player scene at actual
  board scale.
- Build the UI-only Main Menu and player-facing Lab shell defined by the active
  delivery plan; keep it distinct from the Dev Lab authoring scene.

Exit: raw parameters are available in the Dev Lab and absent from normal play;
the same fixed seed produces the same run in both scenes.

### Sprint 2 - First trustworthy upgrades

Primary outcome: the player makes an upgrade choice whose effect is predictable
and visible.

- Define explicit application and stacking semantics for the first 6-10
  upgrades.
- Record the selected upgrade loadout in the effective ruleset and run result.
- Add effect previews and the minimum activation/contribution telemetry needed
  to evaluate the catalog.
- Cover upgrade application, invalid combinations, and deterministic replay
  with focused tests.

Exit: at least one numeric, one spatial, one conditional, and one tradeoff
upgrade can be selected, previewed, observed, and reproduced.

### Sprint 3 - Species/build co-design

Primary outcome: three builds produce distinct strategies rather than only
different final numbers.

- Select and tune the smallest vertical-slice roster from the existing species
  assets.
- Run fixed-seed baselines, parameter sweeps, and representative visual reviews.
- Remove or revise upgrades that rarely activate, always win, or feel identical.
- Add tooling only for a comparison task that has become repeatedly expensive.

Exit: three named builds have distinct behavior, strengths, weaknesses, and
scenario interactions, with both simulation evidence and an in-game review.

### Sprint 4 - Presentation and feedback pass

Primary outcome: a new player can read the board and understand important
changes without developer explanation.

- Choose and apply the vertical-slice art direction across the board and core
  player UI.
- Replace technical labels and forms with player language and progressive
  disclosure.
- Implement the initial audio palette and rate-limited simulation feedback.
- Add upgrade, danger, success, failure, and result feedback using coordinated
  visual and audio cues.

Exit: a short comprehension playtest can identify species roles, current
pressure, selected upgrade effect, and run outcome cause.

### Sprint 5 - Roguelike loop

Primary outcome: one completed run changes the choices available in the next.

- Add the main menu and new/continue flow.
- Evaluate accomplishments in run results.
- Persist the first scenario, species, or upgrade unlocks in versioned save
  data.
- Present earned rewards and return cleanly to the next-run flow.

Exit: a fresh profile can complete a run, earn a defined unlock, restart the
game, and use that unlock in a subsequent run.

### Sprint 6 - Vertical-slice validation

Primary outcome: an external player can complete and replay the slice without
developer intervention.

- Run structured onboarding, comprehension, build-diversity, and replay-intent
  playtests.
- Fix blockers, causal-feedback failures, dominant choices, save issues, and
  measured performance problems.
- Validate representative deterministic runs and supported input/display
  configurations.
- Decide which content enters M3 and which ideas remain research.

Exit: M2 criteria are met or the evidence produces a short, prioritized revision
plan. Do not begin broad content production before this gate.

## Explicitly outside the initial slice

- Polishing every currently authored species.
- A universal upgrade scripting, modifier, event-bus, or behavior-plugin system.
- A broad custom tooling suite without demonstrated workflows.
- Geometry-directed colony construction, ant tunnels, or beaver dams beyond a
  separately approved viability experiment.
- Large-scale procedural environment work unrelated to the selected scenario.
- Final-volume art or audio production before the slice direction is validated.

## Planning rhythm

At each sprint review, record:

- The playable outcome demonstrated.
- Evidence from seeded runs and human playtests.
- What changed in the product assumptions.
- What was cut or deferred.
- The next sprint's single primary outcome and exit test.

Track implementation tasks outside this document. Update this roadmap only when
milestone scope, dependencies, or product direction materially changes.
