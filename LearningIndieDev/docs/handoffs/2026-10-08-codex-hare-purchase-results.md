# Hare purchase panel: completion and results review

2026-10-08. BevBranch / HEAD e7a6c602. No new simulation, gameplay tuning,
commit, push or external board change in this results-review turn.
The launch and implementation remain recorded in
`2026-10-08-codex-hare-purchase-panel.md`.

## Verified completion

Root: `artifacts/cellsim-hare-purchase-20261008-132317/`.
Pipeline and batch state: Completed. All 512 chunks / 12,800 assigned runs
completed; 128 arms each have 100 fresh matched seeds 50000-50099.
Simulations: 2361.19 seconds / 39.35 minutes, 5.42 runs/second, 16 workers,
93.81 MiB peak reported worker working set. Automatic analysis completed
2026-10-08 18:15:21 UTC / 14:15 EDT; error log empty.

Batch identity: `7fc4666c6ad2459af4c6917e32ca21987d8399a0417f2cfaf6c8a1767899fafa`.
Raw SHA256: `aa03a3c4a6943249bf479672009367bb4fa93b36a7cc2f4ef80090a77af94aba`.
Completed-sweep input hashes, raw aggregate hash and analysis output hashes were
rechecked via the frozen analyzer. Report provenance matches the batch.
Every arm has APS/AHS Valid n=100 with zero invalid/missing cumulative values.
All 64,000 unique assigned window rows exist: 57,532 reached, 6,468 not reached.
Stop counts: policy-cap 47,307; insufficient-currency 5,500; target-met 4,725;
no placement/capacity stops observed. These are observations of the approved panel.

## Results and interpretation

The focused analysis is `purchase-analysis/focused-review.md` and `.json`.
It contains restoration tables with Hare/all-species survival, both comparison
references and mean APS/AHS, a purchase-effect matrix, and all-path qualifications.
The build-focused-review.py script regenerates the derived review from the
completed evidence without changing frozen tools, raw output or inputs.

- Restoration improved both survival endpoints compared with each path's own
  no-purchase control on all three paths in D4/S19, D4/S14 and D5/S25.
  C2/S14 Gardeners dropped from 44% to 42% Hare survival and 34% to 26%
  all-species survival. It still beat Skip all with the same buying policy;
  those comparisons answer different questions.
- Late-five improved both survival endpoints in all twelve candidate/path arms;
  all-species gains ranged from +1 to +8 percentage points. Most are small
  exploratory estimates. D4/S19 Gardeners still had one point lower Hare
  survival than Skip all with the same late-five purchases (24% vs 25%).
- Early-five reduced all-species survival in 14/16 total arms, was neutral in
  one, and improved D4/S19 Warren by one point.
- D5/S25 Gardeners each-five was the highest observed all-species survival arm:
  59% versus 39% without purchases (+20 pp; 35 matched wins/15 losses;
  unadjusted approximate 95% CI +6.6 to +33.4 pp). Hare survival was 63% versus
  42%; mean APS -0.797 versus -0.692; mean AHS -0.109 versus -0.052.
  Actual mean ADD 21.91 / spend 219.1 Field Data per assigned run, versus
  restoration's 6.36 / 63.6 and late-five's 3.45 / 34.5.
- D5/S25 restoration supported the three paths with all-species survival
  44/45/48% (Trailblazer/Warren/Gardeners), versus 30/34/39% without buying.
  D4/S14 Gardeners restoration reached 49% versus 38%, another strong arm.

Interpretation: purchase timing/current-population conditions appear more useful
than unconditional early buying. Survival alone does not establish healthy
biological growth, balance or enjoyment; APS/AHS often decrease. Cause has not
been isolated by these descriptive outcomes. Proposed follow-ups are fresh-seed
confirmation and matched gameplay review of restoration, late-five and the
high-spend D5/S25 Gardeners arm. No follow-up execution is authorized by this note.

## Evidence and limits

All arms: `purchase-analysis/report.md` and `results.json`.
Assigned window/recovery diagnostics: `purchase-analysis/windows.csv`.
Status-aware metric distributions / matched deltas:
`analysis/metrics.csv`, `paired-deltas.csv`, `metrics.sqlite`, `analysis.json`.
Review retains actual observation windows and both references: same-path/no-buy
and Skip-all/same-buy. Statistical comparisons are exploratory and unadjusted
for multiple testing; selecting maxima adds optimism. Frozen source is unchanged
from launch. Enjoyment and tick-700 ecology viability need separate evidence.
