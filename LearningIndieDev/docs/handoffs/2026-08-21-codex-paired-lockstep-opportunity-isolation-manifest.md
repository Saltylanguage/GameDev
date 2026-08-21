# Paired lockstep opportunity isolation evidence manifest

## Scope

This manifest is the tracked index for the paired diagnostic completed on
2026-08-21. The raw JSON and generated Markdown under `artifacts/` are ignored
runtime evidence; these paths are intentionally preserved here for replay and
review.

## Frozen design and result

- Design: `docs/handoffs/2026-08-21-codex-paired-lockstep-opportunity-isolation-design.md`
- Final handoff: `docs/handoffs/2026-08-21-codex-paired-lockstep-opportunity-isolation.md`
- Prior failed fixed-rate handoff: `docs/handoffs/2026-08-21-codex-opportunity-isolation.md`

## Calibration, seeds 10100-10119

- Baseline (`none`): `artifacts/cellular-experiment-20260821-024335/report.json`
- Baseline analysis: `artifacts/cellular-experiment-20260821-024335/analysis.md`
- Block+2: `artifacts/cellular-experiment-20260821-024414/report.json`
- Block+2 analysis: `artifacts/cellular-experiment-20260821-024414/analysis.md`
- Pair comparison: `artifacts/cellular-experiment-20260821-024414/comparison.md`

## Held-out, seeds 10125-10144

These are the existing held-out development seeds, not fresh validation data.

- Baseline (`none`): `artifacts/cellular-experiment-20260821-024452/report.json`
- Baseline analysis: `artifacts/cellular-experiment-20260821-024452/analysis.md`
- Block+2: `artifacts/cellular-experiment-20260821-024530/report.json`
- Block+2 analysis: `artifacts/cellular-experiment-20260821-024530/analysis.md`
- Pair comparison: `artifacts/cellular-experiment-20260821-024530/comparison.md`

## Hard gate summary

| Group | Scheduled | Paired attempts (each arm) | Mismatches | Invalidated | Common / union candidates |
| --- | ---: | ---: | ---: | ---: | ---: |
| Calibration | 1,334 / 1,334 | 84 / 84 | 0 | 0 | 91 / 186 (48.9%) |
| Held-out | 1,333 / 1,333 | 102 / 102 | 0 | 0 | 120 / 224 (53.6%) |

Per-seed ID and common-vs-attempt checks are recorded in the comparison files
and were rerun after report regeneration; both groups returned zero failures.

## Unity validation

- Focused EditMode 147/147: `artifacts/unity-tests-20260821-024259/EditMode-results.xml`
- Final full suite: `artifacts/unity-tests-20260821-024745/`
- Full-suite result: EditMode 147/147; PlayMode 4/6. The two PlayMode failures
  are the known Noesis `TextureSource` native-pointer failures at
  `Assets/UI/HUD/Scripts/SpeciesSimulationViewModel.cs:464`.

## Non-evidence pilots

- `artifacts/cellular-experiment-20260821-023611/`
- `artifacts/cellular-experiment-20260821-023716/`

These pilots predate the corrected slot-telemetry semantics and are retained
only as implementation history, not as ecological evidence.
