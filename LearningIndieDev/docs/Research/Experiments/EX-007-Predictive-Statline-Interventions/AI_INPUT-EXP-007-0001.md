# EX-007 bounded AI input — PRED-EXP-007-0001

**Status:** Pre-registered before intervention runs  
**Baseline:** `artifacts/cellular-experiment-20260903-152434/report.json`  
**Baseline validation:** `VALIDATED_WITH_LIMITATIONS` via the independent statline validator  
**Baseline configuration:** ForestEdge, Hare, 32x32, 20s, 0.1s, opposed-roll, natural opportunities, `bev-experimental`  
**Training seeds:** 1–20  
**Held-out seeds:** 101–105  
**Ruleset fingerprint:** `5ee5dca0c0a6345e0af5342ffa6717cbcf98a6c9f7e70f6ec5d0d6b0a8563aea`

> **Post-run measurement erratum:** `PREY` in the exported statline means Hare
> deaths caused by carnivores, not successful food/resource events. The original
> prediction values are retained unchanged; their serialized intervention
> representation has been migrated to the snapshot contract. The PREY forecast
> is excluded from
> causal scoring because the metric was misread at setup time.

## Baseline summary

- Mean final Hare population (`FPO`): **70.30** (range 51–91)
- Mean births (`BIR`): **72.45** (range 47–99)
- Mean starvation deaths (`STRV`): **15.95** (range 4–31)
- Mean crowding deaths (`CRWD`): **1.50** (range 0–4)
- Mean resource-finding score (`RFS`): **0.63**
- Mean activity-per-step (`APS`): **0.91**

## Contract and permitted interventions

- **B:** no upgrade.
- **S1:** the serialized snapshot input for `faster-movement`, with
  `movement.speed +0.5`.
- **J1:** one ordered serialized snapshot input containing `faster-movement`
  (`movement.speed +0.5`) followed by `crowding-tolerance`
  (`crowding.tolerance +1`).

The intervention values are represented by the adapter-shaped snapshot object
in `PREDICTION-EXP-007-0001.json`. These are research-only fixtures matching
the legacy EX-007 arms; they are not the newer production `Trailblazer` assets.

No continuous extrapolation, balance recommendation, or desirability judgment is permitted.

## Available telemetry

Statline fields and validity statuses; population history; activity counters;
death events; opposed combat rolls; cooldown suppressions; manifest provenance;
schema; configuration; and ruleset fingerprint.

## Prediction boundary

This input contains only the validated baseline, the experiment contract, the
permitted interventions, and the telemetry allowlist. Trial reports and human
conclusions were not available when the prediction was recorded.
