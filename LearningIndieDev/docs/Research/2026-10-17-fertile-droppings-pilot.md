# Fertile Droppings matched-seed pilot — October 17, 2026

[Working state](../WORKING_STATE.md) | Status: preliminary experiment

## Setup

Two clean CellSim reports used Forest Edge with the same production scenario asset, ruleset fingerprint `c9600d6b`, seeds 10100–10119, 36×20 bounded grid, 400 Plants / 20 Hares / 10 Foxes at launch, and 300 ticks. No temporary Mutation or purchase policy was selected. The treatment launched all Hares with the synthetic active `fertile-droppings` Genome node; the control had no active Genome node. Both reports have identical tick-zero species populations for every seed and report schema 37. This is a mechanics pilot, not a full journey or S5 exit test.

- [Control report](../../artifacts/cellular-experiment-20261017-165500/report.json) and [manifest](../../artifacts/cellular-experiment-20261017-165500/manifest.json)
- [Fertile Droppings report](../../artifacts/cellular-experiment-20261017-165613/report.json) and [manifest](../../artifacts/cellular-experiment-20261017-165613/manifest.json)

## Observations

Values are averages across 20 seeds; differences are treatment minus the matching control seed. The intervals are approximate 95% intervals for the mean paired difference using the 20 observed differences, not acceptance bounds.

| Tick-300 population | Control mean | Fertile mean | Mean paired difference | Approximate interval |
| --- | ---: | ---: | ---: | ---: |
| Hares | 17.50 | 28.65 | +11.15 | +4.03 to +18.27 |
| Plants | 656.25 | 685.90 | +29.65 | +15.69 to +43.61 |
| Foxes | 7.40 | 5.95 | −1.45 | −2.86 to −0.04 |

Hares ended higher on 13 seeds, lower on 5, and equal on 2. Hare extinction at tick 300 occurred on 2 control seeds and 1 treatment seed; no Fox extinction occurred. Mean paired population differences first became visible after tick 50: at tick 100, Hares +0.60 / Plants +25.70 / Foxes −0.05; at tick 200, Hares +4.65 / Plants +36.55 / Foxes −0.95.

The treatment averaged 90.10 successful Hare seed drops versus zero in control. Foxes averaged 63.85 dropping acquisitions, 25.95 dropping sniffs, and 95.85 successful named-scent samples. Fox scent pursuit decisions averaged 2,023.85 in treatment versus 1,589.85 in control; this counter records decisions across movement passes, not actual movement steps or kills. Fox combat kills averaged 22.90 in treatment versus 26.55 in control. Foxes are using the new scent path, but increased pursuit decisions did not increase kills in this setup.

## Interpretation and limits

The observed renewal effect is large enough to measure, and the scent cost is active. In this pilot, the net outcome favors Hares and Plants while average Fox population and kills fall. A plausible explanation is that renewal and altered movement outweigh the extra pursuit opportunity; these totals alone cannot attribute any particular catch or survival to scent. The control also includes the newly implemented ordinary scent system, so this comparison isolates the active node on that baseline.

The 20-seed, 300-tick panel does not cover the full journey, post-clear persistence, player choices, or alternate starting densities. The report has no authored Genome price/capacity or player selection. Treat the approximate intervals as screening evidence only. Do not price the node or claim ecosystem balance from this pilot.

## Next comparison

Keep this seed block as exploration evidence. Once the playtest offer and scenario gate are decided, freeze node values and run a larger matched block plus a disjoint held-out block. Include Hare births/deaths, Plant availability, Fox food and kills, scent events, clear outcome, and post-clear persistence. If the Fox cost remains weak, first inspect whether a predator actually reaches its scent target before changing the agreed 20/16/40/24/36 ranges.
