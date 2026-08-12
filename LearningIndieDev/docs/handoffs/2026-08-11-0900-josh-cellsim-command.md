# Handoff: CellSim command

**Owner:** Josh / Codex
**Date:** 2026-08-11
**Branch:** `codex/unity-simulation-tooling`

## Delivered

- Replaced the interim `tools/sim` shortcut with the project-root `CellSim`
  command.
- The command surface now follows the agreed convention: `CellSim Help`,
  `CellSim Test`, and `CellSim Run -SeedCount 50`.
- `CellSim.cmd` works directly in Command Prompt; PowerShell uses
  `.\CellSim.cmd` because it requires an explicit prefix for local commands.

## Validation

Both `cmd /c CellSim Help` and `.\CellSim.cmd Help` produced the expected
usage text. The dispatcher still delegates to the existing closed-editor-safe
test and experiment commands.
