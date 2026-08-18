# Sprint 0 C2 UX contract

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: Codex
- Branch: `codex/cellular-sprite-tiling`
- Baseline commit: `9cd600e`
- Date: 2026-08-18

## Summary

Locked the smallest Sprint 1 player-facing route and its low-fidelity layout
contract: Main Menu → Lab Overview → Research preview. The contract keeps
representative data explicit, makes invalid states readable, and preserves the
same information order at 1920×1080 and 1280×720.

## Changes

- Added [`SPRINT_0_C2_UX_CONTRACT.md`](../SPRINT_0_C2_UX_CONTRACT.md).
- Defined Main Menu, Lab Overview, and Research-preview destinations and Back
  behavior.
- Defined keyboard/mouse focus order, visible focus requirements, overlays, and
  disabled-action behavior.
- Defined a deterministic representative data fixture with Research, Plant,
  Herbivore, and Carnivore balances plus one affordable and one
  locked/unaffordable Herbivore project.
- Added structural wireframes for 1920×1080 and a stacked 1280×720 layout.

## Decisions and assumptions

- Sprint 1 shows `Enter Lab` as the primary Main Menu action; Settings and
  Credits are visibly prototype-only, and Quit requires confirmation.
- The scientific-data bar appears on Lab surfaces, not Main Menu.
- Research purchase is a disabled prototype action; selecting a project never
  mutates balances or persistence.
- The broader delivery plan remains the source for future Lab surfaces, but
  C2 does not expand the Sprint 1 route to Species Archive or Expedition Setup.

## Validation

- Cross-checked the contract against `PRODUCT_BRIEF.md`,
  `VERTICAL_SLICE_SELECTION.md`, `MAIN_MENU_LAB_DELIVERY_PLAN.md`, and the
  authoritative `SPRINT_1_PLAN.md`.
- Confirmed the board C2 card scope matches the documented deliverables.
- `git diff --check` is pending after the C2 documentation changes.

## Risks and incomplete work

- The Unity/Noesis scene readiness result is still C3, not part of C2.
- Final visual styling and actual purchase behavior remain intentionally
  deferred.

## Next useful step

Execute C3 as a bounded `MainMenu.unity`/Noesis readiness spike using this
screen-state and representative-data contract.
