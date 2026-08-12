# Handoff: Unity simulation tooling

**Owner:** Josh / Codex
**Date:** 2026-08-11
**Branch:** `codex/unity-simulation-tooling`
**Status:** implementation complete; closed-editor Unity batch validation pending

## Purpose

Provide a small, reproducible execution layer for Unity tests and seeded
cellular-simulation experiments. This is meant to support design iteration and
AI-assisted analysis without coupling the simulation to editor automation.

## Delivered

- `tools/Invoke-UnityTests.ps1` runs Edit Mode, Play Mode, or both through Unity
  batch mode and emits NUnit XML plus logs under ignored `artifacts/` output.
- `tools/Run-CellularExperiment.ps1` executes a scenario over a controlled seed
  range and writes a machine-readable population report.
- `Assets/Editor/SimulationTools/CellularSimulationExperimentRunner.cs` is the
  batch-mode Unity entry point. It loads an optional `CellularSimDataAsset`, runs
  the normal initial-grid factory and simulation runner, and writes the full
  population timeline plus final-population metrics.
- `SpeciesInitialGridFactory` now sorts species IDs before initial selection and
  fallback selection. This removes dictionary-insertion-order dependence from
  initial-grid creation for logically identical rulesets.
- Focused runtime coverage verifies the same seed creates matching grids when
  equivalent scenario dictionaries were inserted in a different order.
- [`UNITY_SIMULATION_TOOLING.md`](../UNITY_SIMULATION_TOOLING.md) documents the
  capability set, commands, artifacts, safety boundaries, and limitations.

## Safety boundaries

- The PowerShell entry points refuse to run when `Temp/UnityLockfile` is present.
  They never close Unity, delete the lock, or modify an active editor session.
- Generated output is restricted to `LearningIndieDev/artifacts/`, which is
  ignored by Git.
- No custom runtime agent bridge, global event bus, plugin framework, or
  speculative visualization pipeline was introduced.

## Validation

- `dotnet build SaltyGame.Runtime.csproj --no-restore -v:q` passed with existing
  obsolete-compatibility warnings only.
- `dotnet build SaltyGame.Tests.csproj --no-restore -v:q` passed with existing
  obsolete-compatibility warnings only.
- A static `Assembly-CSharp-Editor` build including the new experiment runner
  passed with no errors (existing compatibility/Noesis warnings only). Its
  temporary generated output was removed afterward.
- All three PowerShell files pass syntax parsing. Their real command entry points
  were invoked and correctly refused to run while the Unity lock file was present.
- `git diff --check` passed.
- Full Unity batch tests and an experiment run are intentionally **not yet run**:
  the Unity Editor was open and its lock file was present. Close Unity, then run:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Invoke-UnityTests.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Run-CellularExperiment.ps1 -SeedStart 1 -SeedCount 5
```

Record the actual output paths and any failures in the next handoff or commit
description. Do not delete the lock file to bypass this guard.

## Next useful step

Run a small fixed-seed baseline report, change one species rule, run the same
seed range again, and compare extinction rates and population trajectories. Add
new telemetry only if that comparison cannot answer the design question.
