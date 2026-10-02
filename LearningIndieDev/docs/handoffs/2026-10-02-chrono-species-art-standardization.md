# Chrono Fox and Rabbit art integration - 2026-10-02

Bevin requested standardized 32/64/128 pixel exports of Chrono's handmade fox
and rabbit, with all species icons and board animals using the same artwork.

## Implementation

- Used the existing 64x64 original PNGs directly; no generated replacement art.
- Rebuilt six standardized PNGs and two retained 1024 compatibility exports.
  Existing `Animals_01_Fox`/`Animals_01_Rabbit` filenames and GUIDs are intentional:
  atlas slots, Species Catalog selection and Noesis resources already use them.
- Configured their importers and the V2 animal atlas through Unity's native
  importer APIs: Point filtering, no mipmaps, uncompressed textures.
- Added the missing semantic `GalapagOS.Image.Species.Fox.128` resource.
- Updated the desktop scene's Fox Sprite assignment to Chrono's original.
  The prototype and both Rabbit assignments already referenced Chrono originals.
- Added **Salty Game > Art > Rebuild Chrono Species Icons** to reproduce these
  exports when Chrono changes the originals. Ownership and paths are documented
  in `docs/Species Design/CHRONO_SPECIES_ART.md`.

## Validation

- Unity 6000.4.6f1 compiled and executed the export method successfully.
- Verified all six requested dimensions, transparent pixels, unchanged palette
  and asset GUIDs. The 64-pixel outputs exactly match their originals; each
  128-pixel output exactly doubles the original pixels.
- Direct graphics-capable PlayMode passed **2/2**, zero failures/skips:
  `GalapagOSDesktopAndSimulationCaptureGameViewEvidence` and
  `CellularPrototypeInitializesEveryAuthoredAnimalSprite`.
- Inspected the captured Field Notes and Fox hunt screens, verifying the
  rabbit portraits, fox/hare board animals, ledger icons and Fox event portrait.
- Bevin visually accepted the integrated artwork on October 2 and authorized
  publication to BevBranch plus additive Trello progress comments.
- Evidence: `artifacts/chrono-species-art-20261002/`, including export log,
  asset-validation JSON, PlayMode XML/log and seven camera screenshots.
- The normal `CellSim Visuals` wrapper stopped at its existing missing
  `Resolve-UnityExecutionLane` helper before running. Direct Unity provided
  the validation above; this task did not repair that unrelated wrapper.

## Preserved local work and review boundary

This handoff ships with the art integration checkpoint on BevBranch, including
the imported Fox source and metadata and the Fox assignments in both scenes.
Both original PNG hashes are unchanged. Bevin's separate prototype 400/20/10
population edits and fox species asset edits are preserved locally and excluded
from this art commit. No simulation rules were changed by this task.

Human visual acceptance and the focused automated checks cover this artwork
integration. Salty's strategy review and matched-seed balance validation remain
open. Review this handoff alongside `docs/Species Design/CHRONO_SPECIES_ART.md`;
the latest checkpoint is available through `git log --oneline --` followed by
this handoff's path.
