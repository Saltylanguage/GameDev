# Remote CellSim worker

This is a low-risk desktop-to-mini-PC bridge for simulation experiments. The
desktop creates a small JSON job; the mini PC polls the queue and runs the
existing `CellSim` batch runner. Completed or failed job records are returned in
the repository, preserving the normal Git and provenance workflow.

## Setup

Use a dedicated branch or clean checkout on the mini PC. Do not run the worker
against a working tree with unsaved Unity changes. Unity must be closed on the
mini PC while a job runs.

From the desktop, submit a job and push the queue file:

```powershell
.\tools\Submit-CellSimJob.ps1 -JobName 'hare-threat-exposure-level-1' `
  -PlayerSpeciesId hare -UpgradeId threat-exposure `
  -ExperimentalFeatures bev-experimental -UpgradeValueOverride 0.5
git add automation/CellSimQueue/Pending
git commit -m 'Queue CellSim hare experiment'
git push
```

On the mini PC, pull and run one job:

```powershell
git pull
.\tools\Start-CellSimWorker.ps1 -Once
git add automation/CellSimQueue/Completed automation/CellSimQueue/Failed artifacts
git commit -m 'Complete CellSim hare experiment'
git push
```

For a continuously waiting worker, omit `-Once`. The hardened worker supports
explicit unattended mode:

```powershell
.\tools\Start-CellSimWorker.ps1 -AutoSync -AutoPublish -PollSeconds 15
```

`-AutoSync` performs an `ff-only` pull only from a clean checkout. `-AutoPublish`
commits and pushes only queue records and packaged report files. Both switches
are opt-in and should be tested with `-Once` before registering a Windows
startup task.

The desktop then pulls the completed record and its tracked result bundle. Each
completed job contains `report.json`, the existing per-seed `report.csv`, the
per-seed `statline.csv` for `bev-experimental` Hare runs, and `manifest.json`.
The stat-line CSV includes `SPO`, `HPS`, `EHS`, `ECN`, `PREY`, `STRV`, `MAT`,
`BIR`, `CRWD`, `FPO`, `pAVI`, `eAVI`, `predAVG`, `sAVI`, `cAVI`, `bAVG`,
`RFS`, and `APS`, plus their validity-status columns. The report manifest
records the source commit, Unity executable, arguments, scenario identity, and
report hash.

## Limits

The bridge is intentionally one-way and pull-based. It does not expose a remote
shell, accept arbitrary commands, or run while Unity is open. The worker refuses
to process jobs when its checkout starts dirty, keeps the tracked Pending job in
place while Unity runs, records pre-run and post-cleanup tree state, restores
only known Unity-generated paths, and packages the report/manifest beside the
completed job. It requires the worker checkout to remain dedicated to this
branch.
