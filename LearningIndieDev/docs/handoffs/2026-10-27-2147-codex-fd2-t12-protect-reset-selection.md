# fd2-t12-protect-reset-selection

[Working state](../WORKING_STATE.md) | Status: complete

- Handoff schema: 1
- Handoff ID: 2026-10-27-2147-codex-fd2-t12-protect-reset-selection
- Owner: codex
- Branch: codex/fd2-t01-integration
- Baseline commit: fa984918
- Date: 2026-10-27
- Supersedes: none

## Summary

Bevin accepted the live success receipt and Gene Lab cost/owned-active labels.
Manual reset selection exposed a trap: buying Population Reserves immediately
activated it and closed the empty-loadout free replacement window. Live state
was88 Hare Data,0 RD,allfive owned,Population Reserves alone active.

## Changes

Local code330d94d9142b4a06c5fe59ee5241cf64c9dd5041 blocks new-node purchases
while a paid reset remains empty. APPLY REPLACEMENT FIRST explains that buying
activates the new node. Owned-node draft/free apply remain available; ordinary
eligible purchases work after applying. No schema,prices,immediate-activation
or20RD-per-replacement changes. Domain/persistence APIs are unchanged.

The new persistence case checks no charge/save/draft loss,free apply,and later
normal purchase. Bound scene coverage verifies the disabled purchase control
and explanation before continuing the purchase/over-capacity/reset workflow.

## Validation

- New NUnit case directly invoked before fix reproduces expectedFalse/actualTrue.
  Pipeline live runner selected zero cases,so those calls are not validation.
- Clean full EditMode702 total:701passed,0failed,1existing parity skip.
- Focused graphicsPlayMode8/8:four Gene Lab and four end-to-end acceptance cases.
- Final guard capture case1/1; t12-reset-protects-free-selection.png inspected.
- XML/logs under artifacts/fd2-t12-20261027/reset-guard-*; graphics under
  reset-fix-visuals/. Exact original profile/recovery keys remain unchanged.
- No fullPlayMode,player-build,new portable-build or balance approval claim.

## Live recovery and remaining review

Preserved the fifth-node purchase. Two actual controlled successes at research1
earned84 Hare Data/+20RD. A fresh20RD reset leaves172 Hare/0RD,fiveowned,
zeroactive,0/8selection,five saved successes. Long Vision is highlighted and
Add to Selection verified enabled. Bevin requested manual selection;do not
choose/apply nodes for him. live-reset-selection-restored.png inspected.

At the initial handoff T12 remained incomplete/InProgress,with reset/free-
replacement and failure clarity pending;the final decision below supersedes
that state. Original source was
not changed. No push,merge or source integration. Temporary isolated demo
storage-prefix/helper files must be restored/removed after review and stopping
the owned Editor. Current demo PID12792;separate prefix retained.

## Final human decision and cleanup - 21:55 EDT

Bevin manually applied Tough Hide,Long Vision,Long Stride and Population
Reserves;Efficient Digestion remains owned/inactive. User: "okay yes that
worked". Reload preserved the saved build. A real journey End/Confirm forfeited
20FD,kept172Hare/0RD,fiveowned/fouractive andfive receipts.
Bevin then explicitly answered "Yes — accept T12 clarity" to the final
success/GeneLab/correctedreset/failure review,with balance/release/push separate.
The owned demo Editor was stopped/closed gracefully. Temporary storage prefix,
Editor helper and generated image-import residue were removed. Separate demo
save archived under artifacts;original saves/source remain protected.
T12 completion is now authorized;no push or source integration.

Trello final state:marked complete in live Done;[acceptance comment](https://trello.com/c/wxVgh3Q6/148-fd2-t12-verify-migration-and-end-to-end-permanent-progression#comment-6ae1567563aaa80267e1df6a) posted/read back;original description preserved. Board proof:`artifacts/fd2-board-20261027/t12-done.png`.
