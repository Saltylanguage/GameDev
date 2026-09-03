# AI-Assisted Ecology Laboratory Research Plan

**Status:** Proposed research program  
**Version:** 1.12<br>
**Created:** 2026-08-15  
**Primary question:** Can a deterministic ecological simulation, connected to AI through an auditable evidence workflow, help a small team discover, test, explain, and promote better game rules—including validated variable interactions and risk thresholds—without surrendering human design authority?

## Executive position

This project may be onto a distinctive combination of systems:

```text
Deterministic ecology simulation
    -> controlled experiments
    -> telemetry, replay, and comparison
    -> AI-assisted hypothesis and diagnosis
    -> interaction maps, thresholds, and risk statements
    -> human design decision
    -> player-facing ecology laboratory
```

The project must not claim technical novelty, research novelty, or intellectual
property value from this idea without a separate literature, prior-art, and
legal review. The immediate goal is to test whether the workflow produces
better evidence and better decisions in practice.

The working thesis is:

> An auditable AI-assisted ecology laboratory can turn emergent simulation
> behavior into explainable, reproducible, and player-relevant design
> knowledge while keeping experiments bounded and humans accountable. Over
> time, accumulated evidence should allow the assistant to identify and test
> predictable interactions between variables, including thresholds, spatial
> effects, and extinction cascades.

This is both a research program and a possible production advantage. It should
be treated seriously, but it must earn promotion through evidence rather than
enthusiasm.

The broader predictive change-impact direction is captured in the companion
[Predictive Change Impact Analysis Research Brief](CHANGE_IMPACT_ANALYSIS_RESEARCH_BRIEF.md).

## Canonical program model

This document is the canonical source of truth for the research program. The
source readings, change-impact brief, experiment packages, reports, analyses,
and templates linked from it are supporting records; they should not introduce
an independent protocol or contradict a decision recorded here.

The current operating model extends the five-stage evidence chain into a
repeatable knowledge loop:

```text
Experiment
    -> Run
    -> Report
    -> Analysis
    -> Evidence dataset
    -> Candidate change-impact prediction
    -> Range/intervention validation
    -> Human Decision
    -> accepted or rejected knowledge
    -> next bounded Experiment
```

The dataset is not a replacement for the original artifacts. It is a derived,
lineage-preserving index that lets humans and AI compare reports, accumulate
patterns, and generate new testable hypotheses. A prediction may become more
trusted through replicated validation and human confirmation, but no model
should silently rewrite the evidence, the acceptance criteria, or the human
design intent.

## Current research synthesis

The initial change-impact reading pass supports the following working rules:

- Keep structural dependencies, historical couplings, runtime deltas,
  sensitivity results, and human design interpretation as separate evidence
  layers.
- Define impact ground truth before measuring prediction: output, tolerance,
  time window, and comparison scope.
- Return **Not currently testable** for unsupported concepts and
  **Underdetermined** when instrumentation or coverage cannot distinguish
  plausible outcomes.
- Use staged budgets: broad screening, paired intervention runs, then joint
  designs or metamodels when interaction questions justify them.
- Track freshness against build, ruleset, telemetry schema, scenario, input
  coverage, and evidence date; stale findings require revalidation.
- Measure false positives as well as false negatives. A noisy report can be as
  harmful as a missed impact if it creates false confidence or review burden.
- Keep model prediction separate from judgments about fun, balance, quality,
  agency, or commercial value.
- Treat determinism as an experimental advantage, not proof: coverage and
  held-out validation still determine whether a prediction earns promotion.

The article-specific evidence, limitations, and deductions remain in [Change
Impact Analysis Source Readings](CHANGE_IMPACT_ANALYSIS_SOURCE_READINGS.md).

## What makes the opportunity interesting

The project already has several pieces of an evidence spine:

- Seeded, deterministic simulation runs.
- Immutable run-start scenario data.
- Ruleset fingerprints.
- Population histories and per-species activity telemetry.
- Mortality, combat, movement, and behavior-state evidence.
- Headless batch execution through `CellSim`.
- Readable reports and machine-readable JSON/CSV artifacts.
- Representative replay and visual capture.
- A player concept built around observing and manipulating ecological systems.

The possible distinction is not any one technique. Unity connectivity,
cellular automata, deterministic simulations, AI-generated code, and AI report
reading are all established ideas. The question is whether their controlled
combination creates a repeatable system in which AI can help discover and
explain game rules, while the same evidence remains useful to players.

## Research objectives

1. Establish whether the current simulation can serve as a trustworthy
   experimental instrument.
2. Measure whether AI-assisted analysis reduces time from question to useful
   evidence without increasing false conclusions.
3. Test whether AI can propose bounded, testable hypotheses rather than vague
   design suggestions.
4. Test whether telemetry and replay are sufficient to explain important
   outcomes such as extinction, runaway growth, or upgrade dominance.
5. Define the human review and promotion process for turning evidence into game
   rules, upgrades, events, and player-facing explanations.
6. Determine which parts of the workflow are worth building into permanent
   studio tooling.
7. Test whether accumulated, standardized reports support generalized,
   validated change-impact predictions for any feature and outcome that the
   model can represent and test, including interactions, thresholds, cascades,
   spatial effects, and out-of-known-range warnings.

## Questions the program must answer

### R1 - Reproducibility

Can the team reproduce the same result from the same scenario, ruleset
fingerprint, seed, and run configuration across repeated executions and
machines?

### R2 - Evidence sufficiency

Can a human or AI identify the important causal sequence behind an outcome from
reports and replay, or are we merely observing correlated final counts?

### R3 - Bounded hypothesis quality

Can AI propose a change that has a clear independent variable, expected effect,
bounded scenario scope, fixed baseline, and measurable success criteria?

### R4 - Diagnostic accuracy

When AI explains a result, how often is the proposed cause supported by a
follow-up controlled experiment?

### R5 - Workflow value

Does AI reduce time-to-evidence and time-to-decision for designers and
developers, or does it create more review and correction work than it saves?

### R6 - Human control

Can the team keep AI proposals, factual evidence, and accepted design decisions
separate and auditable throughout the workflow?

### R7 - Player translation

Can an internal causal finding become a clear player-facing explanation without
exposing raw developer controls or making the simulation feel deterministic in
the wrong way?

### R8 - Generalization

Does the workflow remain useful across species, biomes, upgrades, event cards,
and curator-mode goals, or does it only work for one narrow Forest Edge
fixture?

### R9 - Generalized change impact

As the evidence corpus grows, can the assistant predict the observable impact of
an arbitrary model-representable change on held-out runs? The change may affect
any feature type or system layer, and the outcome may be a population, movement
pattern, resource level, performance measure, failure mode, state transition,
visual result, or another explicitly instrumented output.

### R10 - Calibrated design risk

Can the assistant produce a conditional risk statement that separates an
evidence-backed ecological prediction from a human-owned judgment about balance,
agency, or engagement? Can its confidence and severity ratings be calibrated
against later runs and playtests rather than inferred from wording alone?

## Required protocol

Every research activity follows the five-stage chain:

```text
Experiment -> Run -> Report -> Analysis -> Human Decision
```

These are different artifacts and responsibilities.

In this protocol, **pre-specified means human-owned and recorded before
execution**. AI may help draft an experiment, suggest variables, or identify
possible telemetry gaps, but the named human decision owner must approve the
question, scope, controls, success criteria, and expected report before runs
begin. A material change after execution requires a revised experiment or a
new experiment; it must not silently rewrite the original specification.

### 1. Experiment

The human-approved experiment describes the question before execution. It must
state:

- Experiment ID.
- Question and hypothesis.
- Independent variable.
- Controlled variables.
- Baseline and variant definitions.
- Seed range and sample-size rationale.
- Success and failure criteria.
- Expected report template.
- Human decision owner.

An experiment may produce many runs. It is not itself evidence of an outcome.

### 2. Run

A run is one execution under a specific scenario, build, seed, ruleset, and
configuration. It must preserve enough provenance for replay:

- Run ID and parent Experiment ID.
- Scenario asset and player species.
- Seed and seed range position.
- Ruleset fingerprint.
- Upgrade/event loadout.
- Build, branch, commit, and tool versions.
- Start configuration and environment.
- Completion, manual stop, crash, or extinction status.

Run data is immutable. A changed input or source revision creates a new run.

### 3. Report

The report records observed facts and evidence. It must remain low-noise and
feature-specific. It should include outcomes, changed metrics, event timelines,
anomalies, errors, and artifact links, but not silently convert correlation
into causation.

Use the report contract and bundle defined by [SG-001 - AI Generated
Reports](../Studio%20Guidelines/AI_GENERATED_REPORTS.md).

### 4. Analysis

AI or human analysis may identify patterns, likely causes, uncertainty, and
next experiments. It must cite report evidence and state confidence. AI
analysis is a derived artifact and must not overwrite the factual report.

The analysis must distinguish:

- Observed fact.
- Inference.
- Hypothesis.
- Recommendation.
- Unresolved uncertainty.

For an interaction or risk claim, the analysis must also identify the valid
parameter regime, held-out validation evidence, and whether the conclusion is
ecological, design-related, or playtest-validated.

### 5. Human Decision

A named human reviews the evidence and analysis, then records one of:

```text
Accept
Reject
Revise and rerun
Inconclusive
Promote to production experiment
Promote to player-facing design
Archive
```

Only the human decision can promote a rule, upgrade, event, telemetry change,
or workflow change. AI must not self-promote its own recommendation.

#### Minimal decision record

Human review should remain concise enough that it is consistently completed.
The minimum record is:

```text
Decision: Accept | Reject | Revise and rerun | Inconclusive | Promote | Archive
Key Observation: one sentence describing the decisive human observation
Evidence References: report, analysis, replay, or playtest IDs
Scope: what the decision does and does not authorize
```

Optional reviewer, date, follow-up, expiry, and decision-authority metadata may
be added when useful. `Key Observation` replaces a free-form rationale: it is a
human signal for later analysis, not a substitute for the underlying evidence.
Accepted and rejected decisions are valuable labels for future study, but a
human decision is a governance gate. It can veto promotion or action without
pretending that the empirical result itself is false.

## Predictive change-impact and intervention contract

The long-term research target is not an assistant that merely sounds confident
or only predicts ecological cascades. It is a generalized, simulation-backed
change-impact capability that can answer any intervention question the model can
represent and test, then expose the limits of the answer.

The same contract must support questions such as:

- “What changes if hare speed and fox vision are raised simultaneously?”
- “What downstream output changes when this resource value is reduced?”
- “Which tests, replays, metrics, or state transitions are likely to change if
  this rule or asset field is modified?”

If a requested concept is not represented by the model—for example, animal
weight in a simulation that has no weight field—the correct result is **not
currently testable**, not an invented prediction. The system may recommend the
model extension needed to make it testable, but that extension becomes a new
experiment and must not be smuggled into the analysis.

Every change-impact finding has two explicitly separated layers:

### Layer A - Evidence-backed model claim

This layer is derived from controlled runs and may state:

- The change, baseline, and intervention values.
- The feature types and model components affected.
- The outcome or metric being evaluated, including its time window.
- Effect size, outcome distribution, and uncertainty.
- Parameter/configuration regime where the finding was observed.
- Held-out runs or scenarios used for validation.
- Evidence IDs, report paths, and replay or test candidates.
- Whether the claim is correlational, mechanistically supported, or causally
  supported by a follow-up intervention.
- Whether the relationship is range-invariant, regime-dependent, or unresolved
  across the requested feasible range.

### Layer B - Human-owned interpretation

This layer applies an explicit project standard to Layer A. It may identify a
balance, safety, performance, agency, quality, or engagement threshold, but the
threshold must be defined before evaluation and owned by a human decision. A
simulation output alone cannot establish that a feature is fun, engaging, or
commercially acceptable.

### Required change-impact record

```text
Change-impact ID
Change request and intervention
Baseline and comparison scope
Feature type, model component, and validated range coverage
Outcome type, metric, and time window
Effect size and outcome distribution
Held-out validation evidence
Direct dependency and affected-artifact set
Confidence and calibration status
Supported / Not currently testable / Underdetermined
Range status: Range-invariant / Regime-dependent / Unresolved
Coverage or out-of-distribution warning, when applicable
Human threshold and severity rubric, if applicable
Evidence IDs and replay/test candidates
Human decision and follow-up
```

### Capability and testability gate

Every request must be classified before analysis:

| State | Meaning |
|---|---|
| Supported | The feature, intervention, and outcome are represented and instrumented well enough to test. |
| Not currently testable | The requested concept or output does not exist in the model or telemetry. A model-extension experiment is required. |
| Underdetermined | The concepts exist, but current evidence or telemetry is insufficient to distinguish plausible outcomes. |

### Target output shape

The following is a generalized form, not an unverified claim about the current
simulation:

> Under baseline B, applying intervention I changes outcome O over time window
> T by effect E across N held-out runs. Across the requested feasible range the
> relationship is **range-invariant**, or the report identifies the tested
> **regimes and thresholds** where the effect changes. Confidence is C and the
> result crosses threshold Q under rubric version V. If coverage is insufficient
> to establish either form of generalization, the result is **Unresolved** rather
> than a high-confidence prediction. If the requested feature is absent from
> the model, the result must instead say **Not currently testable** and identify
> the required model extension.

The final report must include the values, sample size, effect, uncertainty,
tested range coverage, range status, capability state, and evidence links. A
severity number without its rubric is not a research result.

### Confidence and severity rules

- Confidence is calculated from evidence quality, replication, held-out
  performance, and calibration history; it is not a free-form AI feeling.
- Severity is distinct from confidence. A low-confidence possibility may still
  deserve investigation, but it must not be presented as a high-confidence
  balance conclusion.
- The initial severity rubric should consider population damage, likelihood,
  persistence, player recovery options, impact on meaningful choices, and
  reversibility.
- Range generalization is a validation gate, not a permanent excuse for a
  narrow prediction. A high-accuracy prediction should replicate across the
  requested feasible range, aside from statistical outliers.
- A systematic threshold, phase transition, or other contiguous behavior
  change is not an outlier. Report it as a regime-dependent, piecewise
  prediction with the boundary and uncertainty.
- If the requested range has not been covered well enough to distinguish
  range-invariant behavior from regime dependence, mark the result
  **Unresolved** and run the missing coverage before making a high-confidence
  statement. Do not silently extrapolate.

The following is a proposed starting rubric and remains subject to human design
approval and later calibration:

| Severity | Meaning |
|---:|---|
| 1/5 | Negligible effect; localized, reversible, and unlikely to alter meaningful play. |
| 2/5 | Minor measurable effect; viable strategies and recovery options remain clear. |
| 3/5 | Significant effect; a strategy, species, or pacing target is impaired but recoverable. |
| 4/5 | Major effect; a viable strategy is frequently invalidated or a cascade is difficult to recover from. |
| 5/5 | Critical systemic effect; the scenario or intended player agency is effectively destroyed. |

### Causal-status rules

The assistant may use stronger causal language only as the evidence earns it:

| Status | Minimum meaning |
|---|---|
| Observed association | Variables move together in the recorded reports; no intervention claim. |
| Mechanistically consistent | Telemetry and timing fit a proposed mechanism, but alternatives remain. |
| Causal evidence supported within model scope | A human-approved intervention recorded before execution, compared against a same-seed baseline across replicated runs, produces a measurable effect supporting the proposed mechanism. |
| Robust causal relationship within validated range | The intervention survives held-out seeds/scenarios and range coverage, with no credible competing explanation in the instrumented model. |
| Unresolved | Evidence, telemetry, or range coverage cannot distinguish the alternatives. |

These statuses describe the supplied model and instrumented scope only. They do
not establish a model-independent or real-world law. Multi-variable claims
require a design that can separate interaction effects (for example, a
factorial comparison), not merely a simultaneous before/after change.

### Recursive improvement (stretch)

The evidence loop may eventually run recursively: accepted evidence updates a
prediction registry, the registry proposes the next bounded experiment, and a
headless job evaluates it on a safe feature branch. This is an optional stretch
goal, not permission for autonomous production changes. The recursive lane
requires an isolated branch/sandbox, fixed compute and experiment budgets,
immutable provenance, rollback, unchanged human-owned criteria, and an explicit
human decision before any promotion. Early iterations should be described as
updating or calibrating the impact model; “training” is only appropriate once a
separate model-training design has been approved.

## Experimental design rules

To make conclusions trustworthy:

- Define the hypothesis before running the comparison whenever practical.
- Hold the seed range and baseline configuration fixed for A/B comparisons.
- Preserve the full report even when the result is boring or contradictory.
- Use enough seeds to distinguish a pattern from a single lucky run.
- Record failed hypotheses and rejected recommendations.
- Run a follow-up intervention when AI claims a cause.
- Prefer effect sizes, distributions, and outcome rates over one representative
  anecdote.
- Use replay to inspect representative and boundary cases, not only the
  average run.
- Use one-variable and two-variable sweeps when searching for interactions;
  preserve held-out seeds or scenarios for validation.
- Track spatial metrics such as occupancy, movement entropy, region
  concentration, and time-to-displacement when a global population total could
  hide the effect.
- Record thresholds and regime changes explicitly; do not reduce a nonlinear
  cascade to a single average.
- Never tune toward a desired conclusion by silently changing the seed range,
  report fields, or success criteria after seeing the result.

These rules are research defaults, not a demand for perfect academic
methodology in every prototype. If an experiment deliberately deviates, record
why and what confidence is lost.

## AI role and boundaries

AI may draft or propose an experiment, but it does not pre-specify one. A
human decision owner must approve and record the experiment contract before
execution, including its question, scope, variables, controls, success
criteria, and expected report fields.

AI is encouraged to:

- Translate a design question into a bounded experiment brief.
- Identify missing telemetry and propose report fields.
- Generate candidate parameter variants inside an approved scenario boundary.
- Run approved seed ranges using existing project commands.
- Compare baseline and variant reports.
- Trace possible causal chains through recorded events.
- Suggest follow-up experiments and replay seeds.
- Maintain a structured change-impact registry with evidence links, valid
  ranges, confidence, and unresolved caveats.
- Produce conditional ecological risk statements and apply an approved design
  rubric without conflating the two.
- Draft player-facing explanations from accepted evidence.

AI must not, without explicit human approval:

- Change arbitrary production code to make a hypothesis pass.
- Modify the baseline after seeing the result.
- Choose the success criteria after the experiment runs.
- Treat a correlation as a proven cause.
- Present an unresolved or regime-dependent range result as a single
  range-invariant prediction.
- Declare that a change is balanced or engaging solely from simulation output.
- Assign an unexplained severity score or use confidence language as a
  substitute for replication and calibration.
- Promote an AI recommendation into a product decision.
- Expand a bounded experiment into a general tooling framework.
- Publish novelty, scientific, legal, or commercial claims.
- Alter shared scenes, serialized assets, or another developer's work merely to
  unblock the experiment.

The AI-assisted development safeguards in [SG-002 - AI-Assisted
Development](../Studio%20Guidelines/AI_ASSISTED_DEVELOPMENT.md) apply to the research lane.

## Experiment portfolio (ranked implementation order)

The EX identifiers are stable record IDs, not a promise that numeric order
equals execution order. The entries below are ordered by implementation
dependencies and intended progression through the research program.

### EX-001 - Reproducibility baseline

**Question:** Does the current Forest Edge scenario reproduce exactly from the
same seed, scenario asset, and ruleset fingerprint?

**Method:** Run the existing baseline over a fixed seed range, repeat the same
range, compare fingerprints, final grids, population histories, and outcome
summaries, then replay at least one representative .seed

**Success:** Identical inputs produce identical machine-readable outcomes, and
any intentional nondeterminism is documented rather than hidden.

**Why first:** Every later AI claim depends on the simulation being a reliable
instrument.

The working package is documented in [EX-001 - Reproducibility Baseline](Experiments/EX-001-Reproducibility-Baseline/README.md).

### EX-001B - Cross-scenario determinism extension

**Question:** Does the shared simulation engine reproduce identical
machine-readable outcomes when each currently authored scenario is repeated with
identical inputs?

**Method:** Repeat the same schema-4 seed range for ForestEdge, OpenRange,
Wetland, and BaselineParity using each scenario's authored grid and player
species. Compare ruleset fingerprints, complete run payloads, population
histories, and final summaries within each scenario pair.

**Success:** Every included scenario pair matches after generated metadata is
excluded, and any environment or scenario-specific limitation is explicit.

**Scope boundary:** A pass supports reproducibility across the tested authored
scenarios only. It does not prove that all cellular automata are deterministic or
that ecological findings transfer between scenarios.

The working package is documented in [EX-001B - Cross-Scenario Determinism](Experiments/EX-001B-Cross-Scenario-Determinism/README.md). All four authored scenarios have complete matching pairs, and the human design owner accepted the bounded reproducibility result. EX-002 is the next causal experiment.

### EX-002 - Herbivore collapse attribution

**Question:** Can the evidence spine identify and causally test a simulation's
rule-defined collapse state, using herbivore decline or extinction in current
BaselineParity as the first concrete adapter?

**Method:** Have the simulation adapter define when a cell type no longer has a
practical rule-governed path to increase its population. This may be extinction,
an unmatched mating requirement, an unavailable growth destination, or another
domain-specific condition; it may be desired rather than harmful. Use the
current BaselineParity reports and telemetry to test candidate causes such as
starvation, movement pressure, predation, reproduction limits, or
terrain/resource identity. The accepted ForestEdge result remains the
instrument-trust reference; BaselineParity is the first concrete adapter.

**Success:** A causal explanation is supported by changed same-seed evidence in a
follow-up intervention, with the adapter's collapse rule, endpoint, validated
range, and remaining instrument gaps recorded. A plausible narrative alone is
insufficient, and no ecology-specific rule is promoted as universal.

**Readiness note:** The EX-002 brief records the intervention surface and the
schema-5/6 run-window, tracked-FSM, per-death-cause, and reproduction-funnel
telemetry. The matched schema-6 control, intervention matrix, and held-out
check are complete for the bounded BaselineParity window. The newer schema-7
food-action counters are a separate current-code instrument change; do not
silently merge them into the historical matrix. The original batch-startup
failure is historical; only a fresh unlocked Unity rerun remains pending.

### EX-003 - AI recommendation validity

**Question:** Can AI propose useful, bounded changes that survive controlled
validation?

**Method:** Give AI the experiment contract, baseline report, and allowed
parameter surface. Capture its hypothesis, predicted direction, and suggested
variant. A human approves the variant before execution. Compare the prediction
with the resulting report.

**Measures:** Prediction accuracy, false-cause rate, useful recommendation rate,
review time, and number of reruns required to clarify the proposal.

### EX-007 - Generalized predictive change impact

**Question:** Can accumulated simulation evidence support a conditional
change-impact statement about any model-representable feature and observable
outcome, including simultaneous changes, thresholds, or cascades?

**Method:** Begin with a small matrix of supported changes in the ForestEdge
fixture, including at least one simultaneous two-variable intervention. The
initial example may use fox speed and fox reproduction, but the experiment
contract must remain domain-neutral: the same method should work for any
supported feature and output. Measure the requested outcome over a named time
window, preserve direct dependency and affected-artifact evidence, and reserve
seeds or a second scenario for validation. Include capability state, range
status, causal status, coverage warnings, and uncertainty. Add a severity score
only after applying an approved human rubric.

**Success:** The assistant predicts the direction and approximate regime of the
effect on held-out runs, reports uncertainty and limits, rejects unsupported
feature requests as not currently testable, and keeps model evidence separate
from balance, quality, or engagement judgments. A human reviewer can trace the
statement to reports and decide whether a model extension, playtest, or
implementation should follow.

### EX-008 - Reversed upgrade-order follow-up

**Question:** Does applying the same Hare upgrades in the opposite order change
the observed outcome relative to EX-007's forward-order sequence?

**Method:** Use the validated EX-007 baseline and run the crowding-only arm plus
the reversed joint sequence (`crowding-tolerance,faster-movement`) on ForestEdge
with the same combat and opportunity settings. Compare population, births,
mortality, predation, movement, resource, and encounter telemetry.

**Scope:** This is follow-up evidence, not a new pre-registered AI prediction.
Because its held-out seeds differ from EX-007's forward-order panel, it cannot
by itself establish an order-only causal result.

The working package is documented in [EX-008 - Reversed Upgrade-Order
Follow-up](Experiments/EX-008-Reversed-Order-Followup/README.md).

### EX-009 - Same-held-out-seed upgrade-order comparison

**Question:** When `faster-movement` and `crowding-tolerance` are applied in
opposite orders, do the two sequences produce different outcomes on the same
held-out seeds?

**Method:** Run `faster-movement,crowding-tolerance` and
`crowding-tolerance,faster-movement` on ForestEdge with Hare, seeds 106–110,
identical combat/opportunity settings, and complete artifact validation. Compare
per-seed final population, births, mortality, predation, movement, resource,
and available encounter telemetry. Do not use the prior different-seed panels
as a substitute.

**Success:** Both arms complete with valid bundles and the pairwise deltas are
reported. Order independence may be stated only if the same-seed comparison
supports it; otherwise the result is order-sensitive or unresolved.

The working package is documented in [EX-009 - Same-Held-Out-Seed Upgrade-Order
Comparison](Experiments/EX-009-Same-Heldout-Order-Comparison/README.md). The
first execution attempt is recorded as blocked before simulation by Unity
Package Manager IPC failure.

### EX-004 - Replay-to-explanation

**Question:** Can a representative replay and report support a player-readable
explanation of why a run succeeded or failed?

**Method:** Select representative, boundary, and surprising runs. Compare AI
analysis with human designer explanations and a small comprehension check.

**Success:** Players can identify the major pressure, intervention, and outcome
cause without seeing raw developer telemetry.

### EX-005 - Upgrade and event discovery

**Question:** Can the workflow identify upgrades or simulation events that create
meaningfully different strategies rather than only changing final numbers?

**Method:** Define a small approved upgrade/event surface, run controlled
comparisons, measure activation and ecological consequences, then validate
distinct builds through seeded runs and human playtests.

### EX-006 - Curator stability discovery

**Question:** Can the same evidence workflow help discover what makes a
self-sustaining biome stable and resilient?

**Method:** Define biome stability metrics, apply bounded environmental events,
and compare recovery, biodiversity, resource renewal, and extinction outcomes.

## Measures of success

### Instrument quality

- Reproduction rate for identical inputs.
- Ruleset and scenario fingerprint coverage.
- Report completeness rate.
- Replay fidelity.
- Canonical outcome-hash coverage for normalized reports.
- Percentage of important outcome transitions with usable evidence.

### AI analysis quality

- Supported-cause precision after follow-up experiments.
- Unsupported-cause or hallucination rate.
- Hypothesis specificity and testability.
- Useful recommendation rate.
- Human correction rate.
- Confidence calibration.
- Held-out impact-direction accuracy.
- Threshold crossing precision and recall.
- Out-of-distribution warning precision.
- False-impact, false-cascade, and false-balance-alarm rate.

### Workflow value

- Time from question to first useful evidence.
- Time from run completion to accepted decision.
- Number of manual steps and reruns.
- Review burden per accepted recommendation.
- Percentage of experiments that end in a clear decision.

### Design and player value

- Number of accepted rules or upgrades with explainable effects.
- Build differentiation across controlled runs.
- Player ability to explain an outcome.
- Player comprehension of intervention risk and reward.
- Replay intent and willingness to try another strategy.
- Agreement between predicted severity and calibrated human review.
- Agreement between balance-risk predictions and later playtest findings.

No single metric proves the thesis. The program succeeds only if evidence quality,
workflow value, and player/design value improve together.

## Program phases and gates

Effort is approximate shared team effort and must not silently replace the
current production sprint commitment. Start with one protected research package
per sprint; increase capacity only when a gate earns it.

| Phase | Approx. effort | Focus | Exit gate |
|---|---:|---|---|
| P0 - Frame the program | 1 sprint / 4-6 hours | IDs, hypotheses, rubric, ownership, report templates | EX-001 brief and decision protocol accepted |
| P1 - Trust the instrument | 1-2 sprints / 10-20 hours | Reproducibility, cross-scenario coverage, fingerprints, replay, telemetry gaps | EX-001/EX-001B pass or known limits are explicit |
| P2 - Diagnose outcomes | 1-2 sprints / 10-20 hours | Herbivore collapse, spatial diagnostics, and causal follow-ups | EX-002 has evidence-backed explanation |
| P3 - Bound AI discovery | 2-3 sprints / 20-30 hours | AI hypotheses, change-impact maps, thresholds, calibration, and human approval | EX-003/EX-007 meet held-out prediction and false-cause gates |
| P4 - Translate to design | 1-2 sprints / 10-20 hours | Upgrades, events, explanations, curator metrics | EX-004/EX-005 produce accepted design candidates |
| P5 - Validate the collaboration loop | 2-3 sprints / 20-30 hours | Repeatability across scenarios and contributors | Workflow is useful beyond one fixture |
| P6 - Promotion decision | 1 sprint / 4-10 hours | Production, studio practice, external research review | Decide what becomes product/tooling/process |

The program may stop at any gate. A failed gate is valuable if it identifies
which assumption was wrong.

## Evidence and storage contract

Use stable IDs:

```text
EXP-001  experiment definition
RUN-001-0001  one execution of an experiment
RPT-RUN-001-0001  factual report bundle
ANL-RPT-RUN-001-0001-v1  derived analysis
DEC-EXP-001-0001  human decision
CIA-001-0001  change-impact candidate or validated relationship
RISK-CIA-001-0001-v1  derived model/design risk statement
```

Persistent experiment briefs belong under `docs/Research/Experiments/` when the
portfolio grows. Generated run artifacts remain under the existing ignored
`artifacts/` path. Accepted findings should link to the relevant roadmap item,
design document, code change, and report bundle.

The derived knowledge store should keep these layers separate and traceable:

1. **Raw evidence:** immutable run outputs and factual reports.
2. **Normalized evidence:** comparable metrics, events, fingerprints, and
   feature/outcome records extracted from reports.
3. **Analysis:** AI or human interpretations, with model/prompt/version
   provenance and explicit uncertainty.
4. **Prediction registry:** candidate and validated change-impact records,
   including range status, causal status, calibration, and freshness.
5. **Human decision ledger:** compact decisions and `Key Observation` entries,
   linked to the evidence they reviewed.

Derived layers may be rebuilt, but must never replace the source artifacts.
Every record needs lineage to its parent IDs, schema/version metadata, and a
clear stale or superseded state when the model, telemetry, or acceptance
criteria change.

Every research bundle should preserve:

- The original hypothesis and success criteria.
- Baseline and variant definitions.
- Seed range and ruleset fingerprints.
- Raw machine-readable outputs.
- Human-readable report.
- AI analysis version and provenance.
- Change-impact records with tested range coverage, range status, uncertainty,
  causal status, calibration status, and held-out validation evidence.
- Design thresholds and severity rubric version used for interpretation.
- Human decision and follow-up action.

## Risks and controls

| Risk | Control |
|---|---|
| AI invents a causal explanation | Require evidence links and controlled follow-up experiments |
| Fixed seeds hide real variation | Use explicit seed ranges and boundary/replay cases |
| Correlation is mistaken for an impact or cause | Use factorial sweeps, controlled interventions, and held-out validation |
| A regime change is hidden by averages | Record distributions, thresholds, and spatial metrics |
| The assistant overgeneralizes across a range | Require range-coverage metadata; distinguish range-invariant, regime-dependent, and unresolved results |
| A model or telemetry change invalidates old predictions | Fingerprint the model/schema and mark affected records stale until revalidated |
| Human decisions become untraceable labels | Store a compact decision, one-sentence Key Observation, scope, and evidence references |
| Severity appears precise but has no shared meaning | Version a human-owned severity rubric and calibrate it against review |
| Balance or engagement is inferred from ecology alone | Separate ecological claims from design thresholds and playtests |
| Telemetry becomes noisy or expensive | Add fields only for a named question; measure report usefulness |
| AI converges on safe but uninteresting designs | Keep human design goals and novelty of playstyle in the decision gate |
| Research platform outruns the game | One bounded package per sprint; reuse `CellSim` first |
| Shared work is damaged by experimentation | Scenario boundaries, branches, ownership checks, and SG-002 alerts |
| A plausible combination is mistaken for novelty | Separate literature, prior-art, and legal review before claims |
| Players see diagnostics instead of a game | Translate accepted findings into readable feedback and choices |

## First execution plan

### Current program state

- The initial source-reading pass for change-impact analysis is captured in
  [Change Impact Analysis Source Readings](CHANGE_IMPACT_ANALYSIS_SOURCE_READINGS.md).
- EX-001 is accepted as a current-code ForestEdge reproducibility baseline.
- EX-001B has an accepted bounded reproducibility result for matching
  current-code pairs across ForestEdge, OpenRange, Wetland, and BaselineParity.
- EX-002 (BaselineParity herbivore decline/extinction attribution) has a
  bounded deterministic schema-6 control, matched intervention matrix, and
  held-out check over the declared seed ranges. Its interpretation remains
  scoped to that protocol.
- CIA-001 remains proposed research work, not validated predictive capability.
  EX-007 now has complete training and held-out run/report/analysis evidence;
  its human decision remains pending. EX-008 added a crowding-only arm and a
  reversed sequence, but used a different held-out panel from the original
  forward sequence. EX-009 is the locked same-held-out-seed A/B follow-up; its
  forward arm is currently blocked at the Unity Package Manager preflight gate.

The first research package should be small enough to complete without blocking
the current production lane:

1. Create and review the [EX-001 package](Experiments/EX-001-Reproducibility-Baseline/README.md).
2. Run the existing Forest Edge baseline over a fixed seed range twice.
3. Compare ruleset fingerprints, final grids, histories, and outcome summaries.
4. Replay one representative and one boundary seed.
5. Generate the SG-001 report bundle and a separate AI analysis.
6. Record a human decision: pass, identify an instrument gap, or revise the
   experiment.
7. Record and review the accepted EX-001B cross-scenario extension.
8. Preserve the completed EX-002 package and its bounded interpretation; create
   a new protocol before expanding the intervention surface or telemetry.

This first package should avoid new simulation mechanics, generalized AI
frameworks, dashboards, or autonomous code changes. It is a trust-building
experiment for the evidence spine.

## Promotion rules

Research findings may be promoted to production only when:

- The result has a reproducible run or a documented reason it cannot be.
- The report and analysis are complete.
- Any change-impact claim has tested range coverage, a range status, uncertainty,
  capability state, causal status, and held-out validation evidence. A
  range-invariant claim must demonstrate replication across the requested
  feasible range; otherwise it must be regime-dependent or unresolved.
- A human decision owner accepts the finding.
- The change has a bounded implementation scope.
- A follow-up test or playtest is named.
- Any balance, severity, or engagement interpretation uses a versioned human
  design rubric.
- The result is linked to the relevant roadmap or design document.

An interesting result without a decision is research. A promising hypothesis
without a controlled run is a proposal. Neither should silently become a game
rule.

## Open questions

- What minimum telemetry is needed to identify a causal chain without making
  reports unreadable?
- What data volume and experimental coverage are sufficient for reliable
  multi-variable change-impact prediction?
- Which spatial metrics best capture meaningful confinement, mobility loss, or
  habitat displacement?
- How should acceptable survival, diversity, agency, and pacing thresholds be
  defined and versioned?
- How should the severity rubric be calibrated against human review and
  playtest outcomes?
- Which AI model, prompt, and context package produce the most reliable
  bounded hypotheses?
- How should we score a recommendation that is scientifically correct but not
  fun?
- How much player-facing explanation improves agency before it becomes noise?
- Does curator mode require different stability metrics than species mode?
- Which portions of this workflow are worth exposing as player-facing Lab
  fiction and which should remain production tooling?
- Is the combination distinctive enough to justify external research,
  publication, patent, or other IP review?

## Revision history

| Version | Date | Change |
|---|---|---|
| 1.0 | 2026-08-15 | Initial research program and experiment protocol. |
| 1.1 | 2026-08-15 | Added generalized predictive change impact, calibrated risk statements, capability gates, and EX-007. |
| 1.2 | 2026-08-15 | Consolidated the evidence-to-prediction loop, human decision record, causal/range validation policy, recursive stretch guardrails, and current program state. |
| 1.3 | 2026-08-15 | Added EX-001B cross-scenario determinism coverage and recorded its completed four-scenario evidence pending human decision. |
| 1.4 | 2026-08-15 | Recorded human acceptance of EX-001B as a bounded four-scenario reproducibility result and advanced EX-002 as the next causal experiment. |
| 1.5 | 2026-08-15 | Corrected EX-002 to use current schema-4 BaselineParity for herbivore attribution, defined the initial causal contract, and recorded the intervention/tooling blockers. |
| 1.6 | 2026-08-15 | Generalized collapse as a simulation-defined loss of practical growth capacity, added run-window and tracked-FSM telemetry seams, and kept desirability separate from collapse detection. |
| 1.7 | 2026-08-15 | Added schema-5 per-death telemetry with proximate cause, entity/resource identity, tick, age, and position; documented the remaining root-cause attribution gap. |
| 1.8 | 2026-08-15 | Integrated death telemetry into EX-002 and reclassified the schema-4 aggregate BaselineParity numbers as pre-telemetry evidence pending a same-seed schema-5 rerun. |
| 1.9 | 2026-08-18 | Added schema-6 reproduction-funnel outcomes while retaining schema-5 death telemetry; advanced the pending instrumented EX-002 baseline to a same-seed schema-6 rerun. |
| 1.10 | 2026-08-22 | Clarified that pre-specification is human-owned: AI may draft proposals, but a human decision owner must approve and record the experiment contract before execution. |
| 1.11 | 2026-09-03 | Added EX-009 to close the same-held-out-seed gap in the EX-007/EX-008 upgrade-order comparison; recorded the current Unity preflight blocker. |
| 1.12 | 2026-09-03 | Reordered the experiment portfolio by implementation dependencies and added the missing EX-008 entry. |
