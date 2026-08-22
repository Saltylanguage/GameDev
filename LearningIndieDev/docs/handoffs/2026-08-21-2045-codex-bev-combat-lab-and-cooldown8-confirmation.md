# Bev combat lab and cooldown-8 confirmation

Date: 2026-08-21
Branch: `codex/bev-experimental-features`

## Deterministic combat lab

The first four-arm lab accidentally applied upgrades to Hare while measuring Fox attacks. All four arms were identical, which correctly exposed the setup error. Those reports are retained as a negative control.

The corrected lab used `PlayerSpeciesId=fox`, fixed-rate diagnostic opportunities, identical seeds `10500-10519`, and four arms: control, `stronger-attack-modifier`, `stronger-damage`, and existing combined `stronger-attack`.

- Control: 6.40 Fox rolls/run; expected hit 57%; actual hit 48%; damage per application 1.00.
- Attack modifier: 5.80 rolls/run; expected hit 62%; actual hit 59%; damage per application 1.00.
- Damage upgrade: same hit probability as control. The ecology lab could not reveal the damage increase because most successful damage was capped by low remaining Hare health.
- Combined attack: same observed result as attack modifier in this ecology for the same reason.

The direct deterministic tests now cover the missing case: a five-health target takes the independent damage increase from `stronger-damage` while attack modifier remains unchanged. EditMode passed 155/155.

## Final cooldown-8 confirmation

Six arms / 60 simulations using seeds `10600-10609`, Forest Edge, natural opportunities: opposed control, block +4, block +6, cooldown 8, block +4/cooldown 8, and block +6/cooldown 8.

Combined cooldown 8 reduced Fox pressure versus block-only:

- Block +4: Fox attempts -2.10/run and kills -1.80/run.
- Block +6: Fox attempts -4.80/run and kills -1.40/run.

However, it did not improve ecology in this confirmation sample:

- Block +4/cooldown 8 versus block +4: mean Hare -1.26, AUC -252, danger-time +6.40 ticks.
- Block +6/cooldown 8 versus block +6: mean Hare -3.05, AUC -612, danger-time +6.20 ticks.

## Decision

Do not promote block +4/+6 with cooldown 8 as a balance change. The interaction reliably changes proximal combat pressure, but the population result remains seed- and ecology-sensitive. Keep the stat split and telemetry in the Bev experimental branch, and use direct combat-lab tests for upgrade semantics before running more ecological sweeps.

## Artifacts

- Corrected lab reports: `artifacts/cellular-experiment-20260821-203237` through `203431`.
- Final confirmation reports: `artifacts/cellular-experiment-20260821-203704` through `203959`.
- Test results: `artifacts/unity-tests-20260821-203611/EditMode-results.xml`.
