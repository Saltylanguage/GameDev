# fd2-t02-migration-recovery-and-validation

[Working state](../WORKING_STATE.md) | Status: blocked

- Handoff schema: 1
- Handoff ID: 2026-10-24-2025-codex-fd2-t02-migration-recovery-and-validation
- Owner: codex
- Branch: BevBranch
- Baseline commit: 9f9ed11
- Date: 2026-10-24
- Supersedes: none

## Summary

Resumed the existing local FD2-T02 migration implementation. Recovery JSON is now
flushed before replacing the legacy store, and the compatibility Currency getter
projects ResearchData directly. Added six EditMode test cases covering repeated
v2 migration, preservation of existing recovery, all snapshot-copy paths, and
invalid/duplicate species wallets. Unity execution remains blocked by the open
Editor window titled "Recovering Scene Backups"; T02 is not ready to close.

## Changes

- `Helper_ProfileSession.Load` persists a new recovery copy before writing v3.
- `ProfileSessionSnapshot.Currency` returns `ResearchData`, avoiding duplicate state.
- `GenomeContractTests` adds migration replay with two profiles (with/without an
  existing recovery copy), fresh-profile defaults, eight snapshot-copy paths,
  defensive copying, read-only wallet access, and invalid/duplicate wallet cases.
- Preserved the earlier local wallet/schema code and two PlayMode tests, plus all
  unrelated terrain metadata, recovery scenes, and FD2 planning changes.

## Decisions and assumptions

- Scope is T02's save/model boundary. It does not consume Hare node effects or
  prices; T01 remains open for Salty's mechanics and effect review.
- A simulated replay restores the original v2 JSON between loads. This tests
  deterministic retry, not an actual process crash or disk-write fault.
- Existing legacy Currency callers remain compatible. Explicit purchase and
  settlement APIs belong to later tickets.

## Validation

- Baseline: `BevBranch`, `9f9ed11`; fetched origin and verified HEAD versus
  origin/BevBranch at 0/0. Work remains uncommitted and unpushed.
- Read live Trello T02: still in Upcoming Work, owner/reviewer Bevin, acceptance
  matches the local design. No external board mutation was performed.
- `CellSim.ps1 -Command Doctor`: healthy CLI/license; project Unreachable with
  its lock present. Evidence: `artifacts/unity-doctor-20261024-202346/`.
- The project Editor is responding, but its main window is "Recovering Scene
  Backups". The user was asked to finish recovery/startup or save and close.
- Existing runtime/test DLLs still date from October 20. No fresh Unity compile
  or Unity test execution is claimed for these changes.
- Attempted `CellSim.ps1 -Command Test -Mode EditMode -Execution Auto -TestFilter
  SaltyGame.Tests.GenomeContractTests`; exit 1 before tests because Auto could
  not choose a safe lane while the locked project is Unreachable.
- `dotnet run --project artifacts/fd2-t02-20261024/SnapshotCheck.csproj -c Release`:
  PASS, 30 assertions against linked current snapshot/species/Genome source.
  Covers model invariants only; does not execute JsonUtility or PlayerPrefs.
- Focused `git diff --check` passed for the changed profile and test files.

## Risks and incomplete work

- Unity serialization, backup/recovery execution, migration replay, and the two
  PlayMode tests remain unverified. No player build, visual or balance acceptance.
- Tests preserve and restore existing profile/recovery PlayerPrefs using the
  fixture's existing setup/teardown. Recovery scene choices remain with Bevin.

## Next useful step

Finish the Editor recovery prompt and wait for compilation. Run focused EditMode
`SaltyGame.Tests.GenomeContractTests` and PlayMode
`SaltyGame.PlayModeTests.MainMenuPlayModeTests` through CellSim. Inspect discovered
counts and XML; zero tests is not a pass. Close T02 only after fresh Unity results.
