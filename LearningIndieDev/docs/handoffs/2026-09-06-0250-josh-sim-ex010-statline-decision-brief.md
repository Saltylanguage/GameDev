# EX-010 — Stat-Line decision brief for Sim

**Date:** 2026-09-06
**Purpose:** Plain-language summary of the Josh/Sim meeting decisions.
**Status:** The Stat-Line meaning review is complete for the decisions below.
EX-010 is executed for the original sequence and its matched reverse-order
comparison. Findings are ready for Sim's review.

## What we agreed

- The expedition has **10 phases**.
- Each phase runs for **200 ticks**.
- At tick 200, the simulation stops before tick 201.
- The player chooses an upgrade or Skip at the boundary.
- The choice is active for tick 201 and every later phase.
- There are **9 decisions** after phases 1 through 9.
- Research will preselect all 9 decisions before running:
  - all 6 current upgrades, each used once;
  - 3 fixed Skip decisions;
  - the same sequence for every seed.
- Chosen upgrades stay in the list and their modifiers add together.
- Skip leaves the upgrade list unchanged.
- If the run ends early, the report stops there and later phases are marked
  **no data**.

## Stat-Line rules Sim confirmed

- Every phase gets a complete, independent Stat-Line.
- The final expedition gets its own independent Stat-Line. It is not made by
  adding or averaging the phase reports.
- “Previous” means the phase or run immediately before the current one in the
  same sequence.
- A delta is `current - previous` when both values exist. If either value is
  `N/A`, the delta is `N/A`.
- The complete Stat-Line is the source of truth. We are not adding new scores,
  efficiency functions, fitness functions, or rankings.
- `FPO = SPO + BIR - PREY - STRV - CRWD`.
- `PREY` means predator kills only. We do not add other removal causes to that
  formula or rename them as `PREY`.
- `BIR` is the actual number of births. `bAVG = BIR / MAT`; `MAT = 0` means
  `bAVG` is `N/A`. Multiple births are kept as actual counts. APS treats the
  missing birth contribution as zero while the original `N/A` remains visible.
- HPS tracks qualifying simulation steps even if populations later become
  extinct. `EHS = 0` is valid perfect avoidance when exposure exists, and
  `ECN = 0` makes `pAVI` `N/A`.
- Reporting on or off must not change simulation outcomes.

## What Sim needs to do

- No runtime rewrite is being assigned to Sim.
- Sim’s semantic confirmation for the rules above is complete.
- After the runs, Sim should review the full phase reports, final report, and
  chronological deltas and confirm the findings directly.

## What was completed

- The exact six upgrade IDs and three Skip positions were locked in the
  original contract, then a matched reverse-order sequence was run for the
  order comparison.
- The scenario, options, tick boundaries, and seed panels were held constant.
- Development seeds `1–20` and held-out seeds `106–110` completed all ten
  phases in both sequences.
- The reports include independent phase Stat-Lines, chronological deltas, and
  independent final Stat-Lines.

## What happens next

- Sim reviews the full phase reports, final reports, and chronological deltas.
- Sim confirms that the bounded order-effect finding is stated accurately.
- Until that review, the result is ready for review rather than a final human
  decision.
