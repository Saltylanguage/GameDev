# fd2-t10-permanent-settlement-results-and-retry

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-27-1137-codex-fd2-t10-permanent-settlement-results-and-retry
- Owner: codex
- Branch: codex/fd2-t01-integration
- Baseline commit: 26ca438
- Date: 2026-10-27
- Supersedes: none

## Summary

T10 connects the accepted 18-round Field Data journey to the existing exactly-once
permanent settlement API and results UI. Saved receipts explain the award;
unpaid results prevent leaving until persistence succeeds; failure forfeits only
journey FD. Work remains local in the isolated checkout.

## Changes

- Added a plain C# presentation controller, `FieldDataPermanentResults`, with
  an explicit owning profile dependency. It formats authoritative saved receipts
  and retains unpaid retry state; it owns no wallet arithmetic or save schema.
- Desktop binds/unbinds the controller. The preview submits terminal success
  or failure, guards pending start/reset/next/exit, and closes the temporary
  wallet only after saved success. Recovery reads existing pending journal inputs.
- The existing shell ViewModel exposes display values and retry intent. The
  Noesis result panel shows source FD, base exchange, conversion research and
  rational multiplier, species deposit, fixed RD bonus and before/after wallets.
  Pending state has an explicit retry button and no confirmed deposit; failure
  shows forfeiture and retained permanent data/Genome unlocks.
- Added11 focused domain cases and3 scene integration tests, with optional
  camera captures. Added one new C# asset with Unity-generated metadata; no
  scene, prefab, package, tuning or profile-schema migration is required.

## Decisions and assumptions

- FD2 extends the accepted FD1/FD2 `FieldDataJourneySession` route:18 rounds,
  three simulations and two shops. The older12-phase Forest Edge POC keeps its
  legacy `RecordSuccessfulJourney` reward behavior. These routes must not share
  duplicate terminal payout callbacks.
- The T04 profile journal remains the sole durable source for payment and retry.
  Display never infers a paid state from an expected award. Frozen profile,
  journey ID and saved inputs retain idempotency across callbacks and reload.
- Pending save blocks Desktop return, close, map/next and direct preview
  start/reset. Failure requires no permanent transaction. A newly observed
  success whose first write never reached disk still needs a live retry before
  termination; this existing persistence limit is unchanged.
- Composition uses direct dependencies; no service locator, global event bus,
  package or parallel store is added. Presentation formatting stays outside the
  domain calculator and XAML contains only layout/bindings.

## Validation

- Unity6000.4.6f1 direct batch CLI targeted the isolated project because the
  wrapper's global Editor guard sees the unrelated original open checkout.
  `artifacts/fd2-t10-20261027/results-editmode.xml`:11/11 new cases passed.
  Covers zero/sub-exchange/normal FD, frozen research, failure retention,
  failed/uncertain writes, pending recovery, profile identity and no duplicate pay.
- `results-playmode.xml`:3/3 new tests passed. Real MainMenu/Desktop flow runs
  the full1800 ticks and both shops, saves and returns; injected persistence
  failure blocks all exit paths, retries once, and a recovered journal reopens.
  Failure retains wallets and Genome fingerprint.
- `results-visual.xml`:3/3 passed with real graphics. Inspected1280x720 camera
  captures `visuals/t10-saved-results.png`, `t10-pending-results.png` and
  `t10-failure-results.png`; settlement/retry/failure text and controls are legible.
  This is automated rendered review, not human acceptance.
- Full clean EditMode691 total:690 passed, zero failed, one existing
  parity-manifest skip at `full-editmode.xml`.
- Portable Release build:zero warnings/errors. Existing deterministic runner
  self-test passes, including full18-round replay (`portable-self-test.log`).
- Related PlayMode:24 total,21 passed,3 failed (`related-playmode.xml`). All
  T10 and T04-T09 focused integration checks pass. The population-control
  expected114/actual90 failure matches the incoming baseline. The two older
  fixed-seed route failures reproduce exactly at pre-T10 HEAD26ca438:
  natural-extinction test reaches Shop, and success seed10100 fails at tick882
  (`pre-t10-route-baseline.xml`). Tracked T10 source was backed up, temporarily
  restored to that HEAD for comparison, then restored byte-for-byte. Its final
  scene rerun passed3/3 (`restored-results-playmode.xml`). No baseline test was
  disabled or changed. Incoming baseline also contains these two failures,
  although its success failure was at tick1061 before T09.
- Exact original profile/recovery preference names/types/values and all four
  protected original source hashes are unchanged after tests. Original Editor
  reports ready. Diff hygiene passes; incidental ProBuilder formatting restored.
  Handoff validator retains73 historical errors, none naming this handoff.
- Local code commit: `1b7d9ab818a5788985ce7e4daeda127d841e4ae2`. T10 moved
  Upcoming Work -> In Progress -> Done and is marked complete. Original card
  description is unchanged; evidence comment posted and visibly read back:
  https://trello.com/c/Cqfr2WyU/146-fd2-t10-show-permanent-settlement-and-failure-results#comment-6ae0c7bb937b17df3d1fc85d.
  Screenshot: `artifacts/fd2-board-20261027/t10-done.png`.

## Risks and incomplete work

- No full PlayMode, player build, human visual acceptance or balance experiment
  is claimed. Historical baseline PlayMode failures remain separate from T10.
- T11 still owns visible permanent Gene Lab purchase/research/loadout controls;
  T12 owns the final end-to-end progression acceptance. No tuning changed.
- Work is local; no push, merge or original-checkout source integration occurred.

## Next useful step

T11: wire permanent wallet, conversion-research and canonical Hare-tree controls
through existing domain/profile transactions and the Noesis ViewModel boundary.
