# generalize-population-metrics

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: josh
- Branch: SaltysFirstBranch
- Baseline commit: 0545c19b
- Date: 2026-08-11

## Summary

Population snapshots now support arbitrary species without changing the result
model for each new species. This closes TODO-CS-02 and removes the hardcoded
three-species assumption from the metrics path.

## Changes

- `SpeciesPopulationSnapshot` now exposes read-only `Counts` keyed by
  `SpeciesId` and `GetCount` for missing-safe lookup.
- Snapshot creation counts any creature species and any terrain resource species
  while preserving the separate `Empty` aggregate.
- `Plants`, `Herbivores`, and `Carnivores` remain compatibility accessors.
- Added runtime coverage for a custom creature, custom resource species, built-in
  species, and empty cells.
- Updated `CELLULAR_SIM_TODOS.md` to mark TODO-CS-02 complete.

## Decisions and assumptions

- A resource terrain cell contributes one count to its resource species; it is
  not counted as both terrain and occupant.
- A resource cell without an explicit resource species retains the legacy plant
  fallback for compatibility.
- Snapshot dictionaries are copied into `ReadOnlyDictionary` instances so
  callers cannot mutate historical run data.
- Existing compatibility properties remain until all consumers migrate to
  species-keyed access.

## Validation

- `dotnet build SaltyGame.Runtime.csproj --no-restore` passed with zero errors.
- `dotnet build SaltyGame.Tests.csproj --no-restore` passed with zero errors;
  existing obsolete-API warnings remain.
- `dotnet test SaltyGame.Tests.csproj --no-build --no-restore` returned success,
  but this generated Unity test project does not emit a test discovery summary.
- `git diff --check` passed.
- Unity Edit Mode execution was not run because the Unity editor process was
  already open on this project.

## Risks and incomplete work

- UI and analysis consumers still use the compatibility properties where they
  exist; migrate them when arbitrary species need to be displayed.
- Ruleset fingerprints and richer metrics remain deferred to TODO-CS-05 and
  future analysis requirements.

## Next useful step

Review the keyed snapshot API, then either migrate a concrete UI/graph consumer
or proceed to the next activated simulation requirement.
