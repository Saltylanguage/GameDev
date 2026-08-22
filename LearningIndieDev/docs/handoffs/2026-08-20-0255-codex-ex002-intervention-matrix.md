# EXP-002 intervention matrix and held-out check

[Working state](../WORKING_STATE.md) | Protocol: [EX-002 matrix protocol](../Research/Experiments/EX-002-Herbivore-Collapse-Attribution/EX-002-MATRIX-PROTOCOL.md) | Status: complete bounded matrix

## Execution

The protocol was committed as `eca83d7` before this evidence pass. All six
reports are schema 6, use the same 32 x 20 BaselineParity dimensions, 20.0 s
duration, 0.1 s step, and `herbivore` as the player species. The two variant
arms have distinct ruleset fingerprints from the control and from each other.

| Arm | Seeds | Report |
|---|---:|---|
| Control | 10100–10119 | `artifacts/cellular-experiment-20260820-025022/report.json` |
| Herbivore energy relief | 10100–10119 | `artifacts/cellular-experiment-20260820-025033/report.json` |
| Predation relief | 10100–10119 | `artifacts/cellular-experiment-20260820-025045/report.json` |
| Control (held out) | 10120–10124 | `artifacts/cellular-experiment-20260820-025056/report.json` |
| Herbivore energy relief (held out) | 10120–10124 | `artifacts/cellular-experiment-20260820-025105/report.json` |
| Predation relief (held out) | 10120–10124 | `artifacts/cellular-experiment-20260820-025115/report.json` |

## Primary endpoint

The operational endpoint is final herbivore population at 20 seconds; a run is
flagged as collapsed when that value is zero. The matrix also records final
extinction rate, births, deaths by proximate cause, food consumed, combat kills,
and reproduction reconciliation.

### Matched matrix

| Arm | Ruleset fingerprint | Final herbivore avg (min–max) | Collapsed | Births | Starvation deaths | Combat kills |
|---|---|---:|---:|---:|---:|---:|
| Control | `90bd5760…464616` | 7.6 (0–28) | 2/20 (10%) | 142 | 246 | 0 |
| Energy relief | `1b7bfae0…f4deb` | 21.0 (0–48) | 1/20 (5%) | 458 | 276 | 0 |
| Predation relief | `8cd1ac65…1d5d4` | 8.35 (0–28) | 2/20 (10%) | 157 | 247 | 0 |

### Held-out seeds

| Arm | Final herbivore avg (min–max) | Collapsed | Births | Starvation deaths | Combat kills |
|---|---:|---:|---:|---:|---:|
| Control | 3.6 (0–10) | 1/5 (20%) | 10 | 64 | 0 |
| Energy relief | 23.0 (10–51) | 0/5 (0%) | 100 | 52 | 0 |
| Predation relief | 3.6 (0–10) | 1/5 (20%) | 10 | 64 | 0 |

Every run's per-species death events reconcile with its activity totals, and
every run reports a reconciled reproduction funnel. All carnivores go extinct
in both the 20-seed and held-out ranges; combat kills are zero in every arm.
The predation arm is therefore a neutral falsification in this observation
window, not evidence that predation is a driver.

## Bounded interpretation

The named herbivore starting-energy intervention (`6 → 12`) increases final
herbivore population and reduces the zero-population endpoint in both the
matched and held-out ranges. This supports a bounded intervention effect under
the BaselineParity ruleset and 20-second window. It does **not** prove that
starvation is the sole root cause, that the game is balanced, or that the
result transfers to other scenarios. Energy trajectories, resource-access
history, and attacker identity remain uninstrumented.

The predation-relief arm changes one declared rule but produces the same
herbivore endpoint as control because no combat kills occur and carnivores
starve out. No predation causal claim is promoted.
