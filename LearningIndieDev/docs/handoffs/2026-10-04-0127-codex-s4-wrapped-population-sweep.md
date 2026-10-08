# s4-wrapped-population-sweep

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-04-0127-codex-s4-wrapped-population-sweep
- Owner: Codex
- Branch: BevBranch
- Baseline commit: 6a0ab08e
- Date: 2026-10-04
- Supersedes: none

## Summary

Bevin visually prefers wrapping despite extinction, and authorized finding interesting starting populations. The 336-preset screen and seven-preset fresh-seed recheck are complete locally: 1,820 main runs plus eight pilot executions. Recommend visual comparison of 600 Plants / 40 Hares / 25 Foxes first; no production preset was selected or tuned.

## Changes

- Added the [population experiment package](../Research/Experiments/S4-03-Wrapped-Population-Sweep/README.md), [factual report](../Research/Experiments/S4-03-Wrapped-Population-Sweep/REPORT.md) and [separate interpretation](../Research/Experiments/S4-03-Wrapped-Population-Sweep/ANALYSIS.md).
- Recorded Bevin's wrapping visual preference as a dated follow-up to the earlier wrapping package. Its 80-run observations remain unchanged.
- Generated ignored local records, full tick histories, final/phase slashline CSVs with statuses, heatmap, fresh-seed trajectories, paired deltas, requests, script hashes and per-run SHA256/timestamp manifest under `artifacts/s4-population-sweep-20261004/`.
- Reused the existing scheduled runner from an ephemeral research script with four isolated local workers. No production source, asset, scene, fixture or project dependency was changed by this work block. Matplotlib lives only in the ignored experiment folder.

## Decisions and assumptions

- 36x20 wrapping, .1-second steps, 600 ticks, six 100-tick phases; Production Forest Edge rules, natural opposed-roll combat, Bev experimental features, no coupled responses, additional Genome, reinforcements or Mutations.
- Screen: Plants 100/200/300/400/500/600, Hares 5/10/20/30/40/60/80, Foxes 1/2/4/6/10/15/25/40, common seeds 11000-11004. All 336 combinations fit 720 cells.
- Seven presets frozen before a 20-seed recheck on 12000-12019. The screen is exploratory, not balance acceptance. Source is baseline 6a0ab08e plus previously uncommitted wrapping/avoidance changes, captured by exact source hashes and diff.
- Bounded remains the authored default; Apply S4 Fixture remains 400/20/10. Wrapping visual preference does not approve ecology balance. Salty's journey/DNA planning remains separate.

## Validation

- All 1,680 screen and 140 recheck runs completed without errors; input seeds/populations, every tick, all 7,310 actual phase windows, FPO accounting, slashline arithmetic and validity statuses passed independent checks.
- Four cases matched serial/parallel execution; one matched the prior normal CLI report for every retained field. Eight pilots are not included in the 1,820 main-run count. Compact output intentionally omits per-event logs.
- All 73 frozen relevant source and production-data asset SHA256 hashes remained unchanged. Charts inspected; `git diff --check` passed. No standard NUnit/Clean suite, build or new long-horizon test was run for these research-only scripts.
- Fresh Hare survival: 600/40/25 = 10/20, 600/20/25 = 11/20, 600/5/10 = 13/20, 500/30/25 = 6/20, 500/5/15 = 8/20, 200/20/15 = 2/20, 400/20/10 reference = 1/20. Only 600/40/25 retained all three species in this recheck, in 2/20 runs.

## Risks and incomplete work

- All presets still fail on some seeds. 600/5/10's higher survival often means one Hare left after a large boom; it is not a healthy ecology. Final Fox/Plant counts on failures belong to the early stop, not tick 600.
- Shortlist selection used only five screening seeds; fresh seeds reduce that bias but do not prove reliability or long-term resilience. High means can be driven by outliers. Ratios use differing denominators; APS is not a cross-population ranking criterion.
- Local scripts/artifacts are ignored, and production changes from prior tasks remain uncommitted. Nothing was pushed or posted to Trello by this sweep.

## Next useful step

Bevin: try **600/40/25, Wrap Edges on**, seeds **12000**, **12011** and **12003**, skipping Mutations. Expected recorded outcomes are an all-three narrow finish, a large all-three recovery, and extinction at tick 493 respectively. Set the fixture first, then override counts/seed so another fixture application does not restore 400/20/10. Compare feel before choosing one or two presets for paired strategy-path tests on fresh seeds.

Salty/agent review: read method -> factual report -> analysis. Verify candidate selection against fresh histories, distinguish presence at 600 from balance, and keep scenario-default or skill changes separate from this exploratory result.
