# Factual report: Trailblazer early-choice timing

## At a glance

- **Tested:** Trailblazer's five choices at ticks 100/200/300/400/500 versus the same five choices at 0/100/200/300/400, then automatic continuation at tick 500.
- **Result:** The early-choice arm cleared the strict all-species finish in 28/200 runs (14%); the current schedule cleared in 30/200 (15%). The paired estimate was -1 percentage point, with a 95% interval from -7.3 to +5.3 points.
- **Supports:** This timing shift did not measurably improve clear rate in this screen. The 40–50% exploratory target remains unmet.
- **Does not establish:** A small timing benefit or harm is still compatible with the uncertainty. This is one scenario, one player species, one strategy policy, and one matched 200-seed panel.

## Frozen setup

Both arms used Forest Edge, 36x20 with wrapping, 600 Plants / 40 Hares / 25 Foxes, Hare player, seeds 14200–14399, six 100-tick phases, 600-tick maximum, 0.1-second steps, `bev-experimental`, opposed-roll natural combat, coupled responses off, no reinforcements, and no genome additions. The reference is the Trailblazer arm in the [strategy policy screen](../S4-03-Strategy-Choice-Policy-Screen-600-40-25/PROTOCOL.md). The frozen contract and source/scenario hashes are in the ignored raw-artifact folder.

Both arms used the same production offer generation and the same policy: choose the lower-level Trailblazer skill when offered, use the first path skill on ties, and skip if neither appears. The audited offers and selections agreed for every corresponding decision both arms reached. Choices that occur after a run ends are naturally absent.

## All-species clear results

Clear means the run reaches tick 600 with Plants > 0, Hares >= 5, and Foxes >= 5; any run that ends earlier fails.

| Arm | Clears | Rate | Wilson 95% interval |
|---|---:|---:|---:|
| Current: first choice at tick 100 | 30/200 | 15.0% | 10.7–20.6% |
| Early: first choice at tick 0 | 28/200 | 14.0% | 9.9–19.5% |

Paired outcomes were candidate-only 20, both 8, current-only 22, neither 150. The paired risk difference was -1.0 percentage point (95% CI -7.35 to +5.35; exact McNemar p=.878). The test does not show a clear-rate difference.

The simulation reached tick 600 in 121 current-schedule runs and 102 early-choice runs. Among those runs only, median final Plants/Hares/Foxes were 6/10/3 and 26/14.5/6, respectively. These medians are conditioned on reaching tick 600 and do not override the lower count of completed runs in the early-choice arm.

## First-phase descriptive measures

| Measure (median per run) | Current schedule | Early choice |
|---|---:|---:|
| Hare population at phase end | 21 | 26 |
| Hare deaths | 45 | 45 |
| Hare combat deaths | 39 | 37.5 |
| Hare starvation deaths | 6 | 7 |
| Fox population at phase end | 31 | 29 |
| Fox deaths / starvation deaths | 15 / 7 | 16 / 8 |
| Fox births | 13 | 12 |
| Fox food consumed | 33 | 31 |
| Fox reproduction candidates / blocked by energy | 2,039.5 / 1,827.5 | 2,061.5 / 1,886.5 |

The phase-one Hare slashline was valid for all 200 runs in both arms. Median pAVI/eAVI/predAVG/sAVI/cAVI/bAVG/RFS/APS were 0.421/0.646/0.527/0.776/1.000/0.0090/-0.130/0.167 on the current schedule and 0.429/0.659/0.537/0.795/1.000/0.0095/-0.108/0.211 with the early choice. These readable player-facing rates move modestly in a positive direction, while the population/death measures show only a small combat-death change and no reduction in total Hare deaths.

## Validation and limits

The one-seed pilot confirmed choice ticks 0/100/200/300/400, acquisition ticks, five total choices, and automatic continuation after tick 500. The 200-run analyzer verified seed coverage, scenario and source hashes, complete population histories, phase accounting, slashline validity and formulas, FPO reconciliation, seed-drop reconciliation, legal offers/selections, and choice/acquisition timing. It wrote `validated-summary.json`.

Raw JSON, CSV, Unity logs, the frozen contract, and the analyzer are under the ignored folder `artifacts/s4-03-trailblazer-early-choice-timing-20261006-600-40-25-wrapped-200/`. The baseline JSON is in `artifacts/s4-03-strategy-choice-policy-screen-20261006-600-40-25-wrapped-200/`. Unity exited successfully; normal thread-cleanup and temp-memory messages appeared at shutdown, with no simulation exception or compile error. No full Unity test suite, build, or visual test was run.
