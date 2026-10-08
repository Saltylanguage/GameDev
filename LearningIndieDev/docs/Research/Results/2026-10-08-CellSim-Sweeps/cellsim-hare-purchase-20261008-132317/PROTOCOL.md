# Hare purchases on the retained four-candidate panel

Date: 2026-10-08. Decision owner: Bevin / SimMasterBev.

Status: **candidate panel and 12,800-run purchase experiment approved by Bevin
on October 8: "yes run it"; validated and launched with 16 workers.**
Run: `artifacts/cellsim-hare-purchase-20261008-132317/`.
Production tuning remains unapproved; research outcomes are pending completion.

## Accepted candidate set

Bevin explicitly retained these four configurations after the quarter-million
screen and the matched all-path survival / APS / AHS review. Keep them together
as a development panel; none is an approved production default.

| Candidate | Plants / Hares / Foxes | Grid | Fox starting energy | Hare vision |
| --- | --- | --- | --- | --- |
| D4/S19 | 325 / 30 / 15 | 48 x 28 | 120 | 8 |
| D4/S14 | 325 / 30 / 15 | 48 x 28 | 80 | 8 |
| C2/S14 | 250 / 20 / 10 | 36 x 20 | 80 | 8 |
| D5/S25 | 325 / 30 / 15 | 54 x 32 | 160 | 9 |

Frozen source evidence: `artifacts/cellsim-250k-sweep-20261008-030316/`;
`all-path-benefit/results.json`, `results-with-statlines.json`,
`report-with-statlines.md` and `sweep/sweep.json`.
Original source hash:
`89c3792a277ff216d0e109ce3b784e3bc4c69de954c77b11b04b336a40bb55a9`.
Original batch identity:
`ea092fa4ac396e4bf93b567e8aa7b1de886f5ef2303c3cb7d70b54aa509af12c`.

## Question and hypothesis

Bevin's requested question: how does the upgrade-window choice to buy additional
Hares affect the player experience and outcomes in these candidate scenarios?

Approved exploratory hypothesis: affordable, well-timed reinforcement can improve Hare
survival and recovery across multiple Mutation paths, but excessive buying may
increase food demand or predator pressure and trade away saved Field Data.
Direction and size are unknown; purchases are not presumed beneficial.

The screen will identify positive, neutral and harmful combinations. It will
not require every path, every metric or every purchase amount to improve.

## Current player contract, verified from code

- `SpeciesSimulationPreview.HareCost` is **10 Field Data for one Hare**.
  `BuyHare()` can be repeated while the run is awaiting an uncommitted phase
  decision, the player is Hare, currency is sufficient and placement succeeds.
- At a reached phase boundary, `HandlePhaseBoundaryReached` credits
  `SimulationRunResults.Create(run).CurrencyEarned` once. That current reward
  equals the living player population. Unspent currency persists within the
  expedition. This first panel starts with zero Field Data; carried journey
  currency is a separate future axis.
- A buy calls the existing `SpeciesSimulationRunner.TryAddBoundaryPopulation`
  before spending. Both the current board and retained restart board must have
  enough passable empty cells and obey the configured total population limit.
  Placement is deterministic and uses the current species rules.
- Purchases are separate from the Mutation choice. Preserve the same reachable,
  offer-aware Skip all / Trailblazer / Warren / Gardeners policies. Do not
  replace a Mutation with a purchase or grant a free extra Mutation.
- There are five eligible windows at ticks **100, 200, 300, 400 and 500** in
  the six-phase, 600-tick panel. No purchase at tick zero or after completion.
  An expedition that ends before a window receives no artificial rescue.
- Successful additions are `ADD`, not births. The existing reconciliation
  and RFS/APS/AHS accounting exclude additions from biological growth.
- The older `SpeciesUpgradeCatalog` reinforcement entry costs **5**, unlike
  the current player Buy Hare button. Do not use that legacy price to simulate
  this choice. The experiment must explicitly freeze the player-facing 10.

Authoritative implementation locations:
`Assets/Scripts/Game/Presentation/SpeciesSimulationPreview.cs`;
`Assets/Scripts/Game/Simulation/SpeciesSimulationRunner.cs`;
`Assets/Scripts/Game/Simulation/SpeciesSimulation.cs`;
`Assets/Scripts/Game/Simulation/SimulationRunResult.cs`;
`Assets/Scripts/Game/Simulation/SpeciesSimulationMetrics.cs`.

## Approved purchase matrix

All counts are **attempt caps**, not guaranteed free additions. Each Hare is
bought individually for 10 Field Data. A policy stops buying in that window
when it reaches its cap or cannot afford/place another Hare; unused currency
is retained. Failure and non-reach are recorded separately.

| Policy | Tick 100 | Tick 200 | Tick 300 | Tick 400 | Tick 500 | Purpose |
| --- | --- | --- | --- | --- | --- | --- |
| No purchases | 0 | 0 | 0 | 0 | 0 | Same-path control |
| Early five | up to 5 | 0 | 0 | 0 | 0 | Early intervention |
| Middle five | 0 | 0 | up to 5 | 0 | 0 | Same cap, later intervention |
| Late five | 0 | 0 | 0 | 0 | up to 5 | Late intervention |
| One each window | up to 1 | up to 1 | up to 1 | up to 1 | up to 1 | Small sustained spend |
| Three each window | up to 3 | up to 3 | up to 3 | up to 3 | up to 3 | Moderate sustained spend |
| Five each window | up to 5 | up to 5 | up to 5 | up to 5 | up to 5 | Higher sustained spend |
| Restore toward start | conditional | conditional | conditional | conditional | conditional | Buy up to min(5, starting Hare count - current Hare count), only when below the starting count |

Proposed main screen: **4 candidates x 4 paths x 8 purchase policies x 100
fresh matched seeds = 12,800 runs**, with **16 workers**, seeds 50000-50099,
six 100-tick phases, Hare player, wrapped maps and opposed-roll combat.

Maximum theoretical spend is 50 Field Data for a single five-Hare intervention,
50 for one each window, 150 for three each window, and 250 for five each window
or the conditional policy. Actual spend is constrained by reached windows,
earned currency and placement. These are diagnostic policy caps, not proposed
new store quantities or player price changes.

Hold the selected starting contexts and original path policy contract fixed.
Create a new frozen snapshot/build/source identity for purchase-capable tooling;
never relabel the old snapshot's source hash to evade compatibility checks.
Re-run all no-purchase controls on the same fresh seeds as the purchase arms.

## Required comparisons and report contract

For every candidate and each of the four paths, produce one row per purchase
policy with:

- tick-600 Hare survival and all-species survival, with matched percentage-point
  deltas versus **the same path with no purchases**;
- average Hare APS and Fox AHS, valid sample sizes, and matched deltas;
- actual observation windows and phase-specific slash lines, retaining early
  extinction runs and unavailable statuses;
- windows reached, Field Data earned, requested purchases, successful `ADD`,
  Field Data spent, ending balance, and failed-buy reasons;
- Hare population before/after a buy, minimum/mean/final Hare and Fox
  populations, births, predation/starvation/crowding counts and applicable rates;
- Hare recovery through the following phase and whether improvement persists
  to tick 600, rather than only the immediate purchased population increase.

Also compare each Mutation path against **Skip all with the same purchase
policy**. These answer different questions: purchase value within a build and
Mutation value when both arms can buy. Report the exploratory interaction
`(path + purchase - path + no purchase) - (Skip all + purchase - Skip all + no purchase)`
separately, with seed pairing and uncertainty; do not conflate it with a main effect.

Survival and slash-line summaries retain all assigned seeds. Cumulative APS/AHS
are per-run means over actual observed windows, not pooled ratios or a combined
cross-species score. Purchases must reconcile as additions, not inflated births
or artificial RFS. Report multiple-comparison/selection limits and select
follow-ups for fresh-seed confirmation.

## Player-experience review

Simulation evidence can show whether affordable choices alter outcomes, give
different paths useful responses, avert a reached-boundary decline, or create
an escalating food/predator burden. It cannot establish enjoyment by itself.

Use matched replays of a helpful purchase, a neutral purchase and a harmful
purchase for human review. Examine readable cause/effect, meaningful decisions,
event pace, recovery duration and whether purchases become obligatory. Record
human acceptance separately from statistical results.

The meeting's tick-700 ecology direction remains a follow-up. No new definition
of viability or automatic tick-700 success gate is introduced here.

## Implemented tooling and launch validation

The shared experiment harness now implements wallet-aware purchase policies in
both Mutation and scheduled Skip-all flows. It uses production reward, spend and
boundary-placement methods, with a per-window ledger and population reconciliation.
The player and harness share `SpeciesProgression.HarePurchaseCost=10`; the public
player `HareCost` keeps the same value. Purchases remain separate from Mutation choices.

Launch validation passed: .NET Release build and purchase self-checks; eight
Unity-to-standalone reference runs; 16 historical no-purchase control comparisons;
all 128 arms on a separate smoke seed, including identical pre-purchase phases,
unchanged reachable Mutation choices and conditional restore caps; both overall
and phase population reconciliation; full analysis/report smoke with 640 window
records; two focused Unity EditMode telemetry tests. Evidence lives in the run's
`validation/` and `smoke/` directories. The normal test wrapper was missing
`Resolve-UnityExecutionLane`; the same Unity CLI was used directly for the focused test.
Currency totals refer to credits and balances at the five eligible upgrade windows;
the completed-run reward is retained separately as `run.currencyEarned`.

Required focused checks:

1. No-purchase controls reproduce prior same-input/seed behavior and phase-one
   trajectories match all purchase arms before the first possible purchase.
2. Earned Field Data is credited once per reached window; successful placement
   charges 10; insufficient funds/space/population capacity charges nothing.
3. Zero or unreachable windows cannot add Hares; all policies respect their
   caps; conditional buying stops at its target; terminal runs do not resurrect.
4. Mutation offers/acquisition timing and ordered upgrades remain unchanged;
   buying does not consume a Mutation choice or use its legacy five-Data cost.
5. Overall and phase FPO/ADD reconciliation and APS/AHS applicability pass;
   purchases remain deterministic under replay and resume.
6. Current-source .NET build, focused Unity checks where shared harness changes
   apply, exact 128-arm input coverage and bounded execution smoke checks pass
   before the 12,800-run screen is started.

## Decision boundary and provenance

SG-001 requires the named human owner to approve scope, hypothesis, success
criteria and expected report contract before research runs. Bevin approved this
eight-policy matrix, first-expedition zero-balance scope, report contract and
12,800-run count with "yes run it" on October 8. Launch followed validation.

Evidence classification: a purchase policy is a follow-up candidate when
its matched survival/recovery results improve and the APS/AHS, resource and
currency tradeoffs are understandable. No minimum effect size or mandatory
all-metric improvement is being invented as a production gate. Harmful or
neutral results are valid findings, not execution failures. Technical failures
include corrupted provenance, broken pairing/accounting, invalid metrics or
unrecorded unaffordable additions.

Analysis generated by Codex / GPT-6 on 2026-10-08 from current code and retained
run artifacts. Candidate selection and execution scope are the human decisions;
the purchase matrix is an approved research screen. No purchase price change or
production balance approval is implied. Execution status is authoritative in
the run's `pipeline-state.json` and `sweep/batch/status.json`.
