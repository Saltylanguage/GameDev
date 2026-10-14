# cellsim-local-workbench

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-09-0141-codex-cellsim-local-workbench
- Owner: codex
- Branch: BevBranch
- Baseline commit: 87a456b0
- Date: 2026-10-09
- Supersedes: none

## Summary

Built the first local Windows CellSim Workbench as requested by Bevin. It gives
the existing exporter a persistent GUI and adds saved batch progress. The goal
is an easy human-operated setup/run/review application without AI prompts;
this slice implements export and monitoring, not simulation setup/launch.

## Changes

- `Open-CellSim-Workbench.cmd` and `tools/Start-CellSimWorkbench.ps1`: build if
  missing/stale, launch hidden-console GUI, explicit build-only option.
- `tools/CellSim.Workbench`: .NET 8 Windows Forms project, app entry, focused
  tool/process adapter, form and self-checks. No third-party .NET packages.
- Excel screen: reports/sweep folders, optional condition/seed filters and
  row/checkpoint settings, new destination, async visible log and workbook/folder
  open actions. Main action/log stay fixed while settings scroll.
- Progress screen: choose batch/parent folder, recorded state/chunks/workers,
  completed run total, 5-second refresh, stale warning, identity check, evidence
  folder and associated sweep transfer to export.
- `.gitignore` includes this project source and excludes its generated bin.
  Workbench guide, existing runbook/export guide and context indexes updated.

## Decisions and assumptions

- Use the already installed .NET 8 Windows Desktop platform for a local GUI.
  Publish a framework-dependent single-file EXE; do not add an Electron/browser
  framework or move simulation/domain logic into the form.
- Existing export script remains authoritative for evidence checks and XLSX
  behavior. Inputs are individually quoted as data before PowerShell encoding.
- Monitoring reads small metadata only. A reported Completed state is not a
  fresh hash/coverage check; missing workers are not silently treated as zero.
- Default export stays short; customization is optional. Source reports and
  existing workbooks are never rewritten. No production or scientific defaults
  changed. Existing local inspection/display/export work remains preserved.
- No new experiment or simulation run is implied by building/launching the app.

## Validation

- Release publish of `CellSim.Workbench.csproj` succeeded without warnings/errors.
  EXE: `artifacts/cellsim-workbench/CellSim.Workbench.exe`.
- Eleven focused self-checks pass, covering project/folder discovery, required source/settings,
  seeds/row/tick/path rejection, actual child PowerShell quoting/arrays/logging,
  overwrite protection, failure/stderr, running/stale/completed/identity/corrupt
  progress and native form rendering/refresh button. The native export button
  produced a workbook from existing two-record legacy evidence, displayed progress,
  completed asynchronously and deferred close while running. No simulations ran.
- Native form images were rendered and inspected; default/customization layout
  was simplified and action/log kept visible on shorter windows.
- PowerShell parser and Git whitespace checks passed; exporter regressions passed
  12/12. Repository-wide handoff validation reports 28 pre-existing failures in
  older notes (missing local artifacts, legacy statuses/fields); none references
  this handoff. Do not describe the full handoff check as passing.
- A deeper native button check initially crashed in the RichTextBox window
  procedure (Windows Application/.NET logs). The log needs plain text only, so
  it now uses TextBox. The same native button/async/close reproduction passes
  with that control. This is verified for the tested path, not a broad claim
  about all Windows control/runtime behavior. PowerShell serialization/progress
  chatter is suppressed so the activity log remains plain text.
- Current focused result: `artifacts/cellsim-workbench/checks-native-final.json`.
  Native images and timestamped integration workbooks/logs remain alongside it.

## Risks and incomplete work

- First version uses Windows x64 and installed .NET 8 Desktop Runtime. The
  project/scripts and existing Python/Node/artifact-tool export runtime remain
  required. The EXE is local tooling, not an independently packaged suite.
- Native file/folder picker and default Excel/Explorer open clicks have not
  been manually exercised. Automated native rendering/refresh and process/export
  checks do not replace hands-on usability acceptance.
- No settings persistence yet. Closing is deferred during an export; cancellation
  is not implemented. A failed export may leave an incomplete workbook and needs
  a fresh filename; it must not be treated as a successful report.
- No setup/launch/stop/resume controls, simulation or Unity tests, new tuning,
  external board update, commit or push in this turn. Work remains local on
  BevBranch baseline87a456b0.

## Next useful step

Try the app with Bevin's own reports, then add a bounded simulation setup screen
around frozen inputs and the existing sweep compiler/executor. Include readable
settings, saved presets, explicit experiment question/owner, validation/total-run
preview, deliberate Run and safe stop/resume for owned workers. Independent
runtime packaging is a separate follow-up; no new balance gates are inferred.
