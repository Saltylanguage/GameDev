# Main Menu and Lab Delivery Plan

> Status: Approved planning baseline | Updated: 2026-08-13 | Target: vertical slice

## Outcome

Build the player-facing shell around the cellular-automata runs:

```text
Launch -> Main Menu -> Lab -> Expedition Setup -> Simulation
                    ^                              |
                    |------ Results / Banking -----|
```

The first delivery may be entirely UI, but it must establish the screen flow and presentation boundaries that later receive profile saves, scientific data, permanent research, species mastery, and branching run upgrades.

## Product responsibilities

### Main Menu

The Main Menu is the application entry point. Its vertical-slice scope is:

- **Enter the Lab:** continue the current research profile.
- **New Profile:** create/reset a profile only after a confirmation step.
- **Settings:** display, audio, input, and accessibility entry point; implementation may begin as a placeholder panel.
- **Credits:** simple static panel.
- **Quit:** desktop only.

Do not add multiple save slots, cloud-save conflict UI, account systems, news panels, stores, or online features for the slice.

### The Lab

The Lab is the home base and primary between-run screen. Its information architecture is:

- **Overview:** current research totals, recent discoveries, active unlocks, and the next useful objective.
- **Research:** permanent Plant, Herbivore, and Carnivore skill trees.
- **Species Archive:** discovered species, mastery status, behaviors, and species-specific unlocks.
- **Expedition Setup:** scenario, player species, starting options, and launch command.
- **Settings / Main Menu:** secondary navigation.

For the UI-only milestone, these surfaces use clearly labeled representative data. Buttons that cannot yet perform a real operation must be visibly disabled or identified as prototype actions; they must not pretend to save or spend currency.

### Lab economy and biology theme

Scientific data is a primary Lab interaction, not a secondary status display.
The Lab must make the relationship between experiments, collected data, and
permanent research understandable from every spending surface.

- A persistent **data bar** shows Research, Plant, Herbivore, and Carnivore Data.
- Species Mastery Data appears when a particular species is selected rather
  than crowding the global bar with every species balance.
- Research nodes show their data type, full cost, prerequisites, unlocked
  run content, and current affordable/unaffordable state before selection.
- A purchase preview explains which balances will be consumed and what becomes
  available. Permanent purchases require confirmation and show updated balances
  immediately after success.
- The Overview summarizes data returned from the last experiment and recommends
  a useful next research goal without choosing it for the player.
- The Species Archive explains how to earn the selected species' mastery data
  and which mastery benefits or content it can unlock.
- Expedition Setup shows which unlocked research options will be available in
  the run, but permanent research is purchased only through the Lab.

The presentation language is an experimental biology workspace: simulations are
**experiments**, currency is **data**, permanent nodes are **research projects**,
and species progression represents **observation and mastery**. Visual motifs may
use specimen cards, microscopes, petri dishes, branching phylogenetic diagrams,
field notes, and analyzed samples. Theme must reinforce function: data types,
costs, prerequisites, and purchase results remain readable without relying on
color or decorative metaphor alone.

## Screen-flow contract

| From | Player action | To | Required state/feedback |
| --- | --- | --- | --- |
| Launch | Start application | Main Menu | Focus lands on the primary action. |
| Main Menu | Enter Lab | Lab Overview | Current profile summary is visible. |
| Main Menu | Open Settings/Credits | Overlay/panel | Back returns focus to the invoking control. |
| Lab | Select Research | Research Trees | Currency totals and locked/unlocked states remain visible. |
| Research Trees | Select project | Purchase Preview | Cost, prerequisites, benefit, and remaining balances are shown. |
| Purchase Preview | Confirm research | Research Trees | Data is deducted once; the unlocked node and newly available paths are revealed. |
| Lab | Select Species Archive | Species Archive | Type and species mastery are clearly distinguished. |
| Lab | Prepare Expedition | Expedition Setup | Scenario, species, and starting choices are summarized before launch. |
| Expedition Setup | Launch | Player Simulation | Selected IDs and profile-derived options form an explicit launch request. |
| Simulation | Finish/Extinction | Results | Earned, spent, banked, and lost data are explained. |
| Results | Return to Lab | Lab Overview | Banked rewards and new unlocks are visible. |

Back behavior is deterministic: overlays close first, Lab sub-pages return to Lab Overview, and Lab returns to Main Menu only through an explicit command. Leaving a run requires confirmation.

## Presentation and technical boundaries

- Reuse the existing `MainMenu.unity` scene and Noesis/XAML/ViewModel conventions.
- Main Menu and Lab can share one scene and one root UI host. Use explicit screen state; do not introduce a general navigation framework for this flow.
- The cellular simulation remains a separate scene and domain boundary.
- UI ViewModels expose presentation-ready values and explicit commands. XAML does not read simulation assets, `PlayerPrefs`, or mutable domain state directly.
- A later profile service owns versioned meta-progression. The UI-only milestone may supply representative data through a small composition fixture, not through fake persisted state.
- Expedition launch data contains stable scenario/species IDs and selected persistent options. The simulation receives an immutable run-start snapshot.
- Scientific data, research unlocks, mastery, and completed-run settlement are domain concepts independent of Noesis.
- The existing Dev Lab remains a developer/authoring surface and is not the player-facing Lab home base.

## UI-only acceptance criteria

- The application can enter Main Menu and navigate to every planned Lab section without entering Play Mode errors.
- Keyboard and mouse can complete the full navigation loop; focus state is always visible.
- Layout is readable at 1920×1080 and functional at 1280×720.
- Main Menu, Lab navigation, overlays, and Back behavior follow the screen-flow contract.
- Research trees distinguish Plant, Herbivore, and Carnivore progression.
- The global data bar and contextual species-mastery balance are represented in
  the correct Lab surfaces.
- Research-node prototypes demonstrate locked, available, affordable,
  unaffordable, selected, purchased, and newly-unlocked states.
- A representative purchase preview identifies every currency cost and resulting
  unlock without changing fake balances.
- Species Archive distinguishes type research from species mastery.
- Expedition Setup demonstrates Forest Edge + Hare and summarizes the selected run.
- Placeholder balances, nodes, and mastery values are visibly marked as representative UI data.
- The UI has empty, locked, affordable, unaffordable, selected, confirmation, and error visual states even where real services are not wired yet.
- A Play Mode smoke test covers Main Menu → Lab → Expedition Setup navigation at the ViewModel/host boundary.

## Delivery epics

### E0 — Product and UX contract

**Goal:** agree on the player-facing shell before implementation spreads across scenes and systems.

Deliverables:

- screen-flow contract and low-fidelity layouts;
- Main Menu and Lab terminology;
- representative data set for every required UI state;
- explicit split between player Lab and Dev Lab.

Exit gate: every primary action has a destination, required data, failure state, and Back behavior.

### E1 — Main Menu and Lab UI shell

**Depends on:** E0.

Deliverables:

- `MainMenu.unity` as the enabled application entry scene;
- one Noesis root shell with Main Menu, Lab Overview, Research, Species Archive, and Expedition Setup panels;
- persistent scientific-data presentation plus representative research costs,
  purchase previews, experiment returns, and species-mastery guidance;
- keyboard/mouse focus, Back behavior, confirmations, responsive layout, and representative data;
- navigation smoke coverage.

Exit gate: the complete UI-only acceptance checklist passes in a standalone Windows development build.

### E2 — Profile and settings foundation

**Depends on:** E1.

Deliverables:

- one versioned local profile containing unlock/progression state;
- separate versioned settings data;
- new/load/reset flows with corrupt-save fallback and migration coverage;
- UI reads profile snapshots and invokes explicit commands.

Exit gate: restart preserves settings and one test unlock; reset requires confirmation; invalid data fails safely.

### E3 — Scientific data wallet and run settlement

**Depends on:** E2 and trustworthy run telemetry.

Deliverables:

- Research, Plant, Herbivore, Carnivore, and current slice-species mastery balances only where each has a proven use;
- one atomic wallet transaction for permanent research purchases, with explicit
  insufficient-data and invalid-prerequisite results;
- capped/diminishing award rules based on completed simulation evidence;
- run ledger separating earned, spent, banked, and lost data;
- deterministic settlement and save tests.

Exit gate: the results screen can explain every balance change and replaying the same recorded run produces the same settlement.

### E4 — Permanent type research

**Depends on:** E3 and approved first node catalog.

Deliverables:

- first small Herbivore research tree for the Hare slice;
- locked, available, affordable, purchased, and prerequisite states;
- biology-themed project descriptions, cost breakdowns, confirmation, and
  purchase-result feedback using the real scientific-data wallet;
- permanent purchases that primarily unlock run choices or information;
- Plant/Carnivore tabs may remain content-light until their first playable species needs them.

Exit gate: one Lab purchase survives restart and changes an eligible choice in the next run without silently mutating base simulation data.

### E5 — Expedition contract and branching run upgrades

**Depends on:** E1, the player/Dev Lab split, and the first trustworthy upgrade catalog.

Deliverables:

- immutable launch request from Lab to the selected scenario;
- explicit Trailblazer, Warren, and Gardeners branches;
- branch prerequisites, exclusions, previews, costs, and ordered loadout recording;
- offer logic that cannot strand a committed branch.

Exit gate: three seeded Hare runs demonstrate visibly distinct builds and reproduce from scenario, seed, base fingerprint, and ordered upgrades.

### E6 — Mastery and complete home-base loop

**Depends on:** E3–E5.

Deliverables:

- Hare mastery objectives based on varied behaviors;
- Species Archive discovery/mastery presentation;
- result-to-Lab reveal of data, mastery, research unlocks, and next objective;
- one complete new-profile → run → reward → Lab purchase → changed next-run loop.

Exit gate: an external player can explain how to earn mastery, how type research differs, and why they would try another species.

## Dependency order

```text
E0 UX contract
  -> E1 UI shell
       -> E2 profile/settings
            -> E3 data wallet/settlement
                 -> E4 permanent research
                 -> E6 mastery/home-base loop
       -> E5 expedition + run branches
            -> E6 mastery/home-base loop
```

Art and audio exploration may accompany E1, but final-volume production waits for the UI shell and comprehension tests. Dev Lab separation and trustworthy upgrades remain prerequisites for connecting E5 to the simulation.

## Planning and delivery workflow

Each epic follows the same lightweight workflow:

1. **Ready:** outcome, non-goals, dependencies, user flow, data contract, risks, and acceptance checks are written.
2. **Slice:** identify the thinnest end-to-end demonstration and split it into reviewable tasks, normally no larger than a few focused days.
3. **Implement:** keep domain, composition, and presentation responsibilities explicit; preserve deterministic run data.
4. **Verify:** run focused domain tests, UI/ViewModel tests, Play Mode navigation checks, and a standalone-build smoke test proportional to the change.
5. **Review:** demonstrate the player outcome, record usability/playtest evidence, and compare it with the exit gate.
6. **Close:** update durable decisions, add a concise handoff, move incomplete work forward explicitly, and do not declare the epic complete with unresolved acceptance failures.

### Definition of ready for an implementation task

- One player or developer outcome.
- Named files/systems or an identified investigation task.
- Inputs, outputs, and invalid states.
- Determinism, persistence, and migration impact identified.
- Verification method and acceptance result.
- Dependencies resolved or explicitly blocked.

### Definition of done

- Acceptance checks pass and evidence is recorded.
- No placeholder is presented as functional behavior.
- Relevant tests/build smoke checks pass.
- Persistent formats are versioned and migration behavior is covered.
- Product and technical documents reflect any changed decision.
- Deferred work has a trigger and does not hide inside comments or speculative abstractions.

## Immediate planning batch

Before E1 implementation begins:

1. Produce low-fidelity layouts for Main Menu, Lab Overview, Research, Species Archive, and Expedition Setup.
2. Define the representative UI data fixture and required visual states.
   Include all currency balances, one experiment-return summary, affordable and
   unaffordable projects, a prerequisite chain, and one Hare mastery objective.
3. Decide whether the existing `MainMenu.unity` Noesis view can host the root shell without scene repair.
4. Write the small screen-state/ViewModel contract and scene transition contract.
5. Add Main Menu to Build Settings only when the shell can launch without trapping normal simulation development.

## Decisions intentionally deferred

- Final Lab art direction and environmental presentation beyond UI.
- Multiple profiles/save slots and Steam Cloud conflict handling.
- Exact data award rates, extinction loss, and permanent node costs.
- Full Plant and Carnivore permanent trees.
- Respec rules and late-game research-tree shape.
- Active-run save/resume.
- A generic navigation framework, skill-tree editor, or upgrade scripting system.
