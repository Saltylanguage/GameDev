# Forest Edge first balance pass

**Date:** 2026-09-21  
**Status:** Provisional tuning baseline  
**Scope:** Forest Edge Hare/Fox balance plus the Fox mating-state stability fix.

## Change

The authored Fox starting population in `Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset` moved from **4 to 6**. Hare values were restored after an inconclusive test of starting energy 6 to 10. The Hare asset is unchanged in the final working tree.

## Matched evidence

- Control: [600-tick report](../../artifacts/cellular-experiment-20260921-204834/report.json)
- Fox 6 candidate: [600-tick report](../../artifacts/cellular-experiment-20260921-210203/report.json)
- Compressed comparison: [report summary](../../artifacts/cellular-experiment-20260921-210203/report-summary.md)
- Seeds: 10100–10119; 20 matched runs; 42×20 current Forest Edge asset; 600 ticks; opposed-roll combat; natural opportunities.
- Focused authoring check: [EditMode result](../../artifacts/unity-tests-20260921-210709/EditMode-results.json), Scenario filter 1/1 passed.

| Metric | Control | Fox 6 | Paired change |
| --- | ---: | ---: | ---: |
| Mean final Hare | 44.00 | 38.85 | -5.15 |
| Mean minimum Hare | 19.15 | 17.35 | -1.80 |
| Mean Hare starvation deaths | 125.05 | 112.05 | -13.00 |
| Mean Hare combat deaths | 20.35 | 24.25 | +3.90 |
| Mean Fox peak | 4.90 | 6.80 | +1.90 |
| Mean Fox kills | 20.35 | 24.25 | +3.90 |

No matched candidate run ended with Hare extinction, and no candidate seed reached a Hare minimum below 10. The candidate creates more visible Fox pressure while keeping a recovery/survival window, but its lower final Hare population means it is not final balance approval.

## Rejected candidate

Hare `startingEnergy: 6 -> 10` reduced starvation by only 6.6 deaths on average, slightly lowered final Hare population by 1.4, and increased combat deaths in half the seeds. It was reverted before the Fox comparison.

## Next step

Keep Fox 6 as the working comparison baseline for the next one-variable pass. Test Hare feeding/energy access or Forest Edge food density separately; do not stack both until the next matched result identifies the controlling lever.

The raw reports remain authoritative. The compressed summary is an analysis aid, not causal proof.

## Metric correction and continuation check

Population matching is not the acceptance metric for this pass. The direct
question is whether Foxes create opening pressure, whether two or three Hare
Mutations improve survival under that pressure, and whether the Hare enters
phase 3 still weakened.

The paired 10-seed continuation used the current Fox 6 scenario, 42×20 grid,
three 100-tick phases, opposed-roll combat, and the existing player-facing
Mutation effects:

| Measure | No Mutation | Tough Hide → Threat Exposure | Delta |
| --- | ---: | ---: | ---: |
| `pAVI` post-contact survival | 0.39 | 0.46 | +0.08 |
| `predAVG` combined predator survival/avoidance | 0.66 | 0.69 | +0.03 |
| `PREY` predation deaths | 15.5 | 15.9 | +0.4 |
| `STRV` starvation deaths | 42.8 | 46.1 | +3.3 |

The opening phase is identical because both schedules start without a
Mutation: 10.3 Fox combat opportunities, 6.5 Fox kills, and 6.5 Hare combat
deaths per run. By phase 3, the Mutation path still produces 8.6 Fox
opportunities and 3.9 kills/run while starvation rises to 35.5 deaths/run,
compared with 6.1 opportunities, 3.8 kills, and 31.4 starvation deaths in the
no-Mutation control. This supports the intended pressure-plus-weakness shape
without requiring Fox and Hare populations to match.

Evidence: [Mutation continuation](../../artifacts/cellular-experiment-20260921-221417/),
[paired control](../../artifacts/cellular-experiment-20260921-221521/).

## Fox mating stability follow-up

The six-phase continuation exposed a phase-3 behavior loop: a Fox could select
`Mating`, fail to keep its partner in the same state because food-seeking ran
first, then wander one cell and re-enter mating on the next tick. This was a
state-selection priority issue, not a phase-boundary reset. The fix makes a
mutually ready adjacent pair select `Mating` before foraging, and movement now
skips creatures whose current state is `Mating`. If either Fox is below the
reproduction-energy gate, the pair is not locked and both remain free to forage.

Focused regression coverage now includes:

- `MatingPairStaysInPlaceInsteadOfWanderingAway`
- `ReadyMatingPairTakesPriorityOverForaging`

The live EditMode suite passed **236/236**. The repeated five-seed,
600-tick, six-phase continuation is retained at
[the follow-up artifact](../../artifacts/cellular-experiment-20260921-224821/).
In phase 3, Fox `Mating→Wandering` and `Wandering→Mating` transitions fell to
**0.2/run each**, from **4.8/run** and **5.2/run** before the fix. Foxes also
remain stationary while their state is `Mating`.

### Cooldown and separation follow-up

There was no explicit mating delay before this follow-up: an eligible parent
could attempt reproduction on every tick. The runtime now applies a shared
**24-tick reproduction cooldown** to the parent and its reproduction-pattern
neighbors after an eligible attempt, including a failed chance roll or an
unavailable birth location. Mate-seeking is suppressed during that cooldown,
so the pair is released to hunt or wander before they can seek one another
again. A real `Mating` pair also performs one shared attempt per tick, which
prevents both parents from creating newborns on opposite escape sides and
boxing themselves in. A full reproduction group is no longer selected as
`Mating`.

Focused coverage now includes `MatingAttemptAppliesCooldownAndLetsThePairSplit`.
The live mating filter passed **4/4**, and the full EditMode suite passed
**237/237**. The follow-up live five-seed, 600-tick, six-phase run is retained
at [the cooldown artifact](../../artifacts/cellular-experiment-20260921-232331/).
That run recorded three Fox births across five seeds, while its aggregate
`Mating` state telemetry was zero; treat that as a follow-up telemetry/
eligibility signal for the next balance pass, not as a population-equality
metric.
