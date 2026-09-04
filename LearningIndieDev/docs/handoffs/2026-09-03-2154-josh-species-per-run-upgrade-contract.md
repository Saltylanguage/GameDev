# Species per-run upgrade contract

[Working state](../WORKING_STATE.md) | Status: in-progress

- Owner: Josh
- Branch: NF/UpgradeContract
- Baseline commit: e1a86701
- Date: 2026-09-03

## Summary

Implemented the first production-quality contract seam for species per-run
upgrades. Scriptable Objects are authoring data only; the simulation consumes a
validated immutable snapshot backed by one explicit stable-ID registry.

## Changes

- Added `SpeciesUpgradeSnapshot` with one target species, signed additive
  modifiers, prerequisites/exclusions, per-run scope, and deterministic
  fingerprinting.
- Added `SpeciesAttributeRegistry` and stable `SpeciesAttributeIds` covering the
  current species-rule attributes with explicit runtime application and integer
  value validation.
- Added `SpeciesUpgradeAsset` as the Unity authoring adapter. It resolves once
  into the immutable snapshot and reports actionable validation failures.
- Added focused Edit Mode tests for registry definitions, multi-modifier
  application, fingerprint order sensitivity, and invalid IDs/duplicates.
- Preserved the existing `SpeciesUpgrade`/`SpeciesProgression` implementation;
  the new snapshot path is additive and does not change permanent-upgrade or Lab
  behavior.
- Wired ordered snapshots through `SimulationLaunchRequest`, the preview launch
  boundary, `SpeciesProgression`, `SpeciesSimulationRunner`, and
  `SimulationRunResult`.
- Added atomic launch preflight so malformed ordered loadouts cannot apply a
  partial prefix. Prerequisite/exclusion self-links and duplicates are rejected
  by the contract, and explicit legacy ID lists must match snapshot IDs when
  both forms are supplied.
- Added registry-backed editor dropdowns with inline contract validation.
- Added ordered upgrade metadata to Play Mode and experiment report
  serialization, including modifier values and fingerprints.

## Decisions and assumptions

- V1 supports only signed additive modifiers: positive/`+` increases and `-`
  decreases. Multiply/set/range/clamp/conditional operations are deferred.
- Every upgrade targets exactly one species. Cross-species upgrades are deferred.
- Runtime application uses the registry's explicit mapping, never reflection or
  field-name discovery.
- Duplicate attribute entries within one upgrade are rejected to keep the
  contract unambiguous.

## Validation

- `git diff --check` passed.
- Runtime and test assemblies compile with `dotnet build --no-restore`. The
  generated Unity Editor project intentionally cannot be built by dotnet because
  it disables standard framework references.
- An initial run was blocked by an already running Unity Editor (PID 9396),
  and the tooling correctly refused to launch a second instance. After the
  editor was closed, the elevated suite completed 187/187 Edit Mode tests.
  The run also corrected an existing test-fixture coordinate typo in
  `BoardSnapshotCopiesCellsAndSpeciesRoles`; the final result is in
  `artifacts/unity-tests-20260904-000457/EditMode-results.xml`.
- The elevated Play Mode suite completed 14/14 runnable tests with one expected
  graphics-dependent test skipped (`CellularPrototypeInitializesEveryAuthoredAnimalSprite`).
  Its result is in the latest `artifacts/unity-tests-*/PlayMode-results.xml`.
- Re-run the suite with:
  `powershell -NoProfile -ExecutionPolicy Bypass -File .\\tools\\Invoke-UnityTests.ps1 -Mode EditMode`

## Risks and incomplete work

- Prediction JSON still uses the existing experiment runner's legacy string
  upgrade options; a future prediction-input adapter can consume immutable
  snapshots when that research workflow is promoted.
- Production catalog assets are intentionally deferred; the editor picker and
  validation path are ready once the first approved content set is defined.

## Next useful step

Close Unity and run the Edit Mode suite. Then author the first approved per-run
upgrade assets and add the prediction-input adapter when that research workflow
is ready to consume contract snapshots.
