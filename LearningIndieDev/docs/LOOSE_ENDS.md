# Loose Ends

This is the durable ledger for unresolved project organization, documentation,
planning, ownership, and understanding gaps.

Run the review with `/Loose Ends`, `Loose Ends`, or `Show me my Loose Ends`. The installed skill's formal UI name is `LooseEnds`.

## Status

- Last reviewed: 2026-08-15
- Report state: Findings recorded; no P0 blockers found

## Triage rules

- **P0** — blocks current work, risks data loss, or represents a material contradiction.
- **P1** — likely to cause avoidable rework or leave an active plan ownerless.
- **P2** — useful cleanup, clarification, or follow-up that is not currently blocking.

## Open items

### P1-001 — Forest Edge balance is outside the vertical-slice target

- **Status:** Still open.
- **Evidence:** `artifacts/playmode-last-run.md` and `.json`; the latest 32x32,
  200-tick run moved Hare from 22 to 57, Fox from 4 to 3, and Plant from 314
  to 684. The current Hare target is approximately 26–33 final population,
  with Foxes remaining relevant but viable.
- **Next action:** Run fixed-seed comparisons that reduce Hare reproduction
  throughput and establish a meaningful regional Grass carrying limit before
  expanding the roster.
- **Likely owner:** Josh + Sim.
- **Confidence:** High.

### P1-002 — Fox mating intent is not producing offspring

- **Status:** Still open.
- **Evidence:** The same report records 236 Fox Mating ticks, 5 kills, 1
  starvation death, and 0 Fox births.
- **Next action:** Add eligible-mating, blocked-mating, and successful-mating
  metrics so energy, adjacency, timing, and chance failures can be separated
  before tuning values.
- **Likely owner:** Simulation/tooling owner.
- **Confidence:** High.

### P1-003 — Eating state telemetry is undercounted for Foxes

- **Status:** New finding from the latest report review.
- **Evidence:** Fox activity records 5 food events, while aggregate Fox Eating
  ticks are zero; tracked transition history still references Eating around
  attack resolution.
- **Next action:** Distinguish FSM decision ticks from resolver-applied action
  states, or record action states after attack/feeding resolution. Add a
  regression assertion for the report fields.
- **Likely owner:** Simulation/tooling owner.
- **Confidence:** High.

### P1-004 — Upgrade catalog and contribution telemetry are incomplete

- **Status:** Still open.
- **Evidence:** `docs/VERTICAL_SLICE_SELECTION.md` and `ROADMAP.md` require
  distinct Trailblazer, Warren, Gardeners, Tracker, and Ambusher paths and an
  initial catalog of roughly 6–10 explicit upgrades. The current prototype only
  exposes movement, attack, and block upgrades; Hare has no attack pattern.
- **Next action:** Define explicit upgrade effects, tradeoffs, stacking rules,
  previews, ordered loadout recording, and activation/contribution telemetry.
- **Likely owner:** Josh + Sim.
- **Confidence:** High.

### P1-005 — Player-facing shell and Dev Lab split lack acceptance evidence

- **Status:** Still open; planning is recorded but implementation readiness is
  not yet demonstrated.
- **Evidence:** `docs/SPRINT_0_CLOSEOUT_PLAN.md` and `docs/SPRINT_1_PLAN.md`
  remain discussion drafts. The Main Menu → Lab → Research route, target
  resolution checks, and separate Dev Lab responsibilities have no recorded
  acceptance result.
- **Next action:** Close S0.C1–S0.C4, confirm MainMenu scene readiness, then
  implement the smallest S1 shell with the stated smoke and focus checks.
- **Likely owner:** Josh + Sim.
- **Confidence:** Medium-high.

### P1-007 — Research source pass and local skill files are uncommitted

- **Status:** New/uncommitted working-tree state.
- **Evidence:** `git status` shows a modified
  `docs/Research/CHANGE_IMPACT_ANALYSIS_RESEARCH_BRIEF.md`, untracked
  `docs/Research/CHANGE_IMPACT_ANALYSIS_SOURCE_READINGS.md`, and untracked
  `.agents/skills/loose-ends/`.
- **Next action:** Decide whether the research source pass belongs in the next
  commit; either complete and commit the LooseEnds skill package or keep it
  explicitly local-only. Do not discard either without an ownership decision.
- **Likely owner:** Repository maintainer.
- **Confidence:** High.

### P2-001 — Discord collaboration proof remains unassigned

- **Status:** Controlled future work, not a current blocker.
- **Evidence:** `docs/DISCORD_AGENT_COLLABORATION_TODOS.md` still leaves the
  final channel contract, authenticated read/write proof, handoff automation,
  and safety validation unchecked.
- **Next action:** Resolve the contract first, then run one restricted transport
  proof before adding automation.
- **Likely owner:** Josh + Sim.
- **Confidence:** High.

### P2-002 — Deferred mechanics need trigger-based review only

- **Status:** Intentionally deferred.
- **Evidence:** `docs/CELLULAR_SIM_TODOS.md`, `docs/NEXT_ARCHITECTURE_BATCH.md`,
  `docs/SPECIES_IDEAS_SCRATCHPAD.md`, and `ROADMAP.md` defer scent, generalized
  event output, custom terrain authoring, alpha qualification/pack behavior,
  and geometry-directed colony construction.
- **Next action:** Leave these out of the current balance/UI lane. Revisit only
  when the documented trigger or vertical-slice gate is met.
- **Likely owner:** Product owner; implementation owner TBD.
- **Confidence:** High.

### P2-003 — Documentation hygiene items remain

- **Status:** Cleanup needed, not blocking.
- **Evidence:** `LearningIndieDev/AGENTS.md` contains a duplicated Studio
  Guidelines instruction. The historical 2026-08-14 Play Mode handoff still
  says full execution is pending even though a newer Play Mode artifact exists.
- **Next action:** Remove the duplicate instruction and add a newer handoff when
  the next material simulation/reporting change is made; preserve the old note
  as history.
- **Likely owner:** Repository maintainer.
- **Confidence:** High.

## Resolved items

### R-001 — Creature identity, Dead state, and correlated FSM logging

- **Evidence:** Commit `ff59da90`; `SpeciesCell`, `SpeciesSimulationMetrics`,
  `SpeciesSimulation`, Play Mode/batch report serializers, and focused tests.
- **Result:** Persistent entity IDs, `Previous → Dead` transitions, runtime
  `[FSM][Tracked]` logs, and report transition streams are implemented and
  pushed.

### R-002 — Simulation data and authored scenario foundation

- **Evidence:** Commit `ff59da90`; SpeciesId migration, terrain/resource layers,
  scenario assets, deterministic fingerprints, initial-grid tooling, and
  runtime/editor test coverage.
- **Result:** The data-driven simulation foundation is in place; generalized
  systems remain intentionally trigger-gated under the TODO list.

### R-003 — Research and studio reporting foundation

- **Evidence:** Commit `ff59da90`; Studio Guidelines, research plan, experiment
  templates, baseline package, and project-context links.
- **Result:** The evidence workflow is documented. EX-001 is now accepted using
  the authored 32 x 32 ForestEdge configuration; the uncommitted source-reading
  pass remains open separately above.

### R-004 — Play Mode result persistence

- **Evidence:** `artifacts/playmode-last-run.md` and `.json`, plus the Unity
  Console verification after the latest run.
- **Result:** The old handoff's “Play Mode pending” statement is historical;
  the completion report and tracked transition output now exist.

### R-005 — EX-001 current-code reproducibility and replay gate

- **Evidence:** `docs/Research/Experiments/EX-001-Reproducibility-Baseline/`
  paired schema-4 reports, normalized comparison, replay manifests, retained
  replay JSON results, and `DEC-EXP-001-0002`.
- **Result:** The Forest Edge matrix reproduced across two current-code runs
  for seeds 10100–10119. Representative seed 10102 and boundary seed 10116
  replayed with 4/4 Play Mode evidence tests passing and matching source player
  populations. The archived 32 x 20 report is retained as historical evidence,
  while the live authored configuration is consistently 32 x 32; no balance or
  causal claim was promoted.
