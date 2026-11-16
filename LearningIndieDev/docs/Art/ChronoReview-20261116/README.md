# Chrono art review set — 2026-11-16

These are the original PNG attachments retrieved from Chrono's approved Trello cards for the M2-05 review. They are source candidates, not new gameplay species or active UI resources. Each file is 64×64. The Fox attachment is byte-identical to the existing `Assets/Art/Species/Animals/Chrono_Fox 1.png`, so it is not duplicated here.

| Source card | Files in this set | Current decision |
| --- | --- | --- |
| [UI Icons](https://trello.com/c/32Y2b0C2/51-ui-icons) | `UI/eco-coin.png`, `final_challenge1.png`–`3.png`, `leaf_restore.png`, `meat_restore.png`, `minus.png`, `plus.png`, `question.png`, `reward_v2.png` | Review candidates. The existing `simulation.png` and green/blue `reward_v1.png` are already wired to journey nodes. The final-challenge files are decorative frames without a self-explanatory finale symbol; no new role was assigned. |
| [Animal Icons](https://trello.com/c/KWadDn89/40-animal-icons) | `Animals/Deer_Tile_Face.png`, `Owl_Tile_Face.png`, `Wolf_Tile_Face.png` | Review candidates. They do not create playable species. The delivered Rabbit replaced the current Rabbit source in `Assets/Art/Species/Animals/Chrono_Rabbit_Tile_Face.png`; Fox was already identical. |
| [Backgrounds / Wallpapers](https://trello.com/c/0uBTZQMI/39-backgrounds-wallpapers) | Latest `Hero_Image.png` is integrated at `Assets/UI/GalapagOS/Art/Journey/WoodlandSummer.png` | Current runtime candidate for the desktop and Main Menu, preserving the existing Unity asset path and GUID. |

The two reward versions differ in palette: v1 is green/blue and v2 is red/blue. V1 remains the runtime candidate because it fits the approved pastel eco-OS direction; final art selection awaits Josh and Sim's visual review. Likewise, the new Hero and Rabbit artwork should be judged at 1280×720 and 1920×1080 in the running UI, not from these PNGs alone.

All attachment byte counts and SHA-256 hashes were checked against the downloaded originals before copying; [SHA256SUMS.txt](SHA256SUMS.txt) records the committed review files. The originals remain available on the linked cards. No grass, desert, or additional species scope is implied by this set.
