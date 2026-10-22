# Earlier feeding and gradual Fox introduction — observed results

**Experiment:** `EXP-FEEDING-FOX-20261021-01`  
**Report:** `RPT-FEEDING-FOX-20261021-01`  
**Feature:** Natural Hare feeding and opening predator pressure  
**Owner:** Josh  
**Generated:** October 21, 2026, America/Toronto  
**Status:** Completed; production decision pending

This records factual output from the [approved scope](2026-10-21-Feeding-and-Fox-introduction-comparison.md).
Interpretation and suggested next work are in the separate
[AI analysis](2026-10-21-Feeding-and-Fox-introduction-analysis-v1.md).

## Evidence and reproduction

The retained [bundle index](../../artifacts/feeding-fox-comparison-20261021-02/run-index.json)
contains absolute raw report/manifest paths and SHA-256 hashes. The
[comparison JSON](../../artifacts/feeding-fox-comparison-20261021-02/comparison.json)
retains fixed inputs, per-arm research inputs and fingerprints, all arrival and
milestone audits, per-seed measurements, and matched differences. The
[per-seed CSV](../../artifacts/feeding-fox-comparison-20261021-02/per-seed.csv)
and [population trajectories](../../artifacts/feeding-fox-comparison-20261021-02/trajectories.json)
support further inspection. Raw reports remain unchanged.

| Arm | Raw report | SHA-256 |
| --- | --- | --- |
| Baseline | `artifacts/cellular-experiment-20261021-113406/report.json` | `E2F896FB7042DA09E36849ED1D2B90E521E7B1562168271BB41F8A09E9AB0BA7` |
| Earlier feeding | `artifacts/cellular-experiment-20261021-113537/report.json` | `DE92690F70C5767D28FF9C6A5D9C271A1F01A83A39335E42FE4172489BC19564` |
| Fox ramp | `artifacts/cellular-experiment-20261021-113706/report.json` | `C75E6E226264E2214436C85CAA80C4EF75B12F1479B5C90AA740BE98085606CC` |
| Combined | `artifacts/cellular-experiment-20261021-113831/report.json` | `4DD72A192AD5EA512BE48D3F64BA81114FB5B9E81C2997FF3D133D7158059E39` |

Run command from `LearningIndieDev`:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File artifacts/feeding-fox-comparison-20261021-02/run-comparison.ps1 -IncludeCombined
```

The script protects completed evidence against overwrite. For reproduction,
copy it into a new artifact directory and use the retained frozen source;
do not rerun into this completed bundle. Each raw directory also retains its
exact command request, Unity arguments, metric dictionary and logs.

## Controlled inputs and termination

- Forest Edge, bounded 36×20, 400 Plants / 20 Hares.
- Seeds 10120–10131, opposed-roll combat, natural attack opportunities.
- 1200 ticks at nominal 0.1 seconds per tick, sixteen continuous 75-tick
  windows with empty loadouts, no Genome or Mutations, no Hare purchases.
- Baseline: feeding threshold 6, ten initial Foxes. Feeding arm changes only
  the threshold to 25. The existing strict-below trigger and refill-to-full
  behavior remain active.
- Ramp: two initial Foxes, one introduced after each of ticks
  75/150/225/300/375/450/525/600. Combined also uses threshold 25.
- Ten Foxes are introduced in total per run, including the initial two, with
  births and deaths changing the live count. Arrival placement is deterministic
  on open passable tiles; it is not map-edge migration. Each arrival acts from
  the following tick. Tick 600 includes the final new Fox.
- Every run observes through tick 1200 after Hare extinction. Gameplay and
  purchase-policy journey termination retain their existing behavior.
- Unity 6000.4.6f1, Clean execution, report schema 39, metric dictionary v1.
  Branch `codex/audio-direction-and-sourcing`, baseline commit `6ffb7b10`, dirty
  local checkout. The bundle retains all 389 frozen source/data/tool files
  and their hashes; a commit alone does not identify this working tree.

The first attempt (`cellular-experiment-20261021-112933`) inherited the journey
stop on Hare extinction and failed the declared full-window acceptance check.
It is retained with a rejection receipt in `feeding-fox-comparison-20261021-01`
and excluded from every comparison below. The correction implemented the
already-approved observation window; all four accepted arms use the same
corrected source.

## Validation

- **48/48 runs** reach tick 1200 with complete tick histories.
- **192/192 scheduled arrivals** succeed: eight in each of 24 ramp runs,
  at the exact requested ticks, each with population-after = population-before
  + 1 and cumulative additions 1–8. Controls have no arrivals.
- **144/144 species accounts** reconcile: initial + births + introductions −
  deaths = final. Tickwise reconstructed births are nonnegative and sum to
  reported births. First Hare birth ticks match the direct milestone observer.
- Fixed report inputs and normalized command inputs match apart from the
  declared feeding/count/schedule changes. Frozen source hashes still match
  after execution. Same-start-count pairs have identical tick-zero population
  records. Count changes also change the initializer's animal placements;
  identical spatial maps across density groups are not claimed.
- All four scenario/ruleset fingerprints differ as expected: this fingerprint
  includes initial populations as well as species rules. Exact fingerprints
  are retained in the comparison JSON, together with intervention provenance.
- The corrected baseline's population history matches the rejected attempt
  through each seed's original stopping tick.
- Focused Unity runner tests **40/40 passed** at
  `artifacts/unity-tests-20261021-113318/EditMode-results.xml`, including research
  observation after extinction and unchanged default gameplay stopping.
  The first test attempt caught misplaced wiring (39/40); the corrected suite
  executed all 40. Standalone build: zero warnings/errors. Portable regression
  self-test passed. No visual playtest was performed.

The existing artifact-summarization skill generated one compact JSON/Markdown
packet per arm, with same-seed baseline deltas for candidates. The additional
comparison script retains schema-39 introductions/milestones and validates
accounting. It applies **no legacy wilt correction**: the runtime mortality
attribution fix is already present in these new reports.

## Population observations

All values below are means over 12 seeds unless a count/range is stated.

| Arm | Hares at 10 s | Hares at 30 s | Hares at 60 s | Hares at 120 s | Foxes at 60 s | Foxes at 120 s |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Baseline | 6.50 | 0.67 | 0.08 | 0.08 | 0.00 | 0.00 |
| Earlier feeding | 20.75 | 30.08 | 0.00 | 0.00 | 7.00 | 0.00 |
| Fox ramp | 15.83 | 104.08 | 0.08 | 0.00 | 15.17 | 0.00 |
| Combined | 63.75 | 46.33 | 0.00 | 0.00 | 2.00 | 0.00 |

Only baseline seed 10127 retains a Hare at tick 1200, with exactly one.
Every other run has zero Hares. Every run has zero Foxes at tick 1200. No run
retains two Hares or joint Plant/Hare/Fox presence at the endpoint.

| Arm | Hare t100 median [range] | Mean within-run Hare peak | First Hare feed [range, ticks] | First Hare birth [range, ticks] | Hare extinction [range, ticks] |
| --- | --- | ---: | --- | --- | --- |
| Baseline | 7 [3–11] | 20.50 | 57–57 | 106–133 | 156–364, 11/12 seeds |
| Earlier feeding | 18.5 [9–33] | 49.00 | 1–1 | 49–54 | 324–566, 12/12 |
| Fox ramp | 16 [13–18] | 118.25 | 57–57 | 106–107 | 496–602, 12/12 |
| Combined | 62.5 [54–87] | 166.08 | 1–1 | 49–50 | 364–418, 12/12 |

All three candidate arms have more Hares than baseline at tick 100 in all
12 matched seeds. Feeding-only also exceeds baseline at tick 300 in all 12.
Combined exceeds the ramp by 38–73 Hares at tick 100 (median +45.5), then has
32–89 fewer by tick 300 (median −57), in all 12 pairs at both checkpoints.
At tick 1200, every candidate ties baseline on 11 seeds and has one fewer Hare
on seed 10127; the median paired difference is zero. These endpoints do not
demonstrate a viable breeding population in any arm.

## Opening births and losses (ticks 1–100)

| Arm | Hare births | Hare combat deaths | Hare starvation deaths |
| --- | ---: | ---: | ---: |
| Baseline | 0.00 | 12.25 | 1.25 |
| Earlier feeding | 17.50 | 15.00 | 1.75 |
| Fox ramp | 0.00 | 3.17 | 1.00 |
| Combined | 47.67 | 3.17 | 0.75 |

The ramp-only arm has 6–12 fewer opening combat deaths than baseline on every
seed (median nine fewer). Feeding-only's larger opening population includes
replacement births; its total opening deaths are not lower. Counts after births
include offspring and must not be described as survival rates of the original
20 Hares.

## Food and later mortality

| Arm | Plants at 10 s | Plants at 30 s | Mean within-run Plant minimum | Seeds reaching zero Plants | Total Hare combat deaths | Total Hare starvation deaths |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Baseline | 386.58 | 410.50 | 371.08 | 0/12 | 27.83 | 2.92 |
| Earlier feeding | 294.58 | 127.33 | 90.58 | 0/12 | 81.33 | 42.00 |
| Fox ramp | 334.00 | 41.17 | 7.17 | 6/12 | 46.25 | 161.75 |
| Combined | 98.92 | 0.00 | 0.00 | 12/12 | 25.75 | 209.67 |

Combined Plants first reach zero at ticks **128–282** (median 157); every
combined run has zero Plants at ticks 300, 600 and 1200. Hares subsequently
reach zero at ticks 364–418. Ramp-only Hares have a mean 141.25 starvation
deaths during ticks 301–600, versus 30.75 combat deaths in that window.
Combined has a mean 170.50 Hare starvation deaths during ticks 101–300.

All Fox deaths in this panel are reported as starvation. Combined Foxes reach
zero at tick 720 in every seed, 120 ticks after the final arrival. Ramp-only
Foxes reach zero at ticks 720–756. No ramp run has an earlier zero-Fox episode;
the analysis nevertheless treats zero Foxes during scheduled arrivals as a
potential gap rather than irreversible extinction.

## Limits and human decision

This is one scenario, 12 paired seeds and a two-minute horizon. It measures
ecological responses; it does not establish fun, production margins or an
accepted natural baseline. Same seed IDs do not hold animal placement fixed
when initial counts change. The existing shared-tile diet-target rejection
remains unchanged in all arms. Plant counts measure presence, not spatially
reachable food energy. Event totals depend on population size and exposure;
raw deaths or reproduction blocks are not per-animal rates.

**Decision:** Pending, Josh. The approved panel is complete. No production
threshold, scenario population, Fox arrival flow or upgrade price/value was
adopted by this experiment. Refer to the separate analysis for proposed next
work; no extra seed sweep or new experimental arm was executed.
