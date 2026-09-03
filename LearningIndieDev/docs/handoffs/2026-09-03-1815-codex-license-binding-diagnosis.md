# Unity license-binding and child-process diagnosis

Date: 2026-09-03

## Findings

- The Unity entitlement file exists and contains the required
  `com.unity.editor.headless` entitlement under the `UnityPersonal` group.
- A restricted-shell `Unity.Licensing.Client --showContext` probe reported
  Windows Management access denied for device, BIOS, and adapter identity
  queries. That explains the startup assertions seen in failed preflights.
- A normal-permission preflight completed the licensing IPC handshake but then
  rejected the editor license because the client requested
  `com.unity.editor.headless` and received a 404. The cached entitlement is
  machine-bound to Wi-Fi MAC `20:C1:9B:71:23:E8`; the client context probe
  selected the Bluetooth MAC `20:C1:9B:71:23:EC`. The binding mismatch is the
  durable cause of the current license failure.
- The Package Manager IPC error is downstream: Unity cannot establish a valid
  licensed editor session, so the UPM startup path never completes.

## Child-process lifecycle fix

`tools/UnityTooling.ps1` now keeps a bounded ten-second cleanup window after
Unity exits. It snapshots pre-existing `UnityPackageManager` and
`Unity.Licensing.Client` PIDs, then terminates only newly observed helpers,
including helpers spawned or re-parented during editor shutdown. A failed
preflight after this change left no active Unity, UPM, or licensing client.
The remaining zero-thread/zero-handle process records are terminated OS
objects and require no project-side cleanup.

The preflight now runs `Unity.Licensing.Client --showContext` first and fails
fast when the shell cannot read host identity data, instead of spending 30
seconds on a doomed Unity launch. A normal host-permission preflight passed
after the change (`artifacts/unity-preflight-20260903-181807`): licensing and
UPM connected and all 65 packages registered.

## Repair path

Run `tools/Test-UnityPreflight.ps1` from a normal host-permission terminal (or
approve the elevated Unity preflight when invoking it through Codex). If the
license is later refreshed or the adapter identity changes, refresh the Unity
Personal activation in Unity Hub (sign out/in or re-activate the editor). Do
not bypass Package Manager or accept a simulation result until preflight
produces a complete licensed run.
