# GLJP-01 Open-ground Sprint implementation

[Working state](../WORKING_STATE.md) | Status: in progress

- Handoff schema: 1
- Handoff ID: 2026-11-11-1400-codex-gljp01-open-ground-sprint
- Owner: codex
- Branch: BevBranch
- Baseline commit: a2c38b3
- Date: 2026-11-11
- Supersedes: none

## Summary

Wired the diagram-approved Sprint Gene Lab research node to a temporary journey Mutation. This advances GLJP-01 by implementing the approved open-ground movement concept using the simulation's currently available exposed terrain. The research cost is 10 Hare Data and prerequisite ordering remains diagram-driven.

## Changes

- Added `open-ground-sprint` to the Hare Field Data Mutation catalog as a one-rank movement-speed upgrade with provisional +0.5 movement speed.
- Mapped owned `hare.open-ground-sprint` research to eligibility; equipping the node is not required. The effect is temporary through existing journey progression.
- Added UI-facing name/description and updated the authored Gene Lab node description.
- An equipped Born Burrower starter suppresses Sprint Mutation eligibility and offers, matching the approved exclusion.
- Added a regression covering owned and unowned eligibility, offer availability, movement effect, rank cap, and the Born Burrower exclusion.
- Updated PDD, SDD, morning review, and finish plan to record the provisional value and current terrain boundary. Bare and Grass are the only current exposed terrain types; burrowed Hares do not move. If covered tiles are added later, the bonus must be suppressed on them.
- Posted and read back a progress comment on [GLJP-01](https://trello.com/c/dR2z6ENf); the card remains In Progress.

## Decisions and assumptions

- The existing movement-speed stat is reused for Sprint to avoid a second movement system.
- +0.5 is a provisional first-pass balance value, matching the existing basic movement Mutation scale. It is not balance-approved.
- No new terrain, cover, or Owl mechanics were added.

## Validation

- `git diff --check`: passed with only existing LF-to-CRLF warnings.
- Focused Unity EditMode command: `unity test . --mode EditMode --filter OpenGroundSprintResearchUnlocksOneRankMovementMutation --output artifacts/gljp-sprint-focused.xml --timeout 180 --non-interactive`.
- First focused CLI attempt timed out after 180 seconds without XML. After closing the stalled Editor, the focused Sprint rerun passed 1/1 in `artifacts/gljp-sprint-focused.xml`; the related `GestationUpgradeResearchUnlocksThreeRankFasterGestationMutation` regression passed 1/1 in `artifacts/gljp-gestation-focused.xml` on Unity 6000.4.6f1. Both focused tests executed against the current project; full-suite and visual acceptance remain pending.
- Current-worktree state and authored map wiring were statically reviewed; the Sprint asset GUID is present in `HareGenome.asset`.

## Risks and incomplete work

- Covered terrain is not a playable terrain type yet. When implemented, the movement application path needs a covered-tile gate before Sprint can be considered fully terrain-specific.
- The +0.5 strength needs balance review.
- GLJP-01 still needs current-revision Unity validation, deeper branch and ranked UI review, and player-facing acceptance. Existing worktree contains unrelated/pre-existing uncommitted changes; nothing was committed or pushed.

## Next useful step

Recover Unity CLI/Editor startup, run the Sprint and Faster Gestation focused regressions plus current-revision acceptance, then review the ranked Gene Lab and journey Mutation screens. Continue GLJP-01 until Sprint value and all player-facing behavior are accepted.

## November 11 full-suite update

Current Unity 6000.4.6f1 full EditMode passed 768/768 with one existing replay-manifest skip (`artifacts/gljp-full-editmode-rerun.xml`). Full PlayMode completed at 97 passed, 4 failed, 2 skipped (`artifacts/gljp-full-playmode-rerun.xml`). Failures: HeightenedSensesOffersBothMutationsInFieldData currently offers Keen Senses without Long Vision; the authored seed-10100 route loses its last Hare at tick 1210 after plant extinction; and two terrain capture tests require a graphics-capable runner because batchmode does not invoke WaitForEndOfFrame. The focused senses rerun reproduced its assertion failure. A speculative offer change was tested and reverted; no fix has been applied. The GLJP-01 Trello comment records this status. GLJP-01 remains In Progress.

## November 11 follow-up: linked Heightened Senses offer fix

The earlier validation paragraph records the prior failure and is superseded by this follow-up. `FieldDataJourneySession.GetOrCreateRewardChoiceSet()` now retains both Long Vision and Keen Senses in the same three-choice offer whenever both are eligible, swapping out a lower-priority tail entry while preserving deterministic ordering and the original first choice. Added `HeightenedSensesOffersBothLinkedMutationsTogether` to `FieldDataJourneySessionTests`.

Validation on Unity 6000.4.6f1:

- Focused session regression passed 1/1: `artifacts/gljp-senses-session-focused-rerun.xml`.
- Heightened Senses PlayMode passed 1/1: `artifacts/gljp-senses-playmode-fixed-final.xml`.
- Seed Dispersal research-to-journey PlayMode passed 1/1: `artifacts/gljp-seeddispersal-journey-focused-rerun.xml`.
- Full EditMode: 769 passed, 0 failed, 1 existing replay-manifest skip: `artifacts/gljp-editmode-final-senses-fix.xml`.
- Full PlayMode: 98 passed, 3 failed, 2 skipped: `artifacts/gljp-playmode-final-senses-fix.xml`.
- Remaining PlayMode issues: authored seed-10100 route loses its last Hare at tick 1210 after plant extinction (GLJP-06); two terrain capture tests require a graphics-capable runner because batchmode does not resume `WaitForEndOfFrame`.
- `git diff --check` passed with line-ending warnings.

Posted this update to GLJP-01 and verified the comment read-back. The card stays In Progress pending deeper visual/player review and balance acceptance of provisional Mutation values.
