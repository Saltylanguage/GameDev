# GalapagOS — Game Design Document

> Status: Working product baseline; Sprint 2 closed, Sprint 3 active after kickoff on 2026-09-17 | Owner: Josh Campbell | Last updated: 2026-09-17 | Horizon: prototype to vertical slice
>
> **Naming note:** GalapagOS is the working player-facing title. Final title approval remains open.

## How to read this document

This is the product and game-design source of truth. It describes the intended player experience, identifies what is playable now, and keeps unresolved decisions visible.

- **Committed** — approved direction or a contract already relied on by the project.
- **Implemented** — available in the current runtime, within the stated limits.
- **Planned** — named in roadmap v2.2 or a linked active plan, but not yet available.
- **Open** — requires a product decision or evidence before implementation.

Engineering ownership, execution order, scene wiring, and verification live in the [Technical Design Document](TDD_TEMPLATE.md). Scheduling lives in [`ROADMAP.md`](../ROADMAP.md). Roadmap v2.2 is the current working baseline. Sprint 2 closed and Sprint 3 was kicked off on 2026-09-17; S3 scope and capacity are committed in its control record.

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
- **Hare** as the player species and **Fox** as opposition; playable plant species are on hold;
- three intended Hare Mutation paths: **Trailblazer**, **Warren**, and **Gardeners**;
- one six-round expedition on the same evolving board, with 10 seconds of simulation time per round;
- five Mutation decision moments: none at the start of Round 1, one after each of Rounds 1–5, and none after Round 6;
- a final outcome, results review, and return to the GalapagOS Desktop;
- later permanent Hare Genome effects and persistence; the identity and launch-snapshot foundation is already in place.

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
  -> round summary
  -> choose one of three Mutations or Skip (after rounds 1–5)
  -> continue the same world (through round 6)
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

**Test-contract decision, 2026-09-17:** opening Simulation from the GalapagOS Desktop is expected to start the current Forest Edge/Hare experience directly. This decision preserves the Bev/Sim simulation and Mutation setup; it does not resolve the missing profile and launch-request handoff described above. The causes of the recorded PlayMode failures have not been verified from individual test results.

### Expedition cadence

- **Committed target:** six consecutive rounds, each 10 seconds of simulation time, with five Mutation-or-Skip decisions.
- **Implemented foundation:** one run can retain its board, creatures, resources, ages, energy, cooldowns, ordered upgrades, and accumulated evidence across phase boundaries.
- **No wall-clock target:** actual elapsed time varies with simulation speed and pauses; the desktop clock is available.
- **Continue:** resumes the frozen world under the current ordered rules. It is not a restart and does not rebuild the starting ecosystem.
- **Restart:** removed as a player action.
- **End expedition:** after a Yes confirmation, abandons the run. Ending before round 6 forfeits rewards.
- **Mutation choices:** three options or Skip after rounds 1–5; there is no initial choice or post-round-6 choice. Present each with an icon and keyword, not a statistic breakdown.
- **Victory:** the player species survives through round 6. Extinction ends the run immediately as a failure with no rewards.
- **Currency:** successful-run currency uses a performance measure in development, not simply final population. A Skip bonus and additional bonus-event awards are undecided and non-blocking.
- **Permanent progression:** Genome Upgrades are bought with currency in the Gene Lab application on the GalapagOS Desktop and fill a Genome skill tree.
- **Authored default:** Forest Edge uses a 36×20 grid; playable plant species, including Fern, are on hold.
- **Disk resume:** outside the initial slice; same-world continuation is currently in memory only.

### Expedition outcomes

The intended terminal presentation distinguishes:

- **Victory:** the player species survives through round 6, in any biome.
- **Failed run:** extinction ends the run immediately and gives no rewards.
- **Player-ended run:** confirming End abandons the run; before round 6, it gives no rewards.

Successful-run currency uses the performance measure under development. The
exact performance-to-currency formula is not set here. Additional bonus-event
rewards are possible but not decided.

## 3. Player agency and controls

| Player action | Intended availability | Current state | Remaining design or connection |
| --- | --- | --- | --- |
| Create/select profile | Main Menu | **Implemented, limited:** ID, display name, and per-species Genome IDs | Carry the selected profile into Desktop and add versioned progression/settings persistence. |
| Prepare expedition | Desktop Planner | **Not available** | Define the smallest planner interaction and immutable launch payload. |
| Start simulation | After valid preparation | **Partly implemented:** Desktop starts a local default preview | Connect selected profile, scenario, player species, seed, schedule, and frozen progression. |
| Pause/resume | During a running round | **Implemented** | Preserve state and clear feedback in the final shell. |
| Change speed | During a running phase | **Visible but disconnected** | Bind controls and define allowed speeds, labels, and accessibility behavior. |
| Zoom the board | While viewing the simulation | **Visible but disconnected** | Bind controls and define scale limits and focus behavior. |
| Inspect a cell/species | While viewing the simulation | **Domain/ViewModel support exists but is unreachable** | Add pointer-to-cell selection and decide what Field Notes reveals. |
| Open Field Notes | From the simulation shell | **Visible but disconnected** | Define content, ownership, and whether it pauses the run. |
| Choose a Mutation | After rounds 1–5 | **Partly implemented:** three visible offer slots | Make the options glanceable and predictable without full statistic breakdowns. |
| Skip | After rounds 1–5 | **Implemented** | Bonus currency is undecided and non-blocking. |
| Continue same world | After Mutation or Skip | **Implemented foundation** | Complete it through the canonical six-round player route. |
| End expedition | During the run | **Implemented in preview form** | Confirm Yes to abandon; before round 6, forfeit rewards. No Restart action. |
| Review results/accomplishments | At terminal state | **Representative shell only** | Add authored outcome, causal evidence, accomplishments, and settlement. |
| Configure Genome | Between simulations in Gene Lab | **Foundation only:** profile/snapshot/catalog/display contracts exist; no production map or player actions | Approve one node/effect/cost contract, then add buying, activation, rule application, and versioned persistence. |

### Input and accessibility target

- **Required input:** keyboard and mouse for Windows 64-bit Steam.
- **Target resolution:** 1920×1080; 1280×720 must remain functional.
- **Current accessibility direction:** pauseable observation, readable state changes, redundant text/icon/color cues, and controls that do not depend on rapid input.
- **Open accessibility scope:** color-vision modes, dyslexia-oriented font options, photosensitivity treatment, remapping, and peripheral support require an explicit accessibility pass before commitment.

## 4. Simulation model

### Board and layers

Each cell has terrain plus separate ecological occupancy data. Terrain, resource/plant state, and creature state can coexist only where the authored and runtime rules allow. Simulation decisions are resolved from the previous committed grid, then the next grid is committed as one completed tick.

The Forest Edge production scenario defaults to **36×20** (720 cells).
Simulation settings can override the grid before a run, so balance evidence
must record the actual scenario dimensions used.

### Slice species

| Species | Ecological role | Player-facing identity | Key pressure/relationship | Status |
| --- | --- | --- | --- | --- |
| Hare | Player herbivore | Mobile breeder and ecosystem shaper | Must find food, reproduce, and survive Fox pressure | **Implemented foundation** |
| Fox | Predator/opposition | Creates spatial and survival pressure | Hunts Hare; should create counterplay rather than arbitrary collapse | **Implemented foundation; evidence refinement active** |

Playable plant species and Fern/Plant naming are on hold. Do not include a plant
species in the current expedition contract; retain internal data identities as
implementation details until that work is resumed.

### Player-readable causal evidence

At minimum, the slice should let the player relate changes to:

- current and recent Hare and Fox populations;
- births, feeding, starvation, predation, and other meaningful deaths;
- resource recovery and depletion;
- the active ordered Mutation build and when each Mutation first became effective;
- the phase boundary and expedition-wide history;
- outcome validity when a measure is unavailable or incomplete.

Developer telemetry may be more detailed, but the player interface must not expose raw internal fields as the only explanation.

## 5. Progression

### Mutations: temporary expedition choices

Mutations are acquired only at the five frozen round boundaries after rounds
one through five. Round one begins without a choice; round six ends in results
without another choice. Each boundary offers three Mutations or Skip. Mutations
last only for the current run.

- Mutations apply in acquisition order to subsequent completed ticks.
- A Mutation does not implicitly refill energy, respawn creatures, or reset terrain.
- Initialization-only effects are launch-only and must not be offered mid-expedition as no-op choices.
- Skip preserves the world and current build. A bonus using Genome Upgrade
  currency is possible but undecided and non-blocking.
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

Permanent Genome Upgrades are bought with currency in the Gene Lab application
on the GalapagOS Desktop and fill a species' Genome skill tree. Every species
has a separate collection of permanently unlocked Genome node IDs and one
active configuration that can be changed between simulations. The implemented
profile and launch contracts freeze every participating species' active Genome
into the run, including species the player does not control. Playable plant
species are on hold.

The effective Species Simulation rules are:

```text
Natural species rules + frozen active Genome + ordered expedition Mutations
```

Biome Simulations use:

```text
Natural species rules + frozen active Genome
```

The first Genome foundation is implemented: stable per-species profile state, immutable launch/run/checkpoint/result snapshots, metadata-only node and map authoring types, an asset-free catalog, and generic Gene Lab display bindings. This foundation records which nodes are unlocked or active, but those nodes do not yet change simulation rules. The visualization-only Hare and Fox maps were removed from the player-home scene on 2026-09-17, so no dummy catalog is presented as production content. Production effects, node costs, buying and unlocking, player editing, versioned save migration and recovery, and the first approved production Genome map remain open. Named loadouts and node-reveal behavior remain deferred questions.

### Scientific data and settlement

Scientific data is the intended permanent research currency. The player should be able to understand what was earned, spent, banked, or lost and why. The current local profile can store Genome node IDs, but it has no production wallet, accomplishments, settlement history, schema migration, or approved purchase flow. Phase transfer rules, extinction loss, permanent purchase costs, and settlement presentation remain provisional in the [Scientific Data Economy](SCIENTIFIC_DATA_ECONOMY.md).

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

- Players can complete the six-round route without developer-only fields.
- Players understand that Continue preserves the same ecosystem.
- Players can identify at least two distinct viable build strategies and explain their tradeoffs.
- The UI communicates why the Hare population grew, struggled, or collapsed.
- Mutation timing and effects are visible in phase and expedition evidence.
- Actual session duration may be recorded for performance characterization, but
  there is no wall-clock pacing target or duration/memory closeout gate.

### Current evidence boundary

- Same-world continuation, phase checkpoints, ordered upgrades, and phase/expedition evidence are implemented and covered through CF-1–CF-5.
- EX-010 supports the approved Forest Edge continuation schedule only within its recorded seeds, values, and acquisition orders; it is not general balance proof.
- EX-011's narrow finding is accepted as bounded evidence that Faster Movement followed by Crowding Tolerance increased final Deer population in the tested Open Range/Deer setup. It does not establish either upgrade's effect by itself or approve player fun, production balance, or generalized transfer.
- Target-resolution graphics checks and a Windows development-build smoke are recorded as complete.
- Full six-round session duration and peak-memory measurement remain optional CF-6 stretch work, not an M1 closeout gate.
- Broader Mutation balance, player comprehension, build diversity, and outcome tuning remain future evidence work.

## 8. Open decisions and newly explicit loose ends

| Decision or gap | Why it matters | Required next evidence/owner | Status |
| --- | --- | --- | --- |
| Final player-facing title | Menu branding and document naming currently differ in capitalization/history | Product owner review | **Open** |
| Playable plant species and Fern/Plant naming | Playable plants are on hold | Revisit only when plant play returns to scope | **Deferred; non-blocking** |
| Skip bonus | Skipping may earn Genome Upgrade currency; whether it does is undecided | Decide amount and limits only when needed | **Undecided; non-blocking** |
| Expedition Planner interaction and launch payload | Current acceptance starts Forest Edge/Hare directly; selected-profile/request handoff is still unresolved | Define an immutable request boundary before making launch choices authoritative | **Planned; underspecified** |
| Profile ownership across Desktop and simulation | Profile selection currently stops at scene validation | Engineering contract; planned with flow/persistence work | **Disconnected** |
| Desktop launch acceptance tests | Latest retained full suites pass, but direct-start acceptance does not prove selected-profile/request handoff | Add focused coverage when that handoff is implemented | **Current route verified; handoff remains open** |
| Mutation offer generation and economy | Only the first three catalog entries are reachable; costs and rerolls are not settled | S3 route work and later balance evidence | **Partly planned** |
| Performance-based currency formula | The measure is in development; final population alone is not the payout metric | Link the accepted metric and conversion when ready | **In progress; not a contract blocker** |
| Field Notes, cell inspection, speed, and zoom semantics | Visible controls imply agency that the current UI does not provide | UX contract, command binding, interaction tests | **Disconnected; tracked here** |
| Gene Lab, collection filters, expedition details/history | Enabled-looking Desktop affordances currently lead nowhere | S4–S7 contracts; disable or label until connected | **Planned or untracked at control level** |
| Currency settlement and bonus events | Successful-run payout follows the performance measure; bonus events remain optional | Finalize alongside the performance metric | **In progress; bonus events undecided** |
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
| 2026-09-23 | Set the Forest Edge production scenario default to 36×20. | Resolve the mismatch between the generated research scenario and the asset used by gameplay. |
| 2026-09-12 | Replaced the placeholder template with a current product baseline and explicit implemented/planned/open labels. | The old document mixed aspirations with implementation claims and did not expose disconnected player routes. |
| 2026-09-12 | Recorded cadence, board-size, Fern/Plant, profile handoff, Mutation reachability, result, and simulation-control gaps. | These gaps affect the main flow or evidence validity and need visible ownership. |
| 2026-09-12 | Aligned the slice with roadmap v2 while preserving its review status. | Prevent a proposed Sprint 3 plan from being mistaken for an approved commitment. |
| 2026-09-17 | Recorded the implemented Genome foundation, removed visualization-only Genome fixtures from the player route, and accepted direct-start Forest Edge/Hare as the Desktop test contract. | Keep design claims aligned with the executable route without promoting dummy content or hiding disconnected profile/launch ownership. |
| 2026-09-17 | Recorded the verified S3 kickoff and committed sprint scope. | Keep product status aligned with the active sprint control record. |
| 2026-09-17 | Replaced the old ten-phase/200-tick player contract with six 10-second simulation rounds and five Mutation choices; recorded current pause, End, victory, and failure rules. | Align product guidance to S3-02 while preserving historical research evidence. |
| 2026-09-06 | Established continuous same-world phase continuation as the canonical runtime foundation. | CF-1–CF-5 and EX-010 verified the bounded lifecycle/evidence contract. |
| 2026-09-04 | Locked the phase-boundary, Continue, Skip, restart, and launch-only initialization semantics. | Keep a reward break distinct from a new expedition or disk save. |
