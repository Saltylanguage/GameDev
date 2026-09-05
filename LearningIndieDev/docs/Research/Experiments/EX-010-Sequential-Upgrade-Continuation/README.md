# EX-010 — Sequential upgrade continuation

**Experiment ID:** `EXP-010`
**Status:** Proposed follow-up; blocked until continuation/resume exists
**Parent:** `EXP-009` / `EXP-007`
**Decision owner:** Human design owner
**Feature owner:** Josh
**Scenario:** ForestEdge and Hare are the initial candidate fixture; final
scenario and values require a human-approved contract

## Question

When a player acquires upgrades between simulation segments and clicks continue,
how do acquisition timing and order affect the later simulation outcome?

## Why EX-009 is not enough

EX-009 applied the complete loadout before the run started. Its zero-delta
result is accepted as a bounded launch-time finding for the two current additive
upgrades. It does not answer what happens when the simulation has already
evolved, an upgrade is acquired, and the player continues from that current
state.

## Proposed contract

Before execution, the human owner must lock:

- the scenario, species, ruleset, and upgrade values;
- the initial seed and starting state;
- the number and duration of simulation segments;
- the exact acquisition tick or checkpoint for each upgrade;
- the A/B orderings and any timing controls;
- the primary and secondary outcomes;
- the checkpoint, snapshot, and report fields needed to replay each segment;
- the acceptance criteria and the fresh validation seed panel.

The design must keep two factors separate:

1. **Order:** which upgrade is acquired first.
2. **Timing/state:** when it is acquired and what the simulation state looks
   like at that point.

## Required implementation seam

The game and headless runner need a reproducible checkpoint/resume path that
captures the evolving simulation state and the immutable upgrade snapshot at
acquisition. Until that seam exists, EX-010 remains a tracked proposal rather
than executable evidence.

## Success criteria

- Both orderings run from the same initial state and matched seed.
- Each acquisition checkpoint is recorded with tick, state fingerprint, and
  upgrade snapshot.
- Complete segment and final bundles can be replayed and diffed.
- The report separates an order effect from a timing/state effect.
- Any conclusion is limited to the tested upgrade types, values, schedule, and
  scenario; no universal commutativity claim is made.

## Follow-up status

This package is intentionally not part of the current S2 implementation slice.
It remains open so the question is not lost. Revisit it when the intended
continue-from-current-state gameplay flow is implemented; otherwise retain it
as an explicit remaining research item rather than silently closing EX-009 as
the answer to a different question.
