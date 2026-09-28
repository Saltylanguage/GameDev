# CellSim compressed artifact summary

Source: `D:\GameDev\GameDev\LearningIndieDev\artifacts\cellular-experiment-20260928-151418\report.json`  
Summary schema: `1`  
Scenario: `Assets/Data/TestData/ForestEdgeNoFoxDiagnostic.asset` · player `hare` · seeds `10100`–`10104`
Run: `600` ticks · `0` phases × `0` · combat `OpposedRoll` · opportunities `Natural`
Ruleset `1716ad175ec178f89aa6d77bd7006fbc8095031ce65e7e7214eb58158970e005` · registry `` · loadout ``

## Provenance

- Source commit: ``; source tree dirty before/after: `None` / `None`.
- Catalog: ``; contract: ``.
- Metric dictionary: `cellsim-experiment-metrics` version `1`; bundled file `` SHA-256 ``.
- Parse warnings: `none`.

## Run outcomes

| Seed | Final population | Player pop | Currency |
| ---: | --- | ---: | ---: |
| 10100 | fox=0, hare=0, plant=0 | 0 | 0 |
| 10101 | fox=0, hare=0, plant=0 | 0 | 0 |
| 10102 | fox=0, hare=0, plant=0 | 0 | 0 |
| 10103 | fox=0, hare=0, plant=0 | 0 | 0 |
| 10104 | fox=0, hare=0, plant=0 | 0 | 0 |

## Phase averages

Cross-seed means keep the Markdown view compact. Exact per-seed phase windows and every source counter remain in `report-summary.json` under `runs[*].phases`.

| Phase | Samples | Window(s) | Loadout(s) | Mean closing population | Mean activity counters by species |
| ---: | ---: | --- | --- | --- | --- |

## Stat lines

All serialized herbivore stat-line fields are retained in the JSON summary.


## Compression contract

Raw event arrays are omitted after decision-useful aggregates are retained. The source report remains the replay/evidence authority.

- Retained: provenance, catalog/snapshot identity, seed-level outcomes, phase windows, populations, activity counters, stat lines, behavior totals, transitions, deaths, combat distributions, and opportunity counts.
- Omitted raw arrays: `populationHistory`, `behaviorTransitions`, `deathEvents`, `combatRolls`, and `opportunityAudit`.
- Unrecognized report fields: `['coupledSpeciesResponses', 'csvOutputPath', 'finalPopulationSummary']`; unrecognized run fields: `[]`.
- A summary is not causal proof and must not replace a declared experiment method or human balance decision.
