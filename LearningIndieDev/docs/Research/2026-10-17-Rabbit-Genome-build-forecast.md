# Rabbit Genome build forecast — Forest Edge

**Status:** AI design forecast for Josh and Sim to review; no balance or implementation decision is accepted here.  
**Date:** 2026-10-17. **Analyst:** Codex (exact model build unavailable to the project). **Method:** design-forecast-v1, evidence synthesis and code inspection; no separate trained predictor was invoked.  
**Source checkout:** `ProjectMain` at `6ffb7b10`; this analysis made no simulation run or gameplay change.  
**Question:** Can Trailblazer, Warren, and Gardener become distinct, balanceable permanent Rabbit Genome branches while S5 proves a fun Forest Edge route?

## Executive forecast

**Conditional yes for the design, no demonstrated S5 balance yet.** The proposed 8/8/9-node branches have coherent identities: Trailblazer distributes Hares, Warren concentrates and protects them, and Gardener renews their food. That creates useful ecological tradeoffs. The present evidence supports only a narrower claim: existing *temporary Mutation* approximations of Gardener improve some Forest Edge outcomes; Trailblazer and Warren often improve Hare survival without improving the whole-ecology goal. The proposed *permanent Genome* branches have not been run. Most signature behaviors do not exist yet, so a numerical success probability for the full trees would be invented precision.

The highest-risk loop is **Hare births and survival increasing faster than plant renewal and Fox access can respond**. A second risk is an opening collapse before later player choices matter. S5 should establish a healthy, readable baseline and two strategies with different visible causal stories before attempting to balance all 25 nodes. Keep the 25-node set as the target design inventory; use staged implementation and matched comparisons to earn values.

## Evidence available to this forecast

| Evidence | Observation | What it does not prove |
| --- | --- | --- |
| [S4 offer-aware 200-seed report](Experiments/S4-03-Strategy-Choice-Policy-Screen-600-40-25/REPORT.md) | In one wrapped 36×20, 600/40/25 node, skip cleared 32/200, Trailblazer 30/200, Warren 34/200, Gardener 51/200. Gardener's paired clear lift was +9.5 percentage points, 95% CI +3.2 to +15.8. All builds had median Foxes below the five-Fox threshold at tick 600 among horizon-reaching runs. | Effects of permanent Genome nodes, another map, a journey, or player enjoyment. |
| [S4 fixed-path confirmation](Experiments/S4-03-Hare-Paired-Seed-Confirmation-600-40-25/ANALYSIS.md) | Seed Dispersal reached 58/200 versus 25/200 skip on fresh matched seeds. Gardener order did not separate reliably. Foxes below five dominated many horizon-reaching non-clears. | Which individual mechanism caused the lift or which acquisition order is best. |
| [October 12 five-world confirmation](Results/2026-10-12-Forest-Edge-Confirmation/ai-analysis-v1.md) | Across selected 55 Hare/35 Fox/400 Plant, closed-edge worlds, the best confirmed ecology-goal arm was 10/100. Trailblazer raised Hare survival by +6.95 points pooled but changed the ecology-goal rate by −0.25 points; Gardener's pooled goal change was +1.30 points. Fox starvation accounted for 90.1% of recorded Fox deaths. Only 28.8% of runs reached all five choice windows. | A causal map-size or gestation effect, a production success rate, or a stable post-600 ecology. The 8,000 arms share only 100 seed blocks. |
| [EX-010](Experiments/EX-010-Sequential-Upgrade-Continuation/REPORT.md) and [EX-011](Experiments/EX-011-Cross-Biome-Species-Transfer/AI_ANALYSIS.md) | Upgrade order can change continued-world outcomes; a declared movement-plus-crowding bundle had a correct directional held-out prediction in a different herbivore panel. | Additive value of arbitrary node combinations or generalized predictive accuracy. |

These are different scenarios and outcome definitions. Their percentages are **not** one pooled baseline. The historical 40–50% first-node review band belongs to the wrapped 600/40/25 study; it is not an approved S5 threshold.

## Testability map for the proposed Genome set

“Proxy now” means the simulation has a stat or temporary Mutation with a related effect. It does **not** mean the Genome node is implemented. “New rule” needs behavior, serialization, and telemetry before a credible prediction run.

| Branch | Proposed nodes (8 / 8 / 9) | Current testability | Primary tuning exposure |
| --- | --- | --- | --- |
| Trailblazer | Faster movement, avoidance, fast gestation, long vision, high crowding, wanderlust, 2× interactions, skip a node | Movement, vision, gestation, crowding, and an avoidance proxy can be screened as stat interventions. Chunk-based wanderlust, 2× interactions, and skip-a-node need exact rules. | Speed can change contact and food access; a future per-move energy charge would add a cost. Gestation/crowding can overshoot food; migration can strand mates or rescue depleted regions. |
| Warren | Burrows, tough hide, good scent, minimum litter, maximum litter, crowding bonus based on minimum litter, burrow speed, offspring/inherited burrows | Block, litter bounds, and crowding have stat proxies. A named “Guarded Burrow” Mutation is a block bonus, **not** a shelter. Actual burrows, scent, and inherited structures are new rules. | Safe mating plus larger litters can turn a protected pocket into local starvation; shelter can make Hares inaccessible to Foxes. |
| Gardener | Pass through bushes, fertile droppings, picky eater, tier-2 drops, tier-3 drops, efficient digestion, favorite food, at one with nature, patrols | Seed-drop chance and digestion have stat proxies. Fertile Droppings needs its predator-trackable scent half. Bush traversal, plant tiers, food ranking/specific multipliers, partial consumption/regrowth, and patrols need new rules or richer food data. | Local food renewal can help both prey and predators, but stacked renewal/digestion can remove scarcity and create plant/Hare boom-and-collapse. |

The current seed-drop rule consumes one food-reserve unit, creates the first registered plant in an adjacent free cell, and records attempts/successes; base Hare seed chance is zero. The current diet has one target species, so “picky” and “favorite” cannot distinguish plant varieties yet. Perception uses vision on current world state, while tracked food can resolve an entity by scanning the entire grid; a 10-tick, longer-range scent channel must define stale location and precedence without silently granting exact global tracking. These are code facts from `SpeciesSimulation`, `SpeciesPerception`, and the production Hare asset at this checkout.

### Gardener node design read

| Node | Distinct job if retained | Forecast / design condition |
| --- | --- | --- |
| Pass through bushes | Reach otherwise blocked food and routes. | Useful only if the playable map has meaningful bush barriers; measure new reachable cells and Fox access, not just movement count. |
| Fertile Droppings | Create new grass from carried reserve. | The best-supported Gardener direction, but its Fox-trackable scent and reserve expense must be real. It cannot rescue a Hare with no reserve. |
| Picky Eater | Choose the richest reachable food. | Needs more than one edible plant value and a bounded search rule; otherwise it is a no-op or invisible optimization. |
| Tier-2 and tier-3 droppings | Improve the *quality* of created plants. | Two separate nodes can work as an explicit progression, but need plant tiers, caps, and a reason not to buy only the highest tier. Do not treat both as independent free multipliers. |
| Efficient Digestion | Gain more usable energy from any meal. | Direct and legible; because it applies broadly, price/cap it against food creation and preservation. |
| Favorite Food | Gain extra energy from one named plant. | Distinct from digestion only if the favorite is not always available; requires an actual plant identity and availability tradeoff. |
| At One with Nature | Preserve part of a consumed plant and accelerate regrowth. | The strongest positive-feedback risk, especially with droppings. Define one trigger/cooldown or resource cost before testing the proposed 50% remaining and 2× growth values. |
| Patrols | Revisit/defend/seed a local patch. | Still a concept rather than a rule. Define the objective and Fox counterplay before giving it a node or assigning value. |

This is **nine nodes if tier 2 and tier 3 are separate**. The branch is balanceable if creation, preservation, selection, and conversion remain different conditional effects. If all nine simply raise net food or energy every tick, the tree will collapse into one dominant scaling path.

## Falsifiable change-impact predictions

These are directional **hypotheses**, not measured Genome effects. Confidence reflects evidence transfer and mechanic readiness, not the probability that a build is fun.

| ID | Prediction under a declared Forest Edge baseline | Confidence / status | Evidence that would refute or qualify it |
| --- | --- | --- | --- |
| G1 | A modest seed-renewal plus digestion Gardener allocation will increase Hare food access and survival relative to zero Genome, but it will not by itself fix Fox persistence or reliably clear a fragile node. | Moderate directional; proxy testable now, full Genome not testable. | Matched runs show no food/plant gain or a dependable all-species clear lift without a baseline correction. |
| T1 | Movement, vision, and avoidance will increase map coverage and some Hare survival. Whole-ecology clear can remain flat or fall if extra Hares exhaust plants or evade Foxes. | Moderate for the survival/goal split in selected Mutation panels; Genome magnitude underdetermined. | Similar coverage and Hare survival to control across matched seeds, or a broad clear gain with no predator/resource cost. |
| W1 | Shelter plus litter bonuses will improve early protected births and lower combat deaths, then increase local crowding and plant consumption unless occupants disperse or food renews. | Low; actual burrows not currently testable. | Births/combat do not shift after verified shelter use, or the locality remains healthy without dispersal/renewal. |
| X1 | Gardener renewal combined with Warren reproduction or Trailblazer dispersal will be non-additive: the first may sustain a breeding pocket; the second may spread renewal across more cells. Which is stronger depends on space, food and Fox access. | Low; spatial interactions not currently testable. | Factorial matched runs show negligible interaction on predeclared spatial and ecology measures. |
| P1 | When scent is refreshed every 10 ticks at longer range than sight, it will create earlier movement toward or away from a *last known* source, while short-range sight remains more accurate. Fertile Droppings should have a measurable predator cost rather than being free plant creation. | Design mechanism hypothesis; not currently testable. | Predator tracks never change encounter timing, or scent behaves as exact continuous target knowledge. |

**Most important abstentions:** A full-tree clear probability, value of “skip a node” and “2× interactions,” plant-tier effects, chunk-migration effect, burrow persistence effect, and a claim that any build is fun are not currently predictable from this code/evidence. Neither AV nor a sum of standalone stat effects can fill those gaps.

## Candidate baselines and margins for a first bounded test

These are **review proposals**, not accepted production values or a green light to execute a formal experiment. Josh/Sim should approve one scenario, goal, observation window, and report contract under SG-001 before a new evidence run.

1. **Freeze one S5 player route baseline** at its actual launch rules, map, populations, shop actions and two-simulation continuation. Compare zero Genome with one small allocation from each branch on the *same* seeds. Record the 600-tick result and at least a 100-tick no-new-choice continuation so a borderline clear can be distinguished from immediate collapse. Keep the old wrapped 600/40/25 panel as historical reference, not the new production target.
2. **Measure both the focal and ecological margins:** Hare survival and birth/death timing; Plant population, production and consumption; Fox food, births, starvation and persistence; all-three coexistence and authored node clear; first-choice reach and time available after the choice. A build is not healthy merely because Hare survival rises. Report paired differences with uncertainty, not just winning seed examples.
3. **Look for a usable region, not a perfect point:** control should permit recovery as well as failure; at least two affordable strategies should rescue some of the same-start runs for different legible reasons; neither should simply make one species disappear. The owner should set the acceptable clear and post-clear bands after inspecting a playable baseline. The historical 40–50% band would require roughly +14.5 to +24.5 points above the old 25.5% Gardener offer-aware rate, but that arithmetic does not transfer to a new world.
4. **Use local mechanical margins before global balance claims:** for every new node, record its trigger denominator and realized rate; spatial coverage or shelter occupancy; costs paid; intended counter-events; and when the affected plant/Hare/Fox trajectory diverges. If a mechanic rarely triggers, no endpoint comparison can fairly price it. If it triggers constantly, cap/range/cost comes before more stacking.
5. **Stage combination testing:** zero/one-node arms first, then within-branch pairs and the two predicted cross-branch interactions. Repeat on a disjoint held-out seed block after values and analysis are frozen. Compare the Genome alone and Genome plus reachable temporary Mutation choices; the Genome applies from tick zero to every Hare population, whereas Mutations appear later only for the selected species.

## Proposed S5 decision sequence

1. **Simulation first:** fix the largest opening and Fox-food failure mode in a declared baseline, then show two human players the same route. In the latest confirmation, merely strengthening Hares did not consistently improve the ecology goal.
2. **First Genome proof:** implement and expose a small, measured Gardener renewal choice with the agreed 10-tick scent consequence, plus a contrasting Trailblazer mobility/vision allocation whose underlying stats already exist. Warren's distinctive burrow choice should follow its shelter rule contract; a block bonus alone will not prove the branch. Preserve the full 8/8/9 inventory as design candidates. Choose exact node costs, capacity, numeric effects, unlock order and interactions only after the mechanical contract and comparison evidence are reviewable.
3. **Player proof:** the Desktop should make a pre-run allocation and its cost clear; the journey should show why a plant patch, Hare group, or Fox hunt changed; the result should suggest a different allocation or temporary choice next attempt. Record confusing or unfair moments separately from automated outcomes.

## Open design definitions before implementation or a full-tree prediction

- For **wanderlust**, define chunk size, density denominator, threshold accumulation, destination/return rule, and whether migration changes mating access or energy cost.
- For **scent**, define source, field spread/decay, 10-tick sampling phase, last-known-location error, sight precedence, and what Foxes learn from Fertile Droppings. Define whether Warren's “good scent” detects food, mates, Foxes, or several signals. A Fox still needs sight before attacking a Hare under the accepted first-node contract. Confirm that exact tracked-target lookup respects the chosen perception contract.
- For **burrows**, define occupancy, construction and maintenance cost, mating safety, Fox counters, death persistence, offspring use, and the precise crowding penalty.
- For **Gardener food**, define plant tiers/species and nutrient values, preference scoring, favorite-food identity, partial-consumption regrowth rule, and spatial caps. Until distinct edible types exist, “picky eater” and “favorite food” have no meaningful target.
- Define whether **2× interactions** multiplies actions, opportunities or interaction rewards, and what **skip a node** bypasses in the eight-point Genome allocation and prerequisite graph.

**Decision owner:** Josh (Genome design), with Sim for simulation effects and evidence. **Next status:** human review; no production values or sprint-card changes implied.
