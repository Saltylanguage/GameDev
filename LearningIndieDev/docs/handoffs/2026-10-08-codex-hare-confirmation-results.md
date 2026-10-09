# Hare purchase confirmation: completion and results

Date: 2026-10-08. Author: Codex / GPT-6. Human decision owner: Bevin.
Status: approved experiment completed and reviewed locally; production tuning and further experiments remain open.

Bevin reported that the batch appeared finished. Independent review confirms all
96,000 runs and automatic analysis/purchase reports completed. This review adds
interpretation and documentation; it does not start another batch or change
production settings. The preceding tooling publication is already pushed through
`e250d32f` on BevBranch. These newly completed results are not in that publication.

## Experiment and evidence

- Protocol: [S4-04 confirmation](../Research/Experiments/S4-04-Hare-Purchase-Confirmation/PROTOCOL.md).
- Artifact root: `artifacts/cellsim-hare-confirmation-20261008-143725`.
- Four retained candidates, four paths including Skip all, six purchase policies.
- 1,000 matched fresh seeds 60000-60999 per arm; 96 arms; 16 workers; 1,920 chunks.
- Same frozen screen snapshot, runner and scientific settings; no pooling of the
  earlier 100-seed screen with this confirmation cohort.
- Horizon: six continuous 100-tick phases; eligible purchase windows at ticks
  100-500; price 10 Field Data; wallet carryover and charge-on-success placement.
- Simulation elapsed: 17,434.28 seconds / 4h 50m 34s, 5.506 runs/s.
- Pipeline completed: `2026-10-08T23:37:50.8078828Z` / 19:37:50 EDT.
- Raw aggregate: 4,841,080,306 bytes. All output and frozen input/tool digests verified.
- Batch identity: `036565a25ed21c1853ad35fec8b3ee8c2474b8dc2114c21b0e29d65e3f067087`.
- Raw SHA256: `757da8d6614a1e4ec4d2602ad09ecd7fb31e74534b712d7291c2fa6abf5f1942`.

The completion review verifies exact per-arm seed coverage and 1,000 Valid
APS/AHS observations per arm. All 480,000 assigned purchase windows are unique;
432,092 were reached and 47,908 were not. Every reached window reconciles
population additions, spent = 10 x added, and wallet before + earned - spent =
wallet after >= 0. Stop reasons were policy-cap 341,894, target-met 47,418,
insufficient-currency 42,780, and not-reached 47,908. No placement/capacity stop
was recorded. Stderr was empty. The live dashboard reports Completed, 96,000
validated runs, all 96 cells complete, and zero active workers.

Reproducible review scripts and outputs:

- [Review with integrity/coverage/ledger assertions](../../artifacts/cellsim-hare-confirmation-20261008-143725/purchase-analysis/review-confirmation.py).
- [Focused tables and interpretation](../../artifacts/cellsim-hare-confirmation-20261008-143725/purchase-analysis/focused-review.md).
- [Full 96-arm purchase report](../../artifacts/cellsim-hare-confirmation-20261008-143725/purchase-analysis/report.md).
- [Direct matched-policy comparison script](../../artifacts/cellsim-hare-confirmation-20261008-143725/purchase-analysis/compare-policies.py)
  and [output](../../artifacts/cellsim-hare-confirmation-20261008-143725/purchase-analysis/policy-comparisons.json).

Raw, SQLite and full CSV artifacts remain local under the ignored artifact root.

## Findings

Restoration and late-five improve both Hare and all-species survival over each
path's own no-buy control on all twelve candidate/skill-path arms. All corresponding
unadjusted 95% paired mean intervals have positive lower bounds (minimum +2.32 pp
for restoration, +2.19 pp for late-five). Restoration also improves both observed
endpoints over Skip all using the same restoration policy in all twelve arms;
that point-estimate statement does not imply every Skip-all interval excludes zero.

D5/S25 restoration supports all three paths:

| Path | Hare alive, none to restore | All alive, none to restore | Delta all vs no-buy | Delta all vs Skip all + restore | Mean APS | Mean AHS | Mean spend |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Trailblazer | 46.6% to 63.7% | 31.8% to 43.4% | +11.6 pp | +5.5 pp | -0.668 | -0.302 | 71.61 |
| Warren | 38.4% to 57.6% | 31.2% to 49.4% | +18.2 pp | +11.5 pp | -0.623 | -0.270 | 68.37 |
| Gardeners | 38.2% to 54.5% | 35.8% to 52.2% | +16.4 pp | +14.3 pp | -0.654 | -0.102 | 63.16 |

The earlier C2/S14 Gardeners restoration decline did not reproduce: all-species
survival is 36.0% vs 29.4% no-buy (+6.6 pp, CI +3.6 to +9.6), and Hare survival
is 42.9% vs 37.6%. Keep C2/S14 among the retained candidates.

The D5/S25 Gardeners each-five screening maximum of 59% did not repeat. Its
confirmation result is 47.7% vs 35.8% no-buy (+11.9 pp), while restoration reaches
52.2%, the highest observed all-species survival in the confirmation panel.

For D5/S25 Gardeners, direct policy comparisons use the same 1,000 seeds:

| Comparison | All-species survival difference | Unadjusted 95% paired mean CI | Wins / losses | Different observed durations |
| --- | --- | --- | --- | --- |
| Restore vs late-five | +4.9 pp | +2.1 to +7.7 pp | 128 / 79 | 444 |
| Restore vs each-five | +4.5 pp | +0.3 to +8.7 pp | 255 / 210 | 709 |
| Each-five vs late-five | +0.4 pp | -3.9 to +4.7 pp | 239 / 235 | 743 |

Late-five spends 32.51 Field Data on average versus 213.67 for each-five, while
their all-species survival is 47.3% and 47.7%. This is no clear evidence of an
each-five survival advantage; it is not a statistical equivalence claim.
Restoration spends 63.16, offers higher survival, and retains APS nearer to
the no-buy value than each-five. The focused report contains all six policies,
APS/AHS means, and the complete twelve-path restoration table.

## Interpretation and next decision

Keep all four candidates. Prioritize D5/S25 restoration and late-five as the
lower-cost timing comparison; retain C2/S14 for its smaller context and different
score/spend profile. Review matched gameplay examples to understand deaths,
births, purchased survivors, and pace before choosing the next experiment.
Restoration thresholds (50/75/100% of starting population), caps (1/3/5), and
eligible-window variants remain proposals, not approved or launched work.

All survival rates refer to tick 600. APS/AHS are means of cumulative per-run
scores including early endings, not pooled or survivor-only scores. Purchased
Hares are ADD and excluded from biological replication fitness. Different
observed durations are retained in paired comparisons. Intervals are unadjusted
for multiple comparisons; shared seeds do not make paths independent studies.
No global optimum, tick-700 viability, or improved player enjoyment is established.
Prior Unity/reference tests remain frozen historical validation, not fresh Unity
checks in this completion review. Trello synchronization remains outstanding.
