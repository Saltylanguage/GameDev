# CellSim live progress dashboard

October 8, 2026. Codex / GPT-6; BevBranch. Bevin requested a visual dashboard
for the ongoing confirmation. Implemented a read-only local Python standard-
library HTTP server and browser page; no runtime dependencies, runner changes,
scientific input changes or simulation restart.

Live URL: `http://127.0.0.1:59057/`. Run root:
`artifacts/cellsim-hare-confirmation-20261008-143725/`.
Server details live in `dashboard-server.json`; separate stdout/stderr logs.
The tab is marked as a deliverable to remain open after the turn.

`tools/CellSim.Batch/dashboard.py` serves only `/` and `/status` on loopback.
It reads frozen sweep cases, plan, pipeline status, batch status, completion
metadata and latest worker-attempt statuses. Completed chunks already validated
by the runner count; partial worker output is excluded. Metadata identity,
condition, offset, chunk size and coverage limits are checked. Raw simulation
reports are not read or rehashed by the dashboard. Immutable completion reads
are cached, but counts are rebuilt from current files at each refresh.

`dashboard.html` refreshes every five seconds: overall runs/chunks, configured
and reported workers, elapsed time, average throughput, simulation ETA, 96
matrix cells and active chunks (PID, candidate, path, policy, seeds, chunk age).
Matrix uses numbers and labels alongside fill/active markers; responsive narrow
views scroll horizontally. It flags source status older than 30 seconds and
disconnections; worker records are not separately checked for process liveness.
Simulation, analysis/reports and final completion are distinct stages. ETA
excludes analysis and inherits the runner's invocation timing/resume caveat.

Validation: `python tools/CellSim.Batch/check_dashboard.py` passed fixtures for
pending/active/completed/tail chunks, ETA, stale status, failed latest attempts,
analysis lifecycle, deleted cached completion handling and foreign identity
rejection. Live endpoint returned 96 cells and 16 workers. Browser showed
20,000/96,000 (20.8%) with all 96 cells rendered; manual refresh changed the
update timestamp; no browser error logs. Five-second refresh was also observed.
Live HTTP checks cover root/status and rejection of other paths. Screenshot:
`<run-root>/dashboard-preview.png`. The simulation continues independently.

Restart instructions are in `docs/CELLSIM_STANDALONE_RUNBOOK.md`. Stop only the
PID recorded in dashboard-server.json to stop this dashboard; closing it does
not stop the batch. No auto-start, notification or external hosting was added.
No Unity validation needed for this metadata-only tool; no commit/push requested.
