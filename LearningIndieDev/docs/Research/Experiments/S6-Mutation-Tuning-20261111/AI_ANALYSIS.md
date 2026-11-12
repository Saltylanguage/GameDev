# S6 predator-first Mutation calibration — analysis

**Analyst:** Codex (GPT-6), November 11, 2026.  
**Evidence:** the [factual report](REPORT.md), [paired seed summary](paired-summary.json), and the validated CellSim.Batch run files.  
**Decision owner:** Josh. No final Mutation balance values are approved here.

## Findings

1. **The starting pressure now matches the design target.** With 12 Foxes at Node 1 and 48 at Node 2, the no-upgrade control failed in 3% of seeds at Node 1 and 60% cumulatively by Node 2. Both point estimates meet the intended screens, and their intervals are consistent with those targets. I recommend keeping these counts as the manual-test baseline rather than reopening the Fox-count sweep first. **Confidence: high** for these measured rates; lower for their generalization beyond this Forest Edge configuration.

2. **Tough Hide is the only tested single-rank candidate that reaches the target point estimate.** `+2 Health` reduced cumulative Node 2 failure from 60% to 42%; among Node 1 survivors, Node 2 failure was 40/98 (40.8%). Foxes recorded 12,521 nonlethal hits, directly confirming that Hares survived successful attacks. In matched seeds, it rescued 31 control failures and created 13 new failures. Its 95% interval of 32.8–51.8% is still wide, so treat it as a provisional candidate. **Confidence: medium** that the mutation moves survival in the desired direction; high that its hit-survival mechanic was exercised.

   Tough Hide also keeps more Hares alive for longer, which increases exposure to both Foxes and food pressure. The lowest Plant checkpoint was 249 instead of control's 381, and starvation deaths rose from 3,558 total to 4,300 across both nodes. No checkpoint reached zero Plants. That is consistent with a meaningful tradeoff while keeping Fox pressure dominant, but it needs a human playtest to assess whether the extra survival and later starvation are readable and satisfying. This explanation is an inference from the measured totals, not an isolated causal test.

   Only rank 1 was tested. The existing Tough Hide cap remains 10 ranks, so repeated `+2 Health` ranks have not been balanced and may be too strong. Do not treat this result as validation of the full upgrade path.

3. **General Faster Movement is not a reliable avoidance answer at `+0.5`.** It raised Hare movement steps by 31.6%, but did not produce a clear attack reduction: Fox hits were 12,721 versus 12,273 in control. It had 55% cumulative Node 2 failure, only five percentage points below control; the matched counts were 26 rescues and 21 regressions. Node 1 failure was 6% versus the baseline's 3%. The data do not support increasing this general movement bonus. **Confidence: medium** that this value is not a useful standalone response; the comparison still has seed variation and a wide rate interval.

4. **Keen Senses, the tested two-choice avoidance path, and Escape Instinct did not improve the small-panel result.** Keen Senses `+1` and the movement-then-senses path each failed in 11/12 seeds by Node 2, versus 8/12 in their matched controls. Escape Instinct `+8` points had 8/12 failures, also matching control. These are exploratory screens only. The movement-plus-senses path also spent two Mutation choices, so it is not an equal-cost comparison to a single-rank option. **Confidence: low** on the exact rates; sufficient to avoid treating these values as proven answers.

   The code has no direct event counter for newly detected predators or successful pre-contact escapes. Hare movement, Threatened-state ticks, Fox proximity, attack activity, and survival are only proxies. This limits our ability to explain why these avoidance candidates missed.

5. **Reproductive Drive `+10` points changed reproduction but did not counter the Fox pressure.** Eligible attempts succeeded 59.0% of the time versus 49.3% in control, and births increased 12.3%; cumulative Node 2 failure nevertheless rose to 69%. The `+20` point screen produced 21.5% more births than its control, but ended with lower mean Hare counts and two Node 1 failures out of 12. Fox kills and starvation also rose in the confirmed `+10` arm. The evidence does not support simply raising reproduction chance further. One plausible explanation is that extra offspring create more Fox feeding opportunities and food demand, but these runs did not isolate those pathways. **Confidence: medium** that the tested chance values do not solve this scenario; low on the exact cause.

   The worktree's candidate value is back at `+10` percentage points per rank. The `+20` value was used only for its isolated diagnostic screen; it is not left in the current code.

## What this says about the three strategies

The **survival** strategy has one provisional value worth manual review. The **avoidance** and **faster reproduction** strategy categories remain plausible, but the tested mechanics do not yet meet the survival target. This is a result about these values and mechanisms, not evidence that the categories themselves cannot work.

## Recommended next step

Use the confirmed no-upgrade baseline and Tough Hide `+2 Health` as the first manual comparison. For the next simulation iteration, keep the opening and Fox schedule fixed and change one response at a time:

- For avoidance, test movement that increases only while a Hare is Threatened, instead of increasing movement in every state. Add or expose a direct count for Fox attacks avoided if testing pre-contact dodge.
- For reproduction, test a separate timing or litter-size lever rather than increasing attempt chance again. Keep its births, Fox kills, starvation, Plant checkpoints, and Hare carry in the same report.

Those are proposed hypotheses and need measurement before they become Mutation values. The current Faster Gestation effect is still an unresolved project decision, so it should not be silently promoted as the answer.

I did not use a predictive model. The current research project does not provide a predictor calibrated to these fresh-node, fixed-population conditions; direct matched seeded runs are the stronger evidence for this decision. A larger seed panel can narrow Tough Hide's interval if its direction and readability remain promising after manual play.

## Follow-up analysis after 100-seed candidate confirmation

This section supersedes the earlier recommendation to begin a manual comparison. Josh asked to prepare avoidance and reproduction first; **no manual review has started**.

1. **The two-rank Quick Escape result did not hold on confirmation seeds.** The 12-seed screen had 6/12 cumulative Node 2 failures, but the 100-seed matched panel recorded 66/100 (95% Wilson interval 56.3–74.5%), compared with 60/100 (50.2–69.1%) for control. Paired seeds had 13 control failures rescued and 19 new failures. Quick Escape raised movement steps by about 29.5% and lowered Fox-adjacent observations per 1,000 Hare observations slightly (67.02 vs. 69.19), yet Fox kills increased (12,995 vs. 12,273). This candidate does not meet the outcome goal. **Confidence: medium** that this two-rank path is not viable at its tested timing/value; the screen reversal shows why the 12-seed result was insufficient.

2. **Larger Minimum Litter raised births and also worsened survival.** Its two-rank path had 70/100 Node 2 failures, 10 percentage points above control; paired seeds had 16 rescues and 26 new failures. Births increased from 11,019 to 12,693, Fox kills from 12,273 to 13,410, and the minimum Plant checkpoint fell from 381 to 244. No Plant checkpoint reached zero. The added food demand and Fox activity are consistent with the outcome, but these runs do not isolate a single cause. **Confidence: medium** that the tested litter path is not a useful response in this scenario.

3. **Higher ranks did not improve either path.** The rank-three screens had 10/12 Node 2 failures for both Quick Escape and Larger Minimum Litter. That is a small-panel result, but it provides no reason to keep increasing those doses. The separate 36-tick gestation probe also increased births without a survival gain in its screen and had more starvation deaths plus a lower Plant checkpoint; it was a run-start stat override, not a tested Mutation.

4. **No avoidance or faster-reproduction candidate is ready for manual review.** Tough Hide remains the only candidate with a confirmed point estimate in the 35–45% cumulative Node 2 failure window. The new Quick Escape code is still only a provisional local candidate; it has not been accepted as a final Mutation. The reproduction strategy needs a different value or mechanism before it is useful in this opening scenario.

## Final preparation checks before manual review

5. **Threat Exposure rank 3 did not establish a reliable avoidance candidate.** Ranks 2 and 4 tied control at eight Node 2 failures in 12 seeds, while rank 5 had nine. Rank 3 had six, so it received a 100-seed confirmation: the candidate ended at 62/100 failures (52.2–70.9% Wilson interval) versus 60/100 for control. Paired outcomes were 22 control failures rescued and 24 new failures; Fox attempts fell by 2.1%, but survival did not improve. This is a three-choice path, and it remains above the 35–45% failure target. Compact run data do not record a direct count of avoided contacts. **Confidence: medium** that this rank-3 path is not a useful answer at the tested timing; this does not rule out a different avoidance mechanism.

6. **Faster gestation worsened the 100-seed outcome even when applied from run start.** At 36 ticks, cumulative Node 2 failure rose to 70/100 (95% Wilson interval 60.4–78.1%) from 60/100 in control. The paired panel had 14 rescued failures and 24 new failures. Births rose 9.2%, while starvation deaths rose 48%, Fox kills rose 9.1%, and the lowest Plant checkpoint fell from 381 to 123. The diagnostic does not prove every possible gestation effect would fail, but it makes a 36-tick “faster gestation” Mutation a poor next candidate; a phase-2 Mutation would receive less time to act and cannot be assumed to improve on this result. **Confidence: medium** that this tested value is unsuitable for the current scenario.

**Preparation decision:** do not start the manual comparison with the tested avoidance and faster-reproduction options. Neither strategy has a candidate that meets the simulation target with a clear, repeatable mechanism. Keep Tough Hide as the provisional survival comparison and design a different avoidance mechanism and reproduction mechanism before scheduling manual review. No manual review, commit, or push has started.

The outcomes above use direct matched simulation evidence. I did not use a predictive model: the research project still has no predictor calibrated to this fixed-count, fresh-node setup. Full tables, reset checks, and provenance are in [`FOLLOWUP-REPORT.md`](FOLLOWUP-REPORT.md).
