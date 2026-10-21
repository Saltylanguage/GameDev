# Field Data T04–T06 verified closeout — October 17, 2026

T04, T05 and T06 are Done on Trello with all five acceptance criteria checked
on each card. Their original descriptions, ownership and source links remain;
the stale status text was corrected and a completion comment records evidence.
Work is local on `BevBranch`; this checkpoint does not commit or publish code.

## Current evidence

| Check | Result | Artifact |
| --- | --- | --- |
| Focused population controls after Plant-placement correction | 8/8 passed | `artifacts/unity-tests-20261017-005904/EditMode-results.xml` |
| New shared-layer capacity regression before correction | 0/1 passed: expected rejection, observed acceptance | `artifacts/unity-tests-20261017-010046/EditMode-results.xml` |
| Current FieldData EditMode suite after both corrections | 49/49 passed | `artifacts/unity-tests-20261017-010128/EditMode-results.xml` |
| Current route, legacy-entry and terminal-lifecycle PlayMode subset | 5/5 passed | `artifacts/unity-tests-20261017-010227/PlayMode-results.xml` |

The PlayMode run verifies the authored 0.2-second seed-10100 route through
1,800 ticks and both shops, retained offers/choices, repeated Continue/results,
successful-results discard, natural failure, manual End/abandonment and fresh
journey restart. Banked currency, species unlocks, Genome unlocks and feature
IDs are seeded and checked across terminal paths. Legacy route selection keeps
the FieldData session absent. EditMode fixtures cover cleanup/unsubscription,
runner replacement, exactly-once settlement, partial node-bonus retry and
extinction before payout at ticks 100, 600 and 1,800.

## T06 corrections

- Plant addition candidates exclude existing Plant resources. An accepted batch
  adds five resources; four valid spaces reject the whole request without charge.
- Population capacity counts both a creature and Plant sharing a cell. The new
  regression reproduced the defect before correction and passes afterward.
- Added checks compare the next seeded placement after a rejected request with
  an uninterrupted matched session and confirm interventions do not change
  natural activity/birth/death counters or settled payout populations.

The user force-closed the stalled interactive Editor. No Editor process remained
and exclusive access to the abandoned lock succeeded; the lock was preserved
at `artifacts/unity-recovery-20261017-005903/UnityLockfile.stale`. Supported clean
test runs subsequently compiled and completed. No Library deletion, package
change or user-profile reset was used.

## Verified Trello handoff

- [T04 completion comment](https://trello.com/c/LKzm9j7r/122-fd-t04-run-all-18-rounds-in-one-retained-journey-session#comment-6ad301cf1ecb1ff09690eed0)
- [T05 completion comment](https://trello.com/c/3cGXubBh/123-fd-t05-settle-awards-once-and-enforce-the-journey-wallet-lifecycle#comment-6ad301f470ca2ecd6186df37)
- [T06 completion comment](https://trello.com/c/CMld0U0x/124-fd-t06-add-the-six-atomic-population-controls#comment-6ad30210c89e24dbe6ddb314)

Each comment was visibly read back, followed by the card's Done-list state and
100% checklist. T06 screenshot: `artifacts/fd-acceptance-20261017/trello-t06-done.png`.

## Next work and limits

T07 signed species effects are next. T13 still needs rendered evidence and
independent human review. T14 needs a common Unity/headless action fixture;
rebuild/refreeze current-source inputs after the T06 changes and preserve the
earlier frozen artifacts. T15/T16 retain their recorded timing and internal
self-play requirements. These focused suites are not a full-suite, player-build,
visual, balance or human accept/revise result. Unrelated terrain metadata edits
and recovery assets remain in the working tree.
