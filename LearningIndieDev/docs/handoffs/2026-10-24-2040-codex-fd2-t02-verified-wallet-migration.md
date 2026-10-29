# fd2-t02-verified-wallet-migration

[Working state](../WORKING_STATE.md) | Status: complete (local T02 and approved board closeout)

- Handoff schema: 1
- Handoff ID: 2026-10-24-2040-codex-fd2-t02-verified-wallet-migration
- Owner: codex
- Branch: BevBranch
- Baseline commit: 9f9ed11
- Date: 2026-10-24
- Supersedes: 2026-10-24-2025-codex-fd2-t02-migration-recovery-and-validation

## Summary

The Editor startup/recovery obstacle is resolved. The current local FD2-T02
implementation compiled in Unity 6000.4.6f1 and passed 17/17 focused EditMode
and 9/9 Main Menu PlayMode tests, with zero failures or skips. This satisfies
the ticket's focused migration, persistence, recovery and snapshot-copy checks.
Source remains local and uncommitted; no Trello update or push is included.

## Changes

- Continues the existing v3 profile schema: legacy Currency migrates to Research
  Data, species-keyed wallets persist independently, Genome progress survives,
  and fresh profiles have zero balances and locked Genome paths.
- Recovery JSON is flushed before the migrated store replaces the old store.
- Currency is a compatibility getter projecting ResearchData directly.
- Six new EditMode cases cover repeat migration with two profiles and existing
  recovery preservation, immutable wallet copies, and invalid/duplicate wallets.
- The earlier two PlayMode migration and v3 roundtrip cases now execute and pass.
- Preserve all unrelated local metadata, recovery scenes and FD2 planning edits.

## Decisions and assumptions

- T02 consumes no Hare node effects; T01 remains open for Salty's mechanics and
  effect review. No new effect, price or balance direction was selected.
- Restoring the original v2 JSON between loads models interrupted/repeated
  migration. A real OS crash or disk-write failure was not injected.
- Test fixtures preserve and restore the user's profile/recovery PlayerPrefs.

## Validation

- `CellSim.ps1 -Command Doctor`: Ready, Pipeline reachable; healthy CLI/license.
  Evidence: `artifacts/unity-doctor-20261024-203852/`.
- `CellSim.ps1 -Command Test -Mode EditMode -Execution Auto -TestFilter
  SaltyGame.Tests.GenomeContractTests`: Live, 17 passed, 0 failed, 0 skipped.
  Evidence: `artifacts/unity-tests-20261024-203857/EditMode-results.json`.
- `CellSim.ps1 -Command Test -Mode PlayMode -Execution Auto -TestFilter
  SaltyGame.PlayModeTests.MainMenuPlayModeTests`: Live, 9 passed, 0 failed,
  0 skipped. Evidence: `artifacts/unity-tests-20261024-203904/PlayMode-results.json`.
  Both new migration/wallet tests appear by name with Passed status.
- Runtime and test assemblies were rebuilt October 24 at approximately 20:30
  local time. Console ground truth reports compilationFailed=false and
  compiling=false. Editor status reports ready and playMode=stopped.
- Retained Console error is the deliberately expected missing-profile launch
  error in MainMenuPlayModeTests (LogAssert.Expect at line 175). Other retained
  warnings include legacy obsolete API usage, existing unreachable UI code,
  AI relay/account availability, Noesis cache regeneration and menu focus.
  No clean-Console claim is made. Console/status JSON is retained alongside
  the PlayMode result. No messages were cleared.
- Earlier linked-source .NET model check passed 30 assertions at
  `artifacts/fd2-t02-20261024/snapshot-check.log`.
- Focused diff whitespace checks pass. The test-runner InitTestScene temporary
  scene was removed by the runner after completion.

## Risks and incomplete work

- These are focused Live tests; no full clean suite, player build, human visual
  acceptance, balance test or real process-crash injection was performed.
- Existing Currency callers remain compatible. Explicit permanent purchase and
  exactly-once settlement operations belong to later FD2 tickets.
- Current changes are uncommitted on BevBranch at baseline 9f9ed11. Trello was
  last read in Upcoming Work; no external mutation is claimed.

## Next useful step

### Approved closeout and dependency refresh - 2026-10-24

Bevin approved the external closeout. T02 was marked complete, moved to Done,
and the comment below was posted and visibly read back. Proof:
`artifacts/fd2-t02-20261024/trello-t02-done.png`. Source is still uncommitted
and unpushed. The review/approval steps below are retained as the earlier plan.

T01 was read live and remains incomplete in In Progress. Freshly fetched
origin/ProjectMain is `7ac2a5456bf0c1f6b8e3c106533cd555f78ba5dc`; it includes
Salty's `be98b03` Rabbit Genome implementation. Its POC catalog has supported
effects such as `poc.hare.long-stride` (+0.25 movement speed) and
`poc.hare.population-pool` (+10 pool capacity), priced at 1 Rabbit Data with
2-point capacity. Its contract sets license/reset fees and success awards to
1. These are incoming implementation values, not newly approved FD2 tuning.
They differ from the local five-node FD2 placeholder sheet (10 license, 5
per node, 20 RD reset). T01 still needs catalog/effect/tuning reconciliation
and SG-005 review. Both incoming profile files change schema-v3 storage;
integration must reconcile those fields with local T02, then retest. No merge
or incoming-code validation was performed during this status check.

T02 is technically verified and ready for review/board closeout. Under SG-002,
obtain approval before the external board update. Suggested additive comment:

"FD2-T02 verified locally on BevBranch (baseline 9f9ed11). Profile-store v3
migrates legacy currency once into Research Data, preserves profile/Genome
state, persists independent species wallets, and retains legacy JSON recovery.
Fresh profiles have zero balances and locked paths. Focused Unity verification
on 2026-10-24: GenomeContractTests EditMode 17/17; MainMenuPlayModeTests 9/9;
zero failures/skips. Includes repeated migration, existing recovery preservation,
snapshot-copy validation and v3 wallet roundtrip. Changes remain uncommitted
and unpushed. Full clean suites, player build and real crash injection were
not run. See docs/handoffs/2026-10-24-2040-codex-fd2-t02-verified-wallet-migration.md."

After approved closeout, FD2-T03 is the next calculation ticket; keep T01's
unresolved effects separate from already recorded conversion inputs.
