# EX-007 - Predictive statline interventions

**Experiment ID:** `EXP-007`  
**Status:** Contract prepared; execution pending  
**Decision owner:** Human design owner  
**Reference scenario:** `Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset`

EX-007 is the first P3 test of the auditable prediction loop using Sim's
herbivore statline. It is deliberately small: one supported single-variable
intervention and one joint intervention, with the same seeds, the same report
schema, and a held-out seed panel.

## What this package must prove

1. A supported single intervention can be stated in executable terms.
2. A simultaneous two-variable intervention can be represented as one vector and
   compared against the same baseline.
3. The prediction input can be restricted to the baseline, this contract, the
   permitted range, and the telemetry allowlist.
4. The prediction records direction, approximate effect, affected outcomes,
   uncertainty, and limits before trial results are available.
5. Paired same-seed runs preserve the causal comparison.
6. A held-out seed panel tests whether the prediction transfers beyond the
   fitting panel.
7. False causes, missed effects, confidence calibration, and review time are
   recorded explicitly.
8. A human accepts, rejects, or revises the conclusion.

## Experiment contract

| Field | Locked value |
|---|---|
| Scenario | `ForestEdge.asset` |
| Player species | `hare` |
| Training seeds | `1-20` |
| Held-out seeds | `101-105` |
| Grid | `32x32` unless the baseline manifest says otherwise |
| Duration / step | `20.0s` / `0.1s` |
| Combat | `opposed-roll` |
| Attack opportunity | `natural` |
| Experimental export | `bev-experimental` |
| Primary endpoint | Mean final Hare population (`FPO`) and per-seed delta |
| Secondary endpoints | `APS`, `RFS`, `predAVG`, `pAVI`, `eAVI`, `sAVI`, `cAVI`, `BIR`, `MAT`, `PREY`, `STRV`, `CRWD`, movement steps, and combat telemetry |

The baseline and every intervention must carry complete `report.json`,
`report.csv`, `statline.csv`, `manifest.json`, and `unity.log` artifacts. Each
arm must pass the artifact validator and the independent statline validator
before its numbers are used for prediction scoring.

## Arms and permitted range

| Arm | Executable intervention | Permitted range | Purpose |
|---|---|---|---|
| B | `upgradeId=none` | no upgrade | Same-seed control |
| S1 | `upgradeId=faster-movement` | catalog value `+0.5` movement speed only | Single-variable effect |
| J1 | `upgradeSequence=faster-movement,crowding-tolerance` | movement `+0.5` and crowding tolerance `+1` only | Joint interaction |

The catalog is discrete at this stage. It does not support a continuous sweep
for these upgrades, so no continuous-range or monotonicity claim is allowed.
The joint arm is one simultaneous intervention vector at run start; its result
must not be reconstructed by adding the two single-arm effects.

## Prediction input boundary

The AI receives only:

- the validated baseline report or its explicitly generated summary;
- this experiment contract;
- the permitted intervention range above; and
- the available telemetry list in `AI_INPUT_TEMPLATE.md`.

The AI must not receive trial reports, post-run comparisons, hidden source
files, or human conclusions before writing its prediction. Unsupported outcomes
must be marked `Not currently testable`, and sparse or invalid metrics must be
marked unresolved rather than filled with a plausible value.

Use [AI_INPUT_TEMPLATE.md](AI_INPUT_TEMPLATE.md) to prepare the bounded input
and [PREDICTION_TEMPLATE.json](PREDICTION_TEMPLATE.json) to capture the
pre-registration prediction.

## Analysis and scoring

For each arm, compare trial minus baseline within the same seed. Report the
mean, median, range, and the per-seed direction. Treat a direction as supported
only when the pre-registered endpoint's sign and tolerance rule are met on the
training panel and are then checked on held-out seeds.

Record:

- predicted outcomes that did not move materially (`false causes` when they
  were presented as causal, not merely as candidates);
- materially changed outcomes missing from the prediction (`missed effects`);
- predicted versus observed direction and effect band;
- confidence versus the observed hit rate, labelled calibration evidence rather
  than a claim of model calibration from one experiment;
- reviewer minutes and reruns needed to understand the proposal; and
- the human decision: accept, reject, or revise, with scope and follow-up.

The factual report and AI analysis remain separate. A measured effect does not
decide balance, fun, quality, or promotion.

## Evidence record

| Artifact | Purpose | Status |
|---|---|---|
| `AI_INPUT_TEMPLATE.md` | Bounded pre-run information supplied to AI | Prepared |
| `PREDICTION_TEMPLATE.json` | Machine-readable pre-registration record | Prepared |
| `REPORT.md` | Factual paired and held-out outcomes | Pending runs |
| `AI_ANALYSIS.md` | Prediction scoring and false-cause/missed-effect review | Pending runs |
| `HUMAN_DECISION.md` | Human accept/reject/revise record | Pending review |

## Execution order

1. Complete and validate the fresh baseline on seeds `1-20`.
2. Generate the bounded AI input and record the prediction before any S1/J1
   trial report is shown to the analyst.
3. Run B, S1, and J1 on the same training seeds; validate every bundle.
4. Run B, S1, and J1 again on held-out seeds `101-105`.
5. Produce the factual report, then score the prediction separately.
6. Record false causes, missed effects, calibration note, review time, and the
   human decision.

Until steps 3-6 are complete, this package is a protocol and not evidence of
predictive success.
