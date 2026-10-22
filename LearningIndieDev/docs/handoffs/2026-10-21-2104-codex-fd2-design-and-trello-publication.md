# FD2 design documents and Trello publication

[Working state](../WORKING_STATE.md) | Status: shared

- Handoff schema: 1
- Handoff ID: 2026-10-21-2104-codex-fd2-design-and-trello-publication
- Owner: codex
- Branch: BevBranch
- Baseline commit: 2d9d630
- Date: 2026-10-21
- Supersedes: none

## Summary

Bevin approved the FD2 permanent-progression direction, then authorized publication
of all 12 actionable cards with himself as owner and reviewer. The PDD, SDD,
ticket breakdown and project-context updates were committed as `3224307` and
pushed to both `origin/BevBranch` and `origin/ProjectMain`. The 12 Trello cards
are in Upcoming Work with Bevin's account assigned; no sprint placement or
estimate was added.

## Changes

- Added [FD2 Process Design](../FIELD_DATA_PERMANENT_PROGRESSION_PROCESS_DESIGN.md),
  describing the player flow, success settlement, failure behavior and first
  Genome Lab loop.
- Added [FD2 Solution Design](../FIELD_DATA_PERMANENT_PROGRESSION_SOLUTION_DESIGN.md),
  mapping the design to existing profile, settlement, Genome, UI and persistence
  systems.
- Added [FD2 ticket draft and published Trello index](../FIELD_DATA_PERMANENT_PROGRESSION_TICKET_DRAFT.md)
  with 12 actionable tickets, dependencies, acceptance criteria and direct card
  links.
- Updated [Project Context](../PROJECT_CONTEXT.md) and [Working State](../WORKING_STATE.md)
  with the approved FD2 direction and publication state.
- All 12 live cards are in the board's Upcoming Work list. Each has a description,
  Nick Nevins (Bevin) assigned, and `Owner: Bevin | Reviewer: Bevin` in its
  description. No card was assigned to a sprint.

## Decisions and assumptions

- Treat the PDD, SDD and ticket draft as design/planning artifacts; FD2 code and
  runtime behavior have not been implemented or verified.
- Rates, costs, cap count, reset fee and the first Hare node subset remain
  explicitly provisional or open as recorded in the design documents.
- Publish only the six FD2 document/context paths plus this handoff. Preserve the
  unrelated Unity importer metadata whitespace changes and `Assets/_Recovery`
  scene files in the working tree.

## Validation

- Read back all 12 cards on Trello. Confirmed their titles, descriptions, owner/
  reviewer text and Nick Nevins member assignment. The board count for Upcoming
  Work was 12 after creation.
- Checked repository-relative Markdown links in the FD2 design, ticket and handoff
files; all resolve. `git diff --check` passed for the staged document set. The
repository-wide handoff validator reports errors in pre-existing historical
handoffs; those files were not changed. No Unity tests or gameplay validation
were needed for this documentation-only change.
- Verified `origin/BevBranch` and `origin/ProjectMain` both received the published
  commit; final reference parity is recorded in the response.

## Risks and incomplete work

- No estimates or Sprint placement are included; sprint planning remains separate.
- Git publication to BevBranch and ProjectMain is requested and remains the next
  step for this handoff.
- The remaining unrelated Unity metadata and recovery-scene changes are not part
  of this publication.

## Next useful step

Continue with FD2-T01: finalize the first-slice progression content and tuning
definition before implementation begins.
