# EX-010 — Sequential upgrade continuation

**Experiment ID:** `EXP-010`
**Status:** Original and matched alternate sequences executed; bounded result
accepted by Josh and Sim
**Parent:** `EXP-009` / `EXP-007`
**Decision owner:** Human design owner
**Feature owner:** Josh
**Scenario:** ForestEdge and Hare

## Question

Across one full ten-phase expedition, how do the phase-by-phase Stat-Lines and
the final expedition Stat-Line change as one fixed sequential upgrade history is
applied?

## Why EX-009 is not enough

EX-009 applied the complete loadout before the run started. Its zero-delta
result is accepted as a bounded launch-time finding for the two current additive
upgrades. It does not answer what happens when the simulation has already
evolved, an upgrade is acquired, and the player continues from that current
state.

## Approved execution

The approved contract uses ForestEdge, Hare, the EX-009 options, and ten
200-tick phases. The first phase has no upgrade. The remaining six eligible
production upgrades are added once in a fixed order, with three Skip decisions
between them. The complete schedule is in [CONTRACT_DRAFT.md](CONTRACT_DRAFT.md).

The development panel used seeds 1–20. The held-out panel used seeds 106–110.
The matched reverse-order run is defined in
[ALTERNATE_ORDER_CONTRACT.md](ALTERNATE_ORDER_CONTRACT.md).

## Required implementation seam

The implementation dependency is now scoped in
[CF-1 through CF-5](../../../CONTINUOUS_SIMULATION_FLOW_PLAN.md).
The [shared evidence impact](../../../CONTINUOUS_SIMULATION_EVIDENCE_IMPACT.md)
adds concrete requirements: current and previous grids, absolute tick and
deterministic entity allocation, immutable rules/acquisition timeline,
phase/expedition metric windows, forecast horizon and checkpoint lineage.
Development/held-out splitting keeps each run in its assigned panel.

The game-side domain exposes the reproducible boundary checkpoint and the
headless runner accepts the authored phase schedule. Each run records the phase
windows, acquisition timeline, effective loadout, and independent final
Stat-Line.

The execution result is summarized in [REPORT.md](REPORT.md). The raw and
derived artifacts are kept in the artifact directories linked there. The human
decision is recorded in [HUMAN_DECISION.md](HUMAN_DECISION.md).

## Execution result

- 50 runs completed across the original and alternate sequences: 40
  development and 10 held-out.
- Every run completed all 10 phases and 2,000 ticks.
- Every acquisition occurred at the locked ticks: 400, 800, 1000, 1200,
  1600, and 1800.
- Every run has 10 phase Stat-Lines, 9 chronological delta rows after the
  first phase, and one independent final Stat-Line.
- No phase was marked `NO_DATA`.
- Both artifact bundles passed bundle validation and EX-010 validation.

The matched alternate sequence now provides an order comparison for this one
scenario, schedule, option set, and seed panel. It does not establish a
universal upgrade-order rule. Sim reviewed the phase and final Stat-Lines in
person and approved the bounded finding.

The paired comparison is summarized in [REPORT.md](REPORT.md) and preserved in
`artifacts/ex010-sequence-comparison-20260906-053441/`.

Review the phase CSV, delta CSV, final CSV, and JSON report before drawing any
conclusion.
