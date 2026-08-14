# Working state

This file is the stable doorway into current collaboration context. It should not
become a master changelog.

- Durable product direction: [`PROJECT_CONTEXT.md`](PROJECT_CONTEXT.md)
- Vertical-slice product brief: [`PRODUCT_BRIEF.md`](PRODUCT_BRIEF.md)
- Vertical-slice scenario, roster, and builds: [`VERTICAL_SLICE_SELECTION.md`](VERTICAL_SLICE_SELECTION.md)
- Future scientific-data economy: [`SCIENTIFIC_DATA_ECONOMY.md`](SCIENTIFIC_DATA_ECONOMY.md)
- Future permanent and per-run upgrade systems: [`UPGRADE_SYSTEM_DIRECTION.md`](UPGRADE_SYSTEM_DIRECTION.md)
- Main Menu, Lab, and progression delivery plan: [`MAIN_MENU_LAB_DELIVERY_PLAN.md`](MAIN_MENU_LAB_DELIVERY_PLAN.md)
- Sprint 0 closeout plan: [`SPRINT_0_CLOSEOUT_PLAN.md`](SPRINT_0_CLOSEOUT_PLAN.md)
- Sprint 1 discussion plan: [`SPRINT_1_PLAN.md`](SPRINT_1_PLAN.md)
- Active production roadmap and sprint plan: [`../ROADMAP.md`](../ROADMAP.md)
- Design scratchpad: [`SPECIES_IDEAS_SCRATCHPAD.md`](SPECIES_IDEAS_SCRATCHPAD.md)
- Hunting-strategy ideation: [`Species Design/HUNTING_STRATEGIES_IDEATION.md`](Species%20Design/HUNTING_STRATEGIES_IDEATION.md)
- Cellular simulation deferred work: [`CELLULAR_SIM_TODOS.md`](CELLULAR_SIM_TODOS.md)
- Unity simulation execution and experiment tooling: [`UNITY_SIMULATION_TOOLING.md`](UNITY_SIMULATION_TOOLING.md)
- Discord agent collaboration goal: [`DISCORD_AGENT_COLLABORATION_TODOS.md`](DISCORD_AGENT_COLLABORATION_TODOS.md)
- Discord message contract: [`DISCORD_AGENT_COLLABORATION_PROTOCOL.md`](DISCORD_AGENT_COLLABORATION_PROTOCOL.md)
- Legacy prototype audit: [`LEGACY_PROTOTYPE_AUDIT.md`](LEGACY_PROTOTYPE_AUDIT.md)
- Next architecture batch: [`NEXT_ARCHITECTURE_BATCH.md`](NEXT_ARCHITECTURE_BATCH.md)
- One-note-per-task handoff journal: [`handoffs/`](handoffs/)
- Handoff process: [`COLLABORATION_WORKFLOW.md`](COLLABORATION_WORKFLOW.md)

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
