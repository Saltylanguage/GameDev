# EX-011 factual report — Cross-biome, cross-species transfer

**Status:** Execution complete; human decision pending
**Scenario/species:** Open Range / Deer
**Intervention:** `faster-movement` → `crowding-tolerance`, active at launch
**Primary metric:** `herbivore-statline.fpo`

## What this experiment established at a glance

| Question | Plain-language answer |
|---|---|
| What was being tested? | Whether the earlier ForestEdge/Hare direction would carry into a different biome and species when Deer received movement speed +0.5 followed by crowding tolerance +1 at launch. |
| What happened? | The intervention ended with more Deer in all 20 development pairs and all 5 held-out pairs. The average gains were +70.8 Deer and +52.4 Deer, respectively. |
| What did the result verify? | The combined intervention's positive final-population direction transferred to Open Range/Deer under the preregistered settings and seed panels. Human acceptance of that bounded statement is still pending. |
| What did it not prove? | It did not isolate either upgrade's individual effect, establish broad transfer to other species or biomes, predict the size of the effect, or show that the change improves balance, fun, or player value. |

## Result in one sentence

The intervention increased final Deer population in all 20 development pairs
and all 5 held-out pairs, so the preregistered directional transfer rule passed.

This is a measured simulation result, not a balance or player-value decision.

## Primary result

| Panel | Control mean | Intervention mean | Mean paired difference | Median | Range | Positive pairs | Required |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Development, seeds 201–220 | 256.2 | 327.0 | +70.8 | +63 | +15 to +165 | 20/20 | At least 12/20 and positive mean |
| Held-out, seeds 301–305 | 264.4 | 316.8 | +52.4 | +47 | +17 to +94 | 5/5 | At least 3/5 and positive mean |

Exact paired values are in [`PAIRED_FPO_DELTAS.csv`](PAIRED_FPO_DELTAS.csv).

## Secondary observations

| Metric | Development mean difference | Held-out mean difference |
| --- | ---: | ---: |
| `herbivore-statline.aps` | -3.82 | -5.49 |
| `herbivore-statline.rfs` | -3.96 | -5.51 |
| `herbivore-statline.bir` | -783.95 | -822.8 |
| `herbivore-statline.strv` | -315.1 | -305.2 |
| `herbivore-statline.crwd` | -538.5 | -589.8 |

FPO increased while APS, RFS, and births decreased in both panels. These
secondary measurements are descriptive and were not allowed to rescue or
overturn the primary endpoint.

## Prediction score

The sealed prediction required a positive held-out mean and at least three
positive held-out pairs. The observed result was a +52.4 mean with five of five
positive pairs. All four artifact bundles passed validation, so the mechanical
verdict is **supported**.

The human transfer decision remains separate and pending.

## Evidence integrity

- Human-approved contract:
  [`PROPOSAL_DRAFT.md`](PROPOSAL_DRAFT.md), SHA-256
  `dfc38d242f2457d87372a7e8f68b72d0af8e6ed62461cba6fe63546d6d304b1a`.
- AI context was sealed before any Open Range output:
  [`AI_CONTEXT_MANIFEST.json`](AI_CONTEXT_MANIFEST.json).
- Seed roles were sealed before execution:
  [`SEED_REGISTRY.json`](SEED_REGISTRY.json).
- The development interpretation and held-out prediction were sealed before
  seeds 301–305 were run:
  [`DEVELOPMENT_ANALYSIS.md`](DEVELOPMENT_ANALYSIS.md) and
  [`HELD_OUT_PREDICTION.json`](HELD_OUT_PREDICTION.json).
- Every report is schema 26 and identifies metric dictionary version 1.
- Every artifact bundle passed `Test-CellSimArtifactBundle.ps1` with status
  `VALID`.
- Every Stat-Line passed independent formula and available raw-source checks
  with status `VALIDATED_WITH_LIMITATIONS`.
- HPS, EHS, and ECN remain limited because the report does not retain their
  event-level source lists. FPO—the primary metric—was independently checked
  against population history.
- All manifests record source commit
  `993a8440cbd86a4fb14caf7c5a58ed83c0f581eb` and disclose a dirty tree before
  and after each run.

## Raw bundles

| Panel/arm | Artifact directory | Report SHA-256 |
| --- | --- | --- |
| Development control | `artifacts/cellular-experiment-20260912-215056/` | `afef711cc304bc156fe9f89c2abd3bb517863038f6da4f8926c0b38924536149` |
| Development intervention | `artifacts/cellular-experiment-20260912-215333/` | `1c0d24c78c65194314b3c4aa57c9aa9bbb3623e96f045af745535828383da12c` |
| Held-out control | `artifacts/cellular-experiment-20260912-215729/` | `3628b39653253734c57fe96ab9da7cad29746fba053b71433473a3ab985dd492` |
| Held-out intervention | `artifacts/cellular-experiment-20260912-215836/` | `fc15b610f5888defa3e721311634dc145b558457ab86cddf47a2dad64cf6811b` |

The intervention bundles also contain generated `report-summary.json` and
`report-summary.md` files. They preserve decision-useful aggregates but do not
replace the raw reports or prove causality.

## Limits

- This tests one two-variable bundle, not either variable by itself.
- It tests one new species and scenario, not general transfer.
- The prior Hare effect size was not predicted or scored here.
- Five held-out seeds are a smoke test, not a high-powered estimate.
- The result says nothing by itself about fun, balance, player comprehension,
  or production promotion.
