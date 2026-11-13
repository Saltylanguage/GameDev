# S6 predator-first Hare journey — evidence and forecast

- **Status:** Design forecast for Josh and Sim; no experiment or balance change approved here.
- **Date / analyst / method:** November 9, 2026; Codex; evidence synthesis and falsifiable directional hypotheses. No trained predictor was invoked.
- **Published source inspected:** `origin/feature/s6-fun-pass` at `a2c38b33`, after its fast-forward to `BevBranch`. November GLJP source and handoffs are published; raw test artifacts remain local to their original worktree.
- **Design authority:** [S6 control record](../Sprints/S6-control-record.md#november-9-predator-first-design-decision).

## What is known now

| Question | Current evidence | Limit |
| --- | --- | --- |
| Is the current authored failure predator-led? | [GLJP-06](https://trello.com/c/eo0iqoRL/154-gljp-06-diagnose-route-seed-survival-mismatch-before-further-development) reports seed 10100 losing its last Plant at tick 915 and its last Hare at tick 1210; the final Hare death was recorded as combat. | Plant exhaustion precedes the final hit. One seed cannot establish the predominant failure cause or a failure rate. |
| Is the opening safe and the second node near 60% no-intervention failure? | Seed 10100 passed the first 600-tick node and reached tick 1210, just beyond the second node's tick-1200 boundary. | No current-November matched seed distribution is published. The target rate is unmeasured. |
| Can a new player try three Mutation responses? | [GLJP-01](https://trello.com/c/dR2z6ENf/149-gljp-01-replace-hare-genome-stat-nodes-with-researched-journey-unlocks) records a fresh-profile research gate: an unresearched journey has no Mutation offers. Avoidance and litter capabilities have focused checks; Faster Gestation and final catalog decisions remain open. | The intro's starting research or trial-access rule is undecided. Focused checks do not prove route viability. |
| Do older strategy rates transfer? | The [S4 offer-aware screen](Experiments/S4-03-Strategy-Choice-Policy-Screen-600-40-25/REPORT.md) found 32/200 skip-all clears, 30/200 Trailblazer, and 34/200 Warren under a 600-tick, 600/40/25, all-species clear rule. [EX-007](Experiments/EX-007-Predictive-Statline-Interventions/AI_ANALYSIS.md) found movement's Fox-kill effect changed direction across panels. | These are historical scenarios, offer policies, and outcome definitions. They are neither the November route baseline nor evidence for the new 60% target. |
| Is Predictive AI ready to prescribe values? | The [P3 gate](P3_GATE_DECISION_2026-09-06.md) accepts bounded, held-out change-impact analysis under declared inputs. | It does not validate a general predictor, numerical balance forecast, or fun claim for this scenario. |

## Directional forecasts to test, not balance claims

| ID | Hypothesis and observable prediction | Confidence / disconfirming evidence |
| --- | --- | --- |
| S6-P1 | If Fox pressure rises while current food dynamics remain, early Hare combat deaths should rise, but the dominant later failure may still be plant depletion. Count deaths and food trajectory before each node boundary. | Medium for the early direction, low for terminal cause. Disconfirmed if Fox contact/kill rates do not rise on matched seeds, or if plants remain healthy through the route. |
| S6-P2 | Increasing Hare survival or births without changing food renewal is likely to increase grazing demand and may move a loss from predation to starvation. Compare consumption, Plant minimum, and death causes, not only final Hare count. | Medium, regime-dependent. Disconfirmed if retained Hares do not increase consumption or if Plant recovery offsets it. |
| S6-P3 | Avoidance should reduce successful Fox kills **per relevant encounter** if detection and escape actually trigger. Faster movement or vision alone may change contact timing and food access without a reliable endpoint gain. | Medium for a triggered local effect, low for route survival. Disconfirmed if verified triggers leave encounter outcomes unchanged. |
| S6-P4 | Defensive survival should increase the share of Fox attacks that Hares survive. Repeated contact may erase that local gain at the journey level. | Medium for block events, low for route survival. Disconfirmed if eligible attacks do not show more surviving Hares. |
| S6-P5 | Faster reproduction can replace predation losses if Hares remain fed and have mating opportunities; it may later accelerate grazing. The first post-choice node should show a birth or litter signal before calling it an observable introductory answer. | Low to medium. Disconfirmed if reproduction rarely triggers after selection or if births do not offset losses on matched seeds. |

No numerical effect size or success-rate estimate is justified until the November source, scenario, action schedule, and matched seed panel are fixed. These predictions should be scored on held-out seeds after an approved experiment, including misses and changed failure causes.

## Smallest useful next comparison

1. Verify the merged November source and freeze a native current snapshot, four-node route, starting populations, Fox scent setting, neutral Genome, research/offer access, seed set, and no-intervention action schedule. Keep the first Mutation acquisition after tick 100.
2. Use a small matched diagnostic set to check first-node safety, second-node timing, Fox kills versus plant loss, and whether each selected Mutation actually triggers. Compare no intervention with one avoidance, one survival, and one reproduction choice; hold later actions fixed.
3. Once the mechanisms and player offer flow work, freeze disjoint held-out seeds large enough to estimate the roughly 60% second-node failure target and paired strategy effects. Play representative cases to judge whether Fox pressure and each response are visible and fair.

Under [SG-001](../Studio%20Guidelines/AI_GENERATED_REPORTS.md), Josh as design owner must approve and record the exact experiment scope and report contract before any new simulation runs. This note is preparation, not that approval or a run result.
