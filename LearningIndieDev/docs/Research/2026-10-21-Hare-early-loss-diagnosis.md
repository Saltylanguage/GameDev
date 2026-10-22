# Early Hare losses — diagnosis v1

**Analysis:** `ANA-HARE-EARLY-LOSSES-20261021-v1`  
**Owner:** Josh  
**Date:** October 21, 2026, America/Toronto  
**Author:** Codex (GPT-6 family; exact runtime model identifier unavailable)  
**Status:** Reporting fix verified; diagnosis ready for review; tuning pending

## Scope and evidence

Josh asked to fix the reporting issues and diagnose early Hare losses.
This analysis uses the retained, approved
[48-run Genome panel](2026-10-21-Rabbit-Genome-build-screening-results.md),
current/source-snapshot rule inspection and a focused deterministic feeding
fixture. No new ecological comparison, production balance edit or player
playtest was performed. The panel remains the original 600-tick Forest Edge
36×20, 400 Plants / 20 Hares / 10 Foxes, 12 matched seeds per build, no Mutations.

The [machine-readable diagnosis](../../artifacts/hare-loss-diagnosis-20261021/diagnosis.json)
retains exact report hashes, per-seed checkpoints, mortality windows and
reconstructed birth ticks. Births at tick t are derived from population(t) −
population(t−1) + actual deaths(t). The script excludes the previously proven
plant-wilt misattributions from animal deaths, checks nonnegative birth counts,
and reconciles every run's derived total with its reported Hare births.
Age-derived birth ticks for dead Hares also agree with the reconstructed birth
cohorts. This is derived accounting, not a new birth-event recording.

## Reporting fix

Resources and creatures can occupy one tile. Death reporting now accepts the
removed layer explicitly. Wilt, resource consumption and population-limit
resource removal report the plant species, zero creature entity ID and
`isCreature=false`. Resource loss beneath an animal no longer borrows that
animal's age. Actual creature deaths on planted tiles retain their species,
entity ID, age and creature flag.

The existing standalone-plant behavior remains covered. Tests also cover Hare
and Fox occupants, both random population-cap removal choices, resource
consumption attribution, and actual combat/starvation identities on shared
tiles. Three new regressions failed on the original code before correction.

Verification:

- Final Clean Unity `SpeciesDomainTests`: **152/152 passed**. Evidence:
  `artifacts/unity-tests-20261021-043010/EditMode-results.xml`.
- Earlier corrected class: 151/151 before adding the feeding-delay fixture.
- Standalone runner build: **0 warnings, 0 errors**; existing portable self-test
  passed, using process-local `--roll-forward Major` because .NET 8 is absent.
- Only telemetry and tests changed. No energy, reproduction, perception, combat,
  random ordering, asset values or report field shapes changed. Report schema
  remains 38; old reports retain the old mortality bug and source identity.
- Empty allocation's blank catalog fingerprint was checked against the source.
  It is the deliberate Empty snapshot contract; no catalog fallback or changed
  allocation identity was introduced.

## Observed early losses

Values are means across 12 seeds per arm. First-birth ranges include only runs
that produced births; no-birth runs are counted separately.

| Build | Hares t25 | Hares t48 | Hares t100 | Births through t100 | Combat deaths through t100 | Starvation deaths through t100 | First birth range | Runs with births |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | --- | ---: |
| Natural | 14.83 | 12.33 | 6.50 | 0 | 12.25 | 1.25 | 106–133 | 12/12 |
| Fertile | 14.83 | 12.33 | 6.08 | 0 | 12.33 | 1.58 | 106–312 | 11/12 |
| Gardener POC | 14.83 | 12.33 | 7.08 | 0 | 12.25 | 0.67 | 106–109 | 12/12 |
| Trailblazer POC | 14.58 | 11.83 | 5.58 | 0 | 11.67 | 2.75 | 106–111 | 9/12 |

All deaths through tick 100 are from the starting Hare cohort; newborn losses
cannot explain the initial decline. Natural loses an average 13.5 of its 20
starting Hares by tick 100, with combat accounting for 12.25 of those losses.

Natural, Fertile and Gardener have identical per-seed Hare populations through
tick 48. Their seed renewal/digestion differences have not changed the early
bottleneck by that point. First recorded Eating transitions occur no earlier
than tick 57 in any arm. Eating transitions are state changes, not an exhaustive
record of feeding actions; the separate fixture below tests action timing.

Plant population does not globally collapse before this loss. Natural's mean
plant counts are 400 at launch, 427 at tick 48 and 386.58 at tick 100.
That does not prove every Hare can reach food safely or that each patch has
equal usable energy.

## Verified rule mechanism

The authored Hare rules and runtime gates have these values:

| Quantity | Current value |
| --- | ---: |
| Starting energy | 12 |
| Maximum energy | 48 |
| Begin feeding/refilling | Strictly below 6 when not already refilling |
| Mating eligibility | At least 24 energy (50% of maximum) |
| Mating energy cost | 24 per partner |
| Ordinary energy loss | 1 every 8 ticks |
| Gestation | 48 ticks |

At launch, the Hare's refill state is Idle. It is above its feeding trigger and
below mating eligibility. With ordinary metabolism it reaches energy 6 at tick
48 and 5 at tick 56. Feeding happens before that tick's metabolism, so the
first ordinary feeding opportunity is tick 57. Refilling then continues toward
maximum energy; eligible mates and a surviving gestating parent are still
required before offspring appear.

`StartingBetweenHungerAndMatingThresholdsDelaysFeedingDespiteAvailableFood`
isolates this behavior in a 3×3 food-rich grid with one Hare and no predators.
It uses these energy thresholds and metabolism, disables reproduction and
uses unit movement speed. It records zero successful food actions for ticks
1–56 and a successful action on tick 57. This verifies the gating mechanism;
it does not estimate the ecological benefit of changing it.

The panel's earliest birth is tick 106. That timing is consistent with refilling
and conception followed by 48 ticks of gestation. Exact conception and pregnancy
loss times are not stored in these reports, so they cannot be reconstructed as
individual events from this evidence alone.

Foxes start at 120 energy with a feeding threshold of 120 and do not use the
Hare's refill hysteresis. They can seek prey immediately. Their 12-tile vision
also exceeds Natural Hare vision of 5 tiles. These are real starting
asymmetries, not proof that either value is independently wrong.

## Interpretation and recommended next comparison

**Leading diagnosis:** immediate predator pressure acts during a long interval
with no Hare replacement. The feeding-onset/starting-energy/mating-threshold
combination creates part of that interval. Predation is the direct cause of
most early deaths; newborn starvation and global plant exhaustion do not
explain the initial decline in this panel.

The feeding delay is isolated and verified. Its contribution to extinction
risk still needs a controlled intervention. Faster movement alone does not close
the reproductive delay; digestion and seed renewal cannot help before food
use activates.

Recommended next experiment, **not executed or approved here**: keep the
declared Natural setup and change only the Hare's feeding onset, for example
compare the current threshold of 6 with an isolated research threshold of 25
(above the 24-energy mating requirement). Keep starting energy, gestation,
predators, combat and seeds fixed. Check first feeding/conception/birth,
0–100 mortality, food use, and both species' persistence before accepting a
new baseline. Starting a run in an active refill phase is another possible
design, but it should be a separate intervention rather than bundled into
the threshold comparison. Josh owns that behavior/tuning choice.

### Separate code observation to resolve before broad tuning

`SpeciesPerception.IsDietTarget` currently rejects a creature on a tile with a
plant resource, even if its species matches the predator's diet. This is a
source-level targeting asymmetry; the panel does not measure how much it
changes predator encounters, and this task did not change it. Confirm whether
that shared-tile protection is intended before treating it as a balance lever;
any correction needs a focused target/attack regression and fresh comparison
evidence. It is not presented as a cause of the early Hare deaths.

## Human decision

**Owner:** Josh. **Status:** Pending for feeding-policy/balance changes and new
experiments. The requested reporting fix is implemented and verified. Existing
raw evidence and its separate mortality correction remain intact; no full
journey, visual replay or fun assessment follows from this diagnosis.
