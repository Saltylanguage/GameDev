# EX-007 — Human decision

**Decision ID:** `DEC-EXP-007-0001`  
**Decision:** Accept — bounded model-scoped evidence
**Owner:** Human design owner  
**Date prepared:** 2026-09-04

## Key Observation

Under the tested ForestEdge/Hare configuration, faster movement increased final
Hare population across both seed panels and crowding tolerance reduced crowding
deaths, but effect size, interaction, and balance remain unresolved and are
outside the scope of EX-007 for now.

## Decisive observations

- Faster movement produced roughly five additional Hares at the end of both
  the training and held-out runs.
- The combined upgrade produced fifteen additional Hares on the held-out
  panel, well above the predicted range.
- Faster movement increased starvation deaths instead of reducing them.
- Crowding tolerance consistently eliminated recorded crowding deaths.

## Choose one

- [x] **Accept** the bounded claim as written.
- [ ] **Reject** the claim and treat this experiment as inconclusive.
- [ ] **Revise** the claim, likely by narrowing the prediction and adding a
  crowding-tolerance-only arm before making an interaction claim.

## Evidence

- [Factual report](REPORT.md)
- [AI analysis](AI_ANALYSIS.md)
- [Prediction](PREDICTION-EXP-007-0001.json)
- Per-seed comparisons: `artifacts/EX-007-20260903-1535/paired-deltas.csv`
- Adapter migration audit: [package README](README.md#adapter-migration-verification-2026-09-04)

## Scope

This decision applies only to the ForestEdge scenario, Hare player, declared
seed panels, exact upgrade values, telemetry, and 20-second observation window.
It records accepted model-scoped evidence only. It does not approve a
production balance change or make a claim about fun, quality, continuous
upgrade scaling, a stable interaction effect, or behavior after an upgrade is
acquired between continuing simulation segments.

## Follow-up after the decision

Record the reviewer's choice, review time, and any requested rerun. EX-008 has
already added the missing crowding-tolerance-only arm; EX-009 now covers the
clean same-held-out-seed comparison of the two joint orders. The separate
sequential-continuation question is tracked as proposed EX-010 and should be
revisited if the intended continue-from-current-state gameplay flow is
implemented.
