# Handoff — GalapagOS graphics acceptance

**Date:** 2026-09-07  
**Owner:** Codex  
**Branch:** `codex/simulation-window-production`  
**Status:** P1-014 acceptance closed

## Result

The GalapagOS desktop and dedicated Simulation composition were exercised in
Unity PlayMode with graphics enabled. The desktop test scene now has valid
serialized Transform back-references, and the desktop XAML fits the authored
1920x1080 canvas into the 1280x720 target through a root `Viewbox`.

The acceptance test drives the Settings surface and multi-window collection,
opens Simulation through the desktop command, verifies the desktop and
Simulation camera handoff, and checks that the board ViewModel has a snapshot.
It captures the desktop home and Simulation compositions with the existing
camera-render evidence path.

## Evidence

- Full 1280x720 PlayMode batch: `artifacts/visual-evidence-20260907-023518/`
  (`22/22` passed; includes `01-galapagos-desktop-home.png` and
  `02-galapagos-simulation.png`).
- Focused 1920x1080 graphics run:
  `artifacts/visual-evidence-20260907-023320/`.
- The acceptance scene is included in `ProjectSettings/EditorBuildSettings.asset`
  so the focused and full PlayMode tests can load it by scene name.

The remaining GalapagOS application surfaces are feature work under the
desktop screens ticket; they are outside this graphics gate.
