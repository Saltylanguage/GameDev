# workbench-readable-dropdowns

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-10-09-2356-codex-workbench-readable-dropdowns
- Owner: codex
- Branch: BevBranch
- Baseline commit: 6da9ba06
- Date: 2026-10-09
- Supersedes: none

## Summary

Fixed the unreadably narrow comparison dropdown shown in Bevin's screenshot.
The selected value, open list and adjacent comparison entry now have usable
width. This is local work on top of the current merged BevBranch baseline.

## Changes

- RunSetupView.cs: dock the picker and value entry across their table column,
  with minimum widths. Measure all dropdown labels using the current font when
  opening, allow room for the scrollbar and bound the list to twelve entries.
- Read-only comparison setting cells keep their label binding but hide the
  misleading dropdown button and wrap long labels.
- Checks.cs: default/compact window and larger-text checks cover all thirty
  labels, open-list width/height, value entry width and read-only display style.

## Decisions and assumptions

- Inventory found one actual ComboBox and one read-only DataGridView combo
  column in the Workbench. Existing binding, values and selection behavior stay
  intact; no new widgets, dependencies or simulation behavior are introduced.
- Preserve the separately merged remote-worker work already at baseline.

## Validation

- New regression fails on the prior picker build; passes after the layout fix.
- Release publish passed without warnings/errors. Native self-test: thirteen
  groups passed; final results at artifacts/cellsim-workbench-dropdown-check/
  final-checks.json. No real export or simulation runs in this check.
- Actual 144 DPI default1920x1350 and compact1500x1200 open-list images inspected;
  larger-font cases also checked. Screenshots and test outputs remain ignored.
- Visual inspection caught native list height ignoring MaxDropDownItems alone;
  explicit pixel height based on current ItemHeight fixes the tall list.
- Source/document whitespace checks passed.

## Risks and incomplete work

- Ordinary human acceptance remains pending. Live monitor-to-monitor DPI
  transitions were not exercised. No Unity tests or ecology experiments needed
  for this native Workbench layout change.
- Source is uncommitted/unpushed; the earlier publication remains in Git history.

## Next useful step

Use Compare values in the rebuilt local Workbench and check the picker against
the screenshot. Commit/push this follow-up when requested.
