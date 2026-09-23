# Hare reserve feeding and mate-seeking

[Working state](../WORKING_STATE.md) | Status: implementation complete, Unity verification pending

- Handoff schema: 1
- Handoff ID: 2026-09-23-codex-hare-full-energy-mate-seeking
- Owner: Codex
- Branch: BevBranch
- Baseline commit: c6b3282
- Date: 2026-09-23
- Supersedes: none

## Summary

The Forest Edge hare starts a refill when its energy falls below 6, feeds until
it reaches its maximum energy (24 in the authored scenario), then stops eating
until its energy falls below 6 again. Refill progress persists through movement
and energy changes. At full reserve, it seeks ready mates in its vision range
and through the existing nearby-mate movement. Its one-point energy loss now
happens every 10 simulation ticks.

## Implementation

- The Hare asset now has `forageBelowEnergy: 6` and an explicit
  `foragesUntilFull` policy; maximum energy remains 24.
- `SpeciesCell` tracks whether a Hare is refilling its reserve. That runtime
  flag follows creatures through movement and energy updates, and clears at
  the cap. A completed reserve does not restart until energy is below 6.
- The full-reserve policy is copied through authored rules, upgrade results,
  preview drafts, and simulation-data fingerprints. A maximum-energy upgrade
  no longer changes the separate forage trigger.
- Hare `energyLossIntervalTicks` is 10; the default remains 1 for other species.
  The cadence uses each creature's existing age counter, which advances once
  per simulation tick.
- Mate readiness accounts for end-of-tick metabolism; mate movement does not
  consume plant resources on its path.
- Unity reported `CanSeekMate` as unresolved because the helper had initially
  been added only inside `SpeciesBehaviorSystem`, while some callers are in
  `SpeciesSimulation`. Added the helper to `SpeciesSimulation` as well; the
  `mateTarget` definite-assignment diagnostic was a follow-on to that failure.
- A full hare can follow a ready mate it can see. Raising maximum energy through
  a Genome attribute leaves the 6-energy forage trigger unchanged.
- Added a focused simulation test for the below-6 / refill-to-cap / wait /
  below-6 cycle, tests for mating readiness and tracking a visible mate without
  eating on the way, a 10-tick energy-loss cadence test, plus an attribute test
  proving the trigger is preserved.
- No scene, prefab, or `.meta` files changed.

## Validation

- `git diff --check` passed.
- Attempted the focused EditMode filter `FullEnergyForager` through
  `CellSim Test -Mode EditMode -Execution Auto`. The runner stopped safely:
  Unity PID 41128 is open with a project lock, but its Pipeline is unreachable.
  `CellSim Doctor -Execution Auto` confirmed the same `Unreachable` state.
- On 2026-09-23, the user's Unity Console screenshot and `Editor.log` confirmed
  CS0103 errors for `CanSeekMate` at the `SpeciesSimulation` call sites and a
  follow-on CS0165 for `mateTarget`. After the helper was added to that class,
  `CellSim Doctor` reported `SafeMode`, `PipelineReachable=False`, and
  `HasLockFile=False`. Unity recompilation and tests remain unverified; do not
  treat the added tests as passing.
- A later `CellSim Doctor` run reported `Unreachable` with
  `PipelineReachable=False` and `HasLockFile=True` while the user was running
  the project. No current-value batch or matched before/after comparison was
  run.
- Rechecked on 2026-09-23 after the reserve hysteresis update. `CellSim Doctor`
  wrote `artifacts/unity-doctor-20260923-024758/doctor.json` and again reported
  `Unreachable`, `PipelineReachable=False`, and `HasLockFile=True`. The focused
  `FullEnergyForager` EditMode request stopped because Auto execution could not
  choose a safe lane; tests remain unrun. `git diff --check` passed after the
  update.
- No balance batch was run; this change modifies the base behavior and needs
  test/runtime verification before interpreting new population outcomes.
- The user is currently running a complete expedition to observe the reserve
  behavior. Unity tests were not started for the 10-tick change to avoid
  interrupting that run. Static diff validation is the available evidence for
  this follow-up.

## Risks and follow-up

- Verify the focused tests, then observe the hare in Play Mode: it should start
  grazing below 6, refill to 24, stop eating until below 6 again, and path
  toward an eligible visible mate. It should lose 1 energy every tenth tick.
  Check upgraded maximum-energy configurations as well.
- Re-run the diagnostic balance panel only after the behavior is accepted; the
  prior no-upgrade collapse batches predate this change.

## Follow-up: starvation concern

The user reported that most hares appeared to starve. Static inspection found
that with authored Hare metabolism 1, maximum energy 24, and Plant energy value
2, a hare at 23 energy would graze every tick, gain only 1 net energy after
metabolism, and spend 2 plant energy. Strict readiness at exactly 24 also
prevented mate seeking from the post-metabolism reserve. Updated both forage
and mate checks to account for metabolism and prevented full-policy hares from
eating while moving toward a mate. This is a code-level explanation, not a
measured current-run result; the existing 2026-09-22 batches predate this rule,
and the active project lock blocked new matched batches.

## Follow-up: reserve hysteresis

The user clarified that Hares should refill to 24, then wait until energy is
below 6 before trying to eat again. Added a per-creature runtime refill flag
because energy between 6 and 24 can represent either a Hare spending a
completed reserve or actively refilling. The Hare asset keeps the 6-energy
trigger; `ForagesUntilFull` is now an explicit species rule and is preserved
when maximum energy changes. Updated the focused cycle regression and the
maximum-energy upgrade contract. Unity verification is pending for this
follow-up; the Editor lock and Pipeline need a fresh check.

## Follow-up: slower Hare energy loss

The user requested that Hares lose one energy every 10 ticks. Added an authored
`energyLossIntervalTicks` rule with a safe default of 1, set the Hare asset to
10, and apply positive metabolism only when the creature's age reaches that
tick interval. Hare reserve-full state is retained through the longer interval
so mate seeking remains enabled while energy decays from 24. Added a focused
20-tick regression. Unity validation is deferred while the user runs a full
expedition; no runtime outcome is claimed yet.
