# Simulation music pilot

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-11-25-1435-codex-simulation-music-pilot
- Owner: Codex
- Branch: codex/rabbit-m2-unification
- Baseline commit: 8ff51921
- Date: 2026-11-25
- Supersedes: none

## Summary

Prepared a provisional, low-volume simulation music loop on the verified Rabbit/M2 branch so the team can listen in the actual game before choosing final audio direction.

## Changes

- Imported pmiller's `calm_track-loop.ogg` as `Assets/Resources/Audio/CalmTrack.ogg` with its Unity `.meta` files and Streaming load setting.
- The simulation music bed loads `Audio/CalmTrack`; the existing generated melody remains the missing-asset fallback. Ambience and event cues are unchanged.
- Added a focused PlayMode assertion for the clip name and Streaming load type, and recorded the source and license in `docs/AUDIO_SOURCES.md`.

## Decisions and assumptions

- This is a pilot track, not approval of final music direction or a playlist.
- The source page lists pmiller as creator, CC0, and a loopable four-minute file. The imported OGG SHA-256 is `8C49A37FB9EE5994C919D0E43CF397DD990CB7F2C6C1200B9710A8FE942AD969`.
- The existing 0.12 music-bed volume remains unchanged.

## Validation

- `CellSim.ps1 -Command Test -Mode PlayMode -Execution Clean -TestFilter SaltyGame.PlayModeTests.GalapagOSVisualAcceptanceTests.SimulationAudioPaletteCreatesDistinctCuesAndTwoLoopingBeds`: 1 passed, 0 failed, 0 skipped on November 25. Retained result: `artifacts/unity-tests-20261125-143414/PlayMode-results.xml`.
- Compared the copied OGG hash against the source working tree and the recorded source note. The asset imports and `Resources.Load` resolves in the focused test.
- The sandboxed Doctor check could not reach the licensing client or Unity service; the elevated clean PlayMode run succeeded.

## Risks and incomplete work

- Human listening at normal and 4× play speed and judgment of the loop seam are pending.
- The focused test does not establish subjective fit or full-suite regression status. The last combined full-suite pass was on the branch before this audio pilot.

## Next useful step

Josh and Sim listen during a representative Forest Edge journey, check the seam, and decide whether this track fits or should be replaced.
