# EXP-002 - Experiment brief

**Experiment ID:** `EXP-002`  
**Question:** How can a cellular-automata simulation report and causally test a
cell type's collapse state, using herbivore decline in BaselineParity as the
first concrete adapter?  
**Stage:** Causal attribution  
**Decision owner:** Human design owner  
**Status:** Schema-5 death telemetry integrated; execution blocked by Unity batch startup failure

## Collapse contract

A cell type is in **collapse** when, under the simulation's own rules and
current state, it no longer has a practical path to increase its population.
That may mean zero cells, one remaining cell with no viable mate, no valid
growth destination, or any other rule-defined condition. Collapse is not
automatically a defect: a completed construction system may intentionally have
no remaining growth path.

The generic research contract is therefore:

- the simulation domain defines what counts as a viable growth path;
- CellSim records the state, rule identity, observation window, and evidence for
  that determination;
- analysis distinguishes `Collapsed`, `Viable`, `Growing`, and
  `NotApplicable` only when the domain adapter can justify those states; and
- desirability remains a separate human design decision.

CellSim must not hard-code “collapse” as extinction or as an ecology-only
condition. BaselineParity is the first adapter, not the general definition.

## Target correction

The plan's phrase “herbivore collapse in the current reference scenario” was
ambiguous. The accepted ForestEdge baseline uses player species `hare`, while
the current authored scenario containing `herbivore` is BaselineParity. EX-002
therefore uses BaselineParity for the phenomenon under study and retains
ForestEdge only as the accepted instrument-trust reference.

The schema-2 reports showing universal herbivore extinction are superseded and
their source artifacts were removed after the provenance facts were transferred
to the current EX-001 record. They are not valid current-code outcome evidence.
The existing schema-4 BaselineParity pair is the pre-telemetry-extension
baseline for this experiment. The first post-change run will emit schema 5 and
must preserve the same simulation inputs before it is treated as a replacement
baseline.

## Baseline evidence already available

| Input | Value |
|---|---|
| Scenario | `Assets/Data/CellularSimulation/Scenarios/BaselineParity.asset` |
| Player species | `herbivore` |
| Grid | `32 x 20` |
| Schema | `4` |
| Seeds | `10100`–`10119` (20 runs) |
| Duration / step | `20.0 s` / `0.1 s` |
| Baseline report | `artifacts/cellular-experiment-20260815-165312/report.json` |
| Ruleset fingerprint | `8e15ed2b7bb31ec503944a2df9f4bcb7a16f4d851d141529312324fb90a5ca3e` |
| Final herbivore population | min `0`, max `37`, average `10.25`, one extinct run |
| Herbivore deaths | `250` total; `245` starvation, `3` crowding, `2` other/remaining deaths |

This is a strong candidate signal, not a causal conclusion. The 20-second
window and one extinct run may be too short or too sparse for a robust
BaselineParity collapse observation.

## Death telemetry integration and result boundary

The EX-002 instrument now emits a schema-5 `deathEvents` array for each run.
Each event records the species or resource identity, entity ID when available,
age, position, tick, creature/resource flag, and proximate cause. The existing
aggregate activity counters remain in the report for continuity and cross-checks.

The table above is therefore retained as **pre-telemetry baseline evidence**.
Its starvation/crowding totals are valid aggregate observations from the schema-4
report, but the report contains no per-event entity, location, or tick data and
must not be backfilled. A schema-5 same-seed rerun is required to produce the
instrumented EX-002 baseline. Until that rerun succeeds, the experiment result is:

- **Observed:** BaselineParity shows a candidate starvation-dominant proximate
  mortality pattern in aggregate data.
- **Instrumented per-death result:** Pending; no schema-5 report has been
  generated because Unity batch startup still fails.
- **Causal conclusion:** Not supported.

## CellSim run and report API

The first useful generic parameters are the run-window overrides:

- `-RunDurationSeconds` defines the observation horizon;
- `-StepIntervalSeconds` defines temporal resolution; and
- the existing scenario, seed range, grid, and player/cell-type identity fields
  preserve controlled provenance.

These are configuration overrides, not interventions. Rule changes must remain
named scenario or ruleset variants with distinct fingerprints. Tailored reports
should select the relevant cell-type IDs and time window from the full JSON
evidence rather than silently dropping unrequested observations.

## Hypothesis and intervention structure

The first hypothesis is that starvation pressure is the dominant proximate
driver of the observed herbivore decline. A causal follow-up must keep the
scenario, seeds, duration, player species, and engine fixed while changing one
approved mechanism at a time. Candidate arms are:

1. BaselineParity control.
2. Herbivore energy/forage relief, changing one named herbivore rule.
3. Predation relief, changing one named carnivore rule or removing only the
   predation pathway.

The current `CellSim Run` command has no parameter-override surface. Before
execution, either create versioned variant assets with distinct fingerprints or
add a narrowly scoped, auditable override contract. Do not edit generated
reports or silently mutate the shared species asset.

## Required evidence

- Matched-seed baseline and intervention reports.
- Ruleset fingerprints and complete run payloads.
- Final population, extinction rate, births, deaths, starvation deaths,
  crowding deaths, combat kills, movement, and food-consumption summaries.
- Population histories by seed and an explicitly defined endpoint/window.
- Per-death telemetry with species, entity/resource identity, age, position,
  tick, and proximate cause (`Combat`, `Starvation`, `Crowding`, `Wilt`,
  `PopulationLimit`, or `ResourceConsumed`).
- A separate analysis that distinguishes proximate death mode from root-cause
  attribution and records uncertainty.

## Instrument gaps and risks

- Activity metrics remain aggregated by species, but schema-5 `deathEvents` now
  link each recorded removal to its species/resource identity, position, tick,
  and proximate cause. They still do not capture the preceding energy/resource
  state or identify an attacker as the root cause.
- Behavior transitions track one representative entity per species rather than
  a complete per-entity trajectory. The new `trackedBehavior` telemetry now
  records that entity's ID, current state, age, position, and state duration.
- Resource occupancy, energy trajectories, and spatial confinement are not
  directly reported, limiting claims about forage access and movement pressure.
- “Collapse” needs an operational threshold, such as extinction, a specified
  population quantile, or a time-to-threshold metric, before runs begin.
- The first two current BaselineParity launches on 2026-08-15 failed during
  Unity batch startup with exit code `-2147483645` and produced no report.
  A subsequent Edit Mode test attempt reproduced the same exit code before
  producing NUnit results. The schema-5 BaselineParity rerun attempted at
  `artifacts/cellular-experiment-20260815-203653/` failed with the same code
  before writing a report. Unity was verified absent afterward. The newest
  local Unity crash report identifies a native crash with no managed exception;
  this must be resolved before evidence collection.

## Success criteria

- Each intervention changes one approved mechanism and has a distinct recorded
  ruleset fingerprint.
- Same-seed comparisons show a predicted directional change in the named
  endpoint and relevant death/activity metrics.
- The result is replicated over a predeclared seed range and survives at least
  one held-out seed subset or duration check.
- The final claim names the tested scenario, intervention, endpoint, range, and
  unresolved instrument gaps.

## Scope boundaries

This experiment does not decide whether the game is balanced, does not promote
an AI recommendation, and does not generalize the finding to all cellular
automata or all scenarios. Human approval is required before any variant is
executed or any causal explanation is accepted.
