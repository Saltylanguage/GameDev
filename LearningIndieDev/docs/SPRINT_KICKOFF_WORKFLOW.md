# Sprint Kickoff and Carry-Over Workflow

**Status:** Approved workflow design; implementation is the project-local
`sprint-kickoff` skill and its Trello board convention.

This workflow uses the project's one-week cadence and approximately 10 hours
per developer. It separates work selected for the next sprint from work planned
for the current sprint but not yet started.

## Trello workflow lists

| List | Meaning | Automatic sprint handling |
|---|---|---|
| `🗂️ Backlog` | Unscheduled executable work, ready or not ready | Default destination for unscheduled and deferred work |
| `🎯 Upcoming Work` | Selected work for the next sprint | Promoted into `Current Work` at kickoff |
| `Current Work` | Current-sprint tasks neither developer has started | Becomes current sprint scope |
| `🛠️ In Progress` | Current-sprint tasks Josh or Sim is actively working on | Preserved during kickoff if still current |
| `⛔ Blocked` | Current-sprint work blocked by a named dependency | Reported at kickoff; never silently discarded |
| `✅ Done` | Completed work | Never moved by default |

`Game Design`, `Research & Future`, `🧭 Roadmap & Milestones`, and `Archived`
are planning/reference areas, not executable sprint queues. Kickoff does not
move cards from those lists automatically.

## Sprint control record

Each sprint needs a durable control record, preferably a Trello control card
linked to a matching report under `docs/Sprints/` when reports are introduced.
It must contain:

```text
Sprint ID: S1
Status: Planned | Active | Closed
Start: YYYY-MM-DD
End: YYYY-MM-DD
Goal: one primary outcome
Capacity: Josh 10h; Sim 10h
Exit criteria: observable review gate
```

Executable cards should retain a stable task ID, owner, estimate, acceptance
check, and sprint ID. The skill must not infer sprint membership solely from a
list name when explicit metadata is available.

## Sprint Kickoff lifecycle

### Preview

Before changing the board, inspect and report:

- active and next sprint records;
- completed, active, unstarted, blocked, unassigned, and stale cards;
- unfinished assigned work grouped by developer;
- estimate totals versus each developer's capacity;
- cards missing an owner, estimate, acceptance check, or sprint ID;
- next-sprint candidates in `🎯 Upcoming Work`;
- cards that would be moved by carry-over rules.

If unfinished assigned work exists, notify the caller and wait for a decision.

### Close and open

After confirmation:

1. Mark the previous sprint control record `Closed`.
2. Leave completed cards in `✅ Done`.
3. Move uncarried unfinished executable work to `🗂️ Backlog`.
4. Apply explicit carry-over decisions through `KickTheCan`.
5. Create or activate the next sprint control record.
6. Move selected next-sprint cards from `🎯 Upcoming Work` to `Current Work`.
7. Leave cards already being worked on in `🛠️ In Progress` only when they are
   explicitly part of the new sprint; otherwise return them to `🗂️ Backlog`.
8. Verify all current-sprint cards and produce the kickoff report.

Kickoff is not complete until the board has been re-read and the resulting
lists, sprint metadata, and control record agree.

## KickTheCan contract

```text
KickTheCan --dev NAME
KickTheCan --sprint ID
KickTheCan --dev NAME --sprint ID
KickTheCan --task ID
KickTheCan --task ID --sprint ID
```

The default selection scope is unfinished cards in the active sprint. The
default target is the next sprint when no target is supplied. A target sprint
that is future-facing goes to `🎯 Upcoming Work`; the active sprint goes to
`Current Work`, except for cards already being worked on, which remain in
`🛠️ In Progress`.

The command preserves ownership, estimates, checklists, labels, attachments,
comments, and card URLs. It never changes scope, acceptance criteria, or owner
without a separate explicit request.

Optional exceptions must be explicit:

```text
--include-unassigned
--include-blocked
--include-done
--reason "why this carry-over is intentional"
--dry-run
```

`--dry-run` is the default behavior for new or ambiguous requests. Mutating
Trello requires explicit caller confirmation after the preview.

## Ready-for-sprint gate

A card should move from `🗂️ Backlog` to `🎯 Upcoming Work` only when it has:

- a clear outcome or problem statement;
- an owner and reviewer;
- a rough estimate;
- dependencies and risks;
- an acceptance check;
- a stable task ID;
- a clear reason it belongs in the next sprint.

Unready ideas remain in the backlog. New work discovered mid-sprint returns to
the backlog unless the caller explicitly acknowledges the capacity impact.

## Reports and auditability

Every kickoff or carry-over operation returns a concise report containing:

- operation ID, caller, timestamp, source sprint, and target sprint;
- preview scope and confirmation status;
- cards selected, moved, skipped, or failed;
- owner/estimate totals before and after;
- warnings for unassigned, blocked, stale, or over-capacity work;
- follow-up actions and unresolved ambiguity.

The report is a coordination artifact. It does not replace Trello activity or
the sprint control record.
