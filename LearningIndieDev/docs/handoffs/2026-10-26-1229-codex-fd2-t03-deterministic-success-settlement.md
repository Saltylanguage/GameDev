# fd2-t03-deterministic-success-settlement

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-26-1229-codex-fd2-t03-deterministic-success-settlement
- Owner: codex
- Branch: codex/fd2-t01-integration
- Baseline commit: 03cd29e
- Date: 2026-10-26
- Supersedes: none

## Summary

Resumed the approved FD2 work after verifying the clean local T01 checkpoint
and fetching origin (ProjectMain remains at 7ac2a54). T03 now calculates the
immutable success settlement breakdown from final unspent FD, frozen research
level and current permanent balances. It does not mutate or save wallets.
All 37 focused settlement tests pass; the clean EditMode suite passes 534 tests
with zero failures and one retained-manifest parity skip.

## Changes

- Added `FieldDataSettlement.TryCalculate` and `FieldDataSettlementReceipt` in
  the existing Simulation domain. The calculator validates a successful journey,
  identity, known selected species, balances and the approved research cap.
- Uses the canonical T01 rate and exact integer ratio. First floor FD/10, then
  floor base data times 11^level/10^level. Quotient/remainder arithmetic avoids
  an overflowing intermediate product for valid large deposits. Both permanent
  wallet additions are checked before any receipt is returned.
- The immutable receipt records journey/species identity, definition version,
  leftover FD, exchange rate, frozen level/ratio, base data, deposit, fixed
  10 RD bonus and before/after balances. Zero leftover still earns 10 RD.
- Inputs are copied into scalar receipt fields; calculation cannot mutate or
  retain the supplied wallet dictionary. No receipt is produced for failure,
  unfinished/closed journeys, invalid inputs or overflow.
- Added 37 domain cases covering the approved examples, all research levels,
  huge representable deposits, both overflow boundaries, selected/missing
  wallets, immutability, non-success states and malformed inputs.
- Included the shared source in the portable compile and fingerprint manifests.
- Fixed the existing GenomeContractTests fixture's input isolation: after
  capturing both real profile keys, clear them for each test. Existing teardown
  restores both values. Production migration/recovery behavior is unchanged.
- Corrected stale proposal-only wording in the already-approved T01 tuning sheet.

## Decisions and assumptions

- Continue in the existing owned worktree/branch:
  `C:\Users\bevin\.codex\worktrees\fd2-t01-integration\GameDev`,
  `codex/fd2-t01-integration`. The original active checkout and Editor are not
  switched or merged. No original-checkout source edits are part of T03.
- T03 is a pure calculator contract. Callers must supply the final unspent
  ledger balance and the research level frozen for settlement. T04 owns receipt
  lookup, duplicate protection, persistence, retry and pending-result lifetime;
  T10 owns the results-screen integration.
- The approved starter definition supports Hare. Unknown or unsupported selected
  species are rejected; unrelated valid species wallets are retained untouched.
  This does not introduce a Fox tree or new product scope.

## Validation

- Unity 6000.4.6f1 focused EditMode: 37 passed, zero failures/skips at
  `artifacts/fd2-t03-20261026/settlement-editmode.xml`.
- Full clean EditMode rerun: 535 total, 534 passed, zero failed, one skipped at
  `artifacts/fd2-t03-20261026/all-editmode-isolated.xml`. The parity fixture
  requires `CELLSIM_FIELD_DATA_PARITY_MANIFEST`; it was not supplied.
- The earlier full run is retained at `all-editmode.xml`: 533 passed, one failed,
  one skipped. `ProfileSessionKeepsValidProfileWhenAnotherRecordIsInvalid`
  expected its recovery fixture despite an already-occupied recovery slot.
  Inspection found existing SetUp preserved the real keys without clearing
  inputs. Added the three-line isolation fix; no assertion was weakened.
- Verified names, registry types and exact values for both profile keys before
  and after the successful rerun without displaying their contents. The saved
  comparison checkpoint is ignored local evidence, not shared documentation.
- `dotnet build tools/CellSim.Batch/CellSim.Batch.csproj -c Release --nologo`:
  zero warnings/errors. `dotnet .../CellSim.Batch.dll self-test`: both existing
  replay/determinism/provenance/policy checks pass, retained in
  `artifacts/fd2-t03-20261026/portable-self-test.log`.
- Unity generated metadata for the two new scripts. Diff hygiene passed.
  Unity's incidental ProBuilder settings line-ending change was restored;
  there is no project-settings change in the feature.
- `tools/Test-Handoffs.ps1` reports 73 historical missing-artifact/schema errors
  across the journal, none naming this new handoff. Retained the diagnostics at
  `artifacts/fd2-t03-20261026/handoff-validation.log`; no old handoff was rewritten.
- Direct `unity test . --mode EditMode ... -- -nographics` targeted the closed
  isolated project. The existing wrapper rejects any open Editor globally;
  the original Editor was confirmed ready/stopped and was left open.

## Risks and incomplete work

- T04 persistence and T10 UI wiring are not implemented by this calculator.
  Repeated calculation is side-effect-free; it is not proof of exactly-once
  payment, crash recovery, results-screen wiring or a player build.
- No new PlayMode run or balance study was needed for the pure calculator.
  The previous October 24 integration evidence still records seven inherited
  PlayMode failures reproduced on untouched ProjectMain; it is historical,
  not a fresh T03 full-suite acceptance claim.
- No push, merge into the active checkout, or live Trello mutation occurred.

## Next useful step

Implement FD2-T04 around this receipt: check the same profile/journey identity
before calculating, stage both balances and receipt together, save atomically,
and retain the same pending success identity on retry. Reentry/reload/save-failure
tests must prove one payment. T01 and T03 are technically ready for scoped card
closeout; their live board statuses have not been changed here.
