# Forest Edge flourish replacement

[Working state](../WORKING_STATE.md) | Status: visually accepted; publication authorized

- Handoff schema: 1
- Handoff ID: 2026-10-14-codex-forest-edge-flourish-replacement
- Owner: codex
- Branch: BevBranch
- Baseline commit: c5d4c5cab2766dfccf267b6c057489fa1c5d4ad1
- Date: 2026-10-14
- Supersedes: none; updates the artwork in the [eight-flourish integration](2026-10-13-codex-forest-edge-chrono-flourishes.md)

## Result and asset mapping

Replaced the eight prior Grass_Assets images with Bevin's ten supplied 64x64
PNGs from `F:/Downloads/`. All production PNG bytes match those source files.
Existing names and GUIDs are retained in
`Assets/Art/Terrain/Blob/64/ForestEdgeFlourishes/`:

| Source | Production name |
| --- | --- |
| Grass_Mushroom1.png | ForestEdge_Mushroom.png |
| Grass_Mushroom2.png | ForestEdge_Mushroom_Green.png (new) |
| Grass_Mushroom3.png | ForestEdge_Mushroom_Brown.png (new) |
| Grass_Bush1.png through Grass_Bush3.png | ForestEdge_Shrub_01.png through ForestEdge_Shrub_03.png |
| Grass_Flower1.png | ForestEdge_Flower_Yellow.png |
| Grass_Flower2.png | ForestEdge_Flower_Blue.png |
| Grass_Flower3.png | ForestEdge_Flower_Purple.png |
| Grass_Flower4.png | ForestEdge_Flower_Cream.png |

The resolver now includes all ten. Placement remains deterministic, only on
passable full grass, at 60% cell size beneath creatures. No ecology or scenario
values changed. The existing screen-pixel alignment is retained.

## Transparency and atlas corrections

All ten source images contain alpha. Their importers use point filtering,
64 PPU, FullRect single sprites, no mipmaps, uncompressed source textures and
the `Noesis` label required by the package premultiplication postprocessor.
Unity created and configured the two new metadata files; the existing eight
metadata files are byte-identical to their session-start copies, including
pre-existing local whitespace changes.

GPU readback found a second issue after import: the V2 atlas had no explicit
default platform entry and packed as DXT5. All source textures had zero RGB in
fully transparent pixels, but packed crops had 14-93 colored transparent edge
pixels each. Setting the V2 SpriteAtlasImporter default platform compression
explicitly to Uncompressed and repacking removes all of those pixels. The atlas
is now one 1024x512 RGBA32 page with 57 sprites (47 grass plus 10 flourishes).
Its uncompressed base texture uses about 2 MiB rather than DXT5's 0.5 MiB;
no performance claim is made. Atlas GUID, packable root and packing settings
are preserved. The asset contract now enforces uncompressed atlas defaults.

`VM_SimulationShell.CreateNamedAtlasSprites` now matches exact sprite names,
including Unity's `(Clone)` suffix, instead of prefixes. Otherwise the original
`ForestEdge_Mushroom` could resolve a new color variant. The live initialization
test now requires all ten crops to exist and be distinct.

## Verification

- Unity 6000.4.6f1, Direct3D12: compilation completed; final Console ground truth
  reports zero errors, one warning and no compilation failure.
- Focused EditMode: Flourish 2/2 and TerrainTile 11/11; one overlap, 12 distinct.
- Presentation PlayMode: 26/26, including all ten distinct crops and the
  rendered terrain-seam check at fractional zoom/pan/UI scale.
- Desktop visual acceptance capture: 1/1 at 1920x1080. Inspected
  `desktop/04-fox-hunt-before.png`: the new flourishes sit over grass without
  rectangular backgrounds. Bevin subsequently confirmed the appearance in his
  own screenshot and authorized pushing the change on October 14.
- GPU readback: all ten source textures and all ten packed atlas crops preserve
  transparent pixels with zero RGB. The failed compressed probe is diagnostic
  evidence; final `alpha-result.json` records the corrected result.
- SHA-256 parity: all ten PNGs match the supplied downloads. All eight original
  GUIDs and full metadata bytes match their pre-change copies.
- Diff whitespace check excluding blank-value metadata whitespace passes;
  the existing eight metadata whitespace changes were present at session start.
- Unity was restored to stopped MainMenu, visual environment overrides cleared,
  and the Editor opened for validation was closed afterward.

Evidence is local and ignored under
`artifacts/forest-edge-flourishes-20261014/`: source preview, source mapping and
hash manifest, import/alpha results, EditMode/PlayMode results, Desktop captures,
final Editor/Console state and original-asset backups.

## Limits and handoff

No full Unity suite, standalone player build, performance profile or balance
experiment was run. Bevin visually accepted the result and authorized publication
on October 14. Git history identifies the publication commit; the related art
production card and Obsidian project/daily/dev-log records receive the final
remote checkpoint after pushing.

Publication preparation removes only generated trailing spaces from touched
metadata; GUIDs, importer settings and serialized fields are preserved. The eight
existing metadata files then match the committed baseline exactly. Their original
session-start copies remain in the ignored evidence directory.

The documented `CellSim.ps1 Test` wrapper fails before running tests because
`Resolve-UnityExecutionLane` is undefined in its unchanged `UnityTooling.ps1`.
Validation used the existing connected Pipeline `run_tests` and `test_status`
commands directly and retained their results. No tooling repair was included.

Reproduction filters: EditMode `Flourish`, EditMode `TerrainTile`, PlayMode
`SpeciesPresentationPlayModeTests`, and PlayMode
`GalapagOSDesktopAndSimulationCaptureGameViewEvidence` with visual output enabled.
The Pipeline test filter does not combine these names using a pipe: that initial
probe returned zero tests and is not counted as validation.
