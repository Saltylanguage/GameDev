# Common-contact representativeness evidence manifest

## Tracked records

- Design: `docs/handoffs/2026-08-21-codex-common-contact-representativeness-design.md`
- Final handoff: `docs/handoffs/2026-08-21-codex-common-contact-representativeness.md`
- Analysis tool: `tools/Analyze-CellSimOpportunityRepresentativeness.ps1`

## Fresh paired reports

Calibration seeds `10100-10119`:

- Baseline: `artifacts/cellular-experiment-20260821-031616/report.json`
- Block+2: `artifacts/cellular-experiment-20260821-031717/report.json`
- Comparison: `artifacts/cellular-experiment-20260821-031717/comparison.md`

Held-out development seeds `10125-10144`:

- Baseline: `artifacts/cellular-experiment-20260821-031803/report.json`
- Block+2: `artifacts/cellular-experiment-20260821-031846/report.json`
- Comparison: `artifacts/cellular-experiment-20260821-031846/comparison.md`

Each report contains `runs[*].opportunityControl.opportunityAudit`, one row per
union candidate contact, including both arm states and the assigned stratum.

## Derived dataset and analysis

- Machine-readable rows: `artifacts/cellular-opportunity-representativeness-20260821-031846/encounter-dataset.json`
- Coverage/time/state/repeat analysis:
  `artifacts/cellular-opportunity-representativeness-20260821-031846/representativeness-analysis.md`

## Validation

- Focused EditMode 148/148:
  `artifacts/unity-tests-20260821-031254/EditMode-results.xml`
- Full suite 148/148 EditMode, 4/6 PlayMode:
  `artifacts/unity-tests-20260821-031337/`
- Known PlayMode failures: Noesis `TextureSource` native-pointer failures;
  no new failures.

## Re-run command

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File tools/Analyze-CellSimOpportunityRepresentativeness.ps1 `
  -CalibrationBaselinePath artifacts/cellular-experiment-20260821-031616/report.json `
  -CalibrationBlockPlusTwoPath artifacts/cellular-experiment-20260821-031717/report.json `
  -HeldOutBaselinePath artifacts/cellular-experiment-20260821-031803/report.json `
  -HeldOutBlockPlusTwoPath artifacts/cellular-experiment-20260821-031846/report.json `
  -OutputDirectory artifacts/cellular-opportunity-representativeness-20260821-031846
```
