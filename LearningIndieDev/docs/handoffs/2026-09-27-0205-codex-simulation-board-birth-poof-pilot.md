# Simulation Board Birth Poof Pilot

[Working state](../WORKING_STATE.md) | Status: in-progress

- Handoff schema: 1
- Handoff ID: 2026-09-27-0205-codex-simulation-board-birth-poof-pilot
- Owner: Codex
- Branch: codex/simulation-board-birth-pilot
- Baseline commit: 8d5cdc37
- Date: 2026-09-27
- Supersedes: none

## Summary

Started the first simulation-board visual polish pilot by extending the existing
positional birth cue with a small pixel poof behind the newborn. The existing
heart and sparkle remain. This is presentation-only; simulation rules and event
records are unchanged.

## Changes

- `SpeciesSimulationBoard` draws a two-tone pixel cloud at the newborn cell
  before drawing its sprite, so the poof reads as a brief arrival effect.
- The poof shares the existing mating/birth cue's timing and fades within its
  first 0.58 seconds.
- Birth cue animation now freezes during the preview's Paused state in both the
  prototype and GalapagOS hosts.
- The visual exploration note records pilot status and remaining gaps.

## Decisions and assumptions

- Keep this first pass code-drawn in the existing batched Noesis board. Replace
  the provisional pixel treatment with reviewed production art only after a
  live visual review.
- Reuse the current birth-event coordinates; do not infer births from changed
  board snapshots.
- Do not add a placeholder sound. The active project has no simulation audio
  assets or mixer path. Sound design and an authored clip remain the next part
  of the event pilot.
- The current ViewModel exposes one newest birth at a time. A full litter cue
  needs a follow-up presentation design and is not implemented here.

## Validation

- `git diff --check` passed. Git reported only line-ending normalization
  warnings for the edited files.
- No Unity compilation, automated tests, or live visual capture has run for this
  pilot. Treat runtime behavior and appearance as unverified.

## Risks and incomplete work

- The cloud may obscure a small newborn or read as snow/smoke at the minimum
  board scale; inspect it in the actual view at 1280×720 and 1920×1080.
- The cue still represents one newborn even when a litter places several
  children in one tick.
- The sound cue, authoring review, and runtime verification remain open.

## Next useful step

Open the Forest Edge simulation at 1280×720 and 1920×1080, capture the birth
cue at normal zoom and 4× simulation speed, and adjust its silhouette, opacity,
or duration from what is visible. Then review whether a single sound should
represent the litter and add an authored clip through the project's audio path.
