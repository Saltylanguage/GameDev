# EX-007 — Human decision

**Decision ID:** `DEC-EXP-007-0001`  
**Decision:** Pending human review
**Owner:** Human design owner  
**Date prepared:** 2026-09-03

## The decision in one sentence

The experiment supports a cautious statement that the tested Hare upgrades can
change population outcomes in ForestEdge, but it does not yet support a stable
effect size or a reliable interaction claim.

## Decisive observations

- Faster movement produced roughly five additional Hares at the end of both
  the training and held-out runs.
- The combined upgrade produced fifteen additional Hares on the held-out
  panel, well above the predicted range.
- Faster movement increased starvation deaths instead of reducing them.
- Crowding tolerance consistently eliminated recorded crowding deaths.

## Choose one

- [ ] **Accept** the bounded claim as written.
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
It does not approve a production balance change or make a claim about fun,
quality, or continuous upgrade scaling.

## Follow-up after the decision

Record the reviewer's choice, review time, and any requested rerun. EX-008 has
already added the missing crowding-tolerance-only arm; EX-009 now covers the
remaining clean same-held-out-seed comparison of the two joint orders.
