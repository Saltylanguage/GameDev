# rabbit genome profile economy foundation

[Working state](../WORKING_STATE.md) | Status: in-progress

- Handoff schema: 1
- Handoff ID: 2026-10-17-1025-codex-rabbit-genome-profile-economy-foundation
- Owner: Codex
- Branch: ProjectMain
- Baseline commit: 6df406c3
- Date: 2026-10-17
- Supersedes: none

## Summary

Implemented the saved Rabbit Genome economy boundary approved in P1-029. This
lets a profile hold distinct Rabbit Data and Research Data, qualify for Gene Lab
after three unique completed runs, purchase permanent access to the Rabbit
tree, buy active nodes with an eight-point limit, and reset those purchases with
an exact Rabbit Data refund and Research Data fee. The first production node and
its balance values have not been authored.

## Changes

- Profile store v3 adds the two balances, completed-run IDs/count, and per-node
  allocation receipts (Rabbit Data spent and capacity points). Older stores
  still load with zero new balances, and legacy permanent node IDs stay intact.
- Rabbit purchase methods validate tree ownership, duplicate selection,
  catalog species, active prerequisites, capacity, and funds before one profile
  save. Reset clears active selections, preserves the tree license and legacy
  unlocks, refunds receipt totals, and spends the supplied Research Data fee.
- The Gene Lab view model now receives the loaded profile on desktop launch and
  displays capacity points used instead of counting selected nodes.

## Decisions and assumptions

- The accepted P1-029 contract is authoritative: a whole-tree permanent license
  plus resettable active purchases. No node price, reset fee, or capacity cost
  was chosen here; the purchase methods receive those values from future
  authoring/UI composition.
- Existing `UnlockedNodeIds` are preserved as legacy entitlements, including
  on reset. New purchased nodes live in allocation receipts and active IDs.
- `RecordCompletedRun(completionId)` requires a stable unique ID so repeated
  settlement callbacks cannot advance the three-run gate twice.

## Validation

- Clean Unity EditMode `SaltyGame.Tests.GenomeContractTests`: 13/13 passed,
  including purchase, refund, save/reload, duplicate completion, and v2
  migration cases. Evidence:
  `artifacts/unity-tests-20261017-102655/EditMode-results.xml`.
- Clean Unity PlayMode
  `SaltyGame.PlayModeTests.LabPlayModeTests.GalapagOSDesktopAppsRemainOpenTogether`:
  1/1 passed. Evidence:
  `artifacts/unity-tests-20261017-102457/PlayMode-results.xml`.
- An earlier focused EditMode run failed 2 newly added tests because their
  EditMode setup omitted `Reload()` after `AddComponent`. That setup was fixed;
  the same fixture passed on rerun.

## Risks and incomplete work

- The journey result flow has no completed-run settlement hook yet. It must call
  `RecordCompletedRun` once per unique completed journey; individual simulation
  completion events are not equivalent to a completed journey.
- The Gene Lab screen is still a read-only catalog preview. Its purchase/reset
  commands, first-selection tutorial, prices, and affordance states remain.
- No production Rabbit node asset exists. Fertile Droppings' effect, scent rule,
  seed chance, and balance values need design before runtime work.
- The authored catalog key is `hare`, while some older simulation code uses
  `herbivore`; the effect integration must map these deliberately before an
  active node can affect live Hares. Current snapshots only carry IDs.
- Legacy free `UnlockGenome` and direct `TrySetGenomeConfiguration` remain for
  existing callers/tests; the latter rejects overwriting paid allocations.
  Production UI must use the purchase/reset methods.

## Next useful step

Design and author Fertile Droppings' first offer (price and capacity points)
and its seed/scent effect contract, then bind Gene Lab actions to the approved
offer values. Wire unique journey completion settlement separately when that
flow is implemented.
