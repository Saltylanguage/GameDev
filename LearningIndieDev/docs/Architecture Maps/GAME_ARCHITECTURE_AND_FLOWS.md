# Game architecture and flows

> Status: Living reference  
> Last reviewed: 2026-09-06
> Scope: Player loop, runtime boundaries, and production progression

This document compresses the current game structure into three complementary
views. It introduces no new design decision. Product and implementation details
remain authoritative in the linked source documents.

## 1. Player and run loop

```mermaid
flowchart TB
    START["Launch"]
    MENU["Main Menu<br/>Profile · Continue · Quit"]
    LAB["GalapagOS Lab<br/>Overview · Gene Lab<br/>Species Archive · Expedition Setup"]
    SETUP["Prepare Simulation<br/>Mode · Scenario · Species · Seed<br/>Frozen Active Genomes"]

    SIM["Simulate 200 ticks<br/>Forest Edge: Fern → Hare → Fox"]
    END{"Extinct or<br/>phase ten complete?"}
    SUMMARY["Phase Summary<br/>Population · Births · Deaths<br/>Food · Movement · Combat"]
    UPGRADE["Choose one Mutation or skip<br/>World remains frozen"]
    RULESET["Update ordered, fingerprinted<br/>selected-species rules"]

    RESULTS["Results<br/>Victory · Narrow Survival · Defeat"]
    REWARD["Accomplishments<br/>and persistent unlocks"]

    START --> MENU --> LAB --> SETUP --> SIM
    SIM --> END

    END -->|"No: phases 1–9"| SUMMARY --> UPGRADE --> RULESET
    RULESET -->|"Continue same world and next tick"| SIM
    END -->|"Yes"| RESULTS --> REWARD --> LAB
```

The vertical-slice contract is ten phases, with 200 ticks as the current
per-phase target, and nine Mutation decision points,
and an immediate end after a completed tick causes extinction. The player
changes the species rules rather than directly commanding individual cells.

Between simulations, the player can permanently unlock Genome options and
change which unlocked nodes are active. Each participating species receives
its frozen active Genome even when it is not the player-controlled species.
Mutations exist only in Species Simulations; Biome Simulations use active
Genomes without Mutation choices.

This is the target player loop. The controlled preview now retains creatures,
resources, time and history through its phase decisions. Only a new expedition
or explicit restart creates a new board. The configurable prototype phase and
the product's longer viewing-time target are separate pacing settings.

## 2. Runtime architecture

```mermaid
flowchart TB
    VIEW["Noesis XAML Views<br/>V_Panel_*"]
    VM["Unity ViewModels<br/>VM_*"]
    HELPER["Unity Helpers<br/>Simulation · Profiles · Transitions"]
    DOMAIN["Plain C# Domain<br/>Simulation · Progression · Results"]

    ASSETS["Scenario and Natural Species Assets"]
    GENOME["Permanent Unlocks + Frozen Active Genomes<br/>All Scenario Species"]
    MUTATIONS["Ordered Expedition Mutations<br/>Selected Species Mode Only"]
    DATA["Frozen Simulation Data<br/>Mode · Natural · Active Genome<br/>Mutations When Allowed"]
    SAVE["Versioned Profile<br/>Settings · Genome Unlocks<br/>Active Configurations"]

    SHELL["Read-Only UI Snapshots"]
    BOARD["SimulationBoardSnapshot"]
    RENDERER["Custom Batched<br/>Board Renderer"]

    DEVLAB["Developer Lab<br/>Seeds · Tuning · Diagnostics"]

    VIEW -->|"Commands"| VM
    VM -->|"Player intent"| HELPER
    HELPER -->|"Validated requests"| DOMAIN

    ASSETS --> DATA --> DOMAIN
    SAVE --> GENOME --> DATA
    MUTATIONS --> DATA
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

The current production question is deliberately small: can a player choose a
Mutation, observe it changing the ecosystem, and understand why the Species
Simulation changed? The next progression question is whether a permanent Genome
unlock can be activated or deactivated between simulations, is applied to its
species in both player and background roles when active, and produces readable
focal and Biome consequences.

## Authoritative sources

- [Project context](../PROJECT_CONTEXT.md)
- [Vertical-slice product brief](../PRODUCT_BRIEF.md)
- [Production roadmap](../../ROADMAP.md)
- [Main Menu and Lab delivery plan](../MAIN_MENU_LAB_DELIVERY_PLAN.md)
- [Unity MVVM architecture plan](../UNITY_MVVM_ARCHITECTURE_PLAN.md)
- [Upgrade-system direction](../UPGRADE_SYSTEM_DIRECTION.md)
- [Upgrade and ecology balance guideline](../Studio%20Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md)
- [Current work-bucket plan](../NEXT_WORK_BUCKET_PLAN.md)
