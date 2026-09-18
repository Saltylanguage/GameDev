# Unity MCP and Pipeline operations

The current Codex integration uses the installed Unity CLI as its MCP server,
pinned to this project. The active Codex configuration is equivalent to:

```toml
[mcp_servers.unity]
command = "C:\\Users\\joshc\\AppData\\Local\\Unity\\bin\\unity.exe"
args = ["mcp", "--project-path", "D:\\GameDev\\GameDev\\LearningIndieDev"]
```

Create or refresh that entry through the CLI rather than editing it by hand:

```powershell
unity mcp configure codex --project-path "D:\GameDev\GameDev\LearningIndieDev"
```

Restart Codex or start a new task after changing MCP configuration. An existing
task does not acquire newly registered MCP tools mid-session.

## Operating model

- `com.unity.pipeline` `0.7.0-exp.1` runs inside the project Editor and exposes
  its command registry.
- `unity status --project-path <path>` identifies the exact project state.
- `CellSim -Execution Live` uses a ready connection for focused tests, visual
  checks, and experiments without launching another Editor.
- `CellSim -Execution Clean` uses a CLI-managed batch Editor for acceptance
  evidence when the project is closed.
- `CellSim -Execution Auto` chooses between those paths. Busy, Safe Mode, and
  unreachable locked states are reported instead of being force-cleaned.

For direct CLI commands, put global command attribution before the command name:

```powershell
unity command --caller plugin --skill unity-cli `
    --project-path "D:\GameDev\GameDev\LearningIndieDev" `
    --format json <command> <command-arguments>
```

The project wrappers avoid `--no-pager` because Unity CLI `1.0.0-beta.6`
currently rejects it even though it appears in help output.

## Legacy relay note

The former Codex `unity_mcp` entry used user relay executables and required
manual relay-count diagnosis. It has been removed from the active configuration.
`tools/Get-UnityMcpRelayHealth.ps1` remains only for diagnosing the separate
Unity AI Assistant package or reading historical handoffs; it is not a required
step in the current test or experiment workflow. Do not terminate relay or Unity
processes based on a count alone. Identify the owning project and invocation
before cleaning up a confirmed stale process.
