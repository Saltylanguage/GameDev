# EX-007 bounded AI input

**Prediction ID:** `PRED-EXP-007-0001`  
**Status:** Pre-registration template  
**Baseline artifact:** `artifacts/.../report.json`  
**Baseline validation:** Pending  
**Prepared before trial results:** Yes / No

This is the complete input boundary for the prediction step. Attach the
validated baseline report or a generated summary at the marked location. Do not
add trial reports or human conclusions.

## Baseline

- Baseline report or summary: `TBD`
- Source commit and ruleset fingerprint: `TBD`
- Schema version: `TBD`
- Training seed panel: `1-20`
- Scenario/configuration: ForestEdge, Hare, 32x32, 20s, 0.1s, opposed-roll,
  natural opportunities, `bev-experimental`

## Experiment contract

Read [README.md](README.md). The permitted arms are:

- B: no upgrade;
- S1: one `species-upgrade-prediction-input-v1` snapshot for
  `faster-movement`, with `movement.speed +0.5`;
- J1: one ordered `species-upgrade-prediction-input-v1` loadout containing
  `faster-movement` (`movement.speed +0.5`) followed by
  `crowding-tolerance` (`crowding.tolerance +1`).

The intervention objects in the prediction record must come from
`SpeciesUpgradePredictionInputAdapter` (or an equivalent serialized snapshot),
not from a hand-written list of values. EX-007 uses research-only fixture assets
under `Assets/Data/CellularSimulation/Upgrades/Research/EX-007/` so the legacy
arms remain exactly comparable to their historical reports.

No continuous extrapolation is permitted.

## Available telemetry

The report may expose the following fields for each seed:

- Statline: `SPO`, `HPS`, `EHS`, `ECN`, `PREY` (Hares killed by carnivores),
  `STRV`, `MAT`, `BIR`, `CRWD`, `FPO`, `pAVI` (survival across recorded
  predator encounters), `eAVI`, `predAVG`, `sAVI`, `cAVI`, `bAVG`, `RFS`, `APS`,
  and each metric's validity status.
- Population history by species and tick.
- Activity counters: births, food actions, movement steps, combat attempts,
  hits, blocked attacks, lethal/non-lethal hits, deaths, and reproduction
  funnel counters.
- Death events with species, tick, location, entity identity, and proximate
  cause.
- Opposed combat rolls and cooldown suppressions when present.
- Manifest provenance, schema, configuration, and ruleset fingerprint.

## Required prediction fields

Before seeing S1 or J1 results, predict for each intervention:

- direction (`increase`, `decrease`, `no_material_change`, or `unresolved`);
- approximate effect band and its unit;
- affected outcomes;
- uncertainty and confidence from `0.0` to `1.0`;
- candidate causes, clearly labelled as candidates; and
- limits, unsupported outcomes, and range/scenario caveats.

Do not decide whether an effect is desirable. That belongs in the human
decision record.
