# species simulation UI flow

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: codex
- Branch: GridDesignWork
- Baseline commit: d9e9b577
- Date: 2026-08-10

## Summary

Expanded the species simulation preview into a first-pass player-facing loop. The prototype now has a larger readable control panel, explicit run states, three upgrade choices, and a player-update screen before the next run.

## Changes

- Added `Ready`, `Running`, `Rewards`, and `Results` preview states.
- Replaced the small start/reward controls with larger panel buttons and expanded panel/card layouts.
- Added three purchasable upgrade choices: movement speed, attack amount, and block amount.
- Applied the selected upgrade to the player species and displayed updated stats before the next run.
- Added a minimal prototype background camera to remove the no-rendered-camera warning without re-enabling cave rendering.

## Decisions and assumptions

- UI remains an exploratory `OnGUI` implementation for rapid iteration; production UI migration can happen after the flow is validated.
- The player can select one upgrade after each completed run, or continue without an upgrade.
- Upgrade choices currently cost five currency each and use the existing progression model.
- Typography is intentionally scaled to approximately 200% of the original prototype sizing.

## Validation

- `dotnet build LearningIndieDev.slnx --no-restore`: succeeded with 0 errors and the existing `DelegateCommand` warning.
- `git diff --check`: passed.
- Unity Play Mode was not run in this session.

## Risks and incomplete work

- Panel dimensions are tuned for a large Game view and may need responsive layout work at smaller resolutions.
- The UI uses immediate-mode controls and placeholder styling; it is not yet wired to Noesis.
- Reward balancing and upgrade names/effects are prototype values.

## Next useful step

Run the prototype through several complete cycles, verify the reward and stat transitions visually, then decide which UI elements should move into authored Noesis views.
