# M2 Q20 reduced motion explanation

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-11-18-1236-codex-m2-q20-reduced-motion-explanation
- Owner: Codex
- Branch: codex/m2-species-scenario-select
- Baseline commit: 2d8ada2d
- Date: 2026-11-18
- Supersedes: none

## Summary

Q20 remains unverified by Josh and Sim. The Reduced Motion option was hard to
notice because its effects occur only when brief board events play or a
selected Journey marker is visible. The Settings copy now states the specific
effects so a reviewer knows what to compare.

## Changes

- Replaced the generic Settings sentence with the current behavior: hunt and
  birth board animations are hidden, the selected Journey marker stops
  pulsing, and the event text remains available.
- Clarified the Q20 review record and named the event states for retesting.

## Decisions and assumptions

- Source inspection of `VM_GalapagOS_Desktop`, `SpeciesSimulationBoard`, and
  `VM_SimulationShell` confirms these effects. The board animations have no
  static replacements today; the Settings copy does not claim they do.
- This is a discoverability clarification, not a decision that the current
  reduced-motion presentation satisfies the M2-10 accessibility gate.

## Validation

- XAML parsed as XML; `git diff --check` passed.
- Clean 1920×1080 visual capture passed 1/1 at
  `artifacts/visual-evidence-20261118-123736`. The Settings screenshot
  `02b-galapagos-settings.png` was inspected after increasing the explanatory
  text to 14 pt; it fits without clipping and displays the current ON state.

## Risks and incomplete work

- Josh and Sim have not compared normal and reduced motion during the same
  Fox-hunt or birth event. Essential danger and outcome equivalence remains
  unverified, particularly because the board animations disappear outright.
- Broader board-event feedback is tracked in deferred Track B work.

## Next useful step

Retest Q20 on the same event with Reduced Motion on and off. Check the event
text, board danger cue, selected Journey marker, choice and result states.
Keep M2-10 open until human equivalence review.
