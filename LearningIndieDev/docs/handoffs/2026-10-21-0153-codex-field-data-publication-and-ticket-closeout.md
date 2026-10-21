# Field Data publication and ticket closeout

[Working state](../WORKING_STATE.md) | Status: shared

- Handoff schema: 1
- Handoff ID: 2026-10-21-0153-codex-field-data-publication-and-ticket-closeout
- Owner: codex
- Branch: BevBranch
- Baseline commit: dc4f6c5
- Date: 2026-10-21
- Supersedes: none

## Summary

Bevin accepted the initial five-node Field Data journey and confirmed APS fixed,
then explicitly requested publication and closure of all FD tickets. Accepted
implementation is shared on BevBranch and ProjectMain. A live Trello audit
verified all 16 cards in Done with complete flags and 79 resolved current criteria.
Sprint 5 remains Active for other work; WTR-01 remains In Progress.

## Changes

- Commit `e6cadcd`: accepted FD implementation, authored route/assets, portable
  runner/report support, focused tests and accumulated design/evidence records
  (97 paths). Includes the approved 20-Fox opener and separate ADD/REM accounting.
- Merge `dc4f6c510cb6342f92576e7f597631087f281d4f` incorporates incoming ProjectMain
  `6df406c37504249e37f7a1a5259649364b08791a`. Three documentation conflicts were
  resolved by retaining the later acceptance plus Salty's dated planning/capacity
  history. No code conflict. Both remote branches were verified at this merge
  before the documentation-only closeout commit containing this handoff.
- T13 and T16 moved from In Progress to Done and marked complete. T09's unset
  completion flag was corrected. T06/T08/T11 criteria now reflect accepted
  control windows, shop-only income and repeatable approved Hare skills.
- T14/T15 comments link the October 19 refresh and explicitly distinguish it
  from the later APS fix. Owners/reviewers remain unassigned as requested.
- Current repo status/index/self-play/sprint records now distinguish publication
  from earlier local checkpoints. Raw experiment artifacts remain unchanged.

Live readbacks:

- [T13 acceptance](https://trello.com/c/FGFDVXcQ/131-fd-t13-author-the-test-route-and-verify-the-complete-desktop-flow#comment-6ad8517de8c380f558bf762e).
- [T16 decision and deferrals](https://trello.com/c/osmzcZVf/134-fd-t16-review-internal-self-play-and-record-accept-revise#comment-6ad8527f794bb2a3e26f1825).
- [Sprint checkpoint](https://trello.com/c/hqQczLQ9/135-sprint-5-control-record-field-data-and-playability#comment-6ad853274498e3ff589854a2).

## Decisions and assumptions

- ACCEPT the first slice, based on Bevin's completed 1,800-tick journey, reported
  near population misses and October 21 explicit acceptance/APS confirmation.
  The later failed seed10100/tick1168 screenshot is separate evidence.
- T16 originally requested deliberation times and matched human tradeoff notes;
  these were not collected. Its revised checklist explicitly defers them for
  this accepted closeout, rather than asserting they were performed. Conditional
  economy-revision replay is not applicable because no revision was requested.
  Next content remains undecided. No numeric ecology acceptance gate was added.
- Final rules include shop-only income modifiers, ordinary Hare free skills up
  to level 10, no Hare generic Attack, controls only after local rounds 1–5,
  no controls at node settlement, and deliberate map entry between all nodes.
- Keep unrelated terrain importer edits and Unity recovery scene local. Their
  13 file hashes are retained in the publication scope manifest; no reset/stash
  or unrelated publication is used to produce an apparently clean worktree.

## Validation

- Repeated this publication session: portable Release build, zero warnings/errors;
  portable self-test PASS, including the full 18-round journey and deterministic
  wallet/route behavior. Documentation reconciliation made no code changes.
- Latest accepted-code Unity evidence remains dated October 20: 159 unique
  focused EditMode passes; 4 Field Data PlayMode passes with 2 graphics skips.
  See the [APS fix handoff](2026-10-20-0007-codex-field-data-human-playthrough-and-aps-accounting-fix.md).
- October 19: eight authored action fixtures pass bounded Unity/headless parity
  (1/1 dedicated test, zero skips); 600/600 reports across seeds10100–10299 and
  three v3 policies validate. These predate the APS fix; no fresh post-fix cohort
  or parity export is claimed. See the [refresh handoff](2026-10-19-1601-codex-field-data-current-parity-and-matched-cohort-refresh.md).
- Live per-card audit: 16 Done, 16 complete, all 79 current criteria resolved.
  Local readback: `artifacts/fd-publication-20261021/trello-final-audit.json`.
  Sprint/WTR readback and screenshot are in the same directory. Snapshot
  manifests and branch-ref verification are also retained there (ignored artifacts).
- Unity doctor reports the Editor healthy and in Play Mode. This publication
  preserves that open run; no new Unity tests or player build were run this session.

## Risks and incomplete work

- First playable baseline only: more content, art/polish, longer-term economy,
  broader species effects and future balance tuning remain separate work.
- Timing/matched-human notes remain uncollected. Future multi-value tuning
  should retain matched seed comparisons without isolated-causality claims.
- Working tree intentionally retains unrelated metadata/recovery files; this
  does not mean accepted FD code remains unpublished.

## Next useful step

Plan the next bounded gameplay/content improvement from actual team priorities.
Do not reopen completed FD cards merely to absorb unrelated Sprint 5 work.
