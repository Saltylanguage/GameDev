# Developer and AI collaboration workflow

This repository is the shared context channel between developers and their AI
assistants. Keep context close to the code, concise enough to reread frequently,
and explicit about what is decided versus what is still exploratory.

## Sources of truth

Use these sources for different kinds of information:

1. [`PROJECT_CONTEXT.md`](PROJECT_CONTEXT.md) records durable product direction,
   architectural guardrails, and research that should survive individual tasks.
2. [`WORKING_STATE.md`](WORKING_STATE.md) is a stable index into the append-only
   [`handoffs/`](handoffs/) journal. Each task or handoff gets its own note rather
   than adding entries to one master file.
3. Git history and the code record implementation details. Use focused commits
   with messages that explain intent, and inspect recent diffs before modifying
   another contributor's area.

Do not copy entire AI conversations into documentation. Extract only decisions,
constraints, assumptions that affect future work, and evidence needed to reproduce
or validate behavior.

## Starting a work session

Each developer or AI assistant should:

1. Fetch or pull through the developer's normal Git workflow.
2. Read `AGENTS.md`, `PROJECT_CONTEXT.md`, `WORKING_STATE.md`, and the newest or
   task-relevant files in `docs/handoffs/`.
3. Inspect `git status`, the current branch, and recent commits.
4. Inspect the relevant code and tests instead of assuming the context documents
   describe every implementation detail.
5. State any assumption that would materially change the design before building
   on it.

Uncommitted changes belong to the developer who owns that working tree. Preserve
them unless that developer explicitly asks for them to be changed or discarded.

## During development

- Keep commits focused enough that another developer can review or revert one
  idea without losing unrelated work.
- Add tests alongside changed domain rules and record which checks actually ran.
- Use deterministic seeds and record them when reporting simulation regressions.
- Separate experimental conclusions from approved direction. An experiment may
  prove feasibility without establishing the final architecture.
- When two branches touch the same integration seam, record that risk in the
  task's handoff note before sharing it.

## Ending or handing off a work session

### Push reminder

From now on, whenever we add new work to the project, updating the related
Trello board(s) is part of the push. After pushing, take a quick look at the
matching card(s) and bring them up to date so the board and the project tell
the same story. A push is not fully wrapped up until that Trello update is
done.

Before another developer takes over:

1. Commit and push the work intended for sharing, or clearly identify anything
   that remains local.
2. Create a handoff note with `tools/New-Handoff.cmd`, then fill in the actual
   changes, decisions, validation, risks, and next steps. Use a new note for each
   independently reviewable task rather than accumulating a personal diary.
3. Update `PROJECT_CONTEXT.md` only when durable product direction or a lasting
   architectural decision changed.
4. Provide the branch name and commit hash. Never describe work as shared merely
   because it exists in one local working tree.
5. Record commands or Unity Test Runner suites that were actually executed, and
   distinguish successful compilation from executed tests.

## Resolving conflicting context

The checked-out code and Git history are authoritative for what is implemented.
`PROJECT_CONTEXT.md` is authoritative for current product intent. Handoff notes
describe work at a particular branch and commit and may lag behind integration.
If product intent and implementation disagree, surface the discrepancy rather
than silently changing either one.
