---
name: sprint-kickoff
description: Manage this project's one-week sprint lifecycle in Trello: audit the active sprint, warn about unfinished assigned work, close the previous sprint, start the next sprint, and carry unfinished cards with KickTheCan. Use for Sprint Kickoff, sprint rollover, carry-over, backlog normalization, or KickTheCan requests.
---

# Sprint Kickoff

## Overview

Use this skill for this project's Trello sprint lifecycle. Read the canonical
workflow in `LearningIndieDev/docs/SPRINT_KICKOFF_WORKFLOW.md` before changing
sprint state.

## Board state model

Read [`LearningIndieDev/docs/SPRINT_KICKOFF_WORKFLOW.md`](../../../LearningIndieDev/docs/SPRINT_KICKOFF_WORKFLOW.md)
before changing sprint state. Keep executable cards in exactly one workflow
list:

- `🗂️ Backlog` — unscheduled work, including work that is not ready.
- `🎯 Upcoming Work` — selected work for the next sprint.
- `Current Work` — current-sprint tasks not started by either developer.
- `🛠️ In Progress` — current-sprint tasks actively being worked on.
- `⛔ Blocked` — current-sprint work blocked by a named dependency.
- `✅ Done` — completed work.

Do not automatically move reference or initiative cards in `Game Design`,
`Research & Future`, `🧭 Roadmap & Milestones`, or `Archived`.

## Required safety sequence

1. Inspect the current board and sprint control record before mutating anything.
2. Resolve the current sprint ID, next sprint ID, dates, goal, and capacity.
3. Produce a dry-run summary of cards, owners, estimates, list state, and
   carry-over candidates.
4. Notify the caller about every unfinished assigned current-sprint card,
   unassigned card, blocked card, missing estimate, and capacity overage.
5. Ask for explicit confirmation immediately before Trello mutations.
6. Apply only the confirmed scope, preserving titles, owners, estimates,
   checklists, labels, attachments, comments, and card URLs.
7. Re-read the board and report each successful or failed move. Never claim a
   sprint closed or opened without verification.

Default to dry-run behavior. Never include completed cards in carry-over unless
the caller explicitly requests `--include-done`.

## Sprint Kickoff

`SprintKickoff` ends the active sprint and starts the named next sprint. It must:

- audit unfinished current-sprint work before closure;
- leave completed work in `✅ Done`;
- move unfinished work to `🗂️ Backlog` unless explicitly carried to the next
  sprint;
- move selected next-sprint cards from `🎯 Upcoming Work` to `Current Work`;
- preserve cards already actively being worked on in `🛠️ In Progress`;
- create or update the sprint control record and status;
- verify owner, estimate, acceptance check, and sprint metadata for each
  current-sprint task;
- return a kickoff report with carry-over, unassigned, blocked, capacity, and
  verification results.

If unfinished assigned work exists, warn the caller and wait for the caller's
decision. Do not silently discard or reassign it.

## KickTheCan

Use the following selector and target rules:

```text
KickTheCan --dev Sim
KickTheCan --sprint S2
KickTheCan --dev Sim --sprint S2
KickTheCan --task T-058
KickTheCan --task T-058 --sprint S2
```

- `--dev NAME`: select unfinished current-sprint cards assigned to that dev;
  default target is the next sprint.
- `--sprint ID`: select all unfinished current-sprint cards and move them to
  the specified sprint.
- `--dev NAME --sprint ID`: select the intersection and move it to that sprint.
- `--task ID`: select exactly one unfinished card; default target is the next
  sprint.
- `--task ID --sprint ID`: select exactly one card and use the explicit target.

Use the card's stable Trello short ID or the project's explicit task ID. A
missing, ambiguous, completed, or non-current-sprint target is an error unless
the caller supplies an explicit override. `--reason` should be included when a
carry-over decision needs context.

After selecting a target sprint:

- future-sprint cards belong in `🎯 Upcoming Work`;
- cards assigned to the active sprint belong in `Current Work` unless already
  active, in which case they belong in `🛠️ In Progress`;
- blocked work remains `⛔ Blocked` only when its target sprint is active and
  the blocker is still current;
- preserve the owner and do not alter scope or estimate automatically.

Do not move unassigned, blocked, or completed cards by default. Support explicit
opt-ins such as `--include-unassigned`, `--include-blocked`, and
`--include-done`, and list those exceptions in the final report.

## Failure handling

Stop before mutation when the sprint record is missing, the target sprint is
ambiguous, a card cannot be classified, or board state disagrees with the
caller-provided scope. If a multi-card mutation partially succeeds, report the
exact completed and failed cards and make the next run safe to repeat.

## Useful companion operation

When the caller asks for a status check without changing the board, run a
read-only `SprintAudit`: report sprint dates and goal, list state mismatches,
missing metadata, owner capacity, carry-over candidates, and cards that need
refinement before entering `🎯 Upcoming Work`.
