# Species IDs

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: Codex
- Branch: SpeciesBalanceWork
- Baseline commit: 18d24dd9
- Date: 2026-08-11

## Summary

Species identity is now data-driven through stable string-backed `SpeciesId`
values. The simulation can accept a species collection that is not limited to
plant, herbivore, and carnivore while preserving a compatibility path for
existing enum callers.

## Changes

- Added `SpeciesId`, built-in `SpeciesIds`, and legacy conversion helpers.
- Migrated `CellularSimData`, `SpeciesCell`, `SpeciesRules`, the simulation
  stepper, runner, initial-grid factory, run state/results, upgrades, and the
  preview settings dictionaries to `SpeciesId`.
- Initial seeding now walks the configured probability dictionary rather than
  hardcoding three cumulative branches.
- Kept the old `SpeciesArchetype` APIs as `[Obsolete]` shims so Sim can migrate
  callers incrementally.
- Added a custom-ID test using a `scavenger` species.

## Decisions and assumptions

- IDs are ordinal-independent and use exact, trimmed string equality.
- `SpeciesArchetype` is compatibility-only; new code should use `SpeciesIds`
  or construct a validated `SpeciesId`.
- Plant/resource behavior remains keyed to `SpeciesIds.Plant` for now. A
  terrain/resource registry is the next planned item and will remove that
  special case when a second meaningful terrain resource requires it.
- Population snapshots still expose the three existing aggregate counters;
  arbitrary per-species metrics remain TODO-CS-02.

## Validation

- `dotnet build LearningIndieDev/SaltyGame.Runtime.csproj --no-restore`: passed
  (0 errors; expected obsolete compatibility warnings).
- `dotnet build LearningIndieDev/SaltyGame.Tests.csproj --no-restore`: passed
  (0 errors; expected obsolete compatibility warnings).
- Unity batch test launch was attempted but did not produce a result file because
  the project already had Unity processes running; rerun Edit Mode tests after
  the editor is closed.

## Risks and incomplete work

- The old enum and compatibility members should be removed only after other
  callers (including Sim's branch) have migrated.
- The preview still has fixed three-species authoring fields and color choices;
  the domain is ID-based, but fully dynamic UI authoring is not part of this
  pass.

## Next useful step

Follow up with TODO-CS-03: define a small terrain registry that separates
terrain identity/resource behavior from entity identity, then migrate grass
and dirt feeding, passability, and rendering together.
