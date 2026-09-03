# Handoff — CellSim evidence-quality gate and repeated control

**Date:** 2026-09-02 23:32
**Owner:** Codex
**Branch:** `UI/ControlLibrary`
**Worker branch:** `codex/cellsim-worker`
**Status:** Evidence gate implemented; repeated control jobs queued and awaiting worker pickup.

## Completed

- `Run-CellularExperiment.ps1` now records the source commit plus explicit
  before/after source-tree state in the manifest.
- Report checksums use canonical UTF-8 text with LF line endings, so Git's
  Windows checkout normalization cannot create a false mismatch.
- `Start-CellSimWorker.ps1` now refuses to publish an incomplete result and
  requires `report.json`, `report.csv`, `statline.csv` for experimental Hare
  runs, `manifest.json`, and `unity.log`. It verifies `reportSha256` before
  copying the completed bundle.
- `Test-CellSimArtifactBundle.ps1` provides a read-only bundle validator and a
  strict `-RequireUnityLog` mode for new evidence.
- `Submit-RemoteCellSimJob.ps1` now captures Git's successful worktree progress
  without treating stderr progress as a terminating PowerShell error; native
  exit codes still fail the submission.

## Commits

- `f315d3d6` — evidence-gate tooling/docs on `UI/ControlLibrary`.
- `03011357` — same worker-side contract on `codex/cellsim-worker`.
- `022ea769` — submission-wrapper fix and loose-ends ledger update on
  `UI/ControlLibrary`.

## Validation

- PowerShell parser checks pass for the runner, worker, validator, and remote
  submission wrapper.
- Historical baseline bundle
  `automation/CellSimQueue/Completed/20260831-234216-ec3350ed` validates
  `VALID_WITH_WARNINGS`; its only warning is that the old manifest predates
  explicit before/after source-tree fields.
- Historical Escape Artist bundle
  `automation/CellSimQueue/Completed/20260831-234200-d484a2b2` correctly
  validates `INVALID` because its old package lacks `report.csv` and
  `statline.csv`. The validator no longer reports a checksum mismatch after
  canonical line-ending handling.
- Two older 5-seed Escape Artist reports
  (`20260831-135702-a383c7cb` and `20260831-223509-e77de2a6`) have matching
  normalized runs and summaries, providing a small deterministic repeat check
  but not a promotion-quality sample.

## Fresh repeated control

The following identical jobs were submitted with Forest Edge defaults, Hare,
no upgrade, `bev-experimental`, and seeds 1–20:

- `20260902-233024-d8d75c20` (`step1-hare-baseline-a`)
- `20260902-233045-437566fd` (`step1-hare-baseline-b`)

As of this handoff both remain in `Pending` on the worker branch; no worker
poll has moved them to `Running` or `Completed`. Do not begin upgrade-effect
promotion from the historical incomplete Escape Artist package. Once a worker
processes the jobs, run `Test-CellSimArtifactBundle.ps1 -RequireUnityLog` on
both directories, compare normalized outcomes, then use the accepted control as
the baseline for the next predeclared research arm.

## Re-entry commands

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Test-CellSimArtifactBundle.ps1 `
  -ArtifactDirectory .\automation\CellSimQueue\Completed\<job-id> `
  -RequireUnityLog
```

The two untracked Unity recovery files under `Assets/_Recovery/` were not
modified or staged.
