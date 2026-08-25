# Loose Ends

This is the durable ledger for unresolved project organization, documentation,
planning, ownership, and understanding gaps.

Run the review with `/Loose Ends`, `Loose Ends`, or `Show me my Loose Ends`. The installed skill's formal UI name is `LooseEnds`.

## Status

- Last reviewed: 2026-08-20
- Report state: the current-head S1 build/launch, telemetry validation, and
  schema-7 Forest Edge control plus held-out baseline are reconciled. Remaining
  work is the matched upgrade/results gate and the Noesis analytics privacy
  decision.

## Triage rules

- **P0** — blocks current work, risks data loss, or represents a material contradiction.
- **P1** — likely to cause avoidable rework or leave an active plan ownerless.
- **P2** — useful cleanup, clarification, or follow-up that is not currently blocking.

## Open items

### P1-001 — Forest Edge balance is outside the vertical-slice target

- **Status:** Still open.
- **Evidence:** The current schema-7 20-seed baseline is recorded at
  `artifacts/cellular-experiment-20260820-123724/report.json` (analysis:
  `artifacts/cellular-experiment-20260820-123724/analysis.md`). Hare final
  population is 12–46 (27.15 average), Fox is 0–4 (2.40 average; 2/20 extinct
  final runs), and Plant is 736–931 (840 average). This establishes the
  distribution without promoting a balance target. The held-out control at
  `artifacts/cellular-experiment-20260820-154509/report.json` (analysis:
  `artifacts/cellular-experiment-20260820-154509/analysis.md`) completed all
  five seeds with zero reconciliation mismatches; Hare was 14–27 (20.80
  average), Fox 3–4 (3.40 average), and Plant 831–920 (884.60 average), all
  within the control envelope. The matched schema-8 `faster-movement` arm is
  recorded at `artifacts/cellular-experiment-20260820-160818/report.json`
  (20 seeds) and its held-out check at
  `artifacts/cellular-experiment-20260820-161029/report.json` (5 seeds). The
  paired 20-seed Hare delta was −5.30 average, while the held-out delta was
  +9.60; this sign reversal is descriptive evidence, not a promotion result.
  The predeclared schema-8 `stronger-block-2` diagnostic is recorded at
  `artifacts/cellular-experiment-20260820-222600/report.json` (20 seeds) and
  its held-out check at `artifacts/cellular-experiment-20260820-222705/report.json`
  (5 seeds). It changed Fox 2.40 → 1.85 and Hare 27.15 → 23.25 on the paired
  range, but Fox 3.40 → 2.60 and Hare 20.80 → 26.40 on held-out seeds; the
  direction reverses, so this is descriptive evidence and is not promoted.
- **Next action:** Keep balance changes blocked, investigate the block-relevant
  combat and sign reversal, and choose the next predeclared arm before
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

### P1-003 — Fox eating/action telemetry needs a current Forest Edge report

- **Status:** Resolved for the current baseline report; the preflight prevents
  the prior licensing hang.
- **Evidence:** `SpeciesSimulationMetrics` now distinguishes behavior-decision
  ticks from resolver food-action attempts, successes, and failures. Predation
  and plant feeding both record the action result; focused domain regressions
  cover successful and blocked predation plus plant feeding. Current-head
  EditMode is 142/142 and graphics-capable PlayMode is 6/6 in
  `artifacts/unity-tests-20260820-160709/` and
  `artifacts/visual-evidence-20260820-101101/`. Existing schema-6 EX-002
  artifacts remain historical; new arm output is schema 8 with loadout
  metadata.
- **Next action:** Use the reconciled control and held-out reports, then run
  the single-upgrade arms before changing balance values.
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

- **Status:** Windows development-build and launch smoke are now resolved for
  the bounded shell; the broader Dev Lab split remains follow-up work.
- **Evidence:** `a90bf5f` implements the Main Menu → Lab → Research shell;
  `d5ad141` adds the route smoke test. The Play Mode evidence passes at
  1280×720 (`artifacts/visual-evidence-20260818-210020/PlayMode-results.xml`)
  and 1920×1080
  (`artifacts/visual-evidence-20260818-210108/PlayMode-results.xml`).
  `artifacts/audit-windows-build-current-20260820-101211/` contains the fresh
  successful build and 15-second player startup log; graphics PlayMode is 6/6.
- **Next action:** Move the bounded build/launch evidence through the S1
  verification card; keep broader Dev Lab separation out of this gate.
- **Likely owner:** Josh + Sim.
- **Confidence:** Medium-high.

### P1-006 — Named dual-grid terrain preview awaits validation

- **Status:** Refactored; visual validation remains open.
- **Evidence:** `TerrainTilePreviewWindow` now loads the named `Grass_` and
  `Desert_` sprites from `Assets/Art/Terrain/Standardized/128/` and previews
  all 16 four-corner masks. No runtime screenshot records acceptance yet.
- **Next action:** Run the named dual-grid preview and cellular prototype,
  record visual evidence, and include the refactor in a focused reviewed commit.
### P1-006 — Editor smart-tiling preview path fix

- **Status:** Resolved and evidenced.
- **Evidence:** Commit `771ca50` is pushed. The 16-mask preview artifacts are
  `artifacts/editor-smart-tiling-20260820-023512/grass-16-masks.png` and
  `desert-16-masks.png`, with `mapping.txt` documenting the mask mapping.
- **Next action:** Keep the artifact paths attached to the completed Trello 67
  record; do not reopen this finding without a new visual regression.
- **Likely owner:** Presentation/art owner.
- **Confidence:** High.

### P1-007 — Species sprite fallback fix

- **Status:** Resolved for the graphics-capable runtime path.
- **Evidence:** Commit `024ea86` is pushed. The authoritative graphics-capable
  PlayMode artifact `artifacts/visual-evidence-20260820-025530/PlayMode-results.xml`
  passes 6/6, including the species-presentation coverage. The generic
  nographics run remains 4/6 only because Noesis cannot create native textures
  without a graphics device.
- **Next action:** Preserve the explicit nographics limitation; do not treat it
  as a product regression.
- **Likely owner:** Presentation owner.
- **Confidence:** High.

### P1-008 — Art/presentation runtime acceptance

- **Status:** Resolved for the bounded runtime-art acceptance; product UX polish
  remains a separate open concern.
- **Evidence:** The pushed art/presentation commits are covered by the graphics
  PlayMode 6/6 artifact above and screenshots `01-settings.png` through
  `04-results.png` in `artifacts/visual-evidence-20260820-025530/`.
- **Next action:** Track label overlap and generic reward/results presentation
  under the upgrade/results work; do not reopen this completed art finding.

### P1-012 — Embedded Noesis editor analytics requires a release decision

- **Status:** Open security/privacy decision; not a player-build code path.
- **Evidence:** The embedded vendor package's Editor-only assembly calls
  `GoogleAnalyticsHelper.Install` from `NoesisUpdater` when the package version
  changes. The helper sends a `unity_install` event to Google Analytics and
  contains a committed credential. The Editor asmdef includes only `Editor`, so
  the code is not compiled into the Windows player; the Editor can still make
  the network request during package installation/update.
- **Next action:** Obtain vendor/project approval to disable or update the
  telemetry through a supported package mechanism, then rotate the credential
  if it is genuine. Do not patch the embedded vendor package casually or copy
  the value into project documentation.
- **Likely owner:** Repository maintainer + vendor/license owner.
- **Confidence:** High.

### P1-009 — EX-002 intervention matrix and held-out check

- **Status:** Resolved for the bounded BaselineParity experiment window.
- **Evidence:** Paired schema-6 BaselineParity controls over seeds 10100–10119
  are deterministic:
  `artifacts/cellular-experiment-20260818-210354/report.json` and
  `artifacts/cellular-experiment-20260818-210443/report.json`. All 20 runs
  contain death events; creature death-event counts reconcile with aggregate
  activity. The committed matrix and held-out evidence are recorded in
  `docs/handoffs/2026-08-20-0255-codex-ex002-intervention-matrix.md` with
  distinct fingerprints for both intervention arms. The energy-relief arm
  improves the declared endpoint in both ranges; the predation-relief arm is
  neutral because combat kills are zero and carnivores go extinct.
- **Next action:** Preserve the causal boundary: do not generalize beyond
  BaselineParity, the tested 20-second window, and the declared seed ranges
  without new instrumentation and a new protocol.
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

- **Status:** Resolved; the tool, handoff, commit boundary, and focused tests are
  now recorded.
- **Evidence:** Commit `029f2a7` contains the JSON/CSV converter, editor test
  assembly, package dependency, and README. The current Edit Mode artifact
  `artifacts/unity-tests-20260818-200450/EditMode-results.xml` reports
  `SaltyGame.Editor.Tests.dll` 24/24 passed, including `JsonCsvConverterTests`.
- **Next action:** Leave this out of the active sprint unless a measured,
  evidence-backed tooling outcome is explicitly promoted at sprint review.
- **Likely owner:** Current editor-tool author + repository maintainer.
- **Confidence:** High.

### P2-001 — Discord collaboration proof remains unassigned

- **Status:** On hold for the foreseeable future by product-owner decision on
  2026-08-25; no active work should be assigned.
- **Evidence:** `docs/DISCORD_AGENT_COLLABORATION_TODOS.md` still leaves the
  final channel contract, authenticated read/write proof, handoff automation,
  and safety validation unchecked.
- **Next action:** None until the product owner explicitly reactivates the
  initiative. If reactivated, resolve the contract before any transport proof.
- **Likely owner:** Josh + Sim.
- **Confidence:** High.

### P1-013 — Effective simulation provenance was incomplete

- **Status:** Scenario-data fingerprint fixed locally; execution-option identity
  and durable artifact retention remain open.
- **Evidence:** `CellularSimDataFingerprint` version 5 omitted attack modifier,
  damage amount, maximum energy, and litter bounds. Version 6 now includes all
  current `SpeciesRules` fields and focused inequality coverage. Combat mode,
  attack-opportunity mode, experimental feature ID, and Fox cooldown are runner
  inputs rather than `CellularSimData`; they remain separate report fields and
  must not be described as part of the scenario-data fingerprint. Sampled
  ignored `artifacts/cellular-experiment-*` bundles cited by handoffs are absent
  from this checkout, so their summaries are durable but their raw evidence is
  not independently auditable here.
- **Next action:** Run the focused fingerprint test after Unity is closed. Before
  new ecological experiments, define a combined run-provenance identity or
  manifest that includes execution options, report checksum, source revision,
  scenario GUID/path, command line, seed range, and reconciliation verdict.
- **Likely owner:** Simulation/tooling owner.
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

- **Status:** Mostly resolved; the duplicated guidance line has been removed.
- **Evidence:** `LearningIndieDev/AGENTS.md` now contains one Studio Guidelines
  instruction. The historical 2026-08-14 Play Mode handoff still says full
  execution is pending, but it is retained as history and superseded by newer
  artifacts and handoffs.
- **Next action:** Leave the historical handoff intact; add newer notes when
  status changes rather than rewriting history.
- **Likely owner:** Repository maintainer.
- **Confidence:** High.

### P2-004 — Research paper contains a stale instrument-readiness statement

- **Status:** Resolved; the implementation summary now reflects the accepted
  reproducibility gates.
- **Evidence:** `docs/Research/AI_ASSISTED_ECOLOGY_LAB_RESEARCH_PAPER.md` says
  under “What is implemented so far” that current-code reproducibility is still
  needed, while its later EX-001/EX-001B sections and the accepted experiment
  packages record that the current-code reproducibility gates passed.
- **Next action:** Preserve the remaining causal, calibration, and workflow-
  value boundaries in future research edits.
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
  relay-health tooling are shared. Graphics-capable runtime-art acceptance is
  resolved; remaining label/reward readability work belongs to the upgrade/UI
  lane.
