# AI analysis — fresh-node Fox-count sweep

**Analyst:** Codex (GPT-6), November 10, 2026. **Method:** code inspection and bounded inference from the [factual report](REPORT.md) and [per-seed evidence](paired-summary.json); no trained predictor was invoked. **Decision owner:** Josh. No balance value is approved here.

## Summary

The clarified node reset works and produces the intended second-node pressure: fixed Fox populations return after often disappearing during node one, and 51–61% of no-action seeds fail by node two across the three tested counts. The opening remains too lethal for the design's “possible but unlikely” failure aim: 15–20% fail before the first shop. None of 16, 18, or 20 Foxes meets both screens. This is evidence for changing the *shape* of pressure across nodes, not evidence that one of these three counts is a safe starting value.

## Findings and interpretation

1. **Node two now has a real predator reset.** Foxes were extinct at node-one close in 59, 64, and 80 of 100 seeds for the 16, 18, and 20 arms, then reopened at exactly the declared 16, 18, or 20. Every seed carried only the Hare count. Among node-one survivors, 36/81, 36/85, and 41/80 failed during node two ([report](REPORT.md#seed-outcomes)). This supports the hypothesis that restoring predators, rather than retaining a depleted Fox population, creates the second-node ramp. **Confidence: high** for the measured mechanism.
2. **The first-node screen fails.** The best observed opening was 18 Foxes at 15/100 failures; its 95% Wilson interval is 9.3–23.3%, above the contract's 5/100 screen. The 16 and 20 arms failed 19/100 and 20/100. Their intervals overlap substantially; the 18-Fox arm is not established as a reliable winner. **Confidence: high** that the three tested arms miss the screen, **low** in their exact ranking.
3. **Predation is the primary measured mortality.** Across confirmation seeds, Fox kills accounted for 6,885/8,168, 6,246/7,245, and 5,777/6,655 Hare deaths in node one. Plants were present at both node closes in every seed; the smallest node-one closing stock was 383 or more. Local starvation still occurred, so this does not eliminate food as a secondary factor. **Confidence: high** for the recorded death totals and node-close Plant stock.
4. **Starting count changes do not yield a clean monotonic effect.** On matched seeds, 16 versus 18 Foxes produced 24 failures unique to 16 and 20 unique to 18; 18 versus 20 produced 17 and 27. The grid generator places Foxes before Hares and Plants using a shared shuffled index list, so changing Fox count also shifts later species' starting positions. That spatial shift is a plausible contributor to reversals, not a proven cause. The independent 100-seed rates remain valid for each full starting setup, but paired differences cannot be attributed solely to predator number. **Confidence: high** in the placement coupling from code inspection, **medium** that it materially affects outcomes.

## Suggested next decision

**First:** keep the fresh-node rule and avoid selecting 16, 18, or 20 as a final balance value. Test a lower first-node Fox range with the same fixed 55 Hares and 400 Plants, using an opening safety screen. Record the exact starting layouts so any count effect can be separated from placement effects. If a single count that makes node one safe also makes node two too safe, test *fixed, authored counts per node* (a gentler first node and a stronger second node). That preserves static, testable populations while giving the two difficulty targets independent controls. This is a proposed experiment, not an implementation or balance decision.

**Then:** test avoidance, survival, and faster reproduction as actual obtainable Mutation choices on the corrected route. For each, preregister the expected local signal (avoided hunts, surviving hits, or births), measure the seed outcome, and play representative cases. The present no-action panel cannot show whether those choices are available, effective, or fun. Existing P3 work supports bounded, held-out directional forecasts; it has not calibrated a numerical predictor for this fresh-node scenario, so a trained-model value recommendation would add little confidence here ([P3 decision](../../P3_GATE_DECISION_2026-09-06.md)).

## Risks and limits

- The no-action proxy deliberately continues observation after extinction, whereas the live Field Data journey ends when the played species dies. Cumulative failure counts agree with that terminal interpretation, but later ecology on extinct seeds is diagnostic only.
- A reset recreates Hares at default age and energy. This is the chosen design contract; it can improve node-two survival relative to carrying tired individuals. The run did not isolate that effect from the Fox and Plant reset.
- The 50–70% and at-most-5% bands are screens in this experiment contract, not player-tested acceptance criteria. Player readability and the three Mutation strategies remain open.
