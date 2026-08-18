# Sprint 0 Closeout Plan — Make Sprint 1 Ready

> Status: C1–C3 executed; C4 remaining | Dates: August 14–16, 2026 | Team: Josh + Sim

## Closeout goal

Finish the planning sprint with Sprint 1 ready to implement. Do not start broad feature work during the closeout window.

Sprint 0 ends with:

- an agreed Main Menu → Lab Overview → Research-preview contract;
- low-fidelity layouts and representative data;
- a known-safe technical path through the existing `MainMenu.unity` scene;
- a cleanly ordered Sprint 1 task list with owners and acceptance checks;
- an explicit plan for the current mixed working tree and branch divergence.

## Remaining capacity assumption

Because only the remainder of the week remains and substantial Sprint 0 planning is already complete, cap closeout work at approximately **4 hours each**:

| Contributor | Remaining effort |
| --- | ---: |
| Josh | 4 hours |
| Sim | 4 hours |
| **Total** | **8 hours** |

If less time is available, preserve the UX contract and project-state inventory; defer the technical spike to the beginning of Sprint 1.

## Work packages

### S0.C1 — Project-state and planning consolidation

**Effort:** 2 hours — Josh 1, Sim 1

- Inventory the current `ProjectMain` working tree without deleting or overwriting existing work.
- Identify which changes belong together and which contributor owns unresolved batches.
- Record that local `ProjectMain` is currently behind its remote before any integration action.
- Reconcile duplicate, moved, or overlapping planning documents.
- Confirm which document is authoritative for Sprint 1.

Acceptance:

- No unknown file is swept into a commit or discarded.
- Sprint 1 can begin from an explicitly chosen Git state.
- `SPRINT_1_PLAN.md` is the authoritative execution plan.

### S0.C2 — UX contract and low-fidelity layouts

**Effort:** 3 hours — Josh 2, Sim 1

Decide and sketch:

- Main Menu, Lab Overview, and Herbivore Research preview;
- forward, Back, overlay, and keyboard-focus behavior;
- 1920×1080 and 1280×720 layouts;
- the global scientific-data bar;
- one available and one locked/unaffordable Herbivore project;
- consistent experiment, data, research, and mastery terminology.

Acceptance:

- Every Sprint 1 action has a destination and invalid state.
- Currency type, cost, prerequisite, and benefit are readable without color alone.
- No blocking UX decision is delegated to implementation by accident.

### S0.C3 — Main Menu technical readiness spike

**Effort:** 2 hours — Josh 0.5, Sim 1.5

- Inspect the existing `MainMenu.unity` Noesis view and its XAML/host references.
- Identify the smallest files and scene changes needed for the Sprint 1 shell.
- Decide how the explicit screen state and representative-data fixture enter the ViewModel.
- Identify the Build Settings promotion and smoke-test steps.
- Do not create a general navigation or UI-component framework.

Acceptance:

- The scene is confirmed reusable or a specific repair is documented.
- Sprint 1 tasks name their likely files, risks, and verification path.
- Scene/GUID preservation requirements are understood.

### S0.C4 — Sprint 1 kickoff preparation

**Effort:** 1 hour — Josh 0.5, Sim 0.5

- Confirm the August 17–23 sprint window and 10-hour availability each.
- Split committed work into tasks no larger than roughly 2–4 hours.
- Assign an owner and reviewer to each task.
- Agree on the midweek checkpoint and final review time.

Acceptance:

- No Sprint 1 task lacks an owner, dependency, or acceptance check.
- Total committed plus reserve effort does not exceed 20 hours.

## Suggested remainder-of-week flow

### Friday, August 14

- Complete S0.C1.
- Begin the screen contract and decide the first Herbivore research concept.

### Saturday, August 15

- Finish S0.C2.
- Run the time-boxed `MainMenu.unity` readiness spike.

### Sunday, August 16

- Resolve only blocking findings from the spike.
- Complete S0.C4 and hold a short Sprint 0 review.
- Do not pull extra implementation into the sprint because planning finished early.

## Closeout review

Demonstrate or review:

- the three low-fidelity screens;
- representative balances and research-project states;
- the technical readiness conclusion;
- the safe Git/integration plan;
- the dated Sprint 1 work packages and owners.

## Sprint 0 exit gate

Sprint 0 is complete when Sprint 1 can start without reopening product scope, guessing at the UI flow, or risking unrelated working-tree changes. Any unresolved blocker must be named and assigned; it cannot remain implied in a planning document.

