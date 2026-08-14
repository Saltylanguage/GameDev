# Sprint 1 Plan — Player Shell and Lab Foundation

> Status: Discussion draft | Updated: 2026-08-14 | Default cadence: two weeks | Owners: unassigned

## Sprint goal

Deliver the first end-to-end player shell:

```text
Launch -> Main Menu -> Lab -> Expedition Setup -> Forest Edge simulation
```

A player can reach and run the curated Hare experiment without seeing raw tuning fields. A developer can still open a separate Dev Lab, configure the same scenario and seed, and reproduce the same simulation-domain result.

The Lab uses representative scientific-data and research content during this sprint. Real profiles, wallet transactions, permanent purchases, data settlement, and mastery progression are explicitly later work.

## Sprint review demonstration

The review should show one uninterrupted flow:

1. Launch a Windows development build into Main Menu.
2. Enter the Lab and navigate Overview, Research, Species Archive, and Expedition Setup.
3. Show Research, Plant, Herbivore, and Carnivore Data plus contextual Hare Mastery using clearly labeled representative values.
4. Inspect an affordable and unaffordable Herbivore research project without performing a fake purchase.
5. Select Forest Edge + Hare and launch the player simulation.
6. Start, pause, change speed, inspect essential run information, and reach a placeholder result without exposing authoring controls.
7. Open the Dev Lab separately, run the same scenario and fixed seed, and confirm the simulation result matches the player path.

## Committed scope

### S1.1 — Finalize the UI and scene contract

**Size:** Small | **Depends on:** current Noesis and scene audit

Define before implementation:

- screen states and Back behavior;
- representative Lab data and every required visual state;
- scene ownership for Main Menu/Lab, Player Simulation, and Dev Lab;
- immutable expedition request fields;
- which existing `CellularAutomataPrototype` objects move, remain, or are composed differently.

Acceptance:

- Main Menu, Lab, Player Simulation, and Dev Lab each have one written responsibility.
- Every primary action has a destination, invalid state, and expected feedback.
- No implementation task depends on an unresolved navigation or scene-ownership decision.

### S1.2 — Main Menu and Lab Noesis shell

**Size:** Large | **Depends on:** S1.1

Use the existing `MainMenu.unity` and established Noesis/XAML/ViewModel conventions to implement:

- Main Menu with Enter Lab, New Profile placeholder, Settings, Credits, and Quit;
- Lab navigation for Overview, Research, Species Archive, and Expedition Setup;
- deterministic Back and overlay behavior;
- visible keyboard focus and mouse operation;
- layouts functional at 1920×1080 and 1280×720.

Acceptance:

- All planned panels are reachable and return correctly.
- Prototype-only actions are disabled or clearly labeled.
- Navigation contains no simulation or persistence logic.
- `MainMenu.unity` becomes the first enabled Build Settings scene only after it can launch without trapping development workflows.

### S1.3 — Scientific-data and research presentation

**Size:** Medium | **Depends on:** S1.2

Create one representative, presentation-only data fixture covering:

- Research, Plant, Herbivore, and Carnivore Data balances;
- contextual Hare Mastery;
- a recent experiment-return summary;
- affordable, unaffordable, locked, selected, purchased, and newly available research-node states;
- one prerequisite chain and one Hare mastery objective;
- purchase preview without balance mutation.

Acceptance:

- Data type, cost, prerequisite, and benefit remain readable without color alone.
- Type research and species mastery cannot be mistaken for the same progression.
- Biology/experiment terminology is consistent across the Lab.
- No representative balance is written to disk or presented as a real purchase.

### S1.4 — Player and Dev Lab separation

**Size:** Large | **Depends on:** S1.1

Create two compositions over the same simulation-domain APIs:

- **Player Simulation:** scenario context, board, start/pause/speed, essential inspection, upgrade/result placeholders, and no raw tuning fields.
- **Dev Lab:** scenario/species selection, seed, global and species tuning controls, runtime controls, metrics, and diagnostics.

Reuse current assets and controls rather than rewriting the simulation preview. Preserve serialized references and `.meta` files through any scene migration.

Acceptance:

- Raw authoring values are absent from the player path and remain available in Dev Lab.
- Both paths create immutable run-start data through the existing scenario/domain boundary.
- The same Forest Edge scenario and fixed seed produce the same ruleset fingerprint and final domain result through both compositions.
- Existing cellular simulation tests remain green.

### S1.5 — Expedition handoff

**Size:** Medium | **Depends on:** S1.2 and S1.4

Define and connect the smallest explicit launch request containing:

- scenario ID;
- player species ID;
- seed policy/value;
- selected starting options that are already supported.

Acceptance:

- Expedition Setup visibly summarizes Forest Edge + Hare before launch.
- Invalid or unavailable selections cannot launch.
- The player scene receives a complete immutable request without reading Lab ViewModels or XAML state.
- Returning from a placeholder result has a defined route; real reward banking is not implemented.

### S1.6 — Verification and build smoke pass

**Size:** Medium | **Depends on:** S1.2–S1.5

Add the smallest useful checks:

- ViewModel/navigation tests for screen and Back transitions;
- Play Mode smoke coverage for Main Menu → Lab → Expedition Setup;
- fixed-seed parity check for Player Simulation versus Dev Lab;
- standalone Windows development-build launch and navigation check;
- `git diff --check` plus existing runtime/Edit Mode suites.

Acceptance:

- All committed acceptance checks pass with evidence recorded in a handoff.
- No Play Mode errors occur during the sprint-review flow.
- Failures are fixed or explicitly carried forward; the sprint is not closed around a broken application entry path.

## Stretch scope

Only begin these after the committed review flow works:

- one cohesive biology-themed visual spike applied across Main Menu, Lab, and player HUD;
- temporary menu/Lab ambience and basic selection/confirmation audio;
- polished transition animation between Lab sections;
- richer empty-state illustrations or research-project iconography;
- returning from placeholder Results directly to the same Lab session.

Stretch work must not introduce final-volume assets or delay verification.

## Explicitly out of scope

- Real scientific-data earning, spending, banking, or loss.
- Profile save files, migrations, multiple slots, or Steam Cloud.
- Permanent research purchases or respec behavior.
- Functional species mastery progression.
- Final upgrade trees or branching run-upgrade implementation.
- Complete Settings functionality.
- Active-run save/resume.
- General navigation, skill-tree, modifier, or UI-component frameworks.
- Final art, audio, animation, localization, or accessibility coverage.

## Suggested sequence

This ordering is capacity guidance, not a calendar promise.

### Opening

- Complete S1.1.
- Produce low-fidelity layouts and the representative-data table.
- Prove the existing `MainMenu.unity` Noesis view can host the shell.

### Middle

- Build S1.2 and S1.3 together around the same explicit screen-state contract.
- Begin S1.4 once scene ownership is agreed; preserve the current prototype as the reference until parity is demonstrated.
- Start focused navigation tests as soon as the first transitions exist.

### Closing

- Connect S1.5 only after both ends of the handoff are stable.
- Complete S1.6, fix review-flow blockers, and run the standalone build.
- Add stretch presentation work only if the exit gate already appears achievable.

## Dependencies and preconditions

- The existing `MainMenu.unity` Noesis view must be repairable without replacing the presentation stack.
- Forest Edge and Hare remain the vertical-slice selection.
- `CellularSimData`/scenario conversion remains the run-start source of truth.
- Existing user work and Unity recovery files in the working tree must not be overwritten or swept into unrelated commits.
- Scene moves or duplication require preserved GUIDs and a deliberate Unity Editor migration.

## Risks and responses

| Risk | Impact | Response |
| --- | --- | --- |
| Scene separation breaks serialized references | High | Preserve the prototype as reference, migrate in Unity, and validate `.meta`/GUID integrity before cleanup. |
| Main Menu shell grows into a navigation framework | Medium | Use one explicit screen-state enum and commands for the known flow. Generalize only after a second real flow proves the need. |
| Representative currency UI is mistaken for functionality | Medium | Mark fixture values, disable mutation, and demonstrate previews without purchases. |
| UI shell and simulation split compete for capacity | High | Protect the review path; cut stretch art/audio first, then defer nonessential Lab states without weakening scene separation. |
| Player and Dev Lab produce different runs | High | Compare fixed seed, scenario, fingerprint, and final result before accepting either composition. |
| Noesis layout fails at target resolutions | Medium | Test 1920×1080 and 1280×720 during implementation, not only at sprint close. |

## Sprint exit gate

Sprint 1 is complete when:

- the sprint-review demonstration runs end to end in a Windows development build;
- the player simulation exposes no raw tuning controls;
- Dev Lab retains the necessary authoring and diagnostics;
- fixed-seed domain results match between player and developer paths;
- the Lab clearly demonstrates scientific-data balances, permanent-research structure, and species mastery without claiming those systems are functional;
- navigation, parity, and relevant existing tests pass;
- remaining work is explicitly reassigned to a later epic or sprint.

If the full shell cannot pass, the preferred partial completion is Main Menu → Lab → Expedition Setup plus a verified scene contract. Do not enable a broken Main Menu as the application entry point merely to claim more scope.

## Discussion points before commitment

- Is two weeks still the intended Sprint 1 duration, and what developer capacity is available?
- Should Sprint 1 commit to launching the actual simulation, or stop at a validated Expedition Setup handoff prototype?
- Does the Player Simulation need a separate scene immediately, or can the first separation use two explicit compositions while scene migration follows?
- Which Lab panels need polished layouts versus credible wireframes for this sprint review?
- Is temporary art/audio part of the commitment or firmly stretch scope?

