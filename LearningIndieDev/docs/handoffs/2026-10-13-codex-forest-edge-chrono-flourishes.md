# Forest Edge Chrono flourish integration

[Working state](../WORKING_STATE.md) | Status: complete

- Handoff schema: 1
- Handoff ID: 2026-10-13-codex-forest-edge-chrono-flourishes
- Owner: codex
- Branch: BevBranch
- Baseline commit: d64915ef94dca51a4139cf9e870a4d43d566e409
- Date: 2026-10-13
- Supersedes: none

## Summary

Imported Chrono's eight supplied 64x64 Forest Edge decorations: one mushroom,
three shrubs and cream, purple, blue and yellow flowers. The board places them
over full grass tiles, beneath creatures. Bevin liked the presentation and
requested transparent backgrounds; the missing Noesis texture labels are now
corrected. Bevin confirmed the final appearance and authorized publishing the
combined artwork/transparency/seam changes on October 13.

## Changes and Unity locations

- Source sprites and metadata: `Assets/Art/Terrain/Blob/64/ForestEdgeFlourishes/`.
  Original PNG bytes are preserved. Point filtering, 64 PPU, single FullRect
  sprites, no mipmaps and uncompressed imports retain pixel art.
- Existing `Assets/Art/Terrain/Terrain_01.spriteatlasv2` packs the parent folder;
  its GUID and settings are preserved. Packing produced 55 sprites on one page.
- `ForestEdgeFlourishResolver` selects deterministic coordinate-based decoration
  on passable Grass with mask 255. It does not consume simulation randomness.
- `VM_SimulationShell` resolves named atlas crops for the ForestEdge scenario.
  `SpeciesSimulationBoard` draws decorations at 60% cell size after terrain and
  before creatures. Both `GalapagOSDesktopNoesisHost` and
  `SpeciesSimulationNoesisHost` pass the images through the existing snapshot path.
- No scenario, ecology, reward, population or balance values changed.

## Transparency diagnosis

The source PNGs already have alpha. Noesis draws premultiplied-alpha textures;
its existing package `NoesisPostprocessor` performs this conversion for textures
labelled `Noesis`. All eight new imports now carry that label and were reimported
and repacked. This removes the colored rectangles without editing source art.
The asset contract test now enforces that label.

## Decisions and assumptions

These are cosmetic flourishes, not new resource species or terrain families.
Stable placements remain under animals and avoid grass edges and bare paths.
The custom atlas-backed board uses resolved `CroppedBitmap` objects and no XAML
image literals: the SG-006 custom-rendering exception applies. Owner: presentation
maintainers; reconsider dictionary ownership if these images acquire XAML consumers.

## Validation

- Unity 6000.4.6f1 compiled without errors after the completed change.
- Focused EditMode: Flourish filter 2/2 and TerrainTile filter 11/11; one overlap,
  12 distinct tests. After the alpha correction, Flourish 2/2 passed again,
  including the Noesis-label assertion.
- Desktop Play Mode: Forest Edge journey launched, all eight variants were
  observed, paused tick 0 and tick 20 screenshots captured. The corrected
  `desktop-transparent.png` was visually inspected: grass shows through the
  flourish backgrounds. Original animal-art boxes are outside this change.
- Source PNG hash parity, new-asset metadata parity and `git diff --check` pass.
- Local evidence: `artifacts/forest-edge-flourishes-20261012/`, including
  `test-results.json`, `editor.log`, before screenshots, source preview and
  `desktop-transparent.png`. These artifacts are ignored by Git.

## Risks and incomplete validation

- No full Unity suite or standalone player build was run for this cosmetic work.
- A prototype Play Mode screenshot was captured before the alpha correction;
  its binding eval timed out. Unity was closed before the final prototype check.
  The prototype uses the same atlas/board but its corrected render is not freshly
  visually verified.
- Live switching to another authored scenario encountered an empty species
  definition; a legacy exclusion probe also failed. These probes do not establish
  a successful scenario-switch check. Source gates decorations to ForestEdge;
  do not describe the failed probes as passing runtime coverage.

## Next useful step

Bevin has accepted the corrected presentation and authorized publication. If
checking scenario switching, first establish a valid selectable
scenario/session state and inspect the returned selection result.

## October 13 follow-up: fox transparency

Bevin's screenshot showed the same colored rectangle around the fox. All three
scene references (Desktop, prototype, D5 visual review) resolve to original
`Assets/Art/Species/Animals/Chrono_Fox 1.png`, GUID
`b9588baa12e33ea48a287a04e48229a8`, rather than the already-labelled standardized
export. That source importer lacked `Noesis`; the rabbit source already had it.
Added only the fox's `Noesis` label and reimported using the existing package
premultiplication pipeline. Source PNG bytes, GUID and all other import settings
are unchanged. This resolves the animal boxes noted in the earlier validation.

Fresh Desktop Play Mode and an enlarged board screenshot visually confirm
grass/paths through fox backgrounds. GPU texture readback asserts corner RGBA
`(0,0,0,0)`. `git diff --check` passes; Console ground truth has no compile or
Console errors (one warning). Evidence is under local ignored
`artifacts/fox-transparency-20261013/`, especially `before.png` and
`fox-transparent.png`. Reimport invalidated the old live texture reference;
a fresh Play session was used for final verification. Unity restored to stopped
MainMenu afterward. No new logic or scene changes; no additional full-suite or
player-build run. The source-only label change is included in the authorized
combined publication. See the [accepted seam fix](2026-10-13-codex-terrain-seam-rendering-fix.md)
for the subsequent 26-test presentation check and human visual acceptance.
