# Intermittent terrain tile seams

[Working state](../WORKING_STATE.md) | Status: complete

- Handoff schema: 1
- Handoff ID: 2026-10-13-codex-terrain-seam-rendering-fix
- Owner: codex
- Branch: BevBranch
- Baseline commit: d64915ef94dca51a4139cf9e870a4d43d566e409
- Date: 2026-10-13
- Supersedes: none

## Summary

Bevin reported intermittent dark lines between Forest Edge tiles. Fractional
zoom, panning and the UI Viewbox scale left separately drawn tile boundaries
between screen pixels, exposing the dark base underneath. The shared board now
aligns row and column boundaries to screen pixels and uses those same boundaries
for terrain and creature rectangles. Bevin visually accepted the result and
authorized publication on October 13, together with the preceding
[Chrono art integration](2026-10-13-codex-forest-edge-chrono-flourishes.md).

## Evidence and implementation

- Reproduced lines in Desktop Play Mode. They persisted with nearest-neighbor
  filtering, disabled PPAA and a standalone fully opaque flat texture with no
  atlas. This isolated rectangle coverage rather than the source PNG or atlas.
- Local-coordinate rounding removed only some lines because the UI scales the
  board afterward. Screen-space alignment removes the reproduced seams.
- `SpeciesSimulationBoard.GetPixelAlignedEdges` projects boundaries into screen
  coordinates, rounds them, then maps back. `GetCellRect` shares those row/column
  edges across both rendering passes. There is no tile overlap or source crop.
- Panning invalidates the visual. A layout callback compares the rendered
  origin/unit transform and redraws when scaling changes, preventing stale
  alignment after layout or viewport changes.
- No artwork, atlas settings, scene assets, simulation state or balance values
  changed for this fix. The PlayMode test assembly now explicitly references
  the already-installed Noesis assembly to exercise rendered pixel coverage.

## Validation

- Unity 6000.4.6f1: all **26 presentation PlayMode tests pass**, including zoom
  commands, serialized composition, sprite initialization and journey behavior.
- New rendered regression checks shared boundaries on opaque probe tiles at
  zoom 0.75, 1.13704991, 2 and 4, fractional pan (-3.25, 4.75), and UI scale
  (0.83, 0.91). Cell interiors are excluded because meadow/pebble details are
  intentional. Camera, transform, sprite bindings and temporary textures restore.
- Negative control: temporarily bypassing alignment fails the same regression
  with a visible seam (RGB difference 104, threshold 6). Alignment restored;
  focused regression passes again after resource-cleanup changes.
- Actual-art Desktop renders inspected at 1070x602, 1600x900 and 900x700, zoom
  2.13704991 and fractional pan: no reproduced grid seams. Camera targets and
  Noesis view sizes were updated together for each render.
- Automatic layout-alignment refresh assertion passes after those viewport
  changes. Compilation/Console errors zero; one warning. `git diff --check` passes.
- Local ignored evidence: `artifacts/terrain-seams-20261013/verification.json`,
  `negative-control.png`, diagnostic captures and `fixed-*.png`.
- Human acceptance: Bevin confirmed the corrected appearance on October 13
  and explicitly authorized pushing the combined work.

## Limits and next useful step

No full project suite, standalone player build or performance profile was run.
The alignment assumes the board's current axis-aligned scale/translation UI;
arbitrary rotation is not a supported presentation mode. Unity restored to
stopped MainMenu. Visual review is accepted; publication is authorized. Git
history identifies the combined artwork/rendering publication checkpoint.
