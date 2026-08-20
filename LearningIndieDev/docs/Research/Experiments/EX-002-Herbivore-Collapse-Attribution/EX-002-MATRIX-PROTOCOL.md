# EXP-002 intervention matrix protocol

**Protocol date:** 2026-08-20  
**Reference scenario:** `Assets/Data/CellularSimulation/Scenarios/BaselineParity.asset`  
**Player/cell type:** `herbivore`  
**Simulation build:** current `BevBranch` checkout

## Endpoint and scope

For this bounded adapter, the primary outcome is the **final herbivore
population at the end of the 20-second run**. A run is flagged as the
operational collapse endpoint when that final population is zero. Secondary
outcomes are final-population extinction rate, births, deaths by proximate
cause, food consumed, combat kills, and reproduction reconciliation.

This is a BaselineParity run-level endpoint. It is not a generic definition of
collapse for other simulation domains, and it does not by itself establish
desirability or balance.

## Arms and approved changes

All arms retain the same scenario dimensions, seed, duration, step interval,
player species, and engine. Each intervention changes one serialized rule in a
versioned asset; the shared BaselineParity assets are not edited.

| Arm | Scenario asset | Single changed rule | Baseline → intervention |
|---|---|---|---:|
| Control | `Scenarios/BaselineParity.asset` | none | — |
| Herbivore energy relief | `Scenarios/EX002/EX002_HerbivoreEnergyRelief.asset` | herbivore `startingEnergy` | `6 → 12` |
| Predation relief | `Scenarios/EX002/EX002_PredationRelief.asset` | carnivore `attackAmount` | `2 → 0` |

## Seed plan and acceptance

- Matched matrix: seeds `10100`–`10119` (20 runs per arm).
- Held-out check: seeds `10120`–`10124` (5 runs per arm).
- Reports must be schema 6, carry distinct ruleset fingerprints for the two
  intervention arms, and retain complete run payloads and death telemetry.
- A directional energy-relief result is supported only if the matched matrix
  and held-out check both move final herbivore population upward and do not
  increase the collapse rate.
- The predation arm is reported independently; a neutral result is valid and
  does not authorize a stronger causal claim.

Execution evidence is recorded in the dated handoff that follows this
protocol. The causal interpretation remains bounded by the missing energy
trajectories, resource-access history, and attacker identity noted in the
experiment brief.
