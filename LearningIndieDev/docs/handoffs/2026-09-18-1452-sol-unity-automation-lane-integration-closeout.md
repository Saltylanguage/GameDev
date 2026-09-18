# Unity automation lane integration closeout

[Working state](../WORKING_STATE.md) | Status: complete

- Handoff schema: 1
- Handoff ID: 2026-09-18-1452-sol-unity-automation-lane-integration-closeout
- Owner: Sol
- Branch: Production/ProjectCleanup
- Baseline commit: 9ff5c36c
- Date: 2026-09-18
- Supersedes: none

## Summary

Unity automation now has one project-owned command surface that works with a
ready Editor or a closed project. Focused work can reuse the live Pipeline
connection, while clean CLI-managed runs remain the acceptance path. This
removes the old requirement to close Unity before every test, visual check, or
seeded experiment and removes machine-wide process cleanup from normal runs.

## Changes

- Added `Auto`, `Live`, and `Clean` routing to tests, visual evidence, and
  experiments, plus `CellSim Doctor` for fast CLI/license diagnostics.
- Added test-name, assembly, and category filtering. Current categories are
  `Core`, `Simulation`, `Graphics`, `UI`, `Authoring`, and `Tooling`.
- Added the project-owned `cellsim_run` Pipeline command and a request-file
  bridge so live and clean experiment lanes execute the same implementation.
- Made live PlayMode and visual runs survive domain reload by starting tests
  asynchronously and polling Pipeline status.
- Replaced environment-only visual configuration with a project-local request
  file that both live and clean Editors can read.
- Configured Codex MCP through `unity mcp`, pinned to this project, and retired
  the active `unity_mcp` user-relay configuration.
- Removed the obsolete
  `ContinuousSkipPreservesWorldHistoryAndMetricsUntilTheSameAbsoluteTick` test,
  which encoded the superseded 200 tick/final decision flow.
- Updated project context, simulation tooling, visual review, MCP operations,
  workflow routing, and working-state documentation.

## Decisions and assumptions

- `Auto` uses Pipeline only when the exact project reports `ready`; it uses a
  clean batch only when the project is closed. Busy, Safe Mode, and unreachable
  locked states fail with diagnostics.
- Normal automation does not delete lock files or terminate global Unity,
  Package Manager, licensing, or relay processes.
- Full-suite acceptance uses `Clean`. `Live` is the fast focused-feedback lane.
- Unity CLI `1.0.0-beta.6` rejects its advertised `--no-pager` flag, so the
  wrappers omit it. Global `unity command` attribution precedes the command
  name.
- Cold `unity run --command cellsim_run` sent the command before the Editor was
  ready and returned 503 without retrying. Clean experiments therefore use the
  established batch `executeMethod` through `unity run`; live experiments use
  `cellsim_run` directly.

## Validation

- PowerShell parser: all five changed command scripts parse successfully.
- Doctor: healthy, including CLI and license checks:
  [`unity-doctor-20260918-133047`](../../artifacts/unity-doctor-20260918-133047/).
- Live EditMode `Core`: 26/26 passed:
  [`unity-tests-20260918-133423`](../../artifacts/unity-tests-20260918-133423/).
- Live seeded experiment: one Forest Edge/Hare seed, 600 ticks, complete report:
  [`cellular-experiment-20260918-133615`](../../artifacts/cellular-experiment-20260918-133615/).
- Live visual acceptance: 1/1 passed; the generated 1280x720 screenshot was
  visually inspected:
  [`visual-evidence-20260918-133629`](../../artifacts/visual-evidence-20260918-133629/).
- Clean seeded experiment: one Forest Edge/Hare seed, 600 ticks, complete
  report in about 25 seconds:
  [`cellular-experiment-20260918-143940`](../../artifacts/cellular-experiment-20260918-143940/).
- Final exact `CellSim.ps1 -Command Test -Mode All -Execution Clean`: EditMode
  234/234 passed; PlayMode 28 passed, 0 failed, and 2 graphics-only tests skipped:
  [`unity-tests-20260918-144956`](../../artifacts/unity-tests-20260918-144956/).
- `unity mcp configure codex --project-path ...` completed and the resulting
  configuration contains the project-pinned `mcp_servers.unity` entry only.

## Risks and incomplete work

- The main integration is committed in `37f01ddb`. This closeout captures the
  follow-up stale-test removal, clean experiment flag fix, and final workflow
  documentation. Broader project planning documents have their own separate
  pending documentation sync.
- A live warm-Editor `UI` category run passed 10/11; the profile/desktop scene
  transition assertion failed there but passed in the clean full PlayMode run.
  Treat this as a warm-state/test-isolation issue and investigate only if it
  recurs during focused live work.
- Cold Editor logs are currently very large because Unity emits extensive import
  output. The artifacts are ignored, but log-volume reduction is a useful later
  tooling improvement if disk use becomes material.
- Codex must restart or open a new task before the newly configured Unity MCP
  server becomes available to that task.

## Next useful step

Restart Codex or open a new task and run one read-only Unity MCP status/query
smoke check. For daily work, use `Auto`; use `Clean` for a handoff or release
gate. Investigate the warm `UI` isolation failure only if a second live run
reproduces it.
