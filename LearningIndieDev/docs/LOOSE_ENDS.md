# Loose Ends

This is the actionable ledger for unresolved project, planning, ownership, and
documentation gaps. Closed history stays in Git and task handoffs.

## Current status — 2026-09-29

- No P0 issue is verified. `codex/forest-edge-visual-pass` includes the
  `BevBranch` merge at `51abd41d` and the tested cap fix in pushed commit
  `974827f2`. The retained Clean suites passed EditMode 263/263 and PlayMode
  34 tests; two graphics-only tests were skipped as expected, and none failed.
- Island Survivor slice retirement completed on 2026-09-18: its scene, runtime,
  dedicated tests/validator, and Island Chores textures are removed; historical
  handoffs remain unchanged. The [main-flow cleanup record](MAIN_GAME_FLOW_CLEANUP_CANDIDATES.md)
  tracks the remaining candidates separately.
- S3-01 through S3-04 and S3-06 through S3-08 are complete. S3-08 Fox
  telemetry clarification is validated. The bounded S3-06 pass is accepted;
  future polish remains iterative. S3-05 remains uncommitted stretch work in
  Backlog.
- The accepted expedition contract is six 10-second rounds, with a Mutation
  choice or Skip after rounds 1–5. The player offer now shows three distinct
  applicable free Mutations plus Skip. Invalid Reproductive Drive and coupled
  Fox Brood Drive offers are hidden. Hare Reinforcements remain a separate
  purchase. The S3-04 V1 path and Tough Hide evidence are accepted for the
  current sprint; future mutation iteration remains open.
- Josh accepted the current Forest Edge evidence for S3 closeout; it does not
  establish broad ecological balance. Main Menu generated-art review is done,
  with Chrono working on the selected direction. Josh manually closed S3 on
  2026-09-29; committed S3 cards and the control card are in Trello Done. Josh
  accepted M1 on 2026-09-29 using distributed evidence; later changes may need
  retesting. Refine the Proposed S4 work list before a separate kickoff.

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

### P1-034 — Forest Edge balance values remain provisional

- **Status:** Josh accepts the current Forest Edge evidence for this S3
  closeout. The incoming production Fox and Hare baselines, including crowding
  metabolism, Hare reproduction, and grass reseeding, remain provisional for
  later iteration; this is not a broad balance approval or a current S3 gate.
- **Evidence:** [Hare asset](../Assets/Data/ProductionData/CellularSimulation/Species/hare.asset),
  [Fox asset](../Assets/Data/ProductionData/CellularSimulation/Species/fox.asset),
  [SpeciesUpgrade.cs](../Assets/Scripts/Game/Species/SpeciesUpgrade.cs),
  [current-value diagnostic handoff](handoffs/2026-09-22-2200-codex-forest-edge-current-values-diagnostic-batches.md),
  [Fox/Hare balance handoff](handoffs/2026-09-21-codex-forest-edge-first-balance-pass.md),
  and commits `5e0e28c2` / `0ba7a9cd`.
- **Next action:** Continue matched Forest Edge balance work when selected for a
  later iteration. Keep it separate from the accepted S3 V1 Mutation offer.
  **Owner:** Josh + Sim.
  **Confidence:** High.

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

## Resolved or pruned in this review

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
- **P1-031 resolved for the S3 review:** Josh completed the Main Menu generated-
  art direction review; Chrono is working on the art. The merged-baseline Clean
  run passed EditMode 263/263 and PlayMode 34 tests; two graphics-only tests
  were skipped as expected, and none failed. See the [S3 merged-baseline handoff](handoffs/2026-09-29-0341-codex-s3-merged-cap-review.md)
  and [retained results](../artifacts/unity-tests-20260929-033047/). Art
  production and further UI polish remain future work.
- **P1-035 resolved:** The S3-08 report now distinguishes pre-resolution `Mating`
  state ticks from resolver candidate evaluations. Eligible attempts reconcile
  with failed chance rolls, unavailable birth locations, and successful
  attempts in both report paths; the five-seed Forest Edge check and focused
  regressions passed. See the
  [S3-08 telemetry handoff](handoffs/2026-09-27-codex-s3-08-fox-telemetry.md).
- **P1-036 resolved:** Verified the S3-02 Trello card already records the
  36×20 production default and defers playable Fern/Plant identity, Skip bonus,
  and the final performance-to-currency formula. Reconciled S3-01's stale
  post-fix verification caveat, S3-03/04's stale offer/integration notes, and
  S3-06's Forest Edge approval wording. S3-07 and the S3 control card are now
  complete in Done. See the [S3 control record](Sprints/S3-control-record.md)
  and [merged-baseline closeout handoff](handoffs/2026-09-29-0341-codex-s3-merged-cap-review.md).
- Removed the long 2026-08-20 historical-open and R-001–R-034 resolved-item
  catalogues. Their source handoffs and Git history remain available; this file
  now lists actionable gaps only.
- The prior `ProjectMain` staged-bundle snapshot was superseded by the active
  `Balance/ForestEdge` branch and commits `5e0e28c2`/`0ba7a9cd`; its branch and
  validation follow-ups are no longer current.

- **P1-029 — First production Genome node contract:** Josh approved the Fertile
  Droppings direction and progression rules, including the three-run Gene Lab
  gate, Rabbit Data purchases, reset refunds for a flat Research Data fee,
  eight-point capacity, default stacking, concise copy, and predator counterplay.
  This closes the design-contract gap only; implementation and balance evidence
  remain follow-up work. See the [accepted contract](handoffs/2026-10-01-0154-codex-p1-029-first-hare-genome-contract.md).
