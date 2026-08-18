# Unity MCP relay operations

This project uses two different relay roles:

- The Unity AI Assistant package owns one relay under `Library/PackageCache`.
- Codex owns the user relay at `C:\Users\<user>\.unity\relay\relay_win.exe`.

The active Codex configuration contains one `unity_mcp` server entry. A fresh
restart of the current Codex app-server was observed to create three Codex user
relays and one Unity Assistant package relay. Treat more than three Codex user
relays as accumulation until a different normal baseline is established.

## Check before working

From the repository root, run:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\LearningIndieDev\tools\Get-UnityMcpRelayHealth.ps1
```

The checker is read-only. It reports process IDs, start times, memory, and the
relay source. By default it warns above three Codex user relays; pass
`-WarnUserRelayCount` to change that threshold. It never terminates anything.

## Safe operating procedure

1. Keep only the Codex-to-Unity connections you actually need for this editor.
2. Avoid repeated revoke/reconnect cycles while Codex remains open.
3. If a reconnect is needed, close Codex first, then reconnect once.
4. Run the health check again and record any repeated relay accumulation.

Do not terminate relay processes while Codex or Unity is still using them unless
the process has been identified as stale and the user has approved cleanup.

## Current diagnosis

The duplicate user relays observed on 2026-08-16 were all children of the
Codex `app-server` process, while the package relay was a child of Unity. A
clean Codex restart recreated three user relays within seconds, and repeated
MCP calls did not increase that count. This establishes three as the current
observed baseline; growth beyond it should be captured as a possible Codex
relay-lifecycle issue rather than a Unity project-configuration problem.
