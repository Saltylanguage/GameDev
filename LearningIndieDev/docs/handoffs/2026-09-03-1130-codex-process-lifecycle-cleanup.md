# Unity batch process lifecycle cleanup

Date: 2026-09-03

## Change

`tools/UnityTooling.ps1` now owns the lifecycle of processes started by
`Invoke-UnityBatch`:

- The helper snapshots pre-existing `UnityPackageManager` and
  `Unity.Licensing.Client` PIDs before launch.
- A `finally` block stops the Unity process tree it started and terminates only
  newly observed UPM/licensing helpers.
- Root and child cleanup use `Stop-Process` first, with `taskkill` as a process-
  tree fallback; failures are reported at verbose level rather than hidden.
- Existing Unity Hub licensing services are not targeted.

## Verification

- PowerShell parser check passed.
- A long-running test process was started through the helper's cleanup path and
  was confirmed absent afterward (`ProcessTreeCheck=PASS`).
- No Unity process is running after the check.

The two helper records tied to the latest probes (`37128` and `91400`) were
also targeted explicitly. They remain visible only as zero-thread/zero-handle
records, confirming that they are terminated process objects rather than
running clients; a reboot is required to reap those OS records.

This closes the previous lifecycle gap where a failed Unity launch could leave
helper processes behind. The existing Unity/Hub IPC failure remains a separate
machine-state issue; this change does not bypass the preflight evidence gate.

The lifecycle change is currently local to `UI/ControlLibrary`; the detached
`codex/cellsim-worker` branch must receive the tool commit before the next
remote worker run, or that worker will still use its older cleanup behavior.
