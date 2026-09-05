# Continued simulation — Stat-Line, predictive AI and telemetry impact

**Status:** Proposed migration contracts and dependency notice; no telemetry
producer, historical report, prediction, or acceptance decision is changed.  
**Date:** 2026-09-04. **Runtime/research owner:** Josh. **Stat meaning reviewer:** Sim.  
**Parent:** [Consecutive simulation flow plan](CONTINUOUS_SIMULATION_FLOW_PLAN.md).

## Meaning of the change

A 20-second gameplay phase will continue from the previous phase's evolved
ecosystem. Buying an upgrade or skipping it must retain creatures, resources,
ages, energy, cooldowns, absolute time, prior perception state and history.
Independent fresh-start experiments remain useful, but answer a different
question from interventions on an already-evolved world.

Existing results are not globally invalid. Their recorded observations remain
evidence for their original code, configuration, window and intervention timing.
What is invalid is presenting those results as proof of the new continuation
behavior, later-phase balance, or acquisition-order effects that were not tested.

## Notice to Stat-Line work

Sim owns stat meaning and telemetry review; Josh owns the lifecycle, upgrades and
integration. This notice does not assign Sim the runtime refactor or change an
existing sprint estimate.

The current shared API is `SpeciesSimulationMetrics.CreateHerbivoreStatLine`.
It combines accumulated counters/death events with supplied opening/closing
population. `SimulationReportSerialization` currently supplies the first and last
entries of the whole run history; `VM_SimulationShell` uses the same calculation.
Neither path currently specifies an independent phase window.

| Existing work | Conflict / invalidated assumption | Required follow-up |
| --- | --- | --- |
| S1-STAT-01 field contract | “Run” and “starting population” are no longer unambiguous. | Define expedition and phase scope, tick interval, opening/closing sample, units and metric version. |
| S1-STAT-02 raw ledger | Clearing counters/tracked entities at every break destroys continuity; repeating a boundary sample double counts events. | Preserve cumulative telemetry and record window baselines; verify source events belong to exactly one phase. |
| S1-STAT-03 derived rates | Whole-run counts cannot be combined with phase-only populations; averages of phase ratios do not reproduce expedition ratios. | Compute rates from raw numerator/denominator in the selected window; pool counts before computing an expedition rate. |
| S1-STAT-04 / current S2.3 reporting | One loadout and effective fingerprint cannot describe all ticks of a continued expedition. | Add acquisition timeline, phase identity and per-phase fingerprints to JSON, CSV, Markdown and thin UI projections. |
| S1-STAT-05 validation | Same seed plus final loadout does not determine when the upgrades were applied or the state they changed. | Include lifecycle, initial/checkpoint identity, exact acquisition schedule and options; add segmentation/replay parity. |
| S1-STAT-06 review | Earlier single-window acceptance does not establish multi-phase readiness. | Preserve old acceptance and add a new bounded continuation review/retest result. |

Current board target: [S2.3 — Upgrade loadout report/stat-line integration](https://trello.com/c/pZ4qG2DM).
The live card assigns Sim 3h, while the current repository S2 plan says Sim is
not assigned to the upgrade stream. This is an observed ownership inconsistency;
Josh must reconcile the intended scope before allocating the new work. Do not
silently turn that 3h card into the entire telemetry migration.

### Proposed window contract

- Identify an expedition/attempt and phase; declare `windowStartTickExclusive`
  and `windowEndTickInclusive`. Phase two is events `(200,400]`, opening
  population at tick 200, closing population at tick 400.
- Population snapshots can share a boundary endpoint for display; that does not
  duplicate exposure or activity. Define AUC integration and sample ownership
  explicitly. Phase peak/minimum includes the declared endpoints; whole-run peak
  and minimum come from the full trajectory.
- Subtract cumulative **raw** counter snapshots for a phase; filter timestamped
  events to its interval. Never subtract already-derived rates or reset the
  persistent metrics object to obtain a window. HPS/EHS/ECN need boundary raw
  snapshots because their full per-tick event lists are currently unavailable.
- `SPO` means opening population of the selected window. `FPO` means closing
  population; both must carry scope. Births and every removal cause reconcile
  those populations. An individual can enter a phase already old, injured,
  hungry, in a behavior state or in cooldown; birth/death identity continues.
- Preserve `NotApplicable`, invalid, unreconciled and partial statuses. No
  opportunity is distinct from zero success. Report actual ticks for early
  termination; do not pad truncated phases with fictional exposure.
- Separate population/exposure counts, ecological rates and currency. A final
  population reused at each reward break is not evidence of newly earned data.

### Existing stat limitations requiring explicit retests

1. The experimental Hare reconciliation `SPO + BIR - PREY - STRV - CRWD`
   excludes other possible creature removals, including `PopulationLimit`.
   Longer accumulated populations can expose this gap. Keep the legacy formula
   and its failure visible until Sim accepts a revised cause-complete contract;
   do not silently reinterpret PREY as all deaths.
2. `bAVG = BIR / MAT` currently rejects `BIR > MAT` in both C# and the independent
   validator. The reproduction system can produce multiple offspring per
   successful candidate. Test multi-offspring fixtures and decide whether this
   is offspring per opportunity (which may exceed one) or success probability
   (which needs successful attempts). This is an existing semantic issue, not a
   proven regression caused by continuation.
3. HPS/EHS/ECN cannot currently be independently reconstructed from full event
   lists; `VALIDATED_WITH_LIMITATIONS` remains limited after segmentation. Adding
   scope metadata alone does not close the instrumentation gap.
4. RFS/APS depend on population change, rates and validity. They are neither
   additive across phases nor accepted universal species ratings. Keep their
   experimental status and the existing Josh/Sim decision on their future role.
5. Current experimental collection/display is conditional on BEV/herbivore mode.
   The general Stat-Line cannot claim equivalent coverage for every species or
   production mode without explicit instrumentation and tests.

Retest hand-calculated windows, endpoint ownership, no-predator/no-encounter
phases, carry-over entities, cooldowns, births/deaths at the boundary, partial
last phases, population limits and pooled rates. Compare reporting enabled and
disabled to ensure instrumentation never changes the ecosystem.

## Notice to predictive AI research

The immutable upgrade snapshot adapter remains useful. Its current
`species-upgrade-prediction-input-v1` payload records ordered upgrades but no
acquisition tick, phase, evolved state or future horizon. It is sufficient for
its declared launch-time interventions; it is insufficient to specify EX-010.

Do not overwrite EX-007/008/009 predictions, reports, analyses or human decisions.
Add a versioned continuation prediction envelope around resolved snapshots and
generate new preregistrations. Keep scoring outside the simulation, as required
by SPAI-C01; do not calculate a convenient new outcome after seeing results.

The continuation input must declare:

- experiment/run/attempt identity, code/build, lifecycle and telemetry contracts;
- initial seed and base data, origin or parent checkpoint and content hash;
- resolved rules/options and acquired upgrade snapshots **at prediction time**;
- acquisition tick/phase and effective-from tick for each intended intervention,
  including explicit no-upgrade controls;
- initial/current observations allowed to the model, their versions and hashes;
- forecast window/horizon, species, units, outcome definitions and validity rules;
- comparison target: checkpoint-conditioned change, raw outcome, per-seed result
  or panel mean; development/held-out designation and frozen panel policy.

Information recorded for audit is not automatically information supplied to the
predictor. Freeze the permitted input boundary before running forecast arms;
exclude future phase outcomes, later checkpoints, scoring and held-out results.

### EX-010 implementation and experiment boundary

The existing EX-010 proposal already asks the right sequential question. Connect
it to CF-5 rather than creating another experiment with the same purpose.

Proposed development controls, to be fixed by Josh before execution:

- All-skip continuation versus one uninterrupted simulation tests segmentation.
- Baseline and A-at-boundary fork from the identical complete checkpoint tests a
  state-conditioned upgrade effect.
- A then B and B then A at matched boundary ticks tests order under that schedule.
  Include the relevant single-upgrade/timing controls before claiming which
  effect is due to first exposure, timing, combination or accumulated state.
- Matched initial seeds alone do not create matched state after earlier
  interventions diverge. Preserve lineage and state hashes; do not re-pair
  different worlds because their final loadouts happen to match.
- Keep a seed/expedition lineage wholly within one development or held-out split.
  Adjacent phases and branches of one checkpoint are correlated observations,
  not additional independent seeds. Do not split later phases from the same
  expedition into a supposed fresh validation panel.
- Include extinctions and aborts according to the preregistered target. Restricting
  a later-phase analysis to survivors changes the question and must be explicit.

The existing EX-009 five-pair result remains an accepted bounded launch-time
finding. It does not establish universal commutativity, sequential order
independence, predictive calibration or long-run ecological balance. New
continuation forecasts, sample sizing and P3 promotion remain human decisions.

Current board target: [Predictive AI research program](https://trello.com/c/DViOsvbd).
Its older program summary should not override the newer repository EX-009 and
EX-010 status. A dependency notice should preserve that chronology.

## Telemetry producer and consumer migration

Version the actual contracts at implementation time. Current inspected versions
are experiment report **23**, Play Mode report **7**, and upgrade prediction
input **v1**. Do not confuse these with metric formula or lifecycle versions.

| Contract | Required fields / behavior |
| --- | --- |
| Experiment/run manifest | Experiment ID, research run ID, expedition/attempt ID, source commit and dirty-content provenance where needed, lifecycle mode/version, report and metric contract versions. |
| Expedition origin | Scenario/player species, seed, frozen base config/fingerprint, step interval, phase schedule and terminal policy, initial state/checkpoint identity, combat/opportunity/experimental option values and versions. |
| Phase result | Phase index, parent phase/checkpoint, absolute start/end ticks, actual duration, opening/closing state hashes and populations, raw window counts, derived values/statuses, rules and loadout applied during this interval, completion/abort reason. |
| Acquisition event | Stable event ID, choice or Skip, decision tick and effective-from tick, ordered resolved modifier snapshot, cost and currency before/after, rules/loadout fingerprints before/after. |
| Expedition final result | Final outcome, absolute tick, phase count, aggregated raw totals, correctly recomputed rates, acquired timeline and settlement identity/status. |
| Checkpoint | Full replay data per the architecture plan, checkpoint schema/hash and parent lineage. A fingerprint names state but does not replace it. |
| Failure output | Last committed phase/tick, completed/partial/aborted/crashed status, error and missing fields; do not classify a failed report write as successful evidence. |

Keep one JSON record per seed/expedition with nested phase records, or an equally
explicit separate phase table. Recommendation: retain `runs` as the seed-level
unit, add phases beneath it, and export a separate phase CSV keyed by expedition
and phase. The bundle validator must check both seed cardinality and phase
cardinality/continuity. Never flatten phases into more “independent runs.”

Use separate immutable artifact directories for expedition/phase/final outputs.
`playmode-last-run.*` may remain a compatibility pointer/copy but is not the
evidence store. Emit compact boundary/upgrade/end log events with the same IDs
and ticks for SG-001 observability parity; avoid per-cell console spam.

Migrate these consumers together with producers:

- `SimulationReportSerialization`, `PlayModeSimulationResultLogger`,
  `CellularSimulationExperimentRunner` and `SpeciesUpgradePredictionInputAdapter`;
- `New-CellSimReport.ps1`, `Validate-HerbivoreStatLine.ps1` and
  `Test-CellSimArtifactBundle.ps1`, including their CSV assumptions;
- `CellSim.ps1`, `Run-CellularExperiment.ps1`, local/remote job submission,
  worker forwarding/manifests and report replay/visual evidence;
- opportunity comparison/representativeness scripts and paired-run outputs:
  either support declared continuation windows or reject them clearly;
- Noesis Stat-Line/summary projection and any future report-dashboard importer;
  consume versioned values instead of independently recalculating game stats.

Comparisons require compatible lifecycle, windows, metric definitions,
termination policies and lineage/matched cases. Scenario, grid, step, player
species, base rules and mode/options must match except for the prespecified
intervention. Whole schedules need not be identical when timing/order is the
intervention, but their differences must exactly match the declared experiment.
Report mismatch reasons; do not call every mismatch “different seeds.”

## Explicit evidence validity register

| Evidence / claim | Still valid | No longer valid as support for continued gameplay | Action |
| --- | --- | --- | --- |
| EX-001/001B reproducibility and old authored scenario panels | Observations/replay checks for their original source, setup and fresh window. | Proof of continuity, checkpoint replay or long-lived ecosystem stability. | Retain immutable evidence; add continuation parity and longer-horizon fixtures. |
| EX-002 collapse attribution / Forest Edge and Hare-Fox balance reports | Bounded effects for their declared initialization, duration, modes and values. | Estimates of phase-two-plus collapse, repeated upgrade effects or five-phase success rates. | New baseline on evolved states; keep mortality causes and all phases visible. |
| BEV/Block/cooldown/opportunity-isolation studies | Mechanic/diagnostic results under the documented experiment. | General continued-world avoidance/defense claims without cooldown, history and ID replay checks. | Preserve diagnostic mode; retest cross-boundary cooldown/perception/identity. |
| EX-007/008 forecasts and scores | Historical pilot observations under the original information/metric contract, including known PREY and scoring limitations. | Forecast accuracy for upgrades acquired later, or calibration of continuation predictions. | New envelope/preregistration; never retrofit the old forecast. |
| EX-009, including adapter reruns over seeds 106–110 | Accepted zero-delta launch-time result for the tested additive pair. | Sequential acquisition commutativity or absence of timing/state effects. | EX-010 after CF-5 and a human-approved contract. |
| Schema-21 EX-007 vs schema-23 adapter output | Original raw artifacts; matched core payload comparisons where documented. | Unqualified cross-version derived-stat comparisons. | Keep the 2026-09-04 adapter handoff's metric-version restriction. |
| Current schema-23/Play Mode schema-7 reports | Fresh-window records exactly as generated. | Complete reproduction of acquisition timing or evolved checkpoints. | New schemas; do not manufacture missing phase fields in old files. |
| S1/S2 green tests and accepted upgrade slice | Their executed single-window/configuration/snapshot behavior. | Proof of continued-world gameplay or all new report semantics. | Add the architecture plan's continuity/integration gates. |
| Old population/currency averages or rate/composite aggregates | Descriptive results within their original window/contract. | Continued expedition economy, later-phase balance, or pooling with new windows. | Separate baseline families; recompute from appropriate raw data only. |

This register is an applicability overlay, not a rewrite or blanket rejection of
historical evidence. Link it from active report/research indexes. Keep completed
reports, historical handoffs, preregistrations, raw bundles and human decisions
immutable. Any replacement experiment or analysis receives a new identity and
links its predecessor.

## Coordination and release gate

Before implementation, Josh and Sim review the window and stat-definition
contract together and resolve the board/repository ownership discrepancy.
Before collecting continued-play balance evidence, all relevant writers,
validators and comparators must support it or reject it explicitly. Before
EX-010, Josh freezes the new experiment and forecast information boundary.

The planning task records notices in the canonical documents and on the two
existing workstream cards. A posted notice is not an acknowledgment by Sim,
acceptance of the plan, a sprint reassignment, or an experiment authorization.
The local handoff records whether external delivery actually succeeded.
