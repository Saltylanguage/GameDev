# ruleset-fingerprints

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Owner: josh
- Branch: SaltysFirstBranch
- Baseline commit: e4c15b39
- Date: 2026-08-11

## Summary

Implemented stable ruleset fingerprints so A/B runs can be identified and
reproduced from their exact `CellularSimData` snapshot.

## Changes

- Added `CellularSimDataFingerprint` with versioned canonical serialization and
  SHA-256 hashing.
- Included global settings, starting probabilities, every species rule and
  pattern offset, and terrain definitions/presentation data.
- Sorted dictionary entries by stable IDs and formatted numbers invariantly.
- Added `CellularSimData.Fingerprint`.
- Data-backed `SpeciesSimulationRunner` instances attach the fingerprint to
  `SimulationRunState` and `SimulationRunResult`.
- Added tests for dictionary-order independence, ruleset sensitivity, and run
  result propagation.
- Marked TODO-CS-05 complete.

## Decisions and assumptions

- The fingerprint schema is explicitly versioned as `cellular-sim-data-v1`.
  Change the version when canonical fields or encoding semantics change.
- Pattern offset order is significant and preserved because it can affect
  deterministic tie-breaking in future rule consumers.
- SHA-256 is used for stable comparison metadata, not security or identity.
- Legacy runners constructed only from a rules dictionary do not claim a full
  scenario fingerprint because global and terrain data are unavailable.

## Validation

- `dotnet build SaltyGame.Runtime.csproj --no-restore` passed with zero errors;
  the generated project was temporarily supplied the new source file for this
  check and that generated-project edit was removed afterward.
- `dotnet build SaltyGame.Tests.csproj --no-restore` passed with zero errors.
- `git diff --check` passed.
- Unity Edit Mode execution was not run because the Unity editor process was
  already open on this project.

## Risks and incomplete work

- The canonical schema must be version-bumped if future `CellularSimData`
  fields are added or the meaning of an existing field changes.
- A future persisted run format may need to store both the fingerprint version
  and the hash explicitly rather than only the combined data fingerprint.

## Next useful step

Use the fingerprint in the balance/A-B experiment report path, or activate the
next concrete requirement that needs custom rule logic or editor authoring.
