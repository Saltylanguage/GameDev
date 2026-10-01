# Population configuration cleanup

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-01-1916-codex-population-configuration-cleanup
- Owner: Codex
- Branch: BevBranch
- Baseline commit: ac0b103
- Date: 2026-10-01
- Supersedes: none

## Summary

Population editing and experiment inputs are ready for review on BevBranch.
Grid and population overrides now apply together, and saved preview presets
are preserved. The production Forest Edge asset remains 400 Plants / 55
Hares / 35 Foxes at a 0.2-second step; 400/25/15 at 0.1 seconds is an explicit
experiment input. This work does not implement or validate the S4 basic skills.

## Changes

- Kept Species Catalog starting-population editing through the existing
  serialized-object, Undo and Save Assets path.
- Added `-StartingPopulations` forwarding through `CellSim.ps1` Run/Baseline
  and `tools/Run-CellularExperiment.ps1`; the runner validates a combined
  grid/count configuration with the existing immutable-data helper.
- Removed the ambiguous hardcoded reset of saved 400/55/35 settings.
  Explicit saved v4/v5 presets survive loading; defaults follow the asset.
- Added focused regression tests with isolated assets and restored PlayerPrefs.
- Clarified historical run protocols in WORKING_STATE and packaged a compact
  [historical report](../Research/Reports/2026-10-01-historical-forest-edge-populations/report.md)
  with 240 per-seed rows and complete recorded Hare slash lines.
- The earlier S4-01 workbook is unchanged; see its
  [separate handoff](2026-10-01-1711-codex-s4-01-hare-workbook-review.md).

## Validation

Unity 6000.4.6f1, clean CLI lane, on this working tree before commit:

```powershell
.\CellSim.ps1 -Command Test -Mode EditMode -Execution Clean -TestFilter SaltyGame.EditorTests.PopulationConfigurationTests
.\CellSim.ps1 -Command Test -Mode PlayMode -Execution Clean -TestFilter SaltyGame.PlayModeTests.SpeciesPresentationPlayModeTests.DeveloperSettings
.\CellSim.ps1 -Command Run -Execution Clean -SeedStart 10100 -SeedCount 1 -ScenarioPath Assets/Data/ProductionData/CellularSimulation/Scenarios/ForestEdge.asset -StartingPopulations 'plant=400,hare=25,fox=15' -PlayerSpeciesId hare -RunTicks 2 -StepIntervalSeconds 0.1
```

- EditMode: 10 passed, zero failed/skipped. Checks combined grid/count overrides,
  invalid values, saved presets/defaults and edit/Undo/save/reload behavior.
- PlayMode: 2 passed, zero failed/skipped. Existing developer-settings tests
  cover exact populations and rejection of an invalid population cap.
- Two-tick smoke: seed 10100 started exactly 400/25/15 at 0.1 seconds.
- All 40 complete historical Hare slash lines match the original JSON.
- PowerShell parsing and `git diff --check` passed.
- Production asset and both ProjectSettings files matched HEAD byte-for-byte;
  the workbook retained its approved SHA-256.

Raw current-check artifacts are local and ignored:
`artifacts/unity-tests-20261001-180224/`,
`artifacts/unity-tests-20261001-180800/`, and
`artifacts/cellular-experiment-20261001-180623/`.
The compact historical evidence package is included in the commit.

## Limits and next useful step

Review these configuration changes independently of the S4-01 design workbook.
No full-suite or visual Catalog mouse-interaction acceptance was performed.
The two-tick smoke is input verification, not balance evidence. Historical
Guarded Burrow is a composite Mutation, not the proposed basic Hide skill.
The S4-01 Trello item remains parked awaiting review; this push does not change
its status or approve further balance experiments.
