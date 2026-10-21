# FD-T01 route entry and compatibility review

[Working state](../WORKING_STATE.md) | Status: complete

- Handoff schema: 1
- Handoff ID: 2026-10-16-1335-codex-fd-t01-route-entry-and-compatibility-review
- Owner: codex
- Branch: BevBranch
- Baseline commit: acc371a
- Date: 2026-10-16
- Supersedes: none

## Acceptance update - October 16, 2026

Bevin explicitly accepted FD-T01 and waived any further review: "we dont need
any further review that FD-T01 is good to go". The additional Salty review is
no longer required for this ticket. All four criteria are closed under that
user-directed exception; no review by Salty is claimed. The accepted entry is
Desktop Simulation -> larger map -> explicit five-node test-route selection.
FD-T02 may proceed. No implementation, test run, commit or push is implied.
The original preparation checkpoint below is retained as history.

## Summary

Prepared FD-T01's concrete entry and compatibility review against `acc371a`.
Bevin selected the larger journey map as the Desktop Simulation entry, with
the five-node test route selected there. Salty's review is still outstanding;
the ticket remains In Progress rather than being closed from source review.

## Changes

- Added [entry/compatibility review](../FIELD_DATA_ENTRY_REVIEW.md), including player entry, explicit route selection, source findings, development inputs and evidence limits.
- Updated PDD P-01, SDD section 11, combined alignment and current collaboration context.
- Began FD-T01 on Trello; owner/reviewer allocation remains unchanged. Other 15 tickets remain Current Work.

## Decisions and assumptions

- User confirmed larger-map entry followed by test-route selection. No extra opening reward/condition is inserted into the approved five-node route.
- Proposed composition retains separate larger entry map and active five-node map/definition references; explicit selection controls the mode. Exact launch-action placement remains FD-T13 rendered review.
- Retain ForestEdge/Hare seed 10100 as the development launch reference and current 36x20 Plant400/Hare55/Fox35 authored inputs. This is not a balanced fixture or a frozen 200-seed panel.
- Existing row-specific eligibility, two-cycle target and legacy award/reset paths cannot be reused blindly for the new mode. No global six-round constants or existing map nodes are reinterpreted.

## Validation

- Inspected Desktop host/scene composition, launch request/defaults, map/node types, preview continuation/clock/award/reset logic, authored scenario/species GUIDs and relevant existing test sources.
- Recorded source/input hashes in ignored `artifacts/fd-t01-entry-20261016/source-baseline.json` at the inspected revision.
- Checked local documentation links, scoped whitespace, original design checklist preservation and unchanged runtime/assets before handoff.
- No gameplay implementation, compilation, Unity run, player build or 200-seed comparison performed. Existing test source is a regression reference, not fresh pass evidence.

## Risks and incomplete work

- Salty has not reviewed this proposal. FD-T01's first criterion explicitly requests both Bevin and Salty's review; it remains open.
- Current ForestEdge references Plant under TestData; retain the actual source and GUID rather than moving it during this task.
- Current profile-input review and real app exit/relaunch acceptance remain separate P1-032 work.
- Ten pre-existing terrain importer edits remain untouched and local. No new commit or push is part of this review pass.

## Next useful step

Review the concrete FD-T01 proposal with Salty, record any changes and close
the remaining criterion when that review occurs. Then begin FD-T02's immutable
definition contract and validation within these agreed boundaries.
