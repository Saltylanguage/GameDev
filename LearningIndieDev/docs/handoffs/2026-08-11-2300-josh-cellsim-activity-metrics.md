# Handoff: CellSim activity metrics and layered population accounting

**Owner:** Josh / Codex
**Date:** 2026-08-11
**Branch:** `codex/unity-simulation-tooling`

## Delivered

- Added per-species, run-total telemetry to `CellSim` JSON and Markdown reports:
  births, food consumed, movement steps, combat damage/kills, total deaths, and
  directly resolved starvation, crowding, wilt, and population-cap deaths.
- Added focused Edit Mode coverage for births, food consumption, combat damage,
  starvation, wilt, and terrain-resource population counting.
- Corrected a terrain/resource identity leak: after a creature leaves a grass
  tile, the tile retains its resource species instead of the departed creature's
  species.
- Population snapshots now count a creature and its passable grass resource as
  separate layers. This matches the simulation model: grass is terrain/resource,
  not a blocking creature.

## Replacement 20-seed baseline

Command:

```powershell
.\CellSim.cmd Run -SeedStart 1 -SeedCount 20
.\CellSim.cmd Report -ReportPath artifacts\cellular-experiment-20260811-225332\report.json
```

Ruleset fingerprint: `9ad6c5afe84669e2c6becc1ab45d76fa5db4bfbe7b28119aa25b0a02659b533f`

| Species | Final avg. | Extinct runs | Births/run | Food/run | Deaths/run |
| --- | ---: | ---: | ---: | ---: | ---: |
| Carnivore | 35.35 | 3 / 20 | 2155.55 | 584.10 | 2141.85 |
| Herbivore | 0 | 20 / 20 | 612.20 | 2036.83 | 688.00 |
| Plant | 616.65 | 0 / 20 | 936.05 | 0 | 465.95 |

The report ledger reconciles for every species: initial population + births -
deaths equals final population. The earlier population-only baseline is not
comparable because terrain resources were previously counted as the species of
the last creature to leave their tile.

## Evidence

- Edit Mode: 75 / 75 passed (`artifacts/unity-tests-20260811-225306`)
- Play Mode: 4 / 4 passed (`artifacts/unity-tests-20260811-225438`)
- Local ignored report:
  `artifacts/cellular-experiment-20260811-225332/analysis.md`

## Next use

Use this report as the new controlled baseline for one-variable balance trials.
The immediate evidence is that herbivores consume abundant plant food but still
go extinct, while carnivores show a very high birth/death turnover. Do not add
per-tick event telemetry yet; add it only when a timing-specific balance question
cannot be answered from the current cumulative totals and population history.
