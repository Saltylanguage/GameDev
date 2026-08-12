# Handoff: CellSim automation and first baseline

**Owner:** Josh / Codex
**Date:** 2026-08-11
**Branch:** `codex/unity-simulation-tooling`

## Delivered

- Expanded the project-root command into a small automation API:
  `CellSim Help`, `Test`, `Run`, `Report`, `Compare`, and `Baseline`.
- `Baseline` runs the complete Unity test suite, performs a seeded simulation,
  and writes readable Markdown analytics beside the machine-readable JSON.
- Reports now include final-population ranges/extinction rates, start/midpoint/end
  average populations, per-seed outcomes, test-suite totals, and comparison
  deltas. Comparisons explicitly warn when seed ranges do not match.
- Unity batch execution now uses a waited process result instead of relying on
  PowerShell's inconsistent GUI-process exit variable. The test runner no longer
  passes `-quit`, which had caused Unity to exit before executing tests; the
  cellular experiment runner explicitly exits after writing its report.

## First real baseline

Command:

```powershell
.\CellSim.cmd Baseline -SeedStart 1 -SeedCount 20
```

Scenario: fresh default `CellularSimData` snapshot, 32 x 20 grid, 20 seconds,
0.1-second step interval, player species `herbivore`.

| Metric | Result |
| --- | --- |
| Ruleset fingerprint | `9ad6c5afe84669e2c6becc1ab45d76fa5db4bfbe7b28119aa25b0a02659b533f` |
| Edit Mode | 75 / 75 passed |
| Play Mode | 4 / 4 passed |
| Plants | extinct in 20 / 20; final average 0 |
| Herbivores | extinct in 20 / 20; final average 0 |
| Carnivores | survived in 20 / 20; final average 484.4 (range 454–511) |

Average population trajectory:

| Stage | Tick | Carnivore | Herbivore | Plant |
| --- | ---: | ---: | ---: | ---: |
| Start | 0 | 21.65 | 75.8 | 146.55 |
| Midpoint | 100 | 484.1 | 0 | 0.05 |
| End | 200 | 484.4 | 0 | 0 |

The current default ruleset is therefore not a stable three-species equilibrium.
Treat this report as the reference point for one-variable balance experiments;
it is not a recommendation to change every species simultaneously.

## Test maintenance found by the batch suite

- A deterministic simulation test used the obsolete `Species` enum accessor on
  a cell that can be empty after movement. It now compares stable `SpeciesId`.
- The retained Island Survivor Play Mode test expected seven interaction targets,
  while `WorldRuntime` intentionally creates eight. Its action indices remain
  unchanged; the target-count assertion now reflects the current scene contract.

## Evidence and next step

The local, ignored full report is generated at:

`artifacts/cellular-experiment-20260811-223303/analysis.md`

Run a trial with exactly seeds 1–20 after changing one scenario value, then use
`CellSim Compare` with the baseline and trial `report.json` paths. Do not treat
a comparison with different seed ranges as A/B evidence.
