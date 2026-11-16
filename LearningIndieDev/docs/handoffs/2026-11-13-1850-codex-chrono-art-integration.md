# Chrono art integration

[Working state](../WORKING_STATE.md) | Status: in-progress

- Handoff schema: 1
- Handoff ID: 2026-11-13-1850-codex-chrono-art-integration
- Owner: Codex
- Branch: codex/s6-predator-first
- Baseline commit: 9b77c146
- Date: 2026-11-13
- Supersedes: none

## Summary

Chrono's newly delivered art was checked against the Unity project. Two original PNGs were retrieved before the Trello browser download route stalled: `simulation.png` and `reward_v1.png`. Both are now imported in Unity. The simulation icon is used by journey map simulation nodes; `reward_v1` is retained as a source variant until it can be compared with `reward_v2`. This work is local and uncommitted.

## Changes

- Copied the two byte-identical 64×64 Trello originals to `Assets/UI/GalapagOS/Art/Icons/`, keeping their original names. SHA-256: simulation `d7eda0e9b92743a619e1ebf8840f91ef45eb3733ce7f6576394af8cab3f3f0b7`; reward v1 `5f18eb53dc660712e2fc2272ba31e3e50c7e54d7219897bdfad4228193f95f67`.
- Imported them through the Unity Editor as single sprites with point filtering, no mipmaps, clamp wrapping, uncompressed default textures, alpha transparency, and the Noesis label. Unity generated their `.meta` files and GUIDs.
- Added `GalapagOS.Image.Journey.Node.Simulation` to `Assets/UI/ImageResources.xaml` and replaced the journey map's simulation placeholder path with that resource in `Assets/UI/HUD/XAML/V_Window_CellSimulation.xaml`.
- Left all terrain art untouched at Josh's direction. The desert biome is not implemented, and the need for a grass update is undecided.

## Decisions and assumptions

- The [UI Icons card](https://trello.com/c/32Y2b0C2/51-ui-icons) identifies `simulation.png` as a simulation icon. Its project usage is the existing simulation journey node.
- `reward_v1.png` depicts a blue DNA helix. The same card also contains `reward_v2.png`; no variant was selected for the journey reward node without comparing the delivered originals.
- The [Animal Icons card](https://trello.com/c/KWadDn89/40-animal-icons) includes Fox, already integrated, plus Deer, Owl, Wolf, and a newer Rabbit original. No species rule or UI mapping was invented for those pending originals.
- The [Backgrounds card](https://trello.com/c/0uBTZQMI/39-backgrounds-wallpapers) has a newer `Hero_Image.png` than the local older download; the older file was not substituted.

## Validation

- The two project PNG hashes match the retrieved originals; their byte counts match Trello's attachment metadata (925 and 1242 bytes).
- Unity imported both assets and generated `.meta` files; the Noesis label and sprite import settings were read back from those files. The two changed XAML files parse as XML, and `git diff --check` passed.
- `CellSim.ps1 -Command Visuals -Execution Live -TestFilter SaltyGame.PlayModeTests.GalapagOSVisualAcceptanceTests.JourneyReportReturnsToMapWithAvailableRoutes` passed 1/1. The captured `09-cycle-one-route-choice.png` and `11-next-simulation-available.png` in `artifacts/visual-evidence-20261113-184918/` show the simulation icon on journey nodes.
- Unity was launched with the host identity after sandboxed Unity Doctor could not reach the license client. Host Doctor passed. The installed Unity CLI is 1.0.0-beta.6, which does not support the newer skill's `--caller` and `--skill` flags; the successful Editor eval used the CLI's supported syntax.

## Risks and incomplete work

- Ten other originals from the [UI Icons card](https://trello.com/c/32Y2b0C2/51-ui-icons) are still missing locally: `eco-coin.png`, `final_challenge1.png`, `final_challenge2.png`, `final_challenge3.png`, `leaf_restore.png`, `meat_restore.png`, `minus.png`, `plus.png`, `question.png`, and `reward_v2.png`.
- The [Animal Icons card](https://trello.com/c/KWadDn89/40-animal-icons) originals for Deer, Owl, Wolf, and the newer Rabbit are missing locally. The latest `Hero_Image.png` on the [Backgrounds card](https://trello.com/c/0uBTZQMI/39-backgrounds-wallpapers) is also missing locally.
- Trello metadata access worked, but browser retrieval of attachments stalled repeatedly. Only the two retrieved originals were imported. The panorama card's `download.jpg` is a reference image, not final art.
- The Editor log emitted repeated Noesis `InvalidCastException` entries during asset refresh. The focused journey map visual test still passed and rendered the new icon; broader impact of those Editor exceptions was not investigated.

## Next useful step

Obtain the remaining original files as a single attachment bundle from Chrono or a reliable Trello export, verify them against Trello attachment metadata, then import and wire each applicable asset. Compare reward v1/v2 before choosing a UI variant. Keep grass and desert terrain out of this work until Josh decides their scope.
