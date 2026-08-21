# EX-002 - Herbivore collapse attribution

**Experiment ID:** `EXP-002`  
**Status:** Schema-6 BaselineParity control validated deterministically; the full EX-002 matrix remains open
**Decision owner:** Human design owner  
**Reference scenario:** `Assets/Data/CellularSimulation/Scenarios/BaselineParity.asset`

EX-002 investigates how a simulation-specific growth rule can identify and test
a cell type's collapse state. The accepted ForestEdge result remains the
instrument-trust baseline, but ForestEdge's player species is `hare`; it is not
the correct reference for an herbivore-specific attribution claim. BaselineParity
is the first scenario adapter because it defines `herbivore` and produced one
extinction in its pre-telemetry-extension 20-seed EX-001B pair.

## Package contents

- [Experiment brief](EXP-002-brief.md)

The paired schema-6 BaselineParity control is now recorded at
`artifacts/cellular-experiment-20260818-210354/report.json` and
`artifacts/cellular-experiment-20260818-210443/report.json`. The factual
causal report and separate analysis must still wait for the declared
intervention matrix and held-out check.

## Current interpretation boundary

The existing evidence supports a proximate starvation signal in BaselineParity,
not a complete causal explanation or a universal collapse detector. The prior
schema-4 report contains aggregate death counts only; it cannot be retroactively
converted into per-entity death events. Schema 5 introduced the required death
event stream; schema 6 retains it and adds the reproduction funnel. The paired
schema-6 control verifies the event stream before intervention analysis.
Nothing currently establishes that starvation is the root cause, that the rules
are unbalanced, or that the result transfers to other scenarios.
