# EX-008 — Reversed upgrade-order follow-up

**Status:** Runs complete; incorporated into the P3 cohesive report
**Scenario:** `ForestEdge.asset`
**Species:** `hare`

This is an exploratory follow-up to EX-007. It adds the missing crowding-only
arm and runs the joint upgrade in the requested order:

`No Upgrade -> Crowding Tolerance -> Crowding Tolerance + Faster Movement`

It was not a new pre-registered AI prediction, so its results are reported as
follow-up evidence rather than scored as a fresh prediction.

## Contract

| Field | Value |
|---|---|
| Training seeds | 1–20 |
| New held-out seeds | 106–110 |
| Combat | `opposed-roll` |
| Attack opportunities | `natural` |
| Experimental export | `bev-experimental` |
| C1 | `upgradeId=crowding-tolerance` |
| CJ1-reversed | `upgradeSequence=crowding-tolerance,faster-movement` |

The no-upgrade control for seeds 1–20 is the validated EX-007 baseline. A fresh
no-upgrade control was run for seeds 106–110 so all five new held-out seeds have
same-seed controls.

## Evidence

The paired follow-up table is `artifacts/EX-008-20260903/paired-deltas.csv`.
The five run bundles are recorded in the audit table in the EX-007
[plain-language report](../EX-007-Predictive-Statline-Interventions/REPORT.md).

Because the original forward-order joint arm used held-out seeds 101–105 and
this reversed-order arm uses 106–110, the follow-up is not an order-only causal
test. The same-held-out-seed comparison is tracked as
[EX-009](../EX-009-Same-Heldout-Order-Comparison/README.md).
