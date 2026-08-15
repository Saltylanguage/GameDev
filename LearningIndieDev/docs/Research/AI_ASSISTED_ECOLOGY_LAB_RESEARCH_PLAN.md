# AI-Assisted Ecology Laboratory Research Plan

**Status:** Proposed research program  
**Version:** 1.1  
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

### 1. Experiment

The experiment describes the question before execution. It must state:

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
Feature type, model component, and supported range
Outcome type, metric, and time window
Effect size and outcome distribution
Held-out validation evidence
Direct dependency and affected-artifact set
Confidence and calibration status
Supported / Not currently testable / Underdetermined
Out-of-range warning
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
> T by effect E across N held-out runs. The result is valid only within range R,
> has confidence C, and crosses threshold Q under rubric version V. Any outcome
> outside R is out-of-distribution. If the requested feature is absent from the
> model, the result must instead say **Not currently testable** and identify the
> required model extension.

The final report must include the values, sample size, effect, uncertainty, valid
range, capability state, and evidence links. A severity number without its
rubric is not a research result.

### Confidence and severity rules

- Confidence is calculated from evidence quality, replication, held-out
  performance, and calibration history; it is not a free-form AI feeling.
- Severity is distinct from confidence. A low-confidence possibility may still
  deserve investigation, but it must not be presented as a high-confidence
  balance conclusion.
- The initial severity rubric should consider population damage, likelihood,
  persistence, player recovery options, impact on meaningful choices, and
  reversibility.
- Findings outside the observed parameter or scenario range must be marked
  out-of-distribution and should trigger a new experiment rather than silent
  extrapolation.

The following is a proposed starting rubric and remains subject to human design
approval and later calibration:

| Severity | Meaning |
|---:|---|
| 1/5 | Negligible effect; localized, reversible, and unlikely to alter meaningful play. |
| 2/5 | Minor measurable effect; viable strategies and recovery options remain clear. |
| 3/5 | Significant effect; a strategy, species, or pacing target is impaired but recoverable. |
| 4/5 | Major effect; a viable strategy is frequently invalidated or a cascade is difficult to recover from. |
| 5/5 | Critical systemic effect; the scenario or intended player agency is effectively destroyed. |

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
- Treat a result outside its tested range as a valid prediction without an
  out-of-distribution warning.
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

## Initial experiment portfolio

### EX-001 - Reproducibility baseline

**Question:** Does the current Forest Edge scenario reproduce exactly from the
same seed, scenario asset, and ruleset fingerprint?

**Method:** Run the existing baseline over a fixed seed range, repeat the same
range, compare fingerprints, final grids, population histories, and outcome
summaries, then replay at least one representative seed.

**Success:** Identical inputs produce identical machine-readable outcomes, and
any intentional nondeterminism is documented rather than hidden.

**Why first:** Every later AI claim depends on the simulation being a reliable
instrument.

The working package is documented in [EX-001 - Reproducibility Baseline](Experiments/EX-001-Reproducibility-Baseline/README.md).

### EX-002 - Herbivore collapse attribution

**Question:** Can the evidence spine explain why herbivores collapse in the
current reference scenario?

**Method:** Use the existing reports and telemetry to identify candidate causes
such as starvation, movement pressure, predation, reproduction limits, or
terrain/resource identity. Include spatial measures where movement or regional
confinement may be part of the failure. Select a small number of controlled
interventions and rerun the same seed range.

**Success:** A causal explanation is supported by changed evidence in a follow-up
experiment, not just by a plausible narrative.

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
seeds or a second scenario for validation. Include capability state,
out-of-range warnings, uncertainty, and a severity score only after applying an
approved human rubric.

**Success:** The assistant predicts the direction and approximate regime of the
effect on held-out runs, reports uncertainty and limits, rejects unsupported
feature requests as not currently testable, and keeps model evidence separate
from balance, quality, or engagement judgments. A human reviewer can trace the
statement to reports and decide whether a model extension, playtest, or
implementation should follow.

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
| P1 - Trust the instrument | 1-2 sprints / 10-20 hours | Reproducibility, fingerprints, replay, telemetry gaps | EX-001 passes or known limits are explicit |
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

Every research bundle should preserve:

- The original hypothesis and success criteria.
- Baseline and variant definitions.
- Seed range and ruleset fingerprints.
- Raw machine-readable outputs.
- Human-readable report.
- AI analysis version and provenance.
- Change-impact records with valid ranges, uncertainty, calibration status, and
  held-out validation evidence.
- Design thresholds and severity rubric version used for interpretation.
- Human decision and follow-up action.

## Risks and controls

| Risk | Control |
|---|---|
| AI invents a causal explanation | Require evidence links and controlled follow-up experiments |
| Fixed seeds hide real variation | Use explicit seed ranges and boundary/replay cases |
| Correlation is mistaken for an impact or cause | Use factorial sweeps, controlled interventions, and held-out validation |
| A regime change is hidden by averages | Record distributions, thresholds, and spatial metrics |
| The assistant extrapolates beyond known data | Require valid-range metadata and out-of-distribution warnings |
| Severity appears precise but has no shared meaning | Version a human-owned severity rubric and calibrate it against review |
| Balance or engagement is inferred from ecology alone | Separate ecological claims from design thresholds and playtests |
| Telemetry becomes noisy or expensive | Add fields only for a named question; measure report usefulness |
| AI converges on safe but uninteresting designs | Keep human design goals and novelty of playstyle in the decision gate |
| Research platform outruns the game | One bounded package per sprint; reuse `CellSim` first |
| Shared work is damaged by experimentation | Scenario boundaries, branches, ownership checks, and SG-002 alerts |
| A plausible combination is mistaken for novelty | Separate literature, prior-art, and legal review before claims |
| Players see diagnostics instead of a game | Translate accepted findings into readable feedback and choices |

## First execution plan

The first research package should be small enough to complete without blocking
the current production lane:

1. Create and review the [EX-001 package](Experiments/EX-001-Reproducibility-Baseline/README.md).
2. Run the existing Forest Edge baseline over a fixed seed range twice.
3. Compare ruleset fingerprints, final grids, histories, and outcome summaries.
4. Replay one representative and one boundary seed.
5. Generate the SG-001 report bundle and a separate AI analysis.
6. Record a human decision: pass, identify an instrument gap, or revise the
   experiment.
7. Create EX-002 only after EX-001's reproducibility gate is understood.

The current EX-001 package contains a matching historical report pair, but its
current-code rerun and replay gate remain open. Do not treat the historical pair
as permission to make interaction or balance claims until the current-code
decision is recorded.

This first package should avoid new simulation mechanics, generalized AI
frameworks, dashboards, or autonomous code changes. It is a trust-building
experiment for the evidence spine.

## Promotion rules

Research findings may be promoted to production only when:

- The result has a reproducible run or a documented reason it cannot be.
- The report and analysis are complete.
- Any change-impact claim has a stated valid range, uncertainty, capability
  state, and held-out validation evidence.
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
