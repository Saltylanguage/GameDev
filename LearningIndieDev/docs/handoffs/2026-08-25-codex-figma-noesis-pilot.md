# Figma to Noesis pipeline pilot

[Working state](../WORKING_STATE.md) | Status: partial-pilot

- Owner: Codex
- Branch: `codex/figma-noesis-pilot`
- Baseline commit: `b3a083fe`
- Date: 2026-08-25

## Summary

Created an isolated, runtime-neutral pilot for translating project UI tokens and
component intent from Figma into reusable Noesis resources. The local Noesis
slice is ready for review; the Figma component and screenshot pass remains
incomplete because the connected Starter account reached its MCP call quota.

## Changes

- Created the Figma Draft `GalapagOS UI Design System Pilot` with 24 variables,
  semantic aliases, three text styles, one effect style, and Cover/Foundation
  content.
- Added `Assets/UI/DesignSystem/FigmaNoesisPilotResources.xaml` with semantic
  brushes, scalar spacing, corner radii, text styles, and shared button styles.
- Added `Assets/UI/DesignSystem/FigmaNoesisPilot.xaml` as a standalone preview of
  Primary/Secondary buttons at Small/Medium/Large sizes.
- Added `docs/FIGMA_NOESIS_PILOT.md` with the token, component, naming, and
  handoff contract.

## Decisions and assumptions

- The pilot does not modify application resources, a scene, or an existing view.
- Repository values are the source for the initial palette and 12px window
  radius.
- Figma Web code syntax uses valid `var(--ui-...)` values; the project document
  maps those names to Noesis resource keys because Figma has no XAML syntax slot.
- Native Code Connect publication remains outside this Starter-plan pilot.
- The unverified Figma window shadow was not copied into Noesis speculatively.

## Validation

- Both new XAML files parsed as XML.
- All three new Unity GUIDs are unique in the repository.
- Unity imported both files as `NoesisXaml` assets.
- A settled forced reimport completed with no new Console warnings or errors.
- Figma variable validation found zero broken aliases, missing code syntax values,
  or invalid semantic scopes before the quota was reached.

## Risks and incomplete work

- Figma Starter is limited to three pages and exhausted its MCP call quota during
  this pilot.
- The `Window` component, six `Button` variants, composed Figma preview, metadata
  validation, and screenshots are still pending.
- Neither the Figma page nor the Noesis preview has rendered visual acceptance
  evidence yet.
- The branch was created from a working tree containing unrelated local work;
  only the pilot files belong in the pilot commit.

## Next useful step

After the Figma quota resets, complete the components and screenshot gate, then
render the standalone Noesis preview before deciding whether to promote the
resources into application dictionaries or live views.
