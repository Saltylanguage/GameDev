# Player slashline field notes and results

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-01-2358-codex-player-slashline-field-notes-and-results
- Owner: Codex
- Branch: BevBranch
- Baseline commit: 3eea4e8
- Date: 2026-10-01
- Follow-up completed: 2026-10-02
- Supersedes: none

## Summary

Bevin requested the DARWIN OR DIE slashline metrics back in the game while
exploring starting populations. The calculations were intact, but display was
restricted to Developer Mode and the player Field Notes button had no command.
Field Notes now opens the cumulative metrics during the expedition; the results
screen also shows them for completion and extinction. Work remains uncommitted
on BevBranch alongside the earlier S4 slice; no push or Trello edit occurred.

## Changes

- `Assets/UI/HUD/Scripts/VM_SimulationShell.cs`: exposes player-visible metrics,
  open/close commands, source seed/tick and a short metric guide. Reuses existing
  Herbivore/Predator formatters and domain metrics; no new formulas or scores.
- `Assets/UI/HUD/XAML/V_Window_CellSimulation.xaml`: connects Field Notes to a
  scrollable paper panel and adds neutral species headings, context and guide
  to results. The existing report remains scrollable. No scene/GUID changes.
- `Assets/Tests/PlayMode/SpeciesPresentationPlayModeTests.cs`: updates the
  existing diagnostics test to check player-visible slashlines, ADD/FPO,
  notes open/close and continued Developer Mode gating of upgrade diagnostics.
- `docs/WORKING_STATE.md`: indexes this change and its verification limits.

## Decisions and assumptions

- Whole-expedition cumulative totals, including every completed/current phase,
  are labelled with species, seed, source tick and LIVE/FINAL. They are not
  isolated per-phase deltas. Running totals update each tick; opening notes
  does not pause or advance the simulation. Use the existing Pause control for
  a fixed-tick inspection. Close returns to the underlying run/decision view.
- Hare shows pAVI/eAVI/predAVG/sAVI/cAVI/bAVG/RFS/APS plus supporting counts;
  Fox shows its corresponding hAVG/aAVG/huntAVG/sAVI/cAVI/bAVG/RFS/AHS.
  ADD and population reconciliation stay in the existing domain path.
- N/A and INVALID remain explicit. The guide explains cAVI measures crowding
  deaths, not energy saved. The legacy public property names remain compatible.
- Bevin's preferred 400/20/10 remains an exploration candidate. Neither the
  implemented 400/25/15 S4 fixture nor production defaults were changed.

## Validation

- Connected to the existing Unity 6000.4.6f1 Editor via Pipeline at port 7801.
  Explicit recompile completed with failed=false and no compiler errors.
- Live C# assertion checks in the existing prototype scene passed for:
  Hare without Developer Mode; correct SPO/FPO; N/A at tick zero; paused
  one-tick updates; seed/tick context; all Hare ratio keys; and unchanged pause
  state when opening notes.
- Actual Noesis FieldNotesPanel/FieldNotesStatLine bindings were checked for
  visibility and source text. Inspected a 1280x720 camera capture of the panel.
- A six-phase, 10-tick diagnostic with seed 5 and 400/20/10 checked notes at a
  decision, one purchased Hare (ADD=1, reconciled population), close behavior
  and completed results at tick 60. This is mechanics evidence, not balance.
- Fox at seed 5 and 400/20/10 passed its role-specific stats/context/guide and
  hidden Developer diagnostics checks. An initial harness assertion incorrectly
  expected the experimental flag to disable: the current preview intentionally
  forces that player path on. Corrected the assertion, retained the failed
  harness output, and reran successfully; no gameplay change was needed.
- Reproduced the earlier seed-5, 400/25/15, 100-tick-phase run with Movement at
  tick 100. Extinction at tick 180 showed FINAL, FPO=0, reconciliation=True and
  player-visible metrics. Inspected completed and failed result captures.
- Raw scripts/results/images are ignored local evidence under
  `artifacts/slashline-ui-20261001/`. Four passing live assertion files cover
  Hare, boundary/results, Fox and extinction, plus a Noesis binding check.
  Use `field-notes-hare-camera.png`, `slashline-results.png` and
  `slashline-extinction.png` as rendered evidence. The first screen-source
  capture returned a stale backbuffer and was not used for visual validation.
- The updated NUnit test was not run: the open prototype scene had unsaved
  edits. Live checks did not reload or save it. Unity was returned to stopped
  Play Mode, with the same scene still dirty. Prior 180+2 test counts describe
  the earlier S4 slice, not this UI change. `git diff --check` passed.

## Risks and incomplete work

- Bevin subsequently reported on October 2 that the new display looks good.
  This is positive visual feedback on the displayed panel; the checked render
  is the prototype at 1280x720, not every resolution or profile launch route.
- Full NUnit suites and the modified focused test remain unexecuted for this
  UI change. Run the focused test once the user finishes the unsaved scene work.
- Earlier S4 card-footer clipping, stale Ready setup display and execution
  wrapper helper problems remain separate follow-ups.

## Next useful step

With Bevin's positive display feedback recorded, the next implementation slice
is Crowding Tolerance for Warren, then Seed Dispersal for Gardeners. Retain the
earlier UI follow-ups and validation limits. Commit/push only when
requested, and update the related board as part of that push.
