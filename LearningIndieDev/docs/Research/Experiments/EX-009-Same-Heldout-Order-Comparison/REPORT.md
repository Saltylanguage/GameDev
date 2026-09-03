# EX-009 — Same-held-out-seed A/B report

**Experiment:** `EXP-009`  
**Report:** `RPT-RUN-009-0001`  
**Feature:** Ordered upgrade application  
**Task:** Compare Fast Movement → Crowding Tolerance against the reverse order  
**Focus:** Same-seed outcome differences, deterministic application, telemetry completeness  
**Location:** `artifacts/EX-009-20260903/`  
**Status:** Blocked before execution  
**Generated:** 2026-09-03

## Locked A/B inputs

| Arm | Ordered loadout | Seed range | Status |
|---|---|---:|---|
| A | `faster-movement,crowding-tolerance` | 106–110 | Blocked at Unity preflight |
| B | `crowding-tolerance,faster-movement` | 106–110 | Existing validated bundle |

## Observed execution result

Arm A was attempted twice. Both attempts stopped before simulation because the
Unity Package Manager IPC preflight failed. No `report.json`, `report.csv`,
`statline.csv`, or valid run bundle was produced for Arm A.

| Attempt | Preflight evidence | Result |
|---|---|---|
| 1 | `artifacts/unity-preflight-20260903-173849/license-probe.log` | Failed before simulation |
| 2 | `artifacts/unity-preflight-20260903-174035/license-probe.log` | Failed before simulation |

## A/B comparison

**Not available.** Comparing Arm B with the prior forward-order runs on seeds
101–105 would repeat the exact design error this experiment is intended to fix.
No order conclusion is made.

## Known limitations

- Unity licensing/Package Manager IPC must be repaired before Arm A can run.
- Existing Arm B was produced on the same source revision but while the working
  tree was dirty; provenance must be checked again when Arm A completes.
- This experiment only answers the discrete order question for ForestEdge,
  Hare, the declared values, and seeds 106–110.

## Human decision

**Decision ID:** `DEC-EXP-009-0001`  
**Decision:** Pending  
**Owner:** Human design owner  
**Key Observation:** No valid A/B order comparison exists because the forward arm was blocked before simulation.  
**Evidence References:** [EX-009 README](README.md), preflight logs above, existing Arm B bundle.  
**Scope:** Authorizes no order-independence, balance, or production conclusion.  
**Follow-up:** Repair Unity/Hub IPC state, rerun Arm A on seeds 106–110, validate the bundle, then compare pairwise with Arm B.
