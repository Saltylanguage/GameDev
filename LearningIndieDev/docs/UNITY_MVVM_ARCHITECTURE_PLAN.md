# Unity MVVM Architecture Plan

> Status: Accepted direction; implementation staged and not yet started
> Date: 2026-08-25
> Scope: player-facing Main Menu, GalapagOS Lab, and simulation UI

This plan adapts MVVM to Unity and Noesis without moving simulation rules into
the UI layer.

The durable review rules for this architecture are recorded in [SG-003 — UI
Architecture and MVVM Boundaries](Studio%20Guidelines/SG-003-UI-MVVM-ARCHITECTURE.md).
The T0 state, snapshot, command, handoff, and composition contracts are recorded
in [Unity MVVM UI Contracts](UNITY_MVVM_UI_CONTRACTS.md).

## Layer contract

```text
XAML View (V_Panel_*)
    -> Unity ViewModel (VM_*)
        -> Unity helper/controller (Helper_*)
            -> plain C# domain model and services
```

### Views

XAML files represent the visual view. They contain layout, bindings, styles,
visual states, transitions, and commands. They do not own simulation rules,
profile persistence, or business decisions.

### ViewModels

Unity `MonoBehaviour`s represent the ViewModels. They expose binding fields,
display-ready projections, and commands available to the player. A ViewModel
may own view state and input formatting, but it does not advance the simulation
or implement domain rules.

### Helpers/controllers

Unity `MonoBehaviour`s prefixed `Helper_` own significant Unity-facing
orchestration and business workflows that should remain separate from a
ViewModel. Helpers do not reference XAML, Noesis controls, or ViewModels.

The simulation manager is one of these helpers. It owns run lifecycle and
passes immutable/read-only presentation snapshots to the relevant ViewModel.

### Domain

Plain C# simulation, progression, scenario, results, and persistence contracts
remain UI-agnostic. Existing domain types such as `CellularSimData`,
`SpeciesRules`, `SpeciesSimulationRunner`, and `SpeciesProgression` remain in
this layer.

## Naming convention

The naming convention applies to both the Unity component class and its source
file where practical:

| Responsibility | Convention | Example |
| --- | --- | --- |
| ViewModel | `VM_<Feature>.cs` and class `VM_<Feature>` | `VM_Settings.cs` |
| XAML view/panel | `V_Panel_<Feature>.xaml` | `V_Panel_Settings.xaml` |
| Unity helper/controller | `Helper_<Feature>.cs` and class `Helper_<Feature>` | `Helper_Settings.cs` |

The naming migration is staged. Rename assets only when the owning feature is
already being changed, preserve Unity `.meta` files and serialized references,
and verify scene/prefab loading after each batch. No broad rename pass should
precede a working composition path.

Existing names such as `MainMenuViewModel`, `SpeciesSimulationViewModel`,
`SpeciesSimulationPreview`, and `MainMenuShell.xaml` remain compatibility
names until their feature is migrated safely.

## Scene and feature structure

### Main Menu scene

The Main Menu is the application entry point and has one primary ViewModel and
one primary XAML view:

```text
Main Menu scene
  VM_MainMenu
    V_Panel_MainMenu.xaml
      Profile Selection
      Continue
      Quit
```

Profile selection can be represented as a visual state or overlay within the
same view. It is also the first-launch path for creating or selecting the first
available profile; there is no separate New Game or Load Game button. Continue
loads the Lab with the last loaded profile and is unavailable until a profile
has been loaded.

Settings and Credits are not Main Menu actions in this contract. They may be
added to the Lab later if they become required.

### GalapagOS Lab scene

Continue transitions to the GalapagOS Lab scene with the last loaded profile.
The Lab is the between-run operating-system-style home base and owns permanent
progression, collectibles, profile information, and expedition preparation.

```text
GalapagOS Lab scene
  VM_Lab
    V_Panel_Lab.xaml          # GalapagOS desktop/window shell
      VM_Overview             V_Panel_Overview.xaml
      VM_Research             V_Panel_Research.xaml
      VM_SpeciesArchive       V_Panel_SpeciesArchive.xaml
      VM_ExpeditionSetup      V_Panel_ExpeditionSetup.xaml
      VM_Settings             V_Panel_Settings.xaml
```

Each desktop icon or feature window gets its own ViewModel/View pair. The
GalapagOS shell remains its own pair and is responsible for opening, closing,
focusing, and transitioning feature panels. Each feature ViewModel calls its
corresponding helper for business operations such as profile reads, research
purchase validation, collectible state, or expedition launch requests.

The Lab ViewModels must not directly mutate the simulation or persistence
domain. They submit explicit commands and receive updated snapshots/results.

### Simulation scene

The simulation remains a separate scene and domain boundary.

```text
Simulation scene
  Helper_SimulationManager
    VM_SimulationShell
      V_Panel_SimulationShell.xaml
        Welcome state
        Play state
        Reward selection state
        Failure state
        Final report state

    VM_SimulationBoard
      V_Panel_SimulationBoard.xaml (if a XAML host is needed)
      SpeciesSimulationBoard (custom batched renderer)
```

The manager advances the simulation and publishes the current run state and
board snapshot. The shell ViewModel exposes the player-facing controls and
state-specific bindings. The board ViewModel exposes only the current board
projection, selection/inspection state, and presentation data needed by the
custom board renderer.

## Simulation flow state

The authoritative flow state belongs to the simulation manager/application
layer, not to XAML alone. The manager exposes a small state such as:

```text
Welcome -> Play -> Rewards
                 -> Failure
Rewards -> Play
Play -> FinalReport
Failure -> FinalReport
FinalReport -> GalapagOS Lab
```

Noesis `VisualStateManager` maps that state to visual presentation and handles
transitions, animation, visibility, and polish. It should not be the only place
that knows whether a run is complete, failed, or ready for rewards; that would
make business flow inaccessible to tests, reports, and scene transitions.

Each state has an explicit entry/exit contract. Scene transitions are invoked
by a helper/controller and preserve the profile/session handoff through an
explicit launch or return request.

## Migration order

1. **Lock the contracts:** define state enums, snapshot shapes, command names,
   and scene handoff requests without changing simulation behavior.
2. **Name new work correctly:** use `VM_*`, `V_Panel_*`, and `Helper_*` for all
   new files; do not rename unrelated legacy assets yet.
3. **Extract simulation ownership:** treat the current
   `SpeciesSimulationPreview` as the first simulation-manager seam, then move
   lifecycle/orchestration behind the helper contract.
4. **Split shell and board presentation:** keep shell controls/rewards separate
   from the board-only ViewModel and snapshot projection.
5. **Make composition explicit:** wire scene references through the host or
   serialized fields; remove `FindAnyObjectByType` and runtime component
   creation from the normal path when the replacement is verified.
6. **Migrate Main Menu and Lab:** rename and split features as they are touched,
   preserve the existing player-flow contract, and add visual-state transitions.
7. **Validate each seam:** run focused domain tests, ViewModel/host Play Mode
   tests, scene-load checks, and the Windows smoke path after each scene batch.

## Implementation task breakdown

The full migration is executable with the current decisions and project
context, but it should not be attempted as one opaque refactor. Each task below
is a reviewable slice with a working state at its exit.

### T0 — Freeze contracts and composition map

Define the simulation flow states, shell/board snapshot shapes, command names,
profile/session handoff, and serialized composition references. This task is
contract-only unless a type is needed to make the contract testable.

**Exit gate:** the contracts identify one owner for each state and describe the
inputs, outputs, and invalid states for Main Menu, Lab, and Simulation.

### T1 — Establish the simulation-manager seam

Introduce the smallest `Helper_SimulationManager` seam around the current
simulation lifecycle. Move or wrap tick advancement, run state, and completion
notifications without changing simulation behavior. Keep the existing preview
as a compatibility surface until the new seam is verified.

**Exit gate:** the manager is the only tick owner, existing runs still behave
the same, and a seeded run produces the same result with presentation attached
or detached.

### T2 — Split shell and board presentation

Create the shell and board ViewModel boundaries. The shell exposes controls,
rewards, results, and flow state. The board ViewModel exposes only the current
board projection, inspection/selection state, and renderer inputs. The custom
board control remains the high-throughput View.

**Exit gate:** the board draws from a snapshot, shell commands do not mutate
board internals, and no XAML control is created per simulation cell.

### T3 — Make Noesis composition explicit

Replace normal-path `Find*` discovery and runtime `AddComponent` setup with
serialized host references and explicit data-context wiring. Keep the host
responsible for composition, not simulation or feature behavior.

**Exit gate:** the target scene opens from its serialized composition alone,
the Noesis binding smoke test passes, and no compatibility reference was lost.

### T4 — Migrate the Main Menu contract

Migrate the Main Menu to the `VM_MainMenu` / `V_Panel_MainMenu` convention when
the shell is touched. Implement only Profile Selection, Continue, and Quit.
Profile Selection handles the first profile create/select path; Continue is
disabled until a profile exists and then loads the Lab with the last loaded
profile.

**Exit gate:** first-launch and returning-profile flows work in Play Mode, Quit
is wired, and no New Game or Load Game action remains in the player menu.

### T5 — Build the GalapagOS Lab shell and feature pairs

Create the separate Lab scene with one top-level Noesis root. Add the meaningful
feature pairs for Overview, Research, Species Archive, Expedition Setup, and
Settings. Feature windows may use separate XAML roots or panels beneath the
Lab root. Use clearly labelled representative data until services are ready.

**Exit gate:** keyboard/mouse navigation, focus, Back behavior, disabled
prototype actions, and the Lab UI-only acceptance checklist work without
pretending that data was saved or spent.

### T6 — Connect the end-to-end scene flow

Use explicit `Single` scene transitions and the profile/session handoff for
Main Menu → Lab → Expedition Setup → Simulation → Results → Lab. Pass an
immutable run-start snapshot into the simulation and return a result snapshot
without relying on destroyed scene objects.

**Exit gate:** the full navigation loop works, leaving a run is intentional,
and the same seed/profile/options reproduce the same simulation result.

### T7 — Safe naming migration and cleanup

Rename touched legacy files and types to `VM_*`, `V_Panel_*`, and `Helper_*`
only after their replacement path works. Preserve `.meta` files, Unity GUIDs,
serialized references, and compatibility shims until scene validation passes.
Remove obsolete wiring only after the new path has a focused test or smoke
check.

**Exit gate:** no untracked compatibility dependency remains unexplained,
Unity scenes load cleanly, focused tests pass, and the documentation reflects
the implemented boundaries.

Shared scene edits, serialized asset changes, and broad renames remain separate
review points; they should not be bundled into an otherwise unrelated task.

## Guardrails

- Do not put simulation rules, persistence mutation, or reward calculations in
  XAML or ViewModels.
- Do not make the VisualStateManager the authoritative gameplay state machine.
- Do not create a generic navigation framework before the Main Menu/Lab flow
  demonstrates a concrete need.
- Do not create one ViewModel per decorative icon; create one per meaningful
  feature/window with its own state or business interaction.
- Do not keep duplicate authoritative drafts in both a ViewModel and helper.
- Keep the custom board renderer optimized for a snapshot, not one XAML control
  per simulation cell.

## Decisions recorded before implementation

1. **Continue** loads the GalapagOS Lab with the last loaded profile. It is
   unavailable on first launch when no profile has been loaded.
2. **Load Game** and **New Game** are not Main Menu actions. Profile Selection
   is the entry point for creating or selecting the profile used by Continue.
3. Main Menu, Lab, and Simulation use separate scenes loaded in `Single` mode.
   Profile/session state crosses scene boundaries through an explicit
   application/session owner or launch snapshot; scenes do not remain loaded
   merely to keep references alive.
4. The Lab has one top-level Noesis root/host. Feature windows may use separate
   XAML roots or panels beneath that host; this is an implementation detail and
   does not change feature ownership or the one-ViewModel/one-View contract.
