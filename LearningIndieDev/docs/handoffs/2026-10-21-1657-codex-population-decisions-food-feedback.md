# Population decisions and food feedback

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-21-1657-codex-population-decisions-food-feedback
- Owner: codex
- Branch: codex/audio-direction-and-sourcing
- Baseline commit: 6ffb7b10
- Date: 2026-10-21
- Supersedes: none

## Summary

Josh approved building refresh/event choices and food-pressure feedback after the
population-pool work. The local implementation adds the six reviewed Field Data
population controls, two provisional encounters between the existing simulations,
and observed food feedback. Unchecked Hares can still exhaust food and starve.
The encounter quantities are playtest defaults; no ecology experiment or natural
species/scenario rebalance ran. No commit, push, or Trello write was performed.
The checkout already contained substantial preceding local work, which remains.

[Feature contract and tunable defaults](../POPULATION_DECISIONS_PLAYTEST.md).

## Implementation

- Round decisions offer +/-1 Hare, +/-1 Fox and +/-5 Plants, each 10 Field Data.
  Validation, suitable positions, pool capacity, global limits and the final
  player-individual floor are checked before changing the world or charging.
  The helper/manager exposes runner operations; Noesis commands coordinate the
  existing run wallet. Controls lock after a Mutation commits or outside a round
  boundary. Layer-preserving removal and zero-food Plant configuration are tested.
- Rest Stop offers up to 10 Hares within the pool, 10 Plants, or no change.
  Seed Cache offers disclosed 50% +10 Plants / 50% -5 Plants, or leaving; it is
  free in this slice. Both cache outcomes must be feasible before opening.
  Serialized values freeze into immutable options during journey preparation.
  Run seed plus stable node ID fixes the roll; cancel/reopen cannot reroll it.
  Claims are one-time and continue into the connected existing second simulation
  without a stat-condition requirement, reseeding, or time advance.
- Unity AssetDatabase generated RestStop and SeedCache assets/metas and linked
  all three first-cycle routes. Existing GUIDs and later placeholder nodes remain.
  The idempotent authoring command is
  `CreateSpeciesScenarioAssets.CreatePopulationEncounters`.
- The live ledger adds net round Plant-count change and EMPTY when depleted.
  Decision/encounter views show supply status, pool capacity, current Hares, and
  cumulative starvation deaths. This is observed supply, not a nutrient forecast.
  Visual review fixed ledger crowding and three-digit Field Data label clipping.
- `PopulationIntervention` records signed actual population changes, source,
  tick, phase and placement seed in order. Checkpoints retain this list. Live
  interventions are discarded on runner restart; the prior reinforcement API
  keeps its existing restart-baseline behavior. Domain availability checks do
  not consume entity IDs or random state.
- Schema 41 exports action records and `REM` in whole-run stat lines. Explicit
  removals reconcile final populations without creating simulation death events.
  Replication fitness excludes paid population changes. Existing mortality-rate
  denominator conventions remain; old report artifacts were not edited.

## Verification actually executed

- Final EditMode suite: **404/404 passed**, Live via `CellSim.ps1 -Command Test
  -Execution Auto -Mode EditMode`.
  [Result](../../artifacts/unity-tests-20261021-165319/EditMode-results.json).
- New Desktop/Noesis integration: **1/1 passed at 1280x720** and **1/1 at
  1920x1080**. It invokes all six rendered button commands, verifies 60 Data
  charged without time advance, drives Plant supply to zero through the UI,
  checks rejection without funds, claims a refresh, cancels/reopens a seeded
  cache, verifies its actual result and one-time claim, and continues to round 7.
  [720p](../../artifacts/visual-evidence-20261021-165617/PlayMode-results.json) |
  [1080p](../../artifacts/visual-evidence-20261021-165641/PlayMode-results.json).
- Existing population Genome/profile/full-pool integration: **1/1 passed**.
  It now also asserts the new +Hare control is disabled with a full-pool reason.
  [Result](../../artifacts/visual-evidence-20261021-165426/PlayMode-results.json).
- Existing original-condition route, cashout, persistence, failure and Gene Lab
  unlock integration: **1/1 passed**.
  [Result](../../artifacts/unity-tests-20261021-165451/PlayMode-results.json).
- Portable .NET build: **0 warnings / 0 errors**; portable `self-test` passed.
- Final Unity compilation succeeded. Console inspection found **0 errors** and
  no Noesis binding/resource messages; an existing unreachable-code warning
  remains in `VM_SimulationBoard`. [Console](../../artifacts/population-decisions-console.json).
- **Ten final captures visually inspected:** controls with an unavailable removal,
  exhausted food, compact ledger, Rest Stop and Seed Cache at both resolutions.
  The ledger legend remains visible and three-digit currency fits. These are AI
  visual checks; Josh's playability acceptance remains open.
- [Verification receipt and final source/capture hashes](../../artifacts/population-decisions-verification-20261021-01/verification.json).

Initial discovery exposed CLI-version differences: installed beta.6 supports
`unity command recompile`, and rejects newer caller/skill flags and root-level
`recompile`/`self-update`. The multi-name semicolon filter in the Live lane
selected zero tests; it was not counted as evidence. Supported single filters
and the complete EditMode suite produced the retained results above.

## Limits and next work

The automated route uses short phases and no initial predators to isolate feature
wiring. It establishes transaction and transition correctness, not viable ecology
or fun. The next useful work is playing the normal route: assess whether choices
arrive early enough, whether food pressure reads clearly, and whether the
provisional quantities/prices create meaningful tradeoffs before tuning or
expanding the event/Genome content. A new seeded ecology comparison still needs
its own approved scope under SG-001/SG-005.
