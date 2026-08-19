# fox-reproduction-funnel

[Working state](../WORKING_STATE.md) | Status: implemented and validated on a
repeatable Forest Edge seed; balance decision remains open

- Owner: codex
- Branch: `codex/cellular-sprite-tiling`
- Baseline commit: `6bdf6af`
- Date: 2026-08-18

## Goal

Explain why a Forest Edge run recorded 322 Fox Mating behavior ticks but only
one Fox birth without changing reproduction or balance values.

## Local implementation

- Added a mutually exclusive reproduction resolver funnel with candidates,
  blocked-energy, blocked-mate, blocked-group-limit, failed-chance,
  blocked-no-birth-location, and successful-attempt counts.
- Kept births separate because one successful attempt may produce a litter.
- Added reconciliation properties so every candidate must have exactly one
  outcome.
- Added focused deterministic tests for all six outcomes and a two-birth,
  one-successful-attempt case.
- Added the funnel to Play Mode JSON/Markdown and batch JSON/Markdown reports.
- Advanced experiment reports to schema 6 and Play Mode reports to schema 5.
- Updated EX-002 forward-looking documentation to require a schema-6 rerun
  while preserving schema-5 death telemetry as historical provenance.

## Validation completed

- `git diff --check`: passed.
- `SaltyGame.Runtime.csproj`: compiled with zero errors.
- `SaltyGame.Tests.csproj`: compiled with zero errors.
- `Assembly-CSharp-Editor.csproj`: compiled with zero errors.
- `tools/New-CellSimReport.ps1`: PowerShell parser passed.
- Unity Edit Mode produced 126/134 passing tests in
  `artifacts/unity-tests-20260818-200450/EditMode-results.xml`; all six
  reproduction-funnel tests passed. Eight unrelated legacy behavior tests
  remain red in this working tree.
- Forest Edge schema-6 replay succeeded twice at seed `-877772592`:
  `artifacts/cellular-experiment-20260818-200811/report.json` and
  `artifacts/cellular-experiment-20260818-200842/report.json`. Population,
  death events, and Fox funnel counters are identical across both reports.

## Diagnostic result

Fox had 810 reproduction candidates: 410 blocked by insufficient energy
(50.6%), 341 by missing mate (42.1%), 2 by group limit, 56 by the chance roll,
0 by unavailable birth location, and 1 successful attempt. The funnel
reconciles exactly. The actionable finding is that energy and mate availability
are the dominant gates; no birth-space defect is indicated by this seed.

The runner now accepts signed deterministic seeds so the retained diagnostic
seed can be replayed directly; this does not change simulation rules or balance
values.

## Validation blockers

- A stale empty `Temp/UnityLockfile` remained after Unity closed. Codex verified
  that no Unity or Hub process existed, resolved the exact project-local path,
  and removed only that temporary lock before retrying validation.
- The elevated Unity run was required because sandboxed launches cannot read
  the machine identifiers used by the Licensing Client. Keep this as an
  environment prerequisite for future batch runs.
- Eight legacy Edit Mode tests remain red; they are unrelated to the new
  reproduction funnel and need a separate regression triage.

## Exact next actions

1. Triage the eight unrelated legacy Edit Mode failures separately.
2. Predeclare the P1-001 intervention surface without changing values yet.
3. Run the matched multi-seed EX-002 schema-6 control/intervention matrix and
   a held-out seed check.
4. Use the observed gate split to choose whether an energy/mate rule change is
   justified.

## Working-tree boundary

The reproduction work modifies simulation, metrics, tests, report tooling, and
research documentation. Existing solution and Unity ProjectSettings changes
were already present or were produced by the live Unity/editor workflow and
remain user-owned; do not include them in a focused reproduction commit without
review.
