# Paired lockstep Fox attack-opportunity isolation design

## Decision

Use a diagnostic-only **paired lockstep** mode named
`PairedLockstepDiagnostic`. It is not the production attack path. At each
scheduled tick, the runner advances the `none` and `stronger-block-2` worlds
with the same seed and tick, enumerates their current Fox-to-Hare candidate
contacts, intersects those contacts by stable species/coordinate/contact
identity, and selects one deterministic common contact. The selected contact
is validated after behavior has advanced in both worlds and is then executed
in both arms.

## Pairing unit and controls

- Pairing unit: `scheduled tick + one deterministic common contact identity`.
- `baselineValid` and `blockPlusTwoValid` describe the selected scheduled slot,
  not every candidate in the world.
- `commonValid` is the shared scheduled slot; it must equal `pairedAttempts`.
- `baselineOnly` and `blockPlusTwoOnly` are excluded from the causal sample.
- Candidate counts (`baselineCandidateCount`, `blockPlusTwoCandidateCount`,
  `commonCandidateCount`, `unionCandidateCount`) remain separate so
  intersection censoring is visible rather than hidden in the exact gate.
- A paired mismatch or post-behavior invalidation fails the exposure gate.

This fixes the prior fixed-rate failure: identical schedules did not prevent
arm-local contact eligibility from diverging after the first different combat
outcome. The paired runner removes that exposure confound without changing
damage, metabolism, reproduction, resources, capacity, populations, or the
production natural-mode resolver.

## Frozen run

- Scenario: `Assets/Data/CellularSimulation/Scenarios/ForestEdge.asset`
- Grid: `32 x 32`; duration `20.0 s`; step `0.1 s`
- Combat: opposed-roll
- Arms: `none` vs `stronger-block-2`
- Calibration: seeds `10100–10119`
- Held-out: seeds `10125–10144`
- Required sample: 40 paired seeds / 80 selected-arm reports

## Acceptance gates

1. Exact scheduled equality in both arms.
2. `commonValid == pairedAttempts` for every seed in both arms.
3. Zero paired opportunity mismatches and zero invalidated common slots.
4. Matching stable paired-opportunity IDs per seed.
5. Food, combat, reproduction, and opportunity accounting reconciles.
6. The result is interpreted at the combat layer separately from whole-world
   ecological outcomes.

## Known limitation

The intersection intentionally censors one-arm-only candidate contacts. The
paired result is therefore an isolated realized-exposure estimate, not a
claim that natural-world encounter generation is unchanged. Candidate
common/union coverage is reported as the representativeness bound.
