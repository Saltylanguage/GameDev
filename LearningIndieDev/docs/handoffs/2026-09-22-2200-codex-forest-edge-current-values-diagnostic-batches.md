# Forest Edge current values diagnostic batches

[Working state](../WORKING_STATE.md) | Status: complete

- Handoff schema: 1
- Handoff ID: 2026-09-22-2200-codex-forest-edge-current-values-diagnostic-batches
- Owner: Codex
- Branch: BevBranch
- Baseline commit: c6b3282
- Date: 2026-09-22
- Supersedes: none

## Summary

Ran two 20-seed Clean CellSim batches on the current authored Forest Edge
configuration at commit `c6b3282`: seeds 10100-10119 at 600 and 1,200 ticks.
The runs used opposed-roll combat, natural attack opportunities, no upgrades,
and the authored 20x20 / 0.2-second scenario. They provide an exploratory
current-configuration baseline after Salty's changes.

Foxes ended extinct in all 20 seeds by tick 600. Hares ended extinct in 10/20
seeds at both horizons; their mean final count was 0.80 at 600 ticks and 0.85
at 1,200. This result does not isolate which of the co-occurring authored
changes accounts for the collapse.

## Changes

- No code or authored asset changes. Both source trees were clean before and
  after the runs.
- 600-tick evidence: [raw report](../../artifacts/cellular-experiment-20260922-215025/report.json),
  [summary](../../artifacts/cellular-experiment-20260922-215025/report-summary.md),
  and [manifest](../../artifacts/cellular-experiment-20260922-215025/manifest.json).
- 1,200-tick evidence: [raw report](../../artifacts/cellular-experiment-20260922-215328/report.json),
  [summary](../../artifacts/cellular-experiment-20260922-215328/report-summary.md),
  and [manifest](../../artifacts/cellular-experiment-20260922-215328/manifest.json).

## Decisions and assumptions

- No balance decision or success threshold was predeclared. Treat these as
  diagnostic runs, not formal balance acceptance.
- Configuration values changed together in Salty's commit; these runs describe
  the combined configuration and do not establish causality for any one value.
- Design context reported by Bevin on 2026-09-22: Bevin and Salty are exploring
  a two-stage game in which players first survive as a species to earn data,
  with collapse expected without intervention, then spend data on upgrades in
  a second area to build a healthy, collapse-resistant environment. Progress
  follows only after that scenario is made healthy. This is a working concept,
  not a finalized specification or success gate. The no-upgrade runs therefore
  show the proposed first-stage pressure baseline; they do not evaluate the
  upgraded survival loop, the second-stage ecosystem goal, or player experience.

## Validation

- `CellSim Doctor -Execution Auto`: healthy; Unity CLI 1.0.0-beta.8, Unity
  Editor 6000.4.6f1, active Unity Personal license; project initially closed.
- Both `CellSim Run -Execution Clean` batches completed with 20 runs each,
  source commit `c6b3282cb5aad6257cdf47a531453bbc2b40533a`, and clean source
  state. Each report parsed without warnings through the artifact-summarization
  skill.
- At 600 ticks, means per run: Fox births 9.90, deaths 17.95, starvation deaths
  15.85, and combat kills 15.65; final Fox extinction 20/20. Hare births 0.60,
  combat deaths 15.65, starvation deaths 6.15, final count 0.80, extinction
  10/20. Plant count averaged 90.70 at start and 361.20 at finish.
- At 1,200 ticks, Fox outcomes were unchanged because Foxes were already extinct
  by 600. Hare births averaged 0.80, combat deaths 15.65, starvation deaths
  6.30, final count 0.85, extinction 10/20. Plant count averaged 373.55 at
  finish.
- No Unity unit-test suite was run. The specialized experimental Stat-Line was
  not enabled; the reports retain population histories and direct activity and
  death-cause counters.

## Risks and incomplete work

- The no-upgrade setup collapses across this seed panel. In light of the
  reported two-stage concept, this can be expected first-stage pressure rather
  than a balance failure. The runs do not establish whether that pressure makes
  the survival phase engaging or how upgrades change outcomes.
- The results bundle multiple authored changes, and no formal experiment gate
  was approved before execution. A future balance decision needs a declared
  phase-specific question and criteria.

## Next useful step

Keep these runs as the no-upgrade baseline. When the two-stage concept is ready
for evaluation, define first-stage survival/data measures and second-stage
healthy-ecosystem criteria, then test the relevant upgrade configurations.
