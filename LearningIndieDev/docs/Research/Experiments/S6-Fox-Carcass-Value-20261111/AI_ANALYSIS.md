# AI analysis — Fox carcass-value diagnostic

**Analyst:** Codex (GPT-6), November 13, 2026.
**Analysis timestamp:** 2026-11-13 21:06 UTC.
**Prompt/template:** Interactive comparison of the approved experiment contract and validated evidence using the artifact-summarization V1 evidence workflow; no separate reusable analysis prompt was used.
**Method:** Comparative interpretation of the [factual report](REPORT.md), the per-seed [expanded summary](../../../../artifacts/S6-Fox-Carcass-Value-20261113-153600/expanded-validated-summary.json), and the approved contract. No predictive AI or trained model was used; no model is calibrated for this carcass rule.
**Decision owner:** Josh. **Decision status:** No production ratio or energy value is approved by this analysis.

## Summary

The calorie arithmetic holds as a narrow maintenance estimate: one 96-energy Hare carcass equals two Foxes' 48-energy maintenance over 600 ticks. The simulation does **not** support treating that as a survival or sustainable-population ratio. At the same 2:1 starting ratio, the chance that a seed records at least one Fox starvation death rises from 47% at 2 Foxes/1 Hare, to 82% at 4/2, to 100% at 6/3. The three estimates have non-overlapping 95% Wilson intervals. Every Hare goes extinct in all 300 of those ratio-arm seeds.

This is strong evidence that the current encounter ecology and reproduction do not preserve a constant outcome when the opening populations scale. It does not establish a linear, exponential, or other general curve. The measured event is “one or more Foxes starved in this seed,” which becomes more likely as the Fox group and its within-node births grow; it is not an individual Fox's starvation probability.

## Findings

1. **The 2:1 energy ledger is locally correct, but it is not an ecological ratio.** In the tested 2:1 arms, median carcass energy consumed per starting Fox is 48, exactly the authored per-node maintenance amount. Across each arm, total consumption is below created carcass energy (9,357/9,600; 19,084/19,200; 28,658/28,704), yet 47, 82, and 100 of 100 seeds still contain at least one Fox starvation death ([report tables](REPORT.md#seed-outcomes), [energy ledger](REPORT.md#population-and-feeding-measures)). Whole-population energy availability therefore does not guarantee every individual gets enough food before its starvation check. Timing, local access, and births are plausible contributors, but this run did not isolate which one caused a particular death. **Confidence: high** for the measured aggregate mismatch; **medium** for those candidate mechanisms.
2. **The same opening ratio has scale-dependent outcomes in this scenario.** The starvation-event intervals for 2/1 (37.51–56.71%), 4/2 (73.33–88.30%), and 6/3 (96.30–100%) do not overlap. Median Fox births rise from 2 to 5 to 11, while Hare births are zero in every seed in these three arms. This pattern shows materially different outcomes across the tested sizes; because the metric is an “any Fox” seed event and births remain enabled, it cannot be read as the per-Fox hazard or fitted as a scale law. **Confidence: high** for the seed-level differences; **low** for any linear/exponential interpretation.
3. **More prey helped the six-Fox arm, but did not reliably preserve Hares.** At 6 Foxes/6 Hares, 6/20 seeds recorded Fox starvation (30%, 95% Wilson 14.55–51.90%) and 5/20 retained Hares (25%, 95% Wilson 11.19–46.87%). At 6/1 and 6/3, Fox-starvation events occurred in all 20 and all 100 seeds respectively, and every Hare went extinct. The 6/6 panel is small and is a comparison of complete starting setups; it suggests prey supply can improve Fox food access but is not a demonstrated stable ratio. **Confidence: medium** for the direction in these tested setups; lower for the exact 6/6 rates.
4. **Predation is the primary Hare loss in the three scaled 2:1 arms.** Those runs record 599 Hare combat deaths and one Hare starvation death, with no Hare births. The 6/6 control records 206 combat and 61 starvation deaths, with births in some seeds. Fox reproduction also remains active: the scaled arms record 160, 530, and 1,059 Fox births across 100 seeds each. The experiment therefore captures a predator–prey system with reproductive feedback, not a fixed conversion of each opening Fox into a fixed number of Hares. **Confidence: high** for the recorded counts.

## What this answers about “how many Hares is a Fox worth?”

For **maintenance energy only**, a Fox needs half of a Hare carcass per 600-tick node under the current values: 48 Fox energy divided by 96 Hare energy. In that limited bookkeeping sense, one Hare contains two Fox maintenance budgets.

For **Fox survival while Hares remain present**, this experiment supplies no single stable Hare-per-Fox ratio. The same 2:1 ratio changes from a 47% to an 82% to a 100% seed-level Fox-starvation event rate as both starting populations scale. For **Hare survival**, even the 1:1 six-Fox control ends with Hare extinction in 15 of 20 seeds. These outcomes rule out using the calorie ratio alone as a game-balance guarantee; they do not establish the right replacement ratio.

## Recommended next actions

1. **Keep the 96-energy carcass and 48-energy upkeep labelled provisional; do not infer a universal ratio from them.** The next balance decision should distinguish maintenance energy from ecological survival and Hare preservation.
2. **If the next question is why the same 2:1 ratio scales differently, isolate Fox reproduction or measure individual food intake and survival exposure.** A matched reproduction-on/off diagnostic on the same three starting setups would test whether Fox population growth explains much of the increased starvation-event incidence. Record the change as a separate experiment and keep the current report immutable.
3. **Treat the 6:6 arm as a follow-up candidate, not a selected starting balance.** If it is a serious candidate, extend that control beyond 20 seeds before relying on its exact survival rates, then review a manual run for player-facing pressure and clarity.

Josh remains the decision owner for keeping or changing the production values. These are AI recommendations, not a recorded human decision.

## Uncertainty and limits

- Only one map, map size, tick window, combat mode, and no-action configuration were tested. The fixed-six-Fox controls remain at 20 seeds each; the extension increased only the three 2:1 scale arms to 100.
- Fox reproduction is enabled. “Any Fox starved” pools seeds with different starting Fox counts and later Fox births, so its rate is affected by the number of opportunities for an event. It is not an individual survival probability.
- The arms share seed IDs within each panel, but changing the population counts changes the realized initial layout as well as density. Matched seeds compare whole starting setups, not a perfectly controlled counterfactual in which only the number changes and all animal positions remain fixed.
- Batch reports retain aggregate outcomes and population histories, not detailed combat rolls or individual death timing. The experiment cannot establish why a particular Fox missed food or when that happened.
- The full Unity suite retained 12 failed tests and no visual inspection was done ([validation evidence](REPORT.md#provenance-and-validation)). The 160 focused species-domain tests passed.
- The portable batch assembly targets .NET 8 and ran under installed .NET 10.0.4 through major-version roll-forward. Unity-reference checks, the snapshot, and all batch provenance are retained, but a native .NET 8 runtime was not available.
