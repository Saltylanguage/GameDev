# Opposed-roll `stronger-block-2` validation

[Working state](../WORKING_STATE.md) | Implementation commit: `c4fa541` | Verdict: **FAIL** under the predeclared population-robustness gate

## Experiment manifest

- Source revision: `c4fa541` (`Implement opt-in opposed combat rolls`)
- Unity: `6000.4.6f1`, `F:\Editor\6000.4.6f1-x86_64\Editor\Unity.exe`
- Scenario: `Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset`
- Player: `hare`; grid: `32 x 32`; duration: `20.0 s`; step: `0.1 s`
- Combat mode: `opposed-roll` for both arms
- Baseline: `-UpgradeId none`
- Trial: `-UpgradeId stronger-block-2` (`BlockAmount +2`)
- Calibration seeds: `10100–10119`
- Held-out seeds: `10125–10144` (fresh; `10120–10124` were already observed in an earlier run and were not reused as held-out evidence)
- All runs used elevated Unity preflight and the same authored scenario/configuration.

Commands:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Run-CellularExperiment.ps1 `
  -ScenarioPath Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset `
  -SeedStart 10100 -SeedCount 20 -PlayerSpeciesId hare `
  -CombatMode opposed-roll -UpgradeId none

powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Run-CellularExperiment.ps1 `
  -ScenarioPath Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset `
  -SeedStart 10100 -SeedCount 20 -PlayerSpeciesId hare `
  -CombatMode opposed-roll -UpgradeId stronger-block-2

powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Run-CellularExperiment.ps1 `
  -ScenarioPath Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset `
  -SeedStart 10125 -SeedCount 20 -PlayerSpeciesId hare `
  -CombatMode opposed-roll -UpgradeId none

powershell -NoProfile -ExecutionPolicy Bypass -File .\tools\Run-CellularExperiment.ps1 `
  -ScenarioPath Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset `
  -SeedStart 10125 -SeedCount 20 -PlayerSpeciesId hare `
  -CombatMode opposed-roll -UpgradeId stronger-block-2
```

## Raw evidence

| Set | Arm | Report | Analysis |
|---|---|---|---|
| Calibration `10100–10119` | Opposed / none | `artifacts/cellular-experiment-20260820-230749/report.json` | `artifacts/cellular-experiment-20260820-230749/analysis.md` |
| Calibration `10100–10119` | Opposed / stronger-block-2 | `artifacts/cellular-experiment-20260820-234138/report.json` | `artifacts/cellular-experiment-20260820-234138/analysis.md` |
| Held-out `10125–10144` | Opposed / none | `artifacts/cellular-experiment-20260820-234305/report.json` | `artifacts/cellular-experiment-20260820-234305/analysis.md` |
| Held-out `10125–10144` | Opposed / stronger-block-2 | `artifacts/cellular-experiment-20260820-234337/report.json` | `artifacts/cellular-experiment-20260820-234337/analysis.md` |

All reports are schema 9. Baseline and trial ruleset fingerprints match within each set's controlled comparison except for the intended upgrade change. Every run has zero food-action reconciliation mismatches.

The trial reports identify `upgradeId: stronger-block-2`, `upgradeType: BlockAmount`, and `upgradeValue: 2`; the baseline reports identify `upgradeId: none`. This verifies that the arm changes Hare's opposed block modifier and no unrelated upgrade.

## Aggregate results

| Set | Arm | Fox attempts | Fox hits | Hit rate | Final Hare avg. | Final Fox avg. | Fox extinctions | Reconciliation |
|---|---|---:|---:|---:|---:|---:|---:|---:|
| Calibration | Opposed / none | 85 | 41 | 48.2% | 22.10 | 2.50 | 2/20 | 0 |
| Calibration | Opposed / stronger-block-2 | 98 | 38 | 38.8% | 21.55 | 2.55 | 2/20 | 0 |
| Held-out | Opposed / none | 96 | 46 | 47.9% | 21.10 | 3.10 | 0/20 | 0 |
| Held-out | Opposed / stronger-block-2 | 95 | 40 | 42.1% | 21.20 | 3.00 | 0/20 | 0 |

### Paired aggregate deltas

- Calibration: hit rate `-9.46 pp`; Hare `-0.55`; Fox `+0.05`; Fox extinction unchanged.
- Held-out: hit rate `-5.81 pp`; Hare `+0.10`; Fox `-0.10`; Fox extinction unchanged.
- Average minimum Hare population was effectively unchanged: `10.50 → 10.60` calibration and `10.30 → 10.35` held-out.

## Paired per-seed results

`Delta pp` is `stronger-block-2 hit rate - baseline hit rate`. `Recon` is baseline/trial mismatch count; every row is `0,0`.

### Calibration seeds

| Seed | Base hit/att | Block +2 hit/att | Delta pp | Base Hare | Block Hare | Delta Hare | Base Fox | Block Fox | Recon |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 10100 | 1/4 | 1/4 | 0.0 | 31 | 31 | 0 | 2 | 2 | 0,0 |
| 10101 | 3/5 | 3/5 | 0.0 | 5 | 5 | 0 | 2 | 2 | 0,0 |
| 10102 | 1/2 | 1/2 | 0.0 | 13 | 13 | 0 | 3 | 3 | 0,0 |
| 10103 | 3/11 | 3/11 | 0.0 | 31 | 31 | 0 | 4 | 4 | 0,0 |
| 10104 | 2/3 | 2/3 | 0.0 | 23 | 23 | 0 | 2 | 2 | 0,0 |
| 10105 | 3/5 | 3/5 | 0.0 | 11 | 11 | 0 | 4 | 4 | 0,0 |
| 10106 | 3/4 | 3/4 | 0.0 | 20 | 20 | 0 | 2 | 2 | 0,0 |
| 10107 | 0/0 | 0/0 | 0.0 | 34 | 34 | 0 | 0 | 0 | 0,0 |
| 10108 | 1/1 | 2/8 | -75.0 | 12 | 6 | -6 | 3 | 3 | 0,0 |
| 10109 | 2/3 | 2/3 | 0.0 | 28 | 28 | 0 | 3 | 3 | 0,0 |
| 10110 | 4/8 | 3/4 | 25.0 | 21 | 15 | -6 | 3 | 3 | 0,0 |
| 10111 | 3/7 | 3/7 | 0.0 | 28 | 28 | 0 | 3 | 3 | 0,0 |
| 10112 | 2/7 | 2/7 | 0.0 | 12 | 12 | 0 | 3 | 3 | 0,0 |
| 10113 | 1/6 | 1/6 | 0.0 | 26 | 26 | 0 | 1 | 1 | 0,0 |
| 10114 | 6/9 | 5/11 | -21.2 | 28 | 32 | 4 | 4 | 4 | 0,0 |
| 10115 | 0/0 | 0/0 | 0.0 | 40 | 40 | 0 | 0 | 0 | 0,0 |
| 10116 | 3/5 | 3/5 | 0.0 | 17 | 17 | 0 | 3 | 3 | 0,0 |
| 10117 | 0/0 | 0/0 | 0.0 | 26 | 26 | 0 | 3 | 3 | 0,0 |
| 10118 | 1/3 | 1/11 | -24.2 | 8 | 7 | -1 | 2 | 3 | 0,0 |
| 10119 | 2/2 | 0/2 | -100.0 | 28 | 26 | -2 | 3 | 3 | 0,0 |

### Held-out seeds

| Seed | Base hit/att | Block +2 hit/att | Delta pp | Base Hare | Block Hare | Delta Hare | Base Fox | Block Fox | Recon |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 10125 | 2/4 | 3/7 | -7.1 | 18 | 12 | -6 | 5 | 4 | 0,0 |
| 10126 | 2/6 | 2/6 | 0.0 | 42 | 42 | 0 | 4 | 4 | 0,0 |
| 10127 | 3/7 | 3/7 | 0.0 | 8 | 8 | 0 | 3 | 3 | 0,0 |
| 10128 | 1/1 | 1/1 | 0.0 | 25 | 25 | 0 | 3 | 3 | 0,0 |
| 10129 | 2/4 | 2/4 | 0.0 | 36 | 36 | 0 | 2 | 2 | 0,0 |
| 10130 | 2/5 | 2/5 | 0.0 | 14 | 14 | 0 | 3 | 3 | 0,0 |
| 10131 | 2/3 | 2/3 | 0.0 | 11 | 11 | 0 | 1 | 1 | 0,0 |
| 10132 | 2/8 | 2/8 | 0.0 | 18 | 18 | 0 | 3 | 3 | 0,0 |
| 10133 | 3/4 | 3/4 | 0.0 | 10 | 10 | 0 | 5 | 5 | 0,0 |
| 10134 | 0/0 | 0/0 | 0.0 | 36 | 36 | 0 | 1 | 1 | 0,0 |
| 10135 | 4/7 | 4/7 | 0.0 | 7 | 7 | 0 | 2 | 2 | 0,0 |
| 10136 | 1/3 | 1/3 | 0.0 | 30 | 30 | 0 | 4 | 4 | 0,0 |
| 10137 | 3/7 | 1/4 | -17.9 | 16 | 17 | 1 | 5 | 5 | 0,0 |
| 10138 | 1/1 | 1/1 | 0.0 | 3 | 3 | 0 | 3 | 3 | 0,0 |
| 10139 | 2/2 | 2/2 | 0.0 | 26 | 26 | 0 | 4 | 4 | 0,0 |
| 10140 | 1/1 | 1/1 | 0.0 | 34 | 34 | 0 | 2 | 2 | 0,0 |
| 10141 | 4/7 | 4/7 | 0.0 | 11 | 11 | 0 | 3 | 3 | 0,0 |
| 10142 | 3/6 | 2/7 | -21.4 | 31 | 28 | -3 | 2 | 2 | 0,0 |
| 10143 | 6/17 | 2/14 | -21.0 | 18 | 26 | 8 | 4 | 3 | 0,0 |
| 10144 | 2/3 | 2/4 | -16.7 | 28 | 30 | 2 | 3 | 3 | 0,0 |

## Gate evaluation

- **SC-1 Mechanical effect: PASS.** Block +2 lowers pooled Fox hit rate in both sets (`-9.46 pp` calibration; `-5.81 pp` held-out). Paired hit-rate deltas are non-positive for 19/20 calibration seeds (one positive, four negative, fifteen zero) and 20/20 held-out seeds (five negative, fifteen zero); aggregate direction is lower in both sets.
- **SC-2 Hare population robustness: FAIL.** Calibration Hare delta is `-0.55`; held-out delta is `+0.10`, a sign reversal under the predeclared rule. Both effects are near zero, so this is not evidence of a meaningful harm or benefit.
- **SC-3 Fox extinction safety: PASS.** Extinction is unchanged (`2/20 → 2/20` calibration; `0/20 → 0/20` held-out).
- **SC-4 Reconciliation integrity: PASS.** All 80 accepted runs have zero food-action reconciliation mismatches.

## Verdict and bounded interpretation

**FAIL.** The opposed-roll mechanic responds to Block +2 as intended, but the upgrade does not produce a repeatable Hare population benefit. The population response is effectively neutral/noisy and reverses sign across the independent sets, so the arm is not promoted. This does not prove that opposed rolls are unusable; it proves this Block +2 value is not a validated ecological lever under the current 20-second Forest Edge protocol.

## Repository and deferred work

This evidence pass made no simulation or asset changes. The implementation remains `c4fa541`; the only repository addition is this handoff. The known architecture debt remains deferred: separate `attack-roll modifier`, `block-roll modifier`, and `damage modifier` so defensive tuning does not reuse `AttackAmount` damage semantics. Do not implement that redesign as part of this failed validation unless a later experiment requires it.

## Limitations

The reports record opposed roll events only when a directional block is present; event counts are therefore lower than tick counts. No new statistical-significance threshold was introduced after viewing results. Root-cause links between resource histories and individual attackers remain uninstrumented. The conclusion is bounded to Forest Edge, the authored 32 x 32 grid, the 20-second window, schema 9, and these seed sets.
