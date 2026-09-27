# S3-08 Fox telemetry clarification

## Scope

Expose how many reproduction candidates passed the energy, mate, and group-cap
gates and reached chance or birth-location processing. Keep simulation
decisions and balance values unchanged.

## Change

`SpeciesReproductionActivity.EligibleAttempts` is derived from failed chance
rolls, unavailable birth locations, and successful attempts. The shared
simulation report serializer writes this as `reproductionEligibleAttempts` for
both Play Mode and experiment reports. Play Mode schema is 9; experiment
schema is 27. The Play Mode Markdown and `CellSim Report` reproduction table
show Eligible next to the blocked gates and success count. The reports state
that `Mating` ticks are pre-resolution FSM decisions and need not equal
resolver candidate evaluations.

The runtime regression assertion uses a single simulation step containing a
successful attempt and a no-location attempt and verifies both are eligible.
An Editor-mode JSON regression also verifies that the shared report serializer
writes eligible attempts, a blocked mate count, successful attempts, and the
reconciliation flag. Existing reproduction funnel assertions continue to cover
the blocked reasons and candidate reconciliation.

## Validation

The user's Safe Mode screenshot exposed one compilation error in the new Editor
regression: this project's NUnit does not support `Assert.Multiple`. It was
replaced with individual `Assert.That` calls. On 2026-09-27, both focused
EditMode checks passed in the installed Unity Editor CLI (report serializer
1/1; reproduction funnel 1/1), and the Editor log contains no compiler errors.
The screenshot's warnings are not the Safe Mode blocker. `unity status` still
lists no connected GUI Editor, so validation used the installed Unity Editor in
CLI test mode. A five-seed, 600-tick Forest Edge experiment then completed on
the authored 36x20 grid. All 15 species/run activity records reconcile, and all
eligible-attempt values match failed chance rolls + no-location outcomes +
successful attempts. Its JSON includes `reproductionEligibleAttempts`, and the
generated Markdown has the Eligible column. Reports are in
`artifacts/s3-08-reproduction-telemetry/`. The first experiment invocation used
a stale scenario path and stopped with `FileNotFoundException`; the retry used
the production scenario path. No species behavior or balance value changed.

## Boundaries

- No Fox mating, food, cooldown, chance, or population tuning.
- No Trello edit or push was requested.
- The pre-existing `LearningIndieDev/ProjectSettings/ProjectSettings.asset`
  worktree edit was left untouched.
