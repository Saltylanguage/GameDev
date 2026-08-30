# Remote CellSim worker

This is a low-risk desktop-to-mini-PC bridge for simulation experiments. The
desktop creates a small JSON job; the mini PC polls the queue and runs the
existing `CellSim` batch runner. Completed or failed job records are returned in
the repository, preserving the normal Git and provenance workflow.

## Setup

Use a dedicated branch or clean checkout on the mini PC. Do not run the worker
against a working tree with unsaved Unity changes. Unity must be closed on the
mini PC while a job runs.

From the desktop, submit a job through a temporary worktree. The helper fetches
the worker branch, creates the pending JSON there, commits only that JSON, and
pushes the commit. It never checks out or edits the desktop branch.

```powershell
.\tools\Submit-RemoteCellSimJob.ps1 -JobName 'hare-escape-artist-20-seeds' `
  -SeedStart 1 -SeedCount 20 -PlayerSpeciesId hare `
  -UpgradeId escape-artist -ExperimentalFeatures bev-experimental `
  -UpgradeValueOverride 0.5
```

For a safe local check that generates and validates the JSON but does not
commit or push, add `-DryRun` to the same command.

On the mini PC, pull and run one job:

```powershell
git pull
.\tools\Start-CellSimWorker.ps1 -Once
git add automation/CellSimQueue/Completed automation/CellSimQueue/Failed artifacts
git commit -m 'Complete CellSim hare experiment'
git push
```

For a continuously waiting worker, omit `-Once`. The desktop helper uses a
non-forcing push, so a concurrent worker-branch update fails closed and leaves
the desktop branch untouched.

The desktop then pulls the completed record and the referenced report under
`artifacts/`. The report manifest records the source commit, Unity executable,
arguments, scenario identity, and report hash.

## Limits

This first bridge is intentionally one-way and pull-based. It does not expose a
remote shell, accept arbitrary commands, or run while Unity is open. It also
requires the two machines to use a dedicated queue branch or coordinate Git
pull/push operations so they do not edit the same files concurrently.
