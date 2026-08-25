# Sprint 1 Ticket Summaries — Species Stat Line

> Status: Priority override for Sim's Sprint 1 capacity  
> Date: 2026-08-24  
> Owner: Sim  
> Reviewer: Josh  
> Related: [`SPRINT_1_PLAN.md`](SPRINT_1_PLAN.md), [`UNITY_SIMULATION_TOOLING.md`](UNITY_SIMULATION_TOOLING.md)

## Scope override

These tickets take precedence over Sim's remaining unstarted Sprint 1 Main Menu
and Lab work. Existing shell work is not deleted; unfinished Sim-owned items are
paused and carried forward explicitly after this work. Josh's existing review,
product, and integration responsibilities remain unless separately re-planned.

This ticket set assumes “baseball analytics model” means a compact stat line
made from:

1. **Counting stats** — what happened;
2. **Rate stats** — how often it happened when an opportunity existed; and
3. **Context/splits** — when, against whom, and under which scenario or phase it
   happened.

The first pass must not invent a single composite “species rating.” Preserve the
raw ledger so future derived metrics can be audited back to events.

## First stat-line surface

The exact labels are part of S1-STAT-01, but the first useful line should cover:

| Section | Counting evidence | Candidate rate/context evidence |
| --- | --- | --- |
| Population | Starting, final, peak, minimum, births, deaths | Survival/extinction, mean/AUC, phase or time window |
| Feeding/resources | Food attempts, successes, failures, food consumed | Feeding success, food per opportunity, resource/terrain split |
| Pressure/combat | Attack opportunities, attempts, resolved contacts, hits, damage, kills | Hit rate, kill conversion, damage per contact, attacker/target split |
| Defense/survival | Threat encounters, avoided/blocked/missed contacts, non-lethal/lethal outcomes | Avoidance/survival rate, danger exposure, mortality cause split |
| Reproduction | Candidates, blocked outcomes, successful attempts, births/litter | Reproduction conversion, gate breakdown, phase/age context |
| Movement/behavior | Move attempts/successes, state ticks, notable transitions | Success rate, time in behavior state, terrain or phase split |

Metrics that do not yet have reliable opportunity denominators should remain
counting stats until the missing instrumentation is added. A rate with an
ambiguous denominator is worse than no rate.

## Ticket summaries

### S1-STAT-01 — Freeze the stat-line contract

**Priority:** P0  
**Estimate:** 2 hours  
**Depends on:** current schema-9 report and `SpeciesSimulationMetrics` inventory

Define the first stat-line categories, stable field names, display labels, and
baseball-inspired terminology. Classify every field as a raw count, rate,
context/split, or presentation-only value. For each rate, define its numerator,
denominator, time window, and zero-denominator behavior.

Acceptance:

- A reviewed Markdown contract lists the first stat-line fields and their source
  events or existing counters.
- Every rate has an explicit denominator and a documented zero case.
- Existing schema fields are reused where they already answer the question;
  duplicate counters are named and rejected.
- The contract says which fields are implementation-ready, which require new
  instrumentation, and which are deferred.

Non-goals: a universal analytics framework, a single species score, balance
claims, or final player-facing copy.

### S1-STAT-02 — Complete the raw opportunity/outcome ledger

**Priority:** P0  
**Estimate:** 5 hours  
**Depends on:** S1-STAT-01

Add only the missing raw counters required to distinguish opportunities,
attempts, successes, failures, and outcomes. Reuse the existing simulation
metrics and resolver paths rather than creating a parallel event system.
Preserve actor species, target species/resource, tick/window, and causal outcome
where the current model already makes them available.

Acceptance:

- Feeding, movement, combat, defense, and reproduction counters reconcile with
  their existing ledgers and event records.
- Actor/target attribution is not silently swapped when a carnivore attacks a
  herbivore or a species interacts with a resource.
- Counters are emitted once per resolved opportunity and do not change
  simulation behavior.
- Focused tests cover success, failure, blocked, lethal, and zero-opportunity
  cases.

Non-goals: adding Scent, Endurance, Armor, Dodge, or other future species stats.

### S1-STAT-03 — Compute derived rate statistics

**Priority:** P0  
**Estimate:** 4 hours  
**Depends on:** S1-STAT-01 and S1-STAT-02

Build the smallest derived-stat layer over the raw ledger. Initial candidates
include feeding success, attack hit rate, kill conversion, damage per resolved
contact, reproduction conversion, movement success, and survival/danger rates.
Keep raw counts beside every rate and support the same calculation for a whole
run and a named time window.

Acceptance:

- Hand-calculated fixtures match the generated values, including zero-
  denominator behavior.
- A rate cannot be mistaken for a count in JSON, CSV, Markdown, or UI-facing
  view data.
- Derived values identify their numerator, denominator, and window in the
  contract or metadata.
- No composite “MVP,” “WAR,” or overall species score is introduced in this
  ticket.

### S1-STAT-04 — Serialize and present the stat line

**Priority:** P1  
**Estimate:** 4 hours  
**Depends on:** S1-STAT-03

Expose the stat line through the existing report path and the smallest useful
review surface. JSON remains the machine-readable source; CSV and Markdown
should use the same field names, and any Noesis/ViewModel projection should be a
thin presentation adapter rather than a second calculation path.

Include scenario, seed range, ruleset fingerprint, upgrade loadout, species
identity, time window, and denominator metadata so a line can be compared and
replayed. Make the distinction between “no opportunity” and “zero success”
visible.

Acceptance:

- One saved run produces a readable per-species stat line in the report output.
- JSON, CSV, Markdown, and any UI preview agree on values and labels.
- A reviewer can trace a displayed value back to the raw count and source
  opportunity without opening runtime code.
- Existing `CellSim Report` and `CellSim Compare` workflows remain usable.

Non-goals: final visual polish, a full dashboard, player progression, or a
large charting framework.

### S1-STAT-05 — Deterministic validation and baseline comparison

**Priority:** P0  
**Estimate:** 3 hours  
**Depends on:** S1-STAT-02 and S1-STAT-03

Create focused tests and a small fixed-seed validation pass. Verify that the
stat line is deterministic, that ledger totals reconcile, and that adding the
reporting path does not alter the simulation result.

Acceptance:

- Same scenario, seed, ruleset fingerprint, and loadout produce identical stat
  lines on replay.
- Population, feeding, combat, mortality, and reproduction ledgers reconcile.
- A known fixture proves the difference between an opportunity with zero
  success and no opportunity at all.
- The comparison report identifies stat-line deltas without presenting them as
  balance conclusions.

### S1-STAT-06 — Review, handoff, and follow-up split

**Priority:** P1  
**Estimate:** 2 hours  
**Depends on:** S1-STAT-04 and S1-STAT-05

Run the stat line through a Josh review, record accepted terminology and cuts,
and split any missing species-stat work into separate future tickets. Update
the Sprint 1 handoff with changed files, validation commands, report examples,
known limitations, and the next experiment.

Acceptance:

- The reviewed stat line answers what happened, how often it happened, and in
  what context without requiring raw-log interpretation.
- Deferred fields are named rather than silently omitted.
- The next owner can reproduce the validation from the handoff and one saved
  report.

## Effort and sequencing

| Ticket | Estimate | Sequence |
| --- | ---: | --- |
| S1-STAT-01 | 2h | First; contract gate |
| S1-STAT-02 | 5h | After contract |
| S1-STAT-03 | 4h | After raw ledger |
| S1-STAT-04 | 4h | After derived metrics |
| S1-STAT-05 | 3h | Parallel with report integration where safe |
| S1-STAT-06 | 2h | Final review |
| **Total Sim allocation** | **20h** | Protect the full stat-line slice |

The first ticket is the decision gate. If the contract reveals that a proposed
metric needs a new species mechanic rather than telemetry, split that mechanic
out of Sprint 1 instead of expanding this work silently.

## Explicitly deferred from this ticket set

- Scent, Endurance, Sprint state, Armor, Dodge/Evasion, pregnancy, maturity
  tiers, and other future species stats;
- a universal event bus or analytics/plugin framework;
- automatic balance tuning or rubber-banding based on the stat line;
- a composite species rating or leaderboard score;
- replacing the deterministic simulation with an analytics-only model.

