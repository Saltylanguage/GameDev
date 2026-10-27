# fd2-t08-reset-and-arrange-permanent-loadouts

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-26-2254-codex-fd2-t08-reset-and-arrange-permanent-loadouts
- Owner: codex
- Branch: codex/fd2-t01-integration
- Baseline commit: 961eae4
- Date: 2026-10-26
- Supersedes: none

## Summary

T08 adds a no-refund permanent Genome reset and free selection from the resulting
empty loadout. Bevin confirmed a new 20 RD reset is required for each replacement
loadout. The canonical Hare catalog validates ownership, ancestors and 8-point
capacity. Schema v7 persists reset generation so stale reset callbacks cannot
charge again after nodes have been reactivated.

## Changes

- Added `TryResetGenomeLoadout(profileId, speciesId, expectedResetGeneration)`.
  It atomically saves the 20 RD fee, cleared active IDs and incremented generation
  before publication. Empty reset, stale/mismatched profile/generation, missing
  license, unsupported species, POC allocations and insufficient RD are rejected.
- Added `TryArrangeGenomeLoadout` for one ordered complete selection from an
  empty loadout. It saves only owned, supported nodes with owned ancestors and
  catalog capacity <=8. Selection is free; ownership and species wallets never
  change. A different/reordered/empty selection after application requires reset.
  Repeating an identical valid selection is a no-op without saving or charging.
- Added `SpeciesGenomeProfile.ResetGeneration`; all persistent profile-copy and
  existing POC/configuration paths retain it. The generation is not a gameplay
  modifier and does not enter the effective-rule fingerprint.
- Bumped profile store to v7. Pre-v7 Genome records start at generation zero
  without changing old ownership, selections, wallets, research or settlements.
  Invalid v7 Genome records are preserved/rejected rather than losing reset
  duplicate protection by dropping a record. Updated migration version assertions
  and the T07 legacy fixture to include current-version generation metadata.
- Added 36 T08 EditMode cases and one PlayMode case. No new assets, metadata,
  scenes, packages or configuration are introduced. Existing POC APIs remain
  separate; T11 must use the FD2 APIs instead of trusted legacy setters.

## Decisions and assumptions

- Bevin explicitly chose "Charge 20 RD for each replacement loadout" on
  2026-10-26. The tuning sheet, solution design and ticket record carry this
  clarification. Permanent free rearrangement after the first reset is excluded.
- A caller edits a local draft then applies one full selection from empty.
  Empty-to-empty selection is a no-op and does not consume the choice; no new
  editing-mode flag is needed. Reset of an already-empty loadout is rejected
  without a fee, consistent with the solution design's no-op guidance.
- The first catalog supports Hare only; unsupported species are rejected and
  other species' Genome state stays intact. Refundable POC allocations are
  preserved/rejected, not silently converted into permanent purchases.
- The saved generation identifies a reset request. Retrying must retain the
  original generation; UI code must not refresh it on a repeated callback.
- T09 owns the next-launch snapshot seam, and T11 owns visible Lab controls.
  Work remains local in the isolated branch; no push or active-source integration.

## Validation

- Direct Unity 6000.4.6f1 batch CLI targeted the isolated project while the
  original Editor remained available. No Editor was stopped or lock deleted.
- Initial reset-only EditMode: 18 cases, 15 passed/3 failed. The failures were
  test assumptions about the existing recovery helper moving unreadable data
  out of the primary slot when recovery is free. Corrected the expectations and
  added occupied-slot coverage; no production recovery behavior was weakened.
  Retained `artifacts/fd2-t08-20261026/reset-editmode.xml` and matching log.
- Focused reset/arrangement: 32/32 passed at `loadout-editmode.xml` in that folder.
  Four additional POC/corruption cases were then included in the final full run.
- Full clean EditMode: 667 total, 666 passed, zero failed, one existing
  parity-manifest skip at `all-editmode.xml`. All36 new T08 cases and all132
  profile transaction cases pass. Coverage includes exact fee/fund boundaries,
  empty/no-refund behavior, per-replacement charging, duplicate/stale callbacks,
  owned inactive ancestors, capacity/locked/unknown/duplicate selections, save
  failure and uncertain restart, v6 migration/v7 recovery, other-species and
  research/settlement preservation.
- Portable Release build passed with zero warnings/errors; deterministic runner
  self-test passed at `portable-self-test.log` in the same artifact folder.
- Independent clean Editor writer PID11248 and reader PID14764 each passed
  `HareResetAndArrangementProcessReloadCheckpoint` once. The writer saves the paid
  empty loadout; a new process rejects the stale reset and freely arranges owned
  terminal nodes without another fee. Artifacts: `process-write.xml`,
  `process-read.xml`, `process-loadout-store.json` in the same artifact folder.
- Focused PlayMode: 8/8 passed, zero failed/skipped at `focused-playmode.xml`.
  The new real PlayerPrefs scene test purchases all five nodes across resets,
  reloads both the paid empty state and final four-node replacement, checks
  inactive-owned prerequisites/effects and rejects stale/duplicate charges.
  Prior node/license/research/settlement/launch/migration checks also pass.
- Exact original profile/recovery preference names, registry types and values
  match the protected capture. All four original source hashes are unchanged.
  Original Editor reports ready through the CLI. Diff hygiene passes; Unity's
  incidental ProBuilder line-ending change was restored, with no setting change.
- Code commit `d90c2f5` remains local. Live T08 moved Upcoming Work -> In Progress
  -> Done and is marked complete. The original description is unchanged, and
  the verification comment was posted and visibly read back:
  https://trello.com/c/TZe1J7oQ/144-fd2-t08-reset-and-arrange-an-active-genome-loadout#comment-6ae013d687320fb2409a962e.
  Screenshot: `artifacts/fd2-board-20261026/t08-done.png`. T01-T07 remain Done.
  Counts/source hashes are retained in
  `artifacts/fd2-t08-20261026/validation-summary.json`.
- Repository handoff validation still reports 73 historical journal issues;
  none names the new T08 handoff. Diagnostic: `handoff-validation.log` in the
  same artifact folder. No repository-wide documentation-validation pass claimed.

## Risks and incomplete work

- No player build, full PlayMode rerun, human visual or balance acceptance is
  established. The seven historical incoming PlayMode failures remain open.
- New FD2 reset/arrangement rejects POC refundable allocations; their old UI
  still uses its original semantics until T11. Do not infer a completed player
  progression loop or use trusted legacy setters as FD2 purchase controls.
- Save failure/retry tests are not an OS power-loss durability guarantee.
- No source integration into the open original checkout or push occurred.

## Next useful step

Continue with T09: freeze the current validated FD2 active Genome configuration
into the next launch for every relevant population of the species; profile
changes must not alter a running simulation. Use owned prerequisites, the full
approved catalog effects and fingerprinted immutable snapshots.
