# Field Data design baseline review

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-15-2335-codex-field-data-design-baseline-review
- Owner: codex
- Branch: BevBranch
- Baseline commit: 735b189c
- Date: 2026-10-15
- Supersedes: none

## Summary

Reviewed the complete Field Data Step 1–5 plan against Bevin's direct decisions
and consolidated the first-slice rules into [Field Data and Journey Loop Design
v1.0](../FIELD_DATA_JOURNEY_DESIGN.md). Design scope and provisional settings are
complete; the document is local and ready for review. Implementation and
internal self-play have not begun.

## Changes

- Created a standalone design containing the continuous five-node route, wallet,
  population controls, ordered income/reorders, complete starter catalogs,
  reward schedule, UI requirements, correctness checks and internal test plan.
- Preserved all 124 original IDs in the [numbered checklist](../FIELD_DATA_DESIGN_PLAN.md).
  Distinguished completed design decisions from pending implementation/tests.
- Preserved the full synthetic ledger and source audit as design appendices.
  Corrected the stale paper pass-rule question and missing delivery gaps,
  including positive-only existing upgrade construction versus signed shops.
- Replaced duplicated current summaries in economy history, Project Context and
  Working State with concise authoritative pointers. Historical suffixes were
  checked unchanged against pre-review copies.

## Decisions and assumptions

Bevin closed five review gaps on 2026-10-15:

1. Start random reordering at 10 FD; escalate the unrounded basis 10% after a
   successful paid reorder of either kind; round the random charge up and keep
   chosen at exactly 10 times that charged price; reset at the next shop.
2. Use 200 matched fixed seeds per comparison arm. Actual IDs, scenario and build
   snapshots are frozen before execution; they are not invented in the design.
3. Three free choices or Skip after rounds 1–5; a free three-choice node reward
   after Simulations 1 and 2; 10 FD after each successful simulation clear.
   Eligible income offers remain randomized, not guaranteed.
4. Block removal of the final selected-species individual.
5. Keep population controls on nonterminal round-6 results before each shop;
   close them after terminal success.

Preserved prior decisions: internal team testing, deferred external-player
research, related economy changes allowed as a bundle, team judgment without a
numeric acceptance gate, permanent-currency conversion deferred, and next
content chosen only from subsequent self-play findings.

## Validation

- Exact-arithmetic check passed all 18 synthetic payout boundaries, 12 strategy
  outcomes, nine incremental-income/payback cases and reorder-price examples.
- All 124 IDs were preserved uniquely and in order; five design sections exist.
- Local links, Markdown fences and whitespace checked for the new baseline and
  checklist. Historical suffix preservation checked for the three summary docs.
- `git diff --check` passed for tracked review documents; untracked document
  whitespace was checked separately. A repository-wide diff check reports
  pre-existing trailing whitespace in terrain import metadata; those unrelated
  asset changes were preserved.
- Inspected current catalog/stat bounds and progression/settlement integration
  sources, plus the profile persistence handoff. Source inspection is not runtime
  acceptance.
- No gameplay changes, Unity tests, build, ecology experiments, board writes,
  commit or push were performed.

## Risks and incomplete work

Prices and effects are provisional first-test settings, not balance findings.
The route, income modifiers, signed shop effects, settlement corrections and
correctness checks are still delivery work. The 200-seed panel has not run.
Its actual seeds and input snapshots must be recorded during test preparation.
The paper ledger assumes named offers appear and skips free income rewards;
its paths do not model real ecology or intervention outcomes.

The checkout already contained terrain import metadata, journey documentation
and a Sprint 5 draft. That unrelated work was preserved. Pre-review document
copies and the arithmetic check output are in the temporary local folder
`C:/Users/bevin/AppData/Local/Temp/field-data-design-review-rnlhbxdn/`.

## Next useful step

Review the standalone design. A subsequent implementation work block can use
the checklist to deliver the first slice, then run the frozen internal panel
and record the team's accept/revise decision. Keep permanent progression and
new content outside this slice until their deferred decisions are revisited.
