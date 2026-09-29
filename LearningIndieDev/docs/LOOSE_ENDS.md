# Loose Ends

This is the actionable ledger for unresolved project, planning, ownership, and
documentation gaps. Closed history stays in Git and task handoffs.

## Current status — 2026-09-27

- No P0 issue is verified. `BevBranch` incorporates the Forest Edge visual
  pass. The directly installed Unity 6000.4.6f1 Editor compiled the merged
  project; the incoming ecology values have not received a matched Forest
  Edge balance review or full visual acceptance.
- Island Survivor slice retirement completed on 2026-09-18: its scene, runtime,
  dedicated tests/validator, and Island Chores textures are removed; historical
  handoffs remain unchanged. The [main-flow cleanup record](MAIN_GAME_FLOW_CLEANUP_CANDIDATES.md)
  tracks the remaining candidates separately.
- S3-01 through S3-04 are complete. S3-08 Fox telemetry clarification is
  validated; S3-06 visual polish is in progress, S3-07 is reserve, and S3-05
  is stretch.
- The accepted expedition contract is six 10-second rounds, with a Mutation
  choice or Skip after rounds 1–5. The player offer now shows three distinct
  free Mutations plus Skip, and focused tests cover repeat stacking and
  continuation. Tough Hide's first bounded evidence review is recorded and its
  copy/card markers are accepted for this slice. The completed S3-02 Trello card
  wording has not been refreshed. The earlier two-Mutation-plus-Reinforcements
  offer is superseded by this S3-04 implementation.
- The latest retained full Unity run from 2026-09-24 passed EditMode 256/256
  and PlayMode 33/33. Newer focused S3-04 and S3-08 checks passed. The merge
  received a direct Editor compile on 2026-09-27; no post-merge test suite was
  run.

## Triage rules

- **P0** — blocks current work, risks data loss, or creates a material
  contradiction.
- **P1** — likely to cause avoidable rework or leave an active decision
  unresolved.
- **P2** — useful cleanup or follow-up that is not currently blocking.

## Open items

### P1-026 — Remote worker branch has stale Unity lifecycle tooling

- **Status:** Still open. A fresh 2026-09-24 comparison confirms a material
  source divergence: the remote worker file is blob
  `c1abb327c9abbac939b4dac34cb8b434875ac233`, while the current verified file is
  `33d399b16c29c30bdc21f7f5945decd28c24c8db`. The remote still has the older
  process-lock/preflight flow and lacks the current Pipeline CLI/state-routing
  implementation. This requires propagating and validating code on the worker
  branch, not a documentation-only correction; that shared branch was not
  changed in this pass.
- **Evidence:** [process-lifecycle handoff](handoffs/2026-09-03-1130-codex-process-lifecycle-cleanup.md)
  and the 2026-09-24 `UnityTooling.ps1` blob comparison.
- **Next action:** Propagate the cleanup before the next remote worker run and
  verify process cleanup there.
- **Owner:** Simulation/tooling owner. **Confidence:** High.

### P1-029 — Production Genome decisions and behavior remain open

- **Status:** Identity, profile, immutable snapshot, catalog, and generic Gene
  Lab foundations exist. No production Genome node or executable effect is
  approved.
- **Evidence:** [SpeciesGenomeContract.cs](../Assets/Scripts/Game/Species/SpeciesGenomeContract.cs),
  profile/launch/run snapshot contracts, and the [Genome reconciliation handoff](handoffs/2026-09-17-codex-genome-fixture-and-doc-reconciliation.md).
- **Next action:** Before implementation, approve one small Hare node's effect,
  wording, cost, prerequisite, activation rule, evidence plan, and persistence
  owner.
- **Owner:** Josh with design/simulation owners. **Confidence:** High.

### P1-031 — Validation status and player-shell review need reconciliation

- **Status:** The final post-change run passed EditMode 256/256 and PlayMode
  33/33. Two stale checks were corrected: the Main Menu test now waits for its
  0.95-second scene transition, and the fractional-digestion fixture keeps its
  animal hungry for all 20 bites so it tests remainder accumulation instead
  of threshold crossing and movement. Dedicated Settings and My Collection
  PlayMode checks pass. Main Menu branding/generated-art acceptance remains
  human review.
- **Evidence:** [full EditMode and PlayMode results](../artifacts/unity-tests-20260924-134934/),
  [focused My Collection result](../artifacts/unity-tests-20260924-134853/),
  [focused Main Menu rerun](../artifacts/unity-tests-20260924-133132/),
  [Main Menu test timing correction](../Assets/Tests/PlayMode/MainMenuPlayModeTests.cs),
  [fractional digestion fixture](../Assets/Tests/Runtime/SpeciesDomainTests.cs),
  [population reinforcement handoff](handoffs/2026-09-23-codex-population-reinforcement-mutation.md),
  [Unity automation handoff](handoffs/2026-09-18-1452-sol-unity-automation-lane-integration-closeout.md),
  and the [phase-selection polish handoff](handoffs/2026-09-18-2208-codex-upgrade-selection-polish.md).
- **Next action:** Complete the human visual review of Main Menu branding and
  generated-art promotion. **Owner:** Josh + UI/repository maintainer.
  **Confidence:** High.

### P1-032 — Desktop route drops profile and launch context

- **Status:** Scheduled as the first item in the next post-S3 work block,
  ahead of S4-01, in the S4 draft. The draft forecasts 2026-10-01–2026-10-14,
  but still requires an estimate and feature-capacity trade at kickoff before
  implementation; the 8h integration/review reserve remains protected. This is
  separate from the completed S3-03 Lab-to-expedition flow.
- **Evidence:** Helper_SceneTransition.LoadDesktop,
  GalapagOSDesktopNoesisHost.OpenSimulation, the [Desktop delivery plan](MAIN_MENU_LAB_DELIVERY_PLAN.md),
  the [S4 control record](Sprints/S4-control-record.md), and the GDD/TDD route
  matrices.
- **Next action:** At S4 kickoff, size and capacity-trade the migration, then
  route Desktop launch through `SimulationLaunchRequest` and verify the frozen
  scenario/species/profile inputs. Keep persistence and reward settlement out
  of this migration.
- **Owner:** Josh + UI/runtime owner. **Confidence:** High.

### P1-036 — S3-02 Trello wording may lag the accepted expedition contract

- **Status:** The player offer path now returns three Mutation choices or Skip
  at each phase boundary; the runtime gap is resolved under S3-04. The completed
  S3-02 Trello card may still need wording updated for the 36×20 Forest Edge
  default and the still-deferred playable-plant decision.
- **Evidence:** [S3-02 expedition contract](Sprints/S3-02-expedition-contract.md),
  [S3-04 plan](Sprints/S3-04-mutation-readability-plan.md), and Trello card c3i7HO09.
  The production scenario now defaults to 36×20. The editor generator creates
  a separate legacy scenario, so its 36×20 setting was not the gameplay source.
- **Next action:** If the completed Trello card still has old acceptance text,
  clarify the 36×20 default and keep playable plants deferred. No board edit
  was made during this S3-04 implementation.
- **Owner:** Sprint board owner + simulation/design owner. **Confidence:** High.

### P1-034 — Forest Edge balance values remain provisional

- **Status:** The `+1` minimum- and maximum-litter Mutations were approved
  for production on 2026-09-24. The incoming Forest Edge branch changes the
  production Fox and Hare baselines, including crowding metabolism, Hare
  reproduction, and grass reseeding. The user chose those incoming values for
  this merge; their ecological effect has not been established by a matched
  Forest Edge comparison.
- **Evidence:** [Hare asset](../Assets/Data/ProductionData/CellularSimulation/Species/hare.asset),
  [Fox asset](../Assets/Data/ProductionData/CellularSimulation/Species/fox.asset),
  [SpeciesUpgrade.cs](../Assets/Scripts/Game/Species/SpeciesUpgrade.cs),
  [current-value diagnostic handoff](handoffs/2026-09-22-2200-codex-forest-edge-current-values-diagnostic-batches.md),
  [Fox/Hare balance handoff](handoffs/2026-09-21-codex-forest-edge-first-balance-pass.md),
  and commits `5e0e28c2` / `0ba7a9cd`.
- **Next action:** Run a matched Forest Edge comparison before treating the
  incoming Fox/Hare values as balance-approved. Keep that question separate
  from the approved production Mutations. **Owner:** Josh + Sim.
  **Confidence:** High.

### P1-035 — Fox mating telemetry does not yet explain eligibility

- **Status:** The mating stability and cooldown regressions are covered, and
  the live mating filter passed 4/4; however, the latest five-seed, 600-tick,
  six-phase continuation recorded three Fox births while aggregate `Mating`
  state telemetry was zero. This is an evidence/telemetry discrepancy, not a
  reason to claim the reproduction behavior is fully validated.
- **Evidence:** [balance handoff](handoffs/2026-09-21-codex-forest-edge-first-balance-pass.md),
  [cooldown experiment](../artifacts/cellular-experiment-20260921-232331/), and
  the latest targeted EditMode artifact [96/96 domain tests](../artifacts/unity-tests-20260921-232912/).
- **Next action:** Run a targeted reproduction/telemetry check that records
  eligibility, mating state, cooldown, and birth outcome in the same run;
  explain or correct the zero-state count before closing S3-08. **Owner:**
  Josh + Sim. **Confidence:** High.

### P2-005 — Raw worker artifact retention policy

- **Status:** One conservative cleanup pass is complete; five semantic
  duplicate bundles remain candidates for recoverable archive/removal.
- **Evidence:** [artifact-retention audit](handoffs/2026-09-09-artifact-retention-audit.md)
  and its linked cleanup handoff.
- **Next action:** Review the five candidates, archive approved bundles
  recoverably, then rerun the summarizer and DirtyBoy before removal.
- **Owner:** Repository maintainer + tooling owner. **Confidence:** High.

## Deferred hygiene

These are deliberate follow-ups, not current sprint blockers. Preserve stable
IDs because other project documents reference them. Detailed evidence, owners,
and sequencing remain in [Project Hygiene Ticket Summaries](PROJECT_HYGIENE_TICKET_SUMMARIES.md).

- **P2-007 — SpeciesArchetype compatibility:** Migrate remaining production
  and test callsites to SpeciesId before removing the obsolete compatibility
  surface. Keep its shim until a zero-reference/compile gate passes.
- **P2-014 — MainMenu_Old naming:** The directory contains the active Main Menu
  implementation. Rename only through a GUID-preserving Unity migration after
  the current UI work stabilizes.
- **P2-015 — Large multi-responsibility files:** Refactor one demonstrated seam
  at a time when it blocks active work; avoid a broad refactor during S3.
- **P2-022 — TerrainPaint runtime IMGUI:** Decide after the terrain workflow
  settles whether to retain it as a developer diagnostic, migrate it, or remove
  the scene/script/test together. It is not player UI; see the [cleanup candidates](MAIN_GAME_FLOW_CLEANUP_CANDIDATES.md).
- **CF-6 performance measurement (former P1-030):** Optional stretch work with
  no committed capacity. Reopen only when time is explicitly scheduled.

## Pruned in this review

- The Island Survivor runtime slice was removed on 2026-09-18 after confirming
  it was outside the Main Menu → Desktop → Simulation flow. Historical
  handoffs and decisions were preserved.
- **P2-024 retired:** The ledger referred to
  tools/Generate-BlobTerrainTiles.ps1, but that script is absent from the
  current project tree. The active authored-representative workflow is recorded
  in the [terrain handoff](handoffs/2026-09-17-codex-terrain-art-standard-64px.md).
- **P2-006 resolved:** Documented the manual Forest Edge diagnostic scenario
  comparisons and their mutable-asset caveats in
  [Unity Simulation Tooling](UNITY_SIMULATION_TOOLING.md).
- **P2-025 resolved:** Removed the absent `Assets/Materials/` placeholder from
  the [main-flow cleanup inventory](MAIN_GAME_FLOW_CLEANUP_CANDIDATES.md).
- **P1-033 resolved:** Confirmed phase-boundary Mutations are free, including
  the fixed repeatable Reinforcements third choice; aligned the S3-02/S3-04
  records and S3 control row. The full Unity run passed EditMode 256/256 and
  PlayMode 33/33, including zero-Data legacy, authored, and Reinforcements
  choices and the same-run repeated-choice regression ([retained results](../artifacts/unity-tests-20260924-134934/)).
  The separate legacy terminal reward path still retains its catalog costs.
- Removed the long 2026-08-20 historical-open and R-001–R-034 resolved-item
  catalogues. Their source handoffs and Git history remain available; this file
  now lists actionable gaps only.
- The prior `ProjectMain` staged-bundle snapshot was superseded by the active
  `Balance/ForestEdge` branch and commits `5e0e28c2`/`0ba7a9cd`; its branch and
  validation follow-ups are no longer current.
