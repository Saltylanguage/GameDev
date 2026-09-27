# Free Mutations and near-term Desktop migration

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-09-24-1341-codex-free-mutations-and-near-term-desktop-migration
- Owner: Codex
- Branch: Balance/ForestEdge
- Baseline commit: 0ba7a9cd
- Date: 2026-09-24
- Supersedes: 2026-09-23-codex-population-reinforcement-mutation.md

## Summary

Resolved P1-033's phase-Mutation economy mismatch: phase-boundary choices now
apply without spending Data, and the fixed repeatable Reinforcements offer is
recorded as the third choice. Updated the expedition and S3-04 records, pruned
P1-033 from the open ledger, and refreshed P1-031 test evidence. Scheduled the
P1-032 Desktop launch-context migration as the first post-S3 work item in the
S4 draft. All work remains local on `Balance/ForestEdge` and is uncommitted.

## Changes

- `SpeciesSimulationPreview` now validates and applies authored or legacy
  phase-boundary choices without currency checks/spending. The phase UI says
  `FREE`; non-phase legacy reward purchases retain their catalog costs.
- Added/updated PlayMode assertions for zero-Data authored and legacy choices,
  the fixed Reinforcements option, and free repeated pool selections.
- Fixed two stale test fixtures: Main Menu now waits for its intentional
  0.95-second transition; the fractional-digestion test keeps its test animal
  hungry for all 20 bites so it isolates energy-remainder accumulation.
- Updated `S3-02`, the `S3-04` plan, S3 control row, Working State, and
  `LOOSE_ENDS.md`. P1-033 is recorded under pruned history.
- Added the P1-032 migration scope and first-work-block slot to the S4 draft
  and Main Menu delivery plan. No Desktop runtime migration was implemented.

## Decisions and assumptions

- Mutation options at each of the five phase boundaries cost no Data. This
  does not change Gene Lab Genome prices or separate terminal legacy reward
  purchases.
- The S3 offer is two distinct choices from the existing role-specific
  five-Mutation pool plus fixed repeatable Reinforcements as the third. Skip
  remains unrewarded. Reinforcements balance remains provisional; this closes
  the contract/economy inconsistency, not the broader S3-04 evidence review.
- P1-032 is scheduled first in the next post-S3 work block (S4 forecast
  2026-10-01–2026-10-14), ahead of S4-01. S4 remains proposed: kickoff must
  estimate and trade feature capacity before implementation, without using the
  protected 8h reserve.

## Validation

- `CellSim.ps1 -Command Test -Mode All -Execution Auto` — passed, EditMode
  256/256 and PlayMode 33/33; retained at
  `artifacts/unity-tests-20260924-134934/` (Live lane, Unity status ready).
- Dedicated My Collection surface test — passed 1/1 at
  `artifacts/unity-tests-20260924-134853/`; the full suite also includes it and
  the dedicated Settings surface test.
- Focused authored Mutation test — passed 1/1 at
  `artifacts/unity-tests-20260924-132800/`.
- Focused Main Menu transition test — passed 1/1 at
  `artifacts/unity-tests-20260924-133132/`.
- `git diff --check` — passed.

## Risks and incomplete work

- P1-031 remains open only for human review of Main Menu branding/generated
  art; Settings and My Collection automated coverage is green.
- P1-026 remains open: `origin/codex/cellsim-worker` has a material
  `UnityTooling.ps1` source divergence. The shared worker branch was not
  changed; propagation and process-cleanup validation are still required.
- P1-032 is scheduled but has no approved estimate/capacity allocation yet.
- No matched Forest Edge balance batch was run. Approved implementation and
  free-choice regression coverage are not a balance approval.

## Next useful step

At S4 kickoff, size P1-032 and trade it against feature work; separately
complete the human visual review recorded in P1-031.
Keep the existing balance branch/worktree changes intact until their owner
accepts, commits, or reverts them.
