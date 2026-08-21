# Common-contact representativeness audit result

## Verdict

**MATERIALLY_BIASED**

## Censoring classification

**R6 - multiple interacting biases**

The common subset is materially early/divergence-biased and has a different
encounter-history and organism-state mix from excluded contacts. The L1 result
remains causally valid inside the common-contact sample, but its effect size
must not be generalized to natural encounter pressure.

## Coverage and reconciliation

| Group | Common | Baseline-only | Block-only | Union | Common / union |
| --- | ---: | ---: | ---: | ---: | ---: |
| Calibration | 91 | 29 | 66 | 186 | 48.925% |
| Held-out | 120 | 57 | 47 | 224 | 53.571% |

Every seed passed `COMMON + BASELINE_ONLY + BLOCK_ONLY = UNION`, and every
stratum count reconciled to the paired report's baseline, Block+2, common, and
union candidate counts. The selected `none` and `stronger-block-2` reports had
zero audit-row mismatches in both groups.

## Time representativeness

| Group | Q1 | Q2 | Q3 | Q4 | Q5 |
| --- | ---: | ---: | ---: | ---: | ---: |
| Calibration common / union | 93.333% | 45.161% | 64.444% | 34.091% | 25.000% |
| Held-out common / union | 100.000% | 78.182% | 47.500% | 50.000% | 13.846% |

Coverage collapses in the final quintile, especially held-out. This is strong
evidence of divergence/time censoring rather than a roughly random half-sample.

## Population-state representativeness

Material `COMMON` vs `UNION` differences (absolute SMD >= 0.25) include:

- Calibration baseline: Fox population `3.813 vs 3.558` (SMD `0.288`), with
  common contacts at younger Fox/Hare ages and lower Hare food reserve
  (`attacker age -0.259`, `target age -0.316`, `target reserve -0.310`).
- Calibration Block+2: Hare population `16.736 vs 15.057` (SMD `0.295`).
- Held-out baseline: Fox population `3.817 vs 3.520` (SMD `0.357`), plant
  population `749.067 vs 790.305` (SMD `-0.269`), attacker age `-0.539`,
  attacker food reserve `-0.446`, target age `-0.300`, and target reserve
  `-0.316`.
- Held-out Block+2 had no `COMMON` vs `UNION` state SMD at or above `0.25`,
  but its common sample is still time- and encounter-structure-biased.

Local density differences were generally smaller in `COMMON` vs `UNION`, with
terrain/resource energy becoming material in some arm-specific comparisons.
Region/biome identity and explicit hunger/reproduction state were not available
without inventing new semantics.

## Fox/Hare state and encounter structure

Excluded contacts are not merely different locations; they are older, more
repeated encounter regimes. Repeat-contact shares were:

| Group | Common | Baseline-only | Block-only |
| --- | ---: | ---: | ---: |
| Calibration | 18.681% / 18.681% | 68.966% | 69.697% |
| Held-out | 20.833% / 21.667% | 70.175% | 76.596% |

The first value in each common cell is baseline; the second is Block+2. Common
contacts therefore underrepresent repeated pressure by roughly 50 percentage
points. Calibration arm-specific exclusions also differed strongly: baseline-
only vs Block-only attacker age SMD `0.928`, target age `1.647`, target reserve
`1.425`, Hare population `1.124`, and Fox population `-0.975`. Held-out showed
attacker age `1.324`, attacker food reserve `0.927`, and Fox population `-0.546`.

## Practical consequence for L1

The paired-lockstep experiment still establishes:

```text
same realized attempts -> lower Block+2 hit rate -> fewer successful hits
-> fewer Fox-caused Hare deaths
```

That causal combat statement is sound. This audit shows the common sample is
mostly early/pre-divergence and disproportionately first-contact, while the
excluded strata contain older organisms, lower reserves, different population
states, and much more repeat pressure. Those are combat/ecology-relevant
conditions, so the isolated hit-rate effect should not be treated as a natural-
world encounter-pressure estimate.

## Recommended next experiment

Run one **stratified natural-world encounter validation** that reports the
combat effect separately by time quintile, first/repeat contact, and the
material Fox/Hare state bands identified here. Do not tune Block+2 or ecology in
that experiment; its purpose is to estimate natural encounter pressure without
using the censored intersection as the sole sample.

## Tests and accounting

- Focused EditMode: **148/148 passed** -
  `artifacts/unity-tests-20260821-031254/EditMode-results.xml`.
- Full suite: EditMode **148/148 passed**; PlayMode **4/6 passed** in
  `artifacts/unity-tests-20260821-031337/`.
- The two PlayMode failures are the same pre-existing Noesis
  `TextureSource` native-pointer exceptions at
  `Assets/UI/HUD/Scripts/SpeciesSimulationViewModel.cs:464`; no new failures
  were introduced.
- Food, reproduction, combat, and paired-opportunity reconciliation remained
  clean in all four accepted reports. Contact-stratum reconciliation failures:
  `0` for every seed.

## Evidence

- Design: `docs/handoffs/2026-08-21-codex-common-contact-representativeness-design.md`
- Manifest: `docs/handoffs/2026-08-21-codex-common-contact-representativeness-manifest.md`
- Dataset: `artifacts/cellular-opportunity-representativeness-20260821-031846/encounter-dataset.json`
- Analysis: `artifacts/cellular-opportunity-representativeness-20260821-031846/representativeness-analysis.md`
- Calibration baseline/trial: `artifacts/cellular-experiment-20260821-031616/` and `...-031717/`
- Held-out baseline/trial: `artifacts/cellular-experiment-20260821-031803/` and `...-031846/`

## Repository and Trello

- Branch began at `6a38264`; telemetry, analysis, and handoff changes are
  committed in `051c247`.
- `origin/BevBranch` remains `ab5fc89`; no push or history rewrite performed.
- Card 59 remains In Progress and Card 29 remains Backlog & Ideas. The final
  representativeness comment is a follow-up to the already-posted paired result,
  not a duplicate of the old NOT ISOLATED summary.

## Remaining uncertainty

This audit does not map every candidate row to a resolved combat roll because
existing combat-roll telemetry lacks coordinate/event identity. It therefore
classifies selection bias and encounter structure, not a stratum-specific
natural-world mortality effect. The L1 common-sample causality is preserved;
natural-world effect-size generalization remains unresolved until the proposed
stratified validation.
