# [Bio OS] (placeholder) - Game Design Document

> Status: Draft | Owner: [Josh Campbell] | Last updated: [2026-08-18] | Decision horizon: [prototype / vertical slice / alpha / release]

## How to use this document

This is the product/design source of truth. Keep it focused on player experience, rules, content, and decisions. Every section should distinguish **Committed**, **Experimental**, and **Open Question** content Link implementation details to the TDD instead of duplicating them.

## 1. Product definition

### Elevator pitch
Bio Os is an ecology simulation game that follows the trends of incremental games, with roguelike features and a cute, pixelated aesthetic.  Players focus on collecting and understanding different species and their interactions with each other in an ecosystem to discover emergent behavior based on cellular a automata engine 

### Player promise
[What the player repeatedly gets to do, feel, and master.]
The player will follow a rapid game loop where they make a decision, see the results of the new simulation, get evaluated and awarded currency (or penalty), return to the ecology lab, spend their currency on upgrades and new features, and run another simluation. As players understand the interactions within an ecosystem and unlock new upgrades and species/biome features they will master the ability to create successful species and balanced ecologies.


### Design pillars

1. **[Collection]** - Simulations collect data, data is used to collect new upgrades and species.  The entire game is about collecting a data set that is displayed in a cute and beautiful visual representation.
2. **[Legible ecological asymmetry]** - Species should create distinct ecological problems or opportunities, not merely have larger numbers.
3. **[Planning and progression across runs]** - Upgrades should change what players want to establish for future runs, rather than provide only immediate bonuses.
4. **[Visible cause and effect]** - Players should be able to explain successful strategies through terrain, resources, behavior, and population pressure.

### Non-goals

- [Explicitly out of scope.]
- [Explicitly out of scope.]

## 2. Core game loop

1.Player decisions (species selection, upgrades, settings)
2. Player Starts the simulation
3. Simulation Runs
4. Results screen
5. Player Decision (continue/end)
6. if continue -> select upgrade. else go back to step 1.
7. Run next simulation

### Moment-to-moment cadence

- Run start: [player selects which type of run and hit start]
  
- Active phase:
- [cellular automata ecological simulation runs during this phase.  Currency is accrued during this phae as well]
  
- Decision points:- [selecting a species to simulate, selecting a biome, selecting upgrades, choosing when to end the run]
  
- Run end: [Species collapse, player either ends run because they think the run will be a bad result, or they reach the time cap.]

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

[How upgrades are earned, selected, combined, and communicated.]

### Between-run progression

[Unlocks, persistent upgrades, scenario/species unlocks, and reset rules.]

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
| [Date] | [Change] | [Reason] | [Sections] |
