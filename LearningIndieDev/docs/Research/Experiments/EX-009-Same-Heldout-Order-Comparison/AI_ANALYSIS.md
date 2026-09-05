# EX-009 — Analysis status

**Analysis:** `ANL-RPT-RUN-009-0001-v1`  
**Experiment:** `EXP-009`  
**Source report:** `REPORT.md`  
**Status:** Scored; bounded launch-time result accepted
**Generated:** 2026-09-04

Both ordered loadouts now have complete adapter-backed schema-23 bundles on the
same held-out seeds (106–110). The two reports share the same scenario, ruleset
fingerprint, catalog path, registry fingerprint, run configuration, and seed
panel. Their ordered loadout and order-sensitive fingerprints differ as
expected.

## Result

All five per-seed comparisons are exact matches after excluding the intentionally
different ordered loadout record. Final population, births, predation,
starvation, crowding, movement, food, and available encounter/statline measures
all have A-minus-B = 0. The full table is in
[`paired-deltas.csv`](paired-deltas.csv).

## Interpretation

The evidence supports the bounded statement:

> Under the current additive upgrade implementation, `faster-movement` and
> `crowding-tolerance` commute for ForestEdge/Hare at the declared values and
> seeds 106–110.

This is stronger than the earlier different-panel comparison, but it remains a
local implementation result. It does not establish commutativity for upgrades
that share fields, use caps or multipliers, alter prerequisites, or introduce
stateful side effects. The five-seed panel is also too small to serve as a
standalone balance or promotion study.

The result also does not answer the intended continue-from-current-state flow,
where an upgrade is acquired between simulation segments. Acquisition timing
and accumulated simulation state can change later outcomes even when the
launch-time loadout order commutes. That question is tracked separately as
proposed EX-010.

## Validation note

Both artifact bundles passed the strict bundle validator. StatLine validation
returned `VALIDATED_WITH_LIMITATIONS` for the known accumulated-counter limits;
those limits do not affect the exact paired equality reported here.
