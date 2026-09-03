# SG-004 — Conversation Scope and Continuity

**Guideline ID:** SG-004
**Status:** Active
**Version:** 1.0
**Adopted:** 2026-09-01
**Audience:** Developers, designers, producers, and AI agents working in project conversations.
**Related documents:** [Collaboration Workflow](../COLLABORATION_WORKFLOW.md), [Working State](../WORKING_STATE.md), [SG-002 — AI-Assisted Development](AI_ASSISTED_DEVELOPMENT.md)

## Purpose

Project conversations are useful working sessions, but they are not the
project's permanent memory. A thread may naturally wander while people think,
investigate, and solve adjacent problems. That is healthy and should not be
discouraged by constant requests to start over.

The goal of this guideline is to keep conversations useful without turning
conversation organization into friction. Start a new thread when the current
one has become materially harder to follow or is now serving a different
workstream—not whenever a small side question appears.

## Default behavior

- Continue the current thread for follow-ups, clarifications, experiments, and
  small-to-medium deviations that still share the same general goal.
- Keep related decisions together when they concern the same feature, artifact,
  branch, or milestone.
- Record durable decisions, implementation state, and handoff information in
  project documents rather than relying on chat history alone.
- Archive completed or unwieldy conversations when useful. Archiving is an
  organizational action; it is not a deletion or a usage reset.

## When to suggest a new conversation

The AI may give a brief, non-blocking recommendation when one or more of these
conditions is true:

1. The primary goal has changed to a different feature, workstream, or
   deliverable, and the old context is no longer helping the current task.
2. Several unrelated workstreams have accumulated and important decisions are
   becoming difficult to locate or distinguish.
3. The current task needs a different repository, branch, application, owner, or
   approval boundary.
4. The thread is causing repeated restatement, contradictory assumptions, or
   uncertainty about which decision is current.
5. A clean handoff or review would be materially easier with a focused context.

These are judgment calls, not a fixed message count or token threshold. The
recommendation should explain the concrete reason in one sentence and offer the
smallest useful next step.

## How to make the recommendation

When the threshold is reached, the AI should:

1. Finish or pause the current atomic task safely when possible.
2. State what has been completed, what remains, and which durable documents
   contain the important context.
3. Suggest a focused title or scope for the next conversation.
4. Leave the choice with the human; do not refuse ordinary work, force a split,
   or imply that a new thread is required.

If the user chooses to continue, the AI should continue with the stated scope
and manage the context carefully.

## What does not justify a warning

Do not suggest a new conversation solely because:

- The user asks a related clarification or quick factual question.
- The thread contains normal planning, implementation, testing, and review for
  one feature.
- The conversation is long but the current goal, ownership, and source of truth
  remain clear.
- A short tangent can be answered without changing files, authority, or the
  current workstream.

Do not use this guideline as a refusal, a productivity lecture, or a substitute
for writing a concise handoff note.

## Ownership and record keeping

The human decides whether to continue, start a new conversation, or archive an
old one. The AI is responsible for noticing material context drift, explaining
the recommendation without overclaiming, and keeping project state in the
repository's durable documents. Conversation history may be searched later, but
it should not be treated as the only record of an accepted project decision.

## Review and exceptions

This guideline is advisory. A conversation may remain open across multiple
workstreams when the human intentionally wants one continuous investigation and
the current state remains understandable. Record that choice only when it
affects a handoff, review, or external collaboration.

## Revision history

| Version | Date | Change |
| --- | --- | --- |
| 1.0 | 2026-09-01 | Initial guideline: allow natural topic drift and recommend new threads only when context materially degrades. |
