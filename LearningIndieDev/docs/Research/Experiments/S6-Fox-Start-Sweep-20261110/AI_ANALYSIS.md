# S6 starting Fox count and node-two pressure — AI analysis v1

- **Analysis ID:** `S6-FOX-START-A1`
- **Source:** [Factual report](REPORT.md), [per-seed summary](paired-summary.json), and the [approved contract](CONTRACT.md)
- **Analyst / method / timestamp:** Codex (GPT-6); matched-seed interpretation of the frozen simulation; November 10, 2026. No trained predictive model was used.
- **Decision state:** Recommendation only. Josh has not approved a balance value or new scenario rule.

## Executive summary

All three starting Fox counts are valid simulation inputs, but none produces the desired no-action difficulty curve on the disjoint 100-seed panel. Hare extinction by the end of node one is 19% at 16 Foxes, 15% at 18, and 20% at 20, versus the contract's provisional at-most-5% screen. Cumulative extinction by node two is only 25%, 26%, and 31%, versus the 50–70% screen for an approximately 60% target. These are outcomes for fixed tested seeds, not measured player failure rates or a population-wide probability.

The sweep also exposes a change in pressure. Fox kills dominate Hare deaths in node one, while node two records far more Hare starvation deaths than Fox kills. Most node-two Hare extinctions follow the first zero-Plant tick. That is consistent with food depletion becoming the later ecological constraint, although the telemetry does not prove that starvation caused every final death. A starting-count adjustment across 16–20 Foxes alone is therefore a poor next balance lever for a gentle predator-led opening followed by a sharper predator-led second node.

## Findings and confidence

| Finding | Evidence | Interpretation and confidence |
| --- | --- | --- |
| The 12 known seeds overstated baseline failure. | The diagnostic 20-Fox arm failed 6/12 by tick 600 and 7/12 by 1200; the disjoint 100-seed arm failed 20/100 and 31/100. | **High** that the small diagnostic panel was unsuitable for selecting a difficulty value. The 100 consecutive confirmation seeds are stronger, but still are not a random sample of all seeds. |
| Fewer starting Foxes do not reliably protect a given seed. | Relative to 20 Foxes, 16 rescued 13 first-node seeds and introduced 12 different first-node failures; 18 rescued 17 and introduced 12. First-node failure counts across the three arms are not monotonic. | **High** for this matched panel. The nonlinear ecology changes after the first encounters; an initial population change is not a guaranteed directional route change. |
| Initial predator contact responds to starting Fox count, but later activity reverses. | Across 100 seeds, Fox kills in ticks 1–100 rise 2,568 → 2,960 → 3,248 for 16 → 18 → 20 Foxes. By tick 600 the totals are 6,885 → 6,246 → 5,777; Hare births are 4,379 → 3,795 → 3,023. | **Medium** causal hypothesis: lower initial Fox pressure leaves more Hares able to reproduce and sustain later predator encounters. These aggregate counts do not isolate individual survival, encounter opportunities, or the random trajectory. |
| Node two is not primarily a sustained Fox crisis in these runs. | Foxes are already extinct at tick 600 in 59, 64, and 80 of 100 seeds. Node-two Fox kills are 461, 320, and 91, compared with 25,144, 25,428, and 22,878 Hare starvation deaths. Plants reach zero by tick 1200 in 63, 62, and 54 seeds. | **High** that starvation and plant exhaustion dominate the recorded later deaths; **medium** that they are the main cause of late route failure, because failure timing and final death attribution need a targeted trace. |

## Recommended next design move

1. **Josh / scenario direction:** Keep the locked Fox-overhunting premise, but treat opening safety and second-node escalation as separate controls. The next controlled experiment should make the first node safe on a broad disjoint seed panel, then test a visible Fox pressure increase after the first choice. A timed introduction, replenishment, or other legible predator change is a design hypothesis, not an approved rule. It should be measured against both stage-specific extinction counts and actual Fox encounters.
2. **Josh and Sim / resource constraint:** In a separate arm, tune Plant renewal or Hare consumption enough to keep gentle overgrazing in the background through node two. Measure Plant stock, first-zero time, starvation deaths, and whether rescued Hares remain viable. Changing Fox pressure and Plants together would obscure which change fixes the curve.
3. **Josh / player choice:** Once the no-action pressure has the right shape, test avoidance, survival, and genuinely faster reproduction as obtainable first choices in the actual fresh-profile route. The earlier diagnostic showed the three strategy families were not consistently offered and `larger-minimum-litter` did not increase early births. This sweep did not test Mutations or offer access.

The smallest useful next contract is a staged ecology comparison with two separately varied levers and a disjoint confirmation panel. It should reject any candidate that reaches roughly 60% cumulative failure mainly through first-node losses, or that reaches the rate through Plant collapse while Fox pressure disappears. Josh should choose the specific escalation and resource mechanism before implementation. No production balance value changed in this sweep.

**Uncertainty:** The run is a deterministic portable proxy of the authored route, with a verified 20-Fox diagnostic-prefix match to previous Unity-derived evidence. The 16- and 18-Fox overrides have not been authored into Unity or played by new users. The 1200-tick window and no-action policy do not establish enjoyment, Mutation value, or eventual journey success.
