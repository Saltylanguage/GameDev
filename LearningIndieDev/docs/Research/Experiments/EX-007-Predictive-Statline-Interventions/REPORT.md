# EX-007 — What happened when we changed the Hare

**Status:** Runs complete; human review pending
**Prediction ID:** `PRED-EXP-007-0001`

## The short version

We ran the same 20-second ForestEdge simulation with the same starting seeds,
then repeated it on five new seeds. The only thing that changed was the Hare
upgrade:

- **B (baseline):** no upgrade.
- **S1:** Hares move a little faster.
- **J1:** Hares move a little faster and tolerate more crowding.

The faster Hare setting produced about **5 more Hares at the end of a run** on
both seed panels. The combined setting produced about **5 more Hares on the
training seeds and 15 more on the held-out seeds**. That tells us the settings
can change the result, but not yet how large or dependable the change is.

Every run produced a complete, validated report. The per-seed comparisons are
in `artifacts/EX-007-20260903-1535/paired-deltas.csv`.

## Adapter-backed rerun audit

To verify that the research path consumes the same snapshot contract as the
game, S1 and J1 were rerun with the EX-007 research fixture catalog through
`SpeciesUpgradePredictionInputAdapter`. The new schema-23 bundles are:

| Arm | Seeds | Artifact | Bundle | Statline |
|---|---:|---|---|---|
| S1 | 1–20 | `artifacts/cellular-experiment-20260904-191654` | Valid | Validated with limitations |
| J1 | 1–20 | `artifacts/cellular-experiment-20260904-191915` | Valid | Validated with limitations |
| S1 | 101–105 | `artifacts/cellular-experiment-20260904-192118` | Valid | Validated with limitations |
| J1 | 101–105 | `artifacts/cellular-experiment-20260904-192239` | Valid | Validated with limitations |

The adapter-backed reports record the fixture catalog path and complete
snapshot provenance. Their core run payloads match the historical arms after
accounting for the intentionally different loadout metadata. The historical
reports are schema 21 and the reruns are schema 23; derived StatLine fields
were recalculated by the current telemetry code, so this audit does not claim
full byte-for-byte StatLine parity.

## What the words mean

- **Final Hares (FPO):** how many Hares were alive at the end.
- **Births (BIR):** how many new Hares were born during the run.
- **Starvation deaths (STRV):** Hares that died from starvation.
- **Crowding deaths (CRWD):** Hares removed because the area was too crowded.
- **Predation deaths (PREY):** Hares killed by carnivores (foxes) during the
  run. This is not a food-gathering measure.
- **Predator-encounter survival (pAVI):** the estimated share of recorded
  predator encounters that did not end with the Hare being killed.
- **Resource-finding score (RFS):** a normalized measure of resource access.
- **Activity per step (APS):** how much recorded activity occurred per tick.

## The result in plain language

### Final Hare population

| Seed panel | No upgrade | Faster movement | Faster movement + crowding tolerance |
|---|---:|---:|---:|
| Training (20 seeds) | 70.3 | 76.1 (**+5.8**) | 75.6 (**+5.3**) |
| Held-out (5 new seeds) | 71.4 | 76.0 (**+4.6**) | 86.8 (**+15.4**) |

The number in parentheses is the average change from the matching baseline
seed. Individual seeds varied widely, so the averages should not be treated as
guaranteed outcomes.

### Other changes worth noticing

- **Faster movement:** births increased on average, but starvation deaths also
  increased. More movement did not simply make Hares healthier.
- **Crowding tolerance:** the combined setting reduced crowding deaths to zero
  on average in both panels.
- **Predation:** faster movement produced slightly more Hare deaths from foxes
  on training seeds (+0.85 on average) but fewer on held-out seeds (-2.80).
  The per-encounter survival score rose on average in both panels, so the
  reversal appears to be about how often Hares encountered foxes and when,
  not a simple increase in vulnerability.

## What we can and cannot conclude

We can say that these supported upgrades changed Hare outcomes under the exact
ForestEdge configuration, and that the main population direction repeated on
new seeds.

We cannot yet say that:

- the combined upgrade is truly more powerful than the single upgrade;
- the effect will hold in another scenario or over a longer simulation;
- the effect scales smoothly with different upgrade values; or
- either setting is better for game balance or fun.

The combined arm has no matching “crowding tolerance only” arm, so its
`J1-S1` comparison is an incremental contrast, not a complete two-variable
interaction test. That contrast was **-0.45 Hares on training seeds and +10.80
on held-out seeds**, which is too inconsistent to call a stable interaction.

## Audit details

| Arm | Seeds | Artifact | Bundle | Statline |
|---|---|---|---|---|
| B | 1–20 | `artifacts/cellular-experiment-20260903-152434` | Valid | Validated with limitations |
| S1 | 1–20 | `artifacts/cellular-experiment-20260903-152750` | Valid | Validated with limitations |
| J1 | 1–20 | `artifacts/cellular-experiment-20260903-152854` | Valid | Validated with limitations |
| B | 101–105 | `artifacts/cellular-experiment-20260903-153000` | Valid | Validated with limitations |
| S1 | 101–105 | `artifacts/cellular-experiment-20260903-153055` | Valid | Validated with limitations |
| J1 | 101–105 | `artifacts/cellular-experiment-20260903-153149` | Valid | Validated with limitations |

The statline validator's limitations concern accumulated counters such as
total movement/resource opportunities; they do not invalidate the paired FPO
comparison above.

## Follow-up: crowding tolerance first

We then ran a follow-up with the order requested for the next test:

- **B:** no upgrade.
- **C1:** crowding tolerance only.
- **CJ1-reversed:** crowding tolerance, then faster movement.

The training panel reused seeds **1–20** so it could be compared with the
existing baseline. The new held-out panel used **seeds 106–110**. This gives us
five additional held-out seeds, but it is not a clean order-only test against
the earlier forward-order J1 arm because that earlier held-out arm used seeds
101–105.

### Follow-up results

| Seed panel | No upgrade FPO | Crowding tolerance FPO | Crowding then faster FPO |
|---|---:|---:|---:|
| Training (20 seeds) | 70.3 | 76.4 (**+6.1**) | 75.6 (**+5.3**) |
| New held-out (5 seeds) | 77.6 | 80.2 (**+2.6**) | 78.8 (**+1.2**) |

The numbers in parentheses are average changes from the matching baseline seed.
On the new held-out seeds, crowding tolerance alone increased births by **8.2**
and reduced fox kills of Hares by **1.8** on average. Adding faster movement on
top of it increased births by **12.0** and reduced fox kills by **3.4**, while
ending with only **1.2** additional Hares on average. Crowding deaths were zero
for both upgrade arms on this small panel.

The training results are especially useful for checking implementation: the
reversed sequence produced the same aggregate values as the earlier
faster-then-crowding joint arm on seeds 1–20. That is consistent with these two
catalog upgrades being applied as independent rule changes, but it is not proof
that upgrade order can never matter elsewhere.

The new follow-up paired file is
`artifacts/EX-008-20260903/paired-deltas.csv`.

### Follow-up audit details

| Arm | Seeds | Artifact | Bundle | Statline |
|---|---|---|---|---|
| C1 | 1–20 | `artifacts/cellular-experiment-20260903-160513` | Valid | Validated with limitations |
| CJ1-reversed | 1–20 | `artifacts/cellular-experiment-20260903-160610` | Valid | Validated with limitations |
| B | 106–110 | `artifacts/cellular-experiment-20260903-160706` | Valid | Validated with limitations |
| C1 | 106–110 | `artifacts/cellular-experiment-20260903-160743` | Valid | Validated with limitations |
| CJ1-reversed | 106–110 | `artifacts/cellular-experiment-20260903-160827` | Valid | Validated with limitations |
