# Population-sweep observations - October 4, 2026

## At a glance

The screen completed **1, 680 runs across 336 starting-population combinations**. Hares survived to tick 600 in 128 runs; only six retained all three species. Seven frozen presets were rechecked on 20 fresh common seeds each: **140 more runs**, 51 Hare survivors and two all-three survivors. All 1, 820 main runs completed with zero execution errors and passed population, input, phase, metric and source-provenance checks. This identifies candidates for visual testing, not an accepted population default or balanced ecology.

Experiment: S4-03-20261004-wrapped-population-sweep. Report v1, October 4. Human owner: Bevin; team reviewer: Salty. Dirty BevBranch, baseline `6a0ab08e155b1a8a2962d19a6c5eca1bc8233118` plus wrapping and avoidance fixes; the commit alone does not identify executed source. Unity 6000.4.6f1. See [frozen method](README.md) and separate [interpretation](ANALYSIS.md).

## Screening coverage

All combinations of Plants 100/200/300/400/500/600, Hares 5/10/20/30/40/60/80 and Foxes 1/2/4/6/10/15/25/40 were run on seeds 11000-11004. Wrap on, 36x20, .1-second steps, six 100-tick phases. No Mutations, additional Genome, reinforcements or coupled responses. Species and skill values remained fixed. Hare extinction ends a run; early failures remain included.

- 62 of 336 presets had at least one survivor; two had 5/5: 500/5/15 and 600/5/10. Each averaged only 2.0 and 1.6 final Hares respectively, and neither had an all-three survivor.
- Six presets had one all-three survivor each; none had more than one. See the heatmap and full population summary for the complete matrix.
- With all tested Hare/Fox counts and seeds equally represented, survivors by starting Plants were: 100 = 5/280, 200 = 14/280, 300 = 9/280, 400 = 18/280, 500 = 31/280, 600 = 51/280. These are matrix aggregates, not a chosen-preset success rate.
- Existing 400/20/10 had 0/5 survivors. All screening counts are exploratory; the five seeds were used for selection, not independent confirmation.

## Fresh-seed recheck

The seven presets and selection reasons were frozen in `shortlist.json` before execution on seeds 12000-12019. Results below include every seed. Survival means a positive Hare count at tick 600. All-three means that surviving run also retained Plants and Foxes. Mean stop tick includes complete runs censored at 600; it is not an uncensored lifespan estimate.

| Plants/Hares/Foxes | Screen survival | Fresh survival | Fresh all three | Mean stop tick | Mean final Hares | Median final Hares | Recovery runs |
|---|---:|---:|---:|---:|---:|---:|---:|
| 600/40/25 | 4/5 | 10/20 | 2/20 | 497.80 | 23.45 | 0.5 | 12/20 |
| 600/20/25 | 4/5 | 11/20 | 0/20 | 513.40 | 1.45 | 1.0 | 13/20 |
| 600/5/10 | 5/5 | 13/20 | 0/20 | 578.30 | 2.70 | 1.0 | 0/20 |
| 500/30/25 | 3/5 | 6/20 | 0/20 | 339.25 | 19.55 | 0.0 | 6/20 |
| 500/5/15 | 5/5 | 8/20 | 0/20 | 424.35 | 4.70 | 0.0 | 2/20 |
| 200/20/15 | 3/5 | 2/20 | 0/20 | 319.75 | 0.20 | 0.0 | 3/20 |
| 400/20/10 | 0/5 | 1/20 | 0/20 | 438.30 | 0.30 | 0.0 | 1/20 |

Recovery is a descriptive screening flag: after tick 100, a positive trough at most half initial Hares, followed by at least a doubling and a gain of max(5, 25% of initial Hares). A later extinction still counts as a recovery event. It is not an approved fun/health threshold.

Paired comparison to 400/20/10: 600/40/25 survived in nine seeds where the reference failed, the reference survived in none where this candidate failed, both survived in one, and both failed in ten. Mean stop tick increased 59.5. The higher-survival 600/5/10 preset instead had 13 candidate-only survivors, one reference-only survivor and six common failures; it does not improve every seed.

## Slashlines and trajectories

Values below are means of per-run counts or Valid per-run ratios. Ratios are not pooled counts; sAVI's applicable sample count is shown. eAVI, bAVG and APS are Valid for all 20 seeds of each preset. N/A is preserved, not replaced by zero. The CSVs include pAVI, predAVG, cAVI, RFS and validity counts as well.

| Plants/Hares/Foxes | Mean PREY | Mean STRV | eAVI | bAVG | sAVI (Valid n) | APS |
|---|---:|---:|---:|---:|---:|---:|
| 600/40/25 | 109.30 | 90.60 | 0.652 | 0.0115 | 0.095 (20/20) | -0.564 |
| 600/20/25 | 64.35 | 238.35 | 0.790 | 0.0083 | 0.014 (19/20) | -0.495 |
| 600/5/10 | 4.90 | 364.80 | 0.976 | 0.0079 | 0.057 (20/20) | -0.240 |
| 500/30/25 | 57.45 | 46.80 | 0.671 | 0.0068 | 0.114 (19/20) | -0.142 |
| 500/5/15 | 12.50 | 191.80 | 0.917 | 0.0051 | 0.090 (14/20) | 0.043 |
| 200/20/15 | 57.60 | 22.25 | 0.713 | 0.0101 | 0.012 (16/20) | -0.426 |
| 400/20/10 | 106.35 | 151.70 | 0.661 | 0.0103 | 0.001 (20/20) | -0.658 |

600/5/10 averaged a Hare peak of 340.35, final 2.70, and 364.80 starvation deaths. It retained all three species in 0/20. 500/30/25's mean final 19.55 includes seed 12004 with 309 Hares; its median is 0. Across 600/40/25, mean peak 121.90, mean final 23.45 and median 0.5 coexist with 10 failures. These counts distinguish outlier populations, narrow finishes and growth/collapse from reliable survival.

HPS counts predator-active herbivore steps. eAVI=1-EHS/HPS; pAVI=1-PREY/ECN; bAVG=BIR/MAT. sAVI uses surviving population before starvation as its denominator; it is not seed survival. RFS=(FPO-SPO-ADD)*bAVG. APS is the existing composite, not a universal population-ranking score. See `METRIC_GUIDE.md` for every counter and denominator. cAVI measures crowding deaths, not energy savings.

## Recorded examples for manual comparison

All examples are 600/40/25, wrapping, no Mutations; final values are listed Plants/Hares/Foxes.

| Seed | Stop tick | Plants/Hares/Foxes at stop | Observed outcome |
|---|---:|---|---|
|12000|600|150/8/41|All three present; narrow Hare finish after recovery.|
|12011|600|115/198/10|All three present; large Hare recovery.|
|12003|493|462/0/40|Extinction contrast using exactly the same population preset.|

These examples are selected for contrasting histories, not a representative random sample. The full 20-seed set is retained. No additional visual acceptance of these presets has occurred.

## Verification and provenance

- All 336 combinations have exactly the declared five seeds; all seven recheck presets have exactly 20 fresh seeds, disjoint from screening and the older 10100-10119 panel. Compact file IDs, actual tick-zero populations, topology, ticks and empty loadouts match their requests.
- Every main run's final FPO reconciles with SPO+ADD+BIR-PREY-STRV-CRWD; all actual phase windows reconcile. There are 6, 646 screen and 664 recheck windows, **7, 310 total**. Histories include every tick through the actual stop. No failed run was discarded.
- Independent arithmetic checks passed for final and phase pAVI/eAVI/predAVG/sAVI/cAVI/bAVG/RFS/APS and their applicability statuses. No INVALID ratio appeared. Final screen pAVI has 126 N/A, sAVI 265 N/A and cAVI 1, 552 N/A; final recheck sAVI has 12 N/A and cAVI 89 N/A. Other final ratio statuses are Valid.
- Four pilot cases matched serial versus four-worker execution across every retained field; one matched the previous normal CLI full report. Eight pilot executions are separate from the 1, 820 research runs. Float tolerance 1e-6 covers serializer precision; only unused diagnostic null/empty arrays are normalized. Full event arrays are intentionally absent from compact screening records.
- All 73 frozen Game/Editor simulation-source and production-data asset hashes remain unchanged before/after both stages. Source diff, exact requests, script hashes, Unity/Python environment, per-run fingerprints, completion timestamps and output SHA256 are retained. No project dependency, species rule, asset, scene or fixture was changed by this sweep.
- Screen ran 05:06:31-05:23:20 UTC; recheck 05:24:22-05:26:28 UTC on October 4. Four local workers reused the scheduled runner; Unity remained stopped and responsive. No standard NUnit/Clean suite or build was run for these research-only scripts. This is not a new feature/build-readiness result.

## Evidence locations

Local ignored root: `artifacts/s4-population-sweep-20261004/`. It contains immutable compact records in `screen/` and `recheck/`, frozen contract/requests/shortlist, source diff, run-manifest with hashes/timestamps, validation JSON, `screen-populations.csv`, `screen-runs.csv`, `screen-phases.csv`, matching recheck CSVs, `paired-recheck-deltas.csv`, `shortlist-results.json`, `METRIC_GUIDE.md`, pilot records and runnable research scripts.

[Screen survival heatmap](../../../../artifacts/s4-population-sweep-20261004/survival-heatmap.png) | [Fresh population trajectories](../../../../artifacts/s4-population-sweep-20261004/shortlist-trajectories.png)

Charts use Matplotlib installed only under this ignored experiment folder; no Unity/repository dependency was added. All chart labels and exported images were inspected. Authored bounded-default and Inspector 400/20/10 fixture remain unchanged. No commit, push, Trello update, skill tuning, full-strategy-path test or long-horizon ecology approval is implied.
