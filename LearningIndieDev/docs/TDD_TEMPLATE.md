# [Game title] - Technical Design Document

> Status: Draft | Owner: [name/team] | Last updated: [YYYY-MM-DD] | Target build: [version/branch]

## How to use this document

This is the engineering source of truth for implementation boundaries, data contracts, deterministic behavior, tooling, and verification. It should answer “where does this belong, what are its invariants, and how do we prove it works?” Link back to the GDD for player intent.

## 1. Technical goals and constraints

### Goals

- [Technical goal]
- [Technical goal]

### Non-goals

- [Explicitly deferred technical work]

### Constraints

- Engine/version: [Unity version]
- Presentation stack: [Noesis/XAML/etc.]
- Target platform: [platform]
- Performance budget: [tick time, board size, frame budget]
- Determinism requirement: [yes/no and scope]

## 2. Architecture map

```text
[Input/UI] -> [Composition/Controller] -> [Domain snapshot] -> [Simulation]
                                      -> [Telemetry/Replay]
```

### Layer ownership

| Layer | Owns | Must not know about | Main entry points |
|---|---|---|---|
| Domain/simulation | [Rules/state] | [Unity/UI] | [Types] |
| Data/assets | [Authoring data] | [Runtime-only state] | [Types] |
| Composition | [Wiring/conversion] | [Game rules] | [Types] |
| Presentation | [ViewModels/rendering] | [Simulation internals] | [Types] |
| Tools/tests | [Validation/reporting] | [Player-only assumptions] | [Commands] |

## 3. Runtime data model

### Core entities and value objects

| Type | Responsibility | Immutable? | Identity/equality | Serialization |
|---|---|---|---|---|
| [Type] | [Responsibility] | [Yes/no] | [Rule] | [Rule] |

### Cell representation

[Document terrain, resource/item, creature, occupancy, empty/depleted states, and legal transitions.]

### Scenario and species data

[Document asset ownership, scenario-specific values, reusable species definitions, validation, and runtime-data creation.] 

## 4. Simulation pipeline

### Consecutive-phase lifecycle

Locked design, implementation pending: retain one domain run/runner across
reward breaks, with an absolute tick, prior source grid, evolving cells,
progression and accumulated telemetry. A phase boundary is resumable; terminal
completion is not. Apply validated immutable upgrade snapshots only at frozen
boundaries. Keep authoring changes and explicit new-expedition initialization
separate from Continue.

The [consecutive simulation plan](CONTINUOUS_SIMULATION_FLOW_PLAN.md) is the
current migration plan, including state ownership, clocks, checkpoint
contents, atomic upgrade/reward handling and tests. The
[evidence contract impact](CONTINUOUS_SIMULATION_EVIDENCE_IMPACT.md) defines the
locked phase/expedition reporting split and compatibility requirements. These
plans do not claim that the existing runtime already supports continuation.

### Tick order

1. [Copy source state]
2. [Age/metabolism/regrowth]
3. [Perception and intent selection]
4. [Movement/interaction]
5. [Combat/feeding/reproduction]
6. [Population limits and metrics]
7. [Commit snapshot]

### Invariants

- [Invariant, e.g. a creature and resource may coexist in separate layers.]
- [Invariant, e.g. source state is never mutated during a tick.]
- [Invariant, e.g. all random choices use the seeded RNG.] 

### Randomness and determinism

- Seed source: [source]
- Random consumption contract: [what must remain stable]
- Fingerprint/version: [format and version]
- Replay artifact: [what is recorded]

## 5. Feature design template

- Feature: [name]
- GDD intent: [link/section]
- Owning layer: [domain/data/composition/presentation/tooling]
- Public API/data contract: [types and methods]
- State transitions: [before -> after]
- Error/invalid-data behavior: [behavior]
- Determinism impact: [impact]
- Performance impact: [impact]
- Telemetry: [metrics/events]
- Test plan: [unit/integration/editor/playmode]
- Rollback/deprecation plan: [plan]
- Status: [Committed / Experiment / Open]

## 6. Authoring and validation

### Asset authoring contract

- Required fields: [fields]
- Defaults: [defaults]
- Ranges: [ranges]
- Cross-field validation: [rules]
- Migration/versioning: [strategy]
- Inspector/tool workflow: [workflow]

### Generated/runtime data

[Describe how authored assets become immutable runtime data and how fingerprints are calculated.] 

## 7. Presentation and tooling boundaries

### Player-facing UI

[ViewModels, commands, snapshot projection, accessibility, and feedback boundaries.]

### Developer Lab

[Runtime controls, scenario editing, visualization, metrics, and experiment export.]

### Debug/telemetry contract

| Signal | Producer | Consumer | Cost | Retention |
|---|---|---|---|---|
| [Signal] | [Producer] | [Consumer] | [Cost] | [Retention] |

## 8. Verification and operations

### Test matrix

| Area | Test type | Fixture/seeds | Assertion | Owner |
|---|---|---|---|---|
| [Area] | [Unit/integration/editor/playmode] | [Fixture] | [Assertion] | [Owner] |

### Repro steps

1. [Checkout/build step]
2. [Scenario/ruleset/seed]
3. [Observed failure or expected result]
4. [Artifact/report location]

### Performance checks

- Board size: [size]
- Tick budget: [budget]
- Worst-case species/cell count: [count]
- Measurement command/profile: [method]

## 9. Risks, decisions, and change log

### Technical risks

| Risk | Likelihood | Impact | Mitigation | Trigger |
|---|---|---|---|---|
| [Risk] | [L/M/H] | [L/M/H] | [Mitigation] | [Trigger] |

### Architecture decisions

| Date | Decision | Alternatives rejected | Consequence |
|---|---|---|---|
| [Date] | [Decision] | [Alternatives] | [Consequence] |

### Change log

| Date | Change | Reason | Impacted interfaces |
|---|---|---|---|
| [Date] | [Change] | [Reason] | [Interfaces] |
