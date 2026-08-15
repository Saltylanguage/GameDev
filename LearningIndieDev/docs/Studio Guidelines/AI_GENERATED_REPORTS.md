# AI Generated Reports

**Guideline ID:** SG-001  
**Status:** Active  
**Version:** 1.0  
**Adopted:** 2026-08-15  
**Scope:** AI-assisted workflows, automated tools, simulations, experiments, diagnostics, content generation, and feature-validation workflows.

## Purpose

Every automated workflow should produce an inspectable, diagnosable result. A successful output is not enough: a human designer or developer must be able to understand what was attempted, what happened, why the result may have happened, and what should happen next.

Reports are evidence for human decision-making. They are not a replacement for human judgment.

## Core protocol

All meaningful automated work follows this chain:

```text
Experiment → Run → Report → Analysis → Human Decision
```

These stages must remain distinct.

### 1. Experiment

The experiment defines the question being investigated before execution begins.

It should record:

- Question or problem.
- Hypothesis.
- Success criteria.
- Failure criteria.
- Scope and feature under investigation.
- Expected inputs and relevant constraints.

An experiment may produce many runs. The experiment is the durable identity of the investigation; it is not a single result.

### 2. Run

A run is one execution of an experiment under a specific configuration.

Each run must identify, where applicable:

- Run ID and parent Experiment ID.
- Date and time.
- Git commit, branch, and build/version.
- Random seed.
- Input data and configuration.
- Tool, model, prompt, script, and dependency versions.
- Environment information needed for reproduction.
- Whether the run completed, was stopped, or crashed.

Runs are immutable evidence. A changed configuration or source revision creates a new run; it does not rewrite an old one.

### 3. Report

The report records what objectively happened during a run. It should not silently mix observations with interpretation.

The standard request contract is:

```text
Report: Simulation-MySimulation
Feature: My Feature
Task: Testing My Feature for bugs
Focus: crashes, null references, code clarity, performance
Location: reports/MyFeature/
```

Reports should be low-noise and focused on the requested feature or task. Raw data belongs in machine-readable artifacts; the human-readable report should emphasize outcomes, changes, anomalies, and evidence.

#### Runtime observability parity

When a report or output log gains a meaningful runtime event, the runtime debug
log should expose that same event at the time it occurs. Console output may use
a more compact presentation, but it must preserve the identifiers and values
needed to correlate it with the report. Omit console output only when the event
would be noisy without adding diagnostic value; document that exception in the
relevant tool or report.

### 4. Analysis

AI and human interpretation must be stored separately from the factual report. Analysis may identify likely causes, patterns, tradeoffs, or recommended experiments, but it must cite the evidence supporting those conclusions.

AI analysis should include:

- Executive summary.
- Findings linked to report evidence.
- Hypotheses and confidence levels.
- Uncertainty and missing data.
- Suggested actions ranked by priority.
- Expected impact and tradeoffs.
- Suggested owner or follow-up task.
- AI model, prompt/template version, and analysis timestamp.

The original report must never be rewritten when analysis changes. New analysis creates a new version, such as `ai-analysis-v2.md`.

### 5. Human Decision

The team decides what to do with the evidence and analysis. The decision must be recorded separately from the AI recommendation.

Useful decision states include:

```text
Generated
Needs Review
Accepted
Rejected
Inconclusive
Superseded
Archived
```

A suggestion is not a project decision until a human accepts it. Accepted decisions should link back to the source report and analysis.

## Report bundle

A report should normally be stored as a small bundle:

```text
reports/
  <feature>/
    <report-id>/
      report.md
      metadata.json
      metrics.json
      events.jsonl
      ai-analysis-v1.md
      artifacts/
```

Not every report needs every file. The template determines which files are required.

- `report.md`: concise, human-readable factual summary.
- `metadata.json`: identity, provenance, scope, status, and template information.
- `metrics.json`: normalized measurements and values.
- `events.jsonl`: ordered event or timeline records when applicable.
- `ai-analysis-vN.md`: derived interpretation and suggested actions.
- `artifacts/`: screenshots, traces, logs, replays, builds, or other supporting evidence.

## Report templates and defaults

Reusable templates define what to collect, how to present it, what noise to omit, and where to store it. A template should include:

- Template ID and version.
- Purpose and scope.
- Default feature, task, focus, and location.
- Required and optional sections.
- Required inputs.
- Noise policy.
- Storage pattern.
- Comparison or baseline rules.

The preferred shorthand for selecting a template is:

```text
Create Report: "My Report", SO_MyReportTemplate
```

Values supplied explicitly in the request override values from the ScriptableObject. Values from the template override project defaults. Current task, branch, commit, date, and other safe context may fill remaining fields automatically.

Unity ScriptableObjects are appropriate for designer-authored report templates. Runtime and tool-generated data should be supplied through a report-run data object or an exported JSON payload so the same contract can be consumed outside Unity.

## Organization and indexing

Reports must be classified and searchable rather than treated as an anonymous stack of files. Each report should have:

- Stable Experiment, Run, Report, and Analysis IDs.
- Feature/system area.
- Scenario and test type.
- Relevant biome, species, event, or mode.
- Source commit/build.
- Outcome and status.
- Related task, milestone, or decision.
- Baseline or parent report, when applicable.

The registry should support queries such as:

- All reports for a feature.
- All crashes involving a specific event or species.
- Results before and after a code change.
- Reports generated by a specific tool or AI template.
- Reports linked to an unresolved decision.

Initial implementation may use versioned Markdown and JSON plus a small index. A database or dashboard should be added only when real query needs justify it.

## Diffability and reproducibility

Reports must be designed for both human review and automated comparison.

- Use stable section and field ordering.
- Use canonical JSON with sorted keys.
- Use consistent units and numeric precision.
- Sort lists and event records deterministically.
- Keep timestamps and unique IDs in metadata rather than factual summaries.
- Record schema and template versions.
- Keep large binary artifacts out of text files; store their paths and hashes.
- Mark approved baseline runs for regression comparisons.

Generated reports should make it possible to compare a baseline and a variant by showing changed metrics, new anomalies, resolved anomalies, and meaningful outcome differences.

## Failure handling

Reports are required for normal completion, manual stopping, crashes, and failed automation. A partial failure should preserve:

- What completed.
- Where execution stopped.
- Which outputs are incomplete.
- Errors and warnings.
- The last known configuration and state.

“No reward was produced” is not a sufficient crash report.

## Minimum quality standard

An automated workflow is not considered complete until a human can:

1. Identify what question the work was investigating.
2. Reproduce or meaningfully investigate the run.
3. Separate observed facts from AI interpretation.
4. Locate the evidence behind important claims.
5. Compare the result with a baseline or prior run when appropriate.
6. Record and link the resulting human decision.

> Every automated result must be inspectable, reproducible, attributable, and connected to a human decision.

## Revision history

| Version | Date | Change |
|---|---|---|
| 1.0 | 2026-08-15 | Initial studio guideline adopted. |
