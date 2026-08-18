# Loose Ends

This is the durable ledger for unresolved project organization, documentation,
planning, ownership, and understanding gaps.

Run the review with `/Loose Ends`, `Loose Ends`, or `Show me my Loose Ends`. The installed skill's formal UI name is `LooseEnds`.

## Status

- Last reviewed: 2026-08-17
- Report state: Fresh review after `4f04f4b2`; no P0 blockers; art validation and simulation/research blockers remain

## Triage rules

- **P0** — blocks current work, risks data loss, or represents a material contradiction.
- **P1** — likely to cause avoidable rework or leave an active plan ownerless.
- **P2** — useful cleanup, clarification, or follow-up that is not currently blocking.

## Open items

### P1-001 — Forest Edge balance is outside the vertical-slice target

- **Status:** Still open.
- **Evidence:** `artifacts/playmode-last-run.md` and `.json`; the latest 32x32,
  200-tick run ended at Fox 5, Hare 19, and Plant 902. The current Hare target
  is approximately 26–33 final population, with Foxes remaining relevant but
  viable. This is a single unpaired seed, so it is a tuning signal rather than
  a balance conclusion.
- **Next action:** Run fixed-seed comparisons that reduce Hare reproduction
  throughput and establish a meaningful regional Grass carrying limit before
  expanding the roster.
- **Likely owner:** Josh + Sim.
- **Confidence:** High.

### P1-002 — Fox mating reliability is not established

- **Status:** Partially improved; still open.
- **Evidence:** The latest report records 1 Fox birth, 322 Fox Mating ticks,
  6 kills, and a final Fox population of 5. The earlier zero-birth result is
  historical; one birth does not establish reliable reproduction.
- **Next action:** Add eligible-mating, blocked-mating, and successful-mating
  metrics so energy, adjacency, timing, and chance failures can be separated
  before tuning values.
- **Likely owner:** Simulation/tooling owner.
- **Confidence:** High.

### P1-003 — Eating state telemetry is undercounted for Foxes

- **Status:** New finding from the latest report review.
- **Evidence:** Fox activity records 6 food events, while the aggregate table
  still has no Fox Eating row; tracked transition history references
  Hunting → Attacking → Eating → Wandering around attack resolution.
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

### P1-006 — Editor smart-tiling preview references a deleted asset

- **Status:** New finding from the art-pipeline review.
- **Evidence:** `Assets/Editor/SimulationTools/TerrainTilePreviewWindow.cs:10`
  still points at `Assets/Resources/CellularArt/Terrain_01_SpriteSheet.png`.
  That path is absent; the current asset is
  `Assets/Art/Terrain/Terrain_01_SpriteSheet.png`.
- **Next action:** Update the preview to the current asset/SpriteAtlas model,
  then run the 16-mask preview and record visual evidence.
- **Likely owner:** Presentation/art owner.
- **Confidence:** High.

### P1-007 — Species sprite fallback can omit non-Fox/Rabbit species

- **Status:** New finding from the art-pipeline review.
- **Evidence:** `Assets/UI/HUD/Scripts/SpeciesSimulationViewModel.cs:330-332`
  skips the atlas fallback whenever either direct Fox or Rabbit sprite override
  is present. `CreateSpeciesSprites()` only fills the Fox and Rabbit slots,
  while the scene wires both overrides and an animal atlas. Other authored
  species can therefore receive null sprite entries.
- **Next action:** Merge direct overrides with atlas fallback, or explicitly
  scope the scene to those species and add a test covering every authored
  species.
- **Likely owner:** Presentation owner.
- **Confidence:** High.

### P1-008 — Art/presentation commit lacks post-commit runtime acceptance

- **Status:** New finding; implementation is committed and pushed, acceptance
  is still open.
- **Evidence:** Commit `4f04f4b2` standardized the art/SpriteAtlas pipeline and
  scene wiring. No Unity screenshots or gameplay-scale visual checks exist
  after that commit; the relay health check only reports `OK` with zero relay
  processes when Unity is not active.
- **Next action:** Open the preview and `CellularAutomataPrototype` in Unity,
  capture gameplay-scale screenshots, and verify atlas loading, terrain seams,
  and species icon scale.
- **Likely owner:** Josh + Sim.
- **Confidence:** High.

### P1-009 — EX-002 schema-5 execution remains blocked by Unity startup failure

- **Status:** Still open.
- **Evidence:** `docs/Research/Experiments/EX-002-Herbivore-Collapse-Attribution/README.md`
  records integrated schema-5 death telemetry but no factual report because
  Unity batch startup fails before writing artifacts.
- **Next action:** Repair the Unity startup/cache issue, run the same-seed
  schema-5 control, verify death events against aggregate activity, then run
  the declared intervention matrix.
- **Likely owner:** Simulation/tooling owner.
- **Confidence:** High.

### P1-010 — Sprint 1 readiness/control record is not formally closed

- **Status:** Planning readiness is still open.
- **Evidence:** `docs/SPRINT_1_PLAN.md` remains a Discussion draft for
  August 17–23, while `docs/SPRINT_KICKOFF_WORKFLOW.md` requires a durable
  sprint control record with owners, acceptance, and carry-over decisions.
- **Next action:** Confirm the Trello sprint control record, owners, acceptance
  checks, and cut order; record the kickoff/closeout state before treating the
  sprint as active execution.
- **Likely owner:** Josh + Sim.
- **Confidence:** Medium-high.

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
- **Next action:** Remove the duplicate instruction and preserve old handoffs as
  history while adding newer notes when status changes.
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
  the authored 32 x 32 ForestEdge configuration. The source-reading pass is
  tracked and committed; EX-002 execution remains separately blocked above.

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
  populations. The superseded 32 x 20 report's provenance facts remain in the
  current EX-001 record, but its source artifact was removed; the live authored
  configuration is consistently 32 x 32. No balance or causal claim was
  promoted.

### R-006 — Research source pass and local skill files are now tracked

- **Evidence:** `git ls-files` includes
  `docs/Research/CHANGE_IMPACT_ANALYSIS_SOURCE_READINGS.md` and
  `.agents/skills/loose-ends/**`; the branch is clean.
- **Result:** The previous uncommitted-working-tree finding is resolved. Future
  changes to the source-reading package should be handled as normal commits.

### R-007 — Art/presentation working-tree batch is committed and pushed

- **Evidence:** Commit `4f04f4b2` is present on
  `codex/cellular-sprite-tiling` and the branch is synchronized with origin.
- **Result:** Standardized art exports, SpriteAtlas assets, scene wiring, and
  relay-health tooling are shared. Runtime visual acceptance remains open as
  P1-008.
