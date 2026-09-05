# EX-009 — Same-held-out-seed A/B report

**Experiment:** `EXP-009`  
**Report:** `RPT-RUN-009-0001`  
**Feature:** Ordered upgrade application  
**Task:** Compare Fast Movement → Crowding Tolerance against the reverse order  
**Focus:** Same-seed outcome differences, deterministic application, telemetry completeness  
**Location:** `artifacts/cellular-experiment-20260904-192559/` and
`artifacts/cellular-experiment-20260904-192703/`
**Status:** Complete; bounded launch-time result accepted
**Generated:** 2026-09-04

## Locked A/B inputs

| Arm | Ordered loadout | Seed range | Status |
|---|---|---:|---|
| A | `faster-movement,crowding-tolerance` | 106–110 | Complete, schema 23 |
| B | `crowding-tolerance,faster-movement` | 106–110 | Complete, schema 23 |

## Observed execution result

Both arms ran through the authored-upgrade adapter with the explicit EX-007
research fixture catalog. Each produced `report.json`, `report.csv`,
`statline.csv`, `manifest.json`, and `unity.log`; both bundles passed the strict
artifact validator. The independent StatLine validator returned
`VALIDATED_WITH_LIMITATIONS` for each arm.

| Arm | Artifact | Validation |
|---|---|---|
| A | `artifacts/cellular-experiment-20260904-192559` | Bundle valid; StatLine validated with limitations |
| B | `artifacts/cellular-experiment-20260904-192703` | Bundle valid; StatLine validated with limitations |

## A/B comparison

The paired comparison is complete. Every seed-level delta in
[`paired-deltas.csv`](paired-deltas.csv) is zero for the final populations,
births, predation, starvation, crowding, movement, food, and available
encounter/statline measures. The five per-run objects are JSON-equivalent after
excluding the intentionally different ordered loadout record.

The ruleset fingerprint is identical between arms, while the ordered loadout
and its fingerprints differ as intended. This is evidence that these two
current additive upgrades commute under the locked test conditions. It does not
generalize to future stateful or non-additive upgrades.

## Known limitations

- The five-seed panel is a transfer smoke test, not a high-powered promotion
  sample.
- StatLine validation limitations remain for accumulated counters; they are
  reported explicitly and do not change the exact A/B equality observed here.
- This experiment only answers the discrete order question for ForestEdge,
  Hare, the declared values, and seeds 106–110.

## Human decision

**Decision ID:** `DEC-EXP-009-0001`  
**Decision:** Accept — bounded launch-time order result
**Owner:** Human design owner  
**Key Observation:** Upgrade order matched across the recorded outcomes and
telemetry for all five pairs. While this is indicative of commutativity for the
launch-time loadout, the intended game flow will let the player acquire
upgrades between simulation segments and continue from the current state. The
time of acquisition and accumulated state can therefore change the outcome;
EX-009 does not answer that sequential-continuation question.
**Evidence References:** [EX-009 README](README.md),
[paired deltas](paired-deltas.csv), Arm A and Arm B bundles above.
**Scope:** This accepts only the bounded launch-time model-behavior result for
ForestEdge/Hare, the declared additive values, telemetry, and held-out seeds
106–110. It is not a balance, fun, or production approval and not a universal
order theorem.
**Follow-up:** Add a focused launch-time commutativity regression test. Track
the sequential-continuation question as proposed EX-010 and revisit it if the
continue-from-current-state gameplay flow is implemented.
