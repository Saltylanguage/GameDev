# Working state

This file is the stable doorway into current collaboration context. It should not
become a master changelog.

## Current focus

**Context refresh: 2026-09-12.** The repository has moved from the 2026-09-09
simulation/Genome checkpoint into roadmap review and player-shell polish. The
working tree contains both completed-in-handoff work and changes still awaiting
review; treat the status notes below as bounded claims, not an implicit commit.

**Roadmap v2 is active as of 2026-09-11.** It marks M0 complete, keeps M1
active, assigns F01–F20 feature IDs and effort bands, allocates the proposed
Sprint 3 forty-hour envelope, and treats S4–S7 dates as a forecast rather than a
commitment. The GalapagOS Desktop is the canonical player home; the standalone
Lab remains a legacy/developer route until deliberately migrated.

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

The project-owned Noesis/XAML image pipeline is now migrated: every XAML image
and icon consumer uses a `StaticResource` from
[`Assets/UI/ImageResources.xaml`](../Assets/UI/ImageResources.xaml), with
`GlobalResources.xaml` providing the shared merge and Main Menu loading the
dictionary directly. The custom simulation-board sprite atlas remains a
runtime renderer input rather than an XAML image consumer.
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
profile work remain planned. Roadmap v2 schedules the first Genome contract in
S4 and its persistence-backed implementation in S6. Named
Genome loadouts are deferred and non-blocking, and Species Mastery remains
deferred and non-gating. Skip is a valid current choice with no current bonus or
penalty; any future reward-doubling for skipping is a separate deferred economy
rule. Provisional scientific-data settlement remains open pending feature-owner
approval.

The current worktree also contains a Main Menu polish/refinement pass, broader
GalapagOS Desktop/Simulation/Lab XAML and ViewModel updates, and new concept
art for Settings, Species Collection, Expedition Setup, and Field Notes. The
Main Menu handoff is **Needs Review**: the meadow background, CRT-style boot and
desktop handoff, focus behavior, and procedural chime were verified in Unity,
but title/brand and promotion of generated art remain human decisions. The
concept images are references, not approved production assets. A focused
Settings/Collection PlayMode invocation on 2026-09-12 exited without producing
results XML, so it is not acceptance evidence.

The 2026-09-09 artifact-retention audit and cleanup removed only approved,
verified duplicates and old clean logs (about 1.35 GB). Compact summaries and
cited raw evidence remain; five semantic duplicate bundles are still review
candidates under the retention handoff.
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
- Official Noesis image resource pipeline: [`Studio Guidelines/SG-006-NOESIS-IMAGE-RESOURCE-PIPELINE.md`](Studio%20Guidelines/SG-006-NOESIS-IMAGE-RESOURCE-PIPELINE.md)
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
- Roadmap v2 review handoff: [`handoffs/2026-09-11-codex-roadmap-v2.md`](handoffs/2026-09-11-codex-roadmap-v2.md)
- Current baseline before Genome: [`handoffs/2026-09-12-0523-codex-current-baseline-before-genome.md`](handoffs/2026-09-12-0523-codex-current-baseline-before-genome.md)
- Main Menu polish and refinement handoff: [`handoffs/2026-09-09-codex-main-menu-polish-first-pass.md`](handoffs/2026-09-09-codex-main-menu-polish-first-pass.md)
- Artifact retention audit: [`handoffs/2026-09-09-artifact-retention-audit.md`](handoffs/2026-09-09-artifact-retention-audit.md)
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
