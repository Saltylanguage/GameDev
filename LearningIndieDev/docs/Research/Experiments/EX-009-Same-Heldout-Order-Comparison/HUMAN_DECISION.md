# EX-009 — Human decision

**Decision ID:** `DEC-EXP-009-0001`  
**Decision:** Accept — bounded launch-time order result
**Owner:** Human design owner  
**Date prepared:** 2026-09-04

## Decision options

- [x] Accept the bounded A/B order result for this upgrade pair and test configuration.
- [ ] Reject the order claim as inconclusive.
- [ ] Revise and rerun with additional same-seed controls or telemetry.

## Key Observation

Upgrade order matched across the recorded outcomes and telemetry for all five
pairs. While this is indicative of commutativity for the launch-time loadout,
the intended game flow will let the player acquire upgrades between simulation
segments and continue from the current state. The time of acquisition and
accumulated state can therefore change the outcome; EX-009 does not answer that
sequential-continuation question.

## Scope

This decision accepts only the bounded launch-time model-behavior result for
ForestEdge/Hare, the declared additive values, telemetry, and held-out seeds
106–110. It must not be read as balance, fun, engagement, or production
approval, and it does not establish commutativity when upgrades are acquired
between continuing simulation segments.

## Evidence

- [Factual report](REPORT.md)
- [AI analysis](AI_ANALYSIS.md)
- [Paired deltas](paired-deltas.csv)
- Arm A: `artifacts/cellular-experiment-20260904-192559`
- Arm B: `artifacts/cellular-experiment-20260904-192703`

## Follow-up

The sequential-continuation question is tracked as proposed EX-010. Add a
commutativity regression test for launch-time additive loadouts, then revisit
EX-010 if the continue-from-current-state gameplay flow is implemented.
