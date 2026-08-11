# alpha-ruleset-assets

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: josh
- Branch: SaltysFirstBranch
- Baseline commit: a2967b9c
- Date: 2026-08-11

## Summary

Activated the first bounded implementations of CS-04 and CS-06 without adding a
general rule engine or mutable asset-backed runtime state.

## Changes

- Added `AlphaOffspringRule`: a species-keyed chance to promote a newly created
  non-plant offspring to alpha and add health/energy bonuses.
- Added `SpeciesCell.IsAlpha`; normal simulation updates preserve the flag.
- `CellularSimData` owns immutable alpha-rule data, supports copy-on-edit, and
  includes it in the versioned v2 ruleset fingerprint.
- Added `CellularSimDataAsset`: Unity Inspector definitions convert to fresh
  immutable `CellularSimData` snapshots. The asset exposes globals, arbitrary
  species IDs, rules, offset patterns, and alpha values.
- Added coverage for alpha newborn promotion/fingerprint changes and asset
  snapshot conversion.

## Decisions and assumptions

- Alpha behavior runs at offspring creation, after normal reproduction checks
  and before global population limiting.
- A chance and starting stat bonuses are enough to prove the custom-rule path;
  special diet, inheritance, pack caps, and alpha presentation are deferred.
- The first asset path intentionally uses existing bare/grass terrain defaults.
  The active preview continues to use the three-species runtime settings UI;
  adding an asset selector there would require a separate UI generalization.
- No generic stage interface, plugin registry, delegate callbacks, event bus, or
  scripting system was introduced. Add a shared abstraction only when two real
  mechanics need the same shape.

## Validation

- `dotnet build SaltyGame.Runtime.csproj --no-restore` passed after temporarily
  including newly added Unity scripts in the generated project file.
- `dotnet build SaltyGame.Tests.csproj --no-restore` passed with existing legacy
  obsolete-API warnings only.
- `dotnet test` exits successfully but this generated Unity project does not
  discover/run Unity NUnit tests from the CLI; run the new tests in Unity's Test
  Runner for execution coverage.

## Next useful step

Create an experiment asset from **Assets > Create > Salty Game > Cellular
Simulation Data**, then decide whether the next focused mechanic is alpha
qualification or sight/target selection.
