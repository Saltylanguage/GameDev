# Predictive AI: An Auditable Change-Impact Research Program

**Draft status:** Research paper draft v0.5<br>
**Date:** 2026-08-15  
**Project:** LearningIndieDev  
**Research area:** Predictive AI, deterministic simulation, evidence systems, and human-governed design decisions

## Abstract

Predictive AI is a proposed development and design system for answering a
practical question: when a model-representable feature, rule, asset value, or
system change is introduced, what observable outputs are likely to change, over
what time window, in which parameter regime, and with what uncertainty? The
project is not intended to produce an assistant that merely sounds confident or
to automate product decisions. It combines deterministic simulation, controlled
interventions, structured telemetry, replayable evidence, AI-assisted analysis,
and an explicit human decision gate.

The initial implementation is an ecological cellular-automata simulation, but
the research target is broader than one experiment or one biome. The same
contract is intended to cover population outcomes, movement, resources,
performance, failures, state transitions, visual results, upgrade interactions,
and other instrumented outputs. The project distinguishes evidence-backed model
claims from human judgments about balance, agency, quality, safety, or fun. It
also distinguishes supported questions from concepts that are not currently
testable and questions that remain underdetermined by available evidence.

The project now has a substantial evidence foundation: immutable run-start
scenario data, stable species and terrain identities, deterministic ruleset
fingerprints, seeded runs, population and activity telemetry, behavior-state
tracking, headless automated test tooling, standardized simulation report templates, comparison workflows,
and replay metadata. The first current-code EX-001 launch failed during Unity
startup; after the cache/permission issue was resolved, the schema-4 matrix ran
twice with matching payloads and selected replays. This gives the project a
current reproducibility result while retaining the old 32 x 20 provenance facts
only; the superseded source artifacts were removed and the live authored
ForestEdge configuration is now consistently 32 x 32.

## 1. The project in one sentence

Predictive AI is an auditable loop that uses AI to propose and interpret bounded
change-impact experiments while deterministic models, provenance, validation,
and human decisions constrain what may be claimed or promoted.

The intended loop is:

```text
Question or proposed change
    -> model/testability check
    -> human controlled intervention design
    -> deterministic runs and telemetry
    -> AI Generated data report and replay evidence
    -> AI-assisted analysis and prediction -> Human Guided
    -> held-out/range validation
    -> human decision -> prediction accepted/rejected -> Accepted increases predictive accuracy, Rejected Decreases it.
    -> accepted knowledge, revised model, or next experiment
```

The loop is deliberately evidence-first. AI can accelerate search, comparison,
hypothesis generation, and explanation, but it cannot turn an unsupported
concept into a measurable one or promote its own recommendation.

## 2. Motivation and thesis

Emergent systems create a recurring development problem. A small change can
produce delayed, nonlinear, spatial, or interacting effects that are difficult
to see in source code and difficult to explain from a single playtest. Final
counts alone are insufficient: they can show that something changed without
showing when, where, or why it changed. Human teams also need to distinguish a
simulation effect from a design judgment. A population increase may be real
without being desirable; a predicted risk may be worth investigating without
being certain.

The working thesis is:

> An auditable AI-assisted evidence loop can reduce the time from a design or
> engineering question to a useful, reviewable change-impact hypothesis, while
> improving calibration and preserving human authority over interpretation and
> promotion.

This is a research hypothesis, not a novelty claim. Deterministic simulation,
AI analysis, telemetry, replay, and change-impact testing all have established
precedents. The question is whether their disciplined combination produces
better decisions for a small game team.

## 3. Scope: broad target, narrow current model

The long-term target is generalized change-impact prediction. A request may
ask, for example:

- What changes if hare movement speed and fox vision are raised together?
- Which populations, resources, or state transitions change when a terrain
  value is reduced?
- Which tests, replays, metrics, or visual checkpoints are likely to change if
  an upgrade or ruleset field is modified?
- Does a proposed change create a threshold, cascade, spatial effect, or
  out-of-known-range risk?

The current model is narrower: a deterministic cellular-automata ecology with
species, terrain, movement, feeding, reproduction, mortality, behavior states,
and authored Forest Edge scenarios. The project must not imply that the current
implementation already predicts arbitrary game or real-world outcomes. A
feature or outcome becomes part of the predictive system only when it is
represented, instrumented, and covered by an experiment design.

## 4. System architecture

Predictive AI is organized into four separable layers.

### 4.1 Model and execution layer

The Unity simulation supplies the model under test. Scenario assets are converted
to immutable runtime snapshots. Seeded initialization and stepping preserve
repeatability. Stable `SpeciesId` and `TerrainId` values avoid accidental
dependence on dictionary ordering. The ruleset produces a versioned SHA-256
fingerprint so a result can be tied to the exact configuration that generated it.

This layer is intentionally UI-agnostic. Runtime simulation, editor tooling,
and Noesis presentation remain separate so that evidence runs do not depend on
player-facing controls.

### 4.2 Evidence layer

The evidence layer records machine-readable run payloads, population histories,
final summaries, activity, mortality causes, behavior-state ticks, entity
transitions, provenance, and replay metadata. Automated testing exposes project-local
commands for tests, runs, reports, comparisons, baselines, and visual replay.
Generated JSON, CSV, logs, and visual evidence remain artifacts rather than
being silently folded into source documentation.

### 4.3 Predictive analysis layer

AI analysis consumes factual reports and proposes interpretations, hypotheses,
comparisons, and next experiments. It must label observed facts, inferences,
hypotheses, recommendations, and unresolved uncertainty separately. A future
prediction registry may index accepted findings, validated ranges, effect sizes,
and calibration history, but the registry is derived from source evidence and
does not replace it.

### 4.4 Governance and translation layer

Human review determines whether an evidence-backed finding is accepted,
rejected, revised, archived, or promoted to another experiment or player-facing
design. A separate interpretation layer may apply a human-owned balance,
agency, quality, safety, or engagement rubric. The simulation cannot establish
that a result is fun, fair, commercially valuable, or appropriate for players by
itself.

## 5. Evidence protocol

Every meaningful activity follows five distinct stages:

```text
Experiment -> Run -> Report -> Analysis -> Human Decision
```

The experiment defines the question, hypothesis, independent variable,
controls, seed/sample plan, success criteria, report template, and decision
owner before execution where practical. In this protocol, that
pre-specification is human-owned: AI may draft or propose the experiment, but
the named human decision owner must approve and record the contract before
execution. A run is immutable provenance for one configuration and seed. A
report records observed facts and anomalies. Analysis interprets those facts
without overwriting them. A human decision determines what the evidence
authorizes.

This separation supports both scientific discipline and practical debugging.
Failed automation, partial runs, crashes, and missing outputs are evidence about
the instrument and must remain visible. A successful process exit is not enough;
the resulting artifact must be inspectable and complete.

## 6. Predictive change-impact contract

Every proposed prediction must record:

- change request, baseline, intervention, and comparison scope;
- feature type and model component affected;
- output metric and time window;
- effect size, distribution, uncertainty, and sample size;
- tested parameter regime and range coverage;
- held-out seeds, scenarios, or builds;
- direct dependencies and affected artifact types;
- confidence, calibration status, and coverage warnings;
- evidence IDs, replay candidates, and test references;
- human threshold, severity rubric, and decision.

Before analysis, the request receives one of three capability states:

| State | Meaning |
|---|---|
| **Supported** | The feature, intervention, output, and telemetry are represented well enough to test. |
| **Not currently testable** | The requested concept or output is absent from the model or instrumentation and requires a new model-extension experiment. |
| **Underdetermined** | The concepts exist, but current evidence, replay, or coverage cannot distinguish plausible outcomes. |

Range status is reported separately as **Range-invariant**, **Regime-dependent**,
or **Unresolved**. A threshold or phase transition is not discarded as noise;
it is reported as a regime boundary with uncertainty. Unsupported extrapolation
is not a prediction.

The target output is conditional rather than absolute:

> Under baseline B, intervention I changes outcome O over time window T by effect
> E across N held-out runs. The relationship is range-invariant or regime-
> dependent over the tested range, with confidence C and explicit coverage
> limits. Any design threshold is a human-owned interpretation under rubric V.

## 7. Causal language and calibration

Predictive AI uses progressively stronger causal labels:

1. **Observed association:** variables move together in recorded reports.
2. **Mechanistically consistent:** telemetry and timing fit a proposed mechanism,
   but alternatives remain.
3. **Causal evidence supported within model scope:** a human-approved
   intervention recorded before execution is tested against a same-seed
   baseline, replicated, and shown to produce a measurable effect.
4. **Robust causal relationship within validated range:** the effect survives
   held-out seeds or scenarios and range coverage without a credible competing
   explanation in the instrumented model.
5. **Unresolved:** evidence or coverage cannot distinguish the alternatives.

Confidence is not a writing style. It should reflect evidence quality,
replication, held-out performance, and calibration history. Severity is separate
from confidence: a low-confidence possibility may still merit urgent follow-up.
The initial severity rubric considers population damage, likelihood, persistence,
player recovery, meaningful-choice impact, and reversibility. The rubric itself
requires later human review and calibration.

## 8. What is implemented so far

The current foundation includes:

- authored `CellularSimData` and scenario definitions;
- immutable run-start snapshots;
- stable species, terrain, entity, and behavior identities;
- deterministic seeded initialization and stepping;
- versioned ruleset fingerprints;
- movement, terrain-cost, resource, forage, reproduction, alpha-offspring, and
  mortality telemetry, including schema-5 per-death cause events;
- population histories and per-species activity summaries;
- tracked finite-state behavior transitions and death-path logging;
- headless `CellSim` commands for tests and seeded experiment runs;
- Markdown and JSON/CSV report generation;
- controlled report comparison and replay manifest generation;
- Noesis presentation and a custom board renderer that remain separate from the
  simulation domain;
- studio report templates and human-decision guidance;
- a research plan and change-impact brief defining the wider Predictive AI
  program.

These are implementation capabilities, not proof that the full research thesis
has succeeded. Current-code reproducibility is accepted for the authored
ForestEdge baseline and the bounded EX-001B cross-scenario check; the project
still needs causal intervention studies, held-out validation, calibration
measurements, and workflow-value evaluation.

## 9. What has been learned so far

### Determinism is an advantage, not a conclusion

The historical EX-001 pair matched across all 20 seeds after normalization,
including run payloads and final summaries. This demonstrates that the report
shape can capture strong repeatability evidence. It does not prove that the
current checkout, another machine, or another scenario will reproduce.

### Full histories are more valuable than final counts

Population trajectories, activity, mortality, and behavior-state telemetry
provide diagnostic context that final populations cannot. The new per-death
events preserve proximate cause, species/resource identity, entity ID when
available, tick, age, and position. They still do not identify a unique root
cause; causal claims require interventions designed to separate plausible
mechanisms and may need preceding resource-state or attacker-link telemetry.

### Provenance must survive tool and schema changes

Raw report hashes include generated timestamps and paths, so normalized outcome
hashes are more useful for equality checks. Schema, fingerprint, scenario,
build, branch, and tool versions must remain visible so stale findings are not
mistaken for current evidence.

### Replay is its own validation problem

A report can contain enough metadata to request replay without proving that the
replay matches state, timing, or presentation. Representative and boundary
seeds need explicit replay manifests and human inspection.

### Instrument failures are research results

The first current-code EX-001 attempt failed during native batch startup. After
the Unity cache/permission issue was resolved, the exact schema-4 matrix ran
twice and both selected replays passed. The package preserves the failed attempt
as evidence and records the successful paired run separately, preventing either
failure or historical success from being silently hidden.

## 10. EX-001 as a case study

EX-001 is the first instrument-trust study, not the definition of Predictive AI.
It asks whether Forest Edge reproduces from a fixed scenario, ruleset, seed
range, and run configuration. The historical reports cover seeds `10100`–`10119`
and show final hare counts from 248 to 345 and fox extinction in 3 of 20 runs.
Those are descriptive observations and useful replay candidates, not balance or
causal conclusions.

The current-code gate now passes for the authored 32 x 32 configuration. The
superseded schema-2 brief and report recorded 32 x 20; their provenance facts
were transferred to the EX-001 record and source artifacts were removed. The
case study thus demonstrates both sides of the program: simulation evidence can
be strong when artifacts match, and governance must stop a prediction until the
current instrument has run.

## 11. EX-001B as a generalization check

EX-001B extends the reproducibility question across the currently authored
scenario library without changing the simulation mechanics. ForestEdge (32 x
32), OpenRange (32 x 20), and Wetland (32 x 20) each reproduced their complete
20-seed matrix on a second run, with matching ruleset fingerprints, normalized
run-payload hashes, and final-summary hashes. The pairs used different species
rosters and scenario fingerprints, so this is stronger evidence than repeating
ForestEdge alone.

The extension now covers all four authored scenario assets. The current evidence
supports reproducibility across those four tested scenarios, not all scenarios or
all cellular automata. It also demonstrates an operational lesson: a rejected
player-species input and a nonzero Unity wrapper status must remain visible as
setup/environment anomalies rather than being mistaken for simulation
divergence.

The human design owner accepted EX-001B on 2026-08-15 as a bounded
reproducibility result. This acceptance authorizes the next causal experiment,
EX-002, but does not authorize universal cellular-automata claims, ecological
finding transfer, or claims that the simulation is correct or balanced.

## 12. Evaluation program

The wider research program must answer ten questions:

1. Can identical inputs reproduce across runs and machines?
2. Can reports and replay distinguish causal sequences from correlations?
3. Can AI propose bounded, measurable hypotheses?
4. How often are AI explanations supported by follow-up experiments?
5. Does the workflow reduce time-to-evidence and time-to-decision?
6. Can humans keep evidence, analysis, and decisions separate?
7. Can internal findings become clear player-facing explanations?
8. Does the workflow generalize across species, biomes, upgrades, and modes?
9. Can accumulated evidence support held-out change-impact predictions?
10. Can confidence and design-risk statements be calibrated against later runs
    and playtests?

The staged evaluation path is:

1. use the accepted EX-001 baseline as the instrument-trust reference;
2. run paired single-variable interventions;
3. add factorial or joint designs for interactions;
4. validate predictions on held-out seeds, scenarios, and parameter ranges;
5. measure analyst time, false positives, false negatives, and review burden;
6. test translation into player-facing explanations;
7. promote only the tooling and findings that earn human approval.

## 13. Risks, limits, and guardrails

The main risks are false confidence, stale evidence, hidden instrumentation
gaps, confounding in multi-variable changes, overfitting to Forest Edge, and
confusing ecological output with product judgment. A prediction may also create
review burden without improving decisions.

The guardrails are therefore substantive:

- no claim without a traceable report and provenance;
- no causal language from correlation alone;
- no silent extrapolation outside validated ranges;
- explicit `Not currently testable` and `Underdetermined` states;
- separate confidence and severity;
- human-owned thresholds and promotion decisions;
- immutable source artifacts and derived analyses;
- isolated branches and fixed budgets for any future recursive automation;
- no autonomous production or player-facing changes.

The project also avoids claiming technical, research, or intellectual-property
novelty without a separate literature, prior-art, and legal review.

## 14. Roadmap and success criteria

The near-term success criterion is not a polished prediction dashboard. It is a
working, reviewable loop in which a human can identify the question, reproduce
the relevant run, inspect the evidence, understand the AI analysis, and record
what the result authorizes.

The next milestones are:

- preserve the known-good elevated Unity batch path and use the accepted EX-001 baseline;
- create the first same-seed intervention experiment;
- formalize a normalized evidence index without replacing source reports;
- add replay/state comparison for selected seeds;
- define held-out and range-coverage budgets;
- measure prediction calibration and analyst workflow value;
- proceed to EX-002 and test additional cross-scenario or cross-feature
  generalization after the causal baseline is documented;
- only then consider a prediction registry or safe recursive experiment lane.

## 15. Conclusion

Predictive AI is best understood as a governed research and development
workflow, not a single model. Its value would come from connecting bounded
interventions, deterministic execution, rich evidence, AI-assisted reasoning,
and human decisions in one traceable loop. The implementation already provides a
credible foundation for that investigation. The evidence so far supports
repeatability of a historical fixture and demonstrates the importance of
instrument limitations; it does not yet support generalized predictive claims.

The project should proceed by earning broader claims one validated range,
intervention, and human decision at a time.

## References and project records

- [AI-Assisted Ecology Laboratory Research Plan](AI_ASSISTED_ECOLOGY_LAB_RESEARCH_PLAN.md)
- [Predictive Change Impact Analysis Research Brief](CHANGE_IMPACT_ANALYSIS_RESEARCH_BRIEF.md)
- [Change Impact Analysis Source Readings](CHANGE_IMPACT_ANALYSIS_SOURCE_READINGS.md)
- [EX-001 experiment brief](Experiments/EX-001-Reproducibility-Baseline/EXP-001-brief.md)
- [EX-001 current-code execution attempt](Experiments/EX-001-Reproducibility-Baseline/RPT-RUN-001-0003-0020.md)
- [EX-001 current-code paired evidence](Experiments/EX-001-Reproducibility-Baseline/RPT-RUN-001-0004-0020.md)
- [Unity simulation tooling](../UNITY_SIMULATION_TOOLING.md)
- [Studio guideline SG-001 - AI Generated Reports](../Studio%20Guidelines/AI_GENERATED_REPORTS.md)
- [Studio guideline SG-002 - AI-Assisted Development](../Studio%20Guidelines/AI_ASSISTED_DEVELOPMENT.md)
