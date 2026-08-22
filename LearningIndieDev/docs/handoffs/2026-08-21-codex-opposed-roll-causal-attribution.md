# Opposed-roll `stronger-block-2` causal attribution

## Verdict

**ATTRIBUTED**

The expected defensive benefit disappears in three linked places:

1. The previous hit-rate metric counted only directional block rolls. Hare blocks are cardinal, while Fox attacks use eight directions, so unblocked diagonal attacks were omitted.
2. In the complete funnel, `Block +2` reduced hit probability but did not reduce attack pressure: Fox opportunities/attempts increased, and every successful Fox hit was lethal (`2` damage against Hare health `1`).
3. In the held-out set, the small reduction in Fox-caused deaths was replaced by starvation/crowding, while births and the transient Hare trajectory rose before converging at the terminal sample.

Primary classification: **G — multiple interacting mechanisms**, specifically combat lethality/opportunity compensation, held-out mortality substitution, and temporal/resource compensation.

No balance value was changed. This is a diagnostic instrumentation pass only.

## Scope and provenance

- Baseline implementation: `926fc5d` (`Record opposed block validation failure`).
- Diagnostic telemetry commit: `c20bb7f` (`Add opposed combat causal telemetry`).
- Unity: `6000.4.6f1`, `F:\Editor\6000.4.6f1-x86_64\Editor\Unity.exe`.
- Scenario: `Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset`.
- Grid: `32 x 32`; duration `20.0 s`; step `0.1 s`; player `hare`.
- Arms: `opposed-roll + none` and `opposed-roll + stronger-block-2`.
- Calibration: seeds `10100–10119`; held-out: seeds `10125–10144`.
- Four runs were executed under elevated Unity licensing preflight; all reports are schema 10.

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

## Existing telemetry audit

| Causal stage | Existing metric | Sufficient? | Diagnostic addition |
| --- | --- | --- | --- |
| Attack opportunity | No complete combat funnel; food-action attempts were available | No | `combatOpportunities` |
| Resolved attack | Only directional `combatRolls` | No | `combatAttempts`, `combatHits`, `combatBlocked` |
| Damage/kill outcome | `damageDealt`, `combatKills`, combat death events | Partial | `combatDamageApplications`, `combatNonLethalHits`, `combatLethalHits` |
| Fox-caused Hare death | `deathEvents` with `cause: Combat` | Yes | None |
| Total Hare mortality/cause | `deaths`, starvation/crowding/wilt counters and `deathEvents` | Yes | None |
| Hare lifespan | Creature `age` on each death event | Yes as an age-at-death distribution | None |
| Reproduction | Candidate resolver funnel and births | Yes | None |
| Resource pressure | Hare food consumption/successes and plant history/death events | Yes for this bounded run | None |
| Population trajectory | Per-tick `populationHistory` | Yes | None |

The new counters are increments after existing branch decisions and consume no random values. No production assets, scenes, packages, or balance parameters changed.

## Previous result versus complete combat funnel

The earlier validation's `Fox attempts/hits` values were counts of `combatRolls`. A roll is emitted only when the target has a directional block. They were not all Fox→Hare attacks.

| Set | Arm | Directional rolls | Directional roll hits | Prior rate | Complete attempts | Complete hits | Complete rate |
| --- | --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Calibration | none | 85 | 41 | 48.2% | 127 | 83 | 65.35% |
| Calibration | Block +2 | 98 | 38 | 38.8% | 146 | 86 | 58.90% |
| Held-out | none | 96 | 46 | 47.9% | 155 | 105 | 67.74% |
| Held-out | Block +2 | 95 | 40 | 42.1% | 158 | 103 | 65.19% |

The complete hit-rate deltas are therefore `-6.45 pp` calibration and `-2.55 pp` held-out. The prior directional-roll effect is real, but it overstated the total Fox attack-rate change relevant to mortality.

## Combat funnel

### Calibration `10100–10119`

| Metric | None | Block +2 | Delta |
| --- | ---: | ---: | ---: |
| Fox creature opportunities | 132 | 150 | +18 |
| Fox resolved attacks | 127 | 146 | +19 |
| Fox hits | 83 | 86 | +3 |
| Blocked rolls | 44 | 60 | +16 |
| Damage applications | 83 | 86 | +3 |
| Non-lethal hits | 0 | 0 | 0 |
| Lethal hits / combat kills | 83 | 86 | +3 |
| Fox-caused Hare deaths | 83 | 86 | +3 |
| Kills per attack | 65.35% | 58.90% | -6.45 pp |
| Lethality given hit | 100% | 100% | 0 pp |

### Held-out `10125–10144`

| Metric | None | Block +2 | Delta |
| --- | ---: | ---: | ---: |
| Fox creature opportunities | 159 | 162 | +3 |
| Fox resolved attacks | 155 | 158 | +3 |
| Fox hits | 105 | 103 | -2 |
| Blocked rolls | 50 | 55 | +5 |
| Damage applications | 105 | 103 | -2 |
| Non-lethal hits | 0 | 0 | 0 |
| Lethal hits / combat kills | 105 | 103 | -2 |
| Fox-caused Hare deaths | 105 | 103 | -2 |
| Kills per attack | 67.74% | 65.19% | -2.55 pp |
| Lethality given hit | 100% | 100% | 0 pp |

The increased opportunities/attempts are an observed encounter-pressure compensation, not proof of an adaptive Fox behavior change. The simulation has no new Fox policy in this pass.

## Hare mortality, survival, and reproduction

| Set | Metric | None | Block +2 | Delta |
| --- | --- | ---: | ---: | ---: |
| Calibration | Total Hare deaths | 656 | 654 | -2 |
| Calibration | Fox combat deaths | 83 | 86 | +3 |
| Calibration | Starvation deaths | 512 | 505 | -7 |
| Calibration | Crowding deaths | 33 | 31 | -2 |
| Calibration | Wilt deaths | 28 | 32 | +4 |
| Calibration | Hare births | 630 | 613 | -17 |
| Calibration | Reproduction candidates | 72,867 | 72,703 | -164 |
| Calibration | Successful reproduction attempts | 630 | 613 | -17 |
| Calibration | Mean Hare death age | 52.974 | 53.145 | +0.171 |
| Calibration | Median Hare death age | 38 | 39.5 | +1.5 |
| Held-out | Total Hare deaths | 659 | 683 | +24 |
| Held-out | Fox combat deaths | 105 | 103 | -2 |
| Held-out | Starvation deaths | 487 | 511 | +24 |
| Held-out | Crowding deaths | 36 | 39 | +3 |
| Held-out | Wilt deaths | 31 | 30 | -1 |
| Held-out | Hare births | 610 | 637 | +27 |
| Held-out | Reproduction candidates | 67,553 | 68,796 | +1,243 |
| Held-out | Successful reproduction attempts | 610 | 637 | +27 |
| Held-out | Mean Hare death age | 50.020 | 50.483 | +0.463 |
| Held-out | Median Hare death age | 37 | 38 | +1 |

The age-at-death distribution shifts slightly older, but it does not produce a stable population benefit. In the held-out set, two fewer Fox deaths are more than offset by twenty-four additional starvation deaths and three additional crowding deaths.

## Resource and ecological compensation

| Set | Metric | None | Block +2 | Delta |
| --- | --- | ---: | ---: | ---: |
| Calibration | Hare food consumed | 124,701 | 124,370 | -331 |
| Calibration | Mean plant population over trajectory | 794.421 | 795.243 | +0.822 |
| Calibration | Plant population-time integral | 159,678.6 | 159,843.9 | +165.3 |
| Held-out | Hare food consumed | 118,293 | 120,497 | +2,204 |
| Held-out | Mean plant population over trajectory | 802.932 | 801.251 | -1.681 |
| Held-out | Plant population-time integral | 161,389.4 | 161,051.4 | -338.0 |

There is no global plant collapse. The held-out arm consumes more food while plant stock is slightly lower, and the extra Hares experience more starvation/crowding. This supports local resource/competition compensation rather than a single global carrying-cap failure.

## Population trajectory

| Set | Metric | None | Block +2 | Delta |
| --- | --- | ---: | ---: | ---: |
| Calibration | Mean minimum Hare population | 10.50 | 10.60 | +0.10 |
| Calibration | Mean Hare population | 18.392 | 18.347 | -0.045 |
| Calibration | Mean maximum Hare population | 28.35 | 28.40 | +0.05 |
| Calibration | Hare population-time integral | 3,696.85 | 3,687.80 | -9.05 |
| Calibration | Final Hare population | 22.10 | 21.55 | -0.55 |
| Held-out | Mean minimum Hare population | 10.30 | 10.35 | +0.05 |
| Held-out | Mean Hare population | 17.065 | 17.381 | +0.315 |
| Held-out | Mean maximum Hare population | 27.90 | 28.20 | +0.30 |
| Held-out | Hare population-time integral | 3,430.15 | 3,493.65 | +63.50 |
| Held-out | Final Hare population | 21.10 | 21.20 | +0.10 |

The held-out trial has a measurable transient/area-under-trajectory benefit that is almost entirely hidden by the terminal metric (`+0.10` final). Calibration does not show the same benefit.

## Paired seed deltas

All unlisted seeds had zero delta across the diagnostic counters and Hare trajectory metrics. The following are the only seeds with any arm divergence.

| Set/seed | Δ Fox opp | Δ Fox attempts | Δ Fox hits | Δ hit rate | Δ Fox deaths | Δ total Hare deaths | Δ starvation | Δ births | Δ food | Δ Hare mean | Δ Hare AUC | Δ final |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Cal 10108 | +7 | +7 | +1 | -54.55 pp | +1 | +3 | +4 | -2 | -238 | -0.224 | -45 | -6 |
| Cal 10110 | -4 | -3 | 0 | +25.71 pp | 0 | -3 | -6 | -12 | -765 | -4.040 | -812 | -6 |
| Cal 10114 | +4 | +4 | +1 | -12.86 pp | +1 | -1 | -3 | +3 | +459 | +1.154 | +232 | +4 |
| Cal 10118 | +11 | +11 | +3 | -16.67 pp | +3 | -7 | -10 | -8 | +548 | +0.990 | +199 | -1 |
| Cal 10119 | 0 | 0 | -2 | -40.00 pp | -2 | +6 | +8 | +2 | -335 | +1.218 | +245 | -2 |
| Held 10125 | +1 | +1 | -1 | -17.78 pp | -1 | +6 | +8 | +2 | +326 | +0.592 | +119 | -6 |
| Held 10137 | -2 | -2 | -1 | +3.33 pp | -1 | +2 | +4 | +4 | -48 | +0.234 | +47 | +1 |
| Held 10142 | +3 | +3 | +1 | -5.56 pp | +1 | +9 | +7 | +6 | +352 | +1.811 | +364 | -3 |
| Held 10143 | 0 | 0 | -1 | -5.26 pp | -1 | +8 | +5 | +14 | +1,557 | +3.666 | +737 | +8 |
| Held 10144 | +1 | +1 | 0 | -9.72 pp | 0 | -1 | 0 | +1 | +17 | +0.015 | -3 | +2 |

## Hypothesis disposition

- **H1 supported:** every hit was lethal, and the complete attack funnel showed more attempts/opportunities in the trial; lower hit probability did not lower kills reliably.
- **H2 supported in held-out, mixed in calibration:** held-out Fox deaths fell by 2 while starvation rose by 24 and crowding by 3. Calibration Fox deaths rose by 3, so substitution is not the only mechanism.
- **H3 not a stable bottleneck:** age-at-death rose slightly, but births fell by 17 in calibration and rose by 27 held-out.
- **H4 supported:** the held-out trial consumed more food with slightly lower plant stock and more starvation/crowding, consistent with local resource/competition compensation.
- **H5 supported for held-out:** mean trajectory rose by 0.315 and AUC by 63.5 while final population rose only 0.10.
- **H6 narrowed:** encounter/attempt counts rose in the trial, but this is not evidence of an adaptive Fox policy; no behavior policy was changed or separately instrumented.
- **H7 not primary:** the causal funnel has enough signal to identify the mechanisms; the remaining seed-level variance affects magnitude, not the location of the missing link.

## Causal chain

```text
Block +2
→ directional block-roll wins fall, but prior metric omitted unblocked diagonal attacks
→ complete Fox hit rate falls only 6.45 pp calibration / 2.55 pp held-out
→ Fox attack opportunities/attempts rise (+18/+19 calibration; +3/+3 held-out)
→ every remaining hit is lethal, so combat deaths are +3 calibration / -2 held-out
→ held-out saved Hares are offset by +24 starvation and +3 crowding deaths
→ births/trajectory can rise transiently, but terminal population converges
```

## Acceptance and correctness

- Diagnostic telemetry added: `combatOpportunities`, `combatAttempts`, `combatHits`, `combatBlocked`, `combatDamageApplications`, `combatNonLethalHits`, `combatLethalHits`.
- EditMode: **144/144 passed**, artifact `artifacts/unity-tests-20260821-000401/EditMode-results.xml`.
- Full test run: EditMode **144/144**; PlayMode **4/6 passed, 2 failed** in pre-existing Noesis texture-native-pointer paths (`CavePreviewPlayModeTests.CellularAutomataPrototypeCreatesAndAnimatesTheSpeciesPreview` and `SpeciesPresentationPlayModeTests.CellularPrototypeInitializesEveryAuthoredAnimalSprite`). Artifact: `artifacts/unity-tests-20260821-001357/PlayMode-results.xml`.
- Diagnostic runs: **80/80 completed** under elevated Unity preflight.
- Food reconciliation failures: `0`.
- Reproduction reconciliation failures: `0`.
- Combat `hits + blocked = attempts` failures: `0`.

## Evidence paths

| Evidence | Path |
| --- | --- |
| Calibration control report | `artifacts/cellular-experiment-20260821-000438/report.json` |
| Calibration control analysis | `artifacts/cellular-experiment-20260821-000438/analysis.md` |
| Calibration Block +2 report | `artifacts/cellular-experiment-20260821-000510/report.json` |
| Calibration Block +2 analysis | `artifacts/cellular-experiment-20260821-000510/analysis.md` |
| Held-out control report | `artifacts/cellular-experiment-20260821-000543/report.json` |
| Held-out control analysis | `artifacts/cellular-experiment-20260821-000543/analysis.md` |
| Held-out Block +2 report | `artifacts/cellular-experiment-20260821-000615/report.json` |
| Held-out Block +2 analysis | `artifacts/cellular-experiment-20260821-000615/analysis.md` |
| EditMode results | `artifacts/unity-tests-20260821-000401/EditMode-results.xml` |
| Full-suite PlayMode results | `artifacts/unity-tests-20260821-001357/PlayMode-results.xml` |

## Recommended next experiment

Do not test Block +3 or tune damage yet. The single most informative next lever is an **instrumented combat-pressure control** that holds the complete Fox attack-opportunity funnel constant, or a narrowly scoped encounter-frequency arm, so block accuracy can be separated from encounter count. If the project must choose a gameplay lever after that separation, independently tunable block-roll and damage modifiers remain the correct architecture direction; do not promote a balance value from this result.

## Repository and Trello

- Branch: `BevBranch`.
- Starting revision: `926fc5d`.
- Implementation revision: `c20bb7f` (`Add opposed combat causal telemetry`).
- Handoff documentation is committed on top of that implementation; the working tree is clean.
- Origin: local branch is five commits ahead of `origin/BevBranch`; no push performed.
- Card 59 remains **In Progress**; add the diagnostic attribution comment and evidence links there.
- Card 29 remains **Backlog & Ideas**; keep independent attack/block/damage modifier architecture deferred.

## Remaining uncertainty

The reports do not identify attacker identity or local resource state immediately before each starvation event, and they do not prove an adaptive Fox targeting policy. The conclusion is bounded to Forest Edge, the authored `32 x 32` grid, the `20 s` window, schema 10, and the two previously validated seed sets.
