# Loose Ends

This is the durable ledger for unresolved project organization, documentation,
planning, ownership, and understanding gaps.

Run the review with `/Loose Ends`, `Loose Ends`, or `Show me my Loose Ends`. The installed skill's formal UI name is `LooseEnds`.

## Status

- Last reviewed: 2026-09-12
- Report state: the continuation implementation and evidence-preparation
  checkpoint are recorded in `79423b4e` (with the earlier lifecycle, cleanup,
  and S2-register checkpoints retained in history). Unity EditMode is green at
  212/212. The latest general PlayMode batch passed 21/22 with one intentional
  visual-capture skip and no failures. The retained graphics acceptance remains
  22/22, with GalapagOS desktop and Simulation captures at 1280x720 plus a
  focused 1920x1080 run. The current EX-007/EX-008/EX-009 run bundles and the
  new continuation smoke bundles pass the strict artifact validator with Unity
  logs. P3 is now closed as a
  bounded research phase under `DEC-P3-0001`; EX-003 is explicitly deferred as
  a separate workflow-validity study and broader promotion remains out of
  scope.
  BoardSnapshot fixture repair, the
  terrain documentation contract, the editor pattern drift, XAML whitespace,
  historical ID ambiguity, and the S2 register mapping are resolved or
  explicitly bounded below. EX-010's original and matched alternate sequences
  are now executed and accepted by Josh and Sim; broader promotion review
  remains intentionally out of scope. Roadmap v2 is ready for review, the
  GalapagOS player-shell polish is still a human-review item, and the latest
  focused Settings/Collection PlayMode invocation produced no result XML.

### Decisions recorded this pass

- Workspace cleanliness is intentionally not tracked as a Loose End; ongoing
  uncommitted work is expected in this project.
- The player-facing expedition contract is ten phases. Project-wide wording
  was reconciled to that contract, with no stale phase-count wording found.
- Skip is a valid current choice at each Mutation decision point. It has no
  current bonus or penalty; any future reward-doubling or other Skip incentive
  is a separate deferred economy rule.
- Genome design and implementation are deferred from the M1 closeout. Roadmap
  v2 schedules the first Genome contract in S4 and persistence-backed
  implementation in S6. Named Genome loadouts are deferred and non-blocking,
  and Species Mastery remains deferred and non-gating.
- Provisional scientific-data settlement remains open pending feature-owner
  approval. This pass does not resolve wallet, phase-transfer, or
  extinction-loss semantics.
- Roadmap v2 is the active production baseline: M0 is complete, M1 remains
  active, Sprint 3 is proposed, and S4–S7 are forecast windows. Its proposed
  allocation is not a committed sprint until the S2 review.
- The Noesis image-resource migration and SG-006 guideline are present in the
  current worktree. The Main Menu polish/refinement handoff is **Needs Review**;
  generated art and the new Settings, Species Collection, Expedition Setup,
  and Field Notes concepts remain review candidates, not approved production
  assets.
- The artifact-retention audit and conservative cleanup completed its approved
  removals (about 1.35 GB). Five semantic duplicate bundles remain candidates
  for a later recoverable archive/removal decision.
- Current delivery plans now agree that independent phase windows and the
  bounded EX-010 Stat-Line review are complete. The Windows player smoke also
  passed; CF-6 now retains only the outer ten-phase duration and peak-memory
  measurement.
- Terrain planning now records the runtime's exact eight-neighbor bit order and
  clearly labels `000`, diagonal, and layer semantics as an active art-contract
  decision. The current resolver behavior is not presented as Chrono's final
  delivery contract.
- The docs concept image is canonical at
  `docs/Art Direction/Concepts/GalapagOS_Desktop_UI_Concept_Options_v1.png`;
  the duplicate Unity asset and its `.meta` file were removed.
- The GalapagOS desktop UI follow-up is now tracked by the
  [screens and components ticket](https://trello.com/c/QDjvRK9V/95-galapagos-desktop-ui-screens-and-components).
- Deprecated HUD/debug IMGUI and the orphan Life preview are closed under
  R-015. The low-priority orphan/template cleanup is closed under R-018. The
  terrain diagnostic remains the sole runtime-IMGUI exception and stays open
  under P2-022 while the resumed terrain work determines its long-term role.
- The GalapagOS desktop and Simulation now use separate Noesis compositions;
  the desktop view receives its ViewModel DataContext during startup, and the
  Simulation command no longer falls through to the generic app placeholder.
- The active Lab → CellularAutomataPrototype route now explicitly defaults to
  Forest Edge and Hare. The separate GalapagOSDesktopTest scene remains an
  isolated acceptance surface with its legacy-default sentinel. The Hare data
  asset and Rabbit presentation asset both resolve; the earlier missing-Hare
  report was caused by a measurement wrapper that omitted `-scenarioPath`.
- Continuous state is implemented and canonical under CF-0 through CF-5. Sim
  approved the EX-010 Stat-Line interpretation and target-resolution graphics
  acceptance is complete. The Windows player smoke and corrected scenario run
  are complete; remaining CF work is limited to outer ten-phase
  duration/memory measurement, not lifecycle design.
- The research index, canonical plan, paper, architecture map, experiment index,
  historical DeepResearch treatment, and active feature plan now agree: P0–P3
  are complete within their bounds, P4–P6 are not started, and no next research
  experiment is selected.
- Noesis Editor analytics is an accepted, non-blocking development risk at the
  current project scale; no remediation is tracked unless the project or its
  privacy requirements materially expand.
- P1-014 graphics acceptance, P1-022's terrain-contract contradiction, and the
  obsolete P2-023 Figma continuation are closed under R-023. The live pilot-
  named Lab resource dictionary remains in use and was not removed.
- The unreferenced recovery scene, starter `Intro` scene, unreachable EX-002
  generator, and obsolete empty placeholder trees are closed under R-024.
  Deliberate empty ownership boundaries remain documented and retained.

## Triage rules

- **P0** — blocks current work, risks data loss, or represents a material contradiction.
- **P1** — likely to cause avoidable rework or leave an active plan ownerless.
- **P2** — useful cleanup, clarification, or follow-up that is not currently blocking.

## Current open items (2026-09-09)

### P1-016 — First trustworthy upgrade catalog needs design and balance review

- **Status:** Catalog and authoring path are complete; the bounded EX-007
  decision is accepted, while player readability and balance/promotion review
  remain open. This item now absorbs the overlapping former P1-018 finding.
- **Evidence:** `docs/NEXT_WORK_BUCKET_PLAN.md` records seven authored
  production assets, the catalog validator, snapshot adapter, the completed
  EX-009 same-seed check, and the bounded EX-007 decision. The remaining
  questions are design balance, player readability, and any follow-up needed
  before promotion. EX-007/EX-008 effect direction and size vary by panel;
  EX-009's zero-delta result is bounded implementation evidence, not balance.
- **Next action:** Approve the Forest Edge reference panel and name the first
  balance/playtest follow-up. Keep all claims bounded to their scenario, values,
  telemetry, and seed panels; keep permanent Genome work separate.
- **Likely owner:** Josh.
- **Confidence:** High.

### P1-017 — Historical worker bundles remain incomplete

- **Status:** Packaging contract fix is implemented; historical bundles remain
  incomplete, while the current EX-007/EX-008 local run bundles are complete and
  pass the strict validator with Unity logs.
- **Evidence:** Matched 100-seed Forest Edge artifacts are present under
  `automation/CellSimQueue/Completed/`: baseline
  `20260831-234216-ec3350ed` (Fox 2.94 average, Hare 21.23 average, Plant
  879.73 average; `report.csv` and `statline.csv` present) and Escape Artist
  `20260831-234200-d484a2b2` (Fox 2.91, Hare 23.06, Plant 866.13; expected
  CSV/statline files absent). Both manifests say `sourceTreeDirty: true`, while
  their queue records say the worker was clean before and after execution. The
  new worker contract now captures explicit before/after source-tree state,
  canonicalizes report hashing across Git line endings, copies `unity.log`,
  verifies `reportSha256`, and refuses to publish an incomplete bundle. The
  read-only `tools/Test-CellSimArtifactBundle.ps1` validator reports the old
  baseline as valid-with-warnings and the old Escape Artist arm as invalid for
  missing CSV/statline files.
- **Next action:** Preserve compact summaries and provenance for the current
  valid bundles, keep the historical invalid/incomplete bundles clearly
  excluded, and keep the bounded P3 decision linked to the accepted evidence.
  The detached worker must receive the latest lifecycle tooling before another
  remote run. Do not use the old diagnostic pair for a new claim.
- **Likely owner:** Simulation/tooling owner.
- **Confidence:** High.

### P1-026 — Remote worker branch lacks the latest Unity lifecycle tooling

- **Status:** The cleanup change is present in the current tree; the remote
  `origin/codex/cellsim-worker` branch still has a different
  `UnityTooling.ps1`. There is no local worker branch currently checked out.
- **Evidence:** `docs/handoffs/2026-09-03-1130-codex-process-lifecycle-cleanup.md`
  explicitly requires propagation before the next remote worker run. On
  2026-09-07, the current script hashed to
  `4e9a0ae4b3f0c986b586829b90ada58ebe76b36a`, while the copy on
  `origin/codex/cellsim-worker` hashed to
  `f14e6499be6de9ad07b2582c3737b26dd719b92e`.
- **Next action:** Commit the lifecycle cleanup and propagate it to the worker
  branch, then verify worker-side process cleanup before another run.
- **Likely owner:** Simulation/tooling owner.
- **Confidence:** High.

### P1-027 — Sprint 2 task register is not uniquely mapped to board work

- **Status:** Mostly resolved; every row now has a stable ID and board link,
  and S2.2A's repository and Trello status are synchronized. Remaining
  owner/status reconciliation, including the Fox telemetry card formerly
  tracked as P1-028, is still for the sprint review.
- **Evidence:** `docs/NEXT_WORK_BUCKET_PLAN.md` and
  `docs/Sprints/S2-control-record.md` now assign stable IDs and verified Trello
  links to every listed work row. The duplicate board prefixes are disambiguated
  as `S2.2A/B` and `S2.3A/B`, and the Fox/BoardSnapshot reserve cards have
  explicit `S2-CORR-*` IDs. The S2.2A card is now in `✅ Done` with the
  repository's V1 acceptance scope and evidence. The board currently assigns
  the parallel S2.3A and
  S2-QA cards to Sim while the repository plan assigns the implementation lane
  to Josh; that discrepancy is recorded rather than hidden. The latest board
  pass places completed CF-1, S2.3B, and EX-010 execution in `✅ Done`; the
  board's EX-010 schedule/approval label still predates the completed run and
  needs synchronization. Sim's
  active telemetry lanes remain in `Current Work`. Current code exposes the Fox
  reproduction and food-action fields, but the card still needs owner review.
- **Next action:** Reconcile card ownership, list placement, and completion
  status during the S2 review, including closing or carrying the Fox telemetry
  card; review the proposed S3 allocation in `ROADMAP.md` and
  `docs/Sprints/S3-control-record.md`; do not infer completion or commitment
  from the control card.
- **Likely owner:** Josh + Sim.
- **Confidence:** High.

### P1-029 — Mutation/Genome contract still has player-facing decisions open

- **Status:** Direction is recorded. Genome design and implementation are
  deferred from the M1 closeout; Roadmap v2 schedules the first contract in S4
  and persistence-backed implementation in S6. The detailed node, profile,
  persistence, and economy contract stays open.
- **Evidence:** The current model applies natural rules, then a permanent
  per-species Genome, then ordered temporary expedition Mutations. Skip is a
  valid current choice with no bonus or penalty. The active planning baseline
  is 8 points per species, reallocated freely between simulations and frozen at
  launch; node costs and capacity growth remain open. Named Genome loadouts are
  deferred/non-blocking, and Species Mastery is deferred/non-gating. The
  [balance guideline handoff](handoffs/2026-09-06-1523-codex-mutation-genome-balance-guideline.md)
  remains historical; the Forest Edge reference panel and first one-time
  effect contract still need their own review. Provisional settlement remains
  open pending feature-owner approval.
- **Next action:** At the S4 contract review, start with the profile/launch
  snapshot contract and keep named loadouts and Mastery out of the gate. Hold
  persistence-backed implementation for S6 unless the approved roadmap changes.
  Before wallet or permanent-purchase work, obtain owner approval for
  provisional settlement and the remaining node/economy rules. Do not infer a
  contract from the existing upgrade shim.
- **Likely owner:** Josh + design/simulation owners.
- **Confidence:** High.

### P1-030 — CF-6 production build and performance evidence is not fully closed

- **Status:** The Windows player smoke passed, and the corrected ten-phase
  Forest Edge/Hare run completed. The outer duration and peak-memory sampling
  still needs one valid rerun; two earlier performance bundles are invalid
  because their wrapper omitted `-scenarioPath`.
- **Evidence:** The successful player smoke is recorded under
  `artifacts/windows-build-20260908-065954/`. The corrected scenario report is
  `artifacts/cellular-experiment-20260908-123559/report.json`, which records
  `ForestEdge`, `hare`, ten phases, and 2,000 total ticks. The invalid wrapper
  records remain under `artifacts/ten-phase-performance-20260908-070937/` and
  `artifacts/ten-phase-performance-20260908-071652/` and must not be used as
  Forest Edge evidence.
- **Next action:** Rerun the performance wrapper with the explicit scenario
  path and capture wall duration, peak working set, and peak private memory.
  Keep the invalid runs preserved as operational history but exclude them from
  conclusions.
- **Likely owner:** Simulation/tooling owner.
- **Confidence:** High.

### P1-031 — Player-shell polish and focused UI acceptance need review

- **Status:** The Main Menu polish/refinement pass is implemented in the
  current worktree and verified interactively, but its handoff is **Needs
  Review**. The broader Desktop/Simulation/Lab XAML and ViewModel changes are
  also not yet an accepted production-route checkpoint.
- **Evidence:**
  [`2026-09-09-codex-main-menu-polish-first-pass.md`](handoffs/2026-09-09-codex-main-menu-polish-first-pass.md)
  records the meadow background, CRT treatments, focus behavior, procedural
  chime, and zero-warning interactive checks. The focused
  `settings-collection-ui-20260912-010749` Unity invocation exited with code 1
  and produced no `results.xml`, so it cannot establish PlayMode acceptance.
- **Next action:** Josh/UI reviewer should approve or revise the Main Menu
  title/brand and generated-art direction, then rerun the focused Settings and
  Species Collection acceptance with a captured result file before promoting
  the current UI worktree.
- **Likely owner:** Josh + UI/art reviewer.
- **Confidence:** High.

### P2-005 — Large raw worker artifacts need a retention policy

- **Status:** Audit and one conservative cleanup pass are complete; the
  retention policy and next recoverable archive/removal decision remain open.
- **Evidence:** The
  [`2026-09-09-artifact-retention-audit.md`](handoffs/2026-09-09-artifact-retention-audit.md)
  inventory covered 335 artifact directories and 5.95 GB of data, summarized
  all 72 CellSim reports, and identified five semantic duplicate candidates.
  The companion cleanup handoff records 41 exact, hash-verified removals and
  about 1.35 GB reclaimed while retaining NUnit XML, cited evidence, summaries,
  parse-warning reports, and current-day logs.
- **Next action:** Review the five candidates, move any approved bundles to a
  recoverable archive, rerun the summarizer and DirtyBoy, and only then remove
  them. Keep raw evidence for cited or unresolved reports.
- **Likely owner:** Repository maintainer + tooling owner.
- **Confidence:** High.

## Deferred hygiene candidates — originally audited 2026-09-03

These are newly recorded removal, archival, documentation, and refactor
candidates from the project hygiene review. They are triage markers, not
authorization to delete or rename Unity assets. Any serialized asset change
still requires a focused Unity validation pass and preservation of its `.meta`
file/GUID.

Ticket summaries for these items are recorded in
[`PROJECT_HYGIENE_TICKET_SUMMARIES.md`](PROJECT_HYGIENE_TICKET_SUMMARIES.md).

### P2-007 — Legacy `SpeciesArchetype` compatibility surface is not retired

- **Status:** Active compatibility debt; not safe to remove yet.
- **Evidence:** `Assets/Scripts/Game/Species/SpeciesArchetype.cs:5-11` is marked
  obsolete, but `SpeciesId.cs:57-104`, `SpeciesCell.cs:218-219`,
  `SpeciesDefinition.cs:14-15`, and `SpeciesRules.cs:193-196` still expose
  conversions or legacy properties. `Assets/Tests/Runtime/SpeciesDomainTests.cs`
  still contains extensive `SpeciesArchetype`-based fixtures and assertions.
- **Next action:** Migrate remaining production/test callsites to `SpeciesId`
  and ID-keyed collections, add a zero-reference/compile gate, then remove the
  enum, implicit conversion, legacy properties, and overloads in a separate
  breaking cleanup. Until then, keep the shim and do not “simplify” it locally.
- **Likely owner:** Sim/domain owner.
- **Confidence:** High.

### P2-022 — Temporary terrain diagnostic still uses runtime IMGUI

- **Status:** The deprecated Island Survivor HUD/debug IMGUI and orphan Life
  preview were removed. `TerrainPaintPreview` remains as a separate manual
  diagnostic scene while the terrain delivery-readiness work is active.
- **Evidence:** `Assets/Scripts/Game/Presentation/TerrainPaintPreview.cs` still
  implements `OnGUI`/`GUILayout`, and `Assets/Scenes/TerrainPaintTest.unity`
  serializes that component. The editor terrain preview is a separate
  editor-only utility.
- **Next action:** After the terrain asset contract and presentation workflow
  settle, either retain this as a clearly bounded developer diagnostic, migrate
  it to Noesis, or explicitly remove the scene, script, and focused helper test.
  Do not treat the diagnostic as player-facing UI.
- **Likely owner:** Presentation/art owner + Josh.
- **Confidence:** High.

### P2-014 — `MainMenu_Old` is active but now misleadingly named

- **Status:** Refactor/rename candidate; definitely not a deletion candidate.
- **Evidence:** `Assets/Scenes/MainMenu.unity` serializes the GUIDs for
  `MainMenu_Old/VM_MainMenu.cs`, `MainMenuNoesisHost.cs`, and
  `V_Panel_MainMenu.xaml`; `MainMenuPlayModeTests` also requires the active
  `VM_MainMenu` type. The current architecture names this the Main Menu
  contract, so the directory name now implies retirement incorrectly.
- **Next action:** After the current UI feature set stabilizes, perform a Unity
  Editor folder migration or equivalent GUID-preserving rename, update
  documentation and tests, and validate the scene. Do not rename the folder
  from the filesystem as a casual cleanup.
- **Likely owner:** UI owner.
- **Confidence:** High.

### P2-015 — Several core files have accumulated multiple responsibilities

- **Status:** Deferred staged-refactor candidate; current behavior is covered
  and should remain stable while evidence gates are open.
- **Evidence:** Current file sizes are approximately 2,851 lines for
  `SpeciesSimulation.cs`, 1,183 for `SpeciesSimulationMetrics.cs`, 1,335 for
  `SpeciesSimulationPreview.cs`, 1,278 for `VM_SimulationShell.cs`, 1,185 for
  `CellularSimulationExperimentRunner.cs`, and 3,576 for
  `SpeciesDomainTests.cs`. These combine distinct concerns such as simulation
  phases, telemetry/statline calculation, presentation/settings persistence,
  XAML shell orchestration, CLI/report serialization, and broad behavior
  fixtures.
- **Next action:** P3 and baseline graphics acceptance are green, but do not
  launch a broad refactor. When a named file blocks current M1 work, choose one
  seam at a time: separate metrics DTOs from accumulation, extract presentation
  persistence/formatting, isolate report writers, or partition tests by
  behavior. Preserve public/serialized names until focused tests and Unity
  validation support each extraction.
- **Likely owner:** Sim + UI/tooling owners.
- **Confidence:** High for complexity; medium for the exact split.

## Historical open items — 2026-08-20

The entries below are retained as historical evidence from the prior review.
Use the current section above for active triage; do not infer current status
from an older entry without checking its cited artifacts.

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
### P1-006b — Editor smart-tiling preview path fix

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

- **Status:** Scenario-data and run-provenance fingerprints are fixed locally;
  durable artifact retention and Unity validation remain open.
- **Evidence:** `CellularSimDataFingerprint` version 5 omitted attack modifier,
  damage amount, maximum energy, and litter bounds. Version 6 now includes all
  current `SpeciesRules` fields and focused inequality coverage. Combat mode,
  attack-opportunity mode, experimental feature ID, and Fox cooldown are runner
  inputs rather than `CellularSimData`; they remain separate report fields and
  must not be described as part of the scenario-data fingerprint. Run-provenance
  fingerprint v1, report schema 18, checksum/source manifests, and paired-runner
  orchestration coverage are now implemented locally. Sampled ignored
  `artifacts/cellular-experiment-*` bundles cited by handoffs are absent from
  this checkout, so their summaries are durable but their raw evidence is not
  independently auditable here.
- **Next action:** Run the focused fingerprint/paired-runner tests and full
  suites after the current UPM/licensing IPC failure is repaired. Retain
  complete experiment directories so the report, checksum/source manifest, CSV,
  and log remain independently auditable together.
- **Likely owner:** Simulation/tooling owner.
- **Confidence:** High.

#### 2026-08-25 historical continuation

Run-provenance fingerprint v1, report schema 18, and the sibling checksum/source
manifest are implemented locally. Direct paired-runner orchestration coverage is
also present. Unity is closed, but validation is now blocked by the absent local
entitlement file; these changes must remain unaccepted until the focused and full
Unity suites run successfully.

#### 2026-09-03 current continuation

The local entitlement file is present; current validation is blocked by the
machine-level UPM/licensing IPC handshake rather than a missing entitlement.

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
- **Evidence:** `docs/Research/AI_ASSISTED_ECOLOGY_LAB_RESEARCH_PAPER.md:236-262`
  now states that the authored Forest Edge current-code reproducibility result
  is accepted; the later EX-001/EX-001B sections retain the bounded scope and
  the accepted experiment packages provide the supporting records.
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

### R-008 — GalapagOS ControlLibrary batch is committed and pushed

- **Evidence:** Commit `640d8f5d` on `UI/ControlLibrary` adds the desktop test
  scene, shared resource-driven window template, four palette variants, and the
  desktop-panel cleanup. The companion handoff is
  `docs/handoffs/2026-09-02-codex-control-library-and-loose-ends.md`.
- **Result:** The source change has a focused commit boundary and is available
  on `origin/UI/ControlLibrary`. Runtime visual acceptance is closed under
  P1-014; the normal-host preflight gate is also resolved.

### R-009 — CellSim evidence-quality gate is implemented

- **Evidence:** Commit `f315d3d6` on `UI/ControlLibrary` and matching worker
  commit `03011357` on `codex/cellsim-worker` add canonical report hashing,
  explicit source-state provenance, strict completed-bundle packaging, and the
  read-only `tools/Test-CellSimArtifactBundle.ps1` validator. The wrapper fix in
  the follow-up commit prevents successful Git worktree progress from being
  misclassified as a submission failure.
- **Result:** Historical artifacts were not rewritten. Two identical 20-seed
  Hare diagnostic jobs (`20260902-233024-d8d75c20` and
  `20260902-233045-437566fd`) failed before simulation and are not P3 evidence
  because they used the default temporary scenario and legacy combat mode. The
  corrected EX-007 training job (`20260903-033218-3b7607ba`) also failed before
  simulation when Unity preflight found an active editor; the corrected
  held-out job (`20260903-033240-b1b43c58`) remains pending. No upgrade or
  balance claim is promoted until the P3 gate completes.

### R-010 — Detached worker publication path verified

- **Evidence:** Commit `b52208f3` on `codex/cellsim-worker` changes result
  publication to push `HEAD:refs/heads/codex/cellsim-worker`. The isolated pass
  published three failure records (`5a11fe37`, `de45d803`, `ba92cb03`) instead
  of silently losing queue state.
- **Result:** Queue state is now auditable even when Unity preflight blocks a
  run. The remaining blocker is machine state, not worker publication.

### R-011 — EX-009 same-held-out upgrade-order comparison completed

- **Former item:** P1-025.
- **Evidence:** Both adapter-backed arms passed validation on seeds 106–110:
  `artifacts/cellular-experiment-20260904-192559` and
  `artifacts/cellular-experiment-20260904-192703`. The paired table in the
  EX-009 package records zero deltas across all compared outcome and telemetry
  fields.
- **Result:** The locked order question is resolved. Add a focused
  commutativity regression test and reopen an order-specific experiment only if
  future upgrades introduce stateful or non-additive behavior. Historical
  preflight failures remain preserved as operational records.

### R-012 — Current standards documentation reconciled

- **Former item:** P1-023.
- **Evidence:** `FRAMEWORK.md:3-6,47-49` now labels the Bootstrap/Island
  Survivor guidance historical and identifies the active Main Menu → Lab →
  CellularAutomataPrototype flow. `docs/UNITY_STANDARDS_ADOPTION_PLAN.md` and
  `docs/UNITY_ENGINEERING_STANDARDS.md` now match the current scene, Play Mode,
  starter-`BaseViewModel`, and direct Noesis import facts; the historical
  Island validator remains explicitly scoped to the deprecated prototype.
- **Result:** The documentation drift is resolved without changing scenes,
  packages, or runtime code. Future Unity visual/test acceptance remains tracked
  by the UI and preflight items rather than this documentation closure.

### R-013 — Low-risk checkpoint and hygiene items resolved

- **Former items:** P2-006, P1-024, P2-016, P2-019, and P2-020.
- **Evidence:** Commits `1d345a93`, `745e630b`, and `824444fe` record the S2
  control/plan checkpoint, continuation implementation, editor pattern-default
  reuse, XAML whitespace cleanup, and distinct historical terrain-preview IDs.
  The current branch is synchronized with `origin/NF/ConsecutiveRuns` and the
  working tree contains only the separately tracked continuation edits.
- **Result:** These bookkeeping and low-risk cleanup findings are closed. S2
  board ownership follow-up remains active under P1-027; no historical evidence
  was deleted.

### R-014 — BoardSnapshot fixture repair resolved

- **Former scope:** The resolved half of P1-028.
- **Evidence:** The [BoardSnapshot card](https://trello.com/c/Cy2TOMOh) is in
  `✅ Done`; `SimulationManagerTests.BoardSnapshotCopiesCellsAndSpeciesRoles`
  uses the correct `(x, y)` fixture and was included in the 187/187 Edit Mode
  artifact recorded in the 2026-09-03 upgrade handoff.
- **Result:** The fixture repair is closed. Fox telemetry review remains active
  under P1-028.

### R-015 — Runtime IMGUI HUD/debug and orphan Life preview removed

- **Evidence:** `GameHud.cs`, `RuntimeDebugPanel.cs`, and their `.meta` files
  were removed from the deprecated Island Survivor runtime; `GameRuntime` no
  longer adds those components or handles the F3 debug toggle. The unreferenced
  `LifeSimulationPreview.cs` and `.meta` were also removed. The active
  cellular simulation UI remains on the Noesis/XAML path.
- **Result:** The former player/runtime HUD and debug IMGUI paths are retired.
  Editor-only utility windows remain separate; `TerrainPaintPreview` is still a
  diagnostic runtime scene and keeps P2-022 open until its explicit removal or
  Noesis migration is approved.

### R-016 — Unity normal-host preflight gate resolved

- **Evidence:** `artifacts/unity-preflight-20260905-131655` records a successful
  licensing context probe, a stable licensing handshake, Package Manager IPC,
  registration of 65 packages, and a clean batchmode exit. A later elevated
  attempt was stopped by the existing Unity editor guard, not by a permission
  failure.
- **Result:** The restricted-host-permission issue is closed. Graphics runtime
  acceptance is closed separately under P1-014.

### R-017 — EX-010 sequential continuation comparison resolved

- **Former item:** P2-021.
- **Evidence:** The original and matched reverse-order sequences completed on
  2026-09-06 using development seeds 1–20 and held-out seeds 106–110. All ten
  200-tick phases completed, both bundles and the EX-010 validators are
  `VALID`, and the paired report records a bounded order effect without a
  general ranking. See `docs/Research/Experiments/EX-010-Sequential-Upgrade-Continuation/REPORT.md`.
- **Result:** Execution, comparison, and Sim's interpretation review are
  resolved. The bounded finding is recorded in `HUMAN_DECISION.md`; broader
  promotion remains intentionally outside this item.

### R-019 — P3 bounded research gate resolved

- **Evidence:** `docs/Research/P3_GATE_DECISION_2026-09-06.md`, the accepted
  EX-007 and EX-009 decisions, and the accepted EX-010 decision.
- **Result:** P3 is closed with revised bounded exit criteria. EX-003 is
  archived/deferred because it has no execution package; no generalized AI
  recommendation or production-balance claim is promoted.

### R-020 — EX-010 research status synchronized

- **Evidence:** The EX-010 execution report, human decision, research plan,
  continuation evidence review, documentation audit, Working State, and next
  work plan now agree that the experiment is executed and accepted.
- **Result:** The stale “prepared/unexecuted” status is closed. Historical gate
  snapshots remain unchanged and are still labeled as historical.

### R-021 — Continuous-flow and research status documentation synchronized

- **Evidence:** `docs/PROJECT_CONTEXT.md`,
  `docs/CONTINUOUS_SIMULATION_FLOW_PLAN.md`, the canonical research plan and
  paper, both research indexes, the P3 architecture map, the EX-010 Sim brief,
  the historical DeepResearch treatment, and
  `docs/INCOMPLETE_FEATURES_ACTION_PLAN.md` were reconciled on 2026-09-07.
- **Result:** Continuous state and CF-0 through CF-5 are recorded as implemented;
  Sim's semantic review is complete; P3 is closed as bounded; P4–P6 are not
  started; historical snapshots are explicitly labeled; the missing EX-002 raw
  controls are disclosed. No new research or balance claim was promoted.

### R-022 — Noesis Editor analytics risk accepted at current scale

- **Former items:** P1-019 and the historical duplicate P1-012.
- **Evidence:** The embedded Noesis Editor assembly can send a Google Analytics
  `unity_install` event when the package version changes. The assembly is
  Editor-only and is not part of the Windows player; the request contains
  development-environment metadata rather than gameplay state.
- **Result:** Josh accepted this as a non-blocking development risk for the
  current project scale. No disable, package patch, or credential remediation
  is planned. Reopen only if the project scales up or its privacy/security
  requirements change.

### R-023 — Graphics, terrain-contract, pilot, and duplicate-ledger cleanup

- **Former items:** P1-014, P1-022, P2-023, plus duplicate tracking entries
  P1-018 and P1-028.
- **Evidence:** The 2026-09-07 GalapagOS graphics handoff records 22/22 PlayMode
  acceptance at 1280×720 and a focused 1920×1080 pass. Terrain code, tests,
  preview paths, and active documentation agree on 47 valid normalized masks;
  the diagnostic presentation remains tracked under P2-022 while the resumed
  terrain work resolves the asset contract. The dark
  Figma/Noesis pilot is explicitly superseded by the pastel GalapagOS direction,
  while its live Lab resource dictionary is retained. P1-018's balance boundary
  is now part of P1-016, and P1-028's board review is now part of P1-027.
- **Result:** Closed gates and duplicate records no longer appear as current
  work. No live XAML resource, Unity asset, historical evidence, or unresolved
  balance/board action was deleted or misclassified.

### R-024 — Stale scenes, one-shot generator, and empty shells removed

- **Former items:** P2-012, P2-013, P2-017, and P2-018.
- **Evidence:** Final name, GUID, source-reference, and Build Settings checks
  found no consumer for `_Recovery/0.unity` or the untouched camera/light
  `Intro.unity` starter scene. `PrepareEx002Variants` had no menu attribute or
  source callsite; its four generated assets remain versioned, and
  `EX-002-MATRIX-PROTOCOL.md` preserves their paths and exact rule changes.
  Empty placeholder folder GUIDs had no serialized consumers.
- **Result:** The two scenes, unreachable generator, obsolete `Assets/Project`
  planning tree, retired UI shells, stale `Resources/CellularArt` shell, and
  other ownerless empty placeholders were removed with their matching `.meta`
  files. Deliberate empty boundaries for audio, materials, third-party content,
  plant art, and runtime diagnostics remain. All removals are recoverable from
  Git history.

### R-025 — Production simulation default is explicitly Forest Edge/Hare

- **Former scope:** The ambiguous authored-scenario default discovered during
  CF-6 acceptance.
- **Evidence:** `Assets/Scenes/CellularAutomataPrototype.unity` now selects
  scenario option `0` (`ForestEdge`) and serializes `playerSpeciesKey: hare`.
  The Lab launch contract already requests `ForestEdge` and `hare`, while the
  corrected batch run loaded the same scenario and species successfully. The
  focused graphics-capable PlayMode check passed in
  `artifacts/visual-evidence-20260908-161521/`.
- **Result:** The active Lab → Simulation route no longer falls back to the
  generic `plant`/`herbivore`/`carnivore` defaults. `GalapagOSDesktopTest`
  remains unchanged as an isolated acceptance scene, and `OpenRange` remains
  an explicit Deer/Wolf option rather than an implicit default.

### R-026 — Current delivery-document drift reconciled

- **Evidence:** `INCOMPLETE_FEATURES_ACTION_PLAN.md` now records the runtime's
  eight-neighbor mask order without treating unsettled terrain semantics as a
  final art contract. `CONTINUOUS_SIMULATION_EVIDENCE_IMPACT.md`,
  `CONTINUOUS_SIMULATION_FLOW_PLAN.md`, and `GAME_FEATURE_ROADMAP_TRIAGE.md`
  now agree with the implemented phase windows, accepted EX-010 Stat-Line
  meaning, completed Windows smoke, and remaining CF-6 measurement. The
  superseded GalapagOS direction draft now labels its old gate and decisions as
  historical.
- **Result:** Current planning no longer reopens completed simulation or visual
  gates. The unresolved terrain-delivery questions remain assigned to their
  dedicated task instead of being guessed into the documentation.

### R-018 — Low-priority orphan and template cleanup resolved

- **Evidence:** After repository-wide name/GUID scans found no active scene,
  prefab, asset, or code dependency, the orphan `CavePreview` script/meta,
  copied starter `BaseViewModel` and four `TestUI.xaml` pairs, the unreferenced
  `Assets/UI/EcoSim` placeholder tree, and Unity's URP `Readme`/`TutorialInfo`
  template tree were removed together with their matching `.meta` files.
  Deterministic cave/Life domain code, active Main Menu/Lab/GalapagOS UI, and
  research artifacts were retained.
- **Result:** P2-008, P2-009, P2-010, and P2-011 are closed. A full Unity
  graphics PlayMode pass later completed 22/22 on 2026-09-07, providing the
  normal post-cleanup import/scene check for the covered UI paths.
