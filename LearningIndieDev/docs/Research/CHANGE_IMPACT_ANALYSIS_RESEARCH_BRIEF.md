# Predictive Change Impact Analysis - Research Brief

**Status:** Exploratory research brief  
**Version:** 1.1  
**Created:** 2026-08-15  
**Relationship to the ecology lab plan:** Proposed adjacent capability; not yet an approved implementation project

The [AI-Assisted Ecology Laboratory Research Plan](AI_ASSISTED_ECOLOGY_LAB_RESEARCH_PLAN.md)
is canonical. This brief preserves the focused change-impact research question,
literature signals, and CIA-001 proposal; it does not define a separate
protocol.

The first source pass is logged in [Predictive Change Impact Analysis - Source Readings](CHANGE_IMPACT_ANALYSIS_SOURCE_READINGS.md), with each article kept separate from project-level deductions.

## Scope correction

This research is not limited to population cascades, regional confinement, or
ecological outcomes. Those are examples of possible outputs in the current
game. The target capability is generalized:

> Given a model-representable change, predict which observable outcomes will
> change, by how much, under which conditions, with what uncertainty, and what
> evidence supports the prediction.

The change may be a code rule, data field, species parameter, upgrade, event,
terrain setting, UI behavior, or another supported input. The outcome may be a
population, state transition, movement pattern, resource level, performance
measure, failure mode, visual result, test result, or any other instrumented
metric.

If a requested concept is not represented by the model—for example, animal
weight in a simulation with no weight field—the correct response is **Not
currently testable**. The assistant may identify the model extension required
to test it, but it must not invent a result for an unsupported variable.

## Terminology

The phrase **change impact analysis** has at least two relevant meanings:

1. **Software impact analysis:** estimating affected code, artifacts, tests, or
   dependencies after a software change.
2. **Simulation impact analysis:** estimating changed behavior or outcomes after
   an intervention to model inputs, rules, or configuration.

The proposed project capability combines both. It should connect direct
dependency evidence with paired seeded executions and outcome-level analysis.

## Initial literature signals

### Software change impact

Research on large systems uses dependency information to identify relevant
parts of a system and reduce blind regression testing. A case study describes
impact analysis for a system with millions of methods and dependencies, with
the goal of selecting a smaller relevant test subset.

Historical change analysis is a second signal: files that have changed together
in past revisions can help prioritize likely impact areas for a new change. The
usefulness of these historical relationships depends on the history window and
age of the data, so the evidence corpus itself needs a freshness policy.

### Simulation and sensitivity analysis

Sensitivity analysis provides a useful methodological bridge. Morris-style
screening estimates the influence of parameters with relatively few runs and
can reveal nonlinearity or interactions through variation in elementary
effects. Global variance-based methods such as Sobol analysis examine how input
variation contributes to output variation, including interaction effects.

These methods are not substitutes for causal validation. They help decide what
to investigate and where to spend simulation budget; controlled interventions
and held-out runs are still required for strong predictive claims.

## Proposed capability model

```text
Change request
    -> capability/testability gate
    -> direct dependency and affected-artifact analysis
    -> paired baseline/intervention runs
    -> sensitivity or interaction search when needed
    -> held-out validation
    -> change-impact report
    -> human interpretation and decision
```

### Capability states

| State | Meaning |
|---|---|
| Supported | The requested change and outcome are represented and instrumented well enough to test. |
| Not currently testable | The requested concept or outcome is absent from the model or telemetry. |
| Underdetermined | The concepts exist, but current evidence cannot distinguish plausible outcomes. |

### Change-impact record

Every prediction should preserve:

- Change-impact ID.
- Natural-language request and executable intervention.
- Baseline, comparison scope, and controlled variables.
- Feature type, model component, and tested range coverage.
- Outcome type, metric, and time window.
- Direct dependencies and affected artifacts.
- Effect size, distribution, and uncertainty.
- Seed/scenario/build coverage and held-out validation.
- Confidence and calibration status.
- Range status (range-invariant, regime-dependent, or unresolved), plus
  coverage/out-of-distribution or unsupported-feature warnings where applicable.
- Human thresholds and severity rubric, if applicable.
- Evidence IDs, replay/test candidates, and human decision.

## Example request classes

### Supported simultaneous intervention

> “What would happen if hare speed and fox vision increased at the same time?”

This requires a joint intervention, not two independent predictions added
together. The analysis should compare the paired variant against the same-seed
baseline and report interaction effects where the combined result differs from
the individual changes.

### Unsupported concept

> “What would happen if animal weight reduced movement and increased hunger?”

If weight, movement cost, or hunger coupling is not represented, the result is
**Not currently testable**. The assistant should identify the required model
fields, rules, telemetry, and validation experiment rather than fabricate an
ecological answer.

### General output statement

> Under baseline B, intervention I changes outcome O over time window T by
> effect E across N held-out runs. Across the requested feasible range the
> relationship is range-invariant, or the report identifies the regimes and
> thresholds where the effect changes. Confidence is C and it crosses threshold
> Q under rubric V; insufficient coverage is reported as unresolved.

## Research questions

- Can natural-language change requests be compiled into safe, explicit
  interventions without losing important assumptions?
- Can direct dependency analysis predict which artifacts, tests, and replays are
  likely to be affected?
- When do paired seeded runs provide enough evidence, and when is a sensitivity
  or interaction sweep required?
- Can the system distinguish unsupported concepts from merely under-instrumented
  concepts?
- Can impact predictions generalize to held-out seeds, scenarios, builds, and
  feature ranges?
- How should confidence be calibrated against observed prediction accuracy?
- How should human thresholds and severity judgments remain separate from model
  predictions?

## Proposed first study

**CIA-001: General predictive change impact**

Start with supported parameters in the existing simulation, but keep the study
contract domain-neutral. Use one single-variable change and one simultaneous
two-variable change. For each:

1. Define baseline, intervention, outcome metrics, and time window.
2. Run paired same-seed comparisons.
3. Record direct dependencies and affected artifacts.
4. Use a small sensitivity/interaction search only if the paired result is
   insufficient to explain the change.
5. Hold out seeds or a scenario for validation.
6. Produce a change-impact report with capability state, uncertainty, tested
   range coverage/status, causal status, and evidence links.
7. Record a human decision before changing production design or model code.

This study should begin only after EX-001's current-code reproducibility and
replay gate is understood. It should not assume that a successful ecological
example proves generality.

## Boundary conditions

- A prediction is not a causal explanation unless a controlled intervention
  supports the causal interpretation.
- A high-confidence ecological or technical prediction is not automatically a
  claim about fun, balance, safety, or commercial value.
- An unsupported variable is a model-design question, not an invitation to
  hallucinate a result.
- Range generalization is a validation gate. A high-accuracy result should
  replicate across the requested feasible range, aside from statistical
  outliers. Systematic thresholds are regime changes, not outliers; report
  them piecewise. Insufficient coverage is unresolved and requires a new
  experiment before a high-confidence claim.
- AI may propose and analyze; a human approves model extensions, thresholds,
  design decisions, and promotion.

## Initial source reading

- [Change Impact Analysis for Large-scale Enterprise Systems](https://www.scitepress.org/Papers/2012/41487/41487.pdf) - dependency-based impact analysis and targeted regression selection.
- [Empirical Software Change Impact Analysis using Singular Value Decomposition](https://www.cs.virginia.edu/~sherriff/papers/ICST_Sherriff.pdf) - historical co-change evidence for prioritizing potentially affected areas.
- [Designing oil palm architectural ideotypes through sensitivity analysis](https://academic.oup.com/aob/article/121/5/909/4774932) - Morris elementary-effects screening and interaction/nonlinearity signals.
- [Global sensitivity analysis of computer models with functional inputs](https://doi.org/10.1016/j.ress.2008.09.010) - variance-based global sensitivity and interaction analysis for nonlinear models.

## Revision history

| Version | Date | Change |
|---|---|---|
| 1.0 | 2026-08-15 | Initial scope correction and literature-oriented research brief. |
| 1.1 | 2026-08-15 | Aligned the supporting brief with the canonical plan's range-generalization and causal-status policy. |
