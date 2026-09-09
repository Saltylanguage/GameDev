# Handoff — Dual Noesis views for the GalapagOS Simulation route

**Date:** 2026-09-06  
**Owner:** Codex  
**Branch:** `codex/simulation-window-production`  
**Status:** Local, uncommitted UI integration

## Decision

The GalapagOS desktop and the Eco Simulation now use separate cameras and
separate `NoesisView` components. The desktop view remains bound to
`VM_GalapagOS_Desktop`; the simulation view is bound to `VM_SimulationShell`
and owns the real `V_Panel_SimulationShell` visual tree.

The previous single-view approach attempted to replace `NoesisView.Xaml` at
runtime. That did not rebuild the mounted Noesis visual tree, so the desktop
XAML remained visible while receiving the simulation view-model. This caused
the generic desktop-app skeleton and binding errors shown in the test scene.

Opening Simulation now disables the desktop camera/view and enables the
simulation camera/view. Closing Simulation reverses that composition. The
simulation remains board-first with its existing setup overlay; opening it
does not start or advance a run.

The desktop command now treats Simulation as a dedicated route. It will not
fall through to the generic desktop-app placeholder if the launcher binding
is missing. A failed board lookup also rolls the composition back to the
desktop instead of leaving an empty simulation view active.

The desktop Noesis view also explicitly receives `VM_GalapagOS_Desktop` as
its `DataContext` during host startup. Without that assignment, the desktop
bindings were unresolved: the placeholder window remained visible by default
and desktop icon commands were not available to the UI.

## Changed files

- `Assets/UI/GalapagOS/Scripts/GalapagOSDesktopNoesisHost.cs` — manages the
  two view compositions, wires the simulation board to its board VM, and
  initializes the desktop DataContext, and safely rolls back incomplete opens.
- `Assets/UI/GalapagOS/Scripts/VM_GalapagOS_Desktop.cs` — routes Simulation
  exclusively through the simulation launcher rather than the placeholder
  desktop-app window.
- `Assets/Scenes/GalapagOSDesktopTest.unity` — adds the disabled child camera
  and Noesis view for the simulation XAML, with stable scene references.

## Verification

- Scene references and new file IDs were checked statically.
- Desktop, simulation, and shared-resource XAML files parse successfully.
- `git diff --check` passes aside from normal LF/CRLF conversion warnings.
- A fresh Unity Play Mode session was started and the Simulation command was
  invoked directly. Runtime state confirmed the desktop camera was disabled,
  the simulation camera and Noesis view were enabled, and the board VM had a
  non-null snapshot.
- A fresh Play Mode session after the DataContext fix confirmed the desktop
  command was initialized and the Simulation command switched cameras.
- Unity console logs contained no simulation errors after the fresh route
  check. The editor camera-capture tool does not capture the Noesis overlay,
  so final visual acceptance still requires checking the actual Game view.
