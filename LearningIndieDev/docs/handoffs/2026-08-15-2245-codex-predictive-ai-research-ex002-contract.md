# predictive-ai-research-ex002-contract

[Working state](../WORKING_STATE.md) | Status: death telemetry integrated; Unity execution blocked

- Owner: codex
- Branch: `Tooling/AI_Workflows`
- Date: 2026-08-15

## Decision and scope

EX-002 now treats collapse as a simulation-defined loss of practical growth
capacity, not as a synonym for extinction or an ecology-only condition. A cell
type may be collapsed when it has no viable mate, destination, resource, or
other rule-governed growth path; the state may be intentional and desirable.
The simulation adapter must define the rule and observation window, while human
review decides whether the collapse matters.

BaselineParity is the first concrete adapter because it contains `herbivore`.
ForestEdge remains the accepted instrument-trust baseline, not the herbivore
phenomenon under study.

## Implementation changes

- CellSim accepts `-RunDurationSeconds` and `-StepIntervalSeconds` run-window
  overrides without mutating authored scenario assets.
- Experiment reports advance to schema 5 and include `trackedBehavior` records
  with entity ID, age, position, state, and state duration, plus `deathEvents`
  with species/resource identity, entity ID, age, position, tick, and proximate
  cause. Existing transition records remain intact.
- All current removal paths are instrumented: combat, starvation, crowding,
  wilt, population-limit removal, and resource consumption. This is proximate
  cause telemetry; preceding resource state and attacker linkage remain open.
- EX-002 now distinguishes the schema-4 aggregate BaselineParity result from the
  required schema-5 instrumented baseline. Aggregate counts are retained, but
  no per-death event data is backfilled into the old report.
- `SpeciesSimulationMetrics.TryGetTrackedBehavior` exposes the same tracked FSM
  identity used by the logging path.
- EX-002 documentation defines named, fingerprinted intervention variants;
  arbitrary reflection-based rule mutation is intentionally not added.
- Superseded schema-2 EX-001 report/analysis files and their two generated
  artifact directories were removed after their provenance facts were copied to
  the current EX-001 record.

## Validation and blocker

- PowerShell command and script parsing checks pass; `git diff --check` passes.
- Unity is not running after the attempts.
- Two current BaselineParity launches failed before writing artifacts with exit
  code `-2147483645`; a later Edit Mode test and the schema-5 BaselineParity
  rerun at `artifacts/cellular-experiment-20260815-203653/` reproduced the same
  failure before NUnit/report output. Local crash reports confirm a native Unity
  crash with no managed exception. Unity startup/cache/editor repair is
  required before the first schema-5 causal run.

## Next action

Resolve the Unity native startup issue, run a same-seed schema-5 BaselineParity
control with the declared observation window, verify `deathEvents` against the
aggregate activity totals, then add one-mechanism intervention variants and
held-out validation before making an EX-002 decision.
