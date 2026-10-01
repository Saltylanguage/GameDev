# P1-026: CellSim worker Unity lifecycle safety

Date: 2026-10-01
Owner: Codex
Branch: `codex/p1-026-worker-lifecycle`, based on worker commit `ba92cb03`

## Change

The remote worker branch still uses the older batch-based Unity wrappers. This
change ports the process cleanup and licensing-context preflight from the
2026-09-03 implementation into `tools/UnityTooling.ps1`, while preserving the
worker branch's existing command interface. The tool now:

- refuses to remove a project `Temp/UnityLockfile` automatically;
- snapshots pre-existing Package Manager and licensing-client PIDs before launch;
- uses `taskkill /T /F` while the started Unity parent is live, then falls back
  to `Stop-Process` and reports an unverified tree cleanup as an error;
- runs a bounded 10-second cleanup for newly observed helpers in `finally`;
- checks licensing host context before invoking the batch preflight.

The process-tree order corrects a gap in the 2026-09-03 version: stopping the
parent before `taskkill /T` could leave descendants alive.

## Verification

- PowerShell parser: pass.
- `git diff --check`: pass.
- Synthetic PowerShell parent/child tree: both PIDs terminated with normal host
  permissions. The first sandboxed run returned `taskkill: Access denied`, so
  this check was repeated with host permissions; only the synthetic PIDs were
  targeted.
- `Invoke-UnityBatch` with a synthetic executable: exit 0 and exit 7 paths
  passed, including the original nonzero exit report.
- A mock project lockfile stayed present after the guard rejected the run.

No Unity Editor, queued experiment, or remote mini-PC worker run was started.

## Integration risk and next step

`codex/cellsim-worker` is 144 commits behind `ProjectMain`. It still contains
pending job `20260903-033240-b1b43c58` (`p3-ex007-baseline-heldout`), whose
scenario path is absent on `ProjectMain`. A full fast-forward would change its
simulation provenance and make that path invalid. This focused backport keeps
that job and its source revision intact.

The Pipeline CLI/state-routing migration remains separate: it would require the
new package, wrappers, CLI availability on the mini PC, and a decision about
this pending job. The backport was pushed to `origin/codex/cellsim-worker` at
`a227f118`. P1-026 stays open until the worker host confirms cleanup and the
Pipeline CLI/state-routing migration is addressed.
