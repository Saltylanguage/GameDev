# Game architecture and flows

> Status: Living reference  
> Last reviewed: 2026-09-04
> Scope: Player loop, runtime boundaries, and production progression

This document compresses the current game structure into three complementary
views. It introduces no new design decision. Product and implementation details
remain authoritative in the linked source documents.

## 1. Player and run loop

```mermaid
flowchart TB
    START["Launch"]
    MENU["Main Menu<br/>Profile · Continue · Quit"]
    LAB["GalapagOS Lab<br/>Overview · Research<br/>Species Archive · Expedition Setup"]
    SETUP["Prepare Expedition<br/>Scenario · Species · Seed · Unlocks"]

    SIM["Simulate 200 ticks<br/>Forest Edge: Fern → Hare → Fox"]
    END{"Extinct or<br/>phase five complete?"}
    SUMMARY["Phase Summary<br/>Population · Births · Deaths<br/>Food · Movement · Combat"]
    UPGRADE["Choose one upgrade or skip<br/>World remains frozen"]
    RULESET["Update ordered, fingerprinted<br/>Hare ruleset"]

    RESULTS["Results<br/>Victory · Narrow Survival · Defeat"]
    REWARD["Accomplishments<br/>and persistent unlocks"]

    START --> MENU --> LAB --> SETUP --> SIM
    SIM --> END

    END -->|"No: phases 1–4"| SUMMARY --> UPGRADE --> RULESET
    RULESET -->|"Continue same world and next tick"| SIM
    END -->|"Yes"| RESULTS --> REWARD --> LAB
```

The vertical-slice contract is five 200-tick phases, four upgrade decisions,
and an immediate end after a completed tick causes extinction. The player
changes the species rules rather than directly commanding individual cells.

This is the target player loop. The current preview still reinitializes between
windows; its replacement is [planned](../CONTINUOUS_SIMULATION_FLOW_PLAN.md).
Phases retain creatures, resources, time and history. Only a new expedition or
explicit restart creates a new board. The current 20-second prototype phase and
the product's longer viewing-time target are separate pacing settings.

## 2. Runtime architecture

```mermaid
flowchart TB
    VIEW["Noesis XAML Views<br/>V_Panel_*"]
    VM["Unity ViewModels<br/>VM_*"]
    HELPER["Unity Helpers<br/>Simulation · Profiles · Transitions"]
    DOMAIN["Plain C# Domain<br/>Simulation · Progression · Results"]

    ASSETS["Scenario and Species Assets"]
    DATA["Frozen Expedition Base Data<br/>Immutable Effective Rules per Phase"]
    SAVE["Versioned Profile<br/>Settings and Unlocks"]

    SHELL["Read-Only UI Snapshots"]
    BOARD["SimulationBoardSnapshot"]
    RENDERER["Custom Batched<br/>Board Renderer"]

    DEVLAB["Developer Lab<br/>Seeds · Tuning · Diagnostics"]

    VIEW -->|"Commands"| VM
    VM -->|"Player intent"| HELPER
    HELPER -->|"Validated requests"| DOMAIN

    ASSETS --> DATA --> DOMAIN
    SAVE <--> HELPER
    DEVLAB --> HELPER

    DOMAIN --> SHELL --> VM
    DOMAIN --> BOARD --> RENDERER --> VIEW
```

The dependency direction is:

```text
View → ViewModel → Helper → Domain
```

The domain does not depend on Noesis, XAML, or player-facing UI state. The live
board uses a dedicated renderer backed by an immutable snapshot instead of one
XAML control per cell.

## 3. Production roadmap

```mermaid
flowchart TB
    M0["M0 · Production Definition<br/>Complete"]
    M1["M1 · Playable Upgrade Loop<br/>Current milestone"]
    M2["M2 · Vertical Slice<br/>One scenario · Three builds<br/>Complete roguelike loop"]
    M3["M3 · Content Alpha<br/>More species and scenarios<br/>Feature lock"]
    M4["M4 · Beta and Release<br/>Balance · Onboarding · Performance<br/>Accessibility · Platform work"]

    S2["Active Sprint 2<br/>First trustworthy<br/>temporary upgrade"]

    M0 --> M1 --> M2 --> M3 --> M4
    S2 -.-> M1
```

The current production question is deliberately small: can a player choose an
upgrade, observe it changing the ecosystem, and understand why the run changed?

## Authoritative sources

- [Project context](../PROJECT_CONTEXT.md)
- [Vertical-slice product brief](../PRODUCT_BRIEF.md)
- [Production roadmap](../../ROADMAP.md)
- [Main Menu and Lab delivery plan](../MAIN_MENU_LAB_DELIVERY_PLAN.md)
- [Unity MVVM architecture plan](../UNITY_MVVM_ARCHITECTURE_PLAN.md)
- [Upgrade-system direction](../UPGRADE_SYSTEM_DIRECTION.md)
- [Current work-bucket plan](../NEXT_WORK_BUCKET_PLAN.md)
