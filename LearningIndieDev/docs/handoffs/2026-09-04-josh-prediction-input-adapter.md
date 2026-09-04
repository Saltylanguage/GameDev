# Authored upgrade prediction-input adapter

**Status:** Implementation complete; elevated Unity verification is pending
while the Unity Editor is open.

## What changed

- Added `SpeciesUpgradePredictionInputAdapter` under
  `Assets/Editor/SimulationTools/`.
- Added `SpeciesUpgradeLoadoutFingerprint` to give an ordered set of resolved
  snapshots one deterministic identity.
- Added the `-UpgradeAssetSequence` option to
  `tools/Run-CellularExperiment.ps1` and the Unity experiment runner.
- Authored research runs now resolve IDs from the production catalog, apply the
  immutable snapshots in the requested order, pass those snapshots into every
  run state, and write the prediction input and fingerprints into `report.json`.
- Editor callers that already hold a Scriptable Object list can use
  `CreateInputFromAssets` or `SerializeAssets`; runtime state still receives
  snapshots rather than asset references.
- Legacy `-UpgradeId` and `-UpgradeSequence` remain available for historical
  experiments and diagnostic arms. The new and legacy options cannot be mixed.
- Migrated EX-007's prediction template and preregistered prediction to the
  snapshot-shaped intervention format. Added research-only EX-007 fixture
  assets so the migration preserves the original legacy values instead of
  silently substituting newer production upgrades.

## How to use it

From `LearningIndieDev`, with Unity closed:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Run-CellularExperiment.ps1 `
    -ScenarioPath Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset `
    -SeedStart 10100 `
    -SeedCount 20 `
    -PlayerSpeciesId hare `
    -UpgradeAssetSequence trailblazer-long-stride,warren-guarded-burrow
```

The IDs are stable upgrade IDs, not file names or asset paths. The adapter
rejects unknown IDs, duplicate IDs, invalid production assets, and upgrades
that target a different player species.

EX-007's fixtures are intentionally resolved from an explicit asset list rather
than the production catalog because its historical `faster-movement` and
`crowding-tolerance` arms predate the current production catalog.

## Report evidence

Schema 22 authored reports include:

- `predictionInput`: ordered IDs plus each snapshot's modifiers, prerequisites,
  exclusions, contract version, registry fingerprint, and snapshot fingerprint.
- `upgradeLoadoutFingerprint`: deterministic identity that changes when the
  snapshot values or order changes.
- Per-run `upgradeLoadout` records using the same snapshot serializer as the
  player-facing run path.

## Verification

The new Edit Mode tests cover catalog resolution, serialization, unknown and
duplicate IDs, and order-sensitive loadout fingerprints. Run the full elevated
Edit Mode suite after closing Unity. The verified gate is
`artifacts/unity-tests-20260904-165536/EditMode-results.xml` (**198/198**).
The end-to-end authored run is
`artifacts/cellular-experiment-20260904-170039/report.json`; it contains schema
22 `predictionInput` metadata and matching per-run snapshot records for
`trailblazer-long-stride,warren-guarded-burrow`.
