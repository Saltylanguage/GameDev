# fd2-t07-permanent-hare-node-purchases

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-26-2109-codex-fd2-t07-permanent-hare-node-purchases
- Owner: codex
- Branch: codex/fd2-t01-integration
- Baseline commit: fe74c66
- Date: 2026-10-26
- Supersedes: none

## Summary

T07 adds permanent Hare Genome node purchases using the approved T01 catalog.
One saved transaction charges 5 Hare Data, adds permanent ownership and activates
the node. The full proposed loadout must have all owned ancestors and fit the
8-point catalog capacity. Existing v6 storage and atomic profile commits suffice.

## Changes

- Added `TryPurchaseHareGenomeNode(profileId, nodeId, out error)` to the existing
  profile session. Profile identity, transaction guard, path license and a known
  Hare node are required. Price and capacity cannot be supplied by a caller.
- Appended owned/active IDs in acquisition order; saved fee, ownership and
  activation together before notification. No refundable POC allocation is added.
- Used `FreezeOwnedLoadout` to validate all owned ancestors, supported effects
  and full catalog capacity, including already active nodes. Owned prerequisites
  may be inactive. Existing ownership/POC allocations cannot be repurchased.
- Added 32 T07 persistence/domain cases and one real PlayerPrefs/scene-reload
  PlayMode case. Updated the solution design and ticket implementation record.

## Decisions and assumptions

- Scope is domain/persistence; T08 owns reset/free arrangement, T09 next-launch
  integration and T11 visible Gene Lab controls. Approved effects/tuning, asset
  GUIDs, scenes, packages, configuration and the existing POC APIs are unchanged.
- Permanent purchases use `unlockedNodeIds` plus `activeNodeIds`, not refundable
  allocation records. FD2 capacity must be computed from the catalog; the POC's
  `SpeciesGenomeProfile.CapacityUsed` fallback counts legacy IDs as one point.
- Unknown/incompatible legacy active state is retained and rejected, not migrated
  or silently reinterpreted. No save-schema bump beyond v6 is necessary.
- Work is isolated on `codex/fd2-t01-integration`; original active checkout source
  is preserved. No push or integration into the open checkout is authorized.

## Validation

- Direct Unity 6000.4.6f1 batch CLI targeted the isolated project. The shared
  wrapper's global open-Editor guard would reject the separate original Editor;
  no lock was deleted and no existing Editor was stopped.
- Initial focused EditMode: 30/30 T07 cases passed at
  `artifacts/fd2-t07-20261026/node-editmode.xml`. Two subsequent legacy-state
  regression cases were added and included in the final full suite.
- Full clean EditMode: 631 total, 630 passed, zero failed, one existing
  parity-manifest skip at `artifacts/fd2-t07-20261026/all-editmode.xml`.
  All 32 T07 cases and all 96 profile transaction cases pass. Cases include exact
  0/4/5/6/long-max funds, every node/effect, inactive owned prerequisites,
  missing ancestors/license/wallet, unsupported IDs, duplicates, eight-point
  boundary, reentry, failed/uncertain saves, restart/retry and preservation of
  research, other species, receipts and pending success.
- Independent clean Editor writer PID35648 and reader PID14888 each passed the
  saved-fixture `HareNodeProcessReloadCheckpoint` once. Ownership, activation and
  zero balance survived; duplicate purchase made no additional write/charge.
  Artifacts: `process-write.xml`, `process-read.xml`, `process-node-store.json`
  under the same artifact folder.
- Portable Release build passed with zero warnings/errors; the existing runner
  self-test passed at `artifacts/fd2-t07-20261026/portable-self-test.log`.
- Focused PlayMode: 7/7 passed, zero failed/skipped at `focused-playmode.xml`.
  The new real PlayerPrefs scene test buys the license and four nodes, reloads,
  verifies all permanent/active IDs and exact natural-rule additions, and rejects
  a duplicate and fifth node without a further charge. Prior license, research,
  settlement, unlicensed journey launch and both migration checks also pass.
- Exact original profile/recovery preference names, registry types and values
  match the protected pre-validation capture. All four original source hashes
  still match their earlier checkpoint. Their contents were not published.
- Original Editor remains ready with Play Mode stopped. Diff hygiene passes;
  restored Unity's incidental ProBuilder line-ending rewrite. No setting change.
- Revision hashes and counts are retained in `validation-summary.json` under
  the same artifact directory. Live T07 was moved Upcoming Work -> In Progress;
  completion comment/Done closeout follows the local implementation commit.

## Risks and incomplete work

- The new API is complete independently of the visible POC Gene Lab, which still
  uses its existing purchase/reset flow until T11. Do not claim a complete player
  loop or use POC capacity/refund semantics for FD2.
- No player build, full PlayMode rerun, human visual or balance acceptance is
  established. The seven historical incoming PlayMode failures remain open.
- Failed and uncertain saves are tested; this does not establish OS power-loss
  durability. No external publishing or original-source integration occurred.

## Next useful step

Continue with T08: reset one species' active selection for the canonical
20 Research Data fee, refund no Species Data, retain all permanent ownership,
then arrange owned nodes freely within catalog capacity. Reject an already-empty
reset to avoid charging for a no-op. Preserve the owned-prerequisite rule and
other profile state.
