# AI contribution instructions

For Unity work in this project:

- Read [`docs/PROJECT_CONTEXT.md`](docs/PROJECT_CONTEXT.md) for current product direction, research references, and durable design decisions that should carry across Codex desktop and IDE conversations.
- Read [`docs/WORKING_STATE.md`](docs/WORKING_STATE.md) as the collaboration entry point, then inspect the newest and task-relevant notes in `docs/handoffs/`. Confirm them against the current branch and recent Git history before relying on them.
- Follow [`docs/COLLABORATION_WORKFLOW.md`](docs/COLLABORATION_WORKFLOW.md) when handing work between developers or AI sessions. Update shared context when a material decision or integration state changes; do not store raw chat transcripts in the repository.
- Follow the Studio Guidelines in [`docs/Studio Guidelines/AI_GENERATED_REPORTS.md`](docs/Studio%20Guidelines/AI_GENERATED_REPORTS.md) and [`docs/Studio Guidelines/AI_ASSISTED_DEVELOPMENT.md`](docs/Studio%20Guidelines/AI_ASSISTED_DEVELOPMENT.md). During the enforcement prototype, policy alerts are advisory warnings only; they must not be treated as automatic blocks.
- Follow the Studio Guidelines in [`docs/Studio Guidelines/AI_GENERATED_REPORTS.md`](docs/Studio%20Guidelines/AI_GENERATED_REPORTS.md) and [`docs/Studio Guidelines/AI_ASSISTED_DEVELOPMENT.md`](docs/Studio%20Guidelines/AI_ASSISTED_DEVELOPMENT.md). During the enforcement prototype, policy alerts are advisory warnings only; they must not be treated as automatic blocks.
- Follow [`docs/UNITY_ENGINEERING_STANDARDS.md`](docs/UNITY_ENGINEERING_STANDARDS.md) and use [`docs/UNITY_STANDARDS_ADOPTION_PLAN.md`](docs/UNITY_STANDARDS_ADOPTION_PLAN.md) for migration scope.
- For terrain added on top of existing tiles or textures, follow [`docs/TILE_AUTHORING_GUIDE.md`](docs/TILE_AUTHORING_GUIDE.md) before creating or wiring production art.
- Inspect nearby code before choosing a convention; preserve the dominant first-party convention where it is stable.
- Preserve serialized fields, Unity GUIDs, and every `.meta` file. Do not move or rename Unity assets unless the goal explicitly requests a Unity Editor migration.
- Keep dependencies visible; avoid global state, service locators, hidden discovery, premature patterns, and unmeasured optimizations.
- Treat named design patterns as solutions to demonstrated problems: the current `IActivityTarget.CreateActivity()` factory is approved; simple enums remain the default for state; do not add Singleton, Builder, Proxy, Observer/event bus, Mediator, or Prototype without the trigger defined in `docs/UNITY_ENGINEERING_STANDARDS.md`.
- Add focused tests for changed domain logic. Profile before claiming a performance improvement.
- Keep Editor code out of runtime assemblies and avoid broad refactors during feature work.
- Report any intentional deviation from the stylesheet in the change summary.

## Loose Ends protocol

Treat `/Loose Ends`, `Loose Ends`, and `Show me my Loose Ends` as the same project-documentation review request. The installed skill's formal UI name is `LooseEnds`; the spaced slash form is the project shorthand. Inspect the repository guidance, working state, handoffs, plans, TODOs, recent decisions, implementation state, and relevant conversation context. Return a triaged report covering:

1. Unassigned work.
2. Plans that remain informal or have not been recorded durably.
3. Orphaned or misplaced artifacts and documents.
4. Discussed decisions, risks, assumptions, or follow-ups that could be forgotten.
5. Contradictions or uncertainty in the assistant's understanding.

For each item include priority (`P0`-`P2`), evidence/location, current status, recommended next action, likely owner, and confidence. Distinguish new, resolved, and still-open items. Do not silently modify project files while reporting. Keep the durable ledger in [`docs/LOOSE_ENDS.md`](docs/LOOSE_ENDS.md) and update it only when the user asks to record or resolve findings.
