# workbench-setup-run-stop-resume

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-09-0201-codex-workbench-setup-run-stop-resume
- Owner: codex
- Branch: BevBranch
- Baseline commit: 87a456b0
- Date: 2026-10-09
- Supersedes: none

## Summary

CellSim Workbench now connects setup → run → review → export in one native
Windows app. The user can save readable presets, see the requested run count,
validate without simulating, deliberately Run, watch progress, stop safely and
resume the same frozen work after restarting the app. This extends the earlier
[review/export slice](2026-10-09-0141-codex-cellsim-local-workbench.md).
Everything remains local/uncommitted/unpushed on BevBranch at baseline 87a456b0;
preceding inspection/display/export work is preserved. No board changes.

## Changes

- `tools/CellSim.Workbench/RunTools.cs`: strict setup/preset validation, atomic
  preset saves, existing sweep-compiler integration, fresh experiment folders,
  exact runner pinning, eight-file identity manifest and process lifecycle.
- `tools/CellSim.Workbench/RunPage.cs`: Set up & run tab, readable settings and
  experiment notes, count preview, Validate/Run gate, log/progress, saved-run
  loading, scoped Stop/Resume and handoff to the existing Excel screen.
- `tools/CellSim.Workbench/WorkbenchForm.cs`, `Tools.cs`, `Program.cs`:
  integration with existing export/monitor, one owned operation at a time,
  close deferral until owned run cleanup, shared process output handling and
  native window sizing clamped to the working area.
- `tools/CellSim.Workbench/presets/D5-Gardeners-review.json`: Load D5 example
  proposes Gardeners × six purchase policies × four seeds = 24 runs. Uses the
  existing local neutral confirmation snapshot, wrapped 54×32, Plants325 /
  Hares30 / Foxes15, vision9, Fox starting energy160 and six100-tick rounds.
- `tools/CellSim.Batch/Program.cs`: batch accepts an invocation-specific
  `--stop-file`; cancellation follows existing owned-worker cleanup and
  aggregation checks. Shared simulation sources/rules are unchanged.
- `tools/CellSim.Batch/sweep.py`: optional explicit compiler preflight runner;
  default standalone behavior is preserved. `check.py` accepts a runner path
  so its existing integration suite can exercise the isolated Workbench build.
- `tools/Start-CellSimWorkbench.ps1`: builds the Workbench runner separately
  under `artifacts/cellsim-workbench/runner`, including shared-source freshness
  checks, and publishes the existing framework-dependent Windows EXE.
- `RunChecks.cs` adds setup/execution fixtures. `Checks.cs` restores the WinForms
  synchronization context after manual test message pumping and enables strict
  cross-thread checks. Guide/runbook/project context/working-state links updated.

## Decisions and assumptions

- Validate freezes `experiment.json`, `snapshot.json`, `spec.json`,
  `sweep/plan.json`, `sweep/sweep.json` and the runner DLL/deps/runtimeconfig.
  These exact eight files are hash-checked before execution/resume. No workers
  start during validation. Editing a scientific setting disables Run and
  requires a new experiment. Saved presets are editable; frozen evidence is not.
- Each experiment lives in a fresh timestamped `artifacts/workbench-runs/`
  folder. Keep it in place because compiled evidence uses absolute paths;
  changed/moved evidence is refused rather than silently repaired.
- Stop writes only this invocation's unique marker. The coordinator stops
  dispatch, interrupts unfinished owned work and waits for cleanup. Completed
  chunks are retained; unfinished chunks may execute again on resume. No
  arbitrary PID killing. After a crash, existing exclusive locks/claims govern
  surviving workers; normal Stop does not kill inherited workers.
- Export remains a separate deliberate action after completion. Required
  experiment expectation/success/failure notes record the user's intent; this
  tooling does not invent balance gates or approve production tuning.
- Automatic Gardeners is an existing research policy, not a definition of the
  human play style described by Bevin. The D5 example is exploratory and starts
  nothing. Its referenced local snapshot must exist to validate.

## Validation

- Release runner/GUI builds and final publish: zero warnings/errors.
- Published EXE `--run-check`:
  `artifacts/cellsim-workbench/run-checks-final.json` Passed all nine groups;
  26 final fixture rows across bounded checks, unchanged source snapshot,
  exact seed/condition coverage, no-worker validation, changed-input refusal,
  completed resume identity, stop retaining completed chunks, released worker
  locks and resume preserving their bytes, native Validate → Run → Export.
  Interrupted unfinished work can run again; 26 is final rows, not a claim of
  exact physical simulation executions across diagnostic attempts.
- Updated runner portable suite:
  `artifacts/cellsim-workbench-runner-validation-20261009/checks.json` Passed
  all twelve checks: determinism, strict inputs, dry-run, serial/parallel parity,
  replay, resume identity, controller exclusion, orphan recovery, aggregate
  interruption recovery, corrupt chunk rejection, timeout and parity refusal.
- Final published EXE `--self-test` with
  `artifacts/worksheets/validation-legacy/report.json`:
  `artifacts/cellsim-workbench/gui-regression-final.json` Passed all eleven
  existing checks, including real GUI workbook export, close guard, quoting,
  overwrite protection, batch metadata errors and native rendering. Exit0.
- Historical `tools/CellSim.Batch/bin/Release/net8.0/CellSim.Batch.dll` SHA256
  still matches assemblyHash in the completed confirmation batch metadata;
  that 96,000-run research batch was not rerun. `sweep.py` syntax check passed.
- `git diff --check` passed. The repository-wide handoff checker still reports
  the same 28 pre-existing errors in older notes; none names this handoff.
- A native test initially lost its UI synchronization context during temporary
  `Application.DoEvents` pumping. Restoring it in the test harness and enabling
  cross-thread checks resolved that failure. This does not establish the cause
  of an earlier separate native crash. Window size was also checked from the
  rendered form after DPI scaling. No Unity checks were required by this change.

## Risks and incomplete work

- Setup covers neutral frozen Plant/Hare/Fox inputs and the existing six-round
  flow. Fresh Unity snapshot generation, other species, arbitrary scientific
  axes/horizons and wider preset discovery remain future work.
- Requires Windows x64, .NET8 Desktop Runtime, source project and Python3.11+;
  Excel retains the existing Node/artifact-tool dependencies. This is a local
  project app, not an independently distributed simulation suite.
- Native file/folder picker, default Excel and Explorer interactions were not
  manually exercised. Automated controls/process/export/render checks passed;
  user acceptance of readability and daily use is still needed.
- No Unity suites, production asset/domain changes, new ecology conclusions,
  large research batch, commit/push or external board updates this turn.

## Next useful step

Open `Open-CellSim-Workbench.cmd`, load D5 example, review/edit the settings and
notes, then Validate and deliberately Run if desired. Review the completed
batch and export it. Collect Bevin's feedback before expanding supported
scenario authoring or packaging. Read [the usage guide](../CELLSIM_WORKBENCH.md)
for exact controls, stop/resume behavior and reproducible checks.
