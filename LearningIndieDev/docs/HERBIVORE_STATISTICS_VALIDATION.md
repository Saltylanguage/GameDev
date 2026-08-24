# Herbivore Slash-Line Measurement Contract

Status: baseline and controlled-trial validation completed on 2026-08-23,
with one schema limitation: ECN cannot currently be reconstructed from the
exported event records.

This document separates four claims:

1. formula correctness;
2. game calculation correctness;
3. simulation behavior and event accumulation;
4. causal effects of rule changes.

The independent validation path uses the raw count fields from an actual
simulation report as inputs. It does not consume the game's already-computed
metric values to calculate anything.

## Formula contract

| Statistic | Formula | Raw inputs | Undefined/invalid handling |
| --- | --- | --- | --- |
| FPO | `SPO + BIR - PREY - STRV - CRWD` | `SPO`, `BIR`, `PREY`, `STRV`, `CRWD` | Compare the formula result to the observed final population. A mismatch is an FPO reconciliation failure. |
| pAVI | `1 - PREY / ECN` | `PREY`, `ECN` | `ECN=0, PREY=0` is N/A. Positive `PREY` with `ECN=0`, negative exposure, or `PREY>ECN` is INVALID. |
| sAVI | `1 - STRV / (SPO + BIR - PREY)` | `STRV`, `SPO`, `BIR`, `PREY` | Zero exposure with zero `STRV` is N/A. Positive `STRV`, negative exposure, or `STRV>denominator` is INVALID. |
| cAVI | `1 - CRWD / (SPO + BIR - PREY - STRV)` | `CRWD`, `SPO`, `BIR`, `PREY`, `STRV` | Zero exposure with zero `CRWD` is N/A. Positive `CRWD`, negative exposure, or `CRWD>denominator` is INVALID. |
| bAVG | `BIR / MAT` | `BIR`, `MAT` | `MAT=0, BIR=0` is N/A. Positive `BIR` with `MAT=0`, negative opportunity, or `BIR>MAT` is INVALID. |
| RFS | `(FPO - SPO) * bAVG` | `FPO`, `SPO`, `bAVG` | Valid `bAVG=0` is a valid zero multiplier when `MAT>0`. N/A bAVG remains N/A; INVALID bAVG remains INVALID. |
| APS | `RFS + pAVI - (1-sAVI) - (1-cAVI)` | RFS, pAVI, sAVI, cAVI | N/A contributions are neutral. Any INVALID component or FPO reconciliation failure makes APS INVALID. |

The game stores raw counts as integers and computed export values as floats.
The UI formats valid floats with up to two decimal places (`0.##`); the JSON
export retains the float value. The independent validator compares JSON values
with a `0.00001` tolerance and treats UI display comparison as a separate
rounded check.

## Raw-count origins

| Raw value | Current source |
| --- | --- |
| SPO/FPO | First and final `PopulationHistory` snapshots for the player species. |
| ECN | `SpeciesSimulationMetrics.RecordHerbivoreEncounter`, called when a carnivore has a creature target whose role is herbivore. |
| PREY | `SpeciesSimulationMetrics.RecordHerbivorePreyed`, called when that carnivore-herbivore combat resolves lethally. |
| STRV/CRWD | Creature `SpeciesDeathEvent` records with `Starvation` or `Crowding` cause. |
| MAT | `SpeciesReproductionActivity.Candidates`; one candidate is recorded when the reproduction resolver evaluates a live parent. This is resolver opportunity, not merely a behavior-state `Mating` tick. |
| BIR | `SpeciesSimulationActivity.Births`; incremented once for each offspring actually placed. |

The stat line is assembled in
`Assets/Scripts/Game/Simulation/SpeciesSimulationMetrics.cs` by
`CreateHerbivoreStatLine`. The authoritative formulas and status handling live
in `SpeciesHerbivoreStatLine` in that same file.

## Game display and export paths

- Rewards/results UI: `Assets/UI/HUD/Scripts/SpeciesSimulationViewModel.cs`
  calls `CreateHerbivoreStatLine` and formats the result when
  `bev-experimental` is enabled for a herbivore player.
- Batch export: `Assets/Editor/SimulationTools/SimulationReportSerialization.cs`
  calls the same `CreateHerbivoreStatLine` path and serializes both raw counts
  and computed values into `herbivoreStatLine`.
- Human report: `Tools/New-CellSimReport.ps1` displays the serialized export.

The UI and export do not currently have separate formula implementations; they
share the domain calculation path. The independent audit therefore compares
against export values and separately verifies that the UI path is populated.

## Current evidence boundary

The JSON report exports accumulated raw count fields, death events, activity,
population history, and combat telemetry. Death, birth, mating-candidate, SPO,
and FPO counts can be independently cross-checked from those supporting
records. ECN is currently an accumulated counter rather than a per-encounter
event list in the JSON schema, so the first validation proves the formulas and
the exported counter boundary, but does not yet independently reconstruct every
ECN encounter from event records. A mismatch there must be treated as an
instrumentation gap, not silently adjusted.

## Validation sequence

`simulation → raw event/count output → independent calculation → game/export comparison → controlled one-rule perturbation`

Fixed numeric fixtures belong only in edge-case calculator tests. A real
simulation report is required before making claims about the game's statistics
or about causal effects of changing a simulation rule.

## First controlled experiment matrix

The first causal comparison should use the same Forest Edge scenario, player
species (`hare`), seed panel (`1-20`), grid, run length, step interval, combat
mode, attack-opportunity mode, and `bev-experimental` export flag in both arms:

| Arm | Single changed rule | Everything else |
| --- | --- | --- |
| Baseline | `upgradeId=none` | Shared fixed configuration and seeds |
| Trial | `upgradeId=faster-movement` | Shared fixed configuration and seeds |

Acceptance for this experiment is not a desired APS/RFS direction. Both arms
must first pass independent formula validation per seed. Only then should the
raw-count deltas and statistic deltas be described as evidence of the movement
rule's effect. The baseline and trial reports, validation outputs, and revision
identifier must be preserved together.

## Completed validation evidence

Both arms used Forest Edge, hare, seeds `1-20`, a `32x32` grid, 20 seconds,
`0.1` second steps, opposed-roll combat, natural attack opportunities, and
`bev-experimental`. The only requested rule change was the player upgrade:

The top-level reports matched on all 12 shared configuration fields checked;
the only differences were `upgradeId`, `upgradeType`, `upgradeValue`, and the
derived `rulesetFingerprint`.

| Arm | Report | Validation | Formula comparisons | Supporting raw checks |
| --- | --- | --- | ---: | ---: |
| Baseline (`upgradeId=none`) | `artifacts/cellular-experiment-20260823-114102/report.json` | `VALIDATED_WITH_LIMITATIONS` | 140/140 pass | 140 pass; 20 ECN unavailable |
| Trial (`upgradeId=faster-movement`) | `artifacts/cellular-experiment-20260823-114357/report.json` | `VALIDATED_WITH_LIMITATIONS` | 140/140 pass | 140 pass; 20 ECN unavailable |

The independent validator calculated FPO, pAVI, sAVI, cAVI, bAVG, RFS, and
APS from each report's raw stat-line counts. It did not use the game's metric
values as calculation inputs. Both arms had zero metric mismatches and zero
available raw-count mismatches. The 20 unavailable checks per arm are the
known ECN instrumentation limitation described above.

The paired movement trial changed the following mean values relative to the
same-seed baseline. These are observations of this controlled panel, not a
claim that the upgrade is universally beneficial:

| Field | Seeds changed | Mean delta (trial - baseline) |
| --- | ---: | ---: |
| ECN | 20/20 | -0.80 |
| PREY | 14/20 | -0.30 |
| STRV | 20/20 | +1.25 |
| MAT | 20/20 | +529.70 |
| BIR | 18/20 | +5.70 |
| CRWD | 16/20 | +0.50 |
| FPO | 19/20 | +4.25 |
| pAVI | 18/20 | -0.049162 |
| sAVI | 20/20 | +0.037308 |
| cAVI | 20/20 | -0.004337 |
| bAVG | 20/20 | +0.000036 |
| RFS | 20/20 | +0.045175 |
| APS | 20/20 | +0.028985 |

The production EditMode suite also passed all 166 tests, including focused
coverage for valid formulas, N/A neutralization, invalid zero-exposure
deaths, zero-bAVG RFS, FPO reconciliation failure, and uncounted deaths.
The PlayMode suite was not accepted as presentation evidence: 4 of 7 tests
passed and 3 failed on the existing headless Noesis texture-native-pointer
error before the presentation assertions could be trusted.
