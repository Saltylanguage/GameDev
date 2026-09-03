# EX-007 factual report

**Status:** Pending execution  
**Prediction ID:** `PRED-EXP-007-0001`

This file will contain only observed results from the training and held-out
paired runs. Keep the AI prediction in `PREDICTION_TEMPLATE.json` (or its
completed copy) and the interpretation in `AI_ANALYSIS.md`.

## Evidence

| Arm | Seed panel | Artifact path | Bundle validation | Statline validation |
|---|---|---|---|---|
| B | 1-20 | `TBD` | Pending | Pending |
| S1 | 1-20 | `TBD` | Pending | Pending |
| J1 | 1-20 | `TBD` | Pending | Pending |
| B | 101-105 | `TBD` | Pending | Pending |
| S1 | 101-105 | `TBD` | Pending | Pending |
| J1 | 101-105 | `TBD` | Pending | Pending |

## Observed outcomes

Populate after all six bundles validate. For each metric, include mean,
median, range, per-seed deltas, and validity/coverage warnings.

## Interaction

For each endpoint, compare the joint result with the two single-arm results.
Do not call a difference an interaction until the paired seed and coverage
checks pass.

## Limits

- No continuous upgrade sweep is covered.
- No scenario transfer is covered unless a second scenario is added.
- Statline HPS/EHS/ECN remain accumulated counters rather than independently
  reconstructable event lists.
