# Predictive Change Impact Analysis - Source Readings

**Reading status:** Initial source pass  
**Read:** 2026-08-15  
**Parent brief:** [Predictive Change Impact Analysis Research Brief](CHANGE_IMPACT_ANALYSIS_RESEARCH_BRIEF.md)  
**Purpose:** Preserve article-specific evidence, deductions, limitations, and project relevance without blending conclusions from different methods.

## How to read this document

Each article has its own section. Statements are separated into:

- **What the source contributes:** What the authors actually studied or propose.
- **Meaningful deductions:** Conclusions that follow for our research program.
- **Project inferences:** Our proposed application, not a result established by
  the source.
- **Cautions:** Limits, transfer risks, or assumptions that must remain visible.

The source set does not prove that our proposed AI workflow is novel or that it
will work. It gives us methods, evaluation patterns, and failure modes to test.

## Cross-source synthesis

The readings cover four different impact questions:

| Question | Best-supported method family | What it can answer |
|---|---|---|
| Where might a change propagate in the project? | Static dependency and artifact analysis | Which code, data, tests, assets, and documents may be affected? |
| What has tended to change together historically? | Evolutionary coupling and change-history mining | Which artifacts are empirical co-change neighbors? |
| What actually changes during representative executions? | Dynamic analysis and execution differencing | Which observed states, paths, or outputs changed under tested conditions? |
| How do inputs influence outputs across a range? | Sensitivity analysis and metamodels | Which variables matter, where interactions occur, and where nonlinear behavior may appear? |

The important overall deduction is that our proposed system should not call one
of these techniques “the impact.” It should produce a layered impact package:

```text
Dependency candidates
    -> historical priors
    -> paired baseline/intervention execution
    -> sensitivity or interaction search
    -> held-out validation and accuracy scoring
    -> human decision
```

The papers repeatedly reinforce three requirements for our work:

1. Predictions need a measurable ground truth or validation target.
2. Coverage and freshness matter; a prediction model can be incomplete or stale.
3. Different evidence types answer different questions and should remain
   separately labeled.

---

## 1. Chen et al. - Change Impact Analysis for Large-scale Enterprise Systems

**Source:** [SciTePress article page](https://www.scitepress.org/PublishedPapers/2012/41487/)  
**Paper:** Wen Chen, Asif Iqbal, Akbar Abdrakhmanov, Jay Parlar, Chris George, Mark Lawford, Tom Maibaum, and Alan Wassyng (2012).  
**Primary topic:** Dependency-based impact analysis to reduce regression-testing scope in very large systems.

### What the source contributes

The paper targets systems with hundreds of thousands of classes and millions of
methods. Its motivation is practical: retesting everything after every change
is expensive, but selective retesting is dangerous when the impact is not
understood. The approach uses dependency information to identify a smaller,
relevant subset of tests, and reports a case study on a system with 4.6 million
methods and 10 million dependencies.

### Meaningful deductions

- Change impact analysis is partly a **test-selection problem**. It is not only
  a prediction of behavior; it also determines what evidence must be collected
  next.
- “Affected artifacts” and “affected outcomes” should be separate fields. A
  dependency graph can identify what might be touched without proving that a
  player-visible or simulation outcome changes.
- At scale, the output should prioritize a bounded evidence set rather than
  demand exhaustive inspection of everything.

### Project inferences

For our workflow, a change-impact report should identify likely affected:

- Unity scripts and serialized data.
- `CellSim` commands, tests, reports, and replay manifests.
- Scenario assets, upgrades, event definitions, and UI explanations.
- Metrics and report fields that should be compared after the change.

This suggests a useful first layer before AI analysis: a direct dependency and
affected-artifact set that tells us which runs and tests are relevant.

### Cautions

- Targeted regression selection is not proof that an unselected area is safe.
- Dependency analysis is strongest for structural impact, not emergent behavior.
- The paper’s scale and enterprise context do not transfer directly to a small
  Unity project; the useful idea is the separation of impact candidates from
  expensive validation, not the exact implementation.

### Relevance rating

**High for:** affected-test selection, artifact scope, evidence budgeting.  
**Low for:** predicting emergent ecological or player-facing outcomes directly.

---

## 2. Sherriff and Williams - Empirical Software Change Impact Analysis using Singular Value Decomposition

**Source:** [Full paper PDF](https://www.cs.virginia.edu/~sherriff/papers/ICST_Sherriff.pdf)  
**Paper:** Mark Sherriff and Laurie Williams.  
**Primary topic:** Mining historical change records to find artifacts that tend to change together.

### What the source contributes

The authors build a matrix of how often files changed together, apply singular
value decomposition, and use the resulting association clusters to identify
potentially affected files for a new change. The study evaluates five open
source Java projects and compares the approach with PathImpact and
CoverageImpact. The method can include non-source files such as images,
documentation, and configuration files because it operates on change records,
not only executable code.

The paper also makes its limits unusually clear. Opportunistic changes—files
checked in together for convenience but not because they are causally related—
can inflate associations. If no historical data exists for a change, the method
cannot provide safe guidance.

### Meaningful deductions

- Historical co-change is a useful **prior**, not a causal graph.
- Non-code artifacts matter in real impact analysis. This is directly relevant
  to Unity projects where assets, prefabs, scenario data, reports, and code can
  be coupled.
- Unknown-history cases should be represented as “no evidence,” not as “no
  impact.”
- Historical models need a way to distinguish genuine repeated coupling from
  convenience bundling.

### Project inferences

We could eventually build an empirical coupling layer from our own records:

- Which scenario assets tend to change with which systems.
- Which report fields change when a simulation rule changes.
- Which tests, replays, and visual captures have historically exposed a change.
- Which design documents and production tasks tend to move with a feature.

This layer would help the assistant suggest where to look and what to rerun. It
should never be the sole basis for a causal claim about simulation behavior.

### Cautions

- Our current history is small and may contain many opportunistic changes.
- A co-change relationship can be a workflow artifact rather than a system
  dependency.
- A new feature or unrepresented variable has no useful historical prior. The
  correct state is **Not currently testable** or **No historical evidence**, not
  a confident prediction.
- The paper’s file-level approach is not automatically the right granularity
  for our model features or telemetry fields.

### Relevance rating

**High for:** project-wide artifact scope, Unity asset coupling, history-aware
priors.  
**Low for:** proving dynamic or ecological causality.

---

## 3. Cai and Santelices - A Comprehensive Study of the Predictive Accuracy of Dynamic Change-Impact Analysis

**Source:** [Full paper PDF](https://chapering.github.io/pubs/jss15.pdf)  
**Paper:** Haipeng Cai and Raul Santelices (2015).  
**Primary topic:** Measuring whether dynamic change-impact predictions are actually accurate.

### What the source contributes

The paper evaluates predictive dynamic impact analysis using both large numbers
of injected changes and more than 100 repository changes. It uses sensitivity
analysis and execution differencing to compare predicted impact sets against
the actual effects observed after changes are applied. The evaluated technique
showed average precision around 38–50% and recall around 50–56% in most cases.

The paper’s strongest contribution for us is not the specific percentages; it is
the evaluation design. It treats the modified program’s observed behavior as a
ground-truth target and measures false positives and false negatives. It also
shows that short or limited executions can miss impacts and that prediction
quality is not something to assume from a plausible method.

### Meaningful deductions

- A predictive impact system needs a **prediction accuracy study** as part of
  its design, not only a demo that produces plausible explanations.
- Precision and recall are both important. Overpredicting every possible effect
  creates review noise; missing real effects creates unsafe confidence.
- Representative execution coverage is a first-class limitation. An impact
  prediction made from a narrow run profile may be wrong for another profile.
- Ground truth must be defined relative to a specific observation protocol,
  not treated as an abstract universal set of effects.

### Project inferences

For our deterministic simulation, we can define a practical ground-truth loop:

1. Record a baseline report for a fixed scenario, seed set, and time window.
2. Apply one approved change and run the same seeds/configuration.
3. Compute actual changed metrics, state transitions, report fields, and replay
   observations.
4. Compare those actual changes with the assistant’s predicted impact set.
5. Track false positives, false negatives, effect-direction accuracy, and
   threshold-crossing accuracy.

This is a strong reason to preserve paired baseline/intervention reports instead
of keeping only a final AI summary.

### Cautions

- The source measures code-level impact sets, not game-design outcomes. We must
  define tolerances and “actual impact” for each output type.
- A simulation can have several valid ground truths at different resolutions:
  code path, state, metric, spatial pattern, and player-facing result.
- The source warns against treating dynamic analysis as automatically accurate;
  our deterministic runs make validation easier, but do not remove the need for
  validation.

### Relevance rating

**Very high for:** prediction scoring, paired runs, ground-truth design,
precision/recall, coverage warnings, and calibrated confidence.

---

## 4. Moonen et al. - Exploring the Effects of History Length and Age on Mining Software Change Impact

**Source:** [Full paper PDF](https://www.cs.loyola.edu/~binkley/papers/scam16-history-exploration.pdf)  
**Paper:** Leon Moonen, Stefano Di Alesio, Thomas Rolfsnes, and Dave W. Binkley (2016).  
**Primary topic:** How the amount and freshness of change history affect mined impact predictions.

### What the source contributes

The study evaluates evolutionary-coupling impact analysis over two industrial
systems and 17 open-source systems. It varies history length and history age,
then measures how those choices affect prediction quality and applicability.
The reported results show that longer history generally improves quality with
diminishing returns, while even modest aging can significantly degrade quality.
The authors derive project-specific guidance for history length and model
rebuilding.

### Meaningful deductions

- An evidence-driven impact model has a **freshness problem**. A prediction can
  be methodologically sound but stale relative to the system’s current state.
- “More data” is not automatically “better data.” The model needs a policy for
  history window, aging, and rebuild triggers.
- Evaluation splits must account for time. Training on old data and testing on
  later changes is not equivalent to continuously updating the model.

### Project inferences

Our interaction and change-impact registry should record:

- The last validated build/ruleset and report schema.
- The date and evidence range used to support a prediction.
- Which scenarios, seeds, and feature ranges are represented.
- Whether a later code/data change invalidates or weakens the prior.
- When the registry should be recalculated or marked stale.

This applies to simulation behavior as well as software artifacts. A prediction
about a rule from an old ruleset fingerprint should not silently carry forward
to a new ruleset.

### Cautions

- The reported history-length values are specific to the studied systems and
  must not be copied into our project as universal requirements.
- Simulation evidence may age because rulesets and telemetry schemas change,
  even when source history remains available.
- A “current” model can still be wrong if the new feature is outside the
  represented regime.

### Relevance rating

**High for:** freshness metadata, registry invalidation, temporal evaluation,
and model rebuild policy.  
**Medium for:** choosing a data-window size for simulation evidence.

---

## 5. Perez et al. - Designing Oil Palm Architectural Ideotypes through a Sensitivity Analysis

**Source:** [Full article](https://academic.oup.com/aob/article/121/5/909/4774932)  
**Paper:** Raphaël P. A. Perez, Jean Dauzat, Benoît Pallas, Julien Lamour, Philippe Verley, Jean-Pierre Caliman, Evelyne Costes, and Robert Faivre (2018).  
**Primary topic:** Efficiently exploring many model parameters and identifying influential variables and interactions.

### What the source contributes

The study uses a deterministic model and a Morris one-at-a-time screening design.
Parameters are sampled over defined ranges, and random trajectories vary one
parameter at a time. The authors use multiple outputs and developmental stages,
then build a quadratic metamodel over 8,192 parameter combinations. The
metamodel includes linear, squared, and pairwise interaction terms.

The reported results show that parameter importance can vary with developmental
stage and output. The study also identifies parameters with high interaction
signals and uses the explored space to identify high-performing combinations.

### Meaningful deductions

- Screening and prediction should be separate stages. A cheap screening pass
  can identify influential variables; a richer joint design can then test
  interactions and nonlinearities.
- Impact is often output- and time-window-specific. A parameter can matter for
  one metric or stage and matter less for another.
- A metamodel can make broad what-if exploration cheaper, but it is only as good
  as the sampled range, output definition, and validation design.
- Determinism is valuable because it lets the experiment spend runs on coverage
  and replication rather than noise estimation.

### Project inferences

For our system, a generalized change-impact query could select a method based on
the question:

- Morris-style screening for many candidate fields.
- Paired seeded runs for a concrete change.
- Factorial or response-surface sampling for two or more interacting changes.
- A metamodel only after the domain and output range are well specified.

The report must preserve the time window and output definition. “Impact on the
simulation” is too vague; impact might mean final population, last-five-second
starvation, movement entropy, test failures, or another explicitly named
observable.

### Cautions

- The paper’s plant model and parameter ranges do not establish that the same
  sampling design is optimal for our simulation.
- One-at-a-time screening can miss interaction behavior if used as the final
  answer.
- A metamodel can produce smooth predictions across a range while hiding a
  discrete state transition or extinction boundary.
- High sensitivity is not automatically high design importance.

### Relevance rating

**Very high for:** experiment-budget allocation, multi-output analysis,
time-windowed impact, parameter interactions, and staged exploration.

---

## 6. Iooss and Ribatet - Global Sensitivity Analysis of Computer Models with Functional Inputs

**Source:** [Journal article and DOI](https://doi.org/10.1016/j.ress.2008.09.010)  
**Paper:** Bertrand Iooss and Mathieu Ribatet (2009).  
**Primary topic:** Global sensitivity analysis when inputs are functional, temporal, spatial, or otherwise too rich to treat as one scalar.

### What the source contributes

The paper addresses computer models with scalar inputs plus functional inputs
such as stochastic processes or spatial random fields. It discusses variance-
based global sensitivity indices and metamodeling approaches that model both
the mean and dispersion of outputs. The central problem is that simply treating
every discretized time or spatial value as a separate scalar quickly becomes
intractable and difficult to interpret.

### Meaningful deductions

- Some changes are not single numbers. Weather sequences, terrain fields,
  event schedules, and movement trajectories may need structured representations
  rather than hundreds of independent fields.
- Impact analysis should distinguish changes to a scalar parameter from changes
  to a temporal or spatial function.
- Modeling output dispersion matters. A change may leave the average outcome
  stable while increasing variance or the probability of catastrophic outcomes.
- Metamodeling is useful when direct simulation is expensive, but the reduction
  from rich inputs to a surrogate must remain interpretable and validated.

### Project inferences

This is relevant to future biome and event systems:

- A storm or weather profile is an input function, not merely “rainfall +10%.”
- Terrain layout and resource distribution are spatial inputs.
- A run’s event schedule is a temporal input.
- Outcome reports should preserve both central tendency and tail risk.

For a generalized change-impact statement, the input contract should identify
whether the intervention is scalar, categorical, temporal, spatial, or a model
structure change.

### Cautions

- Functional-input methods are more complex than our current needs and should
  not be implemented before simpler paired and screening methods are validated.
- A surrogate can hide important regime boundaries if the input representation
  is too compressed.
- Global sensitivity indices depend on the input distributions and ranges that
  are chosen; they are not universal importance scores.

### Relevance rating

**High for:** future terrain, weather, schedules, spatial layouts, variance and
tail-risk analysis.  
**Low for:** immediate EX-001 execution.

---

## Consolidated deductions for our research program

These are project-level conclusions synthesized across the readings, not claims
made by any single article:

1. **Use a layered impact package.** Structural dependencies, historical
   couplings, observed runtime deltas, sensitivity results, and human design
   interpretation should be separate evidence layers.
2. **Define ground truth before measuring prediction.** For each output type,
   specify what counts as an impact, the tolerance, the time window, and the
   comparison scope.
3. **Make capability explicit.** Unsupported variables and uninstrumented
   outputs must return “Not currently testable” or “Underdetermined.”
4. **Use staged experiment budgets.** Screen broadly, test concrete changes with
   paired runs, then use joint designs or metamodels only when interaction
   questions justify them.
5. **Track freshness and validity range.** A result is tied to a build,
   ruleset, telemetry schema, scenario, input range, and evidence date.
6. **Score false positives and false negatives.** A verbose impact report can be
   as harmful as a missing warning if it overwhelms review or creates false
   confidence.
7. **Keep model prediction separate from design judgment.** A measured effect
   can be reliable while the conclusion about fun, balance, quality, or agency
   remains unresolved.
8. **Treat deterministic execution as an advantage, not proof.** Determinism
   makes paired comparisons and replay easier, but coverage and validation are
   still required.

## Proposed follow-up reading

- A foundational treatment of formal software change-impact analysis and
  change-effects models.
- Dynamic and static impact-analysis combinations for heterogeneous systems.
- Design-of-experiments methods for discrete, categorical, and mixed inputs.
- Calibration and selective prediction for uncertainty-aware AI outputs.
- Causal inference methods for intervention effects when outcomes are temporal
  and stateful.

## Revision history

| Version | Date | Change |
|---|---|---|
| 1.0 | 2026-08-15 | Initial article-by-article source reading and project deductions. |
