# Field Data: round-7 reward exhaustion

Date: October 19, 2026. Local `BevBranch`, based on `acc371a`.

Bevin reported disabled mutation cards after Shop 1, at round 7, despite 133 FD.
The screenshot showed General Movement, Attack and Block with legacy 5-Data
prices. Those were not legal Field Data Hare reward offers.

## Confirmed cause

`FieldDataJourneySession.IsOrdinaryRewardEligible` excluded a skill as soon as
its level exceeded zero. `SpeciesProgression.TryApplyRunUpgrade` likewise
rejected another stat snapshot with the same ID. Choosing every ordinary reward
in rounds 1–5 plus the first node reward therefore consumed all six Hare skills.
Round 7 generated an empty choice set. The ViewModel incorrectly filled its
missing slots using the old paid reward labels, while correctly disabling the
nonexistent offers. The unresolved empty free choice also kept population
controls closed until Skip.

A clean pre-fix regression, taking every free reward and purchasing in shops,
failed specifically at round 7 with an empty pool:
`artifacts/unity-tests-20261019-144355/EditMode-results.xml`.
Earlier route tests primarily skipped free choices and missed this path.

## Correction

- Reuse the catalogue's six Hare skill IDs and existing max levels (10).
  Session eligibility and Field Data reward application share that cap.
- Add a Field Data-specific progression entry point using the existing common
  validation/application code. Ordinary authored run-upgrade uniqueness,
  repeatable population reinforcement, species checks, prerequisites and
  exclusions retain their existing behavior.
- Preserve the separate three-copy shop family cap; shop purchases do not
  consume ordinary skill levels. No prices or natural ecology settings changed.
- Never fill missing Field Data slots with legacy reward cards. Hide unavailable
  slots; if genuinely empty, explain Skip. Clear stale previous-boundary status.
- Regression coverage takes every free reward through both shops, tests all six
  ordinary caps and empty-pool Skip, and exercises round-7 free choice followed
  by a 10-FD population purchase through the player-facing commands.

## Verification

- Clean focused EditMode (`FieldData|SpeciesUpgradeContractTests`): **91 passed,
  0 failed, 1 dedicated parity fixture skipped**, under
  `artifacts/unity-tests-20261019-144728/`.
- Portable runner Release build: zero warnings/errors; `self-test` passes,
  including the existing full 18-round replay/settlement checks.
- Clean focused Field Data PlayMode: **4 passed, 0 failed, 2 separate graphics
  checks skipped**, `artifacts/unity-tests-20261019-144826/`. The player-command
  fixture takes all six earlier rewards, buys from Shop 1, explicitly enters
  Simulation 2, takes a free round-7 mutation without charging FD, and purchases
  Add Hare for exactly 10 FD. A fully capped future-state fixture hides all
  missing cards, explains Skip and permits continuation after Skip.
- Clean rendered Field Data checks: **6/6 at 1280x720**,
  `artifacts/visual-evidence-20261019-144940/`. Focused ownership/reorder/round-7
  check: **1/1 at 1920x1080**, `artifacts/visual-evidence-20261019-145041/`.
  Inspected the round-7 enabled mutation cards and post-purchase population
  controls. These are synthetic UI/ownership fixtures, not balance evidence.
  The separate authored route remains included in the six-test rendered pass.

The original Editor was closed; all Unity checks used isolated clean test
Editors. No user Editor was closed or interrupted. Full-suite and player-build
validation were not rerun for this correction.

This corrects reward availability and presentation. Frozen balance/replay
artifacts remain historical; it does not establish new balance acceptance.
No Trello update, commit or push is claimed.
