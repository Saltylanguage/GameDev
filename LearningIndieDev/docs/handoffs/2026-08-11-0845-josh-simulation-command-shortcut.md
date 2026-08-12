# Handoff: simulation command shortcut

**Owner:** Josh / Codex
**Date:** 2026-08-11
**Branch:** `codex/unity-simulation-tooling`

## Delivered

- Added `tools/sim.cmd` as the short terminal entry point for the Unity
  simulation tools.
- Added `tools/sim.ps1` as the minimal dispatcher: `help`, `test`, and `run`.
- Documented the short commands in `UNITY_SIMULATION_TOOLING.md`.

## Validation

`cmd /c .\tools\sim.cmd help` produced the expected usage text. The shortcut
delegates to the existing scripts, so it preserves their closed-editor safety
guard and artifact behavior.
