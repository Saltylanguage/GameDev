# AI analysis - S4 Hare paired-seed threshold confirmation

**Generated:** 2026-10-06 (America/New_York)  
**Model:** OpenAI Codex; exact model build is not exposed in this workspace.  
**Evidence:** [factual report](REPORT.md), frozen `experiment-contract.json`, full reports and independent validation under `artifacts/s4-paired-confirmation-20261006-600-40-25-wrapped-200/`.

## Executive summary

All three Hare interventions improved the chance of clearing the player-defined all-species threshold over skip-all on the same fresh seeds. Seed Dispersal had the highest observed rate at 58/200 (29%); the two Gardeners orders were close at 50/200 (25%) and 57/200 (28.5%). On matched seeds, each strategy won more clear outcomes than it lost to control. The two acquisition orders were not meaningfully different on this sample.

The main balance signal is a trade-off. Dispersal helps preserve larger Hare populations and raises threshold-clear frequency, while Gardeners Digestion-first tends to leave more Plants but finishes with fewer Hares than the reverse order. Even the best arm cleared fewer than one in three runs, so these values do not yet establish a comfortable early-node difficulty.

## Findings

1. **Seed Dispersal is the best first skill candidate in this bounded challenge.** It cleared 29% versus 12.5% control. It cleared on 47 matched seeds where control failed, while control alone cleared on 14; the paired risk difference was +16.5 percentage points (approximate 95% CI +9.2 to +23.8; exact two-sided McNemar p=0.000027). This repeats the direction of the separate 100-seed screen. The interval and test apply only to this fixed 600/40/25 wrapped node.

2. **Gardeners also improves node clears, but the order is unresolved.** Digestion-first cleared 25% (+12.5 points paired to control; 35 candidate-only versus 10 control-only). Dispersal-first cleared 28.5% (+16 points; 47 versus 15). The direct paired order difference is -3.5 points for Digestion-first (approximate 95% CI -11.5 to +4.5; exact McNemar p=0.464). Treat the 7-clear gap as noise until a larger confirmation or a design-specific reason favors one order.

3. **The five-animal cutoff is chiefly testing predator persistence after a successful intervention.** Among horizon-reaching non-clears, fewer than five Foxes appeared in 70/78 Seed Dispersal failures, 81/83 Digestion-first failures, and 73/77 Dispersal-first failures. Seed Dispersal's Hare support raises successful clears, but the next limiting population is often Foxes. This points to ecology-level counterplay or conditions in the node, not simply making Hares breed faster.

4. **The two Gardeners orders expose a readable growth/resource trade-off.** Digestion-first has median final Plants 320.5 and Hares 4; Dispersal-first has 256.5 Plants and 6.5 Hares. Seed Dispersal alone has median final Hares 15 and Plants 194.5. Median sAVI is higher for Digestion-first (0.124) and Seed Dispersal alone (0.115) than for Dispersal-first (0.104), but the all-species clear rates do not establish a clear order winner. The median values describe different ending populations and do not by themselves establish a healthy equilibrium.

5. **Human-readable slashlines and counters support the clear result but do not explain its cause.** All 800 FPO lines reconciled; sAVI's median rose from 0.050 control to 0.104-0.124 in the intervention arms. Seed-drop successes and food also reconciled. The experiment did not intervene on one mechanism at a time, so do not claim that planted food caused the clear-rate lift. cAVI is N/A in 64-71 runs per arm; that means its denominator did not apply and must not be read as zero crowding.

## Confidence and limits

- **Moderate confidence, narrow scope:** Seed Dispersal and either tested Gardeners path raise strict clear odds above skip-all at 600/40/25 on wrapped Forest Edge, 36x20, Hare player, with this ruleset and horizon. There are 200 matched new seeds per arm and the direction is consistent across the earlier screen and this independent panel.
- **Low confidence, order preference:** Neither Gardeners order is established as superior.
- **Low confidence, causal mechanism:** seed planting, energy intake, Fox reproduction, and trajectory feedback were not isolated. Death and slashline measures are descriptive, not causal mediation.
- This is a six-phase node, not a full journey. The runner uses scripted choices and fixed choices; it does not model random offers, adaptive human decisions, later nodes, rewards, or a long-running ecology. A tick-600 count above five is a node clear only, not long-term ecological balance.
- Three intervention-v-control comparisons were planned, but the exact p-values are unadjusted. Confidence intervals and effect sizes should lead interpretation.

## Recommended next work

1. **Do not retune skill values yet.** Preserve current numbers while the team decides what success rate makes the first Forest Edge node feel fair. Current strategies improve odds but leave the clear rate at 25-29%.
2. **Use the failure profile to select the next ecology question.** If Foxes must remain above five for a node clear, test how a player can maintain prey availability and predator viability together. Consider an explicit ecology/node modifier or a future Fox-facing counter; keep that design separate from Hare skill edits.
3. **Before adding code, test the intended challenge with actual available choices.** A small next screen can compare realistic phase-by-phase choices using the real offer pool, including skip, rather than repeating a fixed skill forever. Keep the same seed panel or use a new disjoint matched set, and retain whole-ecology clears plus player-readable slashlines.

**AI recommendation:** keep Seed Dispersal and both Gardeners paths as promising candidates, record no winning order, and do not change the authored population or skill values from this result alone.  
**Human decision:** Bevin / Salty review pending.
