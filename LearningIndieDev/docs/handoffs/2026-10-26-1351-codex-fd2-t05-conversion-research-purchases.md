# fd2-t05-conversion-research-purchases

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-26-1351-codex-fd2-t05-conversion-research-purchases
- Owner: codex
- Branch: codex/fd2-t01-integration
- Baseline commit: 7e6d659
- Date: 2026-10-26
- Supersedes: none

## Summary

T05 adds persistent per-profile conversion research with the approved 10/20/30
RD prices and three-level cap. A purchase saves its RD deduction and new level
together before publication. Future settlements use the owned level, while
pending and completed receipts retain their original conversion terms.

## Changes

- Profile schema v6 stores a required conversion research level. v0-v5 migrate
  to zero without changing existing wallets, receipts, pending inputs or raw
  recovery archives. Missing/null/out-of-range v6 research is rejected for
  recovery instead of silently resetting purchased progress.
- Added `TryPurchaseConversionResearch(profileId, expectedLevel, out error)`.
  The profile and expected level identify the one permitted purchase at that
  level. Wrong/stale/repeated requests, insufficient RD, cap, reentry and failed
  saves change no published purchase state. An uncertain save can be retried
  using the same expected level without a second deduction.
- Reused the existing profile save transaction; no new storage abstraction,
  package, asset or configuration was introduced. Research never spends a
  species wallet or changes another profile's level.
- Every snapshot-copy path retains the research level. Pending successes keep
  their frozen level even when research is purchased before their retry.
- Added a settlement overload that derives the level from the loaded profile.
  Fresh requests through the older explicit-level overload must match the
  profile's owned level, preventing unpurchased multiplier claims. Duplicate
  receipts and pending-success retries retain their earlier inputs.
- Added 25 research cases, a distinct-process research checkpoint and a real
  PlayerPrefs/scene-reload case. Existing T04 fixtures now purchase level one
  before using it; all 23 cases still pass. Updated output-schema expectations.

## Decisions and assumptions

- Continue in the owned isolated worktree
  `C:\Users\bevin\.codex\worktrees\fd2-t01-integration\GameDev`, branch
  `codex/fd2-t01-integration`, starting at local `7e6d659`.
- Keep the original expected level when retrying a research purchase. Reading
  the newly advanced level creates a request for the next purchase, not a retry.
  This is sufficient identity because each capped level can only be bought once.
- Research is shared among species within one profile, and independent between
  profiles. Exact costs/cap come from the existing canonical T01 definition.
- T11 still owns visible Genome Lab controls/retry presentation; T10 still owns
  player result integration. This ticket completes the persistence/domain API.
- The user authorized updating Trello as tickets finish. T05 was moved to
  In Progress before implementation; completion closeout follows validation.

## Validation

- Focused research EditMode: 25/25 passed at
  `artifacts/fd2-t05-20261026/research-editmode.xml`.
- Full clean EditMode: 583 total, 582 passed, zero failed, one retained-manifest
  parity skip at `artifacts/fd2-t05-20261026/all-editmode.xml`. This includes
  all 48 profile persistence/research cases and the existing calculator cases.
- Before adding the research cases, the adapted 23 T04 cases passed separately
  in `persistence-baseline.xml`; no T04 assertions were weakened or disabled.
- Portable Release build: zero warnings/errors. The existing deterministic
  runner self-test passed at `portable-self-test.log` in the same artifact folder.
- Distinct clean Editor processes 24244 (write) and 32472 (read) each passed
  `ResearchProcessReloadCheckpoint` once, in `process-write.xml` and
  `process-read.xml`. The reader loaded the saved level/deduction, rejected
  the old purchase without another write, and bought the next level for its
  correct 20 RD cost. The fixture JSON crossed the process boundary.
- Focused PlayMode: 4/4 passed, zero failures/skips at `focused-playmode.xml`:
  real PlayerPrefs research/scene reload, prior settlement/no-repayment scene
  reload, and both migration cases. The research case retains level one,
  purchases level two, and verifies 11/12-data deposits and the earlier receipt
  after another scene reload.
- Exact profile/recovery preference names, registry types and values match
  the pre-validation capture. Their contents were not displayed/published.
- Diff hygiene passed. The incidental Unity ProBuilder line-ending change
  is excluded from the feature; no project configuration change is intended.

## Risks and incomplete work

- The purchase API and owned-level settlement behavior are implemented. Gene Lab
  buttons and player result integration remain T11/T10; their existing POC flow
  is not a completed FD2 player loop.
- No player build, human visual or balance acceptance is claimed. The historical
  seven full-PlayMode baseline failures remain open; a focused pass does not
  establish that the full PlayMode suite is green.
- Tests model failed and uncertain writes; no OS power-loss guarantee is made.
  Code remains local until explicitly published, and the active checkout's
  source/Editor is preserved.

## Next useful step

Continue with T06's Hare Genome path license: charge exactly 10 Hare Data once,
preserve other wallets, persist the license and prevent duplicate purchases.
T10/T11 must use the saved research level and keep original request identities
on retry when the player-facing transaction controls are wired.
