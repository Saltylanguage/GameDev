# M2-04 three-response diagnostic — AI analysis v1

- **Analysis ID:** `M2-04-THREE-20261115-A1`
- **Source:** [factual report](REPORT.md), [machine-readable metrics](METRICS.json), and the [approved contract](CONTRACT.md)
- **Analysis model and template:** GPT-6; SG-001 v1.1, M2-04 comparison v1
- **Prepared:** November 15, 2026, America/Toronto
- **Decision owner:** Josh; status `Needs Review`

## Summary

The 12-seed, four-arm panel is complete and comparable on its fixed inputs. It does not establish a viable four-node Hare route: every arm lost all Hares before node two. This is strong evidence for a current-source M2-03 route blocker and for keeping M2-04 open. It is not a production balance decision. The controlled tests still support the local mechanics of the three Mutations; this headless report does not expose their direct effect counters.

## Findings and confidence

1. **The assigned route fails before the first node boundary in every arm (high confidence for this panel).** The [report](REPORT.md#recorded-outcomes) gives all 48 first extinction ticks, ranging from 281 to 566. Foxes and Plants reset at tick 600, but the carried Hare count is zero. A complete player route and a comparison of later-node strategy tradeoffs remain unobserved. The S6 control record's earlier count sweeps used different source states and should not be read as a current acceptance rate.
2. **Tough Hide changes local combat outcomes but does not rescue this route (high confidence for the observed nonlethal hits; moderate for attribution).** It records 338 Fox nonlethal hits where the control records none, and its loadout adds two Hare health. The focused `ToughHideLetsAHareSurviveOneBaseFoxAttack` test establishes the intended isolated mechanism. The mean extinction tick is 425.8, versus 444.7 in control; eight paired seeds die earlier and four later. More surviving contacts can also expose Hares to later attacks, so nonlethal hits alone do not predict route survival. The matched report lacks the stronger bonus-specific `DecisiveFullHealthHits` counter.
3. **Escape Instinct changes some trajectories without a clear survival advantage here (moderate confidence for outcome; low for mechanism frequency).** Seven paired seeds die later, four earlier, and one at the same tick; the mean first extinction tick is 444.6 versus 444.7 in control. Mean Fox attempts fall from 79.3 to 76.2, but these whole-run totals also change when the population trajectory changes. The report omits `AvoidedContacts`, so the number of avoided contacts in this panel is unknown even though the isolated avoidance test passes.
4. **Reproductive Drive shows little aggregate birth or survival gain under this action policy (moderate confidence for outcome; low for mechanism frequency).** It produces 407 Hare births versus 404 in control across all 12 seeds; four seeds gain births, two lose births, and six tie. Its mean extinction tick is 441.3 versus 444.7. Eligible mating opportunities are limited and many reproduction candidates are blocked by energy or mate conditions in the raw activity records. The report omits `ChangedReproductionRolls`, so this panel cannot say how often the +0.1 chance directly changed a roll.

## What this means for M2

The panel supplies current-source **diagnostic evidence for M2-04**: the three ranks were acquired on schedule, paired outcomes differ, and Tough Hide has a clear combat proxy. It does **not close M2-04**, because no arm has a credible complete route here and direct per-arm effect counts and player-observed tradeoffs remain missing. It reinforces the **M2-03** legitimate route failure. It gives M2-08 data about possible causes (combat and starvation counts), but it cannot validate a player's causal explanation or the actual results screen. It does not validate M2-07's displayed feedback or audio.

## Recommended next work

1. **M2-03 / M2-04 implementation and a new approved balance experiment:** choose a current-source candidate opening and later-node pressure design that keeps the intended Fox threat visible while allowing a plausible response path. Freeze its exact source, scenario values, seed panel, action schedule, and success criteria before a new comparison under SG-001 and SG-005. Do not infer an approved Fox count or energy value from this panel. Suggested owner: Josh for design decision, assigned gameplay engineer for implementation.
2. **M2-04 / M2-08 telemetry:** expose the existing per-phase `MutationEffects` counters in CellSim's report contract, with a focused serialization check, then use them in a new matched run. This is report plumbing, not a change to the three effects. Suggested owner: simulation engineer.
3. **M2-03 / M2-04 player route review:** after a representative route can legitimately progress, observe each response from a fresh profile and ask the player what changed, what it cost, and why the route ended. Use that journey to judge M2-07 and M2-08 feedback and results as well. Suggested owner: Josh and a first-time player/reviewer.

The remaining M2-01, M2-02, M2-05, M2-06, M2-09, and M2-10 gates are not decided by this simulation. The [validation handoff](../../../handoffs/2026-11-15-0940-codex-m2-validation-batch.md) lists their focused automated evidence and the required hands-on checks. No human acceptance decision has been recorded.
