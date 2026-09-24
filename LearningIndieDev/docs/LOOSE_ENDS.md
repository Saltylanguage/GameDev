# Loose Ends

This is the actionable ledger for unresolved project, planning, ownership, and
documentation gaps. Closed history stays in Git and task handoffs.

## Current status — 2026-09-22

- No P0 issue is verified. Local `ProjectMain` is at `c6b3282c` and is one
  commit ahead of `origin/ProjectMain`; that commit contains the Forest Edge
  balance/UI pass and its handoff. The worktree still has four uncommitted
  follow-on edits: the Forest Edge scenario, `SpeciesCell.cs`, its domain test,
  and the Chrono Rabbit tile art. Preserve those edits until the active balance
  follow-up is reconciled; do not treat the local commit as the shared sprint
  baseline yet.
- Island Survivor slice retirement completed on 2026-09-18: its scene, runtime,
  dedicated tests/validator, and Island Chores textures are removed; historical
  handoffs remain unchanged. The [main-flow cleanup record](MAIN_GAME_FLOW_CLEANUP_CANDIDATES.md)
  tracks the remaining candidates separately.
- S3-01, S3-02, and S3-03 are complete. Current Sprint 3 work is S3-04
  (Mutation readability), S3-06 (visual polish), and S3-08 (Fox telemetry);
  S3-07 is reserve and S3-05 is stretch.
- The accepted expedition contract is six 10-second rounds, with a Mutation
  choice or Skip after rounds 1–5. The contract requires three Mutation offers
  plus Skip; the current offer path still exposes two. S3-04 records this gap
  and the decisions still needed.
- The retained no-graphics acceptance baseline remains EditMode 234/234 and
  PlayMode 28/30 (0 failures, 2 expected graphics-only skips). Newer targeted
  evidence is narrower: the Species domain filter passed 96/96 and the latest
  graphics-capable visual capture passed 1/1. Those results support the current
  balance/UI edits but do not replace the broader acceptance baseline. The
  focused phase-decision PlayMode check and the separate Main Menu/Settings
  human review remain open.

## Triage rules

- **P0** — blocks current work, risks data loss, or creates a material
  contradiction.
- **P1** — likely to cause avoidable rework or leave an active decision
  unresolved.
- **P2** — useful cleanup or follow-up that is not currently blocking.

## Open items

### P1-016 — Mutation catalog readability and evidence review

- **Status:** The catalog and authoring path exist. The bounded EX-007 decision
  is accepted; readability and a bounded Forest Edge/Hare review remain open
  under S3-04.
- **Evidence:** [S3-04 plan](Sprints/S3-04-mutation-readability-plan.md) and
  [Next Work Bucket Plan](NEXT_WORK_BUCKET_PLAN.md). EX-009's zero-delta result
  is implementation evidence, not a balance conclusion.
- **Next action:** Complete the S3-04 readability and bounded evidence review.
  Record broader catalog balance or promotion as separate work.
- **Owner:** Josh; Sim supplies evidence. **Confidence:** High.

### P1-026 — Remote worker branch has stale Unity lifecycle tooling

- **Status:** The current tree has the lifecycle cleanup, but
  origin/codex/cellsim-worker still has a different UnityTooling.ps1;
  no local worker branch is checked out.
- **Evidence:** [process-lifecycle handoff](handoffs/2026-09-03-1130-codex-process-lifecycle-cleanup.md)
  and the 2026-09-17 file comparison.
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

- **Status:** The retained broad acceptance baseline is still EditMode 234/234
  and no-graphics PlayMode 28/30 with 0 failures and 2 expected graphics-only
  skips. Since then, the Species domain filter passed 96/96 and the latest
  graphics-capable visual capture passed 1/1, but neither is a replacement for
  the full acceptance run. The Settings/Collection focused invocation still
  has no results XML, and Main Menu branding/generated-art acceptance remains
  human review.
- **Evidence:** [clean test artifacts](../artifacts/unity-tests-20260918-144956/),
  [latest targeted EditMode artifacts](../artifacts/unity-tests-20260921-232912/),
  [latest visual capture artifacts](../artifacts/visual-evidence-20260921-232618/),
  the [Unity automation handoff](handoffs/2026-09-18-1452-sol-unity-automation-lane-integration-closeout.md),
  and the [phase-selection polish handoff](handoffs/2026-09-18-2208-codex-upgrade-selection-polish.md).
- **Next action:** Run the focused phase-decision PlayMode check after the
  shared Editor is free. Keep the Settings/Collection result gap and Main Menu
  human review separate from the completed S3-01 scope. **Owner:** Josh +
  UI/repository maintainer. **Confidence:** High.

### P1-032 — Desktop route drops profile and launch context

- **Status:** The Desktop starts a local Forest Edge/Hare preview instead of
  consuming the profile and frozen launch request. This is separate from the
  completed S3-03 Lab-to-expedition flow.
- **Evidence:** Helper_SceneTransition.LoadDesktop,
  GalapagOSDesktopNoesisHost.OpenSimulation, and the GDD/TDD route matrices.
- **Next action:** Decide when profile and launch-context transfer becomes a
  required Desktop contract. Keep persistence and reward settlement out of S3
  unless explicitly rescheduled.
- **Owner:** Josh + UI/runtime owner. **Confidence:** High.

### P1-033 — Mutation offer contract and completed-card wording disagree

- **Status:** S3-02 is complete and its contract is authoritative: three
  Mutation offers or Skip after rounds 1–5. The current player offer path
  returns two. The S3-04 plan says the owner must decide whether the smallest
  offer correction belongs there or should be tracked separately. The completed
  S3-02 Trello card may still need wording updated for the 36×20 Forest Edge
  default and the still-deferred playable-plant decision.
- **Evidence:** [S3-02 expedition contract](Sprints/S3-02-expedition-contract.md),
  [S3-04 plan](Sprints/S3-04-mutation-readability-plan.md), and Trello card c3i7HO09.
  The production scenario now defaults to 36×20. The editor generator creates
  a separate legacy scenario, so its 36×20 setting was not the gameplay source.
- **Next action:** Assign the three-offer correction without reopening S3-02.
  If the completed Trello card still has old acceptance text, clarify the
  36×20 default and keep playable plants deferred. The current player offer
  path still needs its separate three-offer correction.
- **Owner:** Josh + simulation/design owner. **Confidence:** High.

### P1-034 — Forest Edge balance pass is provisional and uncommitted

- **Status:** The committed Fox 6 pass is a provisional working comparison
  baseline, and its follow-up fixed mutually-ready Fox mating priority,
  separation, and the shared 24-tick reproduction cooldown. The balance task's
  latest turn is complete, but the worktree still contains four uncommitted
  follow-on edits, so the result is not yet the shared sprint baseline. The
  current evidence supports the mating-state fix; it does not approve final
  Forest Edge balance.
- **Evidence:** [Forest Edge scenario](../Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset),
  [current balance handoff](handoffs/2026-09-21-codex-forest-edge-first-balance-pass.md),
  `c6b3282c` (`Balance Forest Edge simulation and improve board feedback`), and
  the completed `Tune fox hare forest values` task.
- **Next action:** Reconcile the four dirty files with the committed baseline,
  then run one matched one-variable comparison before deciding whether to keep
  Fox 6 and committing the scenario/evidence together. **Owner:** Josh + Sim.
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
- Removed the long 2026-08-20 historical-open and R-001–R-034 resolved-item
  catalogues. Their source handoffs and Git history remain available; this file
  now lists actionable gaps only.
