---
name: cellsim-experiment
description: Design, run, or compare seeded CellSim experiments for species, upgrades, scenarios, and ecology. Use when controlled simulation evidence is needed.
---

# CellSim Experiment

Turn a specific game-design or simulation question into reproducible evidence. This skill coordinates the project's existing runner, reports, and analysis tools; it does not introduce a second simulator or decide production balance values.

## Start from the current contract

Read the task-relevant parts of [SG-001](../../../LearningIndieDev/docs/Studio%20Guidelines/AI_GENERATED_REPORTS.md), [SG-005](../../../LearningIndieDev/docs/Studio%20Guidelines/SG-005-UPGRADE-AND-ECOLOGY-BALANCE.md), [simulation tooling](../../../LearningIndieDev/docs/UNITY_SIMULATION_TOOLING.md), and [Unity plugin workflows](../../../LearningIndieDev/docs/UNITY_PLUGIN_WORKFLOWS.md). Check the current branch, working tree, scenario, runner, report schema, and relevant recent handoff. Use the current code and artifacts to establish what is implemented; dated reports may describe older rules.

First determine whether the request is to design an experiment, execute an already approved experiment, or analyze existing reports. Do not rerun a completed experiment just to answer a report-reading question.

## Experiment contract

Before a new run, identify the human decision owner and record the question, hypothesis, success and failure criteria, scope, intended changed variable, baseline and candidate arms, and required telemetry. SG-001 requires the named owner to approve the experiment scope before execution; existing approval of this exact scope counts. If approval is missing, complete the design and any read-only preparation, then request only that approval.

Freeze the comparison inputs that matter: source revision and dirty-state disclosure, scenario and snapshot, simulation mode, map and starting populations, species and active Genome, ordered Mutations and acquisition timing, seed set, tick/phase window, and runner/report versions. State any variable that cannot be held fixed. Use a small diagnostic seed set first; enlarge it only when variance, rare events, or the decision's importance warrants it. Do not treat an old independent-window report as evidence for a continued-world journey.

## Run and compare

- Use `LearningIndieDev/CellSim.ps1` and the documented Live/Clean routing. Use `unity-cli` for Unity Editor or test operations. Follow the project's elevated-host default for EcoSim tests and experiments. Do not edit a production scenario merely to form an experiment arm when input overrides or isolated fixtures suffice.
- Retain each run's command, seed list, source identity, input and ruleset fingerprints, status, and artifact paths. Check that both arms actually completed and that the supposedly fixed inputs match before presenting paired differences. If they do not, report a comparability failure and design a corrected run.
- Use `artifact-summarization` for reports too large to inspect directly or for compact paired evidence, while keeping the raw reports. Examine per-seed differences and variation as well as aggregates; use phase windows and the relevant activity, mortality, resource, and population measures. Keep focal-species, biome, and economy outcomes separate where applicable.
- Keep factual report contents, interpretation, and the human accept/revise decision distinct. A matched comparison supports an effect under its declared inputs; it does not by itself prove a universal balance rule, player enjoyment, or causality when more than the intended variable changed.

Return the question, controlled inputs, commands and artifacts, observed results, confidence and limits, the next smallest discriminating experiment if needed, and the decision awaiting its owner. For a durable experiment record, follow the [experiment report template](../../../LearningIndieDev/docs/Research/Templates/EXPERIMENT_REPORT_TEMPLATE.md) and the project's handoff convention; do not rewrite raw run evidence when analysis changes.
