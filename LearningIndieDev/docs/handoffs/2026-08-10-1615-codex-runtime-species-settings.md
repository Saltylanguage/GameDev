# runtime species settings

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: codex
- Branch: GridDesignWork
- Date: 2026-08-10
- Feature: Runtime species rule configuration and reset flow

## Summary

Added a runtime settings screen to the species preview. The screen edits draft
values for Plant, Herbivore, and Carnivore rules and applies them when the first
simulation is started.

## Included controls

- Species selector for Plant, Herbivore, and Carnivore.
- Boolean toggles for movement, attack, reproduction, wilt, and seed drops.
- Numeric text fields for speed, attack/block values, reproduction requirements,
  energy, crowding cost, food reserve, and chances.
- Pattern selectors for Cardinal and Moore patterns.
- Diet-target selector for None, Plant, Herbivore, or Carnivore.

## Reset behavior

- Normal next-run progression preserves purchased player upgrades and does not
  overwrite the session's applied rules.
- The Rewards screen now includes `RESET TO SETTINGS`.
- Reset discards the current run/progression session, returns to the settings
  screen, and generates a new seed when randomization is enabled.
- The active run seed remains visible in the runtime header.

## Validation

- `dotnet build LearningIndieDev.slnx --no-restore`: succeeded with 0 errors and
  the existing `DelegateCommand` warning.
- `git diff --check`: passed.
- Unity visual validation remains pending.

## Risks and follow-up

- This remains an immediate-mode prototype UI; layout should be visually checked
  at the target Game view resolutions.
- Invalid numeric text is left visible while the last valid value remains active.
- A future settings asset could persist named configurations once the rule set is
  stable enough to save and share.
