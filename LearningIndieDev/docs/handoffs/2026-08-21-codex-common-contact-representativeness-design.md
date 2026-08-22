# Common-contact representativeness audit design

## Scope

This diagnostic audits the sampling properties of the paired-lockstep
intersection. It does not retest Block+2 mechanics and does not change combat,
movement, resources, reproduction, starvation, carrying capacity, or starting
populations.

## Reference population and strata

The reference population is every naturally valid Fox-to-Hare candidate contact
in the same paired diagnostic seed/configuration population. Each row is
assigned exactly one stratum:

- `COMMON`: valid in both arms.
- `BASELINE_ONLY`: valid only in `none`.
- `BLOCK_ONLY`: valid only in `stronger-block-2`.
- `UNION`: the disjoint union of the three strata.

The row identity is `tick + stable coordinate/species/contact identity +
occurrence`. Entity IDs are recorded as state telemetry only and never define
cross-arm identity.

## Snapshot and dimensions

State is captured from each arm's source grid before behavior, opposed rolls,
damage, or deaths for that tick. The row records:

- tick and normalized time quintile;
- global Hare, Fox, and plant-resource counts;
- local 3x3 Hare/Fox/resource densities and attacker terrain energy;
- available Fox/Hare health, age, energy, food reserve, alpha flag, behavior
  state, terrain, and entity IDs.

The model has no explicit hunger, reproduction-state, region, or cooldown field
at this boundary; those are not fabricated. First/repeat contact is derived
from repeated pre-contact attacker/target entity IDs within a seed. This is
opportunity history, not a claim that every prior opportunity resolved as an
attack.

## Comparisons and practical threshold

Report separately for calibration (`10100-10119`) and held-out development
seeds (`10125-10144`):

- `COMMON` vs `UNION` for each arm;
- `COMMON` vs `BASELINE_ONLY`;
- `COMMON` vs `BLOCK_ONLY`;
- `BASELINE_ONLY` vs `BLOCK_ONLY`.

Continuous state uses standardized mean differences (SMD). Absolute SMD
`<0.10` is negligible, `0.10-0.25` is small/potentially meaningful, and
`>=0.25` is flagged material for investigation. Time coverage is reported per
quintile as `COMMON / UNION`; no weighting or matching is applied.

## Integrity gates

- Per-seed row counts reconcile to the paired report candidate counts.
- `COMMON + BASELINE_ONLY + BLOCK_ONLY = UNION` for every seed.
- The baseline and Block+2 selected reports contain identical audit rows.
- Telemetry is diagnostic-only and must not change RNG, opportunity validity, or
  combat outcomes.
