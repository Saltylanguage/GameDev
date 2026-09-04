# Authored upgrade prediction-input adapter

**Status:** Implementation and elevated verification complete
**Updated:** 2026-09-04

## What changed

- Added `SpeciesUpgradePredictionInputAdapter` under
  `Assets/Scripts/Game/Species/`.
- Added `SpeciesUpgradeLoadoutFingerprint` to give an ordered set of resolved
  snapshots one deterministic identity.
- Added `-UpgradeAssetSequence` and the optional
  `-UpgradeAssetCatalogPath` options to `tools/Run-CellularExperiment.ps1` and
  the Unity experiment runner.
- Authored research runs now resolve IDs from either the production catalog or
  an explicitly named research fixture catalog, apply immutable snapshots in
  the requested order, pass those snapshots into every run state, and write the
  prediction input and fingerprints into `report.json`.
- Editor callers that already hold a Scriptable Object list can use
  `CreateInputFromAssets` or `SerializeAssets`; runtime state still receives
  snapshots rather than asset references.
- Legacy `-UpgradeId` and `-UpgradeSequence` remain available for historical
  experiments and diagnostic arms. The new and legacy options cannot be mixed.
- Migrated EX-007's prediction template and preregistered prediction to the
  snapshot-shaped intervention format. Added research-only EX-007 fixture
  assets so the migration preserves the original legacy values instead of
  silently substituting newer production upgrades.

## Verification

The focused elevated Edit Mode gate passed **200/200** tests:
`artifacts/unity-tests-20260904-191205/EditMode-results.xml`.

The adapter-backed EX-007 arms passed bundle validation and StatLine validation
with limitations:

- `artifacts/cellular-experiment-20260904-191654` (S1, seeds 1–20)
- `artifacts/cellular-experiment-20260904-191915` (J1, seeds 1–20)
- `artifacts/cellular-experiment-20260904-192118` (S1, seeds 101–105)
- `artifacts/cellular-experiment-20260904-192239` (J1, seeds 101–105)

The EX-009 same-held-out order comparison also completed through the adapter:

- `artifacts/cellular-experiment-20260904-192559` (A,
  `faster-movement,crowding-tolerance`)
- `artifacts/cellular-experiment-20260904-192703` (B,
  `crowding-tolerance,faster-movement`)

All five EX-009 pairs are exact matches after excluding the intentionally
different ordered loadout record. The pairwise result is recorded in
`docs/Research/Experiments/EX-009-Same-Heldout-Order-Comparison/`.

## Provenance note

Historical EX-007 artifacts remain unchanged. They use schema 21, while the
adapter-backed reruns use schema 23 and current telemetry code. Core simulation
payloads match where the ruleset is unchanged; derived StatLine fields were
recalculated and must be compared only within the matching telemetry version.
