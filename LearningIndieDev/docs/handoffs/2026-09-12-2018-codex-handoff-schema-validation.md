# Handoff schema validation

[Working state](../WORKING_STATE.md) | Status: ready-for-review

- Handoff schema: 1
- Handoff ID: 2026-09-12-2018-codex-handoff-schema-validation
- Owner: Codex
- Branch: codex/simulation-window-production
- Baseline commit: 993a8440
- Date: 2026-09-12
- Supersedes: none

## Summary

New handoffs now carry a stable ID and replacement metadata, and a lightweight
validator checks their required fields, local links, and local artifact
references. Historical notes remain unchanged.

## Changes

- Updated the handoff generator with schema, ID, and `Supersedes` fields.
- Added `tools/Test-Handoffs.ps1`.
- Documented the schema and validation command in the handoff journal README.
- Added the first schema-1 handoff as an integration fixture.

## Decisions and assumptions

- Schema 1 is strict only for newly marked notes; older notes receive warnings
  instead of requiring a bulk migration.
- Missing local artifacts are warnings because ignored evidence may not exist
  on every checkout.
- Detailed warnings are opt-in so the routine check stays readable.

## Validation

- PowerShell parser checks passed for `New-Handoff.ps1` and
  `Test-Handoffs.ps1`.
- `Test-Handoffs.ps1` passed across 116 notes, including this first schema-1
  note. It reported 93 unavailable legacy artifact references as warnings.

## Risks and incomplete work

- Historical artifact references have not been repaired or classified.
- The validator checks local existence, not whether an artifact is the correct
  evidence for a claim.

## Next useful step

Use schema 1 for new handoffs and classify the reported historical warnings as
later cleanup work rather than rewriting the journal now.
