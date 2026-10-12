# Forest Edge fresh-seed confirmation analysis v1

Status: Needs Review. AI recommendations are not accepted production decisions.
Analysis timestamp: 2026-10-12T20:09:20.004082+00:00. Analyst: Codex; model identity unavailable in runtime metadata. Template: confirmation-review-v1.
Experiment group: Forest Edge confirmation presets 01-05. Parent: Forest-Edge-review-20261010-212931-ab2004. Source code hash: 62d30f34dc7c3b531a710f26234b8091c4d0bfbc858b8dcb7adfb3eead7ea08d. Checkout baseline: BevBranch / 612e65d5 plus existing local tooling changes.
Scope: read-only analysis of five supplied workbooks and five completed frozen batches. No simulations, app updates, production edits, commits or pushes.

## Executive summary

The fresh seeds confirm that useful choices and no-purchase successes exist, but none of the 80 tested conditions provides reliable tick-600 ecology. The best observed condition is 64x48, Hare gestation 4, Fox gestation 16, Gardeners with Restore toward start: 10/100 goal successes, with a 95% Wilson interval of 5.5%-17.4%. The discovery leaders returned 8/100 each, replacing their earlier 2/3 estimates with much lower confirmation rates.

## Evidence and verification

The workbooks are cumulative, not five disjoint exports. Their Batch summary totals are 1,600, 3,200, 4,800, 6,400 and 8,000. Every earlier summary row equals its corresponding latest row. The latest workbook is authoritative for the combined panel. Do not add all five totals; that would count repeated runs.

| Export | Matching runs | Conditions | SHA256 |
| --- | ---: | ---: | --- |
| Simulation review-20261012-151534-194.xlsx | 1,600 | 16 | 01b48d8b875faec41aea780f5e85e152aa1370fa126dc1f5e5296f490906b29e |
| Simulation review-20261012-152404-143.xlsx | 3,200 | 32 | 3f698df1cedac7a717e94c9b9e75b0298e1f6ae972e0eabb6b485a2395835f05 |
| Simulation review-20261012-154558-677.xlsx | 4,800 | 48 | ea013317247fbb719ac57fd8537284a9711dbdd331ac525a28fabe1e1c294e2f |
| Simulation review-20261012-155421-054.xlsx | 6,400 | 64 | d2b4faf3e7a1641de4f90fbdc8b8c311843daa524f77697f18f21a5706a25960 |
| Simulation review-20261012-155920-014.xlsx | 8,000 | 80 | ae24e95778d6c94c1c7f6562d765dbcddf6dd861282a1fbdd330e7d5b10b0a2c |

All five completed sweep inputs, resolved plans and complete raw-report hashes passed the existing completed-sweep verifier. All 40 Workbench frozen files passed their saved manifest hashes. Runner and snapshot hashes match the discovery inputs. All 8,000 condition/seed pairs are unique; each of the 80 conditions contains exactly seeds 10,001-10,100. A total of 7,720 workbook metrics were cross-checked against raw outcomes, including all 1,000 exported example rows across the five files, and all 120 latest matched-comparison rows. Original workbook hashes were unchanged.

Latest workbook: `F:/ForkBin/GameDev/LearningIndieDev/artifacts/worksheets/Simulation review-20261012-155920-014.xlsx`. Batch summary header is row 44, data rows 45-124. Choice impact header is row 6. Run data header is row 1. Workbook example rows are outcome-ranked selections, not the population used to calculate rates.

## Protocol and independent information

Five exact worlds x four automatic Mutation strategies x four purchase policies x 100 matched seeds = 8,000 runs. Initial totals: 55 Hares / 35 Foxes / 400 Plants; closed edges, 100 ticks per round, six rounds, horizon 600. The research goal is at least 5 Hares, 5 Foxes and 100 Plants at tick 600. These review floors are not approved production balance ranges.

There are 100 seed IDs per condition. The same IDs are reused across all 80 conditions, so this is not 8,000 independent seed draws. Each condition uses Wilson intervals as exported. Cross-condition comparison uncertainty below uses 20,000 bootstrap resamples of whole seed blocks, RNG seed 20261012, retaining all settings/policies/strategies for a resampled seed. These descriptive intervals refer to this fixed selected panel and are not corrected for examining multiple contrasts. No global superiority claim follows.

## Outcomes

Across the panel: 261/8,000 (3.26%) met the goal; 2,167 (27.09%) had Hares alive at 600; 1,438 (17.98%) retained all three species/resources above zero. Median endpoint: tick 280.5. Only 2,304 (28.8%) reached all five recorded decision windows. These pooled counts describe the panel; no binomial interval is assigned to 8,000 correlated arms.

| Preset | Map | Hare / Fox gestation | Goal met / 1,600 | Hares alive at 600 | All three alive at 600 | Median endpoint |
| --- | --- | --- | ---: | ---: | ---: | ---: |
| 01 | 48x48 | 4 / 24 | 38 (2.38%) | 193 (12.06%) | 117 (7.31%) | 244 |
| 02 | 48x48 | 8 / 32 | 28 (1.75%) | 145 (9.06%) | 94 (5.88%) | 229 |
| 03 | 64x48 | 24 / 8 | 37 (2.31%) | 467 (29.19%) | 242 (15.12%) | 269 |
| 04 | 64x48 | 4 / 16 | 91 (5.69%) | 757 (47.31%) | 564 (35.25%) | 569 |
| 05 | 64x48 | 8 / 24 | 67 (4.19%) | 605 (37.81%) | 421 (26.31%) | 365.5 |

Interpretation (high confidence within this panel): Preset 04 is the strongest next research candidate. Preset 03, originally labeled Unassisted candidate, is not the best no-intervention world on fresh seeds. No-purchase Skip all goal counts for presets 01-05 are 1, 1, 3, 7 and 3 out of 100. Names are historical selection labels, not confirmed rankings.

## Strategy and purchase effects

Each Mutation row below compares against Skip all with the same world, purchase policy and seed. Each purchase row compares against No purchases with the same world, strategy and seed. Each contrast pools 2,000 paired arms, built from only 100 seed blocks.

| Comparison | Goal wins / losses | Goal rate change (percentage points) | Seed-block 95% interval | Hare survival rate change | Survival interval |
| --- | ---: | ---: | --- | ---: | --- |
| Mutations/gardeners | 70 / 44 | +1.30 | +0.15 to +2.50 | +1.75 | -0.55 to +4.05 |
| Mutations/trailblazer | 36 / 41 | -0.25 | -1.05 to +0.55 | +6.95 | +4.15 to +9.85 |
| Mutations/warren | 53 / 45 | +0.40 | -0.80 to +1.70 | +3.45 | +1.20 to +5.85 |
| Purchases/each-one | 55 / 58 | -0.15 | -1.30 to +0.95 | +7.45 | +4.50 to +10.35 |
| Purchases/each-three | 63 / 57 | +0.30 | -0.85 to +1.45 | +4.75 | +1.75 to +7.80 |
| Purchases/restore-toward-start | 71 / 53 | +0.90 | -0.35 to +2.15 | +6.15 | +3.10 to +9.15 |

Interpretation: Trailblazer has the clearest focal-species survival improvement (+6.95 points), while its ecology-goal rate is slightly lower than Skip all. Gardeners has the largest observed goal improvement (+1.30 points, 70 paired goal wins and 44 losses); the small positive interval is suggestive within this selected panel, not proof of a universal winner. Warren improves Hare survival with a small uncertain goal benefit. Purchases improve Hare survival, but all three pooled goal-effect intervals include zero. More purchased Hares is not automatically better for whole-ecology outcomes.

The best observed condition is Gardeners + Restore in preset 04: 10 goals, 52 Hare survivors and 46 all-three survivors per 100. Gardeners + Each three in preset 04 and Gardeners + Restore in preset 05 each return 8 goals per 100 (95% Wilson 4.1%-15.0%). The latter two were the old 2/3 discovery leaders. Their old success rate did not generalize.

## No-purchase and unassisted evidence

There are 60 goals among 2,000 No purchases runs, spanning all four strategies. Exactly 60 goal runs actually added zero purchased Hares; none of the purchase-enabled goal successes happened to buy zero. Fifteen successes among 500 Skip all + No purchases runs are fully unassisted under this tooling definition. This supports existence of autonomous tick-600 outcomes, not reliable or indefinitely stable autonomy.

Recommended examples for visual inspection are selected for an interpretable comparison or endpoint margin, not to estimate rates. All use No purchases:

| Seed | Map | Hare / Fox gestation | Strategy | Endpoint tick | Final Hares / Foxes / Plants | Why inspect |
| ---: | --- | --- | --- | ---: | --- | --- |
| 10016 | 48x48 | 4 / 24 | gardeners | 600 | 105 / 13 / 165 | Same-seed Skip all loses Hares at tick 235 |
| 10029 | 64x48 | 8 / 24 | gardeners | 600 | 79 / 12 / 164 | Same-seed Skip all loses Hares at tick 384 |
| 10030 | 64x48 | 4 / 16 | Skip all | 600 | 57 / 12 / 164 | Fully unassisted reference |

Seed 10016 (preset 01) is the clearest isolated strategy example: Skip all loses Hares at tick 235, Warren at 237 and Trailblazer at 242. Gardeners reaches 600 with 105 / 13 / 165, zero purchases and five decisions: Seed dispersal, Efficient digestion, Efficient digestion, Seed dispersal, Efficient digestion. Run data row 5 in the latest workbook contains this goal run; endpoint populations are V5:X5. The matched Skip all control comes from the verified raw report and need not appear in the 200-row shortlist. Seed 10029 (preset 05) gives a second Gardeners rescue: 79 / 12 / 164 versus Skip all extinction at tick 384. Seed 10030 (preset 04), Skip all, ends at 57 / 12 / 164; Run data row 28, V28:X28.

## Collapse and late-horizon risks

Reported Hare deaths: 76.1% predation, 22.0% starvation, 1.9% wilt. Reported Fox deaths: 90.1% starvation. These are recorded activity/stat-line totals; they do not by themselves prove a unique causal mechanism or isolate gestation. Sixteen runs have no Hare births, so most failures involve reproduction followed by loss, rather than reproduction being wholly absent.

Of the 2,167 Hare survivors, 729 lack Foxes or Plants at 600. Among the 1,438 runs retaining all three above zero, 807 have fewer than 100 Plants, 497 fewer than 5 Foxes and 138 fewer than 5 Hares; categories overlap. Hare survival is therefore an incomplete ecological score.

Of the 261 goal successes, 210 (80.5%) have fewer Plants at 600 than at 500; 183 simultaneously have more Hares and fewer Plants. This is an observed last-window trend, not a proof of later collapse. Example: seed 10016 Gardeners has Plants 304 -> 232 -> 165 and Hares 35 -> 74 -> 105 at ticks 400/500/600. Meeting the endpoint floor does not establish recovery cycles or sustained equilibrium.

Map and gestation are not isolated across these five selected worlds. Two or more settings change between most comparisons; map dimensions also change density. Do not infer an optimal Hare gestation or a causal map-size effect from the rank order alone.

## Recommended actions for Bevin to review

1. **Priority 1: manually inspect seeds 10016, 10029 and 10030.** Check whether the successful choices, starvation/predation pressure and resource decline are readable and interesting to a player. Expected value: concrete proof that choices can matter and a baseline for autonomy. Tradeoff: manually picked examples are not statistical evidence of broad reliability.
2. **Priority 1: use preset 04 as the next controlled research world.** Retain Skip all / No purchases and compare the useful strategy/purchase arms. Isolate one variable at a time around H4/F16 rather than launching another wide product. Choose baseline-pressure variables after telemetry/manual review; do not automatically shorten gestation or tune production. Expected value: interpretable causes of fragility. Owner: Bevin/design review.
3. **Priority 2: test persistence after tick 600 without moving the five choice moments.** The current ticks-per-round control shifts both decision timing and horizon. A valid continuation protocol needs the existing 100/200/300/400/500 decisions with later observation and no new purchases/Mutations, or another explicitly agreed treatment. Include failures and marginal cases, not only handpicked winners. Expected value: distinguish temporary coexistence from recovery/persistence. No runner change or new run is authorized by this recommendation.
4. **Priority 2: improve workbook wording in a separate tooling task.** The generic notes about a two-seed screen and confirmation needing new seeds are not tailored to this 100-fresh-seed stage. Cumulative report selection should be visibly obvious. These are usability recommendations, not edits performed here.

## Limits and decision boundary

High confidence: completed counts, exported arithmetic, exact paired endpoints, matching frozen inputs. Moderate confidence: strategy effects within the selected five-world seed panel. Open: production balance, long-term ecology, emergent stability, human fun/readability and interactive Unity parity for these exact examples. All runs here are automatic policies. No future continuation, manual acceptance, or production change is implied.

Supporting artifacts: `verification.json` (source identities/hashes and metric checks), `summary.json` (grouped metrics, paired effects and seed-block bootstrap), `condition-results.csv` (all 80 arms), `choice-effects.csv` (all 120 contrasts), `confirmation-overview.png` (panel comparison and seed 10016 checkpoints). Original experiment/report/workbook files remain unchanged.

## Frozen source batches

- `F:\ForkBin\GameDev\LearningIndieDev\artifacts\workbench-runs\Confirmation-04---64x48-H4-F16---Purchase-leader-20261012-154706-6a96f3`. Batch identity `203b097f732ea641b02d713d394e004ee4c237a6947a93fd23a060e6a4f842eb`; raw SHA256 `c2de9c0cfa77d6e0bebef783debb184be11fdd7ac350b99bf5be59e6e0938978`.
- `F:\ForkBin\GameDev\LearningIndieDev\artifacts\workbench-runs\Confirmation-05---64x48-H8-F24---Restoration-lea-20261012-155508-05da69`. Batch identity `6d684e05b6028150f8844bf55a189a606e17d3d2281c23f9e06084a4c806dac9`; raw SHA256 `a72354148453ec0e3a2d2a8bd45a6d046065b009c89e299f61091eca6f9c1907`.
- `F:\ForkBin\GameDev\LearningIndieDev\artifacts\workbench-runs\Confirmation-01---48x48-H4-F24---Mutation-rescue-20261012-151156-5c6070`. Batch identity `3413654b0fd76f0fd1e6d09d7540e644530f6c986ec3f357c8510b19dd15c3a0`; raw SHA256 `dcffa8e1568a03e2eb506288872490e8f399cbb38cd52ce9de4d75a532e2418c`.
- `F:\ForkBin\GameDev\LearningIndieDev\artifacts\workbench-runs\Confirmation-03---64x48-H24-F8---Unassisted-cand-20261012-154141-517ae0`. Batch identity `76e01107ee48da244b27647b0ecc068acb2746e584a2a7fe8cce3824b9c62842`; raw SHA256 `6fe5e188fc9e642cef2dba0fad7bb4373c42fd1286fa17a01f60a895d5a35c17`.
- `F:\ForkBin\GameDev\LearningIndieDev\artifacts\workbench-runs\Confirmation-02---48x48-H8-F32---Build-compariso-20261012-151744-433409`. Batch identity `76e92e6ddd2b74d5e1dfab6313ba821817524467286777637038822ccd9d1802`; raw SHA256 `ce5e6934dfaa19ab75a5f9ba1d0e5bcb92260bd0d1a15f1bd585c3c9ca51bb29`.
