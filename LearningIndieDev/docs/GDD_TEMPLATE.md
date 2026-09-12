# GalapagOS — Game Design Document

> Status: Working product baseline; roadmap v2 and proposed Sprint 3 still require product-owner review | Owner: Josh Campbell | Last updated: 2026-09-12 | Horizon: prototype to vertical slice
>
> **Naming note:** GalapagOS is the working player-facing title. Final title approval remains open.

## How to read this document

This is the product and game-design source of truth. It describes the intended player experience, identifies what is playable now, and keeps unresolved decisions visible.

- **Committed** — approved direction or a contract already relied on by the project.
- **Implemented** — available in the current runtime, within the stated limits.
- **Planned** — named in roadmap v2 or a linked active plan, but not yet available.
- **Open** — requires a product decision or evidence before implementation.

Engineering ownership, execution order, scene wiring, and verification live in the [Technical Design Document](TDD_TEMPLATE.md). Scheduling lives in [`ROADMAP.md`](../ROADMAP.md). Roadmap v2 is the current working baseline, but its Sprint 3 allocation remains proposed until the Sprint 2 review.

## 1. Product definition

### Elevator pitch

GalapagOS is a cute pixel-art ecology game built on deterministic cellular automata. The player prepares an expedition, watches species interact in a living habitat, and makes roguelike Mutation choices between simulation phases. Across expeditions, observation and permanent Genome research let the player shape more capable species and increasingly resilient ecosystems.

### Player promise

The player can understand why an ecosystem changed, choose how to respond, and see that choice alter the same continuing world. Short-term Mutation builds create tension inside one expedition; long-term Genome choices create reasons to return, experiment, and master ecological relationships across runs.

### Design pillars

1. **Ecosystem stewardship.** The long-term goal is to develop diverse, resilient ecosystems, not merely complete a collection.
2. **Legible ecological asymmetry.** Species create different problems and opportunities through their roles, needs, and interactions—not just larger numbers.
3. **Planning across runs.** Temporary Mutations and permanent Genomes should change future plans and invite new experiments.
4. **Visible cause and effect.** The board, summaries, and evidence should help a player explain success, pressure, and collapse.
5. **Evidence before expansion.** New content is added only after the small vertical slice proves its rules, readability, and performance.

### Vertical-slice scope

The current slice centers on:

- the **Forest Edge** scenario;
- **Hare** as the player species, **Fern** as its food/support species, and **Fox** as opposition;
- three intended Hare Mutation paths: **Trailblazer**, **Warren**, and **Gardeners**;
- one ten-phase expedition on the same evolving board;
- a decision to take one offered Mutation or Skip at each of the first nine boundaries;
- a final outcome, results review, and return to the GalapagOS Desktop;
- later permanent Hare Genome progression, once its contract and persistence are ready.

### Non-goals for the slice

- Direct control of individual cells, action combat, or editing raw simulation state mid-tick.
- Broad species, biome, Mutation, or scenario production before the first slice is validated.
- Multiplayer, live services, cloud saves, or active-expedition disk save/resume.
- Controller, Steam Deck, macOS, Linux, console, or mobile support in the initial Windows slice.
- Large procedural worlds, caves, colony management, or unrelated simulation systems.
- Final-volume art, audio, localization, or accessibility production before the flow and readability are proven.
- A generalized modifier, evolution, plugin, or content framework built ahead of a concrete need.

## 2. Player journey and expedition loop

### Target main flow

```text
Launch
  -> Main Menu
  -> create or select a local profile
  -> GalapagOS Desktop
  -> Expedition Planner
  -> Simulation
  -> phase summary
  -> choose Mutation or Skip
  -> continue the same world (repeat through phase 10)
  -> final results and accomplishments
  -> return to Desktop
  -> Gene Lab / next expedition
```

This is the **committed product direction**, not a claim that every transition exists today.

### What is playable now

The current player route is:

```text
Main Menu -> profile ID/name -> Desktop -> embedded local simulation
          -> generic results/close -> Desktop
```

The Desktop simulation host creates and starts its own local preview. It does not yet receive the selected profile or an expedition-planning launch request. The standalone Lab-to-simulation route can pass an immutable launch request, but it is a legacy/developer path rather than the canonical player flow. These routes must be reconciled before the target journey is complete.

### Expedition cadence

- **Committed target:** ten consecutive phases of 200 completed ticks, with nine Mutation-or-Skip decisions.
- **Implemented foundation:** one run can retain its board, creatures, resources, ages, energy, cooldowns, ordered upgrades, and accumulated evidence across phase boundaries.
- **Current mismatch:** the player preview defaults to 100 ticks per phase. The runtime must be changed to the committed 200-tick schedule or the product target must be revised explicitly.
- **Continue:** resumes the frozen world under the current ordered rules. It is not a restart and does not rebuild the starting ecosystem.
- **Restart/new expedition:** creates a new starting world.
- **End expedition:** intentionally terminates the current run and proceeds to results.
- **Disk resume:** outside the initial slice; same-world continuation is currently in memory only.

### Expedition outcomes

The intended terminal presentation distinguishes:

- **Victory:** the player species meets the expedition’s authored success condition at the final boundary.
- **Narrow survival:** the player species persists but does not meet the stronger success condition.
- **Defeat:** the expedition ends without meeting the survival/success condition.
- **Immediate extinction:** the player species reaches zero after a completed tick and the run ends without waiting for the next phase boundary.
- **Player-ended expedition:** the player deliberately stops and receives an honest partial result.

The exact Forest Edge survival threshold, victory measure, wording, accomplishments, and unlock effects are **open**. The runtime currently shows a generic “Expedition complete” result, so these outcome categories are not yet player-available.

## 3. Player agency and controls

| Player action | Intended availability | Current state | Remaining design or connection |
| --- | --- | --- | --- |
| Create/select profile | Main Menu | **Implemented, limited:** ID and display name only | Carry the selected profile into Desktop and later persist progression/settings. |
| Prepare expedition | Desktop Planner | **Not available** | Define the smallest planner interaction and immutable launch payload. |
| Start simulation | After valid preparation | **Partly implemented:** Desktop starts a local default preview | Connect selected profile, scenario, player species, seed, schedule, and frozen progression. |
| Pause/resume | During a running phase | **Implemented** | Preserve state and clear feedback in the final shell. |
| Change speed | During a running phase | **Visible but disconnected** | Bind controls and define allowed speeds, labels, and accessibility behavior. |
| Zoom the board | While viewing the simulation | **Visible but disconnected** | Bind controls and define scale limits and focus behavior. |
| Inspect a cell/species | While viewing the simulation | **Domain/ViewModel support exists but is unreachable** | Add pointer-to-cell selection and decide what Field Notes reveals. |
| Open Field Notes | From the simulation shell | **Visible but disconnected** | Define content, ownership, and whether it pauses the run. |
| Choose a Mutation | At each non-terminal phase boundary | **Partly implemented:** three visible offer slots | Make all intended build paths reachable and readable. |
| Skip | At a non-terminal phase boundary | **Implemented** | No current bonus or penalty; any incentive needs a separate economy decision. |
| Continue same world | After Mutation or Skip | **Implemented foundation** | Complete it through the canonical player route at the 200-tick target. |
| End/restart expedition | During the run/results | **Implemented in preview form** | Confirm final warnings, outcome semantics, and return route. |
| Review results/accomplishments | At terminal state | **Representative shell only** | Add authored outcome, causal evidence, accomplishments, and settlement. |
| Configure Genome | Between simulations in Gene Lab | **Planned, unimplemented** | Define S4 contract; implement after profile ownership and persistence are stable. |

### Input and accessibility target

- **Required input:** keyboard and mouse for Windows 64-bit Steam.
- **Target resolution:** 1920×1080; 1280×720 must remain functional.
- **Current accessibility direction:** pauseable observation, readable state changes, redundant text/icon/color cues, and controls that do not depend on rapid input.
- **Open accessibility scope:** color-vision modes, dyslexia-oriented font options, photosensitivity treatment, remapping, and peripheral support require an explicit accessibility pass before commitment.

## 4. Simulation model

### Board and layers

Each cell has terrain plus separate ecological occupancy data. Terrain, resource/plant state, and creature state can coexist only where the authored and runtime rules allow. Simulation decisions are resolved from the previous committed grid, then the next grid is committed as one completed tick.

Forest Edge currently has a source-of-truth conflict:

- the checked-in scenario asset is **42×20**;
- the architecture map and scenario-generation tool specify **36×20**.

The authoritative board size is **open**. Until it is resolved, balance evidence must record the actual scenario dimensions it used.

### Slice species

| Species | Ecological role | Player-facing identity | Key pressure/relationship | Status |
| --- | --- | --- | --- | --- |
| Fern | Producer and Hare food source | Renewable food frontier | Growth and seed dispersal must sustain consumption | **Committed identity; runtime data conflict open** |
| Hare | Player herbivore | Mobile breeder and ecosystem shaper | Must find food, reproduce, and survive Fox pressure | **Implemented foundation** |
| Fox | Predator/opposition | Creates spatial and survival pressure | Hunts Hare; should create counterplay rather than arbitrary collapse | **Implemented foundation; evidence refinement active** |

The Forest Edge generator currently creates a legacy Plant resource, and the Hare asset consumes the `plant` identifier. Product documents call that species Fern. Decide whether Fern is the player-facing name for the existing Plant runtime identity or a distinct species/data migration; do not let both concepts drift silently.

### Player-readable causal evidence

At minimum, the slice should let the player relate changes to:

- current and recent Hare, Fox, and Fern populations;
- births, feeding, starvation, predation, and other meaningful deaths;
- resource recovery and depletion;
- the active ordered Mutation build and when each Mutation first became effective;
- the phase boundary and expedition-wide history;
- outcome validity when a measure is unavailable or incomplete.

Developer telemetry may be more detailed, but the player interface must not expose raw internal fields as the only explanation.

## 5. Progression

### Mutations: temporary expedition choices

Mutations are acquired only at frozen phase boundaries and last until that expedition ends. A complete ten-phase expedition has nine decisions. At each decision the player chooses one valid offered Mutation or explicitly Skips.

- Mutations apply in purchase order to subsequent completed ticks.
- A Mutation does not implicitly refill energy, respawn creatures, or reset terrain.
- Initialization-only effects are launch-only and must not be offered mid-expedition as no-op purchases.
- Skip preserves the world and current build, with no current reward or penalty.
- Mutations are exclusive to Species Simulations; Biome Simulations do not use them.

Seven Hare upgrade assets currently exist, but the player UI exposes only three offer slots and the preview loads the catalog in asset order. In practice, only the first three entries are reachable through that route: **Long Stride**, **Large Litters**, and **Far Sight**. The Warren and Gardeners options later in the catalog are therefore authored but disconnected. Offer generation and build-path reachability must be designed and implemented before claiming three playable builds.

Intended build identities:

| Build | Intended strength | Intended cost or obligation |
| --- | --- | --- |
| Trailblazer | Migration, perception, and access to fresh food | Weaker grouping/protection or higher operating cost |
| Warren | Local defense and controlled reproduction | Less mobility and more local resource pressure |
| Gardeners | Feeding efficiency and seed dispersal | Delayed payoff and weak immediate predator defense |

See [Upgrade System Direction](UPGRADE_SYSTEM_DIRECTION.md), the [Upgrade Catalog Acceptance Matrix](UPGRADE_CATALOG_ACCEPTANCE_MATRIX.md), and [`SG-005 — Upgrade and Ecology Balance`](Studio%20Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md).

### Genomes: permanent between-run progression

Every species is intended to have permanently unlocked Genome nodes and one active configuration that can be changed between simulations. A run receives a frozen snapshot of every participating species’ active Genome. Improving Hare must not silently disable Fox or Fern Genome choices.

The effective Species Simulation rules are:

```text
Natural species rules + frozen active Genome + ordered expedition Mutations
```

Biome Simulations use:

```text
Natural species rules + frozen active Genome
```

Genome runtime, Gene Lab configuration, unlock persistence, migration, and recovery are **not implemented**. Roadmap v2 places the first Genome contract in S4 and persistence-backed implementation in S6. Named loadouts and node-reveal behavior remain deferred questions.

### Scientific data and settlement

Scientific data is the intended permanent research currency. The player should be able to understand what was earned, spent, banked, or lost and why. The current profile does not store a wallet, accomplishments, unlocks, or Genome state. Phase transfer rules, extinction loss, permanent purchase costs, and settlement presentation remain provisional in the [Scientific Data Economy](SCIENTIFIC_DATA_ECONOMY.md).

## 6. Presentation direction

### UI world

The GalapagOS Desktop is the canonical player home. Its apps should make the game feel like an ecology field station operating system rather than a conventional debug menu.

- **Expedition Planner:** prepare and launch a valid simulation.
- **Simulation:** observe, pause, inspect, make phase choices, and review results.
- **Field Notes / Species Collection:** understand discovered species and interactions.
- **Gene Lab:** unlock and configure permanent Genome choices.
- **Expedition history:** revisit meaningful prior results once persistence exists.

The current Desktop visually includes several of these destinations, but the Gene Lab, expedition details/history, filters, Field Notes, speed, zoom, and cell inspection are partly or wholly disconnected.

### Art direction

- Cute pixel-art ecology presented through a warm field-research operating system.
- Cream, green, blue, yellow, coral, and brown establish the current UI palette.
- Species, terrain, danger, selection, and depleted resources must remain legible at board scale.
- Generated Main Menu meadow art and UI concepts are direction candidates, not final approved production assets.

### Audio direction

The Main Menu has an initial procedural chime. A systematic audio language for idle ambience, selection, danger, phase completion, Mutation choice, extinction, and expedition outcome is planned for the later art/UI pass; it is not part of the current executable loop.

## 7. Balance and validation

### Slice success signals

- Players can complete the ten-phase route without developer-only fields.
- Players understand that Continue preserves the same ecosystem.
- Players can identify at least two distinct viable build strategies and explain their tradeoffs.
- The UI communicates why the Hare population grew, struggled, or collapsed.
- Mutation timing and effects are visible in phase and expedition evidence.
- A complete Forest Edge run meets the approved duration and memory budget once that budget is recorded.

### Current evidence boundary

- Same-world continuation, phase checkpoints, ordered upgrades, and phase/expedition evidence are implemented and covered through CF-1–CF-5.
- EX-010 supports the approved Forest Edge continuation schedule only within its recorded seeds, values, and acquisition orders; it is not general balance proof.
- Target-resolution graphics checks and a Windows development-build smoke are recorded as complete.
- Outer ten-phase wall duration and peak-memory measurement remain open under CF-6.
- Broader Mutation balance, player comprehension, build diversity, and outcome tuning remain future evidence work.

## 8. Open decisions and newly explicit loose ends

| Decision or gap | Why it matters | Required next evidence/owner | Status |
| --- | --- | --- | --- |
| Accept or revise roadmap v2 and proposed S3 allocation | The baseline is current, but S3 is still a proposal rather than a commitment | Product owner at Sprint 2 review | **Open** |
| Final player-facing title | Menu branding and document naming currently differ in capitalization/history | Product owner review | **Open** |
| 200-tick phase target versus 100-tick preview default | The executable cadence does not match the committed design | Product decision, then runtime/config test | **Open; tracked here** |
| Forest Edge 42×20 asset versus 36×20 generator/map | Different board sizes invalidate direct balance comparisons | Choose authority and regenerate/migrate intentionally | **Open; tracked here** |
| Fern versus legacy Plant runtime identity | Species language, data IDs, art, telemetry, and saves need one contract | Product + engineering decision before migration | **Open; tracked here** |
| Expedition Planner interaction and launch payload | The canonical Desktop route starts a local default instead of the selected expedition | Small UX contract plus immutable request ownership | **Planned; underspecified** |
| Profile ownership across Desktop and simulation | Profile selection currently stops at scene validation | Engineering contract; planned with flow/persistence work | **Disconnected** |
| Mutation offer generation and economy | Only the first three catalog entries are reachable; costs and rerolls are not settled | S3 route work and later balance evidence | **Partly planned** |
| Forest Edge victory/survival threshold and result language | Generic completion cannot support a meaningful win/loss loop | Product rule plus fixed-seed validation | **Open; tracked here** |
| Field Notes, cell inspection, speed, and zoom semantics | Visible controls imply agency that the current UI does not provide | UX contract, command binding, interaction tests | **Disconnected; tracked here** |
| Gene Lab, collection filters, expedition details/history | Enabled-looking Desktop affordances currently lead nowhere | S4–S7 contracts; disable or label until connected | **Planned or untracked at control level** |
| Scientific-data settlement and extinction loss | Permanent progression cannot be implemented safely without it | Economy and persistence contract | **Open** |
| Accessibility acceptance | Basic direction exists, but concrete supported modes and tests do not | Dedicated accessibility pass | **Deferred** |

## 9. Authoritative references

- [`ROADMAP.md`](../ROADMAP.md) — current milestone and feature schedule baseline.
- [Product Brief](PRODUCT_BRIEF.md) — product boundary and progression intent.
- [Vertical Slice Selection](VERTICAL_SLICE_SELECTION.md) — first slice roster and experience.
- [Consecutive Simulation Flow Plan](CONTINUOUS_SIMULATION_FLOW_PLAN.md) — same-world phase lifecycle.
- [Upgrade System Direction](UPGRADE_SYSTEM_DIRECTION.md) — Mutation/Genome contract.
- [Scientific Data Economy](SCIENTIFIC_DATA_ECONOMY.md) — provisional currency and settlement rules.
- [GalapagOS Desktop Feature Set](GALAPAGOS_DESKTOP_FEATURE_SET_PLAN.md) — Desktop application direction.
- [Technical Design Document](TDD_TEMPLATE.md) — implementation state, architecture, and verification.

## 10. Change log

| Date | Change | Reason |
| --- | --- | --- |
| 2026-09-12 | Replaced the placeholder template with a current product baseline and explicit implemented/planned/open labels. | The old document mixed aspirations with implementation claims and did not expose disconnected player routes. |
| 2026-09-12 | Recorded cadence, board-size, Fern/Plant, profile handoff, Mutation reachability, result, and simulation-control gaps. | These gaps affect the main flow or evidence validity and need visible ownership. |
| 2026-09-12 | Aligned the slice with roadmap v2 while preserving its review status. | Prevent a proposed Sprint 3 plan from being mistaken for an approved commitment. |
| 2026-09-06 | Established continuous same-world phase continuation as the canonical runtime foundation. | CF-1–CF-5 and EX-010 verified the bounded lifecycle/evidence contract. |
| 2026-09-04 | Locked the phase-boundary, Continue, Skip, restart, and launch-only initialization semantics. | Keep a reward break distinct from a new expedition or disk save. |
