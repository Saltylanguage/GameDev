# Handoff — GalapagOS desktop icon control

**Date:** 2026-09-05  
**Owner:** Codex  
**Branch:** `codex/simulation-window-production`  
**Status:** Local, uncommitted UI implementation

## Decision

GalapagOS desktop icons use a reusable Button style plus a Noesis behavior.
Icons are positioned as children of a Canvas. A single click arms the icon for
dragging, dragging updates its Canvas position, release snaps to the configured
grid, and only a double-click invokes the attached `ICommand`.

The Canvas requirement is intentional for the first desktop slice. Add a
layout adapter only if icons later need to live in a Grid or WrapPanel.

The desktop launcher column now presents Field Notes, Gene Lab, Simulation,
Biome Data, My Collection, and Settings vertically. Double-clicking a launcher
opens one shared GalapagOS application-window skeleton with app-specific copy,
except Simulation, which switches the desktop's single Noesis view to the real
simulation shell and board. The simulation shell can close back to the desktop
at its ready/results boundary. The dedicated Settings surface is the next
feature pass. The taskbar Start control is a single clean left-anchored element
with the sprout mark.

## Changed files

- `Assets/UI/GalapagOS/Scripts/GalapagOSDesktopIconBehavior.cs` — attached
  command properties and double-click/drag/snap behavior.
- `Assets/UI/GlobalResources.xaml` — reusable `GalapagOS.DesktopIcon` style.
- `Assets/UI/GalapagOS/XAML/Panels/V_Panel_GalapagOS_Desktop.xaml` — six
  draggable desktop launchers, the simplified Start control, and the shared
  app-window skeleton.
- `Assets/UI/GalapagOS/Scripts/VM_GalapagOS_Desktop.cs` — desktop app open/close
  state, app-specific skeleton copy, simulation-launch callback, and taskbar
  panel coordination.
- `Assets/UI/GalapagOS/Scripts/GalapagOSDesktopNoesisHost.cs` — switches the
  single Noesis view between the desktop shell and the real simulation shell.
- `Assets/UI/HUD/Scripts/VM_SimulationShell.cs` — supports a desktop-local close
  callback while preserving the existing Lab scene close behavior.
- `Assets/Scenes/GalapagOSDesktopTest.unity` — carries the simulation preview,
  scenario runtime, shell VM, board VM, and authored visual asset references.

## Verification

- Both changed XAML files parse successfully as XML.
- Static checks confirm double-click gating, command `CanExecute`, mouse
  capture, drag threshold, Canvas movement, grid snapping, and parent bounds.
- `git diff --check` passes aside from normal LF/CRLF conversion warnings.
- The elevated Unity visual-evidence run completed with 18/18 Play Mode cases
  passing and no C# compilation or Noesis/XAML import errors in its log.
- That harness does not directly drive the GalapagOS desktop test scene, so
  manual double-click/drag visual acceptance remains pending.
- A new Unity validation attempt was blocked because an editor instance was
  already running; the harness requires that editor to be closed first.

## Follow-up — taskbar shell

The desktop test surface now includes the requested taskbar contract:

- Start opens a flat 13-app GalapagOS menu with one-click app commands.
- The right utility cluster exposes volume controls, unread notifications,
  day/date/time, and game-speed selection.
- Utility flyouts are mutually exclusive and remain UI-only demo state until
  the relevant audio, notification, clock, and simulation helpers are wired.

Static XML/resource checks and the live Unity editor import are clean. The full
automated suite was not rerun because the repository tooling detected an
already-running Unity editor and intentionally refuses to close it.
