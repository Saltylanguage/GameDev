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
.\tools\Submit-CellSimJob.ps1 -JobName 'hare-escape-artist-level-1' `
  -PlayerSpeciesId hare -UpgradeId escape-artist `
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

For a continuously waiting worker, omit `-Once`. Automatic pulls, commits, and
pushes should be added only after ownership, branch, and failure behavior are
agreed.

The desktop then pulls the completed record and the referenced report under
`artifacts/`. The report manifest records the source commit, Unity executable,
arguments, scenario identity, and report hash.

## Limits

This first bridge is intentionally one-way and pull-based. It does not expose a
remote shell, accept arbitrary commands, or run while Unity is open. It also
requires the two machines to use a dedicated queue branch or coordinate Git
pull/push operations so they do not edit the same files concurrently.
