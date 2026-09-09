# Working state

This file is the stable doorway into current collaboration context. It should not
become a master changelog.

## Current focus

**CF-0 through CF-5 are implemented and verified.** This includes continuation
parity, boundary upgrades, the controlled preview path, phase/final Stat-Lines,
checkpoint replay, the headless schedule, and the accepted EX-010 execution.
CF-6 remains partially open for outer ten-phase duration/memory measurement;
the Windows player smoke and corrected scenario run are complete, and graphics
acceptance is complete.
The active Lab → CellularAutomataPrototype route now explicitly defaults to
Forest Edge with Hare; the separate GalapagOSDesktopTest scene retains its
legacy-default setup for isolated acceptance.
The same-world lifecycle, phase/expedition evidence meaning, initialization-only
upgrade policy, above-cap energy behavior and a versioned fresh-run fixture are
locked in the [consecutive simulation plan](CONTINUOUS_SIMULATION_FLOW_PLAN.md).
The game-design feature sequence is now triaged in
[GAME_FEATURE_ROADMAP_TRIAGE.md](GAME_FEATURE_ROADMAP_TRIAGE.md), starting with
the Expedition Decision Loop. The player-facing expedition contract is ten
phases; remaining design work concerns duration, decision rhythm, rewards, and
terminal outcomes.
The upgrade direction now separates nine temporary Species-Simulation
**Mutations** from each species' permanently unlocked Genome options and
configurable **active Genome**. Mutations never enter Biome Simulations. The
active Genome is frozen at launch and applies to every population of that
species, including when it is not player-controlled. Species and Biome
Simulations use different success scorecards. Scalable balance work uses shared
capabilities and provisional Adaptation Value estimates, but requires
mode-appropriate direct-effect, matchup, and ecosystem evidence under
[`SG-005`](Studio%20Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md). The seven
existing Hare assets are provisional Mutation candidates; Genome runtime and
profile work remain planned.
The preview now supports phase survivor data, live/legacy upgrade choices,
same-run resume, explicit End, and manual inspection. Continuous terminal
completion is results-only; upgrades are offered at phase boundaries, and a
new expedition is an explicit next action. Continuous phases remain the
default player flow; uninterrupted single-run mode is Developer Mode-only.
Phase result/telemetry windows and ordered acquisition timing are now captured
by the runtime and report serializers. Boundary checkpoints can be copied,
restored, and resumed with deterministic runner output. The opt-in headless
schedule applies cumulative per-phase loadouts and emits the same phase
contract. EX-010 has now executed on the approved ten-phase schedule and was
accepted by Josh and Sim as bounded evidence. P3 is closed under its revised
bounded gate; P4–P6 are not started and no next research experiment is selected.

Latest verification: Unity EditMode passed 212/212 on 2026-09-09. The latest
general PlayMode batch passed 21/22 with one intentional visual-capture skip
and no failures. The retained graphics acceptance remains 22/22 at 1280×720,
followed by a focused 1920×1080 pass on 2026-09-07.

- Durable product direction: [`PROJECT_CONTEXT.md`](PROJECT_CONTEXT.md)
- Vertical-slice product brief: [`PRODUCT_BRIEF.md`](PRODUCT_BRIEF.md)
- Consecutive simulation phases — review and migration plan: [`CONTINUOUS_SIMULATION_FLOW_PLAN.md`](CONTINUOUS_SIMULATION_FLOW_PLAN.md)
- Stat-Line, predictive AI and telemetry applicability: [`CONTINUOUS_SIMULATION_EVIDENCE_IMPACT.md`](CONTINUOUS_SIMULATION_EVIDENCE_IMPACT.md)
- Simulation-flow documentation coverage: [`CONTINUOUS_SIMULATION_DOCUMENTATION_AUDIT.md`](CONTINUOUS_SIMULATION_DOCUMENTATION_AUDIT.md)
- Vertical-slice scenario, roster, and builds: [`VERTICAL_SLICE_SELECTION.md`](VERTICAL_SLICE_SELECTION.md)
- Future scientific-data economy: [`SCIENTIFIC_DATA_ECONOMY.md`](SCIENTIFIC_DATA_ECONOMY.md)
- Mutation, Genome, and balance delivery plan: [`UPGRADE_SYSTEM_DIRECTION.md`](UPGRADE_SYSTEM_DIRECTION.md)
- Mutation authoring workflow: [`UPGRADE_AUTHORING_GUIDE.md`](UPGRADE_AUTHORING_GUIDE.md)
- Hare Mutation acceptance matrix: [`UPGRADE_CATALOG_ACCEPTANCE_MATRIX.md`](UPGRADE_CATALOG_ACCEPTANCE_MATRIX.md)
- Official upgrade and ecology balance guideline: [`Studio Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md`](Studio%20Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md)
- Upgrade-system planning concerns: [`Planning concerns/upgrade-system.md`](Planning%20concerns/upgrade-system.md)
- Main Menu, Lab, and progression delivery plan: [`MAIN_MENU_LAB_DELIVERY_PLAN.md`](MAIN_MENU_LAB_DELIVERY_PLAN.md)
- GalapagOS desktop art direction: [`../ART_STYLE_GUIDE.md`](../ART_STYLE_GUIDE.md)
- GalapagOS desktop app ecosystem: [`GALAPAGOS_DESKTOP_APP_ECOSYSTEM_PLAN.md`](GALAPAGOS_DESKTOP_APP_ECOSYSTEM_PLAN.md)
- GalapagOS desktop feature set: [`GALAPAGOS_DESKTOP_FEATURE_SET_PLAN.md`](GALAPAGOS_DESKTOP_FEATURE_SET_PLAN.md)
- Unity MVVM and GalapagOS UI architecture: [`UNITY_MVVM_ARCHITECTURE_PLAN.md`](UNITY_MVVM_ARCHITECTURE_PLAN.md)
- Unity MVVM UI contracts: [`UNITY_MVVM_UI_CONTRACTS.md`](UNITY_MVVM_UI_CONTRACTS.md)
- Sprint 0 closeout plan: [`SPRINT_0_CLOSEOUT_PLAN.md`](SPRINT_0_CLOSEOUT_PLAN.md)
- Sprint 1 authoritative execution plan: [`SPRINT_1_PLAN.md`](SPRINT_1_PLAN.md)
- Sprint Kickoff and carry-over workflow: [`SPRINT_KICKOFF_WORKFLOW.md`](SPRINT_KICKOFF_WORKFLOW.md)
- Active production roadmap and sprint plan: [`../ROADMAP.md`](../ROADMAP.md)
- Proposed Sprint 3 M1 closeout and hygiene plan: [`Sprints/S3-control-record.md`](Sprints/S3-control-record.md)
- Stable-but-incomplete feature action plan: [`INCOMPLETE_FEATURES_ACTION_PLAN.md`](INCOMPLETE_FEATURES_ACTION_PLAN.md)
- Proposed next work bucket: [`NEXT_WORK_BUCKET_PLAN.md`](NEXT_WORK_BUCKET_PLAN.md)
- Sprint 1 species stat-line tickets: [`SPRINT_1_SPECIES_STAT_LINE_TICKETS.md`](SPRINT_1_SPECIES_STAT_LINE_TICKETS.md)
- Fox/Rabbit art foundation lane: [`FOX_RABBIT_ART_FOUNDATION_PLAN.md`](FOX_RABBIT_ART_FOUNDATION_PLAN.md)
- Design scratchpad: [`SPECIES_IDEAS_SCRATCHPAD.md`](SPECIES_IDEAS_SCRATCHPAD.md)
- Fun/design values scratchpad: [`WHAT_IS_FUN.md`](WHAT_IS_FUN.md)
- Hunting-strategy ideation: [`Species Design/HUNTING_STRATEGIES_IDEATION.md`](Species%20Design/HUNTING_STRATEGIES_IDEATION.md)
- Reactive species/ecology arms-race plan: [`REACTIVE_SPECIES_ECOLOGY_PLAN.md`](REACTIVE_SPECIES_ECOLOGY_PLAN.md)
- Cellular simulation deferred work: [`CELLULAR_SIM_TODOS.md`](CELLULAR_SIM_TODOS.md)
- Unity simulation execution and experiment tooling: [`UNITY_SIMULATION_TOOLING.md`](UNITY_SIMULATION_TOOLING.md)
- Proposed custom report dashboard developer tooling: [`CUSTOM_REPORT_DASHBOARD_TOOLING_PLAN.md`](CUSTOM_REPORT_DASHBOARD_TOOLING_PLAN.md)
- Cellular sprite sheets and smart-tiling: [`CELLULAR_SPRITE_TILING_PLAN.md`](CELLULAR_SPRITE_TILING_PLAN.md)
- Future AI workflow skills: [`AI_WORKFLOW_SKILLS_PLAN.md`](AI_WORKFLOW_SKILLS_PLAN.md)
- AI-assisted ecology laboratory research plan: [`Research/AI_ASSISTED_ECOLOGY_LAB_RESEARCH_PLAN.md`](Research/AI_ASSISTED_ECOLOGY_LAB_RESEARCH_PLAN.md)
- Discord agent collaboration goal: [`DISCORD_AGENT_COLLABORATION_TODOS.md`](DISCORD_AGENT_COLLABORATION_TODOS.md)
- Discord message contract: [`DISCORD_AGENT_COLLABORATION_PROTOCOL.md`](DISCORD_AGENT_COLLABORATION_PROTOCOL.md)
- Legacy prototype audit: [`LEGACY_PROTOTYPE_AUDIT.md`](LEGACY_PROTOTYPE_AUDIT.md)
- Next architecture batch: [`NEXT_ARCHITECTURE_BATCH.md`](NEXT_ARCHITECTURE_BATCH.md)
- One-note-per-task handoff journal: [`handoffs/`](handoffs/)
- Handoff process: [`COLLABORATION_WORKFLOW.md`](COLLABORATION_WORKFLOW.md)
- Loose Ends ledger and review protocol: [`LOOSE_ENDS.md`](LOOSE_ENDS.md)
- Project hygiene ticket summaries: [`PROJECT_HYGIENE_TICKET_SUMMARIES.md`](PROJECT_HYGIENE_TICKET_SUMMARIES.md)

## How to get current

1. Read `PROJECT_CONTEXT.md`.
2. Read the newest handoff notes and any notes relevant to the area being changed.
   Filenames sort chronologically and include the contributor and topic.
3. Confirm the notes against the checked-out branch, `git status`, recent commits,
   code, and tests.

Create a new note instead of editing a running history here:

```powershell
.\tools\New-Handoff.cmd -Owner "your-name" -Topic "short feature name"
```

Each generated note links back here. Notes may be corrected while their work is
still local, but once shared they should normally remain historical records; add
a newer note when status or conclusions change.
