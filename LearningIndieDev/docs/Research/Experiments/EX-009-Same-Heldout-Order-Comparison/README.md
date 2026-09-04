# EX-009 — Same-held-out-seed upgrade-order comparison

**Experiment ID:** `EXP-009`  
**Status:** Both same-seed arms complete; order result available; human review pending
**Parent:** `EXP-007` / exploratory follow-up `EXP-008`  
**Decision owner:** Human design owner  
**Scenario:** `Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset`  
**Species:** `hare`

## Question

When the same two upgrades are applied in opposite orders, do the outcomes
change on the same held-out seeds?

## Hypothesis

If `faster-movement` and `crowding-tolerance` are independent rule changes,
the forward and reversed sequences should produce identical or equivalent
same-seed outcomes. Any paired difference is evidence that order, application
state, or another implementation detail matters.

## Locked comparison

| Arm | Ordered loadout | Seeds |
|---|---|---:|
| A | `faster-movement,crowding-tolerance` | 106–110 |
| B | `crowding-tolerance,faster-movement` | 106–110 |

Both arms use ForestEdge, Hare, 32x32, 20.0 seconds, 0.1-second steps,
`opposed-roll` combat, `natural` attack opportunities, `bev-experimental`, the
same source revision, and the same report/statline validators. Both arms were
resolved through the authored-upgrade adapter and the explicit research fixture
catalog at `Assets/Data/CellularSimulation/Upgrades/Research/EX-007`.

## Success criteria

- Both arms produce complete, validated `report.json`, `report.csv`,
  `statline.csv`, `manifest.json`, and `unity.log` bundles.
- Every seed has an A/B pair with matching provenance except for ordered
  loadout and derived ruleset fingerprints.
- Compare final population, births, starvation, crowding, predation, movement,
  resource, and available encounter telemetry per seed.
- Report exact paired deltas and whether any difference is consistent across
  the five held-out seeds.
- Do not claim order independence unless the same-seed comparison supports it.

## Process corrections carried forward

1. **Same held-out panel:** Both orders use seeds 106–110. The prior EX-007 and
   EX-008 held-out panels differed, so they cannot answer this question.
2. **Metric definitions:** `PREY` is Hare deaths caused by carnivores, not food
   gathering. The comparison must use the corrected definition.
3. **No silent extrapolation:** This is a discrete two-order comparison only;
   it does not establish continuous scaling, cross-scenario transfer, or
   balance/fun conclusions.
4. **Evidence gate:** An incomplete Unity run is a failure record, not research
   evidence. No preflight bypass is permitted.
5. **Separate interpretation:** A/B differences describe model behavior; they
   do not decide whether either order is desirable for the game.

## Execution result

Both arms completed on 2026-09-04 through the same adapter-backed path. Each
produced a complete schema-23 bundle and passed the artifact validator. The
independent StatLine validator returned `VALIDATED_WITH_LIMITATIONS` for both
arms, which is the expected status for the accumulated-counter limitations in
this telemetry version.

| Arm | Artifact | Bundle | Statline |
|---|---|---|---|
| A | `artifacts/cellular-experiment-20260904-192559` | Valid | Validated with limitations |
| B | `artifacts/cellular-experiment-20260904-192703` | Valid | Validated with limitations |

The two reports have the same scenario, ruleset fingerprint, run settings, and
seed panel. Their ordered loadout and loadout fingerprints differ as intended;
the research catalog path and registry fingerprint match.

## A/B result

All five same-seed pairs were identical across the available run evidence after
excluding the intentionally different ordered loadout record. Final Hare,
Fox, and Plant populations, births, predation, starvation, crowding, movement,
food, and available encounter/statline measures all have an A-minus-B delta of
zero. The machine-readable comparison is in
[`paired-deltas.csv`](paired-deltas.csv).

| Metric | Arm A mean | Arm B mean | Mean A-minus-B |
|---|---:|---:|---:|
| Final Hare population (FPO) | 78.8 | 78.8 | 0.0 |
| Hare births (BIR) | 88.2 | 88.2 | 0.0 |
| Hare predation deaths (PREY) | 5.8 | 5.8 | 0.0 |
| Hare starvation deaths (STRV) | 25.6 | 25.6 | 0.0 |
| Hare crowding deaths (CRWD) | 0.0 | 0.0 | 0.0 |
| Hare movement steps | 18,119.4 | 18,119.4 | 0.0 |
| Hare food consumed | 14,307.6 | 14,307.6 | 0.0 |
| Resource-finding score (RFS) | 0.519920 | 0.519920 | 0.000000 |
| Activity per step (APS) | 0.955246 | 0.955246 | 0.000000 |

This supports an order-independent result for these two additive upgrades,
values, scenario, ruleset, telemetry, and held-out seeds. It is not a universal
claim that every future upgrade combination commutes; shared state, caps,
multipliers, prerequisites, or side effects would require their own test.

## Historical preflight record

The two failed 2026-09-03 forward-arm attempts remain useful operational
history, but they are not experimental evidence. Their logs are preserved at:

- `artifacts/unity-preflight-20260903-173849/license-probe.log`
- `artifacts/unity-preflight-20260903-174035/license-probe.log`

The earlier EX-008 reverse arm and the prior forward arm on seeds 101–105 are
also retained as historical context only; they were not used for this order
comparison.
