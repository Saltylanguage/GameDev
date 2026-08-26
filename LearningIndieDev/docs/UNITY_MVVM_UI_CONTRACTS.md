# Unity MVVM UI Contracts

> Status: T0–T3 composition baseline; T2 board snapshot boundary implemented
> Date: 2026-08-26
> Scope: Main Menu, GalapagOS Lab, and Simulation UI

This document freezes the smallest contracts needed to migrate the current
Noesis prototype without moving simulation rules into the UI. The shapes below
are design contracts. T2 now provides the concrete board snapshot and board
ViewModel; the shell snapshot remains a staged contract while the legacy shell
ViewModel is retained for compatibility.

## 1. Ownership

| Concern | Owner | UI receives |
| --- | --- | --- |
| Profile selection and last-loaded profile | Profile/session owner reached through a helper | `ProfileSessionSnapshot` |
| Scene transitions | Explicit Unity transition helper | `SceneTransitionRequest` result/status |
| Simulation ticks and run lifecycle | Plain C# `SimulationManager` | `SimulationShellSnapshot` and `SimulationBoardSnapshot` through `Helper_Simulation` |
| Simulation rules and deterministic outcomes | Plain C# domain/runner | Read-only run/result data |
| Shell and feature presentation | `VM_*` plus XAML | Bindable display state and commands |
| Pixel board drawing | `SpeciesSimulationBoard` View | `SimulationBoardSnapshot` |

There is one authoritative owner for each piece of state. ViewModels may hold
UI drafts and selections, but they do not become a second owner of committed
profile, run, reward, or simulation state.

## 2. Flow state

The application and simulation managers own these states. XAML visual states
only present them:

```text
MainMenu
ProfileSelection
LabOverview
LabFeature
SimulationWelcome
SimulationPlay
SimulationRewards
SimulationFailure
SimulationFinalReport
```

`SimulationPlay` includes the engine's paused condition. The existing domain
`SimulationRunStatus` (`Ready`, `Running`, `Paused`, `Complete`) remains the
engine status and is not replaced by the UI flow state.

Required transitions:

```text
MainMenu -> ProfileSelection
ProfileSelection -> MainMenu
MainMenu -> LabOverview       (Continue with a loaded profile)
LabOverview -> LabFeature
LabFeature -> LabOverview
LabOverview/LabFeature -> SimulationWelcome (launch request)
SimulationWelcome -> SimulationPlay
SimulationPlay -> SimulationRewards
SimulationPlay -> SimulationFailure
SimulationRewards -> SimulationPlay
SimulationRewards -> SimulationFinalReport
SimulationFailure -> SimulationFinalReport
SimulationFinalReport -> LabOverview
Lab -> MainMenu                (explicit return command)
```

`Continue` is unavailable when no profile has been loaded. Profile Selection
is the first-launch path for creating or selecting the initial profile. There
is no separate New Game or Load Game transition.

## 3. Snapshot contracts

### Profile session snapshot

The profile/session owner exposes the minimum state needed by menus and scene
handoffs:

```text
ProfileSessionSnapshot
  HasLoadedProfile : bool
  ProfileId        : stable opaque profile identifier
  ProfileName      : display name
```

An empty snapshot has `HasLoadedProfile = false` and no profile ID. The last
loaded profile must survive application restart through the profile system; it
must not depend on a destroyed Lab or Main Menu object.

### Simulation shell snapshot

The shell receives one read-only projection containing:

```text
SimulationShellSnapshot
  Revision              : monotonically increasing presentation revision
  FlowState             : SimulationWelcome | SimulationPlay | ...
  RunStatus             : Ready | Running | Paused | Complete
  ProfileId             : stable opaque profile identifier
  ScenarioId            : stable scenario identifier
  PlayerSpeciesId       : stable species identifier
  RunNumber             : current run within the session
  Tick                  : current simulation tick
  ElapsedSeconds        : current elapsed simulation time
  DurationSeconds       : configured run duration
  SettingsEditable      : bool
  CanStart              : bool
  CanPause              : bool
  CanResume             : bool
  CanRestart             : bool
  CanStop                : bool
  RewardOptions         : read-only option summaries
  Result                : optional read-only result summary
  Message               : validation or user-facing status message
```

The ViewModel formats this data for bindings. The manager/domain owns the
values and transition decisions.

### Simulation board snapshot

The board View receives a read-only row-major projection rather than a live
domain `Grid<SpeciesCell>` reference:

```text
SimulationBoardSnapshot
  Revision        : same or newer presentation revision as the shell
  Tick            : source simulation tick
  Width           : board width
  Height          : board height
  PlayerSpeciesId : stable species identifier
  Cells           : read-only BoardCellSnapshot collection

BoardCellSnapshot
  TerrainId          : stable terrain identifier
  TerrainVariantMask : four-cardinal presentation mask
  SpeciesId          : occupant species identifier, if any
  ResourceSpeciesId  : plant/resource species identifier, if any
  IsCreature         : bool
  IsPlantResource    : bool
  IsTerrainResource  : bool
  IsPassable         : bool
```

The terrain mask is presentation data derived from neighboring terrain during
snapshot creation. The board renderer consumes it; it does not evaluate
simulation rules or query neighboring domain objects. `Revision` and `Tick`
allow stale projections to be ignored during scene changes or delayed updates.

### Result summary

The final report receives a read-only summary containing the stable run/profile
identity, outcome, tick count, duration, player population, earned data, and
ruleset fingerprint. Detailed metrics remain domain/report data and are added
only when a screen has a demonstrated need for them.

## 4. Command contract

Commands express player intent. They do not expose domain methods directly to
XAML.

### Main Menu

```text
OpenProfileSelection()
SelectProfile(profileId)
CreateInitialProfile(profileDraft)       // only when no profile exists
Continue()
RequestQuit()
ConfirmQuit()
CancelQuit()
```

`Continue()` is executable only when `HasLoadedProfile` is true. Profile
selection and quit confirmation may be visual states within the Main Menu
root.

### Lab

```text
OpenFeature(featureId)
CloseFeature()
BackToOverview()
LaunchExpedition(launchOptions)
ReturnToMainMenu()
```

Feature-specific commands such as research purchase remain owned by that
feature's ViewModel/helper and are not added to the Lab shell contract.

### Simulation shell

```text
StartRun()
PauseRun()
ResumeRun()
RestartRun()
StopRun()
SelectReward(optionId)
ContinueWithoutReward()
StartNextRun()
ReturnToLab()
```

Each command is validated by its helper/manager. A disabled command must not
execute, and invalid requests return a clear failure/status result rather than
partially changing state.

## 5. Scene and profile handoff

Main Menu, Lab, and Simulation load as separate Unity scenes in `Single` mode.
The scene name/path is a Unity composition concern; the domain contract uses
stable scene intent and IDs.

```text
SceneTransitionRequest
  Target       : MainMenu | Lab | Simulation
  ProfileId    : required for Lab and Simulation
  Launch       : optional immutable SimulationLaunchRequest
  ReturnReason : optional result/failure/explicit-back reason
```

```text
SimulationLaunchRequest
  ProfileId
  ScenarioId
  PlayerSpeciesId
  Seed
  OrderedUpgradeIds
  RulesetFingerprint or base-data identity
```

The transition helper consumes the request and loads the target scene. The
target scene obtains its serialized composition references locally and uses
the request/session snapshot as input. No scene object reference crosses a
scene unload.

## 6. Serialized composition contract

Each scene has one explicit composition root/host. References are serialized
or assigned by the scene/prefab; normal runtime setup does not use `Find*` or
`AddComponent` discovery.

| Scene | Required composition references |
| --- | --- |
| Main Menu | Root `NoesisView`, `VM_MainMenu`, profile/session helper, scene transition helper |
| Lab | Root `NoesisView`, `VM_Lab`, feature ViewModels/roots, profile/research helpers, scene transition helper |
| Simulation | Root `NoesisView`, `Helper_Simulation`, plain C# `SimulationManager`, shell ViewModel, board View/renderer, atlas/rendering references, scene transition helper |

The Lab may contain separate feature XAML roots or panels, but they remain
under the same top-level Noesis root/host. That choice does not change the
feature ViewModel/helper boundaries.

The simulation prototype currently satisfies this contract with an authored
composition: `CellularAutomataPrototype` wires its helper, preview, camera,
Noesis view, shell ViewModel, and board ViewModel in the scene. The host only
binds those references and reports missing composition; it does not discover or
create them during normal startup.

## 7. T0 exit criteria

- [x] Flow states and authoritative owners are named.
- [x] Main Menu, Lab, and Simulation command intents are named.
- [x] Shell and board snapshot responsibilities are separated.
- [x] Profile/session and scene handoff data are explicit.
- [x] Serialized composition references are identified.
- [x] No runtime code, scene, prefab, or serialized asset was changed in T0.

T2 and T3 are now complete for the simulation prototype. The board projection
is concrete and covered by focused tests, and the authored composition is
covered by a focused Play Mode composition test.
