# October 14 journey meeting context

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-15-0946-codex-october-14-journey-meeting-context
- Owner: Codex
- Branch: ProjectMain
- Baseline commit: f146e33a
- Date: 2026-10-15
- Supersedes: none

## Summary

Recorded the October 14 team meeting's journey decisions alongside the
premeeting proposals. This separates what the team settled from phase gates,
wallet details, and purchase values that still need playtesting or design.

## Changes

- Added the meeting outcomes to the existing October 14 brief without erasing
  its agenda and Josh's earlier currency proposal.
- Updated the durable project context, journey prototype, economy direction,
  product brief supersession note, and working-state index.
- Left Unity scenes, assets, profile saves, and simulation rules untouched.

## Decisions and assumptions

- Bevin guides initial phase-clear criteria; no fixed population or event
  formula was approved. Historical tick and population research stays labeled
  as research.
- Failure ends the whole journey with no cash-out or long-term reward earned
  on that journey. The wording about "long term data" needs clarification
  before any previously banked profile progression could be affected.
- Field Data lasts through the journey. Other currency types can leave a
  successful journey for Desktop Genome upgrades; exact wallet categories
  and amounts are open.
- Intended Field Data options cover removing Hares, adding or removing Foxes
  and Plants, and a random-event gamble. Costs, caps, timing, and first-slice
  scope remain open.
- Josh's premeeting per-species/per-biome wallets and fixed node payouts are
  recorded as proposals; the supplied meeting answers did not explicitly
  settle those details.

## Validation

- Documentation-only change. No Unity build, test suite, or balance run was
  executed for this context update.

## Risks and incomplete work

- The current journey prototype does not implement the approved failure
  settlement or the full set of Field Data purchases.
- The older six-round product brief has different end-of-run rules; its
  canonical-journey supersession is called out at the top of that brief.
- The [S4-03](https://trello.com/c/xyDDiiRC) and
  [S4-07](https://trello.com/c/e5xd0K7c) closeout descriptions say previously
  banked Desktop data stays intact. The supplied meeting note says to "lose
  any long term data." Clarify the intended scope before changing saved data.
- Exact phase gates remain Bevin's playtest decisions. Do not hardcode a
  historical research threshold as a production rule.

## Next useful step

Bevin should record one provisional phase-clear gate and the observations
behind it for the next bounded playtest. Salty should specify the journey
failure and currency settlement state changes, clarify banked-profile-data
scope, and choose the first Field Data purchase slice with Bevin. Then use
current-code deterministic runs and in-game review to tune the values.
