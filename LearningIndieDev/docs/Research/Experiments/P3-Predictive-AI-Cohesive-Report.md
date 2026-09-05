# P3 Predictive AI — Cohesive Results Report

**Scope:** EX-007 baseline experiment, EX-008 reversed-order follow-up, and
EX-009 same-held-out order comparison
**Scenario:** `ForestEdge.asset`
**Species:** Hare
**Status:** Bounded EX-007 and EX-009 decisions accepted; balance and P3
promotion review remain open

## What we tested

The goal was to see whether an AI could make a bounded prediction about a
small, supported simulation change, then be checked against paired same-seed
runs and new held-out seeds.

The first experiment compared:

- **B:** no upgrade
- **S1:** faster movement
- **J1:** faster movement plus crowding tolerance

The follow-up added the missing single-variable comparison and reversed the
joint sequence:

- **B:** no upgrade
- **C1:** crowding tolerance
- **CJ1-reversed:** crowding tolerance, then faster movement

Both training panels use seeds 1–20. The original held-out panel uses 101–105;
the follow-up adds new held-out seeds 106–110. Every bundle passed the artifact
and StatLine validators with the project's documented limitations.

## Main results

### Final Hare population

| Panel | B | S1 faster | J1 faster + crowding | C1 crowding | CJ1-reversed crowding + faster |
|---|---:|---:|---:|---:|---:|
| Training (20) | 70.3 | 76.1 | 75.6 | 76.4 | 75.6 |
| Original held-out (5) | 71.4 | 76.0 | 86.8 | — | — |
| New held-out (5) | 77.6 | — | — | 80.2 | 78.8 |

The direction “more Hares at the end” repeated for faster movement in the
original test and for crowding tolerance in the new panel. The size of the
effect varied substantially by seed panel, so these are not promises about a
future run.

### What changed besides final population

- Faster movement increased births on both original panels, but also increased
  starvation deaths. More movement was not automatically healthier.
- Crowding tolerance consistently removed crowding deaths in the tested arms.
- Fox kills of Hares (`PREY`) were seed-sensitive. Faster movement increased
  them on the original training panel but reduced them on the original held-out
  panel. On new held-out seeds, crowding alone reduced fox kills by 1.8 on
  average and the reversed joint arm reduced them by 3.4.
- Predator-encounter survival (`pAVI`) rose for faster movement on both original
  panels, even when the absolute fox-kill count moved in opposite directions.
  This points to encounter frequency and timing as important drivers.
- The new held-out combined arm produced more births than crowding alone
  (+3.8 incremental births), but ended with 1.4 fewer Hares on average. Births
  and final population therefore should not be treated as interchangeable
  outcomes.

## What the prediction got right

The pre-registered prediction correctly expected:

- faster movement to increase final Hare population;
- the combined arm to increase final population;
- faster movement to increase births; and
- crowding tolerance to reduce crowding deaths.

It incorrectly expected starvation deaths to fall. The original prediction also
used `PREY` as though it measured food/resource events. In this simulation it
means Hares killed by carnivores, so that forecast is excluded from causal
scoring as a metric-definition error rather than relabelled after the fact.

The initial confidence figures were 56% for S1 and 62% for J1. Against the
original four-outcome, two-panel check, S1 had 5/8 direction checks right and J1
had 7/8. This is an early calibration observation, not evidence of reliable
calibration from one experiment.

## What we can conclude

1. The experiment loop works: bounded inputs, pre-registered predictions,
   same-seed comparisons, held-out seeds, machine-readable telemetry, and a
   separate human decision record are all present.
2. The upgrade effects are real under this exact scenario and time window, but
   they are heterogeneous across seeds. Population, births, starvation, and
   predator contact can move in different directions.
3. The added crowding-only arm closes an important interpretation gap: the
   earlier joint result cannot be called a clean interaction effect without a
   same-seed crowding-only comparison.
4. The training aggregates for the forward and reversed joint sequences match,
   which is consistent with these two rule changes being order-independent in
   the current implementation.
5. EX-009 tested both joint orders on the same held-out seeds (106–110). All
   five pairs matched across the recorded outcomes and telemetry, supporting a
   bounded launch-time commutativity result for these two additive upgrades.

## What remains unproven

EX-009 closes the same-held-out launch-time order gap, but the evidence still
does not establish balance, fun, long-run stability, transfer to another
scenario, or behavior at other upgrade values. It also does not answer the
intended game flow where an upgrade is acquired between simulation segments and
the player continues from the current state. That timing/state question is
tracked as proposed EX-010 and requires a checkpoint/resume seam plus a new
human-approved contract.

## Evidence locations

- Plain-language factual report: [EX-007 REPORT.md](EX-007-Predictive-Statline-Interventions/REPORT.md)
- Prediction scoring: [EX-007 AI_ANALYSIS.md](EX-007-Predictive-Statline-Interventions/AI_ANALYSIS.md)
- Original paired deltas: `artifacts/EX-007-20260903-1535/paired-deltas.csv`
- Follow-up paired deltas: `artifacts/EX-008-20260903/paired-deltas.csv`
- Same-held-out order report: [EX-009 REPORT.md](EX-009-Same-Heldout-Order-Comparison/REPORT.md)
- Same-held-out paired deltas: [EX-009 paired-deltas.csv](EX-009-Same-Heldout-Order-Comparison/paired-deltas.csv)
- Human decision record: [HUMAN_DECISION.md](EX-007-Predictive-Statline-Interventions/HUMAN_DECISION.md) (bounded claim accepted)
