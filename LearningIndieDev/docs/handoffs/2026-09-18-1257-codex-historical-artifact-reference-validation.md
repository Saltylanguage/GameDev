# historical artifact reference validation

[Working state](../WORKING_STATE.md) | Status: complete

- Handoff schema: 1
- Handoff ID: 2026-09-18-1257-codex-historical-artifact-reference-validation
- Owner: Codex
- Branch: ProjectMain
- Baseline commit: 34d16ce4
- Date: 2026-09-18
- Supersedes: none

## Summary

Adjusted the handoff validator so retired machine-local artifact files cited
by legacy notes do not produce current availability warnings. The historical
run paths remain intact as provenance, while current schema-1 handoffs still
have their artifact references checked.

## Changes

- Changed `Test-Handoffs.ps1` to check local artifact availability only in
  schema-1 handoffs; Markdown links remain checked in all notes.
- Updated the handoff README and working state to document the validation
  boundary and result.

## Decisions and assumptions

- The 102 unavailable artifact references were all in pre-schema historical
  notes; none of the current schema-1 notes had a missing artifact.
- Kept historical paths and recorded results unchanged. Their files may have
  been cleaned or may not exist in this checkout, so local availability is not
  a current handoff validity requirement for those records.
- If a future task needs raw historical data for a new claim, run and retain a
  new, explicitly scoped experiment instead of treating the old paths as live
  inputs.

## Validation

- Before the change, `Test-Handoffs.ps1 -ShowWarnings` reported 102 missing
  artifact references, all from legacy notes; schema-1 notes had zero.
- `Test-Handoffs.ps1 -ShowWarnings -TreatWarningsAsErrors` passed after the
  change with zero warnings.
- `git diff --check` passed.

## Risks and incomplete work

- Historical raw files remain unavailable locally where they were already
  missing. Their saved summaries remain historical records, not replacements
  for raw replay evidence.

## Next useful step

No further cleanup is needed unless a new research claim requires raw historical
data; in that case, run a new bounded experiment and retain its evidence.
