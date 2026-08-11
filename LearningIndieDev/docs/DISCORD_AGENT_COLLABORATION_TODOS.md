# Discord agent collaboration TODOs

This is a tracked exploration goal for connecting Josh's and Sim's AI-assisted
development workflows through a shared Discord channel. Discord would be a
coordination and handoff layer; the repository and project context files remain
the source of truth for code and durable decisions.

## Goal

Allow both agents to exchange task updates, questions, decisions, validation
results, and handoffs through a shared channel without relying on one shared
conversation hash.

This does **not** promise identical private conversation context. Temporary
reasoning, local editor state, uncommitted files, and tool output still need to
be recorded explicitly when they affect the project.

The proposed message contract is documented in
[`DISCORD_AGENT_COLLABORATION_PROTOCOL.md`](DISCORD_AGENT_COLLABORATION_PROTOCOL.md).

## Roadmap priority

1. Define the shared message and context contract.
2. Prove a minimal Discord read/write integration.
3. Add reliable agent handoffs and repository links.
4. Add safe automation, conflict handling, and observability.

## Active goal

### TODO-COLLAB-01 - Define the collaboration contract (Priority 1)

- [x] Define the minimum message fields and source-of-truth rules for task starts,
  progress updates, decisions, blockers, and completed handoffs.
- [ ] Decide the Discord channel structure and final transport-specific message
  format for task starts, progress updates, decisions, blockers, and completed
  handoffs.
- [ ] Define which information must be copied into project Markdown and which
  information may remain Discord-only.
- [ ] Define the required branch, commit, test, and file references in every
  implementation handoff.
- Trigger: before building any integration so the channel does not become an
  unstructured second source of truth.

### TODO-COLLAB-02 - Minimal Discord integration proof (Priority 2)

- [x] Add a credential-free PowerShell transport adapter with publish/read
  modes and dry-run validation.
- [ ] Determine whether the available Codex/IDE setup can access Discord
  directly or whether a separate bot/service is required.
- [ ] Create a restricted test channel and prove authenticated message posting
  and reading.
- [ ] Confirm that credentials are kept outside the repository and that the
  integration can be disabled without affecting the game project.
- Trigger: TODO-COLLAB-01 is agreed and a Discord integration path is available.

### TODO-COLLAB-03 - Structured agent handoffs (Priority 2)

- [ ] Post task-start and task-complete messages containing branch, commit,
  changed files, validation, risks, and next actions.
- [ ] Link each handoff to the corresponding file in `docs/handoffs/`.
- [ ] Include the latest `WORKING_STATE.md` and relevant TODO document links.
- Trigger: the minimal read/write proof succeeds.

### TODO-COLLAB-04 - Context digest and wake-up flow (Priority 3)

- [ ] Define how an agent detects a new message intended for it.
- [ ] Add a compact context digest so an agent can catch up without replaying
  the full Discord history.
- [ ] Decide whether wake-ups are manual, polled, webhook-driven, or handled by
  an external orchestration service.
- Trigger: handoff messages are consistent enough to automate safely.

### TODO-COLLAB-05 - Safety and conflict handling (Priority 3)

- [ ] Prevent agent-to-agent reply loops and duplicate processing.
- [ ] Establish ownership rules for uncommitted files and branch changes.
- [ ] Record permissions, secret handling, rate limits, and failure behavior.
- [ ] Make repository state and committed Markdown authoritative when Discord
  messages conflict or become stale.
- Trigger: before enabling unattended or bidirectional automation.

### TODO-COLLAB-06 - Operational validation (Later)

- [ ] Test a complete handoff between Josh's and Sim's workflows.
- [ ] Test recovery after a disconnected bot, failed message, stale branch, or
  partially completed task.
- [ ] Document setup and teardown so another contributor can reproduce it.
- Trigger: the integration is used for a real project task.

## Explicit non-goals

- Sharing private conversation history automatically.
- Treating Discord as the source of truth for code or architecture.
- Allowing agents to modify the repository without branch and validation
  records.
- Building a general-purpose multi-agent platform before the workflow proves
  useful.

## Revisit rules

- Keep this integration separate from simulation and gameplay code.
- Promote one TODO at a time after its trigger is satisfied.
- Record implementation and validation in a dated handoff note.
- Close or revise a TODO when a concrete integration decision replaces it.
