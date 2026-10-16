# Field Data actionable ticket draft

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-16-0029-codex-field-data-actionable-ticket-draft
- Owner: codex
- Branch: BevBranch
- Baseline commit: dcd6bb0
- Date: 2026-10-16
- Supersedes: none

## Summary

Prepared a local actionable ticket draft from the Field Data PDD/SDD and the
merged planning alignment. Bevin explicitly chose to leave owners and reviewers
unassigned for team review. No Trello publication or sprint kickoff occurred.

## Changes

- [Ticket draft](../FIELD_DATA_TICKET_DRAFT.md): 16 provisional FD-T IDs,
  outcomes, dependency order, estimated active hours, 79 unchecked acceptance
  items and source traces across all seven PDD stages.
- Linked the draft from the PDD, SDD, combined alignment, original checklist
  and Working State without changing the 124 original item IDs/statuses.
- Proposed FD-T01–FD-T05 as the first foundation review package, not an approved
  sprint. Reused merged map/profile/reset work instead of duplicating it.

## Decisions and assumptions

- Focus the draft on the first Field Data slice already defined in the PDD/SDD.
  Broader ecology corrections and permanent-content work remain separate.
- All owners/reviewers are unassigned. Estimates are proposed planning ranges,
  not accepted promises: 57–95 active team-hours overall, 15–25h for the first
  five. Machine time and a future tuning iteration remain unestimated.
- FD-T01 resolves route-entry/compatibility and relevant technical/product
  review before implementation selection. The 29-marker content map does not
  replace the agreed five-node/18-round acceptance route.
- FD-T02 establishes the definition and supported starter entries; FD-T07
  finishes signed species entries and explicit runtime mappings before they
  enter runs. This prevents an authoring/runtime dependency cycle.
- Tests are included in implementation tickets; integrated UI, replay parity
  and internal economy evaluation retain distinct acceptance evidence.

## Validation

- Checked 16 unique index/body IDs, valid dependency targets and no dependency
  cycle, 79 unchecked acceptance items and coverage of all seven PDD stages.
- Verified estimate sums of 57–95h overall and 15–25h for FD-T01–FD-T05.
- Checked 61 local links, Markdown fences/whitespace and preservation of all
  124 original checklist IDs/order. Scoped `git diff --check -- docs` passed.
- Inspected current branch/status: baseline `dcd6bb0`; ten pre-existing terrain
  metadata changes remain outside this documentation task.
- No gameplay implementation, Unity test suite or 200-seed comparison ran.
  The preceding merge's test results remain dated evidence in its own handoff.

## Risks and incomplete work

- Tickets remain draft and local. Owners/reviewers, accepted estimates, exact
  route entry, frozen execution inputs and sprint capacity selection need review.
- No Trello cards, sprint IDs, assignments, commit or push were created.
- Full-backlog effort exceeds the earlier 32h feature planning envelope; no
  implication that the full feature fits one sprint.

## Next useful step

Review FD-T01–FD-T05 as the first package, revise scope/estimates where needed,
then agree owners/reviewers and the exact cards to publish. Keep later shop/UI/
evidence tickets visible as dependencies without committing the whole backlog.
