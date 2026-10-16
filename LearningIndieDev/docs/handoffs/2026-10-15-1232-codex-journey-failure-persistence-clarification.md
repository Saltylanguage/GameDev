# Journey failure persistence clarification

[Working state](../WORKING_STATE.md) | Status: complete

- Handoff schema: 1
- Handoff ID: 2026-10-15-1232-codex-journey-failure-persistence-clarification
- Owner: Codex
- Branch: ProjectMain
- Baseline commit: 405df647
- Date: 2026-10-15
- Supersedes: 2026-10-15-0946-codex-october-14-journey-meeting-context (failure-persistence ambiguity only)

## Summary

Josh clarified the October 14 meeting's failure rule. This resolves the
previous handoff's uncertainty about what survives a failed journey without
changing the current prototype.

## Changes

- Updated the meeting record, durable project context, journey prototype,
  scientific-data economy, product-brief supersession note, and working-state
  index to state the same failure boundary.
- Left other contributors' local journey-map changes and Unity assets alone.

## Decisions and assumptions

- Failure ends the journey without cash-out. Currency earned and temporary
  upgrades acquired in that run are lost.
- Permanent Desktop upgrades and unlocked upgrade or feature availability
  remain on the profile, including permanent unlocks earned through an
  accomplishment in the run. Exact conditions and award timing are undecided.
- Mating 100 rabbits to unlock a Burrow Genome upgrade is only a hypothetical
  illustration. It is not an approved threshold or an implemented mechanic.

## Validation

- Documentation-only change. No Unity build, test suite, or balance run was
  executed for this clarification. Git whitespace and scoped-diff checks are
  recorded in the commit workflow.

## Risks and incomplete work

- The prototype does not yet enforce the failure settlement, permanent unlock
  recording, or full Field Data spending menu.
- Wallet categories, unlock conditions, award timing, and the first playable
  implementation slice remain open design work.

## Next useful step

Design the run ledger and failure transition so temporary state clears while
permanent profile progression remains. Define a small, explicit permanent
unlock rule with Bevin before implementing or balancing it.
