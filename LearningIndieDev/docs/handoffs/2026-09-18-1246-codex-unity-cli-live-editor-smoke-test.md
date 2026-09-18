# Unity CLI live Editor smoke test

[Working state](../WORKING_STATE.md) | Status: complete

- Handoff schema: 1
- Handoff ID: 2026-09-18-1246-codex-unity-cli-live-editor-smoke-test
- Owner: Codex
- Branch: ProjectMain
- Baseline commit: 34d16ce4
- Date: 2026-09-18
- Supersedes: none

## Summary

Verified that the installed Pipeline package connects the project Editor to
the Unity CLI and exposes live commands. The check did not modify game scenes
or assets. The Editor process was closed after the check.

## Changes

- Updated [`UNITY_PLUGIN_WORKFLOWS.md`](../UNITY_PLUGIN_WORKFLOWS.md) with the
  verified live-connection steps and shutdown result.
- Updated [`PROJECT_CONTEXT.md`](../PROJECT_CONTEXT.md) to explain the
  installed package's live Editor capabilities and the smoke-check evidence.
- Updated [`WORKING_STATE.md`](../WORKING_STATE.md) to link this follow-up
  evidence alongside the package-install and test record.

## Decisions and assumptions

- The smoke check used the project at
  `D:\GameDev\GameDev\LearningIndieDev`, opened with Unity CLI on Unity
  `6000.4.6f1`.
- A ready Pipeline connection and a live command catalog demonstrate that
  Unity CLI can reach and query this project's Editor. They do not validate
  every project skill or every possible Editor command.
- No scene or asset mutation was needed to verify the connection.

## Validation

- `unity status --project-path "D:\GameDev\GameDev\LearningIndieDev"`
  reported one connected instance in `ready` state on Unity `6000.4.6f1`.
- `unity command --caller plugin --skill unity-cli --project-path
  "D:\GameDev\GameDev\LearningIndieDev" --limit 25` succeeded and reported
  151 registered commands from the Pipeline server.
- The initial close request via `CloseMainWindow()` returned `false` because
  the process had no Windows main-window handle.
- A Pipeline `eval` requested `UnityEditor.EditorApplication.Exit(0)`. The CLI
  reported `COMMAND_FAILED` because Unity shut down before it could return a
  valid response. The exact Editor PID (`77240`) then no longer existed, and a
  fresh `unity status` reported zero connected instances. This confirms the
  process exited; the CLI did not return a successful eval response.

## Risks and incomplete work

- The smoke check proves live connection and command discovery only. It did
  not exercise scene edits, asset changes, builds, or the domain-specific
  Unity skills.
- The broad test state remains as recorded in the package handoff; this smoke
  check does not change or reclassify those test results.

## Next useful step

Use the documented Unity skill routing and live CLI connection on the next
concrete Editor task. Continue using the guarded test wrapper with the Editor
closed for project test runs.
