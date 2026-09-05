# [Bio OS] (placeholder) - Game Design Document

> Status: Draft; consecutive-phase direction committed, implementation pending | Owner: Josh Campbell | Last updated: 2026-09-04 | Decision horizon: prototype / vertical slice

## How to use this document

This is the product/design source of truth. Keep it focused on player experience, rules, content, and decisions. Every section should distinguish **Committed**, **Experimental**, and **Open Question** content Link implementation details to the TDD instead of duplicating them.

## 1. Product definition

### Elevator pitch
Bio Os is an ecology simulation game that follows the trends of incremental games, with roguelike features and a cute, pixelated aesthetic.  Players focus on collecting and understanding different species and their interactions with each other in an ecosystem to discover emergent behavior based on cellular a automata engine 

### Player promise
[What the player repeatedly gets to do, feel, and master.]
The player prepares an expedition, observes its evolving ecosystem, reviews each
simulation phase, and buys an upgrade or skips it before continuing the same
world. Creatures and resources carry their current state into the next phase.
When the expedition ends, the player reviews its result and returns to the Lab
for the applicable progression and preparation for a new expedition. As players
understand species interactions and unlock upgrades, species and biomes, they
learn to create successful species and balanced ecologies. Currency and loss
rules remain governed by the product brief and scientific-data economy plan.


### Design pillars

1. **[Collection]** - Simulations collect data, data is used to collect new upgrades and species.  The entire game is about collecting a data set that is displayed in a cute and beautiful visual representation.
2. **[Legible ecological asymmetry]** - Species should create distinct ecological problems or opportunities, not merely have larger numbers.
3. **[Planning and progression across runs]** - Upgrades should change what players want to establish for future runs, rather than provide only immediate bonuses.
4. **[Visible cause and effect]** - Players should be able to explain successful strategies through terrain, resources, behavior, and population pressure.

### Non-goals

- [Explicitly out of scope.]
- [Explicitly out of scope.]

## 2. Core game loop

**Committed direction; runtime migration pending:**

1. Select the scenario, species and starting options; launch one expedition.
2. Create its ecosystem once and advance a simulation phase.
3. Freeze after the phase's final completed tick and show a phase summary.
4. If the expedition continues, buy one eligible upgrade or explicitly skip it.
5. Continue from the frozen ecosystem under the resulting rules. Preserve the
   board, creatures, resources, ages, energy, cooldowns and accumulated history.
6. Repeat until the expedition ends; show final results and return to the Lab.
7. Only an explicit new expedition or restart creates a new starting world.

The prototype phase is currently 20 simulation seconds at a 0.1-second step.
The [product brief](PRODUCT_BRIEF.md) specifies five 200-tick phases and four
decision breaks for the vertical slice. Its longer viewing-time target remains
a separate pacing decision. A reward break is not a new expedition, and ordinary
Continue is not a restart or a player disk-save operation.

Implementation, unresolved mechanic details, cross-project retests and document
coverage are in the [consecutive simulation plan](CONTINUOUS_SIMULATION_FLOW_PLAN.md).

### Moment-to-moment cadence

- Run start: [player selects which type of run and hit start]
  
- Active phase:
- [cellular automata ecological simulation runs during this phase.  Currency is accrued during this phae as well]
  
- Decision points:- [selecting a species to simulate, selecting a biome, selecting upgrades, choosing when to end the run]
  
- Run end: player-species extinction after a completed tick, an explicit player
  decision to end the expedition, or its final phase. A normal phase time limit
  opens a decision break rather than recreating the ecosystem.

## 3. Player agency and controls

### Player verbs

| Verb | When available | Cost/risk | Visible result | Failure case |
|---|---|---|---|---|
| [Verb] | [Condition] | [Cost] | [Feedback] | [Failure] |

### Control contract

- Primary input: [keyboard/mouse]
- Pause/speed controls: [Pausing, stopping ,restarting, speeding up & slowing down interval]
- Undo/retry rules: [none? maybe one per run? purchaseable? ]
- Accessibility requirements: [color blind mode, dyslexic font, support peripherals, epilepsy warning]

## 4. Simulation model

### Cell and layer model

[Describe terrain, resource/item, and creature occupancy. State what can coexist and what blocks what.]

### Tick/turn order

[Numbered order of aging, perception, movement, feeding, combat, reproduction, regrowth, metrics, and presentation snapshot.]

### Species contract - link to another document. there are too many species to keep here conveniently.

For each species, define:

- Role: [plant/resource, herbivore, carnivore, decomposer, etc.]
- Occupancy/layer: [resource or creature]
- Needs: [food, terrain, energy, shelter]
- Decision priorities: [what it seeks/avoids]
- Reproduction: [conditions and cost]
- Distinctive interaction: [what makes it worth adding]

## 5. Progression and upgrades - link to 2 documents here. (one for permanent upgrades, and one for per-run upgrades)

### In-run progression

Temporary upgrades are acquired at frozen phase boundaries and remain in
purchase order for the rest of the expedition. Skipping preserves both the
current world and the existing build. An upgrade changes subsequent rules; it
does not implicitly refill energy, respawn creatures, or reset terrain.
Initialization-only upgrades need an explicit eligibility/effect decision before
being offered mid-expedition. See [upgrade direction](UPGRADE_SYSTEM_DIRECTION.md).

### Between-run progression

Temporary run evolution ends when the expedition ends, not at each phase
summary. Lab unlocks and future permanent research follow their separate
[progression](UPGRADE_SYSTEM_DIRECTION.md) and [economy](SCIENTIFIC_DATA_ECONOMY.md)
contracts. Player save/load of an unfinished expedition remains outside the
initial slice; in-memory phase continuation is required.

### Upgrade template

- Name: [name]
- Player decision: [what choice it creates]
- Affected rule(s): [plain-language rule]
- Expected interaction: [what changes in the ecosystem]
- Counterplay/tradeoff: [why it is not strictly better]
- Telemetry needed: [metrics that prove it is working]
- Status: [Committed / Experiment / Open]

## 6. Species roster and scenarios

### Roster table

| Species | Role | Player-facing identity | Key dependency | Scenario use | Status |
|---|---|---|---|---|---|
| [Species] | [Role] | [Readable identity] | [Dependency] | [Use] | [Status] |

### Scenario template

- Name: [name]
- Fantasy/problem: [what makes this scenario distinct]
- Starting state: [terrain, species, resources]
- Pressure: [scarcity, predator, weather, terrain shift]
- Player choices tested: [choices]
- Success/failure: [criteria]
- Expected strategies: [at least two]
- Balance evidence: [what to measure]

## 7. Feedback, UI, art, and audio

- Causal feedback: [how the player learns why a change happened]
- Board readability: [terrain, layers, species, danger, resources]
- Player-facing UI: [screens and information hierarchy]
- Developer UI: [separate tools and diagnostics]
- Art direction: [shape, palette, readability, references]
- Audio direction: [idle, interaction, danger, reward, run-end cues]

## 8. Balance and validation

### Success signals

- [Player behavior or metric]
- [Player behavior or metric]

### Balance questions

- [Question that needs an experiment]
- [Question that needs an experiment]

### Playtest record

- Build/version: [version]
- Scenario/ruleset: [name/fingerprint]
- Seeds/runs: [range/count]
- Observed result: [result]
- Interpretation: [what it means]
- Decision: [keep/change/defer]

## 9. Open decisions and change log

### Open decisions

| Decision | Options | Evidence needed | Owner | Due |
|---|---|---|---|---|
| [Decision] | [Options] | [Evidence] | [Owner] | [Date] |

### Change log

| Date | Change | Reason | Impacted sections |
|---|---|---|---|
| 2026-09-04 | Make consecutive phases and purchase/skip continuation explicit. | The prototype currently rebuilds the world between windows; the requested design preserves it. Runtime migration is planned, not implemented. | Player promise, core loop, run end, progression |
| [Date] | [Change] | [Reason] | [Sections] |
