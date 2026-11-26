# Rabbit M2 merge and route review

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-11-24-1747-codex-rabbit-m2-merge-and-route-review
- Owner: Codex
- Branch: codex/rabbit-identity-rename (prepared in isolated `codex/rabbit-m2-unification` worktree)
- Baseline commit: c216ddf4
- Date: 2026-11-24
- Supersedes: 2026-11-23-1511-codex-unity-lock-recovery-and-rabbit-integration-verification; 2026-11-24-1105-codex-rabbit-and-m2-integration

## Summary

Merged the clean Rabbit/ProjectMain Unity verification commit `c216ddf4` with Bev's published M2 integration `df27bbe8` in an isolated worktree, preserving unrelated music edits in the regular checkout. The combined source passes both clean Unity suites. Focused visual checks show a working Forest Edge board, a detached-island seam crossing, and a Fox-selected test sandbox through tick 10. The authored Rabbit Field Data visual route, with no Mutation choices, fails at tick 1712; this is route evidence, not a balance decision.

## Changes

- Resolved three merge conflicts. `CountSpeciesPopulations` iterates playable topology cells; the fixed-shape test counts creatures and plant resources correctly. Separate preview tests cover authored wrapping and bounded fallback without a scenario. Both November 24 working-state entries are retained.
- Added a focused Fox planner sandbox test that advances ten paused simulation ticks and captures the game view. Updated the Rabbit visual-route fixture to leave the new species-selection screen before its captures.
- No production scenario, species rule, balance value, serialized asset GUID, or other contributor's uncommitted music file was changed in this worktree.

## Decisions and assumptions

- The regular checkout had new uncommitted music work when the remote branch advanced. All merge, test, and visual work was done in `D:/GameDev/CrossAgent/rabbit-m2-integration-20261124` to avoid touching it.
- Forest Edge remains the authored 34×19 wrapped Rabbit route with a hard 15-Rabbit ceiling. The detached-island visual test uses an in-memory irregular-layout clone, not a new production scenario.
- Automated route checks do not substitute for Josh and Sim's player-feel or balance approval.

## Validation

- Combined `CellSim.ps1 Test -Mode All -Execution Clean`: **831 passed / 0 failed / 1 skipped EditMode**, **109 passed / 0 failed / 7 skipped PlayMode**. XML: `LearningIndieDev/artifacts/unity-tests-20261124-173236/` (local ignored artifacts).
- `dotnet build tools/CellSim.Batch/CellSim.Batch.csproj -c Release` passed with no compilation errors; NuGet vulnerability lookup was unavailable (NU1900). Batch self-test passed with .NET major roll-forward, including shape/seam serialization and journey replay.
- 1920×1080 focused visual checks passed: Fox sandbox to tick 10 (`visual-evidence-20261124-174401`, 1/1), detached island and same-creature row seam (`visual-evidence-20261124-174456`, 1/1), and Desktop/Forest Edge Simulation captures (`visual-evidence-20261124-174553`, 1/1). Screenshots were opened and inspected, not inferred from test status alone.
- The Rabbit Field Data visual route was run twice. The first run (`visual-evidence-20261124-174716`) revealed stale capture navigation: the new selection overlay hid the simulation. After fixing the fixture, the second run (`visual-evidence-20261124-174918`) captured the actual map, board, round decision, shops, and failure screen. It **failed** at tick 1712 before round 18. The retained population CSV shows Rabbits 3 at tick 0, 15 at tick 600, 1 at tick 1200, and 0 at tick 1712; Plants were 641 and Foxes 2 at failure. The run records 19 starvation and 4 combat Rabbit deaths and 77 Field Data lost.
- Post-run `CellSim.ps1 Doctor` passed: healthy, project closed and unlocked (`unity-doctor-20261124-175217`). `git diff --check` passed.

## Risks and incomplete work

- The authored seed-10100 no-Mutation Field Data route is not a successful full journey. A different player choice could change the result; no alternate route or human player assessment was performed.
- The Fox sandbox screenshot shows `Player species: FOX` beside a hard-coded Rabbit portrait. The normal expedition selector still lists Fox as locked; the test sandbox is separate. The population meter denominator `25` is a role-display fallback, not the 15-Rabbit cap, but may be read as a cap. Small animals are difficult to pick out on the 1920×1080 full-map view. These are review findings, not new acceptance decisions.
- The detached-island capture confirms a visible gap and a tracked Rabbit (`#45`) moving from cell (15,17) to (38,17) on the same landmass seam. It contains no ponds or rivers, so water continuity and the all-map edge rule remain unresolved.
- Raw screenshots, CSV and XML evidence are under the ignored local artifact directories above; they are not part of the Git commit.

## Next useful step

Review the valid Rabbit failure screenshot and decide which choices a representative player should try next. Give VS-A7 a full Fox player journey and resolve the hard-coded portrait before considering that card complete. Josh and Sim should judge animal legibility and the 15-Rabbit experience. Resolve the water/topology contract with WTR-01 before VS-A1 closeout.
