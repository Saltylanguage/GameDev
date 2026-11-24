# Unity lock recovery and Rabbit integration verification

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-11-23-1511-codex-unity-lock-recovery-and-rabbit-integration-verification
- Owner: Codex
- Branch: codex/rabbit-identity-rename
- Baseline commit: c99b1141
- Date: 2026-11-24
- Supersedes: 2026-11-21-codex-rabbit-projectmain-integration

## Summary

Recovered the Unity project from a stale lock and ran the previously blocked Rabbit/ProjectMain integration checks. The merge exposed an irregular-map population count bug, a Desktop Fox genome reference to a folder rather than its asset, and test fixtures that still assumed older map and Rabbit values. The current branch now imports and runs both Unity test modes. This is automated integration evidence, not player review or balance acceptance.

## Changes

- Confirmed no Unity Editor owned the November 20 lock, then removed only `LearningIndieDev/Temp/UnityLockfile`. `CellSim.ps1 Doctor` reported the project closed and unlocked.
- Count species only on playable topology cells so irregular-map gaps are never read. Population intervention planning also skips those gaps.
- Repointed the Desktop test scene's Fox genome reference to the existing `FoxGenome.asset` GUID, and removed an obsolete test-only genome reference from the Rabbit POC asset. Existing `.meta` GUIDs are preserved.
- Updated simulation preview labels and Unity fixtures to the authored 34×19 Forest Edge, 3-Rabbit start, 15-Rabbit ceiling, and scenario-specific wrapping. The seven fixed-shape checks verify the ceiling on boundary controls. The older Genome fixture now checks that refills stop at 15; the Field Data route still exercises removal then addition below the ceiling.

## Decisions and assumptions

- The November 19 S6 control record explicitly says the 15-Rabbit ceiling includes purchases, hidden occupants, and pending litters. That newer rule governs the current test pass; the older population pool of 50 remains a separate allowance and does not override the species ceiling.
- The S4 developer fixture is labeled "S4-style" because the authored 15-Rabbit ceiling prevents its historical 20-Rabbit starting count. Historical experiment evidence is unchanged.

## Validation

- `CellSim.ps1 Test -Mode All -Execution Clean` passed on the final source: **830 passed / 0 failed / 1 skipped EditMode** and **106 passed / 0 failed / 7 skipped PlayMode**. XML and logs: `LearningIndieDev/artifacts/unity-tests-20261124-102535/` (local, untracked).
- Focused `AuthoredFixedShapeControlsGeometryWrappingPlacementAndBoardCells` passed **7/7** in `LearningIndieDev/artifacts/unity-tests-20261124-102501/`.
- `dotnet build tools/CellSim.Batch/CellSim.Batch.csproj -c Release --no-restore` passed with zero warnings and errors. `dotnet exec --roll-forward Major tools/CellSim.Batch/bin/Release/net8.0/CellSim.Batch.dll self-test` passed, including portable shape/seam and deterministic journey replay checks. The machine has .NET 9/10 but not the targeted .NET 8 runtime, so the self-test used runtime roll-forward.
- Post-run `CellSim.ps1 Doctor` passed: healthy, project closed, no lock file. Evidence: `LearningIndieDev/artifacts/unity-doctor-20261124-103402/` (local, untracked).
- `git diff --check` passed.

## Risks and incomplete work

- Unity's skipped tests and in-game visual/player review remain outside this automated check.
- The Field Data add button can appear available when the species ceiling is full; the transaction rejects the purchase without charging. UI availability should reflect the cap in a later polish pass.
- Cozy simulation feel, water continuity and all-map wrapping policy, non-Rabbit end-to-end play, and final balance/M2 acceptance remain open.

## Next useful step

Review Forest Edge and the irregular layouts in the Editor, then run a non-Rabbit sandbox route in-game. Josh and Sim should review the 15-Rabbit purchase ceiling with the player experience before final balance or card closeout.
