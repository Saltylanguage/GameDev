# Predator Slash-Line Measurement Contract

Status: implemented alongside the herbivore slash line for the
`bev-experimental` feature bundle. This is a separate role-specific line; do
not pool its AHS values with herbivore APS without an explicit normalization
decision.

## Formula contract

| Statistic | Formula | Raw inputs | Undefined/invalid handling |
| --- | --- | --- | --- |
| FPO | `SPO + BIR - STRV - CRWD` | `SPO`, `BIR`, `STRV`, `CRWD` | Compare with observed final population; mismatch is an FPO reconciliation failure. |
| hAVG | `KIL / HAT` | `KIL`, `HAT` | `HAT=0, KIL=0` is N/A. Positive KIL with no attempts, negative counts, or KIL>HAT is INVALID. |
| aAVG | `EPS / PPS` | `EPS`, `PPS` | `PPS=0, EPS=0` is N/A. Positive EPS with no prey-active predator steps, negative counts, or EPS>PPS is INVALID. |
| huntAVG | Average of applicable hAVG and aAVG | hAVG, aAVG | One valid component stands alone. Both N/A is N/A. Any INVALID component makes huntAVG INVALID. |
| sAVI | `1 - STRV / (SPO + BIR)` | `STRV`, `SPO`, `BIR` | Zero exposure with zero STRV is N/A; positive STRV, negative exposure, or STRV>denominator is INVALID. |
| cAVI | `1 - CRWD / (SPO + BIR - STRV)` | `CRWD`, `SPO`, `BIR`, `STRV` | Zero exposure with zero CRWD is N/A; positive CRWD, negative exposure, or CRWD>denominator is INVALID. |
| bAVG | `BIR / MAT` | `BIR`, `MAT` | `MAT=0, BIR=0` is N/A. Positive BIR with no mating opportunity or BIR>MAT is INVALID. |
| RFS | `(FPO - SPO) * bAVG` | `FPO`, `SPO`, `bAVG` | Valid zero bAVG remains a valid zero multiplier; N/A and INVALID propagate. |
| AHS | `RFS + huntAVG - (1-sAVI) - (1-cAVI)` | RFS, huntAVG, sAVI, cAVI | N/A contributions are neutral. Any INVALID component or FPO reconciliation failure makes AHS INVALID. |

## Raw-count origins

| Raw value | Current source |
| --- | --- |
| SPO/FPO | First and final `PopulationHistory` snapshots for the predator species. |
| PPS | At the start of each step with at least one living member of the predator's diet target, count every living predator of that species once. |
| EPS | On the first eligible encounter in a step, add that step's full living predator population for the species. EPS is species-wide and cannot exceed PPS. |
| ECN | `RecordPredatorEncounter`, after pre-contact avoidance and when an eligible predator-prey combat opportunity is recorded. |
| HAT/KIL | Predator `CombatAttempts` and `CombatKills` activity counters from opposed-roll combat. A block is still an attempt; a kill requires lethal resolution. |
| STRV/CRWD | `SpeciesDeathEvent` records with `Starvation` or `Crowding` cause. |
| MAT/BIR | Predator reproduction candidates and successfully placed offspring. |

The domain implementation is `SpeciesPredatorStatLine` and
`CreatePredatorStatLine` in
`Assets/Scripts/Game/Simulation/SpeciesSimulationMetrics.cs`.

## Game display and export paths

- Rewards/results UI displays the predator line through the existing species
  stat-line panel when the player species is a carnivore.
- Batch export serializes the root `predatorStatLine` and phase-window
  `predatorStatLine` records.
- Play Mode result logging uses the same role-specific phase export path.

The UI and export share the domain calculation path. Fixed formula fixtures
cover valid rates, kill-count population treatment, and impossible hunt-rate
invalidation. Controlled predator balance sweeps remain separate evidence from
formula correctness.
