# fd2-t11-gene-lab-permanent-economy-ui

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-27-1522-codex-fd2-t11-gene-lab-permanent-economy-ui
- Owner: codex
- Branch: codex/fd2-t01-integration
- Baseline commit: 6956098
- Date: 2026-10-27
- Supersedes: none

## Summary

T11 wires the profile-route Gene Lab to the approved permanent Hare economy,
shared conversion research and saved replacement loadouts. The existing Noesis
window now explains balances, prices, prerequisites, ownership, active state,
capacity, save failures and retry. Work is isolated and remains local.

## Changes

- Added `FieldDataGenomeLab`, an explicit profile-owned presentation workflow.
  It projects saved snapshots and the canonical T01 definition into quotes and
  eligibility, delegates purchases/reset/application to T05-T08 APIs, and keeps
  draft selections separate from saved active nodes. No wallet arithmetic or
  persistence is added to XAML or ViewModels.
- The Desktop composition root explicitly selects permanent controls for a
  profile launch. Direct authoring-scene entry retains the existing POC preview.
  Existing refundable Hare allocations automatically retain a labeled legacy
  view and the original refund/reset transaction. After that legacy reset,
  closing/reopening selects permanent controls. Three existing POC scene
  regressions explicitly bind their legacy lane; their original economy and
  runtime assertions are retained.
- The permanent UI uses the five canonical Hare nodes despite POC catalog refresh.
  Research costs10/20/30 RD, the license10 Hare Data, nodes5 Hare Data and2
  points each, active capacity8, and each replacement reset20 RD with no refund.
  Costs and resulting wallet balances are displayed before available actions;
  disabled actions show funding/prerequisite/capacity/ownership explanations.
- Reset retains all ownership. Select/deselect owned nodes while the saved
  loadout is empty; apply the complete selection once for free. Over-capacity
  drafts are rejected, wallet/time refreshes retain a draft, and a changed reset
  generation/profile invalidates it. Applied selection uses canonical catalog
  order. All five first-slice effects are independent additive changes; no
  Mutation acquisition order is altered.
- Added10 domain cases and4 scene tests covering the actual bound Noesis
  controls, save failures/retry, profile ownership, reopen/reload and first-start
  freezing. No schema, package, authored tuning, scene or prefab change.

## Decisions and assumptions

- Approved FD2 ownership differs from the historical refundable POC allocation.
  Keep their transaction paths explicit. Permanent prerequisites require owned
  ancestors, so a replacement may omit an owned ancestor from its active nodes.
  No three-success gate is added to the FD2 license purchase.
- A draft is UI workflow state, not a second persistent Genome. Only a successful
  profile transaction changes active nodes or wallets. Error messages retain
  failed intent for retry. Changing the loaded profile cannot spend through an
  old Lab; the player must close/reopen it for the new owner.
- Upgrade concerns UPG-C01/C03/C04/C07 were checked: this ticket adds no effect
  target or runtime applier, preserves T09 immutable all-species launch capture,
  separates ownership from active state and uses explicit canonical order.
  No recorded Extreme trigger is introduced and no concern is waived.
- New workflow is plain C# presentation, consistent with the T10 seam. Existing
  `GenomeLabViewModel`/window types and MonoBehaviour composition remain in
  place. No discovery, singleton, global event bus or parallel save store.
- Work remains in the owned isolated checkout. Do not copy source to the
  original open project or push/merge without Bevin's authorization.

## Validation

- Unity6000.4.6f1 direct batch CLI targets the isolated project. The live MCP
  bridge targets the original project and reports ready/stopped; it was used
  read-only. The wrapper's global Editor guard still sees that unrelated Editor.
- Initial compile/domain smoke143/143 passed. New workflow9/9 passed at
  `artifacts/fd2-t11-20261027/lab-editmode.xml`; final full suite also includes
  the added stale-generation case. Full clean EditMode701 total:700 passed,
  zero failed, one existing parity-manifest skip at `full-editmode.xml`.
- Initial FD2 scene2/2 passed; expanded focused regression8/8 passed at
  `lab-regression.xml`, including all4 new FD2 cases and existing POC purchase,
  selection, population and repeated journey-reward checks.
- Initial graphics2/2 and expanded graphics4/4 passed. Render review caught
  a long save-error message below the scroll viewport; action status was pinned
  outside the scrolling controls. Also clarified that an owned node can be
  selected for free when the saved loadout is already empty. The corrected
  graphics4/4 passed (`lab-visual-scroll.xml`). Seven final1280x720 captures
  under `visuals-reviewed/` were inspected, including the full capacity reason
  after actual inspector scrolling. Save errors are pinned outside scrolling
  content; idle status no longer duplicates the per-action explanations.
  This is automated rendered review, not human visual acceptance.
- Final reviewed runtime source full EditMode also passes700/701 with zero
  failures and one existing parity skip (`full-editmode-reviewed.xml`).
- Broader related PlayMode29 total:25 passed,4 failed at
  `related-playmode.xml`. All T11 and selected T04-T10 integration cases pass.
  `GalapagOSDesktopAppsRemainOpenTogether` expects7 options but the POC catalog
  has9; population controls expect114 but get90. Both match retained incoming
  baseline `artifacts/fd2-integration/baseline-playmode.xml`. The older natural
  failure test reaches Shop and the success seed10100 fails at tick882; both
  match T10's exact pre-T10 comparison at
  `artifacts/fd2-t10-20261027/pre-t10-route-baseline.xml`. No failing test was
  disabled, and no required FD2 assertion was changed to accept POC behavior.
- Portable Release build and deterministic self-test passed with zero build
  warnings/errors (`portable-build.log`, `portable-self-test.log`).

- All4 wider failures were compared programmatically to retained messages;
  exact matches recorded in `baseline-comparison.json`. Original profile/recovery
  preference names/types/values and four protected source hashes are unchanged.
  Original Editor reports ready. Incidental ProBuilder formatting restored;
  diff hygiene passes. Handoff validation retains73 historical issues, none
  naming this handoff. An empty stale index lock was removed only after checking
  its exact worktree owner, age over15 minutes and absence of Git processes.
- Local code commit `f2a856535599fdc3e7bd03d7b666ad7e05c200a4`. T11 moved
  Upcoming Work -> In Progress -> Done and is marked complete. Original card
  description preserved; verification comment posted and visibly read back:
  https://trello.com/c/YJwJtQ2e/147-fd2-t11-wire-the-gene-lab-permanent-economy-ui#comment-6ae0ff1fafaa9b0d7ede7fef.
  Proof: `artifacts/fd2-board-20261027/t11-done.png`.

## Risks and incomplete work

- No full PlayMode green, player build, human visual acceptance, performance
  measurement or balance approval is claimed. Historical failures remain
  separate from this functional UI work.
- Direct authoring-scene entry intentionally uses the POC preview; permanent
  progression is reached through MainMenu Continue and its profile route.
- T12 still owns final new/migrated-profile end-to-end progression acceptance.

## Next useful step

T12: verify the complete new/migrated-profile permanent progression loop
without changing approved tuning. T11 is complete in live Done; work remains local.
