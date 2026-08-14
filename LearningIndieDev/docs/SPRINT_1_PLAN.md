# Sprint 1 Plan — Main Menu and UI-Only Lab

> Status: Discussion draft | Updated: 2026-08-14 | Cadence: two weeks | Team: Josh + Sim

## Sprint goal

Deliver a credible UI-only application shell:

```text
Launch -> Main Menu -> Lab -> Expedition Setup preview
```

The Lab communicates the planned scientific-data economy, permanent type research, and species mastery using representative data. It does not yet spend currency, persist a profile, or launch the real simulation.

This is deliberately one bucket from the larger delivery plan. Player/Dev Lab separation, scene-to-scene expedition launch, real currencies, saves, and functional research are later work.

## Capacity and effort assumptions

Use a **two-week sprint** with approximately **60 combined person-hours**:

| Contributor | Planning capacity | Primary responsibility |
| --- | ---: | --- |
| Josh | 20 hours | Product decisions, terminology, UX review, Unity scene decisions, playtesting, acceptance. |
| Sim | 40 hours | XAML/ViewModel implementation, representative-data wiring, navigation behavior, tests, documentation. |
| **Total** | **60 hours** | Includes an 8-hour uncertainty/integration reserve. |

These are planning assumptions, not timesheets. Recalculate scope at kickoff if either person's real availability differs. Do not plan more than roughly 85% of available time as feature work; the remaining capacity covers integration, review feedback, Unity/Noesis friction, and defects.

## Sprint review demonstration

The review should show one uninterrupted flow in a Windows development build:

1. Launch into Main Menu.
2. Enter the Lab.
3. Navigate Overview, Research, Species Archive, and Expedition Setup.
4. Show Research, Plant, Herbivore, and Carnivore Data plus contextual Hare Mastery using clearly labeled representative values.
5. Inspect affordable, unaffordable, and locked Herbivore research projects.
6. Open a purchase preview showing costs, prerequisites, benefits, and remaining balances without mutating fake data.
7. Inspect a Hare mastery objective and its possible unlock.
8. Select Forest Edge + Hare and reach a complete expedition summary whose launch action is visibly disabled or labeled as a later integration step.
9. Navigate Back to Lab Overview and Main Menu with correct focus behavior.

## Committed work packages

### S1.1 — UI contract and low-fidelity layouts

**Effort:** 6 hours — Josh 3, Sim 3

Define:

- screen states and Back/overlay behavior;
- layouts for Main Menu, Lab Overview, Research, Species Archive, and Expedition Setup;
- terminology for experiments, data, research projects, and mastery;
- all representative data and required visual states;
- 1920×1080 and 1280×720 layout expectations.

Acceptance:

- Every primary action has a destination, invalid state, and expected feedback.
- Currency, type research, and species mastery are visibly distinct.
- No implementation depends on an unresolved navigation decision.

### S1.2 — Main Menu and Lab navigation shell

**Effort:** 20 hours — Josh 4, Sim 16 | **Depends on:** S1.1

Use the existing `MainMenu.unity` and Noesis conventions to implement:

- Main Menu with Enter Lab, New Profile placeholder, Settings, Credits, and Quit;
- one explicit screen state for the known Main Menu/Lab flow;
- Lab navigation for Overview, Research, Species Archive, and Expedition Setup;
- deterministic Back and overlay behavior;
- visible keyboard focus and mouse operation;
- functional layouts at both target resolutions.

Acceptance:

- Every planned panel is reachable and returns correctly.
- Prototype-only actions are disabled or clearly labeled.
- XAML and ViewModels contain no simulation, wallet, or persistence logic.
- `MainMenu.unity` becomes the first Build Settings scene only after the shell launches reliably.

### S1.3 — Lab scientific-data presentation

**Effort:** 12 hours — Josh 4, Sim 8 | **Depends on:** S1.2

Add representative UI for:

- Research, Plant, Herbivore, and Carnivore Data balances;
- contextual Hare Mastery;
- a recent experiment-return summary;
- affordable, unaffordable, locked, selected, purchased, and newly available research states;
- one Herbivore prerequisite chain;
- purchase preview without balance mutation;
- one Hare mastery objective and possible content benefit.

Acceptance:

- Data type, cost, prerequisite, and benefit remain readable without color alone.
- The biology/experiment language is consistent.
- Type research cannot be mistaken for species mastery.
- Representative balances are not written to disk or modified.

### S1.4 — Expedition Setup preview

**Effort:** 6 hours — Josh 3, Sim 3 | **Depends on:** S1.2

Represent the future expedition contract without connecting scenes:

- Forest Edge scenario;
- Hare player species;
- representative starting options already supported by the design;
- unlocked-research summary;
- complete launch summary and a clearly disabled/prototype launch action.

Acceptance:

- The player can understand what experiment they are preparing.
- Invalid selections have a defined visual state.
- No Lab ViewModel, XAML state, or placeholder fixture leaks into simulation code.

### S1.5 — Verification and review preparation

**Effort:** 8 hours — Josh 4, Sim 4 | **Depends on:** S1.2–S1.4

Verify:

- ViewModel tests for screen, overlay, and Back transitions;
- Play Mode smoke coverage for Main Menu → Lab → Expedition Setup preview;
- keyboard/mouse navigation and visible focus;
- 1920×1080 and 1280×720 layouts;
- standalone Windows development-build launch;
- existing relevant test suites and `git diff --check`.

Acceptance:

- The sprint-review flow runs without Play Mode errors.
- Acceptance evidence is recorded in a handoff.
- A broken application entry path blocks sprint completion.

### Integration reserve

**Effort:** 8 hours — Josh 2, Sim 6

Reserved for Noesis/Unity scene friction, review changes, defects, and integration. It is not advance capacity for stretch features.

## Effort summary

| Work package | Josh | Sim | Total |
| --- | ---: | ---: | ---: |
| S1.1 UI contract/layouts | 3h | 3h | 6h |
| S1.2 Navigation shell | 4h | 16h | 20h |
| S1.3 Data/research presentation | 4h | 8h | 12h |
| S1.4 Expedition preview | 3h | 3h | 6h |
| S1.5 Verification/review | 4h | 4h | 8h |
| Integration reserve | 2h | 6h | 8h |
| **Total** | **20h** | **40h** | **60h** |

## Two-week cadence

### Kickoff — start of Day 1

- Confirm actual Josh/Sim capacity.
- Review sprint goal, exclusions, dependencies, and acceptance criteria.
- Resolve blocking UX decisions and assign work-package owners.
- Break each package into reviewable tasks, normally no more than 4–8 hours each.

### Daily async check-in

Each contributor records:

- completed since last update;
- next task;
- blocker or decision needed;
- remaining estimate if it materially changed.

Avoid status meetings when the written update is sufficient. Escalate blockers the same day rather than waiting for a ceremony.

### Mid-sprint review — end of Week 1

- Demonstrate the current Main Menu → Lab flow.
- Compare remaining work with actual remaining hours.
- Re-estimate S1.3–S1.5.
- Cut optional states or visual polish before reducing navigation, currency clarity, or verification.

### Integration checkpoint — middle of Week 2

- Stop starting new committed features.
- Connect completed panels, focus behavior, representative data, and Build Settings.
- Run the Play Mode smoke path and both target resolutions.

### Review and retrospective — end of Week 2

- Run the defined review demonstration.
- Accept or reject the sprint against the exit gate.
- Record what changed, what was cut, actual effort by package, and planning lessons.
- Choose the next relevant work bucket using current evidence rather than automatically pulling every deferred item.

## Cut order if capacity drops

Preserve the coherent shell. Cut in this order:

1. Credits content beyond a basic placeholder.
2. Settings content beyond a basic placeholder.
3. Purchased/newly-available demonstration states if locked/affordable/unaffordable remain clear.
4. Recent-experiment polish beyond a readable summary.
5. Expedition starting options beyond Forest Edge + Hare.

Do not cut deterministic Back behavior, visible focus, scientific-data clarity, target-resolution checks, or the standalone build smoke test.

## Explicitly deferred to later buckets

- Player Simulation and Dev Lab scene separation.
- Real expedition scene handoff.
- Profile saves, migration, and reset.
- Scientific-data earning, spending, banking, and loss.
- Atomic wallet transactions.
- Functional permanent research purchases.
- Functional species mastery.
- Branching run upgrades.
- Final art, audio, animation, and accessibility coverage.
- General navigation, skill-tree, modifier, or UI-component frameworks.

## Risks

| Risk | Response |
| --- | --- |
| Actual Josh/Sim capacity is lower than assumed | Recalculate at kickoff and apply the cut order immediately. |
| Existing `MainMenu.unity` Noesis setup needs repair | Time-box the investigation inside S1.2; preserve the scene and GUIDs rather than rebuilding blindly. |
| Representative currency UI looks functional | Label fixture data, disable mutation, and test purchase preview without changing balances. |
| Biology theme obscures costs or state | Require text/icon/state redundancy; never depend on color or decorative metaphor alone. |
| Navigation work grows into a framework | Use one explicit screen state for the known flow. Generalize only after another real flow proves the need. |
| UI works only at the authoring resolution | Check 1920×1080 and 1280×720 throughout implementation and at integration. |

## Sprint exit gate

Sprint 1 is complete when:

- the review demonstration runs end to end in a Windows development build;
- the Main Menu and all four Lab sections are navigable with keyboard and mouse;
- scientific-data balances, type research, and Hare mastery are understandable and visibly nonfunctional prototypes;
- Expedition Setup clearly represents Forest Edge + Hare without pretending to launch;
- Back behavior, focus, both target resolutions, navigation tests, and the build smoke check pass;
- remaining work is explicitly placed in a later bucket.

## Discussion points before commitment

- Are 20 Josh hours and 40 Sim hours realistic for the two-week window?
- Should Credits and Settings remain placeholders, or should either be cut entirely?
- Which Lab screen deserves the most visual fidelity for the review?
- Is enabling `MainMenu.unity` in Build Settings acceptable once the smoke path passes?
- Who owns final UX decisions when implementation exposes a conflict?

