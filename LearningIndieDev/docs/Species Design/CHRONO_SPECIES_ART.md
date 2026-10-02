# Chrono species artwork

Chrono's handmade 64x64 originals are the source of truth:

- Fox: `Assets/Art/Species/Animals/Chrono_Fox 1.png`
- Hare/rabbit: `Assets/Art/Species/Animals/Chrono_Rabbit_Tile_Face.png`

Use **Salty Game > Art > Rebuild Chrono Species Icons** after changing either
original. The command uses nearest-neighbor sampling, retains transparency and
the original palette, and imports the exports with Point filtering, no mipmaps,
and no lossy texture compression. It does not modify the original files.

The generated files deliberately keep the existing `Animals_01_Fox.png` and
`Animals_01_Rabbit.png` names and GUIDs. These now contain Chrono's artwork:

- `Standardized/32/`: 32x32 versions for small UI icons and the animal atlas.
- `Standardized/64/`: 64x64 versions for portraits and the Species Catalog.
- `Standardized/128/`: 128x128 versions for large portraits.
- `Standardized/`: retained 1024x1024 compatibility exports, also rebuilt from
  Chrono's originals so older asset references display the same artwork.

All paths above are relative to `Assets/Art/Species/Animals/`. The
`Animals_01.spriteatlasv2` atlas packs the 32-pixel folder and uses Point
filtering and uncompressed textures. Its Fox/Rabbit sprite names remain stable.
The other six animals are unchanged.

Noesis views continue to use the semantic Fox/Rabbit keys in
`Assets/UI/ImageResources.xaml`; changing their standardized PNGs updates the
desktop, setup screen, field guide, population ledger, and event portraits.
Both `CellularAutomataPrototype` and `GalapagOSDesktopTest` assign the original
Chrono Fox/Rabbit sprites to their simulation hosts for the board animals.
This artwork replacement does not change species IDs or simulation rules.
