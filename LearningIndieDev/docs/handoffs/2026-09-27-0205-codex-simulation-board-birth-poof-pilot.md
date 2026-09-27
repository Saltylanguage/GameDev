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
- No automated tests were run.
- A manual live review caught a real Forest Edge birth at seed `1727443675`,
  tick 338, phase 4, 1× simulation speed, 100% board zoom, and 1280×720. The
  preview paused after the birth event at parent `(10,5)` and child `(10,6)`.
  The board and pause state were visible, but the poof was not clear enough in
  that capture to accept visually. The capture was returned inline and was not
  saved as a repository artifact.
- A same-event 1920×1080 capture could not be completed: Unity had left Play
  Mode after the 1280 capture. A later Play session fell back to the desktop;
  the clean test scene was left in Edit Mode. The Editor reported no compile
  failure before that session ended.

## Risks and incomplete work

- The cloud may be too small or low-contrast at normal zoom; inspect a retained
  1280×720 and 1920×1080 capture before accepting or changing its scale,
  silhouette, palette, or duration.
- The cue still represents one newborn even when a litter places several
  children in one tick.
- The sound cue, authoring review, and runtime verification remain open.
- The current Editor console contains earlier errors from upgrade purchases
  attempting a reproduction chance of `1.005`. These occurred before this
  visual pilot and are not addressed here; inspect the upgrade cap separately.

## Next useful step

Open the Forest Edge simulation through the desktop test scene, wait for a real
birth, and pause from the birth event in the same frame. Save captures to disk
before changing Game View resolution; record the seed, phase/tick, speed, and
zoom with each capture. Review the cue at 1280×720 and 1920×1080, then at 1×
and 4× speed. Adjust its art only from those retained captures. Then review
whether a single sound should represent the litter and add an authored clip
through the project's audio path.
