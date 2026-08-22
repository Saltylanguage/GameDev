# AI workflow skills plan

This document records possible repo-local Codex skills that would formalize
repeatable project workflows. Skills should stay thin, reuse existing scripts
and documentation, and be created only when the workflow is stable enough to
test with representative prompts.

## Active project skills

| Skill | Scope and trigger |
|---|---|
| **Skill: Correctness-First Engineering** | Safety harness for conflicting evidence, meaningful uncertainty, failed attempts, challenged claims, regression risk, and correctness reviews. It interrupts speculation and repeated fixes without loading for routine low-risk edits. |
| **LooseEnds** | Evidence-backed review of unassigned work, informal plans, orphaned artifacts, forgotten decisions, and unresolved project uncertainty. |
| **Sprint Kickoff** | Trello sprint audit, rollover, and explicit carry-over workflow. |

## Planned skills

| Skill | Status | Scope and trigger | Build gate |
|---|---|---|---|
| **CellSim Experiments** | In progress / evaluate | Run baselines, comparisons, replays, and report interpretation through the existing `CellSim` command surface. | Revisit after EX-002; do not duplicate or replace the experiment contract already being developed there. |
| **Species Design Treatment** | Planned | Turn species or coupled-species requests into the treatment template: identity, dependencies, hypotheses, balance knobs, telemetry, and promotion decision. | Build when the current treatment template and Forest Edge workflow are used for another iteration. |
| **Handoff** | Planned | Prepare Josh/Sim handoffs with branch, commit, changed files, validation, risks, next actions, and context links. | Build when handoffs are being repeated across contributors or Discord work. |
| **Unity Validation** | Planned | Select the smallest relevant Edit Mode, Play Mode, bootstrap, `CellSim`, replay, and diff checks for a requested validation pass. | Build after the validation commands and ownership boundaries stop changing. |
| **Change Impact Analysis** | Deferred / exploratory | Analyze likely affected artifacts, tests, reports, and simulation outcomes for a proposed change. | Reconsider after EX-002 and the predictive change-impact research brief produce an approved workflow. |

## Boundaries

- Keep broad product direction and detailed engineering rules in `AGENTS.md`
  and project documentation. A safety skill may address a focused reasoning
  failure mode, but it must not replace project or domain guidance.
- Prefer existing `CellSim`, handoff, and authoring scripts over new wrappers.
- Skills may draft reports or plans, but must not silently change canonical
  design documents, scenario assets, or generated evidence.
- Every skill needs a representative prompt test before it is treated as a
  shared project workflow.

## Current decision

The CellSim workflow is already part of EX-002's active work. Treat the future
skill as a possible convenience layer around that work, not as a separate
parallel implementation. The next likely standalone skill is **Species Design
Treatment**, followed by **Handoff** if cross-contributor coordination grows.
