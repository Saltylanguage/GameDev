# SG-003 — UI Architecture and MVVM Boundaries

**Guideline ID:** SG-003
**Status:** Proposed for review
**Version:** 1.0
**Proposed:** 2026-08-25
**Audience:** Developers, designers, technical artists, and AI agents working on Unity player UI.
**Related documents:** [Unity MVVM Architecture Plan](../UNITY_MVVM_ARCHITECTURE_PLAN.md), [Unity Engineering Standards](../UNITY_ENGINEERING_STANDARDS.md)

## Purpose and scope

This guideline defines the project’s Unity/Noesis UI boundaries. It applies to
the Main Menu, GalapagOS Lab, simulation shell, simulation board, and future
player-facing Noesis/XAML features. It is deliberately adapted to Unity:
ViewModels are `MonoBehaviour`s, helpers are Unity-facing `MonoBehaviour`s, and
simulation/domain rules remain plain C# where Unity lifecycle is not required.

The goal is a UI that can change without moving gameplay rules into XAML or
making a ViewModel the owner of the simulation.

The words **MUST**, **SHOULD**, and **MAY** are normative. A deliberate
exception is acceptable when it is recorded in the relevant plan or decision
record with an owner and a removal or review condition.

## 1. Layer contract

```text
XAML View (V_Panel_*)
    -> Unity ViewModel (VM_*)
        -> Unity helper/controller (Helper_*)
            -> plain C# domain and simulation
```

| Layer | Owns | Must not own |
| --- | --- | --- |
| `V_Panel_*` XAML | Layout, bindings, styles, templates, visual states, presentation animation | Simulation rules, persistence, reward calculations, scene discovery, or business decisions |
| `VM_*` `MonoBehaviour` | Display-ready values, UI-only state, commands, validation of input shape | Authoritative domain state, simulation advancement, persistence, or direct feature logic |
| `Helper_*` `MonoBehaviour` | Unity-facing orchestration, feature workflows, scene transitions, persistence adapters, and calls into domain systems | XAML controls, Noesis view objects, or ViewModel references |
| Plain C# domain | Deterministic rules, state transitions, simulation, progression, and result calculation | Unity UI, Noesis, scene objects, or presentation strings |

The ViewModel may call a helper directly. The helper returns state, snapshots,
results, or errors that the ViewModel projects for display. There must be one
authoritative owner for each piece of runtime state.

## 2. Naming and file boundaries

New player-facing UI follows these exact prefixes for both the source file and
the primary type where a type exists:

| Responsibility | File/type convention | Example |
| --- | --- | --- |
| ViewModel | `VM_<Feature>.cs` / `VM_<Feature>` | `VM_Settings.cs` |
| XAML view/panel | `V_Panel_<Feature>.xaml` | `V_Panel_Settings.xaml` |
| Unity helper/controller | `Helper_<Feature>.cs` / `Helper_<Feature>` | `Helper_Settings.cs` |

One primary Unity component belongs in one matching C# file. A feature should
have one intentional ViewModel/View pair; add child pairs for meaningful
windows or tools, not for every decorative control.

Legacy names may remain until the feature is already being changed. Renames
are staged and Unity-aware: preserve `.meta` files and GUIDs, update serialized
references, validate scene/prefab loading, and avoid broad rename passes.

The custom `SpeciesSimulationBoard` renderer is a View implementation exception:
it may remain a C# Noesis control because it draws a large grid efficiently. It
does not become a ViewModel and must not acquire simulation ownership.

## 3. XAML/View restrictions

XAML is declarative presentation. XAML files **MUST** be limited to:

- Layout, resources, styles, templates, bindings, commands, and accessibility
  metadata.
- `VisualStateManager` states, transitions, and animation used to present an
  already-authoritative UI state.
- Presentation-only behaviors such as zoom, focus, pointer gestures, and
  animation triggers.

XAML **MUST NOT** contain:

- `x:Code`, business-logic event handlers, or code-behind that changes domain
  state.
- Simulation rule evaluation, tick advancement, species mutation, reward
  calculation, profile/save writes, or direct `PlayerPrefs` access.
- Scene lookup, `Find*` calls, runtime object creation, asset-loading policy,
  or direct references to domain services.
- A second copy of authoritative state hidden in a trigger, converter, or
  visual-state storyboard.

Visual states are presentation state. They may show `Welcome`, `Play`,
`Rewards`, `Failure`, or `FinalReport`, but they do not decide when the game
enters those states. Bind to a state exposed by the ViewModel and let the
authoritative manager/helper perform the transition.

## 4. ViewModel restrictions

`VM_*` components **MUST** be Unity `MonoBehaviour`s and expose binding
properties and commands through the project’s Noesis-compatible notification
pattern. They **SHOULD** be small enough that a reviewer can identify the
screen’s state and commands without reading simulation code.

ViewModels **MUST**:

- Expose display data and UI state such as visibility, selection, validation, and
  command availability.
- Send player intent to a helper rather than implementing the resulting
  workflow.
- Disabled commands must not run.
- Release subscriptions at the matching Unity lifecycle boundary.

ViewModels **MUST NOT**:

- Advance a simulation, own the authoritative profile/run/reward state, or
  write persistence directly.
- Reimplement domain rules because a value is convenient to bind.
- Discover scene objects with `Find*`, add runtime components as normal setup,
  or reach into a `NoesisView` to find controls. Composition hosts wire the
  references and data context.
- Poll every frame for changes when a helper can publish a completed snapshot
  or a meaningful state-change notification. A bounded poll is a migration
  exception and must have a reason.
- Store a second mutable copy of helper/domain state without an explicit
  distinction between a UI draft and the committed value.

The current legacy `SpeciesSimulationViewModel` and `MainMenuViewModel` may
violate some of these rules during migration. Do not expand those exceptions;
new work should use the target boundary.

## 5. Helper/controller restrictions

`Helper_*` components are the Unity-facing seam between the ViewModel and the
domain. They **MUST**:

- Own explicit serialized references, lifecycle orchestration, scene requests,
  persistence adapters, and feature workflows that are not presentation.
- Return immutable or read-only snapshots/results where practical.
- Keep helpers focused on feature behavior; ViewModels and XAML handle display
  text and presentation.
- Make ownership and cleanup of subscriptions, coroutines, and events explicit.

Helpers **MUST NOT** reference XAML files, `NoesisView`, or `VM_*` types. Do not
introduce a global event bus, service locator, or singleton merely to avoid a
serialized reference. Add an interface only when there is a real substitution,
testing, or module boundary; direct references are preferred while ownership is
clear.

For simulation, plain C# `SimulationManager` owns run lifecycle and calls the
deterministic runner. `Helper_Simulation` is the narrow Unity-facing micro-API
that forwards player intent and Unity lifecycle time to that manager. It
provides board and shell snapshots to their ViewModels. The board renderer only
consumes its projection and draws pixels.

## 6. Domain and determinism

Domain/simulation code **MUST** remain UI-agnostic and deterministic for a
given seed, configuration, and input sequence. UI code may request an action,
but it must not alter the outcome through frame timing, visual-state timing,
or a second random source.

The simulation manager is the single owner of tick advancement. A ViewModel or
renderer must never call `AdvanceOneTick`, mutate a species, or infer a result
from pixels. Presentation snapshots should identify their source tick or run
revision so stale updates can be rejected during asynchronous or scene
transitions.

## 7. State ownership and VisualStateManager

Gameplay flow has one authoritative state owner in the application/helper
layer. The simulation flow is expected to use states such as:

```text
Welcome -> Play -> Rewards
                    \-> Failure
Rewards -> Play
Play -> FinalReport
FinalReport -> GalapagOS Lab
```

The exact transition graph remains a feature decision, but every transition
must be explicit and testable. `VisualStateManager` maps that state to
presentation, focus, animation, and enabled controls. It is not a navigation
framework or a second state machine.

Main Menu, Lab, and Simulation scene transitions must be requested through an
explicit composition/helper boundary. Do not rely on a XAML visual state or a
destroyed scene object to carry session state across loads. The chosen scene
loading mode (single or additive) must be explicit in the transition contract
and recorded when that decision is made.

## 8. Composition and scene structure

Composition roots/hosts own the wiring of views, ViewModels, helpers, board
renderers, and serialized scene references. Normal runtime composition should
not depend on `Find*` calls or hidden `AddComponent` side effects.

The intended player-facing structure is:

```text
Main Menu scene
  VM_MainMenu + V_Panel_MainMenu

GalapagOS Lab scene
  VM_Lab + V_Panel_Lab
  feature pairs such as VM_Research + V_Panel_Research

Simulation scene
  VM_SimulationShell + V_Panel_SimulationShell
  VM_SimulationBoard + board View/custom renderer
  Helper_Simulation
  SimulationManager
```

The Lab shell may host feature windows, but each meaningful feature owns its
own ViewModel/View pair and calls its helper for business operations. The
simulation shell may contain child data contexts, but board projection and
shell flow must not become one undifferentiated ViewModel.

## 9. Data ownership quick reference

| Data or behavior | Authoritative owner | UI representation |
| --- | --- | --- |
| Species rules, ticks, outcomes | Plain C# domain + simulation helper/manager | Read-only ViewModel projection |
| Profile/save data | Persistence/domain owner reached through a helper | Display fields and commands |
| Current UI selection, draft text, modal visibility | ViewModel | XAML bindings |
| Welcome/Play/Rewards/Failure/FinalReport flow | Simulation/application helper | Visual state and enabled controls |
| Pixel board rendering | Board View/custom renderer | Sprite/texture output |
| Animation, focus, hover, transition polish | XAML/VisualStateManager | No gameplay side effects |

When ownership is unclear, stop and document the boundary before adding a new
field or event. Duplicated state is a correctness risk, not just a style issue.

## 10. Review and verification checklist

Every new or substantially changed UI feature should answer these questions:

- [ ] Does the file/type use the `VM_*`, `V_Panel_*`, or `Helper_*` convention?
- [ ] Is there one authoritative owner for each changed piece of state?
- [ ] Does the XAML contain only declarative presentation and presentation-only
      behaviors?
- [ ] Does the ViewModel expose intent and display data without advancing rules
      or writing persistence?
- [ ] Does business logic live behind an explicit helper/domain boundary?
- [ ] Are visual states mapping authoritative state rather than deciding it?
- [ ] Are scene and serialized references explicit and `.meta`/GUIDs preserved?
- [ ] Are subscriptions, coroutines, and event lifetimes cleaned up?
- [ ] Is deterministic domain behavior covered by Edit Mode tests where
      applicable, with Play Mode coverage for composition and binding?
- [ ] Was the diff inspected and `git diff --check` run?

For a simulation UI change, also verify that the same seed and inputs produce
the same result with the UI attached or detached.

## 11. Exceptions and migration

This guideline does not require an unsafe rename or a speculative rewrite. The
existing Noesis prototype may retain compatibility names and temporary wiring
until the owning feature is migrated. Each migration should be small enough to
compile and load in Unity, preserve serialized references, and leave the old
path removable or clearly marked.

Editor tooling, debug panels, and a measured custom renderer may use a
different composition pattern when they are clearly isolated from player
runtime UI. Record the exception near the feature and do not let it become the
default production pattern by imitation.

## Revision history

| Version | Date | Change |
| --- | --- | --- |
| 1.0 | 2026-08-25 | Initial proposed UI/MVVM boundary guideline. |
