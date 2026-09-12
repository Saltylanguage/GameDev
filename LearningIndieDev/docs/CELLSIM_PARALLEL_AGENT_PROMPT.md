# Copy-paste prompt: build parallel CellSim

Give the following prompt to an agent with access to the repository. The linked pack is the authoritative scope and acceptance contract. Sending this draft alone does not start work in the current task.

```text
Implement the parallel CellSim workflow described in:
F:\ForkBin\GameDev\LearningIndieDev\docs\CELLSIM_PARALLEL_BUILD_PACK.md

Read the whole pack and applicable AGENTS.md/project guidance first. Treat G0-G6
as ordered goals, and carry the implementation through their acceptance gates.
Bevin is the human decision owner. The goal is a measured reduction in time to
validated skill-sweep results, with unchanged scientific outputs.

Use the existing simulation and report code. Build one Linux headless player
per immutable source/content snapshot, package it in Docker, and use one local
coordinator to run bounded worker containers. Start with two workers. Preserve
the legacy Editor command as a reference and regression path.

Begin by checking the actual source, dirty work, host prerequisites, and build
dependencies. The pack's repository snapshot may have changed. Preserve existing
user edits. Work in an isolated checkout when necessary, and explicitly include
and hash any current skill edits needed for the requested tests. Never claim a
HEAD-only build contains uncommitted changes. Keep .meta files and GUIDs intact.

My instruction to implement includes local source/tooling changes and the
bounded technical validation fixtures in G0-G6. Use the proposed performance
gate and fixed matrix; choose and record an existing valid skill at G0 from the
current requested testing context. If that choice is ambiguous, ask once before
the dependent runs while continuing independent implementation. This is not an
instruction to retune skills or accept their game balance.

Make routine implementation choices yourself and continue through verified
milestones. Reuse installed tools and dependencies. If a missing prerequisite
needs installation or the route requires a material scope change, prepare the
exact action and explain the blocker. Respect host approval requirements.
Do not push, publish images, activate the old remote queue, provision paid/cloud
resources, or alter unrelated work without a separate instruction.

Do not parallelize ticks or phases within an expedition. Do not scale the old
copy-based queue worker. Use a single coordinator with a batch lock, immutable
job inputs, unique attempt directories, bounded retries, and explicit container
ownership. On restart reconcile live containers before retrying jobs.

Retain the existing report/stat-line semantics and validation limitations. A job
is completed only after successful process exit and verified artifacts. Reject
missing/duplicate seeds and mixed input identities. Compare per-seed scientific
results exactly before benchmarking; isolate cross-platform differences rather
than masking them with tolerances or averages.

Deliver focused regression checks, failure/restart/cancel/resume verification,
serial-versus-parallel timing evidence, a fully validated demonstration sweep,
and a runbook with commands actually exercised on the target host. Track G0-G6
with concise evidence links in one handoff. Report blocked or failed gates
honestly; image build success alone is not completion or a speedup result.
```

For staged delegation, replace the first sentence with `Implement G0-G1 only` (or the next goal range) and keep the rest of the contract. Each following agent must read the previous handoff and reuse its verified artifacts rather than restarting completed work.
