# fd2-t04-exactly-once-settlement-persistence

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-26-1254-codex-fd2-t04-exactly-once-settlement-persistence
- Owner: codex
- Branch: codex/fd2-t01-integration
- Baseline commit: 72a9543
- Date: 2026-10-26
- Supersedes: none

## Summary

T04 now saves the pending success, then commits both wallet deposits and the
profile/journey receipt in one profile-store JSON update. Duplicate callbacks
return the stored receipt without another save or payment. Failed or uncertain
saves retain the original success inputs for retry. Schema v5 preserves older
wallets/history and retains receipts for the life of the profile.

## Changes

- Added the per-session storage boundary with PlayerPrefs as the production
  implementation. Tests inject an isolated store with separate cached/durable
  state and failures before or after persistence; no package was installed.
- Added immutable pending success inputs and retained settlement receipts to
  profile snapshots, preserving them through every existing copy operation.
- `TrySettleFieldDataJourney` checks the loaded profile identity, success and
  prior receipt, persists unpaid inputs, then saves deposits/completion/receipt
  together. `TryRetryPendingFieldDataSettlement` uses the frozen original
  species, leftover FD and level even if a caller supplies changed inputs.
- Only save success publishes a paid snapshot. Failed writes restore the cached
  store; unpaid pending state stays in memory. Reload refuses to discard it
  while persistence keeps failing. Saved pending state survives session restart.
- Existing legacy completion IDs cannot receive an additional FD2 deposit;
  new FD2 completions count once. Reentrant callbacks and profile switches are
  checked before payment. Each profile has its own journey-ID namespace.
- Schema v5 adds a required receipt journal and optional pending record. v0-v4
  migrate with empty receipts and retain wallets, completion history and the
  earlier raw migration archive. Newtonsoft now writes explicit null pending
  records as well as reading the save. Receipt version, identity, arithmetic
  and completion membership are validated on load. Corrupt/missing journals
  reject the entire store for recovery rather than forgetting payment history.
- Receipt rate/version/bonus fields now freeze as scalar values. Updated only
  output-version expectations in the existing migration tests.
- Added 23 persistence cases and one real PlayerPrefs/scene-reload PlayMode case.

## Decisions and assumptions

- Worktree: `C:\Users\bevin\.codex\worktrees\fd2-t01-integration\GameDev`;
  branch `codex/fd2-t01-integration`; starting local commit `72a9543`.
- Retain all receipts for the profile lifetime. There is no eviction/size cap
  that could lose duplicate protection. Future storage scaling must preserve
  old identities and the ability to return their original receipts.
- The caller must supply final unspent FD and the research level frozen for
  settlement. T10 still replaces the Desktop host's legacy fixed-reward call
  (`GalapagOSDesktopNoesisHost`) with this API and pending-result presentation.
  Existing POC flow remains until that explicit integration.
- User authorized keeping Trello synchronized as tickets finish. T01 and T03
  were marked complete, moved to Done, and verification comments read back:
  T01 `#comment-6adf826daf004653e279a9ce`; T03
  `#comment-6adf82b327c86fd384a48e6b`. Original descriptions are preserved.
  T04 was moved to In Progress before implementation; closeout follows verified
  evidence and the local checkpoint commit.

## Validation

- Full clean EditMode: 558 total, 557 passed, zero failed, one parity-manifest
  skip at `artifacts/fd2-t04-20261026/all-editmode.xml`. All 23 persistence cases
  and the existing 37 calculator cases passed. The first focused run passed
  20 cases before the final edge cases were added; it remains separate evidence.
- Two distinct clean Editor processes passed `ProcessReloadCheckpoint` once
  each: write PID 35032 and read PID 19380. `process-write.xml` and
  `process-read.xml` both record one pass. The second process loaded the first
  process's saved fixture JSON, returned the original receipt for changed
  callback inputs, and made no additional save or deposit.
- Focused PlayMode: 3/3 passed, zero failures/skips at
  `artifacts/fd2-t04-20261026/focused-playmode.xml`: new real PlayerPrefs scene
  reload/no-repayment case plus both existing migration/roundtrip cases.
- Exact profile/recovery preference names, registry types and values match the
  capture before validation; contents were not displayed or published.
- Portable Release build: zero warnings/errors. Existing self-test passes at
  `artifacts/fd2-t04-20261026/portable-self-test.log`.
- Initial compile found the new tests lacked a Newtonsoft reference in the
  Runtime test assembly. Placed the new tests in the existing Editor test
  assembly, which already references that installed dependency; no assembly
  or package configuration was changed. No tests executed in that first attempt.
- Unity generated metadata for the two new files; incidental ProBuilder
  line-ending changes were restored. Source diff hygiene passed.
- Direct CLI targeted only the closed isolated project, leaving the original
  active Editor/checkout source unchanged. No full PlayMode rerun is claimed.

## Risks and incomplete work

- A newly observed success cannot survive termination if no write ever reached
  storage. Keep it pending in the live caller and retry; after the pending save,
  reload can resume it. Failure tests include persistence followed by an error,
  but do not simulate OS power loss or prove physical-disk durability.
- The persistent transaction API is complete; T10 player results wiring and
  T05 conversion research purchases remain. No player build, human visual or
  balance acceptance is claimed. Historical seven full-PlayMode baseline
  failures remain open and were not rerun or retuned for T04.
- Code is local, not pushed or merged into the active checkout. Trello closeout
  explicitly reports that boundary; board Done does not imply publication.

## Next useful step

Continue with T05's saved conversion research level and atomic purchase using
the approved 10/20/30 RD prices and three-level cap. Keep prior receipts unchanged
and freeze the level for future settlements. T10 must switch the player success
flow to the new settlement API rather than calling both reward paths.
