# Standalone CellSim work published without interrupting the confirmation

October 8, 2026. Codex / GPT-6. Bevin explicitly requested pushing all completed
work while preserving the active simulation. Main work commit:
`bada43ad66b2959932e956332cf7e5f5e7bd6b33`
(`Add standalone CellSim sweeps, Hare purchases and live progress dashboard`).
Pushed non-forcefully to `origin/BevBranch` in Saltylanguage/GameDev. The remote
advertised SHA, local HEAD and remote-tracking SHA were verified equal afterward.
This handoff is a subsequent documentation checkpoint for that publication.

## Shared scope

74 files in the main commit: standalone .NET 8 runner and snapshot export,
shared compact telemetry and Hare purchase harness, focused Unity test, trajectory-
aware summarizer, sweep generation/analysis, local progress dashboard and fixtures,
protocols/matrices/scripts, runbooks, completed results and handoffs. Player Hare
price remains 10. Production scenario tuning was not changed by publication.

Completed study evidence is indexed at
`docs/Research/Results/2026-10-08-CellSim-Sweeps/README.md`:
32,400-run grid, 250,000-run grid and 12,800-run purchase screen. Compressed
archives contain full metric distributions, paired deltas, snapshots/plans and
derived reports; the purchase screen includes all 64,000 assigned-window rows.
The shared confirmation files are immutable inputs and diagnostics of the prior
screen, not active confirmation results. Package total approximately 103 MB.
The quarter-million archive is 86,181,263 bytes (82.19 MiB); GitHub accepted it
with a warning above its recommended 50 MB size. No LFS migration was performed.

Raw per-run/chunk outputs, duplicate SQLite tables, large logs, binaries/caches
and active outputs stay local in ignored `artifacts/`. Original provenance is
retained verbatim, including historical source commits/dirty-tree state. The
publication commit does not retroactively become an experiment's source commit.
Per-entry/ZIP hashes, CRCs, copied bytes and analysis output hashes were verified.
Scoped `.gitattributes` preserves evidence bytes and recorded hashes across Git
line-ending conversion; original NUnit XML diagnostic whitespace is retained.

## Validation and ongoing run

Fresh isolated Release build: zero warnings/errors. Portable self-test passed;
fresh assembly is byte-identical to the active confirmation's frozen copy.
Dry-run with the active frozen plan returned the identical batch identity.
Sweep-analysis/held-axis/coverage/cohort/integrity regression passed on the
completed 128-arm smoke evidence, without rerunning simulations. Dashboard
fixtures passed. Staged whitespace checks passed; staged archives and immutable
confirmation files matched their recorded hashes; no raw/cache/live paths staged.
No new Unity test run: retain the preceding 259-test and purchase-specific two-
test EditMode XML as historical evidence, plus reference/control checks in the
runbooks. Full PlayMode/gameplay acceptance remains outside these checks.

The active batch retains identity
`036565a25ed21c1853ad35fec8b3ee8c2474b8dc2114c21b0e29d65e3f067087`
and root `artifacts/cellsim-hare-confirmation-20261008-143725/`.
Its original pipeline PID 62072 and dashboard PID 44992 remained alive.
Validated count advanced from 27,200 at the initial check to 30,000 after the
main commit/push, with 16 workers active and simulation error log empty.
Every pinned snapshot/spec/plan/runner/analyzer hash was rechecked unchanged.
No simulation stop, restart, export or frozen-file rewrite occurred.

The local dashboard remains `http://127.0.0.1:59057/`. Git operations change
publication metadata; they do not change that process's loaded code or the
batch's copied DLL. Inspect actual status for later progress/completion.

## Remaining boundaries

The active confirmation results will require their own completion review and
publication once validated. Raw output is preserved locally rather than backed
up by this Git push. Research scores/survival do not establish production
balance, enjoyment or the open tick-700 viability criterion.

`COLLABORATION_WORKFLOW.md` requests corresponding Trello synchronization after
a push. No Trello write was made in this scope: no relevant callable connector
is available here, and the user authorized Git publication rather than sending
an external board update. That board follow-up remains outstanding; Git
publication, frozen-input integrity and continued execution are verified.
