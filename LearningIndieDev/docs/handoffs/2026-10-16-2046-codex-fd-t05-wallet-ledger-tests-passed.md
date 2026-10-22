# FD-T05 wallet ledger and journey verification checkpoint

**Date:** 2026-10-16 20:46 America/New_York  
**Branch:** `BevBranch`  
**Tickets:** [FD-T04](https://trello.com/c/LKzm9j7r/122-fd-t04-run-all-18-rounds-in-one-retained-journey-session), [FD-T05](https://trello.com/c/3cGXubBh/123-fd-t05-settle-awards-once-and-enforce-the-journey-wallet-lifecycle)

## Implemented and verified

- The journey session and ledger remain separate from permanent profile
  currency. Successful round income and each six-round node-clear reward have
  distinct receipts and stable identities; failure forfeits only the journey
  wallet; successful exit can discard the remainder only after all 18 round
  and three node receipts exist.
- Fixed `FieldDataJourneySession.Dispose` so a failed/abandoned session can
  transition to `Closed` after unsubscribing from manager events.
- Updated the route lifecycle fixture to use a deterministic Hare rule and
  predator-free starting state. This validates session progression without
  claiming the current Forest Edge scenario is viable for 1,800 ticks.
- Connected Editor recompile completed with no compilation errors.
- `FieldDataJourney` EditMode suite passed **16/16** at
  `artifacts/unity-tests-20261016-204629/EditMode-results.json`.
- `FieldData` EditMode suite passed **24/24** at
  `artifacts/unity-tests-20261016-204629/EditMode-results.json`.
- Added a 100-tick boundary extinction case confirming the session fails with
  no payout or decision offer.
- The Editor reports no current compilation failure. Its console recorded one
  Pipeline main-thread request timeout during testing, but the focused test
  wrapper completed successfully with all 24 tests passing.
- `git diff --check` passed for the scoped source, tests and docs.

## Status and limits

FD-T04 and FD-T05 remain In Progress; their Trello acceptance criteria remain
open. EditMode coverage does not verify preview rendering, Desktop/map launch,
Noesis wallet/results UI, all reward/shop entry paths, or the complete playable
flow. No balance conclusion or 200-seed result is claimed. The current whole-
board counts remain derived from the last verified board snapshot plus the
verified T05 move: 16 Done, 2 In Progress, 11 Current Work and 65 acceptance
items open.

## Next step

Continue with the remaining T05 integration boundaries and tests, then verify
the preview flow and reconcile the acceptance checklist before closing either
ticket. Keep the ten pre-existing terrain importer metadata edits untouched.
