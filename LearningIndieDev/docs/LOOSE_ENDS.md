# Loose Ends

This is the durable ledger for unresolved project organization, documentation,
planning, ownership, and understanding gaps.

Run the review with `/Loose Ends`, `Loose Ends`, or `Show me my Loose Ends`. The installed skill's formal UI name is `LooseEnds`.

## Status

- Last reviewed: 2026-08-18
- Report state: Fresh review at `178093f`; no P0 blockers; the working tree
  contains active presentation and JSON/CSV editor-tool work, while art
  acceptance and EX-002 execution remain open

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

- **Status:** Diagnostic validated; a balance/rule decision remains open.
- **Evidence:** Unity Edit Mode produced 126/134 passing tests in
  `artifacts/unity-tests-20260818-200450/EditMode-results.xml`; all six new
  reproduction-funnel tests passed. The eight failures are pre-existing
  movement/combat/telemetry expectations unrelated to the funnel. The repeated
  schema-6 Forest Edge replay at seed `-877772592` is identical across
  `artifacts/cellular-experiment-20260818-200811/report.json` and
  `artifacts/cellular-experiment-20260818-200842/report.json`: 810 Fox
  candidates classify as 410 energy blocks, 341 mate blocks, 2 group-limit
  blocks, 56 chance-roll failures, 0 no-location blocks, and 1 successful
  attempt; reconciliation is true. The dominant gates are insufficient energy
  (50.6%) and missing mate (42.1%), not birth-space availability.
- **Next action:** Do not change balance values from this single seed. Use the
  gate split to predeclare the P1-001 intervention candidates, then run the
  matched multi-seed EX-002 control/intervention matrix.
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

### P1-006 — Editor smart-tiling preview path fix awaits validation

- **Status:** Local fix implemented; validation and integration remain open.
- **Evidence:** The uncommitted change in
  `Assets/Editor/SimulationTools/TerrainTilePreviewWindow.cs:10` now points at
  `Assets/Art/Terrain/Terrain_01_SpriteSheet.png` instead of the deleted
  Resources path. No 16-mask preview evidence or commit records acceptance yet.
- **Next action:** Run the 16-mask preview, record visual evidence, and include
  the fix in a focused reviewed commit.
- **Likely owner:** Presentation/art owner.
- **Confidence:** High.

### P1-007 — Species sprite fallback fix awaits validation

- **Status:** Local fix implemented; validation and integration remain open.
- **Evidence:** The uncommitted change in
  `Assets/UI/HUD/Scripts/SpeciesSimulationViewModel.cs` now builds the complete
  named animal set from the atlas and then applies direct Fox/Rabbit overrides.
  The working tree has no focused presentation regression test or recorded
  runtime acceptance for this behavior.
- **Next action:** Add or run coverage for every authored species, verify the
  scene with direct overrides present, and include the fix in a focused
  reviewed commit.
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

### P1-009 — EX-002 instrumented execution remains open after startup recovery

- **Status:** Still open; Unity batch execution is now available with elevated
  licensing access.
- **Evidence:** The schema-6 Forest Edge control artifacts above include death
  events and the reproduction funnel, proving the report path works after the
  licensing restart. The full EX-002 BaselineParity control, intervention
  matrix, and held-out check have not yet been run.
- **Next action:** Predeclare the collapse endpoint and approved intervention
  surface; run the same-seed schema-6 BaselineParity control; verify death
  events against aggregate activity; then run the matched-seed intervention
  matrix and held-out check.
- **Likely owner:** Simulation/tooling owner.
- **Confidence:** High.

### P1-010 — Sprint 1 readiness/control record is not formally closed

- **Status:** Resolved; the active control record and execution plan are now
  durable and aligned to the two-week project cadence.
- **Evidence:** `docs/Sprints/S1-control-record.md` and the Trello S1 control
  card record the August 17–30 window, owners, acceptance checks, cut order,
  and 40-hour planning capacity (20 hours committed).
- **Next action:** Keep the control record and Trello dates synchronized at the
  next sprint review.
- **Likely owner:** Josh + Sim.
- **Confidence:** Medium-high.

### P1-011 — Active JSON/CSV editor-tool batch lacks a durable handoff and acceptance record

- **Status:** New; implementation and tests are present locally but the work
  package is uncommitted and its integration state is not recorded.
- **Evidence:** The working tree adds
  `Assets/Editor/EditorTools/JsonCsvConverter/`, Editor tests, two new assembly
  definitions, a direct `com.unity.nuget.newtonsoft-json` dependency, and
  generated solution entries. No current handoff identifies the owner,
  intended research workflow, test result, or commit boundary for this batch.
- **Next action:** Confirm ownership and intended scope, run the new Editor test
  assembly in Unity, review the package change, and record a focused handoff or
  commit before mixing it with the presentation fixes.
- **Likely owner:** Current editor-tool author + repository maintainer.
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
- **Next action:** Remove the duplicate instruction and preserve old handoffs as
  history while adding newer notes when status changes.
- **Likely owner:** Repository maintainer.
- **Confidence:** High.

### P2-004 — Research paper contains a stale instrument-readiness statement

- **Status:** New documentation inconsistency; not blocking the experiment.
- **Evidence:** `docs/Research/AI_ASSISTED_ECOLOGY_LAB_RESEARCH_PAPER.md` says
  under “What is implemented so far” that current-code reproducibility is still
  needed, while its later EX-001/EX-001B sections and the accepted experiment
  packages record that the current-code reproducibility gates passed.
- **Next action:** Update the implementation summary to distinguish completed
  current-code reproducibility from the still-open causal, calibration, and
  workflow-value gates.
- **Likely owner:** Research documentation maintainer.
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
