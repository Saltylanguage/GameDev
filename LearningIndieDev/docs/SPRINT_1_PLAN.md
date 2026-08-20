# Sprint 1 Plan — Main Menu and Lab Foundation

> Status: Active; authoritative execution plan | Dates: August 17–30, 2026 | Cadence: two weeks | Team: Josh + Sim

This document is the authoritative execution plan for Sprint 1. Product-level
direction remains in `PROJECT_CONTEXT.md` and `ROADMAP.md`; the Sprint 0
closeout document remains the source for Sprint 1 preconditions and exit-gate
decisions.

## Sprint goal

Deliver the smallest credible application shell:

```text
Launch -> Main Menu -> Lab Overview -> Research preview
```

The Lab introduces the experimental-biology theme and scientific-data economy using representative UI data. It does not yet include the complete Species Archive, Expedition Setup, real currency, persistence, purchases, mastery progression, or simulation launching.

## Capacity

| Contributor | Capacity | Primary responsibility |
| --- | ---: | --- |
| Josh | 20 hours | Product decisions, UX review, Unity scene integration, playtesting, acceptance. |
| Sim | 20 hours | Noesis/XAML/ViewModel implementation, representative data, navigation, tests. |
| **Total** | **40 hours** | Two-week planning capacity. The committed S1 scope remains 20 hours including a 3-hour integration/uncertainty reserve; extra capacity is uncommitted. |

Sprint 0 supplies the screen contract, wireframes, representative-data definition, and technical readiness conclusion. If those inputs are incomplete, re-estimate before committing rather than silently consuming the reserve.

## Sprint review demonstration

1. Launch a Windows development build into Main Menu.
2. Select **Enter Lab** and reach Lab Overview.
3. See representative Research, Plant, Herbivore, and Carnivore Data balances.
4. Open the Herbivore Research preview.
5. Inspect one available project and one locked or unaffordable project.
6. See cost, prerequisite, and benefit without performing a fake purchase.
7. Return to Lab Overview and Main Menu with correct Back and focus behavior.

## Preconditions from Sprint 0

- The three-screen flow and target-resolution wireframes are approved.
- The first Herbivore research concept and representative balances are named.
- `MainMenu.unity` is confirmed reusable or has a bounded repair task.
- Current working-tree batches and the target Git starting state are understood.
- Each committed task has an owner, reviewer, and acceptance check.

## Committed work

### S1.1 — Main Menu and Lab shell

**Effort:** 10 hours — Josh 4, Sim 6

Reuse `MainMenu.unity` and existing Noesis conventions to implement:

- Main Menu with Enter Lab and clearly disabled or hidden secondary actions;
- Lab Overview with the persistent scientific-data bar;
- Research-preview navigation;
- one explicit screen state for the known flow;
- deterministic Back behavior, visible keyboard focus, and mouse input;
- functional layout at 1920×1080 and 1280×720.

Acceptance:

- The review route is navigable at both target resolutions.
- XAML and ViewModels contain no wallet, persistence, or simulation logic.
- Existing scene references and GUIDs remain intact.
- `MainMenu.unity` is now the first Build Settings scene; S1 closure still
  requires the Windows smoke path to prove the promotion is safe.

### S1.2 — Herbivore research preview

**Effort:** 4 hours — Josh 2, Sim 2 | **Depends on:** S1.1

Use representative data to show:

- Research, Plant, Herbivore, and Carnivore Data balances;
- one affordable/available Herbivore project;
- one locked or unaffordable project with a prerequisite;
- selected-project cost and benefit;
- a disabled purchase action clearly marked as prototype UI.

Acceptance:

- Data type, cost, prerequisite, and benefit are unambiguous without color alone.
- Representative balances never mutate or persist.
- Experimental-biology terminology matches the Sprint 0 contract.

### S1.3 — Verification and review

**Effort:** 3 hours — Josh 2, Sim 1 | **Depends on:** S1.1–S1.2

- Check Main Menu → Lab → Research → Back transitions.
- Verify keyboard/mouse focus and both target resolutions.
- Run a Play Mode smoke check and Windows development-build launch.
- Run relevant existing tests and `git diff --check`.
- Record acceptance evidence and unresolved work.

### Integration reserve

**Effort:** 3 hours — Josh 2, Sim 1

Reserved for Unity/Noesis scene repair, review changes, coordination, and defects. It is not stretch-feature capacity.

## Effort summary

| Work package | Josh | Sim | Total |
| --- | ---: | ---: | ---: |
| S1.1 Main Menu/Lab shell | 4h | 6h | 10h |
| S1.2 Research preview | 2h | 2h | 4h |
| S1.3 Verification/review | 2h | 1h | 3h |
| Integration reserve | 2h | 1h | 3h |
| **Total** | **10h** | **10h** | **20h** |

## Two-week schedule

### Monday, August 17 — kickoff

- Confirm actual availability and Sprint 0 inputs.
- Assign tasks and begin the Main Menu/Lab shell.

### Tuesday, August 18 – Wednesday, August 19 — shell implementation

- Complete the known navigation states, focus behavior, and target layouts.
- Integrate representative data only through the agreed presentation fixture.

### Thursday, August 20 – Wednesday, August 26 — integration and research preview

- Continue the shell implementation and begin the research-preview states.
- Keep representative data bounded to the agreed presentation fixture.

### Thursday, August 27 — mid-sprint checkpoint

- Demonstrate Main Menu → Lab Overview.
- Compare hours used with remaining work.
- Apply the cut order if the shell is not stable.

### Friday, August 28 – Saturday, August 29 — final integration and verification

- Complete the two project states and selected-project preview.
- Stop starting features before final verification.
- Run Play Mode, target-resolution, and build smoke checks.

### Sunday, August 30 — review and retrospective

- Run the sprint demonstration.
- Record actual hours, accepted work, cuts, defects, and planning lessons.
- Choose the next two-week bucket from evidence.

## Cut order

If 20 hours is insufficient, cut in this order:

1. Main Menu secondary actions beyond Enter Lab.
2. The second research project, retaining one complete project preview.
3. Lab Overview content beyond the data bar and Research entry.
4. Secondary Main Menu content, while retaining the Build Settings smoke gate.

Do not cut deterministic Back behavior, visible focus, currency clarity, target-resolution checks, or basic verification.

## Explicitly deferred

- Full Lab navigation and content.
- Species Archive and contextual mastery UI.
- Expedition Setup and simulation handoff.
- Player/Dev Lab separation.
- Profiles, saves, and migrations.
- Real data earning, spending, banking, or loss.
- Functional research purchases and mastery.
- Branching run upgrades.
- Final art, audio, animation, or UI frameworks.

## Sprint exit gate

Sprint 1 is complete when:

- the review demonstration works in a Windows development build;
- Main Menu, Lab Overview, and Research preview are navigable with keyboard and mouse;
- scientific-data balances and research costs are understandable and clearly representative;
- Back behavior, visible focus, both target resolutions, and the smoke check pass;
- unfinished work is explicitly assigned to a later bucket.

