# ProjectMain Rabbit M2 Cozy Pond integration

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-11-25-2052-codex-projectmain-rabbit-m2-cozy-pond-integration
- Owner: Codex
- Branch: codex/cozy-pond-audio
- Baseline commit: 04d8da9a
- Date: 2026-11-25
- Supersedes: none

## Summary

Integrated the published Rabbit/M2 branch with the newer, player-selected Cozy Pond audio branch. The candidate merge is `04d8da9a` (parents `2a13966e` and `6e151f27`), based on `origin/ProjectMain` at `663b5c2a`. This note records the verification used before advancing and publishing `ProjectMain` from the main worktree.

## Changes

- Retained the Rabbit identity, fixed/irregular map, M2 species/scenario selection, launch-flow, and review work from the Rabbit/M2 branch.
- Retained Cozy Pond playback at the selected branch's 0.32 music-bed volume, its Desktop AudioListener correction, audio source record, and menu-to-journey test.
- Resolved four textual conflicts in the audio test, music-bed code, audio sources, and working-state index. Removed the rejected Calm Track asset and importer metadata from the final tree; its earlier branch commit and handoff remain as history.
- Kept the pre-existing untracked test scene and local artifact directories in `D:/GameDev/GameDev` untouched.

## Decisions and assumptions

- Josh's later Cozy Pond selection supersedes the earlier Calm Track pilot. No other ecology, water-topology, or M2 acceptance decision was made by the merge.
- Scope is the active, published Rabbit/M2 and Cozy Pond branches. Older procedural-water and S6 notebook prototypes remain separate pending their own decisions.
- The selected Cozy Pond Unity `.meta` GUID is preserved. Both branches used the same Audio folder GUID, so the folder metadata required no rewrite.

## Validation

- `CellSim.ps1 -Command Test -Mode All -Execution Clean` in `D:/GameDev/CrossAgent/rabbit-m2-integration-20261124/LearningIndieDev` at `04d8da9a`: EditMode 831 passed / 0 failed / 1 skipped (832 total); PlayMode 110 passed / 0 failed / 7 skipped (117 total). Results: `artifacts/unity-tests-20261125-204551/EditMode-results.xml` and `PlayMode-results.xml` in that worktree.
- The XML records passes for `SimulationAudioPaletteCreatesDistinctCuesAndTwoLoopingBeds`, `RabbitJourneyCanLaunchWithoutAPathLicense`, `DesktopHasAnAudioListenerAfterTheIntroChime`, `PlannerLaunchStartsTheSelectedSandboxSpecies`, and `PlannerLaunchRunsFoxSandboxForTenTicks`.
- The isolated pre/post Doctor checks reported healthy licensing and a closed, unlocked project.
- The first main-worktree Live run used a stale test assembly (830 EditMode and 110 PlayMode passes, but omitted new tests). After refresh, EditMode passed 831/832; that Live PlayMode run was aborted when Play Mode stopped, then cancelled. Neither Live PlayMode result is the acceptance evidence above.

## Risks and incomplete work

- M2 player-route acceptance remains open. The earlier authored Rabbit seed-10100 route without Mutation purchases failed at tick 1712; player feel, water continuity, and a full non-Rabbit journey remain unproven.
- Cozy Pond passed playback checks, but normal/4× listening, volume judgment, and loop-seam review remain for Josh and Sim.
- The clean suite validates the combined code and assets. It does not prove the old prototype branches are ready for ProjectMain.

## Next useful step

Fast-forward the local `ProjectMain` branch to the validated integration plus this handoff, publish it normally, and update the related Trello milestone/audio cards. Keep the older prototypes separate until their design decisions are made.
