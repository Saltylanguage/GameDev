# Forest Edge Board Visual Pass

[Working state](../WORKING_STATE.md) | Status: in-progress

- Handoff schema: 1
- Handoff ID: 2026-09-27-forest-edge-visual-pass
- Owner: Codex
- Branch: codex/forest-edge-visual-pass
- Baseline commit: 55d90eb3
- Date: 2026-09-27
- Supersedes: none

## Goal

Make the Forest Edge board look more like a place and make two meaningful
events easier to notice: a litter arriving and a Fox moving while hunting.
This work is presentation-only so scenery does not silently change routes or
scenario balance.

## Changes in progress

- The board renderer draws a layered pixel canopy at the top and left field
  edges, plus small deterministic ground details. Flowers are tied to grass
  resource state; low pebbles only appear as ground texture. Side rock clusters
  sit in the board margin rather than occupying cells.
- The existing mating/birth animation now draws the larger poof behind each
  child placed in the latest birth tick. The heart and sparkle remain, and one
  short chime plays for the litter. Chime playback is rate-limited and the
  GalapagOS volume and mute controls now set `AudioListener.volume`.
- The board view model tracks the currently hunting Fox, or one salient Fox if
  it has not yet selected one. It records only an adjacent previous cell as a
  footprint, keeps up to four prints, fades each over nine simulation ticks,
  and does not age them while paused.
- The active bunny in the compact phase tracker now makes two soft scale pulses
  followed by a short rest, with its feet anchored to the active phase pip.
- A small Field Ledger reaction card appears for a new litter or a Fox entering
  Hunting. It reuses the shared rabbit-face and Fox pixel-art resources, stays
  collapsed between notable events, and gives birth reactions priority over
  hunt notices. Its display timer pauses with the simulation so the player can
  read it.

## Visual evidence

- Baseline capture: [`visual-evidence-20260927-095257`](../../artifacts/visual-evidence-20260927-095257/)
- First changed capture: [`visual-evidence-20260927-095849`](../../artifacts/visual-evidence-20260927-095849/)
- The first changed capture clearly shows the pixel canopy edge at 1280×720.
  The field center still retains its grass/bare-grid look.
- The first 2026-09-28 capture confirmed the board visuals but exposed that the
  panel was absent: the existing acceptance test drove the board renderer
  directly without setting the shell's event state. The test now checks the
  collapsed state before an event, then shows the Fox reaction and the
  higher-priority birth reaction.
- Reviewed 1280×720 captures:
  [`visual-evidence-20260928-163621`](../../artifacts/visual-evidence-20260928-163621/)
- Reviewed 1920×1080 captures:
  [`visual-evidence-20260928-163450`](../../artifacts/visual-evidence-20260928-163450/)
- At both sizes, the reaction card reads cleanly under phase progress and above
  population. The hunt capture uses the Fox sprite; the birth capture uses the
  expressive rabbit face. The active timeline bunny is visible, although a
  timed motion comparison has not been retained yet.

## Verification state

- The focused `GalapagOSDesktopAndSimulationCaptureGameViewEvidence` PlayMode
  visual test passed 1/1 in the Live lane at 1280×720 and 1920×1080 on
  2026-09-28. Unity imported and rendered the updated XAML in the connected
  Editor; the saved captures were visually inspected.
- The capture test asserts that the card is collapsed before an event, shows the
  hunt copy and Fox portrait, and then replaces it with the birth copy and
  rabbit portrait. It invokes the shell's presentation methods directly, so it
  does not yet prove a natural metrics event passes through each host.
- `git diff --check` passes, both edited XAML files parse as XML, and all new
  image references use shared `StaticResource` keys.
- A later Live capture on 2026-09-27 ended with an Editor crash; at that time the
  project was unreachable behind its lock. `unity status` was ready again on
  2026-09-28. The earlier crash does not establish that the visual edits caused
  it.
- A clean full PlayMode attempt at
  [`unity-tests-20260928-175027`](../../artifacts/unity-tests-20260928-175027/)
  did not start its tests. Unity's licensing client threw an
  `ObjectDisposedException`, then Package Manager IPC timed out. The runner
  exited without a result file, so this is infrastructure failure rather than a
  failed product test. No automatic retry or process cleanup was attempted.
- A timed frame-to-frame pulse comparison has not been retained.

## Open items

- Exercise a natural birth and hunt through `VM_SimulationBoard` and both host
  paths; check the event card at normal zoom, 1× and 4×, pause and mute. Confirm
  that a litter produces one chime and the visual event remains readable beside
  the board.
- Retain a short timed capture or equivalent runtime evidence that the phase
  bunny makes a double pulse and rests between pulses.
- Decide whether to keep the code-synthesized chime or replace it with an
  authored clip after listening to it in the game.
- The board center still needs stronger biome identity. A stream remains
  deferred because water, banks, crossings, and passability need a gameplay
  contract. Trees or boulders that appear inside the field need real terrain
  behavior before production art can imply blocked movement.
- A brief Field Ledger birth note and explicit performance measurement remain
  open.

## Workflow observations for the next visual polish pass

- Search for reusable art and confirm the shared Noesis image keys before
  generating or importing new sprites. The existing rabbit and Fox assets were
  enough for this pass.
- A green visual-test result is insufficient when its setup bypasses the state
  that drives a new UI feature. Inspect the screenshots, make the test assert
  the idle and event states, and check each target resolution.
- Keep the display decision in the shell ViewModel, use the board's recorded
  event cues as the source, and leave simulation rules untouched. The remaining
  gap is an integration check from recorded event through both composition
  hosts.

## Git state

The user authorized pushing this feature branch and explicitly asked not to
merge work. The heartbeat and event reaction implementation is commit
`f1144b53` on `codex/forest-edge-visual-pass`; push state is recorded after the
documentation commit. Do not merge.
