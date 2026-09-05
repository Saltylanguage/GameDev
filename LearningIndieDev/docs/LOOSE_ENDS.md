# Loose Ends

This is the durable ledger for unresolved project organization, documentation,
planning, ownership, and understanding gaps.

Run the review with `/Loose Ends`, `Loose Ends`, or `Show me my Loose Ends`. The installed skill's formal UI name is `LooseEnds`.

## Status

- Last reviewed: 2026-09-05
- Report state: the current continuation implementation is checkpointed at
  `1d345a93`; the low-risk cleanup is checkpointed at `745e630b` and the S2
  register mapping at `824444fe`. The current
  EX-007/EX-008/EX-009 run bundles pass the strict artifact validator with Unity
  logs. Graphics-capable Unity acceptance remains blocked by Unity's
  machine-level UPM/licensing IPC state; the P3 research gate remains open
  because EX-003 and broader promotion review are unresolved. BoardSnapshot
  fixture repair, the terrain documentation contract, the editor pattern drift,
  XAML whitespace, historical ID ambiguity, and the S2 register mapping are
  resolved or explicitly bounded below. EX-010 remains the future
  sequential-continuation question.

## Triage rules

- **P0** — blocks current work, risks data loss, or represents a material contradiction.
- **P1** — likely to cause avoidable rework or leave an active plan ownerless.
- **P2** — useful cleanup, clarification, or follow-up that is not currently blocking.

## Current open items (2026-09-05)

### P1-014 — GalapagOS ControlLibrary runtime acceptance is blocked

- **Status:** Earlier UI batch committed/pushed; follow-up panel/resource/font
  edits are checkpointed locally; runtime acceptance remains pending.
- **Evidence:** Commit `640d8f5d` adds the desktop test scene, shared
  `HeaderedContentControl` window style/resources, four palette variants, and
  the cleaned desktop panel on `UI/ControlLibrary`. XML parsing succeeds for
  the changed XAML. `Assets/Noesis.settings.asset` now points `defaultFont` at
  the imported Pixeloid Sans Noesis font, and the Pixeloid Sans/Bold/Mono `.meta`
  files use `Noesis.NoesisGUI.Editor::NoesisFontImporter`, so Noesis/XAML text
  inherits the project font without per-element assignments. `git diff --check`
  is clean for this follow-up. No Unity screenshot or graphics Play Mode result
  exists for this batch yet.
- **Next action:** Repair the Unity Hub/licensing IPC state, rerun the graphics
  acceptance command, and review the desktop scene at the target resolutions.
- **Likely owner:** Josh + UI owner.
- **Confidence:** High.

### P1-015 — Unity preflight requires a normal host-permission context

- **Status:** Mitigated; preflight passes when launched with normal host
  permissions. Restricted Codex shells now fail fast with an actionable
  diagnostic instead of launching Unity into a doomed IPC timeout.
- **Evidence:** Normal-permission preflight passed in
  `artifacts/unity-preflight-20260903-181807`: licensing and UPM connected and
  all 65 packages registered. Restricted probes reported Windows Management
  access denied for the licensing client's host-identity queries.
- **Tooling follow-up:** `tools/UnityTooling.ps1` now performs the context
  probe first and cleans only newly created UPM/licensing helpers for ten
  seconds after Unity exits; late-spawned clients cannot survive a failed run.
- **Next action:** Run Unity validation from a normal host-permission terminal,
  or approve the elevated Unity preflight when invoking it through Codex. Do
  not bypass the preflight gate.
- **Likely owner:** Josh (host permissions) + tooling owner.
- **Confidence:** High.

### P1-016 — First trustworthy upgrade catalog needs balance review

- **Status:** Catalog and authoring path are complete; balance/promotion review
  remains open.
- **Evidence:** `docs/NEXT_WORK_BUCKET_PLAN.md` records seven authored
  production assets, the catalog validator, snapshot adapter, and the completed
  EX-009 same-seed check. The remaining questions are design balance, player
  readability, and whether the EX-007 evidence should be accepted or revised.
- **Next action:** Record the EX-007 human decision and name any balance or
  playtest follow-up; keep Lab/permanent upgrades out of this slice.
- **Likely owner:** Josh.
- **Confidence:** High.

### P1-017 — Worker result packaging and provenance disagree

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
  excluded, and decide which current bundles qualify as P3 evidence. The
  detached worker must receive the latest lifecycle tooling before another
  remote run. Do not use the old diagnostic pair for the P3 gate.
- **Likely owner:** Simulation/tooling owner.
- **Confidence:** High.

### P1-018 — Upgrade effect evidence is descriptive, not promotable

- **Status:** Open; no balance or production claim should be made yet.
- **Evidence:** EX-007/EX-008 effect direction and size still vary by seed
  panel, and the EX-007 human decision accepts only a bounded model-scoped
  finding. EX-009 provides a clean same-held-out order result: the two current
  additive upgrades matched on all five pairs. That is a bounded implementation
  result, not a balance result.
- **Next action:** Keep all balance or interaction claims bounded to the
  validated scenario, values, telemetry, and seed panels. Revisit the proposed
  EX-010 sequential-continuation study only after active-run checkpoint/resume
  exists.
- **Likely owner:** Josh.
- **Confidence:** High.

### P1-019 — Embedded Noesis editor analytics requires a release decision

- **Status:** Open security/privacy decision; not a player-build path.
- **Evidence:** The vendor Editor-only assembly installs a Google Analytics
  `unity_install` event on package version changes and contains a committed
  credential. It is excluded from the Windows player but can make a network
  request in the Editor during installation/update.
- **Next action:** Obtain vendor/project approval for a supported disable/update
  path and rotate the credential if genuine. Do not patch the embedded package
  casually.
- **Likely owner:** Repository maintainer + vendor/license owner.
- **Confidence:** High.

### P1-020 — Figma/Noesis pilot still lacks final visual evidence

- **Status:** Open and intentionally separate from the ControlLibrary commit.
- **Evidence:** `docs/handoffs/2026-08-25-codex-figma-noesis-pilot.md` records the
  pilot as incomplete because the Figma Starter quota was exhausted and the
  component/screenshot pass was not accepted. The new desktop scene has no
  runtime screenshot yet because of P1-015.
- **Next action:** Revisit the pilot only after runtime acceptance is available
  and the visual target is explicit; do not expand the component surface first.
- **Likely owner:** Presentation/UI owner.
- **Confidence:** Medium-high.

### P2-005 — Large raw worker artifacts need a retention policy

- **Status:** Open cleanup/operations concern.
- **Evidence:** The two latest 100-seed JSON reports are approximately 52.2 MB
  and 53.1 MB, with the completed queue holding roughly 122 MB of raw output.
  The current `artifacts/` directory is ignored and has no Git-tracked files;
  current EX-007/EX-008 reports therefore depend on local raw bundles.
- **Next action:** Keep compact committed summaries, manifests, and paired
  deltas; define when raw JSON/log bundles move to an external archive or are
  pruned. Do not delete the current local evidence before that policy exists.
- **Likely owner:** Repository maintainer + tooling owner.
- **Confidence:** High.

### P2-006 — Sprint rollover and S2 control record need a fresh checkpoint

- **Status:** Resolved for the repository checkpoint; S2 remains active.
- **Evidence:** Commit `1d345a93` includes the durable
  `docs/Sprints/S2-control-record.md` and `docs/NEXT_WORK_BUCKET_PLAN.md` for
  the September 3–16 window. `docs/Sprints/S1-control-record.md` remains the
  historical August 17–30 record.
- **Next action:** Keep the board mapping synchronized during the S2 review;
  that follow-up remains tracked by P1-027.
- **Likely owner:** Josh + Sim.
- **Confidence:** High.

### P1-021 — P3 bound-AI-discovery gate is not yet met

- **Status:** Carry-forward; P3 is not closed. EX-007's bounded decision and
  EX-009's launch-time decision are accepted, but EX-003 still has no
  execution package and EX-010 remains proposed for the future continuation
  flow.
- **Evidence:** `docs/Research/P3_GATE_REVIEW_2026-09-03.md` is now explicitly
  labeled as a historical 03:35 snapshot. The current local
  `docs/Research/Experiments/P3-Predictive-AI-Cohesive-Report.md` records the
  assembled EX-007/EX-008/EX-009 evidence and bounded human decisions; the
  current bundles pass the strict artifact validator with Unity logs.
- **Next action:** Decide whether EX-003 needs to be executed or the exit gate
  revised, and only then decide whether P3 passes. Keep P4 work preparatory;
  revisit EX-010 only after active-run continuation exists.
- **Likely owner:** Josh.
- **Confidence:** High.

### P1-024 — Current follow-up batch is not durably checkpointed

- **Status:** Resolved for the continuation implementation; the follow-up is
  checkpointed locally and this new hygiene pass remains reviewable.
- **Evidence:** Commit `1d345a93` records the ten-phase continuation flow on
  branch `NF/ConsecutiveRuns`. The previously reported 26 modified and 12
  untracked files are no longer present; only the focused cleanup files from
  this review are now local changes.
- **Next action:** Push the accepted local checkpoints before handing the branch
  off; do not describe them as shared until they are pushed.
- **Likely owner:** Josh + current contributors.
- **Confidence:** High.

### P2-021 — Sequential upgrade continuation remains untested

- **Status:** Open future research item; intentionally outside the current S2
  implementation slice.
- **Evidence:** EX-009 was accepted only for a complete loadout applied before
  the run started. The intended game flow acquires upgrades between simulation
  segments and continues from the current state, which introduces acquisition
  timing and accumulated-state effects that EX-009 cannot measure.
- **Next action:** Revisit proposed EX-010 after the active-run
  checkpoint/resume flow exists. If the flow is not implemented, retain EX-010
  as explicit remaining work rather than treating EX-009 as its answer.
- **Likely owner:** Josh.
- **Confidence:** High.

### P2-022 — Legacy Unity IMGUI text is outside the Noesis default-font path

- **Status:** Noesis/XAML now inherits Pixeloid Sans globally; legacy IMGUI
  surfaces still inherit Unity's `GUI.skin.font` and are intentionally
  unchanged.
- **Evidence:** `Assets/Noesis.settings.asset` configures the Noesis
  `defaultFont`, while `Assets/Scripts/Game/Presentation/GameHud.cs:24-29` and
  `Assets/Scripts/Game/Debug/RuntimeDebugPanel.cs:28` create styles from
  `GUI.skin.label`. These are separate rendering paths and do not consume the
  Noesis setting.
- **Next action:** Decide whether the retained debug/IMGUI surfaces should also
  use Pixeloid; if so, set the shared IMGUI skin font at one composition point
  and verify the result without changing player-facing Noesis contracts.
- **Likely owner:** Josh + UI owner.
- **Confidence:** High.

### P1-026 — Detached worker lacks the latest Unity lifecycle tooling

- **Status:** The cleanup change is local to `UI/ControlLibrary`; the detached
  `codex/cellsim-worker` branch still has a different `UnityTooling.ps1`.
- **Evidence:** `docs/handoffs/2026-09-03-1130-codex-process-lifecycle-cleanup.md`
  explicitly requires propagation before the next remote worker run, and the
  local/worker tool hashes differ.
- **Next action:** Commit the lifecycle cleanup and propagate it to the worker
  branch, then verify worker-side process cleanup before another run.
- **Likely owner:** Simulation/tooling owner.
- **Confidence:** High.

### P1-027 — Sprint 2 task register is not uniquely mapped to board work

- **Status:** Partially resolved; every row now has a stable ID and board link,
  but owner/status reconciliation remains for the sprint review.
- **Evidence:** `docs/NEXT_WORK_BUCKET_PLAN.md` and
  `docs/Sprints/S2-control-record.md` now assign stable IDs and verified Trello
  links to every listed work row. The duplicate board prefixes are disambiguated
  as `S2.2A/B` and `S2.3A/B`, and the Fox/BoardSnapshot reserve cards have
  explicit `S2-CORR-*` IDs. The board currently assigns the parallel S2.3A and
  S2-QA cards to Sim while the repository plan assigns the implementation lane
  to Josh; that discrepancy is recorded rather than hidden.
- **Next action:** Reconcile card ownership, list placement, and completion
  status during the S2 review; do not infer completion from the control card.
- **Likely owner:** Josh + Sim.
- **Confidence:** High.

### P1-028 — S2 correctness tasks are absent from the active ledger

- **Status:** BoardSnapshot repair is resolved; Fox telemetry instrumentation is
  implemented and covered, while its Trello card remains open for owner review.
- **Evidence:** The [BoardSnapshot card](https://trello.com/c/Cy2TOMOh) is in
  `✅ Done`; `SimulationManagerTests.BoardSnapshotCopiesCellsAndSpeciesRoles`
  now uses the correct `(x, y)` fixture and was included in the 187/187 Edit
  Mode artifact recorded in the 2026-09-03 upgrade handoff. The [Fox telemetry
  card](https://trello.com/c/BkJwxhkw) remains in `Current Work`; current
  `SpeciesSimulationMetrics` exposes reproduction outcomes and food-action
  attempts/successes/failures, and `SpeciesSimulation` records those resolver
  results at the action sites. No new full Unity acceptance is claimed here.
- **Next action:** Josh/Sim should review the current Fox report fields and close
  or carry the card in Trello; keep the historical evidence boundaries intact.
- **Likely owner:** Sim + Josh.
- **Confidence:** High.

### P2-019 — Pending XAML batch has pre-commit whitespace errors

- **Status:** Resolved; the affected XAML lines are whitespace-clean.
- **Evidence:** The trailing whitespace at
  `Assets/UI/GalapagOS/XAML/Panels/V_Panel_GalapagOS_Desktop.xaml:74-77` was
  removed and `git diff --check` now passes for the working tree.
- **Next action:** Preserve the cleanup when the current UI follow-up is pushed.
- **Likely owner:** UI owner.
- **Confidence:** High.

### P2-020 — Historical Loose Ends IDs are not unique

- **Status:** Resolved; the related historical entries now use distinct IDs.
- **Evidence:** The historical section now uses `P1-006` for the named
  dual-grid terrain preview item and `P1-006b` for the editor
  smart-tiling preview path fix.
- **Next action:** Preserve the historical text and decisions; use the distinct
  IDs when referring to either terrain-preview entry.
- **Likely owner:** Repository maintainer.
- **Confidence:** High.

## New hygiene candidates — 2026-09-03

These are newly recorded removal, archival, documentation, and refactor
candidates from the project hygiene review. They are triage markers, not
authorization to delete or rename Unity assets. Any serialized asset change
still requires a focused Unity validation pass and preservation of its `.meta`
file/GUID.

Ticket summaries for these items are recorded in
[`PROJECT_HYGIENE_TICKET_SUMMARIES.md`](PROJECT_HYGIENE_TICKET_SUMMARIES.md).

### P1-022 — Terrain tiling documentation and implementation disagree

- **Status:** Resolver, preview path, and directly linked documentation now
  reconcile locally; visual Unity acceptance remains open.
- **Evidence:** `Assets/Scripts/Game/Presentation/TerrainTileResolver.cs:26-44`
  implements an eight-neighbor blob resolver with 47 valid masks, and
  `Assets/Tests/Runtime/TerrainTileResolverTests.cs:9-20` asserts that contract.
  The current editor preview at
  `Assets/Editor/SimulationTools/TerrainTilePreviewWindow.cs` now loads the
  47 Grass/Desert assets from their family directories. The linked tiling,
  art-production, MVVM, and Blob README documents now describe that same
  eight-neighbor contract. The older 16-mask artifacts remain historical.
- **Next action:** After Unity IPC recovery, open the preview and cellular
  prototype and record gameplay-scale visual evidence for all families. Do not
  refactor the resolver from the superseded dual-grid plan.
- **Likely owner:** Presentation/art owner + Sim.
- **Confidence:** High.

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

### P2-008 — Orphan `CavePreview` and `LifeSimulationPreview` components await focused removal

- **Status:** Removal candidate already identified, deferred by the Unity
  validation blocker.
- **Evidence:** The legacy audit records that
  `Assets/Scripts/Game/Presentation/CavePreview.cs` and
  `LifeSimulationPreview.cs` have no scene, prefab, or asset references; the
  current GUID scan finds only each file’s own `.meta`. The deterministic cave,
  Life domain code, and tests remain used and must be retained.
- **Next action:** After Unity IPC/preflight is repaired, delete each source
  file together with its matching `.meta` in a focused commit, then run the
  retained Edit Mode tests and the targeted cellular Play Mode test.
- **Likely owner:** Sim/domain owner.
- **Confidence:** High.

### P2-009 — Unused UI starter scaffold should be removed

- **Status:** Strong removal candidate; no current serialized references found.
- **Evidence:** `Assets/UI/MainMenu/Scripts/BaseViewModel.cs` is an unnamespaced
  Yoda/progress-bar demo with its own `NoesisView` hookup and G/F key polling.
  `Assets/UI/MainMenu/TestUI.xaml`, `Assets/UI/EcoSim/TestUI.xaml`,
  `Assets/UI/GalapagOS/TestUI.xaml`, and `Assets/UI/HUD/TestUI.xaml` are byte-for-byte
  identical starter demos. Their GUIDs have no references outside their own
  `.meta` files, while the active Main Menu uses the separate
  `MainMenu_Old/VM_MainMenu.cs` and `V_Panel_MainMenu.xaml` contract.
- **Next action:** Run one final GUID/reference scan after the UI acceptance
  pass, then remove the four `TestUI.xaml` files and matching `.meta` files,
  plus `MainMenu/Scripts/BaseViewModel.cs` and its `.meta`, as one focused
  starter-scaffold cleanup.
- **Likely owner:** UI owner.
- **Confidence:** High.

### P2-010 — `Assets/UI/EcoSim` contains an unreferenced placeholder shell

- **Status:** Removal/archive candidate; ownership is not yet explicit.
- **Evidence:** The folder contains only `TestUI.xaml` plus four small empty or
  near-empty XAML shell/resource files under `XAML/Controls` and `XAML/Panels`.
  The four production-looking EcoSim XAML GUIDs have no references outside
  their own `.meta` files, and no EcoSim scene/host is present in the current
  serialized reference scan.
- **Next action:** Confirm that no future experiment or design handoff depends
  on this initial scaffold, then remove the folder as a separate XAML cleanup;
  otherwise rename/document it as an explicitly owned prototype boundary.
- **Likely owner:** UI/product owner.
- **Confidence:** Medium-high.

### P2-011 — Unity URP template onboarding is stale project baggage

- **Status:** Low-risk cleanup candidate; template ownership decision needed.
- **Evidence:** `Assets/Readme.asset` and `Assets/TutorialInfo/**` are standard
  URP template onboarding assets with no scene references. The only observed
  links are internal Readme references to the template icon/editor script.
  `docs/UNITY_STANDARDS_ADOPTION_PLAN.md` explicitly calls this template
  content out as an exception, so it should not be removed opportunistically.
- **Next action:** If the project no longer needs Unity’s onboarding Readme,
  remove `Assets/Readme.asset` and the complete `Assets/TutorialInfo` tree,
  including matching `.meta` files, in a focused cleanup; otherwise mark it as
  intentionally retained template content.
- **Likely owner:** Josh + repository maintainer.
- **Confidence:** High for unreferenced; medium for removal.

### P2-012 — `_Recovery/0.unity` is an unclassified recovery artifact

- **Status:** Archive/removal candidate; do not touch until recovery value is
  confirmed.
- **Evidence:** `Assets/_Recovery/0.unity` is not in Build Settings and has no
  name/GUID references outside itself. It is dated 2026-08-13 and contains a
  snapshot-like cellular prototype composition, so it may still be useful as a
  recovery point even though it is not part of the active scene graph.
- **Next action:** Josh should decide whether this is a deliberate recovery
  checkpoint. If not, archive or remove the scene and `.meta` in a standalone
  cleanup; if yes, document its retention purpose and owner.
- **Likely owner:** Josh.
- **Confidence:** Medium.

### P2-013 — `Assets/Scenes/Intro.unity` is an unused template scene

- **Status:** Archive/removal candidate.
- **Evidence:** `Intro.unity` is a basic camera/light scene, is not in current
  Build Settings, has no first-party GUID/name references, and only appears to
  come from the original project setup. It should not be confused with the
  recent manual acceptance scenes `GalapagOSDesktopTest.unity` and
  `TerrainPaintTest.unity`, which remain useful despite not being build scenes.
- **Next action:** Confirm that Intro is not reserved as a future splash/entry
  scene, then remove or archive it with its `.meta` in a focused scene cleanup.
- **Likely owner:** Josh + repository maintainer.
- **Confidence:** Medium-high.

### P2-014 — `MainMenu_Old` is active but now misleadingly named

- **Status:** Refactor/rename candidate; definitely not a deletion candidate.
- **Evidence:** `Assets/Scenes/MainMenu.unity` serializes the GUIDs for
  `MainMenu_Old/VM_MainMenu.cs`, `MainMenuNoesisHost.cs`, and
  `V_Panel_MainMenu.xaml`; `MainMenuPlayModeTests` also requires the active
  `VM_MainMenu` type. The current architecture names this the Main Menu
  contract, so the directory name now implies retirement incorrectly.
- **Next action:** After graphics/UI acceptance, perform a Unity Editor folder
  migration or an equivalent GUID-preserving rename, update documentation and
  tests, and validate the scene. Do not rename the folder from the filesystem
  as a casual cleanup.
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
- **Next action:** Once the P3/Unity and UI acceptance gates are green, choose
  one seam at a time: split simulation phases, separate metrics DTOs from
  accumulation, extract presentation persistence/formatting, isolate report
  writers, and partition tests by behavior. Preserve public/serialized names
  until focused tests and Unity validation support each extraction.
- **Likely owner:** Sim + UI/tooling owners.
- **Confidence:** High for complexity; medium for the exact split.

### P2-016 — Editor scenario generation duplicates runtime pattern definitions

- **Status:** Resolved; editor generation consumes the runtime defaults.
- **Evidence:** `Assets/Editor/SimulationTools/CreateSpeciesScenarioAssets.cs`
  now copies the ordered offsets from
  `SpeciesRuleDefaults.CreateCardinalPattern()` and `CreateMoorePattern()` into
  fresh arrays for serialization. The duplicate literals were removed; runtime
  behavior, generated values, and Editor/runtime assembly boundaries are
  unchanged.
- **Next action:** Keep the runtime pattern order stable when changing these
  defaults; no additional abstraction is needed.
- **Likely owner:** Sim/tooling owner.
- **Confidence:** Medium.

### P2-017 — One-shot `PrepareEx002Variants` generator needs a retention decision

- **Status:** Retention/archive candidate; keep while EX-002 reproducibility is
  still active.
- **Evidence:** `Assets/Editor/SimulationTools/PrepareEx002Variants.cs:10-48`
  is a menu-driven generator with no source callsites beyond Unity’s menu
  binding, but it produces the versioned EX-002 intervention assets used by
  the experiment documentation and reports.
- **Next action:** After the EX-002 artifact-retention/reproducibility policy
  is explicit, either retain it as the documented regeneration tool or archive
  the script and preserve a compact reproducibility record.
- **Likely owner:** Sim/tooling owner.
- **Confidence:** Medium-high.

### P2-018 — Empty Unity directories need an ownership decision

- **Status:** Low-value structural cleanup; no immediate action recommended.
- **Evidence:** Empty directory shells remain under `Assets/Audio`,
  `Materials`, `ThirdParty`, several `Assets/Project/**` paths,
  `Assets/UI/Prefabs`, `PuzzleUI`, `Textures`, `UI/EcoSim/Scripts`,
  `UI/GalapagOS/Art`, `UI/MainMenu/XAML`, and other placeholder locations.
  Several are intentional future ownership boundaries, so directory absence
  would not necessarily improve the project.
- **Next action:** Remove only directories with no named owner or planned
  near-term use, as part of a single structure cleanup. Preserve any folder
  `.meta` files required by the chosen Unity layout and do not mix this with
  gameplay changes.
- **Likely owner:** Repository maintainer + feature owners.
- **Confidence:** High that they are empty; low-medium that removal is useful.

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
  on `origin/UI/ControlLibrary`. Runtime visual acceptance remains open under
  current item P1-014 because Unity preflight is blocked by P1-015.

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
