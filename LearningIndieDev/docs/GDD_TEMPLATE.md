# [Game title] - Game Design Document

> Status: Draft | Owner: [name/team] | Last updated: [YYYY-MM-DD] | Decision horizon: [prototype / vertical slice / alpha / release]

## How to use this document

This is the product/design source of truth. Keep it focused on player experience, rules, content, and decisions. Every section should distinguish **Committed**, **Experimental**, and **Open Question** content. Link implementation details to the TDD instead of duplicating them.

## 1. Product definition

### Elevator pitch

[One or two sentences.]

### Player promise

[What the player repeatedly gets to do, feel, and master.]

### Design pillars

1. **[Pillar]** - [Why it matters and how the game proves it.]
2. **[Pillar]** - [Why it matters and how the game proves it.]
3. **[Pillar]** - [Why it matters and how the game proves it.]

### Non-goals

- [Explicitly out of scope.]
- [Explicitly out of scope.]

## 2. Core game loop

1. [Player action]
2. [Simulation response]
3. [Player interpretation/decision]
4. [Reward, risk, or progression]

### Moment-to-moment cadence

- Run start: [state and choices]
- Active phase: [what changes each tick/turn]
- Decision points: [when the player acts]
- Run end: [success/failure and reward]

## 3. Player agency and controls

### Player verbs

| Verb | When available | Cost/risk | Visible result | Failure case |
|---|---|---|---|---|
| [Verb] | [Condition] | [Cost] | [Feedback] | [Failure] |

### Control contract

- Primary input: [keyboard/controller/mouse/touch]
- Pause/speed controls: [behavior]
- Undo/retry rules: [behavior]
- Accessibility requirements: [requirements]

## 4. Simulation model

### Cell and layer model

[Describe terrain, resource/item, and creature occupancy. State what can coexist and what blocks what.]

### Tick/turn order

[Numbered order of aging, perception, movement, feeding, combat, reproduction, regrowth, metrics, and presentation snapshot.]

### Species contract

For each species, define:

- Role: [plant/resource, herbivore, carnivore, decomposer, etc.]
- Occupancy/layer: [resource or creature]
- Needs: [food, terrain, energy, shelter]
- Decision priorities: [what it seeks/avoids]
- Reproduction: [conditions and cost]
- Distinctive interaction: [what makes it worth adding]

## 5. Progression and upgrades

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
