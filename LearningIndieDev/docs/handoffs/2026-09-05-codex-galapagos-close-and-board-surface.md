# Handoff — GalapagOS close behavior and simulation board surface

**Date:** 2026-09-05  
**Owner:** Codex  
**Branch:** `codex/simulation-window-production`  
**Status:** Local, uncommitted UI integration

## Decision

The default GalapagOS Lab window is now a closable desktop surface. Its
visibility is owned by `VM_GalapagOS_Desktop`, and the window shell uses the
shared close-command attached property.

The Start menu remains a toggle: clicking Start again closes it. When the menu
is open, a transparent dismiss layer covers the desktop area above the taskbar;
clicking outside the menu closes it without adding code-behind input handling.

The real `SpeciesSimulationBoard` remains the simulation shell's presentation
surface in every preview state, including Ready. The Ready setup card is now
left-aligned so the seeded board is visible as soon as Simulation opens. The
existing Start command still owns the transition into a running expedition;
opening a window does not start or advance the simulation.

## Changed files

- `Assets/UI/GalapagOS/Scripts/VM_GalapagOS_Desktop.cs` — Lab window close
  command/state and visibility notifications.
- `Assets/UI/GalapagOS/XAML/Panels/V_Panel_GalapagOS_Desktop.xaml` — Lab close
  binding and outside-click Start-menu dismiss layer.
- `Assets/UI/HUD/Scripts/VM_SimulationShell.cs` — keeps the board surface
  visible while setup, active, decision, reward, and results overlays render.
- `Assets/UI/HUD/XAML/V_Panel_SimulationShell.xaml` — positions the setup card
  to expose the board on first open.

## Verification

- Both changed XAML files parse successfully as XML.
- `git diff --check` passes aside from normal LF/CRLF conversion warnings.
- Unity runtime validation remains pending because the existing Unity editor
  process is open and the project harness intentionally refuses to close it.
