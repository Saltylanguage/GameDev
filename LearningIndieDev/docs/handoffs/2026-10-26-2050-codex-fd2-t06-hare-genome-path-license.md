# fd2-t06-hare-genome-path-license

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-26-2050-codex-fd2-t06-hare-genome-path-license
- Owner: codex
- Branch: codex/fd2-t01-integration
- Baseline commit: 2f77e4c
- Date: 2026-10-26
- Supersedes: none

## Summary

T06 adds the FD2 Hare Genome path license purchase. It charges the canonical
10 Hare Data once and saves the deduction with the existing license flag before
publication. Other wallets, research, Genomes and settlement state are preserved;
licensing grants no node effect. The implementation reuses schema v6 and the
existing atomic profile commit path.

## Changes

- Added `TryPurchaseHareGenomeLicense(profileId, out error)`. It validates the
  loaded profile, matching Hare wallet, existing ownership and transaction guard.
  It accepts no caller-supplied price or alternate wallet and returns errors for
  rejected/failed purchases without publishing a charged or unlocked snapshot.
- Reused `UnlockedGenomeSpeciesIds` and `WithGenomeEconomy`; no new schema,
  storage, package, asset or config is introduced. A saved license is the
  permanent purchase identity; duplicate and inherited ownership cannot pay twice.
- Failed and uncertain writes use T04's rollback/retry behavior. Profile switches,
  reentrant callbacks and insufficient/missing Hare Data cannot buy the license.
- Kept the incoming POC price/three-run method and UI behavior unchanged. T11
  later wires the FD2 operation. The FD2 purchase requires a loaded, funded profile
  and can be made after the first successful journey; it adds no three-run gate.
- Added 16 license cases and two PlayMode cases for real PlayerPrefs license
  persistence/no extra effects and an actual unlicensed journey launch.
- Corrected stale proposal/placeholder wording in the solution design to reflect
  the already-approved and authored T01 catalog.

## Decisions and assumptions

- Continue in the owned isolated worktree
  `C:\Users\bevin\.codex\worktrees\fd2-t01-integration\GameDev`, branch
  `codex/fd2-t01-integration`, baseline `2f77e4c`.
- The path license gives tree access only. It does not activate or unlock nodes;
  existing node state is preserved. First-slice price stays 10 Hare Data from
  the approved canonical definition, not a new tuning decision.
- The user authorized updating Trello as tickets finish. T06 was moved to
  In Progress before implementation, then marked complete and moved to Done
  after code commit `2e6fcd8` and validation. Verification comment
  `#comment-6adff66df4b9147e647dac68` was posted and read back; the original
  description is preserved. Proof: `artifacts/fd2-board-20261026/t06-done.png`.
- Visible Gene Lab controls remain T11; permanent node purchase is T07. Ordinary
  journeys remain available without a license or active Genome nodes.

## Validation

- Focused license EditMode: 16/16 passed, zero failures/skips at
  `artifacts/fd2-t06-20261026/license-editmode-fixed.xml`.
- Full clean EditMode: 599 total, 598 passed, zero failed, one parity-manifest
  skip at `artifacts/fd2-t06-20261026/all-editmode.xml`. All 64 profile transaction
  cases pass, including the tightened fresh-profile/first-success license case.
- Initial compilation caught two test-authoring errors: the negative collection
  assertion selected a string overload, and a callback parameter named `_`
  shadowed an out discard. Corrected those tests; no tests executed in that run.
  The failed compilation log remains at `license-editmode.log`.
- Portable Release build: zero warnings/errors. Existing deterministic runner
  self-test passed at `portable-self-test.log` in the same artifact folder.
- Distinct clean Editor processes 30308 (write) and 36724 (read) each passed
  `HareLicenseProcessReloadCheckpoint` once in `process-write.xml` and
  `process-read.xml`. The saved fixture crossed the process boundary; the reader
  retained ownership/zero Hare balance and rejected another purchase without
  another save or charge.
- Focused PlayMode: 6/6 passed, zero failures/skips at `focused-playmode.xml`.
  New cases verify real PlayerPrefs license/scene reload with no node effects,
  and a fresh unlicensed Main Menu profile reaching Desktop, entering the first
  simulation and advancing at least two ticks. Prior research, settlement and
  both migration cases also pass. This is a launch check, not journey/balance
  acceptance.
- Exact profile/recovery preference names, registry types and values match the
  pre-validation capture; their contents were not displayed or published.
- Source diff hygiene passed. Restored Unity's incidental ProBuilder line-ending
  change; no project setting, scene, prefab or serialized asset is changed.

## Risks and incomplete work

- The domain/persistence purchase is complete; the visible Gene Lab button still
  uses the incoming POC path until T11. Do not claim a completed FD2 player loop.
- No player build, human visual or balance acceptance is claimed. The historical
  seven full-PlayMode baseline failures remain open; focused checks do not
  establish a green full PlayMode suite.
- Failed/uncertain saves are tested; no OS power-loss durability guarantee is
  implied. Work remains local until explicitly published, with original active
  checkout source/Editor preserved.

## Next useful step

Continue with T07 permanent Hare node purchases using the canonical five-node
catalog: require the path license and owned prerequisites, charge 5 Hare Data,
unlock and activate together within eight capacity points, and persist atomically.
