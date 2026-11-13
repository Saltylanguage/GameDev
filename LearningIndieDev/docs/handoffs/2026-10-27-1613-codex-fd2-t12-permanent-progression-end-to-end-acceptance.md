# fd2-t12-permanent-progression-end-to-end-acceptance

[Working state](../WORKING_STATE.md) | Status: in-progress

- Handoff schema: 1
- Handoff ID: 2026-10-27-1613-codex-fd2-t12-permanent-progression-end-to-end-acceptance
- Owner: codex
- Branch: codex/fd2-t01-integration
- Baseline commit: 5157926
- Date: 2026-10-27
- Supersedes: none

## Summary

T12 verifies earned permanent progression across new and migrated profiles.
Automated end-to-end cases now complete journeys, settle once, buy through the
bound Gene Lab controls, reset, reload, launch the saved configuration and
retain progress on failure. A discovered Field Data reopen auto-start defect
was corrected. Human clarity review is still required before closing T12.

## Changes

- Added4 PlayMode cases: zero-wallet new profile, legacy v2 generic currency,
  local v3 species wallets and incoming v3 Rabbit wallets. No direct permanent
  currency grant or pre-purchased Genome is used by the new-profile case.
- Desktop reopen now treats configured Field Data routes as map-driven journeys,
  preserving Ready until explicit start. The previous condition checked only
  the older POC JourneyActive flag and auto-started a prepared FD run. Other
  standalone and POC entry behavior is retained. The new loop test reproduced
  the failure before the three-condition fix and passes after it.
- Test setup uses existing runtime settings/rule-edit APIs, without editing any
  authored asset. All free rewards are skipped, both shops traversed through
  the shell/map and no shop purchases made. Saved receipt replay with altered
  proposed FD returns the original award, then actual storage reload verifies
  one payment. Confirmed manual End after the first paid round verifies forfeiture.

## Decisions and assumptions

- This is a transaction/navigation fixture, not production ecology or balance
  approval. Actual fixture: ForestEdge,36x20, seed10100,200 plants/20 Hares/0 Foxes,
  explicit starting populations, max/min population0,1800 ticks,0.1 step interval.
  Hare movement/reproduction disabled, metabolism0, starting/max energy100;
  remaining authored rule values retained. No permanent Genome on first launch.
- The stable20 Hares yield20 FD per round for18 rounds plus10 FD at each of3
  node clears:390 FD. Base conversion39 Hare Data. The first success pays39
  Hare Data and10 RD; buying research level1 costs10 RD, license10 Hare Data,
  four nodes20 Hare Data. Two more successes each pay42 Hare Data and10 RD.
  One20 RD reset and a free one-node replacement leave93 Hare Data/0 RD.
  These are fixture arithmetic observations, not a viable-player pacing claim.
- v2 preserves73 generic currency as RD; incoming v3 combines5 generic+70 RD
  and preserves9 Rabbit as Hare Data; local v3 keeps42 RD/19 Hare/7 Fox Data.
  Old canonical root ownership/license, inactive Fox unlock, feature/species
  unlocks and recovery JSON are retained. Legacy completion IDs are retained
  where supported; local species-wallet v3 never supported completion fields.
- T12 explicitly requires human confirmation of result and Gene Lab clarity.
  Automated screenshots cannot satisfy that requirement. Keep the live card
  incomplete until Bevin reviews the concrete evidence. No push, merge or
  original-checkout source integration is authorized.

## Validation

- Direct Unity6000.4.6f1 batch CLI targets the isolated checkout. Original open
  Editor is not used to mutate or test this isolated source.
- Initial4/4 failed: the new loop exposed the reopen auto-start defect; migration
  fixtures incorrectly required exact play time despite scene activity and
  mixed unsupported completion fields into local v3. Corrected fixtures to
  check no lost time and use the actual supported source schemas.
- Navigation-corrected run1/4 passed: fresh earned loop passed; all migrations
  reached saved success but tried to click a node in the same frame that a
  Research transaction rebuilt its containers. Added the existing one-frame
  Noesis layout wait. Final `acceptance-ui-timing.xml`:4/4 passed.
- Full clean EditMode701 total:700 passed,zero failed,one existing parity skip
  at `artifacts/fd2-t12-20261027/full-editmode.xml`.
- Portable Release build and deterministic runner self-test passed with zero
  build warnings/errors. Broader PlayMode33 total:29 passed,4 failed. All29
  focused cases pass; each wider failure message exactly matches T11 baseline
  (catalog7-vs-9, population114-vs-90, natural failure reaches Shop, tick882).
  See `related-playmode.xml` and `baseline-comparison.json`.
- Final graphics `acceptance-visual-final.xml`:4/4 passed. Ten current1280x720
  captures under `visuals-final/` inspected. The initial failure fixture directly
  removed cells and produced intentionally unreconciled statistics; the final
  fixture uses the actual End/Confirm commands so both outcome and accounting
  are valid. Initial/intermediate evidence is retained, not used for human approval.
- Exact original preference names/types/values and four protected source hashes
  are unchanged. Original Editor reports ready. A stale empty owned Git index
  lock was removed only after owner/age/no-Git-process checks. No source was
  copied to the original checkout and no push occurred.
- Code/tests checkpoint: `ada175980a31d5d8f095a107de15bf15b2146e5e`.
  [Concrete human review](../FD2_PERMANENT_PROGRESSION_ACCEPTANCE.md) is ready;
  T12 remains incomplete pending Bevin's clarity decision.

- Automated evidence posted and visibly read back on T12; original description
  retained. Card remains incomplete in live In Progress, because its human
  clarity requirement is outstanding:
  https://trello.com/c/wxVgh3Q6/148-fd2-t12-verify-migration-and-end-to-end-permanent-progression#comment-6ae10ab7d93167a216017b9b.
  Proof: `artifacts/fd2-board-20261027/t12-awaiting-review.png`.

## Risks and incomplete work

- Human clarity acceptance remains pending. Do not mark T12 or the whole FD2
  slice accepted based on this automated evidence alone.
- Production seed10100 previously fails in the older authored success test;
  the controlled survival fixture does not resolve that ecology/content issue.
  No current balance, pacing, fun, full PlayMode or player-build claim.
- The existing route is selected through the Field Data route control in the
  Simulation map; the older POC route remains available. This test does not
  silently change the product's default route or approved tuning.

## Next useful step

Ask Bevin to confirm the three concrete result/failure/Gene Lab screens in the
acceptance report. Record that human decision separately from test results
before the T12 Done transition. Automated work and preservation checks are complete.
