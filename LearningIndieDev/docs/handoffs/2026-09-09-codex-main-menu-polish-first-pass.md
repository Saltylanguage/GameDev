# Handoff — Main Menu polish first pass

**Date:** 2026-09-09  
**Owner:** Codex  
**Branch:** `codex/simulation-window-production`  
**Status:** Needs Review

## Result

Reworked the existing Main Menu presentation into a game-facing GalapagOS
entry screen while preserving the implemented Profile Selection, Continue, and
Quit contract. No ViewModel, helper, scene, persistence, or transition behavior
changed.

The screen now uses a full-bleed authored meadow/field-station background, a
branded `GALAPAGOS` title treatment, a focused expedition desk, responsive
16:9-safe overlay composition, visible keyboard focus states, a themed profile
window, and a themed quit confirmation. The launch canvas adds a restrained
panel reveal and drifting pollen motes, and the host defers keyboard focus by
one UI tick so it reliably lands on Continue or Profile Selection as
appropriate.

## Generated asset provenance

- Output: `Assets/UI/MainMenu_Old/Art/GalapagOS_MainMenu_Background_v1.png`
- Tool: Codex built-in image generation; underlying model/version not exposed
- Reference: `docs/Art Direction/Concepts/GalapagOS_Desktop_UI_Concept_Options_v1.png`
  used for palette and mood only
- Prompt: create an original polished 16:9 pixel-art ecological reserve at
  sunrise with layered mountains, river, wildflowers, a distant greenhouse,
  subtle cellular plot patterns, and one hare on the lower right; preserve calm
  negative space on the left for native menu UI; use the approved vanilla,
  meadow green, sky blue, pollen, coral, and dark-brown GalapagOS palette; no
  words, logos, HUD, controls, watermark, photorealism, or commercial-game
  imitation.

The imported texture is labeled `Noesis`, uses point filtering, disables
mipmaps, clamps at the edge, preserves its non-power-of-two size, and uses the
project's high-quality texture compression setting.

## Verification

- Parsed `V_Panel_MainMenu.xaml` as XML.
- Reimported the XAML and generated texture through the live Unity editor.
- Rendered and inspected Main Menu, Profile Selection, and Quit Confirmation in
  Play Mode through the scene's actual Main Camera.
- Confirmed the launch focus ring lands on the enabled primary action.
- Confirmed the final Unity Console contains zero warnings and zero errors.
- `git diff --check` passes for the XAML aside from the checkout's normal
  LF-to-CRLF warning.

## Review focus

This is intentionally a first art pass. Human review should decide whether
`GALAPAGOS` is the final player-facing title and whether this generated meadow
is promoted beyond the vertical slice. Replace or paint over the background
when final title/brand ownership or production-art direction changes; the XAML
composition does not depend on details inside the image.

## Refinement pass — 2026-09-10

- Primary buttons now have a pollen accent edge, brighter arrow plate, and
  subtle depth shadow; secondary actions gain a coral hover edge and shadow.
- Added a short CRT-style boot reveal: signal panel compresses from a scanline
  into the full menu before fading away.
- Continue now presents a CRT desktop handoff overlay, plays an original
  procedural pentatonic chime with a soft echo, then opens
  `GalapagOSDesktopTest` after 0.95 seconds. Batch-mode tests keep the prior
  immediate scene assertion path.
- Reimported the XAML and VM in Unity 6000.4.6f1; the final editor state is
  `MainMenu.unity`, Edit Mode, not dirty, with zero Unity errors or warnings.
