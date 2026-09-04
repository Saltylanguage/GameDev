---
name: plain-language-docs
description: Rewrite project planning documents so people can understand and explain the work easily, without losing scope, ownership, estimates, decisions, or acceptance criteria.
---

# Plain-Language Docs

Use this skill when a roadmap, sprint plan, feature plan, handoff, or similar
document needs to be easy to read aloud in a meeting.

## What to preserve

- the purpose and problem being solved;
- approved scope, non-goals, owners, reviewers, estimates, and dependencies;
- acceptance criteria and evidence requirements;
- stable IDs, file links, dates, and other facts that make the plan traceable.

Do not silently change a decision, estimate, owner, or scope while simplifying
the wording. If the plan needs a real change, call it out as a plan change.

## How to write it

1. Start with the outcome in ordinary language.
2. For each work item, answer: what are we trying to do, what is needed, and
   what problem does it solve?
3. Prefer familiar words over specialist terms. Define a technical term once
   only when it is necessary for accuracy.
4. Keep the roadmap high-level. Put implementation details in a feature brief,
   ticket, test plan, or linked technical note.
5. Use compact tables when they make ownership, status, effort, or sequence
   easy to scan.
6. Read the result aloud mentally. A developer or designer should understand
   the point without decoding several terms before reaching the verb.

## When work is shared

If two tasks should move together, describe them as one shared goal with clear
subtasks and one acceptance gate. Keep the subtasks distinct when they have
different failure modes or verification needs.

## Guardrails

Plain language is not permission to remove useful precision. Keep exact
numbers, seed requirements, reproducibility rules, and evidence boundaries when
they protect the result. Do not turn a plan into a technical design document,
or add new process, tooling, or scope merely to make it sound complete.
