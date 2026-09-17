# EX-011 proposal — Cross-biome, cross-species transfer

**Status:** Approved by Josh on 2026-09-12; preregistered; no Open Range results inspected before approval
**Decision owner:** Josh
**Proposed capability:** Test when an accepted Forest Edge/Hare finding can be
reused, and when the assistant must abstain.

The historical filename is retained so the proposal link remains stable. This
is now the approved execution contract. No experiment result was inspected
before Josh approved the locked choices below.

## Plain-language question

EX-007 found that adding both movement speed and crowding tolerance increased
the final Hare population in Forest Edge. Does the *direction* of that result
also appear when the same rule changes are applied to Deer in Open Range?

This is a small transfer test, not evidence that the finding works for every
species, biome, seed range, or upgrade value.

## What ability this unlocks

A completed test gives the workflow its first bounded rule for reusing an old
finding in a materially different setting. A failed or mixed result is equally
useful: it teaches the assistant to request new evidence instead of carrying a
Forest Edge/Hare conclusion forward.

## Locked contract

| Field | Locked value |
| --- | --- |
| Scenario | `Assets/Data/CellularSimulation/Scenarios/OpenRange.asset` |
| Scored species | `deer` |
| Control arm | No upgrade |
| Intervention arm | `faster-movement` → `crowding-tolerance`: movement speed `+0.5`, then crowding tolerance `+1`, both active at launch |
| Upgrade source | New EX-011 research fixtures, identical in value to EX-007 but targeted to Deer |
| Combat | `opposed-roll` |
| Attack opportunity | `natural` |
| Experimental export | `bev-experimental` |
| Development seeds | `201–220` |
| Held-out seeds | `301–305`; never inspect before the prediction and development interpretation are sealed |
| Run window | Scenario default: `20.0s` at `0.1s` per step |
| Metric dictionary | `cellsim-experiment-metrics` version `1` |
| Raw evidence retention | Keep every arm's complete artifact directory through the human decision |

## Pre-registered prediction

Based only on the accepted EX-007 direction, the intervention is predicted to
increase mean Deer final population relative to the same-seed control on both
the development and held-out panels.

The prediction is deliberately directional. The prior Hare effect size is not
transferred because Open Range has a different grid, species mix, starting
conditions, and ruleset fingerprint.

## Endpoints and decision rule

The primary endpoint is the paired per-seed difference in
`herbivore-statline.fpo` (intervention minus control).

The transfer direction is supported only when all of these are true:

1. mean FPO difference is greater than zero on development seeds;
2. mean FPO difference is greater than zero on held-out seeds;
3. at least 12 of 20 development pairs and 3 of 5 held-out pairs are positive;
4. every bundle and Stat-Line passes its validator; and
5. no report or contract identity is mixed across arms.

A zero, negative, invalid, or split held-out result is **not transferred**. A
positive mean that fails the per-seed rule is **unresolved** rather than
promoted.

Secondary metrics are descriptive only:

- `herbivore-statline.aps`
- `herbivore-statline.rfs`
- `herbivore-statline.bir`
- `herbivore-statline.crwd`
- `herbivore-statline.predavg`

They may explain a result but cannot rescue a failed primary endpoint.

## Evidence sequence

1. Human approves or edits this contract. **Complete: approved unchanged by
   Josh on 2026-09-12.**
2. Create the Deer-targeted research fixtures and a seed-role registry.
3. Seal the AI input files and their hashes before any Open Range result is
   shown to the analyst.
4. Run and validate the development control and intervention bundles.
5. Write a development-only AI interpretation and held-out prediction.
6. Run and validate the held-out bundles.
7. Produce a factual report, then a separate AI scoring analysis.
8. Stop for Josh's human accept, reject, or revise decision.

## Known limits accepted by approval

- This is one scenario, one new herbivore species, one two-variable
  intervention, and 25 paired seeds.
- The test can establish directional transfer only inside this contract.
- The two rule changes remain bundled, so the test does not identify which one
  caused any transferred effect.
- A simulation outcome does not establish balance, fun, or player value.
- Human approval authorizes execution of this research contract, not promotion
  of its result into production.

## Approval gate

- [x] Josh approves the question, arms, values, order, seed roles, endpoint, thresholds,
      evidence budget, retention rule, and limits.
- [x] The approved contract receives an immutable SHA-256 sidecar before runs.

**Human approval:** Approved unchanged by Josh on 2026-09-12.
