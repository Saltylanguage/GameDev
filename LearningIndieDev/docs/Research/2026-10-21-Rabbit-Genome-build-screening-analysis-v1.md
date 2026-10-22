# Rabbit Genome screening — AI analysis v1

**Analysis:** `ANA-RABBIT-GENOME-20261021-01-v1`  
**Experiment:** `EXP-RABBIT-GENOME-20261021-01`  
**Author:** Codex (GPT-6 family; exact runtime model identifier unavailable)  
**Date:** October 21, 2026, America/Toronto  
**Template:** SG-001 analysis sections; experiment-specific v1  
**Status:** Recommendation awaiting Josh's decision

[Observed facts and evidence](2026-10-21-Rabbit-Genome-build-screening-results.md)
are separate from this interpretation. The task was to screen the four approved
builds for measurable benefits and ecological costs; no values were retuned.

## What the evidence suggests

1. **The builds can produce meaningfully different ecological outcomes.**
   Fertile and Gardener retain Hares and Foxes in more seeds than Natural or
   this Trailblazer POC. This supports continuing the upgrade approach. It does
   not establish that the full proposed trees are balanced or enjoyable.

2. **The current reference setup has an early survival problem.** All arms
   fall from 20 Hares to roughly 6–7 by tick 100. Natural loses Hares in 11/12
   seeds and Foxes in all 12 by tick 600. Combat accounts for 334/369 of the
   Natural Hare deaths, while all Fox deaths are starvation. These facts make
   early predator–prey pressure and reproduction timing useful diagnostic
   targets. They do not prove that a specific Fox stat or population setting
   is wrong. Confidence is high in this panel's collapse, limited in its cause.

3. **Fertile Droppings has visible renewal and high outcome variation.** Its
   mean final Hare population is 43.42, median 0.5, and range 0–241. Six seeds
   lose Hares. The large mean should not be described as a reliable 43-Hare
   improvement or used to price the node. Renewal and scent activity are
   observed; a runaway classification needs an agreed range and longer or
   repeated evidence.

4. **The digestion addition changes the tradeoff instead of simply raising
   every metric.** Gardener's mean final Hare count is lower than Fertile's,
   yet its median is higher, Hare extinction falls from 6/12 to 3/12, and
   all-species endpoint presence rises from 3/12 to 6/12. Foxes also make more
   kills on average. A plausible explanation is a change in prey persistence,
   spatial encounters and predator feeding; the present counters cannot isolate
   that mechanism. More Hares alive on an average seed and a higher average
   population are different objectives. This is a useful candidate tradeoff,
   not proof of stable coexistence.

5. **This Trailblazer POC offers little sustained recovery in this setup.** It
   leaves at most one Hare per seed and no Foxes at tick 600. Its exact values
   and this pressure context need review. This does not reject the proposed
   Trailblazer branch, whose avoidance, gestation, migration and other abilities
   are not present in this two-stat fixture. Lower total movement is not proof
   that movement speed failed; total movement also depends on animal exposure.

## Pre-run predictions versus observations

| Prediction | Observation | Assessment |
| --- | --- | --- |
| Fertile activates renewal and extra scent interactions | Seed creation, dropping sniffs and named tracking occur; both animal populations sometimes persist | Mechanism prediction supported; sustainable balance unproven |
| Digestion improves reproduction or persistence | Lower mean births/final population than Fertile, but fewer extinction seeds and more joint endpoint presence | Mixed; evidence of a different tradeoff, not monotonic power |
| Trailblazer changes threat/food access and may improve outcomes | Little endpoint recovery; no breeding-sized population remains | No useful sustained benefit demonstrated by this panel |

## Recommended order of work

1. **Fix the plant-wilt mortality attribution and add one focused stacked-tile
   regression.** Owner: engineering/Josh. This is an existing telemetry defect,
   not a speculative tuning change. The current report's explicit derived
   correction is verified, but future reports and prediction features should
   not inherit the wrong labels. This task did not apply the fix.
2. **Diagnose early Hare losses before pricing or scaling the tree.** Owner:
   Josh with Sim. Start from the retained 0–100/101–300 mortality windows and
   representative seeds 10120 (collapse), 10121 (build difference), and 10125
   (large Fertile rebound). Confirm how early combat pressure, mating gates,
   gestation and available food interact. If a new experiment is needed,
   choose one lever and compare it with the unchanged baseline; a small
   lower-starting-Fox panel is one candidate, not an approved intervention.
3. **Keep Fertile and Gardener as content prototypes.** Their different
   survival, renewal and predator outcomes give us a concrete tuning problem.
   Agree what the initial playtest should permit—recoverable bad starts,
   readable predator pressure, and more than one useful response—then test
   those conditions. Healthy numerical margins remain unset.
4. **Revisit Trailblazer after the reference pressure is understood.** Isolate
   one stat or add its intended supported counterplay deliberately; do not
   buff several provisional values at once or promote this POC to production.

No extra simulations, production changes, statistical confirmation, visual
replay or human fun assessment are implied by these recommendations. The
scientific conclusion is that meaningful build differences are measurable,
while the reference ecology and durable balance margins still need work.
