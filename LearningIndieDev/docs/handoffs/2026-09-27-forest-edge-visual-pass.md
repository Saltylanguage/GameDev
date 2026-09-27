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

## Visual evidence

- Baseline capture: [`visual-evidence-20260927-095257`](../../artifacts/visual-evidence-20260927-095257/)
- First changed capture: [`visual-evidence-20260927-095849`](../../artifacts/visual-evidence-20260927-095849/)
- The first changed capture clearly shows the pixel canopy edge at 1280×720.
  The field center still retains its previous grid look. Changes made after
  that capture, including revised canopy placement and the side rock clusters,
  are not yet visually reviewed.

## Verification state

- The baseline and first changed visual acceptance captures each completed
  through the Live runner. The first changed capture preceded the latest audio
  and rock placement edits.
- A later Live capture started at 10:08 local on 2026-09-27 and Unity crashed
  before the runner saved screenshots or results. The Editor log records the
  crash; its dump contains no readable stack report. The crash alone does not
  establish whether these edits caused it.
- `unity status` reports no connected Editor while
  `LearningIndieDev/Temp/UnityLockfile` exists (last modified 2026-09-23). The
  clean runner refuses to launch in this locked/unreachable state. Do not remove
  the lock or terminate unverified processes to bypass this.
- `CellSim.ps1 -Command Doctor` reports the project as unreachable, with the
  licensing client unreachable and the Unity services endpoint refusing
  connection.
- `git diff --check` passes. `dotnet build Assembly-CSharp.csproj --no-restore`
  cannot run because the generated `Temp/obj/Assembly-CSharp/project.assets.json`
  is absent; restore did not create it. No post-audio Unity compile or
  1920×1080 visual capture has completed.

## Open items

- Re-run Unity compilation and visual acceptance when the Editor can reconnect
  or the project can safely use the Clean lane. Capture at 1280×720 and
  1920×1080, then review the later canopy and rock edits.
- Review a real birth and hunt at normal zoom, 1× and 4×, with pause and mute.
  Confirm that a litter produces one chime and that the latest child positions
  and Fox trail are legible without hiding animals.
- Decide whether to keep the code-synthesized chime or replace it with an
  authored clip after listening to it in the game.
- The board center still needs stronger biome identity. A stream remains
  deferred because water, banks, crossings, and passability need a gameplay
  contract. Trees or boulders that appear inside the field need real terrain
  behavior before production art can imply blocked movement.
- A brief Field Ledger birth note and explicit performance measurement remain
  open.

## Git state

The user authorized pushing this feature branch and explicitly asked not to
merge work. Implementation commit `9a66f9ec` is pushed to
`origin/codex/forest-edge-visual-pass`; the working tree is clean. Do not merge.
