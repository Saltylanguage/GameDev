# Loose Ends retention decisions

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-01-0112-codex-loose-ends-retention-decisions
- Owner: Codex
- Branch: ProjectMain
- Baseline commit: 2e11ce5b
- Date: 2026-10-01
- Supersedes: none

## Summary

Reviewed the active Loose Ends ledger against the current checkout and the
2026-09-09 artifact audit. Three items had a decision already supported by
current evidence, and one was an unbounded refactor rather than a concrete
defect. This pass records those dispositions without removing assets or raw
evidence.

## Changes

- Resolved P2-005 by retaining the three raw duplicate bundles with
  trailing-comma parse warnings. The other two candidates had their raw files
  removed by the 2026-09-09 cleanup, so the ledger's five-candidate status was
  stale. Updated the artifact audit with the decision.
- Resolved P2-022 by retaining the Terrain Paint scene, script, and test as a
  manual developer diagnostic. Updated the main-flow cleanup inventory.
- Pruned optional CF-6 measurement from the active loose-ends list; S3-05
  remains the backlog owner. Pruned P2-015's unbounded large-file refactor and
  kept its candidate seams in the hygiene summaries for a future focused
  blocker.
- Pruned the ledger's resolved history and duplicated planned work. P1-032
  remains in the S4 control record; P1-034 is now explicit in the future
  sprint roadmap; P2-007 and P2-014 remain in the hygiene tickets. At the time
  of this review, P1-026 and P1-029 were the two open loose ends; their later
  closure is recorded in separate dated handoffs.

## Decisions and assumptions

- These dispositions close planning/retention questions. They do not imply
  that terrain presentation, runtime IMGUI generally, or any large-file seam
  was rewritten.
- The parsed clean `cellular-experiment-20260905-064045` report stays the
  representative, and the three legacy raw reports stay available for
  forensic comparison. No archive or deletion was needed.

## Validation

- Confirmed the two earlier duplicate bundles no longer contain raw
  `report.json` or `unity.log`, while the three parse-warning reports and clean
  representative still do.
- Checked source references for `TerrainPaintPreview`: the diagnostic scene
  owns the component, and its test covers cell-coordinate mapping. The scene
  is outside the documented player route and Build Settings.
- `git diff --check` passed after the documentation edits. No Unity tests or
  simulation runs were performed for this documentation-only pass.

## Risks and incomplete work

- At the time of this review, P1-026 worker tooling had a separate local
  candidate commit `a227f118` on `codex/p1-026-worker-lifecycle`; this pass did
  not touch that checkout or the shared worker branch. Its later propagation
  and validation are recorded in the P1-026 closeout handoff.
- At the time of this review, P1-026 and P1-029 remained open and S4 remained
  Proposed. Their later status changes are recorded in separate dated
  handoffs. P1-032 is owned by the S4 control record, P1-034 by the future
  sprint roadmap, and P2-007/P2-014 by the hygiene ticket summaries.
- The shared `ProjectMain` checkout already contained Main Menu and S4 planning
  edits. They were preserved during the review and integrated with the S4
  kickoff documentation before this work was shared.

## Next useful step

Review the remaining gated items with their owners, then update the ledger only
when their concrete acceptance checks pass.
