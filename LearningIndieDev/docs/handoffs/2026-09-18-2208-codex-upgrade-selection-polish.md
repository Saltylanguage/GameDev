# Upgrade Selection screen polish

[Working state](../WORKING_STATE.md) | Status: complete

- Handoff schema: 1
- Handoff ID: 2026-09-18-2208-codex-upgrade-selection-polish
- Owner: Codex
- Branch: Production/ProjectCleanup
- Baseline commit: a4f93633
- Date: 2026-09-18
- Supersedes: none

## Summary

The phase decision overlay now presents the completed phase and Mutation
terminology directly, without exposing the run seed or tick counter in the
selection panel.

## Changes

- Added a view-model title for the completed phase, such as `PHASE 01 COMPLETE`.
- Removed the overlay's seed/tick/time details block while retaining the field
  ledger details on the main simulation view.
- Changed the selection prompt, explanatory copy, and card labels to use
  `Mutation` terminology.
- Inset the cream panel content and increased its corner radius so the olive
  border stroke is not cut off at the rounded corners.
- Added a PlayMode assertion for the phase title and included the generated
  solution ordering update in this cleanup push.

## Validation

- XAML parse and targeted static binding checks passed.
- `git diff --check` passed.
- The focused PlayMode test was not run because the shared Unity Editor was in
  `playing` state; both Clean and Live lanes correctly refused to interrupt it.
- A supplementary `dotnet build --no-restore` was unavailable because Unity's
  generated `Temp/obj` assets file was absent; this is not a Unity compilation
  result.

## Next step

Run the full PlayMode suite or exercise the phase decision flow manually after
the current Unity play session ends.
