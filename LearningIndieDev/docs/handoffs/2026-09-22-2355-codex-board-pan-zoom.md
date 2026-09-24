# Field observation board pan and zoom

[Working state](../WORKING_STATE.md) | Status: implementation complete, runtime verification pending

- Handoff schema: 1
- Handoff ID: 2026-09-22-2355-codex-board-pan-zoom
- Owner: Codex
- Branch: BevBranch
- Baseline commit: c6b3282
- Date: 2026-09-22
- Supersedes: none

## Summary

Restored mouse navigation for the Forest Edge field observation board. Hold the
left mouse button and drag to pan. Use the wheel to zoom between 0.75x and 4x;
zoom stays anchored under the pointer. Pan is clamped so the board remains in
the clipped viewport.

## Cause and implementation

- History shows the original `TranslateZoomRotateBehavior` was introduced in
  `2152f20`, restored after a merge in `7c06c4e`, and removed during the shell
  rewrite in `dbc282a`. The current `SpeciesSimulationBoard` had no input
  handlers or render transform.
- Pan and zoom state and input now live in `SpeciesSimulationBoard`, the custom
  renderer that remains present across the shell XAML. It captures the mouse
  during a drag and uses parent-space pointer positions. Wheel zoom uses a
  matrix transform and preserves the point under the cursor where the viewport
  bounds allow it.
- The board title and species/resource legend are informational overlays, so
  they no longer block input reaching the board.
- No simulation or ViewModel state changed. No assets or `.meta` files changed.

## Validation and limits

- `git diff --check` passed. A scratch .NET compile of `SpeciesSimulationBoard.cs` against current Unity/Noesis managed assemblies reported 0 errors and one `MSB3277` System.Net.Http reference-version warning; this is not a Unity Editor compile.
- Unity was open, but `unity status --project-path F:\ForkBin\GameDev\LearningIndieDev`
  reported no connected Pipeline instance. No live Unity compilation or runtime
  interaction check was available; no automated tests were run.
- Before acceptance, confirm left-drag pan, wheel zoom direction and cursor
  anchoring, min/max zoom, and panning after zoom in the actual viewer.
