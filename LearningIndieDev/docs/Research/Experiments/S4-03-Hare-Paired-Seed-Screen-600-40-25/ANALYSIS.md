# AI analysis - S4 Hare 600/40/25 paired screen

**Generated:** 2026-10-05 (America/New_York)  
**Model:** OpenAI Codex; exact model build is not exposed in this workspace.  
**Evidence:** [factual report](REPORT.md), especially `Outcomes`, `Human-readable slashline and direct observations`, and the retained paired CSVs under `artifacts/s4-paired-screen-20261005-600-40-25-wrapped-100/`.

## Executive summary

At 600/40/25 with wrapping, the clearest follow-up candidate is the **Gardeners resource path**. Seed Dispersal alone and both Gardeners orders had 61, 61, and 60 Hare survivors at tick 600, and each retained all three species in 43/100 runs, versus 19/100 for control. The matched Hare-survival advantage is only four to five seeds per hundred and its paired win/loss counts do not settle the question. The stronger lead is therefore persistence of the whole trio at the horizon, alongside substantially higher sAVI and recorded seed planting.

Both **Trailblazer orders** underperformed control on this panel: 42/100 and 46/100 Hare survivors versus 56/100. Their paired tables favor control, though this is one exploratory 13-arm screen with multiple comparisons. It is a useful warning for this population and wrapped map, not a general verdict on Movement or Avoidance.

No build is validated as balanced. Median Hare FPO was 0-3.5 across every arm while means were often much higher, showing that a few large survivors pull averages up. This makes the all-three-at-tick-600 count and the slashlines more informative than mean FPO alone, but all-three presence still does not establish healthy populations, recovery, or stable ecology.

## User-defined all-species persistence threshold (2026-10-06)

Bevin clarified the target: count a run as a successful all-species finish only if Plants remain present and both Hares and Foxes finish with at least five individuals. Exactly five qualifies. Re-scoring the retained records at tick 600 with `PlantFinal > 0`, `finalHares >= 5`, and `FoxFinal >= 5` changes the Gardeners signal from 43/100 simple-presence finishes to 30-33/100 threshold-qualified finishes, versus 13/100 for control. Seed Dispersal alone qualifies in 31/100. Against control, matched candidate-only / same-outcome / control-only threshold results are shown below; ties include both runs passing or both failing.

| Arm | Threshold-qualified /100 | Paired W/T/L vs control |
|---|---:|---:|
| Skip-all control | 13 | 0 / 100 / 0 |
| General Movement | 10 | 8 / 81 / 11 |
| Threat Avoidance | 11 | 8 / 82 / 10 |
| Tough Hide | 15 | 11 / 80 / 9 |
| Crowding Tolerance | 13 | 4 / 92 / 4 |
| Efficient Digestion | 14 | 8 / 85 / 7 |
| Seed Dispersal | 31 | 25 / 68 / 7 |
| Trailblazer: Movement then Avoidance | 15 | 11 / 80 / 9 |
| Trailblazer: Avoidance then Movement | 7 | 6 / 82 / 12 |
| Warren: Hide then Crowding | 12 | 8 / 83 / 9 |
| Warren: Crowding then Hide | 14 | 6 / 89 / 5 |
| Gardeners: Digestion then Dispersal | 33 | 25 / 70 / 5 |
| Gardeners: Dispersal then Digestion | 30 | 25 / 67 / 8 |

This threshold strengthens the case for a focused Gardeners/Seed Dispersal confirmation. It is still a post-hoc re-score of the same exploratory 13-arm screen, not independent confirmation; one configuration and one 600-tick horizon cannot establish long-term ecology balance. Continue using wrapped 600/40/25 as a provisional Hare-strategy test bed, and confirm control, Seed Dispersal, and both Gardeners orders on fresh matched seeds before tuning values. The Gardeners order comparison is 16/71/13 (Digestion-first alone qualifies / same result / Dispersal-first alone qualifies), so it does not identify a clear order winner.

## Challenge framing (2026-10-06)

Bevin clarified the objective: the challenge is to keep the ecology balanced through player intervention well enough to clear a scenario and advance to the next journey step, in the planned node-based roguelike progression. The skip-all arm is the unattended-collapse pressure benchmark, not the intended player success path. Under the five-animal threshold, 13/100 skip-all runs qualify, compared with 30-33/100 in the Gardeners paths. This is evidence that the intervention can change outcomes in this setup, but most Gardeners runs still fail the endpoint test; a fresh confirmation and human review of the intended challenge difficulty should precede tuning. The journey-map implementation and exact progression rules remain separate planning work.

## Evidence-based findings

1. **Seed Dispersal is the cleanest single skill to investigate next.** It had 61/100 Hare survivors, 43/100 all-three survivors, and an 18/69/13 candidate-only / same-outcome / control-only paired survival split. Control had 56 Hare survivors and 19 all-three survivors. Seed Dispersal recorded a mean 115.19 successful plantings per run (11,519 over the batch); survival and all-three presence both moved in the favorable direction, but the paired Hare-survival difference is small and variable.

2. **Both Gardeners orders show the same whole-ecology signal.** Digestion then Dispersal had 61 Hare survivors and 43 all-three runs; Dispersal then Digestion had 60 and 43. Their paired order comparison is 12/77/11 for Hare survival, and final FPO differences have median zero. The run therefore supports testing the pair further, but it does not establish a meaningful order advantage. Each order reached the first choice in all seeds and the second in 99/100 seeds.

3. **Trailblazer is a risk signal at this starting state.** Movement then Avoidance survived in 42 seeds, with 14 paired candidate-only survivors versus 28 control-only survivors; reversing the order survived in 46, with 8 versus 18. Median FPO was zero for both. Movement can affect food access and threat contact at once, and the batch did not isolate those paths, so the cause is unresolved.

4. **The basic skills changed their intended slashline dimensions without a clear general survival gain.** Tough Hide had the highest pAVI mean (0.486 versus 0.419 control), while eAVI was 0.615 versus 0.657 control; its Hare survival was 55/100 and all-three count 25/100. Crowding Tolerance and Efficient Digestion were close to control survival (56 and 54), with no clear terminal whole-ecology advantage. These measures keep the player-readable contact, avoidance and starvation picture visible, but the means end at different extinction times and should be read with their per-run validity and exposure windows.

5. **Warren order does not have a decisive direction.** Hide then Crowding had 55 Hare survivors and 23 all-three runs; Crowding then Hide had 60 and 18. Paired survival was 13/69/18 in favor of Crowding first, but whole-trio retention went the other way. That difference calls for a design choice about which player experience matters, not selection by one number.

## Hypotheses and confidence

- **Medium confidence, narrow scope:** Seed Dispersal and Gardeners are promising candidates for maintaining the plant-herbivore-predator relationship in the tested wrapped 600/40/25 setup. Evidence: all-three survivors, seed-drop counter, sAVI, starvation deaths, and the 100 matched seed records.
- **Low confidence, causal:** More seed-created plant food caused the higher all-three count. The experiment records the effect but did not mediate or ablate planting; other evolved trajectories and run duration also differ.
- **Low confidence, general:** Movement or Trailblazer is intrinsically weak. Evidence only concerns one scenario, one initial state, wrapping on, one skill level and acquisition at tick 100/200.
- **Low confidence, order effect:** A particular order is best for Trailblazer or Warren. Counts move in different directions across Hare survival and all-three retention; this screen is not powered or adjusted to claim pair-order significance.

## Limits and next work

The 100 seeds estimate outcomes for this configuration, not future journey nodes, the bounded map, another population, or full five-choice runs. Thirteen arms create multiple opportunities for apparent winners; no multiplicity-adjusted tests were preregistered. Whole-run FPO and slashline windows are censored at Hare extinction. A positive count at tick 600 is only presence, not a minimum healthy population. The scripted legacy schedule also skips player offer eligibility and repeats.

Recommended next step: review whether the Gardeners all-three persistence pattern is strategically interesting, then run a fresh, narrower confirmation of control, Seed Dispersal and both Gardeners orders on non-overlapping seeds, preserving this same wrapped 600/40/25 setup. Also compare the actual player-facing offer/card flow before treating the research schedule as a player experience. Keep authored population defaults and skill tuning unchanged until the human owners decide how to use this evidence.

**Human decision:** Bevin / Salty review pending. This analysis is not an approved strategy or balance decision.
