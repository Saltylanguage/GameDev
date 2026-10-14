# Forest Edge fresh-seed confirmation results review

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-12-1609-codex-forest-edge-fresh-seed-confirmation-results-review
- Owner: codex
- Branch: BevBranch
- Baseline commit: 612e65d5
- Date: 2026-10-12
- Supersedes: none

## Summary

Reviewed Bevin's five October 12 workbook exports against their frozen completed
batches. They are cumulative: 1,600/3,200/4,800/6,400/8,000 runs. The latest
contains 8,000 unique runs, 80 conditions and 100 matched seed IDs. None of the
selected worlds provides reliable tick-600 ecology; useful strategy effects
and fully unassisted successes do exist.

## Changes

- Read-only analysis bundle: `artifacts/confirmation-review-20261012/`.
  `ai-analysis-v1.md` distinguishes facts, interpretation, uncertainty and
  recommendations; `confirmation-overview.png` compares worlds and seed 10016.
- Overall 261/8,000 goals (3.26%), 2,167 Hare survivors (27.09%) and 1,438
  all-three survivors (17.98%). Only 2,304 reach all five recorded decisions.
- Best observed condition: preset 04, 64x48 H4/F16, Gardeners + Restore,
  10/100 goals (95% Wilson 5.5%-17.4%). Old 2/3 discovery leaders each give
  8/100 on confirmation seeds. No-purchase goals: 60; fully unassisted
  Skip all/No purchases goals: 15/500.
- Paired descriptive comparisons: Trailblazer improves Hare survival by
  6.95 percentage points; Gardeners improves the ecology goal by 1.30 points.
  Uncertainty resamples 100 whole seed blocks, retaining all correlated arms.
- Original workbooks, experiment files, runtime and app code unchanged.

## Decisions and assumptions

- User supplied the workbooks following preset preparation; interpreted as a
  results review. No new experiment or workload was launched.
- 5 Hares/5 Foxes/100 Plants at 600 is the research lens, not approved production
  tuning. Recommendations remain Needs Review.
- Do not sum cumulative exports or treat 8,000 arms as independent seed trials.

## Validation

- Existing completed-sweep verifier passed five complete raw hashes and plans.
- Independently verified 40 frozen Workbench file hashes, identical discovery
  runner/snapshot, all 80 exact seed panels and unique condition/seed pairs.
- 7,720 workbook metrics matched raw outcomes, including all 1,000 selected
  examples across the five exports and all 120 latest paired comparisons.
- All earlier summary rows match the latest; original workbook hashes unchanged.
- Analysis self-check scripts executed successfully; chart visually inspected.
  No Unity tests or simulation reruns were required for read-only analysis.

## Risks and incomplete work

- Among 261 goal successes, 210 have fewer Plants at 600 than at 500. Endpoint
  success does not establish lasting stability or recovery.
- Five candidate worlds change multiple settings, so ranks do not isolate
  gestation or map effects. Automatic policies are not human play acceptance.
- Local/uncommitted/unpushed. Earlier local tooling changes preserved.

## Next useful step

Bevin to review preset 04 as the next controlled research world and visually
inspect no-purchase seed 10016 Gardeners (48x48 H4/F24), seed 10029 Gardeners
(64x48 H8/F24), and seed 10030 Skip all (64x48 H4/F16). Design any post-600 test
to retain choice ticks 100/200/300/400/500; changing ticks per round would move
the interventions. No follow-up workload or production change is yet approved.
