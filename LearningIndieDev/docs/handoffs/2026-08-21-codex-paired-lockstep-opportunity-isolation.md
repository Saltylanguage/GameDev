# Paired lockstep Fox attack-opportunity isolation result

## Verdict

**ISOLATED_WITH_CENSORING_LIMITATION**

The realized combat exposure is isolated: every accepted paired seed executed
the same selected common opportunity in both arms. The natural candidate set is
not fully represented because one-arm-only contacts are excluded; common/union
candidate coverage was `91/186 = 48.9%` in calibration and
`120/224 = 53.6%` held-out.

## Mechanism

The old fixed-rate diagnostic only paired the schedule. After different combat
outcomes, each arm could present a different Fox-to-Hare contact set, so equal
scheduled ticks did not imply equal attack exposure. The new paired runner
intersects stable abstract contact identities at each tick, drops one-arm-only
contacts, validates the selected identity in both post-behavior worlds, and
executes that one contact in both. It does not alter production natural mode.

## Accepted controlled runs

### Calibration (`10100–10119`, 20 paired seeds)

| Metric | None | Block +2 | Delta |
| --- | ---: | ---: | ---: |
| Scheduled slots | 1,334 | 1,334 | 0 |
| Selected-slot valid | 112 | 121 | +9 |
| Common valid / paired attempts | 84 | 84 | 0 |
| Baseline-only / Block+2-only | 28 | 37 | — |
| Paired mismatches / invalidated | 0 | 0 | 0 |
| Candidate contacts (baseline / trial / common / union) | 120 / 157 / 91 / 186 | same | — |
| Fox hit rate | 72.619% | 65.476% | -7.143 pp |
| Successful hits / Fox-caused Hare deaths | 61 / 61 | 55 / 55 | -6 / -6 |
| Lethality per hit | 100% | 100% | 0 pp |
| Hare total deaths | 611 | 603 | -8 |
| Mean Hare population | 17.552 | 17.208 | -0.344 |
| Hare AUC | 3,527.9 | 3,458.85 | -69.05 |
| Final Hare population | 22.45 | 22.95 | +0.5 |

### Held-out (`10125–10144`, 20 paired seeds)

| Metric | None | Block +2 | Delta |
| --- | ---: | ---: | ---: |
| Scheduled slots | 1,333 | 1,333 | 0 |
| Selected-slot valid | 146 | 144 | -2 |
| Common valid / paired attempts | 102 | 102 | 0 |
| Baseline-only / Block+2-only | 44 / 42 | — | — |
| Paired mismatches / invalidated | 0 | 0 | 0 |
| Candidate contacts (baseline / trial / common / union) | 177 / 167 / 120 / 224 | same | — |
| Fox hit rate | 72.549% | 65.686% | -6.863 pp |
| Successful hits / Fox-caused Hare deaths | 74 / 74 | 67 / 67 | -7 / -7 |
| Lethality per hit | 100% | 100% | 0 pp |
| Hare total deaths | 669 | 671 | +2 |
| Mean Hare population | 18.882 | 18.762 | -0.12 |
| Hare AUC | 3,795.35 | 3,771.15 | -24.2 |
| Final Hare population | 23.5 | 22.7 | -0.8 |

## Interpretation

At the controlled combat layer this is **L1**: Block+2 lowers opposed-roll
hit rate and successful Fox kills under identical paired attempts. Lethality is
unchanged at 100%, so the lever is accuracy, not damage conversion. The
whole-world ecology is mixed (**L3-like context**): starvation and other deaths
vary, and Hare mean/AUC/final outcomes do not improve consistently. Do not
promote Block+2 as a balance change from this diagnostic alone.

## Gate and accounting evidence

- Calibration per-seed gate: scheduled `1,334/1,334`; paired attempts `84/84`;
  common-vs-attempt mismatches `0`; ID mismatches `0`; invalidated `0`.
- Held-out per-seed gate: scheduled `1,333/1,333`; paired attempts `102/102`;
  common-vs-attempt mismatches `0`; ID mismatches `0`; invalidated `0`.
- Food, combat, and reproduction reconciliation are true in all four reports.
- Focused EditMode: **147/147 passed** —
  `artifacts/unity-tests-20260821-024259/EditMode-results.xml`.
- Final full suite: EditMode **147/147 passed**; PlayMode **4/6 passed**.
  The two PlayMode failures are the same pre-existing Noesis
  `TextureSource` native-pointer failures at
  `Assets/UI/HUD/Scripts/SpeciesSimulationViewModel.cs:464`; they are not
  introduced by this change. Artifact directory:
  `artifacts/unity-tests-20260821-024745/`.

## Evidence index

Calibration:

- `artifacts/cellular-experiment-20260821-024335/report.json`
- `artifacts/cellular-experiment-20260821-024335/analysis.md`
- `artifacts/cellular-experiment-20260821-024414/report.json`
- `artifacts/cellular-experiment-20260821-024414/analysis.md`
- `artifacts/cellular-experiment-20260821-024414/comparison.md`

Held-out:

- `artifacts/cellular-experiment-20260821-024452/report.json`
- `artifacts/cellular-experiment-20260821-024452/analysis.md`
- `artifacts/cellular-experiment-20260821-024530/report.json`
- `artifacts/cellular-experiment-20260821-024530/analysis.md`
- `artifacts/cellular-experiment-20260821-024530/comparison.md`

The earlier `20260821-023611`/`023716` pilot is retained as non-evidence
because it predates the corrected slot telemetry semantics. The old fixed-rate
reports remain historical evidence of the prior `NOT ISOLATED` result.

## Recommended next step

Keep this implementation diagnostic-only. The next high-information experiment
is repeated natural-world encounter generation or a larger paired sample that
tests whether the roughly 49–54% common/union coverage is representative. Do
not change Block+2 balance, damage, resources, reproduction, capacity, or
starting populations until that censoring question is answered.

## Repository state

- Branch: `BevBranch`.
- Implementation commit: `24922ef` (`Add paired lockstep opportunity isolation`).
- The handoff and branch-state metadata are committed on `BevBranch`.
- `origin/BevBranch` remains `ab5fc89`; local `HEAD` is six commits ahead. No
  push was performed; the user can push `BevBranch` when ready.
- No Trello card was moved; Card 59 remains In Progress and Card 29 remains
  Backlog & Ideas.
