# Earlier feeding and gradual Fox introduction — AI analysis v1

**Analysis:** `ANA-FEEDING-FOX-20261021-01-v1`  
**Experiment:** `EXP-FEEDING-FOX-20261021-01`  
**Author:** Codex (GPT-6 family; exact runtime model identifier unavailable)  
**Date:** October 21, 2026, America/Toronto  
**Template/prompt:** SG-001 analysis sections, experiment-specific v1; compare
the approved four arms for opening losses and second-minute persistence  
**Status:** Recommendation awaiting Josh's decision

[Observed facts](2026-10-21-Feeding-and-Fox-introduction-results.md) and the
[machine-readable comparison](../../artifacts/feeding-fox-comparison-20261021-02/comparison.json)
are separate from this interpretation.

## What the evidence supports

1. **Gradual predator introduction is a useful opening-pressure lever.**
   Ramp-only opening combat deaths fall from 12.25 to 3.17 per run, with fewer
   combat deaths in all 12 matched seeds. The initial 20 Hares retain a larger
   population until reproduction begins. Confidence is high in the measured
   effect for this setup. The arm combines fewer initial Foxes and delayed
   arrivals, and also changes animal placements; their individual contributions
   are not isolated. This does not establish that the exact schedule should be
   the production default.

2. **Earlier feeding removes the measured replacement delay.** With the same
   initial counts, first feeding shifts from tick 57 to 1 and births from
   106–133 to 49–54. Hares exceed baseline at ticks 100 and 300 in every seed.
   Opening losses still occur; earlier births replace them. Confidence is high
   in the timing effect and limited in the suitability of threshold 25.

3. **Combining the two strong interventions exposes a food bottleneck.**
   Combined reaches a mean within-run Hare peak of 166.08, exhausts Plants in
   all seeds by tick 282, then loses all Hares by tick 418. Starvation accounts
   for about 89% of its Hare deaths. Ramp-only also has severe food depletion
   and about 78% starvation deaths. These observations strongly support food
   supply versus demand as the next diagnostic target. They do not isolate
   plant renewal, edible energy, food access, mating cost, litter size or
   crowding as the one setting to change.

4. **No candidate establishes a persistent ecology or a production baseline.**
   Every candidate loses all Hares and Foxes by two minutes. Baseline's one
   surviving lone Hare is not evidence of a functioning breeding population.
   The stronger opening curves therefore do not justify accepting either
   intervention unchanged. Confidence is high in the observed failure to
   persist for these seeds; universal balance and enjoyment remain untested.

## Predictions versus observations

| Pre-run prediction | Observation | Assessment |
| --- | --- | --- |
| Earlier feeding permits earlier replacement | First feeding tick 1 and births 49–54; higher Hares at 100/300 in all feeding/control pairs | Timing prediction supported |
| Fewer opening Foxes reduce early combat losses | Ramp-only 3.17 versus 12.25 opening combat deaths; lower in all seeds | Supported for the combined count/timing intervention |
| Later Foxes restore pressure and may allow ongoing interactions | Arrivals all succeed, but Hares and then Foxes disappear | Restored populations do not demonstrate persistence |
| Combination may improve persistence or overshoot | High early growth, Plant extinction in all seeds, then Hare starvation | Overshoot/food-collapse pattern observed; no persistent improvement |

## Recommended order of work

1. **Audit food renewal and access before selecting another tuning value.**
   Owner: engineering with Josh/Sim. Start with current-source and existing
   replay inspection: how Plants replenish, whether animal occupancy changes
   renewal/access, how food energy is consumed, and what happens when the last
   local or global plant disappears. Review the already identified
   `SpeciesPerception.IsDietTarget` shared-tile rejection against intended layer
   semantics. Its impact on this panel is unmeasured; it should not be claimed
   as the explanation or silently changed during a replay comparison.
   Expected value: distinguish an implementation defect from a genuine
   resource budget before compensating with upgrade values.

2. **Check what limits Hare growth while food is plentiful.** Owner: Josh/Sim
   with engineering telemetry. Examine the mating energy/litter/gestation loop
   and local crowding against renewable food capacity. The current outcomes
   show that relieving predation and energy gating can produce more animals
   than this resource system supports. Raw reproduction block totals need
   population-exposure normalization before comparing their effectiveness.
   Expected value: a natural population range with space for Genomes to help,
   rather than a requirement to purchase renewal just to avoid collapse.

3. **Then propose the smallest bounded follow-up.** Preserve a matched control
   and change one food-demand, food-renewal or feeding-onset variable justified
   by that audit. Keep the gradual-introduction setup as a useful candidate
   pressure condition, not an adopted default. Do not combine more aggressive
   reproduction and more food without isolating their effects. The next exact
   scope and values remain a human decision under SG-001.

The immediate recommendation is to investigate **food renewal/access and the
growth limit**, then tune the natural baseline before pricing or tuning the
Gardener upgrades. A larger Hare count at ten seconds is insufficient for the
desired playable loop. These runs provide a concrete failure mode to work on;
they do not reject the Genome tree concept.

## Uncertainty and decision boundary

- Twelve seeds, one map configuration and no player decisions or visual test.
- Density arms change initial animal placements; timing and placement effects
  cannot be separated from this panel alone.
- Global Plant counts do not measure accessible nutrition or local depletion.
- The known shared-tile targeting behavior remains fixed, and its ecological
  effect needs intent and regression review.
- No stable coexistence interval, fun criterion or production safety margin
  has been established. No additional experiment or production edit is implied
  by this analysis. Josh owns adoption and follow-up scope.
