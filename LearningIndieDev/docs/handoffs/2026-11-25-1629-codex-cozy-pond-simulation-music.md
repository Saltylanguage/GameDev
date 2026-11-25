# Cozy Pond Simulation Music

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-11-25-1629-codex-cozy-pond-simulation-music
- Owner: Codex
- Branch: codex/cozy-pond-audio
- Baseline commit: c216ddf4
- Date: 2026-11-25
- Supersedes: none

## Summary

The player rejected the spooky Calm Track pilot and selected a brighter CC0 track, called Cozy pond during auditions, for simulation music. The previous menu-to-simulation route had also been silent because the Desktop lacked an enabled AudioListener.

## Changes

- Imported `Assets/Resources/Audio/CozyPond.ogg` with its Unity metadata. Its source and license are recorded in [AUDIO_SOURCES.md](../AUDIO_SOURCES.md).
- The simulation audio bed plays CozyPond with the existing wind and UI cues.
- The Desktop host supplies one AudioListener and disables the temporary menu chime listener on scene arrival.
- PlayMode checks assert the imported clip is present and playing on the Main Menu to Rabbit journey route.

## Decisions and assumptions

- The player selected Cozy pond as the best audition. Sherwood was also liked but remains a backup outside the Unity project.
- This branch preserves the local playable state the player approved. It starts from `c216ddf4` and does not include eight later commits on `origin/codex/rabbit-identity-rename`, which also contain the rejected Calm Track pilot.

## Validation

- Unity script recompile: passed, no compilation errors.
- `SimulationAudioPaletteCreatesDistinctCuesAndTwoLoopingBeds`: PlayMode 1 passed, 0 failed.
- `RabbitJourneyCanLaunchWithoutAPathLicense`: PlayMode 1 passed, 0 failed; asserted CozyPond is playing in the running simulation.
- Source and documentation whitespace check passed; Unity-generated `.meta` files retain the importer's trailing spaces in empty YAML fields.

## Risks and incomplete work

- The automated checks confirm AudioSource playback, not the perceived volume or loop seam. The player's next Editor listen can assess those.
- Merging this branch with the newer Rabbit/M2 branch will require reconciling its changed launch flow and removing its Calm Track asset.

## Next useful step

Play the Main Menu -> Desktop -> Rabbit journey route in the Editor, then integrate this audio choice with the newer Rabbit/M2 work.
