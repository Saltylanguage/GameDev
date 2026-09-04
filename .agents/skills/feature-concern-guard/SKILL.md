---
name: feature-concern-guard
description: >
  Capture concrete Mild or Extreme concerns while planning a named feature,
  experiment, integration, migration, or work block, then detect and enforce
  their recorded triggers during later work. Use when the user asks to identify,
  document, review, or enforce planning concerns, or when current work has a
  matching concern record. Do not use for generic code review, speculative risk
  lists, Loose Ends triage, or routine low-risk edits with no matching record.
---

# Feature Concern Guard

Preserve feature-scoped concerns from planning through implementation. Surface a
concern only when its recorded trigger applies to the current action.

This skill constrains the agent's execution, not the user's authority. It does
not replace project policy, approval requirements, Correctness-First
Engineering, or LooseEnds.

## Severity contract

Use exactly two severities.

### Mild

A recoverable concern that could cause rework, confusion, maintenance debt, or
a degraded result.

When triggered:

1. Warn once during the current work attempt.
2. State what was detected, why it matters, and the smallest useful mitigation.
3. Continue unless the user asks to stop or the action independently requires
   approval.
4. Warn again only if the evidence, scope, or consequence materially changes.

Use this format:

> **Mild concern — `<id>`: `<title>`.** `<trigger and consequence>`. I can
> continue, but `<smallest mitigation>` would reduce the risk.

### Extreme

A concern with a concrete path to invalidating the work's main objective,
damaging or losing work, creating a difficult-to-reverse shared effect,
violating an accepted architecture or product boundary, exposing protected
information, or making evidence fundamentally untrustworthy.

When triggered:

1. Pause before the implicated action. If discovered afterward, stop further
   affected work immediately.
2. Identify the trigger, evidence, consequence, affected action, and safest next
   step.
3. Ask the user to mitigate, revise the plan, change the recorded severity with
   rationale, or explicitly accept the scoped risk.
4. Continue unrelated safe work when possible.
5. Record an explicit waiver as `Accepted Risk`, including its exact scope. A
   waiver does not authorize anything prohibited by higher-level policy or
   permission rules.

Use this format:

> **Extreme concern — `<id>`: `<title>`.** `<trigger and concrete consequence>`.
> I have paused `<affected action>`. The safest next step is `<mitigation or
> decision needed>`.

Pause only the implicated action. Never claim to prevent the user from acting
independently.

## Planning workflow

When planning concerns are requested:

1. Establish the named feature or work-block boundary and its human owner when
   known.
2. Read the canonical plan, relevant architecture and product decisions, nearby
   implementation, dependencies, working state, and task-relevant handoffs.
3. Search for an existing concern record before creating one. Prefer a clearly
   labelled concerns section or link in the canonical feature plan. Otherwise
   use `LearningIndieDev/docs/Planning Concerns/<work-slug>.md`.
4. Identify only concerns supported by evidence or a specific, credible failure
   mode. Separate them from ordinary implementation details, open design
   questions, unrelated future work, and uncertainty that merely needs
   investigation.
5. Propose the concern, severity, trigger, and smallest mitigation to the user.
   Treat it as durable only after the user accepts or adjusts it.
6. Record accepted concerns using
   [the concern-record template](references/concern-record-template.md). Keep the
   record short and readable at a glance.
7. Do not silently change the approved plan while documenting a concern.

If an existing plan already has a concise concern section, maintain it rather
than creating a competing ledger. One work block should have one authoritative
concern record.

## Execution workflow

When implementing or resuming a named work block:

1. Locate its concern record through the canonical plan, a direct link, or the
   fallback directory. Do not scan unrelated concern records.
2. Read each active concern's scope, trigger, status, mitigation, and waiver.
3. Before a material action, check whether that action satisfies a recorded
   trigger.
4. Apply the Mild or Extreme behavior exactly as defined above.
5. Update status when new evidence proves the concern mitigated, accepted,
   resolved, or superseded. Never silently remove, broaden, or downgrade it.
6. When a new concern emerges during execution, present it for review before
   adding it. Ordinary project safety rules still apply immediately even when
   the new concern is not yet recorded.

Status behavior:

- `Open` and `Acknowledged`: active.
- `Mitigated`: active only if its recorded mitigation is absent or fails.
- `Accepted Risk`: do not repeat within the recorded waiver scope; warn if the
  action exceeds that scope or the evidence materially worsens.
- `Resolved` and `Superseded`: inactive, retained as history.

## Noise and scope controls

Do not:

- treat every uncertainty, TODO, code smell, or hypothetical edge case as a
  concern;
- activate a concern outside its recorded feature, action, or trigger;
- promote an incomplete-evidence concern to Extreme without a concrete
  high-impact consequence;
- repeatedly warn about an unchanged Mild concern;
- block unrelated safe work because one Extreme action is paused;
- duplicate generic safety, permission, code-review, LooseEnds, or
  Correctness-First guidance;
- create hooks, monitors, dashboards, dependencies, or enforcement
  infrastructure;
- silently modify production behavior, architecture, scope, or external systems
  while recording a concern.

Small and medium deviations are normal. Warn only when a recorded trigger makes
the concern relevant to the action now being considered.

## Relationship to other project skills

- **Correctness-First Engineering** addresses evidence quality, failed
  reasoning, uncertainty, and overconfidence during technical work.
- **LooseEnds** finds organizational gaps, forgotten work, contradictions, and
  orphaned artifacts.
- **Feature Concern Guard** preserves previously reviewed, feature-scoped
  concerns and responds when their concrete triggers occur.

A concern may cite another skill's finding, but must not duplicate that skill's
whole workflow.

## Definition of done

The skill has done its job when concerns are scoped, evidence-backed, accepted
by the user, stored once, and acted on proportionally; Extreme gates stop only
the unsafe step; waivers and resolutions remain traceable; and ordinary work is
not burdened by unrelated or repetitive warnings.
