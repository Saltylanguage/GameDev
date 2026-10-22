# feeding-fox-ramp-comparison

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-21-1139-codex-feeding-fox-ramp-comparison
- Owner: codex
- Branch: codex/audio-direction-and-sourcing
- Baseline commit: 6ffb7b10
- Date: 2026-10-21
- Supersedes: none

## Summary

Josh approved the complete 48-run feeding/Fox ramp panel. All runs completed
and accounting validates. Both interventions improve the opening Hare curve,
but none produces persistent Hare/Fox populations. The combined intervention
exhausts Plants in all seeds before Hare starvation. Next useful diagnosis is
food renewal/access and growth limits. Results and code remain local.

[Observed results](../Research/2026-10-21-Feeding-and-Fox-introduction-results.md)
and [AI analysis](../Research/2026-10-21-Feeding-and-Fox-introduction-analysis-v1.md)
are recorded separately under SG-001.

## Changes

- Executed the approved four-arm panel from
  `artifacts/feeding-fox-comparison-20261021-02/run-comparison.ps1 -IncludeCombined`.
  Its run-index, frozen source/assets, commands, manifests, raw reports and
  analysis scripts/hashes are retained.
- Corrected scheduled ecology observation to continue through its declared
  window after player-species extinction. The runner's new constructor option
  defaults to existing gameplay behavior; scheduled research disables the stop
  only when no Hare purchase policy is active. Mutation policies and purchase
  policies retain their prior behavior.
- Added a focused regression proving full observation and default gameplay
  stopping, and updated tooling/context/scope documentation.
- Added derived comparison JSON, per-seed CSV, trajectories and per-arm compact
  summaries. No raw evidence was rewritten or legacy wilt correction applied.

## Decisions and assumptions

- Scope owner Josh explicitly selected all four arms: 12 seeds 10120–10131,
  Forest Edge 36×20, 400 Plants/20 Hares, 1200 ticks at 0.1 seconds, no Genome
  or Mutations, sixteen empty 75-tick continuous windows. Feeding threshold
  6/25; ten initial Foxes or two plus eight arrivals at ticks 75–600.
- First baseline attempt inherited journey extinction stopping and failed
  technical acceptance. It is preserved in `cellular-experiment-20261021-112933`
  with a rejection receipt in bundle `feeding-fox-comparison-20261021-01`.
  All accepted arms were then run on the same corrected frozen source in `-02`.
- Initial count changes alter animal placement; same-start-count feeding pairs
  share initializer inputs. This is not an identical-map timing-only study.
- Production balance assets, gameplay Fox arrivals, upgrade values/economy,
  Git history and external cards were not changed by this experiment.

## Validation

- Unity Clean focused PopulationConfigurationTests: **40/40 passed**,
  `artifacts/unity-tests-20261021-113318/EditMode-results.xml`. The first focused
  attempt was 39/40; its new regression caught wiring applied to the wrong
  runner method, which was corrected before panel execution.
- Standalone .NET build: **0 warnings, 0 errors**. Portable regression self-test
  passed (run with `dotnet --roll-forward Major`).
- Comparison validation: **48** full-window runs; **192** exact successful
  arrivals; **144** species flow accounts; complete tick histories;
  nonnegative tickwise births matching totals and first-birth milestones;
  command/header controls; all **389** frozen source/data/tool files unchanged.
- Corrected baseline population histories match the first attempt's prefixes.
- Artifact-summarization produced four compact packets with same-seed baseline
  comparisons. Custom comparison preserves new schema-39 metadata.
- Scoped `git diff --check` passed. No player/visual acceptance test performed.

## Risks and incomplete work

- One scenario/12 seeds is exploratory evidence. Neither natural values nor
  the exact ramp is accepted as production balance. No follow-up arm is approved.
- Shared-tile diet-target rejection remains unchanged; its intent and ecological
  effect still need review. Plant counts do not measure reachable energy.
- Only baseline seed 10127 retains one Hare at tick 1200; no seed in any arm
  retains Foxes or a two-Hare breeding population. Combined Plant loss is 12/12.
- Local dirty checkout includes earlier Genome/tooling/art work. Preserve it;
  this task did not commit or push. Use source hashes, not HEAD alone, to replay.

## Next useful step

Review food renewal and food access from current source and retained reports,
then check how reproduction/crowding limits demand. Resolve confirmed defects
with narrow regressions before proposing another matched balance comparison.
Josh/Sim own subsequent tuning values and experiment scope.
