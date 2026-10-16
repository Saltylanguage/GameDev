# Field Data implementation design drafts

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-15-2352-codex-field-data-implementation-design-drafts
- Owner: codex
- Branch: BevBranch
- Baseline commit: 735b189c
- Date: 2026-10-15
- Supersedes: none

## Summary

Drafted separate process and solution designs for implementing the reviewed
Field Data baseline. Both are draft 0.1 for Bevin's review; product rules remain
in the canonical v1.0 design. This was documentation and source inspection only.

## Changes

- [Process Design, PDD-FD-001](../FIELD_DATA_PROCESS_DESIGN.md): current/required
  player flow, controls/exceptions, ownership and seven proposed delivery stages
  FD-I01–FD-I07 with exit evidence.
- [Solution Design, SDD-FD-001](../FIELD_DATA_SOLUTION_DESIGN.md): current-source
  integration map, proposed session/definition/helper ownership, transactions,
  signed effects, deterministic actions, Noesis integration and shared runner
  evidence/compatibility design.
- Linked both drafts from the baseline, original checklist and Working State.

## Decisions and assumptions

- Preserve one retained world and `SpeciesProgression` wallet; centralize new
  journey settlement and action authority in a feature-local plain C# session.
- Use explicit composition, existing simulation timing and Noesis boundaries;
  avoid a second wallet, copied simulator or gameplay calculations in the UI.
- Extend population changes atomically with resource/creature-layer semantics.
  Recompute signed shop overlays from net contributions before final bounds.
- Retain independent identities for population and node-clear awards. Retrying
  after one commits must finish the missing award without paying the first twice
  or resetting an already-resolved choice/shop.
- These are proposed implementation choices, not approved code or added rules.
  Sprint scheduling remains with Salty; this draft assigns no delivery owner.

## Validation

- Inspected the existing progression, simulation/preview, journey assets,
  upgrade contracts/registry, Noesis shell, profile and standalone source paths.
- Read back both drafts and checked their alignment with the approved rules,
  seven-stage mapping, 18-round route and separate permanent-state boundary.
- Checked local source/document links, Markdown fences and scoped whitespace;
  preserved all 124 unique original checklist IDs. No new link failure remains
  in the draft documents or edited navigation sections.
- A broader link scan also found 14 historical artifact links in the unchanged
  Working State history whose targets are absent locally. They are outside this
  change and were left intact; the scoped check covers the new navigation.
- No Unity, runtime, player-build, balance or 200-seed check ran. Documentation
  validation is not evidence that the proposed feature is implemented.

## Risks and incomplete work

- Concrete proposed files/APIs require implementation review. Avoidance needs
  an explicit signed mapping beyond its existing positive-only snapshot field.
- Existing batch schedules do not implement this journey; shared-source runner
  integration and parity must precede the future 200-seed comparison.
- Implementation, tests, runtime/visual acceptance and internal self-play remain
  future work. Existing unrelated terrain metadata and documentation changes
  were preserved. No Trello edits, commit or push occurred.

## Next useful step

Review the two drafts, then schedule FD-I01 if accepted. Do not start gameplay
implementation merely because the planning documents exist. At execution
preparation, freeze actual scenario/build/catalog snapshots and 200 seed IDs.
